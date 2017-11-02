Public Class x23EntityField_ComboDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x23EntityField_Combo
        Dim s As String = "select *," & bas.RecTail("x23") & " FROM x23EntityField_Combo WHERE x23ID=@pid"
        Return _cDB.GetRecord(Of BO.x23EntityField_Combo)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x23_delete", pars)

    End Function
    Public Function Save(cRec As BO.x23EntityField_Combo) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x23ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x23Name", .x23Name, DbType.String)
            pars.Add("x23DataSource", .x23DataSource, DbType.String, , , True, "Externí datový zdroj")
            pars.Add("x23DataSourceTable", .x23DataSourceTable, DbType.String)
            pars.Add("x23DataSourceFieldPID", .x23DataSourceFieldPID, DbType.String)
            pars.Add("x23DataSourceFieldTEXT", .x23DataSourceFieldTEXT, DbType.String)
            pars.Add("x23Ordinary", .x23Ordinary, DbType.Int32)
            pars.Add("x23ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x23ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x23EntityField_Combo", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True

        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x23EntityField_Combo)
        Dim s As String = "select *," & bas.RecTail("x23")
        s += " FROM x23EntityField_Combo"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("x23ID", myQuery)
            strW += bas.ParseWhereValidity("x23", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY x23Ordinary"

        Return _cDB.GetList(Of BO.x23EntityField_Combo)(s)

    End Function
End Class
