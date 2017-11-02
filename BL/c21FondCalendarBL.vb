Public Interface Ic21FondCalendarBL
    Inherits IFMother
    Function Save(cRec As BO.c21FondCalendar) As Boolean
    Function Load(intPID As Integer) As BO.c21FondCalendar
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.c21FondCalendar)
    Function GetSumHours(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As Double
    Function GetSumHoursPerMonth(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.FondHours)
    Function GetList_c22(c21ids As List(Of Integer), datFrom As Date, datUntil As Date, bolExcludeScopeFlag3 As Boolean) As IEnumerable(Of BO.c22FondCalendar_Date)
End Interface

Class c21FondCalendarBL
    Inherits BLMother
    Implements Ic21FondCalendarBL
    Private WithEvents _cDL As DL.c21FondCalendarDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.c21FondCalendarDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.c21FondCalendar) As Boolean Implements Ic21FondCalendarBL.Save
        With cRec
            If Trim(.c21Name) = "" Then _Error = "Chybí název kalendáře." : Return False
            If .c21ScopeFlag <> BO.c21ScopeFlagENUM.Basic Then
                .c21Day1_Hours = 0
                .c21Day2_Hours = 0
                .c21Day3_Hours = 0
                .c21Day4_Hours = 0
                .c21Day5_Hours = 0
                .c21Day6_Hours = 0
                .c21Day7_Hours = 0
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.c21FondCalendar Implements Ic21FondCalendarBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ic21FondCalendarBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.c21FondCalendar) Implements Ic21FondCalendarBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetSumHours(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As Double Implements Ic21FondCalendarBL.GetSumHours
        Return _cDL.GetSumHours(intC21ID, intJ17ID, d1, d2)
    End Function
    Public Function GetSumHoursPerMonth(intC21ID As Integer, intJ17ID As Integer, d1 As Date, d2 As Date) As IEnumerable(Of BO.FondHours) Implements Ic21FondCalendarBL.GetSumHoursPerMonth
        Return _cDL.GetSumHoursPerMonth(intC21ID, intJ17ID, d1, d2)
    End Function
    Public Function GetList_c22(c21ids As List(Of Integer), datFrom As Date, datUntil As Date, bolExcludeScopeFlag3 As Boolean) As IEnumerable(Of BO.c22FondCalendar_Date) Implements Ic21FondCalendarBL.GetList_c22
        Return _cDL.GetList_c22(c21ids, datFrom, datUntil, bolExcludeScopeFlag3)
    End Function
End Class
