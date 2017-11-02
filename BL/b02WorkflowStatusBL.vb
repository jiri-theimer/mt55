Public Interface Ib02WorkflowStatusBL
    Inherits ifMother
    Function Save(cRec As BO.b02WorkflowStatus) As Boolean
    Function Load(intPID As Integer) As BO.b02WorkflowStatus
    Function Delete(intPID As Integer) As Boolean
    Function GetList(intB01ID As Integer) As IEnumerable(Of BO.b02WorkflowStatus)
    
End Interface
Class b02WorkflowStatusBL
    Inherits BLMother
    Implements Ib02WorkflowStatusBL
    Private WithEvents _cDL As DL.b02WorkflowStatusDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.b02WorkflowStatusDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Delete(intPID As Integer) As Boolean Implements Ib02WorkflowStatusBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(intB01ID As Integer) As System.Collections.Generic.IEnumerable(Of BO.b02WorkflowStatus) Implements Ib02WorkflowStatusBL.GetList
        Return _cDL.GetList(intB01ID)
    End Function

    Public Function Load(intPID As Integer) As BO.b02WorkflowStatus Implements Ib02WorkflowStatusBL.Load
        Return _cDL.Load(intPID)
    End Function

    Public Function Save(cRec As BO.b02WorkflowStatus) As Boolean Implements Ib02WorkflowStatusBL.Save
        If cRec.b01ID = 0 Or Trim(cRec.b02Name) = "" Then
            _Error = "[Název stavu] a [Workflow šablona] jsou povinná pole k vyplnění."
        End If
        If _Error <> "" Then Return False
        If _cDL.Save(cRec) Then
            Return True
        Else
            Return False
        End If
    End Function
    
End Class
