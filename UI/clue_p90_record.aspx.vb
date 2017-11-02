Public Class clue_p90_record
    Inherits System.Web.UI.Page
  
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            If Request.Item("dr") = "1" Then
                panContainer.Style.Clear()
            End If

            RefreshRecord()

        End If
    End Sub
    Private Sub RefreshRecord()

        Dim cRec As BO.p90Proforma = Master.Factory.p90ProformaBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .p90Code
            Me.j27Code.Text = .j27Code
            Me.p90Amount.Text = BO.BAS.FN(.p90Amount)
            Me.p28Name.Text = .p28Name
            Me.p90Text1.Text = .p90Text1
            Me.p90DateBilled.Text = BO.BAS.FD(.p90DateBilled)
            Me.p90Amount_Billed.Text = BO.BAS.FN(.p90Amount_Billed)
        End With

       
        rp1.DataSource = Master.Factory.p90ProformaBL.GetList_p99(0, Master.DataPID, 0)
        rp1.DataBind()

    End Sub
End Class