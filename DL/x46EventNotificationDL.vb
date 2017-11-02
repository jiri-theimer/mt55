Public Class x46EventNotificationDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x46EventNotification
        Dim s As String = GetSQLPart1()
        s += " WHERE a.x46ID=@x46id"

        Return _cDB.GetRecord(Of BO.x46EventNotification)(s, New With {.x46id = intPID})
    End Function

    Public Function Save(cRec As BO.x46EventNotification) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x46ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
            pars.Add("j11ID", BO.BAS.IsNullDBKey(.j11ID), DbType.Int32)
            pars.Add("x67ID", BO.BAS.IsNullDBKey(.x67ID), DbType.Int32)
            pars.Add("x45ID", BO.BAS.IsNullDBKey(.x45ID), DbType.Int32)
            pars.Add("x67ID_Reference", BO.BAS.IsNullDBKey(.x67ID_Reference), DbType.Int32)
            pars.Add("x29ID_Reference", BO.BAS.IsNullDBKey(.x29ID_Reference), DbType.Int32)
            pars.Add("x46MessageTemplate", .x46MessageTemplate, DbType.String, , , True, "Obsah zprávy")
            pars.Add("x46MessageSubject", .x46MessageSubject, DbType.String, , , True, "Předmět zprávy")
            pars.Add("x46IsExcludeAuthor", .x46IsExcludeAuthor, DbType.Boolean)
            pars.Add("x46IsForRecordOwner", .x46IsForRecordOwner, DbType.Boolean)
            pars.Add("x46IsForRecordOwner_Reference", .x46IsForRecordOwner_Reference, DbType.Boolean)
            pars.Add("x46IsUseSystemTemplate", .x46IsUseSystemTemplate, DbType.Boolean)
            pars.Add("x46IsForAllRoles", .x46IsForAllRoles, DbType.Boolean)
            pars.Add("x46IsForAllReferenceRoles", .x46IsForAllReferenceRoles, DbType.Boolean)

            pars.Add("x46validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("x46validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("x46EventNotification", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("x46_delete", pars)
    End Function


    Public Function GetList(myQuery As BO.myQuery, Optional intJ02ID As Integer = 0, Optional intX45ID As Integer = 0) As IEnumerable(Of BO.x46EventNotification)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.x46ID", myQuery)
        If intJ02ID <> 0 Then
            pars.Add("j02id", intJ02ID, DbType.Int32)
            strW += " AND (a.j02ID=@j02id OR a.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id))"
        End If
        If intX45ID > 0 Then
            pars.Add("x45id", intX45ID, DbType.Int32)
            strW += " AND a.x45ID=@x45id"
        End If
        strW += bas.ParseWhereValidity("x46", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.x45ID"

        Return _cDB.GetList(Of BO.x46EventNotification)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,j11.j11Name as _j11Name,j02.j02LastName+' '+j02.j02Firstname as _Person,x45.x45Name as _x45Name," & bas.RecTail("x46", "a")
        s += ",x67.x67Name as _x67Name,x29.x29NameSingle as _x29NameSingle"
        s += " FROM x46EventNotification a INNER JOIN x45Event x45 ON a.x45ID=x45.x45ID LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID"
        s += " LEFT OUTER JOIN x67EntityRole x67 ON a.x67ID=x67.x67ID LEFT OUTER JOIN x29Entity x29 ON a.x29ID_Reference=x29.x29ID"
        Return s
    End Function
End Class
