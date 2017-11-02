Public Enum x48TaskOutputFlagENUM
    RunSql = 1
    PIDsTable = 2

End Enum

Public Class x48SqlTask
    Inherits BOMother
    Public Property x29ID As BO.x29IdEnum
    Public Property x48Name As String
    Public Property x48TaskOutputFlag As x48TaskOutputFlagENUM
    Public Property x48Ordinary As Integer
    Public Property x48Sql As String
    Public Property x31ID As Integer

    Public Property x48MailSubject As String
    Public Property x48MailBody As String
    Public Property x48MailTo As String


    Public Property x48IsRunInDay1 As Boolean
    Public Property x48IsRunInDay2 As Boolean
    Public Property x48IsRunInDay3 As Boolean
    Public Property x48IsRunInDay4 As Boolean
    Public Property x48IsRunInDay5 As Boolean
    Public Property x48IsRunInDay6 As Boolean
    Public Property x48IsRunInDay7 As Boolean
    Public Property x48RunInTime As String
    Public Property x48IsRepeat As Boolean
    Public Property x48LastScheduledRun As DateTime?
End Class