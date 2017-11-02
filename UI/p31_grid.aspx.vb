Imports Telerik.Web.UI

Public Class p31_grid
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Property _curJ62 As BO.j62MenuHome
    Private Property _needFilterIsChanged As Boolean = False
    Private Property _curIsExport As Boolean

    Private Sub p31_grid_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_grid"
    End Sub
    
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
            Master.SiteMenuValue = "hm" & value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            If Request.Item("masterpid") <> "" Then
                Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid")) : Me.CurrentMasterPrefix = Request.Item("masterprefix")
            End If
            If Request.Item("sgf") <> "" Then
                hidSGF.Value = Request.Item("sgf")
                hidSGV.Value = Request.Item("sgv")
                hidSGA.Value = Server.UrlDecode(Request.Item("sga"))
                hidDrillDownP31IDs.Value = Request.Item("drilldown_p31ids")
            End If
            If Request.Item("aw") <> "" Then
                Me.hidMasterAW.Value = Replace(Server.UrlDecode(Request.Item("aw")), "xxx", "=")
            End If

            With Master
                .PageTitle = "WORKSHEET datový přehled"

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p31_grid-pagesize")
                    .Add("p31_grid-query-p34id")
                    .Add("p31-j70id")
                    .Add("p31_grid-periodtype")
                    .Add("p31_grid-period")
                    .Add("periodcombo-custom_query")
                    .Add("p31_grid-groupby")
                    .Add("p31_grid-groups-autoexpanded")
                    .Add("p31_grid-sort")
                    .Add("p31_grid-filter_setting")
                    .Add("p31_grid-filter_sql")
                    .Add("p31_grid-tabqueryflag")
                    .Add("x18_querybuilder-value-p31-p31grid")
                    .Add("x18_querybuilder-text-p31-p31grid")
                    .Add("p31_grid-query-on-top")
                    .Add("o51_querybuilder-p31")
                End With
                cbxGroupBy.DataSource = .Factory.j70QueryTemplateBL.GroupByPallet(BO.x29IdEnum.p31Worksheet)
                cbxGroupBy.DataBind()
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Dim strDefTabQueryFlag As String = Request.Item("p31tabautoquery")
                    If strDefTabQueryFlag = "" Then strDefTabQueryFlag = Request.Item("tab")
                    If strDefTabQueryFlag = "" Then strDefTabQueryFlag = .GetUserParam("p31_grid-tabqueryflag", "p31")
                    basUI.SelectDropdownlistValue(Me.cbxTabQueryFlag, strDefTabQueryFlag)

                    basUI.SelectDropdownlistValue(cbxPaging, .GetUserParam("p31_grid-pagesize", "20"))
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("p31_grid-period")
                    basUI.SelectDropdownlistValue(Me.cbxPeriodType, .GetUserParam("p31_grid-periodtype", "p31Date"))

                    basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("p31_grid-groupby"))
                    hidX18_value.Value = .GetUserParam("x18_querybuilder-value-p31-p31grid")
                    Me.x18_querybuilder_info.Text = .GetUserParam("x18_querybuilder-text-p31-p31grid")

                    Dim cPT As BO.QueryByTags = Master.Factory.o51TagBL.ParseQueryByTags("p31", .GetUserParam("o51_querybuilder-p31"))
                    hidO51IDs.Value = cPT.o51IDsInline
                    Me.o51_querybuilder_info.Text = cPT.HtmlInline
                End With


            End With

            Me.CurrentJ62ID = BO.BAS.IsNullInt(Request.Item("j62id"))
            If Me.CurrentJ62ID <> 0 Then
                _curJ62 = Master.Factory.j62MenuHomeBL.Load(Me.CurrentJ62ID)
                If _curJ62 Is Nothing Then Master.StopPage("j62 record not found")
            Else
                Master.SiteMenuValue = "p31_grid"
            End If

            With Master.Factory.j03UserBL
                Me.chkGroupsAutoExpanded.Checked = BO.BAS.BG(.GetUserParam("p31_grid-groups-autoexpanded", "1"))
                Dim strJ70ID As String = Request.Item("j70id")
                If strJ70ID = "" Then strJ70ID = .GetUserParam("p31-j70id")
                designer1.RefreshData(BO.BAS.IsNullInt(strJ70ID))

                
                SetupGrid(.GetUserParam("p31_grid-filter_setting"), .GetUserParam("p31_grid-filter_sql"), .GetUserParam("p31_grid-sort"))

            End With


            If CurrentMasterPrefix <> "" Then
                panAdditionalQuery.Visible = True
                Me.MasterRecord.Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(hidMasterPrefix.Value), BO.BAS.IsNullInt(hidMasterPID.Value))
                Me.MasterRecord.NavigateUrl = hidMasterPrefix.Value & "_framework.aspx?pid=" & hidMasterPID.Value
                Select Case hidMasterPrefix.Value
                    Case "p41" : imgEntity.ImageUrl = "Images/project.png"
                    Case "j02" : imgEntity.ImageUrl = "Images/person.png"
                    Case "p28" : imgEntity.ImageUrl = "Images/contact.png"
                    Case "p91" : imgEntity.ImageUrl = "Images/invoice.png"
                    Case "o23"
                        imgEntity.ImageUrl = "Images/notepad.png"
                        Me.MasterRecord.NavigateUrl = "o23_fixwork.aspx?pid=" & hidMasterPID.Value
                End Select
            Else
                panAdditionalQuery.Visible = False
            End If
            If hidSGF.Value <> "" Then
                SetupSG()
            End If

            RecalcVirtualRowCount()
            Handle_Permissions()
        End If
        'If Me.chkQueryOnTop.Checked Then
        '    Dim ctl As New Control
        '    ctl = Me.clue_query
        '    Me.panCurrentQuery.Controls.Remove(Me.clue_query)
        '    Me.placeQuery.Controls.Add(ctl)
        '    ctl = New Control
        '    ctl = Me.j70ID
        '    Me.panJ70.Controls.Remove(Me.j70ID)
        '    Me.placeQuery.Controls.Add(ctl)
        'End If
    End Sub
    Private Sub SetupSG()
        panAdditionalQuery.Visible = True : cmdDrillDownClear.Visible = True
        

        Dim a() As String = Split(hidSGF.Value, "|"), b() As String = Split(hidSGV.Value, "|"), lis As List(Of BO.GridColumn) = Master.Factory.j70QueryTemplateBL.ColumnsPallete(BO.x29IdEnum.p31Worksheet), strW As String = ""
        For i As Integer = 0 To UBound(a)
            Dim strF As String = a(i)
            Dim c As BO.GridColumn = lis.Where(Function(p) p.ColumnName = strF).First
            Dim strLW As String = c.Pivot_GroupBySql
            If Trim(c.SqlSyntax_FROM) <> "" Then
                If hidFrom.Value = "" Then
                    hidFrom.Value = c.SqlSyntax_FROM
                Else
                    If hidFrom.Value.IndexOf(c.SqlSyntax_FROM) < 0 Then
                        hidFrom.Value += " " & c.SqlSyntax_FROM
                    End If
                End If
            End If
            If b(i) = "" Then
                strLW += " IS NULL"
            Else
                If strLW.IndexOf("convert") >= 0 Then
                    strLW += "='" & b(i) & "'"
                Else
                    strLW += "=" & b(i)
                End If
            End If

            If i = 0 Then
                lblDrillDown.Text = c.ColumnHeader
                strW = strLW
            Else
                lblDrillDown.Text += " -> " & c.ColumnHeader
                strW += " AND " & strLW
            End If
        Next
        If hidMasterAW.Value = "" Then
            hidMasterAW.Value = strW
        Else
            hidMasterAW.Value += " AND (" & strW & ")"
        End If
        lblDrillDown.Text = "<img src='Images/pivot.png' style='padding-right:6px;'/>" & lblDrillDown.Text & ":"
        linkDrillDown.Text = hidSGA.Value
        linkDrillDown.NavigateUrl = "p31_sumgrid.aspx?masterprefix=" + Me.CurrentMasterPrefix + "&masterpid=" + Me.CurrentMasterPID.ToString & "&tabqueryflag=" + Me.cbxTabQueryFlag.SelectedValue & "&pid=" & hidSGV.Value

        If hidDrillDownP31IDs.Value <> "" Then
            'na vstupu výběr p31ids
            linkDrillDown.NavigateUrl += "&p31ids=" & hidDrillDownP31IDs.Value
          
        End If
    End Sub
    
    

    
    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click

        Select Case Me.hidHardRefreshFlag.Value
         
            Case "quickquery"
                grid1.Rebind(False)
            Case "p31-save"
                grid1.Rebind(True)

            Case "p31-delete"

                ReloadPage()
            Case Else
                ReloadPage()
        End Select

        Me.hidHardRefreshFlag.Value = ""
        Me.hidHardRefreshPID.Value = ""


    End Sub

    

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-pagesize", Me.cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String, strSortExpression As String)
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Me.hidDefaultSorting.Value = cJ70.j70OrderBy
        ''Dim strAddSqlFrom As String = "", strSqlSumCols As String = ""
        Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
        With cS
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .AllowCustomPaging = True
            .AllowMultiSelect = Not _curIsExport
            .AllowMultiSelectCheckboxSelector = True
            .FilterSetting = strFilterSetting
            .FilterExpression = strFilterExpression
            .SortExpression = strSortExpression
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM
        Me.hidSumCols.Value = cG.SumCols

        ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue), True, Not _curIsExport, True, strFilterSetting, strFilterExpression, strSortExpression, strAddSqlFrom, , strSqlSumCols)
        ''Me.hidFrom.Value = strAddSqlFrom
        ''Me.hidSumCols.Value = strSqlSumCols
        

        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
    End Sub
    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        grid1.radGridOrig.GroupingSettings.RetainGroupFootersVisibility = True
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

    Private Sub grid1_DataBinding(sender As Object, e As EventArgs) Handles grid1.DataBinding

    End Sub

  

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, True, False, "p31_grid")
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

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If e.IsFromDetailTable Then
            Return
        End If
        Dim mq As New BO.myQueryP31
        With mq
            .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
        End With
        InhaleMyQuery(mq)

      
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("p31_grid-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("p31_grid-filter_sql", grid1.GetFilterExpression())
                .SetUserParam("p31_grid-filter_completesql", grid1.GetFilterExpressionCompleteSql())
            End With
            RecalcVirtualRowCount()
        End If
        If _curIsExport Then mq.MG_PageSize = 2000
        Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridDataSource(mq)
        If dt Is Nothing Then
            Master.Notify(Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            grid1.DataSourceDataTable = dt
        End If

        

        ''grid1.DataSource = Master.Factory.p31WorksheetBL.GetList(mq)

    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)
        With mq
            Select Case Me.CurrentMasterPrefix
                Case "p41"
                    .p41ID = Me.CurrentMasterPID
                Case "p28"
                    .p28ID_Client = Me.CurrentMasterPID
                Case "j02"
                    .j02ID = Me.CurrentMasterPID
                Case "p56"
                    .p56IDs = New List(Of Integer)
                    .p56IDs.Add(Me.CurrentMasterPID)
                Case "p91"
                    .p91ID = Me.CurrentMasterPID
                Case "o23"
                    .o23ID = Me.CurrentMasterPID
                Case Else

            End Select
            .j70ID = designer1.CurrentJ70ID
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()

            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            If period1.SelectedValue <> "" Then
                Select Case Me.cbxPeriodType.SelectedValue
                    Case "p91Date" : .PeriodType = BO.myQueryP31_Period.p91Date
                    Case "p91DateSupply" : .PeriodType = BO.myQueryP31_Period.p91DateSupply
                    Case "p31DateInsert" : .PeriodType = BO.myQueryP31_Period.p31DateInsert
                    Case Else
                        .PeriodType = BO.myQueryP31_Period.p31Date
                End Select
                .DateFrom = period1.DateFrom
                .DateUntil = period1.DateUntil
            End If
            .MG_AdditionalSqlWHERE = Me.hidMasterAW.Value
            
            .TabAutoQuery = cbxTabQueryFlag.SelectedValue
            .x18Value = Me.hidX18_value.Value
            If hidO51IDs.Value <> "" Then
                .o51IDs = BO.BAS.ConvertPIDs2List(hidO51IDs.Value)
            End If
            If hidDrillDownP31IDs.Value <> "" Then
                mq.PIDs = BO.BAS.ConvertPIDs2List(hidDrillDownP31IDs.Value)
            End If
        End With
        If Not Page.IsPostBack Then
            If Request.Item("pid") <> "" Then
                mq = New BO.myQueryP31    'vyčistit filtr
                mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
                mq.MG_AdditionalSqlFROM = Me.hidFrom.Value
                mq.MG_GridSqlColumns = Me.hidCols.Value
                mq.PIDs = BO.BAS.ConvertPIDs2List(Request.Item("pid"))
            End If
        End If
    End Sub


    
    Private Sub RecalcVirtualRowCount()
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)

        Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
        grid1.VirtualRowCount = dt.Rows(0).Item(0)
        Me.hidFooterString.Value = grid1.CompleteFooterString(dt, Me.hidSumCols.Value)

      
        grid1.radGridOrig.CurrentPageIndex = 0
        Me.lblFormHeader.Text = BO.BAS.FNI(grid1.VirtualRowCount) & "x"
        

    End Sub



    Private Sub ReloadPage()
        Response.Redirect(GetPage4Reload())
    End Sub
    Private Function GetPage4Reload() As String
        Dim s As String = ""
        If Me.CurrentMasterPID <> 0 Then
            s = basUI.AddQuerystring2Page(s, "masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString)
        End If
        If Me.CurrentJ62ID > 0 Then
            s = basUI.AddQuerystring2Page(s, "j62id=" & Me.CurrentJ62ID.ToString)
        End If
        If hidSGF.Value <> "" Then
            s = basUI.AddQuerystring2Page(s, "sgf=" & hidSGF.Value & "&sgv=" & hidSGV.Value & "&sga=" & hidSGA.Value)
        End If
        Return "p31_grid.aspx" & s
        
    End Function

    Private Sub p31_grid_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        designer1.ReloadUrl = GetPage4Reload()
       
        If grid1.GetFilterExpression <> "" Then
            cmdCĺearFilter.Visible = True
        Else
            cmdCĺearFilter.Visible = False
        End If

        Me.CurrentQuery.Text = ""
        
        With Me.cbxTabQueryFlag
            If .SelectedIndex > 0 Then
                .BackColor = basUI.ColorQueryRGB
                Me.CurrentQuery.Text += "<img src='Images/query.png' style='margin-left:20px;'/>" & .SelectedItem.Text
            Else
                .BackColor = Nothing
            End If
        End With
        If hidO51IDs.Value <> "" Then
            Me.CurrentQuery.Text += Me.o51_querybuilder_info.Text & "<a href='javascript:clear_o51()' title='Zrušit filtr štítků'><img src='Images/delete.png'></a>"
            cmdClearO51.Visible = True
        Else
            cmdClearO51.Visible = False
        End If
        If hidX18_value.Value <> "" Then
            Me.CurrentQuery.Text += "<img src='Images/query.png' style='margin-left:20px;'/><img src='Images/label.png'/>" & Me.x18_querybuilder_info.Text
            cmdClearX18.Visible = True
        Else
            cmdClearX18.Visible = False
        End If
        With Me.cbxPeriodType
            Select Case .SelectedIndex
                Case 1 : .BackColor = Drawing.Color.LightSkyBlue
                Case 2 : .BackColor = Drawing.Color.Green
                Case Else : .BackColor = Nothing
            End Select
        End With
        


        If cbxGroupBy.SelectedValue <> "" Then chkGroupsAutoExpanded.Visible = True Else chkGroupsAutoExpanded.Visible = False

        
    End Sub



    Private Sub grid1_NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object) Handles grid1.NeedFooterSource
        ''footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"


        grid1.ParseFooterItemString(footerItem, Me.hidFooterString.Value)

    End Sub

    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        Dim cXLS As New clsExportToXls(Master.Factory)
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)

        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)
        mq.MG_GridGroupByField = ""
        Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridDataSource(mq)

        Dim strFileName As String = cXLS.ExportDataGrid(dt.AsEnumerable, cJ70)
        If strFileName = "" Then
            Master.Notify(cXLS.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub Handle_Permissions()
        With Master.Factory
            ''menu1.FindItemByValue("cmdApprove").Visible = .SysUser.IsApprovingPerson
            menu1.FindItemByValue("cmdApprove").Visible = .TestPermission(BO.x53PermValEnum.GR_P31_Approver)
            menu1.FindItemByValue("cmdMove").Visible = .TestPermission(BO.x53PermValEnum.GR_P31_Owner)
            menu1.FindItemByValue("cmdInvoice").Visible = .TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator)
            panExport.Visible = .TestPermission(BO.x53PermValEnum.GR_GridTools)
            designer1.AllowSettingButton = panExport.Visible

            cmdSummary.Visible = .TestPermission(BO.x53PermValEnum.GR_P31_Pivot)
        End With
        
    End Sub

    Private Sub cbxGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGroupBy.SelectedIndexChanged
        Me.CurrentJ62ID = 0
        Master.Factory.j03UserBL.SetUserParam("p31_grid-groupby", Me.cbxGroupBy.SelectedValue)
        ReloadPage()

        ''With Me.cbxGroupBy.SelectedItem
        ''    SetupGrouping(.Value, .Text)
        ''End With
        ''grid1.Rebind(True)
    End Sub
   
  
    

    
    
    

    Private Sub cmdCĺearFilter_Click(sender As Object, e As EventArgs) Handles cmdCĺearFilter.Click
        With Master.Factory.j03UserBL
            .SetUserParam("p31_grid-filter_setting", "")
            .SetUserParam("p31_grid-filter_sql", "")
            .SetUserParam("p31_grid-filter_completesql", "")
        End With
        ReloadPage()
    End Sub
    Private Sub chkGroupsAutoExpanded_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupsAutoExpanded.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-groups-autoexpanded", BO.BAS.GB(Me.chkGroupsAutoExpanded.Checked))
        ReloadPage()
    End Sub

    Private Sub grid1_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid1.SortCommand
        ''If strOwnerTableName = "drilldown" Then Return 'neukládat třídění z drill-down
        Master.Factory.j03UserBL.SetUserParam("p31_grid-sort", SortExpression)
    End Sub

    Private Sub GridExport(strFormat As String)
        _curIsExport = True
        basUIMT.Handle_GridTelerikExport(grid1, strFormat)


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

   
    Private Sub cbxTabQueryFlag_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxTabQueryFlag.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-tabqueryflag", Me.cbxTabQueryFlag.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cbxPeriodType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPeriodType.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-periodtype", Me.cbxPeriodType.SelectedValue)
        ReloadPage()
    End Sub
    Private Sub cmdClearX18_Click(sender As Object, e As ImageClickEventArgs) Handles cmdClearX18.Click
        With Master.Factory.j03UserBL
            .SetUserParam("x18_querybuilder-value-p31-p31grid", "")
            .SetUserParam("x18_querybuilder-text-p31-p31grid", "")
        End With
        ReloadPage()
    End Sub
    
    Private Sub cmdClearO51_Click(sender As Object, e As ImageClickEventArgs) Handles cmdClearO51.Click
        With Master.Factory.j03UserBL
            .SetUserParam("o51_querybuilder-p31", "")
        End With
        ReloadPage()
    End Sub

    Private Sub cmdDrillDownClear_Click(sender As Object, e As ImageClickEventArgs) Handles cmdDrillDownClear.Click
        hidSGF.Value = ""
        hidSGV.Value = ""
        hidDrillDownP31IDs.Value = ""
        ReloadPage()
    End Sub
End Class