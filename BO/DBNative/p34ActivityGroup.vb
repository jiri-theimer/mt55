Public Enum p34IncomeStatementFlagENUM
    Vydaj = 1
    Prijem = 2
End Enum
Public Enum p34ActivityEntryFlagENUM
    AktivitaSeNezadava = 1
    AktivitaJeNepovinna = 2
    AktivitaJePovinna = 3
End Enum
Public Class p34ActivityGroup
    Inherits BOMother
    Public Property p33ID As p33IdENUM
    Public Property p34Name As String
    Public Property p34IncomeStatementFlag As p34IncomeStatementFlagENUM
    Public Property p34ActivityEntryFlag As p34ActivityEntryFlagENUM
    Public Property p34Code As String
    Public Property p34IsAllow_O27 As Boolean
    Public Property p34IsWorksheetValueHidden As Boolean
    Public Property p34Ordinary As Integer
    Public Property p34Name_EntryLang1 As String
    Public Property p34Name_EntryLang2 As String
    Public Property p34Name_EntryLang3 As String
    Public Property p34Name_EntryLang4 As String
    Public Property p34Name_BillingLang1 As String
    Public Property p34Name_BillingLang2 As String
    Public Property p34Name_BillingLang3 As String
    Public Property p34Name_BillingLang4 As String
    Public Property p34IsRecurrence As Boolean
    Public Property p34IsCapacityPlan As Boolean
    Public Property p34Color As String

    Private Property _p33Name As String
    Private Property _p33Code As String

    Public ReadOnly Property p33Name As String
        Get
            Return _p33Name
        End Get
    End Property
    Public ReadOnly Property p33Code As String
        Get
            Return _p33Code
        End Get
    End Property
End Class
