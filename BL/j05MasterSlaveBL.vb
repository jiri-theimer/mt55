
Public Interface Ij05MasterSlaveBL
    Inherits IFMother
    Function Save(cRec As BO.j05MasterSlave) As Boolean
    Function Load(intPID As Integer) As BO.j05MasterSlave
    Function Delete(intPID As Integer) As Boolean
    Function GetList(intJ02ID_Master As Integer, intJ02ID_Slave As Integer, intJ11ID_Slave As Integer) As IEnumerable(Of BO.j05MasterSlave)
End Interface

Class j05MasterSlaveBL
    Inherits BLMother
    Implements Ij05MasterSlaveBL
    Private WithEvents _cDL As DL.j05MasterSlaveDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j05MasterSlaveDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j05MasterSlave) As Boolean Implements Ij05MasterSlaveBL.Save
        With cRec
            If .j02ID_Master = 0 Then _Error = "Chybí nadřízená osoba." : Return False
            If .j02ID_Slave = 0 And .j11ID_Slave = 0 Then _Error = "Chybí podřízená osoba nebo tým." : Return False
            If .j02ID_Slave <> 0 And .j11ID_Slave <> 0 Then _Error = "Za podřízenou stranu vyberte buď osobu nebo pouze tým." : Return False
            If .j02ID_Master = .j02ID_Slave Then _Error = "Vazba není logická." : Return False
        End With
        If cRec.j02ID_Slave > 0 Then
            If GetList(cRec.j02ID_Master, cRec.j02ID_Slave, 0).Where(Function(p) p.PID <> cRec.PID).Count > 0 Then
                _Error = "Tento vztah podřízený/nadřízený je již uložen v jiném záznamu!" : Return False
            End If
        End If
        If cRec.j11ID_Slave > 0 Then
            If GetList(cRec.j02ID_Master, 0, cRec.j11ID_Slave).Where(Function(p) p.PID <> cRec.PID).Count > 0 Then
                _Error = "Tento vztah podřízený/nadřízený je již uložen v jiném záznamu!" : Return False
            End If
        End If
        
        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j05MasterSlave Implements Ij05MasterSlaveBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij05MasterSlaveBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(intJ02ID_Master As Integer, intJ02ID_Slave As Integer, intJ11ID_Slave As Integer) As IEnumerable(Of BO.j05MasterSlave) Implements Ij05MasterSlaveBL.GetList
        Return _cDL.GetList(intJ02ID_Master, intJ02ID_Slave, intJ11ID_Slave)
    End Function

    
End Class
