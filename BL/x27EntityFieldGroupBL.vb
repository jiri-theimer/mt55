
Public Interface Ix27EntityFieldGroupBL
    Inherits IFMother
    Function Save(cRec As BO.x27EntityFieldGroup) As Boolean
    Function Load(intPID As Integer) As BO.x27EntityFieldGroup
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x27EntityFieldGroup)

End Interface
Class x27EntityFieldGroupBL
    Inherits BLMother
    Implements Ix27EntityFieldGroupBL
    Private WithEvents _cDL As DL.x27EntityFieldGroupDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x27EntityFieldGroupDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x27EntityFieldGroup) As Boolean Implements Ix27EntityFieldGroupBL.Save
        With cRec
            If Trim(.x27Name) = "" Then _Error = "Chybí název skupiny." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.x27EntityFieldGroup Implements Ix27EntityFieldGroupBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix27EntityFieldGroupBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x27EntityFieldGroup) Implements Ix27EntityFieldGroupBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
