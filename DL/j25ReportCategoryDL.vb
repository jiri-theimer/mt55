Public Class j25ReportCategoryDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j25ReportCategory
        Dim s As String = "select *," & bas.RecTail("j25") & " FROM j25ReportCategory WHERE j25ID=@pid"
        Return _cDB.GetRecord(Of BO.j25ReportCategory)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j25_delete", pars)

    End Function
    Public Function Save(cRec As BO.j25ReportCategory) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j25ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j25Name", .j25Name, DbType.String, , , True, "Název")
            pars.Add("j25Ordinary", .j25Ordinary, DbType.Int32)
            pars.Add("j25ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j25ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j25ReportCategory", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j25ReportCategory)
        Dim s As String = "select *," & bas.RecTail("j25")
        s += " FROM j25ReportCategory"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j25ID", myQuery)
            strW += bas.ParseWhereValidity("j25", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j25Ordinary,j25Name"

        Return _cDB.GetList(Of BO.j25ReportCategory)(s)

    End Function
End Class
