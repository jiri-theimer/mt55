Public Class c26HolidayDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.c26Holiday
        Dim s As String = "select a.*,j17Name as _j17Name," & bas.RecTail("c26", "a") & " FROM c26Holiday a LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID WHERE a.c26ID=@pid"
        Return _cDB.GetRecord(Of BO.c26Holiday)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("c26_delete", pars)

    End Function
    Public Function Save(cRec As BO.c26Holiday) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "c26ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
            pars.Add("c26Name", .c26Name, DbType.String, , , True, "Název")
            pars.Add("c26Date", .c26Date, DbType.DateTime)
            pars.Add("c26ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("c26ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("c26Holiday", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            pars = New DbParameters
            With pars
                .Add("c26id", Me.LastSavedRecordPID, DbType.Int32)
                .Add("j03id_sys", _curUser.PID, DbType.Int32)
            End With
            If _cDB.RunSP("c26_aftersave", pars) Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.c26Holiday)
        Dim s As String = "select a.*,j17.j17Name as _j17Name," & bas.RecTail("c26", "a")
        s += " FROM c26Holiday a LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID"
        Dim pars As New DbParameters
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.c26ID", myQuery)
            strW += bas.ParseWhereValidity("c26", "a", myQuery)
            strW += " AND c26Date BETWEEN @d1 AND @d2"
            pars.Add("d1", myQuery.DateFrom, DbType.DateTime)
            pars.Add("d2", myQuery.DateUntil, DbType.DateTime)

            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY c26Date"

        Return _cDB.GetList(Of BO.c26Holiday)(s, pars)

    End Function
End Class
