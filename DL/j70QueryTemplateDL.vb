Public Class j70QueryTemplateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j70QueryTemplate
        Dim s As String = GetSQLPart1() & " WHERE a.j70ID=@j70id"
        Return _cDB.GetRecord(Of BO.j70QueryTemplate)(s, New With {.j70id = intPID})
    End Function
    Public Function LoadSystemTemplate(x29id As BO.x29IdEnum, intJ03ID As Integer, Optional strMasterPrefix As String = "") As BO.j70QueryTemplate
        Dim pars As New DbParameters
        pars.Add("j03id", intJ03ID, DbType.Int32)
        pars.Add("x29id", x29id, DbType.Int32)

        Dim s As String = GetSQLPart1() & " WHERE a.j70IsSystem=1 AND a.j03ID=@j03id AND x29ID=@x29id"
        If strMasterPrefix <> "" Then
            pars.Add("masterprefix", strMasterPrefix, DbType.String)
            s += " AND j70MasterPrefix=@masterprefix"
        Else
            s += " AND j70MasterPrefix is null"
        End If
        Return _cDB.GetRecord(Of BO.j70QueryTemplate)(s, pars)
    End Function

    Public Function Save(cRec As BO.j70QueryTemplate, lisJ71 As List(Of BO.j71QueryTemplate_Item), lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        _Error = ""
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j70ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j03ID", .j03ID, DbType.Int32)
            pars.Add("j02ID_Owner", IIf(.j02ID_Owner <> 0, .j02ID_Owner, _curUser.j02ID), DbType.Int32)
            pars.Add("x29ID", BO.BAS.IsNullDBKey(cRec.x29ID), DbType.Int32)

            pars.Add("j70Name", .j70Name, DbType.String, , , True, "Název")

            pars.Add("j70IsSystem", .j70IsSystem, DbType.Boolean)
            pars.Add("j70BinFlag", .j70BinFlag, DbType.Int32)
            pars.Add("j70IsNegation", .j70IsNegation, DbType.Boolean)

            pars.Add("j70ColumnNames", .j70ColumnNames, DbType.String)
            pars.Add("j70OrderBy", .j70OrderBy, DbType.String)
            pars.Add("j70IsFilteringByColumn", .j70IsFilteringByColumn, DbType.Boolean)
            pars.Add("j70ScrollingFlag", CInt(.j70ScrollingFlag), DbType.Int32)
            pars.Add("j70MasterPrefix", .j70MasterPrefix, DbType.String)
            pars.Add("j70PageLayoutFlag", CInt(.j70PageLayoutFlag), DbType.Int32)
            pars.Add("j70validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("j70validuntil", cRec.ValidUntil, DbType.DateTime)

        End With


        If _cDB.SaveRecord("j70QueryTemplate", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intJ70ID As Integer = _cDB.LastSavedRecordPID
            If Not lisJ71 Is Nothing Then
                If Not bolINSERT Then _cDB.RunSQL("DELETE FROM j71QueryTemplate_Item WHERE j70ID=" & intJ70ID.ToString)

                For Each c In lisJ71
                    pars = New DbParameters
                    pars.Add("j70ID", intJ70ID, DbType.Int32)
                    pars.Add("x29ID", BO.BAS.IsNullDBKey(c.x29ID), DbType.Int32)
                    pars.Add("j71Field", c.j71Field, DbType.String)
                    pars.Add("j71RecordPID", BO.BAS.IsNullDBKey(c.j71RecordPID), DbType.Int32)
                    pars.Add("j71RecordName", c.j71RecordName, DbType.String)
                    pars.Add("j71RecordPID_Extension", BO.BAS.IsNullDBKey(c.j71RecordPID_Extension), DbType.Int32)
                    pars.Add("j71RecordName_Extension", c.j71RecordName_Extension, DbType.String)
                    pars.Add("j71ValueType", c.j71ValueType, DbType.String)
                    pars.Add("j71ValueFrom", c.j71ValueFrom, DbType.String)
                    pars.Add("j71ValueUntil", c.j71ValueUntil, DbType.String)
                    pars.Add("j71ValueString", c.j71ValueString, DbType.String)
                    pars.Add("j71StringOperator", c.j71StringOperator, DbType.String)
                    pars.Add("j71FieldLabel", c.j71FieldLabel, DbType.String)
                    pars.Add("j71SqlExpression", c.j71SqlExpression, DbType.String)
                    pars.Add("x28ID", BO.BAS.IsNullDBKey(c.x28ID), DbType.Int32)
                    If Not _cDB.SaveRecord("j71QueryTemplate_Item", pars, True, , , , False) Then

                    End If
                Next
            End If
            
            If Not lisX69 Is Nothing Then   'přiřazení rolí k filtru
                bas.SaveX69(_cDB, BO.x29IdEnum.j70QueryTemplate, intJ70ID, lisX69, bolINSERT)
            End If
            Return True
        Else
            Return False
        End If

    End Function
    Public Function GetList(myQuery As BO.myQuery, _x29id As BO.x29IdEnum, Optional strMasterPrefix As String = "", Optional onlyQuery As BO.BooleanQueryMode = BO.BooleanQueryMode.NoQuery) As IEnumerable(Of BO.j70QueryTemplate)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters

        Dim strW As String = "(a.j03ID=@j03id OR a.j70ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=170 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query))))"
        pars.Add("j03id", _curUser.PID, DbType.Int32)
        pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)

        If _x29id > BO.x29IdEnum._NotSpecified Then
            pars.Add("x29id", _x29id, DbType.Int32)
            strW += " AND a.x29ID=@x29id"
        End If
        If strMasterPrefix <> "-1" Then     '-1 volá návrhář přehledu, pokud pracuje s MasterPrefix<>""
            If strMasterPrefix <> "" Then
                pars.Add("masterprefix", strMasterPrefix, DbType.String)
                s += " AND a.j70MasterPrefix=@masterprefix"
            Else
                s += " AND a.j70MasterPrefix is null"
            End If
        End If

        If onlyQuery = BO.BooleanQueryMode.TrueQuery Then
            s += " AND (a.j70BinFlag>0 OR a.j70ID IN (SELECT j70ID FROM j71QueryTemplate_Item))"
        End If

        If Not myQuery Is Nothing Then
            strW += bas.ParseWhereMultiPIDs("a.j70ID", myQuery)
            strW += bas.ParseWhereValidity("j70", "a", myQuery)
        End If

        s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.j70IsSystem DESC,a.j70ID DESC"

        Return _cDB.GetList(Of BO.j70QueryTemplate)(s, pars)

    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j70_delete", pars)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,case when a.j03ID<>" & _curUser.PID.ToString & " then '*' end as _Mark," & bas.RecTail("j70", "a")
        s += " FROM j70QueryTemplate a INNER JOIN j03User j03 ON a.j03ID=j03.j03ID"
        Return s
    End Function


    Public Sub Setupj71TempList(intPID As Integer, strGUID As String)
        Dim pars As New DbParameters
        Dim s As String = "IF exists(select p85id FROM p85TempBox WHERE p85GUID='" & strGUID & "') DELETE FROM p85TempBox WHERE p85GUID=@guid;"
        s += "INSERT INTO p85TempBox(p85GUID,p85DataPID,p85OtherKey1,p85OtherKey2,p85FreeText01,p85OtherKey3,p85FreeText02,p85FreeText03,p85FreeText04,p85FreeText05,p85FreeText06,p85FreeText07,p85FreeText08,p85FreeText09,p85Message,p85OtherKey4)"
        s += " SELECT @guid,j71ID,j71RecordPID,a.x29ID,j71RecordName,j71RecordPID_Extension,j71RecordName_Extension,isnull(x29NameSingle,'Různé'),j71Field,j71ValueFrom,j71ValueUntil,j71ValueType,j71StringOperator,j71ValueString,j71FieldLabel,a.x28ID FROM j71QueryTemplate_Item a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID WHERE a.j70ID=@j70id"
        pars.Add("guid", strGUID, DbType.String)
        pars.Add("j70id", intPID, DbType.Int32)
        _cDB.RunSQL(s, pars)
    End Sub

    Public Function GetList_j71(intPID As Integer) As IEnumerable(Of BO.j71QueryTemplate_Item)
        Dim s As String = "select a.*,x29.x29NameSingle"
        s += " FROM j71QueryTemplate_Item a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID WHERE a.j70ID=@pid"
        s += " order by a.j71Field"

        Return _cDB.GetList(Of BO.j71QueryTemplate_Item)(s, New With {.pid = intPID})
    End Function

    Public Function GetSqlWhere(intJ70ID As Integer) As String
        Return bas.CompleteSqlJ70(_cDB, intJ70ID, _curUser)
    End Function
End Class
