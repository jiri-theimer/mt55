Public Interface IDataCopyManagerBL
    Inherits IFMother
    Function CopyDataTableContent(dt As DataTable, Optional strExplicitDestTableName As String = "") As Boolean
End Interface
Class DataCopyManagerBL
    Inherits BLMother
    Implements IDataCopyManagerBL
    Private WithEvents _cDL As DL.SysDbObjectDL
    

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.SysDbObjectDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function CopyDataTableContent(dt As DataTable, Optional strExplicitDestTableName As String = "") As Boolean Implements IDataCopyManagerBL.CopyDataTableContent
        Return _cDL.CopyDataTableContent(dt, strExplicitDestTableName)
    End Function
End Class
