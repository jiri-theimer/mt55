Public Class p91_proforma
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Class RealAmount
        Public Property p82ID As Integer
        Public Property Percentage As Double
        Public Property Total As Double
        Public Property WithoutVat As Double
        Public Property VatAmount As Double

    End Class

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
                .AddToolbarButton("Uložit vazbu na úhradu zálohy", "save", , "Images/save.png")

            End With
            For i As Integer = 100 To 1 Step -1
                perc.Items.Add(New ListItem(i.ToString, i.ToString))
            Next
            Handle_p90_Combo()

            
            rpP99.DataSource = Master.Factory.p90ProformaBL.GetList_p99(Master.DataPID, 0, 0)
            rpP99.DataBind()
            If rpP99.Items.Count > 0 Then panP99.Visible = True
        End If
    End Sub

    Private Sub Handle_p90_Combo()
        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)

        Dim mq As New BO.myQueryP90
        mq.j27ID = cRec.j27ID
        
        If Me.chkClientOnly.Checked Then
            mq.p28ID = cRec.p28ID
        End If

        Dim lis As IEnumerable(Of BO.p90Proforma) = Master.Factory.p90ProformaBL.GetList(mq).Where(Function(p) p.p90Amount_Debt < 20)
        Dim qry = From p In lis Select p.PID, p.CodeWithClient Distinct
        Me.p90ID.DataSource = qry
        Me.p90ID.DataBind()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            Dim intP90ID As Integer = BO.BAS.IsNullInt(Me.p90ID.SelectedValue)
            Dim intP82ID As Integer = BO.BAS.IsNullInt(Me.p82ID.SelectedValue)

            If intP90ID = 0 Then
                Master.Notify("Musíte vybrat zálohovou fakturu.", NotifyLevel.WarningMessage)
                Return
            End If
            If BO.BAS.IsNullInt(Me.p82ID.SelectedValue) = 0 Then
                Master.Notify("Chybí úhrada zálohové faktury.", NotifyLevel.ErrorMessage)
                Return
            End If
            Dim dblPerc As Double = CDbl(perc.SelectedValue), cP82 As BO.p82Proforma_Payment = Master.Factory.p90ProformaBL.LoadP82(intP82ID)
            If Me.p99Amount_WithoutVat.Value > 0 Then
                dblPerc = 100 * CDbl(Me.p99Amount_WithoutVat.Value) / cP82.p82Amount_WithoutVat
            End If

            With Master.Factory.p91InvoiceBL
                If .SaveP99(Master.DataPID, intP90ID, BO.BAS.IsNullInt(Me.p82ID.SelectedValue), dblPerc) Then
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
            Handle_ChangePerc()
        End If

    End Sub

    

    Private Sub rpP99_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP99.ItemCommand
        Dim intP99ID As Integer = CInt(e.CommandArgument)
        If Master.Factory.p91InvoiceBL.DeleteP99(intP99ID) Then
            Master.CloseAndRefreshParent("p91-save")
        End If
    End Sub

    Private Sub rpP99_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP99.ItemDataBound
        Dim cRec As BO.p99Invoice_Proforma = CType(e.Item.DataItem, BO.p99Invoice_Proforma)
        CType(e.Item.FindControl("cmdDelete"), Button).CommandArgument = cRec.PID.ToString
    End Sub

    
    Private Sub p91_proforma_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.clue_p90.Visible = False
        Me.trPerc.Visible = False
        trUhrada.Visible = False
        If BO.BAS.IsNullInt(Me.p90ID.SelectedValue) > 0 Then

            Me.clue_p90.Visible = True
            Me.trUhrada.Visible = True


        End If
        If Me.p82ID.SelectedValue <> "" Then
            trPerc.Visible = True
        End If
    End Sub

    

    Private Sub chkClientOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkClientOnly.CheckedChanged
        Handle_p90_Combo()
    End Sub

    Private Function InhaleAmount() As RealAmount
        Dim cRec As BO.p82Proforma_Payment = Master.Factory.p90ProformaBL.LoadP82(BO.BAS.IsNullInt(Me.p82ID.SelectedValue))
        Dim c As New RealAmount, perc As Double = BO.BAS.IsNullNum(Me.perc.SelectedValue)
        c.p82ID = cRec.PID
        c.Total = Math.Round(cRec.p82Amount * perc / 100, 2)
        c.WithoutVat = Math.Round(cRec.p82Amount_WithoutVat * perc / 100, 2)
        c.VatAmount = c.Total - c.WithoutVat
        Return c
    End Function

   
    Private Sub perc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles perc.SelectedIndexChanged
        Handle_ChangePerc()
    End Sub

    Private Sub Handle_ChangePerc()
        If Me.p82ID.Items.Count = 0 Then Return
        Dim c As RealAmount = InhaleAmount()
        Me.AfterPerc.Text = String.Format("Celkem: {0},-, z toho bez DPH: {1},- + DPH: {2},-", BO.BAS.FN(c.Total), BO.BAS.FN(c.WithoutVat), BO.BAS.FN(c.VatAmount))
        Me.p99Amount_WithoutVat.Value = c.WithoutVat

    End Sub

End Class