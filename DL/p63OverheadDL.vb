Public Class p63OverheadDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p63Overhead
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p63ID=@p63ID"

        Return _cDB.GetRecord(Of BO.p63Overhead)(s, New With {.p63ID = intPID})
    End Function

    Public Function Save(cRec As BO.p63Overhead) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p63ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p32ID", BO.BAS.IsNullDBKey(.p32ID), DbType.Int32)
                pars.Add("p63Name", .p63Name, DbType.String, , , True, "Název")

                pars.Add("p63PercentRate", .p63PercentRate, DbType.Double)
                pars.Add("p63IsIncludeTime", .p63IsIncludeTime, DbType.Boolean)
                pars.Add("p63IsIncludeFees", .p63IsIncludeFees, DbType.Boolean)
                pars.Add("p63IsIncludeExpense", .p63IsIncludeExpense, DbType.Boolean)

                pars.Add("p63validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p63validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p63Overhead", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p63_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p63Overhead)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p63ID", myQuery)
        strW += bas.ParseWhereValidity("p63", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)



        Return _cDB.GetList(Of BO.p63Overhead)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,p32.p32Name as _p32Name," & bas.RecTail("p63", "a")
        s += " FROM p63Overhead a LEFT OUTER JOIN p32Activity p32 ON a.p32ID=p32.p32ID"

        Return s
    End Function
End Class
