Public Class o22_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord
    Private Property _curBindingLastJ24ID As Integer


    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return CType(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
        End Set
    End Property
    Public Property CurrentMasterDataPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterDataPID.Value)
        End Get
        Set(value As Integer)
            Me.hidMasterDataPID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentO21Flag As BO.o21FlagEnum
        Get
            Return DirectCast(CInt(Me.hidO21Flag.Value), BO.o21FlagEnum)
        End Get
        Set(value As BO.o21FlagEnum)
            Me.hidO21Flag.Value = CInt(value).ToString
        End Set
    End Property

    Private Sub o22_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            ''ViewState("guid_o19") = BO.BAS.GetGUID()
            ViewState("guid_o20") = BO.BAS.GetGUID()
            With Master
                .HeaderIcon = "Images/calendar_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Událost v kalendáři"


                Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("masterprefix"))
                Me.CurrentMasterDataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))

                If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified And .DataPID <> 0 Then
                    Dim cRec As BO.o22Milestone = .Factory.o22MilestoneBL.Load(.DataPID)
                    Me.CurrentX29ID = cRec.x29ID
                End If
                If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified And .DataPID = 0 Then
                    Server.Transfer("select_project.aspx?oper=createo22&" & basUI.GetCompleteQuerystring(Request))
                    ''Me.CurrentX29ID = BO.x29IdEnum.j02Person
                    ''Me.CurrentMasterDataPID = .Factory.SysUser.j02ID
                End If
                If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified Then
                    .StopPage("masterprefix missing.")
                End If

                If Me.CurrentMasterDataPID = 0 And .DataPID = 0 Then
                    .StopPage("masterpid missing.")
                End If




            End With

            Me.o21ID.DataSource = Master.Factory.o21MilestoneTypeBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
            Me.o21ID.DataBind()
            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If

        End If
    End Sub

    Private Sub InhaleObject()
        If Me.CurrentMasterDataPID = 0 Then
            Me.BoundObject.Text = ""
            Return
        End If

        Me.BoundObject.Text = Master.Factory.GetRecordCaption(Me.CurrentX29ID, Me.CurrentMasterDataPID)
        Me.lblObject.Text = BO.BAS.GetX29EntityAlias(Me.CurrentX29ID, False) & ":"
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.j02Person

                If Master.DataPID = 0 And rpO20.Items.Count = 0 Then
                    CreateTempRecord_o20(Me.CurrentMasterDataPID, Me.BoundObject.Text, 0, "")  'předvyplnit osobu
                    RefreshTempO20()
                End If
            Case Else
        End Select

    End Sub


    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            InhaleObject()
            InhaleMyDefault()
            Handle_Change_o21ID()
            Return
        End If


        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)
        Handle_Permissions(cRec)
        With cRec
            Me.CurrentO21Flag = .o21Flag
            Me.CurrentX29ID = .x29ID
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.j02Person : Me.CurrentMasterDataPID = .j02ID
                Case BO.x29IdEnum.p28Contact : Me.CurrentMasterDataPID = .p28ID
                Case BO.x29IdEnum.p41Project : Me.CurrentMasterDataPID = .p41ID
                Case BO.x29IdEnum.p56Task : Me.CurrentMasterDataPID = .p56ID
                Case BO.x29IdEnum.p91Invoice : Me.CurrentMasterDataPID = .p56ID
                Case BO.x29IdEnum.p90Proforma : Me.CurrentMasterDataPID = .p90ID

            End Select
            InhaleObject()

            Me.o22Name.Text = .o22Name
            If Not BO.BAS.IsNullDBDate(.o22DateFrom) Is Nothing Then
                Me.o22DateFrom.SelectedDate = .o22DateFrom
            End If
            If Not BO.BAS.IsNullDBDate(.o22DateUntil) Is Nothing Then
                Me.o22DateUntil.SelectedDate = .o22DateUntil
            End If
            If Not BO.BAS.IsNullDBDate(.o22ReminderDate) Is Nothing Then
                Me.o22ReminderDate.SelectedDate = .o22ReminderDate

            End If
            Me.o22IsAllDay.Checked = .o22IsAllDay
            Me.o22Description.Text = .o22Description
            Me.o21ID.SelectedValue = .o21ID.ToString
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.o22IsNoNotify.Checked = .o22IsNoNotify
            Me.o22Location.Text = .o22Location
            Master.Timestamp = .Timestamp

            Handle_FF()

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

       

        Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = Master.Factory.o22MilestoneBL.GetList_o20(cRec.PID)
        For Each c In lisO20
            CreateTempRecord_o20(c.j02ID, c.Person, c.j11ID, c.j11Name)
        Next
        If lisO20.Count > 0 Then RefreshTempO20()
    End Sub

    Private Sub Handle_Permissions(cRec As BO.o22Milestone)
        If Master.Factory.SysUser.IsAdmin Then Return 'administrátor
        If cRec.j02ID_Owner = Master.Factory.SysUser.j02ID Then Return 'vlastník záznamu

        Server.Transfer("clue_o22_record.aspx?mode=readonly&pid=" & cRec.PID.ToString)

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o22MilestoneBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o22-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        Server.Transfer("o22_record.aspx?pid=" & Master.DataPID.ToString & "&masterprefix=" & BO.BAS.GetDataPrefix(Me.CurrentX29ID) & "&masterpid=" & Me.CurrentMasterDataPID.ToString)
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o22MilestoneBL
            Dim cRec As BO.o22Milestone = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o22Milestone)
            With cRec
                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.j02Person
                        .j02ID = Me.CurrentMasterDataPID
                    Case BO.x29IdEnum.p28Contact
                        .p28ID = Me.CurrentMasterDataPID
                    Case BO.x29IdEnum.p41Project
                        .p41ID = Me.CurrentMasterDataPID
                    Case BO.x29IdEnum.p91Invoice
                        .p91ID = Me.CurrentMasterDataPID
                    Case BO.x29IdEnum.p90Proforma
                        .p90ID = Me.CurrentMasterDataPID
                    Case BO.x29IdEnum.p56Task
                        .p56ID = Me.CurrentMasterDataPID
                End Select
                .o22Name = Me.o22Name.Text
                .o22ReminderDate = BO.BAS.IsNullDBDate(Me.o22ReminderDate.SelectedDate)

                .o21ID = BO.BAS.IsNullInt(Me.o21ID.SelectedValue)
                .o22DateFrom = BO.BAS.IsNullDBDate(Me.o22DateFrom.SelectedDate)
                .o22DateUntil = BO.BAS.IsNullDBDate(Me.o22DateUntil.SelectedDate)
                .o22IsAllDay = Me.o22IsAllDay.Checked

                .o22Location = Me.o22Location.Text
                .o22Description = Me.o22Description.Text
                .o22IsNoNotify = Me.o22IsNoNotify.Checked

                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

           
            Dim lisO20 As New List(Of BO.o20Milestone_Receiver)
            For Each cTemp In Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o20"))
                Dim c As New BO.o20Milestone_Receiver
                c.j02ID = cTemp.p85OtherKey1
                c.j11ID = cTemp.p85OtherKey2
                lisO20.Add(c)
            Next

            If .Save(cRec, lisO20) Then
                Master.DataPID = .LastSavedPID
                Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.o22Milestone, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)

                Master.CloseAndRefreshParent("o22-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    
    Private Sub o21ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles o21ID.NeedMissingItem
        Dim cRec As BO.o21MilestoneType = Master.Factory.o21MilestoneTypeBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.o21Name
        End If
    End Sub

   
    Private Sub o22_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.lblDateFrom.Visible = False : Me.o22DateFrom.Visible = False
        Me.lblDateUntil.Visible = False : Me.o22DateUntil.Visible = False
        Me.o22IsAllDay.Visible = False

        Select Case Me.CurrentO21Flag
            Case BO.o21FlagEnum.DeadlineOrMilestone
                Me.lblDateUntil.Visible = True : Me.o22DateUntil.Visible = True
                If Me.o22DateUntil.IsEmpty Then Me.o22DateUntil.SelectedDate = Today.AddDays(1)
                imgO21Flag.ImageUrl = "Images/milestone.png"
                lblDateUntil.Text = "Termín:"
                With Me.o22DateUntil.DateInput
                    .DateFormat = "d.M.yyyy HH:mm ddd"
                    .DisplayDateFormat = .DateFormat
                End With
                Me.o22DateUntil.TimePopupButton.Visible = True
                'panReservation.Visible = False
            Case BO.o21FlagEnum.EventFromUntil
                Me.o22IsAllDay.Visible = True
                Me.lblDateFrom.Visible = True : Me.o22DateFrom.Visible = True
                Me.lblDateUntil.Visible = True : Me.o22DateUntil.Visible = True
                If Me.o22DateFrom.IsEmpty Then
                    Me.o22DateFrom.SelectedDate = Today.AddDays(1).AddHours(10)
                    Me.o22DateUntil.SelectedDate = Today.AddDays(1).AddHours(12)
                End If
                imgO21Flag.ImageUrl = "Images/event.png"
                lblDateUntil.Text = "Konec:"
                With Me.o22DateFrom.DateInput
                    If Not Me.o22IsAllDay.Checked Then
                        .DateFormat = "d.M.yyyy HH:mm ddd"
                    Else
                        .DateFormat = "d.M.yyyy ddd"
                    End If
                    .DisplayDateFormat = .DateFormat
                End With
                With Me.o22DateUntil.DateInput
                    .DateFormat = Me.o22DateFrom.DateInput.DateFormat
                    .DisplayDateFormat = .DateFormat
                End With
                Me.o22DateFrom.TimePopupButton.Visible = Not Me.o22IsAllDay.Checked
                Me.o22DateUntil.TimePopupButton.Visible = Not Me.o22IsAllDay.Checked
              
                
            Case Else
                imgO21Flag.ImageUrl = "Images/notepad.png"
                'panReservation.Visible = False
        End Select
        With Me.cbxSelectJ11ID
            If .Rows <= 1 Then
                Dim lis As IEnumerable(Of BO.j11Team) = Master.Factory.j11TeamBL.GetList(New BO.myQuery)
                .DataSource = lis
                .DataBind()
            End If
        End With
        
        Master.HeaderText = Me.o21ID.Text & " | " & Me.BoundObject.Text
    End Sub

    Private Sub o21ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles o21ID.SelectedIndexChanged
        Handle_Change_o21ID()

    End Sub
    Private Sub Handle_Change_o21ID()
        If Me.o21ID.SelectedValue = "" Then Return
        Dim cRec As BO.o21MilestoneType = Master.Factory.o21MilestoneTypeBL.Load(BO.BAS.IsNullInt(Me.o21ID.SelectedValue))
        Me.CurrentO21Flag = cRec.o21Flag
        Handle_FF()
    End Sub

    Private Sub InhaleMyDefault()

        If Master.DataPID <> 0 Then Return
        Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
        Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc

        Dim cRecLast As BO.o22Milestone = Master.Factory.o22MilestoneBL.LoadMyLastCreated()
        If Not cRecLast Is Nothing Then
            With cRecLast
                Me.o21ID.SelectedValue = .o21ID.ToString
                Me.CurrentO21Flag = .o21Flag
            End With
        End If
        

        If Request.Item("t1") <> "" And Request.Item("t2") <> "" Then
            Dim dt1 As New BO.DateTimeByQuerystring(Request.Item("t1")), dt2 As New BO.DateTimeByQuerystring(Request.Item("t2")), intJ02ID As Integer = BO.BAS.IsNullInt(Request.Item("j02id"))
            Me.o22DateFrom.SelectedDate = dt1.DateWithTime
            Me.o22DateUntil.SelectedDate = dt2.DateWithTime
        End If

    End Sub

    'Private Sub cbxSelectJ23ID_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles cbxSelectJ23ID.ItemDataBound
    '    Dim cRec As BO.j23NonPerson = CType(e.Item.DataItem, BO.j23NonPerson)
    '    If _curBindingLastJ24ID <> cRec.j24ID Then
    '        Dim item As New Telerik.Web.UI.RadComboBoxItem(cRec.j24Name, "")
    '        With item
    '            .BackColor = Drawing.Color.WhiteSmoke
    '            .Enabled = False
    '            .Font.Bold = True
    '            .ForeColor = Drawing.Color.Black
    '        End With
    '        Me.cbxSelectJ23ID.RadCombo.Items.Insert(cbxSelectJ23ID.Rows - 1, item)
    '    End If
    '    _curBindingLastJ24ID = cRec.j24ID
    'End Sub

    'Private Sub cbxSelectJ23ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles cbxSelectJ23ID.SelectedIndexChanged
    '    Dim intJ23ID As Integer = BO.BAS.IsNullInt(Me.cbxSelectJ23ID.SelectedValue)
    '    If intJ23ID = 0 Then Return
    '    If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o19")).Where(Function(p) p.p85OtherKey1 = intJ23ID).Count > 0 Then
    '        Return
    '    End If
    '    CreateTempRecord_o19(intJ23ID, cbxSelectJ23ID.Text)
    '    'Dim c As New BO.p85TempBox()
    '    'c.p85GUID = ViewState("guid_o19")
    '    'c.p85OtherKey1 = intJ23ID
    '    'c.p85FreeText01 = cbxSelectJ23ID.Text
    '    'Master.Factory.p85TempBoxBL.Save(c)
    '    RefreshTempO19()
    '    Me.cbxSelectJ23ID.SelectedIndex = 0
    'End Sub

    'Private Sub RefreshTempO19()
    '    Me.j23ids.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o19"))
    '    Me.j23ids.DataBind()
    'End Sub

    'Private Sub j23ids_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles j23ids.ItemCommand
    '    Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
    '    If e.CommandName = "delete" Then
    '        If Master.Factory.p85TempBoxBL.Delete(cRec) Then
    '            RefreshTempO19()
    '        End If
    '    End If
    'End Sub

    'Private Sub j23ids_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles j23ids.ItemDataBound
    '    Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
    '    CType(e.Item.FindControl("Source"), Label).Text = cRec.p85FreeText01
    '    With CType(e.Item.FindControl("cmdDelete"), ImageButton)
    '        .CommandArgument = cRec.PID.ToString
    '    End With
    'End Sub

    Private Sub cbxSelectJ11ID_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles cbxSelectJ11ID.ItemDataBound
        Dim cRec As BO.j11Team = CType(e.Item.DataItem, BO.j11Team)
        If cRec.j11IsAllPersons Then e.Item.Text = "Všichni"

    End Sub

    Private Sub cbxSelectJ11ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles cbxSelectJ11ID.SelectedIndexChanged
        Dim intJ11ID As Integer = BO.BAS.IsNullInt(Me.cbxSelectJ11ID.SelectedValue)
        If intJ11ID = 0 Then Return
        If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o20")).Where(Function(p) p.p85OtherKey2 = intJ11ID).Count > 0 Then
            Return
        End If
        CreateTempRecord_o20(0, "", intJ11ID, cbxSelectJ11ID.Text)
        'Dim c As New BO.p85TempBox()
        'c.p85GUID = ViewState("guid_o20")
        'c.p85OtherKey2 = intJ11ID
        'c.p85FreeText01 = cbxSelectJ11ID.Text
        'Master.Factory.p85TempBoxBL.Save(c)
        RefreshTempO20()
        Me.cbxSelectJ11ID.SelectedIndex = 0
    End Sub
    Private Sub RefreshTempO20()
        Me.rpO20.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o20"))
        Me.rpO20.DataBind()
    End Sub

    Private Sub rpO20_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpO20.ItemCommand
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempO20()
            End If
        End If
    End Sub

    Private Sub rpO20_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO20.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With CType(e.Item.FindControl("img1"), Image)
            If cRec.p85OtherKey1 <> 0 Then
                .ImageUrl = "Images/person.png"
            Else
                .ImageUrl = "Images/team.png"
            End If
        End With
        
        CType(e.Item.FindControl("Source"), Label).Text = cRec.p85FreeText01
        With CType(e.Item.FindControl("cmdDelete"), ImageButton)
            .CommandArgument = cRec.PID.ToString
        End With
    End Sub

    Private Sub j02ID_Search_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles j02ID_Search.AutoPostBack_SelectedIndexChanged
        Dim intJ02ID As Integer = BO.BAS.IsNullInt(Me.j02ID_Search.Value)
        If intJ02ID = 0 Then Return
        If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o20")).Where(Function(p) p.p85OtherKey1 = intJ02ID).Count > 0 Then
            Return
        End If
        CreateTempRecord_o20(intJ02ID, Me.j02ID_Search.Text, 0, "")
        'Dim c As New BO.p85TempBox()
        'c.p85GUID = ViewState("guid_o20")
        'c.p85OtherKey1 = intJ02ID
        'c.p85FreeText01 = Me.j02ID_Search.Text
        'Master.Factory.p85TempBoxBL.Save(c)
        RefreshTempO20()
        Me.j02ID_Search.Value = ""
        Me.j02ID_Search.Text = ""
    End Sub

    Private Sub CreateTempRecord_o20(intJ02ID As Integer, strPerson As String, intJ11ID As Integer, strJ11Name As String)
        Dim c As New BO.p85TempBox()
        c.p85GUID = ViewState("guid_o20")
        If intJ02ID <> 0 Then
            c.p85OtherKey1 = intJ02ID
            c.p85FreeText01 = strPerson
        End If
        If intJ11ID <> 0 Then
            c.p85OtherKey2 = intJ11ID
            c.p85FreeText01 = strJ11Name
        End If
        Master.Factory.p85TempBoxBL.Save(c)
    End Sub
    'Private Sub CreateTempRecord_o19(intJ23ID As Integer, strJ23Name As String)
    '    Dim c As New BO.p85TempBox()
    '    c.p85GUID = ViewState("guid_o19")
    '    c.p85OtherKey1 = intJ23ID
    '    c.p85FreeText01 = strJ23Name
    '    Master.Factory.p85TempBoxBL.Save(c)
    'End Sub

    Private Sub Handle_FF()
        Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.o22Milestone, Master.DataPID, BO.BAS.IsNullInt(Me.o21ID.SelectedValue))
        Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.o22Milestone, BO.BAS.IsNullInt(Me.o21ID.SelectedValue))
        ff1.FillData(fields, lisX20X18, "o22Milestone_FreeField", Master.DataPID)

    End Sub
End Class