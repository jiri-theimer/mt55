Imports Telerik.Web.UI
Public Class x18_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False

    Private Sub x18_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Správa typů dokumentů"
                .SiteMenuValue = "x18_framework"
                If Not Request.UrlReferrer Is Nothing Then
                    If Request.UrlReferrer.AbsolutePath.IndexOf("admin") > 0 Or Request.Item("source") = "admin" Then
                        hidSource.Value = "admin"
                    End If
                End If
                .neededPermission = BO.x53PermValEnum.GR_X18_Admin

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("x18_framework-pagesize")
                    .Add("x18_framework-query-closed")
                    .Add("x18_framework-filter_setting")
                    .Add("x18_framework-filter_sql")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

                
            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("x18_framework-pagesize", "20")
                basUI.SelectDropdownlistValue(Me.cbxValidity, .GetUserParam("x18_framework-query-closed", "1"))
            End With

            With Master.Factory.j03UserBL
                SetupGrid(.GetUserParam("x18_framework-filter_setting"), .GetUserParam("x18_framework-filter_sql"))
            End With

        End If
    End Sub
    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        With grid1
            .ClearColumns()
            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
            .radGridOrig.ShowFooter = False
            .AddSystemColumn(20)
            .AddColumn("x18Name", "Název")
         
            .AddColumn("TagOrDoc", "Dokument/Kategorie", BO.cfENUM.AnyString)
            .AddColumn("Is_p41", "Projekty", BO.cfENUM.Checkbox)
            .AddColumn("Is_p28", "Klienti", BO.cfENUM.Checkbox)
            .AddColumn("Is_p56", "Úkoly", BO.cfENUM.Checkbox)
            .AddColumn("Is_p31", "Worksheet", BO.cfENUM.Checkbox)
            .AddColumn("Is_o23", "Dokumenty", BO.cfENUM.Checkbox)
            .AddColumn("Is_j02", "Osoby", BO.cfENUM.Checkbox)
            .AddColumn("Is_o22", "Kal.události", BO.cfENUM.Checkbox)
            .AddColumn("x18Ordinary", "#", BO.cfENUM.Numeric0, , , , , , False)
            '.AddColumn("DateInsert", "Založeno", BO.cfENUM.DateTime, , , , , , False)

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With

    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.x18EntityCategory = CType(e.Item.DataItem, BO.x18EntityCategory)
        If cRec.IsClosed Then dataItem.Font.Strikeout = True
        ''If cRec.x18IsMultiSelect Then
        ''    dataItem.Item("systemcolumn").BackColor = System.Drawing.Color.Green
        ''End If
    End Sub
    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("x18_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("x18_framework-filter_sql", grid1.GetFilterExpression())
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
        
        Dim lis As IEnumerable(Of BO.x18EntityCategory) = Master.Factory.x18EntityCategoryBL.GetList(mq, , -1, True)

        grid1.DataSource = lis
    End Sub


    Private Sub ReloadPage()
        Response.Redirect("x18_framework.aspx")
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("x18_framework-pagesize", cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    Private Sub cbxValidity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxValidity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("x18_framework-query-closed", Me.cbxValidity.SelectedValue)
        grid1.Rebind(False)
    End Sub


   

  
End Class