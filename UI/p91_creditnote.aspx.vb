Public Class p91_creditnote
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_creditnote_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                '.HeaderIcon = "Images/proforma_32.png"
                .HeaderText = "Vytvořit opravný doklad"
                .AddToolbarButton("Vygenerovat opravný doklad", "save", , "Images/save.png")

            End With
            Me.p92ID.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p92InvoiceType = BO.p92InvoiceTypeENUM.CreditNote)
            Me.p92ID.DataBind()


            
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            Dim intP92ID As Integer = BO.BAS.IsNullInt(Me.p92ID.SelectedValue)

            If intP92ID = 0 Then
                Master.Notify("Musíte vybrat typ opravného dokladu.", NotifyLevel.WarningMessage)
                Return
            End If

            With Master.Factory.p91InvoiceBL
                Dim intP91ID As Integer = .CreateCreditNote(Master.DataPID, intP92ID)
                If intP91ID > 0 Then
                    Master.DataPID = intP91ID
                    Master.CloseAndRefreshParent("p91-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
                
            End With
        End If
    End Sub
End Class