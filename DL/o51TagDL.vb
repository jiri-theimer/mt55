Public Class o51TagDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o51Tag
        Dim s As String = "select a.*," & bas.RecTail("o51", "a") & ",j02.j02LastName+' '+j02.j02FirstName as _Owner FROM o51Tag a INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.J02ID WHERE a.o51id=@o51id"
        Return _cDB.GetRecord(Of BO.o51Tag)(s, New With {.o51id = intPID})
    End Function
    Public Function LoadByName(strName As String, intExcludePID As Integer) As BO.o51Tag
        Dim s As String = "select a.*," & bas.RecTail("o51", "a") & ",j02.j02LastName+' '+j02.j02FirstName as _Owner FROM o51Tag a INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.J02ID WHERE a.o51Name LIKE @expr AND a.o51ID<>@o51id"
        Dim pars As New DbParameters()
        pars.Add("expr", strName, DbType.String)
        pars.Add("o51id", intExcludePID, DbType.Int32)
        Return _cDB.GetRecord(Of BO.o51Tag)(s, pars)
    End Function

    Public Function Save(cRec As BO.o51Tag) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "o51id=@pid"
                pars.Add("pid", cRec.PID)
            End If

            With pars
                .Add("j02ID_Owner", BO.BAS.IsNullDBKey(cRec.j02ID_Owner), DbType.Int32)
                .Add("o51name", cRec.o51Name, DbType.String, , , True, "Název")
                .Add("o51Description", cRec.o51Description, DbType.String, , , True, "Popis")
                .Add("o51ScopeFlag", cRec.o51ScopeFlag, DbType.Int32)
                .Add("o51IsP41", cRec.o51IsP41, DbType.Boolean)
                .Add("o51IsP28", cRec.o51IsP28, DbType.Boolean)
                .Add("o51IsP91", cRec.o51IsP91, DbType.Boolean)
                .Add("o51IsP31", cRec.o51IsP31, DbType.Boolean)
                .Add("o51IsJ02", cRec.o51IsJ02, DbType.Boolean)
                .Add("o51IsO23", cRec.o51IsO23, DbType.Boolean)
                .Add("o51IsP56", cRec.o51IsP56, DbType.Boolean)
                .Add("o51IsP90", cRec.o51IsP90, DbType.Boolean)
                .Add("o51BackColor", cRec.o51BackColor, DbType.String)
                .Add("o51ForeColor", cRec.o51ForeColor, DbType.String)
                .Add("o51validfrom", cRec.ValidFrom, DbType.DateTime)
                .Add("o51validuntil", cRec.ValidUntil, DbType.DateTime)
            End With
            If _cDB.SaveRecord("o51Tag", pars, bolINSERT, strW, True, _curUser.j03Login) Then

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
        Return _cDB.RunSP("o51_delete", pars)

    End Function

    Public Function GetList(myQuery As BO.myQuery, strPrefix As String, usedInO52 As BO.BooleanQueryMode) As IEnumerable(Of BO.o51Tag)
        Dim s As String = "select a.*," & bas.RecTail("o51", "a") & ",j02.j02LastName+' '+j02.j02FirstName as _Owner FROM o51Tag a INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.J02ID"

        Dim pars As New DbParameters

        Dim strW As String = bas.ParseWhereMultiPIDs("a.o51ID", myQuery)
        ''strW += bas.ParseWhereValidity("o51", "a", myQuery)
        If myQuery.SearchExpression <> "" Then
            pars.Add("expr", myQuery.SearchExpression, DbType.String)
            strW += " AND a.o51Name LIKE '%'+@expr+'%'"
        End If
        If strPrefix <> "all" Then
            If strPrefix <> "" Then
                strW += " AND (a.o51Is" & UCase(strPrefix) & "=1 OR a.o51ScopeFlag=1)"
            Else
                strW += " AND a.o51ScopeFlag=1"
            End If
        End If
        Select Case usedInO52
            Case BO.BooleanQueryMode.TrueQuery
                strW += " AND a.o51ID IN (select o51ID FROM o52TagBinding)"
            Case BO.BooleanQueryMode.FalseQuery
                strW += " AND a.o51ID NOT IN (select o51ID FROM o52TagBinding)"
        End Select
        If myQuery.j02ID_Owner <> 0 Then
            strW += " AND a.j02ID_Owner=@owner"
            pars.Add("owner", _curUser.j02ID, DbType.Int32)
        End If

        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)


        s += " ORDER BY a.o51name"

        Return _cDB.GetList(Of BO.o51Tag)(s, pars)

    End Function



    Public Function GetList_o52(strPrefix As String, intRecordPID As Integer) As IEnumerable(Of BO.o52TagBinding)
        Dim pars As New DbParameters, intX29ID As Integer = CInt(BO.BAS.GetX29FromPrefix(strPrefix))
        pars.Add("recpid", intRecordPID, DbType.Int32)
        pars.Add("x29id", intX29ID, DbType.Int32)
        Return _cDB.GetList(Of BO.o52TagBinding)("select a.*,o51.o51Name,o51.o51ForeColor,o51.o51BackColor FROM o52TagBinding a inner join o51Tag o51 on a.o51ID=o51.o51ID WHERE a.o52RecordPID=@recpid AND a.x29ID=@x29id ORDER BY o51.o51Name", pars)
    End Function

    Public Function SaveBinding(strPrefix As String, intRecordPID As Integer, o51IDs As List(Of Integer)) As Boolean
        Dim pars As New DbParameters, intX29ID As Integer = CInt(BO.BAS.GetX29FromPrefix(strPrefix))
        pars.Add("recpid", intRecordPID, DbType.Int32)
        pars.Add("x29id", intX29ID, DbType.Int32)

        If o51IDs.Count = 0 Then
            Return _cDB.RunSQL("if exists(select o52ID FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id) DELETE FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id", pars)
        End If

        Dim strO51IDs As String = String.Join(",", o51IDs)
        pars.Add("login", _curUser.j03Login, DbType.String)
        _cDB.RunSQL("if exists(select o52ID FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id AND o51ID NOT IN (" & strO51IDs & ")) DELETE FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id AND o51ID NOT IN (" & strO51IDs & ")", pars)
        _cDB.RunSQL("INSERT INTO o52TagBinding(o51ID,o52RecordPID,x29ID,o52DateInsert,o52DateUpdate,o52UserInsert,o52UserUpdate) SELECT o51ID,@recpid,@x29id,getdate(),getdate(),@login,@login FROM o51Tag WHERE o51ID IN (" & strO51IDs & ") AND o51ID NOT IN (select o51ID FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id)", pars)

        Return _cDB.RunSQL("UPDATE o52TagBinding set o52DateUpdate=getdate(),o52UserUpdate=@login WHERE o52RecordPID=@recpid AND x29ID=@x29id", pars)
       
    End Function

    Public Function SaveBatch(strPrefix As String, pids As List(Of Integer), o51IDs As List(Of Integer), bolReplace As Boolean) As Boolean
        Dim pars As New DbParameters, intX29ID As Integer = CInt(BO.BAS.GetX29FromPrefix(strPrefix))

        pars.Add("x29id", intX29ID, DbType.Int32)
        If bolReplace Then
            _cDB.RunSQL("DELETE FROM o52TagBinding WHERE x29ID=@x29id AND o52RecordPID IN (" & String.Join(",", pids) & ")", pars)
        End If
        If o51IDs.Count = 0 Then Return True

        Dim strO51IDs As String = String.Join(",", o51IDs)
        For Each intPID As Integer In pids
            pars = New DbParameters
            pars.Add("x29id", intX29ID, DbType.Int32)
            pars.Add("login", _curUser.j03Login, DbType.String)
            pars.Add("recpid", intPID, DbType.Int32)
            _cDB.RunSQL("INSERT INTO o52TagBinding(o51ID,o52RecordPID,x29ID,o52DateInsert,o52DateUpdate,o52UserInsert,o52UserUpdate) SELECT o51ID,@recpid,@x29id,getdate(),getdate(),@login,@login FROM o51Tag WHERE o51ID IN (" & strO51IDs & ") AND o51ID NOT IN (select o51ID FROM o52TagBinding WHERE o52RecordPID=@recpid AND x29ID=@x29id)", pars)

        Next
        

        
        Return _cDB.RunSQL("UPDATE o52TagBinding set o52DateUpdate=getdate(),o52UserUpdate=@login WHERE o52RecordPID IN (" & String.Join(",", pids) & ") AND x29ID=@x29id", pars)

    End Function
End Class
