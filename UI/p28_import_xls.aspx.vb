Public Class p28_import_xls
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub p28_import_xls_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Server.ScriptTimeout = 5000
        upload1.Factory = Master.Factory
        upl1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            upload1.GUID = BO.BAS.GetGUID
            upl1.GUID = upload1.GUID
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
            End With
        End If
    End Sub

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll

    End Sub

    Private Sub upload1_AfterUploadOneFile(strFulllPath As String, strErrorMessage As String) Handles upload1.AfterUploadOneFile

        upload1.Visible = False
    End Sub

    Private Sub upload1_ErrorUpload(strFileName As String, strError As String) Handles upload1.ErrorUpload
        Master.Notify("Soubor [" & strFileName & "] nemá platný MS Excel formát.", 2)
        upl1.Visible = True
    End Sub
End Class