Public Class p50OfficePriceListDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    

    Public Function Load(intPID As Integer) As BO.p50OfficePriceList
        Dim s As String = "select a.*," & bas.RecTail("p50", "a", True, False) & " FROM p50OfficePriceList a INNER JOIN p51PriceList p51 ON a.p51ID=p51.p51ID WHERE a.p50ID=@pid"
        Return _cDB.GetRecord(Of BO.p50OfficePriceList)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p50_delete", pars)

    End Function
    Public Function Save(cRec As BO.p50OfficePriceList) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p50ID=@pid"
            pars.Add("pid", cRec.PID, DbType.Int32)
        End If
        With cRec
            pars.Add("p51ID", .p51ID, DbType.Int32)
            pars.Add("p50RatesFlag", .p50RatesFlag, DbType.Int32)
            pars.Add("p50ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p50ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p50OfficePriceList", pars, bolINSERT, strW, False) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p50OfficePriceList)
        Dim s As String = "select a.*,p51.p51Name as _p51Name," & bas.RecTail("p50", "a", True, False)
        s += " FROM p50OfficePriceList a INNER JOIN p51PriceList p51 ON a.p51ID=p51.p51ID"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.p50ID", myQuery)
            strW += bas.ParseWhereValidity("p50", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY p50ValidUntil DESC"

        Return _cDB.GetList(Of BO.p50OfficePriceList)(s)

    End Function
End Class
