Public Enum p31HoursEntryFlagENUM
    Hodiny = 1
    Minuty = 2
    PresnyCasOdDo = 3
    NeniCas = 0
End Enum


Public Class p31Worksheet
    Inherits BOMother
    Public Property p41ID As Integer
    Public Property p41Name As String
    Public Property p41NameShort As String
    Public Property p28ID_Client As Integer
    Public Property ClientName As String
    Public Property p28CompanyShortName As String

    Public Property j02ID As Integer
    Public Property Person As String

    Public Property p32ID As Integer
    Public Property p28ID_Supplier As Integer
    Public Property SupplierName As String
    Public Property j02ID_ContactPerson As Integer
    Public Property ContactPerson As String
    Public Property p32Name As String
    Public Property p32IsBillable As Boolean
    Public Property p32ManualFeeFlag As Integer
    Public Property p34ID As Integer
    Public Property p33ID As BO.p33IdENUM
    Public Property p34Name As String
    Public Property p34IncomeStatementFlag As p34IncomeStatementFlagENUM
    Public Property p95ID As Integer
    Public Property p95Name As String
    Public Property p56ID As Integer
    Public Property p56Name As String
    Public Property p56Code As String
    Public Property p49ID As Integer

    Public Property p91ID As Integer
    Public Property p91Code As String

    Public Property j02ID_Owner As Integer
    Public Property Owner As String

    Public Property p70ID As p70IdENUM
    Public Property p70Name As String

    Public Property p71ID As p71IdENUM
    Public Property p71Name As String

    Public Property p72ID_AfterApprove As p72IdENUM
    Public Property approve_p72Name As String
    Public Property p72ID_AfterTrimming As p72IdENUM
    Public Property trim_p72Name As String

    Public Property p31Date As Date
    Public Property p31DateUntil As Date
    Public Property p31DateTimeFrom_Orig As Date?
    Public Property p31DateTimeUntil_Orig As Date?
    Public Property p31Text As String
    Public Property p31Code As String

    Public Property p31Value_Orig As Double
    Public Property p31Value_Trimmed As Double
    Public Property p31Value_Approved_Billing As Double
    Public Property p31Value_Approved_Internal As Double
    Public Property p31Value_Invoiced As Double

    Public Property p31Minutes_Orig As Integer
    Public Property p31Minutes_Trimmed As Integer
    Public Property p31Minutes_Approved_Billing As Integer
    Public Property p31Minutes_Approved_Internal As Integer
    Public Property p31Minutes_Invoiced As Integer

    Public Property p31Hours_Orig As Double
    Public Property p31Hours_Trimmed As Double
    Public Property p31Hours_Approved_Billing As Double
    Public Property p31Hours_Approved_Internal As Double
    Public Property p31Hours_Invoiced As Double

    Public Property p31HHMM_Orig As String
    Public Property p31HHMM_Trimmed As String
    Public Property p31HHMM_Approved_Billing As String
    Public Property p31HHMM_Approved_Internal As String
    Public Property p31HHMM_Invoiced As String


    Public Property p31Rate_Billing_Orig As Double
    Public Property p31Rate_Internal_Orig As Double
    Public Property p31Amount_Internal As Double
    Public Property p31Rate_Billing_Approved As Double
    Public Property p31Rate_Internal_Approved As Double
    Public Property p31Rate_Billing_Invoiced As Double

    Public Property p31Amount_WithoutVat_Orig As Double
    Public Property p31Amount_WithVat_Orig As Double
    Public Property p31Amount_Vat_Orig As Double
    Public Property p31VatRate_Orig As Double
    Public Property j27ID_Billing_Orig As Integer
    Public Property j27ID_Internal As Integer

    Public Property p31Amount_WithoutVat_Approved As Double
    Public Property p31Amount_WithVat_Approved As Double
    Public Property p31Amount_Vat_Approved As Double
    Public Property p31VatRate_Approved As Double
    
    Public Property p31Amount_WithoutVat_Invoiced As Double
    Public Property p31Amount_WithVat_Invoiced As Double
    Public Property p31VatRate_Invoiced As Double
    Public Property p31Amount_Vat_Invoiced As Double
    Public Property p31ExchangeRate_Invoice As Double
    Public Property j27ID_Billing_Invoiced As Integer

    Public Property p31Amount_WithoutVat_Invoiced_Domestic As Double
    Public Property p31Amount_WithVat_Invoiced_Domestic As Double
    Public Property p31Amount_Vat_Invoiced_Domestic As Double
    Public Property p31ExchangeRate_Domestic As Double
    Public Property j27ID_Billing_Invoiced_Domestic As Integer

    Public Property p31Amount_WithoutVat_FixedCurrency As Double


    Public Property p31ExternalPID As String

    Public Property c11ID As Integer
    Public Property j02ID_ApprovedBy As Integer

    
    Public Property p31Value_Orig_Entried As String

    Public Property p31HoursEntryFlag As p31HoursEntryFlagENUM
    Public Property p31Approved_When As Date?
        
    Public Property p31IsPlanRecord As Boolean

    Public Property j27Code_Billing_Orig As String

    Public Property p31Calc_Pieces As Double
    Public Property p31Calc_PieceAmount As Double
    Public Property p35ID As Integer
    Public Property j19ID As Integer
    Public Property p31ApprovingSet As String
    Public Property p31ApprovingLevel As Integer
    Public Property o23ID_First As Integer
    Public Property p31Value_FixPrice As Double
    Public Property p31IsInvoiceManual As Boolean

    Public ReadOnly Property TimeFrom As String
        Get
            If Me.p31DateTimeFrom_Orig Is Nothing Then
                Return "00:00"
            Else
                Return Format(Me.p31DateTimeFrom_Orig, "HH:mm")
            End If
        End Get
    End Property
    Public ReadOnly Property TimeUntil As String
        Get
            If Me.p31DateTimeUntil_Orig Is Nothing Then
                Return "00:00"
            Else
                Return Format(Me.p31DateTimeUntil_Orig, "HH:mm")
            End If
        End Get
    End Property





    Public Function IsRecommendedHHMM() As Boolean
        If Me.p33ID = p33IdENUM.Cas Then
            If Me.p31Value_Orig_Entried <> "" Then
                If Me.p31Value_Orig_Entried.IndexOf(":") > 0 Then Return True 'původně zadaná hodnota obsahuje rovnou hodnota v HH:MM
            End If
            If Len(Me.p31Value_Orig.ToString) > 5 Then Return True 'desetinné číslo s velkým počtem desetinných míst
        End If

        Return False
    End Function
    Public Function IsRecommendedHHMM_Invoiced() As Boolean
        If Me.p33ID = p33IdENUM.Cas Then
            If Len(Me.p31Hours_Invoiced.ToString) > 5 Then Return True 'desetinné číslo s velkým počtem desetinných míst
        End If

        Return False
    End Function
End Class
