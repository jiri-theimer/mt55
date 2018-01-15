
Public Enum p12FlagENUM
    Prichod = 1
    Odchod = 2
    Aktivita = 3
End Enum
Public Class p12Pass
    Inherits BOMother
    Public Property p11ID As Integer
    Public Property p32ID As Integer
    Public Property p12TimeStamp As Date
    Public Property p12Description As String
    Public Property p32Name As String
    Public Property p12Flag As p12FlagENUM
    Public Property p12Duration As Integer
    Public Property p12ActivityDuration As Integer
End Class
