Public Interface Ip63OverheadBL
    Inherits IFMother
    Function Save(cRec As BO.p63Overhead) As Boolean
    Function Load(intPID As Integer) As BO.p63Overhead
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p63Overhead)

End Interface
Class p63OverheadBL
    Inherits BLMother
    Implements Ip63OverheadBL
    Private WithEvents _cDL As DL.p63OverheadDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p63OverheadDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p63Overhead) As Boolean Implements Ip63OverheadBL.Save
        With cRec
            If Trim(.p63Name) = "" Then _Error = "Chybí název." : Return False
            If .p32ID = 0 Then _Error = "Chybí vazba na aktivitu." : Return False
            If .p63PercentRate = 0 Then _Error = "Nulové procento přirážky." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p63Overhead Implements Ip63OverheadBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip63OverheadBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p63Overhead) Implements Ip63OverheadBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
