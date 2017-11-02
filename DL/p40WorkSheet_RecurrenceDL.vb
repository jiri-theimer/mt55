Public Class p40WorkSheet_RecurrenceDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p40WorkSheet_Recurrence
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p40ID=@p40id"

        Return _cDB.GetRecord(Of BO.p40WorkSheet_Recurrence)(s, New With {.p40id = intPID})
    End Function

    Public Function Save(cRec As BO.p40WorkSheet_Recurrence) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p40ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
                pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
                pars.Add("p34ID", BO.BAS.IsNullDBKey(.p34ID), DbType.Int32)
                pars.Add("p32ID", BO.BAS.IsNullDBKey(.p32ID), DbType.Int32)
                pars.Add("p56ID", BO.BAS.IsNullDBKey(.p56ID), DbType.Int32)
                pars.Add("j27ID", BO.BAS.IsNullDBKey(.j27ID), DbType.Int32)
                pars.Add("x15ID", BO.BAS.IsNullDBKey(.x15ID), DbType.Int32)
                pars.Add("p40Name", .p40Name, DbType.String, , , True, "Název")
                pars.Add("p40Text", .p40Text, DbType.String, , , True, "Popis úkonu")
                pars.Add("p40RecurrenceType", CInt(.p40RecurrenceType), DbType.Int32)
                pars.Add("p40Value", .p40Value, DbType.Double)
                pars.Add("p40FirstSupplyDate", .p40FirstSupplyDate, DbType.Date)
                pars.Add("p40LastSupplyDate", .p40LastSupplyDate, DbType.Date)
                pars.Add("p40GenerateDayAfterSupply", .p40GenerateDayAfterSupply, DbType.Int32)


                pars.Add("p40validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p40validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p40WorkSheet_Recurrence", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedP40ID As Integer = _cDB.LastSavedRecordPID
                pars = New DbParameters
                With pars
                    .Add("p40id", intLastSavedP40ID, DbType.Int32)
                    .Add("j03id_sys", _curUser.PID, DbType.Int32)
                End With
                If _cDB.RunSP("p40_aftersave", pars) Then
                    sc.Complete()
                    Return True
                Else
                    Return False
                End If
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
        Return _cDB.RunSP("p40_delete", pars)
    End Function

    
    Public Function GetList(intP41ID As Integer, intP56ID As Integer) As IEnumerable(Of BO.p40WorkSheet_Recurrence)
        Dim s As String = GetSQLPart1()
        Dim strW As String = "a.p41ID=@p41id", pars As New DbParameters
        pars.Add("p41id", intP41ID, DbType.Int32)
        If intP56ID <> 0 Then
            pars.Add("p56id", intP56ID, DbType.Int32)
            strW += " AND a.p56ID=@p56id"
        End If
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.p40WorkSheet_Recurrence)(s, pars)

    End Function

    Public Function GetList_WaitingForGenerate(datNow As Date) As IEnumerable(Of BO.p40WorkSheet_Recurrence)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = "getdate() BETWEEN a.p40ValidFrom AND a.p40ValidUntil AND a.p56ID IS NULL AND a.p40ID IN (select p40ID FROM p39WorkSheet_Recurrence_Plan WHERE p31ID_NewInstance IS NULL AND p39DateCreate BETWEEN dateadd(day,-2,@dat) AND @dat)"
        pars.Add("dat", datNow, DbType.DateTime)

        s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.p40WorkSheet_Recurrence)(s, pars)

    End Function
    Function LoadP39_FirstWaiting(intP40ID As Integer, datNow As DateTime) As BO.p39WorkSheet_Recurrence_Plan
        Dim pars As New DbParameters
        Dim s As String = "select top 1 * FROM p39WorkSheet_Recurrence_Plan WHERE p40ID=@p40id AND p31ID_NewInstance IS NULL AND p39DateCreate BETWEEN dateadd(day,-2,@dat) AND @dat ORDER BY p39DateCreate"
        pars.Add("dat", datNow, DbType.DateTime)
        pars.Add("p40id", intP40ID, DbType.Int32)
        Return _cDB.GetRecord(Of BO.p39WorkSheet_Recurrence_Plan)(s, pars)
    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,p34.p34Name as _p34Name,p32.p32Name as _p32Name,j02.j02LastName+' '+j02.j02FirstName as _Person,p57.p57Name+': '+p56.p56RecurNameMask as _Task," & bas.RecTail("p40", "a")
        s += " FROM p40WorkSheet_Recurrence a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID LEFT OUTER JOIN p57TaskType p57 ON p56.p57ID=p57.p57ID"
        Return s
    End Function

    Function GetList_p39(intPID As Integer) As IEnumerable(Of BO.p39WorkSheet_Recurrence_Plan)
        Dim s As String = "select a.*,b.p41ID as _p41ID,p41.p41Name as _p41Name,p28.p28Name as _p28Name FROM p39WorkSheet_Recurrence_Plan a INNER JOIN p40WorkSheet_Recurrence b ON a.p40ID=b.p40ID INNER JOIN p41Project p41 ON b.p41ID=p41.p41ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID"
        s += " WHERE a.p40ID=@pid"
        Return _cDB.GetList(Of BO.p39WorkSheet_Recurrence_Plan)(s, New With {.pid = intPID})
    End Function
    Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.p39WorkSheet_Recurrence_Plan)
        Dim s As String = "select a.*,b.p41ID as _p41ID,p41.p41Name as _p41Name,p28.p28Name as _p28Name FROM p39WorkSheet_Recurrence_Plan a INNER JOIN p40WorkSheet_Recurrence b ON a.p40ID=b.p40ID INNER JOIN p41Project p41 ON b.p41ID=p41.p41ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID"
        s += " WHERE (b.j02ID=@j02id or b.p40UserInsert=@login) AND a.p39DateCreate BETWEEN @d1 AND @d2"

        Dim pars As New DbParameters
        pars.Add("j02id", intJ02ID, DbType.Int32)
        pars.Add("login", _curUser.j03Login, DbType.String)
        pars.Add("d1", DateAdd(DateInterval.Day, -1, Now), DbType.DateTime)
        pars.Add("d2", DateAdd(DateInterval.Day, 2, Now), DbType.DateTime)

        Return _cDB.GetList(Of BO.p39WorkSheet_Recurrence_Plan)(s, pars)
    End Function

    Public Function Update_p31Instance(intP39ID As Integer, intP31ID As Integer, strErrorMessage As String) As Boolean
        Dim pars As New DbParameters()
        pars.Add("p31id", BO.BAS.IsNullDBKey(intP31ID), DbType.Int32)
        pars.Add("p39id", intP39ID, DbType.Int32)
        pars.Add("error", strErrorMessage, DbType.String)
        Return _cDB.RunSQL("update p39WorkSheet_Recurrence_Plan set p31ID_NewInstance=@p31id,p39ErrorMessage_NewInstance=@error WHERE p39ID=@p39id", pars)
    End Function
End Class
