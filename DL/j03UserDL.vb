Imports System.Web

Public Class j03UserDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j03User
        Dim s As String = GetSQLPart1()
        s += " WHERE a.j03id=@j03id"
        Return _cDB.GetRecord(Of BO.j03User)(s, New With {.j03id = intPID})
    End Function
    Public Function LoadByLogin(strLogin As String) As BO.j03User
        Dim s As String = GetSQLPart1()
        s += " WHERE a.j03login=@login"
        Return _cDB.GetRecord(Of BO.j03User)(s, New With {.login = strLogin})
    End Function
    Public Function LoadByJ02ID(intJ02ID As Integer) As BO.j03User
        Dim s As String = GetSQLPart1(1)
        s += " WHERE a.j02ID=@j02id"
        Return _cDB.GetRecord(Of BO.j03User)(s, New With {.j02id = intJ02ID})
    End Function
    Public Function LoadSysProfile(strLogin As String) As BO.j03UserSYS
        Return _cDB.GetRecord(Of BO.j03UserSYS)("dbo.j03user_load_sysuser", New With {.login = strLogin}, True)

    End Function

    
    Public Function GetVirtualCount(myQuery As BO.myQueryJ03) As Integer
        Dim s As String = "SELECT count(a.j03ID) as Value FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id"
        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        If strW <> "" Then s += " WHERE " & strW

        Return _cDB.GetRecord(Of BO.GetInteger)(s, pars).Value
    End Function
    Private Function GetSQLWHERE(myQuery As BO.myQueryJ03, ByRef pars As DL.DbParameters) As String
        Dim strW As String = bas.ParseWhereMultiPIDs("a.j03ID", myQuery)
        strW += bas.ParseWhereValidity("j03", "a", myQuery)
        With myQuery
            If .j04ID <> 0 Then
                pars.Add("j04id", .j04ID, DbType.Int32)
                strW += " AND a.j04ID=@j04id"
            End If

            If .j02ID <> 0 Then
                pars.Add("j02id", .j02ID, DbType.Int32)
                strW += " AND a.j02ID=@j02id"
            End If
            If .SearchExpression <> "" Then
                strW += " AND (j03login like '%'+@expr+'%' OR isnull(j02LastName,'') LIKE '%'+@expr+'%' OR isnull(j02FirstName,'') LIKE '%'+@expr+'%')"
                pars.Add("expr", myQuery.SearchExpression, DbType.String)
            End If

        End With
        If strW <> "" Then strW = bas.TrimWHERE(strW)
        Return strW
    End Function
    Public Overloads Function GetList(myQuery As BO.myQueryJ03) As IEnumerable(Of BO.j03User)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly)

        Dim pars As New DL.DbParameters
        Dim strW As String = GetSQLWHERE(myQuery, pars)
        With myQuery
            Dim strSort As String = .MG_SortString
            strSort = Replace(strSort, "FullName", "j02LastName")
            If strSort = "" Then strSort = "j02lastname,j02firstname,j03login"
            If .MG_PageSize <> 0 Then
                'použít stránkování do gridu
                s = GetSQL_OFFSET(strW, strSort, .MG_PageSize, .MG_CurrentPageIndex, pars)
            Else
                'normální select
                If strW <> "" Then s += " WHERE " & strW
                If strSort <> "" Then
                    s += " ORDER BY " & strSort
                End If
            End If
        End With

        Return _cDB.GetList(Of BO.j03User)(s, pars)
    End Function
    
    
    Private Function GetSQLPart1(Optional intTOP As Integer = 0) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*," & bas.RecTail("j03", "a") & ",j04.j04name as _j04Name,j02.j02LastName as _j02LastName,j02.j02FirstName as _j02FirstName,j02.j02TitleBeforeName as _j02TitleBeforeName,j02.j02Email as _j02Email,j02.j02WorksheetAccessFlag as _j02WorksheetAccessFlag,a.j03PageMenuFlag"
        s += " FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id"
        Return s
    End Function
    Private Function GetSQL_OFFSET(strWHERE As String, strORDERBY As String, intPageSize As Integer, intCurrentPageIndex As Integer, ByRef pars As DL.DbParameters) As String
        Dim intStart As Integer = (intCurrentPageIndex) * intPageSize
        Dim s As String = "WITH rst AS (SELECT ROW_NUMBER() OVER (ORDER BY " & strORDERBY & ")-1 as RowIndex"
        s += ",a.*," & bas.RecTail("j03", "a") & ",j04.j04name,j02.j02LastName,j02.j02FirstName,j02.j02TitleBeforeName"
        s += " FROM j03user a INNER JOIN j04userrole j04 on a.j04id=j04.j04id LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id"
        If strWHERE <> "" Then s += " WHERE " & strWHERE
        s += ") SELECT TOP " & intPageSize.ToString & " *,j04Name as _j04Name,j02LastName as _j02LastName,j02FirstName as _j02FirstName,j02TitleBeforeName as _j02TitleBeforeName FROM rst"
        pars.Add("start", intStart, DbType.Int32)
        pars.Add("end", (intStart + intPageSize - 1), DbType.Int32)
        s += " WHERE RowIndex BETWEEN @start AND @end"
        s += " ORDER BY " & strORDERBY
        Return s
    End Function

    Public Function UpdateProfile(cRec As BO.j03User) As Boolean
        Dim pars As New DbParameters()
        With cRec
            pars.Add("pid", .PID)

            pars.Add("j03login", .j03Login, DbType.String)
            pars.Add("j02ID", .j02ID, DbType.Int32)
            pars.Add("j03MembershipUserId", .j03MembershipUserId, DbType.String)
        End With

        If _cDB.SaveRecord("j03user", pars, False, "j03id=@pid", True, _curUser.j03Login) Then

            Return True
        Else
            Return False
        End If

    End Function
    
    Public Function Save(cRec As BO.j03User) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "j03ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With pars
                .Add("j04id", BO.BAS.IsNullDBKey(cRec.j04ID), DbType.Int32)
                .Add("j02id", BO.BAS.IsNullDBKey(cRec.j02ID), DbType.Int32)
                .Add("j03login", cRec.j03Login, DbType.String, , , True, "Login")
                .Add("j03IsLiveChatSupport", cRec.j03IsLiveChatSupport, DbType.Boolean)
                .Add("j03SiteMenuSkin", cRec.j03SiteMenuSkin, DbType.String)
                .Add("j03PageMenuFlag", cRec.j03PageMenuFlag, DbType.Int32)
                .Add("j03IsSiteMenuOnClick", cRec.j03IsSiteMenuOnClick, DbType.Boolean)
                .Add("j03IsDomainAccount", cRec.j03IsDomainAccount, DbType.Boolean)
                .Add("j03IsSystemAccount", cRec.j03IsSystemAccount, DbType.Boolean)
                .Add("j03MembershipUserId", cRec.j03MembershipUserId, DbType.String)
                .Add("j03validfrom", cRec.ValidFrom, DbType.DateTime)
                .Add("j03validuntil", cRec.ValidUntil, DbType.DateTime)
                .Add("j03Aspx_PersonalPage", cRec.j03Aspx_PersonalPage, DbType.String)
                .Add("j03Aspx_PersonalPage_Mobile", cRec.j03Aspx_PersonalPage_Mobile, DbType.String)
                .Add("j03IsMustChangePassword", cRec.j03IsMustChangePassword, DbType.Boolean)
                .Add("j03PasswordExpiration", BO.BAS.IsNullDBDate(cRec.j03PasswordExpiration), DbType.DateTime)
                pars.Add("j03MobileForwardFlag", CInt(cRec.j03MobileForwardFlag), DbType.Int32)
                pars.Add("j03ProjectMaskIndex", cRec.j03ProjectMaskIndex, DbType.Int32)
            End With

            If _cDB.SaveRecord("j03User", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                sc.Complete()
                Return True
            Else
                Return False
            End If

           
        End Using


    End Function

    Public Function RenameLogin(cRec As BO.j03User, strNewLogin As String) As Boolean
        If cRec.j03MembershipUserId = "" Then
            _Error = "j03MembershipUserId není naplněn."
            Return False
        End If
        _cDB.ChangeConString2Membership()
        Dim pars As New DbParameters()
        Dim MyGuid As Guid = New Guid(cRec.j03MembershipUserId)

        With pars
            .Add("UserId", MyGuid, DbType.Guid)
            .Add("UserName", strNewLogin, DbType.String)
            .Add("ApplicationName", "/", DbType.String)
        End With
        If _cDB.RunSP("UpdateUserName", pars) Then
            _cDB.ChangeConString2Primary()
            If _cDB.RunSQL("UPDATE j03User set j03Login='" & strNewLogin & "' WHERE j03ID=" & cRec.PID.ToString) Then
                Return True
            Else
                Return False
            End If

        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function

    Public Sub SetAsVisitedUpgradeInfo(intPID As Integer)
        _cDB.RunSQL("UPDATE j03User set j03IsShallReadUpgradeInfo=0 WHERE j03ID=" & intPID.ToString)
    End Sub
    Public Sub SetAsWaitingOnVisitUpgradeInfo()
        _cDB.RunSQL("UPDATE j03User set j03IsShallReadUpgradeInfo=1")
    End Sub

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j03_delete", pars)
    End Function

    Public Function IsExistUserByLogin(strLogin As String, intJ03ID_Exclude As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("login", Trim(strLogin), DbType.String)
        pars.Add("j03id_exclude", intJ03ID_Exclude, DbType.Int32)
        If _cDB.GetIntegerValueFROMSQL("select j03ID as Value FROM j03User where j03Login LIKE @login AND j03ID<>@j03id_exclude", pars) <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    
    Public Sub SYS_AppendJ90Log(intJ03ID As Integer, cLog As BO.j90LoginAccessLog)
        Dim pars As New DbParameters
        With pars
            .Add("j03ID", intJ03ID, DbType.Int32)
            .Add("j90Date", Now, DbType.DateTime)
            .Add("j90ClientBrowser", cLog.j90ClientBrowser, DbType.String)
            .Add("j90Platform", cLog.j90Platform, DbType.String)
            .Add("j90IsMobileDevice", cLog.j90IsMobileDevice, DbType.Boolean)
            .Add("j90MobileDevice", cLog.j90MobileDevice, DbType.String)
            .Add("j90ScreenPixelsWidth", cLog.j90ScreenPixelsWidth, DbType.Int32)
            .Add("j90ScreenPixelsHeight", cLog.j90ScreenPixelsHeight, DbType.Int32)
            .Add("j90UserHostAddress", cLog.j90UserHostAddress, DbType.String)
            .Add("j90UserHostName", cLog.j90UserHostName, DbType.String)
            .Add("j90IsDomainTrusted", cLog.j90IsDomainTrusted, DbType.Boolean)
            .Add("j90DomainUserName", cLog.j90DomainUserName, DbType.String)
            .Add("j90RequestURL", cLog.j90RequestURL, DbType.String)
        End With
        If _cDB.SaveRecord("j90LoginAccessLog", pars, True) Then
            pars = New DbParameters
            pars.Add("pid", intJ03ID, DbType.Int32)
            _cDB.RunSQL("UPDATE j03User SET j03Ping_TimeStamp=GETDATE() WHERE j03ID=@pid", pars)
        End If
    End Sub

    Public Function SYS_GetList_UserParams(intJ03ID As Integer, x36keys As List(Of String)) As IEnumerable(Of BO.x36UserParam)
        If x36keys.Count = 0 Then Return Nothing

        Dim pars As New DbParameters, x As Integer, strParsInList As String = ""
        pars.Add("j03id", intJ03ID, DbType.Int32)
        Dim s As String = "select j03ID,x36Key,x36Value," & bas.RecTail("x36", "x36UserParam", True)
        For Each strKey As String In x36keys
            x += 1
            pars.Add("key" & x.ToString, strKey, DbType.String)
            If x = 1 Then
                strParsInList = "@key1"
            Else
                strParsInList += ",@key" & x.ToString
            End If
        Next

        s += " FROM x36UserParam WHERE j03id=@j03id AND x36Key IN (" & strParsInList & ")"

        Return _cDB.GetList(Of BO.x36UserParam)(s, pars)
    End Function
    Public Function SYS_DeleteAllUserParams(intJ03ID As Integer) As Boolean
        'vymazat vše kromě vlastních časových období
        Return _cDB.RunSQL("DELETE FROM x36UserParam WHERE x36Key <> 'periodcombo-custom_query' AND j03ID=" & intJ03ID.ToString)

    End Function
    Public Function SYS_SetUserParam(intJ03ID As Integer, strKey As String, strValue As String) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id", intJ03ID, DbType.Int32)
            .Add("x36key", strKey, DbType.String)
            .Add("x36value", strValue, DbType.String)
        End With
        Return _cDB.RunSP("x36userparam_save", pars)
    End Function
    Public Function SYS_SetUserParam_AllUsers(strKey As String, strValue As String) As Boolean
        Dim pars As New DbParameters
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("x36key", strKey, DbType.String)
            .Add("x36value", strValue, DbType.String)
        End With
        Return _cDB.RunSP("x36userparam_save_allusers", pars)
    End Function
    Public Function SYS_GetMyTag(intJ03ID As Integer, strKey As String, bolClearAfterRead As Boolean) As String
        Dim pars As New DbParameters
        With pars
            .Add("j03id", intJ03ID, DbType.Int32)
            .Add("x36key", strKey, DbType.String)
            .Add("clear_after_read", bolClearAfterRead, DbType.Boolean)
            .Add("x36value", Nothing, DbType.String, ParameterDirection.Output, 500)
        End With
        If _cDB.RunSP("x36userparam_get_mytag", pars) Then
            Return pars.Get(Of String)("x36value")
        Else
            Return ""
        End If
    End Function

    Public Function IsLoggedToday(intJ03ID As Integer) As Boolean
        If _cDB.GetRecord(Of Integer)("select select top 1 j90id from j90log where j03id=@j03id AND j90date>@today", New With {.j03id = intJ03ID, .today = Today}) > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList_j90(intJ03ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.j90LoginAccessLog)
        Dim pars As New DL.DbParameters
        pars.Add("j03id", intJ03ID, DbType.Int32)
        pars.Add("d1", d1, DbType.DateTime)
        pars.Add("d2", d2, DbType.DateTime)

        Return _cDB.GetList(Of BO.j90LoginAccessLog)("SELECT TOP 1000 j90ID as pid,* FROM j90LoginAccessLog WHERE j03ID=@j03id AND j90Date >= @d1 AND j90Date < DATEADD(DAY,1,@d2) ORDER BY j90ID DESC", pars)
    End Function

    Public Function SaveAllFavouriteProjects(intJ03ID As Integer, p41ids As List(Of Integer)) As Boolean
        Dim pars As New DbParameters
        pars.Add("j03id", intJ03ID, DbType.Int32)
        _cDB.RunSQL("DELETE FROM j13FavourteProject WHERE j03ID=@j03id", pars)
        If p41ids.Count > 0 Then
            Return _cDB.RunSQL("INSERT INTO j13FavourteProject(j03ID,p41ID) SELECT @j03id,p41ID FROM p41Project WHERE p41ID IN (" & String.Join(",", p41ids) & ")", pars)
        Else
            Return True
        End If
    End Function
    Public Function AppendOrRemoveFavouriteProject(intJ03ID As Integer, p41ids As List(Of Integer), bolRemove As Boolean) As Boolean
        Dim pars As New DbParameters
        pars.Add("j03id", intJ03ID, DbType.Int32)
        If bolRemove Then
            Return _cDB.RunSQL("DELETE FROM j13FavourteProject WHERE j03ID=@j03id AND p41ID IN (" & String.Join(",", p41ids) & ")", pars)
        Else
            Return _cDB.RunSQL("INSERT INTO j13FavourteProject(j03ID,p41ID) SELECT @j03id,p41ID FROM p41Project WHERE p41ID IN (" & String.Join(",", p41ids) & ") AND p41ID NOT IN (SELECT p41ID FROM j13FavourteProject WHERE j03ID=@j03id)", pars)
        End If
    End Function

    Public Function GetList_PageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.x61PageTab)
        Dim pars As New DL.DbParameters, s As String = "SELECT a.x61ID,a.x29ID,a.x61Code,a.x61Name,a.x61Ordinary,a.x61URL as _URL FROM x61PageTab a INNER JOIN j63PageTab_User j63 ON a.x61ID=j63.x61ID WHERE j63.j03ID=@j03id AND a.x29ID=@x29id ORDER BY j63.j63ID"
        pars.Add("j03id", intJ03ID, DbType.Int32)
        pars.Add("x29id", CInt(x29id), DbType.Int32)

        Dim lis As IEnumerable(Of BO.x61PageTab) = _cDB.GetList(Of BO.x61PageTab)(s, pars).Where(Function(p) p.x61Code <> "summary") ''summary se již nepoužívá!
        If lis.Count = 0 Then
            Dim strDefCodes As String = "'summary','p31','time','expense','fee','p56','p91'"
            Select Case x29id
                Case BO.x29IdEnum.p28Contact : strDefCodes = "'summary','p31','time','expense','fee','p41','p91'"
            End Select

            _cDB.RunSQL("INSERT INTO j63PageTab_User(x61ID,j03ID) SELECT x61ID,@j03id FROM x61PageTab WHERE x29ID=@x29id AND x61Code IN (" & strDefCodes & ") ORDER BY x61Ordinary", pars)
            lis = _cDB.GetList(Of BO.x61PageTab)(s, pars).Where(Function(p) p.x61Code <> "summary") ''summary se již nepoužívá!
        End If
        Return lis
    End Function
    Public Function SavePageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum, x61ids As List(Of Integer)) As Boolean
        Dim pars As New DbParameters
        pars.Add("j03id", intJ03ID, DbType.Int32)
        pars.Add("x29id", CInt(x29id), DbType.Int32)
        _cDB.RunSQL("DELETE FROM j63PageTab_User WHERE j03ID=@j03id AND x61ID IN (SELECT x61ID FROM x61PageTab WHERE x29ID=@x29id)", pars)
        For Each intX61ID As Integer In x61ids
            _cDB.RunSQL("INSERT INTO j63PageTab_User(x61ID,j03ID) VALUES(" & intX61ID.ToString & "," & intJ03ID.ToString & ")")
        Next
        Return True
    End Function
End Class
