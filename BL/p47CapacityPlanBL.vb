
Public Interface Ip47CapacityPlanBL
    Inherits IFMother
    Function SaveProjectPlan(intP45ID As Integer, lisP47 As List(Of BO.p47CapacityPlan), lisP44 As List(Of BO.p44CapacityPlan_Exception)) As Boolean
    
    Function GetList(mq As BO.myQueryP47) As IEnumerable(Of BO.p47CapacityPlan)

End Interface
Class p47CapacityPlanBL
    Inherits BLMother
    Implements Ip47CapacityPlanBL
    Private WithEvents _cDL As DL.p47CapacityPlanDL

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p47CapacityPlanDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Function SaveProjectPlan(intP45ID As Integer, lisP47 As List(Of BO.p47CapacityPlan), lisP44 As List(Of BO.p44CapacityPlan_Exception)) As Boolean Implements Ip47CapacityPlanBL.SaveProjectPlan
        Dim lisP46 As IEnumerable(Of BO.p46BudgetPerson) = Factory.p45BudgetBL.GetList_p46(intP45ID)
        For Each c In lisP46
            Dim lis As IEnumerable(Of BO.p47CapacityPlan) = lisP47.Where(Function(p) p.p46ID = c.PID)
            If lis.Sum(Function(p) p.p47HoursTotal) > c.p46HoursTotal Then
                _Error = String.Format("[{0}]: Kapacitní plán by překročil celkový limit hodin v rozpočtu ({1})", c.Person, c.p46HoursTotal) : Return False
            End If
            If lis.Sum(Function(p) p.p47HoursBillable) > c.p46HoursBillable Then
                _Error = String.Format("[{0}]: Kapacitní plán fakturovatelných hodin by překročil celkový limit fakturovatelných hodin v rozpočtu ({1})", c.Person, c.p46HoursBillable) : Return False
            End If
            If lis.Sum(Function(p) p.p47HoursNonBillable) > c.p46HoursNonBillable Then
                _Error = String.Format("[{0}]: Kapacitní plán ne-fakturovatelných hodin by překročil celkový limit ne-fakturovatelných hodin v rozpočtu ({1})", c.Person, c.p46HoursNonBillable) : Return False
            End If
        Next
        Return _cDL.SaveProjectPlan(intP45ID, lisP47, lisP44)
    End Function
    Public Function GetList(mq As BO.myQueryP47) As IEnumerable(Of BO.p47CapacityPlan) Implements Ip47CapacityPlanBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
