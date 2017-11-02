Public Class p57TaskTypeDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p57TaskType
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p57ID=@p57id"

        Return _cDB.GetRecord(Of BO.p57TaskType)(s, New With {.p57id = intPID})
    End Function

    Public Function Save(cRec As BO.p57TaskType) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p57ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
            pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
            pars.Add("p57Name", .p57Name, DbType.String, , , True, "Název")
            pars.Add("p57Code", .p57Code, DbType.String, , , True, "Kód")
            pars.Add("p57Ordinary", .p57Ordinary, DbType.Int32)
            pars.Add("p57IsDefault", .p57IsDefault, DbType.Boolean)
            pars.Add("p57IsHelpdesk", .p57IsHelpdesk, DbType.Boolean)
            pars.Add("p57IsEntry_Receiver", .p57IsEntry_Receiver, DbType.Boolean)
            pars.Add("p57IsEntry_Budget", .p57IsEntry_Budget, DbType.Boolean)
            pars.Add("p57IsEntry_Priority", .p57IsEntry_Priority, DbType.Boolean)
            pars.Add("p57IsEntry_CompletePercent", .p57IsEntry_CompletePercent, DbType.Boolean)
            pars.Add("p57PlanDatesEntryFlag", BO.BAS.IsNullDBKey(.p57PlanDatesEntryFlag), DbType.Int32)
            pars.Add("p57Caption_PlanFrom", .p57Caption_PlanFrom, DbType.String)
            pars.Add("p57Caption_PlanUntil", .p57Caption_PlanUntil, DbType.String)
            pars.Add("p57validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p57validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p57TaskType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p57_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p57TaskType)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p57ID", myQuery)
        strW += bas.ParseWhereValidity("p57", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.p57Ordinary,a.p57Name"

        Return _cDB.GetList(Of BO.p57TaskType)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,b01.b01Name as _b01Name," & bas.RecTail("p57", "a")
        s += " FROM p57TaskType a LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID"

        Return s
    End Function
End Class
