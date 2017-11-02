Public Interface Ij11TeamBL
    Inherits IFMother
    Function Save(cRec As BO.j11Team, lisJ02 As List(Of BO.j02Person)) As Boolean
    Function Load(intPID As Integer) As BO.j11Team
    Function LoadByImapRobotAddress(strRobotAddress As String) As BO.j11Team
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing, Optional strAllPersonsCaption As String = "") As IEnumerable(Of BO.j11Team)
    Function GetList_BoundJ12(intPID As Integer) As IEnumerable(Of BO.j12Team_Person)
End Interface
Class j11TeamBL
    Inherits BLMother
    Implements Ij11TeamBL
    Private WithEvents _cDL As DL.j11TeamDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j11TeamDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j11Team, lisJ02 As List(Of BO.j02Person)) As Boolean Implements Ij11TeamBL.Save
        If Trim(cRec.j11Name) = "" Then _Error = "Chybí název." : Return False
        If BO.BAS.IsListEmpty(lisJ02) And Not cRec.j11IsAllPersons Then
            _Error = "Tým musí mít minimálně jednoho člena!" : Return False
        End If

        Return _cDL.Save(cRec, lisJ02)
    End Function
    Public Function Load(intPID As Integer) As BO.j11Team Implements Ij11TeamBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress As String) As BO.j11Team Implements Ij11TeamBL.LoadByImapRobotAddress
        Return _cDL.LoadByImapRobotAddress(strRobotAddress)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij11TeamBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing, Optional strAllPersonsCaption As String = "") As IEnumerable(Of BO.j11Team) Implements Ij11TeamBL.GetList
        Dim lis As IEnumerable(Of BO.j11Team) = _cDL.GetList(mq)
        If strAllPersonsCaption <> "" Then
            If lis.Where(Function(p) p.j11IsAllPersons = True).Count > 0 Then
                lis.Where(Function(p) p.j11IsAllPersons = True)(0).j11Name = strAllPersonsCaption
            End If
        End If
        Return lis
    End Function
    Public Function GetList_BoundJ12(intPID As Integer) As IEnumerable(Of BO.j12Team_Person) Implements Ij11TeamBL.GetList_BoundJ12
        Return _cDL.GetList_BoundJ12(intPID)
    End Function
End Class
