Public Class p28ContactDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Overloads Function Load(intPID As Integer) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p28ID=@p28id"

        Return _cDB.GetRecord(Of BO.p28Contact)(s, New With {.p28id = intPID})
    End Function
    Public Function LoadMyLastCreated() As BO.p28Contact
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p28UserInsert=@mylogin ORDER BY a.p28ID DESC"

        Return _cDB.GetRecord(Of BO.p28Contact)(s, New With {.mylogin = _curUser.j03Login})
    End Function
    Public Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p28Contact
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p28TreeIndex<=@curindex AND a.p28TreeLevel=0 ORDER BY a.p28TreeIndex DESC"

        Return _cDB.GetRecord(Of BO.p28Contact)(s, New With {.curindex = intCurTreeIndex})
    End Function
    Public Function LoadByRegID(strRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("regid", strRegID, DbType.String)
        s += " WHERE a.p28RegID LIKE @regid"
        If intP28ID_Exclude <> 0 Then
            s += " AND a.p28ID<>@p28id_exclude"
            pars.Add("p28id_exclude", intP28ID_Exclude, DbType.Int32)
        End If

        Return _cDB.GetRecord(Of BO.p28Contact)(s, pars)
    End Function
    Public Function LoadByVatID(strVatID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("vatid", strVatID, DbType.String)
        s += " WHERE a.p28VatID LIKE @vatid"
        If intP28ID_Exclude <> 0 Then
            s += " AND a.p28ID<>@p28id_exclude"
            pars.Add("p28id_exclude", intP28ID_Exclude, DbType.Int32)
        End If
        Return _cDB.GetRecord(Of BO.p28Contact)(s, pars)
    End Function
    Public Function LoadBySupplierID(strRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("supplierid", strRegID, DbType.String)
        s += " WHERE a.p28SupplierID LIKE @supplierid"
        If intP28ID_Exclude <> 0 Then
            s += " AND a.p28ID<>@p28id_exclude"
            pars.Add("p28id_exclude", intP28ID_Exclude, DbType.Int32)
        End If
        Return _cDB.GetRecord(Of BO.p28Contact)(s, pars)
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("externalpid", strExternalPID, DbType.String)
        s += " WHERE a.p28ExternalPID LIKE @externalpid"
        Return _cDB.GetRecord(Of BO.p28Contact)(s, pars)
    End Function
    Public Function LoadByPersonBirthRegID(strBirthRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("p28Person_BirthRegID", strBirthRegID, DbType.String)
        s += " WHERE a.p28Person_BirthRegID LIKE @p28Person_BirthRegID"
        If intP28ID_Exclude <> 0 Then
            s += " AND a.p28ID<>@p28id_exclude"
            pars.Add("p28id_exclude", intP28ID_Exclude, DbType.Int32)
        End If
        Return _cDB.GetRecord(Of BO.p28Contact)(s, pars)
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress As String) As BO.p28Contact
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p28RobotAddress LIKE @robotkey"

        Return _cDB.GetRecord(Of BO.p28Contact)(s, New With {.robotkey = strRobotAddress})
    End Function

    Public Function Save(cRec As BO.p28Contact, lisO37 As List(Of BO.o37Contact_Address), lisO32 As List(Of BO.o32Contact_Medium), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField), ByRef intLastSavedP28ID As Integer) As Boolean
        intLastSavedP28ID = 0
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p28ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                If .p28Code = "" Then .p28Code = "TEMP" & BO.BAS.GetGUID() 'dočasný kód, bude později nahrazen
                If .PID = 0 Then
                    pars.Add("p28IsDraft", .p28IsDraft, DbType.Boolean) 'info o draftu raději ukládat pouze při založení a poté už jenom pomocí workflow
                End If
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
                pars.Add("p29ID", BO.BAS.IsNullDBKey(.p29ID), DbType.Int32)
                pars.Add("p92ID", BO.BAS.IsNullDBKey(.p92ID), DbType.Int32)
                pars.Add("p87ID", BO.BAS.IsNullDBKey(.p87ID), DbType.Int32)
                pars.Add("p51ID_Billing", BO.BAS.IsNullDBKey(.p51ID_Billing), DbType.Int32)
                pars.Add("p51ID_Internal", BO.BAS.IsNullDBKey(.p51ID_Internal), DbType.Int32)
                pars.Add("p63ID", BO.BAS.IsNullDBKey(.p63ID), DbType.Int32)
                pars.Add("p28ParentID", BO.BAS.IsNullDBKey(.p28ParentID), DbType.Int32)
                pars.Add("j02ID_ContactPerson_DefaultInWorksheet", BO.BAS.IsNullDBKey(.j02ID_ContactPerson_DefaultInWorksheet), DbType.Int32)
                pars.Add("j02ID_ContactPerson_DefaultInInvoice", BO.BAS.IsNullDBKey(.j02ID_ContactPerson_DefaultInInvoice), DbType.Int32)
                pars.Add("j61ID_Invoice", BO.BAS.IsNullDBKey(.j61ID_Invoice), DbType.Int32)

                pars.Add("p28Code", .p28Code, DbType.String)
                pars.Add("p28SupplierFlag", .p28SupplierFlag, DbType.Int32)
                pars.Add("p28SupplierID", .p28SupplierID, DbType.String)
                pars.Add("p28IsCompany", .p28IsCompany, DbType.Boolean)
                pars.Add("p28FirstName", .p28FirstName, DbType.String, , , True, "Křestní jméno")
                pars.Add("p28LastName", .p28LastName, DbType.String, , , True, "Příjmení")
                pars.Add("p28TitleBeforeName", .p28TitleBeforeName, DbType.String, , , True, "Titul před jménem")
                pars.Add("p28TitleAfterName", .p28TitleAfterName, DbType.String, , , True, "Za jménem")
                pars.Add("p28RegID", .p28RegID, DbType.String, , , True, "IČ")
                pars.Add("p28VatID", .p28VatID, DbType.String, , , True, "DIČ")
                pars.Add("p28CompanyName", .p28CompanyName, DbType.String, , , True, "Společnost")
                pars.Add("p28CompanyShortName", .p28CompanyShortName, DbType.String, , , True, "Zkrácený název společnosti")
                pars.Add("p28RobotAddress", .p28RobotAddress, DbType.String)
                pars.Add("p28ExternalPID", .p28ExternalPID, DbType.String)
                pars.Add("p28BillingMemo", .p28BillingMemo, DbType.String, , , True, "Fakturační poznámka klienta")
                pars.Add("p28Pohoda_VatCode", .p28Pohoda_VatCode, DbType.String)

                pars.Add("p28InvoiceDefaultText1", .p28InvoiceDefaultText1, DbType.String, , , True, "Výchozí fakturační text")
                pars.Add("p28InvoiceDefaultText2", .p28InvoiceDefaultText2, DbType.String, , , True, "Výchozí doplňkový text faktury")
                pars.Add("p28InvoiceMaturityDays", .p28InvoiceMaturityDays, DbType.Int32)
                pars.Add("p28AvatarImage", .p28AvatarImage, DbType.String)
                pars.Add("p28LimitHours_Notification", .p28LimitHours_Notification, DbType.Decimal)
                pars.Add("p28LimitFee_Notification", .p28LimitFee_Notification, DbType.Decimal)
                pars.Add("p28validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p28validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p28Contact", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                intLastSavedP28ID = _cDB.LastSavedRecordPID

                If Not lisO37 Is Nothing Then   'kontaktní adresy
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM o37Contact_Address WHERE p28ID=" & intLastSavedP28ID.ToString)
                    Dim cDLO38 As New DL.o38AddressDL(_curUser)
                    For Each c In lisO37
                        c.SetPID(c.o38ID)
                        If c.IsSetAsDeleted Then
                            If c.PID <> 0 Then cDLO38.Delete(c.PID)
                        Else
                            If cDLO38.Save(c) Then
                                If Not _cDB.RunSQL("INSERT INTO o37Contact_Address(p28ID,o38ID,o36ID) VALUES(" & intLastSavedP28ID.ToString & "," & cDLO38.LastSavedRecordPID.ToString & "," & CInt(c.o36ID).ToString & ")") Then
                                    Return False
                                End If
                            Else
                                _Error = cDLO38.ErrorMessage : Return False
                            End If
                        End If
                    Next
                End If
                If Not lisO32 Is Nothing Then   'kontaktní média
                    Dim cDLO32 As New DL.o32Contact_MediumDL(_curUser)
                    For Each c In lisO32
                        c.p28ID = intLastSavedP28ID
                        If c.IsSetAsDeleted Then
                            If c.PID <> 0 Then cDLO32.Delete(c.PID)
                        Else
                            cDLO32.Save(c)
                        End If
                    Next
                End If
                If Not lisP30 Is Nothing Then   'kontaktní osoby
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p30Contact_Person WHERE p28ID=" & intLastSavedP28ID.ToString)
                    For Each c In lisP30.Where(Function(p) p.IsSetAsDeleted = False)
                        _cDB.RunSQL("INSERT INTO p30Contact_Person(p28ID,j02ID,p27ID) VALUES(" & intLastSavedP28ID.ToString & "," & c.j02ID.ToString & "," & IIf(c.p27ID <> 0, c.p27ID.ToString, "NULL") & ")")

                    Next
                End If
               
                If Not lisX69 Is Nothing Then   'přiřazení rolí klienta
                    bas.SaveX69(_cDB, BO.x29IdEnum.p28Contact, intLastSavedP28ID, lisX69, bolINSERT)
                End If
                If Not lisFF Is Nothing Then    'volná pole
                    bas.SaveFreeFields(_cDB, lisFF, "p28Contact_FreeField", intLastSavedP28ID)
                End If

                pars = New DbParameters
                With pars
                    .Add("p28id", intLastSavedP28ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                End With
                If _cDB.RunSP("p28_aftersave", pars) Then
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

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p28_delete", pars)
    End Function

    Public Function GetList_o37(intPID As Integer) As IEnumerable(Of BO.o37Contact_Address)
        Dim s As String = "select a.p28ID,a.o36ID,o38.*,o36.o36Name as _o36Name," & bas.RecTail("o38", "o38")
        s += " FROM o37Contact_Address a INNER JOIN o38Address o38 ON a.o38ID=o38.o38ID INNER JOIN o36AddressType o36 ON a.o36ID=o36.o36ID"
        s += " WHERE a.p28ID=@pid"

        Return _cDB.GetList(Of BO.o37Contact_Address)(s, New With {.pid = intPID})
    End Function
    Public Function GetList_o32(intPID As Integer) As IEnumerable(Of BO.o32Contact_Medium)
        Dim s As String = "select a.*,o33.o33Name as _o33Name," & bas.RecTail("o32", "a", False, True)
        s += " FROM o32Contact_Medium a INNER JOIN o33MediumType o33 ON a.o33ID=o33.o33ID"
        s += " WHERE a.p28ID=@pid"

        Return _cDB.GetList(Of BO.o32Contact_Medium)(s, New With {.pid = intPID})
    End Function
    

   

    Private Function GetSQLWHERE(myQuery As BO.myQueryP28, ByRef pars As DL.DbParameters) As String
        Dim s As New System.Text.StringBuilder
        s.Append(bas.ParseWhereMultiPIDs("a.p28ID", myQuery))
        s.Append(bas.ParseWhereValidity("p28", "a", myQuery))
        With myQuery
            If .p29ID <> 0 Then
                pars.Add("p29id", .p29ID, DbType.Int32)
                s.Append(" AND a.p29ID=@p29id")
            End If
            If .j02ID <> 0 Then
                pars.Add("j02id", .j02ID, DbType.Int32)
                s.Append(" AND a.p28ID IN (select p28ID FROM p30Contact_Person WHERE j02ID=@j02id)")
            End If
            If .b02ID <> 0 Then
                pars.Add("b02id", .b02ID, DbType.Int32)
                s.Append(" AND a.b02ID=@b02id")
            End If
            If .p28ParentID <> 0 Then
                pars.Add("parentpid", .p28ParentID, DbType.Int32)
                s.Append(" AND a.p28ParentID=@parentpid")
            End If
            If .TreeIndexFrom > 0 Or .TreeIndexUntil > 0 Then
                pars.Add("treeprev", .TreeIndexFrom, DbType.Int32)
                pars.Add("treenext", .TreeIndexUntil, DbType.Int32)
                s.Append(" AND a.p28TreeIndex>=@treeprev AND a.p28TreeNext<=@treenext")
            End If
            Select Case .p28TreeLevel
                Case -1
                Case 0
                    s.Append(" AND (a.p28TreeLevel=0 OR a.p28TreeLevel IS NULL)")
                Case Is > 0
                    pars.Add("treelevel", .p28TreeLevel, DbType.Int32)
                    s.Append(" AND a.p28TreeLevel=@treelevel")
            End Select
            If Not .DateInsertFrom Is Nothing Then
                pars.Add("d1", .DateInsertFrom)
                pars.Add("d2", .DateInsertUntil)
                s.Append(" AND a.p28DateInsert BETWEEN @d1 AND @d2")
            End If
            Dim bolQD As Boolean = False
            If Not .p31Date_D1 Is Nothing Then
                If Year(.p31Date_D1) > 1900 Then
                    bolQD = True
                    pars.Add("dp31f1", .p31Date_D1)
                    pars.Add("dp31f2", .p31Date_D2)
                    s.Append(" AND a.p28ID IN (SELECT gb.p28ID_Client FROM p31Worksheet ga INNER JOIN p41Project gb ON ga.p41ID=gb.p41ID WHERE gb.p28ID_Client IS NOT NULL AND ga.p31Date BETWEEN @dp31f1 AND @dp31f2)")
                End If
            End If
            If Not bolQD Then
                pars.Add("dp31f1", DateSerial(2000, 1, 1))
                pars.Add("dp31f2", DateSerial(3000, 1, 1))
            End If
            If .QuickQuery > BO.myQueryP28_QuickQuery._NotSpecified Then
                s.Append(" AND " & bas.GetQuickQuerySQL_p28(.QuickQuery))
            End If
            If .CanBeClient = BO.BooleanQueryMode.TrueQuery Then s.Append(" AND a.p28SupplierFlag IN (1,3)")
            If .CanBeClient = BO.BooleanQueryMode.FalseQuery Then s.Append(" AND a.p28SupplierFlag=2")
            If .CanBeSupplier = BO.BooleanQueryMode.TrueQuery Then s.Append(" AND a.p28SupplierFlag IN (2,3)")
            If .CanBeSupplier = BO.BooleanQueryMode.FalseQuery Then s.Append(" AND a.p28SupplierFlag=1")

            Select Case .SpecificQuery
                Case BO.myQueryP28_SpecificQuery.AllowedForRead
                    If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P28_Owner) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P28_Reader) Then
                        'právo paušálně číst všechny kontakty - není třeba skládat podmínku
                    Else
                        Dim strJ11IDs As String = ""
                        If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"

                        s.Append(" AND (a.j02ID_Owner=@j02id_query OR a.p28ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                        s.Append(" WHERE x67.x29ID=328 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append("))")
                    End If
            End Select
            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    s.Append(" AND " & strQueryW)
                End If
            End If
            If .p51ID <> 0 Then
                pars.Add("p51id", .p51ID, DbType.Int32)
                s.Append(" AND (a.p51ID_Billing=@p51id)")
            End If
            If .SpecificQuery > BO.myQueryP28_QuickQuery._NotSpecified Then
                If .j02ID_ExplicitQueryFor > 0 Then
                    pars.Add("j02id_query", .j02ID_ExplicitQueryFor, DbType.Int32)
                Else
                    pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                End If
            End If
            If .ColumnFilteringExpression <> "" Then
                s.Append(" AND " & .ColumnFilteringExpression)
            End If
            If .SearchExpression <> "" Then
                s.Append(" AND (")
                If Len(.SearchExpression) <= 1 Then
                    'hledat pouze podle počátečních písmen
                    s.Append("a.p28Name LIKE @expr+'%' OR a.p28Code LIKE '%'+@expr+'%' OR a.p28CompanyShortName LIKE @expr+'%' OR a.p28CompanyName LIKE @expr+'%'")
                    s.Append(" OR a.p28ID IN (select p30.p28ID FROM p30Contact_Person p30 INNER JOIN j02Person j02 ON p30.j02ID=j02.j02ID WHERE j02LastName LIKE @expr+'%')")
                Else
                    'něco jako fulltext
                    s.Append("a.p28Name LIKE '%'+@expr+'%' OR a.p28CompanyShortName LIKE '%'+@expr+'%' OR a.p28CompanyName LIKE '%'+@expr+'%'")
                    If Len(.SearchExpression) >= 2 Then
                        s.Append(" OR a.p28Code LIKE '%'+@expr+'%' OR a.p28RegID LIKE @expr+'%' OR a.p28VatID LIKE @expr+'%'")
                    End If
                    s.Append(" OR a.p28ID IN (select p30.p28ID FROM p30Contact_Person p30 INNER JOIN j02Person j02 ON p30.j02ID=j02.j02ID WHERE j02LastName LIKE '%'+@expr+'%')")
                End If
                s.Append(")")
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then s.Append(" AND a.p28ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=328 AND o51ID IN (" & String.Join(",", .o51IDs) & "))")
            End If
            If .x18Value <> "" Then
                s.Append(bas.CompleteX18QuerySql("p28", .x18Value))
            End If

        End With
        Return bas.TrimWHERE(s.ToString)
    End Function

    Public Function GetList(myQuery As BO.myQueryP28) As IEnumerable(Of BO.p28Contact)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly) & " " & GetSQLPart2(myQuery)
        If myQuery.MG_SelectPidFieldOnly Then
            'SQL SELECT klauzule bude plnit pouze hodnotu primárního klíče
            s = "SELECT a.p28ID as _pid " & GetSQLPart2(myQuery)
        End If
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then strSort = "a.p28Name"

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
        Return _cDB.GetList(Of BO.p28Contact)(s, pars)
    End Function

    Public Function GetGridDataSource(myQuery As BO.myQueryP28) As DataTable
        Dim s As New System.Text.StringBuilder
        With myQuery
            If Not System.String.IsNullOrEmpty(.MG_GridGroupByField) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 Then
                    Select Case .MG_GridGroupByField
                        Case "Owner" : .MG_GridSqlColumns += ",j02owner.j02LastName+char(32)+j02owner.j02FirstName as Owner"
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If
            .MG_GridSqlColumns += ",a.p28ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.p28ValidFrom AND a.p28ValidUntil THEN 0 else 1 END) as IsClosed,a.p28IsDraft as IsDraft,a.p28SupplierFlag as SupplierFlag,a.p28ParentID as ParentID"
        End With
       
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.p28ID as pid"
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
            If strORDERBY = "" Then strORDERBY = "a.p28Name"

            If .MG_PageSize > 0 Then
                Dim intStart As Integer = (.MG_CurrentPageIndex) * .MG_PageSize

                s.Append("WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & .MG_GridSqlColumns & " " & GetSQLPart2(myQuery))
                If strW <> "" Then s.Append(" WHERE " & strW)

                s.Append(") SELECT * FROM rst")
                pars.Add("start", intStart, DbType.Int32)
                pars.Add("end", (intStart + .MG_PageSize - 1), DbType.Int32)
                s.Append(" WHERE RowIndex BETWEEN @start AND @end")
            Else
                'bez stránkování
                s.Append("SELECT " & .MG_GridSqlColumns & " " & GetSQLPart2(myQuery))
                If strW <> "" Then s.Append(" WHERE " & strW)
                s.Append(" ORDER BY " & strORDERBY)
            End If

        End With

        Dim ds As DataSet = _cDB.GetDataSet(s.ToString, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function

    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "p28UserInsert").Replace("UserUpdate", "p28UserUpdate").Replace("DateInsert", "p28DateInsert").Replace("DateUpdate", "p28DateUpdate")
        strSort = strSort.Replace("Owner", "j02owner.j02LastName").Replace("p51Name_Billing", "p51billing.p51Name").Replace("p51Name_Internal", "p51internal.p51Name")
        Return bas.NormalizeOrderByClause(strSort)
    End Function
    Private Function ParseFilterExpression(strFilter As String) As String
        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function
    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize
        
        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & GetSF() & " " & GetSQLPart2(Nothing)

        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " * FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"
        's += " ORDER BY " & strORDERBY
        Return s
    End Function

    Public Function GetVirtualCount(myQuery As BO.myQueryP28) As Integer
        Dim s As String = "SELECT count(a.p28ID) as Value " & GetSQLPart2(myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP28, strSumFields As String) As DataTable
        Dim s As String = "SELECT count(a.p28ID) as VirtualCount"
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

    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " " & GetSF()
        Return s
    End Function
    Private Function GetSF() As String
        Dim s As New System.Text.StringBuilder
        s.Append("a.p29ID,a.p92ID,a.j02ID_Owner,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.b02ID,a.p63ID,a.p28IsCompany,a.p28IsDraft,a.p28Code,a.p28FirstName,a.p28LastName,a.p28TitleBeforeName,a.p28TitleAfterName,a.p28RegID,a.p28VatID,a.p28Person_BirthRegID,a.p28CompanyName,a.p28CompanyShortName,a.p28InvoiceDefaultText1,a.p28InvoiceDefaultText2,a.p28InvoiceMaturityDays,a.p28LimitHours_Notification,a.p28LimitFee_Notification,a.p28AvatarImage")
        s.Append(",a.p28Name as _p28name,p29.p29Name as _p29Name,p92.p92Name as _p92Name,b02.b02Name as _b02Name,p87.p87Name as _p87Name,a.p28RobotAddress,a.p28SupplierID,a.p28SupplierFlag,a.p28ExternalPID")
        s.Append(",a.p28TreeLevel as _p28TreeLevel,a.p28TreeIndex as _p28TreeIndex,a.p28TreePrev as _p28TreePrev,a.p28TreeNext as _p28TreeNext,a.p28TreePath as _p28TreePath")
        s.Append(",p51billing.p51Name as _p51Name_Billing,p51internal.p51Name as _p51Name_Internal,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,a.p28ParentID,a.p28BillingMemo,a.p28Pohoda_VatCode,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice,a.j61ID_Invoice," & bas.RecTail("p28", "a"))
        Return s.ToString
    End Function
    Private Function GetSQLPart2(mq As BO.myQueryP28) As String
        Dim s As New System.Text.StringBuilder
        s.Append("FROM p28Contact a LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID")
        s.Append(" LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID")
        s.Append(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID")
        s.Append(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID")
        s.Append(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID")
        s.Append(" LEFT OUTER JOIN p51PriceList p51internal ON a.p51ID_Internal=p51internal.p51ID")
        s.Append(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID")
        s.Append(" LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID")
        ''s += " LEFT OUTER JOIN (SELECT p28ID as ParentID,p28Name as ParentContact FROM p28Contact) p28parent ON a.p28ParentID=p28parent.ParentID"
        If Not mq Is Nothing Then
            If mq.MG_AdditionalSqlFROM <> "" Then s.Append(" " & mq.MG_AdditionalSqlFROM)
        End If
        Return s.ToString
    End Function

    Public Sub UpdateSelectedRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP28ID As Integer)
        bas.SaveX69(_cDB, BO.x29IdEnum.p28Contact, intP28ID, lisX69, False, intX67ID)
    End Sub
    Public Sub ClearSelectedRole(intX67ID As Integer, intP28ID As Integer)
        _cDB.RunSQL("DELETE FROM x69EntityRole_Assign WHERE x67ID=" & intX67ID.ToString & " AND x69RecordPID=" & intP28ID.ToString)
    End Sub
    Public Function ConvertFromDraft(intPID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p28id", intPID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p28_convertdraft", pars)

    End Function

    Public Function HasChildRecords(intPID As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intPID, DbType.Int32)
        If _cDB.GetIntegerValueFROMSQL("if exists(select p28ID FROM p28Contact WHERE p28ParentID=@pid) select 1 as Value else select 0 as Value", pars) = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function LoadSumRow(intPID As Integer) As BO.p28ContactSum
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
        End With
        Return _cDB.GetRecord(Of BO.p28ContactSum)("p28_inhale_sumrow", pars, True)
    End Function
    Public Function AppendOrRemove_IsirMoniting(intP28ID As Integer, bolRemove As Boolean) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intP28ID, DbType.Int32)
            If bolRemove Then
                .Add("append_remove_flag", 2, DbType.Int32)
            Else
                .Add("append_remove_flag", 1, DbType.Int32)
            End If
        End With
        Return _cDB.RunSP("p28_append_remove_isir", pars)
    End Function
    Public Function LoadO48Record(intP28ID As Integer) As BO.o48IsirMonitoring
        Return _cDB.GetRecord(Of BO.o48IsirMonitoring)("select *," & bas.RecTail("o48", "") & " from o48IsirMonitoring where p28ID=@p28id", New With {.p28id = intP28ID})
    End Function
    Public Function GetRolesInline(intPID As Integer) As String
        Return _cDB.GetValueFromSQL("SELECT dbo.p28_getroles_inline(" & intPID.ToString & ") as Value")
    End Function
End Class
