Public Class j07PersonPositionDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j07PersonPosition
        Dim s As String = "select *," & bas.RecTail("j07") & " FROM j07PersonPosition WHERE j07ID=@pid"
        Return _cDB.GetRecord(Of BO.j07PersonPosition)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j07_delete", pars)

    End Function
    Public Function Save(cRec As BO.j07PersonPosition) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j07ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j07Name", .j07Name, DbType.String, , , True, "Název")
            pars.Add("j07Ordinary", .j07Ordinary, DbType.Int32)
            pars.Add("j07ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j07ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j07PersonPosition", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j07PersonPosition)
        Dim s As String = "select *," & bas.RecTail("j07")
        s += " FROM j07PersonPosition"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j07ID", myQuery)
            strW += bas.ParseWhereValidity("j07", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j07Ordinary,j07Name"

        Return _cDB.GetList(Of BO.j07PersonPosition)(s)

    End Function
End Class
