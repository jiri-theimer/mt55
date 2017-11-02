Public Interface Io21MilestoneTypeBL
    Inherits IFMother
    Function Save(cRec As BO.o21MilestoneType) As Boolean
    Function Load(intPID As Integer) As BO.o21MilestoneType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o21MilestoneType)

End Interface
Class o21MilestoneTypeBL
    Inherits BLMother
    Implements Io21MilestoneTypeBL
    Private WithEvents _cDL As DL.o21MilestoneTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o21MilestoneTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o21MilestoneType) As Boolean Implements Io21MilestoneTypeBL.Save
        With cRec
            Select Case .x29ID
                Case BO.x29IdEnum.j02Person, BO.x29IdEnum.p41Project, BO.x29IdEnum.p28Contact, BO.x29IdEnum.p91Invoice, BO.x29IdEnum.p90Proforma
                Case Else
                    _Error = "Na vstupu není druh vazební entity." : Return False
            End Select
            If Trim(.o21Name) = "" Then _Error = "Chybí název." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o21MilestoneType Implements Io21MilestoneTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io21MilestoneTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o21MilestoneType) Implements Io21MilestoneTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
