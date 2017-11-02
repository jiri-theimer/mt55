Public Enum p97AmountFlagEnum
    VAT = 1
    WithoutVAT = 2
    WithVAT = 3
End Enum


Public Class p97Invoice_Round_Setting
    Inherits BOMother
    Public Property j27ID As Integer
    Public Property p98ID As Integer
    Public Property p97AmountFlag As p97AmountFlagEnum
    Public Property p97Scale As Integer

    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
End Class
