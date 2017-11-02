Public Class x69EntityRole_Assign
    Public Property x69ID As Integer
    Public Property x67ID As Integer
    Public Property j02ID As Integer
    Public Property j11ID As Integer
    Public Property j07ID As Integer
    Public Property x69RecordPID As Integer
    Public Property x69IsWelcomeNotification As Boolean

    Public Property IsSetAsDeleted As Boolean

    Private Property _Person As String
    Private Property _IsAllPersons As Boolean
    Private Property _j11Name As String
    Private Property _x67Name As String

    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Public ReadOnly Property j11Name As String
        Get
            Return _j11Name
        End Get
    End Property
    Public ReadOnly Property x67Name As String
        Get
            Return _x67Name
        End Get
    End Property
    Public ReadOnly Property IsAllPersons As Boolean
        Get
            Return _IsAllPersons
        End Get
    End Property
End Class
