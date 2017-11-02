'Struktura pro uložení nového úkonu nebo jeho úpravu

Public Class p31WorksheetEntryInput
    Public Property p41ID As Integer
    Public Property p56ID As Integer
    Public Property j02ID As Integer
    Public Property p32ID As Integer
    Public Property p34ID As Integer
    Public Property p48ID As Integer
    Public Property p28ID_Supplier As Integer
    Public Property j02ID_ContactPerson As Integer

    Public Property Value_Orig As String     'vstupní čas nebo počet kusovníku
    Public Property Value_Trimmed As String  'po korekci vstupní čas nebo počet kusovníku
    Public Property p31HoursEntryflag As p31HoursEntryFlagENUM
    Public Property p31Date As Date
    Public Property p31DateUntil As Date
    Public Property TimeFrom As String
    Public Property TimeUntil As String
    Public Property p31Text As String
    Public Property VatRate_Orig As Double

    Public Property Value_Orig_Entried As String
    Public Property ManualFee As Double
    Public Property p31Calc_Pieces As Double
    Public Property p31Calc_PieceAmount As Double
    Public Property p35ID As Integer
    Public Property p31Code As String

    Public Property p49ID As Integer
    Public Property j19ID As Integer
    Public Property p31ExternalPID As String

    Private Property _p31Value_Orig As Double
    Public ReadOnly Property p31Value_Orig As Double
        Get
            Return _p31Value_Orig
        End Get
    End Property
    Private Property _p31Value_Trimmed As Double
    Public ReadOnly Property p31Value_Trimmed As Double
        Get
            Return _p31Value_Trimmed
        End Get
    End Property
    Private Property _p31HHMM_Orig As String
    Public ReadOnly Property p31HHMM_Orig As String
        Get
            Return _p31HHMM_Orig
        End Get
    End Property
    Public Property p31HHMM_Trimmed As String

    Public Property p72ID_AfterTrimming As BO.p72IdENUM = p72IdENUM._NotSpecified

    Private Property _Error As String
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property

    Private Property _p31Minutes_Orig As Integer
    Public ReadOnly Property p31Minutes_Orig As Integer
        Get
            Return _p31Minutes_Orig
        End Get
    End Property
    Private Property _p31Minutes_Trimmed As Integer
    Public ReadOnly Property p31Minutes_Trimmed As Integer
        Get
            Return _p31Minutes_Trimmed
        End Get
    End Property

    Private Property _p31DateTimeFrom_Orig As Date?
    Public ReadOnly Property p31DateTimeFrom_Orig As Date?
        Get
            Return _p31DateTimeFrom_Orig
        End Get
    End Property
    Private Property _p31DateTimeUntil_Orig As Date?
    Public ReadOnly Property p31DateTimeUntil_Orig As Date?
        Get
            Return _p31DateTimeUntil_Orig
        End Get
    End Property

    Public Property Amount_WithoutVat_Orig As Double
    Public Property Amount_WithVat_Orig As Double
    Public Property Amount_Vat_Orig As Double

    Public Property j27ID_Billing_Orig As Integer

    Private Property _p31Amount_WithoutVat_Orig As Double
    Public ReadOnly Property p31Amount_WithoutVat_Orig As Double
        Get
            Return _p31Amount_WithoutVat_Orig
        End Get
    End Property
    Private Property _p31Amount_WithVat_Orig As Double
    Public ReadOnly Property p31Amount_WithVat_Orig As Double
        Get
            Return _p31Amount_WithVat_Orig
        End Get
    End Property
    Private Property _p31Amount_Vat_Orig As Double
    Public ReadOnly Property p31Amount_Vat_Orig As Double
        Get
            Return _p31Amount_Vat_Orig
        End Get
    End Property

    Private Property _pid As Integer
    Public ReadOnly Property PID As Integer
        Get
            Return _pid
        End Get
    End Property

    Public Sub SetPID(intPID As Integer)
        _pid = intPID
    End Sub

    Public Function ValidateEntryKusovnik() As Boolean
        'kusovníkový úkon
        _p31Value_Orig = BO.BAS.IsNullNum(Me.Value_Orig)
        If _p31Value_Orig = 0 Then
            _Error = "Počet nesmí být NULA."
            Return False
        End If
        Return True
    End Function
    Public Function ValidateTrimming(status As BO.p72IdENUM, strValue As String) As Boolean
        Me.p72ID_AfterTrimming = status
        Select Case Me.p72ID_AfterTrimming
            Case p72IdENUM.Fakturovat
                Dim cTime As New BO.clsTime()
                _p31Minutes_Trimmed = cTime.ConvertTimeToSeconds(strValue) / 60
                If _p31Minutes_Trimmed = 0 Then
                    _Error = "Pro korekci statusu [Fakturovat] musí být hodiny větší než nula."
                    Return False
                End If
                _p31Value_Trimmed = _p31Minutes_Trimmed / 60
                Me.p31HHMM_Trimmed = cTime.ShowAsHHMM(strValue)
                Return True
            Case Else
                _p31Minutes_Trimmed = 0
                _p31Value_Trimmed = 0
                Me.p31HHMM_Trimmed = ""
                Me.Value_Trimmed = ""
                Return True
        End Select
    End Function
    Public Function ValidateEntryTime(intRound2Minutes As Integer, Optional bolEnglish As Boolean = False) As Boolean

        'časový úkon
        Dim intSeconds_Orig As Integer = 0, cTime As New BO.clsTime()

        Select Case Me.p31HoursEntryflag
            Case BO.p31HoursEntryFlagENUM.Hodiny
                intSeconds_Orig = cTime.ConvertTimeToSeconds(Me.Value_Orig)
            Case BO.p31HoursEntryFlagENUM.Minuty
                intSeconds_Orig = BO.BAS.IsNullInt(Me.Value_Orig) * 60
            Case BO.p31HoursEntryFlagENUM.PresnyCasOdDo
                ''If Me.TimeFrom = "" Or Me.TimeUntil = "" Then
                ''    _Error = "V tomto režimu zapisování musí být zadán přesný [Čas do] a [Čas od]."
                ''    Return False
                ''End If
                intSeconds_Orig = cTime.ConvertTimeToSeconds(Me.TimeUntil) - cTime.ConvertTimeToSeconds(Me.TimeFrom)
                If intSeconds_Orig = 0 Then
                    intSeconds_Orig = cTime.ConvertTimeToSeconds(Me.Value_Orig)
                    If intSeconds_Orig = 0 Then
                        _Error = "Chybí [Hodiny]."
                        If bolEnglish Then _Error = "Field [Hours] is required."
                        Return False
                    End If
                    Me.TimeFrom = "" : Me.TimeUntil = ""
                End If
                If intSeconds_Orig < 0 Then
                    _Error = "[Čas do] je menší než [Čas od]."
                    If bolEnglish Then _Error = "[Time START] is lower then [Time END]."
                    Return False
                End If
                If Me.TimeFrom <> "" And Me.TimeUntil <> "" Then
                    _p31DateTimeFrom_Orig = Me.p31Date.AddSeconds(cTime.ConvertTimeToSeconds(Me.TimeFrom))
                    _p31DateTimeUntil_Orig = Me.p31Date.AddSeconds(cTime.ConvertTimeToSeconds(Me.TimeUntil))
                Else
                    _p31DateTimeFrom_Orig = Nothing : _p31DateTimeUntil_Orig = Nothing
                End If
        End Select
        If intSeconds_Orig = 0 And (Me.Value_Orig = "0" Or Me.Value_Orig = "" Or Me.Value_Orig = "00:00") Then
            _Error = "Čas úkonu nesmí být NULA."
            If bolEnglish Then _Error = "Field [Hours] is nullable."
            Return False
        End If
        If intSeconds_Orig = 0 And Me.Value_Orig <> "" Then
            _Error = String.Format("Výraz [{0}] není podporovaný zápis času.", Me.Value_Orig)
            If bolEnglish Then _Error = String.Format("The expression [{0}] is not supported time format.", Me.Value_Orig)
            Return False
        End If

        intSeconds_Orig = cTime.RoundSeconds(intSeconds_Orig, 60 * intRound2Minutes)  'zaokrouhlení nahoru
        _p31Minutes_Orig = intSeconds_Orig / 60
        _p31HHMM_Orig = cTime.GetTimeFromSeconds(intSeconds_Orig)
        _p31Value_Orig = CDbl(intSeconds_Orig / 60 / 60)

        Return True
    End Function

    Public Sub RecalcEntryAmount(dblWithoutVat As Double, dblValidatedVatRate As Double)
        _p31Amount_WithoutVat_Orig = dblWithoutVat : _p31Value_Orig = dblWithoutVat

        _p31Amount_Vat_Orig = _p31Amount_WithoutVat_Orig * dblValidatedVatRate / 100
        _p31Amount_WithVat_Orig = _p31Amount_WithoutVat_Orig + _p31Amount_Vat_Orig
    End Sub

    Public Sub SetAmounts()
        If Me.Amount_WithoutVat_Orig = 0 And _p31Value_Orig <> 0 Then
            Me.Amount_WithoutVat_Orig = _p31Value_Orig
        End If
        If Me.Amount_WithoutVat_Orig <> 0 And _p31Value_Orig = 0 Then
            _p31Value_Orig = Me.Amount_WithoutVat_Orig
        End If
        _p31Amount_WithoutVat_Orig = Me.Amount_WithoutVat_Orig
        _p31Amount_Vat_Orig = Me.Amount_Vat_Orig
        _p31Amount_WithVat_Orig = Me.Amount_WithVat_Orig

    End Sub

    Public ReadOnly Property p31Hours_Orig As Double
        Get
            Return CDbl(_p31Minutes_Orig) / 60
        End Get
    End Property
    Public ReadOnly Property p31Hours_Trimmed As Double
        Get
            Return CDbl(_p31Minutes_Trimmed) / 60
        End Get
    End Property
End Class
