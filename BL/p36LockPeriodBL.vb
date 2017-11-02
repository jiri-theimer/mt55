Public Interface Ip36LockPeriodBL
    Inherits IFMother
    Function Save(cRec As BO.p36LockPeriod, lisP37 As List(Of BO.p37LockPeriod_Sheet)) As Boolean
    Function Load(intPID As Integer) As BO.p36LockPeriod
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p36LockPeriod)
    Function GetList_p37(intPID As Integer) As IEnumerable(Of BO.p37LockPeriod_Sheet)
End Interface
Class p36LockPeriodBL
    Inherits BLMother
    Implements Ip36LockPeriodBL
    Private WithEvents _cDL As DL.p36LockPeriodDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p36LockPeriodDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p36LockPeriod, lisP37 As List(Of BO.p37LockPeriod_Sheet)) As Boolean Implements Ip36LockPeriodBL.Save
        With cRec
            If .p36IsAllPersons Then
                .j02ID = 0 : .j11ID = 0
            Else
                If .j02ID = 0 And .j11ID = 0 Then _Error = "Musíte specifikovat osobu nebo tým osob!" : Return False
            End If

            If .p36DateFrom > .p36DateUntil Then
                _Error = "Rozsah období není korektní!" : Return False
            End If
            If Not .p36IsAllSheets Then
                If lisP37.Count = 0 Then _Error = "Musíte zaškrtnout minimálně jeden worksheet sešit." : Return False
            End If
        End With

        If _cDL.Save(cRec, lisP37) Then
            If cRec.PID = 0 Then
                Me.RaiseAppEvent(BO.x45IDEnum.p36_new, _LastSavedPID)
            Else
                Me.RaiseAppEvent(BO.x45IDEnum.p36_update, _LastSavedPID)
            End If
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.p36LockPeriod Implements Ip36LockPeriodBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip36LockPeriodBL.Delete
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p36_delete, intPID)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p36LockPeriod) Implements Ip36LockPeriodBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_p37(intPID As Integer) As IEnumerable(Of BO.p37LockPeriod_Sheet) Implements Ip36LockPeriodBL.GetList_p37
        Return _cDL.GetList_p37(intPID)
    End Function
End Class
