Imports Telerik.Web.UI

Public Class timesheet_calendar
    Inherits System.Web.UI.UserControl
    Private _lisHours As IEnumerable(Of BO.p31WorksheetCalendarHours) = Nothing
    Private _lisHolidays As IEnumerable(Of BO.c26Holiday) = Nothing

    Public Event NeedDataSource(ByRef lisGetMeWorksheetHours As IEnumerable(Of BO.p31WorksheetCalendarHours), ByRef lisHolidays As IEnumerable(Of BO.c26Holiday))
    Public Event SelectedDateChanged(datSelected As Date)
    Public Event ViewChanged()
    Public Event CalendarColumnsChanged(intCalendarColumns As Integer)
    Public Property factory As BL.Factory
   
    Public ReadOnly Property CurrentPocetMesicu As Integer
        Get
            Return BO.BAS.IsNullInt(Me.CalCols.SelectedValue)
        End Get
    End Property
    Public Property CalendarColumns As Integer
        Get
            Return BO.BAS.IsNullInt(Me.CalCols.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.CalCols, value.ToString)
        End Set
    End Property
    Public Property SelectedDate As Date
        Get
            Return cal1.SelectedDate
        End Get
        Set(value As Date)
            cal1.SelectedDate = value
            cal1.FocusedDate = value

        End Set
    End Property
    Public Property CurrentD1 As Date
        Get
            If Me.hidD1.Value = "" Then SetupDefaultVisibleDates()
            Return BO.BAS.ConvertString2Date(hidD1.Value)
        End Get
        Set(value As Date)
            hidD1.Value = Format(value, "dd.MM.yyyy")
        End Set
    End Property
    Public Property CurrentD2 As Date
        Get
            If Me.hidD2.Value = "" Then SetupDefaultVisibleDates()
            Return BO.BAS.ConvertString2Date(hidD2.Value)
        End Get
        Set(value As Date)
            hidD2.Value = Format(value, "dd.MM.yyyy")
        End Set
    End Property

    Public ReadOnly Property VisibleStartDate As Date
        Get
            Return Me.CurrentD1
        End Get
    End Property
    Public ReadOnly Property VisibleEndDate As Date
        Get
            Return Me.CurrentD2
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Me.cal1
            .MultiViewColumns = BO.BAS.IsNullInt(Me.CalCols.SelectedValue)
            .MultiViewRows = 1
        End With

        If Not Page.IsPostBack Then
            If Page.Culture.IndexOf("Czech") >= 0 Or Page.Culture.IndexOf("Če") >= 0 Then
                cmdToday.Text = "Dnes"
            Else
                cmdToday.Text = "Today"
            End If

            If BO.BAS.IsNullDBDate(cal1.SelectedDate) Is Nothing Then
                Me.SelectedDate = Today
            End If

            SetupDefaultVisibleDates()



        End If
    End Sub

    Private Sub SetupDefaultVisibleDates()

        Me.CurrentD1 = cal1.SelectedDate.AddDays(-30)
        Me.CurrentD2 = cal1.SelectedDate.AddDays(Me.CurrentPocetMesicu * 30)
    End Sub

    'Public Sub AddHolidayDays(dates As List(Of BO.c26Holiday))
    '    For Each dat In dates
    '        Dim d As New RadCalendarDay
    '        d.Date = dat.c26Date
    '        d.ToolTip = dat.c26Name
    '        d.ItemStyle.BackColor = Drawing.Color.DarkOrange
    '        d.TemplateID = "holiday"
    '        cal1.SpecialDays.Add(d)
    '    Next
    'End Sub

    Private Sub cal1_DayRender(sender As Object, e As Calendar.DayRenderEventArgs) Handles cal1.DayRender
        If _lisHours Is Nothing Then InhaleHours()
        If _lisHours Is Nothing Then Return

        Dim strHoliday As String = ""
        If Not _lisHolidays Is Nothing Then
            If _lisHolidays.Where(Function(p) p.c26Date = e.Day.Date).Count > 0 Then
                strHoliday = _lisHolidays.Where(Function(p) p.c26Date = e.Day.Date)(0).c26Name
            End If
        End If

        Dim lis As IEnumerable(Of BO.p31WorksheetCalendarHours) = _lisHours.Where(Function(p) p.p31Date = e.Day.Date)
        If lis.Count = 0 Then
            If strHoliday = "" Then
                e.Cell.Text += "<div class='calcell'>-</div>"
            Else
                e.Cell.Text += "<div class='calcell' title='" & strHoliday & "'><img src='Images/holiday.png' border=0></div>"
            End If
        Else
            Dim s As String = ""
            If lis(0).Hours <> 0 Then
                s = BO.BAS.FN(lis(0).Hours)
            End If
            If lis(0).Moneys > 0 Then s += "<b style='color:blue;'>x</b>" '<span style='color:green;font-weight:bold;'>x</span>
            If lis(0).Pieces > 0 Then s += "<b style='color:green;'>x</b>" '<span style='color:green;font-weight:bold;'>x</span>
            If strHoliday = "" Then
                e.Cell.Text += "<div class='calcell'>" & s & "</div>"
            Else
                e.Cell.Text += "<div class='calcell' style='white-space:nowrap;' title='" & strHoliday & "'>" & s & "<img src='Images/holiday.png' border=0></div>"
            End If

        End If


    End Sub

    Private Sub InhaleHours()
        RaiseEvent NeedDataSource(_lisHours, _lisHolidays)
    End Sub

    Private Sub cal1_DefaultViewChanged(sender As Object, e As Calendar.DefaultViewChangedEventArgs) Handles cal1.DefaultViewChanged
        If cal1.SelectedDates.Count > 1 Then
            cal1.SelectedDate = cal1.SelectedDates(cal1.SelectedDates.Count - 1).Date
        End If
        With cal1.CalendarView
            Me.CurrentD1 = .ViewStartDate
            Me.CurrentD2 = .ViewEndDate
            If cal1.SelectedDate >= .ViewStartDate And cal1.SelectedDate <= .ViewEndDate Then
                'ok, aktuální datum je vidět
            Else
                Dim d As Date = .ViewStartDate.AddDays(15)  'změnit aktuální datum
                cal1.SelectedDate = DateSerial(Year(d), Month(d), 1)
                Handle_SelectedDateChanged(cal1.SelectedDate)
            End If
        End With
        
        RaiseEvent ViewChanged()
    End Sub

    Private Sub RefreshMatrix()
        cal1.SelectedDate = Today

        Me.CurrentD1 = Today.AddDays(-30)
        Me.CurrentD2 = Today.AddDays(Me.CurrentPocetMesicu * 31)
    End Sub

    Private Sub CalCols_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalCols.SelectedIndexChanged
        RefreshMatrix()
        RaiseEvent CalendarColumnsChanged(Me.CalendarColumns)
    End Sub

    

    Private Sub cal1_SelectionChanged(sender As Object, e As Calendar.SelectedDatesEventArgs) Handles cal1.SelectionChanged
        If e.SelectedDates.Count = 0 Then Return
        Dim d As Date = e.SelectedDates(0).Date
        Handle_SelectedDateChanged(d)
    
    End Sub

    Private Sub cmdToday_Click(sender As Object, e As EventArgs) Handles cmdToday.Click
        Me.SelectedDate = Today
        Me.CurrentD1 = cal1.CalendarView.ViewStartDate
        Me.CurrentD2 = cal1.CalendarView.ViewEndDate
        If Month(Me.SelectedDate) <> Month(cal1.CalendarView.ViewStartDate) Then
            RaiseEvent ViewChanged()
        End If


        Handle_SelectedDateChanged(Today)

    End Sub

    Private Sub Handle_SelectedDateChanged(d As Date)
        RaiseEvent SelectedDateChanged(d)
    End Sub
End Class