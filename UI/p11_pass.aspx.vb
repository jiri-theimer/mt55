Public Class p11_pass
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

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
            p12TimeStamp.SelectedDate = Now

            Dim mq As New BO.myQueryP32

            p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
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
        If Me.CurrentP11ID = 0 Then
            Dim cP11 As New BO.p11Attendance
            cP11.j02ID = Master.Factory.SysUser.j02ID
            cP11.p11Date = datToday.SelectedDate
            cP11.p11TodayStart = Me.p12TimeStamp.SelectedDate
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
        c.p12TimeStamp = Me.p12TimeStamp.SelectedDate

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

        Dim lis As IEnumerable(Of BO.p12Pass) = Master.Factory.p11AttendanceBL.GetListP12(Me.CurrentP11ID).OrderBy(Function(p) p.p12TimeStamp)
        rp1.DataSource = lis
        rp1.DataBind()

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

    End Sub

    Private Sub p11_pass_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.p12Flag.SelectedValue = "3" Then
            Me.p32ID.Visible = True
        Else
            Me.p32ID.Visible = False
        End If
    End Sub

    Private Sub cmdNew_Click(sender As Object, e As EventArgs) Handles cmdNew.Click
        Me.p12Flag.Items.FindByValue("1").Enabled = True
        Me.p12Flag.Items.FindByValue("2").Enabled = True
        Me.p12Flag.Items.FindByValue("3").Enabled = True

        Dim lis As IEnumerable(Of BO.p12Pass) = Master.Factory.p11AttendanceBL.GetListP12(Me.CurrentP11ID).OrderBy(Function(p) p.p12TimeStamp)
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
                    Me.p12Flag.SelectedValue = "1"
                    Me.p12Flag.Items.FindByValue("2").Enabled = False
                    Me.p12Flag.Items.FindByValue("3").Enabled = False
                Case BO.p12FlagENUM.Aktivita
                    Me.p12Flag.SelectedValue = "1"
            End Select
        End If
        panRecord.Visible = True
        p12TimeStamp.SelectedDate = CDate(datToday.SelectedDate).AddHours(Now.Hour).AddMinutes(Now.Minute)
    End Sub
End Class