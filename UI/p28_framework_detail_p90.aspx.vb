Imports Telerik.Web.UI
Public Class p28_framework_detail_p90
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Private Property _needFilterIsChanged As Boolean = False

    Private Sub p28_framework_detail_p90_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        menu1.DataPrefix = "p28"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                .SiteMenuValue = "p28"
                If .DataPID = 0 Then .StopPage("masterpid is missing")
                If Not .Factory.TestPermission(BO.x53PermValEnum.PF_P90_Reader) Then .StopPage("Nemáte oprávnění číst zálohové faktury.")

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p28_menu-tabskin")
                    ''.Add("p28_menu-menuskin")
                    .Add("p28_menu-x31id-plugin")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    menu1.TabSkin = .GetUserParam("p28_menu-tabskin")
                    ''menu1.MenuSkin = .GetUserParam("p28_menu-menuskin")
                    menu1.x31ID_Plugin = .GetUserParam("p28_menu-x31id-plugin")
                End With
            End With
            SetupGrid()

            

        End If
        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
        Dim cRecSum As BO.p28ContactSum = Master.Factory.p28ContactBL.LoadSumRow(cRec.PID)
        menu1.p28_RefreshRecord(cRec, cRecSum, "p90")

        menu1.DataPID = Master.DataPID
    End Sub
    Private Sub SetupGrid()
        With grid1
            .ClearColumns()
            .PageSize = 20
            .radGridOrig.ShowFooter = False
            .AddSystemColumn(16)
            .AddContextMenuColumn(16)
            .AddColumn("p90Code", "Číslo")
            .AddColumn("p82Code", "DPP", , , , , "Číslo dokladu o přijaté platbě")
            .AddColumn("p90Date", "Datum", BO.cfENUM.DateOnly)

            .AddColumn("p90Amount", "Částka", BO.cfENUM.Numeric2)

            .AddColumn("p90Amount_Debt", "Dluh", BO.cfENUM.Numeric2)
            .AddColumn("p90Text1", "Text")
            .AddColumn("j27Code", "")

        End With

    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.p90Proforma = CType(e.Item.DataItem, BO.p90Proforma)
        If cRec.IsClosed Then dataItem.Font.Strikeout = True
        If cRec.p90IsDraft Then dataItem("systemcolumn").CssClass = "draft"

        With dataItem("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p90','" & cRec.PID.ToString & "',this)" & Chr(34) & "></a>"
        End With
    End Sub

    

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            
        End If
        Dim mq As New BO.myQueryP90
        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.ColumnFilteringExpression = grid1.GetFilterExpression
        mq.p28ID = Master.DataPID
       
        Dim lis As IEnumerable(Of BO.p90Proforma) = Master.Factory.p90ProformaBL.GetList(mq)

        grid1.DataSource = lis
    End Sub
End Class