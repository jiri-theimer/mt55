Public Class p38ActivityTagDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p38ActivityTag
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p38ID=@p38id"

        Return _cDB.GetRecord(Of BO.p38ActivityTag)(s, New With {.p38id = intPID})
    End Function

    Public Function Save(cRec As BO.p38ActivityTag) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p38ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p38Name", .p38Name, DbType.String, , , True, "Název")
            pars.Add("p38Code", .p38Code, DbType.String, , , True, "Kód")
            pars.Add("p38FreeText01", .p38FreeText01, DbType.String, , , True, "Volné pole 1")
            pars.Add("p38FreeText02", .p38FreeText02, DbType.String, , , True, "Volné pole 2")
            pars.Add("p38Ordinary", .p38Ordinary, DbType.Int32)

            pars.Add("p38validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p38validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p38ActivityTag", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p38_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p38ActivityTag)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.p38ID", myQuery)
            strW += bas.ParseWhereValidity("p38", "a", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If

        s += " ORDER BY p38Ordinary,p38Name"

        Return _cDB.GetList(Of BO.p38ActivityTag)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p38", "a")
        s += " FROM p38ActivityTag a"

        Return s
    End Function
End Class
