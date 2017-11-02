Public Class p41_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord


    Private Sub p41_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p41_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            With Master
                .HeaderIcon = "Images/project_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    .StopPage("PID is missing!")
                End If
                Me.p28ID_Client.J03ID_System = .Factory.SysUser.PID.ToString

            End With

            Dim lisP42 As IEnumerable(Of BO.p42ProjectType) = Master.Factory.p42ProjectTypeBL.GetList(New BO.myQuery)
            Me.p42ID.DataSource = lisP42
            Me.p42ID.DataBind()

            basUI.SetupP87Combo(Master.Factory, Me.p87ID)

            SetupPricelistCombo()

            Me.p92id.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice)
            Me.p92id.DataBind()

            Me.p92id.ChangeItemText("", "--Dědit z nastavení klienta projektu--")
            Me.p87ID.ChangeItemText("", "--Dědit z nastavení klienta projektu--")
            Me.j18ID.DataSource = Master.Factory.j18RegionBL.GetList(New BO.myQuery)
            Me.j18ID.DataBind()
            Me.p61ID.DataSource = Master.Factory.p61ActivityClusterBL.GetList(New BO.myQuery)
            Me.p61ID.DataBind()
           
            RefreshRecord()
        End If
    End Sub

    Private Sub SetupPricelistCombo()
        Dim lis As IEnumerable(Of BO.p51PriceList) = Master.Factory.p51PriceListBL.GetList(New BO.myQuery)

        Me.p51ID_Billing.DataSource = lis.Where(Function(p) p.p51IsInternalPriceList = False And p.p51IsMasterPriceList = False And p.p51IsCustomTailor = False)
        Me.p51ID_Billing.DataBind()

        Me.p51ID_Internal.DataSource = lis.Where(Function(p) p.p51IsInternalPriceList = True And p.p51IsMasterPriceList = False And p.p51IsCustomTailor = False)

        Me.p51ID_Internal.DataBind()
        Me.p51ID_Internal.ChangeItemText("", "--Dědit z nastavení systému--")

    End Sub
    Private Sub Handle_Permissions(cRec As BO.p41Project)
        With Master
            Dim cDisp As BO.p41RecordDisposition = .Factory.p41ProjectBL.InhaleRecordDisposition(cRec)
            If Not cDisp.OwnerAccess Then
                .StopPage("Pro klienta nedisponujete vlastnickým (editačním) oprávněním.")
            End If
        End With


    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        Handle_Permissions(cRec)
        With cRec
            Master.HeaderText = "Projekt | " & .FullName

            Me.p41Name.Text = .p41Name
            Me.p41NameShort.Text = .p41NameShort
            Me.p41Code.Text = .p41Code
            Me.p41Code.NavigateUrl = "javascript:recordcode()"
            Me.p42ID.SelectedValue = .p42ID.ToString
            Me.j18ID.SelectedValue = .j18ID.ToString
            Me.p61id.selectedvalue = .p61ID.ToString
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            If .p28ID_Client <> 0 Then
                Me.p28ID_Client.Value = .p28ID_Client.ToString
                Me.p28ID_Client.Text = .Client
            End If
            If .p28ID_Billing <> 0 Then
                Me.p28ID_Billing.Value = .p28ID_Billing.ToString
                Me.p28ID_Billing.Text = Master.Factory.p28ContactBL.Load(.p28ID_Billing).p28Name
            End If
            If Not (BO.BAS.IsNullDBDate(.p41PlanFrom) Is Nothing Or BO.BAS.IsNullDBDate(.p41PlanUntil) Is Nothing) Then
                Me.chkPlanDates.Checked = True
                Me.p41PlanFrom.SelectedDate = .p41PlanFrom
                Me.p41PlanUntil.SelectedDate = .p41PlanUntil
            End If
            Me.p51ID_Billing.SelectedValue = .p51ID_Billing.ToString
            If .p51ID_Billing > 0 Then
                Dim cP51 As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(.p51ID_Billing)
                If cP51.p51IsCustomTailor Then
                    Me.hidP51ID_Tailor.Value = cP51.PID.ToString
                    Me.opgPriceList.SelectedValue = "3"
                Else
                    Me.opgPriceList.SelectedValue = "2"
                End If
            Else
                Me.opgPriceList.SelectedValue = "1"
            End If
            Me.p51ID_Internal.SelectedValue = .p51ID_Internal.ToString
            If .p41ParentID <> 0 Then
                Me.p41ParentID.Value = .p41ParentID.ToString
                Dim cParent As BO.p41Project = Master.Factory.p41ProjectBL.Load(.p41ParentID)
                Me.p41ParentID.Text = cParent.FullName
            End If

            Me.p87ID.SelectedValue = .p87ID.ToString
            Me.p92id.SelectedValue = .p92ID.ToString
            Me.p41InvoiceMaturityDays.Value = .p41InvoiceMaturityDays
            Me.p41InvoiceDefaultText1.Text = .p41InvoiceDefaultText1
            Me.p41InvoiceDefaultText2.Text = .p41InvoiceDefaultText2
            basUI.SelectDropdownlistValue(Me.p72ID_NonBillable, CInt(.p72ID_NonBillable).ToString)
            basUI.SelectDropdownlistValue(Me.p72ID_BillableHours, CInt(.p72ID_BillableHours).ToString)


            Me.p41LimitHours_Notification.Value = .p41LimitHours_Notification
            Me.p41LimitFee_Notification.Value = .p41LimitFee_Notification
            If .p41LimitHours_Notification > 0 Or .p41LimitFee_Notification > 0 Then Me.chkDefineLimits.Checked = True Else Me.chkDefineLimits.Checked = False
            Me.p41RobotAddress.Text = .p41RobotAddress
            Me.p41ExternalPID.Text = .p41ExternalPID
            basUI.SelectDropdownlistValue(Me.p41WorksheetOperFlag, CInt(.p41WorksheetOperFlag).ToString)
            Me.p41IsNoNotify.Checked = .p41IsNoNotify
            ''Me.p41IsEntryP31ByStranger.Checked = .p41IsEntryP31ByStranger
            Master.Timestamp = .Timestamp & " <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Me.p41BillingMemo.Text = .p41BillingMemo
            
        End With
        roles1.InhaleInitialData(cRec.PID)
        tags1.RefreshData(cRec.PID)

        Handle_FF()

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

    
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p41ProjectBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p41-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub p41_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

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
        Dim intP42ID As Integer = BO.BAS.IsNullInt(Me.p42ID.SelectedValue)
        If intP42ID > 0 Then
            Me.clue_p42.Attributes.Item("rel") = "clue_p42_record.aspx?pid=" & Me.p42ID.SelectedValue : clue_p42.Visible = True
            Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(intP42ID)
            RadTabStrip1.FindTabByValue("billing").Visible = cP42.p42IsModule_p31
            Me.RadMultiPage1.FindPageViewByID("billing").Visible = cP42.p42IsModule_p31
            chkDefineLimits.Visible = cP42.p42IsModule_p31
            Me.p61ID.Visible = cP42.p42IsModule_p31
            lblP61ID.Visible = Me.p61ID.Visible
            If Not chkDefineLimits.Visible Then chkDefineLimits.Checked = False
        Else
            clue_p42.Visible = False
        End If
        Me.panLimits.Visible = Me.chkDefineLimits.Checked

        RefreshState_Pricelist()
      
    End Sub

    Private Sub RefreshState_Pricelist()
        lblP51ID_Billing.Visible = True : Me.p51ID_Billing.Visible = True
        Select Case Me.opgPriceList.SelectedValue
            Case "1"
                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False
                cmdNewP51.Visible = False
                cmdEditP51.Visible = False
            Case "2"
                Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(0)"
                Me.cmdNewP51.Visible = True : cmdEditP51.Visible = True
                If Me.p51ID_Billing.SelectedValue <> "" Then
                    cmdEditP51.NavigateUrl = "javascript:p51_edit(" & Me.p51ID_Billing.SelectedValue & ")"
                    cmdEditP51.Visible = True
                Else
                    cmdEditP51.Visible = False
                End If
            Case "3"
                If BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value) <> 0 Then
                    cmdEditP51.NavigateUrl = "javascript:p51_edit(" & Me.hidP51ID_Tailor.Value & ")"
                    Me.cmdNewP51.Visible = False
                    cmdEditP51.Visible = True
                Else
                    Me.cmdNewP51.Visible = True
                    Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(1)"
                    cmdEditP51.Visible = False
                End If

                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False

        End Select
    End Sub

    Private Sub p42ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p42ID.NeedMissingItem
        Dim cRec As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p42Name
    End Sub

    Private Sub p51ID_Billing_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID_Billing.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p51Name
    End Sub

    
    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        With cRec
            .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
            If chkPlanDates.Checked Then
                .p41PlanFrom = Me.p41PlanFrom.SelectedDate
                .p41PlanUntil = Me.p41PlanUntil.SelectedDate
            Else
                .p41PlanFrom = Nothing
                .p41PlanUntil = Nothing
            End If
            .p41Name = Me.p41Name.Text
            .p41NameShort = Me.p41NameShort.Text
            .p42ID = BO.BAS.IsNullInt(Me.p42ID.SelectedValue)
            .j18ID = BO.BAS.IsNullInt(Me.j18ID.SelectedValue)
            .p61ID = BO.BAS.IsNullInt(Me.p61id.selectedvalue)
            .p28ID_Client = BO.BAS.IsNullInt(Me.p28ID_Client.Value)

            Select Case Me.opgPriceList.SelectedValue
                Case "1"
                    .p51ID_Billing = 0
                Case "2"
                    .p51ID_Billing = BO.BAS.IsNullInt(Me.p51ID_Billing.SelectedValue)
                Case "3"
                    .p51ID_Billing = BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value)
            End Select

            If Me.opgPriceList.SelectedValue <> "1" And .p51ID_Billing = 0 Then
                Master.Notify("Chybí ceník sazeb.", NotifyLevel.WarningMessage) : Return
            End If

            .p51ID_Internal = BO.BAS.IsNullInt(Me.p51ID_Internal.SelectedValue)

            .p87ID = BO.BAS.IsNullInt(Me.p87ID.SelectedValue)
            .p28ID_Billing = BO.BAS.IsNullInt(Me.p28ID_Billing.Value)
            .p92ID = BO.BAS.IsNullInt(Me.p92id.SelectedValue)
            .p41InvoiceMaturityDays = BO.BAS.IsNullInt(Me.p41InvoiceMaturityDays.Value)
            .p72ID_NonBillable = BO.BAS.IsNullInt(Me.p72ID_NonBillable.SelectedValue)
            .p72ID_BillableHours = BO.BAS.IsNullInt(Me.p72ID_BillableHours.SelectedValue)

            .p41InvoiceDefaultText1 = Me.p41InvoiceDefaultText1.Text
            .p41InvoiceDefaultText2 = Me.p41InvoiceDefaultText2.Text

            If Me.chkDefineLimits.Checked Then
                .p41LimitHours_Notification = BO.BAS.IsNullNum(Me.p41LimitHours_Notification.Value)
                .p41LimitFee_Notification = BO.BAS.IsNullNum(Me.p41LimitFee_Notification.Value)
            Else
                .p41LimitHours_Notification = 0 : .p41LimitFee_Notification = 0
            End If
            .p41RobotAddress = Me.p41RobotAddress.Text
            .p41ExternalPID = Me.p41ExternalPID.Text
            .p41WorksheetOperFlag = CType(p41WorksheetOperFlag.SelectedValue, BO.p41WorksheetOperFlagEnum)
            .p41IsNoNotify = Me.p41IsNoNotify.Checked
            ''.p41IsEntryP31ByStranger = Me.p41IsEntryP31ByStranger.Checked

            .ValidFrom = Master.RecordValidFrom
            .ValidUntil = Master.RecordValidUntil

            .p41ParentID = BO.BAS.IsNullInt(Me.p41ParentID.Value)
            .p41BillingMemo = Trim(Me.p41BillingMemo.Text)
            
        End With

        Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
        If roles1.ErrorMessage <> "" Then
            Master.Notify(roles1.ErrorMessage, 2)
            Return
        End If
        Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()

        With Master.Factory.p41ProjectBL
            If .Save(cRec, Nothing, Nothing, lisX69, lisFF) Then
                Master.DataPID = .LastSavedPID
                Master.Factory.o51TagBL.SaveBinding("p41", Master.DataPID, tags1.Geto51IDs())
                Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p41Project, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                Master.CloseAndRefreshParent("p41-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j18ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j18ID.NeedMissingItem
        Dim cRec As BO.j18Region = Master.Factory.j18RegionBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.j18Name
    End Sub

    
   
    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click
        Dim strPID As String = Me.HardRefreshPID.Value

        Select Case Me.HardRefreshFlag.Value
            Case "p51-save"
                SetupPricelistCombo()
                Me.p51ID_Billing.SelectedValue = strPID
                Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strPID))
                If cRec.p51IsCustomTailor Then
                    hidP51ID_Tailor.Value = cRec.PID.ToString
                    Me.opgPriceList.SelectedValue = "3"
                Else
                    hidP51ID_Tailor.Value = ""
                    Me.opgPriceList.SelectedValue = "2"
                End If

            Case "p51-delete"
                SetupPricelistCombo()
            
        End Select

        Me.HardRefreshPID.Value = ""
        Me.HardRefreshFlag.Value = ""
    End Sub

    

    Private Sub p51ID_Internal_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID_Internal.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p51Name
    End Sub

    Private Sub p42ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p42ID.SelectedIndexChanged
        Handle_FF()
    End Sub
End Class