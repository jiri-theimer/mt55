Public Class j02PersonDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j02Person
        Dim s As String = GetSQLPart1(0)
        s += " WHERE a.j02id=@j02id"

        Return _cDB.GetRecord(Of BO.j02Person)(s, New With {.j02id = intPID})
    End Function
    
    Public Function LoadByImapRobotAddress(strRobotAddress As String) As BO.j02Person
        Dim s As String = GetSQLPart1(1)
        s += " WHERE a.j02RobotAddress LIKE @robotkey"

        Return _cDB.GetRecord(Of BO.j02Person)(s, New With {.robotkey = strRobotAddress})
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.j02Person
        Dim s As String = GetSQLPart1(1)
        s += " WHERE a.j02ExternalPID LIKE @externalpid"
        Return _cDB.GetRecord(Of BO.j02Person)(s, New With {.externalpid = strExternalPID})
    End Function
    Public Function LoadByEmail(strEmailAddress As String, Optional intJ02ID_Exclude As Integer = 0) As BO.j02Person
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        pars.Add("email", strEmailAddress, DbType.String)
        s += " WHERE a.j02Email LIKE @email"
        If intJ02ID_Exclude <> 0 Then
            pars.Add("j02id_exclude", intJ02ID_Exclude, DbType.Int32)
            s += " AND a.j02ID<>@j02id_exclude"
        End If

        Return _cDB.GetRecord(Of BO.j02Person)(s, pars)
    End Function
    Public Function IsExistPersonByEmail(strEmailAddress As String, intJ02ID_Exclude As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("email", Trim(strEmailAddress), DbType.String)
        pars.Add("j02id_exclude", intJ02ID_Exclude, DbType.Int32)
        If _cDB.GetIntegerValueFROMSQL("select j02ID as Value FROM j02Person where j02Email LIKE @email AND j02ID<>@j02id_exclude", pars) <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function Save(cRec As BO.j02Person, lisFF As List(Of BO.FreeField)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j02id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j07ID", BO.BAS.IsNullDBKey(.j07ID), DbType.Int32)
            pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
            pars.Add("j18ID", BO.BAS.IsNullDBKey(.j18ID), DbType.Int32)
            pars.Add("c21ID", BO.BAS.IsNullDBKey(.c21ID), DbType.Int32)
            pars.Add("o40ID", BO.BAS.IsNullDBKey(.o40ID), DbType.Int32)
            pars.Add("p72ID_NonBillable", BO.BAS.IsNullDBKey(.p72ID_NonBillable), DbType.Int32)
            pars.Add("j02IsIntraPerson", .j02IsIntraPerson, DbType.Boolean)
            pars.Add("j02Email", .j02Email, DbType.String, , , True, "E-mail adresa")
            pars.Add("j02EmailSignature", .j02EmailSignature, DbType.String, , , True, "E-mail podpis")
            pars.Add("j02RobotAddress", .j02RobotAddress, DbType.String)
            pars.Add("j02firstname", .j02FirstName, DbType.String, , , True, "Jméno")
            pars.Add("j02lastname", .j02LastName, DbType.String, , , True, "Příjmení")
            pars.Add("j02titleaftername", .j02TitleAfterName, DbType.String, , , True, "Titul Za")
            pars.Add("j02titlebeforename", .j02TitleBeforeName, DbType.String, , , True, "Titul")
            pars.Add("j02phone", .j02Phone, DbType.String, , , True, "Telefon")
            pars.Add("j02JobTitle", .j02JobTitle, DbType.String, , , True, "Pracovní pozice")
            pars.Add("j02Code", .j02Code, DbType.String, , , True, "Osobní číslo")
            pars.Add("j02Office", .j02Office, DbType.String, , , True, "Kancelář")
            pars.Add("j02mobile", .j02Mobile, DbType.String, , , True, "Mobil")
            pars.Add("j02DomainAccount", .j02DomainAccount, DbType.String)
            pars.Add("j02ExternalPID", .j02ExternalPID, DbType.String)
            pars.Add("j02Description", .j02Description, DbType.String, , , True, "Poznámka")
            pars.Add("j02TimesheetEntryDaysBackLimit", .j02TimesheetEntryDaysBackLimit, DbType.Int32)
            pars.Add("j02TimesheetEntryDaysBackLimit_p34IDs", .j02TimesheetEntryDaysBackLimit_p34IDs, DbType.String)
            pars.Add("j02Salutation", .j02Salutation, DbType.String, , , True, "Oslovení")
            pars.Add("j02AvatarImage", .j02AvatarImage, DbType.String)
            pars.Add("j02IsInvoiceEmail", .j02IsInvoiceEmail, DbType.Boolean)
            pars.Add("j02validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("j02validuntil", .ValidUntil, DbType.DateTime)

            
            pars.Add("j02WorksheetAccessFlag", .j02WorksheetAccessFlag, DbType.Int32)
        End With

        If _cDB.SaveRecord("j02Person", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedJ02ID As Integer = _cDB.LastSavedRecordPID
            If Not lisFF Is Nothing Then    'volná pole
                bas.SaveFreeFields(_cDB, lisFF, "j02Person_FreeField", intLastSavedJ02ID)
            End If
            pars = New DbParameters
            With pars
                .Add("j02id", _cDB.LastSavedRecordPID, DbType.Int32)
                .Add("j03id_sys", _curUser.PID, DbType.Int32)
            End With
            _cDB.RunSP("j02_aftersave", pars)

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
        Return _cDB.RunSP("j02_delete", pars)
    End Function

    Private Function GetSQLWHERE(myQuery As BO.myQueryJ02, ByRef pars As DL.DbParameters) As String
        Dim strW As String = bas.ParseWhereMultiPIDs("a.j02ID", myQuery)
        strW += bas.ParseWhereValidity("j02", "a", myQuery)
        With myQuery
            Select Case .IntraPersons
                Case BO.myQueryJ02_IntraPersons.IntraOnly
                    strW += " AND a.j02IsIntraPerson=1"
                Case BO.myQueryJ02_IntraPersons.NonIntraOnly
                    strW += " AND a.j02IsIntraPerson=0"
            End Select

            If Not _curUser.IsAdmin Then
                pars.Add("j02id_me", _curUser.j02ID, DbType.Int32)
                Select Case .SpecificQuery
                    Case BO.myQueryJ02_SpecificQuery.AllowedForRead
                        If _curUser.IsMasterPerson Then
                            strW += " AND (a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me)"
                            strW += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me)"
                            strW += " OR a.j02ID=@j02id_me OR a.j02IsIntraPerson=0)"
                        Else
                            strW += " AND (a.j02ID=@j02id_me OR a.j02IsIntraPerson=0)"
                        End If
                    Case BO.myQueryJ02_SpecificQuery.AllowedForWorksheetEntry
                        If _curUser.IsMasterPerson Then
                            strW += " AND (a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me AND j05IsCreate_p31=1)"
                            strW += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me AND xj05.j05IsCreate_p31=1)"
                            strW += " OR a.j02ID=@j02id_me)"
                            pars.Add("j02id_me", _curUser.j02ID, DbType.Int32)
                        Else
                            strW += " AND a.j02ID=@j02id_me"
                        End If
                    Case BO.myQueryJ02_SpecificQuery.AllowedForP48Entry
                        If _curUser.IsMasterPerson Then
                            strW += " AND (a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me AND j05IsCreate_p48=1)"
                            strW += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me AND xj05.j05IsCreate_p48=1)"
                            strW += " OR a.j02ID=@j02id_me)"
                            pars.Add("j02id_me", _curUser.j02ID, DbType.Int32)
                        Else
                            strW += " AND a.j02ID=@j02id_me"
                        End If
                End Select
            End If
            If .j70ID > 0 Then
                pars.Add("dp31f1", DateSerial(2000, 1, 1))
                pars.Add("dp31f2", DateSerial(3000, 1, 1))

                Dim strQueryW As String = bas.CompleteSqlJ70(_cDB, .j70ID, _curUser)
                If strQueryW <> "" Then
                    strW += " AND " & strQueryW
                End If
            End If
            If .QuickQuery > BO.myQueryJ02_QuickQuery._NotSpecified Then
                strW += " AND " & bas.GetQuickQuerySQL_j02(.QuickQuery)
            End If
            
            
            If .j04ID <> 0 Then
                pars.Add("j04id", .j04ID, DbType.Int32)
                strW += " AND a.j02ID IN (select j02ID FROM j03User WHERE j04ID=@j04id)"
            End If
            If .j11ID <> 0 Then
                pars.Add("j11id", .j11ID, DbType.Int32)
                strW += " AND a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)"
            End If
            If .j07ID <> 0 Then
                pars.Add("j07id", .j07ID, DbType.Int32) : strW += " AND a.j07ID=@j07id"
            End If
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                strW += " AND a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p41ID=@p41id OR p28ID IN (SELECT p28ID_Client FROM p41Project WHERE p41ID=@p41id AND p28ID_Client IS NOT NULL))"
            End If
            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p28ID=@p28id OR p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id))"
            End If
            If .p91ID <> 0 Then
                pars.Add("p91id", .p91ID, DbType.Int32)
                strW += " AND a.j02ID IN (select a1.j02ID FROM p30Contact_Person a1 INNER JOIN p91Invoice a2 ON a1.p28ID=a2.p28ID WHERE a2.p91ID=@p91id)"
            End If

            If Not .DateInsertFrom Is Nothing Then
                If Year(.DateInsertFrom) > 1900 Then
                    pars.Add("d1", .DateInsertFrom) : pars.Add("d2", .DateInsertUntil)
                    strW += " AND a.j02DateInsert BETWEEN @d1 AND @d2"
                End If
            End If
            If Not .p31Date_D1 Is Nothing Then
                If Year(.p31Date_D1) > 1900 Then
                    pars.Add("dp31f1", .p31Date_D1)
                    pars.Add("dp31f2", .p31Date_D2)
                    strW += " AND a.j02ID IN (SELECT j02ID FROM p31Worksheet WHERE p31Date BETWEEN @dp31f1 AND @dp31f2)"
                End If
            End If
            If .ColumnFilteringExpression <> "" Then
                strW += " AND " & .ColumnFilteringExpression
            End If
            If .SearchExpression <> "" Then
                strW += " AND (a.j02firstname like @expr+'%' OR a.j02LastName LIKE '%'+@expr+'%' OR a.j02Email LIKE '%'+@expr+'%')"
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If .MG_AdditionalSqlWHERE <> "" Then strW += " AND " & .MG_AdditionalSqlWHERE
            If Not .o51IDs Is Nothing Then
                If .o51IDs.Count > 0 Then strW += " AND a.j02ID IN (SELECT o52RecordPID FROM o52TagBinding WHERE x29ID=102 AND o51ID IN (" & String.Join(",", .o51IDs) & "))"
            End If
            If .x18Value <> "" Then
                strW += bas.CompleteX18QuerySql("j02", .x18Value)
            End If
        End With
        Return bas.TrimWHERE(strW)
    End Function

    Public Function GetList_j11(intJ02ID As Integer) As IEnumerable(Of BO.j11Team)
        Dim pars As New DbParameters
        pars.Add("j02id", intJ02ID, DbType.Int32)
        Return _cDB.GetList(Of BO.j11Team)("select *," & bas.RecTail("j11") & " FROM j11team WHERE j11id IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)", pars)
    End Function
    Public Function GetList_j02_join_j11(j02ids As List(Of Integer), j11ids As List(Of Integer)) As IEnumerable(Of BO.j02Person)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        If j02ids.Count = 0 Then j02ids.Add(-1)

        s += " WHERE (a.j02ID IN (" & String.Join(",", j02ids) & ")"
        If j11ids.Count > 0 Then s += " OR a.j02ID IN (SELECT j02ID FROM j12Team_Person WHERE j11ID IN (" & String.Join(",", j11ids) & "))"
        s += ")"
        s += " ORDER BY a.j02LastName,a.j02FirstName"
        Return _cDB.GetList(Of BO.j02Person)(s, pars)
    End Function
    Public Function GetList_Slaves(intJ02ID As Integer, bolDispCreateP31 As Boolean, dispP31 As BO.j05Disposition_p31ENUM, bolDispCreateP48 As Boolean, dispP48 As BO.j05Disposition_p48ENUM) As IEnumerable(Of BO.j02Person)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        Dim strW As String = ""
        If dispP31 > BO.j05Disposition_p31ENUM._NotSpecified Then strW += " OR j05Disposition_p31=" & CInt(dispP31).ToString
        If dispP48 > BO.j05Disposition_p48ENUM._NotSpecified Then strW += " OR j05Disposition_p48=" & CInt(dispP48).ToString
        If bolDispCreateP31 Then strW += " OR j05IsCreate_p31=1"
        If bolDispCreateP48 Then strW += " OR j05IsCreate_p48=1"
        If strW <> "" Then strW = "(" & Right(strW, Len(strW) - 4) & ")"

        pars.Add("j02id_me", intJ02ID, DbType.Int32)
        s += " WHERE (a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me AND " & strW & ")"
        s += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me AND " & strW & ")"
        s += ")"
        s += " ORDER BY a.j02LastName,a.j02FirstName"
        Return _cDB.GetList(Of BO.j02Person)(s, pars)
    End Function
    Public Function GetList_Masters(intJ02ID As Integer) As IEnumerable(Of BO.j02Person)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        pars.Add("j02id_me", intJ02ID, DbType.Int32)
        Dim strW As String = "a.j02ID IN (SELECT j02ID_Master FROM j05MasterSlave WHERE j02ID_Slave=@j02id_me)"
        strW += " OR a.j02ID IN (SELECT xj05.j02ID_Master FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE j12.j02ID=@j02id_me)"
        strW = "(" & strW & ")"
        s += " WHERE " & strW
        s += " ORDER BY a.j02LastName,a.j02FirstName"
        Return _cDB.GetList(Of BO.j02Person)(s, pars)
    End Function
    Public Function GetTeamsInLine(intJ02ID As Integer) As String
        Return _cDB.GetValueFromSQL("SELECT dbo.j02_teams_inline(" & intJ02ID.ToString & ")")
    End Function

    Public Function GetGridDataSource(myQuery As BO.myQueryJ02) As DataTable
        Dim s As String = ""
        With myQuery
            If Not System.String.IsNullOrEmpty(.MG_GridGroupByField) Then
                If .MG_GridSqlColumns.ToLower.IndexOf(.MG_GridGroupByField.ToLower) < 0 And .MG_GridGroupByField <> "" Then
                    Select Case .MG_GridGroupByField
                        Case Else
                            .MG_GridSqlColumns += "," & .MG_GridGroupByField
                    End Select
                End If
            End If
            
            .MG_GridSqlColumns += ",a.j02ID as pid,CONVERT(BIT,CASE WHEN GETDATE() BETWEEN a.j02ValidFrom AND a.j02ValidUntil THEN 0 else 1 END) as IsClosed,a.j02IsIntraPerson as IsIntraPerson"
        End With
        
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            If .MG_SelectPidFieldOnly Then .MG_GridSqlColumns = "a.j02ID as pid"
            Dim strORDERBY As String = .MG_SortString
            If .MG_GridGroupByField <> "" Then
                Dim strPrimarySortField As String = .MG_GridGroupByField
                If strPrimarySortField = "FullNameDesc" Then strPrimarySortField = "a.j02LastName+char(32)+a.j02FirstName"
                If strPrimarySortField = "FullNameAsc" Then strPrimarySortField = "a.j02FirstName+char(32)+a.j02LastName"
                If strORDERBY = "" Or LCase(strPrimarySortField) = Replace(Replace(LCase(.MG_SortString), " desc", ""), " asc", "") Then
                    strORDERBY = strPrimarySortField
                Else
                    strORDERBY = strPrimarySortField & "," & .MG_SortString
                End If
            End If
            If strORDERBY = "" Then strORDERBY = "j02lastname,j02firstname"

            Dim strFROM As String = "FROM j02Person a LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID LEFT OUTER JOIN c21FondCalendar c21 ON a.c21ID=c21.c21ID LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID"
            If .MG_AdditionalSqlFROM <> "" Then
                strFROM += " " & .MG_AdditionalSqlFROM
            End If
            If .MG_PageSize > 0 Then
                Dim intStart As Integer = (.MG_CurrentPageIndex) * .MG_PageSize

                s = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex," & .MG_GridSqlColumns & " " & strFROM
                If strW <> "" Then s += " WHERE " & strW

                s += ") SELECT * FROM rst"
                pars.Add("start", intStart, DbType.Int32)
                pars.Add("end", (intStart + .MG_PageSize - 1), DbType.Int32)
                s += " WHERE RowIndex BETWEEN @start AND @end"
            Else
                'bez stránkování
                s = "SELECT " & .MG_GridSqlColumns & " " & strFROM
                If strW <> "" Then s += " WHERE " & strW
                s += " ORDER BY " & strORDERBY
            End If

        End With

        Dim ds As DataSet = _cDB.GetDataSet(s, , pars.Convert2PluginDbParameters())
        If Not ds Is Nothing Then Return ds.Tables(0) Else Return Nothing
    End Function

    Public Overloads Function GetList(myQuery As BO.myQueryJ02) As IEnumerable(Of BO.j02Person)
        Return GetListProc(myQuery)
    End Function

    Private Function GetListProc(myQuery As BO.myQueryJ02) As IEnumerable(Of BO.j02Person)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly)
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            If strSort = "" Then strSort = "j02lastname,j02firstname"

            If .MG_PageSize <> 0 Then
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
        Return _cDB.GetList(Of BO.j02Person)(s, pars)
    End Function
    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "j02UserInsert").Replace("UserUpdate", "j02UserUpdate").Replace("DateInsert", "j02DateInsert").Replace("DateUpdate", "j02DateUpdate")
        strSort = strSort.Replace("FullNameDesc", "j02LastName").Replace("FullNameAsc", "j02LastName")
        Return bas.NormalizeOrderByClause(strSort)
    End Function
    Private Function ParseFilterExpression(strFilter As String) As String
        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryJ02) As Integer
        Dim s As String = "SELECT count(a.j02ID) as Value FROM j02Person a"
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function

    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.j07ID,a.j17ID,a.j18ID,a.c21ID,a.j02IsIntraPerson,a.j02FirstName,a.j02LastName,a.j02TitleBeforeName,a.j02TitleAfterName,a.j02Code,a.j02JobTitle,a.j02Email,a.j02Mobile,a.j02Phone,a.j02Office,a.j02EmailSignature,a.j02Description,a.j02AvatarImage,a.j02WorksheetAccessFlag,a.o40ID,a.j02DomainAccount"
        s += ",j07.j07Name as _j07Name,c21.c21Name as _c21Name,j18.j18Name as _j18Name,a.j02RobotAddress,a.j02ExternalPID,a.j02TimesheetEntryDaysBackLimit,a.j02TimesheetEntryDaysBackLimit_p34IDs,a.j02Salutation,a.p72ID_NonBillable,a.j02IsInvoiceEmail," & bas.RecTail("j02", "a")
        s += " FROM j02Person a LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID LEFT OUTER JOIN c21FondCalendar c21 ON a.c21ID=c21.c21ID LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID"

        Return s
    End Function
    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize
        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex"
        s += ",a.*,j07.j07Name as _j07Name," & bas.RecTail("j02", "a")
        s += " FROM j02Person a LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID"

        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " * FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"
        s += " ORDER BY " & strORDERBY
        Return s
    End Function
  

    Public Function GetList_AllAssignedEntityRoles(intPID As Integer, x29id_entity As BO.x29IdEnum) As IEnumerable(Of BO.x67EntityRole)
        Dim s As String = "select *," & bas.RecTail("x67") & " FROM x67EntityRole WHERE x29ID=@x29id AND x67ID IN (select a.x67ID FROM x69EntityRole_Assign a LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID WHERE a.j02ID=@j02id OR a.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id))"

        Dim pars As New DbParameters
        pars.Add("x29id", x29id_entity, DbType.Int32)
        pars.Add("j02id", intPID, DbType.Int32)


        Return _cDB.GetList(Of BO.x67EntityRole)(s, pars)

    End Function

    Public Function LoadSumRow(intPID As Integer) As BO.j02PersonSum
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
        End With
        Return _cDB.GetRecord(Of BO.j02PersonSum)("j02_inhale_sumrow", pars, True)
    End Function

    Public Function SavePersonalPlan(lisP66 As List(Of BO.p66PersonalPlan)) As Boolean
        Dim mq As New BO.myQueryP66
        If lisP66.Count > 0 Then
            mq.D1 = lisP66.Select(Function(p) p.p66DateFrom).Min
            mq.D2 = lisP66.Select(Function(p) p.p66DateFrom).Max
        End If
        
        Dim lisSaved As IEnumerable(Of BO.p66PersonalPlan) = GetList_p66(mq)
        For Each c In lisP66
            Dim pars As New DbParameters, strW As String = "", bolINSERT As Boolean = True
            If c.PID <> 0 Then
                strW = "p66ID=@pid"
                pars.Add("pid", c.PID, DbType.Int32)
                bolINSERT = False
            End If
            With c
                pars.Add("j02id", .j02ID, DbType.Int32)
                pars.Add("p66VersionIndex", .p66VersionIndex, DbType.Int32)
                pars.Add("p66DateFrom", .p66DateFrom, DbType.DateTime)
                pars.Add("p66DateUntil", .p66DateUntil, DbType.DateTime)
                pars.Add("p66HoursBillable", .p66HoursBillable, DbType.Double)
                pars.Add("p66HoursNonBillable", .p66HoursNonBillable, DbType.Double)
                pars.Add("p66HoursTotal", .p66HoursTotal, DbType.Double)
                pars.Add("p66HoursInvoiced", .p66HoursInvoiced, DbType.Double)
                pars.Add("p66FreeNumber01", .p66FreeNumber01, DbType.Double)
                pars.Add("p66FreeNumber02", .p66FreeNumber02, DbType.Double)
                pars.Add("p66FreeNumber03", .p66FreeNumber03, DbType.Double)
                pars.Add("p66FreeNumber04", .p66FreeNumber04, DbType.Double)
                pars.Add("p66FreeNumber05", .p66FreeNumber05, DbType.Double)
            End With

            If Not _cDB.SaveRecord("p66PersonalPlan", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Function GetList_p66(mq As BO.myQueryP66) As IEnumerable(Of BO.p66PersonalPlan)
        Dim s As String = "SELECT a.*," & bas.RecTail("p66", "a") & ",j02.j02LastName+' '+j02.j02FirstName as Person FROM p66PersonalPlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID", pars As New DbParameters
        pars.Add("d1", mq.D1, DbType.DateTime)
        pars.Add("d2", mq.D2, DbType.DateTime)
        s += " WHERE p66DateFROM BETWEEN @d1 AND @d2"
        If mq.VersionIndex > 0 Then
            pars.Add("version", mq.VersionIndex, DbType.Int32)
            s += " AND p66VersionIndex=@version"
        End If
        If mq.j11ID <> 0 Then
            pars.Add("j11id", mq.j11ID, DbType.Int32)
            s += " AND a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)"
        End If

        If Not mq.j02IDs Is Nothing Then
            If mq.j02IDs.Count > 0 Then
                s += " AND a.j02ID IN (" & String.Join(",", mq.j02IDs) & ")"
            End If
        End If

        Return _cDB.GetList(Of BO.p66PersonalPlan)(s, pars)
    End Function
End Class
