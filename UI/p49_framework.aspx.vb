Imports Telerik.Web.UI

Public Class p49_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False

    Private Sub p49_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Rozpočty výdajů a fixních odměn"
                .SiteMenuValue = "p49"

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p49_framework-pagesize")
                    .Add("p49_framework-filter_setting")
                    .Add("p49_framework-filter_sql")
                    .Add("p49_framework-period")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

               
            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("p49_framework-pagesize", "20")
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"), True)
                period1.SelectedValue = .GetUserParam("p49_framework-period")

            End With

            With Master.Factory.j03UserBL
                SetupGrid(.GetUserParam("p49_framework-filter_setting"), .GetUserParam("p49_framework-filter_sql"))
            End With

        End If
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        Dim group As New Telerik.Web.UI.GridColumnGroup

        With grid1
            .ClearColumns()

            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)

            .radGridOrig.MasterTableView.ColumnGroups.Add(group)
            group.Name = "plan" : group.HeaderText = "Rozpočet"
            group = New Telerik.Web.UI.GridColumnGroup
            .radGridOrig.MasterTableView.ColumnGroups.Add(group)
            group.Name = "real" : group.HeaderText = "Vykázaná realita"

            .radGridOrig.ShowFooter = True
            .AddSystemColumn(20)
            .AddColumn("Period", "Měsíc", , , , , , , , "plan")
            .AddColumn("Project", "Projekt", , , , , , , , "plan")
            .AddColumn("p32Name", "Aktivita", , , , , , , , "plan")
            .AddColumn("SupplierName", "Dodavatel", , , , , , , , "plan")
            .AddColumn("p49Text", "Text", , , , , , , , "plan")
            .AddColumn("p49Amount", "Částka", BO.cfENUM.Numeric2, , , , , True, , "plan")
            .AddColumn("j27Code", "Měna", , , , , , , , "plan")

            .AddColumn("p31Date", "Datum", BO.cfENUM.DateOnly, , , , , , , "real")
            .AddColumn("p31Amount_WithoutVat_Orig", "Částka", BO.cfENUM.Numeric, , , , , True, , "real")
            ''.AddColumn("p31Code", "Kód dokladu", , , , , , , , "real")
            .AddColumn("p31Count", "Počet", BO.cfENUM.Numeric0, , , , , , , "real")

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With

    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.p49FinancialPlan = CType(e.Item.DataItem, BO.p49FinancialPlanExtended)
        If cRec.IsClosed Then dataItem.Font.Strikeout = True
        
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("p49_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("p49_framework-filter_sql", grid1.GetFilterExpression())
            End With
        End If
        Dim mq As New BO.myQueryP49
        mq.ColumnFilteringExpression = grid1.GetFilterExpression
        If period1.SelectedValue <> "" Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        
        Dim lis As IEnumerable(Of BO.p49FinancialPlanExtended) = Master.Factory.p49FinancialPlanBL.GetList_Extended(mq)

        grid1.DataSource = lis
    End Sub


    Private Sub ReloadPage()
        Response.Redirect("p49_framework.aspx")
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p49_framework-pagesize", cbxPaging.SelectedValue)

        ReloadPage()
    End Sub


    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p49_framework-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub p49_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub
End Class