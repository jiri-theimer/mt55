Public Class p47CapacityPlanDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub

    Public Function GetList(mq As BO.myQueryP47) As IEnumerable(Of BO.p47CapacityPlan)
        Dim pars As New DbParameters, strW As String = ""
        With mq
            If .DateFrom > DateSerial(1900, 1, 1) Then
                strW += " AND a.p47DateFrom>=@datefrom" : pars.Add("datefrom", .DateFrom, DbType.DateTime)
            End If
            If .DateUntil < DateSerial(3000, 1, 1) Then
                strW += " AND a.p47DateUntil<=@dateuntil" : pars.Add("dateuntil", .DateUntil, DbType.DateTime)
            End If
            If .p41ID > 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                strW += " AND a.p46ID IN (SELECT xa.p46ID FROM p46BudgetPerson xa INNER JOIN p45Budget xb ON xa.p45ID=xb.p45ID WHERE xb.p41ID=@p41id)"
            End If
            
            If .j02ID > 0 Then
                pars.Add("j02id", .j02ID, DbType.Int32)
                strW += " AND a.p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE j02ID=@j02id)"
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then
                    strW += " AND a.p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE j02ID IN (" & String.Join(",", .j02IDs) & "))"
                End If
            End If
            If .p45ID > 0 Then
                pars.Add("p45id", .p45ID, DbType.Int32)
                strW += " AND a.p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE p45ID=@p45id)"
            End If
            If .p41ID > 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                strW += " AND a.p46ID IN (SELECT p46ID FROM p46BudgetPerson xa INNER JOIN p45Budget xb ON xa.p45ID=xb.p45ID WHERE xb.p41ID=@p41id)"
            End If
            If .p28ID > 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND a.p46ID IN (SELECT p46ID FROM p46BudgetPerson xa INNER JOIN p45Budget xb ON xa.p45ID=xb.p45ID INNER JOIN p41Project xc ON xb.p41ID=xc.p41ID WHERE xc.p28ID_Client=@p28id)"
            End If
        End With
        strW = bas.TrimWHERE(strW)

        Dim s As String = "select a.*,p46.p45ID as _p45ID,p45.p41ID as _p41ID,p46.j02ID as _j02ID,j02.j02LastName+' '+j02.j02FirstName as _Person,isnull(p28.p28Name+' - ','') + p41.p41Name as _Project," & bas.RecTail("p47", "a")
        s += " FROM p47CapacityPlan a INNER JOIN p46BudgetPerson p46 ON a.p46ID=p46.p46ID INNER JOIN j02Person j02 ON p46.j02ID=j02.j02ID"
        s += " INNER JOIN p45Budget p45 ON p46.p45ID=p45.p45ID INNER JOIN p41Project p41 ON p45.p41ID=p41.p41ID "
        s += " LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID"
        s += " WHERE " & strW
        s += " ORDER BY a.p47DateFrom"
        Return _cDB.GetList(Of BO.p47CapacityPlan)(s, pars)
    End Function

    Public Function SaveProjectPlan(intP45ID As Integer, lisP47 As List(Of BO.p47CapacityPlan), lisP44 As List(Of BO.p44CapacityPlan_Exception)) As Boolean
        Dim mq As New BO.myQueryP47
        mq.p45ID = intP45ID
        Dim lisSaved As IEnumerable(Of BO.p47CapacityPlan) = GetList(mq)
        For Each c In lisP47
            Dim intPID As Integer = 0, bolNew As Boolean = True
            Dim lisFound As IEnumerable(Of BO.p47CapacityPlan) = lisSaved.Where(Function(p) p.p46ID = c.p46ID And p.p47DateFrom = c.p47DateFrom And p.p47DateUntil = c.p47DateUntil)
            If lisFound.Count > 0 Then
                intPID = lisFound(0).PID : bolNew = False
            End If
            Dim pars As New DbParameters, strW As String = ""
            If intPID > 0 Then
                strW = "p47ID=@pid"
                pars.Add("pid", intPID, DbType.Int32)
            End If
            pars.Add("p46ID", c.p46ID, DbType.Int32)
            pars.Add("p47DateFrom", c.p47DateFrom, DbType.DateTime)
            pars.Add("p47DateUntil", c.p47DateUntil, DbType.DateTime)
            pars.Add("p47HoursBillable", c.p47HoursBillable, DbType.Double)
            pars.Add("p47HoursNonBillable", c.p47HoursNonBillable, DbType.Double)
            pars.Add("p47HoursTotal", c.p47HoursTotal, DbType.Double)
            If c.IsSetAsDeleted Then
                _cDB.RunSQL("DELETE FROM p47CapacityPlan WHERE p47ID=" & c.PID.ToString)
            Else
                _cDB.SaveRecord("p47CapacityPlan", pars, bolNew, strW, True, _curUser.j03Login, False)
            End If


        Next
        If Not lisP44 Is Nothing Then
            _cDB.RunSQL("DELETE FROM p44CapacityPlan_Exception WHERE p46ID IN (SELECT p46ID FROM p46BudgetPerson WHERE p45ID=p45ID=" & intP45ID.ToString & ")")
            For Each c In lisP44
                Dim pars As New DbParameters
                pars.Add("p46ID", c.p46ID, DbType.Int32)
                pars.Add("p44ExceptionFlag", CInt(c.p44ExceptionFlag), DbType.Int32)
                pars.Add("p44DateFrom", c.p44DateFrom, DbType.DateTime)
                pars.Add("p44DateUntil", c.p44DateUntil, DbType.DateTime)
                _cDB.SaveRecord("p44CapacityPlan_Exception", pars, True, "", True, _curUser.j03Login, False)
            Next
        End If
        Return True
    End Function

  
End Class
