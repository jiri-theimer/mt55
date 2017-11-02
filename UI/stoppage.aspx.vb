Public Class stoppage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            

            Dim s As String = Server.UrlDecode(Request.Item("message"))
            lblMessage.Text = s
            If Request.Item("neededperms") <> "" Then
                Me.lblNeededPerms.Text = String.Format("Kód požadovaného oprávnění: {0}", Request.Item("neededperms"))
            End If

            If Request.Item("err") = "1" Then
                img1.ImageUrl = "~/Images/exclaim_32.png"
            Else
                img1.ImageUrl = "~/Images/information_32.png"
                lblMessage.CssClass = "infoNotification"
            End If

           
            If Request.Item("modal") = "0" Then
                Master.RadToolbar.Visible = False

            End If

            If lblMessage.Text = "" Then img1.Visible = False
        End If
    End Sub

End Class