Public Class p47CapacityPlan
    Inherits BOMother
    Public Property p46ID As Integer
    Public Property p47DateFrom As Date
    Public Property p47DateUntil As Date
    Public Property p47HoursBillable As Double
    Public Property p47HoursNonBillable As Double
    Public Property p47HoursTotal As Double

    Private Property _p45ID As Integer
    Public ReadOnly Property p45ID As Integer
        Get
            Return _p45ID
        End Get
    End Property
    Private Property _p41ID As Integer
    Public ReadOnly Property p41ID As Integer
        Get
            Return _p41ID
        End Get
    End Property
    Private Property _j02ID As Integer
    Public ReadOnly Property j02ID As Integer
        Get
            Return _j02ID
        End Get
    End Property
    Private Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Private Property _Project As String
    Public ReadOnly Property Project As String
        Get
            Return _Project
        End Get
    End Property
    Private Property _setAsDeleted As Boolean
    Public Sub SetAsDeleted()
        _setAsDeleted = True
    End Sub
    Public ReadOnly Property IsSetAsDeleted As Boolean
        Get
            Return _setAsDeleted
        End Get
    End Property
End Class
