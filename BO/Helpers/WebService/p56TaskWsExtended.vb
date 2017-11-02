Public Class p56TaskWsExtended
    Inherits BO.ServiceResult

    Public Property p41ID As Integer
    Public Property p57ID As Integer
    Public Property o22ID As Integer
    Public Property b02ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property p59ID_Submitter As Integer
    Public Property p59ID_Receiver As Integer


    Public Property p56Name As String
    Public Property p56NameShort As String
    Public Property p56Code As String
    Public Property p56Description As String

    Public Property p56PlanFrom As Date?
    Public Property p56PlanUntil As Date?
    Public Property p56ReminderDate As Date?

    Public Property p56Ordinary As Integer
    Public Property p56Plan_Hours As Double
    Public Property p56Plan_Expenses As Double
    Public Property p56CompletePercent As Integer
    Public Property p56RatingValue As Integer?


    Public Property ReceiversInLine As String


    Public Property o22Name As String

    Public Property Owner As String

    Public Property o43ID As Integer

    Public Property p57Name As String



    Public Property p59NameSubmitter As String


    Public Property p57IsHelpdesk As Boolean


    Public Property b01ID As Integer

    Public Property b02Name As String


    Public Property b02Color As String


    Public Property p41Name As String

    Public Property p41Code As String


    Public Property Client As String


    Public Sub ConvertFromOrig(cOrig As BO.p56Task)
        With cOrig
            Me.PID = .PID
            Me.b01ID = .b01ID
            Me.b02Color = .b02Color
            Me.b02ID = .b02ID
            Me.b02Name = .b02Name
            Me.Client = .Client
            Me.j02ID_Owner = .j02ID_Owner
            Me.o22ID = .o22ID
            Me.o22Name = .o22Name
            Me.o43ID = .o43ID
            Me.Owner = .Owner
            Me.p41Code = .p41Code
            Me.p41ID = .p41ID
            Me.p41Name = .p41Name
            Me.p56Code = .p56Code
            Me.p56CompletePercent = .p56CompletePercent
            Me.p56Description = .p56Description
            Me.p56Name = .p56Name
            Me.p56NameShort = .p56NameShort
            Me.p56Ordinary = .p56Ordinary
            Me.p56Plan_Expenses = .p56Plan_Expenses
            Me.p56Plan_Hours = .p56Plan_Hours
            Me.p56PlanFrom = .p56PlanFrom
            Me.p56PlanUntil = .p56PlanUntil
            Me.p56RatingValue = .p56RatingValue
            Me.p56ReminderDate = p56ReminderDate
            Me.p57ID = .p57ID
            Me.p57IsHelpdesk = .p57IsHelpdesk
            Me.p57Name = .p57Name
            Me.p59ID_Receiver = .p59ID_Receiver
            Me.p59ID_Submitter = .p59ID_Submitter
            Me.p59NameSubmitter = .p59NameSubmitter
            Me.ReceiversInLine = .ReceiversInLine

        End With
    End Sub

End Class
