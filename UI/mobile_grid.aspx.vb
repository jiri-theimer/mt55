Public Class mobile_grid
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Property _x29id As BO.x29IdEnum
    Private Property _needFilterIsChanged As Boolean = False
    Private Property _CurFilterDbField As String = ""

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
   

    Private Sub mobile_grid_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            Me.hidClosedQueryValue.Value = Request.Item("closed")
            With Master
                If Me.CurrentX29ID = BO.x29IdEnum.p91Invoice And Not .Factory.SysUser.j04IsMenu_Invoice Then
                    .StopPage("Nemáte oprávnění k přehledu faktur.")
                End If
                If Me.CurrentX29ID = BO.x29IdEnum.p28Contact And Not .Factory.SysUser.j04IsMenu_Contact And Me.CurrentMasterPrefix = "" Then
                    .StopPage("Nemáte oprávnění k přehledu klientů.")
                End If
                If Me.CurrentX29ID = BO.x29IdEnum.p41Project And Not .Factory.SysUser.j04IsMenu_Project And Me.CurrentMasterPrefix = "" Then
                    .StopPage("Nemáte oprávnění k přehledu projektů.")
                End If
                .MenuPrefix = Me.CurrentPrefix
                designer1.x36Key = Me.CurrentPrefix + "_mobile_grid-j70id"
                designer1.ReloadUrl = "mobile_grid.aspx?" & basUI.GetCompleteQuerystring(Request)
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(Me.CurrentPrefix + "_mobile_grid-pagesize")
                    .Add(designer1.x36Key)
                    .Add("periodcombo-custom_query")
                    .Add(Me.CurrentPrefix + "_framework_detail-pid")
                    .Add(Me.CurrentPrefix + "_mobile_grid-sort")
                    .Add(Me.CurrentPrefix + "_mobile_grid-period")
                    .Add(Me.CurrentPrefix + "_mobile_grid-filter_setting")
                    .Add(Me.CurrentPrefix + "_mobile_grid-filter_sql")
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Dim strJ70ID As String = Request.Item("j70id")
                    If strJ70ID = "" Then strJ70ID = .GetUserParam(designer1.x36Key)
                    designer1.RefreshData(BO.BAS.IsNullInt(strJ70ID))
                    If Me.CurrentMasterPID <> 0 Then
                        With Me.MasterRecord
                            .Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix), Me.CurrentMasterPID)
                            .NavigateUrl = "mobile_" & Me.CurrentMasterPrefix & "_framework.aspx?pid=" & Me.CurrentMasterPID.ToString
                            .Visible = True
                        End With
                    End If

                    basUI.SelectDropdownlistValue(cbxPaging, .GetUserParam(Me.CurrentPrefix + "_mobile_grid-pagesize", "10"))


                    If .GetUserParam(Me.CurrentPrefix + "_mobile-grid-sort") <> "" Then
                        grid1.radGridOrig.MasterTableView.SortExpressions.AddSortExpression(.GetUserParam(Me.CurrentPrefix + "_mobile_grid-sort"))
                    End If
                    SetupGrid(.GetUserParam(Me.CurrentPrefix + "_mobile_grid-filter_setting"), .GetUserParam(Me.CurrentPrefix + "_mobile_grid-filter_sql"))
                    Select Case Me.CurrentX29ID
                        Case BO.x29IdEnum.p31Worksheet, BO.x29IdEnum.p91Invoice
                            period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"), , True)
                            period1.SelectedValue = .GetUserParam(Me.CurrentPrefix + "_mobile_grid-period")
                        Case Else
                            period1.Visible = False
                    End Select

                End With

            End With
            RecalcVirtualRowCount()
        End If
    End Sub


    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)

        Me.hidDefaultSorting.Value = cJ70.j70OrderBy
        Dim strAddtionalSqlFrom As String = ""
        Me.hidCols.Value = basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue), True, False, False, strFilterSetting, strFilterExpression, , strAddtionalSqlFrom)
        Me.hidAdditionalFrom.Value = strAddtionalSqlFrom
        Me.hidFirstLinkCol.Value = grid1.radGridOrig.Columns(1).UniqueName
        With grid1
            .AllowFilteringByColumn = False
            .radGridOrig.RenderMode = Telerik.Web.UI.RenderMode.Auto
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p91Invoice
                    .radGridOrig.ShowFooter = True
                Case Else
                    .radGridOrig.ShowFooter = False
            End Select

        End With

    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
        _CurFilterDbField = strFilterColumn
    End Sub



    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        Select Case Me.CurrentX29ID
            'Case BO.x29IdEnum.p31Worksheet
            '    basUIMT.p31_grid_Handle_ItemDataBound(sender, e, True, Me.hidFirstLinkCol.Value)
            Case BO.x29IdEnum.p41Project
                basUIMT.p41_grid_Handle_ItemDataBound(sender, e, True, Me.hidFirstLinkCol.Value, "")
            Case BO.x29IdEnum.p28Contact
                basUIMT.p28_grid_Handle_ItemDataBound(sender, e, True, Me.hidFirstLinkCol.Value)
            Case BO.x29IdEnum.o23Doc
                basUIMT.o23_grid_Handle_ItemDataBound(sender, e, False, Me.hidFirstLinkCol.Value, "")

            Case BO.x29IdEnum.p56Task
                basUIMT.p56_grid_Handle_ItemDataBound(sender, e, False, True, Me.hidFirstLinkCol.Value, "")
            Case BO.x29IdEnum.p91Invoice
                basUIMT.p91_grid_Handle_ItemDataBound(sender, e, True, Me.hidFirstLinkCol.Value)
        End Select
    End Sub

    
    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam(Me.CurrentPrefix + "_mobile_grid-filter_setting", grid1.GetFilterSetting())
                .SetUserParam(Me.CurrentPrefix + "_mobile_grid-filter_sql", grid1.GetFilterExpression())
            End With
            RecalcVirtualRowCount()
        End If
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p31Worksheet
                Dim mq As New BO.myQueryP31
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p31(mq)
                Dim dt As DataTable = Master.Factory.p31WorksheetBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.p41Project
                Dim mq As New BO.myQueryP41
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p41(mq)

                Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.p28Contact
                Dim mq As New BO.myQueryP28
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex

                End With
                InhaleMyQuery_p28(mq)

                Dim dt As DataTable = Master.Factory.p28ContactBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If

            Case BO.x29IdEnum.p56Task
                Dim mq As New BO.myQueryP56
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p56(mq)

                Dim dt As DataTable = Master.Factory.p56TaskBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p56TaskBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.p91Invoice
                Dim mq As New BO.myQueryP91
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_p91(mq)

                Dim dt As DataTable = Master.Factory.p91InvoiceBL.GetGridDataSource(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.p91InvoiceBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case BO.x29IdEnum.o23Doc
                Dim mq As New BO.myQueryO23(0)
                With mq
                    .MG_PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
                    .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
                End With
                InhaleMyQuery_o23(mq)

                Dim dt As DataTable = Master.Factory.o23DocBL.GetDataTable4Grid(mq)
                If dt Is Nothing Then
                    Master.Notify(Master.Factory.o23DocBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Else
                    grid1.DataSourceDataTable = dt
                End If
            Case Else

        End Select
    End Sub

    Private Sub RecalcVirtualRowCount()
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p31Worksheet
                Dim mq As New BO.myQueryP31
                InhaleMyQuery_p31(mq)
                grid1.VirtualRowCount = Master.Factory.p31WorksheetBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.p41Project
                Dim mq As New BO.myQueryP41
                InhaleMyQuery_p41(mq)
                grid1.VirtualRowCount = Master.Factory.p41ProjectBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.p28Contact
                Dim mq As New BO.myQueryP28
                InhaleMyQuery_p28(mq)
                grid1.VirtualRowCount = Master.Factory.p28ContactBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.p56Task
                Dim mq As New BO.myQueryP56
                InhaleMyQuery_p56(mq)
                grid1.VirtualRowCount = Master.Factory.p56TaskBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.o23Doc
                Dim mq As New BO.myQueryO23(0)
                InhaleMyQuery_o23(mq)
                grid1.VirtualRowCount = Master.Factory.o23DocBL.GetVirtualCount(mq)
            Case BO.x29IdEnum.p91Invoice
                Dim mq As New BO.myQueryP91
                InhaleMyQuery_p91(mq)
                Dim cSum As BO.p91InvoiceSum = Master.Factory.p91InvoiceBL.GetSumRow(mq)
                grid1.VirtualRowCount = cSum.Count
                Me.hidFooterSum.Value = grid1.GenerateFooterItemString(cSum)
        End Select
        grid1.radGridOrig.CurrentPageIndex = 0
        lblRowsCount.Text = grid1.VirtualRowCount.ToString
    End Sub
    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_mobile_grid-pagesize", Me.cbxPaging.SelectedValue)
        ReloadPage()

    End Sub
    Private Sub ReloadPage()
        'Dim s As String = "mobile_grid.aspx?prefix=" & Me.CurrentPrefix
        'If Me.CurrentMasterPID > 0 Then s += "&masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString
        Response.Redirect("mobile_grid.aspx?" & basUI.GetCompleteQuerystring(Request), True)
    End Sub
    Private Sub InhaleMyQuery_p41(ByRef mq As BO.myQueryP41)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            Select Case Me.CurrentMasterPrefix
                Case "p28" : .p28ID = Me.CurrentMasterPID
                Case "p91" : .p91ID = Me.CurrentMasterPID
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

            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
            .j70ID = designer1.CurrentJ70ID
        End With
    End Sub
    Private Sub InhaleMyQuery_p28(ByRef mq As BO.myQueryP28)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
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

            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead
            .j70ID = designer1.CurrentJ70ID
        End With
    End Sub
    Private Sub InhaleMyQuery_p56(ByRef mq As BO.myQueryP56)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
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
            .p56PlanUntil_D1 = period1.DateFrom : .p56PlanUntil_D2 = period1.DateUntil
            Select Case Me.hidClosedQueryValue.Value
                Case "1" : .Closed = BO.BooleanQueryMode.TrueQuery
                Case "0" : .Closed = BO.BooleanQueryMode.FalseQuery
                Case Else : .Closed = BO.BooleanQueryMode.NoQuery
            End Select

            .j70ID = designer1.CurrentJ70ID
            .SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
        End With
    End Sub
    Private Sub InhaleMyQuery_p31(ByRef mq As BO.myQueryP31)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41ID = Me.CurrentMasterPID
                Case "p28" : .p28ID_Client = Me.CurrentMasterPID
                Case "j02" : .j02ID = Me.CurrentMasterPID
                Case "p56"
                    .p56IDs = New List(Of Integer)
                    .p56IDs.Add(Me.CurrentMasterPID)
                Case "p91" : .p91ID = Me.CurrentMasterPID
                Case Else
            End Select
            .j70ID = designer1.CurrentJ70ID
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()

            .SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            If period1.SelectedValue <> "" Then
                .DateFrom = period1.DateFrom
                .DateUntil = period1.DateUntil
            End If
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
    Private Sub InhaleMyQuery_p91(ByRef mq As BO.myQueryP91)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidAdditionalFrom.Value
            .Closed = BO.BooleanQueryMode.NoQuery
            Select Case Me.CurrentMasterPrefix
                Case "p41" : .p41ID = Me.CurrentMasterPID
                Case "j02" : .j02ID = Me.CurrentMasterPID
                Case "p28" : .p28ID = Me.CurrentMasterPID
            End Select
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            .PeriodType = BO.myQueryP91_PeriodType.p91DateSupply
            .DateFrom = period1.DateFrom
            .DateUntil = period1.DateUntil

            .j70ID = designer1.CurrentJ70ID

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
    Private Sub InhaleMyQuery_o23(ByRef mq As BO.myQueryO23)
        With mq
            .MG_GridSqlColumns = Me.hidCols.Value
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
            .Closed = BO.BooleanQueryMode.NoQuery
            .j70ID = designer1.CurrentJ70ID
            .SpecificQuery = BO.myQueryO23_SpecificQuery.AllowedForRead
        End With

    End Sub

    Private Sub grid1_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid1.SortCommand
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_mobile_grid-sort", SortExpression)
    End Sub

    

    Private Sub mobile_grid_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub

   
    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_mobile_grid-period", period1.SelectedValue)
        ReloadPage()
    End Sub
End Class