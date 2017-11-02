Public Class p56_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP41ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP41ID.Value = value.ToString
        End Set
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

                Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
                If Me.CurrentP41ID = 0 And Request.Item("masterprefix") = "p41" And BO.BAS.IsNullInt(Request.Item("masterpid")) <> 0 Then
                    Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                End If

                If Me.CurrentP41ID = 0 And .DataPID = 0 Then
                    If Request.Item("masterprefix") <> "" And Request.Item("masterpid") <> "" Then
                        Server.Transfer("select_project.aspx?oper=createtask&" & basUI.GetCompleteQuerystring(Request))
                    Else
                        .StopPage("Na vstupu chybí ID projektu.")
                    End If

                End If
                With .Factory.j03UserBL
                    .InhaleUserParams("p56_record-more")
                    Me.chkMore.Checked = BO.BAS.BG(.GetUserParam("p56_record-more", "0"))

                End With

                
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

        Dim cRecLast As BO.p56Task = Master.Factory.p56TaskBL.LoadMyLastCreated()
        If cRecLast Is Nothing Then
            Return
        End If
        With cRecLast
            Me.p56IsNoNotify.Checked = .p56IsNoNotify
            Me.p57ID.SelectedValue = .p57ID.ToString
            roles1.InhaleInitialData(.PID)
        End With
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            InhaleMyDefault()
            SetupO22Combo()
            Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)
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
            Me.CurrentP41ID = .p41ID
            Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)

            Me.p57ID.SelectedValue = .p57ID.ToString

            Me.p59ID_Submitter.SelectedValue = .p59ID_Submitter
            Handle_FF()
            SetupO22Combo()
            Me.o22ID.SelectedValue = .o22ID.ToString
            Me.p56IsNoNotify.Checked = .p56IsNoNotify

            Me.p56Name.Text = .p56Name
            If Not BO.BAS.IsNullDBDate(.p56PlanFrom) Is Nothing Then Me.p56PlanFrom.SelectedDate = .p56PlanFrom
            If Not BO.BAS.IsNullDBDate(.p56PlanUntil) Is Nothing Then Me.p56PlanUntil.SelectedDate = .p56PlanUntil
            If Not BO.BAS.IsNullDBDate(.p56ReminderDate) Is Nothing Then Me.p56ReminderDate.SelectedDate = .p56ReminderDate
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
            If .p65ID > 0 Then
                chkMore.Checked = True
            End If
            Me.p56IsStopRecurrence.Checked = .p56IsStopRecurrence
        End With
        roles1.InhaleInitialData(cRec.PID)
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

        

        If Me.p57ID.SelectedIndex > 0 Then
            Master.HeaderText = Me.p57ID.Text & " | " & Me.Project.Text
            If Me.p57ID.Rows = 2 Then
                Me.p57ID.Visible = False : lblP57ID.Visible = False
            Else
                Me.chkMore.Visible = False
            End If

            Dim cRec As BO.p57TaskType = Master.Factory.p57TaskTypeBL.Load(BO.BAS.IsNullInt(Me.p57ID.SelectedValue))
            With cRec
                If .p57IsEntry_Budget And .p57IsEntry_CompletePercent And .p57IsEntry_Priority And .p57IsEntry_Receiver And Me.p57ID.Rows = 2 Then
                    Me.chkMore.Visible = True
                Else
                    Me.chkMore.Visible = False
                End If
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

                panRoles.Visible = .p57IsEntry_Receiver
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
            Master.HeaderText = "Úkol | " & Me.Project.Text
        End If

        If Me.chkMore.Visible Then
            Dim b As Boolean = Me.chkMore.Checked

            'lblDateFrom.Visible = b : p56PlanFrom.Visible = b
            panDescription.Visible = b
            Me.lblOwner.Visible = b : Me.j02ID_Owner.Visible = b
            Me.p56IsNoNotify.Visible = b
            lblP59ID_Submitter.Visible = b : p59ID_Submitter.Visible = b
            lblCompletePercent.Visible = b : p56CompletePercent.Visible = b
            panBudget.Visible = b
            If p57ID.Rows = 2 Then
                p57ID.Visible = b : Me.lblP57ID.Visible = b
            End If

            If ff1.FieldsCount > 0 Or ff1.TagsCount > 0 Or b Then
                RadTabStrip1.FindTabByValue("core").Style.Item("display") = "block"
                RadTabStrip1.FindTabByValue("ff").Style.Item("display") = "block"
            Else
                RadTabStrip1.FindTabByValue("ff").Style.Item("display") = "none"
                RadTabStrip1.FindTabByValue("core").Style.Item("display") = "none"
            End If
            RadTabStrip1.FindTabByValue("other").Style.Item("display") = BO.BAS.GB_Display(b)
        End If

        If Me.o22ID.Rows > 1 Then
            Me.lblO22ID.Visible = True : Me.o22ID.Visible = True
        Else
            Me.lblO22ID.Visible = False : Me.o22ID.Visible = False
        End If
        If Me.p65ID.SelectedIndex > 0 Then
            panRecurrence.Visible = True
        Else
            panRecurrence.Visible = False
        End If


    End Sub

    Private Sub SetupO22Combo()
        If Me.CurrentP41ID = 0 Then Return
        Dim mq As New BO.myQueryO22
        mq.p41ID = Me.CurrentP41ID
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Me.o22ID.DataSource = Master.Factory.o22MilestoneBL.GetList(mq).Where(Function(p) p.o22Name <> "")
        Me.o22ID.DataBind()
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

   
    Private Sub o22ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles o22ID.NeedMissingItem
        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(CInt(strFoundedMissingItemValue))
        strAddMissingItemText = cRec.NameWithDate
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
        roles1.SaveCurrentTempData()
        With Master.Factory.p56TaskBL
            Dim cRec As BO.p56Task = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p56Task)
            With cRec
                .p41ID = Me.CurrentP41ID
                .p56Name = Me.p56Name.Text

                .p57ID = BO.BAS.IsNullInt(Me.p57ID.SelectedValue)

                .o22ID = BO.BAS.IsNullInt(Me.o22ID.SelectedValue)
                .p59ID_Submitter = BO.BAS.IsNullInt(Me.p59ID_Submitter.SelectedValue)

                .p56PlanFrom = BO.BAS.IsNullDBDate(Me.p56PlanFrom.SelectedDate)
                .p56PlanUntil = BO.BAS.IsNullDBDate(Me.p56PlanUntil.SelectedDate)
                .p56ReminderDate = BO.BAS.IsNullDBDate(Me.p56ReminderDate.SelectedDate)
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

    Private Sub chkMore_CheckedChanged(sender As Object, e As EventArgs) Handles chkMore.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p56_record-more", BO.BAS.GB(chkMore.Checked))
    End Sub
End Class