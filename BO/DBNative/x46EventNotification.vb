Public Class x46EventNotification
    Inherits BOMother
    Public Property x45ID As BO.x45IDEnum
    Public Property j02ID As Integer
    Public Property j11ID As Integer
    Public Property x67ID As Integer
    Public Property x29ID_Reference As x29IdEnum = x29IdEnum._NotSpecified
    Public Property x67ID_Reference As Integer
    Public Property x46MessageTemplate As String
    Public Property x46IsExcludeAuthor As Boolean
    Public Property x46MessageSubject As String
    Public Property x46IsForRecordOwner_Reference As Boolean
    Public Property x46IsForRecordOwner As Boolean
    Public Property x46IsForAllRoles As Boolean
    Public Property x46IsForAllReferenceRoles As Boolean
    Public Property x46IsUseSystemTemplate As Boolean
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
    Private Property _j11Name As String
    Public ReadOnly Property j11Name As String
        Get
            Return _j11Name
        End Get
    End Property
    Private Property _x67Name As String
    Public ReadOnly Property x67Name As String
        Get
            Return _x67Name
        End Get
    End Property
    Private Property _x29NameSingle As String
    Public ReadOnly Property x29NameSingle As String
        Get
            Return _x29NameSingle
        End Get
    End Property
End Class
