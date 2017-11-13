Public Class o22_record_outlook
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private Property _Description As String

    Private Sub o22_record_outlook_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

            End With

            RefreshRecord()
        End If
    End Sub
    

    Private Sub RefreshRecord()
        _Description = ""
        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)
        Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = Master.Factory.o22MilestoneBL.GetList_o20(cRec.PID)

        
        With cRec
            Me.o22Name.Text = .o22Name & " (" & .o21Name & ")"

            If .o22Description <> "" Then
                _Description = BO.BAS.CrLfText2Html(.o22Description)
            End If

            If .p41ID <> 0 Then
                _Description += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p41ID, .Project)
            End If
            If .p28ID <> 0 Then
                _Description += String.Format("<a href='{0}/dr.aspx?prefix=p28&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p28ID, .Contact)
            End If
            If .p56ID <> 0 Then
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(.p56ID)
                _Description += String.Format("<a href='{0}/dr.aspx?prefix=p56&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.PID, cTask.NameWithTypeAndCode)
                _Description += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.p41ID, cTask.Client & " - " & cTask.ProjectCodeAndName)

            End If
            If .p91ID <> 0 Then
                Dim cInvoice As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(.p91ID)
                _Description += String.Format("<a href='{0}/dr.aspx?prefix=p91&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p91ID, cInvoice.p92Name & ": " & cInvoice.p91Code)
            End If

            Me.o22Location.Text = .o22Location
            If Not .o22DateFrom Is Nothing Then
                Me.o22DateFrom.Text = BO.BAS.FD(.o22DateFrom, True, False) & " - "
            End If
            If .o22DateFrom Is Nothing Then .o22DateFrom = .o22DateUntil
            Me.o22DateUntil.Text = BO.BAS.FD(.o22DateUntil, True, False)

            


            Me.o22Description.Text = BO.BAS.CrLfText2Html(_Description)
            Me.Attendees.Text = String.Join("<br>", lisO20.Select(Function(p) p.Email))

            Me.o22ReminderBeforeUnits.Text = .o22ReminderBeforeUnits.ToString
            Me.o22ReminderBeforeMetric.Text = .o22ReminderBeforeMetric

            If .Color.BackColor <> "" Then
                lblHeader.BackColor = System.Drawing.Color.FromName(.Color.BackColor)
            End If


            ''Dim c As New Independentsoft.Msg.Message
            ' ''c.EntryId = System.Text.Encoding.UTF8.GetBytes("o22-" & .PID.ToString)

            ''c.MessageClass = "IPM.Appointment"
            ''c.Encoding = System.Text.Encoding.GetEncoding(1250)
            ''c.Subject = Me.o22Name.Text

            If Not System.IO.File.Exists(Master.Factory.x35GlobalParam.TempFolder & "\marktime_calendar_event" & .PID.ToString & ".ics") Or Request.Item("force") = "1" Then
                hidIcsFile.Value = Master.Factory.o22MilestoneBL.CreateICalendarTempFullPath(.PID)
            Else
                hidIcsFile.Value = "marktime_calendar_event" & .PID.ToString & ".ics"
            End If

            cmdOdeslat.Visible = True

            ''Dim rtfBody As Byte() = System.Text.Encoding.UTF8.GetBytes("{\rtf1\ansi\ansicpg1252\fromhtml1 \htmlrtf0  " & _Description & "}")

            ''Dim cHTML As New BO.clsHandleHtml()
            ''_Description = "<html><body><span>" & cHTML.ToFopCZ(_Description) & "</span></body></html>"
            ''c.BodyHtml = System.Text.Encoding.UTF8.GetBytes(_Description)
            ''c.Body = .o22Description
            ''c.Location = .o22Location
            ''Select Case .o22ColorID
            ''    Case "9", "1"
            ''        c.NoteColor = Independentsoft.Msg.NoteColor.Blue
            ''    Case "7", "2"
            ''        c.NoteColor = Independentsoft.Msg.NoteColor.Green
            ''    Case "3"
            ''        c.NoteColor = Independentsoft.Msg.NoteColor.Pink
            ''    Case "5"
            ''        c.NoteColor = Independentsoft.Msg.NoteColor.Yellow

            ''End Select
            ''If cRec.o22IsAllDay Then
            ''    c.IsAllDayEvent = True
            ''    c.AppointmentStartTime = DateSerial(Year(.o22DateFrom), Month(.o22DateFrom), Day(.o22DateFrom))
            ''    c.AppointmentEndTime = DateSerial(Year(.o22DateUntil), Month(.o22DateUntil), Day(.o22DateUntil))
            ''Else
            ''    c.AppointmentStartTime = .o22DateFrom
            ''    c.AppointmentEndTime = .o22DateUntil
            ''End If
            ''If .o22ReminderBeforeUnits > 0 Then
            ''    c.IsReminderSet = True
            ''    Select Case .o22ReminderBeforeMetric
            ''        Case "m" : c.ReminderMinutesBeforeStart = .o22ReminderBeforeUnits
            ''        Case "d" : c.ReminderMinutesBeforeStart = (cRec.o22ReminderBeforeUnits * 24 * 60)
            ''        Case "h" : c.ReminderMinutesBeforeStart = (cRec.o22ReminderBeforeUnits * 60)
            ''    End Select
            ''End If

            ''For Each cO20 In lisO20
            ''    Dim rec As New Independentsoft.Msg.Recipient()
            ''    rec.EmailAddress = cO20.Email
            ''    rec.DisplayName = cO20.Person
            ''    c.Recipients.Add(rec)
            ''Next



            ''c.Save(Master.Factory.x35GlobalParam.TempFolder & "\outlook_event_" & Master.Factory.SysUser.PID.ToString & ".msg", True)
            ''ViewState("msgfile") = "outlook_event_" & Master.Factory.SysUser.PID.ToString & ".msg"


        End With

        
    End Sub
End Class