Public Class j17CountryDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j17Country
        Dim s As String = "select *," & bas.RecTail("j17") & " FROM j17Country WHERE j17ID=@pid"
        Return _cDB.GetRecord(Of BO.j17Country)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j17_delete", pars)

    End Function
    Public Function Save(cRec As BO.j17Country) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j17ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j17Name", .j17Name, DbType.String, , , True, "Název")
            pars.Add("j17Code", .j17Code, DbType.String, , , True, "Kód")
            pars.Add("j17Ordinary", .j17Ordinary, DbType.Int32)
            pars.Add("j17ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j17ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j17Country", pars, bolINSERT, strW, True, _curUser.j03Login) Then

            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j17Country)
        Dim s As String = "select *," & bas.RecTail("j17")
        s += " FROM j17Country"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j17ID", myQuery)
            strW += bas.ParseWhereValidity("j17", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j17Ordinary,j17Name"

        Return _cDB.GetList(Of BO.j17Country)(s)

    End Function
End Class
