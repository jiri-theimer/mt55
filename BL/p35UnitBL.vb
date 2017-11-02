Public Interface Ip35UnitBL
    Inherits IFMother
    Function Save(cRec As BO.p35Unit) As Boolean
    Function Load(intPID As Integer) As BO.p35Unit
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p35Unit)

End Interface
Class p35UnitBL
    Inherits BLMother
    Implements Ip35UnitBL
    Private WithEvents _cDL As DL.p35UnitDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p35UnitDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p35Unit) As Boolean Implements Ip35UnitBL.Save
        With cRec
            If Trim(.p35Name) = "" Then _Error = "Chybí název jednotky." : Return False
            If Trim(.p35Code) = "" Then _Error = "Chybí kód jednotky." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p35Unit Implements Ip35UnitBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip35UnitBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p35Unit) Implements Ip35UnitBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
