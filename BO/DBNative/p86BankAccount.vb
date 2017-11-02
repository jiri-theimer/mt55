Public Class p86BankAccount
    Inherits BOMother
    Public Property p86Name As String
    Public Property p86BankName As String
    Public Property p86BankAccount As String
    Public Property p86BankCode As String
    Public Property p86SWIFT As String
    Public Property p86IBAN As String
    Public Property p86BankAddress As String

    Private Property _NameWithAccount As String
    Public ReadOnly Property NameWithAccount As String
        Get
            Return _NameWithAccount
        End Get
    End Property
End Class
