Public Enum p72IdENUM
    ViditelnyOdpis = 2
    SkrytyOdpis = 3
    Fakturovat = 4
    ZahrnoutDoPausalu = 6
    FakturovatPozdeji = 7
    _NotSpecified = 0
End Enum
Public Class p72PreBillingStatus
    Inherits BOMother
    Public Property p72Name As String
    Public Property p72Code As String
    Public Property p72Name_BillingLang1 As String
    Public Property p72Name_BillingLang2 As String
    Public Property p72Name_BillingLang3 As String
    Public Property p72Name_BillingLang4 As String

    Private Property _p72ID As p72IdENUM
    Public ReadOnly Property p72ID As p72IdENUM
        Get
            Return _p72ID
        End Get
    End Property
    'Public Sub New(status As p72IdENUM)
    '    _p72ID = status
    'End Sub
    Public Sub SetStatus(status As p72IdENUM)
        _p72ID = status
    End Sub
    Public ReadOnly Property ImageUrl As String
        Get
            Select Case Me.p72ID
                Case p72IdENUM.ViditelnyOdpis : Return "Images/a12.gif"
                Case p72IdENUM.SkrytyOdpis : Return "Images/a13.gif"
                Case p72IdENUM.Fakturovat : Return "Images/a14.gif"
                Case p72IdENUM.ZahrnoutDoPausalu : Return "Images/a16.gif"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class
