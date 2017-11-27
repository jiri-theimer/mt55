Public Class p41_framework
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Server.Transfer(basUI.GetPageUrlPerSAW(Request, "p41"), False)
        
    End Sub

End Class