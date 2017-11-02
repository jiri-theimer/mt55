Public Interface Ip89ProformaTypeBL
    Inherits IFMother
    Function Save(cRec As BO.p89ProformaType) As Boolean
    Function Load(intPID As Integer) As BO.p89ProformaType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p89ProformaType)

End Interface
Class p89ProformaTypeBL
    Inherits BLMother
    Implements Ip89ProformaTypeBL
    Private WithEvents _cDL As DL.p89ProformaTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p89ProformaTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p89ProformaType) As Boolean Implements Ip89ProformaTypeBL.Save
        With cRec
            If Trim(.p89Name) = "" Then _Error = "Chybí název typu." : Return False
            If .x38ID = 0 Then _Error = "Chybí specifikace číselné řady." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p89ProformaType Implements Ip89ProformaTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip89ProformaTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p89ProformaType) Implements Ip89ProformaTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
