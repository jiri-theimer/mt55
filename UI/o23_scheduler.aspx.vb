Imports Telerik.Web.UI

Public Class o23_scheduler
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _curD1 As Date
    Private Property _curD2 As Date

    Public ReadOnly Property CurrentX18ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.x18ID.SelectedValue)
        End Get
    End Property
    Public Property CurrentView As SchedulerViewType
        Get
            Return Me.scheduler1.SelectedView
        End Get
        Set(value As SchedulerViewType)
            Me.scheduler1.SelectedView = value
        End Set
    End Property

    Private Sub o23_scheduler_Init(sender As Object, e As EventArgs) Handles Me.Init
        persons1.Factory = Master.Factory
        projects1.Factory = Master.Factory
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "o23_scheduler"
                ViewState("loading_setting") = "0"

                Dim strX18ID As String = Request.Item("x18id")
                With Master.Factory.j03UserBL
                    If strX18ID = "" Then
                        .InhaleUserParams("o23_framework-x18id")
                        strX18ID = .GetUserParam("o23_framework-x18id")
                    End If
                End With
                SetupX18Combo(strX18ID)
                If Me.x18ID.Items.Count > 0 Then
                    strX18ID = Me.x18ID.SelectedValue
                    Handle_ChangeX18ID()
                Else
                    .StopPage("Zatím nebyl nastaven typ dokumentu s podporou kalendářového rozhraní.", False)
                    Return
                End If

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("o23_framework-x18id")
                    .Add("entity_scheduler-view-" & strX18ID)
                    .Add("entity_scheduler-daystarttime")
                    .Add("entity_scheduler-dayendtime")
                    .Add("entity_scheduler-multidays")
                    .Add("entity_scheduler-persons1-scope")
                    .Add("entity_scheduler-persons1-value")
                    .Add("entity_scheduler-persons1-personsrole")
                    .Add("entity_scheduler-projects1-scope")
                    .Add("entity_scheduler-projects1-value")
                    .Add("entity_scheduler-agendadays")
                    .Add("entity_scheduler-timelinedays")
                    .Add("entity_scheduler-include_childs")
                    .Add("entity_scheduler-resourceview-" & strX18ID)
                    .Add("o23_framework-filter_b02id-" & strX18ID)
                    .Add("o23_framework-filter_myrole-" & strX18ID)
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Me.CurrentView = .GetUserParam("entity_scheduler-view-" & strX18ID, "1")

                    Me.persons1.CurrentScope = .GetUserParam("entity_scheduler-persons1-scope", "4")
                    Me.persons1.CurrentValue = .GetUserParam("entity_scheduler-persons1-value")

                    SetupPersonRolesCombo(.GetUserParam("entity_scheduler-persons1-personsrole"))
                    Me.persons1.CurrentPersonsRole = .GetUserParam("entity_scheduler-persons1-personsrole", "1")

                    Me.projects1.CurrentScope = .GetUserParam("entity_scheduler-projects1-scope", "1")
                    Me.projects1.CurrentValue = .GetUserParam("entity_scheduler-projects1-value")



                    basUI.SelectDropdownlistValue(Me.entity_scheduler_daystarttime, .GetUserParam("entity_scheduler-daystarttime", "8"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_dayendtime, .GetUserParam("entity_scheduler-dayendtime", "20"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_multidays, .GetUserParam("entity_scheduler-multidays", "2"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_agendadays, .GetUserParam("entity_scheduler-agendadays", "20"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_timelinedays, .GetUserParam("entity_scheduler-timelinedays", "10"))
                    basUI.SelectDropdownlistValue(Me.cbxResourceView, .GetUserParam("entity_scheduler-resourceview-" & strX18ID, "1"))
                    If cbxQueryB02ID.Visible Then
                        basUI.SelectDropdownlistValue(Me.cbxQueryB02ID, .GetUserParam("o23_framework-filter_b02id-" & strX18ID))

                    End If
                    basUI.SelectDropdownlistValue(Me.cbxMyRole, .GetUserParam("o23_framework-filter_myrole-" & strX18ID))
                End With
            End With


            RefreshData(False)
        End If
    End Sub

    Private Sub SetupX18Combo(strDef As String)
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_X18_Admin, BO.x53PermValEnum.GR_Admin) Then
            mq.MyRecordsDisponible = True
        End If

        Dim lis As IEnumerable(Of BO.x18EntityCategory) = Master.Factory.x18EntityCategoryBL.GetList(mq).Where(Function(p) p.x18IsCalendar = True)
        Me.x18ID.DataSource = lis
        Me.x18ID.DataBind()
        If lis.Count = 0 Then
            Master.Notify("V databázi zatím neexistuje typ dokumentu pro kalendářové rozhraní.", NotifyLevel.InfoMessage)
        Else
            If strDef <> "" Then basUI.SelectDropdownlistValue(Me.x18ID, strDef)
        End If

    End Sub

    Private Sub SetupPersonRolesCombo(strDef As String)
        Dim lis As New List(Of BO.ComboSource)
        Dim c As New BO.ComboSource
        c.pid = -1 : c.ItemText = "Zakladatel záznamu"
        lis.Add(c)
        c = New BO.ComboSource
        c.pid = 1
        c.ItemText = "Nominovaný (schvalovatel/řešitel)"
        lis.Add(c)
        persons1.SetupQueryPersonsRoles(lis)
        persons1.CurrentPersonsRole = strDef
    End Sub



    Private Sub scheduler1_NavigationComplete(sender As Object, e As SchedulerNavigationCompleteEventArgs) Handles scheduler1.NavigationComplete
        Dim bolChangeView As Boolean = False
        Select Case e.Command
            Case SchedulerNavigationCommand.SwitchToAgendaView, SchedulerNavigationCommand.SwitchToMonthView, SchedulerNavigationCommand.SwitchToTimelineView, SchedulerNavigationCommand.SwitchToWeekView, SchedulerNavigationCommand.SwitchToMultiDayView
                bolChangeView = True
        End Select
        If bolChangeView Then
            Master.Factory.j03UserBL.SetUserParam("entity_scheduler-view-" & Me.CurrentX18ID.ToString, CInt(Me.CurrentView).ToString)
        End If
        RefreshData(False)
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("o23_scheduler.aspx?x18id=" & Me.CurrentX18ID.ToString)

    End Sub

    Private Sub Handle_ChangeX18ID()
        Dim c As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(Me.CurrentX18ID)
        hidX23ID.Value = c.x23ID.ToString

        hidx18IsColors.Value = BO.BAS.GB(c.x18IsColors)
        hidCalendarFieldStart.Value = c.x18CalendarFieldStart
        hidCalendarFieldEnd.Value = c.x18CalendarFieldEnd
        hidCalendarFieldSubject.Value = c.x18CalendarFieldSubject
        hidx18CalendarResourceField.Value = c.x18CalendarResourceField

        If c.b01ID <> 0 Then
            hidB01ID.Value = c.b01ID.ToString
            cbxQueryB02ID.Visible = True
            cbxQueryB02ID.DataSource = Master.Factory.b02WorkflowStatusBL.GetList(c.b01ID)
            cbxQueryB02ID.DataBind()
            cbxQueryB02ID.Items.Insert(0, New ListItem("--Filtrovat aktuální stav--", ""))
            
            
        Else
            hidB01ID.Value = ""
        End If

        Dim cDisp As BO.x18RecordDisposition = Master.Factory.x18EntityCategoryBL.InhaleDisposition(c)
        ''menu1.FindItemByValue("cmdNew").Visible = cDisp.CreateItem

        Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(Me.CurrentX18ID).Where(Function(p) p.x20IsClosed = False And (p.x29ID = 102 Or p.x29ID = 141))
        If lisX20X18.Where(Function(p) p.x29ID = 102).Count > 0 Then
            panPersons.Style.Item("display") = "block"
        End If
        If lisX20X18.Where(Function(p) p.x29ID = 141).Count > 0 Then
            panProjects.Style.Item("display") = "block"
        End If

    End Sub

    Private Sub RefreshData(bolData4Export As Boolean)
        Dim bolResources As Boolean = False
        If Me.hidx18CalendarResourceField.Value <> "" And cbxResourceView.SelectedValue <> "" And scheduler1.SelectedView = SchedulerViewType.TimelineView Then bolResources = True

        With Me.scheduler1
            .Appointments.Clear()
            .DayView.DayStartTime = System.TimeSpan.FromHours(CDbl(Me.entity_scheduler_daystarttime.SelectedValue))
            .DayView.DayEndTime = System.TimeSpan.FromHours(CDbl(Me.entity_scheduler_dayendtime.SelectedValue))
            .WeekView.DayStartTime = .DayView.DayStartTime
            .WeekView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.DayStartTime = .DayView.DayStartTime
            .MultiDayView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.NumberOfDays = BO.BAS.IsNullInt(Me.entity_scheduler_multidays.SelectedValue)
            .Localization.HeaderMultiDay = "Multi-den (" & .MultiDayView.NumberOfDays.ToString & ")"
            .AgendaView.NumberOfDays = BO.BAS.IsNullInt(Me.entity_scheduler_agendadays.SelectedValue)
            .TimelineView.NumberOfSlots = BO.BAS.IsNullInt(Me.entity_scheduler_timelinedays.SelectedValue)
            If bolResources Then
                .TimelineView.GroupBy = "resource1"
            Else
                .TimelineView.GroupBy = ""
            End If

        End With
        Dim d1 As Date = scheduler1.VisibleRangeStart.AddDays(-1), d2 As Date = scheduler1.VisibleRangeEnd.AddDays(1)

        Dim mq As New BO.myQueryO23(BO.BAS.IsNullInt(hidX23ID.Value))
        InhaleMyQuery(mq, d1, d2)



        Master.Factory.o23DocBL.SetCalendarDateFields(hidCalendarFieldStart.Value, hidCalendarFieldEnd.Value)
        Dim lis As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList(mq)

        Dim lisItems As New List(Of BO.SchedulerItem), res_foud As New List(Of Integer)


        Dim lisX20 As IEnumerable(Of BO.x20EntiyToCategory) = Nothing, x20ids As List(Of Integer) = Nothing, lisX19 As IEnumerable(Of BO.x19EntityCategory_Binding) = Nothing
        Select Case hidCalendarFieldSubject.Value
            Case "j02_alias", "p41_alias", "p28_alias"
                lisX20 = Master.Factory.x18EntityCategoryBL.GetList_x20(Me.CurrentX18ID)
                Select Case hidCalendarFieldSubject.Value
                    Case "j02_alias"
                        lisX20 = lisX20.Where(Function(p) p.x29ID = 102)
                    Case "p41_alias"
                        lisX20 = lisX20.Where(Function(p) p.x29ID = 141)
                    Case "p28_alias"
                        lisX20 = lisX20.Where(Function(p) p.x29ID = 328)
                End Select
                x20ids = lisX20.Select(Function(p) p.x20ID).ToList
                lisX19 = Master.Factory.x18EntityCategoryBL.GetList_X19(x20ids, True)
        End Select


        For Each cRec In lis
            Dim c As New BO.SchedulerItem

            With cRec
                c.ID = .PID.ToString & ",'o23'"
                c.Description = "clue_o23_record.aspx?pid=" & .PID.ToString

                Select Case hidCalendarFieldSubject.Value
                    Case "o23Name"
                        c.Subject = .o23Name
                    Case "o23Code"
                        c.Subject = .o23Code
                    Case Else
                        If Not lisX19 Is Nothing Then
                            Dim cX19 As BO.x19EntityCategory_Binding = lisX19.First(Function(p) p.o23ID = cRec.PID)
                            If Not cX19 Is Nothing Then
                                c.Subject = cX19.RecordAlias
                                If bolResources Then
                                    Dim res As New Resource("resource1", cX19.x19RecordPID, cX19.RecordAlias)
                                    c.ResourceID = cX19.x19RecordPID
                                    c.ResourceName = cX19.RecordAlias
                                    res_foud.Add(cX19.x19RecordPID)
                                End If



                            End If
                        End If
                End Select


                c.DateStart = .CalendarDateStart
                c.DateEnd = .CalendarDateEnd

                If c.DateEnd > c.DateStart Then
                    If Month(c.DateStart) = Month(c.DateEnd) Then
                        c.Subject += " " & Format(c.DateStart, "d.") & "-" & Format(c.DateEnd, "d.M.")
                    Else
                        c.Subject += " " & Format(c.DateStart, "d.M.") & "-" & Format(c.DateEnd, "d.M.")
                    End If
                Else
                    c.Subject += " " & Format(c.DateEnd, "d.M.")
                End If

                If .b02ID <> 0 Then
                    If .b02Color <> "" Then
                        c.BackgroundColorString = .b02Color
                    End If
                Else
                    If .o23BackColor <> "" Then
                        c.BackgroundColorString = .o23BackColor

                        If .o23ForeColor <> "" Then
                            c.ForeColorString = .o23ForeColor
                        End If
                    End If
                End If


                If (c.DateEnd.Hour = 23 And c.DateEnd.Minute = 59) Or (c.DateEnd.Hour = 0 And c.DateEnd.Minute = 0 And c.DateEnd.Second = 0) Then
                    c.DateStart = DateSerial(Year(c.DateStart), Month(c.DateStart), Day(c.DateStart))
                    c.DateEnd = DateSerial(Year(c.DateEnd), Month(c.DateEnd), Day(c.DateEnd)).AddDays(1)
                End If

                Dim intLeft As Integer = 20
                Dim lngDiff As Long = DateDiff(DateInterval.Day, c.DateStart, c.DateEnd, Microsoft.VisualBasic.FirstDayOfWeek.Monday, FirstWeekOfYear.System)
                Select Case Me.CurrentView
                    Case SchedulerViewType.MonthView, SchedulerViewType.WeekView
                        If lngDiff >= 1 Then intLeft = 50
                        If lngDiff >= 2 Then intLeft = 90
                    Case SchedulerViewType.TimelineView
                        If lngDiff >= 1 Then intLeft = 35
                        If lngDiff >= 2 Then intLeft = 50
                    Case Else
                        intLeft = 100
                End Select
                If Len(c.Subject) >= intLeft Then
                    c.Tooltip = c.Subject
                    c.Subject = Left(c.Subject, intLeft) & "..."

                End If

            End With

            lisItems.Add(c)

        Next
        If bolResources Then
            Dim mqJ02 As New BO.myQueryJ02
            If Me.cbxResourceView.SelectedValue = "1" Then
                If res_foud.Count = 0 Then res_foud.Add(-1)
                mqJ02.PIDs = res_foud

            End If

            Dim lisJ02I As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mqJ02)
            For Each c In lisJ02I
                scheduler1.Resources.Add(New Resource("resource1", c.PID, c.FullNameDesc))
            Next
        End If


        scheduler1.DataSource = lisItems

    End Sub

    Private Sub persons1_OnChange() Handles persons1.OnChange
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-scope", persons1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-value", persons1.CurrentValue)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-personsrole", persons1.CurrentPersonsRole)

        RefreshData(False)
        hidIsPersonsChange.Value = "1"
    End Sub
    Private Sub projects1_OnChange() Handles projects1.OnChange
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-projects1-scope", projects1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-projects1-value", projects1.CurrentValue)
        RefreshData(False)
        hidIsProjectsChange.Value = "1"
    End Sub

    Private Sub o23_scheduler_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        PersonsHeader.Text = persons1.CurrentHeader
        ProjectsHeader.Text = projects1.CurrentHeader
        With scheduler1.TimelineView
            If .NumberOfSlots <= 10 Then
                .ColumnHeaderDateFormat = "ddd d.M.yyyy"
            Else
                .ColumnHeaderDateFormat = "ddd d.M."
            End If
            If .NumberOfSlots >= 30 Then
                .ColumnHeaderDateFormat = "ddd d.M."
            End If
            If .NumberOfSlots >= 50 Then
                .ColumnHeaderDateFormat = "d.M."
            End If

        End With
        If scheduler1.SelectedView = SchedulerViewType.TimelineView And Me.hidx18CalendarResourceField.Value <> "" Then
            Me.panResources.Visible = True
        Else
            Me.panResources.Visible = False
        End If
        If scheduler1.SelectedView = SchedulerViewType.TimelineView Then
            scheduler1.TimeSlotContextMenus(0).FindItemByValue("o23").NavigateUrl = "javascript:o23_record(0)"
        Else
            scheduler1.TimeSlotContextMenus(0).FindItemByValue("o23").NavigateUrl = ""
        End If
        If cbxQueryB02ID.Visible Then
            basUIMT.RenderQueryCombo(Me.cbxQueryB02ID)

        End If
        basUIMT.RenderQueryCombo(Me.cbxMyRole)
    End Sub


    Private Sub entity_scheduler_daystarttime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_daystarttime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-daystarttime", Me.entity_scheduler_daystarttime.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub
    Private Sub entity_scheduler_agendadays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_agendadays.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-agendadays", Me.entity_scheduler_agendadays.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub
    Private Sub entity_scheduler_dayendtime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_dayendtime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-dayendtime", Me.entity_scheduler_dayendtime.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub
    Private Sub entity_scheduler_timelinedays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_timelinedays.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-timelinedays", Me.entity_scheduler_timelinedays.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub scheduler1_AppointmentDataBound(sender As Object, e As SchedulerEventArgs) Handles scheduler1.AppointmentDataBound
        Dim cRec As BO.SchedulerItem = CType(e.Appointment.DataItem, BO.SchedulerItem)
        With cRec
            If .BackgroundColorString = "" Then e.Appointment.BackColor = Drawing.Color.Khaki Else e.Appointment.BackColor = Drawing.Color.FromName(.BackgroundColorString)
            If .ForeColorString = "" Then e.Appointment.ForeColor = Drawing.Color.Black Else e.Appointment.ForeColor = Drawing.Color.FromName(.ForeColorString)
            e.Appointment.ToolTip = .Tooltip
        End With
    End Sub

    Private Sub cbxResourceView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxResourceView.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-resourceview-" & Me.CurrentX18ID.ToString, cbxResourceView.SelectedValue)

        RefreshData(False)

    End Sub

    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryO23, d1 As Date, d2 As Date)
        With mq
            .Closed = BO.BooleanQueryMode.NoQuery
            .x23ID = BO.BAS.IsNullInt(hidX23ID.Value)
            If cbxQueryB02ID.Visible Then
                If cbxQueryB02ID.SelectedIndex > 0 Then .b02IDs = BO.BAS.ConvertPIDs2List(cbxQueryB02ID.SelectedValue)
            End If
            Select Case Me.cbxMyRole.SelectedValue
                Case "1"
                    .Owners = BO.BAS.ConvertInt2List(Master.Factory.SysUser.j02ID)
                Case "2"
                    'řešitel
                    .HasAnyX67Role = BO.BooleanQueryMode.TrueQuery
                Case "3"
                    'podřízení
                    .OnlySlavesPersons = BO.BooleanQueryMode.TrueQuery
            End Select

            .MyRecordsDisponible = True

            If panPersons.Visible Then
                If persons1.CurrentPersonsRole = "-1" Then
                    .Owners = persons1.CurrentJ02IDs
                Else
                    .j02IDs = persons1.CurrentJ02IDs
                End If
            End If
            If panProjects.Visible Then
                .p41IDs = projects1.CurrentP41IDs
            End If


            .CalendarDateFieldStart = hidCalendarFieldStart.Value
            .CalendarDateFieldEnd = hidCalendarFieldEnd.Value
            .DateFrom = d1
            .DateUntil = d2



        End With

    End Sub
    Private Sub cbxQueryB02ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryB02ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o23_framework-filter_b02id-" & Me.CurrentX18ID.ToString, Me.cbxQueryB02ID.SelectedValue)
        RefreshData(False)

    End Sub

    Private Sub cbxMyRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxMyRole.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("o23_framework-filter_myrole-" & Me.CurrentX18ID.ToString, Me.cbxMyRole.SelectedValue)
        RefreshData(False)

    End Sub
End Class