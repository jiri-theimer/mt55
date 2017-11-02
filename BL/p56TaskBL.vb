Public Interface Ip56TaskBL
    Inherits IFMother
    Function Save(cRec As BO.p56Task, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField), strUploadGUID As String) As Boolean
    Function Load(intPID As Integer) As BO.p56Task
    Function LoadByCode(strCode As String) As BO.p56Task
    Function LoadMyLastCreated() As BO.p56Task
    Function LoadByExternalPID(strExternalPID As String) As BO.p56Task
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryP56, Optional bolInhaleReceiversInLine As Boolean = False) As IEnumerable(Of BO.p56Task)
    ''Function GetList_WithWorksheetSum(myQuery As BO.myQueryP56, Optional bolInhaleReceiversInLine As Boolean = False) As IEnumerable(Of BO.p56TaskWithWorksheetSum)
    Function GetGridDataSource(myQuery As BO.myQueryP56) As DataTable
    Function GetVirtualCount(myQuery As BO.myQueryP56) As Integer
    Function GetRolesInline(intPID As Integer) As String
    Function GetList_WaitingOnReminder(datReminderFrom As Date, datReminderUntil As Date) As IEnumerable(Of BO.p56Task)
    Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.p56Task)
    Sub Handle_Reminder()
    Sub UpdateSelectedTaskRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP56ID As Integer)
    Sub ClearSelectedTaskRole(intX67ID As Integer, intP56ID As Integer)
    Function InhaleRecordDisposition(cRec As BO.p56Task) As BO.p56RecordDisposition
    ''Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean
    Function GetTotalTasksCount() As Integer
    Function GetGridFooterSums(myQuery As BO.myQueryP56, strSumFields As String) As DataTable
    Function LoadSumRow(intPID As Integer) As BO.p56TaskSum
End Interface
Class p56TaskBL
    Inherits BLMother
    Implements Ip56TaskBL
    Private WithEvents _cDL As DL.p56TaskDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p56TaskDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Private Function TryFindProjectBySubmitter(intJ02ID_Submiter As Integer) As Integer
        Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Factory.p30Contact_PersonBL.GetList(0, 0, intJ02ID_Submiter)
        If lisP30.Count > 0 Then
            For Each c In lisP30.Where(Function(p) p.p41ID <> 0 Or p.p28ID <> 0).OrderByDescending(Function(p) p.p41ID)
                If c.p41ID <> 0 Then Return c.p41ID
                If c.p28ID <> 0 Then
                    Dim mq As New BO.myQueryP41
                    mq.p28ID = c.p28ID
                    mq.Closed = BO.BooleanQueryMode.FalseQuery
                    Dim cP41 As BO.p41Project = Factory.p41ProjectBL.GetList(mq).First
                    If Not cP41 Is Nothing Then
                        Return cP41.PID
                    End If
                End If
            Next
        End If
        Return 0
    End Function
    Public Function Save(cRec As BO.p56Task, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField), strUploadGUID As String) As Boolean Implements Ip56TaskBL.Save
        Dim lisTempUpload As IEnumerable(Of BO.p85TempBox) = Nothing
        If strUploadGUID <> "" Then
            lisTempUpload = Me.Factory.p85TempBoxBL.GetList(strUploadGUID, True)
        End If
        With cRec
            If Trim(.p56Name) = "" Then _Error = "Chybí název úkolu." : Return False
            If .p57ID = 0 Then _Error = "Chybí typ úkolu!" : Return False
            If .PID = 0 And .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
            If .j02ID_Owner = 0 Then _Error = "Chybí vlastník záznamu." : Return False
            If .p41ID = 0 Then  'zkusit najít chybějící projekt
                .p41ID = TryFindProjectBySubmitter(.j02ID_Owner)
            End If

            If .p41ID = 0 Then _Error = "Chybí vazba na projekt!" : Return False
            If Not BO.BAS.IsNullDBDate(.p56PlanUntil) Is Nothing Then
                If Format(.p56PlanUntil, "HH:mm") = "00:00" Then    'pokud je čas termínu 00:00, pak doplnit 23:59
                    .p56PlanUntil = CDate(.p56PlanUntil).AddDays(1).AddSeconds(-1)
                End If
            End If
            If Not (BO.BAS.IsNullDBDate(.p56PlanFrom) Is Nothing Or BO.BAS.IsNullDBDate(.p56PlanUntil) Is Nothing) Then
                If .p56PlanFrom > .p56PlanUntil Then
                    _Error = "Plánované zahájení úkolu nesmí být větší než plánované dokončení (termín úkolu)!" : Return False
                End If
            End If
            

            If .p56IsPlan_Expenses_Ceiling And .p56Plan_Expenses = 0 Then _Error = "Chybí zadání plánu peněžních výdajů." : Return False
            If .p56IsPlan_Hours_Ceiling And .p56Plan_Hours = 0 Then _Error = "Chybí zadání plánu hodin." : Return False

            If .p65ID > 0 Then
                If .p56RecurBaseDate Is Nothing Then
                    _Error = "Chybí úvodní rozhodné datum u šablony opakovaného projektu." : Return False
                End If
                .p56RecurBaseDate = DateSerial(Year(.p56RecurBaseDate), Month(.p56RecurBaseDate), 1)
                Dim cP65 As BO.p65Recurrence = Me.Factory.p65RecurrenceBL.Load(.p65ID), intM As Integer = Month(.p56RecurBaseDate)
                If cP65.p65RecurFlag = BO.RecurrenceType.Year Then
                    .p56RecurBaseDate = DateSerial(Year(.p56RecurBaseDate), 1, 1)
                End If
                If cP65.p65RecurFlag = BO.RecurrenceType.Quarter And intM <> 1 And intM <> 4 And intM <> 7 And intM <> 10 Then
                    _Error = "Chybně zadané rozhodné datum." : Return False
                End If
            End If
        End With
        Dim cP57 As BO.p57TaskType = Me.Factory.p57TaskTypeBL.Load(cRec.p57ID)
        If cP57 Is Nothing Then _Error = "Chybí typ úkolu" : Return False
        If cP57.p57IsHelpdesk And Len(Trim(cRec.p56Description)) < 3 Then
            _Error = "Musíte vyplnit podrobný popis požadavku." : Return False
        End If
        Select Case cP57.p57PlanDatesEntryFlag
            Case 1, 3, 4
                If BO.BAS.IsNullDBDate(cRec.p56PlanUntil) Is Nothing Then
                    _Error = "Chybí zadání termínu." : Return False
                End If
        End Select
        If (cP57.p57PlanDatesEntryFlag = 3 Or cP57.p57PlanDatesEntryFlag = 4) And BO.BAS.IsNullDBDate(cRec.p56PlanFrom) Is Nothing Then
            _Error = "Chybí zadání plánovaného zahájení." : Return False
        End If
        If Not lisFF Is Nothing Then
            If Not BL.BAS.ValidateFF(lisFF) Then
                _Error = BL.BAS.ErrorMessage : Return False
            End If
        End If
        If Not Me.RaiseAppEvent_TailoringTestBeforeSave(cRec, lisFF, "p56_beforesave") Then Return False

        If _cDL.Save(cRec, lisX69, lisFF) Then
            If strUploadGUID <> "" Then
                Me.Factory.o27AttachmentBL.UploadAndSaveUserControl(lisTempUpload, BO.x29IdEnum.p56Task, _LastSavedPID)
            End If
            Me.RaiseAppEvent_TailoringAfterSave(_LastSavedPID, "p56_aftersave")
            If cRec.PID = 0 Then
                If cP57.b01ID > 0 Then
                    InhaleDefaultWorkflowMove(_LastSavedPID, cP57.b01ID)    'je třeba nahodit výchozí workflow stav
                End If
                Me.RaiseAppEvent(BO.x45IDEnum.p56_new, _LastSavedPID, , , cRec.p56IsNoNotify)
            Else
                Me.RaiseAppEvent(BO.x45IDEnum.p56_update, _LastSavedPID, , , cRec.p56IsNoNotify)
            End If
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.p56Task Implements Ip56TaskBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadMyLastCreated() As BO.p56Task Implements Ip56TaskBL.LoadMyLastCreated
        Return _cDL.LoadMyLastCreated()
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p56Task Implements Ip56TaskBL.LoadByExternalPID
        Return _cDL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function LoadByCode(strCode As String) As BO.p56Task Implements Ip56TaskBL.LoadByCode
        Return _cDL.LoadByCode(strCode)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip56TaskBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.p56Task, intPID) 'úschova kvůli logování historie
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p56_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(mq As BO.myQueryP56, Optional bolInhaleReceiversInLine As Boolean = False) As IEnumerable(Of BO.p56Task) Implements Ip56TaskBL.GetList
        Return _cDL.GetList(mq, bolInhaleReceiversInLine)
    End Function
    ''Public Function GetList_WithWorksheetSum(myQuery As BO.myQueryP56, Optional bolInhaleReceiversInLine As Boolean = False) As IEnumerable(Of BO.p56TaskWithWorksheetSum) Implements Ip56TaskBL.GetList_WithWorksheetSum
    ''    Return _cDL.GetList_WithWorksheetSum(myQuery, bolInhaleReceiversInLine)
    ''End Function
    Public Function GetList_WaitingOnReminder(datReminderFrom As Date, datReminderUntil As Date) As IEnumerable(Of BO.p56Task) Implements Ip56TaskBL.GetList_WaitingOnReminder
        Return _cDL.GetList_WaitingOnReminder(datReminderFrom, datReminderUntil)
    End Function
    Public Function GetGridDataSource(myQuery As BO.myQueryP56) As DataTable Implements Ip56TaskBL.GetGridDataSource
        Return _cDL.GetGridDataSource(myQuery)
    End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryP56) As Integer Implements Ip56TaskBL.GetVirtualCount
        Return _cDL.GetVirtualCount(myQuery)
    End Function
    Public Function GetRolesInline(intPID As Integer) As String Implements Ip56TaskBL.GetRolesInline
        Return _cDL.GetRolesInline(intPID)
    End Function
    Public Sub Handle_Reminder() Implements Ip56TaskBL.Handle_Reminder
        Dim d1 As Date = DateAdd(DateInterval.Day, -2, Now)
        Dim d2 As Date = Now
        Dim lis As IEnumerable(Of BO.p56Task) = _cDL.GetList_WaitingOnReminder(d1, d2)
        For Each cRec In lis
            Me.RaiseAppEvent(BO.x45IDEnum.p56_remind, cRec.PID, cRec.NameWithTypeAndCode)

        Next

    End Sub
    Public Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.p56Task) Implements Ip56TaskBL.GetList_forMessagesDashboard
        Return _cDL.GetList_forMessagesDashboard(intJ02ID)
    End Function

    Private Sub InhaleDefaultWorkflowMove(intP56ID As Integer, intB01ID As Integer)
        Dim cB06 As BO.b06WorkflowStep = Me.Factory.b06WorkflowStepBL.LoadKickOffStep(intB01ID)
        If cB06 Is Nothing Then Return
        Me.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, intP56ID, BO.x29IdEnum.p56Task, "", "", False, Nothing)
    End Sub

    Sub UpdateSelectedTaskRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP56ID As Integer) Implements Ip56TaskBL.UpdateSelectedTaskRole
        _cDL.UpdateSelectedTaskRole(intX67ID, lisX69, intP56ID)
    End Sub
    Sub ClearSelectedTaskRole(intX67ID As Integer, intP56ID As Integer) Implements Ip56TaskBL.ClearSelectedTaskRole
        _cDL.ClearSelectedTaskRole(intX67ID, intP56ID)
    End Sub

    Public Function InhaleRecordDisposition(cRec As BO.p56Task) As BO.p56RecordDisposition Implements Ip56TaskBL.InhaleRecordDisposition
        Dim c As New BO.p56RecordDisposition
        If Factory.SysUser.IsAdmin Then
            c.ReadAccess = True : c.CanMove2Bin = True : c.OwnerAccess = True : c.P31_Create = True
            Return c
        End If

        If cRec.j02ID_Owner = Factory.SysUser.j02ID And cRec.p57IsHelpdesk = False Then
            'vlasník záznamu má plná práva, pokud to není helpdesk zadavatel
            Dim bolTested As Boolean = False
            If cRec.b02ID <> 0 Then
                Dim cB02 As BO.b02WorkflowStatus = Factory.b02WorkflowStatusBL.Load(cRec.b02ID)
                If cB02.b02IsRecordReadOnly4Owner Then  'aktuální workflow status má nastaveno, že vlastník záznam má pouze readonly přístup
                    c.ReadAccess = True
                    bolTested = True
                End If
            End If
            If Not bolTested Then
                c.ReadAccess = True : c.OwnerAccess = True : c.CanMove2Bin = True : c.P31_Create = True 'vlastník má plná práva
                Return c
            End If
            
        End If

        ''If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.p41ID, BO.x53PermValEnum.PR_P56_Owner, True) Then
        ''    'v projektové roli má oprávnění být vlastníkem všech úkolů
        ''    c.ReadAccess = True : c.OwnerAccess = True : c.P31_Create = True
        ''End If
        
        If Not c.ReadAccess Then
            If Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p56Task, cRec.PID).Count > 0 Then
                c.ReadAccess = True 'má nějakou roli v úkolu
            End If
        End If
        If c.ReadAccess Then

            If Not c.P31_Create Then
                If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p56Task, cRec.PID, BO.x53PermValEnum.TR_P31_Creator, False) Then
                    'v úkolové roli má oprávnění zapisovat worksheet
                    c.P31_Create = True
                End If
            End If


            If Not c.OwnerAccess Then
                If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p56Task, cRec.PID, BO.x53PermValEnum.TR_P56_Owner, False) Then
                    'v úkolové roli má oprávnění vlastníka
                    c.OwnerAccess = True
                End If
            End If


        End If




        Return c
    End Function

    ''Public Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean Implements Ip56TaskBL.UpdateImapSource
    ''    Return _cDL.UpdateImapSource(intPID, intO43ID)
    ''End Function
    Public Function GetTotalTasksCount() As Integer Implements Ip56TaskBL.GetTotalTasksCount
        Return _cDL.GetTotalTasksCount()
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP56, strSumFields As String) As DataTable Implements Ip56TaskBL.GetGridFooterSums
        Return _cDL.GetGridFooterSums(myQuery, strSumFields)
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.p56TaskSum Implements Ip56TaskBL.LoadSumRow
        Return _cDL.LoadSumRow(intPID)
    End Function
End Class
