Public Class x67EntityRoleDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x67EntityRole
        If intPID = 0 Then
            Return New BO.x67EntityRole()
        Else
            Dim s As String = "select *," & bas.RecTail("x67") & " FROM x67EntityRole WHERE x67ID=@x67id"

            Return _cDB.GetRecord(Of BO.x67EntityRole)(s, New With {.x67id = intPID})
        End If

    End Function
    Public Function LoadChild(intParentID As Integer) As BO.x67EntityRole
        Dim s As String = "select *," & bas.RecTail("x67") & " FROM x67EntityRole WHERE x67ParentID=@x67id"
        Return _cDB.GetRecord(Of BO.x67EntityRole)(s, New With {.x67id = intParentID})

    End Function

    Public Sub SaveO28(intX67ID As Integer, lisO28 As List(Of BO.o28ProjectRole_Workload))

        _cDB.RunSQL("DELETE FROM o28ProjectRole_Workload WHERE x67ID=" & intX67ID.ToString)
        For Each c In lisO28
            _cDB.RunSQL("INSERT INTO o28ProjectRole_Workload(x67ID,p34ID,o28EntryFlag,o28PermFlag) VALUES(" & intX67ID.ToString & "," & c.p34ID.ToString & "," & CInt(c.o28EntryFlag).ToString & "," & CInt(c.o28PermFlag).ToString & ")")
        Next

    End Sub
    Public Function Save(cRec As BO.x67EntityRole, lisX53 As List(Of BO.x53Permission)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "x67id=@pid"
                pars.Add("pid", cRec.PID)
            End If

            Dim strX67RoleValue As String = Replace(Space(40), " ", "0")
            For Each c In lisX53
                Dim x As Integer = CInt(c.x53Value)
                strX67RoleValue = Left(strX67RoleValue, x - 1) & "1" & Right(strX67RoleValue, Len(strX67RoleValue) - x)
            Next

            With pars
                .Add("x29ID", BO.BAS.IsNullDBKey(cRec.x29ID), DbType.Int32)
                .Add("x67ParentID", BO.BAS.IsNullDBKey(cRec.x67ParentID), DbType.Int32)
                .Add("x67name", cRec.x67Name, DbType.String, , , True, "Název role")
                .Add("x67Ordinary", cRec.x67Ordinary, DbType.Int32)
                .Add("x67RoleValue", strX67RoleValue, DbType.String)
                .Add("x67validfrom", cRec.ValidFrom, DbType.DateTime)
                .Add("x67validuntil", cRec.ValidUntil, DbType.DateTime)
            End With
            If _cDB.SaveRecord("x67EntityRole", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                If Not bolINSERT Then
                    _cDB.RunSQL("DELETE FROM x68EntityRole_Permission WHERE x67ID=" & _cDB.LastSavedRecordPID.ToString)
                End If
                Dim x53ids As List(Of Integer) = lisX53.Select(Function(p) p.PID).ToList()
                If x53ids.Count > 0 Then
                    _cDB.RunSQL("INSERT INTO x68EntityRole_Permission(x67ID,x53ID) SELECT " & _cDB.LastSavedRecordPID.ToString & ",x53ID FROM x53Permission WHERE x53ID IN (" & String.Join(",", x53ids) & ")")
                End If

                sc.Complete()
                Return True
            Else
                Return False
            End If
        End Using


    End Function
    Public Function Delete(intPID) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x67_delete", pars)

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x67EntityRole)
        Dim s As String = "select *," & bas.RecTail("x67") & " FROM x67EntityRole"

        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("x67ID", myQuery)
            strW += bas.ParseWhereValidity("x67", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY x67Ordinary"

        Return _cDB.GetList(Of BO.x67EntityRole)(s)

    End Function

    Public Function GetList_EntityPermissionsSource(x29id As BO.x29IdEnum, intRecordPID As Integer, intJ02ID As Integer) As IEnumerable(Of BO.x67EntityRole)
        Dim pars As New DbParameters
        pars.Add("x29id", x29id, DbType.Int32)
        pars.Add("recordpid", intRecordPID, DbType.Int32)
        pars.Add("j02id", intJ02ID, DbType.Int32)
        Dim strX29IDs As String = "@x29id"
        Dim s As String = "select a.*," & bas.RecTail("x67", "a") & " FROM x67EntityRole a INNER JOIN x69EntityRole_Assign b ON a.x67ID=b.x67ID WHERE (a.x29ID=@x29id AND b.x69RecordPID=@recordpid AND (b.j02ID=@j02id OR b.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id)))"
        If x29id = BO.x29IdEnum.p41Project Then
            'u projektu navíc testovat výchozí oprávnění definované u střediska projektu
            Dim intJ18ID As Integer = _cDB.GetIntegerValueFROMSQL("select j18ID FROM p41Project WHERE p41ID=" & intRecordPID.ToString)
            If intJ18ID <> 0 Then
                pars.Add("j18id", intJ18ID, DbType.Int32)
                s += " OR (a.x29ID=118 AND b.x69RecordPID=@j18id AND (b.j02ID=@j02id OR b.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id)))"
            End If
        End If

        Return _cDB.GetList(Of BO.x67EntityRole)(s, pars)

    End Function

  


    Public Function GetList_BoundX53(intPID As Integer) As IEnumerable(Of BO.x53Permission)
        Dim s As String = "select a.*," & bas.RecTail("x53", "a", True, False) & " FROM x53Permission a inner join x68EntityRole_Permission b on a.x53ID=b.x53ID WHERE b.x67ID=@x67id ORDER BY x53Ordinary"
        Return _cDB.GetList(Of BO.x53Permission)(s, New With {.x67id = intPID})
    End Function

    Public Function GetList_o28(x67ids As List(Of Integer)) As IEnumerable(Of BO.o28ProjectRole_Workload)
        Dim s As String = "select a.*,p34.p34Name as _p34Name," & bas.RecTail("o28", "a", False, False)
        s += " FROM o28ProjectRole_Workload a INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " WHERE a.x67ID IN (" & String.Join(",", x67ids) & ")"

        Return _cDB.GetList(Of BO.o28ProjectRole_Workload)(s)
    End Function
    Public Function _GetList_x69(x29ID As BO.x29IdEnum, recPIDs As List(Of Integer)) As IEnumerable(Of BO.x69EntityRole_Assign)
        Dim pars As New DbParameters
        pars.Add("x29id", x29ID, DbType.Int32)

        Dim s As String = "select a.*,j02.j02LastName+' '+j02.j02FirstName as _Person,j11.j11IsAllPersons as _IsAllPersons,x67.x67Name as _x67Name,j11.j11Name as _j11Name"
        s += " FROM x69EntityRole_Assign a INNER JOIN x67EntityRole x67 ON a.x67ID=x67.x67ID"
        s += " LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID"
        If recPIDs.Count = 1 Then
            s += " WHERE a.x69RecordPID=@recordpid AND x67.x29ID=@x29id"
            pars.Add("recordpid", recPIDs(0), DbType.Int32)
        Else
            s += " WHERE a.x69RecordPID IN (" & String.Join(",", recPIDs) & ") AND x67.x29ID=@x29id"
        End If
        s += " ORDER BY x67.x67Ordinary,a.x67ID"

        Return _cDB.GetList(Of BO.x69EntityRole_Assign)(s, pars)
    End Function


End Class
