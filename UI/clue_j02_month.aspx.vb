Public Class clue_j02_month
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            ViewState("noclue") = Request.Item("noclue")

            If Master.DataPID = 0 Then
                Master.DataPID = Master.Factory.SysUser.j02ID
            End If
            

            Master.Factory.j03UserBL.InhaleUserParams("clue_j02_month-view", "clue_j02_month-query-month", "clue_j02_month-query-year")
            basUI.SelectRadiolistValue(Me.opgView, Master.Factory.j03UserBL.GetUserParam("clue_j02_month-view", "2"))

            Dim intMonth As Integer = BO.BAS.IsNullInt(Request.Item("month"))
            Dim intYear As Integer = BO.BAS.IsNullInt(Request.Item("year"))
            If ViewState("noclue") = "1" Then
                intMonth = Master.Factory.j03UserBL.GetUserParam("clue_j02_month-query-month", Month(Now).ToString)
                intYear = Master.Factory.j03UserBL.GetUserParam("clue_j02_month-query-year", Year(Now).ToString)
                period1.SelectedMonth = intMonth
                period1.SelectedYear = intYear
            End If
            
           
            Me.Mesic.Text = "Měsíc " & intMonth.ToString & "/" & intYear.ToString

            ViewState("d1") = DateSerial(intYear, intMonth, 1)
            
            RefreshRecord()

            RefreshData()

            
        End If
    End Sub

    Private Sub RefreshData()
        Dim d1 As Date = ViewState("d1")
        Dim s As New System.Text.StringBuilder
        Select Case Me.opgView.SelectedValue
            Case "1"
                s.Append("SELECT min(p28Name),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID")
                s.Append(" WHERE p34.p33ID=1 AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY p41.p28ID_Client")
                s.Append(" ORDER BY min(p28Name)")
                With plug1
                    .ColHeaders = "Klient|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "0"
                    .ColFlexSubtotals = "0|11|0|0|0"
                    .ColTypes = "S|N|N|S|D|D"
                End With
            Case "2"
                s.Append("SELECT min(p28Name),min(isnull(p41NameShort,p41Name)),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID")
                s.Append(" WHERE p34.p33ID=1 AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY a.p41ID")
                s.Append(" ORDER BY min(p28Name),min(isnull(p41NameShort,p41Name))")
                With plug1
                    .ColHeaders = "Klient|Projekt|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "1"
                    .ColFlexSubtotals = "1|0|11|0|0|0"
                    .ColTypes = "S|S|N|S|D|D"
                End With
            Case "3"
                s.Append("SELECT min(isnull(p41NameShort,p41Name)),min(p32Name),sum(p31Hours_Orig),convert(varchar(10),count(*))+'x',min(p31Date),max(p31Date)")
                s.Append(" FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID  INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID")
                s.Append(" LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID")
                s.Append(" WHERE p34.p33ID=1 AND a.j02ID=@j02id AND p31Date BETWEEN @d1 AND @d2")
                s.Append(" GROUP BY a.p41ID,a.p32ID")
                s.Append(" ORDER BY min(p28Name),min(p32Name)")
                With plug1
                    .ColHeaders = "Projekt|Aktivita|Hodiny||Od|Do"
                    .ColHideRepeatedValues = "1"
                    .ColFlexSubtotals = "1|0|11|0|0|0"
                    .ColTypes = "S|S|N|S|D|D"
                End With
        End Select

        With plug1

            .AddDbParameter("j02id", Master.DataPID)
            .AddDbParameter("d1", d1)
            .AddDbParameter("d2", d1.AddMonths(1).AddDays(-1))
            .GenerateTable(Master.Factory, s.ToString)
        End With
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .FullNameAsc
            If .IsClosed Then ph1.Font.Strikeout = True

        End With

    End Sub

    Private Sub opgView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgView.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("clue_j02_month-view", Me.opgView.SelectedValue)
        RefreshData()
    End Sub

    Private Sub clue_j02_month_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If ViewState("noclue") = "1" Then
            panContainer.Style.Clear()  'stránka nemá mít chování info bubliny
            panHeader.Visible = False
        Else
            Me.period1.Visible = False

        End If
    End Sub

  
  

   
    Private Sub period1_OnSelectedChanged() Handles period1.OnSelectedChanged
        Master.Factory.j03UserBL.SetUserParam("clue_j02_month-query-month", period1.SelectedMonth.ToString)
        Master.Factory.j03UserBL.SetUserParam("clue_j02_month-query-year", period1.SelectedYear.ToString)
        Server.Transfer("clue_j02_month.aspx?noclue=1")
    End Sub
End Class