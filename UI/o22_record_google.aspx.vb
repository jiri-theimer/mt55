Public Class o22_record_google
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub o22_record_google_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
            End With


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)
        If cRec.o25ID = 0 Then Master.StopPage("V události nebyl zvolen cílový google kalendář.") : Return

        Dim cO25 As BO.o25App = Master.Factory.o25AppBL.Load(cRec.o25ID)
        Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = Master.Factory.o22MilestoneBL.GetList_o20(cRec.PID)
        Me.o25Code.Value = cO25.o25Code
        Me.o25Name.Text = cO25.o25Name

        With cRec
            Me.o22Name.Text = .o22Name & " (" & .o21Name & ")"
            If cRec.o22Description <> "" Then
                Me.o22Description.Text = cRec.o22Description & vbCrLf
            End If

            If cRec.p41ID <> 0 Then
                Me.o22Description.Text += String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cRec.p41ID, cRec.Project)
            End If
            If cRec.p28ID <> 0 Then
                Me.o22Description.Text += String.Format("<a href='{0}/dr.aspx?prefix=p28&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cRec.p28ID, cRec.Contact)
            End If
            If cRec.p56ID <> 0 Then
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(cRec.p56ID)
                Me.o22Description.Text += String.Format("<a href='{0}/dr.aspx?prefix=p56&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.PID, cTask.NameWithTypeAndCode)
                Me.o22Description.Text += vbCrLf & String.Format("<a href='{0}/dr.aspx?prefix=p41&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cTask.p41ID, cTask.Client & " - " & cTask.ProjectCodeAndName)

            End If
            If cRec.p91ID <> 0 Then
                Dim cInvoice As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(cRec.p91ID)
                Me.o22Description.Text += String.Format("<a href='{0}/dr.aspx?prefix=p91&pid={1}'>{2}</a>", Master.Factory.x35GlobalParam.AppHostUrl, cRec.p91ID, cInvoice.p92Name & ": " & cInvoice.p91Code)
            End If

            Me.o22Location.Text = .o22Location
            If Not .o22DateFrom Is Nothing Then
                Me.o22DateFrom.Text = BO.BAS.FD(.o22DateFrom, True, False) & " - "
            End If
            Me.o22DateUntil.Text = BO.BAS.FD(.o22DateUntil, True, False)
            Me.o22Description.Text = BO.BAS.CrLfText2Html(.o22Description)
            Me.Attendees.Text = String.Join("<br>", lisO20.Select(Function(p) p.Email))
            Me.o22ReminderBeforeUnits.Text = .o22ReminderBeforeUnits.ToString
            Me.o22ReminderBeforeMetric.Text = .o22ReminderBeforeMetric
            hidColorID.Value = .o22ColorID
            If .Color.BackColor <> "" Then
                lblHeader.BackColor = System.Drawing.Color.FromName(.Color.BackColor)
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