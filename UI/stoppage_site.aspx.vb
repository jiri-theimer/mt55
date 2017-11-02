Public Class stoppage_site
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub stoppage_site_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim s As String = Server.UrlDecode(Request.Item("message"))
            lblMessage.Text = s
            If Request.Item("err") = "1" Then
                img1.ImageUrl = "~/Images/exclaim_32.png"
            Else
                img1.ImageUrl = "~/Images/information_32.png"
                lblMessage.CssClass = "infoNotification"
            End If


           
        End If
    End Sub

End Class