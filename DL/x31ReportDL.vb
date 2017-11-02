Public Class x31ReportDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x31Report
        Dim s As String = GetSQLPart1()
        s += " WHERE a.x31ID=@x31id"

        Return _cDB.GetRecord(Of BO.x31Report)(s, New With {.x31id = intPID})
    End Function
    Public Function LoadByFilename(strFileName As String) As BO.x31Report
        Dim s As String = GetSQLPart1()
        s += " WHERE a.x31FileName LIKE @filename"

        Return _cDB.GetRecord(Of BO.x31Report)(s, New With {.filename = strFileName})
    End Function
    Public Function LoadByCode(strCode As String) As BO.x31Report
        Dim s As String = GetSQLPart1()
        s += " WHERE a.x31Code LIKE @code"

        Return _cDB.GetRecord(Of BO.x31Report)(s, New With {.code = strCode})
    End Function

    Public Function Save(cRec As BO.x31Report, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x31ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("j25ID", BO.BAS.IsNullDBKey(.j25ID), DbType.Int32)
            pars.Add("x31Code", .x31Code, DbType.String, , , True, "Kód šablony")
            pars.Add("x31Name", .x31Name, DbType.String, , , True, "Název šablony")
            pars.Add("x31FormatFlag", .x31FormatFlag, DbType.Int32)
            pars.Add("x31IsPeriodRequired", .x31IsPeriodRequired, DbType.Boolean)
            pars.Add("x31IsUsableAsPersonalPage", .x31IsUsableAsPersonalPage, DbType.Boolean)
            pars.Add("x31IsScheduling", .x31IsScheduling, DbType.Boolean)
            pars.Add("x31Ordinary", .x31Ordinary, DbType.Int32)
            pars.Add("x31Description", .x31Description, DbType.String)
            pars.Add("x31IsRunInDay1", .x31IsRunInDay1, DbType.Boolean)
            pars.Add("x31IsRunInDay2", .x31IsRunInDay2, DbType.Boolean)
            pars.Add("x31IsRunInDay3", .x31IsRunInDay3, DbType.Boolean)
            pars.Add("x31IsRunInDay4", .x31IsRunInDay4, DbType.Boolean)
            pars.Add("x31IsRunInDay5", .x31IsRunInDay5, DbType.Boolean)
            pars.Add("x31IsRunInDay6", .x31IsRunInDay6, DbType.Boolean)
            pars.Add("x31IsRunInDay7", .x31IsRunInDay7, DbType.Boolean)
            pars.Add("x31RunInTime", .x31RunInTime, DbType.String)
            pars.Add("x31SchedulingReceivers", .x31SchedulingReceivers, DbType.String)
            pars.Add("x31DocSqlSource", .x31DocSqlSource, DbType.String)
            pars.Add("x31DocSqlSourceTabs", .x31DocSqlSourceTabs, DbType.String)
            pars.Add("x31ExportFileNameMask", .x31ExportFileNameMask, DbType.String)
            pars.Add("x31QueryFlag", .x31QueryFlag, DbType.Int32)
            pars.Add("x31PluginFlag", .x31PluginFlag, DbType.Int32)
            pars.Add("x31PluginHeight", .x31PluginHeight, DbType.Int32)

            pars.Add("x31validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("x31validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("x31Report", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedP56ID As Integer = _cDB.LastSavedRecordPID
            If Not lisX69 Is Nothing Then   'přiřazení rolí úlohy
                bas.SaveX69(_cDB, BO.x29IdEnum.x31Report, intLastSavedP56ID, lisX69, bolINSERT)
            End If
            Return True
        Else
            Return False
        End If

    End Function

    Public Function UpdateReportFileName(intPID As Integer, strReportFileName As String) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID)
        Return _cDB.RunSQL("UPDATE x31Report SET x31FileName=" & BO.BAS.GS(strReportFileName) & " WHERE x31ID=@pid", pars)
    End Function
    Public Function UpdateLastScheduledRun(intPID As Integer, dat As Date?) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intPID)
        pars.Add("d", dat, DbType.DateTime)

        Return _cDB.RunSQL("UPDATE x31Report SET x31LastScheduledRun=@d WHERE x31ID=@pid", pars)
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x31_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x31Report)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = ""
        If Not myQuery Is Nothing Then
            strW = bas.ParseWhereMultiPIDs("a.x31ID", myQuery)
            strW += bas.ParseWhereValidity("x31", "", myQuery)

        End If
        If Not BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_Admin) Then
            strW += " AND a.X31ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=931 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))"
            pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
        End If
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY j25Ordinary,x31Ordinary,x31Name"

        Return _cDB.GetList(Of BO.x31Report)(s, pars)

    End Function

   

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,x29.x29Name as _x29Name,j25.j25Name as _j25Name,j25.j25Ordinary as _j25Ordinary,o27.o27ArchiveFileName as _ReportFileName,o27.o27ArchiveFolder as _ReportFolder," & bas.RecTail("x31", "a")
        s += " FROM x31Report a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID LEFT OUTER JOIN j25ReportCategory j25 ON a.j25ID=j25.j25ID"
        s += " LEFT OUTER JOIN (SELECT x31ID,o27ArchiveFolder,o27ArchiveFileName FROM o27Attachment WHERE x31ID IS NOT NULL) o27 ON a.x31ID=o27.x31ID"
        Return s
    End Function
End Class
