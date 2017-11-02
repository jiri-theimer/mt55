Public Class o22Milestone
    Inherits BOMother
    Public Property o21ID As Integer
    Public Property p41ID As Integer
    Public Property p28ID As Integer
    Public Property j02ID As Integer
    Public Property p56ID As Integer
    Public Property p91ID As Integer
    Public Property p90ID As Integer

    Public Property j02ID_Owner As Integer

    Public Property o22Name As String
    Public Property o22Code As String
    Public Property o22Location As String
    Public Property o22Description As String


    Public Property o22DateFrom As Date?
    Public Property o22DateUntil As Date?
    Public Property o22IsAllDay As Boolean
    Public Property o22ReminderDate As Date?
    Public Property o22IsNoNotify As Boolean
    Public Property o22MilestoneGUID As String

    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
    Private Property _Project As String
    Public ReadOnly Property Project As String
        Get
            Return _Project
        End Get
    End Property
    Private Property _Contact As String
    Public ReadOnly Property Contact As String
        Get
            Return _Contact
        End Get
    End Property
    Private Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Private Property _o21Name As String
    Public ReadOnly Property o21Name As String
        Get
            Return _o21Name
        End Get
    End Property

    Private Property _o21Flag As BO.o21FlagEnum
    Public ReadOnly Property o21Flag As BO.o21FlagEnum
        Get
            Return _o21Flag
        End Get
    End Property
    Private Property _x29ID As BO.x29IdEnum
    Public ReadOnly Property x29ID As BO.x29IdEnum
        Get
            Return _x29ID
        End Get
    End Property

    Public ReadOnly Property Period As String
        Get
            Select Case _o21Flag
                Case o21FlagEnum.EventFromUntil
                    If Me.o22IsAllDay Then
                        If o22DateFrom Is Nothing Then Return "??"
                        If o22DateFrom.Value.Day = o22DateUntil.Value.Day And o22DateFrom.Value.Month = o22DateUntil.Value.Month Then
                            Return "Celý den " & BO.BAS.FD(o22DateFrom.Value)
                        Else
                            Return "Celý den " & BO.BAS.FD(o22DateFrom.Value) & " - " & BO.BAS.FD(o22DateUntil.Value)
                        End If
                    Else
                        If o22DateFrom Is Nothing Then Return "??"
                        Return BO.BAS.FD_TimePeriod(o22DateFrom.Value, o22DateUntil.Value)

                    End If
                Case o21FlagEnum.DeadlineOrMilestone
                    Return BO.BAS.FD(o22DateUntil.Value, True, True)
                Case Else
                    Return ""
            End Select
        End Get
    End Property

    Public ReadOnly Property NameWithDate As String
        Get
            Return _o21Name & ": " & Me.o22Name & " (" & Me.Period & ")"
        End Get
    End Property
   
End Class
