Public Class x47EventLogDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x47EventLog
        Dim s As String = GetSQLPart1(0, False)
        s += " WHERE a.x47ID=@x47id"

        Return _cDB.GetRecord(Of BO.x47EventLog)(s, New With {.x47id = intPID})
    End Function
   
    Public Function Create(cRec As BO.x47EventLog) As Integer
        Dim pars As New DbParameters()

        With cRec
            pars.Add("j03ID", _curUser.PID, DbType.Int32)
            pars.Add("x45ID", BO.BAS.IsNullDBKey(.x45ID), DbType.Int32)
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("x29ID_Reference", BO.BAS.IsNullDBKey(.x29ID_Reference), DbType.Int32)
            pars.Add("x47RecordPID", BO.BAS.IsNullDBKey(.x47RecordPID), DbType.Int32)
            pars.Add("x47RecordPID_Reference", BO.BAS.IsNullDBKey(.x47RecordPID_Reference), DbType.Int32)
            pars.Add("x47Name", .x47Name, DbType.String)
            pars.Add("x47NameReference", .x47NameReference, DbType.String)
            pars.Add("x47Description", .x47Description, DbType.String)
            pars.Add("@ret_x47id", , DbType.Int32, ParameterDirection.Output)
        End With
        If _cDB.RunSP("x47_appendlog", pars) Then
            Return pars.Get(Of Int32)("ret_x47id")
        Else
            Return 0
        End If

        'If _cDB.SaveRecord("x47EventLog", pars, True, strW, True, _curUser.j03Login) Then
        '    Return True
        'Else
        '    Return False
        'End If

    End Function

    


    Public Function GetList(myQuery As BO.myQueryX47, Optional intTopRecs As Integer = 0) As IEnumerable(Of BO.x47EventLog)
        Dim s As String = GetSQLPart1(intTopRecs, myQuery.IsShowTagsInColumn), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.x47ID", myQuery)
        With myQuery
            If Year(.DateFrom) > 1900 Or Year(.DateUntil) < 3000 Then
                pars.Add("d1", .DateFrom, DbType.DateTime)
                pars.Add("d2", .DateUntil, DbType.DateTime)
                strW += " AND a.x47DateInsert BETWEEN @d1 AND @d2"
            End If

            If Not .x45ID Is Nothing Then
                pars.Add("x45id", .x45ID, DbType.Int32)
                strW += " AND a.x45ID=@x45id"
            End If
            If .x45IDs <> "" Then
                strW += " AND a.x45ID IN (" & .x45IDs & ")"
                strW += " AND a.x47RecordPID NOT IN (SELECT xa.x47RecordPID FROM x47EventLog xa WHERE xa.x47RecordPID=a.x47RecordPID AND xa.x29ID=a.x29ID AND xa.x45ID IN (14105,32805,35605,39105,10205,22305))"
            End If
            If Not .x29ID Is Nothing Then
                If .x29ID > BO.x29IdEnum._NotSpecified Then
                    pars.Add("x29id", .x29ID, DbType.Int32)
                    strW += " AND (a.x29ID=@x29id OR a.x29ID_Reference=@x29id)"
                End If

            End If
            If .RecordPID > 0 Then
                pars.Add("recordpid", .RecordPID, DbType.Int32)
                strW += " AND (a.x47RecordPID=@recordpid OR a.x47RecordPID_Reference=@recordpid)"
            End If

            If .j03ID > 0 Then
                pars.Add("j03id", .j03ID, DbType.Int32)
                strW += " AND a.j03ID=@j03id"
            End If
        End With


        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.x47ID DESC"

        Return _cDB.GetList(Of BO.x47EventLog)(s, pars)

    End Function


    Private Function GetSQLPart1(intTOP As Integer, bolIncludeTags As Boolean) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*,x45.x45Name as _x45Name,j02.j02LastName+' '+j02.j02FirstName as _Person,x45.x45IsAllowNotification as _x45IsAllowNotification"
        If bolIncludeTags Then s += ",dbo.tag_values_inline_html(a.x29ID,a.x47RecordPID) as TagsInlineHtml" Else s += ",NULL as TagsInlineHtml"
        s += " FROM x47EventLog a INNER JOIN x45Event x45 ON a.x45ID=x45.x45ID"
        s += " INNER JOIN j03User j03 ON a.j03ID=j03.j03ID"
        s += " LEFT OUTER JOIN j02Person j02 ON j03.j02ID=j02.j02ID"

        Return s
    End Function
    Public Function GetObjectAlias(x29id As BO.x29IdEnum, intRecordPID As Integer) As String
        Dim s As String = "SELECT dbo.GetObjectAlias('" & BO.BAS.GetDataPrefix(x29id) & "'," & intRecordPID.ToString & ") as Value"
        Return _cDB.GetRecord(Of BO.GetString)(s).Value
    End Function
   
End Class
