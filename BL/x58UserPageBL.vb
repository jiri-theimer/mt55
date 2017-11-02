
Public Interface Ix58UserPageBL
    Inherits IFMother
    Function Save(cRec As BO.x58UserPage, lisX57 As List(Of BO.x57UserPageBinding)) As Boolean
    Function Load(intPID As Integer) As BO.x58UserPage
    Function LoadByJ03(intJ03ID As Integer) As BO.x58UserPage
    
    Function GetList_x57(intPID As Integer) As IEnumerable(Of BO.x57UserPageBinding)

End Interface
Class x58UserPageBL
    Inherits BLMother
    Implements Ix58UserPageBL
    Private WithEvents _cDL As DL.x58UserPageDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x58UserPageDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x58UserPage, lisX57 As List(Of BO.x57UserPageBinding)) As Boolean Implements Ix58UserPageBL.Save
        With cRec
            If .j03ID = 0 Then _Error = "Chybí ID uživatele." : Return False
        End With

        Return _cDL.Save(cRec, lisX57)
    End Function
    Public Function Load(intPID As Integer) As BO.x58UserPage Implements Ix58UserPageBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByJ03(intJ03ID As Integer) As BO.x58UserPage Implements Ix58UserPageBL.LoadByJ03
        Return _cDL.LoadByJ03(intJ03ID)
    End Function
   
    Public Function GetList_x57(intPID As Integer) As IEnumerable(Of BO.x57UserPageBinding) Implements Ix58UserPageBL.GetList_x57
        Return _cDL.GetList_x57(intPID)
    End Function
End Class
