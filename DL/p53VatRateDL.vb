Public Class p53VatRateDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p53VatRate
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p53ID=@p53id"

        Return _cDB.GetRecord(Of BO.p53VatRate)(s, New With {.p53id = intPID})
    End Function

    Public Function Save(cRec As BO.p53VatRate) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p53ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("x15ID", BO.BAS.IsNullDBKey(.x15ID), DbType.Int32)
                pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
                pars.Add("p53Value", .p53Value, DbType.Double)

                pars.Add("p53validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p53validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p53VatRate", pars, bolINSERT, strW, True, _curUser.j03Login) Then
               
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
        Return _cDB.RunSP("p53_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p53VatRate)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p53ID", myQuery)
        strW += bas.ParseWhereValidity("p53", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY x15Ordinary"

        Return _cDB.GetList(Of BO.p53VatRate)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,x15.x15Name as _x15Name,j27.j27Code as _j27Code,j17.j17Name as _j17Name," & bas.RecTail("p53", "a")
        s += " FROM p53VatRate a INNER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID"

        Return s
    End Function
End Class
