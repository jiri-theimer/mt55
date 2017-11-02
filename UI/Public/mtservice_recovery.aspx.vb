Public Class mtservice_recovery
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub cmd1_Click(sender As Object, e As EventArgs) Handles cmd1.Click
        ''Dim f As BL.Factory = New BL.Factory(, "mtservice")

        ''basMemberShip.RecoveryAccount("mtservice", "info@marktime.cz", "EOIUROEIWR12222222.")
        ''Dim sss As String = basMemberShip.RecoveryPassword("mtservice")
        ''Try
        ''    If f.x40MailQueueBL.SendMessageWithoutQueque("info@marktime.cz", "Obnova mtservice účtu proběhla, nové heslo: " & sss, "mtservice recovery") Then
        ''        lblMessage.Text = "Heslo bylo odesláno na MARKTIME servis."
        ''    Else
        ''        lblMessage.Text = f.x40MailQueueBL.ErrorMessage
        ''    End If
        ''Catch ex As Exception
        ''    Response.Write(sss)
        ''End Try
        


    End Sub
End Class