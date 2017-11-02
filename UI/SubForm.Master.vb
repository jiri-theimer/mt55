Public Class SubForm
    Inherits System.Web.UI.MasterPage
    Private Property _Factory As BL.Factory = Nothing
    Private _quickDataPID As Integer = 0
    Public ReadOnly Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
    End Property
    Public Property HeaderText() As String
        Get
            Return hidPageTitle.Value
        End Get
        Set(ByVal value As String)
            hidPageTitle.Value = value
            pageTitle.Text = value

        End Set
    End Property
    Public Property DataPID() As Integer
        Get
            If _quickDataPID <> 0 Then Return _quickDataPID
            Return BO.BAS.IsNullInt(hidDataPID.Value)
        End Get
        Set(ByVal value As Integer)
            _quickDataPID = value
            hidDataPID.Value = value.ToString
        End Set
    End Property
    Public Property HelpTopicID As String
    Public Property SiteMenuValue() As String
        Get
            Return Me.hidSiteMenuValue.Value
        End Get
        Set(ByVal value As String)
            Me.hidSiteMenuValue.Value = value
        End Set
    End Property
    Public Property IsHideAllRecZooms As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsHideAllRecZooms.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsHideAllRecZooms.Value = BO.BAS.GB(value)
        End Set
    End Property
   

    Public Sub StopPage(ByVal strMessage As String, Optional ByVal bolErrorInfo As Boolean = True, Optional ByVal strNeededPerms As String = "", Optional bolModalPage As Boolean = False)
        Server.Transfer("~/stoppage.aspx?err=" & BO.BAS.GB(bolErrorInfo) & "&message=" & Server.UrlEncode(strMessage) & "&neededperms=" & strNeededPerms & "&modal=" & BO.BAS.GB(bolModalPage), False)

    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.User.Identity.IsAuthenticated And _Factory Is Nothing Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            _Factory = New BL.Factory(, strLogin)
            If _Factory.SysUser Is Nothing Then DoLogOut()
            basUI.PingAccessLog(_Factory, Request)
        End If
        If Not Page.IsPostBack Then
            hidSource.Value = Request.Item("source")
        End If
        
    End Sub
    Private Sub DoLogOut()
        Response.Redirect("~/Account/Login.aspx?autologout=1") 'automatické odhlášení
    End Sub
    Public Sub Notify(ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        basUI.NotifyMessage(Me.notify1, strText, msgLevel)
    End Sub
    

    
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If hidSource.Value = "3" Then
            Dim newControl As UserControl = LoadControl("~/main_menu.ascx")
            newControl.EnableViewState = False
            place1.Controls.Add(newControl)
            CType(newControl, UI.main_menu).RefreshData(_Factory, Me.HelpTopicID, Me.SiteMenuValue)

        End If
    End Sub

    
End Class