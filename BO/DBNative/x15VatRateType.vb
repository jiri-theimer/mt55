Public Enum x15IdEnum
    Nic = 0
    BezDPH = 1
    SnizenaSazba = 2
    ZakladniSazba = 3
    SpecialniSazba = 4
End Enum
Public Class x15VatRateType
    Inherits BOMotherFT
    Public Property x15Name As String
    Public Property x15Ordinary As Integer

    Private Property _x15ID As x15IdEnum
    Public ReadOnly Property x15ID As Integer
        Get
            Return _x15ID
        End Get
    End Property
End Class
