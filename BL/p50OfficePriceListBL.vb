Public Interface Ip50OfficePriceListBL
    Inherits IFMother
    Function Save(cRec As BO.p50OfficePriceList) As Boolean
    Function Load(intPID As Integer) As BO.p50OfficePriceList
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p50OfficePriceList)

End Interface

Class p50OfficePriceListBL
    Inherits BLMother
    Implements Ip50OfficePriceListBL
    Private WithEvents _cDL As DL.p50OfficePriceListDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p50OfficePriceListDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p50OfficePriceList) As Boolean Implements Ip50OfficePriceListBL.Save
        With cRec
            If .p51ID = 0 Then _Error = "Chybí ceník." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p50OfficePriceList Implements Ip50OfficePriceListBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip50OfficePriceListBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p50OfficePriceList) Implements Ip50OfficePriceListBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
