
Public Interface Ip61ActivityClusterBL
    Inherits IFMother
    Function Save(cRec As BO.p61ActivityCluster, lisP32 As List(Of BO.p32Activity)) As Boolean
    Function Load(intPID As Integer) As BO.p61ActivityCluster
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p61ActivityCluster)
    Function GetList_p32(intPID As Integer) As IEnumerable(Of BO.p32Activity)
End Interface
Class p61ActivityClusterBL
    Inherits BLMother
    Implements Ip61ActivityClusterBL
    Private WithEvents _cDL As DL.p61ActivityClusterDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p61ActivityClusterDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p61ActivityCluster, lisP32 As List(Of BO.p32Activity)) As Boolean Implements Ip61ActivityClusterBL.Save
        If Trim(cRec.p61Name) = "" Then _Error = "Chybí název." : Return False
        If BO.BAS.IsListEmpty(lisP32) Then
            _Error = "Klastr musí mít minimálně jednu aktivitu!" : Return False
        End If

        Return _cDL.Save(cRec, lisP32)
    End Function
    Public Function Load(intPID As Integer) As BO.p61ActivityCluster Implements Ip61ActivityClusterBL.Load
        Return _cDL.Load(intPID)
    End Function
   
    Public Function Delete(intPID As Integer) As Boolean Implements Ip61ActivityClusterBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p61ActivityCluster) Implements Ip61ActivityClusterBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_p32(intPID As Integer) As IEnumerable(Of BO.p32Activity) Implements Ip61ActivityClusterBL.GetList_p32
        Dim mq As New BO.myQueryP32
        mq.p61ID = intPID
        Return Factory.p32ActivityBL.GetList(mq)
    End Function
End Class
