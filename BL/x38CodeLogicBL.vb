Public Interface Ix38CodeLogicBL
    Inherits IFMother
    Function Save(cRec As BO.x38CodeLogic) As Boolean
    Function Load(intPID As Integer) As BO.x38CodeLogic
    Function Delete(intPID As Integer) As Boolean
    Function GetList(x29id As BO.x29IdEnum) As IEnumerable(Of BO.x38CodeLogic)

End Interface

Class x38CodeLogicBL
    Inherits BLMother
    Implements Ix38CodeLogicBL
    Private WithEvents _cDL As DL.x38CodeLogicDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x38CodeLogicDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x38CodeLogic) As Boolean Implements Ix38CodeLogicBL.Save
        With cRec
            If Trim(.x38Name) = "" Then _Error = "Chybí název číselné řady." : Return False
            If .x29ID = 0 Then _Error = "Chybí vazba na entitu." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.x38CodeLogic Implements Ix38CodeLogicBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix38CodeLogicBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(x29id As BO.x29IdEnum) As IEnumerable(Of BO.x38CodeLogic) Implements Ix38CodeLogicBL.GetList
        Return _cDL.GetList(x29id)
    End Function
End Class
