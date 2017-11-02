Public Interface Ip41ProjectBL
    Inherits IFMother
    Function Save(cRec As BO.p41Project, lisO39 As List(Of BO.o39Project_Address), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
    Function Load(intPID As Integer) As BO.p41Project
    Function LoadMyLastCreated() As BO.p41Project
    Function LoadByImapRobotAddress(strRobotAddress) As BO.p41Project
    Function LoadByExternalPID(strExternalPID As String) As BO.p41Project
    Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p41Project
    Function LoadSumRow(intPID As Integer) As BO.p41ProjectSum
    Function Delete(intPID As Integer) As Boolean
    Function GetGridDataSource(myQuery As BO.myQueryP41) As DataTable
    Function GetList(mq As BO.myQueryP41) As IEnumerable(Of BO.p41Project)
    Function GetList_o39(intPID As Integer) As IEnumerable(Of BO.o39Project_Address)
    Function GetVirtualCount(myQuery As BO.myQueryP41) As Integer
    Function InhaleRecordDisposition(cRec As BO.p41Project) As BO.p41RecordDisposition
    Sub UpdateSelectedProjectRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP41ID As Integer)
    Sub ClearSelectedProjectRole(intX67ID As Integer, intP41ID As Integer)
    Function ConvertFromDraft(intPID As Integer) As Boolean
    Function HasChildRecords(intPID As Integer) As Boolean
    Function GetTopProjectsByWorksheetEntry(intJ02ID As Integer, intGetTopRecs As Integer) As List(Of Integer)
    Function IsMyFavouriteProject(intPID As Integer) As Boolean
    Function GetGridFooterSums(myQuery As BO.myQueryP41, strSumFields As String) As DataTable
    Function BatchUpdate_TreeChilds(intPID As Integer, bolProjectRoles As Boolean, bolP28ID As Boolean, bolP87ID As Boolean, bolP51ID As Boolean, bolP92ID As Boolean, bolJ18ID As Boolean, bolP61ID As Boolean, bolValidity As Boolean) As Boolean
    Function GetRolesInline(intPID As Integer) As String
End Interface
Class p41ProjectBL
    Inherits BLMother
    Implements Ip41ProjectBL
    Private WithEvents _cDL As DL.p41ProjectDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p41ProjectDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function Load(intPID As Integer) As BO.p41Project Implements Ip41ProjectBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadMyLastCreated() As BO.p41Project Implements Ip41ProjectBL.LoadMyLastCreated
        Return _cDL.LoadMyLastCreated()
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress) As BO.p41Project Implements Ip41ProjectBL.LoadByImapRobotAddress
        Return _cDL.LoadByImapRobotAddress(strRobotAddress)
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p41Project Implements Ip41ProjectBL.LoadByExternalPID
        Return _cDL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.p41ProjectSum Implements Ip41ProjectBL.LoadSumRow
        Return _cDL.LoadSumRow(intPID)
    End Function

    Private Function ValidateBeforeSave(ByRef cRec As BO.p41Project, cP42 As BO.p42ProjectType, lisO39 As List(Of BO.o39Project_Address), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
        If cRec.PID = 0 Then
            'ověřit, jakým způsobem může zakládat nové projekty
            With Factory
                If .TestPermission(BO.x53PermValEnum.GR_P41_Creator) Then
                    'může založit oficiální projekt i draft projekt
                Else
                    If .TestPermission(BO.x53PermValEnum.GR_P41_Draft_Creator) Then
                        cRec.p41IsDraft = True  'násilně založit projekt jako draft, protože na oficiální záznam uživatel právo nemá
                    Else
                        _Error = "Vaše oprávnění nedovoluje zakládat projekty (ani v DRAFT režimu)." : Return False
                    End If
                End If
            End With
        End If
        With cRec
            If Trim(.p41Name) = "" Then
                _Error = "Chybí název projektu!" : Return False
            End If
            If Not BO.BAS.IsNullDBDate(.p41PlanFrom) Is Nothing Then
                If BO.BAS.IsNullDBDate(.p41PlanUntil) Is Nothing Then
                    _Error = "Pokud je plán zahájení vyplněný, musíte vyplnit i datum plánu dokončení." : Return False
                End If
            End If
            If Not BO.BAS.IsNullDBDate(.p41PlanUntil) Is Nothing Then
                If BO.BAS.IsNullDBDate(.p41PlanFrom) Is Nothing Then
                    _Error = "Pokud je plán dokončení vyplněný, musíte vyplnit i datum plánu zahájení." : Return False
                End If
            End If
            If Not (BO.BAS.IsNullDBDate(.p41PlanFrom) Is Nothing Or BO.BAS.IsNullDBDate(.p41PlanUntil) Is Nothing) Then
                If .p41PlanFrom.Value > .p41PlanUntil.Value Then
                    _Error = "Datum plánu zahájení projektu je větší než plán (termín) dokončení projektu!" : Return False
                End If
                If DateDiff(DateInterval.Month, .p41PlanFrom.Value, .p41PlanUntil.Value, FirstDayOfWeek.System) > 100 Then
                    _Error = "Doba časového plánu projektu musí být menší než 100 měsíců." : Return False
                End If
            End If
            If .p41ParentID <> 0 Then
                If .p41ParentID = .PID Then _Error = "Nadřízený záznam se musí lišit od podřízeného." : Return False
            End If
            If .ValidUntil <= Now Then
                'pokus o přesun projektu do archivu
                Select Case cP42.p42ArchiveFlag
                    Case BO.p42ArchiveFlagENUM.NoArchive_Waiting_Invoice
                        If _cDL.ExistWaitingWorksheetForInvoicing(.PID) Then
                            _Error = "Projekt nelze přesunout do achivu, dokud v něm existují nevyfakturované worksheet úkony. Tuto ochranu může změnit administrátor v nastavení typu projektu." : Return False
                        End If
                    Case BO.p42ArchiveFlagENUM.NoArchive_Waiting_Approve
                        If _cDL.ExistWaitingWorksheetForApproving(.PID) Then
                            _Error = "Projekt nelze přesunout do achivu, dokud v něm existují rozpracované worksheet úkony. Rozpracované úkony lze přesunout do archivu nebo tuto ochranu může změnit administrátor v nastavení typu projektu." : Return False
                        End If
                End Select

            End If
            If Len(.p41BillingMemo) > 1000 Then
                _Error = "Obsah fakturační poznámky je příliš dlouhý (nad 1.000 znaků). Pro tak dlouhé fakturační poznámky nebo souborové přílohy v projektu využívejte modul DOKUMENTY." : Return False
            End If
            If .p41ExternalPID <> "" Then
                'externí kód musí být jedinečný
                Dim mq As New BO.myQueryP41
                mq.p41ExternalPID = .p41ExternalPID
                If GetList(mq).Where(Function(p) p.PID <> .PID).Count > 0 Then
                    _Error = "Externí kód projektu musí být jedinečný!" : Return False
                End If
            End If
            If .p65ID > 0 Then
                If .p41RecurBaseDate Is Nothing Then
                    _Error = "Chybí úvodní rozhodné datum u šablony opakovaného projektu." : Return False
                End If
                .p41RecurBaseDate = DateSerial(Year(.p41RecurBaseDate), Month(.p41RecurBaseDate), 1)
                Dim cP65 As BO.p65Recurrence = Me.Factory.p65RecurrenceBL.Load(.p65ID), intM As Integer = Month(.p41RecurBaseDate)
                If cP65.p65RecurFlag = BO.RecurrenceType.Year Then
                    .p41RecurBaseDate = DateSerial(Year(.p41RecurBaseDate), 1, 1)
                End If
                If cP65.p65RecurFlag = BO.RecurrenceType.Quarter And intM <> 1 And intM <> 4 And intM <> 7 And intM <> 9 Then
                    _Error = "Chybně zadané rozhodné datum." : Return False
                End If
            End If
        End With
        If Not lisFF Is Nothing Then
            If Not BL.BAS.ValidateFF(lisFF) Then
                _Error = BL.BAS.ErrorMessage : Return False
            End If
        End If
        If Not lisX69 Is Nothing Then
            If Not TestX69(lisX69) Then Return False
        End If

        With cRec
            If .p41IsDraft = False And .p28ID_Client <> 0 Then
                'ověřit, zda klient není DRAFT
                If Factory.p28ContactBL.Load(.p28ID_Client).p28IsDraft Then _Error = "DRAFT klient může být svázán pouze s DRAFT projektem." : Return False
            End If
        End With

        Return True
    End Function
    Public Function Save(cRec As BO.p41Project, lisO39 As List(Of BO.o39Project_Address), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean Implements Ip41ProjectBL.Save
        With cRec
            If .PID = 0 And .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
        End With
        If cRec.p42ID = 0 Then
            _Error = "Chybí typ projektu." : Return False
        End If
        Dim cP42 As BO.p42ProjectType = Me.Factory.p42ProjectTypeBL.Load(cRec.p42ID)

        If Not ValidateBeforeSave(cRec, cP42, lisO39, lisP30, lisX69, lisFF) Then
            Return False
        End If
        If Not Me.RaiseAppEvent_TailoringTestBeforeSave(cRec, lisFF, "p41_beforesave") Then Return False


        If _cDL.Save(cRec, lisO39, lisP30, lisX69, lisFF, _LastSavedPID) Then
            Me.RaiseAppEvent_TailoringAfterSave(_LastSavedPID, "p41_aftersave")
            If cRec.PID = 0 Then
                If cP42.b01ID > 0 Then
                    InhaleDefaultWorkflowMove(_LastSavedPID, cP42.b01ID)    'je třeba nahodit výchozí workflow stav
                End If
                If cP42.f02ID <> 0 Then
                    Factory.f01FolderBL.CreateUpdateFolder(_LastSavedPID, cP42.f02ID)
                End If

                Me.RaiseAppEvent(BO.x45IDEnum.p41_new, _LastSavedPID, , , cRec.p41IsNoNotify)
            Else
                If cRec.b01ID > 0 And cRec.b02ID = 0 Then
                    InhaleDefaultWorkflowMove(cRec.PID, cRec.b01ID) 'chybí hodnota workflow stavu
                End If
                If cP42.f02ID <> 0 Then
                    Factory.f01FolderBL.CreateUpdateFolder(_LastSavedPID, cP42.f02ID)
                End If
                Me.RaiseAppEvent(BO.x45IDEnum.p41_update, _LastSavedPID, , , cRec.p41IsNoNotify)
            End If

            Return True
        Else
            Return False
        End If


    End Function
   

    Private Sub InhaleDefaultWorkflowMove(intP41ID As Integer, intB01ID As Integer)
        Dim cB06 As BO.b06WorkflowStep = Me.Factory.b06WorkflowStepBL.LoadKickOffStep(intB01ID)
        If cB06 Is Nothing Then Return
        Me.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, intP41ID, BO.x29IdEnum.p41Project, "", "", False, Nothing)
    End Sub

    Private Function TestX69(lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Dim x As Integer = 0
        For Each c In lisX69
            x += 1
            If c.x67ID = 0 Then
                _Error = "V nastavení projektových rolí chybí v řádku " & x.ToString & " specifikace projektové role."
                Return False
            End If
            If c.j02ID = 0 And c.j11ID = 0 Then
                _Error = "V nastavení projektových rolí chybí v řádku " & x.ToString & " specifikace osoby nebo týmu."
                Return False
            End If
        Next
        Return True
    End Function


    Public Function Delete(intPID As Integer) As Boolean Implements Ip41ProjectBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, intPID) 'úschova kvůli logování historie
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p41_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function

    Function GetGridDataSource(myQuery As BO.myQueryP41) As DataTable Implements Ip41ProjectBL.GetGridDataSource
        Return _cDL.GetGridDataSource(myQuery)
    End Function
    Public Function GetList(mq As BO.myQueryP41) As IEnumerable(Of BO.p41Project) Implements Ip41ProjectBL.GetList
        Return _cDL.GetList(mq)
    End Function
    

    Public Function GetList_o39(intPID As Integer) As IEnumerable(Of BO.o39Project_Address) Implements Ip41ProjectBL.GetList_o39
        Return _cDL.GetList_o39(intPID)
    End Function
   
   
    Public Function GetVirtualCount(myQuery As BO.myQueryP41) As Integer Implements Ip41ProjectBL.GetVirtualCount
        Return _cDL.GetVirtualCount(myQuery)
    End Function

    Public Function InhaleRecordDisposition(cRec As BO.p41Project) As BO.p41RecordDisposition Implements Ip41ProjectBL.InhaleRecordDisposition
        Dim c As New BO.p41RecordDisposition
        c.P56_Create = Factory.TestPermission(BO.x53PermValEnum.GR_P56_Creator)

        If Factory.TestPermission(BO.x53PermValEnum.GR_P41_Owner) Or cRec.j02ID_Owner = Factory.SysUser.j02ID Then
            'má vlastnická práva
            c.ReadAccess = True
            c.OwnerAccess = True
        End If

        With Factory.x67EntityRoleBL
            If Not c.OwnerAccess Then
                c.OwnerAccess = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P41_Owner, True)
            End If
            If Not c.OwnerAccess Then
                If Not c.P56_Create Then
                    c.P56_Create = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P56_Creator, True)
                End If
                c.P31_RecalcRates = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P31_RecalcRates, True)
                c.P31_Move2Bin = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P31_Move2Bin, True)
                c.P31_MoveToOtherProject = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P31_MoveToOtherProject, True)
                c.p91_Read = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P91_Reader, True)
            Else
                's vlastnickým právem může téměř vše
                c.P56_Create = True
                c.P31_RecalcRates = True
                c.P31_Move2Bin = True
                c.P31_MoveToOtherProject = True
                c.p91_Read = True

            End If

            c.p91_DraftCreate = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P91_Creator, True)
            If Not c.ReadAccess Then
                c.ReadAccess = .HasPersonEntityRole(BO.x29IdEnum.p41Project, cRec.PID)
            End If

            If c.OwnerAccess Then
                'vlastník má automaticky právo na rozpočet a kapacitní plán
                c.p45_Owner = True : c.p47_Owner = True : c.p45_Read = True
            Else
                c.p45_Owner = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P45_Owner, True)
                c.p47_Owner = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P47_Owner, True)
                c.p45_Read = .TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.PID, BO.x53PermValEnum.PR_P45_Reader, True)

            End If
            c.x67IDs = .TestedX67IDs()
            If Not c.ReadAccess And c.x67IDs.Count > 0 Then c.ReadAccess = True 'pokud má alespoň jednu projektovou roli, pak má přístup na čtení karty projektu
        End With

        Return c
    End Function

    Sub UpdateSelectedProjectRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP41ID As Integer) Implements Ip41ProjectBL.UpdateSelectedProjectRole
        _cDL.UpdateSelectedProjectRole(intX67ID, lisX69, intP41ID)
    End Sub
    Sub ClearSelectedProjectRole(intX67ID As Integer, intP41ID As Integer) Implements Ip41ProjectBL.ClearSelectedProjectRole
        _cDL.ClearSelectedProjectRole(intX67ID, intP41ID)
    End Sub

    Public Function ConvertFromDraft(intPID As Integer) As Boolean Implements Ip41ProjectBL.ConvertFromDraft
        Return _cDL.ConvertFromDraft(intPID)
    End Function
    Public Function HasChildRecords(intPID As Integer) As Boolean Implements Ip41ProjectBL.HasChildRecords
        Return _cDL.HasChildRecords(intPID)
    End Function

    Public Function GetTopProjectsByWorksheetEntry(intJ02ID As Integer, intGetTopRecs As Integer) As List(Of Integer) Implements Ip41ProjectBL.GetTopProjectsByWorksheetEntry
        Return _cDL.GetTopProjectsByWorksheetEntry(intJ02ID, intGetTopRecs)
    End Function
    Public Function IsMyFavouriteProject(intPID As Integer) As Boolean Implements Ip41ProjectBL.IsMyFavouriteProject
        Return _cDL.IsMyFavouriteProject(intPID)
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP41, strSumFields As String) As DataTable Implements Ip41ProjectBL.GetGridFooterSums
        Return _cDL.GetGridFooterSums(myQuery, strSumFields)
    End Function
    Public Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p41Project Implements Ip41ProjectBL.LoadTreeTop
        Return _cDL.LoadTreeTop(intCurTreeIndex)
    End Function
    Public Function BatchUpdate_TreeChilds(intPID As Integer, bolProjectRoles As Boolean, bolP28ID As Boolean, bolP87ID As Boolean, bolP51ID As Boolean, bolP92ID As Boolean, bolJ18ID As Boolean, bolP61ID As Boolean, bolValidity As Boolean) As Boolean Implements Ip41ProjectBL.BatchUpdate_TreeChilds
        Return _cDL.BatchUpdate_TreeChilds(intPID, bolProjectRoles, bolP28ID, bolP87ID, bolP51ID, bolP92ID, bolJ18ID, bolP61ID, bolValidity)

    End Function
    Public Function GetRolesInline(intPID As Integer) As String Implements Ip41ProjectBL.GetRolesInline
        Return _cDL.GetRolesInline(intPID)
    End Function
End Class
