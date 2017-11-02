Public Class p51PriceListDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p51PriceList
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p51ID=@p51id"

        Return _cDB.GetRecord(Of BO.p51PriceList)(s, New With {.p51id = intPID})
    End Function

    Public Function Save(cRec As BO.p51PriceList, lisP52 As List(Of BO.p52PriceList_Item)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p51ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p51Name", .p51Name, DbType.String, , , True, "Název")
                pars.Add("p51Code", .p51Code, DbType.String, , , True, "Kód")
                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("p51ID_Master", BO.BAS.IsNullDBKey(.p51ID_Master), DbType.Int32)
                pars.Add("p51IsMasterPriceList", .p51IsMasterPriceList, DbType.Boolean)
                pars.Add("p51IsInternalPriceList", .p51IsInternalPriceList, DbType.Boolean)
                pars.Add("p51IsCustomTailor", .p51IsCustomTailor, DbType.Boolean)
                pars.Add("p51Ordinary", .p51Ordinary, DbType.Int32)
                pars.Add("p51DefaultRateT", .p51DefaultRateT, DbType.Double)
                pars.Add("p51validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p51validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p51PriceList", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedP51ID As Integer = _cDB.LastSavedRecordPID
                If Not lisP52 Is Nothing Then   'položky ceníku
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p52PriceList_Item WHERE p51ID=" & _cDB.LastSavedRecordPID.ToString)
                    Dim x As Integer = 0
                    For Each c In lisP52
                        pars = New DbParameters
                        pars.Add("p51ID", intLastSavedP51ID, DbType.Int32)
                        pars.Add("j02ID", BO.BAS.IsNullDBKey(c.j02ID), DbType.Int32)
                        pars.Add("j07ID", BO.BAS.IsNullDBKey(c.j07ID), DbType.Int32)
                        pars.Add("p34ID", BO.BAS.IsNullDBKey(c.p34ID), DbType.Int32)
                        pars.Add("p32ID", BO.BAS.IsNullDBKey(c.p32ID), DbType.Int32)
                        pars.Add("p52Name", c.p52Name, DbType.String)
                        pars.Add("p52Rate", c.p52Rate, DbType.Double)
                        pars.Add("p52IsMaster", cRec.p51IsMasterPriceList, DbType.Boolean)
                        pars.Add("p52IsPlusAllTimeSheets", c.p52IsPlusAllTimeSheets, DbType.Boolean)
                        If Not _cDB.SaveRecord("p52PriceList_Item", pars, True, , True, _curUser.j03Login, False) Then
                            Return False
                        End If

                    Next
                End If

                pars = New DbParameters
                With pars
                    .Add("p51id", intLastSavedP51ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                End With
                _cDB.RunSP("p51_aftersave", pars)

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
        Return _cDB.RunSP("p51_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p51PriceList)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p51ID", myQuery), pars As New DbParameters
        strW += bas.ParseWhereValidity("p51", "a", myQuery)
        If Not myQuery Is Nothing Then
            If Not String.IsNullOrEmpty(myQuery.SearchExpression) Then
                strW += " AND (a.p51Name like '%'+@expr+'%' OR a.p51Code LIKE '%'+@expr+'%')"
                pars.Add("expr", myQuery.SearchExpression, DbType.String)
            End If
            If Not String.IsNullOrEmpty(myQuery.ColumnFilteringExpression) Then
                strW += " AND " & myQuery.ColumnFilteringExpression.Replace("p51", "[a.p51").Replace("[", "").Replace("]", "")
            End If
        End If
        
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.p51IsMasterPriceList,a.p51Ordinary,a.p51Name"

        Return _cDB.GetList(Of BO.p51PriceList)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p51", "a") & ",master.p51Name as _p51Name_Master,j27.j27Code as _j27Code"
        s += " FROM p51PriceList a LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " LEFT OUTER JOIN p51PriceList master ON a.p51ID_Master=master.p51ID"
        Return s
    End Function

    Public Function GetList_p52(intPID As Integer) As IEnumerable(Of BO.p52PriceList_Item)
        Dim s As String = "select a.*,j02.j02LastName+' '+j02.j02FirstName as _Person,j07.j07Name as _j07Name,p34.p34Name as _p34Name,p32.p32Name as _p32Name," & bas.RecTail("p52", "a", False, False)
        s += " FROM p52PriceList_Item a LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID"
        s += " LEFT OUTER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " LEFT OUTER JOIN p32Activity p32 ON a.p32ID=p32.p32ID"
        s += " WHERE a.p51ID=@pid"

        Return _cDB.GetList(Of BO.p52PriceList_Item)(s, New With {.pid = intPID})
    End Function
End Class
