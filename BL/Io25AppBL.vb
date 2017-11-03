
Public Interface Io25AppBL
    Inherits IFMother
    Function Save(cRec As BO.o25App) As Boolean
    Function Load(intPID As Integer) As BO.o25App
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o25App)

End Interface
Class o25AppBL
    Inherits BLMother
    Implements Io25AppBL
    Private WithEvents _cDL As DL.o25AppDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o25AppDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o25App) As Boolean Implements Io25AppBL.Save
        With cRec
            If Trim(.o25Code) = "" Then _Error = "Chybí kód aplikace." : Return False
            If Trim(.o25Name) = "" Then _Error = "Chybí název." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o25App Implements Io25AppBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io25AppBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o25App) Implements Io25AppBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
