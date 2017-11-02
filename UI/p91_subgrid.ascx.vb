Imports Telerik.Web.UI
Public Class p91_subgrid
    Inherits System.Web.UI.UserControl
    Public Property MasterDataPID As Integer
    Public Property Factory As BL.Factory
    Public Property x29ID As BO.x29IdEnum

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        designer1.Factory = Me.Factory
        If Not Page.IsPostBack Then

            designer1.MasterPrefix = BO.BAS.GetDataPrefix(Me.x29ID)
            designer1.x36Key = "p91_subgrid-j70id-" & designer1.MasterPrefix
            Dim lisPars As New List(Of String)
            With lisPars
                .Add(designer1.x36Key)
                .Add("p91_framework-periodtype")
                .Add("p91_framework-period")
                .Add("periodcombo-custom_query")
                .Add("p91_subgrid-pagesize")
            End With
            With Factory.j03UserBL
                .InhaleUserParams(lisPars)
                Dim strJ70ID As String = Request.Item("j70id")
                If strJ70ID = "" Then strJ70ID = .GetUserParam(designer1.x36Key, "0")
                designer1.RefreshData(CInt(strJ70ID))

                

                basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("p91_subgrid-pagesize", "10"))
                basUI.SelectDropdownlistValue(Me.cbxPeriodType, .GetUserParam("p91_framework-periodtype", "p91DateSupply"))
                period1.SetupData(Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("p91_framework-period")
            End With

            SetupGridP91()
            cmdExport.Visible = Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
        End If

        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With


    End Sub


    Private Sub gridP91_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles gridP91.ItemDataBound
        basUIMT.p91_grid_Handle_ItemDataBound(sender, e, True)
    End Sub

    Private Sub SetupGridP91()
        

        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Me.hidDefaultSorting.Value = cJ70.j70OrderBy

        Dim cS As New SetupDataGrid(Me.Factory, gridP91, cJ70)
        With cS
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            .AllowCustomPaging = False
            .AllowMultiSelect = False
            .AllowMultiSelectCheckboxSelector = False
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        hidCols.Value = cG.Cols
        Me.hidFrom.Value = cG.AdditionalFROM
        
        ''Me.hidDefaultSorting.Value = cJ70.j70OrderBy
        ''Dim strAddSqlFrom As String = ""
        'Me.hidCols.Value = basUIMT.SetupDataGrid(Me.Factory, Me.gridP91, cJ70, CInt(Me.cbxPaging.SelectedValue), False, False, False, , , , strAddSqlFrom)
        ''Me.hidFrom.Value = strAddSqlFrom
       
    End Sub

    Private Sub SetupGroupByCurrency()
        With gridP91.radGridOrig.MasterTableView
            .ShowGroupFooter = True
            Dim GGE As New Telerik.Web.UI.GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = "j27Code_Grid"
            fld.HeaderText = "Měna"


            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)

        End With
    End Sub

    Private Sub gridP91_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles gridP91.NeedDataSource
        If MasterDataPID = 0 Then Return

        Dim mq As New BO.myQueryP91
        InhaleMyQueryP91(mq)

        Dim dt As DataTable = Factory.p91InvoiceBL.GetGridDataSource(mq)

        ''Dim bolGroupByCurrency As Boolean = False
        ''If gridP91.radGridOrig.MasterTableView.GroupByExpressions.Count = 0 And dt.Rows.Count > 0 Then
        ''    If lis.Select(Function(p) p.j27ID).Distinct.Count > 1 Then
        ''        SetupGroupByCurrency()
        ''        bolGroupByCurrency = True
        ''    End If
        ''End If
        SetupGroupByCurrency()

        gridP91.DataSourceDataTable = dt

        ''If Not bolGroupByCurrency Then
        ''    gridP91.radGridOrig.ShowFooter = True : ViewState("p91_footersum") = ""
        ''    For Each col In gridP91.radGridOrig.MasterTableView.Columns
        ''        If TypeOf col Is GridBoundColumn Then
        ''            If col.Aggregate = GridAggregateFunction.Sum Then
        ''                ''ViewState("p91_footersum") += "|" & col.DataField & ";" & BO.BAS.FN(lis.Sum(Function(p) p.p91Amount_TotalDue))
        ''                If ViewState("p91_footersum") <> "" Then
        ''                    ViewState("p91_footersum") += "|" & col.DataField & ";" & BO.BAS.FN(lis.Sum(Function(p) BO.BAS.GetPropertyValue(p, col.DataField)))
        ''                Else
        ''                    ViewState("p91_footersum") = col.DataField & ";" & BO.BAS.FN(lis.Sum(Function(p) BO.BAS.GetPropertyValue(p, col.DataField)))
        ''                End If

        ''            End If

        ''        End If
        ''    Next
        ''Else
        ''    gridP91.radGridOrig.ShowFooter = False
        ''End If

    End Sub

    Private Sub InhaleMyQueryP91(ByRef mq As BO.myQueryP91)
        With mq
            Select Case Me.x29ID
                Case BO.x29IdEnum.p41Project
                    .p41ID = MasterDataPID
                Case BO.x29IdEnum.p28Contact
                    .p28ID = MasterDataPID
                Case BO.x29IdEnum.j02Person
                    .j02ID = MasterDataPID
                Case BO.x29IdEnum.p56Task
                    .p56ID = MasterDataPID
            End Select

            .Closed = BO.BooleanQueryMode.NoQuery
            .SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead

            Select Case Me.cbxPeriodType.SelectedValue
                Case "p91DateSupply" : .PeriodType = BO.myQueryP91_PeriodType.p91DateSupply
                Case "p91DateMaturity" : .PeriodType = BO.myQueryP91_PeriodType.p91DateMaturity
                Case "p91Date" : .PeriodType = BO.myQueryP91_PeriodType.p91Date
            End Select
            .DateFrom = period1.DateFrom
            .DateUntil = period1.DateUntil
            .MG_GridSqlColumns = Me.hidCols.Value
            .MG_AdditionalSqlFROM = Me.hidFrom.Value
            '.QuickQuery = Me.CurrentQuickQuery


        End With

    End Sub

    Private Sub gridP91_NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object) Handles gridP91.NeedFooterSource
        ''footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"

        gridP91.ParseFooterItemString(footerItem, ViewState("p91_footersum"))
    End Sub
    Private Sub cbxPeriodType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPeriodType.SelectedIndexChanged
        Factory.j03UserBL.SetUserParam("p91_framework-periodtype", Me.cbxPeriodType.SelectedValue)
        gridP91.Rebind(False)
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Factory.j03UserBL.SetUserParam("p91_framework-period", Me.period1.SelectedValue)
        gridP91.Rebind(False)
    End Sub

    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        Dim cJ70 As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(designer1.CurrentJ70ID)
        Dim cXLS As New clsExportToXls(Me.Factory)

        Dim mq As New BO.myQueryP91
        InhaleMyQueryP91(mq)

        Dim lis As IEnumerable(Of BO.p91Invoice) = Me.Factory.p91InvoiceBL.GetList(mq)

        Dim strFileName As String = cXLS.ExportDataGrid(lis, cJ70)
        If strFileName = "" Then
            Response.Write(cXLS.ErrorMessage)
        Else
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Me.Factory.j03UserBL.SetUserParam("p91_subgrid-pagesize", Me.cbxPaging.SelectedValue)
        SetupGridP91()
        gridP91.Rebind(True)
    End Sub
End Class