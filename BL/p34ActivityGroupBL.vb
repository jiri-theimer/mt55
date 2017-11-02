Public Interface Ip34ActivityGroupBL
    Inherits IFMother
    Function Save(cRec As BO.p34ActivityGroup, Optional intP32ID_SystemDefault As Integer = 0) As Boolean
    Function Load(intPID As Integer) As BO.p34ActivityGroup
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p34ActivityGroup)
    Function GetList_WorksheetEntryInProject(intP41ID As Integer, intP42ID As Integer, intJ18ID As Integer, intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup)
    Function GetList_WorksheetEntry_InAllProjects(intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup)
End Interface
Class p34ActivityGroupBL
    Inherits BLMother
    Implements Ip34ActivityGroupBL
    Private WithEvents _cDL As DL.p34ActivityGroupDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p34ActivityGroupDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p34ActivityGroup, Optional intP32ID_SystemDefault As Integer = 0) As Boolean Implements Ip34ActivityGroupBL.Save
        With cRec
            If Trim(.p34Name) = "" Then _Error = "Chybí název sešitu." : Return False
            If CInt(.p33ID) = 0 Then _Error = "Chybí formát vstupních dat." : Return False
        End With

        Return _cDL.Save(cRec, intP32ID_SystemDefault)
    End Function
    Public Function Load(intPID As Integer) As BO.p34ActivityGroup Implements Ip34ActivityGroupBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip34ActivityGroupBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p34ActivityGroup) Implements Ip34ActivityGroupBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_WorksheetEntryInProject(intP41ID As Integer, intP42ID As Integer, intJ18ID As Integer, intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup) Implements Ip34ActivityGroupBL.GetList_WorksheetEntryInProject
        Return _cDL.GetList_WorksheetEntryInProject(intP41ID, intP42ID, intJ18ID, intJ02ID)
    End Function
    Public Function GetList_WorksheetEntry_InAllProjects(intJ02ID As Integer) As IEnumerable(Of BO.p34ActivityGroup) Implements Ip34ActivityGroupBL.GetList_WorksheetEntry_InAllProjects
        Return _cDL.GetList_WorksheetEntry_InAllProjects(intJ02ID)
    End Function
End Class
