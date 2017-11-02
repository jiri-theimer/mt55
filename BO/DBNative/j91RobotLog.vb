Public Enum j91RobotTaskFlag
    Start = 0
    p40 = 1
    MailQueue = 2
    ImapImport = 3
    ScheduledReports = 4
    SqlTasks = 5
    CnbKurzy = 6
    CentralPing = 7
    DbBackup = 8
    RecurrenceP41 = 9
    RecurrenceP56 = 10
    ClearTemp = 11
    AutoWorkflowSteps = 12
End Enum
Public Class j91RobotLog
    Public Property j91ID As Integer
    Public Property j91Date As Date
    Public Property j91BatchGuid As String
    Public Property j91TaskFlag As j91RobotTaskFlag
    Public Property j91InfoMessage As String
    Public Property j91ErrorMessage As String
    Public Property j91Account As String

End Class
