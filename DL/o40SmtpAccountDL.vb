Public Class o40SmtpAccountDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o40SmtpAccount
        Dim s As String = "SELECT *," & bas.RecTail("o40")
        s += " FROM o40SmtpAccount"
        s += " WHERE o40ID=@o40id"

        Return _cDB.GetRecord(Of BO.o40SmtpAccount)(s, New With {.o40id = intPID})
    End Function

    Public Function Save(cRec As BO.o40SmtpAccount) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o40ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("o40Name", .o40Name, DbType.String)
            pars.Add("o40EmailAddress", .o40EmailAddress, DbType.String)
            pars.Add("o40Server", .o40Server, DbType.String)
            pars.Add("o40Login", .o40Login, DbType.String)
            pars.Add("o40Password", .o40Password, DbType.String)
            pars.Add("o40IsVerify", .o40IsVerify, DbType.Boolean)

            pars.Add("o40Port", .o40Port, DbType.String)
            pars.Add("o40SslModeFlag", CInt(.o40SslModeFlag), DbType.Int32)
            pars.Add("o40SmtpAuthentication", CInt(.o40SmtpAuthentication), DbType.Int32)

            pars.Add("o40validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("o40validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("o40SmtpAccount", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o40_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o40SmtpAccount)
        Dim s As String = "SELECT *," & bas.RecTail("o40") & " FROM o40SmtpAccount", pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("o40ID", mq)

        strW += bas.ParseWhereValidity("o40", "", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.o40SmtpAccount)(s, pars)

    End Function

    Public Function SetGlobalDefaultSmtpAccount(intO40ID As Integer) As Boolean
        _cDB.RunSQL("UPDATE o40SmtpAccount set o40IsGlobalDefault=0")
        If intO40ID > 0 Then
            Return _cDB.RunSQL("UPDATE o40SmtpAccount set o40IsGlobalDefault=1 WHERE o40ID=" & intO40ID.ToString)
        Else
            Return True
        End If

    End Function
End Class
