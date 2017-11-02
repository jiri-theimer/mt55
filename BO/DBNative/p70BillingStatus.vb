Public Enum p70IdENUM
    ViditelnyOdpis = 2
    SkrytyOdpis = 3
    Vyfakturovano = 4
    ZahrnutoDoPausalu = 6
    Nic = 0
End Enum
Public Class p70BillingStatus
    Inherits BOMother
    Public Property p70Name As String
    Public Property p70Code As String
    Public Property p70Name_BillingLang1 As String
    Public Property p70Name_BillingLang2 As String
    Public Property p70Name_BillingLang3 As String
    Public Property p70Name_BillingLang4 As String

    Private Property _p70ID As p70IdENUM
    Public ReadOnly Property p70ID As p70IdENUM
        Get
            Return _p70ID
        End Get
    End Property
    Public Sub SetStatus(status As p70IdENUM)
        _p70ID = status
    End Sub
    'Public Sub New(status As p70IdENUM)
    '    _p70ID = status
    'End Sub

    Public ReadOnly Property Color As String
        Get
            Select Case Me.p70ID
                Case p70IdENUM.ViditelnyOdpis : Return "red"
                Case p70IdENUM.SkrytyOdpis : Return "brown"
                Case p70IdENUM.Vyfakturovano : Return "green"
                Case p70IdENUM.ZahrnutoDoPausalu : Return "pink"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class
