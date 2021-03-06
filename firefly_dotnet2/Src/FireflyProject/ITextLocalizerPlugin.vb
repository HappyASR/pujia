'==========================================================================
'
'  File:        ITextLocalizerPlugin.vb
'  Location:    Firefly.Project <Visual Basic .Net>
'  Description: 文本本地化工具插件接口
'  Version:     2010.01.13.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>TextLocalizer的所有插件的接口</summary>
Public Interface ITextLocalizerPlugin
    Inherits IDisposable

    Sub Initialize(ByVal TextNames As IEnumerable(Of String), ByVal Columns As IEnumerable(Of LocalizationTextProvider), ByVal MainColumnIndex As Integer)
    Sub InitializeController(ByVal Receiver As ITextLocalizerApplicationController)
End Interface

''' <summary>TextLocalizer的所有插件的接口的默认基类实现</summary>
Public MustInherit Class TextLocalizerBase
    Implements ITextLocalizerPlugin

    Protected TextNames As IEnumerable(Of String)
    Protected Columns As IEnumerable(Of LocalizationTextProvider)
    Protected MainColumnIndex As Integer
    Protected NameToColumn As New Dictionary(Of String, Integer)

    Public Overridable Sub Initialize(ByVal TextNames As IEnumerable(Of String), ByVal Columns As IEnumerable(Of LocalizationTextProvider), ByVal MainColumnIndex As Integer) Implements ITextLocalizerPlugin.Initialize
        Me.TextNames = TextNames
        Me.Columns = Columns
        Me.MainColumnIndex = MainColumnIndex
        Dim k = 0
        For Each c In Columns
            NameToColumn.Add(c.Name, k)
            k += 1
        Next
    End Sub

    Protected WithEvents Controller As ITextLocalizerApplicationController
    Public Sub InitializeController(ByVal Controller As ITextLocalizerApplicationController) Implements ITextLocalizerPlugin.InitializeController
        Me.Controller = Controller
    End Sub

    ''' <summary>释放托管对象或间接非托管对象(Stream等)。</summary>
    Protected Overridable Sub DisposeManagedResource()
    End Sub

    ''' <summary>释放直接非托管对象(Handle等)。</summary>
    Protected Overridable Sub DisposeUnmanagedResource()
    End Sub

    ''' <summary>将大型字段设置为 null。</summary>
    Protected Overridable Sub DisposeNullify()
    End Sub

    '检测冗余的调用
    Private DisposedValue As Boolean = False
    ''' <summary>释放流的资源。请优先覆盖DisposeManagedResource、DisposeUnmanagedResource、DisposeNullify方法。如果你直接保存非托管对象(Handle等)，请覆盖Finalize方法，并在其中调用Dispose(False)。</summary>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If DisposedValue Then Return
        DisposedValue = True
        If disposing Then
            DisposeManagedResource()
        End If
        DisposeUnmanagedResource()
        DisposeNullify()
    End Sub

    ''' <summary>释放流的资源。</summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

''' <summary>文本风格</summary>
Public Class TextStyle
    Public Index As Integer
    Public Length As Integer
    Public ForeColor As Color
    Public BackColor As Color
End Class

''' <summary>TextLocalizer的高亮插件接口</summary>
Public Interface ITextLocalizerTextHighlighter
    Inherits ITextLocalizerPlugin

    Function GetTextStyles(ByVal TextName As String, ByVal TextIndex As Integer, ByVal FormatedTexts As IEnumerable(Of String)) As IEnumerable(Of TextStyle())
End Interface

''' <summary>TextLocalizer的预览框文本格式化插件接口</summary>
Public Interface ITextLocalizerGridTextFormatter
    Inherits ITextLocalizerPlugin

    Function Format(ByVal TextName As String, ByVal TextIndex As Integer, ByVal FormatedTexts As IEnumerable(Of String)) As IEnumerable(Of String)
End Interface

''' <summary>控件编号</summary>
Public Enum ControlId
    None = 0
    MainWindow = 1
    MainPanel = 2
    Grid = 3
    ToolStrip = 4
End Enum

''' <summary>控件描述</summary>
Public Class ControlDescriptor
    Public Control As Object
    Public Target As ControlId
End Class

''' <summary>TextLocalizer的控件插件接口</summary>
Public Interface ITextLocalizerControlPlugin
    Inherits ITextLocalizerPlugin

    Function GetControlDescriptors() As IEnumerable(Of ControlDescriptor)
End Interface

''' <summary>控件容器</summary>
Public Enum TextLocalizerAction
    None = 0
    RefreshGrid = 1
End Enum

''' <summary>TextLocalizer控制器</summary>
Public Interface ITextLocalizerApplicationController
    Event TextNameChanged(ByVal e As EventArgs)
    Event TextIndexChanged(ByVal e As EventArgs)
    Event ColumnSelectionChanged(ByVal e As EventArgs)
    Event KeyDown(ByVal ControlId As ControlId, ByVal e As KeyEventArgs)
    Event KeyPress(ByVal ControlId As ControlId, ByVal e As KeyPressEventArgs)
    Event KeyUp(ByVal ControlId As ControlId, ByVal e As KeyEventArgs)
    Sub RefreshGrid()
    Sub RefreshColumn(ByVal ColumnIndex As Integer)
    Sub RefreshMainPanel()
    Sub FlushLocalizedText()

    ReadOnly Property Form() As Form
    ReadOnly Property ApplicationName() As String
    Property TextName() As String
    Property TextIndex() As Integer
    Property TextIndices() As IEnumerable(Of Integer)
    Property ColumnIndex() As Integer
    Property SelectionStart() As Integer
    Property SelectionLength() As Integer
    Property Text(ByVal ColumnIndex As Integer) As String
    Sub ScrollToCaret(ByVal ColumnIndex As Integer)
End Interface

''' <summary>TextLocalizer的格式插件接口</summary>
Public Interface ITextLocalizerFormatPlugin
    Inherits ITextLocalizerPlugin

    Function GetTextListFactories() As IEnumerable(Of ILocalizationTextListFactory)
End Interface

''' <summary>TextLocalizer的文本默认值翻译插件接口</summary>
Public Interface ITextLocalizerTranslatorPlugin
    Inherits ITextLocalizerPlugin

    Function TranslateText(ByVal SourceColumn As Integer, ByVal TargeColumn As Integer, ByVal Text As String) As String
End Interface
