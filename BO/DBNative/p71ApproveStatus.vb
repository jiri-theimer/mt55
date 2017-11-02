Public Enum p71IdENUM
    Schvaleno = 1
    Neschvaleno = 2
    Nic = 0
End Enum
Public Class p71ApproveStatus
    Inherits BOMother
    Public Property p71Name As String
    Public Property p71Code As String
    Public Property p71Name_BillingLang1 As String
    Public Property p71Name_BillingLang2 As String
    Public Property p71Name_BillingLang3 As String
    Public Property p71Name_BillingLang4 As String

    Private Property _p71ID As p71IdENUM
    Public ReadOnly Property p71ID As p71IdENUM
        Get
            Return _p71ID
        End Get
    End Property
End Class
