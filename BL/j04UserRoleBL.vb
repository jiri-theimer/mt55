Public Interface Ij04UserRoleBL
    Inherits IFMother
    Function Save(cRec As BO.j04UserRole, lisX53 As List(Of BO.x53Permission)) As Boolean
    Function Load(intPID As Integer) As BO.j04UserRole
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j04UserRole)
End Interface
Class j04UserRoleBL
    Inherits BLMother
    Implements Ij04UserRoleBL
    Private WithEvents _cDL As DL.j04UserRoleDL

    
    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j04UserRoleDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j04UserRole, lisX53 As List(Of BO.x53Permission)) As Boolean Implements Ij04UserRoleBL.Save
        'If BO.BAS.IsListEmpty(lisX53) Then
        '    _Error = "Role musí mít minimálně jedno oprávnění!" : Return False
        'End If
        Dim cX67BL As New BL.x67EntityRoleBL(_cUser)
        Dim cRecX67 As BO.x67EntityRole = Nothing
        If cRec.x67ID <> 0 Then
            cRecX67 = cX67BL.Load(cRec.x67ID)
        Else
            cRecX67 = New BO.x67EntityRole
        End If
        With cRecX67
            .x67Name = cRec.j04Name
            .x29ID = BO.x29IdEnum.j03User
            If cX67BL.Save(cRecX67, lisX53) Then
                cRec.x67ID = cX67BL.LastSavedPID
            Else
                _Error = cX67BL.ErrorMessage
                Return False
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j04UserRole Implements Ij04UserRoleBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij04UserRoleBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j04UserRole) Implements Ij04UserRoleBL.GetList
        Return _cDL.GetList(myQuery)
    End Function

    
End Class
