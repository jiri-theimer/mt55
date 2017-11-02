Public Enum o36IdEnum
    InvoiceAddress = 1
    PostalAddress = 2
    Other = 3
End Enum
Public Class o36AddressType
    Inherits BOMotherFT
    Public Property o36ID As o36IdEnum
    Public Property o36Name As String

End Class
