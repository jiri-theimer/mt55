Public Class p65Recurrence
    Inherits BOMother
    Public Property p65RecurFlag As BO.RecurrenceType?
    Public Property p65Name As String
    Public Property p65RecurGenToBase_D As Integer
    Public Property p65RecurGenToBase_M As Integer
    Public Property p65IsPlanFrom As Boolean
    Public Property p65RecurPlanFromToBase_D As Integer
    Public Property p65RecurPlanFromToBase_M As Integer
    Public Property p65IsPlanUntil As Boolean
    Public Property p65RecurPlanUntilToBase_D As Integer
    Public Property p65RecurPlanUntilToBase_M As Integer

    Public ReadOnly Property NameWithFlag As String
        Get
            Select Case Me.p65RecurFlag
                Case RecurrenceType.Month : Return Me.p65Name & " (MM/YYYY)"
                Case RecurrenceType.Quarter : Return Me.p65Name & " (Q/YYYY)"
                Case RecurrenceType.Year : Return Me.p65Name & " (YYYY)"
                Case Else
                    Return Me.p65Name
            End Select
        End Get
    End Property
End Class

