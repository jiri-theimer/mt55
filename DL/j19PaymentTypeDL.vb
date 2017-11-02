Public Class j19PaymentTypeDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j19PaymentType
        Dim s As String = "select *," & bas.RecTail("j19") & " FROM j19PaymentType WHERE j19ID=@pid"
        Return _cDB.GetRecord(Of BO.j19PaymentType)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j19_delete", pars)

    End Function
    Public Function Save(cRec As BO.j19PaymentType) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j19ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j19Name", .j19Name, DbType.String, , , True, "Název")
            pars.Add("j19Ordinary", .j19Ordinary, DbType.Int32)
            pars.Add("j19ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j19ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j19PaymentType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j19PaymentType)
        Dim s As String = "select *," & bas.RecTail("j19")
        s += " FROM j19PaymentType"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j19ID", myQuery)
            strW += bas.ParseWhereValidity("j19", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j19Ordinary,j19Name"

        Return _cDB.GetList(Of BO.j19PaymentType)(s)

    End Function
End Class
