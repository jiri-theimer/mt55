Public Enum o23LockedTypeENUM
    _NotSpecified = 0
    LockAllFiles = 1

End Enum
Public Class o23Doc
    Inherits BOMother
    Public Property x23ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property b02ID As Integer
    Public Property o43ID As Integer
    Public Property o23Name As String
    Public Property o23Ordinary As Integer
    Public Property o23ArabicCode As String
    Public Property o23Code As String
    Public Property o23BackColor As String
    Public Property o23ForeColor As String

    Public Property o23IsEncrypted As Boolean
    Public Property o23Password As String
    Public Property o23ExternalPID As String
    Public Property o23GUID As String

    Public Property o23IsDraft As Boolean
    Public Property o23LockedFlag As o23LockedTypeENUM = o23LockedTypeENUM._NotSpecified

    Friend Property _x18ID As Integer
    Public ReadOnly Property x18ID As Integer
        Get
            Return _x18ID
        End Get
    End Property
    Friend Property _DocType As String
    Public ReadOnly Property DocType As String
        Get
            Return _DocType
        End Get
    End Property


    
    Friend Property _o23LastLockedWhen As Date?
    Public ReadOnly Property o23LastLockedWhen As Date?
        Get
            Return _o23LastLockedWhen
        End Get
    End Property
    Friend Property _o23LastLockedBy As String
    Public ReadOnly Property o23LastLockedBy As String
        Get
            Return _o23LastLockedBy
        End Get
    End Property

    Public Property o23FreeText01 As String
    Public Property o23FreeText02 As String
    Public Property o23FreeText03 As String
    Public Property o23FreeText04 As String
    Public Property o23FreeText05 As String
    Public Property o23FreeText06 As String
    Public Property o23FreeText07 As String
    Public Property o23FreeText08 As String
    Public Property o23FreeText09 As String
    Public Property o23FreeText10 As String
    Public Property o23FreeText11 As String
    Public Property o23FreeText12 As String
    Public Property o23FreeText13 As String
    Public Property o23FreeText14 As String
    Public Property o23FreeText15 As String
    Public Property o23BigText As String
    Public Property o23FreeNumber01 As Double
    Public Property o23FreeNumber02 As Double
    Public Property o23FreeNumber03 As Double
    Public Property o23FreeNumber04 As Double
    Public Property o23FreeNumber05 As Double
    Public Property o23FreeDate01 As Date?
    Public Property o23FreeDate02 As Date?
    Public Property o23FreeDate03 As Date?
    Public Property o23FreeDate04 As Date?
    Public Property o23FreeDate05 As Date?
    Public Property o23FreeBoolean01 As Boolean
    Public Property o23FreeBoolean02 As Boolean
    Public Property o23FreeBoolean03 As Boolean
    Public Property o23FreeBoolean04 As Boolean
    Public Property o23FreeBoolean05 As Boolean

    Public Property CalendarDateStart As Date?
    Public Property CalendarDateEnd As Date?


    Private Property _x23Name As String
    Public ReadOnly Property x23Name As String
        Get
            Return _x23Name
        End Get
    End Property
    Private Property _b02Name As String
    Public ReadOnly Property b02Name As String
        Get
            Return _b02Name
        End Get
    End Property
    Private Property _b02Color As String
    Public ReadOnly Property b02Color As String
        Get
            Return _b02Color
        End Get
    End Property
    Public ReadOnly Property NameWithComboName As String
        Get
            Return _x23Name & ": " & Me.o23Name
        End Get
    End Property
    Public ReadOnly Property NameWithCode As String
        Get
            If Me.o23Code = "" Then Return Me.o23Name Else Return Me.o23Name + " (" & Me.o23Code + ")"
        End Get
    End Property
    Public ReadOnly Property StyleDecoration As String
        Get
            If Me.IsClosed Then Return "line-through" Else Return ""
        End Get
    End Property

    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
End Class
