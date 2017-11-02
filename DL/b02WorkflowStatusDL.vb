Public Class b02WorkflowStatusDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.b02WorkflowStatus
        Dim s As String = GetSQLPart1() & " WHERE a.b02ID=@b02id"

        Return _cDB.GetRecord(Of BO.b02WorkflowStatus)(s, New With {.b02id = intPID})
    End Function

    Public Function Save(cRec As BO.b02WorkflowStatus) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "b02id=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("b01id", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
                pars.Add("b02name", .b02Name, DbType.String, , , True, "Název šablony")
                pars.Add("b02Code", .b02Code, DbType.String, , , True, "Kód stavu")
                pars.Add("b02color", .b02Color, DbType.String, , , True, "Barva")
                pars.Add("b02Ordinary", .b02Ordinary, DbType.Int32)
                pars.Add("b02IsRecordReadOnly4Owner", .b02IsRecordReadOnly4Owner, DbType.Boolean)
                pars.Add("b02IsDurationSLA", .b02IsDurationSLA, DbType.Boolean)
                pars.Add("b02TimeOut_Total", .b02TimeOut_Total, DbType.Int32)
                pars.Add("b02TimeOut_SLA", .b02TimeOut_SLA, DbType.Int32)
                pars.Add("b02validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("b02validuntil", .ValidUntil, DbType.DateTime)
            End With

            If _cDB.SaveRecord("b02WorkflowStatus", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                sc.Complete()
                Return True
            Else
                Return False
            End If

        End Using



    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If _cDB.RunSP("b02_delete", pars) Then
            If pars.Get(Of String)("err_ret") <> "" Then
                _Error = pars.Get(Of String)("err_ret")
                Return False
            End If
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
  
    Public Function GetList(intB01ID As Integer) As IEnumerable(Of BO.b02WorkflowStatus)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters
        If intB01ID <> 0 Then
            pars.Add("b01id", intB01ID, DbType.Int32)
            s += " WHERE a.b01ID=@b01id"
        End If
        s += " ORDER BY a.b01ID,a.b02Ordinary"

        Return _cDB.GetList(Of BO.b02WorkflowStatus)(s, pars)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,b01.x29ID as _x29ID,b01.b01Name as _b01Name," & bas.RecTail("b02")
        s += " FROM b02WorkflowStatus a INNER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID"
        Return s
    End Function

    


End Class
