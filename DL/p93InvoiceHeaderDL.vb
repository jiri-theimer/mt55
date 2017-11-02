Public Class p93InvoiceHeaderDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p93InvoiceHeader
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p93ID=@p93id"

        Return _cDB.GetRecord(Of BO.p93InvoiceHeader)(s, New With {.p93id = intPID})
    End Function

    Public Function Save(cRec As BO.p93InvoiceHeader, lisP88 As List(Of BO.p88InvoiceHeader_BankAccount)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p93ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p93Name", .p93Name, DbType.String, , , True, "Název")
                pars.Add("p93Company", .p93Company, DbType.String, , , True, "Firma")
                pars.Add("p93City", .p93City, DbType.String, , , True, "Město")
                pars.Add("p93Street", .p93Street, DbType.String, , , True, "Ulice")
                pars.Add("p93Zip", .p93Zip, DbType.String, , , True, "PSČ")
                pars.Add("p93RegID", .p93RegID, DbType.String, , , True, "IČ")
                pars.Add("p93VatID", .p93VatID, DbType.String, , , True, "DIČ")
                pars.Add("p93Contact", .p93Contact, DbType.String, , , True, "Kontaktní info")
                pars.Add("p93Registration", .p93Registration, DbType.String, , , True, "Registrace v rejstříku")
                pars.Add("p93Referent", .p93Referent, DbType.String, , , True, "Referent")
                pars.Add("p93Signature", .p93Signature, DbType.String, , , True, "Podpis")
                pars.Add("p93FreeText01", .p93FreeText01, DbType.String, , , True, "Volné pole1")
                pars.Add("p93FreeText02", .p93FreeText02, DbType.String, , , True, "Volné pole2")
                pars.Add("p93FreeText03", .p93FreeText03, DbType.String, , , True, "Volné pole3")
                pars.Add("p93FreeText04", .p93FreeText04, DbType.String, , , True, "Volné pole4")

                pars.Add("p93validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p93validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p93InvoiceHeader", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intSavedP93ID As Integer = Me.LastSavedRecordPID
                If Not lisP88 Is Nothing Then   'vazba na účty a měny
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p88InvoiceHeader_BankAccount WHERE p93ID=" & intSavedP93ID.ToString)

                    For Each c In lisP88
                        If Not _cDB.RunSQL("INSERT INTO p88InvoiceHeader_BankAccount(p93ID,p86ID,j27ID) VALUES(" & _cDB.LastSavedRecordPID.ToString & "," & c.p86ID.ToString & "," & c.j27ID.ToString & ")") Then
                            Return False
                        End If
                    Next
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
        Return _cDB.RunSP("p93_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p93InvoiceHeader)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p93ID", myQuery)
        strW += bas.ParseWhereValidity("p93", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p93Name"

        Return _cDB.GetList(Of BO.p93InvoiceHeader)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p93", "a")
        s += " FROM p93InvoiceHeader a"
        
        Return s
    End Function

    Public Function GetList_p88(intPID As Integer) As IEnumerable(Of BO.p88InvoiceHeader_BankAccount)
        Dim s As String = "select a.*,p86.p86Name+' '+p86.p86BankAccount+' ('+isnull('/'+p86.p86BankCode,'')+')' as _Account,j27.j27Code as _j27Code," & bas.RecTail("p88", "a", False, False)
        s += " FROM p88InvoiceHeader_BankAccount a INNER JOIN p86BankAccount p86 ON a.p86ID=p86.p86ID INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " WHERE a.p93ID=@pid"

        Return _cDB.GetList(Of BO.p88InvoiceHeader_BankAccount)(s, New With {.pid = intPID})
    End Function
End Class
