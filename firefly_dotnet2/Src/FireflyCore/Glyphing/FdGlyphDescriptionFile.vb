'==========================================================================
'
'  File:        FdGlyphDescriptionFile.vb
'  Location:    Firefly.TextEncoding <Visual Basic .Net>
'  Description: fd字形描述文件
'  Version:     2010.04.08.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports System.IO
Imports Firefly.TextEncoding
Imports Firefly.Imaging

Namespace Glyphing
    Public NotInheritable Class FdGlyphDescriptionFile
        Private Sub New()
        End Sub

        Public Shared Function ReadFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding) As IEnumerable(Of GlyphDescriptor)
            Dim d As New List(Of GlyphDescriptor)
            Using s = Texting.Txt.CreateTextReader(Path, Encoding, True)
                Dim r As New Regex("^U\+(?<Unicode>[0-9A-Fa-f]+)$", RegexOptions.ExplicitCapture)
                Dim LineNumber As Integer = 1
                While Not s.EndOfStream
                    Dim Line = s.ReadLine
                    If Line.Trim <> "" Then
                        Dim Values = Line.Split(","c)
                        If Values.Length <> 10 Then Throw New InvalidDataException(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))

                        Dim Unicodes As New List(Of Char32)
                        If Not Regex.Match(Values(0), "^ *$").Success Then
                            For Each p In Regex.Split(Values(0), " +")
                                Dim m = r.Match(p)
                                If Not m.Success Then Throw New InvalidDataException(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                                Dim Unicode = Integer.Parse(m.Result("${Unicode}"), Globalization.NumberStyles.HexNumber)
                                Unicodes.Add(Unicode)
                            Next
                        End If
                        Dim Code = Values(1)
                        Dim c As StringCode
                        If Code <> "" Then
                            c = New StringCode(Unicodes.ToArray.ToUTF16B, Integer.Parse(Code, Globalization.NumberStyles.HexNumber))
                        Else
                            c = StringCode.FromUniStr(Unicodes.ToArray.ToUTF16B)
                        End If

                        Dim PhysicalBox As New Rectangle(Integer.Parse(Values(2), Globalization.NumberStyles.Integer), Integer.Parse(Values(3), Globalization.NumberStyles.Integer), Integer.Parse(Values(4), Globalization.NumberStyles.Integer), Integer.Parse(Values(5), Globalization.NumberStyles.Integer))
                        Dim VirtualBox As New Rectangle(Integer.Parse(Values(6), Globalization.NumberStyles.Integer), Integer.Parse(Values(7), Globalization.NumberStyles.Integer), Integer.Parse(Values(8), Globalization.NumberStyles.Integer), Integer.Parse(Values(9), Globalization.NumberStyles.Integer))

                        d.Add(New GlyphDescriptor With {.c = c, .PhysicalBox = PhysicalBox, .VirtualBox = VirtualBox})
                    End If
                    LineNumber += 1
                End While
            End Using
            Return d
        End Function
        Public Shared Function ReadFile(ByVal Path As String) As IEnumerable(Of GlyphDescriptor)
            Return ReadFile(Path, TextEncoding.Default)
        End Function
        Public Shared Sub WriteFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding, ByVal GlyphDescriptors As IEnumerable(Of GlyphDescriptor))
            Using s = Texting.Txt.CreateTextWriter(Path, Encoding, True)
                For Each d In GlyphDescriptors
                    Dim Unicode = ""
                    If d.c.HasUnicode Then Unicode = String.Join(" ", (From u In d.c.Unicode.ToUTF32 Select "U+{0:X4}".Formats(u.Value)).ToArray)
                    Dim Code = ""
                    If d.c.HasCode Then Code = d.c.CodeString

                    s.WriteLine(String.Join(",", (From o In New Object() {Unicode, Code, d.PhysicalBox.X, d.PhysicalBox.Y, d.PhysicalBox.Width, d.PhysicalBox.Height, d.VirtualBox.X, d.VirtualBox.Y, d.VirtualBox.Width, d.VirtualBox.Height} Select (o.ToString)).ToArray))
                Next
            End Using
        End Sub
        Public Shared Sub WriteFile(ByVal Path As String, ByVal GlyphDescriptors As IEnumerable(Of GlyphDescriptor))
            WriteFile(Path, TextEncoding.WritingDefault, GlyphDescriptors)
        End Sub

        Public Shared Function ReadFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal ImageReader As IImageReader) As IEnumerable(Of IGlyph)
            Dim GlyphDescriptors = ReadFile(FdPath, Encoding)
            ImageReader.Load()
            Dim l As New List(Of IGlyph)
            For Each d In GlyphDescriptors
                l.Add(New Glyph With {.c = d.c, .Block = ImageReader.GetRectangleAsARGB(d.PhysicalBox.X, d.PhysicalBox.Y, d.PhysicalBox.Width, d.PhysicalBox.Height), .VirtualBox = d.VirtualBox})
            Next
            Return l
        End Function
        Public Shared Function ReadFont(ByVal FdPath As String, ByVal ImageReader As IImageReader) As IEnumerable(Of IGlyph)
            Return ReadFont(FdPath, TextEncoding.Default, ImageReader)
        End Function
        Public Shared Function ReadFont(ByVal FdPath As String) As IEnumerable(Of IGlyph)
            Dim Encoding = TextEncoding.Default
            Dim BmpPath = ChangeExtension(FdPath, "bmp")
            Using ImageReader As New BmpFontImageReader(BmpPath)
                Return ReadFont(FdPath, Encoding, ImageReader)
            End Using
        End Function
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal GlyphDescriptors As IEnumerable(Of GlyphDescriptor), ByVal ImageWriter As IImageWriter, ByVal Width As Integer, ByVal Height As Integer)
            Dim gl = Glyphs.ToArray
            Dim gdl = GlyphDescriptors.ToArray
            If gl.Length <> gdl.Length Then Throw New ArgumentException("GlyphsAndGlyphDescriptorsCountNotMatch")
            Dim PicWidth = Width
            Dim PicHeight = Height

            ImageWriter.Create(PicWidth, PicHeight)
            For GlyphIndex = 0 To gl.Count - 1
                Dim g = gl(GlyphIndex)
                Dim gd = gdl(GlyphIndex)
                Dim x As Integer = gd.PhysicalBox.X
                Dim y As Integer = gd.PhysicalBox.Y
                ImageWriter.SetRectangleFromARGB(x, y, g.Block)
            Next
            WriteFile(FdPath, Encoding, GlyphDescriptors)
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal ImageWriter As IImageWriter, ByVal GlyphArranger As IGlyphArranger, ByVal Width As Integer, ByVal Height As Integer)
            Dim gl = Glyphs.ToArray
            Dim PicWidth = Width
            Dim PicHeight = Height

            Dim GlyphDescriptors = GlyphArranger.GetGlyphArrangement(gl, PicWidth, PicHeight)
            Dim gdl = GlyphDescriptors.ToArray
            If gl.Length <> gdl.Length Then Throw New InvalidOperationException("NumGlyphTooMuch: NumGlyph={0} MaxNumGlyph={1}".Formats(gl.Count, GlyphDescriptors.Count))

            WriteFont(FdPath, Encoding, Glyphs, GlyphDescriptors, ImageWriter, Width, Height)
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal ImageWriter As IImageWriter, ByVal Width As Integer, ByVal Height As Integer)
            Dim gl = Glyphs.ToArray
            Dim PhysicalWidth As Integer = (From g In gl Select (g.PhysicalWidth)).Max
            Dim PhysicalHeight As Integer = (From g In gl Select (g.PhysicalHeight)).Max
            Dim ga As New GlyphArranger(PhysicalWidth, PhysicalHeight)
            Dim PicWidth = Width
            Dim PicHeight = Height

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight)
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal ImageWriter As IImageWriter, ByVal Width As Integer)
            Dim gl = Glyphs.ToArray
            Dim PhysicalWidth As Integer = (From g In gl Select (g.PhysicalWidth)).Max
            Dim PhysicalHeight As Integer = (From g In gl Select (g.PhysicalHeight)).Max
            Dim ga As New GlyphArranger(PhysicalWidth, PhysicalHeight)
            Dim PicWidth = Width
            Dim PicHeight = ga.GetPreferredHeight(gl, Width)

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight)
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Encoding As System.Text.Encoding, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal ImageWriter As IImageWriter)
            Dim gl = Glyphs.ToArray
            Dim PhysicalWidth As Integer = (From g In gl Select (g.PhysicalWidth)).Max
            Dim PhysicalHeight As Integer = (From g In gl Select (g.PhysicalHeight)).Max
            Dim ga As New GlyphArranger(PhysicalWidth, PhysicalHeight)
            Dim Size = ga.GetPreferredSize(gl)
            Dim PicWidth = Size.Width
            Dim PicHeight = Size.Height

            WriteFont(FdPath, Encoding, gl, ImageWriter, ga, PicWidth, PicHeight)
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal BitPerPixel As Integer, ByVal Palette As Int32(), ByVal Quantize As Func(Of Int32, Byte))
            Dim BmpPath = ChangeExtension(FdPath, "bmp")
            Dim Encoding = TextEncoding.WritingDefault
            Using ImageWriter As New BmpFontImageWriter(BmpPath, CShort(BitPerPixel), Palette, Quantize)
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter)
            End Using
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Glyphs As IEnumerable(Of IGlyph), ByVal BitPerPixel As Integer)
            Dim BmpPath = ChangeExtension(FdPath, "bmp")
            Dim Encoding = TextEncoding.WritingDefault
            Using ImageWriter As New BmpFontImageWriter(BmpPath, CShort(BitPerPixel))
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter)
            End Using
        End Sub
        Public Shared Sub WriteFont(ByVal FdPath As String, ByVal Glyphs As IEnumerable(Of IGlyph))
            Dim BmpPath = ChangeExtension(FdPath, "bmp")
            Dim Encoding = TextEncoding.WritingDefault
            Using ImageWriter As New BmpFontImageWriter(BmpPath)
                WriteFont(FdPath, Encoding, Glyphs, ImageWriter)
            End Using
        End Sub
    End Class

    Public Class BmpFontImageReader
        Implements IImageReader

        Private BmpPath As String
        Private b As Bmp

        Public Sub New(ByVal Path As String)
            BmpPath = Path
        End Sub

        Public Sub Load() Implements IImageReader.Load
            If b IsNot Nothing Then Throw New InvalidOperationException
            b = Bmp.Open(BmpPath)
        End Sub
        Public Function GetRectangleAsARGB(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Integer(,) Implements IImageReader.GetRectangleAsARGB
            Return b.GetRectangleAsARGB(x, y, w, h)
        End Function

#Region " IDisposable 支持 "
        ''' <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        Protected Overridable Sub DisposeManagedResource()
            If b IsNot Nothing Then
                b.Dispose()
                b = Nothing
            End If
        End Sub

        ''' <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
        Protected Overridable Sub DisposeUnmanagedResource()
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
        End Sub

        ''' <summary>释放流的资源。</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

    Public Class BmpFontImageWriter
        Implements IImageWriter

        Private BmpPath As String
        Private b As Bmp
        Private BitPerPixel As Integer
        Private Palette As Int32()
        Private Quantize As Func(Of Int32, Byte)

        Public Sub New(ByVal Path As String)
            Me.New(Path, 8)
        End Sub
        Public Sub New(ByVal Path As String, ByVal BitPerPixel As Integer)
            BmpPath = Path
            Me.BitPerPixel = BitPerPixel
            Select Case BitPerPixel
                Case 2
                    Me.BitPerPixel = 4
                    Dim GetGray = Function(ARGB As Int32) CByte((((ARGB And &HFF0000) >> 16) + ((ARGB And &HFF00) >> 8) + (ARGB And &HFF) + 2) \ 3)
                    Dim r = 255 \ ((1 << BitPerPixel) - 1)
                    Palette = (From i In Enumerable.Range(0, 1 << BitPerPixel) Select ConcatBits(CByte(&HFF), 8, CByte(r * i), 8, CByte(r * i), 8, CByte(r * i), 8)).ToArray.Extend(16, 0)
                    Quantize = Function(ARGB As Int32) CByte(GetGray(ARGB) >> (8 - BitPerPixel))
                Case 8
                    Dim GetGray = Function(ARGB As Int32) CByte((((ARGB And &HFF0000) >> 16) + ((ARGB And &HFF00) >> 8) + (ARGB And &HFF) + 2) \ 3)
                    Palette = (From i In Enumerable.Range(0, 1 << BitPerPixel) Select ConcatBits(CByte(&HFF), 8, CByte(i), 8, CByte(i), 8, CByte(i), 8)).ToArray
                    Quantize = GetGray
                Case Is <= 8
                    Dim GetGray = Function(ARGB As Int32) CByte((((ARGB And &HFF0000) >> 16) + ((ARGB And &HFF00) >> 8) + (ARGB And &HFF) + 2) \ 3)
                    Dim r = 255 \ ((1 << BitPerPixel) - 1)
                    Palette = (From i In Enumerable.Range(0, 1 << BitPerPixel) Select ConcatBits(CByte(&HFF), 8, CByte(r * i), 8, CByte(r * i), 8, CByte(r * i), 8)).ToArray
                    Quantize = Function(ARGB As Int32) CByte(GetGray(ARGB) >> (8 - BitPerPixel))
                Case Else
            End Select
        End Sub
        Public Sub New(ByVal Path As String, ByVal BitPerPixel As Integer, ByVal Palette As Int32())
            BmpPath = Path
            Me.BitPerPixel = BitPerPixel
            Me.Palette = Palette
        End Sub
        Public Sub New(ByVal Path As String, ByVal BitPerPixel As Integer, ByVal Palette As Int32(), ByVal Quantize As Func(Of Int32, Byte))
            BmpPath = Path
            Me.BitPerPixel = BitPerPixel
            Me.Palette = Palette
            Me.Quantize = Quantize
        End Sub

        Public Sub Create(ByVal w As Integer, ByVal h As Integer) Implements IImageWriter.Create
            b = New Bmp(BmpPath, w, h, CShort(BitPerPixel))
            If Palette IsNot Nothing Then b.Palette = Palette
        End Sub

        Public Sub SetRectangleFromARGB(ByVal x As Integer, ByVal y As Integer, ByVal a(,) As Integer) Implements IImageWriter.SetRectangleFromARGB
            If Quantize Is Nothing Then
                b.SetRectangleFromARGB(x, y, a)
            Else
                b.SetRectangleFromARGB(x, y, a, Quantize)
            End If
        End Sub

#Region " IDisposable 支持 "
        ''' <summary>释放托管对象或间接非托管对象(Stream等)。可在这里将大型字段设置为 null。</summary>
        Protected Overridable Sub DisposeManagedResource()
            If b IsNot Nothing Then
                b.Dispose()
                b = Nothing
            End If
        End Sub

        ''' <summary>释放直接非托管对象(Handle等)。可在这里将大型字段设置为 null。</summary>
        Protected Overridable Sub DisposeUnmanagedResource()
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
        End Sub

        ''' <summary>释放流的资源。</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean) 中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
