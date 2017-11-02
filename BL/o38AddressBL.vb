Public Interface Io38AddressBL
    Inherits IFMother
    Function Save(cRec As BO.o38Address) As Boolean
    Function Load(intPID As Integer) As BO.o38Address
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryO38) As IEnumerable(Of BO.o38Address)
    Function GetList_DistinctCountries() As List(Of String)
End Interface
Class o38AddressBL
    Inherits BLMother
    Implements Io38AddressBL
    Private WithEvents _cDL As DL.o38AddressDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o38AddressDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o38Address) As Boolean Implements Io38AddressBL.Save
        With cRec
            If Trim(.o38City) = "" And Trim(.o38Street) = "" Then _Error = "V adrese je nutné vyplnit město nebo ulici!" : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o38Address Implements Io38AddressBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io38AddressBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryO38) As IEnumerable(Of BO.o38Address) Implements Io38AddressBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_DistinctCountries() As List(Of String) Implements Io38AddressBL.GetList_DistinctCountries
        Return _cDL.GetList_DistinctCountries()
    End Function
End Class
