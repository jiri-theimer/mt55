Public Class p35UnitDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p35Unit
        Dim s As String = "select *," & bas.RecTail("p35", , True, False) & " FROM p35Unit WHERE p35ID=@pid"
        Return _cDB.GetRecord(Of BO.p35Unit)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p35_delete", pars)

    End Function
    Public Function Save(cRec As BO.p35Unit) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p35ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p35Name", .p35Name, DbType.String, , , True, "Název")
            pars.Add("p35Code", .p35Code, DbType.String)
            pars.Add("p35ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p35ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p35Unit", pars, bolINSERT, strW, False) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p35Unit)
        Dim s As String = "select *," & bas.RecTail("p35", , True, False)
        s += " FROM p35Unit"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("p35ID", myQuery)
            strW += bas.ParseWhereValidity("p35", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY p35Name"

        Return _cDB.GetList(Of BO.p35Unit)(s)

    End Function
End Class
