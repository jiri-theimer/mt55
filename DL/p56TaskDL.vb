Public Class p56TaskDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p56Task
        Dim s As String = GetSQLPart1(0, False) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p56ID=@p56id"

        Return _cDB.GetRecord(Of BO.p56Task)(s, New With {.p56id = intPID})
    End Function
    Public Function LoadMyLastCreated() As BO.p56Task
        Dim s As String = GetSQLPart1(1, False) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.p56ID DESC"

        Return _cDB.GetRecord(Of BO.p56Task)(s, New With {.j02id_owner = _curUser.j02ID})
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p56Task
        Dim s As String = GetSQLPart1(1, False) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p56ExternalPID LIKE @externalpid"
        Return _cDB.GetRecord(Of BO.p56Task)(s, New With {.externalpid = strExternalPID})
    End Function
    Public Function LoadByCode(strCode As String) As BO.p56Task
        Dim s As String = GetSQLPart1(1, False) & " " & GetSQLPart2(Nothing)
        s += " WHERE a.p56Code LIKE @code"
        Return _cDB.GetRecord(Of BO.p56Task)(s, New With {.code = strCode})
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.p56TaskSum
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
        End With
        Return _cDB.GetRecord(Of BO.p56TaskSum)("p56_inhale_sumrow", pars, True)
    End Function

    Public Function Save(cRec As BO.p56Task, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p56ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                If .p56Code = "" Then .p56Code = "TEMP" & BO.BAS.GetGUID() 'dočasný kód, bude později nahrazen
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
                pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
                pars.Add("o22ID", BO.BAS.IsNullDBKey(.o22ID), DbType.Int32)
                pars.Add("p57ID", BO.BAS.IsNullDBKey(.p57ID), DbType.Int32)
                pars.Add("o43ID", BO.BAS.IsNullDBKey(.o43ID), DbType.Int32)
                pars.Add("p59ID_Submitter", BO.BAS.IsNullDBKey(.p59ID_Submitter), DbType.Int32)
                pars.Add("p59ID_Receiver", BO.BAS.IsNullDBKey(.p59ID_Receiver), DbType.Int32)
                pars.Add("p56CompletePercent", .p56CompletePercent, DbType.Int32)
                pars.Add("p56RatingValue", .p56RatingValue, DbType.Int32)

                pars.Add("p56Name", .p56Name, DbType.String, , , True, "Název úlohy")
                pars.Add("p56NameShort", .p56NameShort, DbType.String, , , True, "Zkrácený název")
                pars.Add("p56Code", .p56Code, DbType.String)
                pars.Add("p56ExternalPID", .p56ExternalPID, DbType.String)

                pars.Add("p56Ordinary", .p56Ordinary, DbType.Int32)
                pars.Add("p56Plan_Hours", .p56Plan_Hours, DbType.Double)
                pars.Add("p56Plan_Expenses", .p56Plan_Expenses, DbType.Double)
                pars.Add("p56IsPlan_Hours_Ceiling", .p56IsPlan_Hours_Ceiling, DbType.Boolean)
                pars.Add("p56IsPlan_Expenses_Ceiling", .p56IsPlan_Expenses_Ceiling, DbType.Boolean)

                pars.Add("p56Description", .p56Description, DbType.String, , , True, "Popis úlohy")

                pars.Add("p56PlanFrom", BO.BAS.IsNullDBDate(.p56PlanFrom), DbType.DateTime)
                pars.Add("p56PlanUntil", BO.BAS.IsNullDBDate(.p56PlanUntil), DbType.DateTime)
                pars.Add("p56ReminderDate", BO.BAS.IsNullDBDate(.p56ReminderDate), DbType.DateTime)
                pars.Add("p56IsNoNotify", .p56IsNoNotify, DbType.Boolean)

                pars.Add("p65ID", BO.BAS.IsNullDBKey(.p65ID), DbType.Int32)
                pars.Add("p56RecurNameMask", .p56RecurNameMask, DbType.String)
                pars.Add("p56RecurBaseDate", .p56RecurBaseDate, DbType.DateTime)
                pars.Add("p56RecurMotherID", BO.BAS.IsNullDBKey(.p56RecurMotherID), DbType.Int32)
                pars.Add("p56IsStopRecurrence", .p56IsStopRecurrence, DbType.Boolean)

                pars.Add("p56validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p56validuntil", .ValidUntil, DbType.DateTime)

            End With
            Dim strUserInsert As String = _curUser.j03Login
            If cRec.UserInsert <> "" Then strUserInsert = cRec.UserInsert
            If _cDB.SaveRecord("p56Task", pars, bolINSERT, strW, True, strUserInsert) Then
                Dim intLastSavedP56ID As Integer = _cDB.LastSavedRecordPID
                If Not lisX69 Is Nothing Then   'přiřazení rolí úlohy
                    bas.SaveX69(_cDB, BO.x29IdEnum.p56Task, intLastSavedP56ID, lisX69, bolINSERT)
                End If
                If Not lisFF Is Nothing Then    'volná pole
                    bas.SaveFreeFields(_cDB, lisFF, "p56Task_FreeField", intLastSavedP56ID)
                End If
                pars = New DbParameters
                With pars
                    .Add("p56id", intLastSavedP56ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                End With
                If _cDB.RunSP("p56_aftersave", pars) Then
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
        Return _cDB.RunSP("p56_delete", pars)
    End Function


    Private Function GetSQLWHERE(myQuery As BO.myQueryP56, ByRef pars As DL.DbParameters) As String
        Dim s As New System.Text.StringBuilder
        s.Append(bas.ParseWhereMultiPIDs("a.p56ID", myQuery))
        s.Append(bas.ParseWhereValidity("p56", "a", myQuery))
        With myQuery
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                If Not .IsIncludeChildProjects Then
                    s.Append(" AND a.p41ID=@p41id")
                Else
                    s.Append(" AND (a.p41ID=@p41id OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID=@p41id) AND (select p41TreeNext FROM p41Project WHERE p41ID=@p41id)))")
                End If
            End If
            If Not .p41IDs Is Nothing Then
                If .p41IDs.Count > 0 Then
                    If Not .IsIncludeChildProjects Then
                        s.Append(" AND a.p41ID IN (" & String.Join(",", .p41IDs) & ")")
                    Else
                        s.Append(" AND (a.p41ID IN (" & String.Join(",", .p41IDs) & ") OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & ")) AND (select p41TreeNext FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & "))))")
                    End If
                End If
            End If

            If .j02ID_Owner <> 0 Then
                pars.Add("ownerid", .j02ID_Owner, DbType.Int32)
                s.Append(" AND a.j02ID_Owner=@ownerid")
            End If
            If .o22ID <> 0 Then
                pars.Add("o22id", .o22ID, DbType.Int32)
                s.Append(" AND a.o22ID=@o22id")
            End If
            If .p57ID <> 0 Then
                pars.Add("p57id", .p57ID, DbType.Int32)
                s.Append(" AND a.p57ID=@p57id")
            End If

            If .b02ID <> 0 Then
                pars.Add("b02id", .b02ID, DbType.Int32)
                s.Append(" AND a.b02ID=@b02id")
            End If
            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                s.Append(" AND p41.p28ID_Client=@p28id")
            End If
            If .j02ID <> 0 Then
                'bráno z pohledu, kde je daná osoba příjemcém/řešitelem úkolu
                pars.Add("j02id", .j02ID, DbType.Int32)
                s.Append(" AND (a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)))")   'obdržel nějakou (jakoukoliv) roli v úkolu
                s.Append(" OR (a.j02ID_Owner=@j02id AND a.p56ID NOT IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356))")
                s.Append(")")
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then
                    s.Append(" AND (a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID IN (" & String.Join(",", .j02IDs) & ") OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID IN (" & String.Join(",", .j02IDs) & "))))")   'obdržel nějakou (jakoukoliv) roli v úkolu
                    s.Append(" OR (a.j02ID_Owner IN (" & String.Join(",", .j02IDs) & ") AND a.p56ID NOT IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356))")
                    s.Append(")")
                End If
            End If
            If Not .Owners Is Nothing Then
                If .Owners.Count > 0 Then
                    s.Append(" AND a.j02ID_Owner IN (" & String.Join(",", .Owners) & ")")
                End If
            End If
            If Not .DateInsertFrom Is Nothing Then
                If Year(.DateInsertFrom) > 1900 Then
                    pars.Add("d1", .DateInsertFrom)
                    pars.Add("d2", .DateInsertUntil)
                    s.Append(" AND a.p56DateInsert BETWEEN @d1 AND @d2")
                End If
            End If
            If Not .p31Date_D1 Is Nothing Then
                If Year(.p31Date_D1) > 1900 Then
                    pars.Add("dp31f1", .p31Date_D1)
                    pars.Add("dp31f2", .p31Date_D2)
                    s.Append(" AND a.p56ID IN (SELECT p56ID FROM p31Worksheet WHERE p56ID IS NOT NULL AND p31Date BETWEEN @dp31f1 AND @dp31f2)")
                End If
            End If
            If Not .p56PlanFrom_D1 Is Nothing Then
                If Year(.p56PlanFrom_D1) > 1900 Then
                    pars.Add("dpf1", .p56PlanFrom_D1)
                    pars.Add("dpf2", .p56PlanFrom_D2)
                    s.Append(" AND a.p56PlanFrom BETWEEN @dpf1 AND @dpf2")
                End If
            End If
            If Not .p56PlanUntil_D1 Is Nothing Then
                If Year(.p56PlanUntil_D1) > 1900 Then
                    pars.Add("dpu1", .p56PlanUntil_D1)
                    pars.Add("dpu2", .p56PlanUntil_D2)
                    s.Append(" AND a.p56PlanUntil BETWEEN @dpu1 AND @dpu2")
                End If

            End If
            Select Case .TerminNeniVyplnen
                Case BO.BooleanQueryMode.TrueQuery
                    s.Append(" AND a.p56PlanUntil IS NULL")
                Case BO.BooleanQueryMode.FalseQuery
                    s.Append(" AND a.p56PlanUntil IS NOT NULL")
            End Select
            If .SpecificQuery > BO.myQueryP56_SpecificQuery._NotSpecified Then
                If .j02ID_ExplicitQueryFor > 0 Then
                    pars.Add("j02id_query", .j02ID_ExplicitQueryFor, DbType.Int32)
                Else
                    pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                End If
            End If
            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    s.Append(" AND " & strQueryW)
                End If
            End If

            Dim strInnerW As String = ""
            If .p41ID > 0 Then
                strInnerW = " AND x69.x69RecordPID=@p41id"
            End If
            Select Case .SpecificQuery
                Case BO.myQueryP56_SpecificQuery.AllowedForWorksheetEntry
                    s.Append(" AND (")
                    s.Append("a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID WHERE x67.x29ID=356 AND x68.x53ID=63 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))")          'Zapisovat k úkolu worksheet úkony
                    s.Append(" OR a.p41ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID WHERE x67.x29ID=141 AND x68.x53ID=9" & strInnerW & " AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))")    'Oprávnění vlastníka e všem úkolům v projektu
                    s.Append(")")
                Case BO.myQueryP56_SpecificQuery.AllowedForRead
                    If Not (BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P41_Reader) Or BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P41_Owner)) Then
                        'pokud má právo číst nebo vlastnit všechny projekty, vztahuje se to i na úkoly
                        s.Append(" AND (a.j02ID_Owner=@j02id_query")

                        s.Append(" OR a.p41ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID INNER JOIN x68EntityRole_Permission x68 ON x67.x67ID=x68.x67ID WHERE x67.x29ID=141 AND x68.x53ID IN (3,9)" & strInnerW & " AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))")    'Oprávnění vlastníka nebo uzavíratele ke všem úkolům v projektu

                        s.Append(" OR a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))")   'obdržel nějakou (jakoukoliv) roli v úkolu
                        s.Append(")")
                    End If
            End Select
            If .ColumnFilteringExpression <> "" Then
                s.Append(" AND " & .ColumnFilteringExpression)
            End If
            If .SearchExpression <> "" Then
                s.Append(" AND (")
                'něco jako fulltext
                s.Append("a.p56Name LIKE '%'+@expr+'%' OR a.p56Description LIKE '%'+@expr+'%' OR p41.p41Name LIKE '%'+@expr+'%' OR a.p56Code LIKE @expr+'%' OR p41.p41NameShort LIKE '%'+@expr+'%' OR p28client.p28Name LIKE '%'+@expr+'%' OR p28client.p28CompanyShortName LIKE '%'+@expr+'%'")
                s.Append(")")
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then s.Append(" AND a.p56ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=356 AND o51ID IN (" & String.Join(",", .o51IDs) & "))")
            End If
            If .x18Value <> "" Then
                s.Append(bas.CompleteX18QuerySql("p56", .x18Value))
            End If
            If .IsRecurrenceMother = BO.BooleanQueryMode.TrueQuery Then
                s.Append(" AND a.p65ID IS NOT NULL")
            End If
            If .IsRecurrenceChild = BO.BooleanQueryMode.TrueQuery Then
                s.Append(" AND a.p56RecurMotherID IS NOT NULL AND a.p56RecurBaseDate IS NOT NULL")
            End If
            If .p56RecurMotherID <> 0 Then
                s.Append(" AND a.p56RecurMotherID=@motherid")
                pars.Add("motherid", .p56RecurMotherID, DbType.Int32)
            End If
        End With
        Return bas.TrimWHERE(s.ToString)
    End Function

    Public Function GetList(myQuery As BO.myQueryP56, bolInhaleReceiversInLine As Boolean) As IEnumerable(Of BO.p56Task)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly, myQuery.IsShowTagsInColumn), pars As New DbParameters
        If myQuery.MG_SelectPidFieldOnly Then
            'SQL SELECT klauzule bude plnit pouze hodnotu primárního klíče
            s = "SELECT a.p56ID as _pid"
        End If
        If bolInhaleReceiversInLine Then
            s += ",dbo.p56_getroles_inline(a.p56ID) as _ReceiversInLine"
        End If
        s += " " & GetSQLPart2(myQuery)
        Dim strW As String = GetSQLWHERE(myQuery, pars)


        With myQuery
            Dim strSort As String = .MG_SortString
            If .p41ID <> 0 And strSort = "" Then
                strSort = "a.p56Ordinary,a.p56ID DESC"
            End If
            If strSort = "" Then strSort = "a.p56ID DESC"

            If .MG_PageSize > 0 Then
                'použít stránkování do gridu = zcela jiný SQL dotaz od začátku
                s = GetSQL_OFFSET(strW, ParseSortExpression(strSort), .MG_PageSize, .MG_CurrentPageIndex, pars, bolInhaleReceiversInLine)
            Else
                'normální select - navazuje se na úvodní skladbu
                If strW <> "" Then s += " WHERE " & strW
                If strSort <> "" Then
                    s += " ORDER BY " & ParseSortExpression(strSort)
                End If
            End If

        End With
        Return _cDB.GetList(Of BO.p56Task)(s, pars)
    End Function
    Public Function GetGridDataSource(myQuery As BO.myQueryP56) As DataTable
        Dim s As String = ""
        With myQuery
            If Not String.IsNullOrEmpty(.MG_GridGroupByField) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 Or .MG_GridGroupByField = "Client" Then
                    Select Case .MG_GridGroupByField
                        Case "ProjectCodeAndName" : .MG_GridSqlColumns += ",isnull(p28client.p28Name+char(32)+'-'+char(32),'')+isnull(p41NameShort,p41Name) as ProjectCodeAndName"
                        Case "Client" : .MG_GridSqlColumns += ",p28client.p28Name as Client"
                        Case "p59NameSubmitter" : .MG_GridSqlColumns += ",p59submitter.p59Name as p59NameSubmitter"
                        Case "ReceiversInLine" : .MG_GridSqlColumns += ",dbo.p56_getroles_inline(a.p56ID) as ReceiversInLine"
                        Case "Owner" : .MG_GridSqlColumns += ",j02owner.j02LastName+char(32)+j02owner.j02FirstName as Owner"
                        Case "IsClosed" 'je automaticky ve sloupcích, viz níže
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If

            .MG_GridSqlColumns += ",a.p56ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.p56ValidFrom AND a.p56ValidUntil THEN 0 else 1 END) as IsClosed"
            .MG_GridSqlColumns += ",a.p56PlanUntil as p56PlanUntil_Grid,a.b02ID as b02ID_Grid,b02Color as b02Color_Grid,a.o43ID,a.p65ID"
        End With
        

        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.p56ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If .MG_GridGroupByField <> "" Then
                Dim strPrimarySortField As String = .MG_GridGroupByField
                If strPrimarySortField = "Client" Then strPrimarySortField = "p28client.p28Name"
                If strPrimarySortField = "Owner" Then strPrimarySortField = "j02owner.j02LastName+char(32)+j02owner.j02FirstName"
                If strPrimarySortField = "p59NameSubmitter" Then strPrimarySortField = "p59submitter.p59Name"
                If strPrimarySortField = "ProjectCodeAndName" Then strPrimarySortField = "isnull(p28client.p28Name+char(32)+'-'+char(32),'')+isnull(p41NameShort,p41Name)"
                If strPrimarySortField = "ReceiversInLine" Then strPrimarySortField = "dbo.p56_getroles_inline(a.p56ID)"
                If strPrimarySortField = "IsClosed" Then strPrimarySortField = "CASE WHEN GETDATE() BETWEEN a.p56ValidFrom AND a.p56ValidUntil THEN 0 else 1 END"
                If strORDERBY = "" Or LCase(strPrimarySortField) = Replace(Replace(LCase(.MG_SortString), " desc", ""), " asc", "") Then
                    strORDERBY = strPrimarySortField
                Else
                    strORDERBY = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            If .p41ID <> 0 And strORDERBY = "" Then
                strORDERBY = "a.p56Ordinary,a.p56ID DESC"
            End If
            If strORDERBY = "" Then strORDERBY = "a.p56ID DESC"

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
    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters, bolInhaleReceiversInLine As Boolean) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize

        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & GetSF(False)
        If bolInhaleReceiversInLine Then
            s += ",dbo.p56_getroles_inline(a.p56ID) as _ReceiversInLine"
        End If
        s += " " & GetSQLPart2(Nothing)

        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " * FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"

        Return s
    End Function

    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "p56UserInsert").Replace("UserUpdate", "p56UserUpdate").Replace("DateInsert", "p56DateInsert").Replace("DateUpdate", "p56DateUpdate")
        strSort = strSort.Replace("Owner", "j02owner.j02LastName").Replace("ProjectCodeAndName", "p41Code,p41Name").Replace("Client", "p28Name").Replace("ReceiversInLine", "dbo.p56_getroles_inline(a.p56ID)")
        strSort = strSort.Replace("p59NameSubmitter", "p59submitter.p59name")
        Return bas.NormalizeOrderByClause(strSort)
    End Function
    Private Function ParseFilterExpression(strFilter As String) As String
        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function
    Public Function GetList_WaitingOnReminder(datReminderFrom As Date, datReminderUntil As Date) As IEnumerable(Of BO.p56Task)
        Dim s As String = GetSQLPart1(0, False) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        pars.Add("datereminderfrom", datReminderFrom)
        pars.Add("datereminderuntil", datReminderUntil)
        s += " WHERE p56ReminderDate BETWEEN @datereminderfrom AND @datereminderuntil"
        s += " AND a.p56ID NOT IN (SELECT x47RecordPID FROM x47EventLog WHERE x29ID=356 AND x45ID=35606)"

        Return _cDB.GetList(Of BO.p56Task)(s, pars)
    End Function
    Public Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.p56Task)
        Dim s As String = GetSQLPart1(100, False) & " " & GetSQLPart2(Nothing), pars As New DbParameters
        ''s += " WHERE ((p56PlanUntil BETWEEN DATEADD(DAY,-3,@d1) AND @d2 and getdate() between p56ValidFrom and p56ValidUntil) OR p56ReminderDate between @d1 AND @d2)"
        s += " WHERE ((p56PlanUntil IS NOT NULL and getdate() between p56ValidFrom and p56ValidUntil) OR p56ReminderDate between @d1 AND @d2)"
        's += "AND (a.j02ID_Owner=@j02id OR a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))))"
        s += "AND a.p56ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=356 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)))"

        pars.Add("j02id", intJ02ID, DbType.Int32)
       
        pars.Add("d1", DateAdd(DateInterval.Day, -1, Now), DbType.DateTime)
        pars.Add("d2", DateAdd(DateInterval.Day, 2, Now), DbType.DateTime)

        Return _cDB.GetList(Of BO.p56Task)(s, pars)
    End Function
    ''Public Function GetList_WithWorksheetSum(myQuery As BO.myQueryP56, bolInhaleReceiversInLine As Boolean) As IEnumerable(Of BO.p56TaskWithWorksheetSum)
    ''    Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly) & ",p31.p31RowsCount,p31.Hours_Orig,p31.Expenses_Orig,p31.Incomes_Orig", pars As New DbParameters
    ''    If bolInhaleReceiversInLine Then
    ''        s += ",dbo.p56_getroles_inline(a.p56ID) as _ReceiversInLine"
    ''    End If
    ''    s += " " & GetSQLPart2(myQuery)
    ''    s += " LEFT OUTER JOIN (SELECT xa.p56ID,COUNT(xa.p31ID) as p31RowsCount,sum(case when xc.p33ID=1 then p31Hours_Orig end) as Hours_Orig,sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=1 then p31Value_Orig end) as Expenses_Orig,sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=2 then p31Value_Orig end) as Incomes_Orig"
    ''    s += " FROM p31Worksheet xa INNER JOIN p32Activity xb ON xa.p32ID=xb.p32ID INNER JOIN p34ActivityGroup xc ON xb.p34ID=xc.p34ID WHERE xa.p56ID IS NOT NULL GROUP BY xa.p56ID) p31 ON a.p56ID=p31.p56ID"
    ''    Dim strW As String = GetSQLWHERE(myQuery, pars)
    ''    If strW <> "" Then s += " WHERE " & strW

    ''    With myQuery
    ''        If .MG_SortString = "" Then
    ''            If .p41ID <> 0 Then
    ''                s += " ORDER BY a.p56Ordinary,a.p56ID DESC"
    ''            Else
    ''                s += " ORDER BY a.p56ID DESC"
    ''            End If
    ''        Else
    ''            s += " ORDER BY " & ParseSortExpression(.MG_SortString)
    ''        End If
    ''    End With
    ''    Return _cDB.GetList(Of BO.p56TaskWithWorksheetSum)(s, pars)


    ''End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryP56) As Integer
        Dim s As String = "SELECT count(a.p56ID) as Value " & GetSQLPart2(myQuery)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP56, strSumFields As String) As DataTable
        Dim s As String = "SELECT count(a.p56ID) as VirtualCount"
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

    Private Function GetSF(bolIncludeTags As Boolean) As String
        Dim s As New System.Text.StringBuilder

        s.Append(bas.RecTail("p56", "a"))
        s.Append(",a.p41ID,a.o22ID,a.p57ID,a.j02ID_Owner,a.b02ID,a.p59ID_Submitter,a.p59ID_Receiver,a.o43ID,a.p56Name,a.p56NameShort,a.p56Code,a.p56Description,a.p56Ordinary,a.p56PlanFrom,a.p56PlanUntil,a.p56ReminderDate,a.p56Plan_Hours,a.p56Plan_Expenses,a.p56RatingValue,a.p56CompletePercent,a.p56ExternalPID,a.p56IsPlan_Hours_Ceiling,a.p56IsPlan_Expenses_Ceiling,a.p56IsHtml,a.p56IsNoNotify")
        s.Append(",p28client.p28Name as _Client,p57.p57Name as _p57Name,p59submitter.p59Name as _p59NameSubmitter,isnull(p41.p41NameShort,p41.p41Name) as _p41Name,p41.p41Code as _p41Code,o22.o22Name as _o22Name,b02.b02Name as _b02Name,b02.b02Color as _b02Color,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,p57.p57IsHelpdesk as _p57IsHelpdesk,p57.b01ID as _b01ID,p57.p57PlanDatesEntryFlag as _p57PlanDatesEntryFlag")
        s.Append(",a.p65ID,a.p56RecurNameMask,a.p56RecurBaseDate,a.p56RecurMotherID,a.p56IsStopRecurrence")
        If bolIncludeTags Then
            s.Append(",dbo.tag_values_inline_html(356,a.p56ID) as TagsInlineHtml")
        Else
            s.Append(",NULL as TagsInlineHtml")
        End If
        Return s.ToString

    End Function
    Private Function GetSQLPart1(intTOP As Integer, bolIncludeTags As Boolean) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " " & GetSF(bolIncludeTags)
        Return s
    End Function
    Private Function GetSQLPart2(mq As BO.myQueryP56) As String
        Dim s As New System.Text.StringBuilder
        s.Append("FROM p56Task a INNER JOIN p57TaskType p57 ON a.p57ID=p57.p57ID")
        s.Append(" INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID")
        s.Append(" LEFT OUTER JOIN o22Milestone o22 ON a.o22ID=o22.o22ID")
        s.Append(" LEFT OUTER JOIN p59Priority p59submitter ON a.p59ID_Submitter=p59submitter.p59ID")
        s.Append(" LEFT OUTER JOIN p28Contact p28client ON p41.p28ID_Client=p28client.p28ID")
        s.Append(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID")
        s.Append(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID")
        s.Append(" LEFT OUTER JOIN p56Task_FreeField p56free ON a.p56ID=p56free.p56ID")
        If Not mq Is Nothing Then
            With mq
                If Not String.IsNullOrEmpty(.MG_GridSqlColumns) Then
                    If .MG_GridSqlColumns.IndexOf("p31.") > 0 Then
                        s.Append(" LEFT OUTER JOIN (SELECT xa.p56ID,COUNT(xa.p31ID) as p31RowsCount,sum(case when xc.p33ID=1 then p31Hours_Orig end) as Hours_Orig,sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=1 then p31Value_Orig end) as Expenses_Orig,sum(case when xc.p33ID IN (2,5) AND xc.p34IncomeStatementFlag=2 then p31Value_Orig end) as Incomes_Orig")
                        s.Append(" FROM p31Worksheet xa INNER JOIN p32Activity xb ON xa.p32ID=xb.p32ID INNER JOIN p34ActivityGroup xc ON xb.p34ID=xc.p34ID WHERE xa.p56ID IS NOT NULL GROUP BY xa.p56ID) p31 ON a.p56ID=p31.p56ID")
                    End If
                End If

                If .MG_AdditionalSqlFROM <> "" Then
                    s.Append(" " & .MG_AdditionalSqlFROM)
                End If
            End With
        End If

        Return s.ToString
    End Function

    Public Function GetRolesInline(intPID As Integer) As String        
        Return _cDB.GetValueFromSQL("SELECT dbo.p56_getroles_inline(" & intPID.ToString & ") as Value")
    End Function

    Public Sub UpdateSelectedTaskRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP56ID As Integer)
        bas.SaveX69(_cDB, BO.x29IdEnum.p56Task, intP56ID, lisX69, False, intX67ID)
    End Sub
    Public Sub ClearSelectedTaskRole(intX67ID As Integer, intP56ID As Integer)
        _cDB.RunSQL("DELETE FROM x69EntityRole_Assign WHERE x67ID=" & intX67ID.ToString & " AND x69RecordPID=" & intP56ID.ToString)
    End Sub
    ''Public Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean
    ''    Dim pars As New DbParameters()
    ''    pars.Add("pid", intPID)
    ''    pars.Add("o43ID", BO.BAS.IsNullDBKey(intO43ID), DbType.Int32)
    ''    Return _cDB.SaveRecord("p56Task", pars, False, "p56ID=@pid", False)
    ''End Function
    Public Function GetTotalTasksCount() As Integer
        Return _cDB.GetRecord(Of BO.GetInteger)("SELECT COUNT(*) as Value FROM p56Task").Value
    End Function
End Class
