Public Class j77WorksheetStatTemplateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j77WorksheetStatTemplate
        Dim s As String = GetSQLPart1() & " WHERE a.j77ID=@j77id"
        Return _cDB.GetRecord(Of BO.j77WorksheetStatTemplate)(s, New With {.j77id = intPID})
    End Function
    

    Public Function Save(cRec As BO.j77WorksheetStatTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        _Error = ""
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j77ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j03ID", .j03ID, DbType.Int32)
            pars.Add("j02ID_Owner", .j02ID_Owner, DbType.Int32)

            pars.Add("j77Name", .j77Name, DbType.String, , , True, "Název")

            pars.Add("j77DD1", .j77DD1, DbType.String)
            pars.Add("j77DD2", .j77DD2, DbType.String)
            pars.Add("j77SumFields", .j77SumFields, DbType.String)
            pars.Add("j77ColFields", .j77ColFields, DbType.String)
            pars.Add("j77TabQueryFlag", .j77TabQueryFlag, DbType.String)
            pars.Add("j77Ordinary", .j77Ordinary, DbType.Int32)

            pars.Add("j70ID", BO.BAS.IsNullDBKey(.j70ID), DbType.Int32)

            pars.Add("j77validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("j77validuntil", cRec.ValidUntil, DbType.DateTime)

        End With


        If _cDB.SaveRecord("j77WorksheetStatTemplate", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intJ77ID As Integer = _cDB.LastSavedRecordPID
            If Not lisX69 Is Nothing Then   'přiřazení rolí k šabloně
                bas.SaveX69(_cDB, BO.x29IdEnum.j77WorksheetStatTemplate, _cDB.LastSavedRecordPID, lisX69, bolINSERT)
            End If
            Return True
        Else
            Return False
        End If

    End Function
    Public Function GetList(myQuery As BO.myQuery) As IEnumerable(Of BO.j77WorksheetStatTemplate)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters

        Dim strW As String = "(a.j03ID=@j03id OR a.j77ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=177 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query))))"
        pars.Add("j03id", _curUser.PID, DbType.Int32)
        pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)



        If Not myQuery Is Nothing Then
            strW += bas.ParseWhereMultiPIDs("a.j77ID", myQuery)
            strW += bas.ParseWhereValidity("j77", "a", myQuery)

            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY a.j77Ordinary ASC,a.j77ID DESC"

        Return _cDB.GetList(Of BO.j77WorksheetStatTemplate)(s, pars)

    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j77_delete", pars)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*," & bas.RecTail("j77", "a")
        s += " FROM j77WorksheetStatTemplate a INNER JOIN j03User j03 ON a.j03ID=j03.j03ID"
        Return s
    End Function

    
End Class
