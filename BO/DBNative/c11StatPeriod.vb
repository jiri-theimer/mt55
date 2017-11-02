Public Enum PeriodLevel
    Year = 1
    Quarter = 2
    Month = 3
    Week = 4
    Day = 5
    None = 0
End Enum
Public Class c11StatPeriod
    Public Property c11ID As Integer
    Public Property c11ParentID As Integer
    Public Property c11Name As String
    Public Property c11Ordinary As Integer
    Public Property c11DateFrom As Date
    Public Property c11DateUntil As Date
    Public Property c11Level As PeriodLevel
    Public Property c11Y As Integer
    Public Property c11Q As Integer
    Public Property c11M As Integer
    Public Property c11W As Integer
    Public Property c11D As Integer

End Class
