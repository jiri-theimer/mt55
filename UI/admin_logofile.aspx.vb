Public Class admin_logofile
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub admin_logofile_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderText = "Nahrát grafické logo"
                .HeaderIcon = "Images/setting_32.png"
                .AddToolbarButton("Nahrát logo na server", "ok", , "Images/ok.png")
            End With
            
            Dim cF As New BO.clsFile
            imgLogoPreview.DataValue = cF.GetBinaryContent(basUIMT.GetLogoPath(Master.Factory.SysUser.j03Login, False))


        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            For Each invalidFile As Telerik.Web.UI.UploadedFile In upload1.InvalidFiles
                Master.Notify(String.Format("Soubor [{0}] nelze použít jako grafické logo. Podmínky: png formát, maximální šířka 600px, maximální výška 200px.", invalidFile.FileName), 2)
            Next
            If upload1.UploadedFiles.Count = 0 Then
                Return
            End If

            For Each validFile As Telerik.Web.UI.UploadedFile In upload1.UploadedFiles
                Dim cF As New BO.clsFile
                Dim strFileName As String = "company_logo.png"
                If BO.ASS.GetConfigVal("cloud", "0") = "1" Then
                    strFileName = BO.BAS.ParseDbNameFromCloudLogin(Master.Factory.SysUser.j03Login) & ".png"
                End If

                Dim strDestFullPath As String = BO.ASS.GetApplicationRootFolder & "\Plugins\" & strFileName

                
                Dim strTemp As String = Master.Factory.x35GlobalParam.TempFolder & "\" & validFile.FileName

                Try
                    validFile.SaveAs(strTemp, True)
                    Dim img As System.Drawing.Image = System.Drawing.Image.FromFile(strTemp)
                    If img.Width > 600 Or img.Height > 200 Then
                        Master.Notify("Podmínky: PNG formát, maximální šířka 600px, maximální výška 200px.", NotifyLevel.InfoMessage)
                        Return
                    End If
                    If cF.CopyFile(strTemp, strDestFullPath) Then
                        Dim lis As IEnumerable(Of BO.p93InvoiceHeader) = Master.Factory.p93InvoiceHeaderBL.GetList(New BO.myQuery)  'uložit logo do hlavičky vystavovatele faktury
                        For Each c In lis
                            c.p93LogoFile = strFileName
                            Master.Factory.p93InvoiceHeaderBL.Save(c, Nothing)
                        Next
                        Master.CloseAndRefreshParent("logo")
                    End If

                Catch ex As Exception
                    Master.Notify(ex.Message, NotifyLevel.ErrorMessage)
                    Return
                End Try
            Next
        End If
    End Sub
End Class