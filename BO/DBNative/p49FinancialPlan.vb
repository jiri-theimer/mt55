Public Class p49FinancialPlan
    Inherits BOMother
    Public Property p45ID As Integer
    Public Property j02ID As Integer
    Public Property p34ID As Integer
    Public Property p32ID As Integer
    Public Property j27ID As Integer
    Public Property p28ID_Supplier As Integer

    Public Property p49DateFrom As Date
    Public Property p49DateUntil As Date
    Public Property p49Amount As Double
    Public Property p49Text As String
    
    Friend Property _Person As String
    Public ReadOnly Property Person As String
        Get
            Return _Person
        End Get
    End Property
    Friend Property _Client As String
    Public ReadOnly Property Client As String
        Get
            Return _Client
        End Get
    End Property
    Friend Property _p41ID As Integer
    Public ReadOnly Property p41ID As Integer
        Get
            Return _p41ID
        End Get
    End Property
    Friend Property _Project As String
    Public ReadOnly Property Project As String
        Get
            Return _Project
        End Get
    End Property
    Friend Property _SupplierName As String
    Public ReadOnly Property SupplierName As String
        Get
            Return _SupplierName
        End Get
    End Property
    Friend Property _p32Name As String
    Public ReadOnly Property p32Name As String
        Get
            Return _p32Name
        End Get
    End Property
    Friend Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property
    Friend Property _p34Color As String
    Public ReadOnly Property p34Color As String
        Get
            Return _p34Color
        End Get
    End Property
    Friend Property _p34IncomeStatementFlag As Integer
    Public ReadOnly Property p34IncomeStatementFlag As p34IncomeStatementFlagENUM
        Get
            Return CType(_p34IncomeStatementFlag, BO.p34IncomeStatementFlagENUM)
        End Get
    End Property
    Friend Property _p32Color As String
    Public ReadOnly Property p32Color As String
        Get
            Return _p32Color
        End Get
    End Property
    Friend Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
    Public ReadOnly Property Period As String
        Get
            If Month(Me.p49DateFrom) = Month(Me.p49DateUntil) And Year(Me.p49DateFrom) = Year(Me.p49DateUntil) Then
                Return Year(Me.p49DateFrom).ToString & "/" & Month(Me.p49DateFrom).ToString
            Else
                Return Year(Me.p49DateFrom).ToString & "/" & Month(Me.p49DateFrom).ToString & " - " & Year(Me.p49DateUntil).ToString & "/" & Month(Me.p49DateUntil).ToString
            End If

        End Get
    End Property
    Private Property _setAsDeleted As Boolean
    Public Sub SetAsDeleted()
        _setAsDeleted = True
    End Sub
    Public ReadOnly Property IsSetAsDeleted As Boolean
        Get
            Return _setAsDeleted
        End Get
    End Property

    
End Class

Public Class p49FinancialPlanExtended
    Inherits p49FinancialPlan
    Public Property p31ID As Integer
    Public Property p31Code As String
    Public Property p31Date As Date?
    Public Property p31Amount_WithoutVat_Orig As Double?
    Public Property p31Count As Integer?
End Class
