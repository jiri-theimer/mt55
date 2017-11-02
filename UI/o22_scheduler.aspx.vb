Imports Telerik.Web.UI

Public Class o22_scheduler
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub o22_scheduler_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .HeaderText = "Projektový kalendář"

                '.AddToolbarButton("Uložit změny", "ok", , "Images/save.png")

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("o22_scheduler-view")

                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)

            End With

            SetupScheduler()

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim mq As New BO.myQueryO22
        Dim lis As IEnumerable(Of BO.o22Milestone) = Master.Factory.o22MilestoneBL.GetList(mq)
        For Each cRec In lis
            Dim c As New Appointment()
            With cRec
                c.ID = .PID
                Select Case .o21Flag
                    Case BO.o21FlagEnum.DeadlineOrMilestone
                        'c.BackColor = Drawing.Color.Aquamarine
                        c.BackColor = Drawing.Color.Salmon
                        c.Start = .o22DateUntil
                        c.End = .o22DateUntil
                    Case BO.o21FlagEnum.EventFromUntil
                        c.BackColor = Drawing.Color.AntiqueWhite
                        c.Start = .o22DateFrom
                        c.End = .o22DateUntil
                        If .o22IsAllDay Then

                            c.End = c.Start.AddDays(1)

                        End If
                End Select
                c.BorderColor = Drawing.Color.Gray
                c.BorderStyle = BorderStyle.Dashed

                c.Subject = .o22Name
            End With
            Me.scheduler1.InsertAppointment(c)
        Next
        Dim xx As New Appointment(666, Now.AddHours(-10), Now.AddHours(-8), "AHOJ")
        Me.scheduler1.InsertAppointment(xx)

    End Sub

    Private Sub SetupScheduler()
        With scheduler1
            .TimeSlotContextMenus(0).Enabled = True
            .GroupingDirection = Telerik.Web.UI.GroupingDirection.Vertical
            .SelectedDate = Now

            '.SelectedView = SchedulerViewType.WeekView
            'Select Case Master.cUser.GetUserVal("scheduler_selectedview", "monthview")
            '    Case "dayview" : .SelectedView = SchedulerViewType.DayView : chkRozklad.Checked = True
            '    Case "weekview" : .SelectedView = SchedulerViewType.WeekView
            '    Case "timelineview" : .SelectedView = SchedulerViewType.TimelineView : chkRozklad.Checked = True
            '    Case "multidayview" : .SelectedView = SchedulerViewType.MultiDayView
            '    Case Else
            '        .SelectedView = SchedulerViewType.MonthView
            'End Select


            .TimelineView.NumberOfSlots = 20

            Dim xx As New System.TimeSpan(1, 0, 0, 0)
            .TimelineView.SlotDuration = xx
        End With
    End Sub

    Private Sub scheduler1_AppointmentDataBound(sender As Object, e As SchedulerEventArgs) Handles scheduler1.AppointmentDataBound

    End Sub
End Class