Public Class bare
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub bare_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "DB Backup Repository"
                .SiteMenuValue = "admin_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_Admin)
            End With

            Dim cF As New BO.clsFile
            Dim strDir As String = BO.ASS.GetConfigVal("baredir")
            If strDir = "" Then
                Master.StopPage("baredir key is missing.")
            End If
            Dim lis As List(Of String) = cF.GetFileListFromDir(strDir, "*.bak")
            rp1.DataSource = lis
            rp1.DataBind()

            If lis.Count = 0 Then
                Master.Notify("Žádný BAK soubor (" & strDir & ")", NotifyLevel.WarningMessage)
            End If
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim strPath As String = e.Item.DataItem
        With CType(e.Item.FindControl("link1"), HyperLink)
            .NavigateUrl = "binaryfile.aspx?backupfile=" & strPath
            .Text = strPath
        End With
    End Sub
End Class