Public Class j11_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j11_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "j11_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Tým osob"
            End With
            SetupGrid()
            Me.j07ID.DataSource = Master.Factory.j07PersonPositionBL.GetList()
            Me.j07ID.DataBind()
            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.j11Team = Master.Factory.j11TeamBL.Load(Master.DataPID)
        With cRec
            Me.j11name.Text = .j11Name
            Me.j11IsAllPersons.Checked = .j11IsAllPersons
            Me.j11Email.Text = .j11Email
            Me.j11RobotAddress.Text = .j11RobotAddress
            Me.j11DomainAccount.Text = .j11DomainAccount
            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        Dim lisJ12 As List(Of BO.j12Team_Person) = Master.Factory.j11TeamBL.GetList_BoundJ12(Master.DataPID).ToList
        For Each cJ12 In lisJ12
            Dim cTMP As New BO.p85TempBox
            cTMP.p85GUID = ViewState("guid")
            If Master.IsRecordClone Then
                cTMP.p85DataPID = 0
            Else
                cTMP.p85DataPID = cJ12.j12ID
            End If
            cTMP.p85OtherKey1 = cJ12.j02ID
            
            Master.Factory.p85TempBoxBL.Save(cTMP)
        Next
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j11TeamBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j11-delete")
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

        With Master.Factory.j11TeamBL
            Dim cRec As BO.j11Team = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j11Team)
            cRec.j11Name = Me.j11name.Text
            cRec.j11IsAllPersons = Me.j11IsAllPersons.Checked
            cRec.j11RobotAddress = Me.j11RobotAddress.Text
            cRec.j11Email = Me.j11Email.Text
            cRec.j11DomainAccount = Me.j11DomainAccount.Text
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            Dim lisJ02 As List(Of BO.j02Person) = Nothing
            If Not cRec.j11IsAllPersons Then lisJ02 = GetListJ02()

            If .Save(cRec, lisJ02) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j11-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j11_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panMembers.Visible = Not Me.j11IsAllPersons.Checked
    End Sub
    Public Function GetListJ02() As List(Of BO.j02Person)
        Dim lisTMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        Dim lisJ02 As New List(Of BO.j02Person)
        For Each cTMP In lisTMP
            Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(cTMP.p85OtherKey1)
            lisJ02.Add(cRec)
        Next


        Return lisJ02
    End Function

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim lisTMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        Dim mq As New BO.myQueryJ02
        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.PIDs = lisTMP.Where(Function(p) p.p85OtherKey1 <> 0).Select(Function(r) r.p85OtherKey1).ToList
        If mq.PIDs.Count = 0 Then mq.AddItemToPIDs(-666)
        grid1.DataSource = Master.Factory.j02PersonBL.GetList(mq)
    End Sub

    Private Sub SetupGrid()
        With grid1
            .ClearColumns()
            .radGridOrig.ShowFooter = False
            .AddCheckboxSelector()
            .AddColumn("FullNameDesc", "Osoba")
            .AddColumn("j07Name", "Pozice")
            .AddColumn("j18Name", "Středisko")
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
            Dim cRec As BO.p85TempBox = lisTMP.Find(Function(p) p.p85OtherKey1 = intPID)
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
        Dim intDataPID As Integer

        intDataPID = BO.BAS.IsNullInt(Me.j02id_search.Value)
        If intDataPID = 0 Then
            Master.Notify("Musíte vybrat osobu", 2)
            Return
        End If
        If lisTMP.Where(Function(p) p.p85OtherKey1 = intDataPID).Count > 0 Then
            Master.Notify("Tato osoba již je vybrána.", 1)
            Return
        End If
        cRec.p85OtherKey1 = intDataPID
        j02id_search.Text = ""
        j02id_search.Value = ""

        Master.Factory.p85TempBoxBL.Save(cRec)

        grid1.Rebind(True, intDataPID)
    End Sub

    Private Sub cmdAddJ07_Click(sender As Object, e As EventArgs) Handles cmdAddJ07.Click
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).ToList
        
        Dim intJ07ID As Integer = BO.BAS.IsNullInt(Me.j07ID.SelectedValue)
        If intJ07ID = 0 Then
            Master.Notify("Musíte vybrat osobu", 2)
            Return
        End If
        Dim mq As New BO.myQueryJ02
        mq.j07ID = intJ07ID
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq)
        For Each c In lisJ02
            If lisTMP.Where(Function(p) p.p85OtherKey1 = c.PID).Count = 0 Then
                Dim cRec As New BO.p85TempBox
                cRec.p85GUID = ViewState("guid")
                cRec.p85OtherKey1 = c.PID
                Master.Factory.p85TempBoxBL.Save(cRec)
            End If

        Next
        grid1.Rebind(False)
    End Sub
End Class