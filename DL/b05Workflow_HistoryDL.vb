Public Class b05Workflow_HistoryDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub

    Public Function Load(intPID) As BO.b05Workflow_History
        Dim s As String = GetSQLPart1(1)
        s += " WHERE a.b05ID=@pid"
        Return _cDB.GetRecord(Of BO.b05Workflow_History)(s, New With {.pid = intPID})
    End Function
    

    Public Function Save(cRec As BO.b05Workflow_History) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("b05RecordPID", BO.BAS.IsNullDBKey(cRec.b05RecordPID), DbType.Int32)
            .Add("x29ID", BO.BAS.IsNullDBKey(CInt(cRec.x29ID)), DbType.Int32)
            .Add("b07ID", BO.BAS.IsNullDBKey(cRec.b07ID), DbType.Int32)
            .Add("b02ID_From", BO.BAS.IsNullDBKey(cRec.b02ID_From), DbType.Int32)
            .Add("b02ID_To", BO.BAS.IsNullDBKey(cRec.b02ID_To), DbType.Int32)
            .Add("b06ID", BO.BAS.IsNullDBKey(cRec.b06ID), DbType.Int32)
            .Add("b05IsManualStep", cRec.b05IsManualStep, DbType.Boolean)
            .Add("b05IsCommentOnly", cRec.b05IsCommentOnly, DbType.Boolean)
            .Add("b05DateInsert", Now, DbType.DateTime)
            .Add("b05UserInsert", _curUser.j03Login, DbType.String)
            .Add("b05SQL", cRec.b05SQL, DbType.String)
        End With

        If _cDB.SaveRecord("b05Workflow_History", pars, True, , False, _curUser.j03Login) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
    

    Public Function GetList(intRecordPID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.b05Workflow_History)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters
        pars.Add("pid", intRecordPID)
        pars.Add("x29id", CInt(x29id))
        s += " WHERE a.b05RecordPID=@pid AND a.x29ID=@x29id"
        
        s += " ORDER BY a.b05ID DESC"
        Return _cDB.GetList(Of BO.b05Workflow_History)(s, pars)

    End Function

    Private Function GetSQLPart1(Optional intTopRecs As Integer = 0) As String
        Dim s As String = "SELECT "
        If intTopRecs > 0 Then s += " TOP " & intTopRecs.ToString
        s += " a.*," & bas.RecTail("b05", "a", False, False) & ",b06.b06Name as _b06Name,b02from.b02name as _b02name_from,b02to.b02name as _b02name_to"
        s += ",j03.j03Login as _j03login,j02.j02LastName as _j02lastname,j02.j02FirstName as _j02firstname,a.b05UserInsert,a.b05DateInsert" '',o27.o27ID as _o27ID
        s += " FROM b05Workflow_History a LEFT OUTER JOIN b06WorkflowStep b06 on a.b06ID=b06.b06ID"
        s += " LEFT OUTER JOIN j03user j03 on a.j03ID_Sys=j03.j03ID"
        s += " LEFT OUTER JOIN j02Person j02 on j03.j02ID=j02.j02ID"
        s += " LEFT OUTER JOIN b02WorkflowStatus b02from on a.b02ID_From=b02from.b02ID"
        s += " LEFT OUTER JOIN b02WorkflowStatus b02to on a.b02ID_To=b02to.b02ID"

        Return s
    End Function
End Class
