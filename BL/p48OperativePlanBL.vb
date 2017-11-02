Public Interface Ip48OperativePlanBL
    Inherits IFMother
    Function Save(cRec As BO.p48OperativePlan) As Boolean
    Function SaveBatch(lis As List(Of BO.p48OperativePlan)) As Boolean
    Function Load(intPID As Integer) As BO.p48OperativePlan
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryP48) As IEnumerable(Of BO.p48OperativePlan)
    Function GetList_SumPerPerson(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject)
    Function GetList_SumPerProject(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject)
    Function TestBeforeSave(lis As List(Of BO.p48OperativePlan)) As Boolean

End Interface
Class p48OperativePlanBL
    Inherits BLMother
    Implements Ip48OperativePlanBL
    Private WithEvents _cDL As DL.p48OperativePlanDL
    Private _lisJ02_Slaves_Create As IEnumerable(Of BO.j02Person)
    Private _lisJ02_Slaves_Edit As IEnumerable(Of BO.j02Person)

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p48OperativePlanDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p48OperativePlan) As Boolean Implements Ip48OperativePlanBL.Save
        Dim lis As New List(Of BO.p48OperativePlan)
        lis.Add(cRec)

        If Not TestBeforeSave(lis) Then Return False



        Handle_ExactTime(cRec)
        Return _cDL.Save(cRec)
    End Function

    Private Sub Handle_ExactTime(ByRef cRec As BO.p48OperativePlan)
        cRec.p48DateTimeFrom = Nothing : cRec.p48DateTimeUntil = Nothing
        Dim cT As New BO.clsTime
        Dim intTimeFrom As Integer = cT.ConvertTimeToSeconds(cRec.p48TimeFrom), intTimeUntil As Integer = cT.ConvertTimeToSeconds(cRec.p48TimeUntil)
        Dim intHours As Integer = cRec.p48Hours * 60 * 60
        cRec.p48TimeFrom = "" : cRec.p48TimeUntil = ""
        If intTimeFrom < 0 Or intTimeFrom > 24 * 60 * 60 Then intTimeFrom = 0
        If intTimeUntil < 0 Or intTimeUntil > 24 * 60 * 60 Then intTimeUntil = 0
        If intTimeFrom > 0 Or intTimeUntil > 0 Then
            If intTimeFrom > 0 And intTimeUntil = 0 Then intTimeUntil = intTimeFrom + intHours
            If intTimeUntil > 0 And intTimeFrom = 0 Then intTimeFrom = intTimeUntil - intHours
            If intTimeFrom > intTimeUntil Then intTimeUntil = intTimeFrom
        End If
        If intTimeUntil = intTimeFrom Or intTimeFrom = 0 Or intTimeUntil = 0 Then Return
        If intHours <> (intTimeUntil - intTimeFrom) Then
            intTimeUntil = intTimeFrom + intHours
        End If
        cRec.p48TimeFrom = cT.ShowAsHHMM(cT.GetDecTimeFromSeconds(intTimeFrom))
        cRec.p48TimeUntil = cT.ShowAsHHMM(cT.GetDecTimeFromSeconds(intTimeUntil))
        cRec.p48DateTimeFrom = cRec.p48Date.AddSeconds(intTimeFrom)
        cRec.p48DateTimeUntil = cRec.p48Date.AddSeconds(intTimeUntil)

    End Sub
    Public Function SaveBatch(lis As List(Of BO.p48OperativePlan)) As Boolean Implements Ip48OperativePlanBL.SaveBatch
        If Not TestBeforeSave(lis) Then Return False
        For Each c In lis
            If c.IsSetAsDeleted Then
                If c.PID > 0 Then _cDL.Delete(c.PID)
            Else
                Handle_ExactTime(c)
                _cDL.Save(c)
            End If
        Next
        Return True
    End Function
    Public Function TestBeforeSave(lis As List(Of BO.p48OperativePlan)) As Boolean Implements Ip48OperativePlanBL.TestBeforeSave
        If Not TestGlobalPerm() Then Return False
        Dim x As Integer = 1
        For Each c In lis
            With c
                If .p48Hours <= 0 Then _Error = "Řádek #" & x.ToString & ", v záznamu plánu nesmí být nulové nebo záporné hodiny." : Return False
                If .p34ID = 0 Then _Error = "Řádek #" & x.ToString & ", chybí vazba na sešit aktivit" : Return False
                If .p41ID = 0 Then _Error = "Řádek #" & x.ToString & ", chybí vazba na projekt" : Return False
                If .j02ID = 0 Then _Error = "Řádek #" & x.ToString & ", chybí vazba na osobu" : Return False
                If Not IsTestPerm(c) Then
                    If c.IsSetAsDeleted Then
                        _Error = "Nemáte oprávnění odstranit záznam [ID: " & c.PID.ToString & "]<hr> " & _Error
                    Else
                        _Error = "Záznam #" & x.ToString & "[ID: " & c.PID.ToString & "]<hr> " & _Error
                    End If

                    Return False
                End If
            End With
            x += 1
        Next
        'test oproti kapacitnímu plánu
        For Each intP41ID As Integer In lis.Select(Function(p) p.p41ID).Distinct
            Dim cP45 As BO.p45Budget = Factory.p45BudgetBL.LoadByProject(intP41ID)
            If Not cP45 Is Nothing Then
                Dim p48ids_exclude As List(Of Integer) = lis.Where(Function(p) p.p41ID = intP41ID And p.PID <> 0).Select(Function(p) p.PID).ToList
                Dim mq As New BO.myQueryP48
                mq.p41ID = intP41ID
                Dim lisSavedP48 As List(Of BO.p48OperativePlan) = Factory.p48OperativePlanBL.GetList(mq).ToList
                For Each intP48ID As Integer In p48ids_exclude
                    lisSavedP48.Remove(lisSavedP48.First(Function(p) p.PID = intP48ID))
                Next
                'existuje v projektu rozpočet
                Dim lisP46 As IEnumerable(Of BO.p46BudgetPerson) = Factory.p45BudgetBL.GetList_p46(cP45.PID)
                For Each cP46 In lisP46
                    If lis.Where(Function(p) p.p41ID = intP41ID And p.j02ID = cP46.j02ID).Count > 0 Then
                        Dim dblTestHours1 As Double = lis.Where(Function(p) p.p41ID = intP41ID And p.j02ID = cP46.j02ID).Sum(Function(p) p.p48Hours)
                        Dim dblTestHours2 As Double = lisSavedP48.Where(Function(p) p.p41ID = intP41ID And p.j02ID = cP46.j02ID).Sum(Function(p) p.p48Hours)
                        Dim dblTestHours As Double = dblTestHours1 + dblTestHours2
                        Select Case cP46.p46ExceedFlag

                            Case BO.p46ExceedFlagENUM.StrictTotal
                                If dblTestHours > cP46.p46HoursTotal Then
                                    Dim strProject As String = Factory.p41ProjectBL.Load(intP41ID).FullName
                                    _Error = String.Format("Hodiny operativního plánu ({2}h.) by překročili hodiny rozpočtu ({3}h.) u projektu [{0}] a osoby [{1}].", strProject, cP46.Person, dblTestHours, cP46.p46HoursTotal) : Return False
                                End If
                        End Select
                    End If

                Next
                For Each c In lis.Where(Function(p) p.p41ID = intP41ID)

                Next
            End If
        Next
        Return True
    End Function
    Private Function TestGlobalPerm() As Boolean
        If Not Factory.TestPermission(BO.x53PermValEnum.GR_P48_Creator) Then
            _Error = "Nedisponujete oprávněním zapisovat operativní plán."
            Return False
        End If
        Return True
    End Function
    Private Function IsTestPerm(cRec As BO.p48OperativePlan) As Boolean
        'ověření práv zapisovat za jiné osoby
        If cRec.j02ID = Factory.SysUser.j02ID Then Return True
        If Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
            Return True 'administrátor může zapisovat za kohokoliv
        End If
        If Not Factory.SysUser.IsMasterPerson Then
            _Error = "Nedisponujete oprávněním zapisovat za jinou osobu."
            Return False
        Else

            If cRec.PID = 0 Then
                If _lisJ02_Slaves_Create Is Nothing Then _lisJ02_Slaves_Create = Factory.j02PersonBL.GetList_Slaves(Factory.SysUser.j02ID, False, BO.j05Disposition_p31ENUM._NotSpecified, True, BO.j05Disposition_p48ENUM._NotSpecified)
                If _lisJ02_Slaves_Create.Where(Function(p) p.PID = cRec.j02ID).Count = 0 Then
                    _Error = "Nedisponujete oprávněním zapisovat operativní plán za tuto osobu." : Return False
                End If
            Else
                If _lisJ02_Slaves_Edit Is Nothing Then _lisJ02_Slaves_Edit = Factory.j02PersonBL.GetList_Slaves(Factory.SysUser.j02ID, False, BO.j05Disposition_p31ENUM._NotSpecified, False, BO.j05Disposition_p48ENUM.CistAEdit)
                If _lisJ02_Slaves_Edit.Where(Function(p) p.PID = cRec.j02ID).Count = 0 Then
                    _Error = "Nedisponujete oprávněním upravovat operativní plán této osoby." : Return False
                End If
            End If
        End If
        Return True
    End Function

    Public Function Load(intPID As Integer) As BO.p48OperativePlan Implements Ip48OperativePlanBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip48OperativePlanBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryP48) As IEnumerable(Of BO.p48OperativePlan) Implements Ip48OperativePlanBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_SumPerPerson(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject) Implements Ip48OperativePlanBL.GetList_SumPerPerson
        Return _cDL.GetList_SumPerPerson(mq)
    End Function
    Public Function GetList_SumPerProject(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject) Implements Ip48OperativePlanBL.GetList_SumPerProject
        Return _cDL.GetList_SumPerProject(mq)
    End Function
End Class
