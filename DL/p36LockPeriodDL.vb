Public Class p36LockPeriodDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p36LockPeriod
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p36ID=@p36id"

        Return _cDB.GetRecord(Of BO.p36LockPeriod)(s, New With {.p36id = intPID})
    End Function

    Public Function Save(cRec As BO.p36LockPeriod, lisP37 As List(Of BO.p37LockPeriod_Sheet)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p36ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
                pars.Add("j11ID", BO.BAS.IsNullDBKey(.j11ID), DbType.Int32)

                pars.Add("p36DateFrom", .p36DateFrom, DbType.DateTime)
                pars.Add("p36DateUntil", .p36DateUntil, DbType.DateTime)

                pars.Add("p36IsAllSheets", .p36IsAllSheets, DbType.Boolean)
                pars.Add("p36IsAllPersons", .p36IsAllPersons, DbType.Boolean)

                pars.Add("p36validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p36validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p36LockPeriod", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p37LockPeriod_Sheet WHERE p36ID=" & _cDB.LastSavedRecordPID.ToString)
                If Not lisP37 Is Nothing Then
                    Dim p34ids As List(Of Integer) = lisP37.Where(Function(x) x.IsSetAsDeleted = False).Select(Function(p) p.p34ID).ToList()
                    If p34ids.Count > 0 Then
                        If Not _cDB.RunSQL("INSERT INTO p37LockPeriod_Sheet(p36ID,p34ID) SELECT " & _cDB.LastSavedRecordPID.ToString & ",p34ID FROM p34ActivityGroup WHERE p34ID IN (" & String.Join(",", p34ids) & ")") Then
                            Return False
                        End If
                    End If
                End If
                sc.Complete()
                Return True
            Else
                Return False
            End If
        End Using
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p36_delete", pars)
    End Function


    Public Function GetList(myQuery As BO.myQuery) As IEnumerable(Of BO.p36LockPeriod)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p36ID", myQuery)
        strW += bas.ParseWhereValidity("p36", "a", myQuery)

        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.p36ID DESC"

        Return _cDB.GetList(Of BO.p36LockPeriod)(s, pars)

    End Function
    Public Function GetList_p37(intPID As Integer) As IEnumerable(Of BO.p37LockPeriod_Sheet)
        Dim s As String = "select a.*," & bas.RecTail("p37", "a", False, False) & ",p34.p34Name as _p34Name"
        s += " FROM p37LockPeriod_Sheet a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " WHERE a.p36ID=@pid"

        Return _cDB.GetList(Of BO.p37LockPeriod_Sheet)(s, New With {.pid = intPID})
    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p36", "a")
        s += ",j02.j02LastName+' '+j02.j02FirstName as _Person,j11.j11Name as _j11Name"
        s += " FROM p36LockPeriod a"
        s += " LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID"
        Return s
    End Function
End Class
