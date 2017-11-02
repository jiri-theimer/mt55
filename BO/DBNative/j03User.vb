Public Enum j03MobileForwardFlagENUM
    Auto = 0
    Manual = 1
End Enum
Public Class j03User
    Inherits BOMother
    Public Property j03Login As String
    Public Property j04ID As Integer
    Public Property j02ID As Integer
    Public Property j07ID As Integer
    Public Property j03IsDomainAccount As Boolean
    Public Property j03MembershipUserId As String
    Public Property j03IsSystemAccount As Boolean
    Public Property j03Aspx_PersonalPage As String
    Public Property j03Aspx_PersonalPage_Mobile As String
    Public Property j03IsLiveChatSupport As Boolean
    Public Property j03IsSiteMenuOnClick As Boolean
    Public Property j03SiteMenuSkin As String

    Public Property j03IsShallReadUpgradeInfo As Boolean
    Public Property j03IsMustChangePassword As Boolean
    Public Property j03PasswordExpiration As Date?
    Public Property j03Ping_TimeStamp As Date?
    Public Property j03MobileForwardFlag As j03MobileForwardFlagENUM = j03MobileForwardFlagENUM.Auto
    Public Property j03ModalWindowsFlag As Integer
    Public Property j03ProjectMaskIndex As Integer
    Protected Property _j04Name As String
    Protected Property _j02LastName As String
    Protected Property _j02FirstName As String
    Protected Property _j02TitleBeforeName As String
    Protected Property _j02Email As String
    Protected Property _j02WorksheetAccessFlag As Integer
    Public Property j03PageMenuFlag As Integer

    Public ReadOnly Property j04Name As String
        Get
            Return _j04Name
        End Get
    End Property


    Public ReadOnly Property Person As String
        Get
            Return Trim(_j02TitleBeforeName & " " & _j02FirstName & " " & _j02LastName)
        End Get
    End Property
    Public ReadOnly Property PersonDesc As String
        Get
            Return Trim(_j02LastName & " " & _j02FirstName & " " & _j02TitleBeforeName)
        End Get
    End Property
    Public ReadOnly Property PersonEmail As String
        Get
            Return _j02Email
        End Get
    End Property
    Public ReadOnly Property j02WorksheetAccessFlag As Integer
        Get
            Return _j02WorksheetAccessFlag
        End Get
    End Property
End Class
