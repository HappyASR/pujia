'==========================================================================
'
'  File:        FormMain.vb
'  Location:    Firefly.TextLocalizer <Visual Basic .Net>
'  Description: 文本本地化工具主窗体
'  Version:     2010.01.14.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Text
Imports System.ComponentModel
Imports System.Reflection
Imports Firefly
Imports Firefly.TextEncoding
Imports Firefly.Glyphing
Imports Firefly.Setting
Imports Firefly.Project

Public Class FormMain
    Implements ITextLocalizerApplicationController

    Public CurrentProject As LocalizationProject
    Public CurrentProjectFilePath As String

    Public Columns As New List(Of LocalizationTextProvider)
    Public LocalizationTextBoxes As New List(Of LocalizationTextBox)
    Public TextNames As New List(Of String)
    Public TextNameDict As New Dictionary(Of String, Integer)

    Public Plugins As New List(Of ITextLocalizerPlugin)
    Public TextHighlighters As New List(Of ITextLocalizerTextHighlighter)
    Public GridTextFormatters As New List(Of ITextLocalizerGridTextFormatter)
    Public ControlPlugins As New List(Of ITextLocalizerControlPlugin)
    Public TranslatorPlugins As New List(Of ITextLocalizerTranslatorPlugin)

#Region " 设置 "
    Private Sub TextLocalizer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DataGridView_Multiview.SuspendDraw()

#If CONFIG <> "Debug" Then
        Try
#End If
        Dim Args = System.Environment.GetCommandLineArgs()
        Select Case Args.Length
            Case 1
                Dim Files As String() = Directory.GetFiles(System.Environment.CurrentDirectory, "*.locproj")
                If Files.Length = 0 Then Throw New FileNotFoundException("NoLocprojFileFound")
                CurrentProjectFilePath = Files(0)
            Case 2
                CurrentProjectFilePath = Args(1)
            Case Else
                Throw New ArgumentException("NoMatchArgumentsFound")
        End Select
        System.Environment.CurrentDirectory = GetFileDirectory(CurrentProjectFilePath)

        Application.AddMessageFilter(New Intercepter)

        CurrentProject = Xml.ReadFile(Of LocalizationProject)(CurrentProjectFilePath)
        LoadOpt()
        Me.CenterToScreen()

        TextNames = LocalizationTextBoxes(CurrentProject.MainLocalizationTextBox).TextProvider.Keys.ToList
        TextNames.Sort(StringComparer.CurrentCultureIgnoreCase)
        TextNameDict = TextNames.Select(Function(s, i) New With {.Index = i, .Name = s}).ToDictionary(Function(p) p.Name, Function(p) p.Index)
        For Each t In TextNames
            ComboBox_TextName.Items.Add(t)
        Next

        MeWindowState = Me.WindowState
        MeWidth = Me.Width
        MeHeight = Me.Height
        MeNormalWidth = CurrentProject.WindowWidth
        MeNormalHeight = CurrentProject.WindowHeight
        MainPanelHeight = Panel_LocalizationBoxes.Height
        LocalizationTextBoxHeights = GetLocalizationTextBoxHeights()

        If CurrentProject.Maximized Then
            Me.WindowState = FormWindowState.Maximized
            Panel_LocalizationBoxes_Resize(Nothing, Nothing)
        End If

        LocalizerEnable = False

        OpenDefaultFile()
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex)
            End
        End Try
#End If
    End Sub
    Public Sub FlushLocalizedText()
        For Each L In LocalizationTextBoxes
            If L.TextModified Then L.SaveText()
        Next
    End Sub
    Private Sub TextLocalizer_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        FlushLocalizedText()
        System.Environment.CurrentDirectory = GetFileDirectory(CurrentProjectFilePath)

#If CONFIG <> "Debug" Then
        Try
#End If
        SaveOpt()
        Xml.WriteFile(CurrentProjectFilePath, UTF16, CurrentProject)
#If CONFIG <> "Debug" Then
        Catch ex As Exception
            ExceptionHandler.PopupException(ex)
            End
        End Try
#End If
    End Sub
    Private Sub TextLocalizer_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            MoveToDefaultTextNumber()
            VScrollBar_Bar.Select()
        Finally
            DataGridView_Multiview.ResumeDraw()
            DataGridView_Multiview.Invalidate()
        End Try
    End Sub

    Private Shared BlockRedirectBinding As Boolean = False
    Private Shared Function RedirectBinding(ByVal sender As Object, ByVal e As ResolveEventArgs) As Assembly
        If BlockRedirectBinding Then Return Nothing
        BlockRedirectBinding = True
        Try
            Dim AsmName As New AssemblyName(e.Name)
            AsmName.Version = Nothing
            Dim SimpleName = AsmName.Name
            '不要使用Assembly.Load以外的其他LoadFrom、LoadFile等方法，否则会因为动态生成的一些泛型类型的程序集出现类型不匹配
            Dim Asm As Assembly
            Try
                Asm = Assembly.Load(AssemblyName.GetAssemblyName(GetAbsolutePath(SimpleName & ".dll", Application.StartupPath)))
            Catch
                Try
                    Asm = Assembly.Load(AssemblyName.GetAssemblyName(GetAbsolutePath(GetPath(SimpleName, SimpleName & ".dll"), Application.StartupPath)))
                Catch
                    '动态生成的Assembly会在这里无法解析
                    'ExceptionHandler.PopInfo("{0}无法解析。".Formats(e.Name))
                    Return Nothing
                End Try
            End Try
            Dim LoadedName = New AssemblyName(Asm.FullName)
            If Not (AsmName.GetPublicKeyToken Is Nothing OrElse ArrayEqual(LoadedName.GetPublicKeyToken, AsmName.GetPublicKeyToken)) Then Return Nothing
            Return Asm
        Finally
            BlockRedirectBinding = False
        End Try
    End Function

    Private Factory As New LocalizationTextListFactoryAggregation(New ILocalizationTextListFactory() {New LocalizationTextListFactory()})
    Private Function LoadPlugin(ByVal Asm As Assembly) As Boolean
        Dim k = 0
        For Each Type In From t In Asm.GetTypes() Where t.GetInterfaces.Contains(GetType(ITextLocalizerPlugin))
            If Type.IsAbstract Then Continue For
            If Type.IsInterface Then Continue For
            If (From c In Type.GetConstructors Where c.GetParameters.Count = 0).Count = 0 Then Continue For

            Dim Obj As ITextLocalizerPlugin = Activator.CreateInstance(Type)
            Plugins.Add(Obj)
            k += 1

            Obj.Initialize((From l In LocalizationTextBoxes Select l.TextProvider.Name).ToArray, (From l In LocalizationTextBoxes Select l.TextProvider).ToArray, CurrentProject.MainLocalizationTextBox)
            Obj.InitializeController(Me)

            Dim TextHighlighter = TryCast(Obj, ITextLocalizerTextHighlighter)
            If TextHighlighter IsNot Nothing Then
                TextHighlighters.Add(TextHighlighter)
            End If

            Dim GridTextFormatter = TryCast(Obj, ITextLocalizerGridTextFormatter)
            If GridTextFormatter IsNot Nothing Then
                GridTextFormatters.Add(GridTextFormatter)
            End If

            Dim ControlPlugin = TryCast(Obj, ITextLocalizerControlPlugin)
            If ControlPlugin IsNot Nothing Then
                ControlPlugins.Add(ControlPlugin)
                Dim ControlDescriptors = ControlPlugin.GetControlDescriptors()
                If ControlDescriptors IsNot Nothing Then
                    For Each d In ControlDescriptors
                        Select Case d.Target
                            Case ControlId.MainWindow
                                Me.SuspendLayout()
                                Me.Controls.Add(d.Control)
                                d.Control.BringToFront()
                                Me.ResumeLayout(False)
                            Case ControlId.MainPanel
                                Me.Panel_Work.SuspendLayout()
                                Me.Panel_Work.Controls.Add(d.Control)
                                d.Control.BringToFront()
                                Me.Panel_Work.ResumeLayout(False)
                            Case ControlId.Grid
                                Me.SplitContainer_Main.Panel1.SuspendLayout()
                                Me.SplitContainer_Main.Panel1.Controls.Add(d.Control)
                                d.Control.BringToFront()
                                Me.SplitContainer_Main.Panel1.ResumeLayout(False)
                            Case ControlId.ToolStrip
                                Me.ToolStrip_Tools.Items.Add(d.Control)
                        End Select
                    Next
                End If
            End If

            Dim FormatPlugin = TryCast(Obj, ITextLocalizerFormatPlugin)
            If FormatPlugin IsNot Nothing Then
                Factory.AddFactories(FormatPlugin.GetTextListFactories())
            End If

            Dim TranslatorPlugin = TryCast(Obj, ITextLocalizerTranslatorPlugin)
            If TranslatorPlugin IsNot Nothing Then
                TranslatorPlugins.Add(TranslatorPlugin)
            End If
        Next
        Return k <> 0
    End Function

    Private Sub LoadOpt()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf RedirectBinding

        PreviousTextName = CurrentProject.TextName
        PreviousTextNumber = CurrentProject.TextNumber
        Me.Width = CurrentProject.WindowWidth
        Me.Height = CurrentProject.WindowHeight
        If CurrentProject.MainLocalizationTextBox < 0 OrElse CurrentProject.MainLocalizationTextBox >= CurrentProject.LocalizationTextBoxDescriptors.Length Then CurrentProject.MainLocalizationTextBox = 0

        If CurrentProject.LocalizationTextBoxDescriptors Is Nothing OrElse CurrentProject.LocalizationTextBoxDescriptors.Length < 1 Then
            ExceptionHandler.PopInfo("一栏文本框也没有")
            End
        End If

        Me.Panel_LocalizationBoxes.SuspendLayout()
        Me.SuspendLayout()

        For Each c As Control In Me.Panel_LocalizationBoxes.Controls
            c.Dispose()
        Next
        Me.Panel_LocalizationBoxes.Controls.Clear()

        Dim Splitters As New List(Of Splitter)
        Dim Height As Integer = 0
        Dim Des As LocalizationTextBoxDescriptor
        For n As Integer = 0 To CurrentProject.LocalizationTextBoxDescriptors.Length - 2
            Des = CurrentProject.LocalizationTextBoxDescriptors(n)
            Dim L As New LocalizationTextBox
            With L
                .Dock = System.Windows.Forms.DockStyle.Top
                .Location = New System.Drawing.Point(0, Height)
                .Name = String.Format("LocalizationTextBox{0}", n + 1)
                Height += Des.HeightRatio * Panel_LocalizationBoxes.Height
                .TabIndex = n

                .Space = Des.Space
                If Des.FontName <> "" Then .Font = New Font(Des.FontName, Des.FontPixel, FontStyle.Regular, GraphicsUnit.Pixel)

                .Size = New System.Drawing.Size(Me.Panel_LocalizationBoxes.Width, Des.HeightRatio * Panel_LocalizationBoxes.Height)
            End With
            LocalizationTextBoxes.Add(L)

            Dim S As New Splitter
            With S
                .Dock = System.Windows.Forms.DockStyle.Top
                .Location = New System.Drawing.Point(0, Height)
                .Name = String.Format("Splitter{0}", n + 1)
                .Size = New System.Drawing.Size(Me.Panel_LocalizationBoxes.Width, 3)
                Height += S.Height
                .TabStop = False
                .BackColor = System.Drawing.SystemColors.ScrollBar
                .BorderStyle = BorderStyle.None
            End With
            Splitters.Add(S)
        Next
        Des = CurrentProject.LocalizationTextBoxDescriptors(CurrentProject.LocalizationTextBoxDescriptors.Length - 1)
        Dim LL As New LocalizationTextBox
        With LL
            .Dock = System.Windows.Forms.DockStyle.Fill
            .Location = New System.Drawing.Point(0, Height)
            .Name = String.Format("LocalizationTextBox{0}", CurrentProject.LocalizationTextBoxDescriptors.Length)
            .TabIndex = CurrentProject.LocalizationTextBoxDescriptors.Length - 1

            .Space = Des.Space
            If Des.FontName <> "" Then .Font = New Font(Des.FontName, Des.FontPixel, FontStyle.Regular, GraphicsUnit.Pixel)

            .Size = New System.Drawing.Size(Me.Panel_LocalizationBoxes.Width, Me.Panel_LocalizationBoxes.Height - Height)
        End With
        LocalizationTextBoxes.Add(LL)

        Me.Panel_LocalizationBoxes.Controls.Add(LL)
        For n = LocalizationTextBoxes.Count - 2 To 0 Step -1
            Me.Panel_LocalizationBoxes.Controls.Add(Splitters(n))
            Me.Panel_LocalizationBoxes.Controls.Add(LocalizationTextBoxes(n))
        Next

        For n As Integer = 0 To CurrentProject.LocalizationTextBoxDescriptors.Length - 1
            Des = CurrentProject.LocalizationTextBoxDescriptors(n)
            Dim L = LocalizationTextBoxes(n)
            Dim Encoding As Encoding
            If Des.Encoding <> "" Then
                Dim PureDigits As Boolean = True
                For Each c In Des.Encoding
                    If Not Char.IsDigit(c) Then
                        PureDigits = False
                        Exit For
                    End If
                Next
                If PureDigits Then
                    Encoding = Encoding.GetEncoding(CInt(Des.Encoding))
                Else
                    Encoding = Encoding.GetEncoding(Des.Encoding)
                End If
            Else
                Encoding = TextEncoding.Default
            End If
            Dim tp As New LocalizationTextProvider(Factory, Des.Name, Des.DisplayName, Des.Directory, Des.Extension, Des.Type, Not Des.Editable, Encoding)
            Columns.Add(tp)
            LocalizationTextBoxes(n).Init(tp)
        Next

        For Each L In LocalizationTextBoxes
            AddHandler L.TextBox.SelectionChanged, AddressOf BoxScrolled
            AddHandler L.TextBox.TextChanged, AddressOf Box_TextChanged
            AddHandler L.GotFocus, AddressOf Box_GotFocus
        Next

        If CurrentProject.Plugins Is Nothing Then CurrentProject.Plugins = New PluginDescriptor() {}
        Dim PluginDescriptors As New List(Of PluginDescriptor)
        Dim PluginDescriptorSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each p In CurrentProject.Plugins
#If CONFIG <> "Debug" Then
            Try
#End If
            Dim AsmName As New AssemblyName(p.AssemblyName)
            Dim Asm As Assembly
            Try
                Asm = Assembly.Load(AsmName)
            Catch
                If Not p.Enable Then Continue For
                Throw
            End Try
            Dim NewAsmName = Asm.GetName
            Dim IsVersionMatch = (NewAsmName.Version = AsmName.Version)
            Dim OldFullName = p.AssemblyName
            Dim FullName = NewAsmName.FullName
            AsmName.Version = Nothing
            NewAsmName.Version = Nothing
            Dim IsOtherMatch = NewAsmName.FullName.Equals(AsmName.FullName, StringComparison.OrdinalIgnoreCase)
            If Not IsOtherMatch Then
                If MessageBox.Show("插件签名不匹配。\n原始签名：{0}\n实际签名：{1}\n承认该签名吗？".Descape.Formats(p.AssemblyName, FullName), Application_ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    p.AssemblyName = FullName
                    IsVersionMatch = True
                Else
                    PluginDescriptorSet.Add(AsmName.Name)
                    Continue For
                End If
            End If
            If Not IsVersionMatch Then
                MessageBox.Show("插件版本不匹配。\n原始签名：{0}\n实际签名：{1}\n将采用实际版本。".Descape.Formats(OldFullName, FullName), Application_ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information)
                p.AssemblyName = FullName
            End If
            If Not p.Enable Then
                If Not IsOtherMatch OrElse Not IsVersionMatch Then
                    If MessageBox.Show("插件变更但没有启用。\n签名：{0}\n启用插件吗？".Descape.Formats(FullName), Application_ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                        p.Enable = True
                    End If
                End If
            End If
            If Not p.Enable Then Continue For
            If Not LoadPlugin(Asm) Then
                ExceptionHandler.PopInfo("{0}中没有任何插件可加载。".Formats(p.AssemblyName))
            End If
            PluginDescriptorSet.Add(AsmName.Name)
#If CONFIG <> "Debug" Then
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
                If MessageBox.Show("插件加载出错，程序将关闭。\n签名：{0}\n下次启动是否禁用该插件？".Descape.Formats(p.AssemblyName), Application_ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    p.Enable = False
                    Xml.WriteFile(CurrentProjectFilePath, UTF16, CurrentProject)
                End If
                End
            End Try
#End If
        Next
        Dim NewPlugins As New List(Of PluginDescriptor)
        For Each DllPath In Directory.GetFiles(Application.StartupPath, "*.dll", SearchOption.TopDirectoryOnly).OrderBy(Function(f) f, StringComparer.OrdinalIgnoreCase)
            Dim Asm = Assembly.Load(AssemblyName.GetAssemblyName(GetAbsolutePath(DllPath, Application.StartupPath)))
            If PluginDescriptorSet.Contains(Asm.GetName.Name) Then Continue For
            Dim AsmName = Asm.GetName
            Dim p = New PluginDescriptor With {.AssemblyName = AsmName.FullName, .Enable = True}
#If CONFIG <> "Debug" Then
            Try
#End If
            If LoadPlugin(Asm) Then
                NewPlugins.Add(p)
            End If
#If CONFIG <> "Debug" Then
            Catch ex As Exception
                ExceptionHandler.PopupException(ex)
                If MessageBox.Show("{0}加载出错，程序将关闭。下次启动是否禁用该插件？".Formats(p.AssemblyName), Application_ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    p.Enable = False
                    NewPlugins.Add(p)
                    CurrentProject.Plugins = CurrentProject.Plugins.Concat(NewPlugins).ToArray
                    Xml.WriteFile(CurrentProjectFilePath, UTF16, CurrentProject)
                End If
                End
            End Try
#End If
        Next
        CurrentProject.Plugins = CurrentProject.Plugins.Concat(NewPlugins).ToArray

        For Each L In LocalizationTextBoxes
            Dim tp = L.TextProvider
            If CurrentProject.EnableLocalizationGrid Then
                DataGridView_Multiview.RowTemplate = New DataGridViewRow With {.HeaderCell = New DataGridViewRowIndexHeaderCell}
                If L.IsGlyphText Then
                    DataGridView_Multiview.Columns.Add(New DataGridViewImageColumnEx With {.Name = tp.Name, .HeaderText = tp.DisplayName, .CellTemplate = New DataGridViewImageCellEx With {.ValueIsIcon = False, .ImageLayout = DataGridViewImageCellLayout.Zoom}})
                Else
                    DataGridView_Multiview.Columns.Add(New DataGridViewRichTextBoxColumn With {.Name = tp.Name, .HeaderText = tp.DisplayName})
                End If
            End If
        Next

        If CurrentProject.EnableLocalizationGrid Then
            DataGridView_Multiview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            SplitContainer_Main.SplitterDistance = CurrentProject.LocalizationGridWidthRatio * Me.Width
            Dim RowHeadersWidth As Integer = CurrentProject.LocalizationRowHeaderWidthRatio * SplitContainer_Main.SplitterDistance
            If RowHeadersWidth < 4 Then RowHeadersWidth = 4
            DataGridView_Multiview.RowHeadersWidth = RowHeadersWidth
            For n = 0 To Columns.Count - 1
                Dim c = DataGridView_Multiview.Columns(n)
                c.Width = CurrentProject.LocalizationTextBoxDescriptors(n).ColumnWidthRatio * SplitContainer_Main.SplitterDistance
            Next
            If CurrentProject.LocalizationGridAutoResizeWidth Then
                SplitContainer_Main.FixedPanel = FixedPanel.None
                DataGridView_Multiview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                Dim Sum = DataGridView_Multiview.RowHeadersWidth + (From c As DataGridViewColumn In DataGridView_Multiview.Columns Select c.Width).Sum
                RowHeadersWidth = CurrentProject.LocalizationRowHeaderWidthRatio * Sum
                If RowHeadersWidth < 4 Then RowHeadersWidth = 4
                DataGridView_Multiview.RowHeadersWidth = RowHeadersWidth
                For n = 0 To Columns.Count - 1
                    DataGridView_Multiview.Columns(n).Width = CurrentProject.LocalizationTextBoxDescriptors(n).ColumnWidthRatio * Sum
                Next
            Else
                SplitContainer_Main.FixedPanel = FixedPanel.Panel2
                DataGridView_Multiview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            End If
        Else
            SplitContainer_Main.FixedPanel = FixedPanel.Panel2
            DataGridView_Multiview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            SplitContainer_Main.SplitterDistance = 0
            SplitContainer_Main.IsSplitterFixed = True
            Me.Width = CurrentProject.WindowWidth
            Me.Height = CurrentProject.WindowHeight
        End If

        Dim DisplayLOCBoxTip = False
        For Each L In LocalizationTextBoxes
            If L.IsGlyphText Then
                DisplayLOCBoxTip = True
                Exit For
            End If
        Next
        Label_LOCBoxTip.Visible = DisplayLOCBoxTip

        Me.Panel_LocalizationBoxes.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub
    Private Sub SaveOpt()
        CurrentProject.TextName = TextName
        CurrentProject.TextNumber = TextNumber

        If CurrentProject.Plugins IsNot Nothing Then
            TextHighlighters.Clear()
            GridTextFormatters.Clear()
            ControlPlugins.Clear()

            For Each p In Plugins
                p.Dispose()
            Next
            Plugins.Clear()
        End If

        Select Case MeWindowState
            Case FormWindowState.Normal
                CurrentProject.WindowWidth = MeWidth
                CurrentProject.WindowHeight = MeHeight
                CurrentProject.Maximized = False
            Case FormWindowState.Maximized
                CurrentProject.Maximized = True
            Case Else
                Throw New InvalidOperationException
        End Select
        If Panel_LocalizationBoxes.Height > 0 Then
            For n As Integer = 0 To Columns.Count - 1
                Dim Des = CurrentProject.LocalizationTextBoxDescriptors(n)
                Des.HeightRatio = LocalizationTextBoxHeights(n) / Panel_LocalizationBoxes.Height
            Next
        End If
        If CurrentProject.EnableLocalizationGrid Then
            If FormWindowState.Maximized AndAlso Not CurrentProject.LocalizationGridAutoResizeWidth Then
                Dim SplitContainer_Main_SplitterDistance = MeNormalWidth - (MeWidth - SplitContainer_Main.SplitterDistance)
                If MeNormalWidth > 0 Then CurrentProject.LocalizationGridWidthRatio = SplitContainer_Main_SplitterDistance / MeNormalWidth
                If SplitContainer_Main_SplitterDistance > 0 Then
                    CurrentProject.LocalizationRowHeaderWidthRatio = DataGridView_Multiview.RowHeadersWidth / SplitContainer_Main_SplitterDistance
                    For n = 0 To Columns.Count - 1
                        CurrentProject.LocalizationTextBoxDescriptors(n).ColumnWidthRatio = DataGridView_Multiview.Columns(n).Width / SplitContainer_Main_SplitterDistance
                    Next
                End If
            Else
                If MeWidth > 0 Then CurrentProject.LocalizationGridWidthRatio = SplitContainer_Main.SplitterDistance / MeWidth
                Dim Sum = DataGridView_Multiview.RowHeadersWidth + (From c As DataGridViewColumn In DataGridView_Multiview.Columns Select c.Width).Sum
                If Sum > 0 Then
                    CurrentProject.LocalizationRowHeaderWidthRatio = DataGridView_Multiview.RowHeadersWidth / Sum
                    For n = 0 To Columns.Count - 1
                        CurrentProject.LocalizationTextBoxDescriptors(n).ColumnWidthRatio = DataGridView_Multiview.Columns(n).Width / Sum
                    Next
                End If
            End If
        End If
    End Sub
#End Region

    Private RichTextBoxForText As New ExtendedRichTextBox
    Private GridRows As Object()()
    Private Function IsGridViewRowValueInitialize() As Boolean
        Return GridRows IsNot Nothing
    End Function
    Private Function IsGridViewRowValueCached(ByVal TextIndex As Integer) As Boolean
        Dim r = GridRows(TextIndex)
        Return r IsNot Nothing
    End Function
    Private Function GetGridViewRowValue(ByVal TextIndex As Integer) As Object()
        Return GridRows(TextIndex)
    End Function
    Private Sub UpdateGridRowValue(ByVal TextIndex As Integer)
        Dim r = GridRows(TextIndex)
        If r Is Nothing Then
            r = New Object(Columns.Count - 1) {}
            GridRows(TextIndex) = r
        End If
        Dim Texts = New String(Columns.Count - 1) {}
        For TextColumn = 0 To Columns.Count - 1
            Dim L = LocalizationTextBoxes(TextColumn)
            If L.IsGlyphText Then
                Texts(TextColumn) = ""
            Else
                If L.TextProvider.ContainsKey(TextName) Then
                    Dim TextList = L.TextProvider.Item(TextName)
                    Texts(TextColumn) = TextList.Text(TextIndex)
                Else
                    Texts(TextColumn) = ""
                    Dim k = TextColumn
                    Dim tt = Function(Text) TranslateText(CurrentProject.MainLocalizationTextBox, k, Text)
                    Dim TextList = L.TextProvider.LoadOrCreateItem(TextName, Columns(CurrentProject.MainLocalizationTextBox).Item(TextName), tt)
                    If TextList IsNot Nothing Then
                        Texts(TextColumn) = TextList.Text(TextIndex)
                    End If
                End If
            End If
        Next
        If GridTextFormatters.Count > 0 Then
            For Each GridTextFormatter In GridTextFormatters
                Texts = GridTextFormatter.Format(TextName, TextIndex, Texts).ToArray
            Next
        End If
        For TextColumn = 0 To Columns.Count - 1
            Dim L = LocalizationTextBoxes(TextColumn)
            If L.TextProvider.ContainsKey(TextName) Then
                If L.IsGlyphText Then
                    Dim TextListLOC = CType(L.TextProvider.Item(TextName), LOCList)
                    Dim TextListGlyph = TextListLOC.LOC.GetGlyphText
                    Dim Image = TextListGlyph.GetBitmap(TextIndex, L.Space)
                    If Image Is Nothing Then
                        Image = New Bitmap(1, 1)
                        Using g = Graphics.FromImage(Image)
                            g.Clear(Color.White)
                        End Using
                    End If
                    r(TextColumn) = Image
                Else
                    Dim Text As String = Texts(TextColumn)
                    RichTextBoxForText.Text = Text
                    r(TextColumn) = RichTextBoxForText.Rtf
                End If
            Else
                If L.IsGlyphText Then
                    Dim Image = New Bitmap(1, 1)
                    Using g = Graphics.FromImage(Image)
                        g.Clear(Color.White)
                    End Using
                    r(TextColumn) = Image
                Else
                    RichTextBoxForText.Text = ""
                    r(TextColumn) = RichTextBoxForText.Rtf
                End If
            End If
        Next
        If TextHighlighters.Count > 0 Then
            Dim Highlights = (From h In TextHighlighters Select h.GetTextStyles(TextName, TextIndex, Texts)).ToArray
            For TextColumn = 0 To Columns.Count - 1
                Dim L = LocalizationTextBoxes(TextColumn)
                If L.TextProvider.ContainsKey(TextName) Then
                    If L.IsGlyphText Then
                    Else
                        RichTextBoxForText.Rtf = r(TextColumn)
                        For Each h In Highlights
                            If h Is Nothing Then Continue For
                            If h(TextColumn) Is Nothing Then Continue For
                            For Each ts In h(TextColumn)
                                If ts.Length < 0 OrElse ts.Index < 0 OrElse ts.Index + ts.Length > RichTextBoxForText.TextLength Then Continue For
                                RichTextBoxForText.SelectionStart = ts.Index
                                RichTextBoxForText.SelectionLength = ts.Length
                                RichTextBoxForText.SelectionColor = ts.ForeColor
                                RichTextBoxForText.SelectionBackColor = ts.BackColor
                            Next
                        Next
                        r(TextColumn) = RichTextBoxForText.Rtf
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub UpdateGridTextIndex(ByVal row As DataGridViewRow, ByVal TextIndex As Integer)
        UpdateGridRowValue(TextIndex)
        Dim r = GetGridViewRowValue(TextIndex)
        For TextColumn = 0 To Columns.Count - 1
            row.Cells(TextColumn).Value = r(TextColumn)
        Next
    End Sub

    Private ReadOnly Property TextCount() As Integer
        Get
            If LocalizationTextBoxes Is Nothing Then Return 0
            If CurrentProject.MainLocalizationTextBox < 0 OrElse CurrentProject.MainLocalizationTextBox >= Columns.Count Then Return 0
            Return LocalizationTextBoxes(CurrentProject.MainLocalizationTextBox).TextCount
        End Get
    End Property

    Private Function TranslateText(ByVal SourceColumn As Integer, ByVal TargetColumn As Integer, ByVal s As String) As String
        Dim ret As String = s
        For Each t In TranslatorPlugins
            ret = t.TranslateText(SourceColumn, TargetColumn, ret)
        Next
        Return ret
    End Function

    Private TextNameValue As String
    Private Sub UpdateToTextName(ByVal TextName As String)
        Dim Value = TextName

        TextNameValue = Value
        ComboBox_TextName.Text = Value
        If Value = "" Then Return
        FlushLocalizedText()

        For Each L In LocalizationTextBoxes
            L.LoadText(TextName)
        Next
        LocalizerEnable = False
        Dim MaxCount As Integer = TextCount
        If MaxCount <= 0 Then
            VScrollBar_Bar.Minimum = 1
            VScrollBar_Bar.Maximum = 1
            NumericUpDown_TextNumber.Minimum = 1
            NumericUpDown_TextNumber.Maximum = 1
            Return
        End If
        Dim k = 0
        For Each L In LocalizationTextBoxes
            If Not L.IsLoaded Then
                Dim tt = Function(Text) TranslateText(CurrentProject.MainLocalizationTextBox, k, Text)
                L.LoadOrCreateText(TextName, Columns(CurrentProject.MainLocalizationTextBox).Item(TextName), tt)
            End If
            If Not L.IsReadOnly Then
                If L.TextCount <> MaxCount Then
                    Throw New InvalidDataException("{0}文本条数{1}与主文本条数{2}不一致".Formats(GetPath(L.TextProvider.Directory, TextName & "." & L.TextProvider.Extension), L.TextCount, MaxCount))
                End If
            End If
            k += 1
        Next

        FlushLocalizedText()

        Me.SuspendLayout()
        If CurrentProject.EnableLocalizationGrid Then
            Dim NumRow = LocalizationTextBoxes(CurrentProject.MainLocalizationTextBox).TextCount
            DataGridView_Multiview.CausesValidation = False
            GridRows = New Object(NumRow - 1)() {}
            DataGridView_Multiview.RowCount = NumRow
        End If

        VScrollBar_Bar.Minimum = 1
        VScrollBar_Bar.Maximum = MaxCount
        NumericUpDown_TextNumber.Minimum = 1
        NumericUpDown_TextNumber.Maximum = MaxCount
        UpdateToTextIndex(0)
        LocalizerEnable = True
        Me.ResumeLayout(False)

        RaiseEvent TextNameChanged(Nothing)
    End Sub
    Public Property TextName() As String
        Get
            Return TextNameValue
        End Get
        Set(ByVal Value As String)
            If TextNameValue = Value Then Return
            UpdateToTextName(Value)
        End Set
    End Property

    Public Property LocalizerEnable() As Boolean
        Get
            Return Panel_LocalizationBoxes.Enabled
        End Get
        Set(ByVal Value As Boolean)
            Panel_LocalizationBoxes.Enabled = Value
            VScrollBar_Bar.Enabled = Value
        End Set
    End Property

    Private TextIndexValue As Integer = 0
    Public Property TextNumber() As Integer
        Get
            Return TextIndex + 1
        End Get
        Set(ByVal Value As Integer)
            TextIndex = Value - 1
        End Set
    End Property
    Public Property TextIndex() As Integer
        Get
            Return TextIndexValue
        End Get
        Set(ByVal Value As Integer)
            If TextIndexValue = Value Then Return
            UpdateToTextIndex(Value)
        End Set
    End Property
    Private Sub UpdateToTextIndex(ByVal TextIndex As Integer)
        Dim Value = TextIndex
        FlushLocalizedText()
        If CurrentProject.EnableLocalizationGrid Then
            If TextIndexValue >= 0 AndAlso TextIndexValue < TextCount Then
                UpdateGridTextIndex(DataGridView_Multiview.Rows(TextIndexValue), TextIndexValue)
                DataGridView_Multiview.InvalidateRow(TextIndexValue)
            End If
        End If
        If Value < VScrollBar_Bar.Minimum - 1 Then Value = VScrollBar_Bar.Minimum - 1
        If Value > VScrollBar_Bar.Maximum - 1 Then Value = VScrollBar_Bar.Maximum - 1
        TextIndexValue = Value
        VScrollBar_Bar.Value = Value + 1
        For Each L In LocalizationTextBoxes
            L.TextIndex = Value
        Next
        NumericUpDown_TextNumber.Value = VScrollBar_Bar.Value
        ReHighlight()
        If CurrentProject.EnableLocalizationGrid Then
            If BlockCell Then Return
            BlockCell = True
            Dim First = DataGridView_Multiview.FirstDisplayedScrollingRowIndex
            Dim Count = DataGridView_Multiview.DisplayedRowCount(False)
            Dim Index = Value
            If Index < First OrElse Index >= First + Count Then
                DataGridView_Multiview.FirstDisplayedScrollingRowIndex = Index
            End If
            DataGridView_Multiview.Refresh()
            For Each r As DataGridViewRow In DataGridView_Multiview.Rows
                r.Selected = False
            Next
            DataGridView_Multiview.Rows(Index).Selected = True
            If DataGridView_Multiview.CurrentCell Is Nothing AndAlso DataGridView_Multiview.ColumnCount > 0 Then
                DataGridView_Multiview.CurrentCell = DataGridView_Multiview.Rows(Index).Cells(0)
            ElseIf DataGridView_Multiview.CurrentCell.RowIndex <> Index Then
                DataGridView_Multiview.CurrentCell = DataGridView_Multiview.Rows(Index).Cells(DataGridView_Multiview.CurrentCell.ColumnIndex)
            End If
            BlockCell = False
        End If
        RaiseEvent TextIndexChanged(Nothing)
    End Sub

    Private Sub RePositionBoxScrollBars()
        If Application_ColumnIndex < 0 OrElse Application_ColumnIndex >= LocalizationTextBoxes.Count Then Return
        Dim Foucsed = LocalizationTextBoxes(Application_ColumnIndex)
        Dim si = Foucsed.TextBox.VerticalScrollInformation
        If si Is Nothing Then Return
        If si.Maximum - si.Minimum <= 0 Then Return
        Dim Ratio = si.Position / (si.Maximum - si.Minimum)

        For Each l In LocalizationTextBoxes
            If l Is Foucsed Then Continue For
            Dim lsi = l.TextBox.VerticalScrollInformation
            If lsi Is Nothing Then Continue For
            If lsi.Maximum - lsi.Minimum <= 0 Then Continue For
            Dim n = lsi.Minimum + CInt(Ratio * (lsi.Maximum - lsi.Minimum))
            If n < lsi.Minimum Then n = lsi.Minimum
            If n > lsi.Maximum Then n = lsi.Maximum
            If Abs(n - lsi.Position) < l.Font.Height * 1.5 Then Continue For
            l.TextBox.ScrollPosition = n
        Next
    End Sub

    Private Sub ReHighlight()
        For Each l In LocalizationTextBoxes
            Dim Source = l.TextBox
            Source.SuspendDraw()
            Source.SuspendUndoHistory()
            Dim SourceStart = Source.SelectionStart
            Dim SourceLength = Source.SelectionLength
            Source.SelectAll()
            Source.SelectionColor = System.Drawing.SystemColors.ControlText
            Source.SelectionBackColor = System.Drawing.SystemColors.Window
            Source.SelectionStart = SourceStart
            Source.SelectionLength = SourceLength
        Next
        If TextHighlighters.Count > 0 Then
            Dim Texts = New String(Columns.Count - 1) {}
            For TextColumn = 0 To Columns.Count - 1
                Dim L = LocalizationTextBoxes(TextColumn)
                Dim SourceText = L.TextBox.Text
                If SourceText Is Nothing Then SourceText = ""
                Texts(TextColumn) = SourceText
            Next
            Dim Highlights = (From h In TextHighlighters Select h.GetTextStyles(TextName, TextIndex, Texts)).ToArray
            For TextColumn = 0 To Columns.Count - 1
                Dim L = LocalizationTextBoxes(TextColumn)
                Dim Source = L.TextBox
                Dim SourceStart = Source.SelectionStart
                Dim SourceLength = Source.SelectionLength
                For Each h In Highlights
                    If h Is Nothing Then Continue For
                    If h(TextColumn) Is Nothing Then Continue For
                    For Each ts In h(TextColumn)
                        If ts.Length < 0 OrElse ts.Index < 0 OrElse ts.Index + ts.Length > Source.TextLength Then Continue For
                        Source.SelectionStart = ts.Index
                        Source.SelectionLength = ts.Length
                        Source.SelectionColor = ts.ForeColor
                        Source.SelectionBackColor = ts.BackColor
                    Next
                Next
                Source.SelectionStart = SourceStart
                Source.SelectionLength = SourceLength
            Next
        End If
        For Each l In LocalizationTextBoxes
            Dim Source = l.TextBox
            Source.ResumeDraw()
            Source.ResumeUndoHistory()
            Source.Invalidate()
        Next
    End Sub

    Private Sub BoxScrolled(ByVal sender As Object, ByVal e As EventArgs)
        RePositionBoxScrollBars()
    End Sub

    Private WithEvents Timer As New Timer
    Friend IMECompositing As Integer = 0
    Private Block As Integer = 0
    Private Sub Box_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer.Tick
        If System.Threading.Interlocked.CompareExchange(IMECompositing, -1, -1) Then Return
        System.Threading.Interlocked.Exchange(Block, -1)
        Timer.Stop()
        ReHighlight()
        System.Threading.Interlocked.Exchange(Block, 0)
    End Sub
    Private Sub Box_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        If System.Threading.Interlocked.CompareExchange(Block, -1, -1) Then Return
        Timer.Stop()
        Timer.Interval = 500
        Timer.Start()
    End Sub

    Private PreviousTextName As String
    Private PreviousTextNumber As Integer
    Private Sub OpenDefaultFile()
        If PreviousTextName = "" OrElse Not TextNameDict.ContainsKey(PreviousTextName) Then
            If TextNames.Count > 0 Then
                TextName = TextNames(0)
            Else
                TextName = ""
            End If
        Else
            TextName = PreviousTextName
        End If
    End Sub
    Private Sub MoveToDefaultTextNumber()
        If PreviousTextName = "" OrElse Not TextNameDict.ContainsKey(PreviousTextName) Then
        Else
            TextNumber = PreviousTextNumber
        End If
    End Sub

    Private Sub Button_Open_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Open.Click
        If ComboBox_TextName.Text = "" Then Return
        TextName = ComboBox_TextName.Text
    End Sub

    Public Class Intercepter
        Implements IMessageFilter

        Public Handled As Boolean = True

        Private Function DirectToInt32(ByVal i As IntPtr) As Int32
            If IntPtr.Size = 4 Then Return i.ToInt32
            If IntPtr.Size = 8 Then Return CID(i.ToInt64)
            Return CID(i.ToInt64)
        End Function

        Private Const WM_MOUSEWHEEL = 522
        Private Const WM_IME_STARTCOMPOSITION = &H10D
        Private Const WM_IME_ENDCOMPOSITION = &H10E
        Private Const WM_IME_NOTIFY = &H282
        Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements System.Windows.Forms.IMessageFilter.PreFilterMessage
            Select Case m.Msg
                Case WM_MOUSEWHEEL
                    My.Forms.FormMain.TextLocalizer_MouseWheel(Me, New MouseEventArgs(DirectToInt32(m.WParam) And &HFFFF, 0, DirectToInt32(m.LParam) And &HFFFF, (DirectToInt32(m.LParam.ToInt64) >> 16) And &HFFFF, CUS(CUShort((DirectToInt32(m.WParam) >> 16) And &HFFFF))))
                    Dim h = Handled
                    Handled = True
                    Return h
                Case WM_IME_STARTCOMPOSITION
                    System.Threading.Interlocked.Exchange(My.Forms.FormMain.IMECompositing, -1)
                Case WM_IME_ENDCOMPOSITION
                    System.Threading.Interlocked.Exchange(My.Forms.FormMain.IMECompositing, 0)
                Case WM_IME_NOTIFY

            End Select
            Return False
        End Function
    End Class

    Private Sub TextLocalizer_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        Dim p = DataGridView_Multiview.PointToClient(MousePosition)
        If p.X >= 0 AndAlso p.Y >= 0 AndAlso p.X < DataGridView_Multiview.Width AndAlso p.Y < DataGridView_Multiview.Height Then
            Dim i = TryCast(sender, Intercepter)
            If i IsNot Nothing Then
                If DataGridView_Multiview.Focused Then
                    i.Handled = False
                Else
                    DataGridView_Multiview.Focus()
                End If
            End If
            Return
        End If
        If ComboBox_TextName.DroppedDown Then
            Dim i = TryCast(sender, Intercepter)
            If i IsNot Nothing Then
                i.Handled = False
            End If
            Return
        End If

        If ActiveForm IsNot Me Then Return
        If Not LocalizerEnable Then Return
        TextNumber = CInt(VScrollBar_Bar.Value - e.Delta / 120)
    End Sub
    Private Sub TextLocalizer_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        RaiseEvent Application_KeyDown(ControlId.MainWindow, e)
        If e.Handled Then Return
        Select Case e.KeyData
            Case Keys.PageUp
                If DataGridView_Multiview.Focused Then Return
                If VScrollBar_Bar.Focused Then Return
                If Not LocalizerEnable Then Return
                TextNumber = TextNumber - 1
            Case Keys.PageDown
                If DataGridView_Multiview.Focused Then Return
                If VScrollBar_Bar.Focused Then Return
                If Not LocalizerEnable Then Return
                TextNumber = TextNumber + 1
            Case Keys.F6
                Button_PreviousFile_Click(sender, e)
            Case Keys.F7
                Button_NextFile_Click(sender, e)
            Case Keys.Control Or Keys.G
                If Not NumericUpDown_TextNumber.Focused Then NumericUpDown_TextNumber.Focus()
                NumericUpDown_TextNumber.Select(0, NumericUpDown_TextNumber.Text.Length)
            Case Else
                Return
        End Select
        e.Handled = True
    End Sub
    Private Sub VScrollBar_Bar_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles VScrollBar_Bar.ValueChanged
        If Not LocalizerEnable Then Return
        TextNumber = VScrollBar_Bar.Value
    End Sub
    Private Sub NumericUpDown_TextNumber_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown_TextNumber.ValueChanged
        If VScrollBar_Bar.Value <> NumericUpDown_TextNumber.Value Then VScrollBar_Bar.Value = NumericUpDown_TextNumber.Value
    End Sub

    Private Sub Button_PreviousFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_PreviousFile.Click
        If TextName = "" OrElse Not TextNameDict.ContainsKey(TextName) Then
            If TextNames.Count > 0 Then
                If TextNames.Count > 0 Then
                    TextName = TextNames(0)
                Else
                    TextName = ""
                End If
            End If
        End If
        If TextName <> "" Then
            Dim Number = TextNameDict(TextName)
            If Number < 1 Then Return
            TextName = TextNames(Number - 1)
            Button_Open_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub Button_NextFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_NextFile.Click
        If TextName = "" OrElse Not TextNameDict.ContainsKey(TextName) Then
            If TextNames.Count > 0 Then
                If TextNames.Count > 0 Then
                    TextName = TextNames(0)
                Else
                    TextName = ""
                End If
            End If
        End If
        If TextName <> "" Then
            Dim Number = TextNameDict(TextName)
            If Number + 1 >= TextNames.Count Then Return
            TextName = TextNames(Number + 1)
            Button_Open_Click(Nothing, Nothing)
        End If
    End Sub

    Private MeWindowState As FormWindowState
    Private MeWidth As Integer
    Private MeHeight As Integer
    Private MeNormalWidth As Integer
    Private MeNormalHeight As Integer
    Private MainPanelHeight As Integer
    Private LocalizationTextBoxHeights As Integer()
    Private Function GetLocalizationTextBoxHeights() As Integer()
        Dim LocalizationTextBoxHeights = New Integer(Columns.Count - 1) {}
        For n = 0 To Columns.Count - 1
            LocalizationTextBoxHeights(n) = LocalizationTextBoxes(n).Height
        Next
        Return LocalizationTextBoxHeights
    End Function
    Private Sub Panel_LocalizationBoxes_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel_LocalizationBoxes.Resize
        If Me.WindowState = FormWindowState.Minimized Then Return
        If Panel_LocalizationBoxes.Height = 0 Then Return
        If MainPanelHeight = 0 Then Return
        For n = 0 To Columns.Count - 1
            LocalizationTextBoxes(n).Height *= Panel_LocalizationBoxes.Height / MainPanelHeight
        Next

        MeWindowState = Me.WindowState
        MeWidth = Me.Width
        MeHeight = Me.Height
        If Me.WindowState = FormWindowState.Normal Then
            MeNormalWidth = Me.Width
            MeNormalHeight = Me.Height
        End If
        MainPanelHeight = Panel_LocalizationBoxes.Height
        LocalizationTextBoxHeights = GetLocalizationTextBoxHeights()
    End Sub

    Private BlockCell As Boolean = False
    Private Sub DataGridView_Multiview_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView_Multiview.CellEnter
        If BlockCell Then Return
        If Not LocalizerEnable Then Return
        BlockCell = True
        Try
            TextNumber = e.RowIndex + 1
        Finally
            BlockCell = False
        End Try
    End Sub

    Private Sub DataGridView_Multiview_RowHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView_Multiview.RowHeaderMouseClick
        If BlockCell Then Return
        If Not LocalizerEnable Then Return
        BlockCell = True
        Try
            TextNumber = e.RowIndex + 1
        Finally
            BlockCell = False
        End Try
    End Sub

    Private Sub DataGridView_Multiview_CellValueNeeded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValueEventArgs) Handles DataGridView_Multiview.CellValueNeeded
        If Not IsGridViewRowValueInitialize() Then Return
        If Not IsGridViewRowValueCached(e.RowIndex) Then
            UpdateGridRowValue(e.RowIndex)
        End If
        e.Value = GetGridViewRowValue(e.RowIndex)(e.ColumnIndex)
    End Sub

    Private Sub DataGridView_Multiview_RowPrePaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPrePaintEventArgs) Handles DataGridView_Multiview.RowPrePaint
        Dim TextIndex = e.RowIndex
        Dim PreferredHeight = DataGridView_Multiview.Rows(TextIndex).GetPreferredHeight(TextIndex, DataGridViewAutoSizeRowMode.AllCells, True)
        If DataGridView_Multiview.Rows(TextIndex).Height <> PreferredHeight Then
            DataGridView_Multiview.Rows(TextIndex).Height = PreferredHeight
        End If
    End Sub

    Private BlockCheck As Boolean = False
    Public Sub Application_RefreshGrid() Implements ITextLocalizerApplicationController.RefreshGrid
        If Not LocalizerEnable Then Return
        If BlockCheck Then Return
        BlockCheck = True
        Application.DoEvents()

        DataGridView_Multiview.SuspendLayout()
        Dim NumRow = LocalizationTextBoxes(CurrentProject.MainLocalizationTextBox).TextCount
        For TextIndex As Integer = 0 To NumRow - 1
            UpdateGridTextIndex(DataGridView_Multiview.Rows(TextIndex), TextIndex)
        Next
        DataGridView_Multiview.ResumeLayout()
        DataGridView_Multiview.Invalidate()

        BlockCheck = False
    End Sub
    Public Sub Application_RefreshColumn(ByVal ColumnIndex As Integer) Implements ITextLocalizerApplicationController.RefreshColumn
        LocalizationTextBoxes(ColumnIndex).UpdateDisplay()
    End Sub
    Public Sub Application_RefreshMainPanel() Implements ITextLocalizerApplicationController.RefreshMainPanel
        For Each lb In LocalizationTextBoxes
            Dim TextLength = lb.TextBox.TextLength
            Dim SelectionStart = lb.TextBox.SelectionStart
            Dim SelectionLength = lb.TextBox.SelectionLength
            lb.UpdateDisplay()
            If lb.TextBox.TextLength = TextLength Then
                If lb.TextBox.SelectionStart <> SelectionStart OrElse lb.TextBox.SelectionLength <> SelectionLength Then
                    lb.TextBox.SelectionStart = SelectionStart
                    lb.TextBox.SelectionLength = SelectionLength
                End If
            End If
        Next
    End Sub
    Public Sub Application_FlushLocalizedText() Implements ITextLocalizerApplicationController.FlushLocalizedText
        FlushLocalizedText()
    End Sub

    Public Event TextIndexChanged(ByVal e As System.EventArgs) Implements Firefly.Project.ITextLocalizerApplicationController.TextIndexChanged
    Public Event TextNameChanged(ByVal e As System.EventArgs) Implements Firefly.Project.ITextLocalizerApplicationController.TextNameChanged
    Public Event Application_ColumnSelectionChanged(ByVal e As System.EventArgs) Implements ITextLocalizerApplicationController.ColumnSelectionChanged
    Public Event Application_KeyDown(ByVal ControlId As ControlId, ByVal e As System.Windows.Forms.KeyEventArgs) Implements ITextLocalizerApplicationController.KeyDown
    Public Event Application_KeyPress(ByVal ControlId As ControlId, ByVal e As System.Windows.Forms.KeyPressEventArgs) Implements ITextLocalizerApplicationController.KeyPress
    Public Event Application_KeyUp(ByVal ControlId As ControlId, ByVal e As System.Windows.Forms.KeyEventArgs) Implements ITextLocalizerApplicationController.KeyUp

    Public ReadOnly Property Form() As System.Windows.Forms.Form Implements ITextLocalizerApplicationController.Form
        Get
            Return Me
        End Get
    End Property
    Public ReadOnly Property Application_ApplicationName() As String Implements ITextLocalizerApplicationController.ApplicationName
        Get
            Return Me.Text
        End Get
    End Property
    Public Property Application_TextName() As String Implements ITextLocalizerApplicationController.TextName
        Get
            Return TextName
        End Get
        Set(ByVal Value As String)
            TextName = Value
        End Set
    End Property
    Public Property Application_TextIndex() As Integer Implements ITextLocalizerApplicationController.TextIndex
        Get
            Return TextIndex
        End Get
        Set(ByVal Value As Integer)
            TextIndex = Value
        End Set
    End Property
    Public Property Application_TextIndices() As IEnumerable(Of Integer) Implements ITextLocalizerApplicationController.TextIndices
        Get
            If CurrentProject.EnableLocalizationGrid Then
                Return From c As DataGridViewCell In DataGridView_Multiview.SelectedCells Select c.RowIndex Distinct
            Else
                Return New Integer() {TextIndex}
            End If
        End Get
        Set(ByVal Value As IEnumerable(Of Integer))
            If CurrentProject.EnableLocalizationGrid Then
                For Each r As DataGridViewRow In DataGridView_Multiview.SelectedRows
                    r.Selected = False
                Next
                For Each n In Value
                    If n < 0 OrElse n >= DataGridView_Multiview.RowCount Then Continue For
                    Dim r As DataGridViewRow = DataGridView_Multiview.Rows(n)
                    r.Selected = True
                Next
            ElseIf Value.Count = 1 Then
                TextIndex = Value(0)
            End If
        End Set
    End Property
    Private CurrentColumnIndex As Integer = 0
    Private Sub Box_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        For n = 0 To Columns.Count - 1
            If LocalizationTextBoxes(n).Focused Then
                CurrentColumnIndex = n
                RaiseEvent Application_ColumnSelectionChanged(e)
                Return
            End If
        Next
    End Sub
    Public Property Application_ColumnIndex() As Integer Implements ITextLocalizerApplicationController.ColumnIndex
        Get
            If Columns.Count = 0 Then Throw New InvalidOperationException
            For n = 0 To Columns.Count - 1
                If LocalizationTextBoxes(n).Focused Then
                    Return n
                End If
            Next
            Return CurrentColumnIndex
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 OrElse Value >= Columns.Count Then Return
            LocalizationTextBoxes(Value).Focus()
        End Set
    End Property
    Public Property Application_SelectionStart() As Integer Implements ITextLocalizerApplicationController.SelectionStart
        Get
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            If l.TextBox.Visible Then Return l.TextBox.SelectionStart
            Return 0
        End Get
        Set(ByVal Value As Integer)
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            l.TextBox.SelectionStart = Value
        End Set
    End Property
    Public Property Application_SelectionLength() As Integer Implements ITextLocalizerApplicationController.SelectionLength
        Get
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            If l.TextBox.Visible Then Return l.TextBox.SelectionLength
            Return 0
        End Get
        Set(ByVal Value As Integer)
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            l.TextBox.SelectionLength = Value
        End Set
    End Property
    Public Property Application_Text(ByVal ColumnIndex As Integer) As String Implements ITextLocalizerApplicationController.Text
        Get
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            Return l.Text
        End Get
        Set(ByVal Value As String)
            If Columns.Count = 0 Then Throw New InvalidOperationException
            Dim l = LocalizationTextBoxes(CurrentColumnIndex)
            If l.IsReadOnly Then
                MessageBox.Show("无法修改只读文本。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                l.Text = Value
            End If
        End Set
    End Property
    Public Sub ScrollToCaret(ByVal ColumnIndex As Integer) Implements ITextLocalizerApplicationController.ScrollToCaret
        Dim lb = LocalizationTextBoxes(ColumnIndex)
        lb.TextBox.ScrollToCaret()
        If Not lb.TextBox.Visible Then lb.SwitchBox()
    End Sub
End Class
