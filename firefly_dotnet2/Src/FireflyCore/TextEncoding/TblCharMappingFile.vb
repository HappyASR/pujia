'==========================================================================
'
'  File:        TblCharMappingFile.vb
'  Location:    Firefly.TextEncoding <Visual Basic .Net>
'  Description: tbl字符映射表文件
'  Version:     2009.11.21.
'  Copyright(C) F.R.C.
'
'==========================================================================

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions

Namespace TextEncoding
    Public NotInheritable Class TblCharMappingFile
        Private Sub New()
        End Sub

        Public Shared Function ReadRaw(ByVal Path As String, ByVal Encoding As System.Text.Encoding) As IEnumerable(Of KeyValuePair(Of String, String))
            Dim d As New List(Of KeyValuePair(Of String, String))
            Using s = Texting.Txt.CreateTextReader(Path, Encoding, True)
                Dim r As New Regex("^(?<left>.*?)=(?<right>.*)$", RegexOptions.ExplicitCapture)
                While Not s.EndOfStream
                    Dim Line = s.ReadLine
                    Dim Match = r.Match(Line)
                    If Not Match.Success Then Continue While
                    Dim Left As String = Match.Result("${left}")
                    Dim Right As String = Match.Result("${right}")
                    d.Add(New KeyValuePair(Of String, String)(Left, Right))
                End While
            End Using
            Return d
        End Function
        Public Shared Function ReadRaw(ByVal Path As String) As IEnumerable(Of KeyValuePair(Of String, String))
            Return ReadRaw(Path, TextEncoding.Default)
        End Function
        Public Shared Function ReadFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding) As IEnumerable(Of StringCode)
            Dim d As New List(Of StringCode)
            Using s = Texting.Txt.CreateTextReader(Path, Encoding, True)
                Dim r As New Regex("^(?<left>.*?)=(?<right>.*)$", RegexOptions.ExplicitCapture)
                While Not s.EndOfStream
                    Dim Line = s.ReadLine
                    Dim Match = r.Match(Line)
                    If Not Match.Success Then Continue While
                    Dim Left As String = Match.Result("${left}").Trim(" "c)
                    Dim c = StringCode.FromNothing
                    If Left <> "" Then
                        c.CodeString = Left
                    End If
                    Dim Right As String = Match.Result("${right}")
                    If Right.Trim(" "c).Length >= 1 Then Right = Right.Trim(" "c)
                    If Right.Trim(" "c).Length >= 2 Then Right = Right.Descape
                    If Right <> "" Then
                        c.Unicode = Right
                    End If
                    d.Add(c)
                End While
            End Using
            Return d
        End Function
        Public Shared Function ReadFile(ByVal Path As String) As IEnumerable(Of StringCode)
            Return ReadFile(Path, TextEncoding.Default)
        End Function
        Public Shared Function ReadAsEncoding(ByVal Path As String, ByVal Encoding As System.Text.Encoding) As Encoding
            Return New MultiByteEncoding(ReadFile(Path, Encoding))
        End Function
        Public Shared Function ReadAsEncoding(ByVal Path As String) As Encoding
            Return New MultiByteEncoding(ReadFile(Path, TextEncoding.Default))
        End Function
        Public Shared Sub WriteRaw(ByVal Path As String, ByVal Encoding As System.Text.Encoding, ByVal l As IEnumerable(Of KeyValuePair(Of String, String)))
            Using s = Texting.Txt.CreateTextWriter(Path, Encoding, True)
                For Each p In l
                    s.WriteLine(p.Key & "=" & p.Value)
                Next
            End Using
        End Sub
        Public Shared Sub WriteRaw(ByVal Path As String, ByVal l As IEnumerable(Of KeyValuePair(Of String, String)))
            WriteRaw(Path, TextEncoding.WritingDefault, l)
        End Sub
        Public Shared Sub WriteFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding, ByVal l As IEnumerable(Of StringCode))
            Using s = Texting.Txt.CreateTextWriter(Path, Encoding, True)
                For Each p In l
                    Dim Character = ""
                    If p.HasUnicode Then
                        Character = p.Character
                        Character = Character.Escape
                    End If
                    s.WriteLine(p.CodeString & "=" & Character)
                Next
            End Using
        End Sub
        Public Shared Sub WriteFile(ByVal Path As String, ByVal l As IEnumerable(Of StringCode))
            WriteFile(Path, TextEncoding.WritingDefault, l)
        End Sub
    End Class
End Namespace
