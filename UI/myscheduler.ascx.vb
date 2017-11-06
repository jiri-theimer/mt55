Imports Telerik.Web.UI
Public Class myscheduler
    Inherits System.Web.UI.UserControl
    Public Property factory As BL.Factory
    Private Property _curIsShowP57name As Boolean = False
    Private Property _lastP41ID As Integer = 0
    Private Property _lastDate As Date
    Private Property _showProjectRow As Boolean = False

    Private Property _lisProRow As List(Of ProRow)
    Public Class ProRow
        Public Property recDate As Date?
        Public Property Time As String
        Public Property recType As String
        Public Property recName As String
        Public Property IsClosed As Boolean
        Public Property Tags As String
        Public Property recPID As Integer
        Public Property Prefix As String
        Public Property ClueUrl As String
        Public Property Tooltip As String
        Public Property BackColor As String
        Public Property Status As String
        Public Property Receivers As String
        Public Property NavigateUrl As String
        Public Property p41ID As Integer
        Public Property Project As String
        Public Property ImageUrl As String
        Public Sub New(intPID As Integer, strPrefix As String)
            Me.Prefix = strPrefix
            Me.recPID = intPID
        End Sub
    End Class
    
    Public Property FirstDayMinus As Integer
        Get
            Return CInt(Me.cbxFirstDay.SelectedValue)
        End Get
        Set(value As Integer)
            cbxFirstDay.SelectedValue = value.ToString
        End Set
    End Property
    Public ReadOnly Property ProgramRowCount As Integer
        Get
            Return rpProgram.Items.Count
        End Get
    End Property



    Public Property Prefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property
    Public Property o25ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidO25ID.Value)
        End Get
        Set(value As Integer)
            hidO25ID.Value = value.ToString
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
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(d0 As Date)
        If Me.RecordPID = 0 Then
            Return
        End If
        _lisProRow = New List(Of ProRow)
        _lastDate = DateSerial(2000, 1, 1)
        Dim d1 As Date = d0.AddDays(Me.FirstDayMinus)
        
       
        fill_o22(d1)
        If hidPrefix.Value <> "p56" Then
            fill_p56(d1)
            cmdTasks.Visible = factory.SysUser.j04IsMenu_Task
        End If

        lblNoAppointments.Visible = False
        If _lisProRow.Count = 0 Then
            lblNoAppointments.Visible = True
            If hidPrefix.Value = "p56" Then
                lblNoAppointments.Text = "Žádné kalendářové události k úkolu."
            Else
                lblNoAppointments.Text = "Žádné úkoly/události."
            End If

        End If
        rpProgram.DataSource = _lisProRow.OrderBy(Function(p) p.recDate)
        rpProgram.DataBind()


        cmdSchedulers.Visible = factory.SysUser.j04IsMenu_Scheduler

        If Me.o25ID > 0 Then
            Dim cO25 As BO.o25App = factory.o25AppBL.Load(Me.o25ID)
            linkO25.Text = cO25.o25Name
            linkO25.NavigateUrl = cO25.o25Url
            linkO25.Visible = True
        End If
    End Sub
    Private Sub fill_o22(d1 As Date)
        Dim intRecordPID As Integer = Me.RecordPID
        Dim mq As New BO.myQueryO22
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TopRecordsOnly = 100
        mq.MG_SortString = "a.o22DateFrom,a.o22DateUntil"
        'mq.SpecificQuery = BO.myQueryO22_SpecificQuery.AllowedForRead
        Select Case hidPrefix.Value
            Case "p28"
                mq.p28ID = intRecordPID
            Case "p41"
                mq.p41ID = intRecordPID
            Case "j02"
                mq.j02IDs = BO.BAS.ConvertInt2List(intRecordPID)
            Case "p56"
                mq.p56ID = intRecordPID
            Case Else
                Return
        End Select
        mq.DateFrom = d1
        Dim lis As IEnumerable(Of BO.o22Milestone) = factory.o22MilestoneBL.GetList(mq)
        For Each cRec In lis
            Dim c As New ProRow(cRec.PID, "o22")
            With cRec
                c.ClueUrl = "clue_o22_record.aspx?pid=" & .PID.ToString

                c.recType = .o21Name
                c.recName = .o22Name
                If Len(c.recName) > 120 Then
                    c.Tooltip = .o22Name
                Else
                    c.Tooltip = .o21Name
                End If
                If .o22Description <> "" Then c.Tooltip += .o22Description
                c.BackColor = .Color.BackColor
                c.Status = .o21Name
                c.NavigateUrl = .o22AppUrl

                Select Case .o21Flag
                    Case BO.o21FlagEnum.DeadlineOrMilestone, BO.o21FlagEnum.TaskDeadline, BO.o21FlagEnum.ProjectDeadline
                        c.recDate = .o22DateUntil.Value
                        c.Time = Format(c.recDate, "HH:mm")
                        If c.Time = "00:00" Then c.Time = ""
                        c.ImageUrl = "Images/milestone.png"
                    Case BO.o21FlagEnum.EventFromUntil
                        If .o22DateFrom Is Nothing Then
                            c.recDate = .o22DateUntil.Value
                        Else
                            c.recDate = .o22DateFrom.Value
                            c.Time = Format(c.recDate, "HH:mm") & " - " & Format(.o22DateUntil.Value, "HH:mm")
                            If c.Time = "00:00 - 00:00" Then c.Time = ""
                        End If



                        If .o22IsAllDay Then
                            c.recDate = DateSerial(Year(.o22DateUntil), Month(.o22DateUntil), Day(.o22DateUntil))
                        End If
                        c.ImageUrl = "Images/event.png"

                End Select
                If Not .o22DateUntil Is Nothing Then
                    If .o22DateUntil < Now Then
                        c.IsClosed = True
                    End If
                End If
                
               
            End With
            _lisProRow.Add(c)
        Next
    End Sub
    Private Sub fill_p56(d1 As Date)
        Dim intRecordPID As Integer = Me.RecordPID
        Dim mq As New BO.myQueryP56
        mq.TopRecordsOnly = 100
        mq.Closed = BO.BooleanQueryMode.NoQuery

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
        mq.p56PlanUntil_D1 = d1 : mq.p56PlanUntil_D2 = d1.AddYears(1)
        Dim lis As IEnumerable(Of BO.p56Task) = factory.p56TaskBL.GetList(mq, True)
        For Each cRec In lis
            Dim c As New ProRow(cRec.PID, "p56")
            With cRec
                c.ClueUrl = "clue_p56_record.aspx?pid=" & .PID.ToString
                c.NavigateUrl = "p56_framework.aspx?pid=" & cRec.PID.ToString
                c.recName = BO.BAS.OM3(.p56Name, 100)

                If Len(.p56Name) > 120 Then
                    c.Tooltip = .p56Name
                Else
                    c.Tooltip = .p57Name
                End If
                If .p56Description <> "" Then c.Tooltip += .p56Description

                c.recDate = .p56PlanUntil
                c.Time = Format(.p56PlanUntil, "HH:mm")
                If c.Time = "00:00" Or c.Time = "23:59" Then c.Time = ""

                c.BackColor = .b02Color
                c.Status = .b02Name
                c.Receivers = .ReceiversInLine
                c.p41ID = .p41ID
                c.IsClosed = .IsClosed
                c.ImageUrl = "Images/task.png"
            End With

            _lisProRow.Add(c)
        Next

    End Sub

   

    Public Sub RefreshTasksWithoutDate(bolShowProjectRow As Boolean)
        _showProjectRow = bolShowProjectRow
        If Me.RecordPID = 0 Or Me.Prefix = "" Then Return
        panP56.Visible = False
        Dim mq As New BO.myQueryP56
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TerminNeniVyplnen = BO.BooleanQueryMode.TrueQuery
        mq.TopRecordsOnly = 100
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
        If lisP56.Count = 100 Then
            Me.p56Count.Text = String.Format("Podmínce vyhovuje více než {0} úkolů bez termínu!", 100)
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
       

        ''If Not BO.BAS.IsNullDBDate(cRec.p56PlanUntil) Is Nothing Then
        ''    With CType(e.Item.FindControl("p56PlanUntil"), Label)
        ''        .Text = BO.BAS.FD(cRec.p56PlanUntil, True, True)
        ''        If cRec.p56PlanUntil < Now Then
        ''            .Text += "...je po termínu!" : .ForeColor = Drawing.Color.Red
        ''        Else
        ''            .ForeColor = Drawing.Color.Green
        ''        End If
        ''    End With

        ''End If
        _lastP41ID = cRec.p41ID
    End Sub

    
    
   

    Private Sub cbxFirstDay_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFirstDay.SelectedIndexChanged
        factory.j03UserBL.SetUserParam("myscheduler-firstday", cbxFirstDay.SelectedValue)
        RefreshData(Today)
    End Sub


    
    Private Sub rpProgram_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpProgram.ItemDataBound
        Dim cRec As ProRow = CType(e.Item.DataItem, ProRow)

        With CType(e.Item.FindControl("link1"), HyperLink)

        End With
        If _lastDate <> cRec.recDate Then
            With CType(e.Item.FindControl("calDen"), Label)
                .Text = Day(cRec.recDate).ToString

                If cRec.recDate < Now Then .ForeColor = Drawing.Color.Red


            End With
            With CType(e.Item.FindControl("calMesic"), Label)
                Select Case Weekday(cRec.recDate, Microsoft.VisualBasic.FirstDayOfWeek.Monday)
                    Case 1 : .Text = "pondělí"
                    Case 2 : .Text = "úterý"
                    Case 3 : .Text = "středa"
                    Case 4 : .Text = "čtvrtek"
                    Case 5 : .Text = "pátek"
                    Case 6 : .Text = "sobota"
                    Case 7 : .Text = "neděle"
                End Select
                .Text += "<div style='border-top:solid 1px silver;'>" & Format(cRec.recDate, "MM") & "/" & Year(cRec.recDate).ToString & "</div>"

                
            End With
        End If
        
        With CType(e.Item.FindControl("Time"), Label)
            .Text = cRec.Time
        End With
        With CType(e.Item.FindControl("linkName"), HyperLink)
            If cRec.Prefix = "o22" Then
                .Target = "_blank"
            End If

            .Text = cRec.recName
            .ToolTip = cRec.Tooltip
            .NavigateUrl = cRec.NavigateUrl
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        With CType(e.Item.FindControl("tags"), Label)
            .Text = cRec.Tags
        End With
        With CType(e.Item.FindControl("img1"), Image)
            .ImageUrl = cRec.ImageUrl
        End With

     
        With CType(e.Item.FindControl("pm1"), HyperLink)
            .Attributes.Item("onclick") = "RCM('" & cRec.Prefix & "'," & cRec.recPID.ToString & ",this)"
        End With
        With CType(e.Item.FindControl("Status"), Label)
            .Text = cRec.Status
            If cRec.BackColor <> "" Then .Style.Item("background-color") = cRec.BackColor
            If .Text = "" Then .Text = "."
        End With


        
        _lastP41ID = cRec.p41ID
        _lastDate = cRec.recDate
    End Sub
End Class