Public Class clue_p28_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(intPID)
        If cRec Is Nothing Then Master.StopPage("Record not found.")
        With cRec
            If .p28CompanyShortName <> "" Then
                Me.ph1.Text = .p28CompanyShortName & "<br>" & .p28CompanyName
            Else
                Me.ph1.Text = .p28Name
            End If
            ph1.Text += " <span style='color:gray;padding-left:10px;'>" & .p28Code & "</span>"
            Me.Owner.Text = .Owner
            If .p29ID <> 0 Then
                Me.p29Name.Text = .p29Name
            Else
                trP29.Visible = False
            End If

            Me.p28REGID.Text = .p28RegID
            Me.p28VATID.Text = .p28VatID
            Me.p51Name_Billing.Text = .p51Name_Billing
            Me.p87Name.Text = .p87Name
            Me.p92Name.Text = .p92Name
            If .p28InvoiceMaturityDays > 0 Then
                Me.p28InvoiceMaturityDays.Text = .p28InvoiceMaturityDays.ToString
            End If
            If .p51ID_Billing = 0 And .p87ID = 0 And .p92ID = 0 Then
                panBilling.Visible = False
            End If
            If .p28ParentID > 0 Then
                ParentContact.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .p28ParentID)
                ParentContact.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .p28ParentID)
                ParentContact.NavigateUrl = "p28_framework.aspx?pid=" & .p28ParentID.ToString
            Else
                ParentContact.Visible = False
            End If
        End With
        address1.FillData(Master.Factory.p28ContactBL.GetList_o37(intPID))

        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(intPID)
        If lisO32.Count > 0 Then
            medium1.FillData(lisO32)
        Else
            panMedium.Visible = False
        End If

        Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Master.Factory.p30Contact_PersonBL.GetList(intPID, 0, 0)
        If lisP30.Count > 0 Then
            Me.persons1.FillData(lisP30, False)
        Else
            panP30.Visible = False
        End If
        tags1.RefreshData(intPID)


    End Sub
End Class