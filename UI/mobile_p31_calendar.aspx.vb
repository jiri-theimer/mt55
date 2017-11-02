Public Class mobile_p31_calendar
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_p31_calendar_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.MenuPrefix = "p31"
        With Me.p41id.radComboBoxOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p41_framework_detail-pid")
                .Add("p28_framework_detail-pid")
                .Add("p91_framework_detail-pid")
            End With

            If Master.DataPID > 0 Then
                Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
                With cRec
                    cal1.SelectedDate = .p31Date

                End With

            Else
                cal1.SelectedDate = Today
            End If
            With Master.Factory
                .j03UserBL.InhaleUserParams(lisPars)
                
            End With
            RefreshRecord()
            SetupProjectList()
            RefreshP31List()
            RefreshStatistic()
        End If
    End Sub

    Private Sub SetupProjectList()
        Dim mqP41 As New BO.myQueryP41
        With mqP41
            .Closed = BO.BooleanQueryMode.FalseQuery
            .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        End With
        Dim intVirtualCount As Integer = Master.Factory.p41ProjectBL.GetVirtualCount(mqP41)
        If intVirtualCount > 11 Then
            'je třeba nabízet pouze TOP 10
            mqP41.PIDs = Master.Factory.p41ProjectBL.GetTopProjectsByWorksheetEntry(Master.Factory.SysUser.j02ID, 10)
            If mqP41.PIDs.Count = 0 Then    'zatím nenapsal žádné úkony
                mqP41.TopRecordsOnly = 10
            End If
        End If
        Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mqP41)
        rp1.DataSource = lisP41
        rp1.DataBind()

    End Sub

    Private Sub cal1_NeedDataSource(ByRef lisGetMeWorksheetHours As IEnumerable(Of BO.p31WorksheetCalendarHours), ByRef lisHolidays As IEnumerable(Of BO.c26Holiday)) Handles cal1.NeedDataSource
        lisGetMeWorksheetHours = Master.Factory.p31WorksheetBL.GetList_CalendarHours(Master.Factory.SysUser.j02ID, cal1.VisibleStartDate, cal1.VisibleEndDate)

        Dim mq As New BO.myQuery
        mq.DateFrom = cal1.VisibleStartDate.AddDays(-20)
        mq.DateUntil = cal1.VisibleEndDate.AddDays(20)
        lisHolidays = Master.Factory.c26HolidayBL.GetList(mq)


    End Sub

    Private Sub RefreshP31List()
        Dim mq As New BO.myQueryP31
        mq.j02ID = Master.Factory.SysUser.j02ID
        mq.DateFrom = cal1.SelectedDate
        mq.DateUntil = cal1.SelectedDate
        mq.MG_SortString = "p31Date DESC"

       

        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        Dim strHeader As String = BO.BAS.FD(mq.DateFrom, False, True) & " | " & lis.Count.ToString & "x"
        list1.RefreshData(lis, strHeader)
    End Sub

    Private Sub cal1_SelectedDateChanged(datSelected As Date) Handles cal1.SelectedDateChanged
        RefreshStatistic()
        RefreshP31List()
    End Sub

    Private Sub mobile_p31_calendar_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.lblCurDate.Text = BO.BAS.FD(cal1.SelectedDate)
        Me.hidCurDate.Value = Format(cal1.SelectedDate, "dd.MM.yyyy")
    End Sub

    Private Sub RefreshStatistic()
        Dim mq As New BO.myQueryP31

        mq.DateFrom = DateSerial(Year(cal1.SelectedDate), Month(cal1.SelectedDate), 1)
        mq.DateUntil = mq.DateFrom.AddMonths(1).AddDays(-1)
        mq.j02ID = Master.Factory.SysUser.j02ID

        Dim cRec As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, False)
        With cRec
            Me.Hours_All.Text = BO.BAS.FN(.p31Hours_Orig) & "h."
            
        End With
      

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p41Project = CType(e.Item.DataItem, BO.p41Project)
        With CType(e.Item.FindControl("link1"), HyperLink)
            .NavigateUrl = "javascript:p31_entry(" & cRec.PID.ToString & ")"
            .Text = cRec.ProjectWithMask(Master.Factory.SysUser.j03ProjectMaskIndex)
        End With

    End Sub

    Private Sub RefreshRecord()
        Dim c As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadMyLastCreated(False, 0)
        If c Is Nothing Then
            Me.LastWorksheet.Text = "Zatím jsem nezapsal WORKSHEET úkon."
        Else
            Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(c.p41ID)
            With c
                Me.LastWorksheet.Text = BO.BAS.FD(.p31Date) & "/" & .ClientName & "/" & cP41.PrefferedName & "/" & c.p32Name
                Me.LastWorksheet.NavigateUrl = "mobile_p31_framework.aspx?source=calendar&pid=" & c.PID.ToString
                Me.LastWorksheet.ToolTip = c.p31Text
            End With
        End If
    End Sub
End Class