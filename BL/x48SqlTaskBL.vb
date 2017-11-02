Public Interface Ix48SqlTaskBL
    Inherits IFMother
    Function Save(cRec As BO.x48SqlTask) As Boolean
    Function Load(intPID As Integer) As BO.x48SqlTask
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x48SqlTask)
    Function IsWaiting4AutoGenerate(cRec As BO.x48SqlTask) As Boolean
    Function UpdateLastScheduledRun(intx48ID As Integer, dat As Date?) As Boolean
End Interface
Class x48SqlTaskBL
    Inherits BLMother
    Implements Ix48SqlTaskBL
    Private WithEvents _cDL As DL.x48SqlTaskDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x48SqlTaskDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x48SqlTask) As Boolean Implements Ix48SqlTaskBL.Save

        With cRec
            If Trim(.x48Name) = "" Then
                _Error = "Chybí název úlohy." : Return False
            End If
        End With

        If _cDL.Save(cRec) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.x48SqlTask Implements Ix48SqlTaskBL.Load
        Return _cDL.Load(intPID)
    End Function

    Public Function Delete(intPID As Integer) As Boolean Implements Ix48SqlTaskBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x48SqlTask) Implements Ix48SqlTaskBL.GetList
        Return _cDL.GetList(mq)
    End Function




    Public Function IsWaiting4AutoGenerate(cRec As BO.x48SqlTask) As Boolean Implements Ix48SqlTaskBL.IsWaiting4AutoGenerate
        Dim b As Boolean = False
        With cRec
            If .x48IsRunInDay1 And Weekday(Now, FirstDayOfWeek.Monday) = 1 Then b = True
            If .x48IsRunInDay2 And Weekday(Now, FirstDayOfWeek.Monday) = 2 Then b = True
            If .x48IsRunInDay3 And Weekday(Now, FirstDayOfWeek.Monday) = 3 Then b = True
            If .x48IsRunInDay4 And Weekday(Now, FirstDayOfWeek.Monday) = 4 Then b = True
            If .x48IsRunInDay5 And Weekday(Now, FirstDayOfWeek.Monday) = 5 Then b = True
            If .x48IsRunInDay6 And Weekday(Now, FirstDayOfWeek.Monday) = 6 Then b = True
            If .x48IsRunInDay7 And Weekday(Now, FirstDayOfWeek.Monday) = 7 Then b = True
            If Not b Then Return False
            Dim cT As New BO.clsTime, secsNow As Integer = TimeOfDay.Hour * 60 * 60 + TimeOfDay.Minute * 60 + TimeOfDay.Second

            If secsNow >= cT.ConvertTimeToSeconds(.x48RunInTime) Then
                If .x48LastScheduledRun Is Nothing Then Return True 'úloha ještě nikdy nebyla generována
                If Day(.x48LastScheduledRun) = Day(Now) And Month(.x48LastScheduledRun) = Month(Now) And Year(.x48LastScheduledRun) = Year(Now) Then
                    Return False    'dnes již byla generována
                End If
                Return True
            End If
        End With
        Return False
    End Function
    Public Function UpdateLastScheduledRun(intx48ID As Integer, dat As Date?) As Boolean Implements Ix48SqlTaskBL.UpdateLastScheduledRun
        Return _cDL.UpdateLastScheduledRun(intx48ID, dat)
    End Function
End Class
