Public Enum p33IdENUM
    Cas = 1
    PenizeBezDPH = 2
    Kusovnik = 3
    PenizeVcDPHRozpisu = 5
End Enum
Public Class p33ActivityInputType
    Inherits BOMotherFT
    Public Property p33ID As p33IdENUM
    Public Property p33Name As String
    Public Property p33Code As String

End Class
