Public Class p31_move2project
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_move2project_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_move2project"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p31_grid-period")
                .Add("periodcombo-custom_query")
            End With
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                hidP31IDs.Value = Request.Item("p31ids")

                If .DataPID = 0 And hidP31IDs.Value = "" Then .StopPage("pid or p31ids missing")
                If .DataPID <> 0 Then
                    .HeaderText = "Přesunout worksheet na jiný projekt | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix("p41"), .DataPID)
                End If


                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("p31_grid-period")
                .AddToolbarButton("Uložit změny pro zaškrtlé úkony", "ok", , "Images/save.png")
            End With

            SetupGrid()
            RecalcVirtualRowCount()

            
        End If
    End Sub
    Private Sub SetupGrid()
        With Me.grid1
            .ClearColumns()
            .DataKeyNames = "pid"
            .radGridOrig.ShowFooter = False
            .PageSize = 250
            .AllowMultiSelect = True
            .AddCheckboxSelector()
            .AddSystemColumn(5)
            .AddColumn("p31Date", "Datum", BO.cfENUM.DateOnly)
            .AddColumn("Person", "Jméno")
            If hidP31IDs.Value = "" Then
                .AddColumn("p34Name", "Sešit")
            Else
                .AddColumn("ClientName", "Klient")
                .AddColumn("p41Name", "Projekt")
            End If

            .AddColumn("p32Name", "Aktivita")
            .AddColumn("p31Hours_Orig", "Hodiny", BO.cfENUM.Numeric2)
            .AddColumn("p31Rate_Billing_Orig", "Sazba", BO.cfENUM.Numeric2)
            .AddColumn("p31Amount_WithoutVat_Orig", "Částka", BO.cfENUM.Numeric2)
            .AddColumn("j27Code_Billing_Orig", "")
            .AddColumn("p31Text", "Text")
        End With
    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)
        If Master.DataPID <> 0 Then
            mq.p41ID = Master.DataPID
        End If
        If hidP31IDs.Value <> "" Then
            mq.PIDs = BO.BAS.ConvertPIDs2List(hidP31IDs.Value, ",")
        End If

        mq.QuickQuery = BO.myQueryP31_QuickQuery.EditingOrMovedToBin
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead

        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil

    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p31_move2project")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If Master.DataPID = 0 And hidP31IDs.Value = "" Then Return
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)
        With mq
            .MG_PageSize = 250
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()

        End With

        grid1.DataSource = Master.Factory.p31WorksheetBL.GetList(mq)
    End Sub


    Public Sub RecalcVirtualRowCount()
        If Master.DataPID = 0 And hidP31IDs.Value = "" Then Return
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)

        Dim cSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, False, False)
        If Not cSum Is Nothing Then
            grid1.VirtualRowCount = cSum.RowsCount

        Else

            grid1.VirtualRowCount = 0
        End If
        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub
    Private Sub p31_move2bin_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
        If Not Page.IsPostBack And hidP31IDs.Value <> "" Then
            For Each ri As Telerik.Web.UI.GridDataItem In grid1.radGridOrig.MasterTableView.Items
                ri.Selected = True
            Next
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim intDestP41ID As Integer = BO.BAS.IsNullInt(Me.p41id.Value)
            If intDestP41ID = 0 Then
                Master.Notify("Musíte vybrat cílový projekt.", NotifyLevel.WarningMessage)
                Return
            End If
            If intDestP41ID = Master.DataPID Then
                Master.Notify("Cílový projet se musí lišit od aktuálního.", NotifyLevel.WarningMessage)
                Return
            End If


            Dim pids As List(Of Integer) = grid1.GetSelectedPIDs()
           
            With Master.Factory.p31WorksheetBL
                If .Move2Project(intDestP41ID, pids) Then
                    Master.CloseAndRefreshParent("p31-move")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With

        End If
    End Sub
End Class