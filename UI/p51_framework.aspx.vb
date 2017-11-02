Imports Telerik.Web.UI
Public Class p51_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False

    Private Sub p51_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p51_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Ceníky sazeb"
                .SiteMenuValue = "p51_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_P51_Admin)

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p51_framework-pagesize")
                    .Add("p51_framework-query-closed")
                    .Add("p51_framework-chkShowCustomTailor")
                    .Add("p51_framework-chkMasterPriceLists")
                    .Add("p51_framework-filter_setting")
                    .Add("p51_framework-filter_sql")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)



            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("p51_framework-pagesize", "20")
                Me.chkShowCustomTailor.Checked = BO.BAS.BG(.GetUserParam("p51_framework-chkShowCustomTailor", "0"))
                basUI.SelectDropdownlistValue(Me.cbxValidity, .GetUserParam("p51_framework-query-closed", "1"))
                Me.chkMasterPriceLists.Checked = BO.BAS.BG(.GetUserParam("p51_framework-chkMasterPriceLists", "0"))
            End With

           
            With Master.Factory.j03UserBL
                SetupGrid(.GetUserParam("p51_framework-filter_setting"), .GetUserParam("p51_framework-filter_sql"))
            End With
        End If
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        With grid1
            .ClearColumns()
            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
            .radGridOrig.ShowFooter = False

            .AddContextMenuColumn(16)

            .AddColumn("p51name", "Název ceníku")
            .AddColumn("j27Code", "Měna sazeb")
            .AddColumn("p51IsCustomTailor", "Sazby na míru", BO.cfENUM.Checkbox)
            .AddColumn("p51IsInternalPriceList", "Nákl.ceník", BO.cfENUM.Checkbox)
            .AddColumn("p51Ordinary", "#", BO.cfENUM.Numeric0, , , , , , False)

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With
    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.p51PriceList = CType(e.Item.DataItem, BO.p51PriceList)
        If cRec.IsClosed Then dataItem.Font.Strikeout = True

        With dataItem.Item("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p51','" & cRec.PID.ToString & "',this)" & Chr(34) & "></a>"
        End With
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("p51_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("p51_framework-filter_sql", grid1.GetFilterExpression())
            End With
        End If
        Dim mq As New BO.myQuery
        Select Case Me.cbxValidity.SelectedValue
            Case "1"
                mq.Closed = BO.BooleanQueryMode.NoQuery
            Case "2"
                mq.Closed = BO.BooleanQueryMode.FalseQuery
            Case "3"
                mq.Closed = BO.BooleanQueryMode.TrueQuery
        End Select
        mq.ColumnFilteringExpression = grid1.GetFilterExpression

        Dim lis As IEnumerable(Of BO.p51PriceList) = Master.Factory.p51PriceListBL.GetList(mq)
        If Not Me.chkShowCustomTailor.Checked Then
            lis = lis.Where(Function(p) p.p51IsCustomTailor = False)
        End If
        If Me.chkMasterPriceLists.Checked Then
            lis = lis.Where(Function(p) p.p51IsMasterPriceList = True)
        End If
        grid1.DataSource = lis
    End Sub



    Private Sub chkShowCustomTailor_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowCustomTailor.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p51_framework-chkShowCustomTailor", BO.BAS.GB(Me.chkShowCustomTailor.Checked))
        grid1.Rebind(False)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p51_framework-pagesize", cbxPaging.SelectedValue)

        grid1.PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
        If grid1.radGridOrig.CurrentPageIndex > 0 Then grid1.radGridOrig.CurrentPageIndex = 0
        grid1.Rebind(True)
    End Sub

    Private Sub cbxValidity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxValidity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p51_framework-query-closed", Me.cbxValidity.SelectedValue)
        grid1.Rebind(False)
    End Sub

    Private Sub chkMasterPriceLists_CheckedChanged(sender As Object, e As EventArgs) Handles chkMasterPriceLists.CheckedChanged

        Master.Factory.j03UserBL.SetUserParam("p51_framework-chkMasterPriceLists", BO.BAS.GB(Me.chkMasterPriceLists.Checked))
        grid1.Rebind(False)
    End Sub


    
End Class