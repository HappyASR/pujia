'==========================================================================
'
'  File:        StringCode.vb
'  Location:    Firefly.TextEncoding <Visual Basic .Net>
'  Description: 字符串码点信息
'  Version:     2009.11.26.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace TextEncoding

    ''' <summary>字符串码点值对，可用于码点转换。</summary>
    <DebuggerDisplay("{ToString()}")> _
    Public Class StringCode
        Implements IEquatable(Of StringCode)

        ''' <summary>Unicode字符串。</summary>
        Public Unicode As String = ""
        ''' <summary>码点形式的自定义码点。</summary>
        Public Code As Int32 = -1
        ''' <summary>自定义码点的字节长度。</summary>
        Public CodeLength As Integer = -1

        ''' <summary>已重载。创建字符码点值对的实例。</summary>
        Public Sub New()
            Me.Unicode = ""
            Me.Code = -1
            Me.CodeLength = 0
        End Sub
        ''' <summary>已重载。创建字符码点值对的实例。</summary>
        ''' <param name="UniStr">Unicode字符串，空字符串表示不存在。</param>
        ''' <param name="Code">自定义码点，-1表示不存在。</param>
        ''' <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal UniStr As String, ByVal Code As Int32, Optional ByVal CodeLength As Integer = -1)
            Me.Unicode = UniStr
            Me.Code = Code
            If CodeLength < -1 OrElse CodeLength > 4 Then Throw New ArgumentException
            Me.CodeLength = CodeLength
        End Sub

        ''' <summary>已重载。创建字符码点值对的实例。</summary>
        ''' <param name="UniChar">Unicode字符，-1表示不存在。</param>
        ''' <param name="Code">自定义码点，-1表示不存在。</param>
        ''' <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal UniChar As Char32, ByVal Code As Int32, Optional ByVal CodeLength As Integer = -1)
            If UniChar = -1 Then
                Me.Unicode = ""
            Else
                Me.Unicode = CStr(UniChar)
            End If
            Me.Code = Code
            If CodeLength < -1 OrElse CodeLength > 4 Then Throw New ArgumentException
            Me.CodeLength = CodeLength
        End Sub

        ''' <summary>创建字符码点值对的实例。</summary>
        Public Shared Function FromNothing() As StringCode
            Return New StringCode("", -1, 0)
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="UniStr">Unicode字符串。</param>
        Public Shared Function FromUniStr(ByVal UniStr As String) As StringCode
            Return New StringCode(UniStr, -1, 0)
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="UniChar">Unicode字符。</param>
        Public Shared Function FromUniChar(ByVal UniChar As Char32) As StringCode
            Return New StringCode(UniChar, -1, 0)
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="Unicode">Unicode码。</param>
        Public Shared Function FromUnicode(ByVal Unicode As Int32) As StringCode
            Return New StringCode(New Char32(Unicode), -1, 0)
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="Code">自定义码点，-1表示不存在。</param>
        ''' <param name="CodeLength">自定义码点的字节长度，只能为-1、0、1、2、3、4。其中-1表示不明确，0表示码点不存在。</param>
        Public Shared Function FromCode(ByVal Code As Int32, Optional ByVal CodeLength As Integer = -1) As StringCode
            Return New StringCode("", Code, CodeLength)
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="CodeString">自定义码点的字符串形式，""表示不存在。</param>
        Public Shared Function FromCodeString(ByVal CodeString As String) As StringCode
            If CodeString = "" Then
                Return New StringCode("", -1, 0)
            Else
                Return New StringCode("", Integer.Parse(CodeString, Globalization.NumberStyles.HexNumber), (CodeString.Length + 1) \ 2)
            End If
        End Function

        ''' <summary>创建字符码点值对的实例。</summary>
        ''' <param name="CharCode">CharCode字符串。</param>
        Public Shared Function FromCharCode(ByVal CharCode As CharCode) As StringCode
            Return New StringCode(CharCode.Unicode, CharCode.Code, CharCode.CodeLength)
        End Function

        ''' <summary>字符。</summary>
        Public Property Character() As String
            Get
                If HasUnicode Then Return Unicode
                Throw New InvalidOperationException
            End Get
            Set(ByVal Value As String)
                Unicode = Value
            End Set
        End Property

        ''' <summary>指示是否是控制符。</summary>
        Public Overridable ReadOnly Property IsControlChar() As Boolean
            Get
                If HasUnicode Then
                    Dim Str32 = Unicode.ToUTF32
                    If Str32.Length = 1 Then
                        Return Str32(0) >= 0 AndAlso Str32(0) <= &H1F
                    End If
                End If
                Return False
            End Get
        End Property

        ''' <summary>指示是否是换行符。</summary>
        Public Overridable ReadOnly Property IsNewLine() As Boolean
            Get
                Return Unicode = Lf
            End Get
        End Property

        ''' <summary>指示是否已建立映射。</summary>
        Public ReadOnly Property IsCodeMappable() As Boolean
            Get
                Return HasUnicode AndAlso HasCode
            End Get
        End Property

        ''' <summary>指示Unicode是否存在。</summary>
        Public ReadOnly Property HasUnicode() As Boolean
            Get
                Return Unicode <> ""
            End Get
        End Property

        ''' <summary>指示自定义码点是否存在。</summary>
        Public ReadOnly Property HasCode() As Boolean
            Get
                Return CodeLength <> 0
            End Get
        End Property

        ''' <summary>生成显示用字符串。</summary>
        Public Overrides Function ToString() As String
            Dim List As New List(Of String)
            If HasUnicode Then
                List.Add(String.Join(" ", (From c In Unicode.ToUTF32 Select String.Format("U+{0:X4}", c.Value)).ToArray))
                If Not IsControlChar Then List.Add(String.Format("""{0}""", Unicode))
            End If
            If HasCode Then List.Add(String.Format("Code = {0}", CodeString()))

            Return "StringCode{" & String.Join(", ", List.ToArray) & "}"
        End Function

        ''' <summary>自定义码点的字符串形式。</summary>
        Public Property CodeString() As String
            Get
                Select Case CodeLength
                    Case Is > 0
                        Return Code.ToString("X" & (CodeLength * 2))
                    Case 0
                        Return ""
                    Case Else
                        Select Case Code
                            Case 0 To &HFF
                                Return Code.ToString("X2")
                            Case &H100 To &HFFFF
                                Return Code.ToString("X4")
                            Case &H10000 To &HFFFFFF
                                Return Code.ToString("X6")
                            Case Else
                                Return Code.ToString("X8")
                        End Select
                End Select
            End Get
            Set(ByVal Value As String)
                If Value = "" Then
                    Code = -1
                    CodeLength = 0
                Else
                    Code = Integer.Parse(Value, Globalization.NumberStyles.HexNumber)
                    CodeLength = (Value.Length + 1) \ 2
                End If
            End Set
        End Property

        ''' <summary>比较两个字符码点是否相等。</summary>
        Public Overloads Function Equals(ByVal other As StringCode) As Boolean Implements System.IEquatable(Of StringCode).Equals
            Return Unicode = other.Unicode AndAlso Code = other.Code
        End Function

        ''' <summary>比较两个字符码点是否相等。</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Me Is obj Then Return True
            If obj Is Nothing Then Return False
            Dim c = TryCast(obj, StringCode)
            If c Is Nothing Then Return False
            Return Equals(c)
        End Function

        ''' <summary>获取字符码点的HashCode。</summary>
        Public Overrides Function GetHashCode() As Integer
            Return Unicode.GetHashCode() Xor ((Code << 16) Or ((Code >> 16) And &HFFFF))
        End Function

        ''' <summary>转换CharCode到StringCode。</summary>
        Public Shared Widening Operator CType(ByVal c As CharCode) As StringCode
            Return New StringCode(c.Unicode, c.Code, c.CodeLength)
        End Operator
    End Class

    ''' <summary>字符码点值对字符串。</summary>
    Public Module StringCodeString
        ''' <summary>转换CharCode()到StringCode()。</summary>
        Public Function FromCharCodeString(ByVal s As CharCode()) As StringCode()
            Dim StringCodes = New StringCode(s.Length - 1) {}
            For n = 0 To s.Length - 1
                StringCodes(n) = StringCode.FromCharCode(s(n))
            Next
            Return StringCodes
        End Function

        ''' <summary>转换UTF-32字符串到StringCode()。</summary>
        Public Function FromString32(ByVal s As Char32()) As StringCode()
            Dim StringCodes = New StringCode(s.Length - 1) {}
            For n = 0 To s.Length - 1
                StringCodes(n) = StringCode.FromUniChar(s(n))
            Next
            Return StringCodes
        End Function

        ''' <summary>转换UTF-16 Big-Endian字符串到UTF-32字符串。</summary>
        Public Function FromString16(ByVal s As String) As StringCode()
            Dim cl As New List(Of StringCode)

            For n As Integer = 0 To s.Length - 1
                Dim c As Char = s(n)
                Dim H As Int32 = AscQ(c)
                If H >= &HD800 AndAlso H <= &HDBFF Then
                    cl.Add(StringCode.FromUniChar(Char32.FromString(c & s(n + 1))))
                    n += 1
                Else
                    cl.Add(StringCode.FromUniChar(c))
                End If
            Next

            Return cl.ToArray
        End Function

        ''' <summary>转换UTF-32字符串到UTF-16 Big-Endian字符串。</summary>
        <Extension()> Public Function ToString16(ByVal s As StringCode()) As String
            Dim sb As New StringBuilder

            For Each c In s
                If Not c.HasUnicode Then Throw New InvalidDataException
                sb.Append(c.Unicode.ToString)
            Next

            Return sb.ToString
        End Function
    End Module
End Namespace
