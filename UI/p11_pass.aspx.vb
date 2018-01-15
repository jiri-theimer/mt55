Public Class p11_pass
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private _lis As IEnumerable(Of BO.p12Pass) = Nothing
    Private _curIndex As Integer = 0

    Public Property CurrentP11ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidP11ID.Value)
        End Get
        Set(value As Integer)
            hidP11ID.Value = value.ToString
        End Set
    End Property


    Private Sub p11_pass_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p11_pass"
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            datToday.SelectedDate = Today
            p12TimeStamp.SelectedDate = Today.AddHours(Now.Hour).AddMinutes(Now.Minute)


            Dim mq As New BO.myQueryP32

            p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq).Where(Function(p) p.p32AttendanceFlag > BO.p32AttendanceFlagENUM._None)
            p32ID.DataBind()


            RefreshRecord()
            RefreshList()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p11Attendance = Master.Factory.p11AttendanceBL.LoadByPersonAndDate(Master.Factory.SysUser.j02ID, datToday.SelectedDate)
        If Not cRec Is Nothing Then
            Me.CurrentP11ID = cRec.PID
        Else
            Me.CurrentP11ID = 0
        End If
       
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Dim datTimestamp As Date = CDate(datToday.SelectedDate).AddHours(CDate(p12TimeStamp.SelectedDate).Hour).AddMinutes(CDate(p12TimeStamp.SelectedDate).Minute)
        If Me.CurrentP11ID = 0 Then
            Dim cP11 As New BO.p11Attendance
            cP11.j02ID = Master.Factory.SysUser.j02ID
            cP11.p11Date = datToday.SelectedDate
            cP11.p11TodayStart = datTimestamp
            If Master.Factory.p11AttendanceBL.Save(cP11) Then
                RefreshRecord()
            Else
                Master.Notify(Master.Factory.p11AttendanceBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Return
            End If
        End If


        Dim c As New BO.p12Pass
        c.p11ID = Me.CurrentP11ID
        c.p12Flag = CType(Me.p12Flag.SelectedValue, BO.p12FlagENUM)
        If c.p12Flag = BO.p12FlagENUM.Aktivita Then
            c.p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
        End If

        c.p12Description = Me.p12Description.Text
        c.p12TimeStamp = datTimestamp
        
        If Master.Factory.p11AttendanceBL.SaveP12(c) Then
            RefreshList()
        Else
            Master.Notify(Master.Factory.p11AttendanceBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub

    Private Sub RefreshList()
        panRecord.Visible = False
        If Me.CurrentP11ID = 0 Then
            rp1.DataSource = Nothing
            rp1.DataBind()
            Return
        End If

        _curIndex = 0
        _lis = Master.Factory.p11AttendanceBL.GetListP12(Me.CurrentP11ID).OrderBy(Function(p) p.p12TimeStamp)
        rp1.DataSource = _lis
        rp1.DataBind()

        If _lis.Where(Function(p) p.p12Flag = BO.p12FlagENUM.Odchod).Count > 0 Then
            Dim cT As New BO.clsTime
            TotalDuration.Text = cT.GetTimeFromSeconds(_lis.Sum(Function(p) p.p12Duration * 60))
        Else
            TotalDuration.Text = ""
        End If
        

    End Sub

    Private Sub datToday_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles datToday.SelectedDateChanged
        If datToday.IsEmpty Then
            datToday.SelectedDate = Today
        End If
        RefreshRecord()
        RefreshList()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p12Pass = CType(e.Item.DataItem, BO.p12Pass)
        Select Case cRec.p12Flag
            Case BO.p12FlagENUM.Prichod
                CType(e.Item.FindControl("p32Name"), Label).Text = "Příchod"
            Case BO.p12FlagENUM.Odchod
                CType(e.Item.FindControl("p32Name"), Label).Text = "Odchod"
            Case BO.p12FlagENUM.Aktivita
                CType(e.Item.FindControl("p32Name"), Label).Text = cRec.p32Name
        End Select

        CType(e.Item.FindControl("p12Description"), Label).Text = cRec.p12Description
        CType(e.Item.FindControl("p12TimeStamp"), Label).Text = Format(cRec.p12TimeStamp, "HH:mm")

        If cRec.p12ActivityDuration > 0 Then
            Dim cT As New BO.clsTime
            CType(e.Item.FindControl("Duration"), Label).Text = cT.GetTimeFromSeconds(cRec.p12ActivityDuration * 60)
        End If
        'If cRec.p32ID > 0 Then
        '    If _lis.Count - 1 > _curIndex Then
        '        Dim cRecNext As BO.p12Pass = _lis(_curIndex + 1)
        '        Dim cT As New BO.clsTime
        '        CType(e.Item.FindControl("Duration"), Label).Text = cT.GetTimeFromSeconds(cRecNext.p12Duration * 60)
        '    End If
        'End If
        'If _curIndex < _lis.Count - 1 Then
        '    Dim cRecNext As BO.p12Pass = _lis(_curIndex + 1)
        '    If cRecNext.p12Duration > 0 Then
        '        Dim cT As New BO.clsTime
        '        CType(e.Item.FindControl("Duration"), Label).Text = cT.GetTimeFromSeconds(cRecNext.p12Duration * 60)
        '    End If
        'End If
        
        _curIndex += 1
    End Sub

    Private Sub p11_pass_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.p12Flag.SelectedValue = "3" Then
            Me.p32ID.Visible = True
        Else
            Me.p32ID.Visible = False
        End If
        cmdNew.Text = String.Format("Zapsat záznam docházky ({0})", Format(datToday.SelectedDate, "dd.MM.yyyy"))
    End Sub

    Private Sub cmdNew_Click(sender As Object, e As EventArgs) Handles cmdNew.Click
        Me.p12Flag.Items.FindByValue("1").Enabled = True
        Me.p12Flag.Items.FindByValue("2").Enabled = True
        Me.p12Flag.Items.FindByValue("3").Enabled = True
        Me.p12Description.Text = ""

        Dim lis As IEnumerable(Of BO.p12Pass) = Master.Factory.p11AttendanceBL.GetListP12(Me.CurrentP11ID).OrderByDescending(Function(p) p.p12TimeStamp)
        Dim cRecLast As BO.p12Pass = Nothing
        If lis.Count > 0 Then cRecLast = lis(0)
        If cRecLast Is Nothing Then
            Me.p12Flag.SelectedValue = "1"
            Me.p12Flag.Items.FindByValue("2").Enabled = False
            Me.p12Flag.Items.FindByValue("3").Enabled = False
        Else
            Select Case cRecLast.p12Flag
                Case BO.p12FlagENUM.Prichod
                    Me.p12Flag.Items.FindByValue("1").Enabled = False
                    Me.p12Flag.SelectedValue = "2"
                Case BO.p12FlagENUM.Odchod
                    Master.Notify("Po odchodu je již docházka uzavřena.", NotifyLevel.InfoMessage)
                    Return
                Case BO.p12FlagENUM.Aktivita
                    Me.p12Flag.SelectedValue = "1"
            End Select
        End If
        panRecord.Visible = True
        p12TimeStamp.SelectedDate = CDate(datToday.SelectedDate).AddHours(Now.Hour).AddMinutes(Now.Minute)
        
    End Sub
End Class