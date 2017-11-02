Public Class basMemberShip
    Private Shared _Error As String
    Public Sub New()
    End Sub
    Public Shared ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Shared Function CreateUser(ByVal strLogin As String, ByVal strEmail As String, ByVal strPassword As String) As Boolean
        Try

            membership.CreateUser(strLogin, strPassword, strEmail)
            Return True
        Catch ex As Exception
            log4net.LogManager.GetLogger("membershiplog").Error("strLogin: " & strLogin & ", strEmail: " & strEmail & ", strPassword: " & strPassword, ex)
            _Error = ex.Message
            Return False
        End Try
    End Function
    
    Public Shared Function GetUserID(strLogin As String) As String
        Try
            Return Membership.GetUser(strLogin).ProviderUserKey.ToString
        Catch ex As Exception
            _Error = ex.Message

            Return ""
        End Try
    End Function
    Public Shared Function RecoveryAccount(strLogin As String, strEmail As String, strNewPassword As String) As Integer
        Dim user As MembershipUser = Membership.GetUser(strLogin)
        If Not user Is Nothing Then Return 1
        If CreateUser(strLogin, strEmail, strNewPassword) Then
            Return 2
        Else
            Return 0
        End If
    End Function
    Public Shared Function RecoveryPassword(strLogin As String, Optional strExplicitPassword As String = "") As String
        Dim user As MembershipUser = Membership.GetUser(strLogin)
        Try
            Dim strNewPWD As String = strExplicitPassword
            If strNewPWD = "" Then
                Randomize()
                strNewPWD = Left((Rnd() * 10000).ToString, 2) & Left(BO.BAS.GetGUID(), 6)
            End If
            If user.ChangePassword(user.ResetPassword(), strNewPWD) Then
                Return strNewPWD
            End If
        Catch ex As Exception
            ''log4net.LogManager.GetLogger("membershiplog").Error("strLogin: " & strLogin, ex)
            _Error = ex.Message
            Return ""
        End Try
        Return ""
    End Function
    Public Shared Function ValidatBeforeCreate(strLogin As String, strPassword As String, strVerify As String) As Boolean
        If Membership.MinRequiredPasswordLength > Len(strPassword) Then
            _Error = "Minimální délka hesla je " & Membership.MinRequiredPasswordLength.ToString & " znaků."
            Return False
        End If
        If strPassword <> strVerify Then
            _Error = "Heslo nesouhlasí s ověřením."
            Return False
        End If
        Return True
    End Function
   

    Public Shared Function UpdateUser(ByVal strLogin As String, ByVal strEmail As String, ByVal bolActual As Boolean) As Boolean
        Dim user As MembershipUser = Membership.GetUser(strLogin)
        If user Is Nothing Then
            _Error = "Nelze načíst Membership profil uživatele."

            Return False
        End If
        With user
            .Email = strEmail
            .IsApproved = bolActual
        End With
        Try
            Membership.UpdateUser(user)
            Return True
        Catch ex As Exception
            log4net.LogManager.GetLogger("membershiplog").Error("strLogin: " & strLogin & ", strEmail: " & strEmail & ", bolActual: " & bolActual.ToString & vbCrLf, ex)
            _Error = ex.Message
            Return False
        End Try
    End Function
    Public Shared Function DeleteUser(ByVal strLogin As String) As Boolean
        Try
            Membership.DeleteUser(strLogin, True)
            Return True
        Catch ex As Exception
            log4net.LogManager.GetLogger("membershiplog").Error("strLogin: " & strLogin, ex)
            _Error = ex.Message
            Return False
        End Try
    End Function

    Public Shared Function RecoverySystemAccount() As Boolean
        Dim strMI As String = basMemberShip.GetUserID("mtservice")
        If strMI = "" Then
            Randomize()
            If Not basMemberShip.CreateUser("mtservice", "info@marktime.cz", "A.2" & (Rnd() * 100000).ToString) Then
                _Error = basMemberShip.ErrorMessage
                Return False
            End If
        End If

        Return True
    End Function

End Class
