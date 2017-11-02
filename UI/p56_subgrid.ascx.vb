Imports Telerik.Web.UI
Public Class p56_subgrid
    Inherits System.Web.UI.UserControl
    Public Property MasterDataPID As Integer
    Public Property DefaultSelectedPID As Integer = 0
    Public Property Factory As BL.Factory
    Public Property x29ID As BO.x29IdEnum
    Private Property _curIsExport As Boolean

    Public Property AllowApproving As Boolean
        Get
            Return recmenu1.FindItemByValue("cmdApprove").Visible
        End Get
        Set(value As Boolean)
            recmenu1.FindItemByValue("cmdApprove").Visible = value

        End Set
    End Property
    
    Public Property IsAllowedCreateTasks As Boolean
        Get
            Return recmenu1.FindItemByValue("new").Visible
        End Get
        Set(value As Boolean)
            recmenu1.FindItemByValue("new").Visible = value
            recmenu1.FindItemByValue("clone").Visible = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Me.Factory
        If Not Page.IsPostBack Then
            With Factory.j03UserBL
                designer1.MasterPrefix = BO.BAS.GetDataPrefix(Me.x29ID)
                designer1.x36Key = "p56_subgrid-j70id-" & designer1.MasterPrefix
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add(designer1.x36Key)
                    .Add("p56_subgrid-groupby-" & BO.BAS.GetDataPrefix(Me.x29ID))
                    .Add("p56_subgrid-pagesize")
                    .Add("p56_subgrid-cbxP56Validity")
                End With
                .InhaleUserParams(lisPars)
                Dim strJ70ID As String = Request.Item("j70id")
                If strJ70ID = "" Then strJ70ID = .GetUserParam(designer1.x36Key, "0")
                designer1.RefreshData(CInt(strJ70ID))

                
                basUI.SelectDropdownlistValue(Me.cbxP56Validity, .GetUserParam("p56_subgrid-cbxP56Validity", "1"))
                basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("p56_subgrid-pagesize", "10"))
                basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("p56_subgrid-groupby-" & BO.BAS.GetDataPrefix(Me.x29ID)))
            End With
            panExport.Visible = Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
            RecalcVirtualRowCount()
            SetupGridP56()

        End If
    End Sub

    Private Sub RecalcVirtualRowCount()

        If Me.MasterDataPID = 0 Or Me.x29ID = BO.x29IdEnum._NotSpecified Then Return
        Dim mq As New BO.myQueryP56
        InhaleTasksQuery(mq)

        Dim dt As DataTable = Me.Factory.p56TaskBL.GetGridFooterSums(mq, Me.hidSumCols.Value)
        gridP56.VirtualRowCount = dt.Rows(0).Item(0)
        Me.hidFooterString.Value = gridP56.CompleteFooterString(dt, Me.hidSumCols.Value)

        gridP56.radGridOrig.CurrentPageIndex = 0
        Me.lblHeaderP56.Text = BO.BAS.OM2(Me.lblHeaderP56.Text, BO.BAS.FNI(gridP56.VirtualRowCount))
    End Sub

    Private Sub SetupGridP56()
        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
       
        If cJ70.j70ColumnNames.IndexOf("ReceiversInLine") > 0 Then Me.hidReceiversInLine.Value = "1" Else Me.hidReceiversInLine.Value = ""
        If cJ70.j70ColumnNames.IndexOf("Hours_Orig") > 0 Or cJ70.j70ColumnNames.IndexOf("Expenses_Orig") > 0 Then Me.hidTasksWorksheetColumns.Value = "1" Else Me.hidTasksWorksheetColumns.Value = ""
        Me.hidDefaultSorting.Value = cJ70.j70OrderBy

        Dim cS As New SetupDataGrid(Me.Factory, gridP56, cJ70)
        With cS
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .AllowCustomPaging = True
            .AllowMultiSelect = Not _curIsExport
            .AllowMultiSelectCheckboxSelector = True
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM
        Me.hidSumCols.Value = cG.SumCols

        ''Dim strAddSqlFrom As String = "", strSqlSumCols As String = ""
        ''Me.hidCols.Value = basUIMT.SetupDataGrid(Me.Factory, Me.gridP56, cJ70, CInt(Me.cbxPaging.SelectedValue), True, Not _curIsExport, True, , , , strAddSqlFrom, , strSqlSumCols)
        ''Me.hidFrom.Value = strAddSqlFrom
        ''Me.hidSumCols.Value = strSqlSumCols
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With



    End Sub


    Private Sub gridP56_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gridP56.ItemDataBound
        basUIMT.p56_grid_Handle_ItemDataBound(sender, e, False, True, "", BO.BAS.GetDataPrefix(Me.x29ID))
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
    Private Sub InhaleTasksQuery(ByRef mq As BO.myQueryP56)
        Select Case Me.x29ID
            Case BO.x29IdEnum.p41Project
                mq.p41ID = MasterDataPID
            Case BO.x29IdEnum.p28Contact
                mq.p28ID = MasterDataPID
            Case BO.x29IdEnum.j02Person
                mq.j02ID = MasterDataPID
        End Select

        mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead

        Select Case Me.cbxP56Validity.SelectedValue
            Case "1"
                mq.Closed = BO.BooleanQueryMode.NoQuery
            Case "2"
                mq.Closed = BO.BooleanQueryMode.FalseQuery
            Case "3"
                mq.Closed = BO.BooleanQueryMode.TrueQuery
        End Select
        With mq
            .MG_GridGroupByField = Me.cbxGroupBy.SelectedValue
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            .MG_SortString = gridP56.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If

        End With
    End Sub
    Private Sub gridP56_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles gridP56.NeedDataSource
        If MasterDataPID = 0 Then Return

        Dim mq As New BO.myQueryP56
        InhaleTasksQuery(mq)
        mq.MG_PageSize = CInt(Me.cbxPaging.SelectedValue)
        mq.MG_CurrentPageIndex = gridP56.radGridOrig.MasterTableView.CurrentPageIndex

        ''If Me.hidReceiversInLine.Value = "1" Then bolReceiversInLine = True

        ''If Me.hidTasksWorksheetColumns.Value = "1" Then
        ''    Dim lis As IEnumerable(Of BO.p56TaskWithWorksheetSum) = Factory.p56TaskBL.GetList_WithWorksheetSum(mq, bolReceiversInLine)
        ''    intClosed = lis.Where(Function(p) p.IsClosed = True).Count
        ''    intOpened = lis.Where(Function(p) p.IsClosed = False).Count
        ''    Select Case Me.cbxP56Validity.SelectedValue
        ''        Case "1"
        ''        Case "2" : lis = lis.Where(Function(p) p.IsClosed = False)
        ''        Case "3" : lis = lis.Where(Function(p) p.IsClosed = True)
        ''    End Select
        ''    gridP56.DataSource = lis
        ''Else
        ''    Dim lis As IEnumerable(Of BO.p56Task) = Factory.p56TaskBL.GetList(mq, bolReceiversInLine)
        ''    intClosed = lis.Where(Function(p) p.IsClosed = True).Count
        ''    intOpened = lis.Where(Function(p) p.IsClosed = False).Count
        ''    Select Case Me.cbxP56Validity.SelectedValue
        ''        Case "1"
        ''        Case "2" : lis = lis.Where(Function(p) p.IsClosed = False)
        ''        Case "3" : lis = lis.Where(Function(p) p.IsClosed = True)
        ''    End Select
        ''    gridP56.DataSource = lis
        ''End If
        Dim dt As DataTable = Me.Factory.p56TaskBL.GetGridDataSource(mq)

        If dt Is Nothing Then
            Return
        Else
            gridP56.DataSourceDataTable = dt
        End If

        ''lblHeaderP56.Text = BO.BAS.OM2(lblHeaderP56.Text, dt.Rows.Count.ToString)

        If Me.DefaultSelectedPID <> 0 Then
            If dt.AsEnumerable.Where(Function(p) p.Item("pid") = Me.DefaultSelectedPID).Count > 0 Then
                'záznam je na první stránce
            Else
                Dim mqAll As New BO.myQueryP56
                mqAll.TopRecordsOnly = 0
                mqAll.MG_SelectPidFieldOnly = True
                InhaleTasksQuery(mqAll)
                Dim dtAll As DataTable = Me.Factory.p56TaskBL.GetGridDataSource(mqAll)
                Dim x As Integer, intNewPageIndex As Integer = 0
                For Each dbRow As DataRow In dtAll.Rows
                    x += 1
                    If x > gridP56.PageSize Then
                        intNewPageIndex += 1 : x = 1
                    End If
                    If dbRow.Item("pid") = Me.DefaultSelectedPID Then
                        InhaleTasksQuery(mq)
                        gridP56.radGridOrig.CurrentPageIndex = intNewPageIndex
                        mq.MG_CurrentPageIndex = intNewPageIndex
                        dt = Me.Factory.p56TaskBL.GetGridDataSource(mq) 'nový zdroj pro grid
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub cbxP56Validity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxP56Validity.SelectedIndexChanged
        Factory.j03UserBL.SetUserParam("p56_subgrid-cbxP56Validity", Me.cbxP56Validity.SelectedValue)
        RecalcVirtualRowCount()
        gridP56.Rebind(False)
    End Sub

    ''Private Sub gridP56_NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object) Handles gridP56.NeedFooterSource
    ''    footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"

    ''    gridP56.ParseFooterItemString(footerItem, ViewState("footersum"))
    ''End Sub

    ''Public Sub Rebind(bolKeepSelectedRecord As Boolean)
    ''    RecalcVirtualRowCount()
    ''    gridP56.Rebind(bolKeepSelectedRecord)
    ''End Sub

    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With gridP56.radGridOrig.MasterTableView
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
        Factory.j03UserBL.SetUserParam("p56_subgrid-groupby-" & BO.BAS.GetDataPrefix(Me.x29ID), Me.cbxGroupBy.SelectedValue)
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With
        RecalcVirtualRowCount()
        gridP56.Rebind(True)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Me.Factory.j03UserBL.SetUserParam("p56_subgrid-pagesize", Me.cbxPaging.SelectedValue)
        RecalcVirtualRowCount()
        SetupGridP56()
        gridP56.Rebind(True)
    End Sub

    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cXLS As New clsExportToXls(Me.Factory)

        Dim mq As New BO.myQueryP56
        InhaleTasksQuery(mq)
        mq.MG_GridGroupByField = ""

        Dim dt As DataTable = Me.Factory.p56TaskBL.GetGridDataSource(mq)

        Dim strFileName As String = cXLS.ExportDataGrid(dt.AsEnumerable, cJ70)
        If strFileName = "" Then
            Response.Write(cXLS.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    Private Sub gridP56_NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object) Handles gridP56.NeedFooterSource
        ''footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"
        If Me.hidFooterString.Value = "" And gridP56.radGridOrig.PageCount > 1 Then
            RecalcVirtualRowCount()
        End If

        gridP56.ParseFooterItemString(footerItem, Me.hidFooterString.Value)

        If Me.DefaultSelectedPID <> 0 Then
            gridP56.SelectRecords(Me.DefaultSelectedPID)
        End If
    End Sub


    Private Sub GridExport(strFormat As String)
        _curIsExport = True

        SetupGridP56()
        basUIMT.Handle_GridTelerikExport(Me.gridP56, strFormat)
        
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

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        basUIMT.RenderQueryCombo(Me.cbxP56Validity)
    End Sub
End Class