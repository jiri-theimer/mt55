Public Class p91InvoiceDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer, Optional bolLoadRelation2AllProjects As Boolean = False) As BO.p91Invoice
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p91ID=@p91id"

        Return _cDB.GetRecord(Of BO.p91Invoice)(s, New With {.p91id = intPID})
    End Function
    Public Function LoadCreditNote(intPID As Integer) As BO.p91Invoice
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing) & " INNER JOIN p91Invoice sourcedoc ON a.p91ID_CreditNoteBind=sourcedoc.p91ID"
        s += " WHERE sourcedoc.p91ID=@p91id"

        Return _cDB.GetRecord(Of BO.p91Invoice)(s, New With {.p91id = intPID})
    End Function
    Public Function LoadByCode(strCode As String) As BO.p91Invoice
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing)

        s += " WHERE a.p91Code LIKE @code"
       
        Return _cDB.GetRecord(Of BO.p91Invoice)(s, New With {.code = strCode})
    End Function
    Public Function LoadMyLastCreated() As BO.p91Invoice
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.p91ID DESC"

        Return _cDB.GetRecord(Of BO.p91Invoice)(s, New With {.j02id_owner = _curUser.j02ID})
    End Function
    Public Function LoadLastCreatedByClient(intP28ID As Integer) As BO.p91Invoice
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p28ID=@p28id AND a.p91Amount_WithoutVat>0 ORDER BY a.p91ID DESC"

        Return _cDB.GetRecord(Of BO.p91Invoice)(s, New With {.p28id = intP28ID})
    End Function
    

    Public Function SaveP94(cRec As BO.p94Invoice_Payment) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p94ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p91ID", BO.BAS.IsNullDBKey(.p91ID), DbType.Int32)
            pars.Add("p94Date", .p94Date, DbType.DateTime)
            pars.Add("p94Amount", .p94Amount, DbType.Double)
            pars.Add("p94Code", .p94Code, DbType.String)
            pars.Add("p94Description", .p94Description, DbType.String)
        End With

        If _cDB.SaveRecord("p94Invoice_Payment", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            RecalcAmount(cRec.p91ID)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function DeleteP94(intP94ID As Integer, intP91ID As Integer) As Boolean
        If _cDB.RunSQL("DELETE FROM p94Invoice_Payment WHERE p94ID=" & intP94ID.ToString) Then
            RecalcAmount(intP91ID)
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub ClearExchangeDate(intP91ID As Integer)
        _cDB.RunSQL("update p91Invoice set p91DateExchange=null,p91ExchangeRate=null WHERE p91ID=" & intP91ID.ToString)
    End Sub
    Public Function Update(cRec As BO.p91Invoice, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters()
            pars.Add("pid", cRec.PID)
            With cRec
                If .p91Code = "" Then .p91Code = "TEMP" & BO.BAS.GetGUID() 'dočasný kód, bude později nahrazen
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)

                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
                pars.Add("p92ID", BO.BAS.IsNullDBKey(.p92ID), DbType.Int32)
                pars.Add("j19ID", BO.BAS.IsNullDBKey(.j19ID), DbType.Int32)
                pars.Add("p28ID", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
                pars.Add("p98ID", BO.BAS.IsNullDBKey(.p98ID), DbType.Int32)
                pars.Add("p63ID", BO.BAS.IsNullDBKey(.p63ID), DbType.Int32)
                pars.Add("p80ID", BO.BAS.IsNullDBKey(.p80ID), DbType.Int32)
                pars.Add("j02ID_ContactPerson", BO.BAS.IsNullDBKey(.j02ID_ContactPerson), DbType.Int32)
                pars.Add("o38ID_Primary", BO.BAS.IsNullDBKey(.o38ID_Primary), DbType.Int32)
                pars.Add("o38ID_Delivery", BO.BAS.IsNullDBKey(.o38ID_Delivery), DbType.Int32)


                pars.Add("p91Text1", .p91Text1, DbType.String, , , True, "Text faktury")
                pars.Add("p91Text2", .p91Text2, DbType.String, , , True, "Technický text faktury")
                pars.Add("p91Code", .p91Code, DbType.String)

                pars.Add("p91Date", BO.BAS.IsNullDBDate(.p91Date), DbType.DateTime)
                pars.Add("p91DateMaturity", BO.BAS.IsNullDBDate(.p91DateMaturity), DbType.DateTime)
                pars.Add("p91DateSupply", BO.BAS.IsNullDBDate(.p91DateSupply), DbType.DateTime)
                pars.Add("p91Datep31_From", BO.BAS.IsNullDBDate(.p91Datep31_From), DbType.DateTime)
                pars.Add("p91Datep31_Until", BO.BAS.IsNullDBDate(.p91Datep31_Until), DbType.DateTime)

                pars.Add("p91validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p91validuntil", .ValidUntil, DbType.DateTime)

                pars.Add("p91Client", .p91Client, DbType.String, , , True, "Název klienta")
                pars.Add("p91ClientPerson", .p91ClientPerson, DbType.String, , , True, "Kontaktní osoba")
                pars.Add("p91ClientPerson_Salutation", .p91ClientPerson_Salutation, DbType.String)
                pars.Add("p91Client_RegID", .p91Client_RegID, DbType.String)
                pars.Add("p91Client_VatID", .p91Client_VatID, DbType.String)
                pars.Add("p91ClientAddress1_Street", .p91ClientAddress1_Street, DbType.String)
                pars.Add("p91ClientAddress1_City", .p91ClientAddress1_City, DbType.String)
                pars.Add("p91ClientAddress1_ZIP", .p91ClientAddress1_ZIP, DbType.String, , , True, "PSČ")
                pars.Add("p91ClientAddress1_Country", .p91ClientAddress1_Country, DbType.String)

                pars.Add("p91ClientAddress2", .p91ClientAddress2, DbType.String, , , True, "Poštovní adresa")
            End With

            If _cDB.SaveRecord("p91Invoice", pars, False, "p91ID=@pid", True, _curUser.j03Login) Then
                Dim intLastSavedp91ID As Integer = _cDB.LastSavedRecordPID
                If Not lisX69 Is Nothing Then   'přiřazení rolí
                    bas.SaveX69(_cDB, BO.x29IdEnum.p91Invoice, intLastSavedp91ID, lisX69, False)
                End If
                If Not lisFF Is Nothing Then    'volná pole
                    bas.SaveFreeFields(_cDB, lisFF, "p91Invoice_FreeField", intLastSavedp91ID)
                End If
                pars = New DbParameters
                With pars
                    .Add("p91id", intLastSavedp91ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                    .Add("recalc_amount", True, DbType.Boolean)
                End With
                If _cDB.RunSP("p91_aftersave", pars) Then
                    sc.Complete()
                    Return True
                Else
                    Return False
                End If

            Else
                Return False
            End If
        End Using
    End Function

    Public Function ChangeVat(intP91ID As Integer, x15id As BO.x15IdEnum, dblNewVatRate As Double) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("x15id", BO.BAS.IsNullDBKey(x15id), DbType.Int32)
            .Add("newvatrate", dblNewVatRate, DbType.Double)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_change_vat", pars)

    End Function
    
    Public Sub RecalcAmount(intP91ID As Integer)
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
        End With
        If _cDB.RunSP("p91_recalc_amount", pars) Then

        Else

        End If
    End Sub
    Public Function Create(cP91Create As BO.p91Create) As Integer
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters
            With cP91Create
                pars.Add("guid", .TempGUID, DbType.String)
                pars.Add("j03id_sys", _curUser.PID, DbType.Int32)
                pars.Add("p28id", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
                pars.Add("p92id", BO.BAS.IsNullDBKey(.p92ID), DbType.Int32)
                pars.Add("p91isdraft", .IsDraft, DbType.Boolean)
                pars.Add("p91date", .DateIssue, DbType.DateTime)
                pars.Add("p91datematurity", .DateMaturity, DbType.DateTime)
                pars.Add("p91datesupply", .DateSupply, DbType.DateTime)
                pars.Add("p91datep31_from", .DateP31_From, DbType.DateTime)
                pars.Add("p91datep31_until", .DateP31_Until, DbType.DateTime)
                pars.Add("p91text1", .InvoiceText1, DbType.String)
                pars.Add("j02id_contactperson", BO.BAS.IsNullDBKey(.j02ID_ContactPerson), DbType.Int32)
                pars.Add("ret_p91id", , DbType.Int32, ParameterDirection.Output)
                pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 1000)
            End With
            If _cDB.RunSP("p91_create", pars) Then
                Dim intP91ID As Integer = pars.Get(Of Int32)("ret_p91id")
                sc.Complete()
                Return intP91ID
            Else
                Return 0
            End If
        End Using

    End Function
    Public Function CreateCreditNote(intP91ID As Integer, intP92ID_CreditNote As Integer) As Integer
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("p91id_bind", intP91ID, DbType.Int32)
            .Add("p92id_creditnote", intP92ID_CreditNote, DbType.Int32)
            .Add("ret_p91id", , DbType.Int32, ParameterDirection.Output)

            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If _cDB.RunSP("p91_create_creditnote", pars) Then
            Return pars.Get(Of Int32)("ret_p91id")
        Else
            Return 0
        End If

    End Function

    Public Function Delete(intPID As Integer, strGUID As String) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_delete", pars)
    End Function


    Private Function GetSQLWHERE(myQuery As BO.myQueryP91, ByRef pars As DL.DbParameters) As String
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p91ID", myQuery)
        strW += bas.ParseWhereValidity("p91", "a", myQuery)
        With myQuery
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                strW += " AND a.p91ID IN (select p91ID FROM p31Worksheet WHERE p41ID=@p41id)"
            End If

            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND (a.p28ID=@p28id OR a.p91ID IN (select x1.p91ID FROM p31Worksheet x1 INNER JOIN p41Project x2 ON x1.p41ID=x2.p41ID WHERE x2.p28ID_Client=@p28id))"
            End If
            If .o38ID <> 0 Then
                pars.Add("o38id", .o38ID, DbType.Int32)
                strW += " AND (a.o38ID_Primary=@o38id OR a.o38ID_Delivery=@o38id)"
            End If
            If .j02ID <> 0 Then
                pars.Add("j02id", .j02ID, DbType.Int32)
                strW += " AND a.p91ID IN (select p91ID FROM p31Worksheet WHERE j02ID=@j02id)"
            End If
            If .p56ID <> 0 Then
                pars.Add("p56id", .p56ID, DbType.Int32)
                strW += " AND a.p91ID IN (select p91ID FROM p31Worksheet WHERE p56ID=@p56id)"
            End If
            If .j02ID_Owner <> 0 Then
                pars.Add("ownerid", .j02ID_Owner, DbType.Int32)
                strW += " AND a.j02ID_Owner=@ownerid"
            End If
            If .p92ID <> 0 Then
                pars.Add("p92id", .p92ID, DbType.Int32)
                strW += " AND a.p92ID=@p92id"
            End If
            If .p93ID <> 0 Then
                pars.Add("p93id", .p93ID, DbType.Int32)
                strW += " AND p92.p93ID=@p93id"
            End If
            If .j27ID <> 0 Then
                pars.Add("j27id", .j27ID, DbType.Int32)
                strW += " AND a.j27ID=@j27id"
            End If
            If .b02ID <> 0 Then
                pars.Add("b02id", .b02ID, DbType.Int32)
                strW += " AND a.b02ID=@b02id"
            End If

            If .SpecificQuery > BO.myQueryp91_SpecificQuery._NotSpecified Then
                If .j02ID_ExplicitQueryFor > 0 Then
                    pars.Add("j02id_query", .j02ID_ExplicitQueryFor, DbType.Int32)
                Else
                    pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                End If
            End If
            If Year(.DateFrom) > 2000 Or Year(.DateUntil) < 2100 Then
                pars.Add("d1", .DateFrom, DbType.DateTime)
                pars.Add("d2", .DateUntil, DbType.DateTime)
                Select Case .PeriodType
                    Case BO.myQueryP91_PeriodType.p91DateSupply
                        strW += " AND a.p91DateSupply BETWEEN @d1 AND @d2"
                    Case BO.myQueryP91_PeriodType.p91DateMaturity
                        strW += " AND a.p91DateMaturity BETWEEN @d1 AND @d2"
                    Case BO.myQueryP91_PeriodType.p91Date
                        strW += " AND a.p91Date BETWEEN @d1 AND @d2"
                End Select
            End If
            If Not .DateInsertFrom Is Nothing Then
                pars.Add("di1", .DateInsertFrom)
                pars.Add("di2", .DateInsertUntil)
                strW += " AND a.p91DateInsert BETWEEN @di1 AND @di2"
            End If
            If Not .p31Date_D1 Is Nothing Then
                If Year(.p31Date_D1) > 1900 Then
                    pars.Add("dp31f1", .p31Date_D1)
                    pars.Add("dp31f2", .p31Date_D2)
                    strW += " AND a.p91ID IN (SELECT p91ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p31Date BETWEEN @dp31f1 AND @dp31f2)"
                End If
            End If
            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    strW += " AND " & strQueryW
                End If
            End If
            If .QuickQuery > BO.myQueryP91_QuickQuery._NotSpecified Then
                strW += " AND " & bas.GetQuickQuerySQL_p91(.QuickQuery)
            End If
          
            Select Case .SpecificQuery
                
                Case BO.myQueryp91_SpecificQuery.AllowedForRead
                    ''strW += " AND (a.j02ID_Owner=@j02id_query"

                    If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P91_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P91_Owner) Then
                        'právo paušálně číst všechny faktury - není třeba skládat podmínku
                    Else
                        Dim strJ11IDs As String = ""
                        If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
                        Dim intIndex As Integer = BO.x53PermValEnum.PR_P91_Reader
                        strW += " AND (a.j02ID_Owner=@j02id_query OR a.p91ID IN ("
                        strW += "SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID"
                        strW += " WHERE x67.x29ID=391 AND (x69.j02ID=@j02id_query " & strJ11IDs & "))"
                        strW += " OR a.p41ID_First IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID WHERE x67.x29ID=141 AND x68.x53ID=22 AND (x69.j02ID=@j02id_query " & strJ11IDs & "))"
                        strW += ")"

                    End If
            End Select
            If .ColumnFilteringExpression <> "" Then
                strW += " AND " & .ColumnFilteringExpression
            End If
            If .SearchExpression <> "" Then
                strW += " AND ("
                'něco jako fulltext
                strW += "a.p91Code LIKE '%'+@expr+'%' OR a.p91Text1 LIKE '%'+@expr+'%' OR a.p91Client_RegID LIKE '%'+@expr+'%' OR a.p91Client_VatID LIKE @expr+'%' OR p41.p41Name LIKE '%'+@expr+'%' OR a.p91Client LIKE '%'+@expr+'%' OR p28client.p28Name LIKE '%'+@expr+'%' OR p28client.p28CompanyShortName LIKE '%'+@expr+'%'"
                strW += ")"
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then strW += " AND a.p91ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=391 AND o51ID IN (" & String.Join(",", .o51IDs) & "))"
            End If
            If .x18Value <> "" Then
                strW += bas.CompleteX18QuerySql("p91", .x18Value)
            End If
        End With
        Return bas.TrimWHERE(strW)
    End Function

    Public Function GetList(myQuery As BO.myQueryP91) As IEnumerable(Of BO.p91Invoice)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly), pars As New DbParameters
        s += " " & GetSQLPart2(myQuery)
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then strSort = "a.p91ID DESC"

            If .MG_PageSize > 0 Then
                'použít stránkování do gridu
                s = GetSQL_OFFSET(strW, ParseSortExpression(strSort), .MG_PageSize, .MG_CurrentPageIndex, pars)
            Else
                'normální select
                If strW <> "" Then s += " WHERE " & strW
                If strSort <> "" Then
                    s += " ORDER BY " & ParseSortExpression(strSort)
                End If
            End If
        End With

     
        Return _cDB.GetList(Of BO.p91Invoice)(s, pars)
    End Function
    Public Function GetGridDataSource(myQuery As BO.myQueryP91) As DataTable
        Dim s As String = ""
        With myQuery
            If Not (String.IsNullOrEmpty(.MG_GridSqlColumns) Or String.IsNullOrEmpty(.MG_GridGroupByField)) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 And .MG_GridGroupByField <> "" Then
                    Select Case .MG_GridGroupByField
                        Case "Owner" : .MG_GridSqlColumns += ",j02owner.j02LastName+char(32)+j02owner.j02FirstName as Owner"
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If
            .MG_GridSqlColumns += ",a.p91ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.p91ValidFrom AND a.p91ValidUntil THEN 0 else 1 END) as IsClosed,a.p91IsDraft as IsDraft"
            .MG_GridSqlColumns += ",a.p91Amount_TotalDue as TotalDue,a.p91Amount_Debt as Debt,a.p91DateMaturity as Maturity,p92.p92InvoiceType as InvoiceType,j27.j27Code as j27Code_Grid"

        End With
        
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.p91ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If .MG_GridGroupByField <> "" Then
                Dim strPrimarySortField As String = .MG_GridGroupByField
                If strPrimarySortField = "Owner" Then strPrimarySortField = "j02owner.j02LastName+char(32)+j02owner.j02FirstName"
                If strORDERBY = "" Or LCase(strPrimarySortField) = Replace(Replace(LCase(.MG_SortString), " desc", ""), " asc", "") Then
                    strORDERBY = strPrimarySortField
                Else
                    strORDERBY = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            If strORDERBY = "" Then strORDERBY = "a.p91ID DESC"
            If .MG_PageSize > 0 Then
                Dim intStart As Integer = (.MG_CurrentPageIndex) * .MG_PageSize

                s = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & .MG_GridSqlColumns & " " & GetSQLPart2(myQuery)

                If strW <> "" Then s += " WHERE " & strW
                s += ") SELECT * FROM rst"
                pars.Add("start", intStart, DbType.Int32)
                pars.Add("end", (intStart + .MG_PageSize - 1), DbType.Int32)
                s += " WHERE RowIndex BETWEEN @start AND @end"
            Else
                'bez stránkování
                s = "SELECT " & .MG_GridSqlColumns & " " & GetSQLPart2(myQuery)
                If strW <> "" Then s += " WHERE " & strW
                s += " ORDER BY " & strORDERBY
            End If

        End With
        
        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function
    Public Function GetListAsDR(myQuery As BO.myQueryP91) As SqlClient.SqlDataReader

        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        s += " " & GetSQLPart2(myQuery)
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then strSort = "a.p91ID DESC"

            If .MG_PageSize > 0 Then
                'použít stránkování do gridu
                s = GetSQL_OFFSET(strW, ParseSortExpression(strSort), .MG_PageSize, .MG_CurrentPageIndex, pars)
            Else
                'normální select
                If strW <> "" Then s += " WHERE " & strW
                If strSort <> "" Then
                    s += " ORDER BY " & ParseSortExpression(strSort)
                End If
            End If
        End With


        Return _cDB.GetDataReader(s, , pars.Convert2PluginDbParameters)
    End Function

    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize

        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & GetSF() & " " & GetSQLPart2(Nothing)

        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " * FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"
        Return s
    End Function
    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "p91UserInsert").Replace("UserUpdate", "p91UserUpdate").Replace("DateInsert", "p91DateInsert").Replace("DateUpdate", "p91DateUpdate")
        strSort = strSort.Replace("Owner", "j02owner.j02LastName")
        strSort = strSort.Replace("WithoutVat_Krat_Kurz", "p91Amount_WithoutVat * p91ExchangeRate").Replace("Debt_Krat_Kurz", "p91Amount_Debt * p91ExchangeRate").Replace("p91Amount_TotalDue_Krat_Kurz", "p91Amount_TotalDue * p91ExchangeRate")
        ''.Replace("Debt_CZK", "p91Amount_Debt").Replace("Debt_EUR", "p91Amount_Debt").Replace("WithoutVat_CZK", "CASE WHEN a.j27ID=2 THEN p91Amount_WithoutVat END").Replace("WithoutVat_EUR", "CASE WHEN a.j27ID=3 THEN p91Amount_WithoutVat END")
        Return bas.NormalizeOrderByClause(strSort)
    End Function
    Private Function ParseFilterExpression(strFilter As String) As String
        'If strFilter.IndexOf(",") >= 0 Then strFilter = strFilter.Replace(",", ".")
        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryP91) As Integer

        Dim s As String = "SELECT count(a.p91ID) as Value " & GetSQLPart2(myQuery)

        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP91, strSumFields As String) As DataTable
        Dim s As String = "SELECT count(a.p91ID) as VirtualCount"
        Dim pars As New DL.DbParameters
        If strSumFields <> "" Then
            For Each strField As String In Split(strSumFields, "|")
                s += "," & strField
            Next
        End If
        s += " " & GetSQLPart2(myQuery)
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW
        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function
    Public Function GetSumRow(myQuery As BO.myQueryP91) As BO.p91InvoiceSum
        Dim s As String = "SELECT count(a.p91ID) as Count,sum(p91Amount_WithoutVat) as p91Amount_WithoutVat,sum(p91Amount_Vat) as p91Amount_Vat,sum(p91Amount_WithVat) as p91Amount_WithVat"
        s += ",sum(p91Amount_Billed) as p91Amount_Billed,sum(p91Amount_Debt) as p91Amount_Debt,sum(p91ProformaAmount) as p91ProformaAmount,sum(p91Amount_TotalDue) as p91Amount_TotalDue"
        s += ",sum(p91Amount_WithoutVat*p91ExchangeRate) as WithoutVat_Krat_Kurz,sum(p91Amount_Debt*p91ExchangeRate) as Debt_Krat_Kurz,sum(p91Amount_TotalDue*p91ExchangeRate) as p91Amount_TotalDue_Krat_Kurz"
        ''s += ",sum(case when a.j27ID=2 THEN p91Amount_WithoutVat end) as WithoutVat_CZK,sum(case when a.j27ID=3 THEN p91Amount_WithoutVat end) as WithoutVat_EUR"
        ''s += ",sum(case when a.j27ID=2 THEN p91Amount_Debt end) as Debt_CZK,sum(case when a.j27ID=3 THEN p91Amount_Debt end) as Debt_EUR"

        s += " " & GetSQLPart2(myQuery)
        
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.p91InvoiceSum)(s, pars)
    End Function

    Private Function GetSF() As String
        Dim s As New System.Text.StringBuilder
        s.Append("a.p92ID,a.p28ID,a.j27ID,a.j19ID,a.j02ID_Owner,a.p41ID_First,a.p91ID_CreditNoteBind,a.j17ID,a.p98ID,a.p63ID,a.p80ID,a.o38ID_Primary,a.o38ID_Delivery,a.x15ID,a.b02ID,a.j02ID_ContactPerson,a.p91FixedVatRate,a.p91Code,a.p91IsDraft,a.p91Date,a.p91DateBilled,a.p91DateMaturity,a.p91DateSupply,a.p91DateExchange,a.p91ExchangeRate")
        s.Append(",a.p91Datep31_From,a.p91Datep31_Until,a.p91Amount_WithoutVat,a.p91Amount_Vat,a.p91Amount_Billed,a.p91Amount_WithVat,a.p91Amount_Debt,a.p91RoundFitAmount,a.p91Text1,a.p91Text2,a.p91ProformaAmount,a.p91ProformaBilledAmount,a.p91Amount_WithoutVat_None,a.p91VatRate_Low,a.p91Amount_WithVat_Low,a.p91Amount_WithoutVat_Low,a.p91Amount_Vat_Low")
        s.Append(",a.p91VatRate_Standard,a.p91Amount_WithVat_Standard,a.p91Amount_WithoutVat_Standard,a.p91Amount_Vat_Standard,a.p91VatRate_Special,a.p91Amount_WithVat_Special,a.p91Amount_WithoutVat_Special,a.p91Amount_Vat_Special,a.p91Amount_TotalDue")
        s.Append("," & bas.RecTail("p91", "a") & ",p28client.p28Name as _p28Name,p92.p92Name as _p92Name,p92.p93ID as _p93ID,isnull(p41.p41NameShort,p41.p41Name) as _p41Name,b02.b02Name as _b02Name,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,j17.j17Name as _j17Name,j27.j27Code as _j27Code,p92.p92InvoiceType as _p92InvoiceType,p92.b01ID as _b01ID,p28client.p28CompanyName as _p28CompanyName")
        s.Append(",a.p91Client,a.p91ClientPerson,a.p91ClientPerson_Salutation,a.p91Client_RegID,a.p91Client_VatID,a.p91ClientAddress1_Street,a.p91ClientAddress1_City,a.p91ClientAddress1_ZIP,a.p91ClientAddress1_Country,a.p91ClientAddress2")
        ''s.Append(",case when a.j27ID=2 THEN p91Amount_WithoutVat END as WithoutVat_CZK,case when a.j27ID=3 THEN p91Amount_WithoutVat END as WithoutVat_EUR")
        Return s.ToString

    End Function
    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " " & GetSF()
        Return s
    End Function
    Private Function GetSQLPart2(mq As BO.myQueryP91) As String
        Dim s As New System.Text.StringBuilder
        s.Append("FROM p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID")
        s.Append(" INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID")
        s.Append(" LEFT OUTER JOIN p41Project p41 ON a.p41ID_First=p41.p41ID")
        s.Append(" LEFT OUTER JOIN p28Contact p28client ON a.p28ID=p28client.p28ID")
        s.Append(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID")
        s.Append(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID")
        s.Append(" LEFT OUTER JOIN j17Country j17 ON a.j17ID=j17.j17ID")
        s.Append(" LEFT OUTER JOIN p91Invoice_FreeField p91free ON a.p91ID=p91free.p91ID")
        If Not mq Is Nothing Then
            If mq.MG_AdditionalSqlFROM <> "" Then s.Append(" " & mq.MG_AdditionalSqlFROM)
        End If

        Return s.ToString
    End Function


    Public Function GetList_p94(intPID As Integer) As IEnumerable(Of BO.p94Invoice_Payment)
        Dim s As String = "select *," & bas.RecTail("p94", "", False, False)
        s += " FROM p94Invoice_Payment WHERE p91ID=@pid ORDER BY p94Date DESC"

        Return _cDB.GetList(Of BO.p94Invoice_Payment)(s, New With {.pid = intPID})
    End Function
    Public Function LoadP94ByCode(strP94Code As String) As BO.p94Invoice_Payment
        Dim s As String = "select *," & bas.RecTail("p94", "", False, False)
        s += " FROM p94Invoice_Payment WHERE p94Code LIKE @code"

        Return _cDB.GetRecord(Of BO.p94Invoice_Payment)(s, New With {.code = strP94Code})

    End Function

    Public Function ChangeCurrency(intP91ID As Integer, intJ27ID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("j27id", BO.BAS.IsNullDBKey(intJ27ID), DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_change_currency", pars)

    End Function
    Public Function ConvertFromDraft(intP91ID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_convertdraft", pars)

    End Function

    Public Function SaveP99(intP91ID As Integer, intP90ID As Integer, intP82ID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("p90id", intP90ID, DbType.Int32)
            .Add("p82id", intP82ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_proforma_save", pars)
    End Function
    Public Function DeleteP99(intP91ID As Integer, intP90ID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p91id", intP91ID, DbType.Int32)
            .Add("p90id", intP90ID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p91_proforma_delete", pars)
    End Function

    Public Function RecalcFPR(d1 As Date, d2 As Date, Optional intP51ID As Integer = 0) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p51id", intP51ID, DbType.Int32)
            .Add("d1", d1, DbType.DateTime)
            .Add("d2", d2, DbType.DateTime)
        End With
        Return _cDB.RunSP("p91_fpr_recalc_all_invoices", pars)


    End Function
End Class
