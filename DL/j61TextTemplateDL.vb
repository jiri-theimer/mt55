Public Class j61TextTemplateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j61TextTemplate
        Dim s As String = "select a.*,x29.x29Name as _x29Name,j02.j02LastName+' '+j02.j02FirstName as _Owner," & bas.RecTail("j61", "a") & " FROM j61TextTemplate a INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.j02ID LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID WHERE a.j61ID=@pid"
        Return _cDB.GetRecord(Of BO.j61TextTemplate)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j61_delete", pars)

    End Function
    Public Function Save(cRec As BO.j61TextTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j61ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", .x29ID, DbType.Int32)
            pars.Add("j02ID_Owner", .j02ID_Owner, DbType.Int32)
            pars.Add("j61Name", .j61Name, DbType.String, , , True, "Název šablony")
            pars.Add("j61PlainTextBody", .j61PlainTextBody, DbType.String)
            pars.Add("j61HtmlBody", .j61HtmlBody, DbType.String)
            pars.Add("j61MailSubject", .j61MailSubject, DbType.String)
            pars.Add("j61MailTO", .j61MailTO, DbType.String)
            pars.Add("j61MailCC", .j61MailCC, DbType.String)
            pars.Add("j61MailBCC", .j61MailBCC, DbType.String)
            pars.Add("j61Ordinary", .j61Ordinary, DbType.Int32)
            pars.Add("j61ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j61ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j61TextTemplate", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedO23ID As Integer = _cDB.LastSavedRecordPID
            If Not lisX69 Is Nothing Then   'přiřazení rolí k šabloně
                bas.SaveX69(_cDB, BO.x29IdEnum.j61TextTemplate, intLastSavedO23ID, lisX69, bolINSERT)
            End If
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j61TextTemplate)
        Dim s As String = "select a.*,x29.x29name as _x29Name,j02.j02LastName+' '+j02.j02FirstName as _Owner," & bas.RecTail("j61", "a")
        s += " FROM j61TextTemplate a INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.j02ID LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID"
        Dim strW As String = ""
        If Not myQuery Is Nothing Then
            strW += bas.ParseWhereMultiPIDs("a.j61ID", myQuery)
            strW += bas.ParseWhereValidity("j61", "a", myQuery)
            If strW = " AND " Then strW = ""
        End If
        strW += " AND (a.j02ID_Owner=@j02id_query"
        strW += " OR a.j61ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=161 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))"   'obdržel nějakou (jakoukoliv) roli v šabloně
        strW += ")"
        Dim pars As New DbParameters
        pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
        s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY a.j61Ordinary,a.j61Name"

        Return _cDB.GetList(Of BO.j61TextTemplate)(s, pars)

    End Function
End Class
