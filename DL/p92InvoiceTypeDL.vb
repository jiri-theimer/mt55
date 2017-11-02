Public Class p92InvoiceTypeDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p92InvoiceType
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p92ID=@p92id"

        Return _cDB.GetRecord(Of BO.p92InvoiceType)(s, New With {.p92id = intPID})
    End Function

    Public Function Save(cRec As BO.p92InvoiceType) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p92ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("x15ID", BO.BAS.IsNullDBKey(.x15ID), DbType.Int32)
                pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
                pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
                pars.Add("p98ID", BO.BAS.IsNullDBKey(.p98ID), DbType.Int32)
                pars.Add("x38ID_Draft", BO.BAS.IsNullDBKey(.x38ID_Draft), DbType.Int32)
                pars.Add("p93ID", BO.BAS.IsNullDBKey(.p93ID), DbType.Int32)
                pars.Add("j19ID", BO.BAS.IsNullDBKey(.j19ID), DbType.Int32)
                pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
                pars.Add("x31ID_Invoice", BO.BAS.IsNullDBKey(.x31ID_Invoice), DbType.Int32)
                pars.Add("x31ID_Attachment", BO.BAS.IsNullDBKey(.x31ID_Attachment), DbType.Int32)
                pars.Add("x31ID_Letter", BO.BAS.IsNullDBKey(.x31ID_Letter), DbType.Int32)
                pars.Add("p80ID", BO.BAS.IsNullDBKey(.p80ID), DbType.Int32)
                pars.Add("p92InvoiceType", .p92InvoiceType, DbType.Int32)
                pars.Add("p92Code", .p92Code, DbType.String)
                pars.Add("p92Name", .p92Name, DbType.String, , , True, "Název typu")
                pars.Add("p92InvoiceDefaultText1", .p92InvoiceDefaultText1, DbType.String, , , True, "Výchozí fakturační text")
                pars.Add("p92InvoiceDefaultText2", .p92InvoiceDefaultText2, DbType.String, , , True, "Výchozí technický text faktury")
                pars.Add("p92ReportConstantText", .p92ReportConstantText, DbType.String, , , True, "Report text")
                pars.Add("p92Ordinary", .p92Ordinary, DbType.Int32)
                pars.Add("p92validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p92validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p92InvoiceType", pars, bolINSERT, strW, True, _curUser.j03Login) Then

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
        Return _cDB.RunSP("p92_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p92InvoiceType)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p92ID", myQuery)
        strW += bas.ParseWhereValidity("p92", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p92Ordinary,x15Ordinary"

        Return _cDB.GetList(Of BO.p92InvoiceType)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,x15.x15Name as _x15Name,j27.j27Code as _j27Code,j17.j17Name as _j17Name,p93.p93Name as _p93Name," & bas.RecTail("p92", "a")
        s += " FROM p92InvoiceType a LEFT OUTER JOIN x15VatRateType x15 ON a.x15ID=x15.x15ID LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID LEFT OUTER JOIN p93InvoiceHeader p93 ON a.p93ID=p93.p93ID"

        Return s
    End Function
End Class
