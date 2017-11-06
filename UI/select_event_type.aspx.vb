Public Class select_event_type
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub select_event_type_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidOcas.Value = basUI.GetCompleteQuerystring(Request)
            With Master
                .AddToolbarButton("Pokračovat", "continue", 0, "Images/continue.png")

                Me.o21ID.DataSource = .Factory.o21MilestoneTypeBL.GetList(New BO.myQuery).OrderByDescending(Function(p) p.x29ID).ThenBy(Function(p) p.o21Ordinary)
                Me.o21ID.DataBind()
            End With

        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "continue" Then
            If Me.o21ID.SelectedItem Is Nothing Then
                Master.Notify("Musíte vybrat typ události!", NotifyLevel.WarningMessage)
                Return
            End If
            Dim cRec As BO.o21MilestoneType = Master.Factory.o21MilestoneTypeBL.Load(BO.BAS.IsNullInt(Me.o21ID.SelectedValue))

            Dim intMasterPID As Integer = BO.BAS.IsNullInt(Me.cbx1.SelectedValue)
            If intMasterPID = 0 Then
                Master.Notify(String.Format("Musíte vybrat záznam [{0}]!", BO.BAS.GetX29EntityAlias(cRec.x29ID, False)), NotifyLevel.WarningMessage)
                Return
            End If
            
            Dim strMasterPrefix As String = BO.BAS.GetDataPrefix(cRec.x29ID)
            Dim strURL As String = "o22_record.aspx?pid=0&o21id=" & cRec.PID.ToString & "&masterprefix=" & strMasterPrefix & "&masterpid=" & intMasterPID.ToString
            If hidOcas.Value <> "" Then strURL += "&" & hidOcas.Value
            Server.Transfer(strURL, False)
        End If
    End Sub

    Private Sub o21ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles o21ID.SelectedIndexChanged
        cbx1.Visible = True

        Dim cRec As BO.o21MilestoneType = Master.Factory.o21MilestoneTypeBL.Load(BO.BAS.IsNullInt(Me.o21ID.SelectedValue))

        Select Case cRec.x29ID
            Case BO.x29IdEnum.p41Project
                cbx1.WebServiceSettings.Path = "~/Services/project_service.asmx"
                cbx1.Text = "Najít projekt..."
            Case BO.x29IdEnum.p28Contact
                cbx1.WebServiceSettings.Path = "~/Services/contact_service.asmx"
                cbx1.Text = "Najít klienta..."
            Case BO.x29IdEnum.p91Invoice
                cbx1.WebServiceSettings.Path = "~/Services/invoice_service.asmx"
                cbx1.Text = "Najít fakturu..."
            Case BO.x29IdEnum.p56Task
                cbx1.WebServiceSettings.Path = "~/Services/task_service.asmx"
                cbx1.Text = "Najít úkol..."
            Case BO.x29IdEnum.j02Person
                cbx1.WebServiceSettings.Path = "~/Services/person_service.asmx"
                cbx1.Text = "Najít osobu..."
            Case BO.x29IdEnum.o23Doc
                cbx1.WebServiceSettings.Path = "~/Services/doc_service.asmx"
                cbx1.Text = "Najít dokument..."
        End Select
    End Sub
End Class