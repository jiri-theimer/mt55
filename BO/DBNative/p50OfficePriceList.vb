Public Enum p50RatesFlagENUM
    CostRates = 1
    EffectiveRates = 2
End Enum
Public Class p50OfficePriceList
    Inherits BOMother
    Public Property p51ID As Integer
    Public Property p50RatesFlag As p50RatesFlagENUM

    Private Property _p51Name As String
    Public ReadOnly Property p51Name As String
        Get
            Return _p51Name
        End Get
    End Property

    Public ReadOnly Property Binding As String
        Get
            Select Case Me.p50RatesFlag
                Case p50RatesFlagENUM.CostRates : Return "Nákladové sazby"
                Case p50RatesFlagENUM.EffectiveRates : Return "Efektivní sazby"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class
