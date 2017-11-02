Public Class p89ProformaTypeDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p89ProformaType
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p89ID=@p89id"

        Return _cDB.GetRecord(Of BO.p89ProformaType)(s, New With {.p89id = intPID})
    End Function

    Public Function Save(cRec As BO.p89ProformaType) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p89ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
                pars.Add("x38ID_Draft", BO.BAS.IsNullDBKey(.x38ID_Draft), DbType.Int32)
                pars.Add("x38ID_Payment", BO.BAS.IsNullDBKey(.x38ID_Payment), DbType.Int32)

                pars.Add("p93ID", BO.BAS.IsNullDBKey(.p93ID), DbType.Int32)
                pars.Add("j19ID", BO.BAS.IsNullDBKey(.j19ID), DbType.Int32)
                pars.Add("x31ID", BO.BAS.IsNullDBKey(.x31ID), DbType.Int32)
                pars.Add("x31ID_Payment", BO.BAS.IsNullDBKey(.x31ID_Payment), DbType.Int32)
                pars.Add("p89Code", .p89Code, DbType.String)
                pars.Add("p89Name", .p89Name, DbType.String, , , True, "Název typu")

                pars.Add("p89validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p89validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p89ProformaType", pars, bolINSERT, strW, True, _curUser.j03Login) Then

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
        Return _cDB.RunSP("p89_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p89ProformaType)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p89ID", myQuery)
        strW += bas.ParseWhereValidity("p89", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        ''s += " ORDER BY x15Ordinary"

        Return _cDB.GetList(Of BO.p89ProformaType)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,j27.j27Code as _j27Code,p93.p93Name as _p93Name," & bas.RecTail("p89", "a")
        s += " FROM p89ProformaType a LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " LEFT OUTER JOIN p93InvoiceHeader p93 ON a.p93ID=p93.p93ID"

        Return s
    End Function
End Class
