Imports Telerik.Web.UI

Public Class entity_scheduler
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _curD1 As Date
    Private Property _curD2 As Date

    Public Property CurrentView As SchedulerViewType
        Get
            Return Me.scheduler1.SelectedView
        End Get
        Set(value As SchedulerViewType)
            Me.scheduler1.SelectedView = value
        End Set
    End Property
    
    Public Property CurrentMasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property
    Public ReadOnly Property CurrentMasterX29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(Me.hidMasterPrefix.Value)
        End Get
    End Property
    Public Property CurrentMasterPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterPID.Value)
        End Get
        Set(value As Integer)
            hidMasterPID.Value = value.ToString
        End Set
    End Property

    Private Sub entity_scheduler_Init(sender As Object, e As EventArgs) Handles Me.Init
        persons1.Factory = Master.Factory
        projects1.Factory = Master.Factory
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "entity_scheduler"
                If Request.Item("masterpid") <> "" Then
                    Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid")) : Me.CurrentMasterPrefix = Request.Item("masterprefix")
                End If
                ViewState("loading_setting") = "0"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("entity_scheduler-view")
                    .Add("entity_scheduler-daystarttime")
                    .Add("entity_scheduler-dayendtime")
                    .Add("entity_scheduler-multidays")
                    .Add("entity_scheduler-j70id")
                    .Add("entity_scheduler-persons1-scope")
                    .Add("entity_scheduler-persons1-value")
                    .Add("entity_scheduler-persons1-personsrole")
                    .Add("entity_scheduler-projects1-scope")
                    .Add("entity_scheduler-projects1-value")
                    .Add("entity_scheduler-o22")
                    .Add("entity_scheduler-p48")
                    .Add("entity_scheduler-p56")
                    .Add("entity_scheduler-newrec_prefix")
                    .Add("entity_scheduler-agendadays")
                    .Add("entity_scheduler-include_childs")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Me.CurrentView = .GetUserParam("entity_scheduler-view", "1")
                    If Me.CurrentMasterPrefix = "" Then
                        Me.persons1.CurrentScope = .GetUserParam("entity_scheduler-persons1-scope", "4")
                        Me.persons1.CurrentValue = .GetUserParam("entity_scheduler-persons1-value")

                        SetupPersonRolesCombo(.GetUserParam("entity_scheduler-persons1-personsrole"))
                        Me.persons1.CurrentPersonsRole = .GetUserParam("entity_scheduler-persons1-personsrole", "1")

                        Me.projects1.CurrentScope = .GetUserParam("entity_scheduler-projects1-scope", "1")
                        Me.projects1.CurrentValue = .GetUserParam("entity_scheduler-projects1-value")
                    End If
                    

                    SetupJ70Combo(BO.BAS.IsNullInt(.GetUserParam("entity_scheduler-j70id")))

                    basUI.SelectDropdownlistValue(Me.entity_scheduler_daystarttime, .GetUserParam("entity_scheduler-daystarttime", "8"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_dayendtime, .GetUserParam("entity_scheduler-dayendtime", "20"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_multidays, .GetUserParam("entity_scheduler-multidays", "2"))
                    basUI.SelectDropdownlistValue(Me.cbxNewRecType, .GetUserParam("entity_scheduler-newrec_prefix", "p56"))
                    basUI.SelectDropdownlistValue(Me.entity_scheduler_agendadays, .GetUserParam("entity_scheduler-agendadays", "20"))

                    Select Case Request.Item("scope")
                        Case "scheduler", ""
                            chkSetting_O22.Checked = True : chkSetting_P56.Checked = True
                            panSettingOPlan.Visible = True
                            Me.chkSetting_P48.Checked = .GetUserParam("entity_scheduler-p48", "0")
                        Case "oplan"
                            chkSetting_P48.Checked = True
                            panSettingOPlan.Visible = False
                            Me.j70ID.Visible = False
                            Me.j70ID.SelectedIndex = 0
                            Me.clue_query.Visible = False
                            Me.cmdQuery.Visible = False
                        Case Else

                    End Select
                    
                    Me.chkIncludeChilds.Checked = BO.BAS.BG(.GetUserParam("entity_scheduler-include_childs"))

                End With
            End With
          

            RefreshRecord()
            RefreshData(False)
        End If
    End Sub

    Private Sub SetupPersonRolesCombo(strDef As String)
        Dim lis As New List(Of BO.ComboSource)
        Dim c As New BO.ComboSource
        c.pid = -1 : c.ItemText = "Zadavatel úkolu"
        lis.Add(c)
        c = New BO.ComboSource
        c.pid = 1
        c.ItemText = "Řešitel (příjemce) úkolu"
        lis.Add(c)
        persons1.SetupQueryPersonsRoles(lis)
        persons1.CurrentPersonsRole = strDef
    End Sub
    
    Private Sub RefreshData(bolData4Export As Boolean)
        With Me.scheduler1
            .Appointments.Clear()
            .DayView.DayStartTime = System.TimeSpan.FromHours(CDbl(Me.entity_scheduler_daystarttime.SelectedValue))
            .DayView.DayEndTime = System.TimeSpan.FromHours(CDbl(Me.entity_scheduler_dayendtime.SelectedValue))
            .WeekView.DayStartTime = .DayView.DayStartTime
            .WeekView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.DayStartTime = .DayView.DayStartTime
            .MultiDayView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.NumberOfDays = BO.BAS.IsNullInt(Me.entity_scheduler_multidays.SelectedValue)
            ''.Localization.HeaderMultiDay = "Multi-den (" & .MultiDayView.NumberOfDays.ToString & ")"
            .AgendaView.NumberOfDays = BO.BAS.IsNullInt(Me.entity_scheduler_agendadays.SelectedValue)
           
        End With
        Dim d1 As Date = scheduler1.VisibleRangeStart.AddDays(-1), d2 As Date = scheduler1.VisibleRangeEnd.AddDays(1)
        

        If Me.chkSetting_O22.Checked Then
            Dim mq As New BO.myQueryO22
            'mq.SpecificQuery = BO.myQueryO22_SpecificQuery.AllowedForRead
            Select Case Me.CurrentMasterX29ID
                Case BO.x29IdEnum.p28Contact
                    mq.p28ID = Me.CurrentMasterPID
                Case BO.x29IdEnum.p41Project
                    mq.p41ID = Me.CurrentMasterPID
                    mq.IsIncludeChildProjects = Me.chkIncludeChilds.Checked
                Case BO.x29IdEnum.j02Person
                    mq.j02IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
                Case Else
                    mq.j02IDs = persons1.CurrentJ02IDs
                    mq.p41IDs = projects1.CurrentP41IDs


            End Select
            mq.DateFrom = d1 : mq.DateUntil = d2
            Dim lis As IEnumerable(Of BO.o22Milestone) = Master.Factory.o22MilestoneBL.GetList(mq)
            For Each cRec In lis
                Dim c As New Appointment()
                With cRec
                    c.ID = .PID.ToString & ",'o22'"
                    c.Description = "clue_o22_record.aspx?pid=" & .PID.ToString
                    c.Subject = .o22Name
                    ''If .o22DateUntil < datFoundMin Then datFoundMin = .o22DateUntil
                    Select Case .o21Flag
                        Case BO.o21FlagEnum.DeadlineOrMilestone
                            'c.BackColor = Drawing.Color.Aquamarine
                            c.BackColor = Drawing.Color.Salmon
                            c.Start = .o22DateUntil.Value
                            c.End = .o22DateUntil.Value


                        Case BO.o21FlagEnum.EventFromUntil
                            c.BackColor = Drawing.Color.AntiqueWhite
                            If Not .o22DateFrom Is Nothing Then
                                c.Start = .o22DateFrom
                            Else
                                c.Start = .o22DateUntil
                            End If
                            c.End = .o22DateUntil

                            If .o22IsAllDay Then
                                c.Start = DateSerial(Year(c.Start), Month(c.Start), Day(c.Start))
                                c.End = c.End.AddDays(1)

                            End If

                    End Select
                    If c.End.Hour = 0 And c.End.Minute = 0 And c.End.Second = 0 Then    'nastavit jako celo-denní událost bez času od-do
                        c.Start = DateSerial(Year(c.Start), Month(c.Start), Day(c.Start))
                        c.End = DateSerial(Year(c.End), Month(c.End), Day(c.End)).AddDays(1)
                    End If
                    c.BorderColor = Drawing.Color.Gray
                    c.BorderStyle = BorderStyle.Dashed

                    Select Case Me.CurrentView
                        Case SchedulerViewType.MonthView, SchedulerViewType.TimelineView, SchedulerViewType.WeekView
                            If Len(.o22Name) > 0 Then c.Subject = BO.BAS.OM3(.o22Name, 25)

                            c.ToolTip = BO.BAS.FD(.o22DateUntil, True)
                    End Select
                End With

                scheduler1.InsertAppointment(c)
            Next
        End If
        If Me.chkSetting_P48.Checked Then
            Dim mq As New BO.myQueryP48
            Select Case Me.CurrentMasterX29ID
                Case BO.x29IdEnum.p28Contact
                    mq.p28ID = Me.CurrentMasterPID
                Case BO.x29IdEnum.p41Project
                    mq.p41ID = Me.CurrentMasterPID
                    mq.IsIncludeChildProjects = Me.chkIncludeChilds.Checked
                Case BO.x29IdEnum.j02Person
                    mq.j02IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
                Case Else
                    mq.j02IDs = persons1.CurrentJ02IDs
                    mq.p41IDs = projects1.CurrentP41IDs
            End Select
            mq.DateFrom = d1 : mq.DateUntil = d2
            Dim lis As IEnumerable(Of BO.p48OperativePlan) = Master.Factory.p48OperativePlanBL.GetList(mq)
            For Each cRec In lis
                Dim c As New Appointment()
                With cRec
                    c.ID = .PID.ToString & ",'p48'"
                    c.Description = "clue_p48_record.aspx?pid=" & .PID.ToString & "&js_edit=p48_record(" & .PID.ToString & ")&js_convert=p48_convert(" & .PID.ToString & ")"

                    If Not .p48DateTimeFrom Is Nothing Then
                        c.Start = .p48DateTimeFrom
                    Else
                        c.Start = .p48Date
                    End If
                    If Not .p48DateTimeUntil Is Nothing Then
                        c.End = .p48DateTimeUntil
                    Else
                        c.End = .p48Date.AddDays(1)
                    End If
                    If .p31ID > 0 Then
                        c.Font.Strikeout = True 'plán byl zkonvertován do worksheetu
                        c.Description += "&js_p31record=p31_record(" & .p31ID.ToString & ")"
                    End If
                    c.BorderColor = Drawing.Color.Silver
                    'c.BorderStyle = BorderStyle.Dotted
                    c.BackColor = Drawing.Color.WhiteSmoke
                    c.ForeColor = Drawing.Color.Black
                    c.Subject = .p48Hours.ToString & "h."
                    Select Case Me.CurrentMasterX29ID
                        Case BO.x29IdEnum.p41Project
                            c.Subject += " " & .Person
                        Case BO.x29IdEnum.p28Contact
                            c.Subject += " " & .Person & ": " & .Project
                        Case BO.x29IdEnum.j02Person
                            c.Subject += " " & .ClientAndProject
                        Case Else
                            If persons1.CurrentJ02IDs.Count = 1 Then
                                c.Subject += " " & .ClientAndProject
                            Else
                                c.Subject += " " & .Person & ": " & .ClientAndProject
                            End If

                    End Select

                    Select Case Me.CurrentView
                        Case SchedulerViewType.MonthView, SchedulerViewType.TimelineView
                            c.ToolTip = c.Subject
                            If Len(c.Subject) > 22 Then
                                c.Subject = Left(c.Subject, 20) & "..."
                            End If
                        Case Else

                    End Select
                End With

                scheduler1.InsertAppointment(c)
            Next
        End If
        If chkSetting_P56.Checked Then  'termíny úkolů
            Dim mq As New BO.myQueryP56

            Select Case Me.CurrentMasterX29ID
                Case BO.x29IdEnum.p28Contact
                    mq.p28ID = Me.CurrentMasterPID
                Case BO.x29IdEnum.p41Project
                    mq.p41ID = Me.CurrentMasterPID
                    mq.IsIncludeChildProjects = Me.chkIncludeChilds.Checked
                Case BO.x29IdEnum.j02Person
                    mq.j02ID = Me.CurrentMasterPID
                Case Else
                    mq.j70ID = BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
                    If persons1.CurrentPersonsRole = "-1" Then
                        mq.Owners = persons1.CurrentJ02IDs
                    Else
                        mq.j02IDs = persons1.CurrentJ02IDs
                    End If
                    mq.p41IDs = projects1.CurrentP41IDs
            End Select
            mq.p56PlanUntil_D1 = d1 : mq.p56PlanUntil_D2 = d2
            Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq)
            For Each cRec In lis
                Dim c As New Appointment()
                With cRec
                    c.ID = .PID.ToString & ",'p56'"
                    c.Description = "clue_p56_record.aspx?pid=" & .PID.ToString
                    c.Subject = .p56Name

                    If .p57PlanDatesEntryFlag = 4 And Not .p56PlanFrom Is Nothing Then
                        c.Start = .p56PlanFrom
                    Else
                        c.Start = .p56PlanUntil
                    End If
                    c.End = .p56PlanUntil

                    c.BackColor = Drawing.Color.FromName("#3CB371")

                    If (c.End.Hour = 23 And c.End.Minute = 59) Or (c.End.Hour = 0 And c.End.Minute = 0 And c.End.Second = 0) Then
                        c.Start = DateSerial(Year(c.Start), Month(c.Start), Day(c.Start))
                        c.End = DateSerial(Year(c.End), Month(c.End), Day(c.End)).AddDays(1)
                    End If

                    Select Case Me.CurrentView
                        Case SchedulerViewType.MonthView, SchedulerViewType.TimelineView, SchedulerViewType.WeekView
                            If Len(.p56Name) > 0 Then c.Subject = BO.BAS.OM3(.p56Name, 15)


                    End Select
                End With

                scheduler1.InsertAppointment(c)
            Next
        End If
    End Sub

    Private Sub RefreshRecord()
        If Me.CurrentMasterPrefix <> "" Then
            Me.MasterRecord.NavigateUrl = Me.CurrentMasterPrefix & "_framework.aspx?pid" & Me.CurrentMasterPID.ToString
            With Me.MasterRecord
                .Text = Master.Factory.GetRecordCaption(Me.CurrentMasterX29ID, Me.CurrentMasterPID)
                If .Text.Length > 37 Then .Text = Left(.Text, 35) & "..."
            End With



            Select Case Me.CurrentMasterPrefix
                Case "p41"
                    imgMaster.ImageUrl = "Images/project.png"
                    chkIncludeChilds.Visible = True
                Case "p28"
                    imgMaster.ImageUrl = "Images/contact.png"
                Case "j02"
                    imgMaster.ImageUrl = "Images/person.png"

            End Select

        Else

            panMasterRecord.Visible = False
        End If

    End Sub
    

    Private Sub scheduler1_NavigationComplete(sender As Object, e As SchedulerNavigationCompleteEventArgs) Handles scheduler1.NavigationComplete
        Dim bolChangeView As Boolean = False
        Select Case e.Command
            Case SchedulerNavigationCommand.SwitchToAgendaView, SchedulerNavigationCommand.SwitchToMonthView, SchedulerNavigationCommand.SwitchToTimelineView, SchedulerNavigationCommand.SwitchToWeekView, SchedulerNavigationCommand.SwitchToMultiDayView
                bolChangeView = True
        End Select
        If bolChangeView Then
            Master.Factory.j03UserBL.SetUserParam("entity_scheduler-view", CInt(Me.CurrentView).ToString)
        End If
        RefreshData(False)
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("entity_scheduler.aspx")

    End Sub

    

    Private Sub entity_scheduler_dayendtime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_dayendtime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-dayendtime", Me.entity_scheduler_dayendtime.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub entity_scheduler_daystarttime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_daystarttime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-daystarttime", Me.entity_scheduler_daystarttime.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub p31_scheduler_multidays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_multidays.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-multidays", Me.entity_scheduler_multidays.SelectedValue)
        Me.CurrentView = SchedulerViewType.MultiDayView
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub cmdExportICalendar_Click(sender As Object, e As EventArgs) Handles cmdExportICalendar.Click
        RefreshData(True)
        Dim s As String = RadScheduler.ExportToICalendar(scheduler1.Appointments())
        Dim response As HttpResponse = Page.Response
        response.Clear()
        response.Buffer = True
        response.ContentType = "text/calendar"
        response.ContentEncoding = Encoding.UTF8
        response.Charset = "utf-8"
        response.AddHeader("Content-Disposition", _
                  "attachment;filename=""MARKTIME_CALENDAR.ics""")
        response.Write(s)
        response.[End]()
    End Sub

    Private Sub chkSetting_P48_CheckedChanged(sender As Object, e As EventArgs) Handles chkSetting_P48.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-p48", BO.BAS.GB(Me.chkSetting_P48.Checked))
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub chkSetting_O22_CheckedChanged(sender As Object, e As EventArgs) Handles chkSetting_O22.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-o22", BO.BAS.GB(Me.chkSetting_O22.Checked))
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub chkSetting_P56_CheckedChanged(sender As Object, e As EventArgs) Handles chkSetting_P56.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-p56", BO.BAS.GB(Me.chkSetting_P56.Checked))
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub cbxNewRecType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxNewRecType.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-newrec_prefix", Me.cbxNewRecType.SelectedValue)
        RefreshData(False)
    End Sub

    Private Sub entity_scheduler_agendadays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles entity_scheduler_agendadays.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-agendadays", Me.entity_scheduler_agendadays.SelectedValue)
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    Private Sub chkIncludeChilds_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeChilds.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-include_childs", BO.BAS.GB(Me.chkIncludeChilds.Checked))
        RefreshData(False)
        hidIsLoadingSetting.Value = "1"
    End Sub

    

    Private Sub entity_scheduler_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.CurrentMasterPrefix = "" Then
            
        End If
        basUIMT.RenderQueryCombo(Me.j70ID)
        With Me.j70ID
            If .SelectedIndex > 0 Then
                Me.clue_query.Visible = True
                .ToolTip = .SelectedItem.Text
                Me.clue_query.Attributes("rel") = "clue_quickquery.aspx?j70id=" & .SelectedValue
            Else
                Me.clue_query.Visible = False
            End If
        End With
        PersonsHeader.Text = persons1.CurrentHeader
        ProjectsHeader.Text = projects1.CurrentHeader
        'Master.Notify(String.Join(",", persons1.CurrentJ02IDs))
    End Sub

    Private Sub persons1_OnChange() Handles persons1.OnChange
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-scope", persons1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-value", persons1.CurrentValue)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-persons1-personsrole", persons1.CurrentPersonsRole)

        RefreshData(False)
        hidIsPersonsChange.Value = "1"
    End Sub

    Private Sub SetupJ70Combo(intDef As Integer)
        Dim mq As New BO.myQuery
        j70ID.DataSource = Master.Factory.j70QueryTemplateBL.GetList(mq, BO.x29IdEnum.p56Task)
        j70ID.DataBind()
        j70ID.Items.Insert(0, "--Pojmenovaný filtr úkolů--")

        basUI.SelectDropdownlistValue(Me.j70ID, intDef.ToString)

        cmdQuery.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
    End Sub

    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-j70id", Me.j70ID.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub projects1_OnChange() Handles projects1.OnChange
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-projects1-scope", projects1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("entity_scheduler-projects1-value", projects1.CurrentValue)
        RefreshData(False)
        hidIsProjectsChange.Value = "1"
    End Sub

    Private Sub cmdExportPDF_Click(sender As Object, e As EventArgs) Handles cmdExportPDF.Click

        scheduler1.ExportToPdf()
    End Sub
End Class