Public Enum p28SupplierFlagENUM
    _NotSpecified = 0
    ClientOnly = 1
    SupplierOnly = 2
    ClientAndSupplier = 3
    NotClientNotSupplier = 4
End Enum

Public Class p28Contact
    Inherits BOMother
    Public Property p29ID As Integer    
    Public Property p92ID As Integer
    Public Property p87ID As Integer
    Public Property p51ID_Billing As Integer
    Public Property j02ID_Owner As Integer
    Public Property p51ID_Internal As Integer
    Public Property b02ID As Integer
    Public Property p63ID As Integer
    Public Property j61ID_Invoice As Integer
    Public Property p28IsDraft As Boolean
    Public Property p28IsCompany As Boolean
    Public Property p28Code As String
    Public Property p28FirstName As String
    Public Property p28LastName As String
    Public Property p28TitleBeforeName As String
    Public Property p28TitleAfterName As String
    Public Property p28RegID As String
    Public Property p28VatID As String
    Public Property p28Person_BirthRegID As String
    Public Property p28CompanyName As String
    Public Property p28CompanyShortName As String
    Public Property p28ParentID As Integer
    Public Property p28InvoiceDefaultText1 As String
    Public Property p28InvoiceDefaultText2 As String
    Public Property p28InvoiceMaturityDays As Integer
    Public Property p28AvatarImage As String

    Public p28LimitHours_Notification As Double
    Public p28LimitFee_Notification As Double
    Public Property p28RobotAddress As String
    Public Property p28SupplierFlag As p28SupplierFlagENUM
    Public Property p28SupplierID As String
    Public Property p28ExternalPID As String
    Public Property p28BillingMemo As String
    Public Property p28Pohoda_VatCode As String
    Public Property j02ID_ContactPerson_DefaultInWorksheet As Integer
    Public Property j02ID_ContactPerson_DefaultInInvoice As Integer
    Private Property _p28name As String
    Private Property _Owner As String


    Public ReadOnly Property p28Name As String
        Get
            Return _p28name
        End Get
    End Property

    Private Property _p29Name As String
    Public ReadOnly Property p29Name As String
        Get
            Return _p29Name
        End Get
    End Property

    Private Property _p92Name As String
    Public ReadOnly Property p92Name As String
        Get
            Return _p92Name
        End Get
    End Property

    Private Property _b02Name As String
    Public ReadOnly Property b02Name As String
        Get
            Return _b02Name
        End Get
    End Property

    Private Property _p51Name_Billing As String
    Public ReadOnly Property p51Name_Billing As String
        Get
            Return _p51Name_Billing
        End Get
    End Property

    Private Property _p51Name_Internal As String
    Public ReadOnly Property p51Name_Internal As String
        Get
            Return _p51Name_Internal
        End Get
    End Property

    Private Property _p87Name As String
    Public ReadOnly Property p87Name As String
        Get
            Return _p87Name
        End Get
    End Property
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
    Private Property _p28TreePath As String
    Public ReadOnly Property p28TreePath As String
        Get
            Return _p28TreePath
        End Get
    End Property
    Private Property _p28TreeLevel As Integer
    Public ReadOnly Property p28TreeLevel As Integer
        Get
            Return _p28TreeLevel
        End Get
    End Property
    Private Property _p28TreeIndex As Integer
    Public ReadOnly Property p28TreeIndex As Integer
        Get
            Return _p28TreeIndex
        End Get
    End Property
    Private Property _p28TreePrev As Integer
    Public ReadOnly Property p28TreePrev As Integer
        Get
            Return _p28TreePrev
        End Get
    End Property
    Private Property _p28TreeNext As Integer
    Public ReadOnly Property p28TreeNext As Integer
        Get
            Return _p28TreeNext
        End Get
    End Property


    ' ''----uživatelská pole--------------------
    ''Public Property p28FreeText01 As String
    ''Public Property p28FreeText02 As String
    ''Public Property p28FreeText03 As String
    ''Public Property p28FreeText04 As String
    ''Public Property p28FreeText05 As String
    ''Public Property p28FreeText06 As String
    ''Public Property p28FreeText07 As String
    ''Public Property p28FreeText08 As String
    ''Public Property p28FreeText09 As String
    ''Public Property p28FreeText10 As String

    ''Public Property p28FreeBoolean01 As Boolean
    ''Public Property p28FreeBoolean02 As Boolean
    ''Public Property p28FreeBoolean03 As Boolean
    ''Public Property p28FreeBoolean04 As Boolean
    ''Public Property p28FreeBoolean05 As Boolean
    ''Public Property p28FreeBoolean06 As Boolean
    ''Public Property p28FreeBoolean07 As Boolean
    ''Public Property p28FreeBoolean08 As Boolean
    ''Public Property p28FreeBoolean09 As Boolean
    ''Public Property p28FreeBoolean10 As Boolean

    ''Public Property p28FreeDate01 As DateTime?
    ''Public Property p28FreeDate02 As DateTime?
    ''Public Property p28FreeDate03 As DateTime?
    ''Public Property p28FreeDate04 As DateTime?
    ''Public Property p28FreeDate05 As DateTime?
    ''Public Property p28FreeDate06 As DateTime?
    ''Public Property p28FreeDate07 As DateTime?
    ''Public Property p28FreeDate08 As DateTime?
    ''Public Property p28FreeDate09 As DateTime?
    ''Public Property p28FreeDate10 As DateTime?

    ''Public Property p28FreeNumber01 As Double
    ''Public Property p28FreeNumber02 As Double
    ''Public Property p28FreeNumber03 As Double
    ''Public Property p28FreeNumber04 As Double
    ''Public Property p28FreeNumber05 As Double
    ''Public Property p28FreeNumber06 As Double
    ''Public Property p28FreeNumber07 As Double
    ''Public Property p28FreeNumber08 As Double
    ''Public Property p28FreeNumber09 As Double
    ''Public Property p28FreeNumber10 As Double

    ''Public Property p28FreeCombo01 As Integer?
    ''Public Property p28FreeCombo02 As Integer?
    ''Public Property p28FreeCombo03 As Integer?
    ''Public Property p28FreeCombo04 As Integer?
    ''Public Property p28FreeCombo05 As Integer?
    ''Public Property p28FreeCombo06 As Integer?
    ''Public Property p28FreeCombo07 As Integer?
    ''Public Property p28FreeCombo08 As Integer?
    ''Public Property p28FreeCombo09 As Integer?
    ''Public Property p28FreeCombo10 As Integer?
End Class
