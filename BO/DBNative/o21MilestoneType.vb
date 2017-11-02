Public Enum o21FlagEnum
    DeadlineOrMilestone = 1
    EventFromUntil = 2
    MemoOnly = 3
End Enum
Public Class o21MilestoneType
    Inherits BOMother
    Public Property x29ID As BO.x29IdEnum
    Public Property o21Name As String
    Public Property o21Ordinary As Integer
    Public Property o21Flag As o21FlagEnum = o21FlagEnum.DeadlineOrMilestone

    Public ReadOnly Property FlagIcon As String
        Get
            Select Case Me.o21Flag
                Case o21FlagEnum.DeadlineOrMilestone
                    Return "Images/milestone.png"
                Case o21FlagEnum.EventFromUntil
                    Return "Images/event.png"
                Case o21FlagEnum.MemoOnly
                    Return "Images/notepad.png"
                Case Else
                    Return ""
            End Select
        End Get
    End Property
End Class
