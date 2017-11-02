Public Interface Ij17CountryBL
    Inherits IFMother
    Function Save(cRec As BO.j17Country) As Boolean
    Function Load(intPID As Integer) As BO.j17Country
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j17Country)

End Interface

Class j17CountryBL
    Inherits BLMother
    Implements Ij17CountryBL
    Private WithEvents _cDL As DL.j17CountryDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j17CountryDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j17Country) As Boolean Implements Ij17CountryBL.Save
        With cRec
            If Trim(.j17Name) = "" Then _Error = "Chybí název státu." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j17Country Implements Ij17CountryBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij17CountryBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j17Country) Implements Ij17CountryBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
