Public Enum c21ScopeFlagENUM
    Basic = 1
    Matrix = 2
    PerTimesheet = 3
End Enum
Public Class c21FondCalendar
    Inherits BOMother
    Public Property c21ScopeFlag As c21ScopeFlagENUM = c21ScopeFlagENUM.Basic
    Public Property c21Name As String
    Public Property c21IsTimesheetHours As Boolean
    Public Property c21Ordinary As Integer
    Public Property c21Day1_Hours As Double
    Public Property c21Day2_Hours As Double
    Public Property c21Day3_Hours As Double
    Public Property c21Day4_Hours As Double
    Public Property c21Day5_Hours As Double
    Public Property c21Day6_Hours As Double
    Public Property c21Day7_Hours As Double

End Class
