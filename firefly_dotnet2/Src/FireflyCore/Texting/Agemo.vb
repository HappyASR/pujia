'==========================================================================
'
'  File:        Agemo.vb
'  Location:    Firefly.Texting <Visual Basic .Net>
'  Description: Agemo文本格式
'  Version:     2009.10.31.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Firefly.TextEncoding

Namespace Texting
    Public NotInheritable Class Agemo
        Private Sub New()
        End Sub

        Public Shared Function ReadFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding) As String()
            Dim l As New LinkedList(Of String)
            Dim sb As New List(Of Char32)
            Dim NotNull As Boolean = False
            Dim LineNumber As Integer = 1
            Using s = Txt.CreateTextReader(Path, Encoding, True)
                Dim r As New Regex("^#### *(?<index>\d+) *####$", RegexOptions.ExplicitCapture)
                Dim PreIndex As Integer = 0
                While Not s.EndOfStream
                    Dim LineBuilder As New List(Of Char32)
                    Dim LineSeperator As String = ""
                    While Not s.EndOfStream
                        Dim c As Char32 = ChrQ(s.Read())
                        If c = Cr Then
                            If ChrQ(s.Peek) = Lf Then
                                LineSeperator = CrLf
                                s.Read()
                            Else
                                LineSeperator = Cr
                            End If
                            Exit While
                        ElseIf c = Lf Then
                            LineSeperator = Lf
                            Exit While
                        Else
                            LineBuilder.Add(c)
                        End If
                    End While
                    Dim Line = LineBuilder.ToArray
                    Dim Match = r.Match(Line.ToUTF16B)
                    If Match.Success Then
                        NotNull = True
                        RemoveLast(sb, Lf)
                        RemoveLast(sb, Cr)
                        RemoveLast(sb, Lf)
                        RemoveLast(sb, Cr)
                        l.AddLast(sb.ToArray.ToUTF16B)
                        sb = New List(Of Char32)
                        Dim Index As Integer
                        If Not Integer.TryParse(Match.Result("${index}").Trim, Index) Then
                            Throw New InvalidDataException(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                        End If
                        If Index <> PreIndex + 1 Then
                            Throw New InvalidDataException(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                        End If
                        PreIndex = Index
                    Else
                        sb.AddRange(Line)
                        sb.AddRange(LineSeperator.ToUTF32)
                    End If
                    LineNumber += 1
                End While
                If l.Count > 0 Then l.RemoveFirst()
                RemoveLast(sb, Lf)
                RemoveLast(sb, Cr)
                RemoveLast(sb, Lf)
                RemoveLast(sb, Cr)
                If Not NotNull Then
                    If sb.Count > 0 Then Throw New InvalidDataException(String.Format("{0}({1}) : 格式错误或编码错误。", Path, LineNumber))
                    Return New String() {}
                End If
                l.AddLast(sb.ToArray.ToUTF16B)
                Dim ret = New String(l.Count - 1) {}
                l.CopyTo(ret, 0)
                Return ret
            End Using
        End Function
        Public Shared Function VerifyFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding, <System.Runtime.InteropServices.Out()> ByRef LogText As String, Optional ByVal EnforceEncoding As Boolean = False) As Boolean
            Dim Log As New List(Of String)
            Dim sb As New List(Of Char32)
            Dim NotNull As Boolean = False
            Dim LineNumber As Integer = 0
            Using s = Txt.CreateTextReader(Path, Encoding, Not EnforceEncoding)
                Dim r0 As New Regex("^#### *(?<index>\d+) *####", RegexOptions.ExplicitCapture)
                Dim r As New Regex("^#### *(?<index>\d+) *####$", RegexOptions.ExplicitCapture)
                Dim PreIndex As Integer = 0
                While Not s.EndOfStream
                    LineNumber += 1
                    Dim LineBuilder As New List(Of Char32)
                    Dim LineSeperator As String = ""
                    While Not s.EndOfStream
                        Dim c As Char32 = ChrQ(s.Read())
                        If c = Cr Then
                            If ChrQ(s.Peek) = Lf Then
                                LineSeperator = CrLf
                                s.Read()
                            Else
                                LineSeperator = Cr
                            End If
                            Exit While
                        ElseIf c = Lf Then
                            LineSeperator = Lf
                            Exit While
                        Else
                            LineBuilder.Add(c)
                        End If
                    End While
                    Dim Line = LineBuilder.ToArray
                    Dim Match = r.Match(Line.ToUTF16B)
                    If Match.Success Then
                        NotNull = True
                        RemoveLast(sb, Lf)
                        RemoveLast(sb, Cr)
                        RemoveLast(sb, Lf)
                        RemoveLast(sb, Cr)
                        sb = New List(Of Char32)
                        Dim Index As Integer
                        If Not Integer.TryParse(Match.Result("${index}").Trim, Index) Then
                            Log.Add(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                        Else
                            If Index <> PreIndex + 1 Then
                                Log.Add(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                            End If
                            PreIndex = Index
                        End If
                    Else
                        Dim Match0 = r0.Match(Line.ToUTF16B)
                        If Match0.Success Then
                            Dim Index As Integer
                            If Not Integer.TryParse(Match0.Result("${index}").Trim, Index) Then
                                Log.Add(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                            Else
                                Log.Add(String.Format("{0}({1}) : 格式错误。", Path, LineNumber))
                                PreIndex = Index
                            End If
                        Else
                            sb.AddRange(Line)
                            sb.AddRange(LineSeperator.ToUTF32)
                        End If
                    End If
                End While
                RemoveLast(sb, Lf)
                RemoveLast(sb, Cr)
                RemoveLast(sb, Lf)
                RemoveLast(sb, Cr)
                If Not NotNull Then
                    If sb.Count > 0 Then Log.Add(String.Format("{0}({1}) : 格式错误或编码错误。", Path, LineNumber))
                End If
                LogText = String.Join(System.Environment.NewLine, Log.ToArray)
                Return Log.Count = 0
            End Using
        End Function
        Public Shared Sub WriteFile(ByVal Path As String, ByVal Encoding As System.Text.Encoding, ByVal Value As IEnumerable(Of String))
            Using s = Txt.CreateTextWriter(Path, Encoding, True)
                Dim n = 0
                For Each v In Value
                    s.WriteLine(String.Format("#### {0} ####", n + 1))
                    s.WriteLine(v)
                    s.WriteLine()
                    n += 1
                Next
            End Using
        End Sub
        Public Shared Function ReadFile(ByVal Path As String) As String()
            Return ReadFile(Path, TextEncoding.Default)
        End Function
        Public Shared Sub WriteFile(ByVal Path As String, ByVal Value As IEnumerable(Of String))
            WriteFile(Path, TextEncoding.WritingDefault, Value)
        End Sub
        Private Shared Sub RemoveLast(ByVal sb As List(Of Char32), ByVal c As Char32)
            If sb.Count >= 1 AndAlso sb(sb.Count - 1) = c Then sb.RemoveAt(sb.Count - 1)
        End Sub
    End Class
End Namespace
