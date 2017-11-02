Public Class p56Task
    Inherits BOMother
    Public Property p41ID As Integer
    Public Property p57ID As Integer
    Public Property o22ID As Integer
    Public Property b02ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property p59ID_Submitter As Integer
    Public Property p59ID_Receiver As Integer
    Public Property o43ID As Integer

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
    Public Property p56IsPlan_Hours_Ceiling As Boolean  'zastropovat plán hodin (nelze překročit)
    Public Property p56IsPlan_Expenses_Ceiling As Boolean   'zastropovat plán výdajů (nelze překročit)

    Public Property p56IsHtml As Boolean    'zadání/popis úkolu je v HTML
    Public Property p56IsNoNotify As Boolean


    Public Property p56CompletePercent As Integer
    Public Property p56RatingValue As Integer?
    Public Property p56ExternalPID As String

    Public Property p65ID As Integer
    Public Property p56RecurNameMask As String
    Public Property p56RecurBaseDate As Date?
    Public Property p56RecurMotherID As Integer
    Public Property p56IsStopRecurrence As Boolean


    Public Property TagsInlineHtml As String
    Friend Property _ReceiversInLine As String
    Public ReadOnly Property ReceiversInLine As String
        Get
            Return _ReceiversInLine
        End Get
    End Property
    Friend Property _o22Name As String
    Public ReadOnly Property o22Name As String
        Get
            Return _o22Name
        End Get
    End Property
    Friend Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
    
    Friend Property _p57Name As String
    Public ReadOnly Property p57Name As String
        Get
            Return _p57Name
        End Get
    End Property
    
    Friend Property _p59NameSubmitter As String
    Public ReadOnly Property p59NameSubmitter As String
        Get
            Return _p59NameSubmitter
        End Get
    End Property
    Friend Property _p57IsHelpdesk As Boolean
    Public ReadOnly Property p57IsHelpdesk As Boolean
        Get
            Return _p57IsHelpdesk
        End Get
    End Property
    Friend Property _p57PlanDatesEntryFlag As Integer
    Public ReadOnly Property p57PlanDatesEntryFlag As Integer
        Get
            Return _p57PlanDatesEntryFlag
        End Get
    End Property
    Friend Property _b01ID As Integer
    Public ReadOnly Property b01ID As Integer
        Get
            Return _b01ID
        End Get
    End Property

    Friend Property _b02Name As String
    Public ReadOnly Property b02Name As String
        Get
            Return _b02Name
        End Get
    End Property
    Friend Property _b02Color As String
    Public ReadOnly Property b02Color As String
        Get
            Return _b02Color
        End Get
    End Property

    Friend Property _p41Name As String
    Public ReadOnly Property p41Name As String
        Get
            Return _p41Name
        End Get
    End Property
    

    Friend Property _p41Code As String

    Public ReadOnly Property p41Code As String
        Get
            Return _p41Code
        End Get
    End Property
    Public ReadOnly Property ProjectCodeAndName As String
        Get
            Return _p41Code & " - " & _p41Name

        End Get
    End Property

    Friend Property _Client As String
    Public ReadOnly Property Client As String
        Get
            Return _Client
        End Get
    End Property

    Public ReadOnly Property FullName As String
        Get
            If _Client <> "" Then
                Return Me.p56Name & " [" & _Client & " - " & _p41Name & "]"
            Else
                Return Me.p56Name & " [" & _p41Name & "]"
            End If
        End Get
    End Property
    Public ReadOnly Property NameWithTypeAndCode As String
        Get
            Return _p57Name & ": " & Me.p56Name & " (" & Me.p56Code & ")"
        End Get
    End Property
    Public ReadOnly Property RecurNameMaskWIthTypeAndCode As String
        Get
            Return _p57Name & ": " & Me.p56RecurNameMask & " (" & Me.p56Code & ")"
        End Get
    End Property
End Class
