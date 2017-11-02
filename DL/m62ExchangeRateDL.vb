Public Class m62ExchangeRateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.m62ExchangeRate
        Dim s As String = GetSQLPart1()
        s += " WHERE a.m62ID=@pid"
        Return _cDB.GetRecord(Of BO.m62ExchangeRate)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("m62_delete", pars)

    End Function
    Public Function Save(cRec As BO.m62ExchangeRate) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "m62ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j27ID_Master", BO.BAS.IsNullInt(.j27ID_Master), DbType.Int32)
            pars.Add("j27ID_Slave", BO.BAS.IsNullInt(.j27ID_Slave), DbType.Int32)
            pars.Add("m62RateType", BO.BAS.IsNullInt(.m62RateType), DbType.Int32)
            pars.Add("m62Date", .m62Date, DbType.DateTime)
            pars.Add("m62Rate", .m62Rate, DbType.Double)
            pars.Add("m62Units", .m62Units, DbType.Int32)
            pars.Add("m62ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("m62ValidUntil", .ValidUntil, DbType.DateTime)
        End With
        Dim strLogin As String = _curUser.j03Login
        If cRec.PID = 0 And cRec.UserInsert <> "" Then strLogin = cRec.UserInsert
        If _cDB.SaveRecord("m62ExchangeRate", pars, bolINSERT, strW, True, strLogin) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.m62ExchangeRate)
        Dim s As String = GetSQLPart1()
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.m62ID", myQuery)
            strW += bas.ParseWhereValidity("m62", "a", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY m62Date DESC"

        Return _cDB.GetList(Of BO.m62ExchangeRate)(s)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,j27master.j27Code as _masterj27Code,j27slave.j27Code as _slavej27Code," & bas.RecTail("m62", "a")
        s += " FROM m62ExchangeRate a INNER JOIN j27Currency j27master ON a.j27ID_Master=j27master.j27ID INNER JOIN j27Currency j27slave ON a.j27ID_Slave=j27slave.j27ID"

        Return s
    End Function
End Class
