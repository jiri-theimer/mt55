Public Class clue_j02_capacity
    Inherits System.Web.UI.Page

    Private Class PersonCapacity
        Public Property Rok As Integer
        Public Property Mesic As Integer
        Public Property Fond As Double
        Public Property HodinyFa As Double
        Public Property HodinyNefa As Double
        Public Property PlanMimoProjekt As Double
        Public Property PlanVcProjektu As Double

        Public Property OperPlanFa As Double
        Public Property OperPlanNefa As Double

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            ViewState("p41id") = Request.Item("p41id")

            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)

            RefreshRecord()

        End If
    End Sub


    Private Sub RefreshRecord()
        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .FullNameAsc
            If .IsClosed Then ph1.Font.Strikeout = True
           
        End With
        RefreshCapacity(cRec)
    End Sub

    Private Sub RefreshCapacity(cJ02 As BO.j02Person)
        Dim intP41ID As Integer = BO.BAS.IsNullInt(ViewState("p41id"))

        Dim cRec As BO.p41Project = Nothing
        If intP41ID <> 0 Then
            cRec = Master.Factory.p41ProjectBL.Load(intP41ID)
            Me.Project.Text = cRec.FullName
        End If


        Dim d1 As Date = DateSerial(Year(Today), 1, 1), d2 As Date = DateSerial(Year(Now), 12, 31)
        If Not BO.BAS.IsNullDBDate(cRec.p41PlanFrom) Is Nothing Then
            d1 = cRec.p41PlanFrom.Value
            d1 = DateSerial(Year(d1), Month(d1), 1)
            d2 = cRec.p41PlanUntil.Value
            d2 = DateSerial(Year(d2), Month(d2), 1).AddMonths(1).AddDays(-1)
        End If
        Dim lis As New List(Of PersonCapacity), d As Date = d1
        While d <= d2
            Dim c As New PersonCapacity()
            c.Mesic = d.Month
            c.Rok = d.Year
            lis.Add(c)
            d = d.AddMonths(1)
        End While

        Dim lisFond As IEnumerable(Of BO.FondHours) = Master.Factory.c21FondCalendarBL.GetSumHoursPerMonth(cJ02.c21ID, cJ02.j17ID, d1, d2)
        For Each cFond In lisFond
            lis.Where(Function(p) p.Mesic = cFond.Mesic And p.Rok = cFond.Rok)(0).Fond = cFond.Hodiny
        Next
        Dim lisHours As IEnumerable(Of BO.HoursInMonth) = Master.Factory.p31WorksheetBL.GetSumHoursPerMonth(cJ02.PID, d1, d2)
        For Each cHours In lisHours
            lis.Where(Function(p) p.Mesic = cHours.Mesic And p.Rok = cHours.Rok)(0).HodinyFa = cHours.HodinyFa
            lis.Where(Function(p) p.Mesic = cHours.Mesic And p.Rok = cHours.Rok)(0).HodinyNefa = cHours.HodinyNefa
        Next
        Dim mq As New BO.myQueryP47
        mq.j02ID = cJ02.PID
        mq.DateFrom = d1
        mq.DateUntil = d2
        Dim lisPlan As IEnumerable(Of BO.p47CapacityPlan) = Master.Factory.p47CapacityPlanBL.GetList(mq)

        For Each cPlan In lisPlan.GroupBy(Function(p) p.p47DateFrom)
            Dim lisFound As IEnumerable(Of PersonCapacity) = lis.Where(Function(p) p.Mesic = Month(cPlan(0).p47DateFrom) And p.Rok = Year(cPlan(0).p47DateFrom))
            If lisFound.Count > 0 Then
                lisFound(0).PlanVcProjektu = cPlan.Sum(Function(p) p.p47HoursTotal)
            End If
        Next
        If intP41ID <> 0 Then
            For Each cPlan In lisPlan.Where(Function(p) p.p41ID <> cRec.PID).GroupBy(Function(p) p.p47DateFrom)
                Dim lisFound As IEnumerable(Of PersonCapacity) = lis.Where(Function(p) p.Mesic = Month(cPlan(0).p47DateFrom) And p.Rok = Year(cPlan(0).p47DateFrom))
                If lisFound.Count > 0 Then
                    lisFound(0).PlanMimoProjekt = cPlan.Sum(Function(p) p.p47HoursTotal)
                End If
            Next
        End If
        

        rp1.DataSource = lis
        rp1.DataBind()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As PersonCapacity = CType(e.Item.DataItem, PersonCapacity)
        CType(e.Item.FindControl("Mesic"), Label).Text = cRec.Mesic.ToString & "/" & cRec.Rok.ToString
        CType(e.Item.FindControl("Fond"), Label).Text = FN(cRec.Fond)
        CType(e.Item.FindControl("HodinyFa"), Label).Text = FN(cRec.HodinyFa)
        CType(e.Item.FindControl("HodinyNeFa"), Label).Text = FN(cRec.HodinyNefa)
        CType(e.Item.FindControl("HodinyCelkem"), Label).Text = FN(cRec.HodinyFa + cRec.HodinyNefa)
        CType(e.Item.FindControl("PlanVcProjektu"), Label).Text = FN(cRec.PlanVcProjektu)
        CType(e.Item.FindControl("PlanMimoProjekt"), Label).Text = FN(cRec.PlanMimoProjekt)

    End Sub

    Private Function FN(dbl As Double) As String
        If dbl = 0 Then
            Return ""
        Else
            Return BO.BAS.IsNullNum(dbl)
        End If
    End Function

End Class