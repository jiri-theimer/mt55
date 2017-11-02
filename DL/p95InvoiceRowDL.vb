Public Class p95InvoiceRowDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p95InvoiceRow
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p95ID=@p95id"

        Return _cDB.GetRecord(Of BO.p95InvoiceRow)(s, New With {.p95id = intPID})
    End Function

    Public Function Save(cRec As BO.p95InvoiceRow) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p95ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p95Name", .p95Name, DbType.String, , , True, "Název")
            pars.Add("p95Code", .p95Code, DbType.String, , , True, "Kód")

            pars.Add("p95Ordinary", .p95Ordinary, DbType.Int32)

            pars.Add("p95Name_BillingLang1", .p95Name_BillingLang1, DbType.String, , , True, "Fakturační jazyk 1")
            pars.Add("p95Name_BillingLang2", .p95Name_BillingLang2, DbType.String, , , True, "Fakturační jazyk 2")
            pars.Add("p95Name_BillingLang3", .p95Name_BillingLang3, DbType.String, , , True, "Fakturační jazyk 3")
            pars.Add("p95Name_BillingLang4", .p95Name_BillingLang4, DbType.String, , , True, "Fakturační jazyk 4")
            
            pars.Add("p95validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p95validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p95InvoiceRow", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p95_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p95InvoiceRow)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.p95ID", myQuery)
            strW += bas.ParseWhereValidity("p95", "a", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If

        s += " ORDER BY p95Ordinary,p95Name"

        Return _cDB.GetList(Of BO.p95InvoiceRow)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p95", "a")
        s += " FROM p95InvoiceRow a"

        Return s
    End Function

End Class
