Public Class j02Person
    Inherits BOMother
    Public Property j07ID As Integer
    Public Property j17ID As Integer
    Public Property j18ID As Integer
    Public Property c21ID As Integer
    Public Property o40ID As Integer
    Public Property j02FirstName As String
    Public Property j02LastName As String
    Public Property j02TitleBeforeName As String
    Public Property j02TitleAfterName As String
    Public Property j02Code As String
    Public Property j02Email As String
    Public Property j02EmailSignature As String
    Public Property j02Phone As String
    Public Property j02Mobile As String
    Public Property j02JobTitle As String
    Public Property j02Office As String
    Public Property j02AvatarImage As String
    Public Property j02Description As String
    Public Property j02IsIntraPerson As Boolean
    Public Property j02RobotAddress As String
    Public Property j02ExternalPID As String

    Public Property j02TimesheetEntryDaysBackLimit As Integer
    Public Property j02TimesheetEntryDaysBackLimit_p34IDs As String
    Public Property j02Salutation As String
    Public Property j02WorksheetAccessFlag As Integer   '1= nemá přístup k již vyfakturovaným úkonům
    Public Property p72ID_NonBillable As BO.p72IdENUM
    Public Property j02DomainAccount As String
    Public Property j02IsInvoiceEmail As Boolean

    Public ReadOnly Property FullNameAsc As String
        Get
            Return Trim(j02TitleBeforeName & " " & j02FirstName & " " & j02LastName & " " & j02TitleAfterName)
        End Get
    End Property
    Public ReadOnly Property FullNameDesc As String
        Get
            Return Trim(j02LastName & " " & j02FirstName & " " & j02TitleBeforeName)
        End Get
    End Property
    Public ReadOnly Property FullNameDescWithEmail As String
        Get
            Return Me.FullNameDesc & " [" & Me.j02Email & "]"
        End Get
    End Property
    Public ReadOnly Property FullNameDescWithJobTitle As String
        Get
            If Me.j02JobTitle <> "" Then
                Return Me.FullNameDesc & " [" & Me.j02JobTitle & "]"
            Else
                Return Me.FullNameDesc
            End If

        End Get
    End Property

    Private Property _j07Name As String
    Public ReadOnly Property j07Name As String
        Get
            Return _j07Name
        End Get
    End Property
    Private Property _c21Name As String
    Public ReadOnly Property c21Name As String
        Get
            Return _c21Name
        End Get
    End Property
    Private Property _j18Name As String
    Public ReadOnly Property j18Name As String
        Get
            Return _j18Name
        End Get
    End Property

    '----uživatelská pole--------------------
    Public Property j02FreeText01 As String
    Public Property j02FreeText02 As String
    Public Property j02FreeText03 As String
    Public Property j02FreeText04 As String
    Public Property j02FreeText05 As String
    Public Property j02FreeText06 As String
    Public Property j02FreeText07 As String
    Public Property j02FreeText08 As String
    Public Property j02FreeText09 As String
    Public Property j02FreeText10 As String

    Public Property j02FreeBoolean01 As Boolean
    Public Property j02FreeBoolean02 As Boolean
    Public Property j02FreeBoolean03 As Boolean
    Public Property j02FreeBoolean04 As Boolean
    Public Property j02FreeBoolean05 As Boolean
    Public Property j02FreeBoolean06 As Boolean
    Public Property j02FreeBoolean07 As Boolean
    Public Property j02FreeBoolean08 As Boolean
    Public Property j02FreeBoolean09 As Boolean
    Public Property j02FreeBoolean10 As Boolean

    Public Property j02FreeDate01 As DateTime?
    Public Property j02FreeDate02 As DateTime?
    Public Property j02FreeDate03 As DateTime?
    Public Property j02FreeDate04 As DateTime?
    Public Property j02FreeDate05 As DateTime?
    Public Property j02FreeDate06 As DateTime?
    Public Property j02FreeDate07 As DateTime?
    Public Property j02FreeDate08 As DateTime?
    Public Property j02FreeDate09 As DateTime?
    Public Property j02FreeDate10 As DateTime?

    Public Property j02FreeNumber01 As Double
    Public Property j02FreeNumber02 As Double
    Public Property j02FreeNumber03 As Double
    Public Property j02FreeNumber04 As Double
    Public Property j02FreeNumber05 As Double
    Public Property j02FreeNumber06 As Double
    Public Property j02FreeNumber07 As Double
    Public Property j02FreeNumber08 As Double
    Public Property j02FreeNumber09 As Double
    Public Property j02FreeNumber10 As Double

    Public Property j02FreeCombo01 As Integer?
    Public Property j02FreeCombo02 As Integer?
    Public Property j02FreeCombo03 As Integer?
    Public Property j02FreeCombo04 As Integer?
    Public Property j02FreeCombo05 As Integer?
    Public Property j02FreeCombo06 As Integer?
    Public Property j02FreeCombo07 As Integer?
    Public Property j02FreeCombo08 As Integer?
    Public Property j02FreeCombo09 As Integer?
    Public Property j02FreeCombo10 As Integer?

    Public Property j02FreeCombo01Text As String = Nothing
    Public Property j02FreeCombo02Text As String = Nothing
    Public Property j02FreeCombo03Text As String = Nothing
    Public Property j02FreeCombo04Text As String = Nothing
    Public Property j02FreeCombo05Text As String = Nothing
    Public Property j02FreeCombo06Text As String = Nothing
    Public Property j02FreeCombo07Text As String = Nothing
    Public Property j02FreeCombo08Text As String = Nothing
    Public Property j02FreeCombo09Text As String = Nothing
    Public Property j02FreeCombo10Text As String = Nothing
End Class
