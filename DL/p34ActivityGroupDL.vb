Public Class p34ActivityGroupDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p34ActivityGroup
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p34ID=@p34id"

        Return _cDB.GetRecord(Of BO.p34ActivityGroup)(s, New With {.p34id = intPID})
    End Function

    Public Function Save(cRec As BO.p34ActivityGroup, Optional intP32ID_SystemDefault As Integer = 0) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p34ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p33ID", .p33ID, DbType.Int32)

            pars.Add("p34Name", .p34Name, DbType.String, , , True, "Název")
            pars.Add("p34Code", .p34Code, DbType.String, , , True, "Kód")
            pars.Add("p34Ordinary", .p34Ordinary, DbType.Int32)
            pars.Add("p34Color", .p34Color, DbType.String)

            pars.Add("p34IncomeStatementFlag", .p34IncomeStatementFlag, DbType.Int32)
            pars.Add("p34ActivityEntryFlag", .p34ActivityEntryFlag, DbType.Int32)

            pars.Add("p34IsAllow_O27", .p34IsAllow_O27, DbType.Boolean)
            pars.Add("p34IsWorksheetValueHidden", .p34IsWorksheetValueHidden, DbType.Boolean)
            pars.Add("p34IsRecurrence", .p34IsRecurrence, DbType.Boolean)
            pars.Add("p34IsCapacityPlan", .p34IsCapacityPlan, DbType.Boolean)

            pars.Add("p34Name_EntryLang1", .p34Name_EntryLang1, DbType.String)
            pars.Add("p34Name_EntryLang2", .p34Name_EntryLang2, DbType.String)
            pars.Add("p34Name_EntryLang3", .p34Name_EntryLang3, DbType.String)
            pars.Add("p34Name_EntryLang4", .p34Name_EntryLang4, DbType.String)

            pars.Add("p34Name_BillingLang1", .p34Name_BillingLang1, DbType.String)
            pars.Add("p34Name_BillingLang2", .p34Name_BillingLang2, DbType.String)
            pars.Add("p34Name_BillingLang3", .p34Name_BillingLang3, DbType.String)
            pars.Add("p34Name_BillingLang4", .p34Name_BillingLang4, DbType.String)

            pars.Add("p34validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p34validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p34ActivityGroup", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            If cRec.p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava Then
                _cDB.RunSQL("UPDATE p32Activity SET p32IsSystemDefault=0 WHERE p34ID=" & Me.LastSavedRecordPID.ToString)
                If intP32ID_SystemDefault > 0 Then _cDB.RunSQL("UPDATE p32Activity SET p32IsSystemDefault=1 WHERE p32ID=" & intP32ID_SystemDefault.ToString)
            End If
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
        Return _cDB.RunSP("p34_delete", pars)
    End Function


    Public Function GetList(myQuery As BO.myQuery) As IEnumerable(Of BO.p34ActivityGroup)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p34ID", myQuery)
        
        strW += bas.ParseWhereValidity("p34", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p34Ordinary,p34Name"

        Return _cDB.GetList(Of BO.p34ActivityGroup)(s, pars)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,p33.p33Name as _p33Name," & bas.RecTail("p34", "a")
        s += " FROM p34ActivityGroup a INNER JOIN p33ActivityInputType p33 ON a.p33ID=p33.p33ID"

        Return s
    End Function

    Public Function GetList_WorksheetEntryInProject(intP41ID As Integer, intP42ID As Integer, intJ18ID As Integer, intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup)
        Dim s As String = GetSQLPart1(), pars As New DbParameters, strEntryFlags As String = "1,2"
        If intJ02ID <> _curUser.j02ID Then
            strEntryFlags = "1,2,4"
        End If
        s += " INNER JOIN p43ProjectType_Workload p43 ON a.p34ID=p43.p34ID"
        s += " WHERE p43.p42ID=@p42id AND a.p34ValidFrom<=getdate() AND a.p34ValidUntil>=getdate()"

        If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Creator) Then
            'právo zapisovat worksheeet úkony do všech projektů
            pars.Add("p42id", intP42ID, DbType.Int32)
            s += " ORDER BY a.p34Ordinary,a.p34Name"
            Return _cDB.GetList(Of BO.p34ActivityGroup)(s, pars)
        End If

        s += " AND (a.p34ID IN ("

        s += "SELECT a1.p34ID FROM o28ProjectRole_Workload a1 INNER JOIN x69EntityRole_Assign a2 ON a1.x67ID=a2.x67ID INNER JOIN x67EntityRole a3 ON a2.x67ID=a3.x67ID"
        s += " WHERE a3.x29ID=141 AND a2.x69RecordPID=@p41id AND a1.o28EntryFlag IN (" & strEntryFlags & ") AND (a2.j02ID=@j02id OR a2.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))"
        s += ") "

        If intJ18ID > 0 Then
            s += " OR a.p34ID IN (SELECT a1.p34ID FROM o28ProjectRole_Workload a1 INNER JOIN x69EntityRole_Assign a2 ON a1.x67ID=a2.x67ID INNER JOIN x67EntityRole a3 ON a2.x67ID=a3.x67ID"
            s += " WHERE a3.x29ID=118 AND a2.x69RecordPID=@j18id AND a1.o28EntryFlag IN (" & strEntryFlags & ") AND (a2.j02ID=@j02id OR a2.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))"
            s += ") "
        End If
        s += ")"


        pars.Add("p41id", intP41ID, DbType.Int32)
        pars.Add("p42id", intP42ID, DbType.Int32)
        pars.Add("j02id", intJ02ID, DbType.Int32)
        If intJ18ID > 0 Then pars.Add("j18id", intJ18ID, DbType.Int32)


        s += " ORDER BY a.p34Ordinary,a.p34Name"

        Return _cDB.GetList(Of BO.p34ActivityGroup)(s, pars)
    End Function

    Public Function GetList_WorksheetEntry_InAllProjects(intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup)
        Dim s As String = GetSQLPart1(), pars As New DbParameters, strEntryFlags As String = "1,2"
        If intJ02ID <> _curUser.j02ID Then
            strEntryFlags = "1,2,4"
        End If

        s += " WHERE getdate() BETWEEN a.p34ValidFrom AND a.p34ValidUntil AND a.p34ID IN (select p34ID FROM p43ProjectType_Workload)"

        If BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P31_Creator) Then
            'právo zapisovat worksheeet úkony do všech projektů
            s += " ORDER BY a.p34Ordinary,a.p34Name"
            Return _cDB.GetList(Of BO.p34ActivityGroup)(s, pars)
        End If

        s += " AND (a.p34ID IN ("

        s += "SELECT a1.p34ID FROM o28ProjectRole_Workload a1 INNER JOIN x69EntityRole_Assign a2 ON a1.x67ID=a2.x67ID INNER JOIN x67EntityRole a3 ON a2.x67ID=a3.x67ID"
        s += " WHERE a3.x29ID=141 AND a1.o28EntryFlag IN (" & strEntryFlags & ") AND (a2.j02ID=@j02id OR a2.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id))"
        s += ")"


        s += ")"


        pars.Add("j02id", intJ02ID, DbType.Int32)


        s += " ORDER BY a.p34Ordinary,a.p34Name"

        Return _cDB.GetList(Of BO.p34ActivityGroup)(s, pars)
    End Function
End Class
