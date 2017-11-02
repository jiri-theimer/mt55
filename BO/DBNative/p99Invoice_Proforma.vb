Public Class p99Invoice_Proforma
    Inherits BOMother
    Public Property p91ID As Integer
    Public Property p90ID As Integer
    Public Property p82ID As Integer
    Public Property p99Amount As Double
    Public Property p99Amount_WithoutVat As Double
    Public Property p99Amount_Vat As Double

    Private Property _p90Code As String
    Public ReadOnly Property p90Code As String
        Get
            Return _p90Code
        End Get
    End Property
    Private Property _p91Code As String
    Public ReadOnly Property p91Code As String
        Get
            Return _p91Code
        End Get
    End Property
End Class
