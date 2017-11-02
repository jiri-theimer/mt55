Public Class p31_recalc
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

    Private Sub p31_recalc_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_recalc"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p31_grid-period")
                .Add("periodcombo-custom_query")
            End With
            With Master
                .HeaderIcon = "Images/recalc_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                Me.CurrentPrefix = Request.Item("prefix")
                If Me.CurrentPrefix = "" Then .StopPage("prefix missing")
                If .DataPID = 0 Then .StopPage("pid missing")
                Select Case Me.CurrentPrefix
                    Case "p41"
                        .HeaderText = "Přepočítat rozpracovanost v projektu | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), .DataPID)
                    Case "j02"
                        .HeaderText = "Přepočítat rozpracovanost osoby | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), .DataPID)
                End Select


                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("p31_grid-period")
                .AddToolbarButton("Uložit změny pro zaškrtlé úkony", "ok", , "Images/save.png")
            End With
           

            SetupGrid()
            RecalcVirtualRowCount()
            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        If Me.CurrentPrefix = "p41" Then
            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
            Dim intP51ID As Integer = cRec.p51ID_Billing
            If intP51ID = 0 Then
                If cRec.p28ID_Client <> 0 Then
                    Dim cClient As BO.p28Contact = Master.Factory.p28ContactBL.Load(cRec.p28ID_Client)
                    intP51ID = cClient.p51ID_Billing
                End If
            End If
            If intP51ID = 0 Then
                Master.Notify("Vybraný projekt ani jeho klient nemá přiřazený fakturační ceník!", NotifyLevel.WarningMessage)
            Else
                Dim cP51 As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(intP51ID)
                Me.p51Name.Text = cP51.NameWithCurr
            End If
        Else
            lblP51.Visible = False
            Me.p51Name.Visible = False
        End If
        

    End Sub
    Private Sub SetupGrid()
        With Me.grid1
            .ClearColumns()
            .DataKeyNames = "pid"
            .radGridOrig.ShowFooter = False
            .PageSize = 500
            .AllowMultiSelect = True
            .AddCheckboxSelector()
            .AddSystemColumn(5)
            .AddColumn("p31Date", "Datum", BO.cfENUM.DateOnly)
            .AddColumn("Person", "Osoba")
            .AllowCustomPaging = True
            Select Case Me.CurrentPrefix
                Case "p41"
                Case Else
                    .AddColumn("p28Client.p28Name", "Klient")
                    .AddColumn("p41Name", "Projekt")

            End Select
            .AddColumn("p34Name", "Sešit")
            .AddColumn("p32Name", "Aktivita")
            .AddColumn("p31Hours_Orig", "Hodiny", BO.cfENUM.Numeric2)
            .AddColumn("p31Rate_Billing_Orig", "Sazba", BO.cfENUM.Numeric2)
            .AddColumn("p31Amount_WithoutVat_Orig", "Honorář", BO.cfENUM.Numeric2)
            .AddColumn("j27Code_Billing_Orig", "")
            .AddColumn("p31Text", "Text")
        End With
    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)

        Select Case Me.CurrentPrefix
            Case "p28"
                mq.p28ID_Client = Master.DataPID
            Case "p41"
                mq.p41ID = Master.DataPID
            Case "j02"
                mq.j02ID = Master.DataPID
        End Select
        If Me.chkIncludeBin.Checked Then
            mq.QuickQuery = BO.myQueryP31_QuickQuery.MovedToBin
        Else
            mq.QuickQuery = BO.myQueryP31_QuickQuery.Editing
        End If
        Dim p33ids As New List(Of Integer)
        p33ids.Add(1)
        p33ids.Add(3)
        mq.p33IDs = p33ids

        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead

    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p31_recalc")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If Master.DataPID = 0 Then Return
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)
        With mq
            .MG_PageSize = 500
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
            If grid1.VirtualRowCount = 500 Then
                Master.Notify("Najednou lze maximálně přepočítat sazbu u 500 úkonů.", NotifyLevel.InfoMessage)
            End If
        Else

            grid1.VirtualRowCount = 0
        End If
        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub p31_recalc_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
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
                If .RecalcRates(pids) Then
                    Master.CloseAndRefreshParent("p31-recalc")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With

        End If
    End Sub

    Private Sub chkIncludeBin_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeBin.CheckedChanged
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub
End Class