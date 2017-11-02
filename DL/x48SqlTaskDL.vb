Public Class x48SqlTaskDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x48SqlTask
        Dim s As String = GetSQLPart1()
        s += " WHERE a.x48ID=@x48id"

        Return _cDB.GetRecord(Of BO.x48SqlTask)(s, New With {.x48id = intPID})
    End Function

    Public Function Save(cRec As BO.x48SqlTask) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x48ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("x31ID", BO.BAS.IsNullDBKey(.x31ID), DbType.Int32)
            pars.Add("x48Sql", .x48Sql, DbType.String)
            pars.Add("x48Name", .x48Name, DbType.String, , , True, "Název úlohy")
            pars.Add("x48TaskOutputFlag", .x48TaskOutputFlag, DbType.Int32)

            pars.Add("x48Ordinary", .x48Ordinary, DbType.Int32)
            pars.Add("x48MailSubject", .x48MailSubject, DbType.String)
            pars.Add("x48MailBody", .x48MailBody, DbType.String, , , True, "Mail zpráva")
            pars.Add("x48MailTo", .x48MailTo, DbType.String, , , True, "x48MailTo")

            pars.Add("x48IsRunInDay1", .x48IsRunInDay1, DbType.Boolean)
            pars.Add("x48IsRunInDay2", .x48IsRunInDay2, DbType.Boolean)
            pars.Add("x48IsRunInDay3", .x48IsRunInDay3, DbType.Boolean)
            pars.Add("x48IsRunInDay4", .x48IsRunInDay4, DbType.Boolean)
            pars.Add("x48IsRunInDay5", .x48IsRunInDay5, DbType.Boolean)
            pars.Add("x48IsRunInDay6", .x48IsRunInDay6, DbType.Boolean)
            pars.Add("x48IsRunInDay7", .x48IsRunInDay7, DbType.Boolean)
            pars.Add("x48RunInTime", .x48RunInTime, DbType.String)
            pars.Add("x48IsRepeat", .x48IsRepeat, DbType.Boolean)

            pars.Add("x48validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("x48validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("x48SqlTask", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function UpdateLastScheduledRun(intPID As Integer, dat As Date?) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID)
        pars.Add("d", dat, DbType.DateTime)

        Return _cDB.RunSQL("UPDATE x48SqlTask SET x48LastScheduledRun=@d WHERE x48ID=@pid", pars)
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x48_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x48SqlTask)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = ""
        If Not myQuery Is Nothing Then
            strW = bas.ParseWhereMultiPIDs("a.x48ID", myQuery)
            strW += bas.ParseWhereValidity("x48", "", myQuery)

        End If
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY x29ID,x48Ordinary,x48Name"

        Return _cDB.GetList(Of BO.x48SqlTask)(s, pars)

    End Function



    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,x29.x29Name as _x29Name," & bas.RecTail("x48", "a")
        s += " FROM x48SqlTask a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID"
        Return s
    End Function
End Class

