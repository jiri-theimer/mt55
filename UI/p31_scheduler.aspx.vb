Imports Telerik.Web.UI

Public Class p31_scheduler
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _curD1 As Date
    Private Property _curD2 As Date
    Private Class DaySumRow
        Public Property Datum As Date
        Public Property Hodiny As Double

    End Class
    Public Property CurrentView As SchedulerViewType
        Get
            Return Me.scheduler1.SelectedView
        End Get
        Set(value As SchedulerViewType)
            Me.scheduler1.SelectedView = value
        End Set
    End Property
    Public ReadOnly Property CurrentJ02ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.j02ID.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property CurrentTasksPrefix As String
        Get
            Return Me.p31_scheduler_tasks.SelectedValue
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        timer1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "p31_scheduler"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p31_scheduler-view")
                    .Add("p31_scheduler-daystarttime")
                    .Add("p31_scheduler-dayendtime")
                    .Add("p31_scheduler-multidays")
                    .Add("p31_framework_detail-j02id")
                    .Add("p31_scheduler_tasks")
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Me.CurrentView = .GetUserParam("p31_scheduler-view", "0")
                    basUI.SelectDropdownlistValue(Me.p31_scheduler_daystarttime, .GetUserParam("p31_scheduler-daystarttime", "8"))
                    basUI.SelectDropdownlistValue(Me.p31_scheduler_dayendtime, .GetUserParam("p31_scheduler-dayendtime", "20"))
                    basUI.SelectDropdownlistValue(Me.p31_scheduler_multidays, .GetUserParam("p31_scheduler-multidays", "2"))

                    basUI.SelectDropdownlistValue(Me.p31_scheduler_tasks, .GetUserParam("p31_scheduler_tasks", "p41"))
                    SetupComboPersons(.GetUserParam("p31_framework_detail-j02id"))
                End With



            End With
            Select Case Me.CurrentTasksPrefix
                Case "p56"
                    SetupTasks()
                Case "p41", "favourites"
                    SetupProjects()
                Case Else
                    tasks.Visible = False
                    panTasks.Visible = False
            End Select



            RefreshRecord()
            RefreshData(False)
        End If
    End Sub

    Private Sub SetupTasks()
        Dim mq As New BO.myQueryP56
        mq.j02ID = Me.CurrentJ02ID
        mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForWorksheetEntry
        mq.Closed = BO.BooleanQueryMode.NoQuery

        Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq)
        
        If lis.Count = 0 Then
            Me.lblTasksHeader.Text = "V systému nemáte otevřený úkol."
            tasks.Visible = False
            Return
        Else
            Me.lblTasksHeader.Text = String.Format("Přetáhni úkol do kalendáře ({0}):", lis.Count.ToString)
            tasks.Visible = True
        End If

        Dim bolShowTaskType As Boolean = False
        If lis.GroupBy(Function(p) p.p57ID).Count > 1 Then bolShowTaskType = True

        For Each c In lis
            AddTaskTreeNode(c, bolShowTaskType)
        Next
    End Sub
    Private Sub AddTaskTreeNode(cRec As BO.p56Task, bolShowTaskType As Boolean)
        Dim n As New RadTreeNode()
        With cRec
            n.Value = .PID.ToString
            If bolShowTaskType Then
                n.Text = .p57Name & ": "
            End If
            If Len(.p56Name) > 30 Then
                n.Text += .p56Code & " - " & Left(.p56Name, 30) & "..."
            Else
                n.Text += .p56Code & " - " & .p56Name
            End If
            n.ToolTip = .Client & " - " & .ProjectCodeAndName
        End With
        n.AllowDrag = True
        tasks.Nodes.Add(n)
    End Sub
    Private Sub SetupProjects()
        img1.ImageUrl = "Images/project.png"
        Dim mq As New BO.myQueryP41
        If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then mq.j02ID_ExplicitQueryFor = Me.CurrentJ02ID
        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        mq.Closed = BO.BooleanQueryMode.NoQuery
        If Me.CurrentTasksPrefix = "favourites" Then mq.IsFavourite = BO.BooleanQueryMode.TrueQuery

        Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
        If lis.Count > 10 And Me.CurrentTasksPrefix = "p41" Then
            lis = basUIMT.QueryProjectListByTop10(Master.Factory, Me.CurrentJ02ID)
        End If


        
        If lis.Count = 0 Then
            Me.lblTasksHeader.Text = "V systému nemáte otevřený projekt."
            If Me.CurrentTasksPrefix = "favourites" Then Me.lblTasksHeader.Text += "<img src='Images/favourite.png'/>"
            tasks.Visible = False
            Return
        Else
            Dim s As String = lis.Count.ToString
            If Me.CurrentTasksPrefix = "favourites" Then s += "<img src='/Images/favourite.png'/>"
            Me.lblTasksHeader.Text = String.Format("Přetáhni projekt do kalendáře ({0}):", s)
            tasks.Visible = True
        End If

        For Each c In lis
            AddProjectTreeNode(c)
        Next
    End Sub
    Private Sub AddProjectTreeNode(cRec As BO.p41Project)
        Dim n As New RadTreeNode()
        With cRec
            n.Value = .PID.ToString
            If Len(.FullName) > 30 Then
                n.Text += Left(.FullName, 30) & "..."
                n.ToolTip = .FullName
            Else
                n.Text += .FullName
            End If

        End With
        n.AllowDrag = True
        tasks.Nodes.Add(n)
    End Sub
    Private Sub SetupComboPersons(strDefJ02ID As String)
        Me.j02ID.Items.Clear()
        With Master.Factory.SysUser
            Me.j02ID.Items.Add(New ListItem(.PersonDesc, .j02ID.ToString))
            If .IsMasterPerson Then
                Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList_Slaves(.j02ID, True, BO.j05Disposition_p31ENUM._NotSpecified, False, BO.j05Disposition_p48ENUM._NotSpecified).Where(Function(p) p.IsClosed = False)
                For Each c In lisJ02
                    Me.j02ID.Items.Add(New ListItem(c.FullNameDesc, c.PID.ToString))
                Next
            End If
        End With
        If strDefJ02ID <> "" Then basUI.SelectDropdownlistValue(Me.j02ID, strDefJ02ID)
    End Sub

    Private Sub RefreshData(bolData4Export As Boolean)
        With Me.scheduler1
            .Appointments.Clear()
            .DayView.DayStartTime = System.TimeSpan.FromHours(CDbl(Me.p31_scheduler_daystarttime.SelectedValue))
            .DayView.DayEndTime = System.TimeSpan.FromHours(CDbl(Me.p31_scheduler_dayendtime.SelectedValue))
            .WeekView.DayStartTime = .DayView.DayStartTime
            .WeekView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.DayStartTime = .DayView.DayStartTime
            .MultiDayView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.NumberOfDays = BO.BAS.IsNullInt(Me.p31_scheduler_multidays.SelectedValue)
            .Localization.HeaderMultiDay = "Multi-den (" & .MultiDayView.NumberOfDays.ToString & ")"
            ''If .SelectedView = SchedulerViewType.MonthView Then
            ''    .RowHeight = Unit.Parse("40px")
            ''Else
            ''    .RowHeight = Nothing
            ''End If
        End With
        Dim d1 As Date = scheduler1.VisibleRangeStart.AddDays(-1), d2 As Date = scheduler1.VisibleRangeEnd.AddDays(1)
        Dim mq As New BO.myQueryP31
        mq.DateFrom = d1
        mq.DateUntil = d2
        mq.j02ID = Me.CurrentJ02ID
        'If mq.j02ID <> Master.Factory.SysUser.j02ID Then
        '    mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
        'End If


        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq).Where(Function(p) p.p33ID = BO.p33IdENUM.Cas)
        For Each cRec In lisP31
            Dim c As New Appointment()
            With cRec
                c.ID = .PID
                'c.AllowEdit = False
                'If .j02ID = Master.Factory.SysUser.j02ID Then
                '    If .IsClosed And .p71ID = BO.p71IdENUM.Nic And .p91ID = 0 Then
                '        c.AllowEdit = True
                '    End If
                'End If
                If bolData4Export Then
                    c.Description = .p31Text
                    c.Subject = BO.BAS.FN(.p31Hours_Orig)

                Else
                    c.Description = "clue_p31_record.aspx?pid=" & .PID.ToString & "&js_clone=clone(" & .PID.ToString & ")&js_edit=re(" & .PID.ToString & ")"
                    c.Subject = "<span class='valboldblue'>" & BO.BAS.FN(.p31Hours_Orig) & "</span> "
                End If
                
                Dim s As String = .p28CompanyShortName
                If s = "" Then s = .ClientName
                If s > "" Then
                    If Len(s) > 20 Then c.Subject += Left(s, 20) & "..." Else c.Subject += s
                End If
                c.Subject += " - "
                s = .p41NameShort
                If s = "" Then s = .p41Name
                If Len(s) > 30 Then c.Subject += Left(s, 30) & "..." Else c.Subject += s

                If Me.CurrentView = SchedulerViewType.MonthView Then
                    c.ToolTip = ""
                    ''If Len(c.Subject) > 30 Then c.Subject = Left(c.Subject, 30)
                Else
                    If Len(.p31Text) > 50 Then
                        c.ToolTip = Left(.p31Text, 50) & "..."
                    Else
                        c.ToolTip = .p31Text
                    End If
                End If

                'If .o22DateUntil < datFoundMin Then datFoundMin = .o22DateUntil

                If Not .p31DateTimeFrom_Orig Is Nothing Then
                    c.Start = .p31DateTimeFrom_Orig
                    c.End = .p31DateTimeUntil_Orig
                Else
                    c.Start = .p31Date
                    c.End = c.Start.AddDays(1)
                End If
                If .p32IsBillable Then
                    c.BorderColor = Drawing.Color.Gray
                Else
                    c.BorderColor = Drawing.Color.Red

                End If
                c.BackColor = Drawing.Color.WhiteSmoke
                c.ForeColor = Drawing.Color.Black
                c.BorderStyle = BorderStyle.Dashed


            End With

            scheduler1.InsertAppointment(c)
        Next

        scheduler1.Rebind()

        Dim lis As New List(Of DaySumRow)
        For Each sum In lisP31.GroupBy(Function(p) p.p31Date)
            Dim c As New DaySumRow
            c.Datum = sum.First.p31Date
            c.Hodiny = sum.Sum(Function(p) p.p31Hours_Orig)
            lis.Add(c)
        Next

        Select Case Me.CurrentView
            Case SchedulerViewType.MonthView
                _curD1 = DateSerial(Year(d1.AddDays(10)), Month(d1.AddDays(10)), 1)
                _curD2 = _curD1.AddMonths(1).AddDays(-1)
            Case SchedulerViewType.DayView
                _curD1 = scheduler1.VisibleRangeStart
                _curD2 = _curD1
            Case Else
                _curD1 = scheduler1.VisibleRangeStart
                _curD2 = scheduler1.VisibleRangeEnd.AddDays(-1)
        End Select

        Dim d As Date = _curD1

        While d <= _curD2
            If lis.Where(Function(p) p.Datum = d).Count = 0 Then
                Dim c As New DaySumRow
                c.Datum = d
                c.Hodiny = 0
                lis.Add(c)
            End If
            d = d.AddDays(1)
        End While


        rp1.DataSource = lis.Where(Function(p) p.Datum >= _curD1 And p.Datum <= _curD2).OrderBy(Function(p) p.Datum)
        rp1.DataBind()
        TotalHours.Text = BO.BAS.FN(lis.Where(Function(p) p.Datum >= _curD1 And p.Datum <= _curD2).Sum(Function(p) p.Hodiny))
    End Sub

    Private Sub RefreshRecord()

    End Sub

    

   

    Private Sub scheduler1_NavigationComplete(sender As Object, e As SchedulerNavigationCompleteEventArgs) Handles scheduler1.NavigationComplete
        Dim bolChangeView As Boolean = False
        Select Case e.Command
            Case SchedulerNavigationCommand.SwitchToAgendaView, SchedulerNavigationCommand.SwitchToMonthView, SchedulerNavigationCommand.SwitchToTimelineView, SchedulerNavigationCommand.SwitchToWeekView, SchedulerNavigationCommand.SwitchToMultiDayView
                bolChangeView = True
        End Select
        If bolChangeView Then
            Master.Factory.j03UserBL.SetUserParam("p31_scheduler-view", CInt(Me.CurrentView).ToString)
        End If
        RefreshData(False)
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("p31_scheduler.aspx")

    End Sub

    Private Sub cmdHardRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdHardRefreshOnBehind.Click

        RefreshData(False)
        If timer1.RowsCount > 0 Then
            timer1.RefreshList()
        End If
    End Sub

    
    Private Sub p31_scheduler_dayendtime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p31_scheduler_dayendtime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_scheduler-dayendtime", Me.p31_scheduler_dayendtime.SelectedValue)
        RefreshData(False)
    End Sub

    Private Sub p31_scheduler_daystarttime_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p31_scheduler_daystarttime.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_scheduler-daystarttime", Me.p31_scheduler_daystarttime.SelectedValue)
        RefreshData(False)
    End Sub

    Private Sub p31_scheduler_multidays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p31_scheduler_multidays.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_scheduler-multidays", Me.p31_scheduler_multidays.SelectedValue)
        Me.CurrentView = SchedulerViewType.MultiDayView
        RefreshData(False)

    End Sub

    Private Sub p31_scheduler_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then
            Me.j02ID.BackColor = Drawing.Color.Red
        Else
            Me.j02ID.BackColor = Nothing
        End If
        Me.tabs1.Tabs(0).Text = Me.j02ID.SelectedItem.Text
        If Not Page.IsPostBack Then
            If timer1.RowsCount > 0 Then
                tabs1.SelectedIndex = 1
                RadMultiPage1.SelectedIndex = 1
            End If
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As DaySumRow = CType(e.Item.DataItem, DaySumRow)
        With CType(e.Item.FindControl("Date"), Label)
            .Text = Format(cRec.Datum, "dd.MM ddd")
        End With
        If cRec.Hodiny <> 0 Then
            With CType(e.Item.FindControl("Hours"), Label)
                .Text = BO.BAS.FN(cRec.Hodiny)
            End With
        End If
        Select Case Weekday(cRec.Datum, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
            Case 6, 7
                With CType(e.Item.FindControl("trSumRow"), HtmlTableRow)
                    .BgColor = "whitesmoke"
                End With
        End Select
        
        ''If cRec.Datum >= _curD1 And cRec.Datum <= _curD2 Then
        ''    With CType(e.Item.FindControl("trSumRow"), HtmlTableRow)
        ''        .BgColor = "#FFD732"
        ''    End With
        ''End If
        


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

    
    Private Sub p31_scheduler_tasks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p31_scheduler_tasks.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_scheduler_tasks", Me.p31_scheduler_tasks.SelectedValue)
        ReloadPage()
    End Sub
End Class