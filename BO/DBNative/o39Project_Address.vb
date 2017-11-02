Public Class o39Project_Address
    Inherits o38Address

    Public Property p41ID As Integer
    Public Property o38ID As Integer
    Public Property o36ID As o36IdEnum

    Public Property IsSetAsDeleted As Boolean

    Private Property _o36Name As String
    Public ReadOnly Property o36Name As String
        Get
            Return _o36Name
        End Get
    End Property


End Class
