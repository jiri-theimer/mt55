Public Class p41ProjectDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p41Project
        Dim s As String = GetSQLPart1(0) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p41ID=@p41id"

        Return _cDB.GetRecord(Of BO.p41Project)(s, New With {.p41id = intPID})
    End Function
    Public Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p41Project
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p41TreeIndex<=@curindex AND a.p41TreeLevel=0 ORDER BY a.p41TreeIndex DESC"

        Return _cDB.GetRecord(Of BO.p41Project)(s, New With {.curindex = intCurTreeIndex})
    End Function
    Public Function LoadMyLastCreated() As BO.p41Project
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p41UserInsert=@mylogin ORDER BY a.p41ID DESC"

        Return _cDB.GetRecord(Of BO.p41Project)(s, New With {.mylogin = _curUser.j03Login})
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress As String) As BO.p41Project
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p41RobotAddress LIKE @robotkey"

        Return _cDB.GetRecord(Of BO.p41Project)(s, New With {.robotkey = strRobotAddress})
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p41Project
        Dim s As String = GetSQLPart1(1) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p41ExternalPID LIKE @externalpid"
        Return _cDB.GetRecord(Of BO.p41Project)(s, New With {.externalpid = strExternalPID})
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.p41ProjectSum
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
        End With
        Return _cDB.GetRecord(Of BO.p41ProjectSum)("p41_inhale_sumrow", pars, True)
    End Function
    
    Public Function Save(cRec As BO.p41Project, lisO39 As List(Of BO.o39Project_Address), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField), ByRef intLastSavedP41ID As Integer) As Boolean
        intLastSavedP41ID = 0
        
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p41ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                If .p41Code = "" Then .p41Code = "TEMP" & BO.BAS.GetGUID() 'dočasný kód, bude později nahrazen
                If .PID = 0 Then
                    pars.Add("p41IsDraft", .p41IsDraft, DbType.Boolean) 'příznak draftu raději ukládat pouze při založení a poté už jenom pomocí workflow
                End If
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
                pars.Add("p28ID_Client", BO.BAS.IsNullDBKey(.p28ID_Client), DbType.Int32)
                pars.Add("p28ID_Billing", BO.BAS.IsNullDBKey(.p28ID_Billing), DbType.Int32)
                pars.Add("p41ParentID", BO.BAS.IsNullDBKey(.p41ParentID), DbType.Int32)

                pars.Add("p42ID", BO.BAS.IsNullDBKey(.p42ID), DbType.Int32)
                pars.Add("p92ID", BO.BAS.IsNullDBKey(.p92ID), DbType.Int32)
                pars.Add("p87ID", BO.BAS.IsNullDBKey(.p87ID), DbType.Int32)
                pars.Add("p51ID_Billing", BO.BAS.IsNullDBKey(.p51ID_Billing), DbType.Int32)
                pars.Add("p51ID_Internal", BO.BAS.IsNullDBKey(.p51ID_Internal), DbType.Int32)
                pars.Add("j18ID", BO.BAS.IsNullDBKey(.j18ID), DbType.Int32)
                pars.Add("p61ID", BO.BAS.IsNullDBKey(.p61ID), DbType.Int32)
                pars.Add("p72ID_NonBillable", BO.BAS.IsNullDBKey(.p72ID_NonBillable), DbType.Int32)
                pars.Add("p72ID_BillableHours", BO.BAS.IsNullDBKey(.p72ID_BillableHours), DbType.Int32)
                pars.Add("j02ID_ContactPerson_DefaultInWorksheet", BO.BAS.IsNullDBKey(.j02ID_ContactPerson_DefaultInWorksheet), DbType.Int32)
                pars.Add("j02ID_ContactPerson_DefaultInInvoice", BO.BAS.IsNullDBKey(.j02ID_ContactPerson_DefaultInInvoice), DbType.Int32)
                pars.Add("p65ID", BO.BAS.IsNullDBKey(.p65ID), DbType.Int32)
                pars.Add("p41RecurNameMask", .p41RecurNameMask, DbType.String)
                pars.Add("p41RecurBaseDate", .p41RecurBaseDate, DbType.DateTime)
                pars.Add("p41RecurMotherID", BO.BAS.IsNullDBKey(.p41RecurMotherID), DbType.Int32)

                pars.Add("p41Code", .p41Code, DbType.String)
                pars.Add("p41Name", .p41Name, DbType.String, , , True, "Název projektu")
                pars.Add("p41NameShort", .p41NameShort, DbType.String, , , True, "Zkrácený název projektu")
                pars.Add("p41RobotAddress", .p41RobotAddress, DbType.String)
                pars.Add("p41ExternalPID", .p41ExternalPID, DbType.String)
                pars.Add("p41BillingMemo", .p41BillingMemo, DbType.String, , , True, "Fakturační poznámka projektu")

                pars.Add("p41InvoiceDefaultText1", .p41InvoiceDefaultText1, DbType.String, , , True, "Výchozí fakturační text")
                pars.Add("p41InvoiceDefaultText2", .p41InvoiceDefaultText2, DbType.String, , , True, "Výchozí doplňkový text faktury")
                pars.Add("p41InvoiceMaturityDays", .p41InvoiceMaturityDays, DbType.Int32)

                pars.Add("p41WorksheetOperFlag", .p41WorksheetOperFlag, DbType.Int32)
                pars.Add("p41LimitHours_Notification", .p41LimitHours_Notification, DbType.Decimal)
                pars.Add("p41LimitFee_Notification", .p41LimitFee_Notification, DbType.Decimal)

                pars.Add("p41PlanFrom", BO.BAS.IsNullDBDate(.p41PlanFrom), DbType.DateTime)
                pars.Add("p41PlanUntil", BO.BAS.IsNullDBDate(.p41PlanUntil), DbType.DateTime)
                pars.Add("p41IsNoNotify", .p41IsNoNotify, DbType.Boolean)
                pars.Add("p41IsStopRecurrence", .p41IsStopRecurrence, DbType.Boolean)

                pars.Add("p41validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p41validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p41Project", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                intLastSavedP41ID = _cDB.LastSavedRecordPID
                If Not lisO39 Is Nothing Then   'kontaktní adresy
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM o39Project_Address WHERE p41ID=" & intLastSavedP41ID.ToString)
                    Dim cDLO38 As New DL.o38AddressDL(_curUser)
                    For Each c In lisO39
                        c.SetPID(c.o38ID)
                        If c.IsSetAsDeleted Then
                            If c.PID <> 0 Then cDLO38.Delete(c.PID)
                        Else
                            If cDLO38.Save(c) Then
                                _cDB.RunSQL("INSERT INTO o39Project_Address(p41ID,o38ID,o36ID) VALUES(" & intLastSavedP41ID.ToString & "," & cDLO38.LastSavedRecordPID.ToString & "," & CInt(c.o36ID).ToString & ")")
                            End If
                        End If
                    Next
                End If

                If Not lisP30 Is Nothing Then   'kontaktní osoby
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p30Contact_Person WHERE p41ID=" & intLastSavedP41ID.ToString)
                    For Each c In lisP30.Where(Function(p) p.IsSetAsDeleted = False)
                        _cDB.RunSQL("INSERT INTO p30Contact_Person(p41ID,j02ID,p27ID) VALUES(" & intLastSavedP41ID.ToString & "," & c.j02ID.ToString & "," & IIf(c.p27ID <> 0, c.p27ID.ToString, "NULL") & ")")
                    Next
                End If
                If Not lisX69 Is Nothing Then   'přiřazení rolí projektu
                    bas.SaveX69(_cDB, BO.x29IdEnum.p41Project, intLastSavedP41ID, lisX69, bolINSERT)

                End If
                If Not lisFF Is Nothing Then    'volná pole
                    bas.SaveFreeFields(_cDB, lisFF, "p41Project_FreeField", intLastSavedP41ID, _curUser)
                End If

                pars = New DbParameters
                With pars
                    .Add("p41id", intLastSavedP41ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                End With
                If _cDB.RunSP("p41_aftersave", pars) Then
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

    Public Sub UpdateSelectedProjectRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP41ID As Integer)
        bas.SaveX69(_cDB, BO.x29IdEnum.p41Project, intP41ID, lisX69, False, intX67ID)
    End Sub
    Public Sub ClearSelectedProjectRole(intX67ID As Integer, intP41ID As Integer)
        _cDB.RunSQL("DELETE FROM x69EntityRole_Assign WHERE x67ID=" & intX67ID.ToString & " AND x69RecordPID=" & intP41ID.ToString)
    End Sub


    Public Function SaveCapacityPlan(intPID As Integer, lisP47 As List(Of BO.p47CapacityPlan)) As Boolean
        'Dim lisCur As IEnumerable(Of BO.p47CapacityPlan)
        Return True
    End Function


    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p41_delete", pars)
    End Function

    Public Function GetList(myQuery As BO.myQueryP41) As IEnumerable(Of BO.p41Project)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly) & " " & GetSQLPart2(myQuery)
        If myQuery.MG_SelectPidFieldOnly Then
            'SQL SELECT klauzule bude plnit pouze hodnotu primárního klíče
            s = "SELECT a.p41ID as _pid " & GetSQLPart2(myQuery)
        End If
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then strSort = "p28client.p28Name,a.p41Name"

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

        Return _cDB.GetList(Of BO.p41Project)(s, pars)
    End Function

    Public Function GetGridDataSource(myQuery As BO.myQueryP41) As DataTable
        Dim s As String = ""
        With myQuery
            If Not System.String.IsNullOrEmpty(.MG_GridGroupByField) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 Then
                    Select Case .MG_GridGroupByField
                        Case "Client" : .MG_GridSqlColumns += ",p28client.p28Name as Client"
                        Case "Owner" : .MG_GridSqlColumns += ",j02owner.j02LastName+char(32)+j02owner.j02FirstName as Owner"
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If
            .MG_GridSqlColumns += ",a.p41ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.p41ValidFrom AND a.p41ValidUntil THEN 0 else 1 END) as IsClosed,a.p41IsDraft as IsDraft,j13.j13ID,a.p41TreeLevel as TreeLevel,a.p65ID"
            .MG_AdditionalSqlFROM += " LEFT OUTER JOIN (SELECT j13ID,p41ID FROM j13FavourteProject WHERE j03ID=" & _curUser.PID.ToString & ") j13 ON a.p41ID=j13.p41ID"
        End With

        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.p41ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If .MG_GridGroupByField <> "" Then
                Dim strPrimarySortField As String = .MG_GridGroupByField
                If strPrimarySortField = "Client" Then strPrimarySortField = "p28client.p28Name"
                If strPrimarySortField = "Owner" Then strPrimarySortField = "j02owner.j02LastName+char(32)+j02owner.j02FirstName"
                If strORDERBY = "" Or LCase(strPrimarySortField) = Replace(Replace(LCase(.MG_SortString), " desc", ""), " asc", "") Then
                    strORDERBY = strPrimarySortField
                Else
                    strORDERBY = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            If strORDERBY = "" Then strORDERBY = "p28client.p28Name,a.p41Name"
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



    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "p41UserInsert").Replace("UserUpdate", "p41UserUpdate").Replace("DateInsert", "p41DateInsert").Replace("DateUpdate", "p41DateUpdate")
        strSort = strSort.Replace("Client", "p28client.p28Name").Replace("Owner", "j02owner.j02LastName").Replace("p51Name_Billing", "p51billing.p51Name").Replace("FullName", "p28client.p28Name,p41Name").Replace("p51Name_Internal", "p51internal.p51Name")
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

        Return s
    End Function

    Public Function GetList_o39(intPID As Integer) As IEnumerable(Of BO.o39Project_Address)
        Dim s As String = "select a.p41ID,a.o36ID,o38.*," & bas.RecTail("o38", "o38")
        s += " FROM o39Project_Address a INNER JOIN o38Address o38 ON a.o38ID=o38.o38ID"
        s += " WHERE a.p41ID=@pid"

        Return _cDB.GetList(Of BO.o39Project_Address)(s, New With {.pid = intPID})
    End Function


   

    Private Function GetSQLWHERE(myQuery As BO.myQueryP41, ByRef pars As DL.DbParameters) As String
        Dim s As New System.Text.StringBuilder
        s.Append(bas.ParseWhereMultiPIDs("a.p41ID", myQuery))
        s.Append(bas.ParseWhereValidity("p41", "a", myQuery))
        With myQuery
            If .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForOperPlanEntry Then
                .SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
                s.Append(" AND p42.p42IsModule_p48=1 and getdate() between a.p41ValidFrom AND a.p41ValidUntil")
            End If
            If .p42ID <> 0 Then
                pars.Add("p42id", .p42ID, DbType.Int32)
                s.Append(" AND a.p42ID=@p42id")
            End If
            If .p51ID <> 0 Then
                pars.Add("p51id", .p51ID, DbType.Int32)
                s.Append(" AND (a.p51ID_Billing=@p51id OR a.p51ID_Internal=@p51id)")
            End If
            If .j02ID_Owner <> 0 Then
                pars.Add("ownerid", .j02ID_Owner, DbType.Int32)
                s.Append(" AND a.j02ID_Owner=@ownerid")
            End If
            If .j02ID_ContactPerson > 0 Then
                pars.Add("@j02id_contactperson", .j02ID_ContactPerson, DbType.Int32)
                s.Append(" AND (a.p41ID IN (SELECT p41ID FROM p30Contact_Person WHERE J02ID=@j02id_contactperson AND p41ID IS NOT NULL) OR a.p28ID_Client IN (SELECT p28ID FROM p30Contact_Person WHERE J02ID=@j02id_contactperson AND p28ID IS NOT NULL))")
            End If
            If .IsFavourite > BO.BooleanQueryMode.NoQuery Then
                pars.Add("j03id_sys", _curUser.PID, DbType.Int32)
                s.Append(" AND a.p41ID IN (SELECT p41ID FROM j13FavourteProject WHERE j03ID=@j03id_sys)")
            End If
            If .p41ParentID <> 0 Then
                pars.Add("parentpid", .p41ParentID, DbType.Int32)
                s.Append(" AND a.p41ParentID=@parentpid")
            End If
            If .TreeIndexFrom > 0 Or .TreeIndexUntil > 0 Then
                pars.Add("treeprev", .TreeIndexFrom, DbType.Int32)
                pars.Add("treenext", .TreeIndexUntil, DbType.Int32)
                s.Append(" AND a.p41TreeIndex>=@treeprev AND a.p41TreeNext<=@treenext")
            End If
            Select Case .p41TreeLevel
                Case -1
                Case 0
                    s.Append(" AND (a.p41TreeLevel=0 OR a.p41TreeLevel IS NULL)")
                Case Is > 0
                    pars.Add("treelevel", .p41TreeLevel, DbType.Int32)
                    s.Append(" AND a.p41TreeLevel=@treelevel")
            End Select
            If .b02ID <> 0 Then
                pars.Add("b02id", .b02ID, DbType.Int32)
                s.Append(" AND a.b02ID=@b02id")
            End If
            If .j18ID <> 0 Then
                pars.Add("j18id", .j18ID, DbType.Int32)
                s.Append(" AND a.j18ID=@j18id")
            End If
            If .p28ID > 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                s.Append(" AND (a.p28ID_Client=@p28id OR a.p28ID_Billing=@p28id)")
            End If
            If Not .p28IDs Is Nothing Then
                s.Append(" AND a.p28ID_Client IN (" & String.Join(",", .p28IDs) & ")")
            End If
            If .p61ID > 0 Then
                pars.Add("p61id", .b02ID, DbType.Int32)
                s.Append(" AND a.p61ID=@p61id")
            End If
            If .p91ID > 0 Then
                pars.Add("p91id", .p91ID, DbType.Int32)
                s.Append(" AND a.p41ID IN (SELECT p41ID FROM p31Worksheet WHERE p91ID=@p91id)")
            End If
            If Not .DateInsertFrom Is Nothing Then
                pars.Add("d1", .DateInsertFrom)
                pars.Add("d2", .DateInsertUntil)
                s.Append(" AND a.p41DateInsert BETWEEN @d1 AND @d2")
            End If
            If Not .p41PlanFrom_D1 Is Nothing Then
                If Year(.p41PlanFrom_D1) > 1900 Then
                    pars.Add("dpf1", .p41PlanFrom_D1)
                    pars.Add("dpf2", .p41PlanFrom_D2)
                    s.Append(" AND a.p41PlanFrom BETWEEN @dpf1 AND @dpf2")
                End If
            End If
            If Not .p41PlanUntil_D1 Is Nothing Then
                If Year(.p41PlanUntil_D1) > 1900 Then
                    pars.Add("dpu1", .p41PlanUntil_D1)
                    pars.Add("dpu2", .p41PlanUntil_D2)
                    s.Append(" AND a.p41PlanUntil BETWEEN @dpu1 AND @dpu2")
                End If
            End If
            If .p41ExternalPID <> "" Then
                pars.Add("p41ExternalPID", .p41ExternalPID, DbType.String)
                s.Append(" AND a.p41ExternalPID LIKE @p41ExternalPID")
            End If
            Dim bolQD As Boolean = False
            If Not .p31Date_D1 Is Nothing Then
                If Year(.p31Date_D1) > 1900 Then
                    bolQD = True
                    pars.Add("dp31f1", .p31Date_D1)
                    pars.Add("dp31f2", .p31Date_D2)
                    s.Append(" AND a.p41ID IN (SELECT p41ID FROM p31Worksheet WHERE p31Date BETWEEN @dp31f1 AND @dp31f2)")
                End If
            End If
            If Not bolQD Then
                pars.Add("dp31f1", DateSerial(2000, 1, 1))
                pars.Add("dp31f2", DateSerial(3000, 1, 1))
            End If
            If .p41WorksheetOperFlag > BO.p41WorksheetOperFlagEnum._NotSpecified Then
                pars.Add("p41WorksheetOperFlag", .p41WorksheetOperFlag, DbType.Int32)
                s.Append(" AND a.p41WorksheetOperFlag=@p41WorksheetOperFlag")
            End If
            If .SpecificQuery > BO.myQueryP41_SpecificQuery._NotSpecified Then
                If .j02ID_ExplicitQueryFor > 0 Then
                    pars.Add("j02id_query", .j02ID_ExplicitQueryFor, DbType.Int32)
                Else
                    pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                End If
            End If

            Dim strJ11IDs As String = ""
            If .j02ID_ExplicitQueryFor = _curUser.j02ID Or .j02ID_ExplicitQueryFor = 0 Then
                If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
            Else
                strJ11IDs = "OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=" & .j02ID_ExplicitQueryFor.ToString & ")"
            End If

            Select Case .SpecificQuery
                Case BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
                    s.Append(" AND a.p41IsDraft=0 AND a.p41WorksheetOperFlag>1 AND p42.p42IsModule_p31=1 AND getdate() BETWEEN a.p41ValidFrom AND a.p41ValidUntil")
                    If Not BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Creator) Then
                        s.Append(" AND (a.p41ID IN (")

                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN o28ProjectRole_Workload o28 ON x69.x67ID=o28.x67ID INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                        s.Append(" WHERE x67.x29ID=141 AND o28.o28EntryFlag IN (1,2,4) AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append(") OR a.j18ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN o28ProjectRole_Workload o28 ON x69.x67ID=o28.x67ID INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                        s.Append(" WHERE x67.x29ID=118 AND o28.o28EntryFlag IN (1,2,4) AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append("))")
                    End If
                    
                Case BO.myQueryP41_SpecificQuery.AllowedForRead
                    If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P41_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P41_Owner) Then
                        'právo paušálně číst všechny projekty - není třeba skládat podmínku
                    Else
                        s.Append(" AND (a.j02ID_Owner=@j02id_query OR a.p41ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                        s.Append(" WHERE x67.x29ID=141 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append(") OR a.j18ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                        s.Append(" WHERE x67.x29ID=118 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append("))")
                    End If
                Case BO.myQueryP41_SpecificQuery.AllowedForCreateTask
                    'právo zakládat úkoly
                    If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P56_Creator) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P41_Owner) Then
                        'právo zakládat úkoly ve všech projektech
                    Else
                        s.Append(" AND (a.j02ID_Owner=@j02id_query OR a.p41ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID")
                        s.Append(" WHERE x67.x29ID=141 AND x68.x53ID=7 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append(") OR a.j18ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID")
                        s.Append(" WHERE x67.x29ID=118 AND x68.x53ID=7 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append("))")
                    End If
                Case BO.myQueryP41_SpecificQuery.AllowedForCreateInvoice
                    'právo vytvářet v projektu faktury
                    If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator)) Then
                        s.Append(" AND (a.j02ID_Owner=@j02id_query OR a.p41ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID")
                        s.Append(" WHERE x67.x29ID=141 AND x68.x53ID=21 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append(") OR a.j18ID IN (")
                        s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID")
                        s.Append(" WHERE x67.x29ID=118 AND x68.x53ID=21 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                        s.Append("))")
                    End If
                Case BO.myQueryP41_SpecificQuery.AllowedForApproving
                    s.Append(" AND (a.p41ID IN (")
                    s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN o28ProjectRole_Workload o28 ON x69.x67ID=o28.x67ID INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                    s.Append(" WHERE x67.x29ID=141 AND o28.o28PermFlag IN (3,4) AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                    s.Append(") OR a.j18ID IN (")
                    s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN o28ProjectRole_Workload o28 ON x69.x67ID=o28.x67ID INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                    s.Append(" WHERE x67.x29ID=118 AND o28.o28PermFlag IN (3,4) AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                    s.Append("))")
            End Select
            If .QuickQuery > BO.myQueryP41_QuickQuery._NotSpecified Then
                s.Append(" AND " & bas.GetQuickQuerySQL_p41(.QuickQuery, _curUser))
            End If

            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    s.Append(" AND " & strQueryW)
                End If
            End If

            If .x67ID_ProjectRole > 0 Then
                s.Append(" AND a.p41ID IN (SELECT x69RecordPID FROM x69EntityRole_Assign WHERE x67ID=@x67id AND (j02ID=@j02id_query OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))")
                pars.Add("x67id", .x67ID_ProjectRole, DbType.Int32)
            End If
            If .ColumnFilteringExpression <> "" Then
                's.Append(" AND " & ParseFilterExpression(.ColumnFilteringExpression))
                s.Append(" AND " & .ColumnFilteringExpression)
            End If
            If .SearchExpression <> "" Then
                s.Append(" AND (")
                If Len(.SearchExpression) <= 1 Then
                    'hledat pouze podle počátečních písmen
                    s.Append("a.p41Name LIKE @expr+'%' OR a.p41Code LIKE '%'+@expr+'%' OR p28client.p28Name LIKE @expr+'%' OR a.p41NameShort LIKE @expr+'%' OR p28client.p28CompanyName LIKE @expr+'%'")
                Else
                    'něco jako fulltext
                    s.Append("a.p41Name LIKE '%'+@expr+'%' OR a.p41Code LIKE '%'+@expr+'%' OR a.p41NameShort LIKE '%'+@expr+'%' OR p28client.p28Name LIKE '%'+@expr+'%' OR p28client.p28CompanyName LIKE '%'+@expr+'%'")
                End If
                s.Append(")")
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If .x18Value <> "" Then
                s.Append(bas.CompleteX18QuerySql("p41", .x18Value))
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then s.Append(" AND a.p41ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=141 AND o51ID IN (" & String.Join(",", .o51IDs) & "))")
            End If
            If .IsRecurrenceMother = BO.BooleanQueryMode.TrueQuery Then
                s.Append(" AND a.p65ID IS NOT NULL")
            End If
            If .IsRecurrenceChild = BO.BooleanQueryMode.TrueQuery Then
                s.Append(" AND a.p41RecurMotherID IS NOT NULL AND a.p41RecurBaseDate IS NOT NULL")
            End If
            If .p41RecurMotherID <> 0 Then
                s.Append(" AND a.p41RecurMotherID=@motherid")
                pars.Add("motherid", .p41RecurMotherID, DbType.Int32)
            End If
        End With
        Return bas.TrimWHERE(s.ToString)
    End Function

    Public Function GetVirtualCount(myQuery As BO.myQueryP41) As Integer
        Dim s As String = "SELECT count(a.p41ID) as Value " & GetSQLPart2(myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP41, strSumFields As String) As DataTable
        Dim s As String = "SELECT count(a.p41ID) as VirtualCount"
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

    Private Function GetSF() As String
        Dim s As New System.Text.StringBuilder
        s.Append("a.p42ID,a.j02ID_Owner,a.p41Name,a.p41NameShort,a.p41Code as _p41Code,a.p41IsDraft,a.p28ID_Client,a.p28ID_Billing,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.p92ID,a.b02ID,a.j18ID,a.p61ID,a.p41InvoiceDefaultText1,a.p41InvoiceDefaultText2,a.p41InvoiceMaturityDays,a.p41WorksheetOperFlag,a.p41PlanFrom,a.p41PlanUntil,a.p41LimitHours_Notification,a.p41LimitFee_Notification")
        s.Append(",p28client.p28Name as _Client,p51billing.p51Name as _p51Name_Billing")
        s.Append(",a.p41TreeLevel as _p41TreeLevel,a.p41TreeIndex as _p41TreeIndex,a.p41TreePrev as _p41TreePrev,a.p41TreeNext as _p41TreeNext,a.p41TreePath as _p41TreePath")
        s.Append(",a.p65ID,a.p41RecurNameMask,a.p41RecurBaseDate,a.p41RecurMotherID,a.p41IsStopRecurrence")
        s.Append(",p42.p42Name as _p42Name,p92.p92Name as _p92Name,b02.b02Name as _b02Name,j18.j18Name as _j18Name,a.p41ExternalPID,a.p41ParentID,a.p41BillingMemo," & bas.RecTail("p41", "a"))
        s.Append(",j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,p28client.p87ID as _p87ID_Client,p42.b01ID as _b01ID,a.p41IsNoNotify,a.p41RobotAddress,a.p72ID_NonBillable,a.p72ID_BillableHours,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice")
        Return s.ToString
    End Function
    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " " & GetSF()
        Return s

    End Function
    Private Function GetSQLPart2(mq As BO.myQueryP41) As String
        Dim s As New System.Text.StringBuilder

        s.Append("FROM p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID")
        s.Append(" LEFT OUTER JOIN p28Contact p28client ON a.p28ID_Client=p28client.p28ID")

        s.Append(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID")

        s.Append(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID")
        s.Append(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID")
        s.Append(" LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID")
        s.Append(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID")
        s.Append(" LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID")
        s.Append(" LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID")
        If Not mq Is Nothing Then
            If mq.MG_AdditionalSqlFROM <> "" Then s.Append(" " & mq.MG_AdditionalSqlFROM)
        End If
        Return s.ToString

    End Function

    Public Function ConvertFromDraft(intPID As Integer) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("p41id", intPID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p41_convertdraft", pars)

    End Function

    Public Function ExistWaitingWorksheetForInvoicing(intPID As Integer) As Boolean
        If _cDB.GetValueFromSQL("select top 1 p31ID FROM p31worksheet WHERE p41ID=" & intPID.ToString & " AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil") <> "" Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function ExistWaitingWorksheetForApproving(intPID As Integer) As Boolean
        If _cDB.GetValueFromSQL("select top 1 p31ID FROM p31worksheet WHERE p41ID=" & intPID.ToString & " AND p71ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil") <> "" Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function HasChildRecords(intPID As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intPID, DbType.Int32)
        If _cDB.GetIntegerValueFROMSQL("if exists(select p41ID FROM p41Project WHERE p41ParentID=@pid) select 1 as Value else select 0 as Value", pars) = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetTopProjectsByWorksheetEntry(intJ02ID As Integer, intGetTopRecs As Integer) As List(Of Integer)
        Dim s As String = "SELECT TOP " & intGetTopRecs.ToString & " p41ID as Value FROM view_LastWorksheetDateOfPerson WHERE j02ID=@j02id ORDER BY LastDate DESC"
        Return _cDB.GetList(Of BO.GetInteger)(s, New With {.j02id = intJ02ID}).Select(Function(p) p.Value).ToList
    End Function
    Public Function IsMyFavouriteProject(intPID As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intPID, DbType.Int32)
        pars.Add("j03id", _curUser.PID, DbType.Int32)
        If _cDB.GetIntegerValueFROMSQL("if exists(select j13ID FROM j13FavourteProject WHERE p41ID=@pid AND j03ID=@j03id) select 1 as Value else select 0 as Value", pars) = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function BatchUpdate_TreeChilds(intPID As Integer, bolProjectRoles As Boolean, bolP28ID As Boolean, bolP87ID As Boolean, bolP51ID As Boolean, bolP92ID As Boolean, bolJ18ID As Boolean, bolP61ID As Boolean, bolValidity As Boolean) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("is_roles", bolProjectRoles, DbType.Boolean)
            .Add("is_p28id", bolP28ID, DbType.Boolean)
            .Add("is_p87id", bolP87ID, DbType.Boolean)
            .Add("is_p51id", bolP51ID, DbType.Boolean)
            .Add("is_p92id", bolP92ID, DbType.Boolean)
            .Add("is_j18id", bolJ18ID, DbType.Boolean)
            .Add("is_p61id", bolP61ID, DbType.Boolean)
            .Add("is_validity", bolValidity, DbType.Boolean)
            pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p41_batch_update_childs", pars)

    End Function
    Public Function GetRolesInline(intPID As Integer) As String
        Return _cDB.GetValueFromSQL("SELECT dbo.p41_getroles_inline(" & intPID.ToString & ") as Value")
    End Function
End Class
