Public Class p48OperativePlan
    Inherits BOMother
    Public Property j02ID As Integer
    Public Property p41ID As Integer
    Public Property p34ID As Integer
    Public Property p32ID As Integer
    Public Property p31ID As Integer

    Public Property p48Date As Date
    Public Property p48Hours As Double
    Public Property p48Text
    Public Property p48TimeFrom As String
    Public Property p48TimeUntil As String

    Public Property p48DateTimeFrom As Date?
    
    Public Property p48DateTimeUntil As Date?
      
    Private Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Private Property _Client As String
    Public ReadOnly Property Client As String
        Get
            Return _Client
        End Get
    End Property
    Private Property _Project As String
    Public ReadOnly Property Project As String
        Get
            Return _Project
        End Get
    End Property
    Public ReadOnly Property ClientAndProject As String
        Get
            If _Client = "" Then Return _Project Else Return _Client & " - " & _Project
        End Get
    End Property
    Private Property _p32Name As String
    Public ReadOnly Property p32Name As String
        Get
            Return _p32Name
        End Get
    End Property
    Private Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property
    Private Property _p34Color As String
    Public ReadOnly Property p34Color As String
        Get
            Return _p34Color
        End Get
    End Property
    Private Property _p32Color As String
    Public ReadOnly Property p32Color As String
        Get
            Return _p32Color
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
