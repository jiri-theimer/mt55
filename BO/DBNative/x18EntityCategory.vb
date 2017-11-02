Public Enum x18GridColsENUM
    NameAndCode = 1 '1-kromě vlastních polí zobrazovat v přehledu i název i kód
    CodeOnly = 2    '2-kromě vlastních polí zobrazovat kód
    NameOnly = 3    '3-kromě vlastních polí zobrazovat název
    NotUsed = 4       '4-Ani název ani kód
End Enum
Public Enum x18EntryCodeENUM
    Manual = 1  '1-vyplňovat ručně kód
    NotUsed = 2 '2-nepoužívat kód
    AutoX18 = 3 '3-automaticky generovat v rámci štítku
    AutoP41 = 4 '4-automaticky generovat v rámci projektu
End Enum


Public Enum x18EntryNameENUM
    Manual = 1    '1-vyplňovat ručně název
    NotUsed = 2     '2-nevyplňovat název
End Enum


Public Enum x18EntryOrdinaryENUM
    Manual = 1      '1-pořadí zadávat ručně
    NotUsed = 2     '2-nepracovat s pořadím
End Enum
Public Enum x18DashboardENUM
    NotUsed = 0
    CreateLinkAndGrid = 1
    CreateLinkOnly = 2
    LinkOnly = 3
    ShowItemsLikeNoticeboard = 4

End Enum
Public Enum x18UploadENUM
    NotUsed = 0
    FileSystemUpload = 1

End Enum

Public Class x18EntityCategory
    Inherits BOMother
    Public Property x23ID As Integer
    Public Property b01ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property x38ID As Integer
    Public Property x18Name As String
    Public Property x18NameShort As String

    Public Property x18Ordinary As Integer

    Friend Property _x23Name As String

    Public Property x18IsColors As Boolean
    Public Property x18IsManyItems As Boolean
    Public Property x18Icon As String
    Public Property x18Icon32 As String
    Public Property x18IsClueTip As Boolean
    Public Property x18ReportCodes As String
    Public Property x18GridColsFlag As x18GridColsENUM = x18GridColsENUM.NameAndCode
    Public Property x18EntryNameFlag As x18EntryNameENUM = x18EntryNameENUM.Manual
    Public Property x18EntryCodeFlag As x18EntryCodeENUM = x18EntryCodeENUM.Manual
    Public Property x18EntryOrdinaryFlag As x18EntryOrdinaryENUM = x18EntryOrdinaryENUM.Manual
    Public Property x18IsCalendar As Boolean
    Public Property x18CalendarFieldStart As String
    Public Property x18CalendarFieldEnd As String
    Public Property x18CalendarFieldSubject As String
    Public Property x18CalendarResourceField As String
    Public Property x18DashboardFlag As x18DashboardENUM = x18DashboardENUM.NotUsed
    Public Property x18UploadFlag As x18UploadENUM = x18UploadENUM.NotUsed
    
    Public Property x18MaxOneFileSize As Integer
    Public Property x18AllowedFileExtensions As String
    Public Property x18IsAllowEncryption As Boolean
    Public Property x18JavascriptFile As String
    Public Property x31ID_Plugin As Integer

    Public ReadOnly Property x23Name As String
        Get
            Return _x23Name
        End Get
    End Property
    Friend Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property

    Private Property _Is_p41 As Boolean
    Public ReadOnly Property Is_p41 As Boolean
        Get
            Return _Is_p41
        End Get
    End Property
    Private Property _Is_p28 As Boolean
    Public ReadOnly Property Is_p28 As Boolean
        Get
            Return _Is_p28
        End Get
    End Property
    Private Property _Is_p31 As Boolean
    Public ReadOnly Property Is_p31 As Boolean
        Get
            Return _Is_p31
        End Get
    End Property
    Private Property _Is_j02 As Boolean
    Public ReadOnly Property Is_j02 As Boolean
        Get
            Return _Is_j02
        End Get
    End Property
    Private Property _Is_o23 As Boolean
    Public ReadOnly Property Is_o23 As Boolean
        Get
            Return _Is_o23
        End Get
    End Property
    Private Property _Is_p91 As Boolean
    Public ReadOnly Property Is_p91 As Boolean
        Get
            Return _Is_p91
        End Get
    End Property
    Private Property _Is_p56 As Boolean
    Public ReadOnly Property Is_p56 As Boolean
        Get
            Return _Is_p56
        End Get
    End Property
    Private Property _Is_o22 As Boolean
    Public ReadOnly Property Is_o22 As Boolean
        Get
            Return _Is_o22
        End Get
    End Property

    Public ReadOnly Property TagOrDoc As String
        Get
            If Me.x18IsManyItems Then Return "Typ dokumentu" Else Return "Kategorie"
        End Get
    End Property
End Class
