Imports Telerik.Web.UI
Public Class p91_remove_worksheet
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_remove_worksheet_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_remove_worksheet"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                ViewState("p31ids") = Request.Item("p31ids")
                If ViewState("p31ids") = "" Then .StopPage("p31ids missing.")
                ViewState("oper") = Request.Item("oper")
                If ViewState("oper") = "" Then
                    .StopPage("oper is missing.")
                End If
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")

                Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                Select Case ViewState("oper")
                    Case "cut"
                        .HeaderIcon = "Images/cut_32.png"
                        .AddToolbarButton("Přesunout do archivu", "remove2bin", , "Images/bin.png")
                        .AddToolbarButton("Přesunout do rozpracovanosti", "remove2wip", , "Images/worksheet.png")
                        .AddToolbarButton("Přesunout do schválených", "remove2approve", , "Images/approve.png")
                        .HeaderText = "Vyjmout z faktury vybrané úkony, faktura: " & cRec.p91Code
                    Case "batch-3"
                        .HeaderText = "Odepsat z faktury vybrané úkony, faktura: " & cRec.p91Code
                        .AddToolbarButton("Přiřadit úkonům status [Skrytý odpis]", "batch-3", , "Images/ok.png")
                    Case "batch-2"
                        .HeaderText = "Odepsat z faktury vybrané úkony, faktura: " & cRec.p91Code
                        .AddToolbarButton("Přiřadit úkonům status [Viditelný odpis]", "batch-2", , "Images/ok.png")
                    Case "batch-6"
                        .HeaderText = "Zahrnout do paušálu vybrané úkony, faktura: " & cRec.p91Code
                        .AddToolbarButton("Přiřadit úkonům status [Zahrnuto do paušálu]", "batch-6", , "Images/ok.png")
                End Select



                Dim cDisp As BO.p91RecordDisposition = Master.Factory.p91InvoiceBL.InhaleRecordDisposition(cRec)
                If Not cDisp.OwnerAccess Then .StopPage("V kontextu této faktury nemáte oprávnění k funkci.")
                
            End With

            SetupGrid()
        End If
    End Sub

    Private Sub SetupGrid()
        ''With Master.Factory.j70QueryTemplateBL
        ''    Dim cJ70 As BO.j70QueryTemplate = .LoadSystemTemplate(BO.x29IdEnum.p31Worksheet, Master.Factory.SysUser.PID, "p91")
        ''    cJ70.j70IsFilteringByColumn = False
        ''    basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, 5000, False, False)
        ''End With
       
        With Me.grid1
            .ClearColumns()
            .DataKeyNames = "pid"
            .radGridOrig.ShowFooter = False
            .PageSize = 1000
            .AllowMultiSelect = False
            .AllowCustomPaging = False
            .AddSystemColumn(5)
            .AddColumn("p31Date", "Datum", BO.cfENUM.DateOnly)
            .AddColumn("Person", "Jméno")
            .AddColumn("p41Name", "Projekt")

            .AddColumn("p32Name", "Aktivita")
            .AddColumn("p31Value_Invoiced", "Fakturovaná hodnota", BO.cfENUM.Numeric2)
            .AddColumn("p31Rate_Billing_Invoiced", "Fakturovaná sazba", BO.cfENUM.Numeric2)
            .AddColumn("p31Amount_WithoutVat_Invoiced", "Fakturováno bez DPH", BO.cfENUM.Numeric2)
            .AddColumn("p31Text", "Text")
        End With


    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p91_remove_worksheet")
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim mq As New BO.myQueryP31
        Dim a() As String = Split(ViewState("p31ids"), ",")
        For Each s In a
            mq.AddItemToPIDs(CInt(s))
        Next
        With mq
            .MG_PageSize = 1000
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()

        End With

        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        grid1.DataSource = lis
        
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Dim pids As New List(Of Integer)
        Dim a() As String = Split(ViewState("p31ids"), ",")
        For i As Integer = 0 To UBound(a)
            pids.Add(CInt(a(i)))
        Next

        Select Case strButtonValue
            Case "remove2approve", "remove2wip", "remove2bin"
                With Master.Factory.p31WorksheetBL

                    If strButtonValue = "remove2approve" Then
                        If .RemoveFromInvoice(Master.DataPID, pids) Then
                            Master.CloseAndRefreshParent("p31-remove")
                        Else
                            Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        End If
                    End If
                    If strButtonValue = "remove2wip" Then
                        If .RemoveFromInvoice(Master.DataPID, pids) Then
                            If .RemoveFromApproving(pids) Then
                                Master.CloseAndRefreshParent("p31-remove")
                            End If
                        Else
                            Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        End If
                    End If
                    If strButtonValue = "remove2bin" Then
                        If .RemoveFromInvoice(Master.DataPID, pids) Then
                            If .RemoveFromApproving(pids) Then
                                If .MoveToBin(pids) Then
                                    Master.CloseAndRefreshParent("p31-remove")
                                End If
                            End If
                        End If
                    End If
                End With
            Case "batch-3", "batch-2", "batch-6"
                Dim lis As New List(Of BO.p31WorksheetInvoiceChange)
                For Each intP31ID As Integer In pids
                    Dim c As New BO.p31WorksheetInvoiceChange
                    c.p31ID = intP31ID
                    Select Case strButtonValue
                        Case "batch-3" : c.p70ID = BO.p70IdENUM.SkrytyOdpis
                        Case "batch-2" : c.p70ID = BO.p70IdENUM.ViditelnyOdpis
                        Case "batch-6" : c.p70ID = BO.p70IdENUM.ZahrnutoDoPausalu
                    End Select
                    c.p31IsInvoiceManual = True
                    lis.Add(c)
                Next
                If Master.Factory.p31WorksheetBL.UpdateInvoice(Master.DataPID, lis) Then
                    Master.CloseAndRefreshParent("p31-remove")
                End If

        End Select

    End Sub
End Class