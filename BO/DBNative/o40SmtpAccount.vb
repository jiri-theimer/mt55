Public Enum SslModeENUM
    _NoSSL = 0
    Implicit = 1
    Explicit = 2
End Enum
Public Enum smtpAuthenticationENUM
    _Auto = 0
    CramMD5 = 3
    DigestMD5 = 2
    GssApi = 9
    Login = 4
    Ntlm = 7
    OAuth20 = 10
    Plain = 1
End Enum
Public Class o40SmtpAccount
    Inherits BOMother
    Public Property o40Name As String
    Public Property o40EmailAddress As String
    Public Property o40Server As String
    Public Property o40Port As String
    Public Property o40Login As String
    Public Property o40Password As String
    Public Property o40IsVerify As Boolean
    Public Property o40SslModeFlag As SslModeENUM = SslModeENUM._NoSSL
    Public Property o40IsGlobalDefault As Boolean
    Public Property o40SmtpAuthentication As smtpAuthenticationENUM = smtpAuthenticationENUM._Auto

    Public ReadOnly Property DecryptedPassword As String
        Get
            Return BO.Crypto.Decrypt(Me.o40Password, "o40SmtpAccount")
        End Get
    End Property
End Class
