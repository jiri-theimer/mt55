Public Interface Ip32ActivityBL
    Inherits IFMother
    Function Save(cRec As BO.p32Activity) As Boolean
    Function Load(intPID As Integer) As BO.p32Activity
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryP32) As IEnumerable(Of BO.p32Activity)

End Interface
Class p32ActivityBL
    Inherits BLMother
    Implements Ip32ActivityBL
    Private WithEvents _cDL As DL.p32ActivityDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p32ActivityDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p32Activity) As Boolean Implements Ip32ActivityBL.Save
        With cRec
            If Trim(.p32Name) = "" Then _Error = "Chybí název aktivity." : Return False
            If .p34ID = 0 Then _Error = "Chybí vazba na sešit aktivit" : Return False
            If .p32Code <> "" Then

            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p32Activity Implements Ip32ActivityBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip32ActivityBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryP32) As IEnumerable(Of BO.p32Activity) Implements Ip32ActivityBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class

