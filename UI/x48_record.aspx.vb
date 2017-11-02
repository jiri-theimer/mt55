Public Class x48_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x48_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Sql úloha"

            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            RefreshX31Combo()
            Return
        End If

        Dim cRec As BO.x48SqlTask = Master.Factory.x48SqlTaskBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record is missing.")
        cmdClearLastScheduledRun.Visible = True

        With cRec
            Master.HeaderText = .x48Name
            Me.x48Name.Text = .x48Name
            Me.x48Ordinary.Value = .x48Ordinary
            basUI.SelectRadiolistValue(Me.x48TaskOutputFlag, CInt(.x48TaskOutputFlag).ToString)

            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            RefreshX31Combo()
            basUI.SelectDropdownlistValue(Me.x31ID, .x31ID.ToString)
            
            Me.x48Sql.Text = .x48Sql
            Me.x48MailBody.Text = .x48MailBody
            Me.x48MailSubject.Text = .x48MailSubject
            Me.x48MailTo.Text = .x48MailTo
            
            Me.x48RunInTime.Text = .x48RunInTime
            Me.x48IsRunInDay1.Checked = .x48IsRunInDay1
            Me.x48IsRunInDay2.Checked = .x48IsRunInDay2
            Me.x48IsRunInDay3.Checked = .x48IsRunInDay3
            Me.x48IsRunInDay4.Checked = .x48IsRunInDay4
            Me.x48IsRunInDay5.Checked = .x48IsRunInDay5
            Me.x48IsRunInDay6.Checked = .x48IsRunInDay6
            Me.x48IsRunInDay7.Checked = .x48IsRunInDay7
            Me.x48LastScheduledRun.Text = BO.BAS.FD(.x48LastScheduledRun, True, True)

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x48SqlTaskBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x48-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x48SqlTaskBL
            Dim cRec As BO.x48SqlTask = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x48SqlTask)

            With cRec
                .x48TaskOutputFlag = Me.x48TaskOutputFlag.SelectedValue
                .x29ID = DirectCast(BO.BAS.IsNullInt(Me.x29ID.SelectedValue), BO.x29IdEnum)
                .x31ID = BO.BAS.IsNullInt(Me.x31ID.SelectedValue)
                .x48Name = Me.x48Name.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .x48Ordinary = BO.BAS.IsNullInt(Me.x48Ordinary.Value)
                .x48Sql = Me.x48Sql.Text
                
                .x48IsRunInDay1 = Me.x48IsRunInDay1.Checked
                .x48IsRunInDay2 = Me.x48IsRunInDay2.Checked
                .x48IsRunInDay3 = Me.x48IsRunInDay3.Checked
                .x48IsRunInDay4 = Me.x48IsRunInDay4.Checked
                .x48IsRunInDay5 = Me.x48IsRunInDay5.Checked
                .x48IsRunInDay6 = Me.x48IsRunInDay6.Checked
                .x48IsRunInDay7 = Me.x48IsRunInDay7.Checked
                .x48RunInTime = Me.x48RunInTime.Text

                .x48MailBody = Me.x48MailBody.Text
                .x48MailSubject = Me.x48MailSubject.Text
                .x48MailTo = Me.x48MailTo.Text
            End With
           
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x48-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub RefreshX31Combo()
        Dim lis As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(New BO.myQuery).Where(Function(p) p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik)
        Select Case Me.x29ID.SelectedValue
            Case "102"
                lis = lis.Where(Function(p) p.x29ID = BO.x29IdEnum.j02Person)
            Case "141"
                lis = lis.Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project)
            Case "328"
                lis = lis.Where(Function(p) p.x29ID = BO.x29IdEnum.p28Contact)
            Case Else
                lis = lis.Where(Function(p) p.x29ID = BO.x29IdEnum._NotSpecified)
        End Select
        Me.x31ID.DataSource = lis
        Me.x31ID.DataBind()
        Me.x31ID.Items.Insert(0, "")
    End Sub

    Private Sub x48_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.x48TaskOutputFlag.SelectedValue = "1" Then
            Me.x29ID.Visible = False
        Else
            Me.x29ID.Visible = True
        End If
        Me.lblX29ID.Visible = Me.x29ID.Visible
    End Sub

    Private Sub x29ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x29ID.SelectedIndexChanged
        RefreshX31Combo()
    End Sub

    Private Sub cmdClearLastScheduledRun_Click(sender As Object, e As EventArgs) Handles cmdClearLastScheduledRun.Click
        Master.Factory.x48SqlTaskBL.UpdateLastScheduledRun(Master.DataPID, Nothing)
    End Sub

    Private Sub x48TaskOutputFlag_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x48TaskOutputFlag.SelectedIndexChanged
        RefreshX31Combo()
    End Sub
End Class