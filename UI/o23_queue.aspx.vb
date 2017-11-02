Imports Telerik.Web.UI

Public Class o23_queue
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private Property _needFilterIsChanged As Boolean = False

    Public ReadOnly Property CurrentX18ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidX18ID.Value)
        End Get
    End Property
    Public Property CurrentMasterPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterPID.Value)
        End Get
        Set(value As Integer)
            Me.hidMasterPID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentMasterPrefix As String
        Get
            Return Me.hidMasterPrefix.Value
        End Get
        Set(value As String)
            Me.hidMasterPrefix.Value = value
        End Set
    End Property
    Public ReadOnly Property CurrentMasterX29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix)
        End Get
    End Property

    Private Sub o23_queue_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidX18ID.Value = Request.Item("x18id")
            hidX20ID.Value = Request.Item("x20id")
            With Master
                .HeaderText = "Najít dokument ke spárování"
                If Me.CurrentX18ID = 0 Then
                    .StopPage("x18id is missing.")
                Else
                    Handle_ChangeX18ID()
                End If
                If Request.Item("masterpid") <> "" Then
                    Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid")) : Me.CurrentMasterPrefix = Request.Item("masterprefix")
                End If
                SetupPeriodQuery()

                Dim lisPars As New List(Of String), strX18ID As String = hidX18ID.Value
                With lisPars
                    .Add("o23_queue-sort-" & strX18ID)
                    .Add("periodcombo-custom_query")
                    .Add("o23_queue-periodtype-" & strX18ID)
                    .Add("o23_queue-period-" & strX18ID)
                    .Add("o23_queue-filter_setting-" & strX18ID)
                    .Add("o23_queue-filter_sql-" & strX18ID)
                    .Add("o23_queue-filter_b02id-" & strX18ID)
                    .Add("o23_queue-mode-" & strX18ID)
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                    basUI.SelectDropdownlistValue(Me.cbxMode, .GetUserParam("o23_queue-mode-" & strX18ID, "1"))
                    If .GetUserParam("o23_queue-sort-" & strX18ID) <> "" Then
                        grid1.radGridOrig.MasterTableView.SortExpressions.AddSortExpression(.GetUserParam("o23_queue-sort-" & strX18ID))
                    End If
                    basUI.SelectDropdownlistValue(Me.cbxPeriodType, .GetUserParam("o23_queue-periodtype-" & strX18ID, ""))
                    If Me.cbxPeriodType.SelectedIndex > 0 Then
                        period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                        period1.SelectedValue = .GetUserParam("o23_queue-period-" & strX18ID)
                    End If
                    If panWorkflow.Visible Then
                        basUI.SelectDropdownlistValue(Me.cbxQueryB02ID, .GetUserParam("o23_queue-filter_b02id-" & strX18ID))
                    End If

                    SetupGrid(.GetUserParam("o23_queue-filter_setting-" & strX18ID), .GetUserParam("o23_queue-filter_sql-" & strX18ID))
                End With

                .AddToolbarButton("Vložit vybraný dokument", "ok", , , False, "javascript:SelectRecordAndClose()")
            End With


            RecalcVirtualRowCount()
        End If
    End Sub

    Private Sub Handle_ChangeX18ID()
        Dim c As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(Me.CurrentX18ID)
        hidX23ID.Value = c.x23ID.ToString
        Me.x18Name.Text = c.x18Name

        If c.b01ID <> 0 Then
            hidB01ID.Value = c.b01ID.ToString
            panWorkflow.Visible = True
            cbxQueryB02ID.DataSource = Master.Factory.b02WorkflowStatusBL.GetList(c.b01ID)
            cbxQueryB02ID.DataBind()
            cbxQueryB02ID.Items.Insert(0, New ListItem("--Filtrovat aktuální stav--", ""))

        Else
            panWorkflow.Visible = False
        End If
        hidx18GridColsFlag.Value = CInt(c.x18GridColsFlag).ToString
        If c.x18Icon32 <> "" Then
            img1.ImageUrl = c.x18Icon32
        End If


    End Sub

    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        Dim lisSqlSEL As New List(Of String)
        lisSqlSEL.Add("o23Name")
        lisSqlSEL.Add("o23Code")
        lisSqlSEL.Add("o23Ordinary")
        With grid1
            .ClearColumns()
            .radGridOrig.ShowFooter = False
            .AllowMultiSelect = False
            .DataKeyNames = "pid"
            .AllowCustomSorting = True
            .AddCheckboxSelector()

            .AllowCustomPaging = True
            .AddSystemColumn(20)

            .PageSize = 50

            .radGridOrig.PagerStyle.Mode = Telerik.Web.UI.GridPagerMode.NextPrevAndNumeric
            .AllowFilteringByColumn = True

            .radGridOrig.ClientSettings.Scrolling.AllowScroll = True
            .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = True

            .radGridOrig.MasterTableView.Name = "grid"

            Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(Me.CurrentX18ID)
            lisX20X18 = lisX20X18.Where(Function(p) p.x20IsClosed = False And (p.x20GridColumnFlag = BO.x20GridColumnENUM.CategoryColumn Or p.x20GridColumnFlag = BO.x20GridColumnENUM.Both)).OrderBy(Function(p) p.x20IsMultiSelect).ThenBy(Function(p) p.x29ID)   'omezit pouze na otevřené vazby + vazby vyplňované přes záznam položky kategorie
            For Each c In lisX20X18
                .AddColumn("Entita" & c.x20ID.ToString, c.BindName, BO.cfENUM.AnyString, True, , "dbo.stitek_entity(a.o23ID," & c.x20ID.ToString & ")", , False, True)
                lisSqlSEL.Add("dbo.stitek_entity(a.o23ID," & c.x20ID.ToString & ") as Entita" & c.x20ID.ToString)
            Next
            If hidB01ID.Value <> "" Then
                .AddColumn("b02Name", "Stav", BO.cfENUM.AnyString, True, , "b02Name", , False, True)
                .AddColumn("Receiver", "Řeší", BO.cfENUM.AnyString, True, True, "dbo.o23_getroles_inline(a.o23ID)")
                lisSqlSEL.Add("dbo.o23_getroles_inline(a.o23ID) as Receiver")
            End If
            Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Master.Factory.x18EntityCategoryBL.GetList_x16(Me.CurrentX18ID).Where(Function(p) p.x16IsGridField = True)
            If lisX16.Count = 0 Then
                .AddColumn("o23Name", "Název", BO.cfENUM.AnyString, True, , "o23Name", , False, True)
                .AddColumn("o23Code", "Kód", BO.cfENUM.AnyString, True, , "o23Code", , False, True)
                .AddColumn("o23Ordinary", "#", BO.cfENUM.Numeric0, True, , "o23Ordinary", , False, False)
            Else
                If hidx18GridColsFlag.Value = "1" Or hidx18GridColsFlag.Value = "3" Then
                    .AddColumn("o23Name", "Název", BO.cfENUM.AnyString, True, , "o23Name", , False, True)
                End If
                If hidx18GridColsFlag.Value = "1" Or hidx18GridColsFlag.Value = "2" Then
                    .AddColumn("o23Code", "Kód", BO.cfENUM.AnyString, True, , "o23Code", , False, True)
                End If
                For Each c In lisX16
                    Dim strH As String = c.x16NameGrid
                    If strH = "" Then strH = c.x16Name
                    .AddColumn(c.x16Field, strH, c.GridColumnType, True, , c.x16Field, , False, True)
                    lisSqlSEL.Add(c.x16Field)

                    If c.FieldType = BO.x24IdENUM.tDate Or c.FieldType = BO.x24IdENUM.tDateTime Then
                        Me.cbxPeriodType.Items.Add(New ListItem(c.x16Name, c.x16Field))
                    End If
                Next
            End If


            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With
        hidCols.Value = String.Join(",", lisSqlSEL)
    End Sub
    Private Sub RecalcVirtualRowCount()
        Dim mq As New BO.myQueryO23(0)
        InhaleMyQuery(mq)
        grid1.VirtualRowCount = Master.Factory.o23DocBL.GetVirtualCount(mq)

        lblVirtualCount.Text = grid1.VirtualRowCount.ToString & "x"

        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub
    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryO23)
        With mq
            .x23ID = CInt(hidX23ID.Value)
            If panWorkflow.Visible Then
                If cbxQueryB02ID.SelectedIndex > 0 Then .b02IDs = BO.BAS.ConvertPIDs2List(cbxQueryB02ID.SelectedValue)
            End If
            If Me.cbxMode.SelectedValue = "1" Then
                .x20ID_UnBound = CInt(hidX20ID.Value)
            End If
            If Me.cbxMode.SelectedValue = "2" Then
                .x20ID_Bound = CInt(hidX20ID.Value)
            End If
            .MyRecordsDisponible = True
            .MG_GridSqlColumns = Me.hidCols.Value
            .ColumnFilteringExpression = grid1.GetFilterExpressionCompleteSql()
            If Me.CurrentMasterPrefix <> "" Then
                .Record_x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix)
                .RecordPID = Me.CurrentMasterPID
            End If

            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
            If Me.hidDefaultSorting.Value <> "" Then
                If .MG_SortString = "" Then
                    .MG_SortString = Me.hidDefaultSorting.Value
                Else
                    .MG_SortString = Me.hidDefaultSorting.Value & "," & .MG_SortString
                End If
            End If
            With Me.cbxPeriodType
                If .SelectedValue <> "" Then
                    Select Case .SelectedValue
                        Case "DateInsert"
                            mq.DateInsertFrom = period1.DateFrom : mq.DateInsertUntil = period1.DateUntil
                        Case Else
                            mq.DateFrom = period1.DateFrom
                            mq.DateUntil = period1.DateUntil
                            mq.DateQueryFieldBy = .SelectedValue
                    End Select
                End If

            End With

        End With

    End Sub

    Private Sub o23_queue_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If cbxPeriodType.SelectedIndex > 0 Then
            With Me.period1
                .Visible = True
                If .SelectedValue <> "" Then
                    .BackColor = basUI.ColorQueryRGB
                    Me.CurrentPeriodQuery.Text = "<img src='Images/datepicker.png'/> " & Me.cbxPeriodType.SelectedItem.Text
                    If Year(.DateFrom) = Year(.DateUntil) Then
                        Me.CurrentPeriodQuery.Text += " " & Format(.DateFrom, "d.M") & "-" & Format(.DateUntil, "d.M.yyyy")
                    Else
                        Me.CurrentPeriodQuery.Text += " " & Format(.DateFrom, "d.M.yy") & "-" & Format(.DateUntil, "d.M.yyyy")
                    End If

                Else
                    .BackColor = Nothing
                    Me.CurrentPeriodQuery.Text = ""

                End If
            End With
        Else
            period1.Visible = False
        End If
        If grid1.GetFilterExpression <> "" Then
            cmdCĺearFilter.Visible = True
        Else
            cmdCĺearFilter.Visible = False
        End If
        Me.CurrentQuery.Text = ""

        If panWorkflow.Visible Then
            basUIMT.RenderQueryCombo(Me.cbxQueryB02ID)
        End If
        If Me.cbxMode.SelectedValue = "1" Or Me.cbxMode.SelectedValue = "2" Then
            basUIMT.RenderQueryCombo(Me.cbxMode)

        End If
    End Sub

    Private Sub cmdCĺearFilter_Click(sender As Object, e As EventArgs) Handles cmdCĺearFilter.Click
        With Master.Factory.j03UserBL
            .SetUserParam("o23_queue-filter_setting-" & Me.CurrentX18ID.ToString, "")
            .SetUserParam("o23_queue-filter_sql-" & Me.CurrentX18ID.ToString, "")
        End With
        ReloadPage()
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
        If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
        If hidB01ID.Value <> "" Then
            If Not cRec.Item("b02Color") Is System.DBNull.Value Then
                dataItem.Item("systemcolumn").Style.Item("background-color") = cRec.Item("b02Color")
            End If
        End If
        With dataItem.Item("systemcolumn")
            If cRec.Item("IsO27") Then
                dataItem("systemcolumn").Text += "<a href='fileupload_preview.aspx?prefix=o23&pid=" & cRec.Item("pid").ToString & "' target='_blank' title='Dokument má přílohy'><img src='Images/attachment.png'/></a>"
            End If
        End With
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("o23_queue-filter_setting-" & Me.CurrentX18ID.ToString, grid1.GetFilterSetting())
                .SetUserParam("o23_queue-filter_sql-" & Me.CurrentX18ID.ToString, grid1.GetFilterExpression())
            End With
            RecalcVirtualRowCount()
        End If
        Dim mq As New BO.myQueryO23(CInt(hidX23ID.Value))
        With mq
            .MG_PageSize = 50
            .MG_CurrentPageIndex = grid1.radGridOrig.CurrentPageIndex
        End With
        InhaleMyQuery(mq)

        Dim dt As DataTable = Master.Factory.o23DocBL.GetDataTable4Grid(mq)
        If dt Is Nothing Then
            Master.Notify(Master.Factory.p41ProjectBL.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            grid1.DataSourceDataTable = dt
        End If
    End Sub

    Private Sub grid1_SortCommand(SortExpression As String, strOwnerTableName As String) Handles grid1.SortCommand
        Master.Factory.j03UserBL.SetUserParam("o23_queue-sort-" & Me.CurrentX18ID.ToString, SortExpression)
    End Sub
    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
        ''_CurFilterDbField = strFilterColumn
    End Sub
    Private Sub cbxPeriodType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPeriodType.SelectedIndexChanged
        With Master.Factory.j03UserBL
            If Me.cbxPeriodType.SelectedIndex > 0 And Not period1.Visible Then
                .InhaleUserParams("periodcombo-custom_query", "o23_queue-period-" & Me.CurrentX18ID.ToString)
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("o23_queue-period-" & Me.CurrentX18ID.ToString)
            End If

            .SetUserParam("o23_queue-periodtype-" & Me.CurrentX18ID.ToString, Me.cbxPeriodType.SelectedValue)
        End With

        ReloadPage()
        

    End Sub

    Private Sub ReloadPage()
        'RecalcVirtualRowCount()
        'grid1.Rebind(False)
        Server.Transfer("o23_queue.aspx?x18id=" & Me.hidX18ID.Value & "&x20id=" & hidX20ID.Value & "&masterprefix=" & Me.hidMasterPrefix.Value & "&masterpid=" & hidMasterPID.Value)
    End Sub

    

    Private Sub cbxQueryB02ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryB02ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o23_queue-filter_b02id-" & Me.CurrentX18ID.ToString, Me.cbxQueryB02ID.SelectedValue)
        ReloadPage

    End Sub

    

    Private Sub SetupPeriodQuery()
        With Me.cbxPeriodType.Items
            If .Count > 0 Then .Clear()
            .Add(New ListItem("--Filtrovat období--", ""))
            .Add(New ListItem("Založení dokumentu", "DateInsert"))

        End With

    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("o23_queue-period-" & hidX18ID.Value, Me.period1.SelectedValue)
        ReloadPage()
    End Sub

   
    Private Sub cbxMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxMode.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o23_queue-mode-" & hidX18ID.Value, Me.cbxMode.SelectedValue)
        ReloadPage()
    End Sub
End Class