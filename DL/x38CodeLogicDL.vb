Public Class x38CodeLogicDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x38CodeLogic
        Dim s As String = "select *," & bas.RecTail("x38") & " FROM x38CodeLogic WHERE x38ID=@pid"
        Return _cDB.GetRecord(Of BO.x38CodeLogic)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x38_delete", pars)

    End Function
    Public Function Save(cRec As BO.x38CodeLogic) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x38ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x38Name", .x38Name, DbType.String, , , True, "Název číselné řady")
            pars.Add("x38EditModeFlag", .x38EditModeFlag, DbType.Int32)
            pars.Add("x38MaskSyntax", bas.ClearSqlForAttacks(.x38MaskSyntax), DbType.String, , , True, "Maska")
            pars.Add("x38ConstantBeforeValue", .x38ConstantBeforeValue, DbType.String)
            pars.Add("x38ConstantAfterValue", .x38ConstantAfterValue, DbType.String)
            pars.Add("x38IsDraft", .x38IsDraft, DbType.Boolean)
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("x38Description", .x38Description, DbType.String, , , True, "Poznámka")
            pars.Add("x38Scale", .x38Scale, DbType.Int32)
            pars.Add("x38ExplicitIncrementStart", .x38ExplicitIncrementStart, DbType.Int32)
            pars.Add("x38IsUseDbPID", .x38IsUseDbPID, DbType.Boolean)
            pars.Add("x38ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x38ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x38CodeLogic", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(x29id As BO.x29IdEnum) As IEnumerable(Of BO.x38CodeLogic)
        Dim s As String = "select *," & bas.RecTail("x38")
        s += " FROM x38CodeLogic"
        If x29id > BO.x29IdEnum._NotSpecified Then
            s += " WHERE x29ID=" & CInt(x29id).ToString
        End If
        s += " ORDER BY x29id,x38Name"

        Return _cDB.GetList(Of BO.x38CodeLogic)(s)

    End Function
End Class
