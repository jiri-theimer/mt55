
Public Class p50OfficePriceList
    Inherits BOMother
    Public Property p51ID As Integer
    
    Private Property _p51Name As String
    Public ReadOnly Property p51Name As String
        Get
            Return _p51Name
        End Get
    End Property
    Private Property _p51TypeFlag As BO.p51TypeFlagENUM
   

    Public ReadOnly Property Binding As String
        Get
            Select Case _p51TypeFlag
                Case p51TypeFlagENUM.CostRates : Return "Nákladové sazby"
                Case p51TypeFlagENUM.OverheadRates : Return "Režijní sazby"
                Case p51TypeFlagENUM.EfectiveRates : Return "Efektivní sazby"
                Case Else
                    Return "???"
            End Select
        End Get
    End Property
End Class
