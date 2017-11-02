Public Class b65WorkflowMessageDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.b65WorkflowMessage
        Dim s As String = "select a.*,b01.b01Name as _b01Name," & bas.RecTail("b65", "a") & " FROM b65WorkflowMessage a LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID WHERE a.b65ID=@b65id"
        Return _cDB.GetRecord(Of BO.b65WorkflowMessage)(s, New With {.b65id = intPID})
    End Function
    

    Public Function Save(cRec As BO.b65WorkflowMessage) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "b65id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
            pars.Add("b65Name", .b65Name, DbType.String, , , True, "Název šablony")
            pars.Add("b65MessageSubject", .b65MessageSubject, DbType.String, , , True, "Předmět zprávy")
            pars.Add("b65MessageBody", .b65MessageBody, DbType.String, , , True, "Tělo zprávy")
            pars.Add("b65validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("b65validuntil", .ValidUntil, DbType.DateTime)
        End With


        If _cDB.SaveRecord("b65WorkflowMessage", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        If _cDB.RunSP("b65_delete", pars) Then
            If pars.Get(Of String)("err_ret") <> "" Then
                _Error = pars.Get(Of String)("err_ret")
                Return False
            End If
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function

    Public Function GetList(intB01ID As Integer, Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.b65WorkflowMessage)
        Dim s As String = "select a.*,b01.b01Name as _b01Name," & bas.RecTail("b65", "a")
        s += " FROM b65WorkflowMessage a LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID"
        s += " WHERE a.b01ID=@b01id"
        Dim pars As New DbParameters
        pars.Add("b01id", intB01ID, DbType.Int32)
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("b65ID", myQuery)
            strW += bas.ParseWhereValidity("b65", "a", myQuery)
            If strW <> "" Then s += " AND " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY b65name"
        Return _cDB.GetList(Of BO.b65WorkflowMessage)(s, pars)

    End Function
End Class
