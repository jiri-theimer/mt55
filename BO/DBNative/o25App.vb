Public Enum o25AppFlagENUM
    GoogleCalendar = 1
    OneDrive = 2
End Enum

Public Class o25App
    Inherits BOMother
    Public Property o25Name As String
    Public Property o25Code As String
    Public Property o25AppFlag As o25AppFlagENUM
    Public Property o25Url As String
    Public Property o25IsMainMenu As Boolean

End Class
