Public Interface Ip57TaskTypeBL
    Inherits IFMother
    Function Save(cRec As BO.p57TaskType) As Boolean
    Function Load(intPID As Integer) As BO.p57TaskType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p57TaskType)
End Interface

Class p57TaskTypeBL
    Inherits BLMother
    Implements Ip57TaskTypeBL
    Private WithEvents _cDL As DL.p57TaskTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p57TaskTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p57TaskType) As Boolean Implements Ip57TaskTypeBL.Save
        With cRec
            If Trim(.p57Name) = "" Then _Error = "Chybí název." : Return False
            If .x38ID = 0 Then _Error = "Chybí vazba na číselnou řadu." : Return False
            If .p57IsHelpdesk And .b01ID = 0 Then
                _Error = "Pro zapisování helpdesk požadavků musí existovat vazba typu na workflow šablonu." : Return False
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p57TaskType Implements Ip57TaskTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip57TaskTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p57TaskType) Implements Ip57TaskTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
