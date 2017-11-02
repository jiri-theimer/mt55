Public Class p91_add_worksheet
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_add_worksheet_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_add_worksheet"
    End Sub

    Private ReadOnly Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidP41ID.Value)
        End Get
    End Property
    Private ReadOnly Property CurrentP28ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidP28ID.Value)
        End Get
    End Property
    Private ReadOnly Property CurrentP31IDs As List(Of Integer)
        Get
            If hidP31IDs.Value = "" Then
                Return New List(Of Integer)
            Else
                Return BO.BAS.ConvertPIDs2List(hidP31IDs.Value)
            End If

        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                hidP41ID.Value = Request.Item("p41id")
                hidP28ID.Value = Request.Item("p28id")
                hidP31IDs.Value = Request.Item("p31ids")

                If Me.CurrentP41ID = 0 And Me.CurrentP28ID = 0 And Me.CurrentP31IDs.Count = 0 Then
                    .StopPage("p41id, p31ids and p28id missing.")
                End If
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    panSearchInvoice.Visible = True

                End If
                .HeaderIcon = "Images/worksheet_32.png"
                If Me.CurrentP41ID > 0 Then
                    imgEntity.ImageUrl = "Images/project_32.png"
                    lblEntityHeader.Text = .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)
                End If
                If Me.CurrentP28ID > 0 Then
                    imgEntity.ImageUrl = "Images/contact_32.png"
                    lblEntityHeader.Text = .Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, Me.CurrentP28ID)
                End If
                If Me.CurrentP31IDs.Count > 0 Then
                    Me.period1.Visible = False
                End If

                .AddToolbarButton("Potvrdit", "ok", , "Images/save.png")

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p91_create-period")
                    .Add("periodcombo-custom_query")                   
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                Dim strDefPeriod As String = .Factory.j03UserBL.GetUserParam("p91_create-period")
                If Request.Item("period") <> "" Then strDefPeriod = Request.Item("period")
                period1.SelectedValue = strDefPeriod

                .HeaderText = "Přidat do faktury další položky | " & .Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, .DataPID)
            End With
            RecalcVirtualRowCount()
            SetupGrid()

            
        End If
    End Sub

    Private Sub SetupGrid()

        Dim cJ70 As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.LoadSystemTemplate(BO.x29IdEnum.p31Worksheet, Master.Factory.SysUser.PID, "p41-approved")
        Dim cS As New SetupDataGrid(Master.Factory, grid1, cJ70)
        With cS
            .PageSize = 500
            .AllowCustomPaging = False
            If Me.CurrentP31IDs.Count > 0 Then
                .AllowMultiSelect = False
                .AllowMultiSelectCheckboxSelector = False
            Else
                .AllowMultiSelect = True
                .AllowMultiSelectCheckboxSelector = True
            End If
            
            .ContextMenuWidth = 0
        End With
        Dim cG As PreparedDataGrid = basUIMT.PrepareDataGrid(cS)
        

        ''basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, 500, False, True)

        
    End Sub



    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p91_add_worksheet")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource

        Dim strSort As String = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()
       
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)
        With mq
            .MG_PageSize = 500
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = strSort

        End With
        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        If hidP31IDs.Value <> "" Then
            If Me.CurrentP31IDs.Count > lis.Count Then
                Master.Notify("Do faktury lze vložit pouze schválené a dosud nevyfakturované úkony.")
            End If
        End If
        grid1.DataSource = lis
    End Sub
    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryP31)
        mq.p28ID_Client = Me.CurrentP28ID
        mq.p41ID = Me.CurrentP41ID
        If Me.hidP31IDs.Value <> "" Then mq.PIDs = Me.CurrentP31IDs

        If period1.Visible Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForCreateInvoice

    End Sub
    Public Sub RecalcVirtualRowCount()
        If Master.DataPID = 0 Then Return
        Dim mq As New BO.myQueryP31
        InhaleMyQuery(mq)

        Dim cSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, True)
        If Not cSum Is Nothing Then
            grid1.VirtualRowCount = cSum.RowsCount

            ViewState("footersum") = grid1.GenerateFooterItemString(cSum)
        Else
            ViewState("footersum") = ""
            grid1.VirtualRowCount = 0
        End If


        grid1.radGridOrig.CurrentPageIndex = 0

    End Sub

    Private Sub grid1_NeedFooterSource(footerItem As Telerik.Web.UI.GridFooterItem, footerDatasource As Object) Handles grid1.NeedFooterSource
        footerItem.Item("systemcolumn").Text = "<img src='Images/sum.png'/>"
        grid1.ParseFooterItemString(footerItem, ViewState("footersum"))
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p91_create-period", period1.SelectedValue)
        RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Master.DataPID = 0 Then Master.DataPID = BO.BAS.IsNullInt(Me.p91id_search.Value)
            If Master.DataPID = 0 Then
                Master.Notify("Musíte vybrat fakturu.") : Return
            End If
            Dim pids As List(Of Integer) = grid1.GetSelectedPIDs()

            If Me.CurrentP31IDs.Count > 0 Then
                pids = grid1.GetAllPIDs()
            End If
            If pids.Count = 0 Then
                Master.Notify("Na vstupu musí být alespoň jeden úkon.", NotifyLevel.WarningMessage) : Return
            End If
            With Master.Factory.p31WorksheetBL
                If .AppendToInvoice(Master.DataPID, pids) Then
                    Master.CloseAndRefreshParent("p31-add-p91")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With
        End If
    End Sub

    Private Sub p91_add_worksheet_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub
End Class