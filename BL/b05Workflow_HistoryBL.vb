Public Interface Ib05Workflow_HistoryBL
    Function Load(intPID As Integer) As BO.b05Workflow_History
    Function Save(cRec As BO.b05Workflow_History) As Boolean
    Function GetList(intRecordPID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.b05Workflow_History)
    
End Interface
Class b05Workflow_HistoryBL
    Inherits BLMother
    Implements Ib05Workflow_HistoryBL
    Private WithEvents _cDL As DL.b05Workflow_HistoryDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.b05Workflow_HistoryDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Function Load(intPID As Integer) As BO.b05Workflow_History Implements Ib05Workflow_HistoryBL.Load
        Return _cDL.Load(intPID)
    End Function
    
    Function GetList(intRecordPID As Integer, x29id As BO.x29IdEnum) As IEnumerable(Of BO.b05Workflow_History) Implements Ib05Workflow_HistoryBL.GetList
        Return _cDL.GetList(intRecordPID, x29id)

    End Function

    Public Function Save(cRec As BO.b05Workflow_History) As Boolean Implements Ib05Workflow_HistoryBL.Save
        Return _cDL.Save(cRec)
    End Function

End Class
