Public Class x50_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x50_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        upload1.Factory = Master.Factory
        upl1.Factory = Master.Factory

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            upload1.GUID = BO.BAS.GetGUID
            upl1.GUID = upload1.GUID
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/help_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Uživatelská nápověda"
            End With

            
            RefreshRecord()

            If Master.IsRecordClone Then Master.DataPID = 0
        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            If Request.Item("page") <> "" Then
                x50AspxPage.Text = Request.Item("page")
                If Left(x50AspxPage.Text, 1) = "/" Then x50AspxPage.Text = BO.BAS.OM1(x50AspxPage.Text, 1)
            End If
            Return
        End If

        Dim cRec As BO.x50Help = Master.Factory.x50HelpBL.Load(Master.DataPID)
        With cRec
            x50Name.Text = .x50Name
            x50ExternalURL.Text = .x50ExternalURL
            x50AspxPage.Text = .x50AspxPage
            BodyHtml.Content = .x50Html

            Master.Timestamp = .Timestamp

        End With

        Master.Factory.o27AttachmentBL.CopyRecordsToTemp(upload1.GUID, cRec.PID, BO.x29IdEnum.x50Help)
        upl1.RefreshData_TEMP()
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x50HelpBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent()
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        upload1.TryUploadhWaitingFilesOnClientSide()
        With Master.Factory.x50HelpBL

            Dim cRec As BO.x50Help = IIf(Master.DataPID = 0, New BO.x50Help, .Load(Master.DataPID))
            With cRec
                .x50Name = x50Name.Text
                .x50ExternalURL = x50ExternalURL.Text
                .x50AspxPage = x50AspxPage.Text

                .x50Html = BodyHtml.Content
                .x50PlainText = BodyHtml.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil

            End With

            If .Save(cRec, upload1.GUID) Then
                Response.Redirect("help.aspx?page=" & cRec.x50AspxPage)

            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With


    End Sub
    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        upl1.RefreshData_TEMP()

    End Sub
End Class