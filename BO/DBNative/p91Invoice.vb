Public Class p91Invoice
    Inherits BOMother
    Public Property j27ID As Integer
    Public Property p92ID As Integer
    Public Property p28ID As Integer
    Public Property p41ID_First As Integer
    Public Property j19ID As Integer
    Public Property j17ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property j02ID_ContactPerson As Integer
    Public Property p91ID_CreditNoteBind As Integer
    Public Property o38ID_Primary As Integer
    Public Property o38ID_Delivery As Integer
    Public Property b02ID As Integer
    Public Property p98ID As Integer
    Public Property p63ID As Integer
    Public Property p80ID As Integer

    Public Property p91Code As String
    Public Property p91IsDraft As Boolean
    Public Property p91Date As Date
    Public Property p91DateBilled As Date
    Public Property p91DateMaturity As Date
    Public Property p91DateSupply As Date
    Public Property p91DateExchange As Date

    Public Property p91ExchangeRate As Double
    Public Property p91Datep31_From As Date?
    Public Property p91Datep31_Until As Date?



    Public Property p91Amount_WithoutVat As Double
   
    Public Property p91Amount_Vat As Double
    Public Property p91Amount_Billed As Double
    Public Property p91Amount_WithVat As Double
    Public Property p91Amount_Debt As Double
    Public Property p91ProformaAmount As Double
    Public Property p91ProformaBilledAmount As Double
    Public Property p91Amount_WithoutVat_None As Double

    Public Property p91VatRate_Low As Double
    Public Property p91Amount_WithVat_Low As Double
    Public Property p91Amount_WithoutVat_Low As Double
    Public Property p91Amount_Vat_Low As Double

    Public Property p91VatRate_Standard As Double
    Public Property p91Amount_WithVat_Standard As Double
    Public Property p91Amount_WithoutVat_Standard As Double
    Public Property p91Amount_Vat_Standard As Double

    Public Property p91VatRate_Special As Double
    Public Property p91Amount_WithVat_Special As Double
    Public Property p91Amount_WithoutVat_Special As Double
    Public Property p91Amount_Vat_Special As Double

    Public Property p91Amount_TotalDue As Double
    Public Property p91RoundFitAmount As Double
    Public Property p91FixedVatRate As Double
    Public Property x15ID As Integer

    Public Property p91Text1 As String
    Public Property p91Text2 As String

    Public Property p91Client As String
    Public Property p91ClientPerson As String
    Public Property p91ClientPerson_Salutation As String
    Public Property p91Client_RegID As String
    Public Property p91Client_VatID As String
    Public Property p91ClientAddress1_Street As String
    Public Property p91ClientAddress1_City As String
    Public Property p91ClientAddress1_ZIP As String
    Public Property p91ClientAddress1_Country As String
    Public Property p91ClientAddress2 As String

    Private Property _b01ID As Integer
    Public ReadOnly Property b01ID As Integer
        Get
            Return _b01ID
        End Get
    End Property

    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property
    Private Property _b02Name As String
    Public ReadOnly Property b02Name As String
        Get
            Return _b02Name
        End Get
    End Property
    Private Property _p93ID As Integer
    Public ReadOnly Property p93ID As Integer
        Get
            Return _p93ID
        End Get
    End Property

    Private Property _p28Name As String
    Public ReadOnly Property p28Name As String
        Get
            Return _p28Name
        End Get
    End Property
    Private Property _p28CompanyName As String
    Public ReadOnly Property p28CompanyName As String
        Get
            Return _p28CompanyName
        End Get
    End Property
    Public ReadOnly Property CodeAndAmount As String
        Get
            Return Me.p91Code & " [" & BO.BAS.FD(Me.p91DateSupply) & "] " & BO.BAS.FN(Me.p91Amount_WithoutVat) & " " & _j27Code
        End Get
    End Property

    Private Property _p92Name As String
    Public ReadOnly Property p92Name As String
        Get
            Return _p92Name
        End Get
    End Property
    Private Property _p92InvoiceType As BO.p92InvoiceTypeENUM
    Public ReadOnly Property p92InvoiceType As BO.p92InvoiceTypeENUM
        Get
            Return _p92InvoiceType
        End Get
    End Property
    Private Property _j17Name As String
    Public ReadOnly Property j17Name As String
        Get
            Return _j17Name
        End Get
    End Property
    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property

  

    Private Property _p41Name As String
    Public ReadOnly Property p41Name As String
        Get
            Return _p41Name
        End Get
    End Property
   

    'Public Property WithoutVat_CZK As Double
    'Public Property WithoutVat_EUR As Double
    'Public ReadOnly Property Debt_CZK As Double
    '    Get
    '        If j27ID = 2 Then Return p91Amount_Debt Else Return 0
    '    End Get
    'End Property
    'Public ReadOnly Property Debt_EUR As Double
    '    Get
    '        If j27ID = 3 Then Return p91Amount_Debt Else Return 0
    '    End Get
    'End Property
    Public ReadOnly Property WithoutVat_Krat_Kurz As Double
        Get
            Return Me.p91Amount_WithoutVat * Me.p91ExchangeRate
        End Get
    End Property
    Public ReadOnly Property Debt_Krat_Kurz As Double
        Get
            Return Me.p91Amount_Debt * Me.p91ExchangeRate
        End Get
    End Property
    Public ReadOnly Property p91Amount_TotalDue_Krat_Kurz As Double
        Get
            Return Me.p91Amount_TotalDue * Me.p91ExchangeRate
        End Get
    End Property

    Public ReadOnly Property PrimaryAddress As String
        Get
            Dim s As String = Me.p91ClientAddress1_Street
            If s = "" Then
                s = Me.p91ClientAddress1_City
            Else
                s += ", " & Me.p91ClientAddress1_City
            End If
            If Me.p91ClientAddress1_ZIP <> "" Then s += ", " & Me.p91ClientAddress1_ZIP
            If Me.p91ClientAddress1_Country <> "" Then s += ", " & Me.p91ClientAddress1_Country
            Return s
        End Get
    End Property

    ' ''----uživatelská pole--------------------
    ''Public Property p91FreeText01 As String
    ''Public Property p91FreeText02 As String
    ''Public Property p91FreeText03 As String
    ''Public Property p91FreeText04 As String
    ''Public Property p91FreeText05 As String
    ''Public Property p91FreeText06 As String
    ''Public Property p91FreeText07 As String
    ''Public Property p91FreeText08 As String
    ''Public Property p91FreeText09 As String
    ''Public Property p91FreeText10 As String

    ''Public Property p91FreeBoolean01 As Boolean
    ''Public Property p91FreeBoolean02 As Boolean
    ''Public Property p91FreeBoolean03 As Boolean
    ''Public Property p91FreeBoolean04 As Boolean
    ''Public Property p91FreeBoolean05 As Boolean
    ''Public Property p91FreeBoolean06 As Boolean
    ''Public Property p91FreeBoolean07 As Boolean
    ''Public Property p91FreeBoolean08 As Boolean
    ''Public Property p91FreeBoolean09 As Boolean
    ''Public Property p91FreeBoolean10 As Boolean

    ''Public Property p91FreeDate01 As DateTime?
    ''Public Property p91FreeDate02 As DateTime?
    ''Public Property p91FreeDate03 As DateTime?
    ''Public Property p91FreeDate04 As DateTime?
    ''Public Property p91FreeDate05 As DateTime?
    ''Public Property p91FreeDate06 As DateTime?
    ''Public Property p91FreeDate07 As DateTime?
    ''Public Property p91FreeDate08 As DateTime?
    ''Public Property p91FreeDate09 As DateTime?
    ''Public Property p91FreeDate10 As DateTime?

    ''Public Property p91FreeNumber01 As Double
    ''Public Property p91FreeNumber02 As Double
    ''Public Property p91FreeNumber03 As Double
    ''Public Property p91FreeNumber04 As Double
    ''Public Property p91FreeNumber05 As Double
    ''Public Property p91FreeNumber06 As Double
    ''Public Property p91FreeNumber07 As Double
    ''Public Property p91FreeNumber08 As Double
    ''Public Property p91FreeNumber09 As Double
    ''Public Property p91FreeNumber10 As Double

    ''Public Property p91FreeCombo01 As Integer?
    ''Public Property p91FreeCombo02 As Integer?
    ''Public Property p91FreeCombo03 As Integer?
    ''Public Property p91FreeCombo04 As Integer?
    ''Public Property p91FreeCombo05 As Integer?
    ''Public Property p91FreeCombo06 As Integer?
    ''Public Property p91FreeCombo07 As Integer?
    ''Public Property p91FreeCombo08 As Integer?
    ''Public Property p91FreeCombo09 As Integer?
    ''Public Property p91FreeCombo10 As Integer?
End Class
