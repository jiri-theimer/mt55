
Public Interface Ip11AttendanceBL
    Inherits IFMother
    Function Save(cRec As BO.p11Attendance) As Boolean
    Function Load(intPID As Integer) As BO.p11Attendance
    Function LoadByPersonAndDate(intJ02ID As Integer, p11Date As Date) As BO.p11Attendance
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery, Optional intJ02ID As Integer = 0) As IEnumerable(Of BO.p11Attendance)
    Function FindDefaultP41ID() As Integer

    Function LoadP12(intP12ID As Integer) As BO.p12Pass
    Function GetListP12(intP11ID As Integer) As IEnumerable(Of BO.p12Pass)
    Function DeleteP12(intP12ID As Integer) As Boolean
    Function SaveP12(cRec As BO.p12Pass) As Boolean
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

    Public Function LoadP12(intP12ID As Integer) As BO.p12Pass Implements Ip11AttendanceBL.LoadP12
        Return _cDL.LoadP12(intP12ID)
    End Function
    Public Function DeleteP12(intP12ID As Integer) As Boolean Implements Ip11AttendanceBL.DeleteP12
        Return _cDL.DeleteP12(intP12ID)
    End Function
    Public Function GetListP12(intP11ID As Integer) As IEnumerable(Of BO.p12Pass) Implements Ip11AttendanceBL.GetListP12
        Return _cDL.GetListP12(intP11ID)
    End Function
    Public Function SaveP12(cRec As BO.p12Pass) As Boolean Implements Ip11AttendanceBL.SaveP12
        Dim cRecPrev As BO.p12Pass = Nothing
        With cRec
            If .p12TimeStamp > Now Then
                _Error = "Docházku nelze zapisovat do budoucna!" : Return False
            End If
            If .p11ID = 0 Then _Error = "p11ID missing." : Return False
            If .p12Flag = BO.p12FlagENUM.Aktivita Then
                If .p32ID = 0 Then _Error = "Aktivita chybí." : Return False
            End If
            Dim lis As IEnumerable(Of BO.p12Pass) = GetListP12(cRec.p11ID).OrderByDescending(Function(p) p.p12TimeStamp), intDUR As Integer = 0
            If lis.Count > 0 Then
                cRecPrev = lis(0)
                If cRec.p12TimeStamp < cRecPrev.p12TimeStamp Then
                    _Error = cRec.p12TimeStamp & "#" & String.Format(cRec.p11ID & "Čas musí být větší než {0}", Format(cRecPrev.p12TimeStamp), "dd.MM.yyyy HH:mm") : Return False
                End If
                Dim cT As New BO.clsTime
                intDUR = (cRec.p12TimeStamp.Hour * 60 + cRec.p12TimeStamp.Minute) - (cRecPrev.p12TimeStamp.Hour * 60 + cRecPrev.p12TimeStamp.Minute)

            End If
            cRec.p12Duration = intDUR

        End With
        If _cDL.SaveP12(cRec) Then
            If Not cRecPrev Is Nothing Then
                If cRecPrev.p32ID > 0 Then
                    cRecPrev.p12ActivityDuration = (cRec.p12TimeStamp.Hour * 60 + cRec.p12TimeStamp.Minute) - (cRecPrev.p12TimeStamp.Hour * 60 + cRecPrev.p12TimeStamp.Minute)
                    _cDL.SaveP12(cRecPrev)
                End If
            End If
            If cRec.p12Flag = BO.p12FlagENUM.Odchod Then
                Dim cP11 As BO.p11Attendance = Load(cRec.p11ID)
                cP11.p11TodayEnd = cRec.p12TimeStamp
                Save(cP11)
            End If

            Return True
        Else
            Return False
        End If


    End Function
End Class
