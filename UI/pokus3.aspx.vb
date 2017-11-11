Public Class pokus3
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strUserLogin As String = "lama@marktime50"
        Dim strCon As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationPrimary").ToString()
        Dim x As Integer = strCon.IndexOf("cloud-db-template")
        If x > 0 Then
            Dim pos As Integer = strUserLogin.IndexOf("@")
            Response.Write("pos = " & pos.ToString)
            Response.Write("<hr>")
            Response.Write("ret: " & strUserLogin.Substring(pos + 1, Len(strUserLogin) - pos - 1))

            strCon = Replace(strCon, "cloud-db-template", strUserLogin.Substring(pos + 1, Len(strUserLogin) - pos - 1), , 1, CompareMethod.Binary)
            Response.Write("<hr>con: " & strCon)


        End If
    End Sub

End Class