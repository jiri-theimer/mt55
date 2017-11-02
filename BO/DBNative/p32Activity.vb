Public Enum p32AttendanceFlagENUM
    _None = 0
    HoursOnly = 1
    TimeInterval = 2
End Enum
Public Class p32Activity
    Inherits BOMother
    Public Property p34ID As Integer
    Public Property p95ID As Integer
    Public Property p35ID As Integer
    Public Property x15ID As x15IdEnum = x15IdEnum.Nic
    Public Property p32Name As String
    Public Property p32Code As String
    Public Property p32IsBillable As Boolean
    Public Property p32IsTextRequired As Boolean
    Public Property p32Ordinary As Integer
    Public Property p32Color As String
    Public Property p32Value_Default As Double
    Public Property p32Value_Minimum As Double
    Public Property p32Value_Maximum As Double
    Public Property p32DefaultWorksheetText As String
    Public Property p32HelpText As String

    Public Property p32Name_EntryLang1 As String
    Public Property p32Name_EntryLang2 As String
    Public Property p32Name_EntryLang3 As String
    Public Property p32Name_EntryLang4 As String
    Public Property p32Name_BillingLang1 As String
    Public Property p32Name_BillingLang2 As String
    Public Property p32Name_BillingLang3 As String
    Public Property p32Name_BillingLang4 As String

    Public Property p32DefaultWorksheetText_Lang1 As String
    Public Property p32DefaultWorksheetText_Lang2 As String
    Public Property p32DefaultWorksheetText_Lang3 As String
    Public Property p32DefaultWorksheetText_Lang4 As String

    Public Property p32FreeText01 As String
    Public Property p32FreeText02 As String
    Public Property p32FreeText03 As String

    Public Property p32IsSystemDefault As Boolean
    
    Public Property p32ExternalPID As String
    Public Property p32AttendanceFlag As p32AttendanceFlagENUM = p32AttendanceFlagENUM._None
    Public Property p32ManualFeeFlag As Integer
    Public Property p32ManualFeeDefAmount As Double
    Private Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property
    Private Property _p33ID As Integer
    Public ReadOnly Property p33ID As Integer
        Get
            Return _p33ID
        End Get
    End Property
    Private Property _p34IncomeStatementFlag As Integer
    Public ReadOnly Property p34IncomeStatementFlag As Integer
        Get
            Return _p34IncomeStatementFlag
        End Get
    End Property

    Public ReadOnly Property NameWithSheet As String
        Get
            Return p32Name & " (" & p34Name & ")"
        End Get
    End Property

    Private Property _p95Name As String
    Public ReadOnly Property p95Name As String
        Get
            Return _p95Name
        End Get
    End Property

    Private Property _x15Name As String
    Public ReadOnly Property x15Name As String
        Get
            Return _x15Name
        End Get
    End Property

End Class
