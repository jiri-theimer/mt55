Public Class clue_p92_record
    Inherits System.Web.UI.Page
   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(intPID)
        With cRec
            Me.ph1.Text = .p92Name
            Me.j17Name.Text = .j17Name
            Me.j27Code.Text = .j27Code

        End With
        If cRec.x15ID > 0 Then
            With Master.Factory.ftBL.GetList_X15().Where(Function(p) p.x15ID = cRec.x15ID)
                If .Count > 0 Then
                    Me.x15Name.Text = .First.x15Name
                End If
            End With
        End If
        If cRec.x38ID > 0 Then
            Me.x38Name.Text = Master.Factory.x38CodeLogicBL.Load(cRec.x38ID).x38Name
        End If
        If cRec.p93ID > 0 Then
            Dim cP93 As BO.p93InvoiceHeader = Master.Factory.p93InvoiceHeaderBL.Load(cRec.p93ID)
            With cP93
                Me.p93Company.Text = .p93Company
                Me.Address.Text = .p93Street & ", " & .p93City & " " & .p93Zip
                Me.RegIDVatID.Text = .p93RegID & " | " & .p93VatID
                Me.p93Contact.Text = BO.BAS.CrLfText2Html(.p93Contact)
                Me.p93Referent.Text = BO.BAS.CrLfText2Html(.p93Referent)
                Me.p93Signature.Text = BO.BAS.CrLfText2Html(.p93Signature)

                rpP88.DataSource = Master.Factory.p93InvoiceHeaderBL.GetList_p88(.PID)
                rpP88.DataBind()
            End With

        End If
    End Sub
End Class