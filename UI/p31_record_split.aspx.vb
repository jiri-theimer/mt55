Public Class p31_record_split
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_record_split_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    .StopPage("PID is missing.")
                End If
                ViewState("guid") = Request.Item("guid")
                ''If ViewState("guid") = "" Then Master.StopPage("guid is missing")

                .HeaderText = "Rozdělení časového úkonu na 2 kusy"
                .HeaderIcon = "Images/worksheet_32.png"
                .AddToolbarButton("Uložit změny", "save", , "Images/save.png")

            End With

            RefreshRecord()
            
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        If cRec Is Nothing Then
            Master.StopPage("Záznam nebyl nalezen.", True)
        End If
        With cRec
            If .p33ID <> BO.p33IdENUM.Cas Then
                Master.StopPage("Funkce [Rozdělit] funguje pouze pro časové úkony.", True)
            End If
            If ViewState("guid") = "" And .p71ID > BO.p71IdENUM.Nic Then
                Master.StopPage("Záznam byl již dříve schválen.")
            End If
            If .p91ID <> 0 Then Master.StopPage("Záznam byl již dříve vyfakturován.")
        End With
        
        Dim cD As BO.p31WorksheetDisposition = Master.Factory.p31WorksheetBL.InhaleRecordDisposition(Master.DataPID)
        Select Case cD.RecordDisposition
            Case BO.p31RecordDisposition.CanApprove, BO.p31RecordDisposition.CanApproveAndEdit, BO.p31RecordDisposition.CanEdit
            Case Else
                Master.StopPage("Nedisponujete oprávněním rozdělit tento úkon.", True)
        End Select
        With cRec
            Me.Person.Text = .Person
            Me.hours1.Value = .p31Hours_Orig
            Me.txt1.Text = .p31Text
            Me.txt2.Text = .p31Text
            Me.p31Date.Text = BO.BAS.FD(.p31Date)
            Me.Project.Text = .p41Name
            Me.p31Hours.Text = BO.BAS.FN(.p31Hours_Orig)
        End With



    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
            Dim intP31ID_New As Integer = Master.Factory.p31WorksheetBL.SplitRecord(Master.DataPID, BO.BAS.IsNullNum(Me.hours1.Value), Me.txt1.Text, BO.BAS.IsNullNum(Me.hours2.Value), Me.txt2.Text)
            If intP31ID_New > 0 Then

                If ViewState("guid") <> "" Then
                    Dim c As New BO.p85TempBox()
                    c.p85GUID = ViewState("guid")
                    c.p85DataPID = intP31ID_New
                    Master.Factory.p85TempBoxBL.Save(c)

                    AddRecord2Approving(intP31ID_New)
                    AddRecord2Approving(Master.DataPID)
                    Master.Factory.p31WorksheetBL.UpdateTempField("p31Value_Orig", Me.hours1.Value, ViewState("guid"), Master.DataPID)
                    Master.Factory.p31WorksheetBL.UpdateTempField("p31Hours_Orig", Me.hours1.Value, ViewState("guid"), Master.DataPID)
                    Master.Factory.p31WorksheetBL.UpdateTempField("p31Text", Me.txt1.Text, ViewState("guid"), Master.DataPID)
                End If
                
                Master.CloseAndRefreshParent()
            Else
                Master.Notify(Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End If
    End Sub

    Private Sub AddRecord2Approving(intP31ID As Integer)
        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(intP31ID)
        Dim cApprove As New BO.p31WorksheetApproveInput(cRec.PID, cRec.p33ID)
        With cApprove
            .GUID_TempData = ViewState("guid")
            .p31Date = cRec.p31Date
            .p71id = BO.p71IdENUM.Schvaleno
            If cRec.p32IsBillable Then
                .p72id = BO.p72IdENUM.Fakturovat
                .Value_Approved_Billing = cRec.p31Value_Orig
                .Value_Approved_Internal = cRec.p31Value_Orig
                .Rate_Billing_Approved = cRec.p31Rate_Billing_Orig
                If cRec.p31Rate_Billing_Orig = 0 Then
                    .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                End If
            Else
                .p72id = BO.p72IdENUM.SkrytyOdpis
            End If
        End With
        Master.Factory.p31WorksheetBL.Save_Approving(cApprove, True)
    End Sub
End Class