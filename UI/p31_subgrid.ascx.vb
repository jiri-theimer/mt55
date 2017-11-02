Imports Telerik.Web.UI
Public Class p31_subgrid
    Inherits System.Web.UI.UserControl

    Public Property Factory As BL.Factory           'proměnná nedrží stav!
    Public Property DefaultSelectedPID As Integer = 0
    Public Property ExplicitMyQuery As BO.myQueryP31    'proměnná nedrží stav!

    Private Property _curIsExport As Boolean
    Private Property _needFilterIsChanged As Boolean = False

    Public Property MasterDataPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterDataPID.Value)
        End Get
        Set(value As Integer)
            Me.hidMasterDataPID.Value = value.ToString
        End Set
    End Property
  

   
    Public Property MasterTabAutoQueryFlag As String
        Get
            Return hidMasterTabAutoQueryFlag.Value
        End Get
        Set(value As String)
            hidMasterTabAutoQueryFlag.Value = value
        End Set
    End Property

    Public Property ExplicitDateFrom As Date
        Get
            Return BO.BAS.ConvertString2Date(Me.hidExplicitDateFrom.Value)
        End Get
        Set(value As Date)
            Me.hidExplicitDateFrom.Value = Format(value, "dd.MM.yyyy")

        End Set
    End Property
    Public Property ExplicitDateUntil As Date
        Get
            Return BO.BAS.ConvertString2Date(Me.hidExplicitDateUntil.Value)
        End Get
        Set(value As Date)
            Me.hidExplicitDateUntil.Value = Format(value, "dd.MM.yyyy")

        End Set
    End Property
    Public Property AllowFullScreen As Boolean
        Get
            Return BO.BAS.BG(Me.hidAllowFullScreen.Value)
        End Get
        Set(value As Boolean)
            Me.hidAllowFullScreen.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property HeaderText As String
        Get
            Return Me.lblHeaderP31.Text
        End Get
        Set(value As String)
            Me.lblHeaderP31.Text = value
        End Set
    End Property
    

    Public Property AllowMultiSelect As Boolean
        Get
            Return grid2.AllowMultiSelect
        End Get
        Set(value As Boolean)
            grid2.AllowMultiSelect = value
        End Set
    End Property
    Public Property AllowApproving As Boolean
        Get
            Return recmenu1.FindItemByValue("cmdApprove").Visible
        End Get
        Set(value As Boolean)
            recmenu1.FindItemByValue("cmdApprove").Visible = value
            
        End Set
    End Property
    Public Property EnableEntityChilds As Boolean
        Get
            Return Me.chkIncludeChilds.Visible
        End Get
        Set(value As Boolean)
            Me.chkIncludeChilds.Visible = value
        End Set
    End Property

    Public Property EntityX29ID As BO.x29IdEnum
        Get
            If Me.hidX29ID.Value <> "" Then
                Return CInt(Me.hidX29ID.Value)
            Else
                Return BO.x29IdEnum._NotSpecified
            End If
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
        End Set
    End Property
    Public ReadOnly Property MasterPrefixWithQueryFlag As String
        Get
            If Me.MasterTabAutoQueryFlag = "" Then
                Return BO.BAS.GetDataPrefix(Me.EntityX29ID)
            Else
                Return BO.BAS.GetDataPrefix(Me.EntityX29ID) & "-" & Me.MasterTabAutoQueryFlag
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.MasterDataPID = 0 Then Return
        designer1.Factory = Me.Factory

        If Not Page.IsPostBack Then
            ''ViewState("footersum") = ""
            With Factory.j03UserBL
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("periodcombo-custom_query")
                    .Add("p31_grid-period")
                    .Add("p31_subgrid-j70id-" & Me.MasterPrefixWithQueryFlag)
                    .Add("p31_subgrid-pagesize")
                    .Add("p31_subgrid-groupby-" & Me.MasterPrefixWithQueryFlag)
                    .Add("p31_subgrid-includechilds-" & Me.MasterPrefixWithQueryFlag)
                    ''.Add("p31_subgrid-search")
                    .Add("p31_subgrid-groups-autoexpanded")
                    .Add("p31_subgrid-sort")
                    .Add("x18_querybuilder-value-p31-p31grid")
                    .Add("x18_querybuilder-text-p31-p31grid")
                End With
                .InhaleUserParams(lisPars)

                
                designer1.MasterPrefix = Me.MasterPrefixWithQueryFlag
                designer1.x36Key = "p31_subgrid-j70id-" & Me.MasterPrefixWithQueryFlag
                designer1.RefreshData(BO.BAS.IsNullInt(.GetUserParam("p31_subgrid-j70id-" & Me.MasterPrefixWithQueryFlag)))


                hidX18_value.Value = .GetUserParam("x18_querybuilder-value-p31-p31grid")
                Me.x18_querybuilder_info.Text = .GetUserParam("x18_querybuilder-text-p31-p31grid")
                period1.SetupData(Me.Factory, .GetUserParam("periodcombo-custom_query"))

                period1.SelectedValue = .GetUserParam("p31_grid-period")
                If .GetUserParam("p31_subgrid-sort") <> "" Then
                    grid2.radGridOrig.MasterTableView.SortExpressions.AddSortExpression(.GetUserParam("p31_subgrid-sort"))
                End If
                basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("p31_subgrid-pagesize", "10"))
                basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("p31_subgrid-groupby-" & Me.MasterPrefixWithQueryFlag))
                ''Me.txtSearch.Text = .GetUserParam("p31_subgrid-search")
                Me.chkGroupsAutoExpanded.Checked = BO.BAS.BG(.GetUserParam("p31_subgrid-groups-autoexpanded", "1"))
                If Me.EntityX29ID = BO.x29IdEnum.p41Project Then
                    Me.chkIncludeChilds.Checked = BO.BAS.BG(.GetUserParam("p31_subgrid-includechilds-" & Me.MasterPrefixWithQueryFlag))
                End If

            End With
            panExport.Visible = Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
            recmenu1.FindItemByValue("cmdMove").Visible = Factory.TestPermission(BO.x53PermValEnum.GR_P31_Owner)
            cmdSummary.Visible = Factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot)
            If Not cmdSummary.Visible Then recmenu1.FindItemByValue("cmdSummary").Visible = False
            designer1.Visible = panExport.Visible
            recmenu1.FindItemByValue("cmdInvoice").Visible = Factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator)

            RecalcVirtualRowCount()
            SetupP31Grid()

        End If
       
        RefreshState()
    End Sub
    
   
    Private Sub SetupP31Grid()
        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)

        Me.hidDefaultSorting.Value = cJ70.j70OrderBy
        Dim cS As New SetupDataGrid(Me.Factory, grid2, cJ70)
        With cS
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .AllowCustomPaging = True
            .AllowMultiSelect = Me.AllowMultiSelect
            .AllowMultiSelectCheckboxSelector = Me.AllowMultiSelect
            .MasterPrefix = Me.MasterPrefixWithQueryFlag
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM
        Me.hidSumCols.Value = cG.SumCols

        ''Dim strAddSqlFrom As String = "", strSqlSumCols As String = ""
        ''Me.hidCols.Value = basUIMT.SetupDataGrid(Me.Factory, Me.grid2, cJ70, CInt(Me.cbxPaging.SelectedValue), True, Me.AllowMultiSelect, Me.AllowMultiSelect, , , , strAddSqlFrom, , strSqlSumCols, Me.MasterPrefixWithQueryFlag)
        ''Me.hidFrom.Value = strAddSqlFrom
        ''Me.hidSumCols.Value = strSqlSumCols
        
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With

    End Sub
    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        grid2.radGridOrig.GroupingSettings.RetainGroupFootersVisibility = True
        With grid2.radGridOrig.MasterTableView
            .GroupByExpressions.Clear()
            If strGroupField = "" Then Return
            .ShowGroupFooter = True
            .GroupsDefaultExpanded = chkGroupsAutoExpanded.Checked
            Dim GGE As New GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strFieldHeader

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)
        End With
    End Sub

    
    Private Function GetRowsCount(mq As BO.myQueryP31) As Integer
        Dim cSum As BO.p31WorksheetSum = Factory.p31WorksheetBL.LoadSumRow(mq, False, False)
        Return cSum.RowsCount
    End Function

    Private Sub grid2_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid2.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid2_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid2.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, True, False, Me.MasterPrefixWithQueryFlag)

        If _curIsExport Then
            If TypeOf e.Item Is GridHeaderItem Then
                e.Item.BackColor = Drawing.Color.Silver
            End If
            If TypeOf e.Item Is GridGroupHeaderItem Then
                e.Item.BackColor = Drawing.Color.WhiteSmoke
            End If
            If TypeOf e.Item Is GridDataItem Or TypeOf e.Item Is GridHeaderItem Then
                e.Item.Cells(0).Visible = False
            End If
        End If
    End Sub

    Private Sub grid2_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid2.NeedDataSource
        If Me.MasterDataPID = 0 Or Me.EntityX29ID = BO.x29IdEnum._NotSpecified Then Return
        If e.IsFromDetailTable Then
            Return
        End If
        Dim mq As New BO.myQueryP31
        p31_InhaleMyQuery(mq)
        With mq
            .MG_PageSize = CInt(Me.cbxPaging.SelectedValue)
            .MG_CurrentPageIndex = grid2.radGridOrig.MasterTableView.CurrentPageIndex
        End With
       
        If _curIsExport Then mq.MG_PageSize = 2000
        Dim dt As DataTable = Me.Factory.p31WorksheetBL.GetGridDataSource(mq)
        If dt Is Nothing Then
            grid2.VirtualRowCount = 0
            grid2.DataSource = Nothing
            Return
        End If

        If Me.DefaultSelectedPID <> 0 And Not _curIsExport Then
            If dt.AsEnumerable.Where(Function(p) p.Item("pid") = Me.DefaultSelectedPID).Count > 0 Then
                'záznam je na první stránce
            Else
                Dim mqAll As New BO.myQueryP31
                mqAll.TopRecordsOnly = 0
                mqAll.MG_SelectPidFieldOnly = True
                mqAll.MG_AdditionalSqlFROM = Me.hidFrom.Value
                p31_InhaleMyQuery(mqAll)
                Dim dtAll As DataTable = Me.Factory.p31WorksheetBL.GetGridDataSource(mqAll)
                Dim x As Integer, intNewPageIndex As Integer = 0
                For Each dbRow As DataRow In dtAll.Rows
                    x += 1
                    If x > grid2.PageSize Then
                        intNewPageIndex += 1 : x = 1
                    End If
                    If dbRow.Item("pid") = Me.DefaultSelectedPID Then
                        grid2.radGridOrig.CurrentPageIndex = intNewPageIndex
                        p31_InhaleMyQuery(mq)
                        mq.MG_CurrentPageIndex = intNewPageIndex
                        dt = Me.Factory.p31WorksheetBL.GetGridDataSource(mq) 'nový zdroj pro grid
                        Exit For
                    End If
                Next
            End If
        End If
        If _needFilterIsChanged Then
            RecalcVirtualRowCount()
        End If
        ''Dim lis As IEnumerable(Of BO.p31Worksheet) = Me.Factory.p31WorksheetBL.GetList(mq)
        ''If Me.DefaultSelectedPID <> 0 Then
        ''    If lis.Where(Function(p) p.PID = Me.DefaultSelectedPID).Count > 0 Then
        ''        'záznam je na první stránce
        ''    Else
        ''        Dim mqAll As New BO.myQueryP31
        ''        mqAll.TopRecordsOnly = 0
        ''        p31_InhaleMyQuery(mqAll)
        ''        Dim lisAll As IEnumerable(Of BO.p31Worksheet) = Me.Factory.p31WorksheetBL.GetList(mqAll)
        ''        Dim pids As IEnumerable(Of Integer) = lisAll.Select(Function(p) p.PID)
        ''        Dim x As Integer, intNewPageIndex As Integer = 0
        ''        For Each intPID As Integer In pids
        ''            x += 1
        ''            If x > grid2.PageSize Then
        ''                intNewPageIndex += 1 : x = 1
        ''            End If
        ''            If intPID = Me.DefaultSelectedPID Then
        ''                grid2.radGridOrig.CurrentPageIndex = intNewPageIndex
        ''                mq.MG_CurrentPageIndex = intNewPageIndex
        ''                lis = Me.Factory.p31WorksheetBL.GetList(mq) 'nový zdroj pro grid
        ''                Exit For
        ''            End If
        ''        Next
        ''    End If
        ''End If
        ''grid2.DataSource = lis

        grid2.DataSourceDataTable = dt
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Me.Factory.j03UserBL.SetUserParam("p31_grid-period", Me.period1.SelectedValue)
        RecalcVirtualRowCount()
        grid2.Rebind(False)
        ''If Me.hidDrillDownField.Value = "" Then
        ''    RecalcVirtualRowCount()
        ''    grid2.Rebind(False)
        ''Else
        ''    ReloadPage()
        ''End If
        
    End Sub

    Private Sub grid2_NeedFooterSource(footerItem As Telerik.Web.UI.GridFooterItem, footerDatasource As Object) Handles grid2.NeedFooterSource
        ''footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"
        If Me.hidFooterString.Value = "" And grid2.radGridOrig.PageCount > 1 Then
            RecalcVirtualRowCount()
        End If

        grid2.ParseFooterItemString(footerItem, Me.hidFooterString.Value)

        If Me.DefaultSelectedPID <> 0 Then
            grid2.SelectRecords(Me.DefaultSelectedPID)
        End If
    End Sub

    Public Sub Rebind(bolKeepSelectedItems As Boolean, Optional intExplicitSelectedPID As Integer = 0)
        If grid2.radGridOrig.Columns.Count = 0 Then
            Return
        End If
        grid2.Rebind(bolKeepSelectedItems, intExplicitSelectedPID)

    End Sub

    ''Public Sub SelectRecord(intSelPID As Integer)
    ''    grid2.SelectRecords(intSelPID)
    ''    If grid2.GetSelectedPIDs.Count > 0 Then Return 'záznam byl nalezen na první stránce
    ''    'je třeba najít záznam na dalších stránkách
    ''    Dim mq As New BO.myQueryP31
    ''    mq.TopRecordsOnly = 0
    ''    p31_InhaleMyQuery(mq)

    ''    Dim lis As IEnumerable(Of BO.p31Worksheet) = Me.Factory.p31WorksheetBL.GetList(mq)
    ''    Dim pids As IEnumerable(Of Integer) = lis.Select(Function(p) p.PID)
    ''    Dim x As Integer, intNewPageIndex As Integer = 0
    ''    For Each intPID As Integer In pids
    ''        x += 1
    ''        If x > grid2.PageSize Then
    ''            intNewPageIndex += 1 : x = 1
    ''        End If
    ''        If intPID = intSelPID Then
    ''            grid2.radGridOrig.CurrentPageIndex = intNewPageIndex
    ''            Rebind(False, intSelPID)
    ''            Exit For
    ''        End If

    ''    Next
    ''End Sub
    Public Sub RecalcVirtualRowCount()

        If Me.MasterDataPID = 0 Or Me.EntityX29ID = BO.x29IdEnum._NotSpecified Then Return
        Dim mq As New BO.myQueryP31
        p31_InhaleMyQuery(mq)

        Dim dt As DataTable = Me.Factory.p31WorksheetBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
        grid2.VirtualRowCount = dt.Rows(0).Item(0)
        Me.hidFooterString.Value = grid2.CompleteFooterString(dt, Me.hidSumCols.Value)

        grid2.radGridOrig.CurrentPageIndex = 0
        ''Me.lblHeaderP31.Text = BO.BAS.OM2(Me.lblHeaderP31.Text, BO.BAS.FNI(grid2.VirtualRowCount))
        Me.lblHeaderP31.Text = grid2.VirtualRowCount.ToString & "x"
    End Sub

    Private Sub p31_InhaleMyQuery(ByRef mq As BO.myQueryP31)
        RefreshState()
        If Not Me.ExplicitMyQuery Is Nothing Then
            mq = Me.ExplicitMyQuery
            Return
        End If
        With mq
            Select Case Me.EntityX29ID
                Case BO.x29IdEnum.p41Project
                    .p41ID = Me.MasterDataPID
                    If Me.chkIncludeChilds.Visible Then .IncludeChildProjects = Me.chkIncludeChilds.Checked
                Case BO.x29IdEnum.p28Contact
                    .p28ID_Client = Me.MasterDataPID
                Case BO.x29IdEnum.j02Person
                    .j02ID = Me.MasterDataPID
                Case BO.x29IdEnum.p56Task
                    .p56IDs = New List(Of Integer)
                    .p56IDs.Add(Me.MasterDataPID)
            End Select
            .j70ID = designer1.CurrentJ70ID
            .SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            If period1.Visible Then
                If period1.SelectedValue <> "" Then
                    .DateFrom = period1.DateFrom
                    .DateUntil = period1.DateUntil
                End If
            Else
                .DateFrom = Me.ExplicitDateFrom
                .DateUntil = Me.ExplicitDateUntil
            End If
            ''.SearchExpression = Trim(Me.txtSearch.Text)
            .TabAutoQuery = Me.MasterTabAutoQueryFlag

            .ColumnFilteringExpression = grid2.GetFilterExpressionCompleteSql()
            .MG_SortString = grid2.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .MG_GridSqlColumns = Me.hidCols.Value
            .x18Value = Me.hidX18_value.Value

            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            ''If Me.cbxGroupBy.SelectedValue <> "" Then
            ''    Dim strPrimarySortField As String = Me.cbxGroupBy.SelectedValue
            ''    If strPrimarySortField = "SupplierName" Then strPrimarySortField = "supplier.p28Name"
            ''    If strPrimarySortField = "ClientName" Then strPrimarySortField = "p28client.p28Name"
            ''    If strPrimarySortField = "Person" Then strPrimarySortField = "j02.j02LastName+char(32)+j02.j02Firstname"

            ''    If .MG_SortString = "" Then
            ''        .MG_SortString = strPrimarySortField
            ''    Else
            ''        .MG_SortString = strPrimarySortField & "," & .MG_SortString
            ''    End If
            ''End If
        End With
    End Sub



    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Me.Factory.j03UserBL.SetUserParam("p31_subgrid-pagesize", Me.cbxPaging.SelectedValue)
        SetupP31Grid()
        grid2.Rebind(True)
        ''If Me.hidDrillDownField.Value = "" Then
        ''    SetupP31Grid()
        ''    grid2.Rebind(True)
        ''Else
        ''    ReloadPage()
        ''End If
    End Sub


   

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ''designer1.ReloadUrl = Request.Url.AbsoluteUri.ToString()
        designer1.ReloadUrl = Request.Url.PathAndQuery
    End Sub
    Private Sub RefreshState()
        If Not Me.ExplicitMyQuery Is Nothing Then
            Me.panCommand.Visible = False
        Else
            Me.panCommand.Visible = True
        End If
        If Not BO.BAS.IsNullDBDate(Me.ExplicitDateFrom) Is Nothing Then
            Me.period1.Visible = False
            Me.ExplicitPeriod.Text = BO.BAS.FD(Me.ExplicitDateFrom)

            cmdClearExplicitPeriod.Visible = True
            If Me.ExplicitDateFrom < Me.ExplicitDateUntil Then
                Me.ExplicitPeriod.Text += " - " & BO.BAS.FD(Me.ExplicitDateUntil)
            End If

        Else
            Me.ExplicitPeriod.Text = ""
            Me.cmdClearExplicitPeriod.Visible = False
            With Me.period1
                .Visible = True
                If .SelectedValue <> "" Then
                    .BackColor = basUI.ColorQueryRGB
                Else
                    .BackColor = Nothing
                End If
            End With
        End If
        

        If hidX18_value.Value <> "" Then
            Me.CurrentQuery.Text += "<img src='Images/query.png' style='margin-left:20px;'/><img src='Images/label.png'/>" & Me.x18_querybuilder_info.Text
            cmdClearX18.Visible = True
        Else
            cmdClearX18.Visible = False
        End If

        ''If Trim(txtSearch.Text) = "" Then
        ''    txtSearch.Style.Item("background-color") = ""
        ''Else
        ''    txtSearch.Style.Item("background-color") = "red"
        ''End If
        If cbxGroupBy.SelectedValue <> "" Then chkGroupsAutoExpanded.Visible = True Else chkGroupsAutoExpanded.Visible = False
        cmdFullScreen.Visible = Me.AllowFullScreen

        If Me.AllowFullScreen Or Me.AllowApproving Then
            recmenu1.FindItemByValue("akce").Visible = True
        Else
            recmenu1.FindItemByValue("akce").Visible = False
        End If
    End Sub
    

    Private Sub cbxGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGroupBy.SelectedIndexChanged
        Factory.j03UserBL.SetUserParam("p31_subgrid-groupby-" & Me.MasterPrefixWithQueryFlag, Me.cbxGroupBy.SelectedValue)
        ReloadPage()
        'With Me.cbxGroupBy.SelectedItem
        '    SetupGrouping(.Value, .Text)
        'End With
        'grid2.Rebind(True)
    End Sub
    

    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cXLS As New clsExportToXls(Me.Factory)

        Dim mq As New BO.myQueryP31
        p31_InhaleMyQuery(mq)
        mq.MG_GridGroupByField = ""

        Dim dt As DataTable = Me.Factory.p31WorksheetBL.GetGridDataSource(mq)

        Dim strFileName As String = cXLS.ExportDataGrid(dt.AsEnumerable, cJ70)
        If strFileName = "" Then
            Response.Write(cXLS.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    ''Private Sub Handle_RunSearch()
    ''    Factory.j03UserBL.SetUserParam("p31_subgrid-search", Trim(txtSearch.Text))

    ''    If Me.hidDrillDownField.Value = "" Then
    ''        grid2.Rebind(False)
    ''    Else
    ''        ReloadPage()
    ''    End If

    ''    txtSearch.Focus()
    ''End Sub

    ''Private Sub cmdSearch_Click(sender As Object, e As ImageClickEventArgs) Handles cmdSearch.Click
    ''    Handle_RunSearch()
    ''End Sub

    Private Sub chkGroupsAutoExpanded_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupsAutoExpanded.CheckedChanged
        Factory.j03UserBL.SetUserParam("p31_subgrid-groups-autoexpanded", BO.BAS.GB(Me.chkGroupsAutoExpanded.Checked))
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
        grid2.Rebind(True)
    End Sub

    

   

    Private Sub ReloadPage()
        Response.Redirect(Request.Url.AbsoluteUri.ToString())
    End Sub

    Private Sub grid2_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid2.SortCommand
        Factory.j03UserBL.SetUserParam("p31_subgrid-sort", SortExpression)
    End Sub

    Private Sub GridExport(strFormat As String)
        _curIsExport = True
        Me.AllowMultiSelect = False
        SetupP31Grid()
        With grid2
            .Page.Response.ClearHeaders()
            .Page.Response.Cache.SetCacheability(HttpCacheability.[Private])
            .PageSize = 2000
            .Rebind(False)
            Select Case strFormat
                Case "xls"
                    .radGridOrig.ExportToExcel()
                Case "pdf"
                    With .radGridOrig.ExportSettings.Pdf
                        If grid2.radGridOrig.Columns.Count > 4 Then
                            .PageWidth = Unit.Parse("297mm")
                            .PageHeight = Unit.Parse("210mm")
                        Else
                            .PageHeight = Unit.Parse("297mm")
                            .PageWidth = Unit.Parse("210mm")
                        End If
                    End With
                    .radGridOrig.ExportToPdf()
                Case "doc"
                    .radGridOrig.ExportToWord()
            End Select

        End With
    End Sub

    Private Sub cmdPDF_Click(sender As Object, e As EventArgs) Handles cmdPDF.Click
        GridExport("pdf")
    End Sub

    Private Sub cmdXLS_Click(sender As Object, e As EventArgs) Handles cmdXLS.Click
        GridExport("xls")
    End Sub

    Private Sub cmdDOC_Click(sender As Object, e As EventArgs) Handles cmdDOC.Click
        GridExport("doc")
    End Sub

    Private Sub cmdClearExplicitPeriod_Click(sender As Object, e As ImageClickEventArgs) Handles cmdClearExplicitPeriod.Click
        Me.ExplicitDateFrom = DateSerial(1900, 1, 1)
        Me.ExplicitDateUntil = DateSerial(3000, 1, 1)
        RecalcVirtualRowCount()
        grid2.Rebind(False)
    End Sub

    Private Sub chkIncludeChilds_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeChilds.CheckedChanged
        Factory.j03UserBL.SetUserParam("p31_subgrid-includechilds-" & Me.MasterPrefixWithQueryFlag, BO.BAS.GB(Me.chkIncludeChilds.Checked))
        ReloadPage()
    End Sub

    Private Sub cmdClearX18_Click(sender As Object, e As ImageClickEventArgs) Handles cmdClearX18.Click
        With Me.Factory.j03UserBL
            .SetUserParam("x18_querybuilder-value-p31-p31grid", "")
            .SetUserParam("x18_querybuilder-text-p31-p31grid", "")
        End With
        ReloadPage()
    End Sub

   
End Class