Public Class p61_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p61_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Klastr aktivit"
            End With
            Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(New BO.myQueryP32).OrderBy(Function(p) p.p34Name).ThenBy(Function(p) p.p32Name)
            Me.p32ID.DataBind()
            Me.p34ID.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery)
            Me.p34ID.DataBind()
            SetupGrid()

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p61ActivityCluster = Master.Factory.p61ActivityClusterBL.Load(Master.DataPID)
        With cRec
            Me.p61Name.Text = .p61Name
            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        Dim lisP32 As List(Of BO.p32Activity) = Master.Factory.p61ActivityClusterBL.GetList_p32(Master.DataPID).OrderBy(Function(p) p.p34Name).ThenBy(Function(p) p.p32Name).ToList
        For Each c In lisP32
            Dim cTMP As New BO.p85TempBox
            cTMP.p85GUID = ViewState("guid")
            cTMP.p85DataPID = c.PID

            Master.Factory.p85TempBoxBL.Save(cTMP)
        Next
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p61ActivityClusterBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p61-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid"))
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p61ActivityClusterBL
            Dim cRec As BO.p61ActivityCluster = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p61ActivityCluster)
            cRec.p61Name = Me.p61Name.Text
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            Dim lisP32 As List(Of BO.p32Activity) = GetListP32()

            If .Save(cRec, lisP32) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p61-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Public Function GetListP32() As List(Of BO.p32Activity)
        Dim lisTMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        Dim p32ids As List(Of Integer) = lisTMP.Select(Function(p) p.p85DataPID).ToList
        Dim lisP32 As New List(Of BO.p32Activity)
        Dim mq As New BO.myQueryP32
        mq.PIDs = p32ids
        Return Master.Factory.p32ActivityBL.GetList(mq)
    End Function

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is Telerik.Web.UI.GridDataItem Then Return

        Dim dataItem As Telerik.Web.UI.GridDataItem = CType(e.Item, Telerik.Web.UI.GridDataItem)
        Dim cRec As BO.p32Activity = CType(e.Item.DataItem, BO.p32Activity)
        Select Case cRec.p33ID
            Case 2, 5
                If cRec.p34IncomeStatementFlag = 1 Then
                    dataItem.ForeColor = Drawing.Color.IndianRed
                Else
                    dataItem.ForeColor = Drawing.Color.Blue
                End If

        End Select

    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim lisTMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        Dim mq As New BO.myQueryP32
        mq.PIDs = lisTMP.Where(Function(p) p.p85DataPID <> 0).Select(Function(r) r.p85DataPID).ToList
        If mq.PIDs.Count = 0 Then mq.AddItemToPIDs(-666)
        grid1.DataSource = Master.Factory.p32ActivityBL.GetList(mq).OrderBy(Function(p) p.p34Name).ThenBy(Function(p) p.p32Name)
    End Sub
    Private Sub SetupGrid()
        With grid1
            .ClearColumns()
            .radGridOrig.ShowFooter = False
            .AddCheckboxSelector()
            .AddColumn("p34Name", "Sešit")
            .AddColumn("p32Name", "Aktivita")
            .AddColumn("p32Code", "Kód")
            .AddColumn("p32IsBillable", "Fakturovatelné", BO.cfENUM.Checkbox)
            .PageSize = 50
        End With

    End Sub

    Private Sub cmdRemoveSelected_Click(sender As Object, e As EventArgs) Handles cmdRemoveSelected.Click
        If grid1.GetSelectedPIDs().Count = 0 Then
            Master.Notify("Není vybrán ani jeden řádek ze seznamu!", 2)
            Return
        End If
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).ToList


        For Each intGridPID As Integer In grid1.GetSelectedPIDs()
            Dim intPID As Integer = intGridPID
            Dim cRec As BO.p85TempBox = lisTMP.Find(Function(p) p.p85DataPID = intPID)
            If Not cRec Is Nothing Then
                Master.Factory.p85TempBoxBL.Delete(cRec)
            End If
        Next

        grid1.Rebind(False)
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).ToList
        Dim cRec As New BO.p85TempBox
        cRec.p85GUID = ViewState("guid")
        
        Dim intP32ID As Integer = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
        If intP32ID = 0 Then
            Master.Notify("Musíte vybrat aktivitu", 2)
            Return
        End If
        If lisTMP.Where(Function(p) p.p85DataPID = intP32ID).Count > 0 Then
            Master.Notify("Tato aktivita již je v seznamu.", 1)
            Return
        End If
        cRec.p85DataPID = intP32ID
        Master.Factory.p85TempBoxBL.Save(cRec)

        grid1.Rebind(True, intP32ID)
    End Sub

    Private Sub cmdAddP34ID_Click(sender As Object, e As EventArgs) Handles cmdAddP34ID.Click
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).ToList
        

        Dim intP34ID As Integer = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        If intP34ID = 0 Then
            Master.Notify("Musíte vybrat sešit", 2)
            Return
        End If
        Dim mq As New BO.myQueryP32
        mq.p34ID = intP34ID
        Dim lisP32 As IEnumerable(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq)
        For Each c In lisP32
            If lisTMP.Where(Function(p) p.p85DataPID = c.PID).Count = 0 Then
                Dim cRec As New BO.p85TempBox
                cRec.p85GUID = ViewState("guid")
                cRec.p85DataPID = c.PID
                Master.Factory.p85TempBoxBL.Save(cRec)
            End If
            
        Next
        

        grid1.Rebind(True)
    End Sub
End Class