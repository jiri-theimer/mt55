Imports Telerik.Web.UI

Public Class o43_framework
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

    Private Sub o43_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "o43_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
            Me.CurrentMasterPrefix = Request.Item("masterprefix")
            With Master
                .PageTitle = "Přijaté poštovní zprávy"
                .SiteMenuValue = "o43_framework"

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("o43_framework-pagesize")

                    .Add("o43_framework-filter_setting")
                    .Add("o43_framework-filter_sql")
                    .Add("o43_framework-period")
                    .Add("o43_framework-entity")
                    .Add("periodcombo-custom_query")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam("o43_framework-pagesize", "20")
                basUI.SelectDropdownlistValue(Me.cbxQueryEntity, .GetUserParam("o43_framework-entity"))
            End With


            With Master.Factory.j03UserBL
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("o43_framework-period")
                SetupGrid(.GetUserParam("o43_framework-filter_setting"), .GetUserParam("o43_framework-filter_sql"))
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

            .AddSystemColumn(70, "UserInsert")
            .AddColumn("o41Name", "IMAP účet")
            .AddColumn("o43DateMessage", "Kdy", BO.cfENUM.DateTime, , , , , , False)

            .AddColumn("EntityName", "", , , , , , , False)

           

            .AddColumn("o43From", "Od")
            .AddColumn("o43TO", "Komu")
           

            .AddColumn("o43Subject", "Předmět zprávy")

            '.AddColumn("o43Attachments", "Přílohy")

            .AddColumn("o43DateImport", "Zpracováno", BO.cfENUM.DateTime, , , , , , False)

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With
    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.o43_grid_Handle_ItemDataBound(sender, e)
    End Sub



    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("o43_framework-filter_setting", grid1.GetFilterSetting())
                .SetUserParam("o43_framework-filter_sql", grid1.GetFilterExpression())
            End With
        End If
        Dim mq As New BO.myQuery
        mq.TopRecordsOnly = 1000
        mq.ColumnFilteringExpression = grid1.GetFilterExpression
        If period1.SelectedValue <> "" Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        If Me.CurrentMasterPrefix <> "" Then
            'mq.x29ID_RecordPID = BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix)
            'mq.RecordPID = Me.CurrentMasterPID

        
        End If
       
        If Me.cbxQueryEntity.SelectedIndex > 0 Then
            'mq.x29ID_ExplicitQuery = CType(BO.BAS.IsNullInt(Me.cbxQueryEntity.SelectedValue), BO.x29IdEnum)
        End If


        Dim lis As IEnumerable(Of BO.o43ImapRobotHistory) = Master.Factory.o42ImapRuleBL.GetList_o43(mq)

        grid1.DataSource = lis
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o43_framework-pagesize", cbxPaging.SelectedValue)

        ReloadPage()
    End Sub

    Private Sub o43_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
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
        Master.Factory.j03UserBL.SetUserParam("o43_framework-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        If Me.CurrentMasterPrefix = "" Then
            Response.Redirect("o43_framework.aspx")
        Else
            Response.Redirect("o43_framework.aspx?masterprefix=" & Me.CurrentMasterPrefix & "&masterpid=" & Me.CurrentMasterPID.ToString)
        End If

    End Sub

   

    Private Sub cbxQueryEntity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryEntity.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o43_framework-entity", Me.cbxQueryEntity.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        ReloadPage()
    End Sub
End Class