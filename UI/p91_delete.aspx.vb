Imports Telerik.Web.UI
Public Class p91_delete
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_delete_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_delete"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidGUID.Value = BO.BAS.GetGUID
            With Master

                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing.")
                .AddToolbarButton("Potvrdit odstranění faktury", "ok", , "Images/delete.png")

                Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                .HeaderText = String.Format("Odstranit fakturu {0}", cRec.p91Code)
                Dim cDisp As BO.p91RecordDisposition = Master.Factory.p91InvoiceBL.InhaleRecordDisposition(cRec)
                If Not cDisp.OwnerAccess Then .StopPage("Nemáte vlastnické oprávnění k této faktuře.")


                SetupGrid()

                If cRec.p92InvoiceType = BO.p92InvoiceTypeENUM.CreditNote Then
                    opg1.SelectedValue = "0"

                End If
            End With
            RefreshTemp(Nothing, CInt(opg1.SelectedValue))




        End If
    End Sub

    Private Sub SetupGrid()
        With Master.Factory.j70QueryTemplateBL
            Dim cJ70 As BO.j70QueryTemplate = .LoadSystemTemplate(BO.x29IdEnum.p31Worksheet, Master.Factory.SysUser.PID, "p91")

            basUIMT.SetupDataGrid(Master.Factory, Me.grid1, cJ70, 500, False, True, True, , , , , 80)
        End With

    End Sub
    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        basUIMT.p31_grid_Handle_ItemDataBound(sender, e, False, False, "p91_delete")

        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        'Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)
        With dataItem("systemcolumn")
            Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)

            Dim c As BO.p85TempBox = Master.Factory.p85TempBoxBL.GetList(hidGUID.Value).First(Function(p) p.p85DataPID = cRec.PID)
            If Not c Is Nothing Then
                Select Case c.p85OtherKey1
                    Case 1
                        .Text = "<span style='background-color:orange;color:black;'>Přesunout do schválených</span>"
                    Case 2
                        .Text = "<span style='background-color:white;color:black;'>Přesunout do rozpracovaných</span>"
                    Case 3
                        .Text = "<span style='background-color:silver;color:black;'>Přesunout do archivu</span>"
                    Case 4
                        .Text = "<span style='background-color:red;color:navy;'>Nenávratně odstranit</span>"
                    Case 0
                        .Text = "????"
                End Select
            End If


        End With
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim mq As New BO.myQueryP31
        mq.p91ID = Master.DataPID
        With mq
            .MG_PageSize = 500
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex
            .MG_SortString = grid1.radGridOrig.MasterTableView.SortExpressions.GetSortString()

        End With

        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        grid1.DataSource = lis

    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Master.Factory.p85TempBoxBL.GetList(hidGUID.Value).Where(Function(p) p.p85OtherKey1 = 0).Count > 0 Then
                Master.Notify("Minimálně u jednoho úkonu faktury musíte ještě rozhodnout!", NotifyLevel.ErrorMessage)
                Return
            End If
            With Master.Factory.p91InvoiceBL
                If .Delete(Master.DataPID, hidGUID.Value) Then
                    Master.DataPID = 0
                    Master.CloseAndRefreshParent("p91-delete")
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With
        End If
        
    End Sub

    Private Sub cmdBatch1_Click(sender As Object, e As EventArgs) Handles cmdBatch1.Click
        RunBatch(1)
    End Sub

    Private Sub cmdBatch2_Click(sender As Object, e As EventArgs) Handles cmdBatch2.Click
        RunBatch(2)
    End Sub

    Private Sub RunBatch(intOper As Integer)
        Dim lis As List(Of Integer) = grid1.GetSelectedPIDs()
        If lis.Count = 0 Then
            Master.Notify("Musíte vybrat (zaškrtnout) alespoň jeden záznam.")
            Return
        End If
        If intOper = 4 Then
            'nenávratně odstranit
            For Each intP31ID In lis
                With Master.Factory.p31WorksheetBL.InhaleRecordDisposition(intP31ID)
                    If Not (.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Or .RecordDisposition = BO.p31RecordDisposition.CanEdit) Then
                        Master.Notify(.LockedReasonMessage, NotifyLevel.WarningMessage)
                        Return
                    End If
                End With

            Next
        End If
        RefreshTemp(lis, intOper)
        grid1.Rebind(True)
    End Sub

    Private Sub p91_delete_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
       
    End Sub


    Private Sub opg1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opg1.SelectedIndexChanged
        RefreshTemp(Nothing, CInt(opg1.SelectedValue))


        grid1.Rebind(False)
    End Sub

    Private Sub RefreshTemp(lis As List(Of Integer), intOper As Integer)
        ''Master.Factory.p85TempBoxBL.Truncate(hidGUID.Value)
        Dim mq As New BO.myQueryP31
        mq.p91ID = Master.DataPID
        If lis Is Nothing Then lis = Master.Factory.p31WorksheetBL.GetList(mq).Select(Function(p) p.PID).ToList
        For Each c In lis
            Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID.Value).Where(Function(p) p.p85DataPID = c)
            Dim cTemp As BO.p85TempBox = Nothing
            If lisTemp.Count > 0 Then cTemp = lisTemp(0)
            If cTemp Is Nothing Then cTemp = New BO.p85TempBox
            cTemp.p85Prefix = "p31"
            cTemp.p85GUID = hidGUID.Value
            cTemp.p85DataPID = c
            cTemp.p85OtherKey1 = intOper


            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next

    End Sub

    Private Sub cmdBatch3_Click(sender As Object, e As EventArgs) Handles cmdBatch3.Click
        RunBatch(3)
    End Sub

    Private Sub cmdBatch4_Click(sender As Object, e As EventArgs) Handles cmdBatch4.Click
        RunBatch(4)
    End Sub
End Class