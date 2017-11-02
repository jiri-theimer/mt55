Public Interface Ib01WorkflowTemplateBL
    Inherits ifMother
    Function Save(cRec As BO.b01WorkflowTemplate) As Boolean
    Function Load(intPID As Integer) As BO.b01WorkflowTemplate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.b01WorkflowTemplate)
   
End Interface
Class b01WorkflowTemplateBL
    Inherits BLMother
    Implements Ib01WorkflowTemplateBL
    Private WithEvents _cDL As DL.b01WorkflowTemplateDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.b01WorkflowTemplateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function Load(intPID As Integer) As BO.b01WorkflowTemplate Implements Ib01WorkflowTemplateBL.Load
        Return _cDL.Load(intPID)
    End Function

    Public Function Save(cRec As BO.b01WorkflowTemplate) As Boolean Implements Ib01WorkflowTemplateBL.Save
        Return _cDL.Save(cRec)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ib01WorkflowTemplateBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.b01WorkflowTemplate) Implements Ib01WorkflowTemplateBL.GetList
        Return _cDL.GetList(myQuery)
    End Function
End Class
