Public Class p41_create
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    

    Private Sub p41_create_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p41_create"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_P41_Draft_Creator
                .neededPermissionIfSecond = BO.x53PermValEnum.GR_P41_Creator
                .HeaderText = "Založit projekt"
                .HeaderIcon = "Images/project_32.png"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
                Me.p28ID_Client.J03ID_System = .Factory.SysUser.PID.ToString
                .Factory.j03UserBL.InhaleUserParams("p41_create-chkPlanDates")
                Me.chkPlanDates.Checked = BO.BAS.BG(.Factory.j03UserBL.GetUserParam("p41_create-chkPlanDates", "0"))
                Me.j02ID_Owner.Value = .Factory.SysUser.j02ID.ToString
                Me.j02ID_Owner.Text = .Factory.SysUser.PersonDesc
                If Not .Factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator) Then
                    Me.p41IsDraft.Visible = False : Me.p41IsDraft.Checked = True    'povolit pouze založení DRAFT projektu
                End If

            End With

            Dim lisP42 As IEnumerable(Of BO.p42ProjectType) = Master.Factory.p42ProjectTypeBL.GetList(New BO.myQuery)
            Me.p42ID.DataSource = lisP42
            Me.p42ID.DataBind()
            If lisP42.Where(Function(p) p.p42IsDefault = True).Count > 0 Then
                Me.p42ID.SelectedValue = lisP42.Where(Function(p) p.p42IsDefault = True)(0).PID.ToString
            End If
            Me.j18ID.DataSource = Master.Factory.j18RegionBL.GetList(New BO.myQuery)
            Me.j18ID.DataBind()
            Me.p61ID.DataSource = Master.Factory.p61ActivityClusterBL.GetList(New BO.myQuery)
            Me.p61ID.DataBind()

            basUI.SetupP87Combo(Master.Factory, Me.p87ID)
            SetupPriceList()

            Me.p92id.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice)
            Me.p92id.DataBind()
            
            Me.p92id.ChangeItemText("", "--Dědit z nastavení klienta projektu--")
            Me.p87ID.ChangeItemText("", "--Dědit z nastavení klienta projektu--")

            Me.p41PlanFrom.SelectedDate = DateSerial(Year(Now), Month(Now), Day(Now))
            Me.p41PlanUntil.SelectedDate = DateSerial(Year(Now), Month(Now), Day(Now)).AddMonths(2)
            

            TryInhaleInitialData()

            If Request.Item("clone") <> "1" Then
                Handle_FF()
            End If

        End If
    End Sub

    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p41Project, Master.DataPID, BO.BAS.IsNullInt(Me.p42ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p41Project, BO.BAS.IsNullInt(Me.p42ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "p41Project_FreeField", Master.DataPID)

                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)
            End If
        End With
    End Sub

    Private Sub SetupPriceList()
        Me.p51ID_Billing.DataSource = Master.Factory.p51PriceListBL.GetList(New BO.myQuery).Where(Function(p) p.p51IsInternalPriceList = False And p.p51IsMasterPriceList = False And p.p51IsCustomTailor = False)
        Me.p51ID_Billing.DataBind()
        ''Me.p51ID_Billing.ChangeItemText("", "--Sazby dědit z nastavení klienta projektu--")
    End Sub


    Private Sub TryInhaleInitialData()
        If Request.Item("client_family") = "1" And Request.Item("pid") <> "" Then
            hidCloneP41ID.Value = Request.Item("pid")
            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(BO.BAS.IsNullInt(hidCloneP41ID.Value))
            If cRec Is Nothing Then Return
            With cRec
                Me.p42ID.SelectedValue = .p42ID.ToString
                Me.j18ID.SelectedValue = .j18ID.ToString
                If .p28ID_Client <> 0 Then
                    Me.p28ID_Client.Value = .p28ID_Client.ToString
                    Me.p28ID_Client.Text = .Client
                End If
                Handle_ShowPriceListReference(False, .p51ID_Billing)


                Me.p87ID.SelectedValue = .p87ID.ToString
                Me.p92id.SelectedValue = .p92ID.ToString
            End With
            roles1.InhaleInitialData(cRec.PID)
            If Request.Item("create_parent") = "1" Then
                'založit pod-projekt
                Me.p41ParentID.Value = cRec.PID.ToString
                Me.p41ParentID.Text = cRec.FullName
            End If
        End If
        If Request.Item("clone") = "1" And Request.Item("pid") <> "" Then
            hidCloneP41ID.Value = Request.Item("pid")
            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(BO.BAS.IsNullInt(hidCloneP41ID.Value))
            If cRec Is Nothing Then Return
            With cRec
                Me.p41Name.Text = .p41Name
                Me.p41NameShort.Text = .p41NameShort
                Me.p42ID.SelectedValue = .p42ID.ToString
                Me.j18ID.SelectedValue = .j18ID.ToString
                If .p28ID_Client <> 0 Then
                    Me.p28ID_Client.Value = .p28ID_Client.ToString
                    Me.p28ID_Client.Text = .Client
                End If
                If .p28ID_Billing <> 0 Then
                    Me.p28ID_Billing.Value = .p28ID_Billing.ToString
                    Me.p28ID_Billing.Text = Master.Factory.p28ContactBL.Load(.p28ID_Billing).p28Name
                End If
                If .p41ParentID <> 0 Then
                    Me.p41ParentID.Value = .p41ParentID.ToString
                    Me.p41ParentID.Text = Master.Factory.p41ProjectBL.Load(.p41ParentID).FullName
                End If
                If Not (BO.BAS.IsNullDBDate(.p41PlanFrom) Is Nothing Or BO.BAS.IsNullDBDate(.p41PlanUntil) Is Nothing) Then
                    Me.chkPlanDates.Checked = True
                    Me.p41PlanFrom.SelectedDate = .p41PlanFrom
                    Me.p41PlanUntil.SelectedDate = .p41PlanUntil
                End If
                Me.p41LimitHours_Notification.Value = .p41LimitHours_Notification
                Me.p41LimitFee_Notification.Value = .p41LimitFee_Notification
                Handle_ShowPriceListReference(False, .p51ID_Billing)

                Me.p87ID.SelectedValue = .p87ID.ToString
                Me.p92id.SelectedValue = .p92ID.ToString
                Me.p41InvoiceMaturityDays.Value = .p41InvoiceMaturityDays
                Me.p41InvoiceDefaultText1.Text = .p41InvoiceDefaultText1
                Me.p41InvoiceDefaultText2.Text = .p41InvoiceDefaultText2

            End With
            roles1.InhaleInitialData(cRec.PID)
            Master.DataPID = cRec.PID
            Handle_FF()
            tags1.RefreshData(cRec.PID)

            RefreshTasks()


            Master.DataPID = 0
            Return
        End If
        If Request.Item("p28id") <> "" Then
            Me.p28ID_Client.Value = Request.Item("p28id")
            Me.p28ID_Client.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, BO.BAS.IsNullInt(Request.Item("p28id")))
        End If
        'načtení výchozích dat u nového projektu bez kopírování
        Dim cRecLast As BO.p41Project = Master.Factory.p41ProjectBL.LoadMyLastCreated()
        If Not cRecLast Is Nothing Then
            With cRecLast
                If Me.p42ID.SelectedValue <> .p42ID.ToString Then
                    Me.p42ID.SelectedValue = p42ID.ToString
                End If
                Me.p51ID_Billing.SelectedValue = .p51ID_Billing.ToString
                Me.j18ID.SelectedValue = .j18ID.ToString
                roles1.InhaleInitialData(.PID)
                If .p41LimitFee_Notification > 0 Or .p41LimitHours_Notification > 0 Then
                    Me.chkDefineLimits.Checked = True
                Else
                    Me.chkDefineLimits.Checked = False
                End If
            End With


            Return
        End If

    End Sub

    Private Sub RefreshTasks()
        Dim mq As New BO.myQueryP56
        mq.p41ID = BO.BAS.IsNullInt(hidCloneP41ID.Value)
        Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq, True)
        lis = lis.OrderByDescending(Function(p) p.p65ID).ThenByDescending(Function(p) p.PID)
        If lis.Count = 0 Then
            panCloneTasks.Visible = False
        Else
            panCloneTasks.Visible = True
        End If
        If chkShowMothersOnly.Checked Then lis = lis.Where(Function(p) p.p65ID <> 0)

        rpP56IDs.DataSource = lis
        rpP56IDs.DataBind()
        
    End Sub

    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click
        Dim strPID As String = Me.HardRefreshPID.Value

        Select Case Me.HardRefreshWindow.Value
            Case "p28_client_add"
                If strPID <> "" Then
                    Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(CInt(strPID))
                    If Not cRec Is Nothing Then
                        Me.p28ID_Client.Text = cRec.p28Name
                        Me.p28ID_Client.Value = strPID
                        Me.p28ID_Client.Focus()
                    End If
                End If
            Case "p28_billing_add"
                If strPID <> "" Then
                    Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(CInt(strPID))
                    If Not cRec Is Nothing Then
                        Me.p28ID_Billing.Text = cRec.p28Name
                        Me.p28ID_Billing.Value = strPID
                        Me.p28ID_Billing.Focus()
                    End If
                End If
            Case "p51_billing_add"
                SetupPriceList()
                Handle_ShowPriceListReference(True, BO.BAS.IsNullInt(strPID))
        End Select

        Me.HardRefreshPID.Value = ""
        Me.HardRefreshWindow.Value = ""
    End Sub

    Private Sub Handle_ShowPriceListReference(bolAfterNewPLCreated As Boolean, Optional intP51ID As Integer = 0)
        p51ID_Billing.Visible = True

        If intP51ID = 0 Then
            Me.p51ID_Billing.SelectedValue = ""
        Else
            Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(intP51ID)
            If Not cRec.p51IsCustomTailor Then
                Me.p51ID_Billing.SelectedValue = intP51ID.ToString
            Else
                'sazby na míru
                hidP51ID_Tailor.Value = cRec.PID.ToString
                If Not bolAfterNewPLCreated Then
                    Master.Notify("Vzorový projekt má sazby na míru, proto tento ceník není přednastaven v tomto projektu.", NotifyLevel.InfoMessage)
                Else
                    p51ID_Billing.Visible = False
                    cmdNewP51.Visible = True
                    cmdNewP51.NavigateUrl = "javascript:p51_edit(" & cRec.PID.ToString & ")"


                End If
                
                
            End If
        End If
    End Sub

    Private Sub p41_create_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panPlanDates.Visible = Me.chkPlanDates.Checked
        Dim intJ18ID As Integer = BO.BAS.IsNullInt(Me.j18ID.SelectedValue)
        If intJ18ID > 0 Then
            Me.clue_j18.Visible = True : lblJ18Message.Text = ""
            Me.clue_j18.Attributes.Item("rel") = "clue_j18_record.aspx?pid=" & intJ18ID.ToString
            Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.j18Region, intJ18ID)
            If lisX69.Count > 0 Then
                Me.lblJ18Message.Text = String.Format("V nastavení střediska [{0}] jsou přiřazeny projektové role, jejichž oprávnění se automaticky dědí do projektu.", Me.j18ID.Text)
            End If
        Else
            Me.clue_j18.Visible = False
            lblJ18Message.Text = ""
        End If
        If Me.p42ID.SelectedValue <> "" Then
            Me.clue_p42.Attributes.Item("rel") = "clue_p42_record.aspx?pid=" & Me.p42ID.SelectedValue : clue_p42.Visible = True
        Else
            clue_p42.Visible = False
        End If


        panLimits.Visible = Me.chkDefineLimits.Checked

        lblP51ID_Billing.Visible = True : Me.p51ID_Billing.Visible = True : Me.cmdNewP51.Visible = True
        Select Case Me.opgPriceList.SelectedValue
            Case "1"
                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False
                cmdNewP51.Visible = False
            Case "2"
                Me.cmdNewP51.Text = "Založit nový ceník"
                Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(0)"
            Case "3"
                Me.cmdNewP51.Text = "Definovat sazby projektu na míru"
                If BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value) <> 0 Then
                    cmdNewP51.NavigateUrl = "javascript:p51_edit(" & Me.hidP51ID_Tailor.Value & ")"
                Else
                    Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(1)"
                End If

                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False

        End Select
       
    End Sub

    Private Sub chkPlanDates_CheckedChanged(sender As Object, e As EventArgs) Handles chkPlanDates.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p41_create-chkPlanDates", BO.BAS.GB(chkPlanDates.Checked))
    End Sub

   
    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub
    
    
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        roles1.SaveCurrentTempData()

        If strButtonValue = "ok" Then
            Dim cRec As New BO.p41Project
            With cRec
                .p41IsDraft = Me.p41IsDraft.Checked
                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
                If chkPlanDates.Checked Then
                    .p41PlanFrom = Me.p41PlanFrom.SelectedDate
                    .p41PlanUntil = Me.p41PlanUntil.SelectedDate
                End If
                .p41Name = Me.p41Name.Text
                .p41NameShort = Me.p41NameShort.Text
                .p42ID = BO.BAS.IsNullInt(Me.p42ID.SelectedValue)
                .j18ID = BO.BAS.IsNullInt(Me.j18ID.SelectedValue)
                .p28ID_Client = BO.BAS.IsNullInt(Me.p28ID_Client.Value)
                .p61ID = BO.BAS.IsNullInt(Me.p61ID.SelectedValue)
                .p41ParentID = BO.BAS.IsNullInt(Me.p41ParentID.Value)
                Select Case Me.opgPriceList.SelectedValue
                    Case "1"
                        'projekt bez ceníku
                    Case "2"
                        'přiřazený ceník
                        .p51ID_Billing = BO.BAS.IsNullInt(Me.p51ID_Billing.SelectedValue)
                    Case "3"
                        'sazby na míru
                        .p51ID_Billing = BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value)
                End Select
                If Me.opgPriceList.SelectedValue <> "1" And .p51ID_Billing = 0 Then
                    Master.Notify("Chybí přiřazený ceník.", NotifyLevel.WarningMessage) : Return
                End If

                .p87ID = BO.BAS.IsNullInt(Me.p87ID.SelectedValue)
                .p28ID_Billing = BO.BAS.IsNullInt(Me.p28ID_Billing.Value)
                .p92ID = BO.BAS.IsNullInt(Me.p92id.SelectedValue)
                .p41InvoiceMaturityDays = BO.BAS.IsNullInt(Me.p41InvoiceMaturityDays.Value)
                .p72ID_NonBillable = BO.BAS.IsNullInt(Me.p72ID_NonBillable.SelectedValue)

                .p41InvoiceDefaultText1 = Me.p41InvoiceDefaultText1.Text
                .p41InvoiceDefaultText2 = Me.p41InvoiceDefaultText2.Text

                If Me.chkDefineLimits.Checked Then
                    .p41LimitHours_Notification = BO.BAS.IsNullNum(Me.p41LimitHours_Notification.Value)
                    .p41LimitFee_Notification = BO.BAS.IsNullNum(Me.p41LimitFee_Notification.Value)
                End If
                .p41RobotAddress = Me.p41RobotAddress.Text
                .p41ExternalPID = Me.p41ExternalPID.Text
                .p41WorksheetOperFlag = CType(p41WorksheetOperFlag.SelectedValue, BO.p41WorksheetOperFlagEnum)
                .p41BillingMemo = Trim(Me.p41BillingMemo.Text)
                
            End With

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()
            

            With Master.Factory.p41ProjectBL
                If .Save(cRec, Nothing, Nothing, lisX69, lisFF) Then
                    Master.DataPID = .LastSavedPID
                    Master.Factory.o51TagBL.SaveBinding("p41", Master.DataPID, tags1.Geto51IDs())
                    If ff1.GetTags.Count > 0 Then Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p41Project, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                    If rpP56IDs.Items.Count > 0 Then
                        CreateTasks(Master.DataPID)
                    End If

                    Master.CloseAndRefreshParent("p41-create")
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With
        End If
    End Sub

    Private Sub CreateTasks(intNewP41ID As Integer)
        For Each ri As RepeaterItem In rpP56IDs.Items
            If CType(ri.FindControl("chkSelect"), CheckBox).Checked Then
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(CInt(CType(ri.FindControl("p56ID"), HiddenField).Value))
                Dim c As New BO.p56Task
                With cTask
                    c.p41ID = intNewP41ID
                    c.p57ID = .p57ID
                    c.p56Name = .p56Name
                    c.p56NameShort = .p56NameShort
                    c.p56Description = .p56Description

                    c.p56IsNoNotify = True
                    c.p65ID = .p65ID
                    c.p56RecurNameMask = .p56RecurNameMask
                    c.p56RecurBaseDate = .p56RecurBaseDate
                    c.p56PlanFrom = .p56PlanFrom
                    c.p56PlanUntil = .p56PlanUntil

                    c.p56Plan_Hours = .p56Plan_Hours
                    c.p56Plan_Expenses = .p56Plan_Expenses
                    c.p56IsPlan_Hours_Ceiling = .p56IsPlan_Hours_Ceiling
                    c.p56IsPlan_Expenses_Ceiling = .p56IsPlan_Expenses_Ceiling


                End With
                Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p56Task, cTask.PID)
                Dim ffs As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p56Task, Master.DataPID, c.p57ID)
                Master.Factory.p56TaskBL.Save(c, lisX69, ffs, "")
            End If
        Next
    End Sub

    Private Sub p42ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p42ID.SelectedIndexChanged
        Handle_FF()
    End Sub

    Private Sub rpP56IDs_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP56IDs.ItemDataBound
        Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
        CType(e.Item.FindControl("p56ID"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("Receivers"), Label).Text = cRec.ReceiversInLine
        If cRec.p65ID > 0 Then
            e.Item.FindControl("img1").Visible = True
        Else
            e.Item.FindControl("img1").Visible = False
        End If

    End Sub

    Private Sub chkShowMothersOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowMothersOnly.CheckedChanged
        RefreshTasks()
    End Sub
End Class