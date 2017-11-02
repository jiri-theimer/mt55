Public MustInherit Class DLMother
    Protected Property _Error As String
    Protected WithEvents _cDB As DL.DbHandler
    Protected Property _curUser As BO.j03UserSYS
    Public Event OnError(strError As String)
    Public Event OnSaveRecord(intLastSavedPID As Integer)



    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public ReadOnly Property LastIdentityValue As Integer
        Get
            Return _cDB.LastIdentityValue
        End Get
    End Property
    Public ReadOnly Property LastSavedRecordPID As Integer
        Get
            Return _cDB.LastSavedRecordPID
        End Get
    End Property
    Public Sub New(ServiceUser As BO.j03UserSYS)
        _curUser = ServiceUser

        _cDB = New DL.DbHandler()
        If _curUser Is Nothing Then Return
        If _curUser.ExplicitConnectString = "" Then Return
        _cDB.ChangeConString(_curUser.ExplicitConnectString)    'požadavek za změnu aplikační databáze
    End Sub


    Private Sub _cDB_OnDBError(strError As String) Handles _cDB.OnDBError
        _Error = strError
        RaiseEvent OnError(strError)
    End Sub
    Private Sub _cDB_OnSaveRecord(intLastSavedPID As Integer) Handles _cDB.OnSaveRecord
        RaiseEvent OnSaveRecord(intLastSavedPID)
    End Sub

    Public Sub ChangeConnectString(strConString As String)
        _cDB.ChangeConString(strConString)
    End Sub
    
End Class
