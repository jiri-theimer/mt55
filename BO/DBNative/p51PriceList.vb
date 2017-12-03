
Public Enum p51TypeFlagENUM
    BillingRates = 1
    CostRates = 2
    OverheadRates = 3
    EfectiveRates = 4
End Enum
Public Class p51PriceList
    Inherits BOMother
    Public Property p51Name As String
    Public Property p51Code As String
    Public Property j27ID As Integer
    Public Property p51ID_Master As Integer
    Public Property p51DefaultRateT As Double
    Public Property p51DefaultRateU As Double
    Public Property p51TypeFlag As p51TypeFlagENUM = p51TypeFlagENUM.BillingRates
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

    Public ReadOnly Property TypeAlias As String
        Get
            Select Case Me.p51TypeFlag
                Case p51TypeFlagENUM.BillingRates : Return "Fakturační hodinové sazby"
                Case p51TypeFlagENUM.CostRates : Return "Nákladové hodinové sazby"
                Case p51TypeFlagENUM.EfectiveRates : Return "Efektivní sazby"
                Case p51TypeFlagENUM.OverheadRates : Return "Režijní hodinové sazby"
                Case Else : Return "???"
            End Select
        End Get
    End Property
End Class
