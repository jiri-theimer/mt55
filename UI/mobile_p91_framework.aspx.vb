Public Class mobile_p91_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_p91_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .MenuPrefix = "p91"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p91_framework_detail-pid")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                End With
                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p91_framework_detail-pid", "O"))
                    If .DataPID = 0 Then Response.Redirect("mobile_entity_framework_missing.aspx?prefix=p91")
                Else
                    If .DataPID <> BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p91_framework_detail-pid", "O")) Then
                        .Factory.j03UserBL.SetUserParam("p91_framework_detail-pid", .DataPID.ToString)
                    End If
                End If

            End With

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
        Handle_Permissions(cRec)
        With cRec
            Me.p91Code.Text = .p91Code
            Me.RecordHeader.Text = BO.BAS.OM3(.p92Name, 30) & ": " & .p91Code
            Me.RecordHeader.NavigateUrl = "mobile_p91_framework.aspx?pid=" & .PID.ToString

            Me.p92Name.Text = .p92Name
            Me.imgDraft.Visible = .p91IsDraft

            If .b02ID > 0 Then
                Me.b02Name.Text = .b02Name
                trB02.Visible = True
            End If
            Me.Client.NavigateUrl = "mobile_p28_framework.aspx?pid=" & .p28ID.ToString
            Me.Client.Text = .p28Name

            Me.p91Amount_Debt.Text = BO.BAS.FN(.p91Amount_Debt) & " " & .j27Code
            Me.p91Amount_WithoutVat.Text = BO.BAS.FN(.p91Amount_WithoutVat) & " " & .j27Code
            Me.p91Amount_Vat.Text = BO.BAS.FN(.p91Amount_Vat) & " " & .j27Code

            Me.p91Text1.Text = .p91Text1
            Me.p91Date.Text = BO.BAS.FD(.p91Date)
            Me.p91DateMaturity.Text = BO.BAS.FD(.p91DateMaturity)
            Me.p91DateSupply.Text = BO.BAS.FD(.p91DateSupply)
        End With

        
        
        Dim mqP41 As New BO.myQueryP41
        mqP41.p91ID = cRec.PID
        mqP41.Closed = BO.BooleanQueryMode.NoQuery
        mqP41.MG_SelectPidFieldOnly = True
        Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mqP41)
        Me.CountP41.Text = lisP41.Count.ToString

        Dim mqP31 As New BO.myQueryP31
        mqP31.p91ID = cRec.PID
        mqP31.Closed = BO.BooleanQueryMode.NoQuery
        mqP31.MG_SelectPidFieldOnly = True
        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mqP31)
        Me.CountP31.Text = lisP31.Count.ToString


        HandleDirectReports(cRec.p92ID)

    End Sub

   
    Private Sub HandleDirectReports(intP92ID As Integer)
        Dim cRec As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(intP92ID)
        With cRec
            If .x31ID_Invoice > 0 Then
                Me.cmdReportInvoice.NavigateUrl = "mobile_report.aspx?prefix=p91&pid=" & Master.DataPID.ToString & "&x31id=" & .x31ID_Invoice.ToString
            Else
                Me.cmdReportInvoice.Visible = False
            End If
            If .x31ID_Attachment > 0 Then
                Me.cmdReportAttachment.NavigateUrl = "mobile_report.aspx?prefix=p91&pid=" & Master.DataPID.ToString & "&x31id=" & .x31ID_Attachment.ToString
            Else
                Me.cmdReportAttachment.Visible = False
            End If
        End With
    End Sub

    Private Sub Handle_Permissions(cRec As BO.p91Invoice)
        Dim cDisp As BO.p91RecordDisposition = Master.Factory.p91InvoiceBL.InhaleRecordDisposition(cRec)
        If Not cDisp.ReadAccess Then
            Master.StopPage("Nedisponujete přístupovým oprávněním k faktuře.")
        End If
    End Sub

End Class