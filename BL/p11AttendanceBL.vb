
Public Interface Ip11AttendanceBL
    Inherits IFMother
    Function Save(cRec As BO.p11Attendance) As Boolean
    Function Load(intPID As Integer) As BO.p11Attendance
    Function LoadByPersonAndDate(intJ02ID As Integer, p11Date As Date) As BO.p11Attendance
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery, Optional intJ02ID As Integer = 0) As IEnumerable(Of BO.p11Attendance)
    Function FindDefaultP41ID() As Integer
End Interface
Class p11AttendanceBL
    Inherits BLMother
    Implements Ip11AttendanceBL
    Private WithEvents _cDL As DL.p11AttendanceDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p11AttendanceDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p11Attendance) As Boolean Implements Ip11AttendanceBL.Save
        With cRec
            If Not .p11TodayEnd Is Nothing And Not .p11TodayStart Is Nothing Then
                If .p11TodayStart >= .p11TodayEnd Then
                    _Error = "Čas příchodu musí být menší než čas odchodu." : Return False
                End If
            End If
        End With
        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p11Attendance Implements Ip11AttendanceBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByPersonAndDate(intJ02ID As Integer, p11Date As Date) As BO.p11Attendance Implements Ip11AttendanceBL.LoadByPersonAndDate
        Return _cDL.LoadByPersonAndDate(intJ02ID, p11Date)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip11AttendanceBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery, Optional intJ02ID As Integer = 0) As IEnumerable(Of BO.p11Attendance) Implements Ip11AttendanceBL.GetList
        Return _cDL.GetList(mq, intJ02ID)
    End Function
    Public Function FindDefaultP41ID() As Integer Implements Ip11AttendanceBL.FindDefaultP41ID
        Return _cDL.FindDefaultP41ID()
    End Function
End Class
