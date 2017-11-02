Public Interface Ix67EntityRoleBL
    Inherits IFMother
    Function Save(cRec As BO.x67EntityRole, lisX53 As List(Of BO.x53Permission)) As Boolean
    Function Load(intPID As Integer) As BO.x67EntityRole
    Function LoadChild(intParentID As Integer) As BO.x67EntityRole
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x67EntityRole)
    Function GetList_BoundX53(intPID As Integer) As IEnumerable(Of BO.x53Permission)
    Function GetList_o28(x67ids As List(Of Integer)) As IEnumerable(Of BO.o28ProjectRole_Workload)
    Function GetList_x69(x29ID As BO.x29IdEnum, intRecordPID As Integer) As IEnumerable(Of BO.x69EntityRole_Assign)
    Function GetList_x69(x29ID As BO.x29IdEnum, recPIDs As List(Of Integer)) As IEnumerable(Of BO.x69EntityRole_Assign)
    Sub SaveO28(intX67ID As Integer, lisO28 As List(Of BO.o28ProjectRole_Workload))
    Function TestEntityRolePermission(x29id As BO.x29IdEnum, intRecordPID As Integer, NeededPermissionValue As BO.x53PermValEnum, bolUseLastRowset As Boolean) As Boolean
    Function HasPersonEntityRole(x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean
    ReadOnly Property TestedX67IDs As List(Of Integer)
    Function GetEmails_j02_join_j11(x29ID As BO.x29IdEnum, intRecordPID As Integer, Optional intX67ID As Integer = 0) As List(Of BO.x43MailQueue_Recipient)
End Interface
Class x67EntityRoleBL
    Inherits BLMother
    Implements Ix67EntityRoleBL
    Private WithEvents _cDL As DL.x67EntityRoleDL
    Private _lastLis4TestPerm As IEnumerable(Of BO.x67EntityRole)
    
    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x67EntityRoleDL(ServiceUser)
        _cUser = ServiceUser
        
    End Sub
    Public ReadOnly Property TestedX67IDs As List(Of Integer) Implements Ix67EntityRoleBL.TestedX67IDs
        Get
            If _lastLis4TestPerm Is Nothing Then
                Return New List(Of Integer)
            Else
                Return _lastLis4TestPerm.Select(Function(p) p.PID).ToList
            End If
        End Get
    End Property
    Public Function Save(cRec As BO.x67EntityRole, lisX53 As List(Of BO.x53Permission)) As Boolean Implements Ix67EntityRoleBL.Save
        If cRec.x67Name = "" Then
            _Error = "Chybí název role." : Return False
        End If
        If cRec.x29ID = BO.x29IdEnum._NotSpecified Then
            _Error = "Chybí specifikace entitiy." : Return False
        End If
        Dim lis As IEnumerable(Of BO.x67EntityRole) = GetList().Where(Function(p) p.PID <> cRec.PID And p.x29ID = cRec.x29ID And LCase(Trim(p.x67Name)) = LCase(Trim(cRec.x67Name)))
        If lis.Count > 0 Then
            _Error = "Role s takovým názvem již existuje!" : Return False
        End If
        Dim intX67ID As Integer = 0
        If Not _cDL.Save(cRec, lisX53) Then
            Return False
        Else
            intX67ID = _cDL.LastSavedRecordPID
        End If
        
        If cRec.x29ID = BO.x29IdEnum.p41Project Then
            Dim cChild As BO.x67EntityRole = LoadChild(_cDL.LastSavedRecordPID)
            If Not cChild Is Nothing Then
                cChild.x67Name = cRec.x67Name
                cChild.x67Ordinary = cRec.x67Ordinary
                cChild.ValidFrom = cRec.ValidFrom
                cChild.ValidUntil = cRec.ValidUntil
                cChild.x67RoleValue = cRec.x67RoleValue
                cRec = cChild
            Else
                cRec.SetPID(0)

            End If
            cRec.x67ParentID = _cDL.LastSavedRecordPID
            cRec.x29ID = BO.x29IdEnum.j18Region

            
            _cDL.Save(cRec, lisX53)

            _LastSavedPID = intX67ID
        End If
        Return True
    End Function
    Public Function Load(intPID As Integer) As BO.x67EntityRole Implements Ix67EntityRoleBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadChild(intParentID As Integer) As BO.x67EntityRole Implements Ix67EntityRoleBL.LoadChild
        Return _cDL.LoadChild(intParentID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix67EntityRoleBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x67EntityRole) Implements Ix67EntityRoleBL.GetList
        Return _cDL.GetList(myQuery)
    End Function
    Public Function GetList_BoundX53(intPID As Integer) As IEnumerable(Of BO.x53Permission) Implements Ix67EntityRoleBL.GetList_BoundX53
        Return _cDL.GetList_BoundX53(intPID)
    End Function
    Public Sub SaveO28(intX67ID As Integer, lisO28 As List(Of BO.o28ProjectRole_Workload)) Implements Ix67EntityRoleBL.SaveO28
        _cDL.SaveO28(intX67ID, lisO28)
    End Sub
    Public Function GetList_o28(x67ids As List(Of Integer)) As IEnumerable(Of BO.o28ProjectRole_Workload) Implements Ix67EntityRoleBL.GetList_o28
        Return _cDL.GetList_o28(x67ids)
    End Function
    Public Overloads Function GetList_x69(x29ID As BO.x29IdEnum, intRecordPID As Integer) As IEnumerable(Of BO.x69EntityRole_Assign) Implements Ix67EntityRoleBL.GetList_x69
        Dim recPIDs As New List(Of Integer)
        recPIDs.Add(intRecordPID)
        Return _cDL._GetList_x69(x29ID, recPIDs)
    End Function
    Public Overloads Function GetList_x69(x29ID As BO.x29IdEnum, recPIDs As List(Of Integer)) As IEnumerable(Of BO.x69EntityRole_Assign) Implements Ix67EntityRoleBL.GetList_x69
        Return _cDL._GetList_x69(x29ID, recPIDs)
    End Function
    Public Function GetEmails_j02_join_j11(x29ID As BO.x29IdEnum, intRecordPID As Integer, Optional intX67ID As Integer = 0) As List(Of BO.x43MailQueue_Recipient) Implements Ix67EntityRoleBL.GetEmails_j02_join_j11
        Dim lis As IEnumerable(Of BO.x69EntityRole_Assign) = GetList_x69(x29ID, intRecordPID)
        If intX67ID <> 0 Then lis = lis.Where(Function(p) p.x67ID = intX67ID)

        Dim j02ids As List(Of Integer) = lis.Where(Function(p) p.j02ID <> 0).Select(Function(p) p.j02ID).ToList
        Dim j11ids As List(Of Integer) = lis.Where(Function(p) p.j11ID <> 0).Select(Function(p) p.j11ID).ToList

        Return Me.Factory.j02PersonBL.GetEmails_j02_join_j11(j02ids, j11ids)
    End Function

    Public Function TestEntityRolePermission(x29id As BO.x29IdEnum, intRecordPID As Integer, NeededPermissionValue As BO.x53PermValEnum, bolUseLastRowset As Boolean) As Boolean Implements Ix67EntityRoleBL.TestEntityRolePermission
        Dim lis As IEnumerable(Of BO.x67EntityRole) = _lastLis4TestPerm
        If Not bolUseLastRowset Or _lastLis4TestPerm Is Nothing Then
            lis = _cDL.GetList_EntityPermissionsSource(x29id, intRecordPID, _cUser.j02ID)
            _lastLis4TestPerm = lis
        End If
        For Each c As BO.x67EntityRole In lis
            If c.x67RoleValue.Substring(NeededPermissionValue - 1, 1) = "1" Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function HasPersonEntityRole(x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean Implements Ix67EntityRoleBL.HasPersonEntityRole
        If _lastLis4TestPerm Is Nothing Then
            _lastLis4TestPerm = _cDL.GetList_EntityPermissionsSource(x29id, intRecordPID, _cUser.j02ID)
        End If
        If _lastLis4TestPerm.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
