Public Class p65RecurrenceDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p65Recurrence
        Dim s As String = "select a.*," & bas.RecTail("p65", "a") & " FROM p65Recurrence a WHERE a.p65ID=@pid"
        Return _cDB.GetRecord(Of BO.p65Recurrence)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p65_delete", pars)

    End Function
    Public Function Save(cRec As BO.p65Recurrence) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p65ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p65RecurFlag", CInt(.p65RecurFlag), DbType.Int32)
            pars.Add("p65Name", .p65Name, DbType.String, , , True, "Název")
            pars.Add("p65RecurGenToBase_D", .p65RecurGenToBase_D, DbType.Int32)
            pars.Add("p65RecurGenToBase_M", .p65RecurGenToBase_M, DbType.Int32)
            pars.Add("p65IsPlanUntil", .p65IsPlanUntil, DbType.Boolean)
            pars.Add("p65IsPlanFrom", .p65IsPlanFrom, DbType.Boolean)
            pars.Add("p65RecurPlanFromToBase_D", .p65RecurPlanFromToBase_D, DbType.Int32)
            pars.Add("p65RecurPlanFromToBase_M", .p65RecurPlanFromToBase_M, DbType.Int32)
            pars.Add("p65RecurPlanUntilToBase_D", .p65RecurPlanUntilToBase_D, DbType.Int32)
            pars.Add("p65RecurPlanUntilToBase_M", .p65RecurPlanUntilToBase_M, DbType.Int32)
            pars.Add("p65ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p65ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p65Recurrence", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p65Recurrence)
        Dim s As String = "select a.*," & bas.RecTail("p65", "a") & " FROM p65Recurrence a"
        Dim pars As New DbParameters
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.p65ID", myQuery)
            strW += bas.ParseWhereValidity("p65", "a", myQuery)

            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If


        Return _cDB.GetList(Of BO.p65Recurrence)(s, pars)

    End Function
End Class
