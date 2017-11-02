Public Class p91_framework
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
       
        'Server.Transfer("entity_framework.aspx?prefix=p91&" & basUI.GetCompleteQuerystring(Request), False)
        Server.Transfer(basUI.GetPageUrlPerSAW(Request, "p91"), False)
    End Sub

End Class