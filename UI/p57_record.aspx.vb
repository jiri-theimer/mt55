Public Class p57_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p57_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Typ úkolu"

                Me.x38ID.DataSource = .Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p56Task)
                Me.x38ID.DataBind()
                Me.b01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.p56Task)
                Me.b01ID.DataBind()
            End With

            RefreshRecord()

         
            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p57TaskType = Master.Factory.p57TaskTypeBL.Load(Master.DataPID)
        With cRec
            Me.p57Name.Text = .p57Name
            Me.p57Ordinary.Value = .p57Ordinary

            Me.b01ID.SelectedValue = .b01ID.ToString
            Me.x38ID.SelectedValue = .x38ID.ToString
            Me.p57IsHelpdesk.Checked = .p57IsHelpdesk
            Me.p57IsEntry_Receiver.Checked = .p57IsEntry_Receiver
            Me.p57IsEntry_Budget.Checked = .p57IsEntry_Budget
            Me.p57IsEntry_Priority.Checked = .p57IsEntry_Priority
            Me.p57IsEntry_CompletePercent.Checked = .p57IsEntry_CompletePercent
            basUI.SelectDropdownlistValue(Me.p57PlanDatesEntryFlag, .p57PlanDatesEntryFlag.ToString)
            Me.p57Caption_PlanUntil.Text = .p57Caption_PlanUntil
            Me.p57Caption_PlanFrom.Text = .p57Caption_PlanFrom
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp

           
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p57TaskTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p57-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p57TaskTypeBL
            Dim cRec As BO.p57TaskType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p57TaskType)
            With cRec
                .x38ID = BO.BAS.IsNullInt(Me.x38ID.SelectedValue)
                .p57Name = Me.p57Name.Text
                .p57Ordinary = BO.BAS.IsNullInt(Me.p57Ordinary.Value)

                .b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)
                .p57IsHelpdesk = Me.p57IsHelpdesk.Checked

                .p57IsEntry_Receiver = Me.p57IsEntry_Receiver.Checked
                .p57IsEntry_Budget = Me.p57IsEntry_Budget.Checked
                .p57IsEntry_Priority = Me.p57IsEntry_Priority.Checked
                .p57IsEntry_CompletePercent = Me.p57IsEntry_CompletePercent.Checked
                .p57PlanDatesEntryFlag = BO.BAS.IsNullInt(Me.p57PlanDatesEntryFlag.SelectedValue)
                .p57Caption_PlanFrom = Me.p57Caption_PlanFrom.Text
                .p57Caption_PlanUntil = Me.p57Caption_PlanUntil.Text

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            

          
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p57-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class