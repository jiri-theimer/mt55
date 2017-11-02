Public Class p80InvoiceAmountStructureDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p80InvoiceAmountStructure
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p80ID=@p80ID"

        Return _cDB.GetRecord(Of BO.p80InvoiceAmountStructure)(s, New With {.p80ID = intPID})
    End Function

    Public Function Save(cRec As BO.p80InvoiceAmountStructure) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p80ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p80Name", .p80Name, DbType.String, , , True, "Název")

                pars.Add("p80IsTimeSeparate", .p80IsTimeSeparate, DbType.Boolean)
                pars.Add("p80IsExpenseSeparate", .p80IsExpenseSeparate, DbType.Boolean)
                pars.Add("p80IsFeeSeparate", .p80IsFeeSeparate, DbType.Boolean)

                pars.Add("p80validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p80validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p80InvoiceAmountStructure", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p80_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p80InvoiceAmountStructure)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p80ID", myQuery)
        strW += bas.ParseWhereValidity("p80", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.p80InvoiceAmountStructure)(s)

    End Function


    Private Function GetSQLPart1() As String
        Return "SELECT a.*," & bas.RecTail("p80", "a") & " FROM p80InvoiceAmountStructure a"
    End Function
End Class
