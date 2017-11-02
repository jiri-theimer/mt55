Public Class clue_j02_oplan
    Inherits System.Web.UI.Page

    Public Class PlanRow
        Public Property Project As String
        Public Property p41ID As Integer
        Public Property p47Hours As Double
        Public Property p48Hours As Double
        Public Property p31Hours As Double

    End Class

  
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("noclue") = Request.Item("noclue")
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.DataPID = Master.Factory.SysUser.j02ID


            Dim intMonth As Integer = BO.BAS.IsNullInt(Request.Item("month"))
            Dim intYear As Integer = BO.BAS.IsNullInt(Request.Item("year"))
            If ViewState("noclue") = "1" Then
                intMonth = Master.Factory.j03UserBL.GetUserParam("clue_j02_oplan-query-month", Month(Now).ToString)
                intYear = Master.Factory.j03UserBL.GetUserParam("clue_j02_oplan-query-year", Year(Now).ToString)
                period1.SelectedMonth = intMonth
                period1.SelectedYear = intYear
            End If
            Me.Mesic.Text = "Měsíc " & intMonth.ToString & "/" & intYear.ToString

            RefreshRecord()

            RefreshData(DateSerial(intYear, intMonth, 1))
        End If
    End Sub

    

    Private Sub RefreshRecord()
        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .FullNameAsc
            If .IsClosed Then ph1.Font.Strikeout = True

        End With

    End Sub

    Private Sub RefreshData(d1 As Date)
        Dim lis As New List(Of PlanRow)

        Dim d2 As Date = d1.AddMonths(1).AddDays(-1), j02ids As New List(Of Integer)
        j02ids.Add(Master.DataPID)
        Dim mqP47 As New BO.myQueryP47
        mqP47.DateFrom = d1
        mqP47.DateUntil = d2
        mqP47.j02ID = Master.DataPID
        Dim lisP47 As IEnumerable(Of BO.p47CapacityPlan) = Master.Factory.p47CapacityPlanBL.GetList(mqP47)
        For Each c In lisP47.GroupBy(Function(p) p.p41ID)
            Dim cc As New PlanRow()
            cc.p41ID = c.First.p41ID
            cc.p47Hours = c.Sum(Function(p) p.p47HoursTotal)
            cc.Project = c.First.Project
            lis.Add(cc)
        Next

        Dim mqP48 As New BO.myQueryP48
        mqP48.DateFrom = d1
        mqP48.DateUntil = d2
        mqP48.j02IDs = j02ids
        Dim lisP48 As IEnumerable(Of BO.p48OperativePlan) = Master.Factory.p48OperativePlanBL.GetList(mqP48)
        For Each c In lisP48.GroupBy(Function(p) p.p41ID)
            Dim cc As New PlanRow()
            cc.p41ID = c.First.p41ID
            cc.p48Hours = c.Sum(Function(p) p.p48Hours)
            cc.Project = c.First.Project
            lis.Add(cc)
        Next

        Dim mqP31 As New BO.myQueryP31
        mqP31.DateFrom = d1
        mqP31.DateUntil = d2
        mqP31.j02ID = Master.DataPID
        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mqP31)
        For Each c In lisP31.GroupBy(Function(p) p.p41ID)
            Dim cc As New PlanRow()
            cc.p41ID = c.First.p41ID
            cc.p31Hours = c.Sum(Function(p) p.p31Hours_Orig)
            cc.Project = c.First.p41Name
            lis.Add(cc)
        Next

        Dim lis2 As New List(Of PlanRow)
        For Each c In lis.GroupBy(Function(p) p.p41ID)
            Dim cc As New PlanRow
            cc.Project = c.First.Project
            cc.p47Hours = c.Sum(Function(p) p.p47Hours)
            cc.p48Hours = c.Sum(Function(p) p.p48Hours)
            cc.p31Hours = c.Sum(Function(p) p.p31Hours)
            lis2.add(cc)
        Next

        rp1.DataSource = lis2
        rp1.DataBind()

        p47Total.Text = BO.BAS.FN2(lis2.Sum(Function(p) p.p47Hours))
        p48Total.Text = BO.BAS.FN2(lis2.Sum(Function(p) p.p48Hours))
        p31Total.Text = BO.BAS.FN2(lis2.Sum(Function(p) p.p31Hours))
    End Sub

    Private Sub period1_OnSelectedChanged() Handles period1.OnSelectedChanged
        Master.Factory.j03UserBL.SetUserParam("clue_j02_oplan-query-month", period1.SelectedMonth.ToString)
        Master.Factory.j03UserBL.SetUserParam("clue_j02_oplan-query-year", period1.SelectedYear.ToString)
        Server.Transfer("clue_j02_oplan.aspx?noclue=1")
    End Sub

    Private Sub clue_j02_oplan_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If ViewState("noclue") = "1" Then
            panContainer.Style.Clear()  'stránka nemá mít chování info bubliny
            panHeader.Visible = False
        Else
            Me.period1.Visible = False

        End If
    End Sub
End Class