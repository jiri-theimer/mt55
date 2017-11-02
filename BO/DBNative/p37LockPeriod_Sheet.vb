Public Class p37LockPeriod_Sheet
    Inherits BOMotherNN
    Public Property p36ID As Integer
    Public Property p34ID As Integer

    Public Property IsSetAsDeleted As Boolean

    Private Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property
End Class
