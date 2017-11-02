Public Class Clue
    Inherits System.Web.UI.MasterPage
    Private Property _Factory As BL.Factory = Nothing

    Public ReadOnly Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
    End Property
    Public Property DataPID() As Integer
        Get
            Return BO.BAS.IsNullInt(hidDataPID.Value)
        End Get
        Set(ByVal value As Integer)
            hidDataPID.Value = value.ToString
        End Set
    End Property
    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.User.Identity.IsAuthenticated And _Factory Is Nothing Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            _Factory = New BL.Factory(, strLogin)
            If _Factory.SysUser Is Nothing Then StopPage(" _Factory.SysUser Is Nothing")
        End If
    End Sub
    Public Sub StopPage(ByVal strMessage As String, Optional ByVal bolErrorInfo As Boolean = True, Optional ByVal strNeededPerms As String = "", Optional bolModalPage As Boolean = False)
        Server.Transfer("~/stoppage.aspx?err=" & BO.BAS.GB(bolErrorInfo) & "&message=" & Server.UrlEncode(strMessage) & "&neededperms=" & strNeededPerms & "&modal=" & BO.BAS.GB(bolModalPage), False)

    End Sub

    Public Property HeaderText() As String
        Get
            Return hidPageTitle.Value
        End Get
        Set(ByVal value As String)
            hidPageTitle.Value = value
            pageTitle.Text = value

        End Set
    End Property

    Public Sub Notify(ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        Me.lblNotifyMessage.Text = strText
        Select Case msgLevel
            Case NotifyLevel.InfoMessage
                Me.lblNotifyMessage.CssClass = "valboldblue"
            Case NotifyLevel.WarningMessage
                Me.lblNotifyMessage.CssClass = "valboldred"
        End Select


    End Sub
End Class