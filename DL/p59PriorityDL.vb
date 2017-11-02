Public Class p59PriorityDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p59Priority
        Dim s As String = "select *," & bas.RecTail("p59") & " FROM p59Priority WHERE p59ID=@pid"
        Return _cDB.GetRecord(Of BO.p59Priority)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p59_delete", pars)

    End Function
    Public Function Save(cRec As BO.p59Priority) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p59ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p59Name", .p59Name, DbType.String, , , True, "Název")
            pars.Add("p59Ordinary", .p59Ordinary, DbType.Int32)
            pars.Add("p59ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p59ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p59Priority", pars, bolINSERT, strW, True, _curUser.j03Login) Then

            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p59Priority)
        Dim s As String = "select *," & bas.RecTail("p59")
        s += " FROM p59Priority"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("p59ID", myQuery)
            strW += bas.ParseWhereValidity("p59", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY p59Ordinary,p59Name"

        Return _cDB.GetList(Of BO.p59Priority)(s)

    End Function
End Class
