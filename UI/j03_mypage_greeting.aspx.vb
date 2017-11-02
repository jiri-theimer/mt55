Public Class j03_mypage_greeting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cal1.factory = Master.Factory
        rec1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            lblBuild.Text = "Verze: " & BO.ASS.GetUIVersion()

            Master.SiteMenuValue = "dashboard"
            If Master.Factory.SysUser.j02ID > 0 Then
                Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.Factory.SysUser.j02ID)
                lblHeader.Text = cRec.j02FirstName & ", vítejte v"
            End If

            Dim lisPars As New List(Of String)
            With lisPars
                .Add("j03_mypage_greeting-last_step")
                .Add("j03_mypage_greeting-chkP41")
                .Add("j03_mypage_greeting-chkP28")
                .Add("j03_mypage_greeting-chkP91")
                .Add("j03_mypage_greeting-chkO23")
                .Add("j03_mypage_greeting-chkP56")
                .Add("j03_mypage_greeting-chkJ02")
                .Add("j03_mypage_greeting-chkShowCharts")
                .Add("j03_mypage_greeting-chkSearch")
                .Add("j03_mypage_greeting-cbxP56Types")
                .Add("j03_mypage_greeting-chkLog")
                .Add("j03_mypage_greeting-chkScheduler")
                .Add("j03_mypage_greeting-chkX18")
                .Add("myscheduler-maxtoprecs-j02")
                .Add("myscheduler-numberofdays-j02")
                .Add("myscheduler-firstday")
            End With

            With Master.Factory
                cmdReadUpgradeInfo.Visible = .SysUser.j03IsShallReadUpgradeInfo

                .j03UserBL.InhaleUserParams(lisPars)
                chkLog.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkLog", "1"))
                chkP41.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkP41", "1"))
                chkP28.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkP28", "1"))
                chkP91.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkP91", "0"))
                chkO23.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkO23", "0"))
                chkP56.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkP56", "0"))
                chkJ02.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkJ02", "0"))
                chkSearch.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkSearch", "1"))
                chkShowCharts.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkShowCharts", "1"))
                chkScheduler.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkScheduler", "1"))
                cal1.MaxTopRecs = BO.BAS.IsNullInt(.j03UserBL.GetUserParam("myscheduler-maxtoprecs-j02", "10"))
                cal1.NumberOfDays = BO.BAS.IsNullInt(.j03UserBL.GetUserParam("myscheduler-numberofdays-j02", "10"))
                cal1.FirstDayMinus = BO.BAS.IsNullInt(.j03UserBL.GetUserParam("myscheduler-firstday", "-1"))
                chkX18.Checked = BO.BAS.BG(.j03UserBL.GetUserParam("j03_mypage_greeting-chkX18", "0"))
             

                panSearch_j02.Visible = .SysUser.j04IsMenu_People
                panSearch_p28.Visible = .SysUser.j04IsMenu_Contact
                panSearch_p91.Visible = .SysUser.j04IsMenu_Invoice
                panSearch_P41.Visible = .SysUser.j04IsMenu_Project

                panSearch.Visible = (panSearch_j02.Visible Or panSearch_p28.Visible Or panSearch_p91.Visible Or panSearch_p56.Visible Or panSearch_P41.Visible)
                chkSearch.Visible = panSearch.Visible

               

                linkFulltext.Visible = .TestPermission(BO.x53PermValEnum.GR_Admin)
                If Not linkFulltext.Visible Then linkFulltext.Visible = .TestPermission(BO.x53PermValEnum.GR_P31_Reader)
                If Not linkFulltext.Visible Then linkFulltext.Visible = .TestPermission(BO.x53PermValEnum.GR_P41_Reader)


                If chkSearch.Visible Then

                    panSearch.Visible = chkSearch.Checked
                Else
                    chkSearch.Checked = False
                End If


            End With

            With Master.Factory.j03UserBL
                Select Case .GetUserParam("j03_mypage_greeting-last_step", "0")
                    Case "0"
                        ShowImage()
                        .SetUserParam("j03_mypage_greeting-last_step", "1")
                    Case "1"
                        ShowChart1("1")
                        .SetUserParam("j03_mypage_greeting-last_step", "2")
                    Case "2"
                        ShowChart2("2")
                        .SetUserParam("j03_mypage_greeting-last_step", "3")
                    Case "3"
                        ShowChart2("3")
                        .SetUserParam("j03_mypage_greeting-last_step", "4")
                    Case "4"
                        ShowChart2("4")
                        .SetUserParam("j03_mypage_greeting-last_step", "5")
                    Case "5"
                        ShowChart1("3")
                        .SetUserParam("j03_mypage_greeting-last_step", "6")
                    Case "6"
                        ShowChart2("5")
                        .SetUserParam("j03_mypage_greeting-last_step", "0")
                End Select

            End With

            
            RefreshBoxes()

            RefreshX47Log()

            'If basUI.GetCookieValue(Request, "MT50-SAW") = "1" Then
            '    menu1.Visible = False
            'End If
            If chkScheduler.Checked Then
                cal1.RecordPID = Master.Factory.SysUser.j02ID
                cal1.RefreshData(Today)
                cal1.RefreshTasksWithoutDate(True)
            End If


        Else
            If chkScheduler.Checked Then cal1.RecordPID = Master.Factory.SysUser.j02ID
        End If

        Dim cJ04 As BO.j04UserRole = Master.Factory.j04UserRoleBL.Load(Master.Factory.SysUser.j04ID)
        If Len(cJ04.j04DashboardHtml) > 0 Then
            place_j04DashboardHtml.Controls.Add(New LiteralControl(cJ04.j04DashboardHtml))
        End If

        RefreshX18()
    End Sub

    Private Sub ShowImage()
        If Not chkShowCharts.Checked Then Return
        imgWelcome.Visible = True
        If Request.Item("image") <> "" Then
            imgWelcome.ImageUrl = "Images/Welcome/" & Request.Item("image")
            Return
        End If
        Dim cF As New BO.clsFile
        Dim lisFiles As List(Of String) = cF.GetFileListFromDir(BO.ASS.GetApplicationRootFolder & "\Images\Welcome", "*.*")
        Dim intCount As Integer = lisFiles.Count - 1

        Randomize()
        Dim x As Integer = CInt(Rnd() * 110), strPreffered As String = ""
        If x > intCount Then x = intCount


        'If x > intCount And Now.Hour > 18 Then strPreffered = "19422837_s.jpg" 'rodina
        'If x > intCount And Now.Hour > 19 Then strPreffered = "10694994_s.jpg" 'pivo
        If x > intCount And (Now.Hour > 22 Or Now.Hour <= 6) Then strPreffered = "16805586_s.jpg" 'postel
        'If x > intCount And Now.Hour >= 12 And Now.Hour <= 13 Then strPreffered = "7001764_s.jpg" 'oběd
        If x > intCount And strPreffered = "" And (Weekday(Now, Microsoft.VisualBasic.FirstDayOfWeek.Monday) = 3 Or Weekday(Now, Microsoft.VisualBasic.FirstDayOfWeek.Monday) = 1) Then
            strPreffered = "work.jpng"
        End If
        If strPreffered <> "" Then
            If System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\Images\Welcome\" & strPreffered) Then
                imgWelcome.ImageUrl = "Images/welcome/" & strPreffered
                Return
            End If
        End If
        If x >= 0 And x <= intCount Then
            imgWelcome.ImageUrl = "Images/welcome/" & lisFiles(x)
        End If

    End Sub

    ''Private Sub RefreshFavourites()
    ''    Dim mq As New BO.myQueryP41
    ''    mq.IsFavourite = BO.BooleanQueryMode.TrueQuery
    ''    Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
    ''    If lisP41.Count > 0 Then
    ''        menu1.FindItemByValue("favourites").Visible = True
    ''        With menu1.FindItemByValue("favourites").Items
    ''            For Each c In lisP41
    ''                Dim link1 As New Telerik.Web.UI.RadPanelItem(c.FullName, "p41_framework.aspx?pid=" & c.PID.ToString)
    ''                link1.ImageUrl = "Images/project.png"
    ''                .Add(link1)
    ''            Next
    ''        End With
    ''        menu1.FindItemByValue("favourites").Text += " (" & lisP41.Count.ToString & ")"
    ''    Else
    ''        menu1.Visible = False
    ''    End If
    ''End Sub

    Private Sub RefreshBoxes()
        
        
        
        Dim lisO23 As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        If lisO23.Count > 0 Then
            Me.panO23.Visible = True
            Me.o23Count.Text = lisO23.Count.ToString
            rpO23.DataSource = lisO23
            rpO23.DataBind()
        Else
            Me.panO23.Visible = False
        End If
        rpP39.DataSource = Master.Factory.p40WorkSheet_RecurrenceBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        rpP39.DataBind()
        If rpP39.Items.Count = 0 Then
            panP39.Visible = False
        Else
            p39Count.Text = rpP39.Items.Count.ToString
            panP39.Visible = True
        End If

        ''Dim mqP48 As New BO.myQueryP48
        ''mqP48.j02ID_Owner = Master.Factory.SysUser.j02ID
        ''mqP48.DateFrom = Now.AddDays(-4)
        ''mqP48.DateUntil = Now.AddDays(1)

        ''Dim lisP48 As IEnumerable(Of BO.p48OperativePlan) = Master.Factory.p48OperativePlanBL.GetList(mqP48).Where(Function(p) p.p31ID = 0).OrderBy(Function(p) p.p48Date)
        ''If lisP48.Count > 0 Then
        ''    Me.panP48.Visible = True
        ''    Me.p48Count.Text = lisP48.Count.ToString
        ''    rpP48.DataSource = lisP48
        ''    rpP48.DataBind()
        ''Else
        ''    Me.panP48.Visible = False
        ''End If
    End Sub

    

    

    

    Private Sub rpO23_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO23.ItemDataBound
        Dim cRec As BO.o23Doc = CType(e.Item.DataItem, BO.o23Doc)
        With CType(e.Item.FindControl("link1"), HyperLink)
            .Text = cRec.x23Name & ": "
            If cRec.o23Name <> "" Then
                .Text += cRec.o23Name
            Else
                .Text += cRec.o23Code
            End If
            .NavigateUrl = "o23_framework.aspx?pid=" & cRec.PID.ToString
            If cRec.IsClosed Then .Font.Strikeout = True

        End With
        With CType(e.Item.FindControl("clue1"), HyperLink)
            .Attributes.Item("rel") = "clue_o23_record.aspx?&pid=" & cRec.PID.ToString
        End With

        
    End Sub

    Private Sub rpP39_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP39.ItemDataBound
        Dim cRec As BO.p39WorkSheet_Recurrence_Plan = CType(e.Item.DataItem, BO.p39WorkSheet_Recurrence_Plan)
        With CType(e.Item.FindControl("cmdProject"), HyperLink)
            .Text = cRec.p41Name
            If cRec.p28Name <> "" Then .Text = cRec.p28Name & " - " & cRec.p41Name
            .NavigateUrl = "p41_framework.aspx?pid=" & cRec.p41ID.ToString
        End With
        CType(e.Item.FindControl("p39DateCreate"), Label).Text = BO.BAS.FD(cRec.p39DateCreate, True)
        CType(e.Item.FindControl("p39Date"), Label).Text = BO.BAS.FD(cRec.p39Date)
        CType(e.Item.FindControl("p39Text"), Label).Text = cRec.p39Text

    End Sub

    ''Private Sub rpP48_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP48.ItemDataBound
    ''    Dim cRec As BO.p48OperativePlan = CType(e.Item.DataItem, BO.p48OperativePlan)
    ''    With CType(e.Item.FindControl("link1"), HyperLink)
    ''        .Text = cRec.ClientAndProject
    ''        .NavigateUrl = "javascript:p48_record(" & cRec.PID.ToString & ")"
    ''        If cRec.IsClosed Then .Font.Strikeout = True
    ''    End With
    ''    With CType(e.Item.FindControl("clue1"), HyperLink)
    ''        .Attributes.Item("rel") = "clue_p48_record.aspx?&pid=" & cRec.PID.ToString
    ''    End With
    ''    With CType(e.Item.FindControl("p48Date"), Label)
    ''        .Text = BO.BAS.FD(cRec.p48Date)
    ''        If cRec.p48TimeFrom <> "" Then .Text += " " & cRec.p48TimeFrom & " - " & cRec.p48TimeUntil
    ''    End With
    ''    With CType(e.Item.FindControl("p48Hours"), Label)
    ''        .Text = BO.BAS.FN(cRec.p48Hours) & "h."
    ''    End With
    ''    With CType(e.Item.FindControl("convert1"), HyperLink)
    ''        .NavigateUrl = "javascript: p48_convert(" & cRec.PID.ToString & ")"
    ''    End With
    ''End Sub

    Private Sub ShowChart1(strFlag As String)
        If Not chkShowCharts.Checked Then ShowImage() : Return
        Dim s As String = "select round(sum(case when b.p32IsBillable=1 THEN p31Hours_Orig end),2) as HodinyFa,round(sum(case when b.p32IsBillable=0 THEN p31Hours_Orig end),2) as HodinyNeFa,c11.c11DateFrom as Datum"
        s += " FROM (select c11DateFrom FROM c11StatPeriod WHERE c11Level=5 AND c11DateFrom between @d1 and @d2) c11 LEFT OUTER JOIN (select * from p31Worksheet where j02ID=@j02id and p31Date between @d1 and @d2) a ON c11.c11DateFrom=a.p31Date LEFT OUTER JOIN p32Activity b ON a.p32ID=b.p32ID"
        s += " WHERE c11.c11DateFrom BETWEEN @d1 AND @d2 GROUP BY c11.c11DateFrom ORDER BY c11.c11DateFrom"
        Dim pars As New List(Of BO.PluginDbParameter)
        If strFlag = "3" Then
            pars.Add(New BO.PluginDbParameter("d1", Today.AddDays(-30)))
        Else
            pars.Add(New BO.PluginDbParameter("d1", Today.AddDays(-14)))
        End If

        pars.Add(New BO.PluginDbParameter("d2", Today.AddDays(1)))
        pars.Add(New BO.PluginDbParameter("j02id", Master.Factory.SysUser.j02ID))
        Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        Dim dbl As Double = 0
        For Each row As DataRow In dt.Rows
            dbl += BO.BAS.IsNullNum(row.Item("HodinyFa")) + BO.BAS.IsNullNum(row.Item("HodinyNeFa"))
        Next
        If dbl < 20 Then
            ShowImage()
            Return
        End If
        If strFlag = "3" Then
            panChart3.Visible = True
            With chart3
                .DataSource = dt
                .DataBind()
            End With
        Else
            panChart1.Visible = True
            With chart1
                .DataSource = dt
                .DataBind()
            End With
        End If



    End Sub

    Private Sub ShowChart2(strFlag As String)
        If Not chkShowCharts.Checked Then ShowImage() : Return

        Dim s As String = "select round(sum(p31Hours_Orig),2) as Hodiny,left(min(p28name),20) as Podle FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID WHERE a.j02ID=@j02id AND p34.p33ID=1 AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY p41.p28ID_Client ORDER BY min(p28Name)"
        Select Case strFlag
            Case "3"
                s = "select round(sum(p31Hours_Orig),2) as Hodiny,left(min(p32Name),20) as Podle FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID WHERE a.j02ID=@j02id AND p34.p33ID=1 AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY a.p32ID ORDER BY min(p32Name)"
            Case "4"
                s = "select round(sum(p31Hours_Orig),2) as Hodiny,left(min(p34Name),20) as Podle FROM p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID WHERE a.j02ID=@j02id AND p34.p33ID=1 AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY p32.p34ID ORDER BY min(p34Name)"
            Case "5"
                s = "select round(sum(p31Hours_Orig),2) as Hodiny,left(min(isnull(p28name+' - ','')+p41Name),40) as Podle FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID WHERE a.j02ID=@j02id AND p34.p33ID=1 AND a.p31Date BETWEEN @d1 AND @d2 GROUP BY a.p41ID ORDER BY min(p28Name),min(p41Name)"

        End Select
        Dim d0 As Date = Now
        If Day(Now) <= 2 Then d0 = Now.AddDays(-10)

        Dim d1 As Date = DateSerial(Year(d0), Month(d0), 1)
        Dim d2 As Date = d1.AddMonths(1).AddDays(-1)
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("d1", d1))
        pars.Add(New BO.PluginDbParameter("d2", d2))
        pars.Add(New BO.PluginDbParameter("j02id", Master.Factory.SysUser.j02ID))
        Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        If strFlag = "5" And dt.Rows.Count > 17 Then ShowChart2("") : Return 'nad 17 projektů->graf podle klientů
        If strFlag = "4" And dt.Rows.Count <= 1 Then ShowChart2("3") : Return 'pokud pracuje v jednom sešitě, pak graf nemá smysl

        If dt.Rows.Count <= 1 Then
            ShowImage()
            Return
        End If
        panChart2.Visible = True
        With chart2
            .ChartTitle.Text = "Měsíc " & Month(d0).ToString & "/" & Year(d0).ToString & ": " & BO.BAS.FN(dt.Compute("Sum(Hodiny)", "")) & "h."
            .DataSource = dt
            .DataBind()
        End With


    End Sub

    
    Private Sub RefreshX47Log()
        With Master.Factory
            If Not (.SysUser.j04IsMenu_Project Or .SysUser.j04IsMenu_Contact Or .SysUser.j04IsMenu_Invoice Or .TestPermission(BO.x53PermValEnum.GR_Admin)) Then
                chkLog.Visible = False
                chkLog.Checked = False
                panX47.Visible = False
                Return
            End If
        End With
        If Not chkLog.Checked Then
            panX47.Visible = False
            Return
        End If
        Dim mq As New BO.myQueryX47, x45ids As New List(Of String), b As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
        mq.IsShowTagsInColumn = True
        With Master.Factory
            If (b Or .TestPermission(BO.x53PermValEnum.GR_P41_Reader)) And .SysUser.j04IsMenu_Project Then
                chkP41.Visible = True
                If chkP41.Checked Then x45ids.Add("14101")
            End If
            If b Or .TestPermission(BO.x53PermValEnum.GR_P28_Reader) And .SysUser.j04IsMenu_Contact Then
                chkP28.Visible = True
                If chkP28.Checked Then x45ids.Add("32801")
            End If
            If b Or .TestPermission(BO.x53PermValEnum.GR_P91_Reader) And .SysUser.j04IsMenu_Invoice Then
                chkP91.Visible = True
                If chkP91.Checked Then
                    x45ids.Add("39101")
                    x45ids.Add("39001")
                End If
            End If
            If .SysUser.j04IsMenu_People Then
                chkJ02.Visible = True
                If chkJ02.Checked Then
                    x45ids.Add("10201")
                End If
            End If
            If b Then
                chkO23.Visible = True
                If chkO23.Checked Then x45ids.Add("22301")
                chkP56.Visible = True
                If chkP56.Checked Then x45ids.Add("35601")
            End If
        End With
        If x45ids.Count > 0 Then
            mq.x45IDs = String.Join(",", x45ids)
            Dim lis As IEnumerable(Of BO.x47EventLog) = Master.Factory.x47EventLogBL.GetList(mq, 20)    ''.Where(Function(p) p.x47Description = "")
            rpX47.DataSource = lis
            rpX47.DataBind()
        Else
            If chkP41.Visible Or chkP28.Visible Or chkP91.Visible Or chkO23.Visible Then
                rpX47.Visible = False
            Else
                panX47.Visible = False
            End If

        End If




    End Sub

    Private Sub rpX47_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX47.ItemDataBound
        Dim cRec As BO.x47EventLog = CType(e.Item.DataItem, BO.x47EventLog), s As String = ""

        With CType(e.Item.FindControl("lbl1"), Label)
            Select Case cRec.x45ID
                Case BO.x45IDEnum.p41_new : s = "Images/project.png" : .Text = Resources.common.Projekt
                Case BO.x45IDEnum.p28_new : s = "Images/contact.png" : .Text = Resources.common.Klient
                Case BO.x45IDEnum.p91_new : s = "Images/invoice.png" : .Text = Resources.common.Faktura
                Case BO.x45IDEnum.j02_new : s = "Images/person.png" : .Text = Resources.common.Osoba
                Case BO.x45IDEnum.o23_new : s = "Images/notepad.png" : .Text = Resources.common.Dokument
                Case BO.x45IDEnum.p56_new : s = "Images/task.png" : .Text = Resources.common.Ukol
                Case Else
                    .Text = cRec.x45Name
            End Select
        End With
        If s = "" Then
            e.Item.FindControl("img1").Visible = False
        Else
            CType(e.Item.FindControl("img1"), Image).ImageUrl = s
            CType(e.Item.FindControl("link1"), HyperLink).NavigateUrl = BO.BAS.GetDataPrefix(cRec.x29ID) & "_framework.aspx?pid=" & cRec.x47RecordPID.ToString
            CType(e.Item.FindControl("lbl2"), Label).Text = BO.BAS.OM3(cRec.x47NameReference, 25)
            CType(e.Item.FindControl("tags"), Label).Text = cRec.TagsInlineHtml
        End If
        With CType(e.Item.FindControl("linkPP1"), HyperLink)
            .Attributes.Item("onclick") = "RCM('" & BO.BAS.GetDataPrefix(cRec.x29ID) & "'," & cRec.x47RecordPID.ToString & ",this)"
        End With
        CType(e.Item.FindControl("timestamp"), Label).Text = cRec.Person & "/" & BO.BAS.FD(cRec.DateInsert, True, True)
        CType(e.Item.FindControl("link1"), HyperLink).Text = BO.BAS.OM3(cRec.x47Name, 40)

    End Sub

    Private Sub chkP41_CheckedChanged(sender As Object, e As EventArgs) Handles chkP41.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkP41", BO.BAS.GB(Me.chkP41.Checked))
        ReloadPage()
    End Sub

    Private Sub chkP91_CheckedChanged(sender As Object, e As EventArgs) Handles chkP91.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkP91", BO.BAS.GB(Me.chkP91.Checked))
        ReloadPage()
    End Sub

    Private Sub chkP28_CheckedChanged(sender As Object, e As EventArgs) Handles chkP28.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkP28", BO.BAS.GB(Me.chkP28.Checked))
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("j03_mypage_greeting.aspx")
    End Sub

    Private Sub chkO23_CheckedChanged(sender As Object, e As EventArgs) Handles chkO23.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkO23", BO.BAS.GB(Me.chkO23.Checked))
        ReloadPage()
    End Sub

    Private Sub chkShowCharts_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowCharts.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkShowCharts", BO.BAS.GB(Me.chkShowCharts.Checked))
        ReloadPage()
    End Sub

    Private Sub chkP56_CheckedChanged(sender As Object, e As EventArgs) Handles chkP56.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkP56", BO.BAS.GB(Me.chkP56.Checked))
        ReloadPage()
    End Sub

    Private Sub chkSearch_CheckedChanged(sender As Object, e As EventArgs) Handles chkSearch.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkSearch", BO.BAS.GB(Me.chkSearch.Checked))
        ReloadPage()
    End Sub
    ''Private Sub cbxP56Types_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxP56Types.SelectedIndexChanged
    ''    Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-cbxP56Types", Me.cbxP56Types.SelectedValue)
    ''    ReloadPage()
    ''End Sub

    Private Sub j03_mypage_greeting_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        cal1.Visible = chkScheduler.Checked
    End Sub

    Private Sub chkLog_CheckedChanged(sender As Object, e As EventArgs) Handles chkLog.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkLog", BO.BAS.GB(Me.chkLog.Checked))
        ReloadPage()
    End Sub

    Private Sub chkJ02_CheckedChanged(sender As Object, e As EventArgs) Handles chkJ02.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkJ02", BO.BAS.GB(Me.chkJ02.Checked))
        ReloadPage()
    End Sub

    Private Sub chkScheduler_CheckedChanged(sender As Object, e As EventArgs) Handles chkScheduler.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkScheduler", BO.BAS.GB(Me.chkScheduler.Checked))
        ReloadPage()
    End Sub

    Private Sub RefreshX18()
        panX18.Visible = False
        Dim lisX18 As IEnumerable(Of BO.x18EntityCategory) = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery).Where(Function(p) p.x18DashboardFlag > BO.x18DashboardENUM.NotUsed)
        If lisX18.Count = 0 Then Return

        For Each c In lisX18
            c.x18ReportCodes = ""
            Dim cDisp As BO.x18RecordDisposition = Master.Factory.x18EntityCategoryBL.InhaleDisposition(c)
            If cDisp.CreateItem And (c.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkAndGrid Or c.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkOnly) Then
                c.x18ReportCodes = "1"
            Else
                c.x18ReportCodes = "0"
            End If
            If (cDisp.ReadItems Or cDisp.OwnerItems) And (c.x18DashboardFlag = BO.x18DashboardENUM.LinkOnly Or c.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkAndGrid) Then
                c.x18ReportCodes += "1"
            Else
                c.x18ReportCodes += "0"
            End If
            If c.x18DashboardFlag = BO.x18DashboardENUM.ShowItemsLikeNoticeboard Then
                c.x18ReportCodes = ""
                panNoticeBoard.Visible = True
                Dim mq As New BO.myQueryO23(c.x23ID)
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                Dim lis As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList(mq).OrderBy(Function(p) p.o23Ordinary).OrderByDescending(Function(p) p.o23FreeDate01).ThenByDescending(Function(p) p.DateInsert)
                rpArticle.DataSource = lis
                rpArticle.DataBind()
                lblNoticeBoardHeader.Text = c.x18Name
            End If
        Next
        lisX18 = lisX18.Where(Function(p) p.x18ReportCodes <> "")
        If lisX18.Count > 0 Then
            chkX18.Visible = True
            If chkX18.Checked Then
                panX18.Visible = True
                rpX18.DataSource = lisX18
                rpX18.DataBind()
            End If

        Else
            chkX18.Visible = False
        End If
    End Sub

    Private Sub rpX18_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX18.ItemDataBound
        Dim cRec As BO.x18EntityCategory = CType(e.Item.DataItem, BO.x18EntityCategory)
        With CType(e.Item.FindControl("img1"), Image)
            If cRec.x18Icon32 <> "" Then
                .ImageUrl = cRec.x18Icon32
            Else
                .Visible = False
            End If
        End With
        CType(e.Item.FindControl("x18Name"), Label).Text = cRec.x18Name
        e.Item.FindControl("cmdNew").Visible = False : e.Item.FindControl("linkFramework").Visible = False : e.Item.FindControl("linkCalendar").Visible = False

        If (cRec.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkAndGrid Or cRec.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkOnly) And Left(cRec.x18ReportCodes, 1) = "1" Then
            With CType(e.Item.FindControl("cmdNew"), HtmlButton)
                .Visible = True
                .InnerHtml = "<img src='Images/new.png' />Nový"
                .Attributes.Item("onclick") = "o23_create_dashboard(" & cRec.PID.ToString & ")"
            End With
        End If
        If (cRec.x18DashboardFlag = BO.x18DashboardENUM.CreateLinkAndGrid Or cRec.x18DashboardFlag = BO.x18DashboardENUM.LinkOnly Or cRec.x18DashboardFlag = BO.x18DashboardENUM.ShowItemsLikeNoticeboard) And Right(cRec.x18ReportCodes, 1) = "1" Then
            With CType(e.Item.FindControl("linkFramework"), HyperLink)
                .Visible = True
                .NavigateUrl = "o23_fixwork.aspx?x18id=" & cRec.PID.ToString
            End With
            With CType(e.Item.FindControl("linkCalendar"), HyperLink)
                If cRec.x18IsCalendar Then
                    .Visible = True
                    .NavigateUrl = "o23_scheduler.aspx?x18id=" & cRec.PID.ToString
                End If
            End With
        End If


    End Sub

    Private Sub chkX18_CheckedChanged(sender As Object, e As EventArgs) Handles chkX18.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("j03_mypage_greeting-chkX18", BO.BAS.GB(Me.chkX18.Checked))
        ReloadPage()

    End Sub

    Private Sub rpArticle_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpArticle.ItemCommand
        Dim intO23ID As Integer = e.CommandArgument
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(intO23ID)
        tdRecO23.Visible = True

        If cRec.o23IsEncrypted Then
            rec1.Visible = False
            Master.Notify("Obsah článku je zašifrovaný.")
        Else
            rec1.Visible = True
            rec1.FillData(cRec, Master.Factory.x18EntityCategoryBL.LoadByX23ID(cRec.x23ID), False)
        End If

        comments1.RefreshData(Master.Factory, BO.x29IdEnum.o23Doc, cRec.PID)

        With CType(e.Item.FindControl("link1"), LinkButton)
            .CommandArgument = cRec.PID
            .Text = BO.BAS.OM3(cRec.o23Name, 30)
            .BackColor = Drawing.Color.Yellow
        End With
    End Sub


    Private Sub rpArticle_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpArticle.ItemDataBound
        Dim cRec As BO.o23Doc = CType(e.Item.DataItem, BO.o23Doc)
        With CType(e.Item.FindControl("link1"), LinkButton)
            .CommandArgument = cRec.PID
            .Text = BO.BAS.OM3(cRec.o23Name, 30)

        End With
        With CType(e.Item.FindControl("timestamp"), Label)
            If Not cRec.o23FreeDate01 Is Nothing Then
                .Text = BO.BAS.FD(cRec.o23FreeDate01, True, True) & " " & cRec.UserInsert
            Else
                .Text = BO.BAS.FD(cRec.DateInsert, True, True) & " " & cRec.UserInsert
            End If

        End With
    End Sub
End Class