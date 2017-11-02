Public Class p36_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p36_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p36_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Uzamknuté worksheet období"
            End With

            Me.j11ID.DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            Me.j11ID.DataBind()

            Me.p34ids.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery)
            Me.p34ids.DataBind()

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p36LockPeriod = Master.Factory.p36LockPeriodBL.Load(Master.DataPID)
        With cRec
            Me.p36DateFrom.SelectedDate = .p36datefrom
            Me.p36DateUntil.SelectedDate = .p36DateUntil
            Me.p36IsAllSheets.Checked = .p36IsAllSheets
            If .j02ID <> 0 Then
                Me.j02ID.Value = .j02ID.ToString
                Me.j02ID.Text = .Person
                Me.opgScope.SelectedValue = "j02"
            End If
            If .j11ID <> 0 Then
                Me.j11ID.SelectedValue = .j11ID.ToString
                Me.opgScope.SelectedValue = "j11"
            End If
            If .p36IsAllPersons Then
                Me.opgScope.SelectedValue = "all"
            End If

            Master.Timestamp = .Timestamp

            Dim lis As IEnumerable(Of BO.p37LockPeriod_Sheet) = Master.Factory.p36LockPeriodBL.GetList_p37(Master.DataPID)
            basUI.CheckItems(Me.p34ids, lis.Select(Function(p) p.p34ID).ToList)

            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p36LockPeriodBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p36-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p36LockPeriodBL
            Dim cRec As BO.p36LockPeriod = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p36LockPeriod)

            cRec.j11ID = 0 : cRec.j02ID = 0 : cRec.p36IsAllPersons = False
            Select Case Me.opgScope.SelectedValue
                Case "j02"
                    cRec.j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
                Case "j11"
                    cRec.j11ID = BO.BAS.IsNullInt(Me.j11ID.SelectedValue)
                Case "all"
                    cRec.p36IsAllPersons = True
            End Select
            
            If Not Me.p36DateFrom.IsEmpty Then
                cRec.p36DateFrom = Me.p36DateFrom.SelectedDate
            End If
            If Not Me.p36DateUntil.IsEmpty Then
                cRec.p36DateUntil = Me.p36DateUntil.SelectedDate
            End If

            cRec.p36IsAllSheets = Me.p36IsAllSheets.Checked

            Dim lisP37 As New List(Of BO.p37LockPeriod_Sheet)
            If Not p36IsAllSheets.Checked Then
                For Each intP34ID As Integer In basUI.GetCheckedItems(Me.p34ids)
                    Dim cP37 As New BO.p37LockPeriod_Sheet
                    cP37.p34ID = intP34ID
                    lisP37.Add(cP37)
                Next
            End If
            

            If .Save(cRec, lisP37) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p36-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j11ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j11ID.NeedMissingItem
        Dim cRec As BO.j11Team = Master.Factory.j11TeamBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.j11Name
        End If
    End Sub

    Private Sub p36_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblPerson.Visible = False : Me.j02ID.Visible = False
        lblTeam.Visible = False : Me.j11ID.Visible = False
        Select Case Me.opgScope.SelectedValue
            Case "j02"
                lblPerson.Visible = True : Me.j02ID.Visible = True
            Case "j11"
                lblTeam.Visible = True : Me.j11ID.Visible = True
            Case "all"
        End Select
        
        ph1.Visible = Not Me.p36IsAllSheets.Checked
        p34ids.Visible = ph1.Visible
    End Sub
End Class