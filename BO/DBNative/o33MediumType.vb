Public Enum o33FlagEnum
    _NotSpecified = 0
    Tel = 1
    Email = 2
    URL = 3
    SKYPE = 4
    ICQ = 5
    FAX = 6
    Other = 7
End Enum
Public Class o33MediumType
    Inherits BOMother
    Public Property o33Name As String
    Public Property o33Flag As o33FlagEnum

End Class
