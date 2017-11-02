Public Class b01WorkflowTemplateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.b01WorkflowTemplate
        Dim s As String = GetSQLPart1() & " WHERE b01ID=@b01id"

        Return _cDB.GetRecord(Of BO.b01WorkflowTemplate)(s, New With {.b01id = intPID})
    End Function
   

    Public Function Save(cRec As BO.b01WorkflowTemplate) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "b01id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("b01Name", .b01Name, DbType.String, , , True, "Název šablony")
            pars.Add("b01Code", .b01code, DbType.String)
            pars.Add("b01validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("b01validuntil", .ValidUntil, DbType.DateTime)
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("o40ID", BO.BAS.IsNullDBKey(.o40ID), DbType.Int32)
        End With

        If _cDB.SaveRecord("b01WorkflowTemplate", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("b01_delete", pars)
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.b01WorkflowTemplate)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("b01id", myQuery)
        strW += bas.ParseWhereValidity("b01", "", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY b01name"
        Return _cDB.GetList(Of BO.b01WorkflowTemplate)(s)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select *," & bas.RecTail("b01")
        s += " FROM b01WorkflowTemplate"
        Return s
    End Function

    
End Class
