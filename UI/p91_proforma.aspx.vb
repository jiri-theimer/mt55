Public Class p91_proforma
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_proforma_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderIcon = "Images/proforma_32.png"
                .HeaderText = "Spárovat fakturu s uhrazenou zálohou"
                .AddToolbarButton("Uložit vazbu na zálohu", "save", , "Images/save.png")

            End With
            Handle_p90_Combo()

            
            rpP99.DataSource = Master.Factory.p90ProformaBL.GetList_p99(Master.DataPID, 0, 0)
            rpP99.DataBind()

        End If
    End Sub

    Private Sub Handle_p90_Combo()
        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)

        Dim mq As New BO.myQueryP90
        mq.j27ID = cRec.j27ID
        
        If Me.chkClientOnly.Checked Then
            mq.p28ID = cRec.p28ID
        End If

        Me.p90ID.DataSource = Master.Factory.p90ProformaBL.GetList(mq).Where(Function(p) p.p90Amount_Debt < 20)
        Me.p90ID.DataBind()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            Dim intP90ID As Integer = BO.BAS.IsNullInt(Me.p90ID.SelectedValue)

            If intP90ID = 0 Then
                Master.Notify("Musíte vybrat zálohovou fakturu.", NotifyLevel.WarningMessage)
                Return
            End If
            If BO.BAS.IsNullInt(Me.p82ID.SelectedValue) = 0 Then
                Master.Notify("Chybí úhrada zálohové faktury.", NotifyLevel.ErrorMessage)
                Return
            End If


            With Master.Factory.p91InvoiceBL
                If .SaveP99(Master.DataPID, intP90ID, BO.BAS.IsNullInt(Me.p82ID.SelectedValue)) Then
                    Master.CloseAndRefreshParent("p91-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With
        End If
    End Sub

    Private Sub p90ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p90ID.SelectedIndexChanged
        Dim intPID As Integer = BO.BAS.IsNullInt(Me.p90ID.SelectedValue)
        If intPID > 0 Then

            Me.clue_p90.Attributes("rel") = "clue_p90_record.aspx?pid=" & intPID.ToString
            Me.p82ID.DataSource = Master.Factory.p90ProformaBL.GetList_p82(intPID)
            Me.p82ID.DataBind()

        End If

    End Sub

    

    Private Sub rpP99_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP99.ItemCommand
        Dim intP90ID As Integer = CInt(e.CommandArgument)
        If Master.Factory.p91InvoiceBL.DeleteP99(Master.DataPID, intP90ID) Then
            Master.CloseAndRefreshParent("p91-save")
        End If
    End Sub

    Private Sub rpP99_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP99.ItemDataBound
        Dim cRec As BO.p99Invoice_Proforma = CType(e.Item.DataItem, BO.p99Invoice_Proforma)
        CType(e.Item.FindControl("cmdDelete"), Button).CommandArgument = cRec.p90ID.ToString
    End Sub

    
    Private Sub p91_proforma_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If BO.BAS.IsNullInt(Me.p90ID.SelectedValue) = 0 Then
            Me.p82ID.Visible = False
            Me.clue_p90.Visible = False
        Else
            Me.clue_p90.Visible = True
            Me.p82ID.Visible = True
        End If
    End Sub

    

    Private Sub chkClientOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkClientOnly.CheckedChanged
        Handle_p90_Combo()
    End Sub
End Class