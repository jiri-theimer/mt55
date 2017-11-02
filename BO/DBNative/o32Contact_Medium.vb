Public Class o32Contact_Medium
    Inherits BOMother
    Public Property p28ID As Integer
    Public Property o33ID As o33FlagEnum = o33FlagEnum._NotSpecified
    Public Property o32Value As String
    Public Property o32Description As String
    Public Property o32IsDefaultInInvoice As Boolean


    Public Property IsSetAsDeleted As Boolean

    Private Property _o33Name As String
    Public ReadOnly Property o33Name As String
        Get
            Return _o33Name
        End Get
    End Property
End Class
