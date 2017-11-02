Public Class p31WorksheetInvoiceChange
    Public Property p31ID As Integer
    Public Property p33ID As BO.p33IdENUM
    Public Property p70ID As BO.p70IdENUM
    Public Property InvoiceValue As Double
    Public Property InvoiceRate As Double
    Public Property InvoiceVatRate As Double
    Public Property TextUpdate As String
    Public Property FixPriceValue As Double
    Public Property p31IsInvoiceManual As Boolean
    Public Property ManualFee As Double
    Public Property p32ManualFeeFlag As Integer = 0
End Class
