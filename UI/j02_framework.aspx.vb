Public Class j02_framework
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Server.Transfer("entity_framework.aspx?prefix=j02&" & basUI.GetCompleteQuerystring(Request), False)
        Server.Transfer(basUI.GetPageUrlPerSAW(Request, "j02"), False)
    End Sub

End Class