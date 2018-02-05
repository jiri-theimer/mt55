Public Class p56_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Public ReadOnly Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p41ID.Value)
        End Get
        
    End Property
    Private Sub p56_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p56_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            With Master
                .HeaderIcon = "Images/task_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Úkol"

                Me.p41ID.Value = Request.Item("p41id")
                If .DataPID = 0 And Me.CurrentP41ID > 0 Then

                    p41ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)
                End If

               
                Me.p57ID.DataSource = .Factory.p57TaskTypeBL.GetList(New BO.myQuery)
                Me.p57ID.DataBind()
               
                Me.p59ID_Submitter.DataSource = .Factory.p59PriorityBL.GetList(New BO.myQuery)
                Me.p59ID_Submitter.DataBind()
            End With
            Me.p65ID.DataSource = Master.Factory.p65RecurrenceBL.GetList(New BO.myQuery)
            Me.p65ID.DataBind()
            Me.p65ID.Items.Insert(0, "--Úkol není matkou opakovaných úkolů--")

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
            
        End If
    End Sub

    Private Sub InhaleMyDefault()
        If Master.DataPID <> 0 Then Return
        Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
        Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc

        If Request.Item("t1") <> "" And Request.Item("t2") <> "" Then
            Dim dt1 As New BO.DateTimeByQuerystring(Request.Item("t1")), dt2 As New BO.DateTimeByQuerystring(Request.Item("t2")), intJ02ID As Integer = BO.BAS.IsNullInt(Request.Item("j02id"))
            If dt2.DateWithTime > Today Then
                Me.p56PlanUntil.SelectedDate = dt2.DateWithTime
                If DateDiff(DateInterval.Hour, dt1.DateWithTime, dt2.DateWithTime, Microsoft.VisualBasic.FirstDayOfWeek.Monday) > 2 Then
                    Me.p56PlanFrom.SelectedDate = dt1.DateWithTime
                End If
            End If
            'Me.o22DateUntil.SelectedDate = dt2.DateWithTime
        End If
        If Request.Item("p57id") <> "" Then
            Me.p57ID.SelectedValue = Request.Item("p57id")
            Return
        End If
        If Request.Item("guid_import") <> "" Then
            'import z MS-OUTLOOK přes PLUGIN
            Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(Request.Item("guid_import"))
            If lis.Where(Function(p) p.p85FreeText02 = "p56Name").Count > 0 Then
                Me.p56Name.Text = lis.Where(Function(p) p.p85FreeText02 = "p56Name")(0).p85Message
            End If
            If lis.Where(Function(p) p.p85FreeText02 = "p56Description").Count > 0 Then
                Me.p56Description.Text = lis.Where(Function(p) p.p85FreeText02 = "p56Description")(0).p85Message
            End If
            If lis.Where(Function(p) p.p85FreeText02 = "p56PlanUntil").Count > 0 Then
                Me.p56PlanUntil.SelectedDate = lis.Where(Function(p) p.p85FreeText02 = "p56PlanUntil")(0).p85FreeDate01
            End If
            Return
        End If

        Dim cRecLast As BO.p56Task = Master.Factory.p56TaskBL.LoadMyLastCreated()
        If cRecLast Is Nothing Then
            Return
        End If
        With cRecLast
            If Me.CurrentP41ID = 0 Then
                Me.p41ID.Value = .p41ID.ToString
                Me.p41ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID)
            End If
            Me.p56IsNoNotify.Checked = .p56IsNoNotify
            Me.p57ID.SelectedValue = .p57ID.ToString
            roles1.InhaleInitialData(.PID)
        End With
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            InhaleMyDefault()

            If Me.p57ID.SelectedIndex = 0 And Me.p57ID.Rows > 1 Then
                Me.p57ID.SelectedIndex = 1
            End If
            Handle_FF()
            If roles1.RowsCount = 0 Then
                roles1.AddNewRow(Master.Factory.SysUser.j02ID)

            End If
            Return
        End If


        Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(Master.DataPID)
        Handle_Permissions(cRec)
        
        With cRec
            Me.p41ID.Value = .p41ID.ToString
            Me.p41ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID)
            
            Me.p57ID.SelectedValue = .p57ID.ToString

            Me.p59ID_Submitter.SelectedValue = .p59ID_Submitter
            Handle_FF()

            Me.p56IsNoNotify.Checked = .p56IsNoNotify

            Me.p56Name.Text = .p56Name
            Me.p56Code.Text = .p56Code
            Me.p56Code.NavigateUrl = "javascript:recordcode()"
            If Not BO.BAS.IsNullDBDate(.p56PlanFrom) Is Nothing Then Me.p56PlanFrom.SelectedDate = .p56PlanFrom
            If Not BO.BAS.IsNullDBDate(.p56PlanUntil) Is Nothing Then Me.p56PlanUntil.SelectedDate = .p56PlanUntil
            Me.p56Description.Text = .p56Description
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.p56Ordinary.Value = .p56Ordinary
            Me.p56Plan_Hours.Value = .p56Plan_Hours
            Me.p56Plan_Expenses.Value = .p56Plan_Expenses
            Me.p56CompletePercent.Value = .p56CompletePercent
            Me.p56ExternalPID.Text = .p56ExternalPID
            Me.p56IsPlan_Hours_Ceiling.Checked = .p56IsPlan_Hours_Ceiling
            Me.p56IsPlan_Expenses_Ceiling.Checked = .p56IsPlan_Expenses_Ceiling
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp & " <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"

            Me.p56RecurNameMask.Text = .p56RecurNameMask
            basUI.SelectDropdownlistValue(Me.p65ID, .p65ID.ToString)
            If Not .p56RecurBaseDate Is Nothing Then Me.p56RecurBaseDate.SelectedDate = .p56RecurBaseDate
            
            Me.p56IsStopRecurrence.Checked = .p56IsStopRecurrence
        End With
        roles1.InhaleInitialData(cRec.PID)
        If roles1.RowsCount = 0 And Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString Then
            Me.chkMyPrivateTask.Checked = True
        End If
        tags1.RefreshData(cRec.PID)

       
    End Sub

    Private Sub Handle_Permissions(cRec As BO.p56Task)
        Dim cDisp As BO.p56RecordDisposition = Master.Factory.p56TaskBL.InhaleRecordDisposition(cRec)
        If Not cDisp.ReadAccess Then
            Master.StopPage("Nedisponujete oprávněním číst tento úkol.")
        End If
        If cDisp.OwnerAccess Then Return 'editační práva

        If cRec.b01ID <> 0 Then
            Server.Transfer("workflow_dialog.aspx?prefix=p56&pid=" & cRec.PID.ToString)
        Else
            Server.Transfer("clue_p56_record.aspx?mode=readonly&pid=" & cRec.PID.ToString)
        End If

    End Sub

    Private Sub p56_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblDateUntil.CssClass = "lbl" : lblDateFrom.CssClass = "lbl"

        lblDateFrom.Text = "Plánované zahájení:" : lblDateUntil.Text = "Termín splnění úkolu:"

        Dim bolShowRoles As Boolean = True

        If Me.p57ID.SelectedIndex > 0 Then
            Master.HeaderText = Me.p57ID.Text & " | " & Me.p41ID.Text
            If Me.p57ID.Rows = 2 Then
                Me.p57ID.Visible = False : lblP57ID.Visible = False
            End If

            Dim cRec As BO.p57TaskType = Master.Factory.p57TaskTypeBL.Load(BO.BAS.IsNullInt(Me.p57ID.SelectedValue))
            With cRec
                
                lblP59ID_Submitter.Visible = .p57IsEntry_Priority
                p59ID_Submitter.Visible = .p57IsEntry_Priority
                lblDateFrom.Visible = True : p56PlanFrom.Visible = True
                Select Case .p57PlanDatesEntryFlag
                    Case 1, 3, 4
                        lblDateUntil.CssClass = "lblReq"
                    Case 0
                        lblDateFrom.Visible = False
                        p56PlanFrom.Visible = False
                End Select
                Select Case .p57PlanDatesEntryFlag
                    Case 3, 4
                        lblDateFrom.CssClass = "lblReq"
                End Select
                If .p57PlanDatesEntryFlag = 4 Then
                    If Me.p56PlanUntil.IsEmpty Then
                        Me.p56PlanUntil.SelectedDate = Today
                    End If
                    If Me.p56PlanFrom.IsEmpty Then
                        Me.p56PlanFrom.SelectedDate = Me.p56PlanUntil.SelectedDate
                    End If
                End If
                panBudget.Visible = .p57IsEntry_Budget

                bolShowRoles = .p57IsEntry_Receiver
                lblCompletePercent.Visible = .p57IsEntry_CompletePercent
                p56CompletePercent.Visible = .p57IsEntry_CompletePercent
                If .p57Caption_PlanFrom <> "" Then
                    lblDateFrom.Text = .p57Caption_PlanFrom & ":"
                End If
                If .p57Caption_PlanUntil <> "" Then
                    lblDateUntil.Text = .p57Caption_PlanUntil & ":"
                End If
            End With
        Else
            Master.HeaderText = "Úkol | " & Me.p41ID.Text
        End If

        If Me.chkMyPrivateTask.Checked Then
            panRoles.Visible = False
        Else
            panRoles.Visible = bolShowRoles
        End If
        If Me.p65ID.SelectedIndex > 0 Then
            panRecurrence.Visible = True
        Else
            panRecurrence.Visible = False
        End If


    End Sub

    

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p56TaskBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p56-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

   
    

    Private Sub p57ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p57ID.NeedMissingItem
        Dim cRec As BO.p57TaskType = Master.Factory.p57TaskTypeBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.p57Name
        End If

    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        Server.Transfer("p56_record.aspx?pid=" & Master.DataPID.ToString & "&p41id=" & Me.CurrentP41ID.ToString)
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Me.chkMyPrivateTask.Checked And BO.BAS.IsNullInt(Me.j02ID_Owner.Value) <> Master.Factory.SysUser.j02ID Then
            Master.Notify(String.Format("U osobního úkolu musí být vlastník: {0}.", Master.Factory.SysUser.Person), NotifyLevel.ErrorMessage)
            Return
        End If
        roles1.SaveCurrentTempData()
        With Master.Factory.p56TaskBL
            Dim cRec As BO.p56Task = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p56Task)
            With cRec
                .p41ID = Me.CurrentP41ID
                .p56Name = Me.p56Name.Text

                .p57ID = BO.BAS.IsNullInt(Me.p57ID.SelectedValue)


                .p59ID_Submitter = BO.BAS.IsNullInt(Me.p59ID_Submitter.SelectedValue)

                .p56PlanFrom = BO.BAS.IsNullDBDate(Me.p56PlanFrom.SelectedDate)
                .p56PlanUntil = BO.BAS.IsNullDBDate(Me.p56PlanUntil.SelectedDate)

                .p56IsNoNotify = Me.p56IsNoNotify.Checked

                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
                .p56Description = Me.p56Description.Text
                .p56Ordinary = BO.BAS.IsNullInt(Me.p56Ordinary.Value)
                .p56Plan_Hours = BO.BAS.IsNullNum(Me.p56Plan_Hours.Value)
                .p56IsPlan_Hours_Ceiling = Me.p56IsPlan_Hours_Ceiling.Checked
                .p56IsPlan_Expenses_Ceiling = Me.p56IsPlan_Expenses_Ceiling.Checked
                .p56Plan_Expenses = BO.BAS.IsNullNum(Me.p56Plan_Expenses.Value)
                .p56CompletePercent = BO.BAS.IsNullInt(Me.p56CompletePercent.Value)
                .p56ExternalPID = Me.p56ExternalPID.Text

                .p65ID = BO.BAS.IsNullInt(Me.p65ID.SelectedValue)
                If .p65ID <> 0 Then
                    .p56RecurBaseDate = Me.p56RecurBaseDate.SelectedDate
                    .p56RecurNameMask = Me.p56RecurNameMask.Text
                End If
                .p56IsStopRecurrence = Me.p56IsStopRecurrence.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = Nothing
            If chkMyPrivateTask.Checked Then
                lisX69 = New List(Of BO.x69EntityRole_Assign)
            End If
            If panRoles.Visible Then
                lisX69 = roles1.GetData4Save()
                If roles1.ErrorMessage <> "" Then
                    Master.Notify(roles1.ErrorMessage, 2)
                    Return
                End If
            End If

            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()

            If .Save(cRec, lisX69, lisFF, "") Then
                Dim bolNew As Boolean = Master.IsRecordNew
                Master.DataPID = .LastSavedPID
                Master.Factory.o51TagBL.SaveBinding("p56", Master.DataPID, tags1.Geto51IDs())
                If Not bolNew Or ff1.TagsCount > 0 Then
                    Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p56Task, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                End If
                Master.CloseAndRefreshParent("p56-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p56Task, Master.DataPID, BO.BAS.IsNullInt(Me.p57ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p56Task, BO.BAS.IsNullInt(Me.p57ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "p56Task_FreeField", Master.DataPID)
                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)

                
            End If
        End With
    End Sub

    Private Sub p57ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p57ID.SelectedIndexChanged
        Handle_FF()
    End Sub

    
    
    Private Sub chkMyPrivateTask_CheckedChanged(sender As Object, e As EventArgs) Handles chkMyPrivateTask.CheckedChanged
        If Me.chkMyPrivateTask.Checked Then
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
        End If
        
    End Sub
End Class