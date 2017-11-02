Public Class p11AttendanceDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function FindDefaultP41ID() As Integer
        Return _cDB.GetIntegerValueFROMSQL("select dbo.p11_find_p41id_default(" & _curUser.j02ID.ToString & ")")
    End Function
    Public Function Load(intPID As Integer) As BO.p11Attendance
        Dim s As String = "SELECT a.*," & bas.RecTail("p11", "a")
        s += " FROM p11Attendance a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " WHERE a.p11ID=@p11id"

        Return _cDB.GetRecord(Of BO.p11Attendance)(s, New With {.p11id = intPID})
    End Function
    Public Function LoadByPersonAndDate(intJ02ID As Integer, p11Date As Date) As BO.p11Attendance
        Dim s As String = "SELECT a.*," & bas.RecTail("p11", "a")
        s += " FROM p11Attendance a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " WHERE a.j02ID=@j02id AND a.p11Date=@p11date"

        Dim pars As New DbParameters
        pars.Add("j02id", intJ02ID, DbType.Int32)
        pars.Add("p11date", p11Date, DbType.DateTime)

        Return _cDB.GetRecord(Of BO.p11Attendance)(s, pars)
    End Function

    Public Function Save(cRec As BO.p11Attendance) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p11ID=@pid"
            pars.Add("pid", cRec.PID)
        Else
            cRec.ValidFrom = Now
            cRec.ValidUntil = DateSerial(3000, 1, 1)
        End If
        With cRec
            pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
            pars.Add("p11Date", .p11Date, DbType.DateTime)
            pars.Add("p11TodayStart", .p11TodayStart, DbType.DateTime)
            pars.Add("p11TodayEnd", .p11TodayEnd, DbType.DateTime)
            pars.Add("p11validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p11validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p11Attendance", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p11_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQuery, Optional intJ02ID As Integer = 0) As IEnumerable(Of BO.p11Attendance)
        Dim s As String = "SELECT a.* FROM p11Attendance a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID", pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("p11ID", mq)
        With mq
            If Year(.DateFrom) > 1900 Then
                pars.Add("datfrom", .DateFrom, DbType.DateTime)
                strW += " AND a.p11Date>=@datfrom"
            End If
            If Year(.DateUntil) < 3000 Then
                pars.Add("datuntil", .DateUntil, DbType.DateTime)
                strW += " AND a.p11Date<=@datuntil"
            End If
        End With
        If intJ02ID <> 0 Then
            pars.Add("j02id", intJ02ID, DbType.Int32)
            strW += " AND a.j02ID=@j02id"
        End If
        strW += bas.ParseWhereValidity("p11", "a", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.p11Attendance)(s, pars)

    End Function

End Class
