Public Class o23_framework
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Server.Transfer(basUI.GetPageUrlPerSAW(Request, "o23"), False)
    End Sub

End Class