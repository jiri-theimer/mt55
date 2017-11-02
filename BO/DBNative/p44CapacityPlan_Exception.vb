Public Enum p44ExceptionFlagENUM
    OperativePlan = 1
End Enum

Public Class p44CapacityPlan_Exception
    Inherits BOMother
    Public Property p46ID As Integer
    Public Property p44DateFrom As Date
    Public Property p44DateUntil As Date
    Public Property p44ExceptionFlag As p44ExceptionFlagENUM = p44ExceptionFlagENUM.OperativePlan
    
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
    
    
End Class
