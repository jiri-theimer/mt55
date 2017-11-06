Public Class o23DocDL
    Inherits DLMother
    Public Property CalendarFieldStart As String = "NULL"
    Public Property CalendarFieldEnd As String = "NULL"
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o23Doc
        Dim s As String = "SELECT " & GetSQLPart1(0) & " WHERE a.o23ID=@pid"
        Return _cDB.GetRecord(Of BO.o23Doc)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByCode(strCode As String, intX23ID As Integer) As BO.o23Doc
        Dim pars As New DbParameters
        pars.Add("x23id", intX23ID, DbType.Int32)
        pars.Add("code", strCode, DbType.String)
        Dim s As String = "SELECT " & GetSQLPart1(0) & " WHERE a.x23ID=@x23id AND a.o23Code=@code"
        Return _cDB.GetRecord(Of BO.o23Doc)(s, pars)
    End Function
    Public Function LoadHtmlContent(intPID As Integer) As String
        If intPID = 0 Then Return ""
        Dim pars As New DbParameters
        pars.Add("pid", intPID, DbType.Int32)        
        Return _cDB.GetRecord(Of BO.GetString)("SELECT o23HtmlContent as Value FROM o23BigData WHERE o23ID=@pid", pars).Value
    End Function
    ''Public Function LoadFolders(intPID As Integer) As String
    ''    If intPID = 0 Then Return ""
    ''    Return _cDB.GetValueFromSQL("SELECT o23Folders FROM o23BigData WHERE o23ID=" & intPID.ToString)
    ''End Function
    Public Function GetEntityPidByX20ID(intO23ID As Integer, intX20ID As Integer) As Integer
        Return _cDB.GetIntegerValueFROMSQL("select dbo.stitek_entity_pid_by_x20id(" & intO23ID.ToString & "," & intX20ID.ToString & ")")
    End Function
    Private Function GetSQLPart1(intTopRecs As Integer) As String
        Dim s As String = ""
        If intTopRecs > 0 Then s += " TOP " & intTopRecs.ToString
        s += " a.*," & bas.RecTail("o23", "a") & ",x23.x23Name as _x23Name,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner,b02.b02Name as _b02Name,b02.b02Color as _b02Color,x18.x18ID as _x18ID,x18.x18Name as _DocType"
        s += ",a.o23IsDraft as IsDraft,convert(bit,case when o27.o27ExistInt=1 then 1 else 0 end) as IsO27"
        If Me.CalendarFieldStart <> "" Then s += "," & Me.CalendarFieldStart & " AS CalendarDateStart"
        If Me.CalendarFieldEnd <> "" Then s += "," & Me.CalendarFieldEnd & " AS CalendarDateEnd"
        s += " " & GetSQLPart2_From()
        Return s
    End Function
    
    Private Function GetSQLPart2_From() As String
        Return "FROM o23Doc a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID INNER JOIN x18EntityCategory x18 ON x23.x23ID=x18.x23ID LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN (SELECT DISTINCT za.b07RecordPID as o23ID,1 as o27ExistInt FROM b07Comment za INNER JOIN o27Attachment zb ON za.b07ID=zb.b07ID WHERE za.x29ID=223) o27 ON a.o23ID=o27.o23ID"

    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o23_delete", pars)

    End Function
    Public Function RunSp_AfterSave(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("o23id", intPID, DbType.Int32)
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
        End With
        If _cDB.RunSP("o23_aftersave", pars) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function SaveHtmlContent(intPID As Integer, strHtmlContent As String, strPlainText As String) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID, DbType.Int32)
        pars.Add("s", strHtmlContent, DbType.String)
        pars.Add("t", strPlainText, DbType.String)
        If _cDB.GetIntegerValueFROMSQL("select o23ID FROM o23BigData WHERE o23ID=" & intPID.ToString) = 0 Then
            Return _cDB.RunSQL("INSERT INTO o23BigData(o23ID,o23HtmlContent,o23TextContent) VALUES(@pid,@s,@t)", pars)
        Else
            Return _cDB.RunSQL("UPDATE o23BigData set o23HtmlContent=@s,o23TextContent=@t WHERE o23ID=@pid", pars)
        End If
    End Function
    ''Public Function SaveFolders(intPID As Integer, strFoldersByPipes As String) As Boolean
    ''    Dim pars As New DbParameters()
    ''    pars.Add("pid", intPID, DbType.Int32)
    ''    pars.Add("s", strFoldersByPipes, DbType.String)        
    ''    If _cDB.GetIntegerValueFROMSQL("select o23ID FROM o23BigData WHERE o23ID=" & intPID.ToString) = 0 Then
    ''        Return _cDB.RunSQL("INSERT INTO o23BigData(o23ID,o23Folders) VALUES(@pid,@s)", pars)
    ''    Else
    ''        Return _cDB.RunSQL("UPDATE o23BigData set o23Folders=@s WHERE o23ID=@pid", pars)
    ''    End If
    ''End Function
    Public Function Save(cRec As BO.o23Doc, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "o23ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("o23GUID", .o23GUID, DbType.String)
                pars.Add("x23ID", BO.BAS.IsNullDBKey(.x23ID), DbType.Int32)
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
                pars.Add("o43ID", BO.BAS.IsNullDBKey(.o43ID), DbType.Int32)
                pars.Add("o23Name", .o23Name, DbType.String, , , True, "Název")
                pars.Add("o23Code", .o23Code, DbType.String)
                pars.Add("o23ArabicCode", .o23ArabicCode, DbType.String)
                pars.Add("o23ForeColor", .o23ForeColor, DbType.String)
                pars.Add("o23BackColor", .o23BackColor, DbType.String)
                pars.Add("o23Ordinary", .o23Ordinary, DbType.Int32)

                pars.Add("o23ValidFrom", .ValidFrom, DbType.DateTime)
                pars.Add("o23ValidUntil", .ValidUntil, DbType.DateTime)

                pars.Add("o23IsDraft", .o23IsDraft, DbType.Boolean)
                pars.Add("o23IsEncrypted", .o23IsEncrypted, DbType.Boolean)
                pars.Add("o23Password", .o23Password, DbType.String)

                pars.Add("o23FreeText01", .o23FreeText01, DbType.String, , , True, "Text 1")
                pars.Add("o23FreeText02", .o23FreeText02, DbType.String, , , True, "Text 2")
                pars.Add("o23FreeText03", .o23FreeText03, DbType.String, , , True, "Text 3")
                pars.Add("o23FreeText04", .o23FreeText04, DbType.String, , , True, "Text 4")
                pars.Add("o23FreeText05", .o23FreeText05, DbType.String, , , True, "Text 5")
                pars.Add("o23FreeText06", .o23FreeText06, DbType.String, , , True, "Text 6")
                pars.Add("o23FreeText07", .o23FreeText07, DbType.String, , , True, "Text 7")
                pars.Add("o23FreeText08", .o23FreeText08, DbType.String, , , True, "Text 8")
                pars.Add("o23FreeText09", .o23FreeText09, DbType.String, , , True, "Text 9")
                pars.Add("o23FreeText10", .o23FreeText10, DbType.String, , , True, "Text 10")
                pars.Add("o23FreeText11", .o23FreeText11, DbType.String, , , True, "Text 11")
                pars.Add("o23FreeText12", .o23FreeText12, DbType.String, , , True, "Text 12")
                pars.Add("o23FreeText13", .o23FreeText13, DbType.String, , , True, "Text 13")
                pars.Add("o23FreeText14", .o23FreeText14, DbType.String, , , True, "Text 14")
                pars.Add("o23FreeText15", .o23FreeText15, DbType.String, , , True, "Text 15")
                pars.Add("o23BigText", .o23BigText, DbType.String, , , True, "Podrobný popis")

                pars.Add("o23FreeNumber01", .o23FreeNumber01, DbType.Double)
                pars.Add("o23FreeNumber02", .o23FreeNumber02, DbType.Double)
                pars.Add("o23FreeNumber03", .o23FreeNumber03, DbType.Double)
                pars.Add("o23FreeNumber04", .o23FreeNumber04, DbType.Double)
                pars.Add("o23FreeNumber05", .o23FreeNumber05, DbType.Double)
                pars.Add("o23FreeDate01", .o23FreeDate01, DbType.DateTime)
                pars.Add("o23FreeDate02", .o23FreeDate02, DbType.DateTime)
                pars.Add("o23FreeDate03", .o23FreeDate03, DbType.DateTime)
                pars.Add("o23FreeDate04", .o23FreeDate04, DbType.DateTime)
                pars.Add("o23FreeDate05", .o23FreeDate05, DbType.DateTime)
                pars.Add("o23FreeBoolean01", .o23FreeBoolean01, DbType.Boolean)
                pars.Add("o23FreeBoolean02", .o23FreeBoolean02, DbType.Boolean)
                pars.Add("o23FreeBoolean03", .o23FreeBoolean03, DbType.Boolean)
                pars.Add("o23FreeBoolean04", .o23FreeBoolean04, DbType.Boolean)
                pars.Add("o23FreeBoolean05", .o23FreeBoolean05, DbType.Boolean)
            End With

            If _cDB.SaveRecord("o23Doc", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim into23ID As Integer = _cDB.LastSavedRecordPID
                If Not lisX69 Is Nothing Then   'přiřazení rolí v samotném záznamu štítku
                    bas.SaveX69(_cDB, BO.x29IdEnum.o23Doc, into23ID, lisX69, bolINSERT)
                End If


                sc.Complete()

                Return True

            Else
                Return False
            End If
        End Using
    End Function
    Public Sub UpdateComboItemTextInData(cX28_ComboField As BO.x28EntityField, co23 As BO.o23Doc)
        Dim strTab As String = ""
        With cX28_ComboField
            _cDB.RunSQL("UPDATE " & .SourceTableName & " SET " & .x28Field & "Text=" & BO.BAS.GS(co23.o23Name) & " WHERE " & .x28Field & "=" & co23.PID.ToString)
        End With
    End Sub
    Public Sub ClearComboItemTextInData(cX28_ComboField As BO.x28EntityField, into23ID As Integer)
        Dim strTab As String = ""
        With cX28_ComboField
            _cDB.RunSQL("UPDATE " & .SourceTableName & " SET " & .x28Field & "Text=NULL," & .x28Field & "=NULL WHERE " & .x28Field & "=" & into23ID.ToString)
        End With
    End Sub
    Public Function GetList(myQuery As BO.myQueryO23) As IEnumerable(Of BO.o23Doc)
        Dim s As String = "SELECT " & GetSQLPart1(myQuery.TopRecordsOnly)
        Dim pars As New DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)



        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY a.x23ID,a.o23Ordinary,a.o23Name"

        Return _cDB.GetList(Of BO.o23Doc)(s, pars)

    End Function

    Private Function GetSQLWHERE(myQuery As BO.myQueryO23, ByRef pars As DL.DbParameters) As String
        Dim s As New System.Text.StringBuilder
        pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
        s.Append(bas.ParseWhereMultiPIDs("a.o23ID", myQuery))
        s.Append(bas.ParseWhereValidity("o23", "a", myQuery))

        With myQuery
            If Not BO.BAS.IsNullDBDate(.DateFrom) Is Nothing Then

                pars.Add("d1", .DateFrom, DbType.DateTime) : pars.Add("d2", .DateUntil, DbType.DateTime)
                If .DateQueryFieldBy <> "" Then
                    s.Append(" AND " & .DateQueryFieldBy & " BETWEEN @d1 AND @d2")
                End If
                If .CalendarDateFieldStart <> "" And .CalendarDateFieldEnd <> "" Then
                    ''strW += " AND " & .CalendarDateFieldStart & " IS NOT NULL"
                    ''If .CalendarDateFieldStart <> .CalendarDateFieldEnd Then
                    ''    strW += " AND " & .CalendarDateFieldEnd & " IS NOT NULL"
                    ''End If
                    s.Append(" AND (" & .CalendarDateFieldStart & " BETWEEN @d1 AND @d2 OR " & .CalendarDateFieldEnd & " BETWEEN @d1 AND @d2 OR (" & .CalendarDateFieldStart & "<@d1 AND " & .CalendarDateFieldEnd & ">@d2))")
                End If
            End If

            If Not .DateInsertFrom Is Nothing Then
                If Year(.DateInsertFrom) > 1900 Then
                    pars.Add("d1", .DateInsertFrom) : pars.Add("d2", .DateInsertUntil)
                    s.Append(" AND a.o23DateInsert BETWEEN @d1 AND @d2")
                End If
            End If
            If .x20ID_UnBound <> 0 Then
                s.Append(" AND  a.o23ID NOT IN (select o23ID FROM x19EntityCategory_Binding WHERE x20ID=@x20id_unbound)")
                pars.Add("x20id_unbound", .x20ID_UnBound, DbType.Int32)
            End If
            If .x20ID_Bound <> 0 Then
                s.Append(" AND  a.o23ID IN (select o23ID FROM x19EntityCategory_Binding WHERE x20ID=@x20id_bound)")
                pars.Add("x20id_bound", .x20ID_Bound, DbType.Int32)
            End If
            If .x23ID <> 0 Then
                s.Append(" AND  a.x23ID=@x23id")
                pars.Add("x23id", .x23ID, DbType.Int32)
            End If
            If .RecordPID <> 0 And .Record_x29ID > BO.x29IdEnum._NotSpecified Then
                pars.Add("recordpid", .RecordPID, DbType.Int32)
                pars.Add("x29id", CInt(.Record_x29ID), DbType.Int32)
                s.Append(" AND a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=@x29id AND xa.x19RecordPID=@recordpid)")

            End If
            If Not .p41IDs Is Nothing Then
                If .p41IDs.Count > 0 Then s.Append(" AND a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=141 AND xa.x19RecordPID IN (" & String.Join(",", .p41IDs) & "))")
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then s.Append(" AND a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=102 AND xa.x19RecordPID IN (" & String.Join(",", .j02IDs) & "))")
            End If
            If Not .Owners Is Nothing Then
                If .Owners.Count > 0 Then s.Append(" AND a.j02ID_Owner IN (" & String.Join(",", .Owners) & ")")
            End If
            If Not .p28IDs Is Nothing Then
                If .p28IDs.Count > 0 Then s.Append(" AND a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=328 AND xa.x19RecordPID IN (" & String.Join(",", .p28IDs) & "))")
            End If
            If Not .p56IDs Is Nothing Then
                If .p56IDs.Count > 0 Then s.Append(" AND a.o23ID IN (select xa.o23ID FROM x19EntityCategory_Binding xa INNER JOIN x20EntiyToCategory xb ON xa.x20ID=xb.x20ID WHERE xb.x29ID=356 AND xa.x19RecordPID IN (" & String.Join(",", .p56IDs) & "))")
            End If
            If Not .b02IDs Is Nothing Then
                If .b02IDs.Count > 0 Then s.Append(" AND a.b02ID IN (" & String.Join(",", .b02IDs) & ")")
            End If

            If .ColumnFilteringExpression <> "" Then
                s.Append(" AND " & .ColumnFilteringExpression)
            End If
            If .SearchExpression <> "" Then
                s.Append(" AND (")
                'něco jako fulltext
                s.Append("a.o23Name LIKE '%'+@expr+'%' OR a.o23Code LIKE '%'+@expr+'%' OR a.o23FreeText01 LIKE '%'+@expr+'%' OR a.o23FreeText02 LIKE '%'+@expr+'%' OR a.o23FreeText03 LIKE '%'+@expr+'%' OR a.o23FreeText04 LIKE '%'+@expr+'%' OR a.o23FreeText05 LIKE '%'+@expr+'%' OR a.o23BigText LIKE '%'+@expr+'%'")
                s.Append(")")
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If .j70ID > 0 Then
                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    s.Append(" AND " & strQueryW)
                End If
            End If

            If .x67ID_MyRole > 0 Or .HasAnyX67Role = BO.BooleanQueryMode.TrueQuery Then
                Dim strJ11IDs As String = ""
                If _curUser.j11IDs <> "" Then strJ11IDs = "OR x69.j11ID IN (" & _curUser.j11IDs & ")"
                s.Append(" AND a.o23ID IN (")
                s.Append("SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID")
                If .x67ID_MyRole > 0 Then
                    pars.Add("x67id", .x67ID_MyRole, DbType.Int32)
                    s.Append(" WHERE x69.x67ID=@x67id AND x67.x29ID=223 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                End If
                If .HasAnyX67Role = BO.BooleanQueryMode.TrueQuery Then
                    s.Append(" WHERE x67.x29ID=223 AND (x69.j02ID=@j02id_query " & strJ11IDs & ")")
                End If
                s.Append(")")
            End If
            If .OnlySlavesPersons = BO.BooleanQueryMode.TrueQuery Then
                pars.Add("j02id_me", _curUser.j02ID, DbType.Int32)
                s.Append(" AND (a.j02ID_Owner IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me)")
                s.Append(" OR a.j02ID_Owner IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me)")
                s.Append(")")
            End If
            
            If .o23GUID <> "" Then
                s.Append(" AND a.o23GUID=@guid")
                pars.Add("guid", .o23GUID, DbType.String)
            End If
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then s.Append(" AND a.o23ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=223 AND o51ID IN (" & String.Join(",", .o51IDs) & "))")
            End If
        End With
        s.Append(bas.ParseWhereValidity("o23", "a", myQuery))
        Return bas.TrimWHERE(s.ToString)
    End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryO23) As Integer
        Dim s As String = "SELECT count(a.o23ID) as Value " & GetSQLPart2_From()
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW
        Try
            Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
        Catch ex As Exception
            Return 0
        End Try

    End Function

    
    Public Function GetDataTable4Grid(myQuery As BO.myQueryO23) As DataTable
        Dim s As String = ""
        With myQuery
            If .MG_GridSqlColumns <> "" Then .MG_GridSqlColumns += ","
            .MG_GridSqlColumns += "a.o23ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.o23ValidFrom AND a.o23ValidUntil THEN 0 else 1 END) as IsClosed"
            .MG_GridSqlColumns += ",a.o23BackColor,a.o23ForeColor,a.b02ID,b02.b02Color,a.o23IsDraft as IsDraft,convert(bit,case when o27.o27ExistInt=1 then 1 else 0 end) as IsO27,a.o23IsEncrypted,a.o23LockedFlag,x18.x18ID,x18.x18Name as DocType,a.o43ID"
            If Me.CalendarFieldStart <> "" Then .MG_GridSqlColumns += "," & Me.CalendarFieldStart & " AS CalendarDateStart"
            If Me.CalendarFieldEnd <> "" Then .MG_GridSqlColumns += "," & Me.CalendarFieldEnd & " AS CalendarDateEnd"
        End With

        Dim pars As New DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)

        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.o23ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If strORDERBY = "" Then strORDERBY = "a.o23ID DESC"
            If .MG_PageSize > 0 Then
                Dim intStart As Integer = (.MG_CurrentPageIndex) * .MG_PageSize

                s = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & .MG_GridSqlColumns & " " & GetSQLPart2_From()

                If strW <> "" Then s += " WHERE " & strW
                s += ") SELECT * FROM rst"
                pars.Add("start", intStart, DbType.Int32)
                pars.Add("end", (intStart + .MG_PageSize - 1), DbType.Int32)
                s += " WHERE RowIndex BETWEEN @start AND @end"
            Else
                'bez stránkování
                s = "SELECT " & .MG_GridSqlColumns & " " & GetSQLPart2_From()
                If strW <> "" Then s += " WHERE " & strW
                s += " ORDER BY " & strORDERBY
            End If

            
        End With

        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function
    Public Function GetRolesInline(intPID As Integer) As String
        Return _cDB.GetValueFromSQL("SELECT dbo.o23_getroles_inline(" & intPID.ToString & ") as Value")
    End Function

    ''Public Function GetList_WaitingOnReminder(datReminderFrom As Date, datReminderUntil As Date) As IEnumerable(Of BO.o23Doc)
    ''    Dim s As String = GetSQLPart1(0), pars As New DbParameters
    ''    pars.Add("datereminderfrom", datReminderFrom)
    ''    pars.Add("datereminderuntil", datReminderUntil)
    ''    s += " WHERE o23ReminderDate BETWEEN @datereminderfrom AND @datereminderuntil"
    ''    s += " AND o23ID NOT IN (SELECT x47RecordPID FROM x47EventLog WHERE x29ID=223 AND x45ID=22306)"

    ''    Return _cDB.GetList(Of BO.o23Doc)(s, pars)
    ''End Function
    ''Public Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.o23Doc)
    ''    Dim s As String = "SELECT " & GetSQLPart1(50)

    ''    Dim pars As New DbParameters
    ''    s += " WHERE a.o23ReminderDate BETWEEN @d1 AND @d2"
    ''    s += " AND (a.j02ID_Owner=@j02id OR a.o23ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=223 AND (x69.j02ID=@j02id OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))))"

    ''    pars.Add("j02id", intJ02ID, DbType.Int32)
    ''    pars.Add("d1", DateAdd(DateInterval.Day, -1, Now), DbType.DateTime)
    ''    pars.Add("d2", DateAdd(DateInterval.Day, 2, Now), DbType.DateTime)

    ''    Return _cDB.GetList(Of BO.o23Doc)(s, pars)
    ''End Function

    Public Function LockFiles(intPID As Integer, lockFlag As BO.o23LockedTypeENUM) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID)
        pars.Add("o23LockedFlag", lockFlag, DbType.Int32)
        pars.Add("o23LastLockedWhen", Now, DbType.DateTime)
        pars.Add("o23LastLockedBy", _curUser.j03Login, DbType.String)

        Return _cDB.SaveRecord("o23Doc", pars, False, "o23ID=@pid", True, _curUser.j03Login)
    End Function
    Public Function UnLockFiles(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID)
        pars.Add("o23LockedFlag", Nothing, DbType.Int32)
        pars.Add("o23LastLockedWhen", Nothing, DbType.DateTime)
        pars.Add("o23LastLockedBy", Nothing, DbType.String)

        Return _cDB.SaveRecord("o23Doc", pars, False, "o23ID=@pid", True, _curUser.j03Login)
    End Function

    ''Public Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean
    ''    Dim pars As New DbParameters()
    ''    pars.Add("pid", intPID)
    ''    pars.Add("o43ID", BO.BAS.IsNullDBKey(intO43ID), DbType.Int32)
    ''    Return _cDB.SaveRecord("o23Doc", pars, False, "o23ID=@pid", False)
    ''End Function
End Class
