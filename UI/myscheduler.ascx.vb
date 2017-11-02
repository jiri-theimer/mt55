Imports Telerik.Web.UI
Public Class myscheduler
    Inherits System.Web.UI.UserControl
    Public Property factory As BL.Factory
    Private Property _curIsShowP57name As Boolean = False
    Private Property _lastP41ID As Integer = 0
    Private Property _showProjectRow As Boolean = True


    Public Property NumberOfDays As Integer
        Get
            Return Me.scheduler1.AgendaView.NumberOfDays
        End Get
        Set(value As Integer)
            Me.scheduler1.AgendaView.NumberOfDays = value
            basUI.SelectDropdownlistValue(cbxNumberOfDays, value.ToString)

        End Set
    End Property
    Public Property MaxTopRecs As Integer
        Get
            Return CInt(Me.cbxTopRecs.SelectedValue)
        End Get
        Set(value As Integer)
            cbxTopRecs.SelectedValue = value.ToString
        End Set
    End Property
    Public Property FirstDayMinus As Integer
        Get
            Return CInt(Me.cbxFirstDay.SelectedValue)
        End Get
        Set(value As Integer)
            cbxFirstDay.SelectedValue = value.ToString
        End Set
    End Property
    Public Property Prefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property
    Public Property RecordPID As Integer
        Get
            Return BO.BAS.IsNullInt(hidRecordPID.Value)
        End Get
        Set(value As Integer)
            hidRecordPID.Value = value.ToString
        End Set
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return scheduler1.Appointments.Count
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(d0 As Date)
        scheduler1.AgendaView.NumberOfDays = CInt(cbxNumberOfDays.SelectedValue)
        If Me.RecordPID = 0 Then
            Return
        End If

        Dim d1 As Date = d0.AddDays(Me.FirstDayMinus)
        Dim d2 As Date = d1.AddDays(Me.NumberOfDays)

        With scheduler1
            .Appointments.Clear()
            If .SelectedView = SchedulerViewType.AgendaView Then
                .SelectedDate = d1
            End If
            If .SelectedView = SchedulerViewType.DayView Then
                d1 = .SelectedDate
                d2 = d1.AddDays(1)
            End If
        End With


        fill_o22(d1, d2)
        fill_p56(d1, d2)
        lblNoAppointments.Visible = False
        With scheduler1
            If .Appointments.Count = 0 Then
                .Height = Unit.Parse("40px")
                lblNoAppointments.Visible = True
                If .SelectedView = SchedulerViewType.AgendaView Then
                    lblNoAppointments.Text = String.Format("V [{0}] žádné úkoly/události s termínem.", Format(.SelectedDate, "dd.MM.yyyy") & " - " & Format(.SelectedDate.AddDays(.AgendaView.NumberOfDays), "dd.MM.yyyy"))
                End If
                If .SelectedView = SchedulerViewType.DayView Then
                    lblNoAppointments.Text = String.Format("V [{0}] žádné úkoly/události s termínem.", Format(.SelectedDate, "dd.MM.yyyy"))
                End If
            Else
                .Height = Unit.Parse("300px")
            End If

        End With


    End Sub
    Private Sub fill_o22(d1 As Date, d2 As Date)
        Dim intRecordPID As Integer = Me.RecordPID
        Dim mq As New BO.myQueryO22
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TopRecordsOnly = Me.MaxTopRecs
        mq.MG_SortString = "a.o22DateFrom,a.o22DateUntil"
        'mq.SpecificQuery = BO.myQueryO22_SpecificQuery.AllowedForRead
        Select Case hidPrefix.Value
            Case "p28"
                mq.p28ID = intRecordPID
            Case "p41"
                mq.p41ID = intRecordPID

            Case "j02"
                mq.j02IDs = BO.BAS.ConvertInt2List(intRecordPID)
            Case Else
                Return
        End Select
        mq.DateFrom = d1 : mq.DateUntil = d2
        Dim lis As IEnumerable(Of BO.o22Milestone) = factory.o22MilestoneBL.GetList(mq)
        For Each cRec In lis
            Dim c As New Appointment()
            With cRec
                c.ID = .PID.ToString & ",'o22'"
                c.Description = "clue_o22_record.aspx?pid=" & .PID.ToString
                c.Subject = "<img src='Images/datepicker.png'/> " & BO.BAS.OM3(.o22Name, 100)
                If Len(c.Subject) > 120 Then
                    c.ToolTip = .o22Name
                Else
                    c.ToolTip = .o21Name
                End If

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
                If c.End < Today Then c.Font.Strikeout = True

                c.BorderColor = Drawing.Color.Gray
                c.BorderStyle = BorderStyle.Dashed
            End With
            scheduler1.InsertAppointment(c)
        Next
    End Sub
    Private Sub fill_p56(d1 As Date, d2 As Date)
        Dim intRecordPID As Integer = Me.RecordPID
        Dim mq As New BO.myQueryP56
        mq.TopRecordsOnly = Me.MaxTopRecs
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.MG_SortString = "a.p56PlanUntil"
        Select Case hidPrefix.Value
            Case "p28"
                mq.p28ID = intRecordPID
            Case "p41"
                mq.p41ID = intRecordPID
            Case "j02"
                mq.j02ID = intRecordPID
            Case Else
                Return
        End Select
        mq.p56PlanUntil_D1 = d1 : mq.p56PlanUntil_D2 = d2
        Dim lis As IEnumerable(Of BO.p56Task) = factory.p56TaskBL.GetList(mq)
        For Each cRec In lis
            Dim c As New Appointment()
            With cRec
                c.ID = .PID.ToString & ",'p56'"
                c.Description = "clue_p56_record.aspx?pid=" & .PID.ToString
                c.Subject = "<img src='Images/task.png'/> " & BO.BAS.OM3(.p56Name, 100)
                If .b02ID <> 0 Then c.Subject += " [" & .b02Name & "]"
                If Len(c.Subject) > 120 Then
                    c.ToolTip = .p56Name
                Else
                    c.ToolTip = .p57Name
                End If


                If .p57PlanDatesEntryFlag = 4 And Not .p56PlanFrom Is Nothing Then
                    c.Start = .p56PlanFrom
                Else
                    c.Start = .p56PlanUntil
                End If
                c.End = .p56PlanUntil

                c.BackColor = Drawing.Color.FromName("#3CB371")

                If (c.End.Hour = 23 And c.End.Minute = 59) Or (c.End.Hour = 0 And c.End.Minute = 0 And c.End.Second = 0) Then   'scheduler1.SelectedView = SchedulerViewType.DayView And 
                    c.Start = DateSerial(Year(c.Start), Month(c.Start), Day(c.Start))
                    c.End = DateSerial(Year(c.End), Month(c.End), Day(c.End)).AddDays(1)

                End If

            End With

            scheduler1.InsertAppointment(c)
        Next

    End Sub

    Private Sub scheduler1_NavigationComplete(sender As Object, e As SchedulerNavigationCompleteEventArgs) Handles scheduler1.NavigationComplete

        RefreshData(scheduler1.SelectedDate)
    End Sub

    Public Sub RefreshTasksWithoutDate(bolShowProjectRow As Boolean)
        _showProjectRow = bolShowProjectRow
        If Me.RecordPID = 0 Or Me.Prefix = "" Then Return
        panP56.Visible = False
        Dim mq As New BO.myQueryP56
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TerminNeniVyplnen = BO.BooleanQueryMode.TrueQuery
        mq.TopRecordsOnly = Me.MaxTopRecs
        mq.IsShowTagsInColumn = True

        Select Case hidPrefix.Value
            Case "p28"
                mq.p28ID = Me.RecordPID
            Case "p41"
                mq.p41ID = Me.RecordPID
            Case "j02"
                mq.j02ID = Me.RecordPID
            Case Else
                Return
        End Select
        Dim lisP56 As IEnumerable(Of BO.p56Task) = factory.p56TaskBL.GetList(mq).OrderBy(Function(p) p.Client).ThenBy(Function(p) p.p41Name)

        If lisP56.Select(Function(p) p.p57ID).Distinct.Count > 1 Then _curIsShowP57name = True
        Me.p56Count.Text = lisP56.Count.ToString
        If lisP56.Count = Me.MaxTopRecs Then
            Me.p56Count.Text = String.Format("Podmínce vyhovuje více než {0} úkolů bez termínu!", MaxTopRecs)
        End If
        _lastP41ID = 0
        rpP56.DataSource = lisP56
        rpP56.DataBind()
        If lisP56.Count = 0 Then
            Me.panP56.Visible = False
        Else
            Me.panP56.Visible = True
        End If
    End Sub

    Private Sub rpP56_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP56.ItemDataBound
        Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
        With CType(e.Item.FindControl("link1"), HyperLink)
            If _curIsShowP57name Then
                .Text = cRec.NameWithTypeAndCode
            Else
                .Text = cRec.p56Name
            End If

            .NavigateUrl = "p56_framework.aspx?pid=" & cRec.PID.ToString
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        With CType(e.Item.FindControl("tags"), Label)
            .Text = cRec.TagsInlineHtml
        End With
        ''With CType(e.Item.FindControl("clue1"), HyperLink)
        ''    .Attributes.Item("rel") = "clue_p56_record.aspx?&pid=" & cRec.PID.ToString
        ''End With
        If Not cRec.p56ReminderDate Is Nothing Then
            e.Item.FindControl("img1").Visible = True
        Else
            e.Item.FindControl("img1").Visible = False
        End If
        If _showProjectRow Then
            With CType(e.Item.FindControl("Project"), Label)
                If _lastP41ID = cRec.p41ID Then
                    .Visible = False
                Else
                    .Text = BO.BAS.OM3(cRec.p41Name, 100)
                    If cRec.Client <> "" Then
                        .Text = cRec.Client & " - " & .Text
                    End If
                End If
            End With
        Else
            e.Item.FindControl("Project").Visible = False
        End If
        With CType(e.Item.FindControl("pm1"), HyperLink)
            .Attributes.Item("onclick") = "RCM('p56'," & cRec.PID.ToString & ",this)"
        End With
        With CType(e.Item.FindControl("lblB02Name"), Label)
            .Text = cRec.b02Name
            If cRec.b02Color <> "" Then .Style.Item("background-color") = cRec.b02Color

        End With
       

        If Not BO.BAS.IsNullDBDate(cRec.p56PlanUntil) Is Nothing Then
            With CType(e.Item.FindControl("p56PlanUntil"), Label)
                .Text = BO.BAS.FD(cRec.p56PlanUntil, True, True)
                If cRec.p56PlanUntil < Now Then
                    .Text += "...je po termínu!" : .ForeColor = Drawing.Color.Red
                Else
                    .ForeColor = Drawing.Color.Green
                End If
            End With

        End If
        _lastP41ID = cRec.p41ID
    End Sub

    
    Private Sub cbxTopRecs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxTopRecs.SelectedIndexChanged
        factory.j03UserBL.SetUserParam("myscheduler-maxtoprecs-" & Me.Prefix, cbxTopRecs.SelectedValue)
        RefreshData(Today)
        RefreshTasksWithoutDate(True)
    End Sub

    Private Sub cbxNumberOfDays_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxNumberOfDays.SelectedIndexChanged
        factory.j03UserBL.SetUserParam("myscheduler-numberofdays-" & Me.Prefix, cbxNumberOfDays.SelectedValue)
        RefreshData(Today)

    End Sub

    Private Sub cbxFirstDay_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFirstDay.SelectedIndexChanged
        factory.j03UserBL.SetUserParam("myscheduler-firstday", cbxFirstDay.SelectedValue)
        RefreshData(Today)
    End Sub


    
End Class