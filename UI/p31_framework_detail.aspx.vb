Public Class p31_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm

    Public ReadOnly Property CurrentJ02ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.j02ID.SelectedValue)
        End Get
    End Property
    
    Private Sub p31_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        gridP31.Factory = Master.Factory
        gridP31.AllowApproving = False
        If Not Page.IsPostBack Then
            Master.SiteMenuValue = "p31_framework"
            Me.hidParentWidth.Value = BO.BAS.IsNullInt(Request.Item("parentWidth")).ToString
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p31_framework_detail-calendarcolumns")
                .Add("p31_framework_detail-j02id")
                .Add("p31_framework-timer")
                .Add("p31_framework-grid")
            End With
            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)
                cal1.CalendarColumns = BO.BAS.IsNullInt(.GetUserParam("p31_framework_detail-calendarcolumns", "1"))
                SetupComboPersons(.GetUserParam("p31_framework_detail-j02id"))
                Dim strDefIsGrid As String = "1", strDefIsTimer As String = "1"
                If basUI.GetCookieValue(Request, "MT50-SAW") = "1" Then
                    strDefIsGrid = "0" : strDefIsTimer = "0"
                End If
                Me.chkTimer.Checked = BO.BAS.BG(.GetUserParam("p31_framework-timer", strDefIsTimer))
                If basUI.GetCookieValue(Request, "MT50-SAW") = "1" Then
                    chkTimer.Checked = False
                    chkTimer.Visible = False
                End If

                Me.chkGrid.Checked = BO.BAS.BG(.GetUserParam("p31_framework-grid", strDefIsGrid))
            End With



            Dim intPID As Integer = BO.BAS.IsNullInt(Request.Item("pid"))
            If intPID > 0 Then
                Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(intPID)
                With cRec
                    cal1.SelectedDate = .p31Date
                    ''Me.p41ID.Value = .p41ID.ToString
                    ''If .p28ID_Client > 0 Then
                    ''    Me.p41ID.Text = .ClientName & " - " & .p41Name
                    ''Else
                    ''    Me.p41ID.Text = .p41Name
                    ''End If

                End With

            Else
                cal1.SelectedDate = Today
            End If

            With gridP31
                .MasterDataPID = Me.CurrentJ02ID
                .ExplicitDateFrom = cal1.SelectedDate
                .ExplicitDateUntil = cal1.SelectedDate
                .RecalcVirtualRowCount()
                .Rebind(False)
            End With

            cmdReport.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_X31_Personal)

            If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then
                Me.p41ID.J02ID_Explicit = Me.CurrentJ02ID
            End If
        End If
        gridP31.MasterDataPID = Me.CurrentJ02ID

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

    Private Sub cal1_CalendarColumnsChanged(intCalendarColumns As Integer) Handles cal1.CalendarColumnsChanged
        Master.Factory.j03UserBL.SetUserParam("p31_framework_detail-calendarcolumns", cal1.CalendarColumns.ToString)
        ReloadPage("")
    End Sub

    
    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        If Me.hidHardRefreshFlag.Value = "" And Me.hidHardRefreshPID.Value = "" Then Return
        
        ReloadPage(Me.hidHardRefreshPID.Value)
    End Sub

    Private Sub ReloadPage(strPID As String)
        Response.Redirect("p31_framework_detail.aspx?pid=" & strPID)
    End Sub

    Private Sub cal1_NeedDataSource(ByRef lisGetMeWorksheetHours As IEnumerable(Of BO.p31WorksheetCalendarHours), ByRef lisHolidays As IEnumerable(Of BO.c26Holiday)) Handles cal1.NeedDataSource
        lisGetMeWorksheetHours = Master.Factory.p31WorksheetBL.GetList_CalendarHours(Me.CurrentJ02ID, cal1.VisibleStartDate, cal1.VisibleEndDate)

        Dim mq As New BO.myQuery
        mq.DateFrom = cal1.VisibleStartDate.AddDays(-20)
        mq.DateUntil = cal1.VisibleEndDate.AddDays(20)
        lisHolidays = Master.Factory.c26HolidayBL.GetList(mq)
        

    End Sub

    Private Sub cal1_SelectedDateChanged(datSelected As Date) Handles cal1.SelectedDateChanged
        With gridP31
            .ExplicitDateFrom = cal1.SelectedDate
            .ExplicitDateUntil = cal1.SelectedDate
            .RecalcVirtualRowCount()
            .Rebind(True)
        End With
    End Sub

    Private Sub p31_framework_detail_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

        Me.cmdNew.InnerText = String.Format(Resources.p31_framework_detail.cmdNewP31, BO.BAS.FD(cal1.SelectedDate))
        'If cal1.CalendarColumns > 1 And Month(cal1.SelectedDate) <> Month(cal1.VisibleStartDate) Then
        '    RefreshStatistic()
        'End If
        RefreshStatistic()
        Me.clue_timesheet.Attributes("rel") = "clue_j02_month.aspx?pid=" & Me.CurrentJ02ID.ToString & "&month=" & Month(Me.GetReportingDate()).ToString & "&year=" & Year(Me.GetReportingDate()).ToString
        If Me.CurrentJ02ID <> Master.Factory.SysUser.j02ID Then
            Me.j02ID.BackColor = Drawing.Color.Red
        Else
            Me.j02ID.BackColor = Nothing
        End If
    End Sub

    Private Function GetReportingDate() As Date
        'Dim d As Date = Me.cal1.VisibleStartDate.AddDays(8)
        'If cal1.CalendarColumns > 1 Then d = Me.cal1.SelectedDate
        Dim d As Date = Me.cal1.SelectedDate

        Return DateSerial(Year(d), Month(d), 1)

    End Function
    Private Sub RefreshStatistic()
        Dim mq As New BO.myQueryP31
        
        mq.DateFrom = GetReportingDate()
        mq.DateUntil = mq.DateFrom.AddMonths(1).AddDays(-1)
        mq.j02ID = Me.CurrentJ02ID



        StatHeader.Text = String.Format(Resources.p31_framework_detail.StatHeader, Format(mq.DateFrom, "MM-yyyy"))

        Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(Me.CurrentJ02ID)
        Dim cRec As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, False)
        With cRec
            Me.Hours_All.Text = BO.BAS.FN(.p31Hours_Orig)
            Me.Hours_Billable.Text = BO.BAS.FN(.Hours_Orig_Billable)
            ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            ''    Me.Hours_Billable.Text = BO.BAS.FN(.Hours_Orig_Billable)
            ''Else
            ''    Hours_Billable.Visible = False : lblHours_Billable.Visible = False
            ''End If
        End With
        If Not cJ02 Is Nothing Then
            Dim dblFond As Double = Master.Factory.c21FondCalendarBL.GetSumHours(cJ02.c21ID, cJ02.j17ID, mq.DateFrom, mq.DateUntil)
            Me.Fond_Hours.Text = BO.BAS.FN(dblFond)
            Util_Total.Text = BO.BAS.FN(100 * cRec.p31Hours_Orig / dblFond) & "%"
            Util_Billable.Text = BO.BAS.FN(100 * cRec.Hours_Orig_Billable / dblFond) & "%"
            ''If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            ''    Util_Billable.Text = BO.BAS.FN(100 * cRec.Hours_Orig_Billable / dblFond) & "%"
            ''Else
            ''    Util_Billable.Visible = False : lblUtil_Billable.Visible = False
            ''End If

        End If

    End Sub

    'Private Sub cal1_ViewChanged() Handles cal1.ViewChanged
    '    RefreshStatistic()
    'End Sub

    'Private Sub j02ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j02ID.SelectedIndexChanged
    '    Master.Factory.j03UserBL.SetUserParam("p31_framework_detail-j02id", Me.j02ID.SelectedValue)
    'End Sub

    ''Private Sub chkTimer_CheckedChanged(sender As Object, e As EventArgs) Handles chkTimer.CheckedChanged
    ''    Master.Factory.j03UserBL.SetUserParam("p31_framework-timer", BO.BAS.GB(Me.chkTimer.Checked))
    ''End Sub

    Private Sub cal1_ViewChanged() Handles cal1.ViewChanged

    End Sub

    
End Class