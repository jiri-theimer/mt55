Public Enum m62RateTypeENUM
    InvoiceRate = 1
    FixedRate = 2
End Enum
Public Class m62ExchangeRate
    Inherits BOMother
    Public Property m62RateType As m62RateTypeENUM = m62RateTypeENUM.InvoiceRate
    Public Property j27ID_Master As Integer
    Public Property j27ID_Slave As Integer
    Public Property m62Date As Date
    Public Property m62Rate As Double
    Public Property m62Units As Integer

    Public ReadOnly Property RateType As String
        Get
            If m62RateType = m62RateTypeENUM.InvoiceRate Then
                Return "Fakturační kurz"
            Else
                Return "Fixní kurz"
            End If
        End Get
    End Property
    Private Property _masterj27Code As String
    Public ReadOnly Property j27Code_Master As String
        Get
            Return _masterj27Code
        End Get
    End Property
    Private Property _slavej27Code As String
    Public ReadOnly Property j27Code_Slave As String
        Get
            Return _slavej27Code
        End Get
    End Property
End Class
