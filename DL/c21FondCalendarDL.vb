Public Class c21FondCalendarDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.c21FondCalendar
        Dim s As String = "select *," & bas.RecTail("c21") & " FROM c21FondCalendar WHERE c21ID=@pid"
        Return _cDB.GetRecord(Of BO.c21FondCalendar)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("c21_delete", pars)

    End Function
    Public Function Save(cRec As BO.c21FondCalendar) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "c21ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("c21Name", .c21Name, DbType.String, , , True, "Název")
            pars.Add("c21ScopeFlag", .c21ScopeFlag, DbType.Int32)
            pars.Add("c21Ordinary", .c21Ordinary, DbType.Int32)
            pars.Add("c21Day1_Hours", .c21Day1_Hours, DbType.Double)
            pars.Add("c21Day2_Hours", .c21Day2_Hours, DbType.Double)
            pars.Add("c21Day3_Hours", .c21Day3_Hours, DbType.Double)
            pars.Add("c21Day4_Hours", .c21Day4_Hours, DbType.Double)
            pars.Add("c21Day5_Hours", .c21Day5_Hours, DbType.Double)
            pars.Add("c21Day6_Hours", .c21Day6_Hours, DbType.Double)
            pars.Add("c21Day7_Hours", .c21Day7_Hours, DbType.Double)
            pars.Add("c21ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("c21ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("c21FondCalendar", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            pars = New DbParameters
            With pars
                .Add("c21id", Me.LastSavedRecordPID, DbType.Int32)
                .Add("j03id_sys", _curUser.PID, DbType.Int32)
            End With
            If _cDB.RunSP("c21_aftersave", pars) Then
                Return True
            Else
                Return False
            End If

        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.c21FondCalendar)
        Dim s As String = "select *," & bas.RecTail("c21")
        s += " FROM c21FondCalendar"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("c21ID", myQuery)
            strW += bas.ParseWhereValidity("c21", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY c21Ordinary,c21Name"

        Return _cDB.GetList(Of BO.c21FondCalendar)(s)

    End Function

    Public Function GetList_c22(c21ids As List(Of Integer), datFrom As Date, datUntil As Date, bolExcludeScopeFlag3 As Boolean) As IEnumerable(Of BO.c22FondCalendar_Date)
        Dim s As String = "select a.* FROM c22FondCalendar_Date a INNER JOIN c21FondCalendar b ON a.c21ID=b.c21ID WHERE a.c22Date BETWEEN @d1 AND @d2"
        s += " AND a.c21ID IN (" & String.Join(",", c21ids) & ")"
        If bolExcludeScopeFlag3 Then
            s += " AND b.c21ScopeFlag<>3"
        End If
        Dim pars As New DbParameters
        pars.Add("d1", datFrom, DbType.DateTime)
        pars.Add("d2", datUntil, DbType.DateTime)

        Return _cDB.GetList(Of BO.c22FondCalendar_Date)(s, pars)
    End Function

    Public Function GetSumHours(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As Double
        Dim s As String = "SELECT sum(c22Hours_Work) as Value FROM c22FondCalendar_Date WHERE c21ID=@c21id AND isnull(j17ID,0)=@j17id AND c22Date BETWEEN @d1 AND @d2"
        Dim pars As New DbParameters
        pars.Add("c21id", intC21ID, DbType.Int32)
        pars.Add("j17id", intJ17ID, DbType.Int32)
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetDoubleValueFROMSQL(s, pars)
    End Function
    Public Function GetSumHoursPerMonth(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.FondHours)
        Dim s As String = "SELECT sum(c22Hours_Work) as Hodiny,year(c22Date) as Rok,month(c22Date) as Mesic FROM c22FondCalendar_Date WHERE c21ID=@c21id AND isnull(j17ID,0)=@j17id AND c22Date BETWEEN @d1 AND @d2 GROUP BY year(c22Date),month(c22Date) ORDER BY year(c22Date),month(c22Date)"
        Dim pars As New DbParameters
        pars.Add("c21id", intC21ID, DbType.Int32)
        pars.Add("j17id", intJ17ID, DbType.Int32)
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetList(Of BO.FondHours)(s, pars)
    End Function
End Class
