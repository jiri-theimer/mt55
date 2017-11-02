Public Interface Ip59PriorityBL
    Inherits IFMother
    Function Save(cRec As BO.p59Priority) As Boolean
    Function Load(intPID As Integer) As BO.p59Priority
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p59Priority)

End Interface

Class p59PriorityBL
    Inherits BLMother
    Implements Ip59PriorityBL
    Private WithEvents _cDL As DL.p59PriorityDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p59PriorityDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p59Priority) As Boolean Implements Ip59PriorityBL.Save
        With cRec
            If Trim(.p59Name) = "" Then _Error = "Chybí název priority." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p59Priority Implements Ip59PriorityBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip59PriorityBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p59Priority) Implements Ip59PriorityBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
