Public Enum p46ExceedFlagENUM
    StrictFaStrictNefa = 1
    StrictTotal = 2
    StrictFaFreeNefa = 3
    StrictNeFaFreeFa = 4
    NoLimit = 5
End Enum
Public Class p46BudgetPerson
    Inherits BOMother
    Public Property j02ID As Integer
    Public Property p45ID As Integer
    Public Property p46HoursBillable As Double
    Public Property p46HoursNonBillable As Double
    Public Property p46HoursTotal As Double
    Public Property p46ExceedFlag As p46ExceedFlagENUM = p46ExceedFlagENUM.NoLimit
    Public Property p46Description As String
    Public Property p46BillingRate As Double
    Public Property j27ID_BillingRate As Integer
    Public Property p46CostRate As Double
    Public Property j27ID_CostRate As Integer

    Public Property IsSetAsDeleted As Boolean

    Friend Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property

    Public ReadOnly Property CostAmount As Double
        Get
            Return Me.p46CostRate * Me.p46HoursTotal
        End Get
    End Property
    Public ReadOnly Property BillingAmount As Double
        Get
            Return Me.p46BillingRate * Me.p46HoursBillable
        End Get
    End Property
End Class

Public Class p46BudgetPersonExtented
    Inherits p46BudgetPerson

    Public Property TimesheetFa As Double?
    Public Property TimesheetNeFa As Double?
    Public Property OperFa As Double?
    Public Property OperNeFa As Double?

    Public ReadOnly Property TimesheetAll As Double?
        Get
            If Me.TimesheetFa Is Nothing Then
                Return Me.TimesheetNeFa
            Else
                If Me.TimesheetNeFa Is Nothing Then
                    Return Me.TimesheetFa
                Else
                    Return Me.TimesheetNeFa + Me.TimesheetFa
                End If
            End If
        End Get
    End Property
    Public Property TimeshetAmountBilling As Double?
    Public Property TimesheetAmountCost As Double?
    Public ReadOnly Property TimesheetAllVersusBudget As Double
        Get
            If Me.TimesheetAll Is Nothing Then
                Return Me.p46HoursTotal * -1
            Else
                Return Me.TimesheetAll - Me.p46HoursTotal
            End If
        End Get
    End Property
End Class