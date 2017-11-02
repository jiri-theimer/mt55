Public Class p36LockPeriod
    Inherits BOMother
    Public Property j02ID As Integer
    Public Property j11ID As Integer
    Public Property p36DateFrom As Date
    Public Property p36DateUntil As Date
    Public Property p36IsAllSheets As Boolean
    Public Property p36IsAllPersons As Boolean

    Private Property _j11Name As String
    Private Property _Person As String

    Public ReadOnly Property j11Name As String
        Get
            Return _j11Name
        End Get
    End Property
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
End Class
