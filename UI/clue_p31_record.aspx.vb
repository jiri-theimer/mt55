Public Class clue_p31_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Clue

    Private Sub clue_p31_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        files1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

            If Request.Item("dr") = "1" Then
                panContainer.Style.Clear()
            End If
            RefreshRecord()

            
            cmdEdit.NavigateUrl = "javascript: parent.sw_everywhere('p31_record.aspx?pid=" & Master.DataPID.ToString & "','Images/worksheet.png',true)"
        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record not found")
        Dim cDisp As BO.p31WorksheetDisposition = Master.Factory.p31WorksheetBL.InhaleRecordDisposition(Master.DataPID)
        If cDisp.RecordDisposition = BO.p31RecordDisposition._NoAccess Then Master.StopPage("K úkonu nemáte přístup.", "", True) : Return
        If cDisp.RecordDisposition = BO.p31RecordDisposition.CanEdit Or cRec.j02ID = Master.Factory.SysUser.j02ID Then
            imgClone.Visible = True : cmdClone.Visible = True : cmdClone.NavigateUrl = "javascript: parent.sw_everywhere('p31_record.aspx?clone=1&pid=" & Master.DataPID.ToString & "','Images/copy.png',true)"
        End If
        With cRec
            Me.Project.Text = .p41Name
            Me.Client.Text = .ClientName
            Me.p32Name.Text = .p32Name
            Me.ph1.Text = .p34Name
            Me.Person.Text = .Person
            Me.p31Date.Text = BO.BAS.FD(.p31Date)
            Me.p31Value_Orig.Text = BO.BAS.FN(.p31Value_Orig)
            Select Case .p33ID
                Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                    Me.p31Value_Orig.Text += " " & .j27Code_Billing_Orig
                Case BO.p33IdENUM.Cas
                    If Not .p31DateTimeFrom_Orig Is Nothing Then
                        Me.TimePeriod.Text = .TimeFrom & "-" & .TimeUntil
                    End If
            End Select
            Me.p31Text.Text = BO.BAS.CrLfText2Html(.p31Text)
            If .p56ID <> 0 Then
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(.p56ID)
                Me.Task.Text = cTask.p56Code & " - " & cTask.p56Name
                Me.p57Name.Text = cTask.p57Name & ":"
            End If
            If .j02ID_ContactPerson > 0 Then
                Me.ContactPerson.Text = Master.Factory.j02PersonBL.Load(.j02ID_ContactPerson).FullNameDescWithJobTitle
            End If
            Me.Timestamp.Text = .Timestamp
        End With
        If cRec.o23ID_First <> 0 Then
            Dim cDoc As BO.o23Doc = Master.Factory.o23DocBL.Load(cRec.o23ID_First)
            Me.o23Name.Text = cDoc.x23Name & ": " & cDoc.o23Code & " - " & cDoc.o23Name
            ''Me.files1.RefreshData_O23(cDoc.PID)
        Else
            panFiles.Visible = False
        End If


    End Sub
End Class