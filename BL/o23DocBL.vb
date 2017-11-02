Public Interface Io23DocBL
    Inherits IFMother
    Function Save(cRec As BO.o23Doc, intX18ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), lisX19 As List(Of BO.x19EntityCategory_Binding), x20IDs As List(Of Integer), strUploadGUID As String) As Boolean
    Function Load(intPID As Integer) As BO.o23Doc
    Function LoadByCode(strCode As String, intX23ID As Integer) As BO.o23Doc
    Function LoadByExternalPID(strExternalPID As String) As BO.o23Doc
    Function LoadHtmlContent(intPID As Integer) As String
    ''Function LoadFolders(intPID As Integer) As String
    Function GetEntityPidByX20ID(intO23ID As Integer, intX20ID As Integer) As Integer
    Function SaveHtmlContent(intPID As Integer, strHtmlContent As String, Optional strPlainText As String = "") As Boolean
    ''Function SaveFolders(intPID As Integer, strFoldersByPipes As String) As Boolean
    Function Delete(intPID As Integer) As Boolean
    Function GetList(myQuery As BO.myQueryO23) As IEnumerable(Of BO.o23Doc)
    Function GetDataTable4Grid(myQuery As BO.myQueryO23) As DataTable
    Function GetVirtualCount(myQuery As BO.myQueryO23) As Integer
    Sub SetCalendarDateFields(strCalendarFieldStart As String, strCalendarFieldEnd As String)
    Function GetRolesInline(intPID As Integer) As String
    Sub Handle_Reminder()
    Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.o23Doc)
    Function LockFilesInDocument(cRec As BO.o23Doc, lockFlag As BO.o23LockedTypeENUM) As Boolean
    Function UnLockFilesInDocument(cRec As BO.o23Doc) As Boolean
    Function InhaleDisposition(cRec As BO.o23Doc, Optional cX18 As BO.x18EntityCategory = Nothing) As BO.o23RecordDisposition
    Sub DecryptRecord(ByRef cRec As BO.o23Doc)
    Sub EncryptRecord(ByRef cRec As BO.o23Doc)
    Function EncryptString(str As String) As String
    Function DecryptString(str As String) As String
    ''Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean
End Interface
Class o23DocBL
    Inherits BLMother
    Implements Io23DocBL
    Private WithEvents _cDL As DL.o23DocDL
    Private Const _key As String = "Aesthe22derm"

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o23DocDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o23Doc, intX18ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), lisX19 As List(Of BO.x19EntityCategory_Binding), x20IDs As List(Of Integer), strUploadGUID As String) As Boolean Implements Io23DocBL.Save
        Dim lisTempUpload As IEnumerable(Of BO.p85TempBox) = Nothing

        If strUploadGUID <> "" Then
            lisTempUpload = Me.Factory.p85TempBoxBL.GetList(strUploadGUID, True)
        End If

        With cRec
            If .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
            If intX18ID <> 0 Then
                Dim cX18 As BO.x18EntityCategory = Factory.x18EntityCategoryBL.Load(intX18ID)
                If Trim(.o23Name) = "" And cX18.x18EntryNameFlag = BO.x18EntryNameENUM.Manual Then
                    _Error = "Chybí název." : Return False
                End If
                If cX18.x18IsCalendar And cX18.x18CalendarFieldStart <> "" And cX18.x18CalendarFieldEnd <> "" Then
                    Dim d1 As Date? = BO.BAS.GetPropertyValue(cRec, cX18.x18CalendarFieldStart), d2 As Date? = BO.BAS.GetPropertyValue(cRec, cX18.x18CalendarFieldEnd)
                    If d1 Is Nothing Or d2 Is Nothing Then
                        _Error = "Tento záznam navíc funguje jako událost v kalendáři. Rozsah datumů události není zadán správně." : Return False
                    End If
                    If d1 > d2 Then
                        _Error = "Tento záznam navíc funguje jako událost v kalendáři. Datum začátku události nesmí být větší než datum konce." : Return False
                    End If
                End If
            End If

            ''If .x23ID = 0 Then _Error = "Chybí vazba na číselník." : Return False

            ''Dim cX23 As BO.x23EntityField_Combo = Factory.x23EntityField_ComboBL.Load(.x23ID)
            ''If cX23.x23DataSource <> "" Then
            ''    _Error = "Combo seznam [" & cX23.x23Name & "] má externí datový zdroj combo položek." : Return False
            ''End If
        End With
        If Not x20IDs Is Nothing Then   'otestovat vyplnění povinných vazeb
            If x20IDs.Count > 0 And intX18ID <> 0 Then
                Dim lisAllRequiredX20 As IEnumerable(Of BO.x20_join_x18) = Factory.x18EntityCategoryBL.GetList_x20_join_x18(intX18ID).Where(Function(p) p.x20IsEntryRequired = True)   'povinné vazby
                For Each intX20ID As Integer In x20IDs
                    If lisAllRequiredX20.Where(Function(p) p.x20ID = intX20ID).Count > 0 Then
                        Dim c As BO.x20_join_x18 = lisAllRequiredX20.Where(Function(p) p.x20ID = intX20ID).First
                        If Not c Is Nothing And lisX19.Where(Function(p) p.x20ID = intX20ID).Count = 0 Then
                            _Error = String.Format("V záznamu [{0}] chybí povinná vazba [{1}].", c.x18Name, c.BindName)
                            Return False
                        End If
                    End If
                Next
            End If
        End If

        ''Dim cRecOld As BO.o23Doc = Nothing
        ''If cRec.PID <> 0 Then cRecOld = Load(cRec.PID)

        If _cDL.Save(cRec, lisX69) Then
            Dim intO23ID As Integer = _LastSavedPID

            If Not lisX19 Is Nothing Then
                Factory.x18EntityCategoryBL.SaveX19Binding(into23ID, lisX19, x20IDs)
            End If

            _cDL.RunSp_AfterSave(into23ID)

            If strUploadGUID <> "" Then
                Me.Factory.o27AttachmentBL.UploadAndSaveUserControl(lisTempUpload, BO.x29IdEnum.o23Doc, into23ID)
            End If

            Me.RaiseAppEvent_TailoringAfterSave(into23ID, "o23_aftersave")
            If intX18ID <> 0 Then
                Dim cX18 As BO.x18EntityCategory = Me.Factory.x18EntityCategoryBL.Load(intX18ID)
                ''Handle_CreateFileSystemFolders(intO23ID, cX18)
               
                Dim intB01ID As Integer = cX18.b01ID
                If cRec.PID = 0 Then
                    If intB01ID > 0 Then InhaleDefaultWorkflowMove(into23ID, intB01ID) 'je třeba nahodit výchozí workflow stav
                Else
                    If intB01ID > 0 And cRec.b02ID = 0 Then InhaleDefaultWorkflowMove(cRec.PID, intB01ID) 'chybí hodnota workflow stavu
                End If
            End If


            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetEntityPidByX20ID(intO23ID As Integer, intX20ID As Integer) As Integer Implements Io23DocBL.GetEntityPidByX20ID
        Return _cDL.GetEntityPidByX20ID(intO23ID, intX20ID)
    End Function
    ''Private Sub Handle_CreateFileSystemFolders(intO23ID As Integer, cX18 As BO.x18EntityCategory)
    ''    Dim lisX17 As IEnumerable(Of BO.x17EntityCategory_Folder) = Me.Factory.x18EntityCategoryBL.GetList_x17(cX18.PID)
    ''    If lisX17.Count = 0 Then Return
    ''    Dim objects As New List(Of Object)
    ''    If lisX17.Where(Function(p) p.x17Path.IndexOf("%]") > 0).Count > 0 Then
    ''        Dim lisX20 As IEnumerable(Of BO.x20EntiyToCategory) = Me.Factory.x18EntityCategoryBL.GetList_x20(cX18.PID)
    ''        For Each cX20 In lisX20
    ''            Select Case cX20.x29ID
    ''                Case BO.x29IdEnum.p56Task
    ''                    objects.Add(Factory.p56TaskBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''                Case BO.x29IdEnum.j02Person
    ''                    objects.Add(Factory.j02PersonBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''                Case BO.x29IdEnum.p41Project
    ''                    objects.Add(Factory.p41ProjectBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''                Case BO.x29IdEnum.p28Contact
    ''                    objects.Add(Factory.p28ContactBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''                Case BO.x29IdEnum.p91Invoice
    ''                    objects.Add(Factory.p91InvoiceBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''                Case BO.x29IdEnum.o23Doc
    ''                    objects.Add(Factory.o23DocBL.Load(GetEntityPidByX20ID(intO23ID, cX20.x20ID)))
    ''            End Select
    ''        Next
    ''    End If

    ''    Dim folders As New List(Of String)
    ''    For Each c In lisX17
    ''        Dim strDir As String = c.x17Path
    ''        If strDir.IndexOf("%]") > 0 Then
    ''            Dim cM As New BO.clsMergeContent
    ''            strDir = cM.MergeContent(objects, strDir, "")
    ''        End If
    ''        If Not System.IO.Directory.Exists(strDir) Then
    ''            System.IO.Directory.CreateDirectory(strDir)
    ''            folders.Add(strDir)
    ''        End If
    ''    Next
    ''    Me.SaveFolders(intO23ID, String.Join("|", folders))
    ''End Sub
    Private Sub InhaleDefaultWorkflowMove(into23ID As Integer, intB01ID As Integer)
        Dim cB06 As BO.b06WorkflowStep = Me.Factory.b06WorkflowStepBL.LoadKickOffStep(intB01ID)
        If cB06 Is Nothing Then Return
        Me.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, into23ID, BO.x29IdEnum.o23Doc, "", "", False, Nothing)
    End Sub
    Public Function GetVirtualCount(myQuery As BO.myQueryO23) As Integer Implements Io23DocBL.GetVirtualCount
        Return _cDL.GetVirtualCount(myQuery)
    End Function
    Public Function Load(intPID As Integer) As BO.o23Doc Implements Io23DocBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByCode(strCode As String, intX23ID As Integer) As BO.o23Doc Implements Io23DocBL.LoadByCode
        Return _cDL.LoadByCode(strCode, intX23ID)
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.o23Doc Implements Io23DocBL.LoadByExternalPID
        Return Nothing
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io23DocBL.Delete
        Dim cRec As BO.o23Doc = Load(intPID)
        If _cDL.Delete(intPID) Then
            Dim lisX28 As IEnumerable(Of BO.x28EntityField) = Factory.x28EntityFieldBL.GetList(cRec.x23ID)
            For Each c In lisX28
                _cDL.ClearComboItemTextInData(c, intPID)
            Next
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(myQuery As BO.myQueryO23) As IEnumerable(Of BO.o23Doc) Implements Io23DocBL.GetList
        Return _cDL.GetList(myQuery)
    End Function
    Public Function GetDataTable4Grid(myQuery As BO.myQueryO23) As DataTable Implements Io23DocBL.GetDataTable4Grid
        Return _cDL.GetDataTable4Grid(myQuery)
    End Function
    Public Sub SetCalendarDateFields(strCalendarFieldStart As String, strCalendarFieldEnd As String) Implements Io23DocBL.SetCalendarDateFields
        _cDL.CalendarFieldStart = strCalendarFieldStart
        _cDL.CalendarFieldEnd = strCalendarFieldEnd
    End Sub
    Public Function GetRolesInline(intPID As Integer) As String Implements Io23DocBL.GetRolesInline
        Return _cDL.GetRolesInline(intPID)
    End Function
    Public Function LoadHtmlContent(intPID As Integer) As String Implements Io23DocBL.LoadHtmlContent
        Return _cDL.LoadHtmlContent(intPID)
    End Function
    ''Public Function LoadFolders(intPID As Integer) As String Implements Io23DocBL.LoadFolders
    ''    Return _cDL.LoadFolders(intPID)
    ''End Function
    Public Function SaveHtmlContent(intPID As Integer, strHtmlContent As String, Optional strPlainText As String = "") As Boolean Implements Io23DocBL.SaveHtmlContent
        Return _cDL.SaveHtmlContent(intPID, strHtmlContent, strPlainText)
    End Function
    ''Public Function SaveFolders(intPID As Integer, strFoldersByPipes As String) As Boolean Implements Io23DocBL.SaveFolders
    ''    Return _cDL.SaveFolders(intPID, strFoldersByPipes)
    ''End Function
    Public Sub Handle_Reminder() Implements Io23DocBL.Handle_Reminder
        Dim d1 As Date = DateAdd(DateInterval.Day, -2, Now)
        Dim d2 As Date = Now
        Dim lisO23 As IEnumerable(Of BO.o23Doc) = _cDL.GetList_WaitingOnReminder(d1, d2)
        For Each cRec In lisO23
            Me.RaiseAppEvent(BO.x45IDEnum.o23_remind, cRec.PID, cRec.o23Name)

        Next

    End Sub

    Public Function GetList_forMessagesDashboard(intJ02ID As Integer) As IEnumerable(Of BO.o23Doc) Implements Io23DocBL.GetList_forMessagesDashboard
        Return _cDL.GetList_forMessagesDashboard(intJ02ID)
    End Function

    Public Function LockFilesInDocument(cRec As BO.o23Doc, lockFlag As BO.o23LockedTypeENUM) As Boolean Implements Io23DocBL.LockFilesInDocument
        Dim cDisp As BO.o23RecordDisposition = InhaleDisposition(cRec)
        If Not cDisp.LockUnlockFiles_Flag1 Then
            _Error = "Nemáte oprávnění pro uzamykání/odemykání přístupu k souborům dokumentu." : Return False
        End If
        Return _cDL.LockFiles(cRec.PID, lockFlag)
    End Function
    Public Function UnLockFilesInDocument(cRec As BO.o23Doc) As Boolean Implements Io23DocBL.UnLockFilesInDocument
        Dim cDisp As BO.o23RecordDisposition = InhaleDisposition(cRec)
        If Not cDisp.LockUnlockFiles_Flag1 Then
            _Error = "Nemáte oprávnění pro uzamykání/odemykání přístupu k souborům dokumentu." : Return False
        End If
        Return _cDL.UnLockFiles(cRec.PID)
    End Function

    Public Function InhaleDisposition(cRec As BO.o23Doc, Optional cX18 As BO.x18EntityCategory = Nothing) As BO.o23RecordDisposition Implements Io23DocBL.InhaleDisposition
        If cX18 Is Nothing Then cX18 = Factory.x18EntityCategoryBL.Load(cRec.x18ID)
        Dim cDX18 As BO.x18RecordDisposition = Factory.x18EntityCategoryBL.InhaleDisposition(cX18)

        Dim cD As New BO.o23RecordDisposition
        cD.CreateItem = cDX18.CreateItem
        cD.OwnerAccess = cDX18.OwnerItems
        cD.UploadAndComment = cDX18.ReadAndUploadAndComment
        cD.ReadAccess = cDX18.ReadItems

        If cRec.j02ID_Owner = Factory.SysUser.j02ID Then
            'vlasník záznamu má plná práva, pokud to není ve workflow zakázáno
            Dim bolTested As Boolean = False
            If cRec.b02ID <> 0 Then
                Dim cB02 As BO.b02WorkflowStatus = Factory.b02WorkflowStatusBL.Load(cRec.b02ID)
                If cB02.b02IsRecordReadOnly4Owner Then  'aktuální workflow status má nastaveno, že vlastník záznam má pouze readonly přístup
                    cD.ReadAccess = True
                    bolTested = True
                End If
            End If
            If Not bolTested Then
                cD.ReadAccess = True : cD.OwnerAccess = True : cD.UploadAndComment = True : cD.LockUnlockFiles_Flag1 = True 'vlastník má plná práva
            End If

        End If


        If cD.OwnerAccess Then
            cD.LockUnlockFiles_Flag1 = True
        End If

        Return cD
    End Function

    Public Sub DecryptRecord(ByRef cRec As BO.o23Doc) Implements Io23DocBL.DecryptRecord
        With cRec
            .o23FreeText01 = BO.Crypto.Decrypt(.o23FreeText01, _key)
            .o23FreeText02 = BO.Crypto.Decrypt(.o23FreeText02, _key)
            .o23FreeText03 = BO.Crypto.Decrypt(.o23FreeText03, _key)
            .o23FreeText04 = BO.Crypto.Decrypt(.o23FreeText04, _key)
            .o23FreeText05 = BO.Crypto.Decrypt(.o23FreeText05, _key)
            .o23BigText = BO.Crypto.Decrypt(.o23BigText, _key)
        End With

    End Sub
    Public Sub EncryptRecord(ByRef cRec As BO.o23Doc) Implements Io23DocBL.EncryptRecord
        With cRec
            .o23FreeText01 = BO.Crypto.Encrypt(.o23FreeText01, _key)
            .o23FreeText02 = BO.Crypto.Encrypt(.o23FreeText02, _key)
            .o23FreeText03 = BO.Crypto.Encrypt(.o23FreeText03, _key)
            .o23FreeText04 = BO.Crypto.Encrypt(.o23FreeText04, _key)
            .o23FreeText05 = BO.Crypto.Encrypt(.o23FreeText05, _key)
            .o23BigText = BO.Crypto.Encrypt(.o23BigText, _key)
        End With
    End Sub
    Public Function EncryptString(str As String) As String Implements Io23DocBL.EncryptString
        Return BO.Crypto.Encrypt(str, _key)
    End Function
    Public Function DecryptString(str As String) As String Implements Io23DocBL.DecryptString
        Return BO.Crypto.Decrypt(str, _key)
    End Function

    ''Public Function UpdateImapSource(intPID As Integer, intO43ID As Integer) As Boolean Implements Io23DocBL.UpdateImapSource
    ''    Return _cDL.UpdateImapSource(intPID, intO43ID)
    ''End Function
End Class
