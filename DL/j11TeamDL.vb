Public Class j11TeamDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j11Team
        Dim s As String = "select *," & bas.RecTail("j11") & " FROM j11team WHERE j11id=@pid"
        Return _cDB.GetRecord(Of BO.j11Team)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress As String) As BO.j11Team
        Dim s As String = "select *," & bas.RecTail("j11") & " FROM j11team WHERE j11RobotAddress LIKE @robotkey"
        Return _cDB.GetRecord(Of BO.j11Team)(s, New With {.robotkey = strRobotAddress})
    End Function
    
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j11_delete", pars)

    End Function
    Public Function Save(cRec As BO.j11Team, lisJ02 As List(Of BO.j02Person)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "j11id=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("j11name", .j11Name, DbType.String, , , True, "Název")
                pars.Add("j11RobotAddress", .j11RobotAddress, DbType.String)
                pars.Add("j11Email", .j11Email, DbType.String)
                pars.Add("j11DomainAccount", .j11DomainAccount, DbType.String)
                pars.Add("j11validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("j11validuntil", .ValidUntil, DbType.DateTime)
                pars.Add("j11IsAllPersons", .j11IsAllPersons, DbType.Boolean)
            End With

            If _cDB.SaveRecord("j11Team", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedPID As Integer = cRec.PID
                If bolINSERT Then intLastSavedPID = _cDB.LastIdentityValue
                If Not bolINSERT Then
                    _cDB.RunSQL("DELETE FROM j12Team_Person WHERE j11ID=" & intLastSavedPID.ToString)
                End If
                If Not cRec.j11IsAllPersons Then
                    Dim j02ids As List(Of Integer) = lisJ02.Select(Function(p) p.PID).ToList()

                    _cDB.RunSQL("INSERT INTO j12Team_Person(j11ID,j02ID) SELECT " & intLastSavedPID.ToString & ",j02ID FROM j02Person WHERE j02ID IN (" & String.Join(",", j02ids) & ")")

                    Dim lisJ12 As IEnumerable(Of BO.j12Team_Person) = GetList_BoundJ12(0)
                    j02ids = lisJ12.Select(Function(p) p.j02ID).Distinct.ToList
                    For Each intJ02ID As Integer In j02ids
                        Dim j11ids As IEnumerable(Of Integer) = lisJ12.Where(Function(p) p.j02ID = intJ02ID).Select(Function(p) p.j11ID)
                        _cDB.RunSQL("UPDATE j03User SET j03Cache_j11IDs='" & String.Join(",", j11ids) & "' WHERE j02ID=" & intJ02ID.ToString)
                    Next
                End If
                

                sc.Complete()
                Return True
            Else
                _Error = _cDB.ErrorMessage
                Return False
            End If
        End Using
    End Function

    Public Function GetList_BoundJ12(intPID As Integer) As IEnumerable(Of BO.j12Team_Person)
        Dim s As String = "select a.*,j02.*,a.j12ID as _pid"
        s += " FROM j12Team_Person a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        If intPID <> 0 Then s += " WHERE a.j11id=@pid"

        Return _cDB.GetList(Of BO.j12Team_Person)(s, New With {.pid = intPID})
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j11Team)
        Dim s As String = "select *," & bas.RecTail("j11")
        s += " FROM j11team"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j11ID", myQuery)
            strW += bas.ParseWhereValidity("j11", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j11name"

        Return _cDB.GetList(Of BO.j11Team)(s)

    End Function
End Class
