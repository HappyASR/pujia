'==========================================================================
'
'  File:        LocalizationProject.vb
'  Location:    Firefly.TextLocalizer <Visual Basic .Net>
'  Description: 本地化项目项目文件
'  Version:     2009.10.08.
'  Copyright(C) F.R.C.
'
'==========================================================================

Imports System
Imports System.Collections.Generic
Imports Firefly
Imports Firefly.Project

Public Class LocalizationProject
    Public TextName As String
    Public TextNumber As Integer
    Public Maximized As Boolean = False
    Public WindowWidth As Integer = 800
    Public WindowHeight As Integer = 600
    Public LocalizationTextBoxDescriptors As LocalizationTextBoxDescriptor()
    Public MainLocalizationTextBox As Integer

    Public EnableLocalizationGrid As Boolean = True
    Public LocalizationGridAutoResizeWidth As Boolean = True
    Public LocalizationGridWidthRatio As Double = 300 / 800
    Public LocalizationRowHeaderWidthRatio As Double = 50 / 300

    Public Plugins As PluginDescriptor()
End Class

Public Class LocalizationTextBoxDescriptor
    Public Name As String
    Public DisplayName As String

    Public HeightRatio As Double = 1 / 3
    Public ColumnWidthRatio As Double = 120 / 300

    Public Directory As String
    Public Extension As String
    Public Type As String
    Public Editable As Boolean
    Public Encoding As String

    Public FontName As String
    Public FontPixel As Integer
    Public Space As Integer
End Class

Public Class PluginDescriptor
    Public AssemblyName As String
    Public Enable As Boolean = True
End Class
