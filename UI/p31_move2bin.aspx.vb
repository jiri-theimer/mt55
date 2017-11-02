Public Class p31_move2bin
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Public Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
        Set(value As String)
            Me.hidPrefix.Value = value
        End Set
    End Property
    Private Sub p31_move2bin_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_move2bin"
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p31_grid-period")
                .Add("periodcombo-custom_query")
            End With
            With Master
                Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Request.Item("pid"))
                If pids.Count = 0 Then .StopPage("pid missing")
                Me.hidPIDs.Value = String.Join(",", pids)
                .DataPID = pids(0)
                Me.CurrentPrefix = Request.Item("prefix")
                If Me.CurrentPrefix = "" Then .StopPage("prefix missing")
                .HeaderText = "Přesunout rozpracovanost do archivu"
                If pids.Count = 1 Then
                    .HeaderText += " | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), .DataPID)
                End If
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("p31_grid-period")
                .AddToolbarButton("Uložit změny pro zaškrtlé úkony", "ok", , "Images/save.png")
                If Request.Item("direction") <> "" Then
                    basUI.SelectRadiolistValue(Me.opgDirection, Request.Item("direction"))
                End If
                Dim p41ids As New List(Of Integer)
                Select Case Me.CurrentPrefix
                    Case "p41" : p41ids = pids
                    Case "p28"
                        Dim mq As New BO.myQueryP41
                        mq.p28IDs = pids
                        mq.MG_SelectPidFieldOnly = True
                        pids = .Factory.p41ProjectBL.GetList(mq).Select(Function(p) p.PID).ToList
                        For Each pid In pids
                            p41ids.Add(pid)
                        Next
                End Select
                For Each intP41ID In p41ids
                    Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(intP41ID)
                    Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(cRec.p42ID)
                    Select Case cP42.p42ArchiveFlagP31
                        Case BO.p42ArchiveFlagP31ENUM.EditingOnly
                            Me.opgDirection.Items.FindByValue("3").Enabled = False
                        Case BO.p42ArchiveFlagP31ENUM.NoRecords
                            Me.opgDirection.Items.FindByValue("3").Enabled = False
                            Me.opgDirection.Items.FindByValue("1").Enabled = False
                            Me.opgDirection.SelectedValue = "2"
                            Master.Notify(String.Format("Typ projektu [{0}] má nastavený zákaz přesouvat úkony do archivu.", cP42.p42Name))
                    End Select
                Next
                

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
            .AddColumn("Person", "Osoba")
           
            Select Case Me.CurrentPrefix
                Case "p41"
                Case Else
                    .AddColumn("p41Name", "Projekt")
            End Select
            .AddColumn("p34Name", "Sešit")
            .AddColumn("p32Name", "Aktivita")
            .AddColumn("p31Hours_Orig", "Hodiny", BO.cfENUM.Numeric2)
            .AddColumn("p31Rate_Billing_Orig", "Sazba", BO.cfENUM.Numeric2)
            .AddColumn("p31Amount_WithoutVat_Orig", "Částka", BO.cfENUM.Numeric2)
            .AddColumn("j27Code_Billing_Orig", "")
            .AddColumn("p31Text", "Text")
        End With
    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)

        Select Case Me.CurrentPrefix
            Case "p28"
                mq.p28IDs_Client = BO.BAS.ConvertPIDs2List(Me.hidPIDs.Value)
            Case "p41"
                mq.p41IDs = BO.BAS.ConvertPIDs2List(Me.hidPIDs.Value)
            Case "j02"
                mq.j02ID = Master.DataPID
        End Select
        Select Case Me.opgDirection.SelectedValue
            Case "1"
                mq.QuickQuery = BO.myQueryP31_QuickQuery.Editing
            Case "2"
                mq.QuickQuery = BO.myQueryP31_QuickQuery.MovedToBin
            Case "3"
                mq.QuickQuery = BO.myQueryP31_QuickQuery.Approved
        End Select


        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead

    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p31_move2bin")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If Master.DataPID = 0 Then Return
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
        If Master.DataPID = 0 Then Return
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

    Private Sub opgDirection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgDirection.SelectedIndexChanged
        RecalcVirtualRowCount()
        grid1.Rebind(False)
        With Master
            If Me.opgDirection.SelectedValue = "1" Then
                .HeaderText = "Přesunout rozpracovanost do archivu | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), .DataPID)
            Else
                .HeaderText = "Přesunout úkony z archivu do rozpracovanosti | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), .DataPID)
            End If
        End With
        
    End Sub

    Private Sub p31_move2bin_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim pids As List(Of Integer) = grid1.GetSelectedPIDs()
            If pids.Count = 0 Then
                Master.Notify("Musíte zaškrtnout minimálně jeden záznam.", NotifyLevel.WarningMessage)
                Return
            End If
            With Master.Factory.p31WorksheetBL
                Select Case Me.opgDirection.SelectedValue
                    Case "1", "3"
                        If .MoveToBin(pids) Then
                            Master.CloseAndRefreshParent("p31-bin")
                        Else
                            Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        End If
                    Case "2"
                        If .MoveFromBin(pids) Then
                            Master.CloseAndRefreshParent("p31-bin")
                        Else
                            Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        End If
                End Select
            End With
            
        End If
    End Sub
End Class