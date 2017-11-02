Public Class log_app_update_public
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim cF As New BO.clsFile
            Dim strPath As String = BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\log_app_update.txt"
            
            Dim s As String = cF.GetFileContents(strPath, , False)
            s = s.Replace(Chr(13) + Chr(10), "</br>")
            s = s.Replace("|---------", "<div style='color:red;font-weight:bold;'> ........ ")
            s = s.Replace("---------|", " ................................................................................................................ </div>")
            s = s.Replace("|*********", "<div style='color:blue;padding-left:10px;'>")
            s = s.Replace("*********|", "</div>")
            place1.Controls.Add(New LiteralControl(s))



        End If
    End Sub

End Class