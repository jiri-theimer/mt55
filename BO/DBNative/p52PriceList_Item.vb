Public Class p52PriceList_Item
    Inherits BOMother
    Public Property p51ID As Integer
    Public Property j02ID As Integer
    Public Property j07ID As Integer
    Public Property p34ID As Integer
    Public Property p32ID As Integer
    Public Property p52Name As String
    Public Property p52Rate As Double
    Public Property p52IsPlusAllTimeSheets As Boolean
    Private Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property

    Private Property _j07Name As String
    Public ReadOnly Property j07Name As String
        Get
            Return _j07Name
        End Get
    End Property

    Private Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property

    Private Property _p32Name As String
    Public ReadOnly Property p32Name As String
        Get
            Return _p32Name
        End Get
    End Property
End Class
