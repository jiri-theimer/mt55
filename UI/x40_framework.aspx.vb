Imports Telerik.Web.UI

Public Class x40_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False
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
    Private Sub x40_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "x40_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
            Me.CurrentMasterPrefix = Request.Item("masterprefix")
            With Master
                .PageTitle = "Odeslané poštovní zprávy"
                .SiteMenuValue = "x40_framework"

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("x40_framework-pagesize")

                    .Add("x40_framework-filter_setting")
                    .Add("x40_framework-filter_sql")
                    .Add("x40_framework-period")
                    .Add("x40_framework-status")
                    .Add("x40_framework-entity")
                    .Add("periodcombo-custom_query")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("x40_framework-pagesize", "20")
                basUI.SelectDropdownlistValue(Me.cbxQueryStatus, .GetUserParam("x40_framework-status"))
                basUI.SelectDropdownlistValue(Me.cbxQueryEntity, .GetUserParam("x40_framework-entity"))
            End With


            With Master.Factory.j03UserBL
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("x40_framework-period")
                SetupGrid(.GetUserParam("x40_framework-filter_setting"), .GetUserParam("x40_framework-filter_sql"))
            End With
            If Me.CurrentMasterPrefix <> "" Then
                With Me.linkEntity
                    .Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix), Me.CurrentMasterPID)
                    .NavigateUrl = Me.CurrentMasterPrefix & "_framework.aspx?pid=" & Me.CurrentMasterPID.ToString
                    .Visible = True
                End With
            End If
        End If
    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        With grid1
            .ClearColumns()
            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
            .radGridOrig.ShowFooter = False
            .AllowMultiSelect = True
            .AddCheckboxSelector()

            .AddSystemColumn(16, "UserInsert")
            .AddContextMenuColumn(16)
            .AddColumn("DateUpdate", "Čas", BO.cfENUM.DateTime, , , , , , False)
            .AddColumn("Context", "Kontext", , , , , , , False)
            .AddColumn("x40State", "Stav", , , , , , , False)
            .AddColumn("x40SenderAddress", "Od (adresa)")
            .AddColumn("x40SenderName", "Od (jméno)")
            '.AddColumn("x40SenderAddress", "Adresa")
            .AddColumn("x40Recipient", "Příjemce")
            .AddColumn("x40Subject", "Předmět zprávy")
            .AddColumn("x40WhenProceeded", "Zpracováno", BO.cfENUM.DateTime, , , , , , False)
            .AddColumn("x40ErrorMessage", "Chyba")

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With
    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.x40_grid_Handle_ItemDataBound(sender, e)
    End Sub

   

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("x40_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("x40_framework-filter_sql", grid1.GetFilterExpression())
            End With
        End If
        Dim mq As New BO.myQueryX40
        mq.TopRecordsOnly = 1000
        mq.ColumnFilteringExpression = grid1.GetFilterExpression
        If period1.SelectedValue <> "" Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        If Me.CurrentMasterPrefix <> "" Then
            mq.x29ID_RecordPID = BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix)
            mq.RecordPID = Me.CurrentMasterPID
        Else
            If Not Master.Factory.SysUser.IsAdmin Then
                mq.j03ID_MyRecords = Master.Factory.SysUser.PID
            End If
        End If
        If Me.cbxQueryStatus.SelectedIndex > 0 Then
            mq.x40State = CType(BO.BAS.IsNullInt(Me.cbxQueryStatus.SelectedValue), BO.x40StateENUM)
        End If
        If Me.cbxQueryEntity.SelectedIndex > 0 Then
            mq.x29ID_ExplicitQuery = CType(BO.BAS.IsNullInt(Me.cbxQueryEntity.SelectedValue), BO.x29IdEnum)
        End If
        

        Dim lis As IEnumerable(Of BO.x40MailQueue) = Master.Factory.x40MailQueueBL.GetList(mq)
      
        grid1.DataSource = lis
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("x40_framework-pagesize", cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    Private Sub x40_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        basUIMT.RenderQueryCombo(Me.cbxQueryStatus)
        basUIMT.RenderQueryCombo(cbxQueryEntity)
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("x40_framework-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        If Me.CurrentMasterPrefix = "" Then
            Response.Redirect("x40_framework.aspx")
        Else
            Response.Redirect("x40_framework.aspx?masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString)
        End If

    End Sub

    Private Sub cbxQueryStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryStatus.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("x40_framework-status", Me.cbxQueryStatus.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cbxQueryEntity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryEntity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("x40_framework-entity", Me.cbxQueryEntity.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        ReloadPage()
    End Sub

    Private Sub cmdBatch_Click(sender As Object, e As EventArgs) Handles cmdBatch.Click
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidHardRefreshPID.Value)
        If pids.Count = 0 Then
            Master.Notify("Na vstupu není ani jeden vybraný záznam.", NotifyLevel.ErrorMessage)
            Return
        End If
        Dim status As BO.x40StateENUM = CType(Me.hidHardRefreshFlag.Value, BO.x40StateENUM)
        If status = BO.x40StateENUM._NotSpecified Then Master.Notify("status missing.") : Return
        With Master.Factory.x40MailQueueBL
            For Each intX40ID As Integer In pids
                .UpdateMessageState(intX40ID, status)
            Next
        End With
        grid1.Rebind(True, pids(0))
        ''ReloadPage()
    End Sub
End Class