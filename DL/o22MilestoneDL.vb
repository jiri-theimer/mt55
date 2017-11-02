Public Class o22MilestoneDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o22Milestone
        Dim s As String = GetSQLPart1(0)
        s += " WHERE a.o22ID=@o22id"

        Return _cDB.GetRecord(Of BO.o22Milestone)(s, New With {.o22id = intPID})
    End Function
    Public Function LoadMyLastCreated() As BO.o22Milestone
        Dim s As String = GetSQLPart1(1)
        s += " WHERE a.j02ID_Owner=@j02id_owner ORDER BY a.o22ID DESC"

        Return _cDB.GetRecord(Of BO.o22Milestone)(s, New With {.j02id_owner = _curUser.j02ID})
    End Function

    Public Function Save(cRec As BO.o22Milestone, lisO20 As List(Of BO.o20Milestone_Receiver)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o22ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
            pars.Add("o21ID", BO.BAS.IsNullDBKey(.o21ID), DbType.Int32)
            pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
            pars.Add("p28ID", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
            pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
            pars.Add("p91ID", BO.BAS.IsNullDBKey(.p91ID), DbType.Int32)
            pars.Add("p90ID", BO.BAS.IsNullDBKey(.p90ID), DbType.Int32)

            pars.Add("o22Name", .o22Name, DbType.String, , , True, "Název (předmět)")
            pars.Add("o22Code", .o22Code, DbType.String, , , True, "Kód")
            pars.Add("o22Location", .o22Location, DbType.String, , , True, "Lokalita")
            pars.Add("o22Description", .o22Description, DbType.String, , , True, "Poznámka")


            pars.Add("o22DateFrom", BO.BAS.IsNullDBDate(.o22DateFrom), DbType.DateTime)
            pars.Add("o22DateUntil", BO.BAS.IsNullDBDate(.o22DateUntil), DbType.DateTime)
            pars.Add("o22IsAllDay", .o22IsAllDay, DbType.Boolean)
            pars.Add("o22ReminderDate", BO.BAS.IsNullDBDate(.o22ReminderDate), DbType.DateTime)
            pars.Add("o22IsNoNotify", .o22IsNoNotify, DbType.Boolean)
            If .o22MilestoneGUID = "" Then .o22MilestoneGUID = BO.BAS.GetGUID
            pars.Add("o22MilestoneGUID", .o22MilestoneGUID, DbType.String)
            pars.Add("o22validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("o22validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("o22Milestone", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedPID As Integer = cRec.PID
            If bolINSERT Then intLastSavedPID = _cDB.LastIdentityValue
            If Not bolINSERT Then
                If Not lisO20 Is Nothing Then _cDB.RunSQL("DELETE FROM o20Milestone_Receiver WHERE o22ID=" & intLastSavedPID.ToString)
            End If
    
            If Not lisO20 Is Nothing Then
                For Each c In lisO20
                    If c.j02ID <> 0 Then
                        _cDB.RunSQL("INSERT INTO o20Milestone_Receiver(o22ID,j02ID) VALUES (" & intLastSavedPID.ToString & "," & c.j02ID.ToString & ")")
                    End If
                    If c.x67ID <> 0 Then
                        _cDB.RunSQL("INSERT INTO o20Milestone_Receiver(o22ID,x67ID) VALUES (" & intLastSavedPID.ToString & "," & c.x67ID.ToString & ")")
                    End If
                    If c.j11ID <> 0 Then
                        _cDB.RunSQL("INSERT INTO o20Milestone_Receiver(o22ID,j11ID) VALUES (" & intLastSavedPID.ToString & "," & c.j11ID.ToString & ")")
                    End If
                Next
            End If
            bas.RecoveryUserCache(_cDB, _curUser)



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
        Return _cDB.RunSP("o22_delete", pars)
    End Function

    Public Function GetList_WaitingOnReminder(datReminderFrom As Date, datReminderUntil As Date) As IEnumerable(Of BO.o22Milestone)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        pars.Add("datereminderfrom", datReminderFrom)
        pars.Add("datereminderuntil", datReminderUntil)
        s += " WHERE o22ReminderDate BETWEEN @datereminderfrom AND @datereminderuntil"
        s += " AND o22ID NOT IN (SELECT x47RecordPID FROM x47EventLog WHERE x29ID=222 AND x45ID=22206)"

        Return _cDB.GetList(Of BO.o22Milestone)(s, pars)
    End Function

    Public Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.o22Milestone)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        s += " WHERE (o22DateFrom BETWEEN @d1 AND @d2 OR o22DateUntil BETWEEN @d1 AND @d2 OR o22ReminderDate BETWEEN @d1 AND @d2)"
        s += "AND (a.j02ID_Owner=@j02id OR a.j02ID=@j02id OR o22ID IN (SELECT o22ID FROM o20Milestone_Receiver WHERE j02ID=@j02id OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id)))"
        pars.Add("j02id", intJ02ID, DbType.Int32)
        pars.Add("d1", DateAdd(DateInterval.Day, -1, Now), DbType.DateTime)
        pars.Add("d2", DateAdd(DateInterval.Day, 2, Now), DbType.DateTime)

        Return _cDB.GetList(Of BO.o22Milestone)(s, pars)
    End Function
    Public Function GetList(myQuery As BO.myQueryO22) As IEnumerable(Of BO.o22Milestone)
        Dim s As String = GetSQLPart1(0), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.o22ID", myQuery)
        With myQuery
            If Year(.DateFrom) > 1900 Or Year(.DateUntil) < 3000 Then
                pars.Add("d1", .DateFrom, DbType.DateTime)
                pars.Add("d2", .DateUntil, DbType.DateTime)
                strW += " AND (a.o22DateFrom BETWEEN @d1 AND @d2 OR a.o22DateUntil BETWEEN @d1 AND @d2)"
            End If

            If .o21ID <> 0 Then
                pars.Add("o21id", .p41ID, DbType.Int32)
                strW += " AND a.o21ID=@o21id"
            End If
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                If Not .IsIncludeChildProjects Then
                    strW += " AND a.p41ID=@p41id"
                Else
                    strW += " AND (a.p41ID=@p41id OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID=@p41id) AND (select p41TreeNext FROM p41Project WHERE p41ID=@p41id)))"
                End If
            End If
            If Not .p41IDs Is Nothing Then
                If .p41IDs.Count > 0 Then
                    If Not .IsIncludeChildProjects Then
                        strW += " AND a.p41ID IN (" & String.Join(",", .p41IDs) & ")"
                    Else
                        strW += " AND (a.p41ID IN (" & String.Join(",", .p41IDs) & ") OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & ")) AND (select p41TreeNext FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & "))))"
                    End If
                End If
            End If

            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND (a.p28ID=@p28id OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id))"
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then
                    strW += " AND (a.j02ID IN (" & String.Join(",", .j02IDs) & ") OR a.o22ID IN (SELECT o22ID FROM o20Milestone_Receiver WHERE j02ID IN (" & String.Join(",", .j02IDs) & ") OR j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID IN (" & String.Join(",", .j02IDs) & "))))"
                End If
            End If

            If .p91ID <> 0 Then
                pars.Add("p91id", .p91ID, DbType.Int32)
                strW += " AND a.p91ID=@p91id"
            End If
            
        End With
        strW += bas.ParseWhereValidity("o22", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)


        's += " ORDER BY o22DateUntil"

        Return _cDB.GetList(Of BO.o22Milestone)(s, pars)

    End Function


    Private Function GetSQLPart1(intTOP As Integer) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*,o21.o21Name as _o21Name,o21.o21Flag as _o21Flag,o21.x29ID as _x29ID," & bas.RecTail("o22", "a")
        s += ",p41.p41Name as _Project,p28.p28Name as _Contact,j02.j02LastName+' '+j02.j02FirstName as _Person,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner"
        s += " FROM o22Milestone a INNER JOIN o21MilestoneType o21 ON a.o21ID=o21.o21ID LEFT OUTER JOIN p41Project p41 ON a.p41ID=p41.p41ID"
        s += " LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID"
        s += " LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID"

        Return s
    End Function

    Public Function GetList_o20(intPID As Integer) As IEnumerable(Of BO.o20Milestone_Receiver)
        Dim s As String = "select a.*,j02.j02LastName+' '+j02.j02FirstName as _Person,j11.j11Name as _j11Name," & bas.RecTail("o20", "a", False, False)
        s += " FROM o20Milestone_Receiver a LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID"
        s += " WHERE a.o22ID=@pid"

        Return _cDB.GetList(Of BO.o20Milestone_Receiver)(s, New With {.pid = intPID})
    End Function
    
End Class
