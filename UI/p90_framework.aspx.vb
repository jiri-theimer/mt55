Imports Telerik.Web.UI
Public Class p90_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False

    Private Sub p90_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Zálohové faktury"
                .SiteMenuValue = "p90_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_P90_Reader)

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p90_framework-pagesize")
                    .Add("p90_framework-query-closed")
                    .Add("p90_framework-filter_setting")
                    .Add("p90_framework-filter_sql")
                    .Add("p90_framework-period")
                    .Add("periodcombo-custom_query")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

                period1.SetupData(Master.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("p90_framework-period")

            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("p90_framework-pagesize", "20")
                basUI.SelectDropdownlistValue(Me.cbxValidity, .GetUserParam("p90_framework-query-closed", "1"))
            End With

            With Master.Factory.j03UserBL
                SetupGrid(.GetUserParam("p90_framework-filter_setting"), .GetUserParam("p90_framework-filter_sql"))
            End With

        End If
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        With grid1
            .ClearColumns()
            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
            .radGridOrig.ShowFooter = False
            .AddSystemColumn(16)
            .AddContextMenuColumn(16)

            .AddColumn("p90Code", "Číslo")
            .AddColumn("p82Code", "DPP", , , , , "Číslo dokladu o přijaté platbě")
            .AddColumn("p90Date", "Datum", BO.cfENUM.DateOnly)
            .AddColumn("p28Name", "Klient")

            .AddColumn("p90Amount", "Částka", BO.cfENUM.Numeric2)

            .AddColumn("p90Amount_Debt", "Dluh", BO.cfENUM.Numeric2)
            .AddColumn("p90Text1", "Text")
            .AddColumn("j27Code", "")
            .AddColumn("TagsInlineHtml", "", , False, , , , , False)
            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With

    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.p90Proforma = CType(e.Item.DataItem, BO.p90Proforma)
        If cRec.IsClosed Then dataItem.Font.Strikeout = True
        If cRec.p90Amount_Debt > 0 Then
            dataItem.Item("systemcolumn").BackColor = basUI.ColorQueryRGB
        End If
        With dataItem.Item("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p90','" & cRec.PID.ToString & "',this)" & Chr(34) & "></a>"
        End With
       
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("p90_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("p90_framework-filter_sql", grid1.GetFilterExpression())
            End With
        End If
        Dim mq As New BO.myQueryP90
        mq.IsShowTagsInColumn = True
        Select Case Me.cbxValidity.SelectedValue
            Case "1"
                mq.Closed = BO.BooleanQueryMode.NoQuery
            Case "2"
                mq.Closed = BO.BooleanQueryMode.FalseQuery
            Case "3"
                mq.Closed = BO.BooleanQueryMode.TrueQuery
        End Select
        mq.ColumnFilteringExpression = grid1.GetFilterExpression
        If period1.SelectedValue <> "" Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        Dim lis As IEnumerable(Of BO.p90Proforma) = Master.Factory.p90ProformaBL.GetList(mq)

        grid1.DataSource = lis
    End Sub


    Private Sub ReloadPage()
        Response.Redirect("p90_framework.aspx")
    End Sub
    
    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p90_framework-pagesize", cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    Private Sub cbxValidity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxValidity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p90_framework-query-closed", Me.cbxValidity.SelectedValue)
        grid1.Rebind(False)
    End Sub

   
    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p90_framework-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub
    
    Private Sub p90_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub
End Class