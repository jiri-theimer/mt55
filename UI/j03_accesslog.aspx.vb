Public Class j03_accesslog
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub j03_accesslog_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .DataPID = .Factory.SysUser.PID
                With .Factory.j03UserBL.Load(.DataPID)
                    Me.lblUser.Text = .Person & " [" & .j03Login & "]"
                End With

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("entity_timeline-period")
                    .Add("periodcombo-custom_query")
                    .Add("entity_timeline-pagesize")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("entity_timeline-period")

                basUI.SelectDropdownlistValue(Me.cbxPaging, .Factory.j03UserBL.GetUserParam("entity_timeline-pagesize", "20"))
            End With
            SetupGrid()

        End If
    End Sub

    Public Sub SetupGrid()
        With grid1
            .ClearColumns()
            .AllowMultiSelect = False
            .radGridOrig.ShowFooter = False
            .PageSize = CInt(Me.cbxPaging.SelectedValue)
            .AddColumn("j90Date", "Čas", BO.cfENUM.DateTime)
            .AddColumn("j90RequestURL", "URL")
            .AddColumn("j90ClientBrowser", "Prohlížeč")
            .AddColumn("j90IsMobileDevice", "Mobil", BO.cfENUM.Checkbox)
            .AddColumn("ScreenResolution", "Display")
        End With
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        grid1.DataSource = Master.Factory.j03UserBL.GetList_j90(Master.DataPID, period1.DateFrom, period1.DateUntil)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_timeline-pagesize", Me.cbxPaging.SelectedValue)
        SetupGrid()
        grid1.Rebind(False)
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("entity_timeline-period", Me.period1.SelectedValue)
        grid1.Rebind(False)
    End Sub
End Class