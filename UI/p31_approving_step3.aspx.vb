Imports Telerik.Web.UI

Public Class p31_approving_step3
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _IsShowSubform As Boolean = False

    Private Sub p31_approving_step3_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_approving_dialog"
        _IsShowSubform = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_ApprovingDialog)
    End Sub
    
    Public ReadOnly Property CurrentMasterPrefix As String
        Get
            Return Me.hidMasterPrefix.Value
        End Get
    End Property
    Public ReadOnly Property CurrentMasterPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterPID.Value)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                Me.hidApprovingLevel.Value = Request.Item("approving_level")
                Me.hidMasterPrefix.Value = Request.Item("masterprefix")
                Me.hidMasterPID.Value = Request.Item("masterpid")
                ViewState("guid") = Request.Item("guid")
                ViewState("guid_err") = BO.BAS.GetGUID
                If ViewState("guid") = "" Then .StopPage("guid is missing")
                ViewState("approvingset") = Server.UrlDecode(Request.Item("approvingset"))

                Dim s As String = "Schvalování worksheet úkonů"
                If Request.Item("clearapprove") = "1" Then
                    s = "Vyčištění schvalovacího příznaku"
                End If
                If Me.CurrentMasterPrefix <> "" Then
                    .HeaderText = s & " | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix), Me.CurrentMasterPID)
                Else
                    .HeaderText = s
                End If

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(designer1.x36Key)
                    .Add("p31_approving-use_internal_approving")
                    .Add("p31_approving-group")
                    .Add("p31_approving-autofilter")
                    .Add("p31_approving-defapprovesetup")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                If .Factory.j03UserBL.GetUserParam("p31_approving-group") <> "" Then
                    basUI.SelectRadiolistValue(Me.opgGroupBy, .Factory.j03UserBL.GetUserParam("p31_approving-group"))
                End If
                Me.chkUseInternalApproving.Checked = BO.BAS.BG(.Factory.j03UserBL.GetUserParam("p31_approving-use_internal_approving", "0"))
                Me.chkAutoFilter.Checked = BO.BAS.BG(.Factory.j03UserBL.GetUserParam("p31_approving-autofilter", "0"))
                Me.chkDefaultApproveSetup.Checked = BO.BAS.BG(.Factory.j03UserBL.GetUserParam("p31_approving-defapprovesetup", "1"))

                Dim cB As New clsToolBarButton("Uložit a zavřít", "save_close")
                'cB.ImageURL = "Images/save.png"
                cB.GroupText = "groupsave"
                .AddToolbarButton(cB)
                ' .AddToolbarButton("Uložit a zavřít", "save_close", , "Images/save.png")



                .AddToolbarButton("Sestava", "report", 1, "Images/report.png", False, "javascript:report()")

                .AddToolbarButton("Nový úkon", "p31_create", 1, "Images/worksheet.png", False, "javascript:p31_create('" & Me.CurrentMasterPrefix & "id'," & Me.CurrentMasterPID.ToString & ")")

                .AddToolbarButton("Nastavení", "setting", 1, "Images/arrow_down.gif", False)
                ''.AddToolbarButton("Fakturační poznámka", "o23", 1, "Images/arrow_down.gif", False)

                If _IsShowSubform Then
                    .AddToolbarButton("Editovatelná tabulka", "", 1, , False, "javascript:batch_p31text()")
                End If
                .AddToolbarButton("Další hromadné operace", "batchoper", 1, "Images/batch.png", False)
                .AddToolbarButton("Hromadně nahodit status", "approvebatch", 1, "Images/approve.png", False)


                If .Factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator) Or .Factory.TestPermission(BO.x53PermValEnum.GR_P91_Draft_Creator) Then
                    '.AddToolbarButton("Uložit a pokračovat fakturací", "save_invoice", 1, "Images/save.png")
                    cB = New clsToolBarButton("Uložit a pokračovat fakturací", "save_invoice")
                    cB.GroupText = "groupsave"
                    .AddToolbarButton(cB)
                End If

                If Me.CurrentMasterPrefix <> "" Then
                    '.AddToolbarButton("Uložit", "save_gate", 1, "Images/save.png")
                    cB = New clsToolBarButton("Uložit", "save_gate")
                    cB.GroupText = "groupsave"
                    .AddToolbarButton(cB)
                End If

                .RadToolbar.FindItemByValue("approvebatch").CssClass = "show_hide1"
                .RadToolbar.FindItemByValue("batchoper").CssClass = "show_hide2"
                .RadToolbar.FindItemByValue("setting").CssClass = "show_hide3"


                cB = New clsToolBarButton("Uložit jako billing dávku", "save_set")
                cB.NavigateURL = "javascript:SaveAsSet()"
                cB.AutoPostback = False
                cB.GroupText = "groupsave"
                .AddToolbarButton(cB)

            End With
            designer1.RefreshData(BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam(designer1.x36Key)))

            If Request.Item("reloadonly") = "" Then
                SetupTempData()
            End If


            SetupGrid()

            RefreshRecord()

        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            bm1.RefreshData(Master.Factory, Me.CurrentMasterPrefix, BO.BAS.IsNullInt(Me.CurrentMasterPID))

            If bm1.IsEmpty Then
                cmdBillingMemo.Style.Item("display") = "none"
            Else
                cmdBillingMemo.Style.Item("display") = "block"
            End If
        End If
        
    End Sub

    Private Sub BatchOper_P72(p71id As BO.p71IdENUM, explicit_p72id As BO.p72IdENUM)
        Dim lisPIDs As List(Of Integer) = grid1.GetSelectedPIDs()
        If lisPIDs.Count = 0 Then
            Master.Notify("Musíte vybrat (označit) alespoň jeden záznam (úkon).", NotifyLevel.WarningMessage)
            Return
        End If
        If p71id = BO.p71IdENUM.Nic Then explicit_p72id = BO.p72IdENUM._NotSpecified
        Dim sx As New System.Text.StringBuilder, xx As Integer = 0

        For Each intP31ID As Integer In lisPIDs
            xx += 1
            Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(intP31ID)
            Dim cApprove As New BO.p31WorksheetApproveInput(cRec.PID, cRec.p33ID)
            With cApprove
                .GUID_TempData = ViewState("guid")
                .p31Date = cRec.p31Date
                ''.p71id = BO.p71IdENUM.Schvaleno
                .p71id = p71id
                .p72id = explicit_p72id
                .p31ApprovingSet = cRec.p31ApprovingSet
                .p31ApprovingLevel = BO.BAS.IsNullInt(Me.hidApprovingLevel.Value)
                If explicit_p72id = BO.p72IdENUM.Fakturovat Or explicit_p72id = BO.p72IdENUM.FakturovatPozdeji Then
                    Select Case cRec.p33ID
                        Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                            .Rate_Billing_Approved = cRec.p31Rate_Billing_Orig

                    End Select
                    .Value_Approved_Billing = cRec.p31Value_Orig

                End If
                If Me.chkUseInternalApproving.Checked And (cRec.p33ID = BO.p33IdENUM.Cas Or cRec.p33ID = BO.p33IdENUM.Kusovnik) Then
                    'interní schvalování
                    If .Value_Approved_Internal = 0 And .Rate_Internal_Approved = 0 Then

                        .Value_Approved_Internal = cRec.p31Value_Orig
                        .Rate_Internal_Approved = cRec.p31Rate_Internal_Orig
                    End If
                End If
            End With

            Dim cErr As New BO.p85TempBox
            cErr.p85GUID = ViewState("guid_err")
            cErr.p85DataPID = cRec.PID

            If Not Master.Factory.p31WorksheetBL.Save_Approving(cApprove, True) Then
                cErr.p85FreeText01 = Master.Factory.p31WorksheetBL.ErrorMessage
                sx.Append("<br>#" & xx.ToString & ": " & Master.Factory.p31WorksheetBL.ErrorMessage)
            Else
                cErr.p85FreeText01 = ""
            End If

            Master.Factory.p85TempBoxBL.Save(cErr)
        Next

        grid1.Rebind(False)
        With grid1.radGridOrig
            For Each intPID In lisPIDs
                For Each it As GridDataItem In .MasterTableView.Items
                    If it.GetDataKeyValue("pid") = intPID Then
                        it.Selected = True : Exit For
                    End If
                Next
            Next
        End With
        hiddatapid.Value = lisPIDs(lisPIDs.Count - 1).ToString
        RefreshSubform(hiddatapid.Value)

        If sx.ToString <> "" Then
            Master.Notify(sx.ToString, NotifyLevel.ErrorMessage)
        End If
    End Sub
    Private Sub BatchOper_Levels(intLevel As Integer)
        Dim lisPIDs As List(Of Integer) = grid1.GetSelectedPIDs()
        If lisPIDs.Count = 0 Then
            Master.Notify("Musíte vybrat (označit) alespoň jeden záznam (úkon).", NotifyLevel.WarningMessage)
            Return
        End If
        For Each intP31ID As Integer In lisPIDs
            Master.Factory.p31WorksheetBL.UpdateTempField("p31ApprovingLevel", intLevel, ViewState("guid"), intP31ID)
        Next

        grid1.Rebind(False)
        With grid1.radGridOrig
            For Each intPID In lisPIDs
                For Each it As GridDataItem In .MasterTableView.Items
                    If it.GetDataKeyValue("pid") = intPID Then
                        it.Selected = True : Exit For
                    End If
                Next
            Next
        End With
        hiddatapid.Value = lisPIDs(lisPIDs.Count - 1).ToString
        RefreshSubform(hiddatapid.Value)

        
    End Sub
    Private Sub SetupTempData()
        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85Prefix = ""), intLastP41ID As Integer = 0
        For Each cTemp In lis
            Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(cTemp.p85DataPID)
            Dim bolOK As Boolean = True, intP41ID As Integer = cRec.p41ID
            If cRec.p91ID > 0 Or cRec.p31IsPlanRecord Then bolOK = False
            If bolOK Then
                Dim cApprove As New BO.p31WorksheetApproveInput(cRec.PID, cRec.p33ID)
                With cApprove
                    .GUID_TempData = ViewState("guid")
                    .p31ApprovingSet = cRec.p31ApprovingSet
                    .p31ApprovingLevel = cRec.p31ApprovingLevel
                    If ViewState("approvingset") <> "" And .p31ApprovingSet = "" Then
                        .p31ApprovingSet = ViewState("approvingset")    'dosud nezařazený záznam zařadit do dávky
                    End If

                    If cRec.p71ID = BO.p71IdENUM.Nic Then
                        'dosud neprošlo schvalováním
                        .Rate_Internal_Approved = cRec.p31Rate_Internal_Orig
                        .p31ApprovingLevel = BO.BAS.IsNullInt(Me.hidApprovingLevel.Value)
                        If Me.chkDefaultApproveSetup.Checked Then   'pokud je nastaveno, že se má nahodit výchozí fakturační status
                            .p71id = BO.p71IdENUM.Schvaleno
                            If cRec.p32IsBillable Then
                                .p72id = BO.p72IdENUM.Fakturovat
                                .Value_Approved_Billing = cRec.p31Value_Orig
                                .Value_Approved_Internal = cRec.p31Value_Orig

                                Select Case .p33ID
                                    Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                                        .Rate_Billing_Approved = cRec.p31Rate_Billing_Orig
                                        .VatRate_Approved = cRec.p31VatRate_Orig
                                        If (cRec.p31Rate_Billing_Orig = 0 And cRec.p32ManualFeeFlag = 0) Or (cRec.p31Amount_WithoutVat_Orig = 0 And cRec.p32ManualFeeFlag = 1) Then
                                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                                        End If
                                        If cRec.p32ManualFeeFlag = 1 Then
                                            .p32ManualFeeFlag = 1
                                            .ManualFee_Approved = cRec.p31Amount_WithoutVat_Orig
                                        End If
                                        
                                        If cRec.p72ID_AfterTrimming > BO.p72IdENUM._NotSpecified Then
                                            'uživatel zadal v úkonu výchozí korekci pro schvalování
                                            .p72id = cRec.p72ID_AfterTrimming
                                            If .p72id = BO.p72IdENUM.Fakturovat Then
                                                .Value_Approved_Billing = cRec.p31Value_Trimmed
                                            Else
                                                .Rate_Billing_Approved = 0
                                                .Value_Approved_Billing = 0
                                            End If
                                        Else
                                            'fakturovatelné hodiny mohou být záměrně nulovány do paušálu nebo odpisu
                                            If intP41ID <> intLastP41ID Then
                                                Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(intP41ID)
                                                If cP41.p72ID_BillableHours > BO.p72IdENUM._NotSpecified Then
                                                    .p72id = cP41.p72ID_BillableHours   'v projektu je explicitně nastavený fakturační status, kterým se mají nulovat fakturovatelné hodiny
                                                End If
                                            End If
                                        End If
                                    Case BO.p33IdENUM.PenizeBezDPH
                                        If cRec.p31Value_Orig = 0 Then
                                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                                        End If
                                    Case BO.p33IdENUM.PenizeVcDPHRozpisu
                                        .VatRate_Approved = cRec.p31VatRate_Orig
                                        If cRec.p31Value_Orig = 0 Then
                                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                                        End If
                                End Select
                            Else
                                .Value_Approved_Internal = cRec.p31Value_Orig
                                If cRec.p72ID_AfterTrimming = BO.p72IdENUM._NotSpecified Or cRec.p72ID_AfterTrimming = BO.p72IdENUM.Fakturovat Then
                                    ''.p72id = BO.p72IdENUM.SkrytyOdpis
                                    .p72id = Master.Factory.p31WorksheetBL.Get_p72ID_NonBillableWork(cApprove.p31ID)
                                Else
                                    .p72id = cRec.p72ID_AfterTrimming
                                End If

                            End If
                        End If
                        
                    Else
                        'již dříve schválený záznam
                        .p71id = cRec.p71ID
                        .p72id = cRec.p72ID_AfterApprove
                        .Value_Approved_Billing = cRec.p31Value_Approved_Billing
                        .Value_Approved_Internal = cRec.p31Value_Approved_Internal
                        .VatRate_Approved = cRec.p31VatRate_Approved
                        .Rate_Billing_Approved = cRec.p31Rate_Billing_Approved
                        .Rate_Internal_Approved = cRec.p31Rate_Internal_Approved
                        .p32ManualFeeFlag = cRec.p32ManualFeeFlag
                        If cRec.p32ManualFeeFlag = 1 Then
                            .ManualFee_Approved = cRec.p31Amount_WithoutVat_Approved
                        End If
                        If .p72id = BO.p72IdENUM.ZahrnoutDoPausalu Then
                            .p31Value_FixPrice = cRec.p31Value_FixPrice
                        End If

                    End If
                    .p31Date = cRec.p31Date
                End With
                If Not Master.Factory.p31WorksheetBL.Save_Approving(cApprove, True) Then
                    Dim cErr As New BO.p85TempBox
                    cErr.p85GUID = ViewState("guid_err")
                    cErr.p85DataPID = cRec.PID
                    cErr.p85FreeText01 = Master.Factory.p31WorksheetBL.ErrorMessage
                    Master.Factory.p85TempBoxBL.Save(cErr)
                End If
            End If

            intLastP41ID = intP41ID
        Next


    End Sub

    Private Sub SetupGrid()
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
        With cS
            .PageSize = 5000
            .AllowCustomPaging = False
            .AllowMultiSelect = True
            .AllowMultiSelectCheckboxSelector = True
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        Me.hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM

        'Dim strF As String = ""
        'Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, 5000, False, True, , , , , strF)
        'Me.hidFrom.Value = strF

        ''If Not Page.IsPostBack Then
        ''    Dim intGridHeight As Integer = BO.BAS.IsNullInt(Request.Item("gridheight").Replace(".", ","))
        ''    Dim intFraHeight As Integer = 500
        ''    If intGridHeight > 0 Then intFraHeight = intGridHeight + 50
        ''    Dim strGridHeight As String = "75%", strFraheight As String = "80%"
        ''    If intGridHeight > 0 Then
        ''        strGridHeight = intGridHeight.ToString & "px"
        ''        strFraheight = intFraHeight.ToString & "px"
        ''    End If

        ''    ''fraSubform.Attributes.Item("height") = strFraheight
        ''    ''RadMultiPage1.PageViews(0).Height = Unit.Parse(strFraheight)
        ''End If


        grid1.radGridOrig.MasterTableView.AllowFilteringByColumn = Me.chkAutoFilter.Checked

        Dim strGroupField As String = Me.opgGroupBy.SelectedValue
        If Me.opgGroupBy.SelectedValue = "" Then
            With grid1.radGridOrig.MasterTableView
                .ShowGroupFooter = False
                .GroupByExpressions.Clear()
            End With
            Return
        End If
        Dim strHeaderText As String = Me.opgGroupBy.SelectedItem.Text

        With grid1.radGridOrig.MasterTableView
            .ShowGroupFooter = True
            Dim GGE As New Telerik.Web.UI.GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strHeaderText

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)

        End With


    End Sub

  

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, True, False, "p31_approving_step3")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim mq As New BO.myQueryP31
        With mq
            .MG_PageSize = 0
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()

        End With
        InhaleMyQuery(mq)

        ''Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq, ViewState("guid"))

        ''grid1.DataSource = lis

        Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridDataSource(mq, ViewState("guid"))
        If dt Is Nothing Then
            Master.Notify(Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
            Return
        Else
            grid1.DataSourceDataTable = dt
        End If

        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq, ViewState("guid"))
        RecalcVirtualRowCount(lis)

        Dim sets As List(Of String) = Master.Factory.p31WorksheetBL.GetList_ApprovingSet(ViewState("guid"), Nothing, Nothing)
        With Me.p31ApprovingSet
            .Items.Clear()
            For Each s In sets
                .Items.Add(New Telerik.Web.UI.RadComboBoxItem(s))
            Next
        End With
    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)
        With mq
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .MG_GridGroupByField = opgGroupBy.SelectedValue
        End With
    End Sub

    

    
    Private Sub RefreshSubform(strPID As String)
        If strPID = "" Or _IsShowSubform = False Then Return

        Me.fraSubform.Attributes.Item("src") = "p31_approving_step3_subform.aspx?pid=" & strPID & "&guid=" & ViewState("guid")

    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click

        If Me.hidHardRefreshFlag.Value = "" And Me.hidHardRefreshPID.Value = "" Then Return

        Select Case Me.hidHardRefreshFlag.Value
            Case "j70-run"

                SetupGrid()
                grid1.Rebind(True)
                RefreshSubform(Me.hiddatapid.Value)
            Case "p31text"

                grid1.Rebind(True, BO.BAS.IsNullInt(Me.hidHardRefreshPID.Value))
            Case "save_as_set"
                If Not SaveChanges(Me.hidApprovingSet_Explicit.Value) Then Return
                Master.CloseAndRefreshParent("approving")
            Case Else
                RefreshSubform(Me.hidHardRefreshPID.Value)
                grid1.Rebind(True, BO.BAS.IsNullInt(Me.hidHardRefreshPID.Value))

        End Select

        RefreshRecord()


        Me.hidHardRefreshPID.Value = ""
        Me.hidHardRefreshFlag.Value = ""
    End Sub
    Private Sub RecalcVirtualRowCount(lis As IEnumerable(Of BO.p31Worksheet))
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)


        Dim cSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, False, False, ViewState("guid"))
        If Not cSum Is Nothing Then
            'grid1.VirtualRowCount = cSum.RowsCount

            ViewState("footersum") = grid1.GenerateFooterItemString(cSum)

            hours_billable_orig.Text = BO.BAS.FNI(cSum.p31Hours_Orig)
            hours_4.Text = BO.BAS.FN(cSum.p31Hours_Approved_Billing)
        Else
            ViewState("footersum") = ""
            grid1.VirtualRowCount = 0
        End If

        Me.RowCount.Text = BO.BAS.FNI(lis.Count)
        Dim dblHours_Orig_Billable As Double = lis.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas And p.p32IsBillable = True).Select(Function(p) p.p31Hours_Orig).Sum()
        Me.hours_billable_orig.Text = BO.BAS.FN(dblHours_Orig_Billable)

        Dim dblHours_3 As Double = lis.Where(Function(p) p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.SkrytyOdpis).Select(Function(p) p.p31Hours_Orig).Sum
        hours_3.Text = BO.BAS.FN(dblHours_3)
        Dim dblHours_6 As Double = lis.Where(Function(p) p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.ZahrnoutDoPausalu).Select(Function(p) p.p31Hours_Orig).Sum
        hours_6.Text = BO.BAS.FN(dblHours_6)
        Dim dblHours_2 As Double = lis.Where(Function(p) p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.ViditelnyOdpis).Select(Function(p) p.p31Hours_Orig).Sum
        hours_2.Text = BO.BAS.FN(dblHours_2)

        Me.RowsCount_Approved.Text = BO.BAS.FNI(lis.Where(Function(p) p.p71ID = BO.p71IdENUM.Schvaleno).Count).ToString

        Dim lisJ27_Time As IEnumerable(Of String) = lis.Where(Function(p) p.j27ID_Billing_Orig > 0 And p.p32IsBillable = True And p.p33ID = BO.p33IdENUM.Cas).Select(Function(p) p.j27Code_Billing_Orig).Distinct
        Dim x As Integer = 0
        For Each strJ27Code In lisJ27_Time
            Dim s0 As String = BO.BAS.FN(lis.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas And p.j27Code_Billing_Orig = strJ27Code).Select(Function(p) p.p31Amount_WithoutVat_Orig).Sum) & " " & strJ27Code
            Dim s1 As String = BO.BAS.FN(lis.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas And p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.Fakturovat And p.j27Code_Billing_Orig = strJ27Code).Select(Function(p) p.p31Amount_WithoutVat_Approved).Sum) & " " & strJ27Code
            If x = 0 Then
                fee_billable_orig.Text = " -> " & s0
                fee_4.Text = " -> " & s1
            Else
                fee_billable_orig.Text += " + " & s0
                fee_4.Text += " + " & s1
            End If
            x += 1
        Next

        imgProfitLost_Time.Visible = False
        profit_lost_time.Visible = False
        If lisJ27_Time.Count = 1 Then
            Dim dblFeeOrig As Double = lis.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas).Select(Function(p) p.p31Amount_WithoutVat_Orig).Sum
            Dim dblFee4 As Double = lis.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas And p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.Fakturovat).Select(Function(p) p.p31Amount_WithoutVat_Approved).Sum
            If dblFee4 - dblFeeOrig <> 0 Then
                imgProfitLost_Time.Visible = True : profit_lost_time.Visible = True
            End If
            Select Case dblFee4 - dblFeeOrig
                Case Is > 0
                    imgProfitLost_Time.ImageUrl = "Images/correction_up.gif"
                    profit_lost_time.Text = BO.BAS.FN(dblFee4 - dblFeeOrig) : profit_lost_time.ForeColor = System.Drawing.Color.Blue
                Case Is < 0
                    imgProfitLost_Time.ImageUrl = "Images/correction_down.gif"
                    profit_lost_time.Text = BO.BAS.FN(dblFee4 - dblFeeOrig) : profit_lost_time.ForeColor = System.Drawing.Color.Red
            End Select
        End If

        Dim lisJ27_Other As IEnumerable(Of String) = lis.Where(Function(p) p.j27ID_Billing_Orig > 0 And p.p32IsBillable = True And p.p33ID <> BO.p33IdENUM.Cas).Select(Function(p) p.j27Code_Billing_Orig).Distinct
        x = 0
        For Each strJ27Code In lisJ27_Other
            Dim s0 As String = BO.BAS.FN(lis.Where(Function(p) p.p33ID <> BO.p33IdENUM.Cas And p.j27Code_Billing_Orig = strJ27Code).Select(Function(p) p.p31Amount_WithoutVat_Orig).Sum) & " " & strJ27Code
            Dim s1 As String = BO.BAS.FN(lis.Where(Function(p) p.p33ID <> BO.p33IdENUM.Cas And p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.Fakturovat And p.j27Code_Billing_Orig = strJ27Code).Select(Function(p) p.p31Amount_WithoutVat_Approved).Sum) & " " & strJ27Code
            If x = 0 Then
                other_income_orig.Text = s0
                other_income_approved.Text = s1
            Else
                other_income_orig.Text += " + " & s0
                other_income_approved.Text += " + " & s1
            End If
            x += 1
        Next
        If lisJ27_Other.Count = 1 Then
            Dim dblOrig As Double = lis.Where(Function(p) p.p33ID <> BO.p33IdENUM.Cas).Select(Function(p) p.p31Amount_WithoutVat_Orig).Sum
            Dim dblApproved As Double = lis.Where(Function(p) p.p33ID <> BO.p33IdENUM.Cas And p.p71ID = BO.p71IdENUM.Schvaleno And p.p72ID_AfterApprove = BO.p72IdENUM.Fakturovat).Select(Function(p) p.p31Amount_WithoutVat_Approved).Sum
            If dblApproved - dblOrig <> 0 Then
                imgProfitLost_Other.Visible = True : profit_lost_other.Visible = True
            End If
            Select Case dblApproved - dblOrig
                Case Is > 0
                    imgProfitLost_Other.ImageUrl = "Images/correction_up.gif"
                    profit_lost_other.Text = BO.BAS.FN(dblApproved - dblOrig) : profit_lost_time.ForeColor = System.Drawing.Color.Blue
                Case Is < 0
                    imgProfitLost_Time.ImageUrl = "Images/correction_down.gif"
                    profit_lost_time.Text = BO.BAS.FN(dblApproved - dblOrig) : profit_lost_time.ForeColor = System.Drawing.Color.Red
            End Select
        End If


        'grid1.VirtualRowCount = Master.Factory.p31WorksheetBL.GetVirtualCount(mq)
        'grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub grid1_NeedFooterSource(footerItem As Telerik.Web.UI.GridFooterItem, footerDatasource As Object) Handles grid1.NeedFooterSource
        footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"


        grid1.ParseFooterItemString(footerItem, ViewState("footersum"))

        If Not Page.IsPostBack Then
            If grid1.GetSelectedPIDs.Count = 0 And grid1.RowsCount > 0 Then
                Dim lis As New List(Of Integer)
                lis.Add(grid1.GetAllPIDs(0))
                grid1.SelectRecords(lis)
                RefreshSubform(lis(0).ToString)
            End If
        End If

    End Sub

    Private Sub cmdBatch_2_Click(sender As Object, e As EventArgs) Handles cmdBatch_2.Click
        BatchOper_P72(BO.p71IdENUM.Schvaleno, BO.p72IdENUM.ViditelnyOdpis)
    End Sub

    Private Sub cmdBatch_3_Click(sender As Object, e As EventArgs) Handles cmdBatch_3.Click
        BatchOper_P72(BO.p71IdENUM.Schvaleno, BO.p72IdENUM.SkrytyOdpis)
    End Sub

    Private Sub cmdBatch_4_Click(sender As Object, e As EventArgs) Handles cmdBatch_4.Click
        BatchOper_P72(BO.p71IdENUM.Schvaleno, BO.p72IdENUM.Fakturovat)
    End Sub

    Private Sub cmdBatch_6_Click(sender As Object, e As EventArgs) Handles cmdBatch_6.Click
        BatchOper_P72(BO.p71IdENUM.Schvaleno, BO.p72IdENUM.ZahrnoutDoPausalu)
    End Sub
    Private Sub cmdBatch_7_Click(sender As Object, e As EventArgs) Handles cmdBatch_7.Click
        BatchOper_P72(BO.p71IdENUM.Schvaleno, BO.p72IdENUM.FakturovatPozdeji)
    End Sub

    Private Sub cmdBatch_Clear_Click(sender As Object, e As EventArgs) Handles cmdBatch_Clear.Click
        BatchOper_P72(BO.p71IdENUM.Nic, BO.p72IdENUM._NotSpecified)
    End Sub


    Private Sub chkUseInternalApproving_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseInternalApproving.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving-use_internal_approving", BO.BAS.GB(Me.chkUseInternalApproving.Checked))
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "save_close"
                If Not SaveChanges("") Then Return
                Master.CloseAndRefreshParent("approving")
            Case "save_gate"
                If Not SaveChanges("") Then Return
                Dim cTEMP As BO.p85TempBox = Master.Factory.p85TempBoxBL.LoadByGUID(ViewState("guid") & "-00")
                Response.Redirect("entity_modal_approving.aspx?" & cTEMP.p85Message)
            Case "save_invoice"
                If Not SaveChanges("") Then Return
                Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85Prefix = ""), strGUID As String = BO.BAS.GetGUID(), x As Integer = 0
                For Each cTemp In lis
                    Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(cTemp.p85DataPID)
                    If cRec.p71ID = BO.p71IdENUM.Schvaleno And cRec.p72ID_AfterApprove <> BO.p72IdENUM.FakturovatPozdeji And cRec.p72ID_AfterApprove <> BO.p72IdENUM._NotSpecified And cRec.p91ID = 0 Then
                        Dim c As New BO.p85TempBox
                        c.p85GUID = strGUID
                        c.p85Prefix = "p31"
                        c.p85DataPID = cRec.PID
                        Master.Factory.p85TempBoxBL.Save(c)
                        x += 1
                    End If
                Next
                If x = 0 Then Master.Notify("Pro fakturaci není vhodný ani jeden úkon!", NotifyLevel.WarningMessage) : Return
                Dim s As String = "p91_create_step2.aspx?guid=" & strGUID & "&prefix=" & Me.CurrentMasterPrefix & "&pid=" & Me.CurrentMasterPID
                Response.Redirect(s)
        End Select
       
    End Sub

    Private Function SaveChanges(strApprovingSet As String) As Boolean
        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85Prefix = ""), x As Integer = 0
        Dim strErrs As String = ""
        For Each cTemp In lis
            Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadTempRecord(cTemp.p85DataPID, ViewState("guid"))
            If cRec Is Nothing Then
                Master.Notify(String.Format("Metoda [LoadTempRecord], chyba: cTemp.p85DataPID={0}, guid={1}", cTemp.p85DataPID, ViewState("guid")), NotifyLevel.ErrorMessage)
                Return False
            End If
            Dim cApprove As New BO.p31WorksheetApproveInput(cRec.PID, cRec.p33ID)

            With cApprove
                .p71id = cRec.p71ID
                .p72id = cRec.p72ID_AfterApprove
                If strApprovingSet = "" Then
                    .p31ApprovingSet = cRec.p31ApprovingSet
                Else
                    .p31ApprovingSet = strApprovingSet
                End If

                .Value_Approved_Billing = cRec.p31Value_Approved_Billing
                .Value_Approved_Internal = cRec.p31Value_Approved_Internal
                Select Case cRec.p33ID
                    Case BO.p33IdENUM.Kusovnik, BO.p33IdENUM.Cas
                        .Rate_Internal_Approved = cRec.p31Rate_Internal_Approved
                        .VatRate_Approved = cRec.p31VatRate_Approved
                        If .p72id = BO.p72IdENUM.ZahrnoutDoPausalu Then
                            .p31Value_FixPrice = cRec.p31Value_FixPrice
                        Else
                            .p31Value_FixPrice = 0
                        End If
                        If cRec.p32ManualFeeFlag = 1 Then
                            .p32ManualFeeFlag = 1
                            .ManualFee_Approved = cRec.p31Amount_WithoutVat_Approved
                        Else
                            .Rate_Billing_Approved = cRec.p31Rate_Billing_Approved
                        End If
                    Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                        .VatRate_Approved = cRec.p31VatRate_Approved
                End Select
                .p31Text = cRec.p31Text
                .p31Date = cRec.p31Date
                .p31ApprovingLevel = cRec.p31ApprovingLevel
            End With

            With Master.Factory.p31WorksheetBL
                If .Save_Approving(cApprove, False) Then
                    x += 1
                End If
            End With
        Next
        If x > 0 Then Master.Factory.p31WorksheetBL.SaveFreeFields_Batch_AfterApproving(ViewState("guid"))
        Return True
    End Function

    Private Sub p31_approving_step3_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Not _IsShowSubform Then
            td1.Style.Clear()   'uživatel nemá právo provádět korekce
            td1.Visible = False
        End If

        If Not Page.IsPostBack Then
            If Request.Item("clearapprove") = "1" Then
                'Žádost z aplikace o hromadné vyčištění
                Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(New BO.myQueryP31, ViewState("guid"))
                Dim p31ids As List(Of Integer) = lisP31.Select(Function(p) p.PID).ToList
                If p31ids.Count = 0 Then
                    Master.Notify("Na vstupu není ani jeden schválený úkon.", NotifyLevel.WarningMessage)
                    Return
                End If
                grid1.SelectRecords(p31ids)
                BatchOper_P72(BO.p71IdENUM.Nic, BO.p72IdENUM._NotSpecified)
                RecalcVirtualRowCount(lisP31)
                Master.Notify("Vyčištění schvalovacího příznaku u vybraných úkonů potvrdíte tlačítkem [Uložit změny].", NotifyLevel.InfoMessage)
            End If
        End If
    End Sub


    Private Sub opgGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgGroupBy.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving-group", Me.opgGroupBy.SelectedValue)
        SetupGrid()
        grid1.Rebind(True, BO.BAS.IsNullInt(Me.hiddatapid.Value))
        RefreshSubform(Me.hiddatapid.Value)
    End Sub

    Private Sub chkAutoFilter_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoFilter.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving-autofilter", BO.BAS.GB(Me.chkAutoFilter.Checked))
        SetupGrid()
        grid1.Rebind(True, BO.BAS.IsNullInt(Me.hiddatapid.Value))
    End Sub
    Private Sub chkDefaultApproveSetup_CheckedChanged(sender As Object, e As EventArgs) Handles chkDefaultApproveSetup.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving-defapprovesetup", BO.BAS.GB(Me.chkDefaultApproveSetup.Checked))
        Master.Notify("Nastavení se projeví až v příštím schvalování.", NotifyLevel.InfoMessage)
    End Sub

    
    Private Sub cmdBatch_ApprovingSet_Click(sender As Object, e As EventArgs) Handles cmdBatch_ApprovingSet.Click
        Dim strName As String = Trim(Me.p31ApprovingSet.Text)
        If strName = "" Then
            Master.Notify("Musíte specifikovat název billing dávky.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim p31ids As List(Of Integer) = grid1.GetSelectedPIDs()
       
        Master.Factory.p31WorksheetBL.UpdateDeleteApprovingSet(strName, p31ids, False, ViewState("guid"))
        grid1.Rebind(True)
    End Sub

    Private Sub cmdBatch_ApprovingSet_Clear_Click(sender As Object, e As EventArgs) Handles cmdBatch_ApprovingSet_Clear.Click
        Dim p31ids As List(Of Integer) = grid1.GetSelectedPIDs()
        Master.Factory.p31WorksheetBL.UpdateDeleteApprovingSet("", p31ids, True, ViewState("guid"))
        grid1.Rebind(True)
    End Sub

    
    
    Private Sub cmdBatch_10_Click(sender As Object, e As EventArgs) Handles cmdBatch_10.Click
        BatchOper_Levels(2)
    End Sub

    Private Sub cmdBatch_9_Click(sender As Object, e As EventArgs) Handles cmdBatch_9.Click
        BatchOper_Levels(1)
    End Sub

    Private Sub cmdBatch_8_Click(sender As Object, e As EventArgs) Handles cmdBatch_8.Click
        BatchOper_Levels(0)
    End Sub
End Class