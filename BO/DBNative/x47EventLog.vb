Public Class x47EventLog

    Public Property x29ID As BO.x29IdEnum = x29IdEnum._NotSpecified
    Public Property x29ID_Reference As BO.x29IdEnum = x29IdEnum._NotSpecified
    Public Property x47RecordPID_Reference As Integer
    Public Property x45ID As BO.x45IDEnum

    Public Property j03ID As Integer
    Public Property x47RecordPID As Integer
    Public Property x47Name As String
    Public Property x47NameReference As String
    Public Property x47Description As String
    Public Property TagsInlineHtml As String
    Private Property x47DateInsert As Date
    Public ReadOnly Property DateInsert As Date
        Get
            Return x47DateInsert
        End Get
    End Property
    Private Property x47UserInsert As String
    Public ReadOnly Property UserInsert As Date
        Get
            Return x47UserInsert
        End Get
    End Property

    Private Property x47ID As Integer
    Public ReadOnly Property PID As Integer
        Get
            Return Me.x47ID
        End Get
    End Property
    Public ReadOnly Property IsClosed As Boolean
        Get
            Return False
        End Get
    End Property
    Private Property _x45Name As String
    Public ReadOnly Property x45Name As String
        Get
            Return _x45Name
        End Get
    End Property
    Private Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Private Property _x45IsAllowNotification As Boolean
    Public ReadOnly Property x45IsAllowNotification As Boolean
        Get
            Return _x45IsAllowNotification
        End Get
    End Property
End Class
