Public Class clue_o23_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        rec1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        If cRec.o23IsEncrypted Then Master.StopPage("Obsah dokumentu je zašifrovaný.")
        Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.LoadByX23ID(cRec.x23ID)
        Dim cDisp As BO.o23RecordDisposition = Master.Factory.o23DocBL.InhaleDisposition(cRec, cX18)
        If Not cDisp.ReadAccess Then Master.StopPage("Nemáte přístup k tomuto dokumentu.")

        ph1.Text = cX18.x18Name

        rec1.FillData(cRec, cX18, False)

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.o23Doc, cRec.PID)
        roles1.RefreshData(lisX69, cRec.PID)
        comments1.RefreshData(Master.Factory, BO.x29IdEnum.o23Doc, cRec.PID)
        tags1.RefreshData(cRec.PID)

        panUpload.Visible = False
        If cX18.x18UploadFlag = BO.x18UploadENUM.FileSystemUpload And cRec.o23LockedFlag <> BO.o23LockedTypeENUM.LockAllFiles Then
            Dim mq As New BO.myQueryO27
            mq.Record_x29ID = BO.x29IdEnum.o23Doc
            mq.Record_PID = cRec.PID
            Dim lis As IEnumerable(Of BO.o27Attachment) = Master.Factory.o27AttachmentBL.GetList(mq)
            With Me.filesPreview
                If lis.Count > 0 Then
                    panUpload.Visible = True
                    .Text = BO.BAS.OM2(.Text, lis.Count.ToString)
                    .NavigateUrl = "javascript:file_preview('o23'," & Master.DataPID.ToString & ")"
                End If
            End With
        End If
        
    End Sub

    
End Class