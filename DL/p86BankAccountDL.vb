Public Class p86BankAccountDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p86BankAccount
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p86ID=@p86id"

        Return _cDB.GetRecord(Of BO.p86BankAccount)(s, New With {.p86id = intPID})
    End Function

    Public Function Save(cRec As BO.p86BankAccount) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p86ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p86Name", .p86Name, DbType.String, , , True, "Název účtu")
                pars.Add("p86BankName", .p86BankName, DbType.String, , , True, "Název banky")
                pars.Add("p86BankAccount", .p86BankAccount, DbType.String, , , True, "Číslo účtu")
                pars.Add("p86BankCode", .p86BankCode, DbType.String, , , True, "Kód banky")
                pars.Add("p86SWIFT", .p86SWIFT, DbType.String, , , True, "SWIFT")
                pars.Add("p86IBAN", .p86IBAN, DbType.String, , , True, "IBAN")
                pars.Add("p86BankAddress", .p86BankAddress, DbType.String, , , True, "Adresa banky")

                pars.Add("p86validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p86validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p86BankAccount", pars, bolINSERT, strW, True, _curUser.j03Login) Then

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
        Return _cDB.RunSP("p86_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing, Optional intP93ID As Integer = 0) As IEnumerable(Of BO.p86BankAccount)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p86ID", myQuery)
        strW += bas.ParseWhereValidity("p86", "a", myQuery)

        If intP93ID > 0 Then
            strW += " AND a.p86ID IN (SELECT p86ID FROM p88InvoiceHeader_BankAccount WHERE p93ID=" & intP93ID.ToString & ")"
        End If
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p86Name"

        Return _cDB.GetList(Of BO.p86BankAccount)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,p86Name+' ('+p86BankAccount+isnull('/'+p86BankCode,'')+')' as _NameWithAccount," & bas.RecTail("p86", "a")
        s += " FROM p86BankAccount a"

        Return s
    End Function
End Class
