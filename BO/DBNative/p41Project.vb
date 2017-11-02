Public Enum p41WorksheetOperFlagEnum
    _NotSpecified = 0                   '0 - do projektu je povoleno zapisovat i fakturovat worksheet
    NoEntryData = 1                     '1 - V projektu není povoleno zapisování úkonů
    OnlyEntryData = 2                   '2 - V projektu je povoleno pouze zapisování úkonů
    WithTaskOnly = 3                      '3 - V projektu je povoleno zapisovat úkony pouze přes úkol
    NoLimit = 9
End Enum

Public Class p41Project
    Inherits BOMother
    Public Property p42ID As Integer
    Public Property p28ID_Client As Integer
    Public Property p28ID_Billing As Integer
    Public Property b02ID As Integer
    Public Property p87ID As Integer
    Public Property p51ID_Billing As Integer
    Public Property p51ID_Internal As Integer
    Public Property p92ID As Integer
    Public Property j18ID As Integer
    Public Property p61ID As Integer

    Public Property j02ID_Owner As Integer

    Public Property p41Name As String
    Public Property p41IsDraft As Boolean
    Public Property p41NameShort As String
    Public Property p41RobotAddress As String
    Public Property p41ExternalPID As String
    Public Property p41ParentID As Integer
    Public Property p41BillingMemo As String
    Public Property p72ID_NonBillable As BO.p72IdENUM
    Public Property p72ID_BillableHours As BO.p72IdENUM

    Public Property p65ID As Integer
    Public Property p41RecurNameMask As String
    Public Property p41RecurBaseDate As Date?
    Public Property p41RecurMotherID As Integer
    Public Property p41IsStopRecurrence As Boolean

    Protected Property _p41Code As String
    Public Property p41Code As String
        Get
            Return _p41Code
        End Get
        Set(value As String)
            _p41Code = value
        End Set
    End Property
       
    Public Property p41InvoiceDefaultText1 As String
    Public Property p41InvoiceDefaultText2 As String
    Public Property p41InvoiceMaturityDays As Integer

    Public Property p41WorksheetOperFlag As p41WorksheetOperFlagEnum = p41WorksheetOperFlagEnum.NoLimit

    Public Property p41PlanFrom As Date?
    Public Property p41PlanUntil As Date?

    Public Property p41LimitHours_Notification As Double
    Public Property p41LimitFee_Notification As Double
    Public Property p41IsNoNotify As Boolean
    Public Property j02ID_ContactPerson_DefaultInWorksheet As Integer
    Public Property j02ID_ContactPerson_DefaultInInvoice As Integer

    Private Property _Owner As String
    Private Property _p42name As String
    Private Property _j18Name As String

    Public ReadOnly Property p42Name As String
        Get
            Return _p42name
        End Get
    End Property
    Public ReadOnly Property j18Name As String
        Get
            Return _j18Name
        End Get
    End Property

    Private Property _b02Name As String
    Private Property _b01ID As Integer
    Public ReadOnly Property b01ID As Integer
        Get
            Return _b01ID
        End Get
    End Property
    Public ReadOnly Property b02Name As String
        Get
            Return _b02Name
        End Get
    End Property

    Private Property _p92Name As String
    Public ReadOnly Property p92Name As String
        Get
            Return _p92Name
        End Get
    End Property

    Private Property _Client As String
    Public ReadOnly Property Client As String
        Get
            Return _Client
        End Get
    End Property
    Private Property _p41TreePath As String
    Public ReadOnly Property p41TreePath As String
        Get
            Return _p41TreePath
        End Get
    End Property
    Private Property _p41TreeLevel As Integer
    Public ReadOnly Property p41TreeLevel As Integer
        Get
            Return _p41TreeLevel
        End Get
    End Property
    Private Property _p41TreeIndex As Integer
    Public ReadOnly Property p41TreeIndex As Integer
        Get
            Return _p41TreeIndex
        End Get
    End Property
    Private Property _p41TreePrev As Integer
    Public ReadOnly Property p41TreePrev As Integer
        Get
            Return _p41TreePrev
        End Get
    End Property
    Private Property _p41TreeNext As Integer
    Public ReadOnly Property p41TreeNext As Integer
        Get
            Return _p41TreeNext
        End Get
    End Property

    ''Private Property _ClientBilling As String
    ''Public ReadOnly Property ClientBilling As String
    ''    Get
    ''        Return _ClientBilling
    ''    End Get
    ''End Property

    Private Property _p51Name_Billing As String
    Public ReadOnly Property p51Name_Billing As String
        Get
            Return _p51Name_Billing
        End Get
    End Property

    ''Private Property _p51Name_Internal As String
    ''Public ReadOnly Property p51Name_Internal As String
    ''    Get
    ''        Return _p51Name_Internal
    ''    End Get
    ''End Property

    Private Property _p87ID_Client As Integer
    Public ReadOnly Property p87ID_Client As Integer
        Get
            Return _p87ID_Client
        End Get
    End Property
    
    Public ReadOnly Property FullName As String
        Get
            If p28ID_Client > 0 Then
                If Me.p41NameShort = "" Then Return _Client & " - " & Me.p41Name Else Return _Client & " - " & Me.p41NameShort
            Else
                If _p41TreePath <> "" Then Return _p41TreePath
                If Me.p41NameShort = "" Then Return Me.p41Name Else Return Me.p41NameShort
            End If
        End Get
    End Property
    Public ReadOnly Property ProjectWithMask(intMaskIndex As Integer) As String        
        Get
            Dim s As String = Me.p41NameShort
            If s = "" Then s = Me.p41Name
            Select Case intMaskIndex
                Case 1 : Return s  'pouze název
                Case 2 : Return s & " [" & Me.p41Code & "]"  'název projektu + kód
                Case 3 : Return s & " [" & _Client & "]"    'název+klient
                Case 4 : Return Me.p41Code                'pouze kód projektu
                Case 5 : If _p41TreePath <> "" Then Return _p41TreePath Else Return Me.PrefferedName 'nadřízený+podřízený projekt

                Case Else : Return FullName & " [" & Me.p41Code & "]"
            End Select
        End Get
    End Property
    Public ReadOnly Property PrefferedName As String
        Get
            If Me.p41NameShort <> "" Then Return Me.p41NameShort
            Return Me.p41Name
        End Get
    End Property
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property



End Class
