Public Class p51PriceList
    Inherits BOMother
    Public Property p51Name As String
    Public Property p51Code As String
    Public Property j27ID As Integer
    Public Property p51ID_Master As Integer
    Public Property p51DefaultRateT As Double
    Public Property p51DefaultRateU As Double
    Public Property p51IsInternalPriceList As Boolean
    Public Property p51IsMasterPriceList As Boolean
    Public Property p51Ordinary As Integer
    Public Property p51IsCustomTailor As Boolean

    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property

    Private Property _p51Name_Master As String
    Public ReadOnly Property p51Name_Master As String
        Get
            Return _p51Name_Master
        End Get
    End Property

    Public ReadOnly Property NameWithCurr As String
        Get
            Return Me.p51Name & " (" & _j27Code & ")"
        End Get
    End Property
End Class
