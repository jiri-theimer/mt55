Public Interface Ip42ProjectTypeBL
    Inherits IFMother
    Function Save(cRec As BO.p42ProjectType, lisP43 As List(Of BO.p43ProjectType_Workload)) As Boolean
    Function Load(intPID As Integer) As BO.p42ProjectType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p42ProjectType)
    Function GetList_p43(intPID As Integer) As IEnumerable(Of BO.p43ProjectType_Workload)

End Interface
Class p42ProjectTypeBL
    Inherits BLMother
    Implements Ip42ProjectTypeBL
    Private WithEvents _cDL As DL.p42ProjectTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p42ProjectTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p42ProjectType, lisP43 As List(Of BO.p43ProjectType_Workload)) As Boolean Implements Ip42ProjectTypeBL.Save
        With cRec
            If Trim(.p42Name) = "" Then _Error = "Chybí název typu projektu." : Return False
            If .x38ID = 0 Then _Error = "Chybí vazba na číselnou řadu." : Return False
        End With

        Return _cDL.Save(cRec, lisP43)
    End Function
    Public Function Load(intPID As Integer) As BO.p42ProjectType Implements Ip42ProjectTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip42ProjectTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p42ProjectType) Implements Ip42ProjectTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_p43(intPID As Integer) As IEnumerable(Of BO.p43ProjectType_Workload) Implements Ip42ProjectTypeBL.GetList_p43
        Return _cDL.GetList_p43(intPID)
    End Function
End Class
