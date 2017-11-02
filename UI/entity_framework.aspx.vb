Imports Telerik.Web.UI
Public Class entity_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    
    Private Property _x29id As BO.x29IdEnum
    Private Property _needFilterIsChanged As Boolean = False
    Private Property _CurFilterDbField As String = ""
    Public Property _curIsExport As Boolean
  

    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            If _x29id = BO.x29IdEnum._NotSpecified Then _x29id = CType(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
            Return _x29id
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property
    Public Property CurrentMasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property
    Public Property CurrentMasterPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterPID.Value)
        End Get
        Set(value As Integer)
            hidMasterPID.Value = value.ToString
        End Set
    End Property
  
    Public Property CurrentJ62ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidJ62ID.Value)
        End Get
        Set(value As Integer)
            hidJ62ID.Value = value.ToString
            If value > 0 Then Master.SiteMenuValue = "hm" & value.ToString
        End Set
    End Property

    Private Sub entity_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            If Request.Item("prefix") <> "" Then
                Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
            End If
            If Request.Item("masterpid") <> "" Then
                Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid")) : Me.CurrentMasterPrefix = Request.Item("masterprefix")
            End If
            Handle_Permissions_And_More()
            SetupPeriodQuery()
            With Master
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(Me.CurrentPrefix + "_framework-pagesize")
                    .Add(Me.CurrentPrefix + "-j70id")
                    .Add(Me.CurrentPrefix + "_framework-navigationPane_width")
                    .Add(Me.CurrentPrefix + "_framework-contentPane_height")
                    .Add(Me.CurrentPrefix + "_framework_detail-pid")
                    .Add(Me.CurrentPrefix + "_framework-groupby")
                    .Add(Me.CurrentPrefix + "_framework-sort")
                    ''.Add(Me.CurrentPrefix + "_framework-groups-autoexpanded")
                    .Add(Me.CurrentPrefix + "_framework-checkbox_selector")
                    .Add("periodcombo-custom_query")
                    .Add(Me.CurrentPrefix + "_framework-periodtype")
                    .Add(Me.CurrentPrefix + "_framework-period")
                    .Add(Me.CurrentPrefix + "_framework-queryflag")
                    .Add(Me.CurrentPrefix + "_framework-filter_setting")
                    .Add(Me.CurrentPrefix + "_framework-filter_sql")
                    .Add(Me.CurrentPrefix + "_framework-layout")
                    .Add("x18_querybuilder-value-" & Me.CurrentPrefix & "-grid")
                    .Add("x18_querybuilder-text-" & Me.CurrentPrefix & "-grid")
                    .Add("o51_querybuilder-" & Me.CurrentPrefix)
                End With
                With cbxGroupBy
                    .DataSource = Master.Factory.j70QueryTemplateBL.GroupByPallet(Me.CurrentX29ID)
                    .DataBind()
                End With
                
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    If basUI.GetCookieValue(Request, "MT50-SAW") = "1" Then
                        Me.opgLayout.Value = "3" : lblLayoutMessage.Visible = True

                    Else

                        lblLayoutMessage.Visible = False
                        opgLayout.Value = .GetUserParam(Me.CurrentPrefix + "_framework-layout", "1")
                        FindNode("layout" & opgLayout.Value).ImageUrl = "Images/checked.png"

                    End If


                    basUI.SelectDropdownlistValue(cbxPaging, .GetUserParam(Me.CurrentPrefix + "_framework-pagesize", "20"))
                    Dim strDefWidth As String = "435"
                    Select Case Me.CurrentPrefix
                        Case "o23", "p56", "p91" : strDefWidth = "600"
                        Case Else
                    End Select
                    Select Case Me.opgLayout.Value
                        Case "1"
                            Dim strW As String = .GetUserParam(Me.CurrentPrefix + "_framework-navigationPane_width", strDefWidth)
                            If strW = "-1" Then
                                Me.navigationPane.Collapsed = True
                            Else
                                Me.navigationPane.Width = Unit.Parse(strW & "px")
                            End If
                        Case "2"
                            Dim strH As String = .GetUserParam(Me.CurrentPrefix + "_framework-contentPane_height", "300")
                            If strH = "-1" Then
                                Me.contentPane.Collapsed = True
                            Else
                                Me.contentPane.Height = Unit.Parse(strH & "px")
                            End If

                    End Select


                    basUI.SelectDropdownlistValue(cbxGroupBy, .GetUserParam(Me.CurrentPrefix + "_framework-groupby"))

                    ''Me.chkGroupsAutoExpanded.Checked = BO.BAS.BG(.GetUserParam(Me.CurrentPrefix + "_framework-groups-autoexpanded", "1"))
                    chkCheckboxSelector.SelectedValue = .GetUserParam(Me.CurrentPrefix + "_framework-checkbox_selector", "1")
                    If .GetUserParam(Me.CurrentPrefix + "_framework-sort") <> "" Then
                        grid1.radGridOrig.MasterTableView.SortExpressions.AddSortExpression(.GetUserParam(Me.CurrentPrefix + "_framework-sort"))
                    End If
                    strDefWidth = ""
                    If Me.CurrentPrefix = "p91" Then strDefWidth = "p91DateSupply"
                    basUI.SelectDropdownlistValue(cbxPeriodType, .GetUserParam(Me.CurrentPrefix + "_framework-periodtype", strDefWidth))
                    ''If Me.cbxQueryFlag.Visible Then basUI.SelectDropdownlistValue(Me.cbxQueryFlag, .GetUserParam(Me.CurrentPrefix + "_framework-queryflag"))

                    If cbxPeriodType.SelectedIndex > 0 Then
                        period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                        period1.SelectedValue = .GetUserParam(Me.CurrentPrefix + "_framework-period")
                    End If
                    hidX18_value.Value = .GetUserParam("x18_querybuilder-value-" & Me.CurrentPrefix & "-grid")
                    x18_querybuilder_info.Text = .GetUserParam("x18_querybuilder-text-" & Me.CurrentPrefix & "-grid")

                    Dim cPT As BO.QueryByTags = Master.Factory.o51TagBL.ParseQueryByTags(Me.CurrentPrefix, .GetUserParam("o51_querybuilder-" & Me.CurrentPrefix))
                    hidO51IDs.Value = cPT.o51IDsInline
                    o51_querybuilder_info.Text = cPT.HtmlInline
                End With
            End With

            Me.CurrentJ62ID = BO.BAS.IsNullInt(Request.Item("j62id"))
            designer1.AllowSettingButton = Master.Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
            designer1.Prefix = Me.CurrentPrefix
            designer1.CurrentJ62ID = Me.CurrentJ62ID
            designer1.x36Key = Me.CurrentPrefix + "-j70id"
            'If Me.CurrentJ62ID <> 0 Then
            '    _curJ62 = Master.Factory.j62MenuHomeBL.Load(Me.CurrentJ62ID)
            '    If _curJ62 Is Nothing Then Master.StopPage("j62 record not found")
            'Else
            '    Master.SiteMenuValue = Me.CurrentPrefix
            'End If
            If Me.CurrentJ62ID <> 0 Then
            Else
                Master.SiteMenuValue = Me.CurrentPrefix
            End If


            With Master.Factory.j03UserBL
                Dim strJ70ID As String = Request.Item("j70id")
                If strJ70ID = "" Then strJ70ID = .GetUserParam(Me.CurrentPrefix + "-j70id")
                designer1.RefreshData(BO.BAS.IsNullInt(strJ70ID))

               
                SetupGrid(.GetUserParam(Me.CurrentPrefix + "_framework-filter_setting"), .GetUserParam(Me.CurrentPrefix + "_framework-filter_sql"))
            End With
            If Me.CurrentMasterPID > 0 Then
                Me.designer1.Visible = False
                With Me.MasterEntity
                    .Visible = True
                    .Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix), Me.CurrentMasterPID)
                    .Text = "<a href='" & Me.CurrentMasterPrefix & "_framework.aspx?pid=" & Me.CurrentMasterPID.ToString & "'>" & .Text & "</a>"
                End With
            End If
            RecalcVirtualRowCount()

            If Me.CurrentMasterPID = 0 Then
                Handle_DefaultSelectedRecord()
            Else
                Me.hidContentPaneDefUrl.Value = "entity_framework_detail_missing.aspx?prefix=" & Me.CurrentPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString & "&masterprefix=" & Me.CurrentMasterPrefix & "&source=" & opgLayout.Value
            End If

            AdaptSplitterLayout()
        End If
    
    End Sub
    
    Private Sub AdaptSplitterLayout()
        Select Case Me.opgLayout.Value
            Case "1"    'levý+pravý panel
                RadSplitter1.Orientation = Orientation.Vertical

            Case "2"    'horní a dolní panel
                RadSplitter1.Orientation = Orientation.Horizontal
                With navigationPane
                    .OnClientResized = ""
                    .OnClientCollapsed = ""
                    .OnClientExpanded = ""
                End With
                With contentPane
                    .OnClientResized = "AfterPaneResized"
                    .OnClientCollapsed = "AfterPaneCollapsed"
                    .OnClientExpanded = "AfterPaneExpanded"
                End With

            Case "3"    'pouze jeden panel
                RadSplitter1.Orientation = Orientation.Horizontal
                With navigationPane
                    .Collapsed = False
                    .MaxWidth = 0
                    .Width = Nothing
                    .OnClientResized = ""
                    .OnClientCollapsed = ""
                    .OnClientExpanded = ""
                End With
                grid1.OnRowDblClick = "RowDoubleClick"
                Me.contentPane.Collapsed = True
                Me.contentPane.Visible = False
                Me.RadSplitbar1.Visible = False
                

        End Select
        

        

        
    End Sub
  

    Private Sub SetupPeriodQuery()
        ''Me.cbxQueryFlag.Visible = False
        With cbxPeriodType.Items
            If .Count > 0 Then .Clear()
            .Add(New ListItem("--Nefiltrovat--", ""))
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p41Project
                    .Add(New ListItem("Založení projektu", "DateInsert"))
                    .Add(New ListItem("Plánované zahájení", "p41PlanFrom"))
                    .Add(New ListItem("Plánované dokončení", "p41PlanUntil"))
                Case BO.x29IdEnum.p28Contact
                    .Add(New ListItem("Založení klienta", "DateInsert"))
                Case BO.x29IdEnum.p56Task
                    .Add(New ListItem("Založení úkolu", "DateInsert"))
                    .Add(New ListItem("Plánované zahájení", "p56PlanFrom"))
                    .Add(New ListItem("Termín dokončení", "p56PlanUntil"))
                Case BO.x29IdEnum.o23Doc
                    .Add(New ListItem("Založení dokumentu", "DateInsert"))
                    .Add(New ListItem("Datum dokumentu", "o23FreeDate01"))
                Case BO.x29IdEnum.p91Invoice
                    .Add(New ListItem("Založení faktury", "DateInsert"))
                    .Add(New ListItem("Datum plnění", "p91DateSupply"))
                    .Add(New ListItem("Datum splatnosti", "p91DateMaturity"))
                    .Add(New ListItem("Datum vystavení", "p91Date"))

                Case BO.x29IdEnum.j02Person
                    .Add(New ListItem("Založení záznamu", "DateInsert"))
                    ''cbxQueryFlag.Items.Add(New ListItem("Pouze interní osoby", "1"))
                    ''cbxQueryFlag.Items.Add(New ListItem("Pouze kontaktní osoby", "2"))
                    ''cbxQueryFlag.Items.Add(New ListItem("Všechny osobní profily", "3"))
            End Select
            .Add(New ListItem("Datum worksheet úkonu", "p31Date"))
        End With
        
        ''If Me.cbxQueryFlag.Items.Count > 1 Then cbxQueryFlag.Visible = True
    End Sub

    Private Sub Handle_Permissions_And_More()
        Dim bolCanApprove As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver)
        Dim bolCanInvoice As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Draft_Creator)
        Dim bolSummary As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot)

        With Master
            .PageTitle = BO.BAS.GetX29EntityAlias(Me.CurrentX29ID, True)
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p41Project
                    ''img1.ImageUrl = "Images/project_32.png"
                    If Not .Factory.SysUser.j04IsMenu_Project Then .StopPage("Nemáte přístup do modulu [PROJEKTY].")
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [PROJEKTY]"
                    ''menu1.FindItemByValue("more").Text = "Akce nad projekty"
                Case BO.x29IdEnum.p28Contact
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [KLIENTI]"
                    ''img1.ImageUrl = "Images/contact_32.png"
                    If Not .Factory.SysUser.j04IsMenu_Contact Then .StopPage("Nemáte přístup do modulu [KLIENTI].")
                    ''menu1.FindItemByValue("more").Text = "Akce nad klienty"
                Case BO.x29IdEnum.o23Doc
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [DOKUMENTU]"
                    If Not .Factory.SysUser.j04IsMenu_Notepad Then .StopPage("Nemáte přístup do modulu [DOKUMENTY].")
                    ''img1.ImageUrl = "Images/notepad_32.png"
                    ''menu1.FindItemByValue("more").Text = "Akce nad dokumenty"
                    bolSummary = False
                    bolCanInvoice = False
                Case BO.x29IdEnum.p56Task
                    ''img1.ImageUrl = "Images/task_32.png"
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [ÚKOLY]"
                    ''menu1.FindItemByValue("more").Text = "Akce nad úkoly"
                    If Not .Factory.SysUser.j04IsMenu_Task Then .StopPage("Nemáte přístup do modulu [ÚKOLY].")
                Case BO.x29IdEnum.j02Person
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [LIDÉ]"
                    ''menu1.FindItemByValue("more").Text = "Akce nad přehledem"
                    ''img1.ImageUrl = "Images/person_32.png"
                    If Not .Factory.SysUser.j04IsMenu_People Then .StopPage("Nemáte přístup do modulu [LIDÉ].")
                Case BO.x29IdEnum.p91Invoice
                    FindNode("groupLayout").Text = "Rozvržení panelů v modulu [FAKTURY]"
                    ''menu1.FindItemByValue("more").Text = "Akce nad fakturami"
                    ''img1.ImageUrl = "Images/invoice_32.png"
                    If Not .Factory.SysUser.j04IsMenu_Invoice Then .StopPage("Nemáte přístup do modulu [FAKTURY].")
            End Select
            If Not .Factory.TestPermission(BO.x53PermValEnum.GR_GridTools) Then
                FindNode("groupExport").Items.Clear() : FindNode("groupExport").Visible = False
            End If
            If opgLayout.Value = "3" Then
                bolCanApprove = False
            End If

            If bolSummary Then
                AppendNode("Statistiky", "javascript:drilldown()", "Images/pivot.png", "")
            End If
            If bolCanApprove Then
                AppendNode("Schválit/připravit k fakturaci", "javascript:approve()", "Images/approve.png", "")
            End If
            If bolCanInvoice Then
                AppendNode("Fakturovat zrychleně s přeskočením schvalování", "javascript:invoice()", "Images/invoice.png", "")
            End If
            If Me.CurrentPrefix = "p91" Then
                AppendNode("Hromadně odeslat faktury (e-mail)", "javascript:sendmail_batch()", "Images/email.png", "")
            End If
            AppendNode("Tisková sestava", "javascript:report()", "Images/report.png", "")
            AppendNode("Oštítkovat", "javascript:tags()", "Images/tag.png", "")
            Select Case Me.CurrentPrefix
                Case "p28", "p41", "j02", "p56"
                    AppendNode("Hromadné úpravy záznamů", "javascript:batch()", "Images/batch.png", "")
            End Select
           
        End With
        

    End Sub
    Private Function FindNode(strValue As String) As RadMenuItem
        Return menu1.FindItemByValue(strValue)

    End Function
    Private Sub AppendNode(strText As String, strNavigateUrl As String, strImageUrl As String, strValue As String)
        Dim n As New RadMenuItem(strText, strNavigateUrl)
        n.ImageUrl = strImageUrl
        n.Value = strValue
        With FindNode("groupBatch")
            .Items.Add(n)
        End With
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)

        Me.hidDefaultSorting.Value = cJ70.j70OrderBy

        Dim cS As New UI.SetupDataGrid(Master.Factory, Me.grid1, cJ70)
        cS.PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
        cS.AllowCustomPaging = True
        cS.AllowMultiSelect = True
        cS.AllowMultiSelectCheckboxSelector = BO.BAS.BG(chkCheckboxSelector.SelectedValue)
        cS.FilterSetting = strFilterSetting
        cS.FilterExpression = strFilterExpression

        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        Me.hidCols.Value = cG.Cols
        Me.hidAdditionalFrom.Value = cG.AdditionalFROM
        Me.hidSumCols.Value = cG.SumCols

        ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, BO.BAS.IsNullInt(CType(FindNode("groupOther").FindControl("cbxPaging"), DropDownList).SelectedValue), True, True, BO.BAS.BG(CType(FindNode("groupOther").FindControl("chkCheckboxSelector"), DropDownList).SelectedValue), strFilterSetting, strFilterExpression, , strAddtionalSqlFrom, , strSumCols)
        ''Me.hidAdditionalFrom.Value = strAddtionalSqlFrom
        ''Me.hidSumCols.Value = strSumCols
        If cJ70.j70ScrollingFlag > BO.j70ScrollingFlagENUM.NoScrolling Then
            navigationPane.Scrolling = SplitterPaneScrolling.None
        End If
        If cJ70.j70PageLayoutFlag > BO.j70PageLayoutFlagENUM._None Then
            Me.opgLayout.Value = CInt(cJ70.j70PageLayoutFlag).ToString
            FindNode("groupLayout").Enabled = False
        Else
            FindNode("groupLayout").Enabled = True
        End If

        With grid1
            If Me.hidSumCols.Value = "" Then
                .radGridOrig.ShowFooter = False
            Else
                .radGridOrig.ShowFooter = True
            End If
        End With
        With cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
        _CurFilterDbField = strFilterColumn
    End Sub



    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                basUIMT.p41_grid_Handle_ItemDataBound(sender, e, True, "", "")
            Case BO.x29IdEnum.p28Contact
                basUIMT.p28_grid_Handle_ItemDataBound(sender, e, True)
            Case BO.x29IdEnum.o23Doc
                basUIMT.o23_grid_Handle_ItemDataBound(sender, e, False, "", "")
            Case BO.x29IdEnum.p56Task
                basUIMT.p56_grid_Handle_ItemDataBound(sender, e, False, True, "", "")
            Case BO.x29IdEnum.j02Person
                basUIMT.j02_grid_Handle_ItemDataBound(sender, e)
            Case BO.x29IdEnum.p91Invoice
                basUIMT.p91_grid_Handle_ItemDataBound(sender, e, True)
        End Select
        If _curIsExport Then
            If TypeOf e.Item Is GridHeaderItem Then
                e.Item.BackColor = Drawing.Color.Silver
            End If
            If TypeOf e.Item Is GridGroupHeaderItem Then
                e.Item.BackColor = Drawing.Color.WhiteSmoke
            End If
            'If TypeOf e.Item Is GridDataItem Or TypeOf e.Item Is GridHeaderItem Then
            '    e.Item.Cells(0).Visible = False
            'End If
        End If
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam(Me.CurrentPrefix + "_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam(Me.CurrentPrefix + "_framework-filter_sql", grid1.GetFilterExpression())
            End With
            RecalcVirtualRowCount()
        End If
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                Dim mq As New BO.myQueryP41
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p41(mq)

                If _curIsExport Then mq.MG_PageSize = 2000
                Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.p28Contact
                Dim mq As New BO.myQueryP28
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex

                End With
                InhaleMyQuery_p28(mq)
                If _curIsExport Then mq.MG_PageSize = 2000

                Dim dt As DataTable = Master.Factory.p28ContactBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If

            Case BO.x29IdEnum.p56Task
                Dim mq As New BO.myQueryP56
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex

                End With
                InhaleMyQuery_p56(mq)

                If _curIsExport Then mq.MG_PageSize = 2000

                Dim dt As DataTable = Master.Factory.p56TaskBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p56TaskBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.o23Doc
                Dim mq As New BO.myQueryO23(0)
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_o23(mq)

                If _curIsExport Then mq.MG_PageSize = 2000

                Dim dt As DataTable = Master.Factory.o23DocBL.GetDataTable4Grid(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.o23DocBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.j02Person
                Dim mq As New BO.myQueryJ02
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_j02(mq)

                If _curIsExport Then mq.MG_PageSize = 2000

                Dim dt As DataTable = Master.Factory.j02PersonBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.j02PersonBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.p91Invoice
                Dim mq As New BO.myQueryP91
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p91(mq)

                If _curIsExport Then mq.MG_PageSize = 2000

                Dim dt As DataTable = Master.Factory.p91InvoiceBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p91InvoiceBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If

            Case Else

        End Select





    End Sub

    Private Sub InhaleMyQuery_p91(ByRef mq As BO.myQueryP91)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
            .Closed = BO.BooleanQueryMode.NoQuery
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41ID = Me.CurrentMasterPID
                Case "j02" : .j02ID = Me.CurrentMasterPID
                Case "p28" : .p28ID = Me.CurrentMasterPID
            End Select        
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case cbxPeriodType.SelectedValue
                Case "p91DateSupply" : .PeriodType = BO.myQueryP91_PeriodType.p91DateSupply
                Case "p91DateMaturity" : .PeriodType = BO.myQueryP91_PeriodType.p91DateMaturity
                Case "p91Date" : .PeriodType = BO.myQueryP91_PeriodType.p91Date
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "p31Date"
                    .p31Date_D1 = period1.DateFrom : .p31Date_D2 = period1.DateUntil
            End Select
            If cbxPeriodType.SelectedValue <> "DateInsert" Then
                .DateFrom = period1.DateFrom
                .DateUntil = period1.DateUntil
            End If

            .j70ID = designer1.CurrentJ70ID

            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            If cbxGroupBy.SelectedValue <> "" Then
                Dim strPrimarySortField As String = cbxGroupBy.SelectedValue
                If .MG_SortString = "" Then
                    .MG_SortString = strPrimarySortField
                Else
                    .MG_SortString = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With

    End Sub

    Private Sub InhaleMyQuery_o23(ByRef mq As BO.myQueryO23)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
                Case "j02" : .j02IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
                Case "p28" : .p28IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
                Case "p56" : .p56IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
            End Select
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            If cbxGroupBy.SelectedValue <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = cbxGroupBy.SelectedValue
                Else
                    .MG_SortString = cbxGroupBy.SelectedValue & "," & .MG_SortString
                End If
            End If
            Select Case cbxPeriodType.SelectedValue
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "o23Date"
                    .DateFrom = period1.DateFrom : .DateUntil = period1.DateUntil
            End Select
            .Closed = BO.BooleanQueryMode.NoQuery
            .j70ID = designer1.CurrentJ70ID
            .SpecificQuery = BO.myQueryO23_SpecificQuery.AllowedForRead

            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With

    End Sub

    Private Sub InhaleMyQuery_p56(ByRef mq As BO.myQueryP56)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41ID = Me.CurrentMasterPID
                Case "j02" : .j02ID = Me.CurrentMasterPID
                Case "p28" : .p28ID = Me.CurrentMasterPID
            End Select
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If

            Select Case cbxPeriodType.SelectedValue
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "p56PlanFrom"
                    .p56PlanFrom_D1 = period1.DateFrom : .p56PlanFrom_D2 = period1.DateUntil
                Case "p56PlanUntil"
                    .p56PlanUntil_D1 = period1.DateFrom : .p56PlanUntil_D2 = period1.DateUntil
                Case "p31Date"
                    .p31Date_D1 = period1.DateFrom : .p31Date_D2 = period1.DateUntil
            End Select

            .Closed = BO.BooleanQueryMode.NoQuery
            .j70ID = designer1.CurrentJ70ID
            .SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With
    End Sub
    Private Sub InhaleMyQuery_p41(ByRef mq As BO.myQueryP41)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case Me.CurrentMasterPrefix
                Case "p28" : .p28ID = Me.CurrentMasterPID
                Case "p41"
                    .p41ParentID = Me.CurrentMasterPID    'podřízené projekty
            End Select
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If

            Select Case cbxPeriodType.SelectedValue
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "p41PlanFrom"
                    .p41PlanFrom_D1 = period1.DateFrom : .p41PlanFrom_D2 = period1.DateUntil
                Case "p41PlanUntil"
                    .p41PlanUntil_D1 = period1.DateFrom : .p41PlanUntil_D2 = period1.DateUntil
                Case "p31Date"
                    .p31Date_D1 = period1.DateFrom : .p31Date_D2 = period1.DateUntil
            End Select

            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
            .j70ID = designer1.CurrentJ70ID
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With
    End Sub
    Private Sub InhaleMyQuery_p28(ByRef mq As BO.myQueryP28)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            If Me.CurrentMasterPrefix = "p28" Then
                'podřízení klienti
                .p28ParentID = Me.CurrentMasterPID
            End If

            Select Case cbxPeriodType.SelectedValue
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "p31Date"
                    .p31Date_D1 = period1.DateFrom : .p31Date_D2 = period1.DateUntil
            End Select
            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead
            .j70ID = designer1.CurrentJ70ID
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With

    End Sub
    Private Sub InhaleMyQuery_j02(ByRef mq As BO.myQueryJ02)
        With mq
            .IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41ID = Me.CurrentMasterPID
                Case "p28" : .p28ID = Me.CurrentMasterPID
            End Select
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            If cbxGroupBy.SelectedValue <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = cbxGroupBy.SelectedValue
                Else
                    .MG_SortString = cbxGroupBy.SelectedValue & "," & .MG_SortString
                End If
            End If
            Select Case cbxPeriodType.SelectedValue
                Case "DateInsert"
                    .DateInsertFrom = period1.DateFrom : .DateInsertUntil = period1.DateUntil
                Case "p31Date"
                    .p31Date_D1 = period1.DateFrom : .p31Date_D2 = period1.DateUntil
            End Select
            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead

            ''Select Case Me.cbxQueryFlag.SelectedValue
            ''    Case "1" : .IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
            ''    Case "2" : .IntraPersons = BO.myQueryJ02_IntraPersons.NonIntraOnly
            ''    Case Else : .IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
            ''End Select
            .j70ID = designer1.CurrentJ70ID
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
        End With
    End Sub
 





    Private Sub Handle_DefaultSelectedRecord()
        Me.hidContentPaneDefUrl.Value = Me.CurrentPrefix + "_framework_detail.aspx?source=" & opgLayout.Value

        Dim intSelPID As Integer = 0
        If Not Page.IsPostBack Then
            Dim strDefPID As String = Request.Item("pid")
            If strDefPID = "" Then
                If Request.Item("eid") <> "" Then
                    Select Case Me.CurrentPrefix
                        Case "p56"
                            Dim c As BO.p56Task = Master.Factory.p56TaskBL.LoadByExternalPID(Request.Item("eid"))
                            If c Is Nothing Then
                                Master.StopPage("External ID not found.")
                            Else
                                strDefPID = c.PID.ToString
                            End If
                        Case "p41"
                            Dim c As BO.p41Project = Master.Factory.p41ProjectBL.LoadByExternalPID(Request.Item("eid"))
                            If c Is Nothing Then
                                Master.StopPage("External ID not found.")
                            Else
                                strDefPID = c.PID.ToString
                            End If
                    End Select
                Else
                    strDefPID = Master.Factory.j03UserBL.GetUserParam(Me.CurrentPrefix + "_framework_detail-pid")
                End If
            End If
            If strDefPID > "" Then intSelPID = BO.BAS.IsNullInt(strDefPID)
        End If

        If intSelPID > 0 Then
            'označit výchozí záznam
            grid1.SelectRecords(intSelPID)
            If grid1.GetSelectedPIDs.Count = 0 Then
                Dim dt As DataTable = Nothing
                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.p41Project
                        Dim mq As New BO.myQueryP41
                        InhaleMyQuery_p41(mq)
                        mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0
                        dt = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                    Case BO.x29IdEnum.p28Contact
                        Dim mq As New BO.myQueryP28
                        InhaleMyQuery_p28(mq)
                        mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0
                        dt = Master.Factory.p28ContactBL.GetGridDataSource(mq)
                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.p28ContactBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                    Case BO.x29IdEnum.p56Task
                        Dim mq As New BO.myQueryP56
                        InhaleMyQuery_p56(mq)
                        mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0
                        dt = Master.Factory.p56TaskBL.GetGridDataSource(mq)

                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.p56TaskBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                    Case BO.x29IdEnum.o23Doc
                        Dim mq As New BO.myQueryO23(0)
                        InhaleMyQuery_o23(mq)
                        mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0

                        dt = Master.Factory.o23DocBL.GetDataTable4Grid(mq)
                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.o23DocBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                    Case BO.x29IdEnum.j02Person
                        Dim mq As New BO.myQueryJ02
                        InhaleMyQuery_j02(mq)
                        mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0
                        dt = Master.Factory.j02PersonBL.GetGridDataSource(mq)
                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.j02PersonBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                    Case BO.x29IdEnum.p91Invoice
                        Dim mq As New BO.myQueryP91
                        InhaleMyQuery_p91(mq)
                        ''mq.MG_SelectPidFieldOnly = True
                        mq.TopRecordsOnly = 0
                        mq.MG_SelectPidFieldOnly = True
                        dt = Master.Factory.p91InvoiceBL.GetGridDataSource(mq)
                        If dt Is Nothing Then
                            Master.Notify(Master.Factory.p91InvoiceBL.ErrorMessage, NotifyLevel.ErrorMessage)
                            Return
                        End If
                End Select

                Dim x As Integer, intNewPageIndex As Integer = 0
                For Each dbRow As DataRow In dt.Rows
                    x += 1
                    If x > grid1.PageSize Then
                        intNewPageIndex += 1 : x = 1
                    End If
                    If dbRow.Item("pid") = intSelPID Then
                        ''Master.Notify(intSelPID & ", page-index: " & intNewPageIndex & ", x: " & x)
                        grid1.radGridOrig.CurrentPageIndex = intNewPageIndex
                        grid1.Rebind(False)
                        grid1.SelectRecords(intSelPID)
                        Exit For
                    End If
                Next
            End If

            Me.hidContentPaneDefUrl.Value = Me.CurrentPrefix + "_framework_detail.aspx?pid=" & intSelPID.ToString & "&source=" & opgLayout.Value  'v detailu ho vybereme nezávisle na tom, zda byl nalezen v gridu
        End If
        If Request.Item("force") <> "" Then
            Me.hidContentPaneDefUrl.Value += "&force=" & Request.Item("force")
        End If
    End Sub




    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With grid1.radGridOrig.MasterTableView
            .GroupByExpressions.Clear()
            If strGroupField = "" Then Return
            .ShowGroupFooter = True
            .GroupsDefaultExpanded = True
            Dim GGE As New GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strFieldHeader

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)
        End With
    End Sub
    

    Private Sub ReloadPage()

        Response.Redirect(GetReloadUrl(), True)
    End Sub
    Private Function GetReloadUrl() As String
        Dim s As String = "entity_framework.aspx?prefix=" & Me.CurrentPrefix
        If Me.CurrentJ62ID > 0 Then s += "&j62id=" & Me.CurrentJ62ID.ToString
        If Me.CurrentMasterPID > 0 Then s += "&masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString
        Return s
    End Function

    Private Sub RecalcVirtualRowCount()
        Dim dtSum As DataTable = Nothing

        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                Dim mq As New BO.myQueryP41
                InhaleMyQuery_p41(mq)
                dtSum = Master.Factory.p41ProjectBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
            Case BO.x29IdEnum.p28Contact
                Dim mq As New BO.myQueryP28
                InhaleMyQuery_p28(mq)
                dtSum = Master.Factory.p28ContactBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
            Case BO.x29IdEnum.p56Task
                Dim mq As New BO.myQueryP56
                InhaleMyQuery_p56(mq)
                dtSum = Master.Factory.p56TaskBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
            Case BO.x29IdEnum.o23Doc
                Dim mq As New BO.myQueryO23(0)
                InhaleMyQuery_o23(mq)
                grid1.VirtualRowCount = Master.Factory.o23DocBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.p91Invoice
                Dim mq As New BO.myQueryP91
                InhaleMyQuery_p91(mq)
                dtSum = Master.Factory.p91InvoiceBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
            Case BO.x29IdEnum.j02Person
                Dim mq As New BO.myQueryJ02
                InhaleMyQuery_j02(mq)
                grid1.VirtualRowCount = Master.Factory.j02PersonBL.GetList(mq).Count
        End Select

        If Not dtSum Is Nothing Then
            grid1.VirtualRowCount = dtSum.Rows(0).Item(0)
            Me.hidFooterSum.Value = grid1.CompleteFooterString(dtSum, Me.hidSumCols.Value)
        End If

        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub grid1_NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object) Handles grid1.NeedFooterSource
        If hidFooterSum.Value = "" Then Return
        ''footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"

        grid1.ParseFooterItemString(footerItem, hidFooterSum.Value)
    End Sub






    Private Sub Handle_Export()
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cXLS As New clsExportToXls(Master.Factory)
        ''Dim lis As IEnumerable(Of Object) = Nothing
        Dim dt As DataTable = Nothing

        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                Dim mq As New BO.myQueryP41
                InhaleMyQuery_p41(mq)
                mq.MG_GridGroupByField = ""
                ''lis = Master.Factory.p41ProjectBL.GetList(mq)
                dt = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
            Case BO.x29IdEnum.p28Contact
                Dim mq As New BO.myQueryP28
                InhaleMyQuery_p28(mq)
                mq.MG_GridGroupByField = ""
                ''lis = Master.Factory.p28ContactBL.GetList(mq)
                dt = Master.Factory.p28ContactBL.GetGridDataSource(mq)
            Case BO.x29IdEnum.p56Task
                Dim mq As New BO.myQueryP56
                InhaleMyQuery_p56(mq)
                mq.MG_GridGroupByField = ""
                ''lis = Master.Factory.p56TaskBL.GetList(mq)
                dt = Master.Factory.p56TaskBL.GetGridDataSource(mq)
            Case BO.x29IdEnum.o23Doc
                Dim mq As New BO.myQueryO23(0)
                InhaleMyQuery_o23(mq)
                mq.MG_GridGroupByField = ""

                dt = Master.Factory.o23DocBL.GetDataTable4Grid(mq)
            Case BO.x29IdEnum.j02Person
                Dim mq As New BO.myQueryJ02
                InhaleMyQuery_j02(mq)
                mq.MG_GridGroupByField = ""
                ''lis = Master.Factory.j02PersonBL.GetList(mq)
                ''dt = Master.Factory.j02PersonBL.GetGridDataSource(Me.hidCols.Value, mq, "")
            Case BO.x29IdEnum.p91Invoice
                Dim mq As New BO.myQueryP91
                InhaleMyQuery_p91(mq)
                mq.MG_GridGroupByField = ""
                ''lis = Master.Factory.p91InvoiceBL.GetList(mq)
                dt = Master.Factory.p91InvoiceBL.GetGridDataSource(mq)

        End Select

        Dim strFileName As String = cXLS.ExportDataGrid(dt.AsEnumerable, cJ70)
        If strFileName = "" Then
            Master.Notify(cXLS.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub


    ''Private Sub chkGroupsAutoExpanded_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupsAutoExpanded.CheckedChanged
    ''    Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-groups-autoexpanded", BO.BAS.GB(Me.chkGroupsAutoExpanded.Checked))
    ''    With Me.cbxGroupBy.SelectedItem
    ''        SetupGrouping(.Value, .Text)
    ''    End With
    ''    grid1.Rebind(True)
    ''End Sub

   

    

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-period", Me.period1.SelectedValue)
        RecalcVirtualRowCount()
        grid1.Rebind(False)
        ''hidUIFlag.Value = "period"
    End Sub

    ''Private Sub cbxQueryFlag_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryFlag.SelectedIndexChanged
    ''    Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-queryflag", Me.cbxQueryFlag.SelectedValue)
    ''    RecalcVirtualRowCount()
    ''    grid1.Rebind(False)
    ''End Sub

    Private Sub entity_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        designer1.ReloadUrl = GetReloadUrl()

        If cbxPeriodType.SelectedIndex > 0 Then

            With Me.period1
                .Visible = True
                If .SelectedValue <> "" Then
                    .BackColor = basUI.ColorQueryRGB


                Else
                    .BackColor = Nothing
                    ''    Me.CurrentPeriodQuery.Text = ""

                End If
            End With
        Else
            period1.Visible = False
        End If
        If opgLayout.Value = "2" Or opgLayout.Value = "3" Then
            Me.cbx1.Width = Unit.Parse("200px")
            designer1.Width = "220px"            
        End If


        If grid1.GetFilterExpression <> "" Then
            cmdCĺearFilter.Visible = True
        Else
            cmdCĺearFilter.Visible = False
        End If
        Me.CurrentQuery.Text = ""
        ''If designer1.CurrentJ70ID > 0 Then
        ''    If opgLayout.SelectedValue = "1" Then Me.CurrentQuery.Text = "<img src='Images/query.png'/>" & designer1.CurrentName
        ''End If
        If hidX18_value.Value <> "" Then
            Me.CurrentQuery.Text += "<img src='Images/query.png' style='margin-left:20px;'/><img src='Images/label.png'/>" & x18_querybuilder_info.Text & "<a href='javascript:clear_x18()' title='Zrušit filtr kategorií'><img src='Images/delete.png'></a>"
        End If
        If hidO51IDs.Value <> "" Then
            Me.CurrentQuery.Text += o51_querybuilder_info.Text & "<a href='javascript:clear_o51()' title='Zrušit filtr štítků'><img src='Images/delete.png'></a>"

        End If
        If panSearchbox.Visible Then
            Select Case Me.CurrentPrefix
                Case "p41"
                    cbx1.WebServiceSettings.Path = "~/Services/project_service.asmx"
                Case "p28"
                    cbx1.WebServiceSettings.Path = "~/Services/contact_service.asmx"
                Case "p91"
                    cbx1.WebServiceSettings.Path = "~/Services/invoice_service.asmx"
                Case "p56"
                    cbx1.WebServiceSettings.Path = "~/Services/task_service.asmx"
                Case "j02"
                    cbx1.WebServiceSettings.Path = "~/Services/person_service.asmx"
                Case "o23"
                    cbx1.WebServiceSettings.Path = "~/Services/doc_service.asmx"
            End Select
        End If
    End Sub

    Private Sub cmdCĺearFilter_Click(sender As Object, e As EventArgs) Handles cmdCĺearFilter.Click
        With Master.Factory.j03UserBL
            .SetUserParam(Me.CurrentPrefix + "_framework-filter_setting", "")
            .SetUserParam(Me.CurrentPrefix + "_framework-filter_sql", "")
        End With
        ReloadPage()
    End Sub

    Private Sub grid1_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid1.SortCommand
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-sort", SortExpression)
    End Sub




    Private Sub GridExport(strFormat As String)
        _curIsExport = True
        basUIMT.Handle_GridTelerikExport(Me.grid1, strFormat)


    End Sub









   
    






    

    Private Sub cmdContextMenuCallback_Click(sender As Object, e As EventArgs) Handles cmdContextMenuCallback.Click
        If hidContextMenuFlag.Value = "" Then Return
        Master.Notify(Me.hidContextMenuFlag.Value)
        Select Case hidContextMenuFlag.Value
            Case "pdf", "doc", "xls"
                GridExport(hidContextMenuFlag.Value)
            Case "export"
                Handle_Export()
            Case "chkCheckboxSelector"
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-checkbox_selector", chkCheckboxSelector.SelectedValue)
                ReloadPage()
            Case "cbxGroupBy"
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-groupby", cbxGroupBy.SelectedValue)
                ReloadPage()
            Case "cbxPaging"
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-pagesize", cbxPaging.SelectedValue)
                ReloadPage()
            Case "layout1", "layout2", "layout3"
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_framework-layout", Right(hidContextMenuFlag.Value, 1))
                ReloadPage()
            Case "cbxPeriodType"
                With Master.Factory.j03UserBL
                    ''If CType(FindNode("groupOther").FindControl("cbxPeriodType"), DropDownList).SelectedIndex > 0 And Not period1.Visible Then
                    ''    .InhaleUserParams("periodcombo-custom_query", Me.CurrentPrefix + "_framework-period")
                    ''    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    ''    period1.SelectedValue = .GetUserParam(Me.CurrentPrefix + "_framework-period")
                    ''End If

                    .SetUserParam(Me.CurrentPrefix + "_framework-periodtype", cbxPeriodType.SelectedValue)
                End With
                ReloadPage()
            Case "clear_o51"
                With Master.Factory.j03UserBL
                    .SetUserParam("o51_querybuilder-" & Me.CurrentPrefix, "")
                End With
                ReloadPage()
            Case "clear_x18"
                With Master.Factory.j03UserBL
                    .SetUserParam("x18_querybuilder-value-" & Me.CurrentPrefix & "-grid", "")
                    .SetUserParam("x18_querybuilder-text-" & Me.CurrentPrefix & "-grid", "")
                End With
                ReloadPage()
            Case Else


        End Select

        hidContextMenuFlag.Value = ""
    End Sub
End Class