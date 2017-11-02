Public Class p11_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _cbxSource As List(Of BO.p32Activity)
    Private Property _curMyButtons As List(Of clsMyButtons)

    Private Class clsMyButtons
        Public Property p32ID As Integer
        Public Property p32AttendanceFlag As BO.p32AttendanceFlagENUM = BO.p32AttendanceFlagENUM._None
        Public Sub New(p32ID As Integer, flag As Integer)
            Me.p32ID = p32ID
            Me.p32AttendanceFlag = flag
        End Sub
    End Class

    Private Sub p11_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        gridP31.Factory = Master.Factory
        Master.HelpTopicID = "p11_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID
            Master.SiteMenuValue = "p11_framework"
            datToday.SelectedDate = Today
            p11TodayStart.SelectedDate = Now
            p11TodayEnd.SelectedDate = Now
            With Master.Factory.j03UserBL
                .InhaleUserParams("p11_framework-p41id", "p11_framework-buttons")
                Me.p41ID_Default.Value = .GetUserParam("p11_framework-p41id")
                If Me.p41ID_Default.Value = "" Then
                    'najít výchozí projekt pro neproduktivní hodiny
                    Me.p41ID_Default.Value = Master.Factory.p11AttendanceBL.FindDefaultP41ID().ToString
                    Master.Factory.j03UserBL.SetUserParam("p11_framework-p41id", Me.p41ID_Default.Value)
                End If
                If Me.p41ID_Default.Value <> "" Then
                    Me.p41ID_Default.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, BO.BAS.IsNullInt(Me.p41ID_Default.Value), True)
                End If
                If .GetUserParam("p11_framework-buttons") <> "" Then
                    Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(.GetUserParam("p11_framework-buttons"), "|")
                    For Each s In lis
                        Dim c As New BO.p85TempBox
                        c.p85GUID = ViewState("guid")
                        Dim a() As String = Split(s, "-")
                        c.p85DataPID = BO.BAS.IsNullInt(a(0))
                        c.p85OtherKey1 = BO.BAS.IsNullInt(a(1))
                        Master.Factory.p85TempBoxBL.Save(c)
                    Next
                    RefreshTempList()
                End If
            End With
            RefreshButtons()
            RefreshRecord()
        End If
    End Sub
    Private Sub p11_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        cmdNextDay.Enabled = Not datToday.IsEmpty
        cmdPrevDay.Enabled = Not datToday.IsEmpty
    End Sub
    Private Sub RefreshRecord()
        linkEnd.Text = "Odchod" : linkStart.Text = "Příchod"
        SetupGrid()

        Dim cRec As BO.p11Attendance = Master.Factory.p11AttendanceBL.LoadByPersonAndDate(Master.Factory.SysUser.j02ID, datToday.SelectedDate)
        If cRec Is Nothing Then Return
        With cRec
            If Not .p11TodayStart Is Nothing Then
                p11TodayStart.SelectedDate = .p11TodayStart
                linkStart.Text = BO.BAS.OM2(linkStart.Text, Format(.p11TodayStart, "HH:mm"))
            End If
            If Not .p11TodayEnd Is Nothing Then
                p11TodayEnd.SelectedDate = .p11TodayEnd
                linkEnd.Text = BO.BAS.OM2(linkEnd.Text, Format(.p11TodayEnd, "HH:mm"))
            End If
        End With

    End Sub

    Private Sub SetupGrid()
        With gridP31
            .Visible = True
            .MasterDataPID = Master.Factory.SysUser.j02ID
            .ExplicitDateFrom = datToday.SelectedDate
            .ExplicitDateUntil = datToday.SelectedDate
            .RecalcVirtualRowCount()
            .Rebind(False)
        End With
    End Sub

    Private Sub RefreshButtons()
        Dim mq As New BO.myQueryP32
        mq.p33ID = BO.p33IdENUM.Cas
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Dim lis As List(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq).Where(Function(p) p.p32AttendanceFlag > BO.p32AttendanceFlagENUM._None).ToList

        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each cTemp In lisTEMP
            If Not lis.Exists(Function(p) p.PID = cTemp.p85DataPID) Then
                Dim c As BO.p32Activity = Master.Factory.p32ActivityBL.Load(cTemp.p85DataPID)
                c.p32AttendanceFlag = cTemp.p85OtherKey1
                c.p32HelpText = "mybutton"
                lis.Add(c)
            End If
        Next

        rp1.DataSource = lis
        rp1.DataBind()

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p32Activity = CType(e.Item.DataItem, BO.p32Activity)
        With CType(e.Item.FindControl("link1"), HyperLink)
            If Len(cRec.p32Name) > 30 Then
                .ToolTip = cRec.p32Name
                .Text = BO.BAS.OM3(cRec.p32Name, 28)
            Else
                .Text = cRec.p32Name
            End If
            If cRec.p32IsBillable Then
                .NavigateUrl = "javascript:p31_entry_attendance(" & cRec.PID.ToString & ",'')"
            Else

                If cRec.p32AttendanceFlag = BO.p32AttendanceFlagENUM.TimeInterval Then
                    .NavigateUrl = "javascript:p31_entry_attendance_scheduler(" & cRec.PID.ToString & "," & Me.p41ID_Default.Value & ")"
                Else
                    .NavigateUrl = "javascript:p31_entry_attendance(" & cRec.PID.ToString & "," & Me.p41ID_Default.Value & ")"
                End If

            End If
            If cRec.p32HelpText = "mybutton" Then
                .ForeColor = Drawing.Color.Blue
            End If
        End With
    End Sub

   

    Private Sub datToday_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles datToday.SelectedDateChanged
        If datToday.IsEmpty Then
            datToday.SelectedDate = Today
        End If
        RefreshRecord()
    End Sub


   
    Private Sub cmdSaveStart_Click(sender As Object, e As EventArgs) Handles cmdSaveStart.Click
        ''If Me.p11TodayStart.IsEmpty Then
        ''    Master.Notify("Musíte vyplnit čas příchodu.", NotifyLevel.ErrorMessage)
        ''    Return
        ''End If
        
        Dim cRec As BO.p11Attendance = Master.Factory.p11AttendanceBL.LoadByPersonAndDate(Master.Factory.SysUser.j02ID, datToday.SelectedDate)
        If cRec Is Nothing Then cRec = New BO.p11Attendance
        cRec.j02ID = Master.Factory.SysUser.j02ID
        cRec.p11Date = datToday.SelectedDate
        If Me.p11TodayStart.IsEmpty Then
            cRec.p11TodayStart = Nothing
        Else
            cRec.p11TodayStart = Me.p11TodayStart.SelectedDate
        End If

        If Master.Factory.p11AttendanceBL.Save(cRec) Then
            RefreshRecord()
        Else
            Master.Notify(Master.Factory.p11AttendanceBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub


    Private Sub cmdSaveEnd_Click(sender As Object, e As EventArgs) Handles cmdSaveEnd.Click
        ''If Me.p11TodayEnd.IsEmpty Then
        ''    Master.Notify("Musíte vyplnit čas odchodu.", NotifyLevel.ErrorMessage)
        ''    Return
        ''End If
        Dim cRec As BO.p11Attendance = Master.Factory.p11AttendanceBL.LoadByPersonAndDate(Master.Factory.SysUser.j02ID, datToday.SelectedDate)
        If cRec Is Nothing Then cRec = New BO.p11Attendance
        cRec.j02ID = Master.Factory.SysUser.j02ID
        cRec.p11Date = datToday.SelectedDate

        If Me.p11TodayEnd.IsEmpty Then
            cRec.p11TodayEnd = Nothing
        Else
            cRec.p11TodayEnd = Me.p11TodayEnd.SelectedDate
        End If


        If Master.Factory.p11AttendanceBL.Save(cRec) Then
            RefreshRecord()
        Else
            Master.Notify(Master.Factory.p11AttendanceBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As ImageClickEventArgs) Handles cmdRefresh.Click
        RefreshRecord()
    End Sub

    Private Sub cmdPrevDay_Click(sender As Object, e As ImageClickEventArgs) Handles cmdPrevDay.Click
        Me.RadTabStrip1.SelectedIndex = 0
        Me.RadMultiPage1.SelectedIndex = 0
        datToday.SelectedDate = CDate(datToday.SelectedDate).AddDays(-1)
        RefreshRecord()
    End Sub

    

    Private Sub cmdNextDay_Click(sender As Object, e As ImageClickEventArgs) Handles cmdNextDay.Click
        datToday.SelectedDate = CDate(datToday.SelectedDate).AddDays(1)
        RefreshRecord()
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        SaveTempList()
        Dim c As New BO.p85TempBox()
        c.p85GUID = ViewState("guid")

        Master.Factory.p85TempBoxBL.Save(c)
        RefreshTempList()
    End Sub

    Private Sub RefreshTempList()
        Me.rp2.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        Me.rp2.DataBind()
    End Sub

    Private Sub rp2_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp2.ItemCommand
        SaveTempList()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempList()
            End If
        End If
    End Sub

    Private Sub rp2_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp2.ItemDataBound
        Dim mq As New BO.myQueryP32
        mq.p33ID = BO.p33IdENUM.Cas
        If _cbxSource Is Nothing Then _cbxSource = Master.Factory.p32ActivityBL.GetList(mq).OrderBy(Function(p) p.p34Name).ToList

        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox), intLastP34ID As Integer = 0

        CType(e.Item.FindControl("p85ID"), HiddenField).Value = cRec.PID.ToString

        With CType(e.Item.FindControl("p32ID"), DropDownList)
            If .Items.Count = 0 Then
                .DataSource = _cbxSource
                .DataBind()
            End If
        End With

        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p32ID"), DropDownList), cRec.p85DataPID.ToString)
        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p32AttendanceFlag"), DropDownList), cRec.p85OtherKey1.ToString)

        With CType(e.Item.FindControl("cmdDelete"), ImageButton)
            .CommandArgument = cRec.PID.ToString
        End With
    End Sub
    Private Sub SaveTempList()
        _curMyButtons = New List(Of clsMyButtons)
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rp2.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)

            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85DataPID = BO.BAS.IsNullInt(CType(ri.FindControl("p32ID"), DropDownList).SelectedValue)
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("p32AttendanceFlag"), DropDownList).SelectedValue)

            End With
            Master.Factory.p85TempBoxBL.Save(cRec)

            _curMyButtons.Add(New clsMyButtons(cRec.p85DataPID, cRec.p85OtherKey1))

        Next
    End Sub

    Private Sub cmdSaveSetting_Click(sender As Object, e As EventArgs) Handles cmdSaveSetting.Click
        SaveTempList()

        Master.Factory.j03UserBL.SetUserParam("p11_framework-p41id", Me.p41ID_Default.Value)

        Master.Factory.j03UserBL.SetUserParam("p11_framework-buttons", GetMyButtonsInLine())


        RefreshButtons()
        RefreshRecord()
    End Sub

    Private Function GetMyButtonsInLine() As String
        Dim lis As New List(Of String)
        For Each c In _curMyButtons
            lis.Add(c.p32ID.ToString & "-" & CInt(c.p32AttendanceFlag).ToString)
        Next
        Return String.Join("|", lis)
    End Function

    Private Sub p41ID_Default_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41ID_Default.AutoPostBack_SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p11_framework-p41id", Me.p41ID_Default.Value)
        RefreshButtons()
    End Sub
End Class