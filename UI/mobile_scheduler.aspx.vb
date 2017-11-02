Imports Telerik.Web.UI

Public Class mobile_scheduler
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
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

    Private Sub mobile_scheduler_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.MenuPrefix = "scheduler"
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("mobile_scheduler-view")
                .Add("mobile_scheduler-scope")
            End With

            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)
                Me.CurrentView = .GetUserParam("mobile_scheduler-view", "1")
                basUI.SelectDropdownlistValue(Me.cbxScope, .GetUserParam("mobile_scheduler-scope", "1"))
            End With


            RefreshData()

        End If
    End Sub


    Private Sub RefreshData()
        With Me.scheduler1
            .Appointments.Clear()
            .WeekView.DayStartTime = .DayView.DayStartTime
            .WeekView.DayEndTime = .DayView.DayEndTime
            .MultiDayView.DayStartTime = .DayView.DayStartTime
            .MultiDayView.DayEndTime = .DayView.DayEndTime
            
        End With
        Dim d1 As Date = scheduler1.VisibleRangeStart.AddDays(-1), d2 As Date = scheduler1.VisibleRangeEnd.AddDays(1)

        Dim bolO22 As Boolean = True, intJ02ID As Integer = Master.Factory.SysUser.j02ID, bolP56 As Boolean = True

        If bolO22 Then
            Dim mq As New BO.myQueryO22
            mq.j02IDs = BO.BAS.ConvertInt2List(intJ02ID)

            mq.DateFrom = d1 : mq.DateUntil = d2
            Dim lis As IEnumerable(Of BO.o22Milestone) = Master.Factory.o22MilestoneBL.GetList(mq)
            For Each cRec In lis
                Dim c As New Appointment()
                With cRec
                    c.ID = .PID.ToString & ",'o22'"
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
                                c.End = c.Start.AddDays(1)
                            Else
                                Select Case Me.CurrentView
                                    Case SchedulerViewType.DayView, SchedulerViewType.WeekView
                                        If Me.CurrentView = SchedulerViewType.DayView And scheduler1.DayView.DayStartTime.Hours > c.Start.Hour Then
                                            scheduler1.DayView.DayStartTime = System.TimeSpan.FromHours(c.Start.Hour)
                                        End If
                                        If Me.CurrentView = SchedulerViewType.WeekView And scheduler1.WeekView.DayStartTime.Hours > c.Start.Hour Then
                                            scheduler1.WeekView.DayStartTime = System.TimeSpan.FromHours(c.Start.Hour)
                                        End If
                                End Select
                            End If

                    End Select
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
      
        If bolP56 Then  'termíny úkolů
            Dim mq As New BO.myQueryP56
            Select Case cbxScope.SelectedValue
                Case "1"
                    mq.j02IDs = BO.BAS.ConvertInt2List(intJ02ID)
                Case "2"
                    mq.j02ID_Owner = Master.Factory.SysUser.j02ID
            End Select

            mq.p56PlanUntil_D1 = d1 : mq.p56PlanUntil_D2 = d2
            Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq)
            For Each cRec In lis
                Dim c As New Appointment()
                With cRec
                    c.ID = .PID.ToString & ",'p56'"
                    c.Subject = .p56Name

                    If .p57PlanDatesEntryFlag = 4 And Not .p56PlanFrom Is Nothing Then
                        c.Start = .p56PlanFrom
                    Else
                        c.Start = .p56PlanUntil
                    End If
                    c.End = .p56PlanUntil
                    Select Case Me.CurrentView
                        Case SchedulerViewType.DayView, SchedulerViewType.WeekView
                            If c.End.Hour = 0 And c.End.Minute = 0 Then
                                c.Start = DateSerial(Year(c.End), Month(c.End), Day(c.End))
                                c.End = c.Start.AddDays(1)
                            Else
                                If Me.CurrentView = SchedulerViewType.DayView And scheduler1.DayView.DayStartTime.Hours > c.End.Hour Then
                                    scheduler1.DayView.DayStartTime = System.TimeSpan.FromHours(c.End.Hour)
                                End If
                                If Me.CurrentView = SchedulerViewType.WeekView And scheduler1.WeekView.DayStartTime.Hours > c.End.Hour Then
                                    scheduler1.WeekView.DayStartTime = System.TimeSpan.FromHours(c.End.Hour)
                                End If
                            End If
                    End Select



                    Select Case Me.CurrentView
                        Case SchedulerViewType.MonthView, SchedulerViewType.TimelineView, SchedulerViewType.WeekView
                            If Len(.p56Name) > 0 Then c.Subject = BO.BAS.OM3(.p56Name, 15)


                    End Select
                End With

                scheduler1.InsertAppointment(c)
            Next
        End If
    End Sub
    Private Sub scheduler1_NavigationComplete(sender As Object, e As SchedulerNavigationCompleteEventArgs) Handles scheduler1.NavigationComplete
        Dim bolChangeView As Boolean = False
        Select Case e.Command
            Case SchedulerNavigationCommand.SwitchToAgendaView, SchedulerNavigationCommand.SwitchToMonthView, SchedulerNavigationCommand.SwitchToTimelineView, SchedulerNavigationCommand.SwitchToWeekView, SchedulerNavigationCommand.SwitchToMultiDayView
                bolChangeView = True
        End Select
        If bolChangeView Then
            Master.Factory.j03UserBL.SetUserParam("mobile_scheduler-view", CInt(Me.CurrentView).ToString)
        End If
        RefreshData()
    End Sub
    Private Sub ReloadPage()
        Response.Redirect("mobile_scheduler.aspx")
    End Sub

    Private Sub cbxScope_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxScope.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("mobile_scheduler-scope", Me.cbxScope.SelectedValue)
        RefreshData()
    End Sub
End Class