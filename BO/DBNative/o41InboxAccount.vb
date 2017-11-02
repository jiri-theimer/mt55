Public Enum o41ForwardENUM
    _None = 0
    EntityRoles = 1
    EmailAddress = 2
End Enum
Public Class o41InboxAccount
    Inherits BOMother
    Public Property o41Name As String
    Public Property o41Server As String
    Public Property o41Port As String
    Public Property o41Login As String
    Public Property o41Password As String
    Public Property o41Folder As String = "inbox"
    Public Property o41SslModeFlag As BO.SslModeENUM = SslModeENUM._NoSSL
    Public Property o41IsDeleteMesageAfterImport As Boolean
    Public Property o41DateImportAfter As Date?
    Public Property o41ForwardFlag_New As o41ForwardENUM = o41ForwardENUM._None
    Public Property o41ForwardFlag_Answer As o41ForwardENUM = o41ForwardENUM._None

    Public Property o41ForwardEmail_New As String
    Public Property o41ForwardEmail_Answer As String
    Public Property o41ForwardEmail_UnBound As String


    Public ReadOnly Property DecryptedPassword As String
        Get
            Return BO.Crypto.Decrypt(Me.o41Password, "o41InboxAccount")
        End Get
    End Property
End Class
