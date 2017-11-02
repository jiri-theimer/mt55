Public Class p91_pay
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_pay_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_pay"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderIcon = "Images/payment_32.png"

                .AddToolbarButton("Uložit úhradu", "save", , "Images/save.png")

                Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                .HeaderText = "Zapsat úhradu faktury | " & cRec.p91Code
                Me.p94Date.SelectedDate = Today
                Me.p94Amount.Value = cRec.p91Amount_Debt
                Me.j27Code.Text = cRec.j27Code
                Me.p91Amount_Debt.Text = BO.BAS.FN(cRec.p91Amount_Debt)
            End With

            rpHistory.DataSource = Master.Factory.p91InvoiceBL.GetList_p94(Master.DataPID)
            rpHistory.DataBind()
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            If Me.p94Amount.Value Is Nothing Then
                Master.Notify("Musíte zadat částku.", NotifyLevel.WarningMessage)
                Return
            End If
            If Me.p94Date.IsEmpty Then
                Master.Notify("Musíte zadat datum.", NotifyLevel.WarningMessage)
                Return
            End If
            Dim cRec As New BO.p94Invoice_Payment
            cRec.p94Amount = BO.BAS.IsNullNum(Me.p94Amount.Value)
            cRec.p94Date = Me.p94Date.SelectedDate
            cRec.p91ID = Master.DataPID
            cRec.p94Description = Me.p94Description.Text

            With Master.Factory.p91InvoiceBL
                If .SaveP94(cRec) Then
                    Master.CloseAndRefreshParent("p91-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With
        End If
    End Sub

    Private Sub rpHistory_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpHistory.ItemCommand

        Dim intP94ID As Integer = BO.BAS.IsNullInt(e.CommandArgument)


        If Master.Factory.p91InvoiceBL.DeleteP94(intP94ID, Master.DataPID) Then
            Master.CloseAndRefreshParent("p91-save")
        Else
            Master.Notify(Master.Factory.p91InvoiceBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub

    Private Sub rpHistory_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpHistory.ItemDataBound
        Dim cRec As BO.p94Invoice_Payment = CType(e.Item.DataItem, BO.p94Invoice_Payment)
        CType(e.Item.FindControl("cmdDelete"), Button).CommandArgument = cRec.PID.ToString

    End Sub
End Class