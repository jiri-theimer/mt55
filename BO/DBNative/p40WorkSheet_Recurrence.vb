Public Enum RecurrenceType
    Day = 1
    Week = 2
    Month = 3
    Quarter = 4
    Year = 5
End Enum
Public Class p40WorkSheet_Recurrence
    Inherits BOMother
    Public Property p41ID As Integer
    Public Property j02ID As Integer
    Public Property p34ID As Integer
    Public Property p32ID As Integer

    Public Property j27ID As Integer
    Public Property x15ID As x15IdEnum
    Public Property p56ID As Integer
    Public Property p40RecurrenceType As RecurrenceType
    Public Property p40Name As String
    Public Property p40FirstSupplyDate As Date?
    Public Property p40LastSupplyDate As Date?
    Public Property p40GenerateDayAfterSupply As Integer
    Public Property p40Value As Double
    Public Property p40Text As String

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
    Private Property _Person As String

    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Private Property _Task As String
    Public ReadOnly Property Task As String
        Get
            Return _Task
        End Get
    End Property


End Class
