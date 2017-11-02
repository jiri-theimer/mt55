Public Interface Io13AttachmentTypeBL
    Inherits IFMother
    Function Save(cRec As BO.o13AttachmentType) As Boolean
    Function Load(intPID As Integer) As BO.o13AttachmentType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.o13AttachmentType)

End Interface
Class o13AttachmentTypeBL
    Inherits BLMother
    Implements Io13AttachmentTypeBL
    Private WithEvents _cDL As DL.o13AttachmentTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o13AttachmentTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o13AttachmentType) As Boolean Implements Io13AttachmentTypeBL.Save
        With cRec
            If Trim(.o13Name) = "" Then _Error = "Chybí název typu dokumentu." : Return False
            If .x29ID = BO.x29IdEnum._NotSpecified Then _Error = "Chybí druh entity." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o13AttachmentType Implements Io13AttachmentTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io13AttachmentTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.o13AttachmentType) Implements Io13AttachmentTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
