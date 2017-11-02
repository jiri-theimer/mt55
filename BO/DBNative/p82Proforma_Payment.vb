Public Class p82Proforma_Payment
    Inherits BOMother
    Public Property p90ID As Integer
    Public Property p82Date As Date
    Public Property p82Amount As Double
    Public Property p82Amount_WithoutVat As Double
    Public Property p82Amount_Vat As Double
    Public Property p82Code As String
    Public Property IsSetAsDeleted As Boolean
    Public Property p82Text As String

    Public ReadOnly Property DateWithAmount As String
        Get
            Return BO.BAS.FD(p82Date) & " - " & BO.BAS.FN(Me.p82Amount)
        End Get
    End Property
End Class
