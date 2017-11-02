Public Interface Ij03UserBL
    Inherits IFMother
    Function Save(cRec As BO.j03User) As Boolean
    Function Load(intPID As Integer) As BO.j03User
    Function LoadByLogin(strLogin As String) As BO.j03User
    Function LoadByJ02ID(intJ02ID As Integer) As BO.j03User
    Function LoadSysProfile(strLogin As String) As BO.j03UserSYS
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryJ03) As IEnumerable(Of BO.j03User)
    Function GetVirtualCount(mq As BO.myQueryJ03) As Integer
    Function RenameLogin(cRec As BO.j03User, strNewLogin As String) As Boolean
    Function IsExistUserByLogin(strLogin As String, intJ03ID_Exclude As Integer) As Boolean

    Sub InhaleUserParams(x36keys As List(Of String))
    Sub InhaleUserParams(strKey1 As String, Optional strKey2 As String = "", Optional strKey3 As String = "")
    Function GetUserParam(strKey As String, Optional strDefaultValue As String = "") As String
    Function SetUserParam(strKey As String, strValue As String) As Boolean
    Function SetUserParam_AllUsers(strKey As String, strValue As String) As Boolean

    Function DeleteAllUserParams(intJ03ID As Integer) As Boolean
    Sub SetAsVisitedUpgradeInfo(intPID As Integer)
    Sub SetAsWaitingOnVisitUpgradeInfo()

    'Function LoadDockState(strPage As String) As String
    'Function SaveDockState(strPage As String, strDockState As String)
    Sub AppendAccessLog(intJ03ID As Integer, cLog As BO.j90LoginAccessLog)
    Function GetMyTag(bolClearAfterRead As Boolean) As String
    Sub SetMyTag(strTagValue As String)

    Function GetList_j90(intJ03ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.j90LoginAccessLog)
    Function SaveAllFavouriteProjects(intJ03ID As Integer, p41ids As List(Of Integer)) As Boolean
    Function AppendOrRemoveFavouriteProject(intJ03ID As Integer, p41ids As List(Of Integer), bolRemove As Boolean) As Boolean
    Function GetList_PageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.x61PageTab)
    Function SavePageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum, x61ids As List(Of Integer)) As Boolean
End Interface
Class j03UserBL
    Inherits BLMother
    Implements Ij03UserBL
    Private WithEvents _cDL As DL.j03UserDL
    Private _x36params As List(Of BO.x36UserParam) = Nothing

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j03UserDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j03User) As Boolean Implements Ij03UserBL.Save
        With cRec
            If Trim(.j03Login) = "" Then _Error = "Chybí přihlašovací jméno (login)." : Return False
            If .j04ID = 0 Then _Error = "Chybí aplikační role." : Return False
            If .PID <> 0 And .j02ID = 0 Then _Error = "Chybí vazba na osobní profil." : Return False
            If .PID = 0 Then .j03ModalWindowsFlag = 1
        End With
        If IsExistUserByLogin(cRec.j03Login, cRec.PID) Then
            _Error = "Uživatelské jméno [" & cRec.j03Login & "] již je v databázi obsazeno." : Return False
        End If

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j03User Implements Ij03UserBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByLogin(strLogin As String) As BO.j03User Implements Ij03UserBL.LoadByLogin
        Return _cDL.LoadByLogin(strLogin)
    End Function
    Public Function LoadByJ02ID(intJ02ID As Integer) As BO.j03User Implements Ij03UserBL.LoadByJ02ID
        Return _cDL.LoadByJ02ID(intJ02ID)
    End Function
    Public Function LoadSysProfile(strLogin As String) As BO.j03UserSYS Implements Ij03UserBL.LoadSysProfile
        Return _cDL.LoadSysProfile(strLogin)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij03UserBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryJ03) As IEnumerable(Of BO.j03User) Implements Ij03UserBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetVirtualCount(mq As BO.myQueryJ03) As Integer Implements Ij03UserBL.GetVirtualCount
        Return _cDL.GetVirtualCount(mq)
    End Function

    Public Sub SYS_AppendJ90Log(intJ03ID As Integer, cLog As BO.j90LoginAccessLog) Implements Ij03UserBL.AppendAccessLog
        _cDL.SYS_AppendJ90Log(intJ03ID, cLog)
    End Sub
    Public Function SYS_SetUserParam(strKey As String, strValue As String) As Boolean Implements Ij03UserBL.SetUserParam
        Return _cDL.SYS_SetUserParam(_cUser.PID, strKey, strValue)
    End Function
    Public Function SYS_SetUserParam_AllUsers(strKey As String, strValue As String) As Boolean Implements Ij03UserBL.SetUserParam_AllUsers
        Return _cDL.SYS_SetUserParam_AllUsers(strKey, strValue)
    End Function
    Public Sub InhaleUserParams(x36keys As List(Of String)) Implements Ij03UserBL.InhaleUserParams
        _x36params = _cDL.SYS_GetList_UserParams(_cUser.PID, x36keys).ToList
    End Sub
    Public Sub InhaleUserParams(strKey1 As String, Optional strKey2 As String = "", Optional strKey3 As String = "") Implements Ij03UserBL.InhaleUserParams
        Dim lis As New List(Of String)
        lis.Add(strKey1)
        If strKey2 <> "" Then lis.Add(strKey2)
        If strKey3 <> "" Then lis.Add(strKey3)

        _x36params = _cDL.SYS_GetList_UserParams(_cUser.PID, lis).ToList
    End Sub

    Public Function GetUserParam(strKey As String, Optional strDefaultValue As String = "") As String Implements Ij03UserBL.GetUserParam
        If _x36params Is Nothing Then
            InhaleUserParams(strKey)
            If _x36params Is Nothing Then Return strDefaultValue
        End If
        Dim cRec As BO.x36UserParam = _x36params.Find(Function(p As BO.x36UserParam) p.x36Key = strKey)
        If cRec Is Nothing Then
            Return strDefaultValue
        Else
            Return cRec.x36Value
        End If
    End Function

    Public Function GetMyTag(bolClearAfterRead As Boolean) As String Implements Ij03UserBL.GetMyTag
        Return _cDL.SYS_GetMyTag(_cUser.PID, "mytag", bolClearAfterRead)
    End Function
    Public Sub SetMyTag(strTagValue As String) Implements Ij03UserBL.SetMyTag
        Me.SYS_SetUserParam("mytag", strTagValue)
    End Sub
    Public Function SYS_DeleteAllUserParams(intJ03ID As Integer) As Boolean Implements Ij03UserBL.DeleteAllUserParams
        Return _cDL.SYS_DeleteAllUserParams(intJ03ID)
    End Function
    
    
    
    Public Function RenameLogin(cRec As BO.j03User, strNewLogin As String) As Boolean Implements Ij03UserBL.RenameLogin
        If Trim(strNewLogin) = "" Then
            _Error = "Chybí nový login." : Return False
        End If
        Return _cDL.RenameLogin(cRec, strNewLogin)
    End Function

    Public Function IsExistUserByLogin(strLogin As String, intJ03ID_Exclude As Integer) As Boolean Implements Ij03UserBL.IsExistUserByLogin
        Return _cDL.IsExistUserByLogin(strLogin, intJ03ID_Exclude)
    End Function

    Public Sub SetAsVisitedUpgradeInfo(intPID As Integer) Implements Ij03UserBL.SetAsVisitedUpgradeInfo
        _cDL.SetAsVisitedUpgradeInfo(intPID)
    End Sub
    Public Sub SetAsWaitingOnVisitUpgradeInfo() Implements Ij03UserBL.SetAsWaitingOnVisitUpgradeInfo
        _cDL.SetAsWaitingOnVisitUpgradeInfo()
    End Sub
    Public Function GetList_j90(intJ03ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.j90LoginAccessLog) Implements Ij03UserBL.GetList_j90
        Return _cDL.GetList_j90(intJ03ID, d1, d2)
    End Function
    Public Function SaveAllFavouriteProjects(intJ03ID As Integer, p41ids As List(Of Integer)) As Boolean Implements Ij03UserBL.SaveAllFavouriteProjects
        Return _cDL.SaveAllFavouriteProjects(intJ03ID, p41ids)
    End Function
    Public Function AppendOrRemoveFavouriteProject(intJ03ID As Integer, p41ids As List(Of Integer), bolRemove As Boolean) As Boolean Implements Ij03UserBL.AppendOrRemoveFavouriteProject
        Return _cDL.AppendOrRemoveFavouriteProject(intJ03ID, p41ids, bolRemove)
    End Function
    ''Public Function LoadDockState(strPage As String) As String Implements Ij03UserBL.LoadDockState
    ''    Return _cDL.SYS_LoadDockState(strPage)
    ''End Function
    ''Public Function SaveDockState(strPage As String, strDockState As String) Implements Ij03UserBL.SaveDockState
    ''    Return _cDL.SYS_SaveDockState(strPage, strDockState)
    ''End Function
    Public Function GetList_PageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.x61PageTab) Implements Ij03UserBL.GetList_PageTabs
        Return _cDL.GetList_PageTabs(intJ03ID, x29id)
    End Function
    Public Function SavePageTabs(intJ03ID As Integer, x29id As BO.x29IdEnum, x61ids As List(Of Integer)) As Boolean Implements Ij03UserBL.SavePageTabs
        If x61ids.Count = 0 Then
            _Error = "Musíte vybrat minimálně jednu záložku." : Return False
        End If
        Return _cDL.SavePageTabs(intJ03ID, x29id, x61ids)
    End Function
End Class
