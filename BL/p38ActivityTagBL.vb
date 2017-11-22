Public Interface Ip38ActivityTagBL
    Inherits IFMother
    Function Save(cRec As BO.p38ActivityTag) As Boolean
    Function Load(intPID As Integer) As BO.p38ActivityTag
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p38ActivityTag)
End Interface
Class p38ActivityTagBL
    Inherits BLMother
    Implements Ip38ActivityTagBL
    Private WithEvents _cDL As DL.p38ActivityTagDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p38ActivityTagDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p38ActivityTag) As Boolean Implements Ip38ActivityTagBL.Save
        With cRec
            If Trim(.p38Name) = "" Then _Error = "Chybí název!" : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p38ActivityTag Implements Ip38ActivityTagBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip38ActivityTagBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p38ActivityTag) Implements Ip38ActivityTagBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
