Public Enum x24IdENUM
    tInteger = 1
    tString = 2
    tDecimal = 3
    tDate = 4
    tDateTime = 5
    tTime = 6
    tBoolean = 7
    tNone = 0
End Enum
Public Class x24DataType
    Inherits BOMotherFT
    Public Property x24ID As x24IdENUM
    Public Property x24Name As String
    Public Property x24Description As String


End Class
