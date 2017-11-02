Public Interface Ij07PersonPositionBL
    Inherits IFMother
    Function Save(cRec As BO.j07PersonPosition) As Boolean
    Function Load(intPID As Integer) As BO.j07PersonPosition
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j07PersonPosition)

End Interface
Class j07PersonPositionBL
    Inherits BLMother
    Implements Ij07PersonPositionBL
    Private WithEvents _cDL As DL.j07PersonPositionDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j07PersonPositionDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j07PersonPosition) As Boolean Implements Ij07PersonPositionBL.Save
        With cRec
            If Trim(.j07Name) = "" Then _Error = "Chybí název pozice." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j07PersonPosition Implements Ij07PersonPositionBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij07PersonPositionBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j07PersonPosition) Implements Ij07PersonPositionBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
