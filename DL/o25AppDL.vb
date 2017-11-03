Public Class o25AppDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o25App
        Dim s As String = "select *," & bas.RecTail("o25") & " FROM o25App WHERE o25id=@pid"
        Return _cDB.GetRecord(Of BO.o25App)(s, New With {.pid = intPID})
    End Function
    

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o25_delete", pars)

    End Function
    Public Function Save(cRec As BO.o25App) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "o25id=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("o25name", .o25Name, DbType.String, , , True, "Název")
                pars.Add("o25Code", .o25Code, DbType.String)
                pars.Add("o25AppFlag", CInt(.o25AppFlag), DbType.Int32)
                pars.Add("o25Url", .o25Url, DbType.String)
                pars.Add("o25IsMainMenu", .o25IsMainMenu, DbType.Boolean)
                pars.Add("o25validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("o25validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("o25App", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedPID As Integer = cRec.PID

                sc.Complete()
                Return True
            Else
                _Error = _cDB.ErrorMessage
                Return False
            End If
        End Using
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.o25App)
        Dim s As String = "select *," & bas.RecTail("o25")
        s += " FROM o25App"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("o25ID", myQuery)
            strW += bas.ParseWhereValidity("o25", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY o25name"

        Return _cDB.GetList(Of BO.o25App)(s)

    End Function
End Class
