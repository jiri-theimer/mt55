Public Class alertbox
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub Append(strMessage As String, Optional bolRed As Boolean = False)
        Dim s As String = "<span style='color:black;'>" & strMessage & "</span>"
        If bolRed Then
            s = "<span style='color:red;'>" & strMessage & "</span>"
        End If
        If Me.lblContent.Text = "" Then
            lblContent.Text = s
        Else
            lblContent.Text += "<hr>" & s
        End If
        Me.Visible = True
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Me.lblContent.Text = "" Then Me.Visible = False Else Me.Visible = True
    End Sub
End Class