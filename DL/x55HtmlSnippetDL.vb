Public Class x55HtmlSnippetDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x55HtmlSnippet
        Dim s As String = GetSQLPart1() & " WHERE a.x55ID=@pid"
        Return _cDB.GetRecord(Of BO.x55HtmlSnippet)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByCode(strCode As String) As BO.x55HtmlSnippet
        Dim s As String = GetSQLPart1() & " WHERE a.x55Code LIKE @code"
        Return _cDB.GetRecord(Of BO.x55HtmlSnippet)(s, New With {.code = strCode})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x55_delete", pars)

    End Function
    Public Function Save(cRec As BO.x55HtmlSnippet, lisX56 As List(Of BO.x56SnippetProperty)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x55ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x55TypeFlag", .x55TypeFlag, DbType.Int32)
            pars.Add("x55IsSystem", .x55IsSystem, DbType.Boolean)
            pars.Add("x55Name", .x55Name, DbType.String, , , True, "Název")
            pars.Add("x55Content", .x55Content, DbType.String)
            pars.Add("x55RecordSQL", .x55RecordSQL, DbType.String, , , True, "SQL zdroje záznamu")
            pars.Add("x55Code", .x55Code, DbType.String, , , True, "Kód")
            pars.Add("x55Height", .x55Height, DbType.String)
            pars.Add("x55Ordinary", .x55Ordinary, DbType.Int32)
            pars.Add("x55ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x55ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x55HtmlSnippet", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intX55ID As Integer = _cDB.LastSavedRecordPID
            If Not lisX56 Is Nothing Then
                _cDB.RunSQL("DELETE FROM x56SnippetProperty WHERE x55ID=" & intX55ID.ToString)
                For Each c In lisX56
                    pars = New DbParameters
                    pars.Add("x55id", intX55ID, DbType.Int32)
                    pars.Add("x56Control", c.x56Control, DbType.String)
                    pars.Add("x56ControlPropertyName", c.x56ControlPropertyName, DbType.String)
                    pars.Add("x56ControlPropertyValue", c.x56ControlPropertyValue, DbType.String)
                    _cDB.RunSQL("INSERT INTO x56SnippetProperty(x55ID,x56Control,x56ControlPropertyName,x56ControlPropertyValue) VALUES(@x55id,@x56Control,@x56ControlPropertyName,@x56ControlPropertyValue)", pars)
                Next
            End If
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x55HtmlSnippet)
        Dim s As String = GetSQLPart1()
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.x55ID", myQuery)
            strW += bas.ParseWhereValidity("x55", "a", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY x55Ordinary"

        Return _cDB.GetList(Of BO.x55HtmlSnippet)(s)

    End Function

    Public Function GetList_Properties(intPID As Integer) As IEnumerable(Of BO.x56SnippetProperty)
        Dim s As String = "select * FROM x56SnippetProperty"
        If intPID > 0 Then
            s += " WHERE x55ID=@pid"
        End If

        Return _cDB.GetList(Of BO.x56SnippetProperty)(s, New With {.pid = intPID})
    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("x55", "a")
        s += " FROM x55HtmlSnippet a"

        Return s
    End Function
End Class
