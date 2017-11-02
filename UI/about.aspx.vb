Public Class About
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            lblVersion.Text = "Verze: " & BO.ASS.GetUIVersion() & " | .NET framework: " & BO.ASS.GetFrameworkVersion()

        End If
    End Sub

End Class