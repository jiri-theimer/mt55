Public Class p61ActivityClusterDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p61ActivityCluster
        Dim s As String = "select *," & bas.RecTail("p61") & " FROM p61ActivityCluster WHERE p61id=@pid"
        Return _cDB.GetRecord(Of BO.p61ActivityCluster)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p61_delete", pars)

    End Function
    Public Function Save(cRec As BO.p61ActivityCluster, lisP32 As List(Of BO.p32Activity)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p61id=@pid"
                pars.Add("pid", cRec.PID, DbType.Int32)
            End If
            With cRec
                pars.Add("p61name", .p61Name, DbType.String, , , True, "Název")
                pars.Add("p61validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p61validuntil", .ValidUntil, DbType.DateTime)
            End With

            If _cDB.SaveRecord("p61ActivityCluster", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedPID As Integer = cRec.PID
                If bolINSERT Then intLastSavedPID = _cDB.LastSavedRecordPID
                If Not bolINSERT Then
                    _cDB.RunSQL("DELETE FROM p62ActivityCluster_Item WHERE p61ID=" & intLastSavedPID.ToString)
                End If
                Dim p32ids As List(Of Integer) = lisP32.Select(Function(p) p.PID).ToList()
                _cDB.RunSQL("INSERT INTO p62ActivityCluster_Item(p61ID,p32ID) SELECT " & intLastSavedPID.ToString & ",p32ID FROM p32Activity WHERE p32ID IN (" & String.Join(",", p32ids) & ")")

                sc.Complete()
                Return True
            Else
                _Error = _cDB.ErrorMessage
                Return False
            End If
        End Using
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p61ActivityCluster)
        Dim s As String = "select *," & bas.RecTail("p61")
        s += " FROM p61ActivityCluster"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("p61ID", myQuery)
            strW += bas.ParseWhereValidity("p61", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY p61name"

        Return _cDB.GetList(Of BO.p61ActivityCluster)(s)

    End Function
End Class
