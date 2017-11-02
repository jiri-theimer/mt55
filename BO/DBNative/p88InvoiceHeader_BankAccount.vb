Public Class p88InvoiceHeader_BankAccount
    Public Property p93ID As Integer
    Public Property p86ID As Integer
    Public Property j27ID As Integer
    Private Property _pid As Integer
    Public ReadOnly Property PID As Integer
        Get
            Return _pid
        End Get
    End Property
    
    Private Property _Account As String
    Public ReadOnly Property Account As String
        Get
            Return _Account
        End Get
    End Property
    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
End Class
