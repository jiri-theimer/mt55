Public Class clue_p41_myworksheet
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            ViewState("parent_url_reload") = Request.Item("parent_url_reload")
            ViewState("j02id") = Request.Item("j02id")
            If ViewState("j02id") = "" Then ViewState("j02id") = Master.Factory.SysUser.j02ID.ToString
            Master.Factory.j03UserBL.InhaleUserParams("clue_p41_myworksheet-view", "clue_p41_myworksheet-query-month", "clue_p41_myworksheet-query-year")
            basUI.SelectRadiolistValue(Me.opgView, Master.Factory.j03UserBL.GetUserParam("clue_p41_myworksheet-view", "2"))

            Dim intMonth As Integer = BO.BAS.IsNullInt(Request.Item("month"))
            Dim intYear As Integer = BO.BAS.IsNullInt(Request.Item("year"))

            intMonth = Master.Factory.j03UserBL.GetUserParam("clue_p41_myworksheet-query-month", Month(Now).ToString)
            intYear = Master.Factory.j03UserBL.GetUserParam("clue_p41_myworksheet-query-year", Year(Now).ToString)
            period1.SelectedMonth = intMonth
            period1.SelectedYear = intYear
            ''End If
            Me.Me.Text = Master.Factory.SysUser.Person & ":"



            ViewState("d1") = DateSerial(intYear, intMonth, 1)

            RefreshRecord()

            ''comments1.RefreshData(Master.Factory, BO.x29IdEnum.p41Project, Master.DataPID)
        End If
    End Sub
    Private Sub RefreshRecord()

        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        With cRec
            ph1.Text = .FullName
        End With
        

        Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Master.Factory.p30Contact_PersonBL.GetList(0, Master.DataPID, 0)
        If lisP30.Count > 0 Then
            Me.persons1.FillData(lisP30, False)
        Else
            panP30.Visible = False
        End If

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, cRec.PID)
        Me.roles_project.RefreshData(lisX69, cRec.PID)

        RefreshData()

    End Sub
    Private Sub opgView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgView.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("clue_p41_myworksheet-view", Me.opgView.SelectedValue)
        RefreshData()
    End Sub

   





    Private Sub period1_OnSelectedChanged() Handles period1.OnSelectedChanged
        Master.Factory.j03UserBL.SetUserParam("clue_p41_myworksheet-query-month", period1.SelectedMonth.ToString)
        Master.Factory.j03UserBL.SetUserParam("clue_p41_myworksheet-query-year", period1.SelectedYear.ToString)
        Server.Transfer("clue_p41_myworksheet.aspx?pid=" & Master.DataPID.ToString)
    End Sub


 
    Private Sub RefreshData()
        Dim d1 As Date = ViewState("d1")
        Dim s As New System.Text.StringBuilder
        Select Case Me.opgView.SelectedValue
            Case "1"
                s.Append("SELECT min(p34Name),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" WHERE p34.p33ID=1 AND a.p41ID=@p41id AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY p32.p34ID")
                s.Append(" ORDER BY min(p34Ordinary)")
                With plug1
                    .ColHeaders = "Sešit|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "0"
                    .ColFlexSubtotals = "0|11|0|0|0"
                    .ColTypes = "S|N|S|D|D"
                End With
            Case "2"
                s.Append("SELECT min(p34Name),min(p32Name),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" WHERE p34.p33ID=1 AND a.p41ID=@p41id AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY a.p32ID")
                s.Append(" ORDER BY min(p34Ordinary),min(p32Ordinary)")
                With plug1
                    .ColHeaders = "Sešit|Aktivita|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "1"
                    .ColFlexSubtotals = "1|0|11|0|0|0"
                    .ColTypes = "S|S|N|S|D|D"
                End With
            Case "3"
                s.Append("SELECT min(p57Name+': '+p56Name+' ('+p56Code+')'),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID LEFT OUTER JOIN p57TaskType p57 ON p56.p57ID=p57.p57ID")
                s.Append(" WHERE p34.p33ID=1 AND a.p41ID=@p41id AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY a.p56ID")
                s.Append(" ORDER BY min(p57Name),min(p56Name)")
                With plug1
                    .ColHeaders = "Úkol|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "1"
                    .ColFlexSubtotals = "0|11|0|0|0"
                    .ColTypes = "S|N|S|D|D"
                End With
        End Select

        With plug1
            .AddDbParameter("p41id", Master.DataPID)
            .AddDbParameter("j02id", ViewState("j02id"))
            .AddDbParameter("d1", d1)
            .AddDbParameter("d2", d1.AddMonths(1).AddDays(-1))
            .GenerateTable(Master.Factory, s.ToString)
        End With
    End Sub
End Class