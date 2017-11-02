Public Class o41InboxAccountDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o41InboxAccount
        Dim s As String = "SELECT *," & bas.RecTail("o41")
        s += " FROM o41InboxAccount"
        s += " WHERE o41ID=@o41id"

        Return _cDB.GetRecord(Of BO.o41InboxAccount)(s, New With {.o41id = intPID})
    End Function

    Public Function Save(cRec As BO.o41InboxAccount) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o41ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("o41Name", .o41Name, DbType.String)
            pars.Add("o41Server", .o41Server, DbType.String)
            pars.Add("o41Login", .o41Login, DbType.String)
            pars.Add("o41Password", .o41Password, DbType.String)

            pars.Add("o41Port", .o41Port, DbType.String)
            pars.Add("o41Folder", .o41Folder, DbType.String)
            pars.Add("o41SslModeFlag", CInt(.o41SslModeFlag), DbType.Int32)
            pars.Add("o41IsDeleteMesageAfterImport", .o41IsDeleteMesageAfterImport, DbType.Boolean)
            pars.Add("o41DateImportAfter", .o41DateImportAfter, DbType.DateTime)
            pars.Add("o41validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("o41validuntil", .ValidUntil, DbType.DateTime)

            pars.Add("o41ForwardFlag_New", BO.BAS.IsNullDBKey(CInt(.o41ForwardFlag_New)), DbType.Int32)
            pars.Add("o41ForwardEmail_New", .o41ForwardEmail_New, DbType.String)
            pars.Add("o41ForwardFlag_Answer", BO.BAS.IsNullDBKey(CInt(.o41ForwardFlag_Answer)), DbType.Int32)
            pars.Add("o41ForwardEmail_Answer", .o41ForwardEmail_Answer, DbType.String)
            pars.Add("o41ForwardEmail_UnBound", .o41ForwardEmail_UnBound, DbType.String)
        End With

        If _cDB.SaveRecord("o41InboxAccount", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("o41_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o41InboxAccount)
        Dim s As String = "SELECT *," & bas.RecTail("o41") & " FROM o41InboxAccount", pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("o41ID", mq)
        
        strW += bas.ParseWhereValidity("o41", "", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.o41InboxAccount)(s, pars)

    End Function
End Class
