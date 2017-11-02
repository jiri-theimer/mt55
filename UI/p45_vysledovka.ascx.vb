Public Class p45_vysledovka
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Public Sub RefreshData(factory As BL.Factory, intP45ID As Integer, Optional strTempGuid As String = "")
        Dim mqP49 As New BO.myQueryP49
        mqP49.p45ID = intP45ID
        Dim lisP49 As IEnumerable(Of BO.p49FinancialPlan) = factory.p49FinancialPlanBL.GetList(mqP49)

        Me.result_profit.Text = "" : Me.result_lost.Text = ""
        Dim dblExpenses As Double = lisP49.Where(Function(p) p.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj).Sum(Function(p) p.p49Amount)
        Dim dblIncome As Double = lisP49.Where(Function(p) p.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem).Sum(Function(p) p.p49Amount)

        Me.total_expense.Text = BO.BAS.FN(dblExpenses)
        Me.total_income.Text = BO.BAS.FN(dblIncome)

        Dim dblCostFee As Double = 0, dblBillingFee As Double = 0
        If strTempGuid <> "" Then
            Dim lis As IEnumerable(Of BO.p85TempBox) = factory.p85TempBoxBL.GetList(strTempGuid, False).Where(Function(p) p.p85Prefix = "p46")
            dblCostFee = lis.Sum(Function(p) (p.p85FreeFloat01 + p.p85FreeFloat02) * p.p85FreeNumber02)
            dblBillingFee = lis.Sum(Function(p) p.p85FreeFloat01 * p.p85FreeNumber01)
        Else
            Dim lis As IEnumerable(Of BO.p46BudgetPerson) = factory.p45BudgetBL.GetList_p46(intP45ID)
            dblCostFee = lis.Sum(Function(p) p.CostAmount)
            dblBillingFee = lis.Sum(Function(p) p.BillingAmount)
        End If

        Me.total_costfee.Text = BO.BAS.FN(dblCostFee)
        Me.total_billingfee.Text = BO.BAS.FN(dblBillingFee)

        Me.total_cost.Text = BO.BAS.FN(dblExpenses + dblCostFee)
        Me.total_billing.Text = BO.BAS.FN(dblIncome + dblBillingFee)

        Me.result_t.Text = BO.BAS.FN(dblBillingFee - dblCostFee)
        If dblBillingFee - dblCostFee > 0 Then
            result_t.ForeColor = Drawing.Color.Blue
        Else
            result_t.ForeColor = Drawing.Color.Red
        End If
        Me.result_m.Text = BO.BAS.FN(dblIncome - dblExpenses)
        If dblIncome - dblExpenses > 0 Then
            result_m.ForeColor = Drawing.Color.Blue
        Else
            result_m.ForeColor = Drawing.Color.Red
        End If

        Dim dblResult As Double = (dblIncome + dblBillingFee) - (dblExpenses + dblCostFee)
        Select Case dblResult
            Case Is > 0
                Me.result_profit.Text = "+" + BO.BAS.FN(dblResult)
                imgEmotion.ImageUrl = "Images/emotion_happy.png"
            Case Is < 0
                Me.result_lost.Text = BO.BAS.FN(dblResult)
                imgEmotion.ImageUrl = "Images/emotion_unhappy.png"
            Case 0
                Me.result_profit.Text = "?"
                imgEmotion.ImageUrl = "Images/emotion_amazing.png"
        End Select


    End Sub
End Class