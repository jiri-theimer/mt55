Public Class o22_record_google
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub o22_record_google_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("description") = ""
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                hidAppID.Value = Request.Item("appid")
                hidAppUrl.Value = Server.UrlDecode(Request.Item("appurl"))
                If .DataPID = 0 And hidAppID.Value = "" Then .StopPage("pid or appid missing")
            End With


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 And hidAppID.Value <> "" Then
            'smazat událost v kalendáři
            Me.o25Code.Value = Master.Factory.o25AppBL.Load(BO.BAS.IsNullInt(Request.Item("o25id"))).o25Code
            cmdOdeslat.InnerText = "Odstranit událost v Google kalendáři"
            cmdOdeslat.Attributes.Item("onclick") = "Handle_DeleteEvent()"
            panRecord.Visible = False
            Return
        End If

        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)
        If cRec.o25ID = 0 Then Master.StopPage("V události nebyl zvolen cílový google kalendář.") : Return

        Dim cO25 As BO.o25App = Master.Factory.o25AppBL.Load(cRec.o25ID)
        Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = Master.Factory.o22MilestoneBL.GetList_o20(cRec.PID)
        Me.o25Code.Value = cO25.o25Code
        Me.o25Name.Text = cO25.o25Name

        With cRec
            Me.o22Name.Text = .o22Name & " (" & .o21Name & ")"
            If .o22Description <> "" Then
                ViewState("description") = BO.BAS.CrLfText2Html(.o22Description)
            End If

            If .p41ID <> 0 Then
                ViewState("description") += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p41ID, .Project)
            End If
            If .p28ID <> 0 Then
                ViewState("description") += String.Format("<a href='{0}/dr.aspx?prefix=p28&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p28ID, .Contact)
            End If
            If .p56ID <> 0 Then
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(.p56ID)
                ViewState("description") += String.Format("<a href='{0}/dr.aspx?prefix=p56&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.PID, cTask.NameWithTypeAndCode)
                ViewState("description") += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.p41ID, cTask.Client & " - " & cTask.ProjectCodeAndName)

            End If
            If .p91ID <> 0 Then
                Dim cInvoice As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(.p91ID)
                ViewState("description") += String.Format("<a href='{0}/dr.aspx?prefix=p91&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, .p91ID, cInvoice.p92Name & ": " & cInvoice.p91Code)
            End If

            Me.o22Location.Text = .o22Location
            If Not .o22DateFrom Is Nothing Then
                Me.o22DateFrom.Text = BO.BAS.FD(.o22DateFrom, True, False) & " - "
                hidStart.Value = Format(.o22DateFrom, "yyyy-MM-ddTHH:mm:sszzz")
            Else
                hidStart.Value = Format(.o22DateUntil, "yyyy-MM-ddTHH:mm:sszzz")
            End If
            Me.o22DateUntil.Text = BO.BAS.FD(.o22DateUntil, True, False)
            hidEnd.Value = Format(.o22DateUntil, "yyyy-MM-ddTHH:mm:sszzz")
            If .o22IsAllDay Then
                hidDateDateType.Value = "date"
            Else
                hidDateDateType.Value = "dateTime"
            End If


            Me.o22Description.Text = BO.BAS.CrLfText2Html(ViewState("description"))
            Me.Attendees.Text = String.Join("<br>", lisO20.Select(Function(p) p.Email))
            Me.hidAttendees.Value = String.Join(",", lisO20.Select(Function(p) "{ 'email': '" + p.Email + "' }"))
            Me.o22ReminderBeforeUnits.Text = .o22ReminderBeforeUnits.ToString
            Me.o22ReminderBeforeMetric.Text = .o22ReminderBeforeMetric
            hidColorID.Value = .o22ColorID
            If .Color.BackColor <> "" Then
                lblHeader.BackColor = System.Drawing.Color.FromName(.Color.BackColor)
            End If

            hidAppID.Value = .o22AppID


            If .o22AppID <> "" Then
                cmdOdeslat.InnerText = "Aktualizovat událost v Google kalendáři"
                cmdOdeslat.Attributes.Item("onclick") = "Handle_UpdateEvent()"
            End If


            If .o22ReminderBeforeUnits > 0 Then
                Select Case cRec.o22ReminderBeforeMetric
                    Case "m" : hidMinutesBefore.Value = .o22ReminderBeforeUnits.ToString
                    Case "d" : hidMinutesBefore.Value = (cRec.o22ReminderBeforeUnits * 24 * 60).ToString
                    Case "h" : hidMinutesBefore.Value = (cRec.o22ReminderBeforeUnits * 60).ToString
                End Select
            End If
        End With
    End Sub
End Class