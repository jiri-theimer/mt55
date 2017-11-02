Imports Telerik.Web.UI

Public Class entity_framework_rec_p41
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Private Property _curIsExport As Boolean
    Public Property DefaultSelectedPID As Integer
    Public Property CurrentMasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property

    Private Sub entity_framework_rec_p41_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        designer1.Factory = Master.Factory
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                Me.CurrentMasterPrefix = Request.Item("masterprefix")
                If .DataPID = 0 Or Me.CurrentMasterPrefix = "" Then .StopPage("masterpid or masterprefix is missing")

                .SiteMenuValue = Me.CurrentMasterPrefix
                menu1.DataPrefix = Me.CurrentMasterPrefix
                If Me.menu1.PageSource = "2" Then
                    .IsHideAllRecZooms = True
                End If

                Dim mq As New BO.myQuery
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                x67ID.DataSource = Master.Factory.x67EntityRoleBL.GetList(mq).Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project)
                x67ID.DataBind()
                x67ID.Items.Insert(0, "--Filtr podle projektové role osoby--")

                designer1.MasterPrefix = Me.CurrentMasterPrefix
                designer1.x36Key = "entiy_framework_p41subform-j70id-" & Me.CurrentMasterPrefix
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(designer1.x36Key)
                    .Add(Me.CurrentMasterPrefix & "_menu-tabskin")
                    .Add("entiy_framework_p41subform-groupby-" & Me.CurrentMasterPrefix)
                    .Add("entiy_framework_p41subform-pagesize")
                    .Add("entiy_framework_p41subform-validity")
                    .Add("entiy_framework_p41subform-x67id")
                    .Add(Me.CurrentMasterPrefix & "_menu-x31id-plugin")
                    ''.Add(Me.CurrentMasterPrefix & "_menu-menuskin")
                    .Add(Me.CurrentMasterPrefix & "_menu-remember-tab")
                    .Add(Me.CurrentMasterPrefix & "_framework_detail-tab")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    menu1.TabSkin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-tabskin")
                    ''menu1.MenuSkin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-menuskin")
                    menu1.x31ID_Plugin = .GetUserParam(Me.CurrentMasterPrefix & "_menu-x31id-plugin")
                    If .GetUserParam(Me.CurrentMasterPrefix & "_menu-remember-tab", "0") = "1" Then
                        menu1.LockedTab = .GetUserParam(Me.CurrentMasterPrefix & "_framework_detail-tab")
                    End If

                    Dim strJ70ID As String = Request.Item("j70id")
                    If strJ70ID = "" Then strJ70ID = .GetUserParam(designer1.x36Key, "0")
                    designer1.RefreshData(CInt(strJ70ID))

                    
                    basUI.SelectDropdownlistValue(Me.x67ID, .GetUserParam("entiy_framework_p41subform-x67id"))
                    basUI.SelectDropdownlistValue(Me.cbxValidity, .GetUserParam("entiy_framework_p41subform-validity", "1"))
                    basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("entiy_framework_p41subform-pagesize", "10"))
                    basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("entiy_framework_p41subform-groupby-" & Me.CurrentMasterPrefix))
                End With
            End With
            With Master.Factory
                panExport.Visible = .TestPermission(BO.x53PermValEnum.GR_GridTools)
                recmenu1.FindItemByValue("new").Visible = .TestPermission(BO.x53PermValEnum.GR_P41_Creator)
            End With

            SetupGrid()


        End If
        RefreshRecord()
    End Sub

    Private Sub RefreshRecord()
        Select Case Me.CurrentMasterPrefix
            Case "p41"
                Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p41")
                Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(cRec.p42ID)
                Dim cRecSum As BO.p41ProjectSum = Master.Factory.p41ProjectBL.LoadSumRow(cRec.PID)
                menu1.p41_RefreshRecord(cRec, cRecSum, "p41")
            Case "p28"
                Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p28")
                Dim cRecSum As BO.p28ContactSum = Master.Factory.p28ContactBL.LoadSumRow(cRec.PID)
                menu1.p28_RefreshRecord(cRec, cRecSum, "p41")
            Case "j02"
                Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
                If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=j02")
                Dim cRecSum As BO.j02PersonSum = Master.Factory.j02PersonBL.LoadSumRow(cRec.PID)
                menu1.j02_RefreshRecord(cRec, cRecSum, "p41")
        End Select

    End Sub

    Private Sub SetupGrid()
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Me.hidDefaultSorting.Value = cJ70.j70OrderBy
        Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
        With cS
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .AllowCustomPaging = False
            .AllowMultiSelect = Not _curIsExport
            .AllowMultiSelectCheckboxSelector = True

        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM
        
        ''Dim strAddSqlFrom As String = ""
        ''Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, CInt(Me.cbxPaging.SelectedValue),  False, Not _curIsExport,  True, , , , strAddSqlFrom)
        ''Me.hidFrom.Value = strAddSqlFrom
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With



    End Sub

    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With grid1.radGridOrig.MasterTableView
            .GroupByExpressions.Clear()
            If strGroupField = "" Then Return
            .ShowGroupFooter = True
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
        Master.Factory.j03UserBL.SetUserParam("entiy_framework_p41subform-groupby-" & Me.CurrentMasterPrefix, Me.cbxGroupBy.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entiy_framework_p41subform-pagesize", Me.cbxPaging.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        Response.Redirect(GetReloadedUrl(), True)
    End Sub
    Private Function GetReloadedUrl() As String
        Return "entity_framework_rec_p41.aspx?masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Master.DataPID.ToString
    End Function

    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cXLS As New clsExportToXls(Master.Factory)

        Dim mq As New BO.myQueryP41
        InhaleQuery(mq)
        mq.MG_GridGroupByField = ""

        Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)

        Dim strFileName As String = cXLS.ExportDataGrid(dt.AsEnumerable, cJ70)
        If strFileName = "" Then
            Response.Write(cXLS.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p41_grid_Handle_ItemDataBound(sender, e, True, "", Me.hidMasterPrefix.Value)
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

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource

        Dim mq As New BO.myQueryP41
        InhaleQuery(mq)

        Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)

        If dt Is Nothing Then
            Return
        Else
            grid1.DataSourceDataTable = dt
        End If

        lblHeaderP41.Text = BO.BAS.OM2(lblHeaderP41.Text, dt.Rows.Count.ToString)

        If Me.DefaultSelectedPID <> 0 Then
            If dt.AsEnumerable.Where(Function(p) p.Item("pid") = Me.DefaultSelectedPID).Count > 0 Then
                'záznam je na první stránce
            Else
                Dim mqAll As New BO.myQueryP41
                mqAll.TopRecordsOnly = 0
                mqAll.MG_SelectPidFieldOnly = True
                InhaleQuery(mqAll)
                Dim dtAll As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mqAll)
                Dim x As Integer, intNewPageIndex As Integer = 0
                For Each dbRow As DataRow In dtAll.Rows
                    x += 1
                    If x > grid1.PageSize Then
                        intNewPageIndex += 1 : x = 1
                    End If
                    If dbRow.Item("pid") = Me.DefaultSelectedPID Then
                        InhaleQuery(mq)
                        grid1.radGridOrig.CurrentPageIndex = intNewPageIndex
                        mq.MG_CurrentPageIndex = intNewPageIndex
                        dt = Master.Factory.p41ProjectBL.GetGridDataSource(mq) 'nový zdroj pro grid
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub InhaleQuery(ByRef mq As BO.myQueryP41)
        mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead

        Select Case Me.CurrentMasterPrefix
            Case "p41"
                mq.p41ParentID = Master.DataPID
            Case "p28"
                mq.p28ID = Master.DataPID
            Case "j02"
                mq.j02ID_ExplicitQueryFor = Master.DataPID
                mq.x67ID_ProjectRole = BO.BAS.IsNullInt(Me.x67ID.SelectedValue)

            Case Else

        End Select

        With mq
            Select Case Me.cbxValidity.SelectedValue
                Case "1" : .Closed = BO.BooleanQueryMode.NoQuery
                Case "2" : .Closed = BO.BooleanQueryMode.FalseQuery
                Case "3" : .Closed = BO.BooleanQueryMode.TrueQuery
            End Select
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If

        End With
    End Sub

    Private Sub GridExport(strFormat As String)
        _curIsExport = True

        SetupGrid()
        basUIMT.Handle_GridTelerikExport(Me.grid1, strFormat)
    End Sub
    Private Sub cbxValidity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxValidity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entiy_framework_p41subform-validity", Me.cbxValidity.SelectedValue)
        ReloadPage()
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


    Private Sub x67ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x67ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entiy_framework_p41subform-x67id", Me.x67ID.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub entity_framework_rec_p41_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.CurrentMasterPrefix = "j02" Then
            Me.x67ID.Visible = True
        Else
            Me.x67ID.Visible = False
        End If
        designer1.ReloadUrl = GetReloadedUrl()
    End Sub
End Class