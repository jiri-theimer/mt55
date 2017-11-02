Public Enum x31FormatFlagENUM
    Telerik = 1
    DOCX = 2
    ASPX = 3
    XLSX = 4
End Enum
Public Enum x31QueryFlagENUM
    _None = 0
    p31 = 331
    p41 = 141
    p28 = 328
    p91 = 391
    p56 = 356
    j02 = 102
End Enum
Public Enum x31PluginFlagENUM
    _None = 0
    _AfterEntityMenu = 1
End Enum
Public Class x31Report
    Inherits BOMother
    Public Property x29ID As BO.x29IdEnum
    Public Property x31FormatFlag As x31FormatFlagENUM
    Public Property j25ID As Integer
    Public Property x31Code As String
    Public Property x31Name As String
    Public Property x31Description As String
    Public Property x31Ordinary As Integer
    Public Property x31FileName As String
    
    Public Property x31IsPeriodRequired As Boolean
    Public Property x31IsUsableAsPersonalPage As Boolean
    Public Property x31IsScheduling As Boolean
    Public Property x31SQLSchedulingCondition As String

    Public Property x31IsRunInDay1 As Boolean
    Public Property x31IsRunInDay2 As Boolean
    Public Property x31IsRunInDay3 As Boolean
    Public Property x31IsRunInDay4 As Boolean
    Public Property x31IsRunInDay5 As Boolean
    Public Property x31IsRunInDay6 As Boolean
    Public Property x31IsRunInDay7 As Boolean
    Public Property x31RunInTime As String
    Public Property x31SchedulingReceivers As String
    Public Property x31LastScheduledRun As DateTime?

    Public Property x31DocSqlSource As String
    Public Property x31DocSqlSourceTabs As String
    Public Property x31ExportFileNameMask As String
    Public Property x31QueryFlag As x31QueryFlagENUM = x31QueryFlagENUM._None
    Public Property x31PluginFlag As x31PluginFlagENUM = x31PluginFlagENUM._None
    Public Property x31PluginHeight As Integer

    Private Property _x29Name As String
    Private Property _j25Name As String
    Private Property _j25Ordinary As Integer
    Private Property _o27ID As Integer
    Private Property _ReportFolder As String
    Private Property _ReportFileName As String


    Public ReadOnly Property x29Name As String
        Get
            Return _x29Name
        End Get
    End Property
    Public ReadOnly Property j25Name As String
        Get
            Return _j25Name
        End Get
    End Property
    Public ReadOnly Property j25Ordinary As Integer
        Get
            Return _j25Ordinary
        End Get
    End Property
    Public ReadOnly Property o27ID As Integer
        Get
            Return _o27ID
        End Get
    End Property
    Public ReadOnly Property ReportFileName As String
        Get
            Return _ReportFileName
        End Get
    End Property
    Public ReadOnly Property ReportFolder As String
        Get
            Return _ReportFolder
        End Get
    End Property
    Public ReadOnly Property FormatName As String
        Get
            Select Case Me.x31FormatFlag
                Case x31FormatFlagENUM.ASPX : Return "PLUGIN"
                Case x31FormatFlagENUM.DOCX : Return "DOCX"
                Case x31FormatFlagENUM.Telerik : Return "REPORT"
                Case x31FormatFlagENUM.XLSX : Return "XLSX"
                Case Else : Return ""
            End Select
            
        End Get
    End Property
    Public ReadOnly Property PersonalPageValue As String
        Get
            If Me.PID < 0 Then
                Return _ReportFileName
            Else
                Return Me.PID.ToString
            End If
        End Get
    End Property
    Public ReadOnly Property NameWithCode As String
        Get
            Return Me.x31Name & " [" & Me.x31Code & "]"
        End Get
    End Property
    Public ReadOnly Property NameWithFormat As String
        Get
            Select Case Me.x31FormatFlag
                Case x31FormatFlagENUM.ASPX : Return Me.x31Name & " (PLUGIN)"
                Case x31FormatFlagENUM.DOCX : Return Me.x31Name & " (DOCX)"
                Case x31FormatFlagENUM.XLSX : Return Me.x31Name & " (XLSX)"
                Case Else : Return Me.x31Name
            End Select
        End Get
    End Property

    Public Sub SetPluginUrl(strURL As String, intExplicitJ25Ordinary As Integer)
        _ReportFileName = strURL
        _j25Ordinary = intExplicitJ25Ordinary
    End Sub

    Public ReadOnly Property QueryX29ID As BO.x29IdEnum
        Get
            Select Case Me.x31QueryFlag
                Case x31QueryFlagENUM.p28
                    Return x29IdEnum.p28Contact
                Case x31QueryFlagENUM.p41
                    Return x29IdEnum.p41Project
                Case x31QueryFlagENUM.p31
                    Return x29IdEnum.p31Worksheet
                Case x31QueryFlagENUM.p56
                    Return x29IdEnum.p56Task
                Case x31QueryFlagENUM.p91
                    Return x29IdEnum.p91Invoice
                Case x31QueryFlagENUM.j02
                    Return x29IdEnum.j02Person
                Case Else
                    Return x29IdEnum._NotSpecified
            End Select
        End Get
    End Property
End Class
