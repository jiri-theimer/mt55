Public Class p31WorksheetSum
    Public Property RowsCount As Integer
    Public Property p31Hours_Orig As Double
    Public Property Hours_Orig_Billable As Double
    Public Property WaitingOnApproval_Hours_Count As Integer
    Public Property WaitingOnApproval_Hours_Sum As Double
    Public Property WaitingOnApproval_Other_Sum As Double
    Public Property WaitingOnApproval_Other_Count As Integer
    Public Property WaitingOnInvoice_Hours_Sum As Double
    Public Property WaitingOnInvoice_Hours_Count As Integer
    Public Property WaitingOnInvoice_Other_Sum As Double
    Public Property WaitingOnInvoice_Other_Count As Integer
    Public Property WaitingOnApproval_HoursFee As Double

    Public Property p31Hours_Approved_Billing As Double
    Public Property p31Hours_Approved_Internal As Double
    Public Property p31Hours_Invoiced As Double

    Public Property p31Value_Orig As Double
    Public Property p31Value_Trimmed As Double
    Public Property p31Value_Approved_Billing As Double
    Public Property p31Value_Approved_Internal As Double
    Public Property p31Value_Invoiced As Double
    Public Property p31Amount_WithoutVat_Orig As Double
    Public Property p31Amount_WithVat_Orig As Double
    Public Property p31Amount_Vat_Orig As Double
    Public Property p31Amount_WithoutVat_Approved As Double
    Public Property p31Amount_WithVat_Approved As Double
    Public Property p31Amount_Vat_Approved As Double

    Public Property p31Amount_WithoutVat_Invoiced As Double
    Public Property p31Amount_WithVat_Invoiced As Double
    Public Property p31Amount_Vat_Invoiced As Double

    Public Property p31Amount_WithoutVat_Invoiced_Domestic As Double
    Public Property p31Amount_WithVat_Invoiced_Domestic As Double
    Public Property p31Amount_Vat_Invoiced_Domestic As Double

    Public Property p31Amount_WithoutVat_FixedCurrency As Double

    Public Property Last_p91ID As Integer
End Class
