Public Class p48_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _lastJ02ID As Integer
    Private Property _lastP41ID As Integer
    Private _lastC21ID As Integer
    Private _lastJ17ID As Integer
    Private _lisP48 As IEnumerable(Of BO.p48OperativePlan)
    Private _lisSum As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject)
    Private _lisC22 As IEnumerable(Of BO.c22FondCalendar_Date)
    Private _lisP31 As IEnumerable(Of BO.p31HoursPerEntityAndDay)
    Private _lisP47 As IEnumerable(Of BO.p47CapacityPlan)


    Public Enum RozkladENUM
        j02_p41 = 1
        j02_only = 2
        p41_j02 = 3
        p41_only = 4
    End Enum
    Public Class PlanRow
        Public Property j02ID As Integer
        Public Property p41ID As Integer
        Public Property Person As String
        Public Property Project As String
        Public Property c21ID As Integer
        Public Property j17ID As Integer
        Public Sub New(intJ02ID As Integer)
            Me.j02ID = intJ02ID
        End Sub
    End Class
    Private Sub p48_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p48_framework"
        persons1.Factory = Master.Factory
        projects1.Factory = Master.Factory
    End Sub
    Public ReadOnly Property CurrentMonth As Integer
        Get
            Return CInt(query_month.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property CurrentYear As Integer
        Get
            Return CInt(query_year.SelectedValue)
        End Get
    End Property
    
    Public ReadOnly Property CurrentRozklad As RozkladENUM
        Get
            Return CType(CInt(cbxRozklad.SelectedValue), RozkladENUM)
        End Get
    End Property
    Public Property CurrentMasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property
    Public Property CurrentMasterPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidMasterPID.Value)
        End Get
        Set(value As Integer)
            hidMasterPID.Value = value.ToString
        End Set
    End Property


    Public ReadOnly Property CurrentD1 As Date
        Get
            Return DateSerial(Me.CurrentYear, Me.CurrentMonth, 1)
        End Get
    End Property
    Public ReadOnly Property CurrentD2 As Date
        Get
            Return Me.CurrentD1.AddMonths(1).AddDays(-1)
        End Get
    End Property
    


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Operativní plánování"
                .SiteMenuValue = "p48_framework"
            End With
            If Request.Item("masterpid") <> "" Then
                Me.CurrentMasterPID = BO.BAS.IsNullInt(Request.Item("masterpid")) : Me.CurrentMasterPrefix = Request.Item("masterprefix")
            End If

            With Master.Factory.j03UserBL
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p48_framework-query-year")
                    .Add("p48_framework-query-month")
                    .Add("p48_framework_weekend")
                    .Add("p48_framework_rozklad")
                    .Add("p48_framework_worksheet")
                    .Add("p48_framework-persons1-scope")
                    .Add("p48_framework-persons1-value")
                    .Add("p48_framework-projects1-scope")
                    .Add("p48_framework-projects1-value")
                End With
                .InhaleUserParams(lisPars)
                With query_year
                    If .Items.Count = 0 Then
                        For i As Integer = -2 To 2
                            Dim intY As Integer = Year(Now) + i
                            .Items.Add(New ListItem(intY.ToString, intY.ToString))
                        Next
                    End If
                End With
                basUI.SelectDropdownlistValue(Me.query_year, .GetUserParam("p48_framework-query-year", Year(Now).ToString))
                basUI.SelectDropdownlistValue(Me.query_month, .GetUserParam("p48_framework-query-month", Month(Now).ToString))

                
                Me.chkIncludeWeekend.Checked = BO.BAS.BG(.GetUserParam("p48_framework_weekend", "0"))
                basUI.SelectDropdownlistValue(Me.cbxRozklad, .GetUserParam("p48_framework_rozklad", "1"))
                Me.chkShowWorksheet.Checked = BO.BAS.BG(.GetUserParam("p48_framework_worksheet", "1"))
                Me.persons1.CurrentScope = .GetUserParam("p48_framework-persons1-scope", "2")
                Me.persons1.CurrentValue = .GetUserParam("p48_framework-persons1-value", Master.Factory.SysUser.j02ID.ToString & "||")
                Me.projects1.CurrentScope = .GetUserParam("p48_framework-projects1-scope", "1")
                Me.projects1.CurrentValue = .GetUserParam("p48_framework-projects1-value")
            End With
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P48_Reader) Then
                'čtenář všech plánů v db
                
            Else
                persons1.Visible = False
                persons1.CurrentScope = 2
                persons1.CurrentValue = Master.Factory.SysUser.j02ID.ToString & "||"
            End If

            If Me.CurrentMasterPID > 0 Then
                Me.MasterRecord.Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentMasterPrefix), Me.CurrentMasterPID)
                Me.MasterRecord.NavigateUrl = Me.CurrentMasterPrefix & "_framework.aspx?pid=" & Me.CurrentMasterPID.ToString
            Else
                Me.MasterRecord.Visible = False
            End If
            

            RefreshData()
        End If
    End Sub

    Private Sub query_month_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles query_month.SelectedIndexChanged
        Handle_AfterChangeMonth()

    End Sub

    Private Sub query_year_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles query_year.SelectedIndexChanged
        Handle_AfterChangeYear()

    End Sub

    Private Sub cmdNextMonth_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles cmdNextMonth.Click
        If query_month.SelectedValue = "12" Then
            If query_year.SelectedIndex < query_year.Items.Count - 1 Then
                query_year.SelectedIndex += 1
                query_month.SelectedValue = "1"
                Handle_AfterChangeYear()
            Else
                Return
            End If
        Else
            query_month.SelectedIndex += 1
        End If

        Handle_AfterChangeMonth()
    End Sub

    Private Sub cmdPrevMonth_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles cmdPrevMonth.Click
        If query_month.SelectedValue = "1" Then
            If query_year.SelectedIndex > 0 Then
                query_year.SelectedIndex = query_year.SelectedIndex - 1
                query_month.SelectedValue = "12"
                Handle_AfterChangeYear()
            Else
                Return
            End If
        Else
            query_month.SelectedIndex = query_month.SelectedIndex - 1
        End If
        Handle_AfterChangeMonth()
    End Sub

    Private Sub Handle_AfterChangeMonth()
        Master.Factory.j03UserBL.SetUserParam("p48_framework-query-month", query_month.SelectedValue)
        RefreshData()
    End Sub
    Private Sub Handle_AfterChangeYear()
        Master.Factory.j03UserBL.SetUserParam("p48_framework-query-year", query_year.SelectedValue)
        RefreshData()
    End Sub


    

    Private Sub RenderGridHeader()
        Dim intDaysInMonth As Integer = GetDaysInCurrentMonth()
        For i As Integer = 1 To 31
            If i <= intDaysInMonth Then
                Dim d As Date = DateSerial(Me.CurrentYear, Me.CurrentMonth, i)
                Dim strCssClass As String = "day", intWeekDay As Integer = Weekday(d, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
                If intWeekDay >= 6 Then
                    strCssClass = "weekend"
                Else
                    strCssClass = "workday"
                    'zjišťovat, zda je svátek
                End If
                Dim dayHeader As Label = CType(panLayout.FindControl("d" & i.ToString), Label)
                dayHeader.Text = i.ToString & "<div>" & WeekdayName(intWeekDay, True, Microsoft.VisualBasic.FirstDayOfWeek.Monday) & "</div>"
                CType(panLayout.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("class") = strCssClass
                CType(panLayout.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("width") = "25px"
                CType(panLayout.FindControl("tdd" & i.ToString), HtmlTableCell).Style.Item("display") = ""
            Else
                CType(panLayout.FindControl("d" & i.ToString), Label).Text = ""
                CType(panLayout.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("class") = ""
                CType(panLayout.FindControl("tdd" & i.ToString), HtmlTableCell).Style.Item("display") = "none"
            End If

        Next
    End Sub
    Private Function GetDaysInCurrentMonth() As Integer
        Return DateSerial(Me.CurrentYear, Me.CurrentMonth, 1).AddMonths(1).AddDays(-1).Day
    End Function


    Private Sub RefreshData()
        RenderGridHeader()

        Dim mqP48 As New BO.myQueryP48

        mqP48.DateFrom = Me.CurrentD1
        mqP48.DateUntil = Me.CurrentD2
        Select Case Me.CurrentMasterPrefix
            Case "p41" : mqP48.p41ID = Me.CurrentMasterPID
            Case "p28" : mqP48.p28ID = Me.CurrentMasterPID
            Case "j02" : mqP48.j02IDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
            Case Else
                If persons1.CurrentScope > 1 Then mqP48.j02IDs = persons1.CurrentJ02IDs
                If projects1.CurrentScope > 1 Then mqP48.p41IDs = projects1.CurrentP41IDs
        End Select
        _lisP48 = Master.Factory.p48OperativePlanBL.GetList(mqP48)

        Select Case Me.CurrentRozklad
            Case RozkladENUM.j02_only, RozkladENUM.j02_p41
                _lisSum = Master.Factory.p48OperativePlanBL.GetList_SumPerPerson(mqP48)
            Case RozkladENUM.p41_j02, RozkladENUM.p41_only
                _lisSum = Master.Factory.p48OperativePlanBL.GetList_SumPerProject(mqP48)
        End Select


        'okruh osob
        Dim mqJ02 As New BO.myQueryJ02
        mqJ02.Closed = BO.BooleanQueryMode.NoQuery
        If persons1.CurrentScope > 1 Then mqJ02.PIDs = persons1.CurrentJ02IDs

        mqJ02.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
        Select Case Me.CurrentMasterPrefix
            Case "p41", "p28"  'přidat všechny osoby, které v daném projektu/klientu mají operativní plán
                Dim j02ids As List(Of Integer) = _lisP48.Select(Function(p) p.j02ID).Distinct.ToList
                If j02ids.Count > 0 Then mqJ02.PIDs.AddRange(j02ids)
            Case "j02"
                mqJ02.PIDs = BO.BAS.ConvertInt2List(Me.CurrentMasterPID)
        End Select
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mqJ02)


        Dim lisPlan As New List(Of PlanRow)
        If Me.CurrentRozklad = RozkladENUM.j02_only Or Me.CurrentRozklad = RozkladENUM.j02_p41 Then
            For Each cJ02 In lisJ02
                Dim c As New PlanRow(cJ02.PID)
                c.Person = cJ02.FullNameDesc
                c.c21ID = cJ02.c21ID
                c.j17ID = cJ02.j17ID
                lisPlan.Add(c)
            Next
        End If
        If Me.CurrentRozklad = RozkladENUM.p41_j02 Then
            Dim qry = From p In _lisP48 Select p.p41ID, p.Project Distinct
            For Each row In qry
                Dim c As New PlanRow(0)
                c.p41ID = row.p41ID
                c.Project = row.Project
                lisPlan.Add(c)
            Next
        End If

        If Me.CurrentRozklad = RozkladENUM.p41_j02 Or Me.CurrentRozklad = RozkladENUM.j02_p41 Then
            Dim qry = From p In _lisP48 Select p.j02ID, p.Person, p.p41ID, p.Project Distinct
            For Each row In qry
                Dim c As New PlanRow(row.j02ID)
                c.p41ID = row.p41ID
                c.Person = row.Person
                c.Project = row.Project
                lisPlan.Add(c)
            Next
        End If

        Dim c21ids As List(Of Integer) = lisJ02.Select(Function(p) p.c21ID).Distinct.ToList
        _lisC22 = Master.Factory.c21FondCalendarBL.GetList_c22(c21ids, Me.CurrentD1, Me.CurrentD2, True)

        Dim pids As New List(Of Integer), strPrefix As String = "j02"
        If Me.CurrentRozklad = RozkladENUM.j02_only Or Me.CurrentRozklad = RozkladENUM.j02_p41 Then
            If persons1.CurrentScope > 1 Then
                pids = persons1.CurrentJ02IDs.Select(Function(p) p).ToList
            End If
        Else
            strPrefix = "p41"
        End If
        If Me.chkShowWorksheet.Checked Then
            _lisP31 = Master.Factory.p31WorksheetBL.GetSumHoursPerEntityAndDate(strPrefix, pids, Me.CurrentD1, Me.CurrentD2)
        End If
        Dim mqP47 As New BO.myQueryP47
        mqP47.DateFrom = Me.CurrentD1
        mqP47.DateUntil = Me.CurrentD2
        Select Case Me.CurrentMasterPrefix
            Case "p41" : mqP47.p41ID = Me.CurrentMasterPID
            Case "p28" : mqP47.p28ID = Me.CurrentMasterPID
            Case "j02" : mqP47.j02ID = Me.CurrentMasterPID
        End Select
        _lisP47 = Master.Factory.p47CapacityPlanBL.GetList(mqP47)

        Select Case Me.CurrentRozklad
            Case RozkladENUM.j02_only, RozkladENUM.j02_p41
                rp1.DataSource = lisPlan.OrderBy(Function(p) p.Person).ThenBy(Function(p) p.Project)
            Case RozkladENUM.p41_j02, RozkladENUM.p41_only
                rp1.DataSource = lisPlan.OrderBy(Function(p) p.Project).ThenBy(Function(p) p.Person)
        End Select

        rp1.DataBind()
    End Sub

    

    


    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        If e.CommandName = "remove" Then
            ''Dim j02ids As List(Of Integer) = Me.CurrentJ02IDs
            ''j02ids.Remove(CInt(CType(e.Item.FindControl("j02id"), HiddenField).Value))
            ''Me.CurrentJ02IDs = j02ids
            ''If j02ids.Count = 0 Then
            ''    Master.Notify("Minimálně jedna osoba musí být zobrazena - bude to váš profil.", NotifyLevel.InfoMessage)
            ''End If

            ''RefreshData()
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Select Case Me.CurrentRozklad
            Case RozkladENUM.j02_p41, RozkladENUM.j02_only
                handle_dataBound_By_Person(sender, e)
            Case RozkladENUM.p41_j02, RozkladENUM.p41_only
                handle_dataBound_By_Project(sender, e)
        End Select

    End Sub

    Private Sub handle_dataBound_By_Person(sender As Object, e As RepeaterItemEventArgs)
        Dim cRec As PlanRow = CType(e.Item.DataItem, PlanRow)

        If cRec.j02ID <> _lastJ02ID Then
            'součtový řádek
            CType(e.Item.FindControl("person"), Label).Text = cRec.Person
            CType(e.Item.FindControl("clue_person"), HyperLink).Attributes("rel") = "clue_j02_oplan.aspx?pid=" & cRec.j02ID.ToString & "&month=" & Me.CurrentMonth.ToString & "&year=" & Me.CurrentYear.ToString

            _lastC21ID = cRec.c21ID
            _lastJ17ID = cRec.j17ID
            Dim dblFond As Double = _lisC22.Where(Function(p) p.c21ID = _lastC21ID And p.j17ID = _lastJ17ID).Sum(Function(p) p.c22Hours_Work)
            Dim dblPlan As Double = _lisSum.Where(Function(p) p.j02ID = cRec.j02ID).Sum(Function(p) p.Hours)
            If dblFond = 0 Then dblFond = dblPlan
            CType(e.Item.FindControl("fond"), Label).Text = dblPlan.ToString & "/" & dblFond.ToString
            With CType(e.Item.FindControl("util"), Label)
                If dblPlan = 0 Then
                    .Text = "0%"
                Else
                    .Text = CInt(100 * dblPlan / dblFond).ToString & "%"
                    If dblPlan > dblFond Then .ForeColor = Drawing.Color.Red
                End If
            End With
            If Me.chkShowWorksheet.Checked Then
                CType(e.Item.FindControl("worksheet"), Label).Text = FN(_lisP31.Where(Function(p) p.EntityPID = cRec.j02ID).Sum(Function(p) p.Hours_Orig))
            End If
            CType(e.Item.FindControl("capaplan"), Label).Text = FN(_lisP47.Where(Function(p) p.j02ID = cRec.j02ID).Sum(Function(p) p.p47HoursTotal))
        Else
            e.Item.FindControl("clue_person").Visible = False
            ''e.Item.FindControl("cmdRemove").Visible = False
        End If

        CType(e.Item.FindControl("project"), Label).Text = cRec.Project
        CType(e.Item.FindControl("j02id"), HiddenField).Value = cRec.j02ID.ToString
        CType(e.Item.FindControl("p41id"), HiddenField).Value = cRec.p41ID.ToString

        Dim intDaysInMonth As Integer = GetDaysInCurrentMonth()
        For i As Integer = 1 To intDaysInMonth

            Dim d As Date = DateSerial(Me.CurrentYear, Me.CurrentMonth, i)
            Dim intWeekDay As Integer = Weekday(d, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
            Dim strCssClass As String = ""

            Dim lisFond As IEnumerable(Of BO.c22FondCalendar_Date) = _lisC22.Where(Function(p) p.c21ID = _lastC21ID And p.j17ID = _lastJ17ID And p.c22Date = d)
            If intWeekDay >= 6 Then
                strCssClass = "weekend"
            Else
                strCssClass = "workday"

                If lisFond.Count > 0 Then
                    If lisFond(0).c22Hours_Work = 0 Then strCssClass = "outfond" 'den je mimo rámec pracovního fondu dané osoby
                End If
            End If

            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("class") = strCssClass
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("width") = "25px"
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("val") = i.ToString & "-" & cRec.j02ID.ToString & "-" & cRec.p41ID.ToString
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("isl") = "1"


            If cRec.p41ID = 0 Then
                'první/součtový řádek
                If Me.CurrentRozklad = RozkladENUM.j02_p41 Then
                    Dim lis As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject) = _lisSum.Where(Function(p) p.j02ID = cRec.j02ID And p.p48Date = d)
                    If lis.Count > 0 Then
                        Dim s As String = "<div class='sum'>" & lis(0).Hours.ToString & "</div>"
                        If lisFond.Count > 0 Then
                            If lisFond(0).c22Hours_Work < lis(0).Hours Then
                                'naplánované hodiny jsou větší než fond hodin
                                s = "<div class='sumoverfond' title='Plán přes fond hodin dne!'>" & lis(0).Hours.ToString & "</div>"
                            End If
                        End If
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = s
                    End If
                End If
                If Me.CurrentRozklad = RozkladENUM.j02_only Then
                    Dim lis As IEnumerable(Of BO.p48OperativePlan) = _lisP48.Where(Function(p) p.j02ID = cRec.j02ID And p.p48Date = d)
                    If lis.Count > 0 Then
                        Dim plans As New List(Of String)
                        Dim p48ids As New List(Of Integer)
                        For Each c In lis
                            Dim strTooltip As String = c.Project & vbCrLf & c.p34Name, strCss As String = "plan"
                            If c.p31ID > 0 Then strCss = "reality"
                            If c.p32ID > 0 Then strTooltip += " - " & c.p32Name
                            If Len(c.p48Text) > 0 Then strTooltip += vbCrLf & c.p48Text
                            Dim strStyle As String = ""
                            If c.p34Color <> "" Then strStyle = "style='background-color:" & c.p34Color & ";'"
                            If c.p32Color <> "" Then strStyle = "style='background-color:" & c.p32Color & ";'"
                            'plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'>" & c.p48Hours.ToString & "</div>")
                            plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'><a class='reczoom' rel='clue_p48_record.aspx?pid=" & c.PID.ToString & "'>" & c.p48Hours.ToString & "</a></div>")

                            p48ids.Add(c.PID)
                        Next
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = String.Join(" ", plans)
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("p48ids") = String.Join(",", p48ids)

                    End If
                End If

            Else
                'řádek s rozkladem za projekt
                Dim lis As IEnumerable(Of BO.p48OperativePlan) = _lisP48.Where(Function(p) p.j02ID = cRec.j02ID And p.p41ID = cRec.p41ID And p.p48Date = d)
                If lis.Count > 0 Then
                    Dim plans As New List(Of String)
                    Dim p48ids As New List(Of Integer)
                    For Each c In lis
                        Dim strTooltip As String = c.p34Name, strCss As String = "plan"
                        If c.p31ID > 0 Then strCss = "reality"
                        If c.p32ID > 0 Then strTooltip += " - " & c.p32Name
                        If Len(c.p48Text) > 0 Then strTooltip += vbCrLf & c.p48Text
                        Dim strStyle As String = ""
                        If c.p34Color <> "" Then strStyle = "style='background-color:" & c.p34Color & ";'"
                        If c.p32Color <> "" Then strStyle = "style='background-color:" & c.p32Color & ";'"
                        'plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'>" & c.p48Hours.ToString & "</div>")
                        plans.Add("<div class='" & strCss & "' " & strStyle & "><a class='reczoom' rel='clue_p48_record.aspx?pid=" & c.PID.ToString & "'>" & c.p48Hours.ToString & "</a></div>")
                        p48ids.Add(c.PID)
                    Next
                    CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = String.Join(" ", plans)
                    CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("p48ids") = String.Join(",", p48ids)

                End If
            End If

        Next
        For i = intDaysInMonth + 1 To 31
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Style.Item("display") = "none"
        Next

        _lastJ02ID = cRec.j02ID

    End Sub
    Private Sub handle_dataBound_By_Project(sender As Object, e As RepeaterItemEventArgs)
        Dim cRec As PlanRow = CType(e.Item.DataItem, PlanRow)
        ''e.Item.FindControl("cmdRemove").Visible = False
        e.Item.FindControl("clue_person").Visible = False

        If cRec.p41ID <> _lastP41ID Then
            'součtový řádek
            CType(e.Item.FindControl("person"), Label).Text = cRec.Project
            ' ''CType(e.Item.FindControl("clue_person"), HyperLink).Attributes("rel") = "clue_j02_oplan.aspx?pid=" & cRec.j02ID.ToString & "&month=" & Me.CurrentMonth.ToString & "&year=" & Me.CurrentYear.ToString

            '_lastC21ID = cRec.c21ID
            '_lastJ17ID = cRec.j17ID
            'Dim dblFond As Double = _lisC22.Where(Function(p) p.c21ID = _lastC21ID And p.j17ID = _lastJ17ID).Sum(Function(p) p.c22Hours_Work)

            Dim dblPlan As Double = _lisSum.Where(Function(p) p.p41ID = cRec.p41ID).Sum(Function(p) p.Hours)
            ''If dblFond = 0 Then dblFond = dblPlan
            ''CType(e.Item.FindControl("fond"), Label).Text = dblPlan.ToString & "/" & dblFond.ToString
            ''With CType(e.Item.FindControl("util"), Label)
            ''    If dblPlan = 0 Then
            ''        .Text = "0%"
            ''    Else
            ''        .Text = CInt(100 * dblPlan / dblFond).ToString & "%"
            ''        If dblPlan > dblFond Then .ForeColor = Drawing.Color.Red
            ''    End If
            ''End With
            If Me.chkShowWorksheet.Checked Then
                CType(e.Item.FindControl("worksheet"), Label).Text = FN(_lisP31.Where(Function(p) p.EntityPID = cRec.p41ID).Sum(Function(p) p.Hours_Orig))
            End If
            CType(e.Item.FindControl("capaplan"), Label).Text = FN(_lisP47.Where(Function(p) p.p41ID = cRec.p41ID).Sum(Function(p) p.p47HoursTotal))
        Else
            ''e.Item.FindControl("clue_person").Visible = False
            ''e.Item.FindControl("cmdRemove").Visible = False
        End If

        CType(e.Item.FindControl("project"), Label).Text = cRec.Person
        CType(e.Item.FindControl("j02id"), HiddenField).Value = cRec.j02ID.ToString
        CType(e.Item.FindControl("p41id"), HiddenField).Value = cRec.p41ID.ToString

        Dim intDaysInMonth As Integer = GetDaysInCurrentMonth()
        For i As Integer = 1 To intDaysInMonth

            Dim d As Date = DateSerial(Me.CurrentYear, Me.CurrentMonth, i)
            Dim intWeekDay As Integer = Weekday(d, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
            Dim strCssClass As String = ""

            ''Dim lisFond As IEnumerable(Of BO.c22FondCalendar_Date) = _lisC22.Where(Function(p) p.c21ID = _lastC21ID And p.j17ID = _lastJ17ID And p.c22Date = d)
            If intWeekDay >= 6 Then
                strCssClass = "weekend"
            Else
                strCssClass = "workday"

                ''If lisFond.Count > 0 Then
                ''    If lisFond(0).c22Hours_Work = 0 Then strCssClass = "outfond" 'den je mimo rámec pracovního fondu dané osoby
                ''End If
            End If

            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("class") = strCssClass
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("width") = "25px"
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("val") = i.ToString & "-" & cRec.j02ID.ToString & "-" & cRec.p41ID.ToString
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("isl") = "1"


            If cRec.j02ID = 0 Then
                If Me.CurrentRozklad = RozkladENUM.p41_j02 Then
                    Dim lis As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject) = _lisSum.Where(Function(p) p.p41ID = cRec.p41ID And p.p48Date = d)
                    If lis.Count > 0 Then
                        Dim s As String = "<div class='sum'>" & lis(0).Hours.ToString & "</div>"
                        ''If lisFond.Count > 0 Then
                        ''    If lisFond(0).c22Hours_Work < lis(0).Hours Then
                        ''        'naplánované hodiny jsou větší než fond hodin
                        ''        s = "<div class='sumoverfond' title='Plán přes fond hodin dne!'>" & lis(0).Hours.ToString & "</div>"
                        ''    End If
                        ''End If
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = s
                    End If
                End If
                If Me.CurrentRozklad = RozkladENUM.p41_only Then
                    Dim lis As IEnumerable(Of BO.p48OperativePlan) = _lisP48.Where(Function(p) p.p41ID = cRec.p41ID And p.p48Date = d)
                    If lis.Count > 0 Then
                        Dim plans As New List(Of String)
                        Dim p48ids As New List(Of Integer)
                        For Each c In lis
                            Dim strTooltip As String = c.Project & vbCrLf & c.p34Name, strCss As String = "plan"
                            If c.p31ID > 0 Then strCss = "reality"
                            If c.p32ID > 0 Then strTooltip += " - " & c.p32Name
                            If Len(c.p48Text) > 0 Then strTooltip += vbCrLf & c.p48Text
                            Dim strStyle As String = ""
                            If c.p34Color <> "" Then strStyle = "style='background-color:" & c.p34Color & ";'"
                            If c.p32Color <> "" Then strStyle = "style='background-color:" & c.p32Color & ";'"
                            'plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'>" & c.p48Hours.ToString & "</div>")
                            plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'><a class='reczoom' rel='clue_p48_record.aspx?pid=" & c.PID.ToString & "'>" & c.p48Hours.ToString & "</a></div>")

                            p48ids.Add(c.PID)
                        Next
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = String.Join(" ", plans)
                        CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("p48ids") = String.Join(",", p48ids)

                    End If
                End If

            Else
                'řádek s rozkladem za osobu
                Dim lis As IEnumerable(Of BO.p48OperativePlan) = _lisP48.Where(Function(p) p.j02ID = cRec.j02ID And p.p41ID = cRec.p41ID And p.p48Date = d)
                If lis.Count > 0 Then
                    Dim plans As New List(Of String)
                    Dim p48ids As New List(Of Integer)
                    For Each c In lis
                        Dim strTooltip As String = c.p34Name, strCss As String = "plan"
                        If c.p31ID > 0 Then strCss = "reality"
                        If c.p32ID > 0 Then strTooltip += " - " & c.p32Name
                        If Len(c.p48Text) > 0 Then strTooltip += vbCrLf & c.p48Text
                        Dim strStyle As String = ""
                        If c.p34Color <> "" Then strStyle = "style='background-color:" & c.p34Color & ";'"
                        If c.p32Color <> "" Then strStyle = "style='background-color:" & c.p32Color & ";'"
                        'plans.Add("<div class='" & strCss & "' " & strStyle & " title='" & strTooltip & "'>" & c.p48Hours.ToString & "</div>")
                        plans.Add("<div class='" & strCss & "' " & strStyle & "><a class='reczoom' rel='clue_p48_record.aspx?pid=" & c.PID.ToString & "'>" & c.p48Hours.ToString & "</a></div>")
                        p48ids.Add(c.PID)
                    Next
                    CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).InnerHtml = String.Join(" ", plans)
                    CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Attributes.Item("p48ids") = String.Join(",", p48ids)

                End If
            End If

        Next
        For i = intDaysInMonth + 1 To 31
            CType(e.Item.FindControl("tdd" & i.ToString), HtmlTableCell).Style.Item("display") = "none"
        Next

        _lastP41ID = cRec.p41ID

    End Sub

    Private Sub cmdHardRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdHardRefreshOnBehind.Click
        RefreshData()
    End Sub

    Private Sub chkIncludeWeekend_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeWeekend.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p48_framework_weekend", BO.BAS.GB(Me.chkIncludeWeekend.Checked))
        RefreshData()
    End Sub

    Private Sub cbxRozklad_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxRozklad.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p48_framework_rozklad", Me.cbxRozklad.SelectedValue)
        RefreshData()
    End Sub

    Private Sub chkShowWorksheet_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowWorksheet.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p48_framework_worksheet", BO.BAS.GB(Me.chkShowWorksheet.Checked))
        RefreshData()
    End Sub

    Private Function FN(dbl As Double) As String
        Return BO.BAS.FN3(dbl)
    End Function

   

    Private Sub p48_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.lblHeader.Text = BO.BAS.OM2(Me.lblHeader.Text, Me.CurrentMonth.ToString & "/" & Me.CurrentYear.ToString)
      
        PersonsHeader.Text = persons1.CurrentHeader
        ProjectsHeader.Text = projects1.CurrentHeader
    End Sub

    
    Private Sub persons1_OnChange() Handles persons1.OnChange
        Master.Factory.j03UserBL.SetUserParam("p48_framework-persons1-scope", persons1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("p48_framework-persons1-value", persons1.CurrentValue)
        RefreshData()
        hidIsPersonsChange.Value = "1"
    End Sub

    Private Sub projects1_OnChange() Handles projects1.OnChange
        Master.Factory.j03UserBL.SetUserParam("p48_framework-projects1-scope", projects1.CurrentScope.ToString)
        Master.Factory.j03UserBL.SetUserParam("p48_framework-projects1-value", projects1.CurrentValue)
        RefreshData()
        hidIsProjectsChange.Value = "1"
    End Sub
End Class