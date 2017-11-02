Public Interface Ip65RecurrenceBL
    Inherits IFMother
    Function Save(cRec As BO.p65Recurrence) As Boolean
    Function Load(intPID As Integer) As BO.p65Recurrence
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p65Recurrence)
    Function CalculateDates(cP65 As BO.p65Recurrence, datNow As Date) As BO.RecurrenceCalculation
    Function CalculateNextBaseDate(cP65 As BO.p65Recurrence, datBase As Date) As Date
End Interface
Class p65RecurrenceBL
    Inherits BLMother
    Implements Ip65RecurrenceBL
    Private WithEvents _cDL As DL.p65RecurrenceDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p65RecurrenceDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p65Recurrence) As Boolean Implements Ip65RecurrenceBL.Save
        With cRec
            If Trim(.p65Name) = "" Then _Error = "Chybí název pravidla." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p65Recurrence Implements Ip65RecurrenceBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip65RecurrenceBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p65Recurrence) Implements Ip65RecurrenceBL.GetList
        Return _cDL.GetList(mq)
    End Function

    Public Function CalculateDates(cP65 As BO.p65Recurrence, datNow As Date) As BO.RecurrenceCalculation Implements Ip65RecurrenceBL.CalculateDates
        Dim c As New BO.RecurrenceCalculation

        c.DatBase = DateSerial(Year(datNow), Month(datNow), 1)
        If cP65.p65RecurFlag = BO.RecurrenceType.Year Then
            c.DatBase = DateSerial(Year(datNow), 1, 1)
        End If
        If cP65.p65RecurFlag = BO.RecurrenceType.Quarter Then
            Select Case Month(datNow)
                Case 1, 2, 3 : c.DatBase = DateSerial(Year(datNow), 1, 1)
                Case 4, 5, 6 : c.DatBase = DateSerial(Year(datNow), 4, 1)
                Case 7, 8, 9 : c.DatBase = DateSerial(Year(datNow), 7, 1)
                Case 10, 11, 12 : c.DatBase = DateSerial(Year(datNow), 10, 1)
                Case Else : c.DatBase = DateSerial(Year(datNow), Month(datNow), 1)
            End Select

        End If
        c.DatGen = c.DatBase.AddMonths(cP65.p65RecurGenToBase_M).AddDays(cP65.p65RecurGenToBase_D)

        If cP65.p65IsPlanUntil Or cP65.p65IsPlanFrom Then
            c.DatPlanUntil = c.DatBase.AddMonths(cP65.p65RecurPlanUntilToBase_M).AddDays(cP65.p65RecurPlanUntilToBase_D)
            c.DatPlanFrom = c.DatBase.AddMonths(cP65.p65RecurPlanFromToBase_M).AddDays(cP65.p65RecurPlanFromToBase_D)
        End If

        c.DatBaseNext = CalculateNextBaseDate(cP65, c.DatBase)

        Return c
    End Function
    Public Function CalculateNextBaseDate(cP65 As BO.p65Recurrence, datBase As Date) As Date Implements Ip65RecurrenceBL.CalculateNextBaseDate
        If cP65.p65RecurFlag = BO.RecurrenceType.Month Then Return CDate(datBase).AddMonths(1)
        If cP65.p65RecurFlag = BO.RecurrenceType.Quarter Then Return CDate(datBase).AddMonths(3)
        If cP65.p65RecurFlag = BO.RecurrenceType.Year Then Return CDate(datBase).AddYears(1)
        Return datBase
    End Function
End Class
