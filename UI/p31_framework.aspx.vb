Imports Telerik.Web.UI
Public Class p31_framework
    Inherits System.Web.UI.Page

    Protected WithEvents _MasterPage As Site

    Private _lastP41ID As Integer = 0
    Private Property _needFilterIsChanged As Boolean = False

    
    Public ReadOnly Property CurrentJ02ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidCurrentJ02ID.Value)
        End Get
    End Property
    Public ReadOnly Property GridPrefix As String
        Get
            If tabs1.SelectedIndex = 2 Then Return "p56" Else Return "p41"
        End Get

    End Property
    Public Property IsUseReceiversInLine As Boolean
        Get
            If hidReceiversInLine.Value = "1" Then Return True Else Return False
        End Get
        Set(value As Boolean)
            hidReceiversInLine.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property IsUseTasksWorksheetColumns As Boolean
        Get
            If Me.hidTasksWorksheetColumns.Value = "1" Then Return True Else Return False
        End Get
        Set(value As Boolean)
            hidTasksWorksheetColumns.Value = BO.BAS.GB(value)
        End Set
    End Property
   
    Private Sub p31_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_framework"
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Zapisování úkonů"
                .SiteMenuValue = "p31_framework"
                If Request.Item("showtimer") <> "" Then
                    .Factory.j03UserBL.SetUserParam("p31_framework-timer", Request.Item("showtimer"))
                End If
                If Request.Item("showgrid") <> "" Then
                    .Factory.j03UserBL.SetUserParam("p31_framework-grid", Request.Item("showgrid"))
                End If
                If Request.Item("tab") = "" Then
                    .Factory.j03UserBL.InhaleUserParams("p31_framework-tabindex")
                    tabs1.SelectedIndex = CInt(.Factory.j03UserBL.GetUserParam("p31_framework-tabindex", "0"))
                Else
                    tabs1.SelectedIndex = CInt(Request.Item("tab"))
                End If
                InitialGroupByCombo(tabs1.SelectedIndex)
                designer1.Prefix = Me.GridPrefix
                designer1.x36Key = "p31_framework-j70id-" & Me.GridPrefix
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(designer1.x36Key)
                    .Add("p31_framework-pagesize-" & Me.GridPrefix)
                    .Add("p31_framework-navigationPane_width")
                    .Add("p31_framework_detail-j02id")  'výchozí osoba pro nové úkony
                    .Add("p31_framework-groupby-" & Me.GridPrefix)
                    .Add("p31_framework-sort-" & Me.GridPrefix)
                    .Add("p31_framework-groups-autoexpanded")
                    .Add("p31_framework-timer")
                    .Add("p31_framework-grid")
                    If tabs1.SelectedIndex <> 1 Then    'v top10 se nefiltruje
                        .Add("p31_framework-filter_setting_p41")
                        .Add("p31_framework-filter_sql_p41")
                        .Add("p31_framework-filter_setting_p56")
                        .Add("p31_framework-filter_sql_p56")
                    End If
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Dim strDefIsGrid As String = "1", strDefIsTimer As String = "1"
                    If basUI.GetCookieValue(Request, "MT50-SAW") = "1" Then
                        strDefIsGrid = "0" : strDefIsTimer = "0"
                    End If
                    hidCurrentJ02ID.Value = .GetUserParam("p31_framework_detail-j02id", Master.Factory.SysUser.j02ID.ToString)

                    basUI.SelectDropdownlistValue(cbxPaging, .GetUserParam("p31_framework-pagesize-" & Me.GridPrefix, "20"))
                    Dim strW As String = .GetUserParam("p31_framework-navigationPane_width", "350")
                    If strW = "-1" Then
                        Me.navigationPane.Collapsed = True
                    Else
                        Me.navigationPane.Width = Unit.Parse(.GetUserParam("p31_framework-navigationPane_width", "350") & "px")
                    End If
                    Dim strJ70ID As String = Request.Item("j70id")
                    If strJ70ID = "" Then strJ70ID = .GetUserParam(designer1.x36Key, "0")
                    designer1.AllowSettingButton = Master.Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
                    designer1.RefreshData(CInt(strJ70ID))

                    basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("p31_framework-groupby-" & Me.GridPrefix, IIf(Me.GridPrefix = "p56", "Client", "")))
                    If tabs1.SelectedIndex = 0 Then
                        Me.chkGroupsAutoExpanded.Checked = BO.BAS.BG(.GetUserParam("p31_framework-groups-autoexpanded", "0"))
                    Else
                        chkGroupsAutoExpanded.Checked = True
                        chkGroupsAutoExpanded.Visible = False
                    End If
                    If .GetUserParam("p31_framework-sort-" & Me.GridPrefix) <> "" Then
                        grid1.radGridOrig.MasterTableView.SortExpressions.AddSortExpression(.GetUserParam("p31_framework-sort-" & Me.GridPrefix))
                    End If
                    If .GetUserParam("p31_framework-timer", strDefIsTimer) Then
                        rightPane.ContentUrl = "p31_framework_timer.aspx"
                    Else
                        rightPane.ContentUrl = ""
                        rightPane.Visible = False
                        RadSplitter1.Items.Remove(rightPane)
                    End If
                    If .GetUserParam("p31_framework-grid", strDefIsGrid) <> "1" Then
                        navigationPane.Visible = False
                        RadSplitbar1.Visible = False
                        RadSplitter1.Items.Remove(navigationPane)
                    End If
                End With
            End With


            If navigationPane.Visible Then

                grid1.radGridOrig.MasterTableView.FilterExpression = Master.Factory.j03UserBL.GetUserParam("p31_framework-filter_sql_p41")
                RecalcVirtualRowCount()

                grid1.radGridOrig.MasterTableView.FilterExpression = Master.Factory.j03UserBL.GetUserParam("p31_framework-filter_sql_p56")
                RecalcTasksCount()


                With Master.Factory.j03UserBL
                    SetupGrid(.GetUserParam("p31_framework-filter_setting_" & Me.GridPrefix), .GetUserParam("p31_framework-filter_sql_" & Me.GridPrefix))
                End With



                Handle_DefaultSelectedRecord()



            End If

        End If
    End Sub

    Private Sub InitialGroupByCombo(intTabIndex As Integer)
        With Me.cbxGroupBy
            .Items.Clear()
            .Items.Add(New ListItem(BL.My.Resources.common.BezSouhrnu, ""))
            .Items.Add(New ListItem(BL.My.Resources.common.Klient, "Client"))
            If intTabIndex = 2 Then
                .Items.Add(New ListItem(BL.My.Resources.common.TypUkolu, "p57Name"))
                .Items.Add(New ListItem(BL.My.Resources.common.Projekt, "ProjectCodeAndName"))
                .Items.Add(New ListItem(BL.My.Resources.common.Prijemce, "ReceiversInLine"))
                .Items.Add(New ListItem(BL.My.Resources.common.Milnik, "o22Name"))
                .Items.Add(New ListItem(BL.My.Resources.common.VlastnikZaznamu, "Owner"))

            Else
                .Items.Add(New ListItem(BL.My.Resources.common.TypProjektu, "p42Name"))
                .Items.Add(New ListItem(BL.My.Resources.common.Stredisko, "j18Name"))
                
            End If
        End With
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
        With cS
            .AllowCustomPaging = True
            .AllowMultiSelect = False
            .AllowMultiSelectCheckboxSelector = False
            .FilterSetting = strFilterSetting
            .FilterExpression = strFilterExpression
        End With

        If tabs1.SelectedIndex = 0 Then
            cS.PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue), True, False, , strFilterSetting, strFilterExpression, , strAddSqlFrom)
        Else
            cS.PageSize = 100
            cS.AllowCustomPaging = False
            ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, 100, False, False, , strFilterSetting, strFilterExpression, , strAddSqlFrom)
        End If
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM

        If cJ70.j70ScrollingFlag > BO.j70ScrollingFlagENUM.NoScrolling Then
            navigationPane.Scrolling = SplitterPaneScrolling.None
        End If

        If tabs1.SelectedIndex = 1 Or tabs1.SelectedIndex = 3 Then grid1.AllowFilteringByColumn = False 'v top10 a v oblíbených se nefiltruje

        With grid1
            .radGridOrig.ShowFooter = False
        End With
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
        
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
        

        If Me.GridPrefix = "p41" Then
            'If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
            'dataItem("systemcolumn").Text = "<a title='Zapsat úkon' href='javascript:nw(" & cRec.Item("pid").ToString & ")'><img src='Images/new.png' border=0/></a>"
            basUIMT.p41_grid_Handle_ItemDataBound(sender, e, True, "", "p31_framework")
            'With dataItem("systemcolumn")
            '    .Text = "<a class='reczoom' title='Detail projektu' rel='clue_p41_myworksheet.aspx?parent_url_reload=p31_framework.aspx&pid=" & cRec.Item("pid").ToString & "&j02id=" & Me.CurrentJ02ID.ToString & "' style='margin-left:-10px;'>i</a>" & .Text
            'End With
        Else
            basUIMT.p56_grid_Handle_ItemDataBound(sender, e, False, True, "", "p31_framework")
            ''With dataItem("systemcolumn")
            ''    .Text = "<a class='reczoom' title='Detail úkolu' rel='clue_p56_record.aspx?&pid=" & cRec.Item("pid").ToString & "' style='margin-left:-10px;'>i</a>"
            ''End With
            ''If Not cRec.Item("p56PlanUntil_Grid") Is System.DBNull.Value Then
            ''    If Now > cRec.Item("p56PlanUntil_Grid") Then dataItem.ForeColor = Drawing.Color.DarkRed
            ''End If
        End If



    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If Not navigationPane.Visible Then Return
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("p31_framework-filter_setting_" & Me.GridPrefix, grid1.GetFilterSetting())
                .SetUserParam("p31_framework-filter_sql_" & Me.GridPrefix, grid1.GetFilterExpression())
            End With
            Select Case tabs1.SelectedIndex
                Case 0
                    RecalcVirtualRowCount()
                Case 2
                    RecalcTasksCount()
            End Select
        End If
        If Me.GridPrefix = "p41" Then
            Dim mq As New BO.myQueryP41
            With mq
                If tabs1.SelectedIndex = 0 Then
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                Else
                    .TopRecordsOnly = 0
                    .MG_PageSize = 0
                    .MG_CurrentPageIndex = 0
                End If

            End With

            InhaleMyQuery(mq)
            If tabs1.SelectedIndex = 3 Then mq.IsFavourite = BO.BooleanQueryMode.TrueQuery

            Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
            If dt Is Nothing Then
                Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
            Else
                If tabs1.SelectedIndex = 1 Then
                    dt = basUIMT.QueryProjectListByTop10(Master.Factory, Me.CurrentJ02ID, Me.hidCols.Value, Me.cbxGroupBy.SelectedValue)
                    'lis = basUIMT.QueryProjectListByTop10(Master.Factory, Me.CurrentJ02ID)    'omezit na TOP 10
                End If
                If tabs1.SelectedIndex = 3 And dt.Rows.Count = 0 Then
                    Master.Notify("Váš seznam oblíbených projektů je prázdný.")
                End If
                grid1.DataSourceDataTable = dt
            End If

        End If
        If Me.GridPrefix = "p56" Then
            Dim mq As New BO.myQueryP56
            InhaleMyTaskQuery(mq)
            grid1.DataSourceDataTable = Master.Factory.p56TaskBL.GetGridDataSource(mq)

        End If


    End Sub

    ''Private Function QueryProjectListByTop10(lis As IEnumerable(Of BO.p41Project)) As IEnumerable(Of BO.p41Project)
    ''    Dim mqP31 As New BO.myQueryP31
    ''    If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then
    ''        mqP31.j02ID = Me.CurrentJ02ID
    ''    Else
    ''        mqP31.j02ID = Master.Factory.SysUser.j02ID
    ''    End If
    ''    mqP31.TopRecordsOnly = 100
    ''    mqP31.MG_SortString = "p31dateinsert desc"
    ''    Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mqP31)
    ''    Dim p41ids As New List(Of Integer)
    ''    If lisP31.Count > 0 Then
    ''        If lisP31.Select(Function(p) p.p41ID).Distinct.Count > 10 Then
    ''            For Each c In lisP31
    ''                If p41ids.Where(Function(p) p = c.p41ID).Count = 0 Then
    ''                    p41ids.Add(c.p41ID)
    ''                End If
    ''                If p41ids.Count >= 10 Then Exit For
    ''            Next
    ''        Else
    ''            p41ids = lisP31.Select(Function(p) p.p41ID).Distinct.ToList
    ''        End If
    ''    Else
    ''        p41ids.Add(-1)
    ''    End If
    ''    Dim mqP41 As New BO.myQueryP41
    ''    mqP41.PIDs = p41ids
    ''    mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
    ''    Return Master.Factory.p41ProjectBL.GetList(mqP41)
    ''End Function

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP41)
        With mq
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value

            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
            If tabs1.SelectedIndex = 0 Then
                .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            End If


            If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then .j02ID_ExplicitQueryFor = Me.CurrentJ02ID


        End With

    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_framework-pagesize-" & Me.GridPrefix, Me.cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    

  



    Private Sub Handle_DefaultSelectedRecord()
        If Me.GridPrefix = "p56" Then Return

        Dim intSelPID As Integer = 0
        If Not Page.IsPostBack Then
            Dim strDefPID As String = Request.Item("p41id")

            If strDefPID > "" Then intSelPID = BO.BAS.IsNullInt(strDefPID)
        End If

        If intSelPID > 0 Then
            'označit výchozí projekt
            grid1.SelectRecords(intSelPID)
            If grid1.GetSelectedPIDs.Count = 0 Then
                'hledaný projekt nebyl nalezen na první stránce
                Dim mq As New BO.myQueryP41
                InhaleMyQuery(mq)
                mq.MG_SelectPidFieldOnly = True
                mq.TopRecordsOnly = 0
                Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq), x As Integer, intNewPageIndex As Integer = 0
                If lis Is Nothing Then
                    Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                    Return
                End If
                For Each c In lis
                    x += 1
                    If x > grid1.PageSize Then
                        intNewPageIndex += 1 : x = 1
                    End If
                    If c.PID = intSelPID Then
                        grid1.radGridOrig.CurrentPageIndex = intNewPageIndex
                        grid1.Rebind(False)
                        grid1.SelectRecords(intSelPID)
                    End If
                Next

            End If

        End If
    End Sub

   


    Private Sub p31_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        
        Dim b As Boolean = False
        Select Case Me.tabs1.SelectedIndex
            Case 0
                b = True
                img1.ImageUrl = "Images/project_32.png"
                ''lblFormHeader.Text = ""
            Case 1
                img1.ImageUrl = "Images/project_32.png"
                ''lblFormHeader.Text = Resources.p31_framework.tabs1_p41
            Case 2
                img1.ImageUrl = "Images/task_32.png"
                ''lblFormHeader.Text = Resources.p31_framework.tabs1_todo
        End Select
        ''Me.designer1.Visible = b
        If cbxGroupBy.SelectedIndex = 0 Then
            chkGroupsAutoExpanded.Visible = False
        Else
            chkGroupsAutoExpanded.Visible = True
        End If

        If grid1.GetFilterExpression <> "" Then
            cmdCĺearFilter.Visible = True
        Else
            cmdCĺearFilter.Visible = False
        End If
        

    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        

        Me.hidHardRefreshFlag.Value = ""
        Me.hidHardRefreshPID.Value = ""
    End Sub

    
    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With grid1.radGridOrig.MasterTableView
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
    Private Sub cbxGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGroupBy.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_framework-groupby-" & Me.GridPrefix, Me.cbxGroupBy.SelectedValue)
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
        grid1.Rebind(True)
    End Sub
    
    Private Sub ReloadPage()
        Response.Redirect("p31_framework.aspx")
    End Sub

    Private Sub RecalcVirtualRowCount()
        Dim mq As New BO.myQueryP41
        InhaleMyQuery(mq)
        grid1.VirtualRowCount = Master.Factory.p41ProjectBL.GetVirtualCount(mq)

        grid1.radGridOrig.CurrentPageIndex = 0
        With tabs1.Tabs(0)
            .Text = BO.BAS.OM2(.Text, grid1.VirtualRowCount.ToString)
        End With
        'Me.lblFormHeader.Text = "Worksheet (" & BO.BAS.FNI(grid1.VirtualRowCount) & ")"
    End Sub
    Private Sub RecalcTasksCount()
        With tabs1.Tabs(2)
            .Text = BO.BAS.OM2(.Text, GetTasksCount().ToString)
        End With
    End Sub


   

    Private Sub chkGroupsAutoExpanded_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupsAutoExpanded.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_framework-groups-autoexpanded", BO.BAS.GB(Me.chkGroupsAutoExpanded.Checked))
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
        grid1.Rebind(True)


    End Sub

    Private Sub tabs1_TabClick(sender As Object, e As RadTabStripEventArgs) Handles tabs1.TabClick
        Master.Factory.j03UserBL.SetUserParam("p31_framework-tabindex", tabs1.SelectedIndex.ToString)
        ReloadPage()
    End Sub

    
    Private Function GetTasksCount() As Integer
        Dim mq As New BO.myQueryP56
        InhaleMyTaskQuery(mq)
        Return Master.Factory.p56TaskBL.GetVirtualCount(mq)
    End Function
    Private Function GetTasksList() As IEnumerable(Of BO.p56Task)
        Dim mq As New BO.myQueryP56
        InhaleMyTaskQuery(mq)

        Return Master.Factory.p56TaskBL.GetList(mq, Me.IsUseReceiversInLine)
    End Function
    ''Private Function GetTasksListWithWorksheetSum() As IEnumerable(Of BO.p56TaskWithWorksheetSum)
    ''    Dim mq As New BO.myQueryP56
    ''    InhaleMyTaskQuery(mq)

    ''    Return Master.Factory.p56TaskBL.GetList_WithWorksheetSum(mq, Me.IsUseReceiversInLine)
    ''End Function

    Private Sub InhaleMyTaskQuery(ByRef mq As BO.myQueryP56)
        With mq
            .j02ID = Me.CurrentJ02ID
            .SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .Closed = BO.BooleanQueryMode.FalseQuery
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()

        End With
        

    End Sub

    Private Sub cmdCĺearFilter_Click(sender As Object, e As EventArgs) Handles cmdCĺearFilter.Click
        With Master.Factory.j03UserBL
            .SetUserParam("p31_framework-filter_setting_" & Me.GridPrefix, "")
            .SetUserParam("p31_framework-filter_sql_" & Me.GridPrefix, "")
        End With
        ReloadPage()
    End Sub

    Private Sub grid1_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid1.SortCommand
        Master.Factory.j03UserBL.SetUserParam("p31_framework-sort-" & Me.GridPrefix, SortExpression)
    End Sub

   
End Class