Public Class x57UserPageBinding
    Public Property x57ID As Integer
    Public Property x55ID As Integer
    Public Property x58ID As Integer
    Public Property x57DockID As String
    Public Property x57Height As String
    Public Property x57Width As String

    Private Property _x55Content As String
    Public ReadOnly Property x55Content As String
        Get
            Return _x55Content
        End Get
    End Property
    Private Property _x55Name As String
    Public ReadOnly Property x55Name As String
        Get
            Return _x55Name
        End Get
    End Property
    Private Property _x55RecordSQL As String
    Public ReadOnly Property x55RecordSQL As String
        Get
            Return _x55RecordSQL
        End Get
    End Property
    Private Property _x55TypeFlag As BO.x55TypeENUM
    Public ReadOnly Property x55TypeFlag As BO.x55TypeENUM
        Get
            Return _x55TypeFlag
        End Get
    End Property
    Private Property _x55Height As String
    Public ReadOnly Property x55Height As String
        Get
            Return _x55Height
        End Get
    End Property
    Private Property _x55Width As String
    Public ReadOnly Property x55Width As String
        Get
            Return _x55Width
        End Get
    End Property
End Class
