Public Class p39WorkSheet_Recurrence_Plan
    Public Property p39ID As Integer
    Public Property p40ID As Integer
    Public Property p31ID_NewInstance As Integer
    Public Property p39Text As String
    Public Property p39Date As Date
    Public Property p39DateCreate As Date

    Public Property p39ErrorMessage_NewInstance As String

    Private Property _p41ID As Integer
    Public ReadOnly Property p41ID As Integer
        Get
            Return _p41ID
        End Get
    End Property
    Private Property _p41Name As String
    Public ReadOnly Property p41Name As String
        Get
            Return _p41Name
        End Get
    End Property
    Private Property _p28Name As String
    Public ReadOnly Property p28Name As String
        Get
            Return _p28Name
        End Get
    End Property
End Class
