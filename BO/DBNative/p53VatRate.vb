Public Class p53VatRate
    Inherits BOMother
    Public Property x15ID As x15IdEnum
    Public Property p53Value As Double
    Public Property j27ID As Integer
    Public Property j17ID As Integer

    Private Property _x15Name As String
    Public ReadOnly Property x15Name As String
        Get
            Return _x15Name
        End Get
    End Property
    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
    Private Property _j17Name As String
    Public ReadOnly Property j17Name As String
        Get
            Return _j17Name
        End Get
    End Property
End Class
