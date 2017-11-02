Public Class p31WorksheetApproveInput
    Public Property GUID_TempData As String
    Public Property p31ID As Integer
    Public Property p33ID As BO.p33IdENUM
    Public Property p71id As BO.p71IdENUM
    Public Property p72id As BO.p72IdENUM
    Public Property p31ApprovingSet As String
    Public Property Value_Approved_Billing As Double
    Public Property Value_Approved_Internal As Double
    Public Property Rate_Billing_Approved As Double
    Public Property Rate_Internal_Approved As Double
    Public Property p31Text As String
    Public Property p31Date As Date?
    Public Property p31ApprovingLevel As Integer
    Public Property p31Value_FixPrice As Double
    Public Property VatRate_Approved As Double
    Public Property p32ID As Integer
    Public Property p32ManualFeeFlag As Integer
    Public Property ManualFee_Approved As Double

    Public Sub New(intP31ID As Integer, _p33id As BO.p33IdENUM)
        Me.p31ID = intP31ID
        Me.p33ID = _p33id
    End Sub
End Class
