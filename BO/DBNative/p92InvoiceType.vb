Public Enum p92InvoiceTypeENUM
    ClientInvoice = 1
    CreditNote = 2
End Enum
Public Class p92InvoiceType
    Inherits BOMother
    Public Property p92InvoiceType As p92InvoiceTypeENUM = p92InvoiceTypeENUM.ClientInvoice
    Public Property p92Name As String
    Public Property p92Code As String
    Public Property p93ID As Integer
    Public Property x31ID_Invoice As Integer
    Public Property x31ID_Attachment As Integer
    Public Property x31ID_Letter As Integer
    Public Property j27ID As Integer
    Public Property j17ID As Integer
    Public Property p98ID As Integer
    Public Property x15ID As x15IdEnum?
    Public Property j19ID As Integer
    Public Property b01ID As Integer
    Public Property x38ID As Integer
    Public Property x38ID_Draft As Integer
    Public Property p80ID As Integer
    Public Property p92Ordinary As Integer

    Public Property p92InvoiceDefaultText1 As String
    Public Property p92InvoiceDefaultText2 As String
    Public Property p92ReportConstantText As String

    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
    Private Property _j17Name As String
    Public ReadOnly Property j17Name As String
        Get
            Return _j17Name
        End Get
    End Property

    Private Property _p93Name As String
    Public ReadOnly Property p93Name As String
        Get
            Return _p93Name
        End Get
    End Property
End Class
