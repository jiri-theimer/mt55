Public Interface Ip29ContactTypeBL
    Inherits IFMother
    Function Save(cRec As BO.p29ContactType) As Boolean
    Function Load(intPID As Integer) As BO.p29ContactType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p29ContactType)
    
End Interface
Class p29ContactTypeBL
    Inherits BLMother
    Implements Ip29ContactTypeBL
    Private WithEvents _cDL As DL.p29ContactTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p29ContactTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p29ContactType) As Boolean Implements Ip29ContactTypeBL.Save
        With cRec
            If Trim(.p29Name) = "" Then _Error = "Chybí název typu kontaktu." : Return False
            If .x38ID = 0 Then _Error = "Chybí vazba na číselnou řadu." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p29ContactType Implements Ip29ContactTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip29ContactTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p29ContactType) Implements Ip29ContactTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function

End Class
