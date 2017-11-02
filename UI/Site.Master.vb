Imports Telerik.Web.UI

Public Class Site
    Inherits System.Web.UI.MasterPage
    Public Property HelpTopicID As String = ""
    Public Property neededPermission As BO.x53PermValEnum
    Public Property NoMenu As Boolean = False

    Public Property _Factory As BL.Factory = Nothing

   

    Public ReadOnly Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
    End Property

    Public Property PageTitle() As String
        Get            
            Return hidPageTitle.Value
        End Get
        Set(ByVal value As String)
            hidPageTitle.Value = "MARKTIME - " & value
        End Set
    End Property
    Public Property SiteMenuValue() As String
        Get
            Return Me.hidSiteMenuValue.Value
        End Get
        Set(ByVal value As String)
            Me.hidSiteMenuValue.Value = value

        End Set
    End Property

    Private Sub DoLogOut()
        Response.Redirect("~/Account/Login.aspx?autologout=1") 'automatické odhlášení
    End Sub

    Public Sub Notify(ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        basUI.NotifyMessage(Me.notify1, strText, msgLevel)
    End Sub

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.User.Identity.IsAuthenticated And _Factory Is Nothing Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            _Factory = New BL.Factory(, strLogin)
            If _Factory.SysUser Is Nothing Then DoLogOut()
            basUI.PingAccessLog(_Factory, Request)

            If _Factory.SysUser.j03IsMustChangePassword Then
                If Request.Url.ToString.ToLower.IndexOf("changepassword") < 0 Then
                    Response.Redirect("ChangePassword.aspx")
                End If
            Else
                If Not _Factory.SysUser.j03PasswordExpiration Is Nothing Then
                    If _Factory.SysUser.j03PasswordExpiration < Now Then
                        If Request.Url.ToString.ToLower.IndexOf("changepassword") < 0 Then Response.Redirect("ChangePassword.aspx")
                    End If
                End If
            End If

            ''PersonalizeMenu()
        End If
        ''Response.Cache.SetCacheability(HttpCacheability.NoCache)
        ''Response.Cache.SetNoStore()
    End Sub

    

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        title1.Text = hidPageTitle.Value
        If Me.SiteMenuValue = "dashboard" Then mm1.MasterPageName = "" 'kvůli searchboxu
        If Not Me.NoMenu Then
            mm1.RefreshData(_Factory, Me.HelpTopicID, Me.SiteMenuValue)
        End If

        ''If _Factory.SysUser.j03IsLiveChatSupport Then
        ''    Dim s As New StringBuilder
        ''    s.AppendLine("<!-- Start of SmartSupp Live Chat script -->")
        ''    s.AppendLine("<script type='text/javascript'>")
        ''    s.AppendLine("var _smartsupp = _smartsupp || {};")
        ''    s.AppendLine("_smartsupp.key = 'f47180c069be93b0e812459452fea73be7887dde';")
        ''    s.AppendLine("window.smartsupp||(function(d) {")
        ''    s.AppendLine("var s,c,o=smartsupp=function(){ o._.push(arguments)};o._=[];")
        ''    s.AppendLine("s=d.getElementsByTagName('script')[0];c=d.createElement('script');")
        ''    s.AppendLine("c.type='text/javascript';c.charset='utf-8';c.async=true;")
        ''    s.AppendLine("c.src='//www.smartsuppchat.com/loader.js';s.parentNode.insertBefore(c,s);")
        ''    s.AppendLine("})(document);")
        ''    s.AppendLine("</script>")
        ''    s.AppendLine("<!-- End of SmartSupp Live Chat script -->")

        ''    ''ScriptManager.RegisterStartupScript(Me.myHead, Me.GetType(), "LiveChat", s.ToString, False)
        ''    myHead.Controls.Add(New LiteralControl(s.ToString))
        ''End If
        ''imgVisitUpgradeInfo.Visible = _Factory.SysUser.j03IsShallReadUpgradeInfo

        'menu1.FindItemByValue("more").Items.FindItemByValue("cmdHelp").NavigateUrl = "javascript:help('" & Request.FilePath & "')"
        ''If Me.HelpTopicID = "" Then
        ''    menu1.FindItemByValue("help").NavigateUrl = "http://www.marktime.net/doc/html"
        ''Else
        ''    menu1.FindItemByValue("help").NavigateUrl = "http://www.marktime.net/doc/html/index.html?" & Me.HelpTopicID & ".htm"
        ''End If
        ''If Not menu1.FindItemByValue("more") Is Nothing Then
        ''    With menu1.FindItemByValue("more").Items.FindItemByValue("cmdHelp")
        ''        .NavigateUrl = menu1.FindItemByValue("help").NavigateUrl
        ''    End With
        ''End If

        ''SetupSearchbox()


    End Sub

  
    Public Sub TestNeededPermission(neededPerm As BO.x53PermValEnum)
        If _Factory Is Nothing Then Return

        If Not _Factory.TestPermission(neededPerm) Then
            StopPage("Nedisponujete dostatečným oprávněním pro zobrazení této stránky.", True)
        End If
    End Sub


    Public Sub StopPage(ByVal strMessage As String, Optional ByVal bolErrorInfo As Boolean = True)
        Server.Transfer("~/stoppage_site.aspx?&err=" & BO.BAS.GB(bolErrorInfo) & "&message=" & Server.UrlEncode(strMessage), False)
    End Sub


   
End Class

