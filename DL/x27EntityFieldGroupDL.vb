Public Class x27EntityFieldGroupDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x27EntityFieldGroup
        Dim s As String = "select *," & bas.RecTail("x27") & " FROM x27EntityFieldGroup WHERE x27ID=@pid"
        Return _cDB.GetRecord(Of BO.x27EntityFieldGroup)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x27_delete", pars)

    End Function
    Public Function Save(cRec As BO.x27EntityFieldGroup) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x27ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x27Name", .x27Name, DbType.String, , , True, "Název")
            pars.Add("x27Ordinary", .x27Ordinary, DbType.Int32)
            pars.Add("x27ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x27ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x27EntityFieldGroup", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
           
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x27EntityFieldGroup)
        Dim s As String = "select *," & bas.RecTail("x27")
        s += " FROM x27EntityFieldGroup"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("x27ID", myQuery)
            strW += bas.ParseWhereValidity("x27", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY x27Ordinary"

        Return _cDB.GetList(Of BO.x27EntityFieldGroup)(s)

    End Function
End Class
