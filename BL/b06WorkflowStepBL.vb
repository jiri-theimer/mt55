Public Interface Ib06WorkflowStepBL
    Inherits ifMother
    Function Save(cRec As BO.b06WorkflowStep, lisB08 As List(Of BO.b08WorkflowReceiverToStep), lisB11 As List(Of BO.b11WorkflowMessageToStep), lisB10 As List(Of BO.b10WorkflowCommandCatalog_Binding)) As Boolean
    Function Load(intPID As Integer) As BO.b06WorkflowStep
    Function LoadKickOffStep(intB01ID As Integer) As BO.b06WorkflowStep
    Function Delete(intPID As Integer) As Boolean
    Function GetList(intB01ID As Integer) As IEnumerable(Of BO.b06WorkflowStep)
    Function GetList_Allb09IDs() As IEnumerable(Of BO.b09WorkflowCommandCatalog)
    Function GetList_B10(intPID As Integer) As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding)
    Function GetList_B08(intPID As Integer) As IEnumerable(Of BO.b08WorkflowReceiverToStep)
    Function GetList_B11(intPID As Integer) As IEnumerable(Of BO.b11WorkflowMessageToStep)

    Function GetPossibleWorkflowSteps4Person(strRecordPrefix As String, intRecordPID As Integer, intJ02ID As Integer) As List(Of BO.WorkflowStepPossible4User)

    Function RunWorkflowStep(cB06 As BO.b06WorkflowStep, intRecordPID As Integer, x29id As BO.x29IdEnum, strComment As String, strUploadGUID As String, bolManualStep As Boolean, lisNominee As List(Of BO.x69EntityRole_Assign)) As Boolean

    Function GetAutoWorkflowSQLResult(intRecordPID As Integer, cB06 As BO.b06WorkflowStep) As Integer
End Interface
Class b06WorkflowStepBL
    Inherits BLMother
    Implements Ib06WorkflowStepBL
    Private WithEvents _cDL As DL.b06WorkflowStepDL
    

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.b06WorkflowStepDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Delete(intPID As Integer) As Boolean Implements Ib06WorkflowStepBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(intB01ID As Integer) As System.Collections.Generic.IEnumerable(Of BO.b06WorkflowStep) Implements Ib06WorkflowStepBL.GetList
        Return _cDL.GetList(intB01ID)
    End Function

    Public Function Load(intPID As Integer) As BO.b06WorkflowStep Implements Ib06WorkflowStepBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadKickOffStep(intB01ID As Integer) As BO.b06WorkflowStep Implements Ib06WorkflowStepBL.LoadKickOffStep
        Return _cDL.LoadKickOffStep(intB01ID)
    End Function

    Public Function Save(cRec As BO.b06WorkflowStep, lisB08 As List(Of BO.b08WorkflowReceiverToStep), lisB11 As List(Of BO.b11WorkflowMessageToStep), lisB10 As List(Of BO.b10WorkflowCommandCatalog_Binding)) As Boolean Implements Ib06WorkflowStepBL.Save
        If cRec.b02ID = 0 Then
            _Error = "[Workflow stav] je povinná pole k vyplnění."
        End If
        If cRec.b02ID_Target = 0 And Trim(cRec.b06Name) = "" Then
            _Error = "Pokud se nemění cílový stav, název kroku je povinný."
        End If
        If Not cRec.b06IsKickOffStep Then
            If lisB08.Count = 0 And cRec.b06IsManualStep Then _Error = "V nastavení kroku chybí určení, kdo spouští krok." : Return False
        End If
        If cRec.b06IsKickOffStep And cRec.b02ID_Target = 0 Then
            _Error = "Startovací workflow krok musí mít definován cílový stav." : Return False
        End If
        If cRec.b06IsNominee Then
            If cRec.x67ID_Nominee = 0 Then
                _Error = "Pro nominaci musíte specifikovat roli, kterou nominovaný obdrží."
            End If
        Else
            cRec.b06IsNomineeRequired = False : cRec.x67ID_Nominee = 0
        End If
        If (cRec.j11ID_Direct <> 0 Or cRec.b02ID_LastReceiver_ReturnTo <> 0) And cRec.x67ID_Direct = 0 Then
            _Error = "Musíte specifikovat roli automatického řešitele."
        End If
        If (cRec.j11ID_Direct = 0 And cRec.b02ID_LastReceiver_ReturnTo = 0) And cRec.x67ID_Direct <> 0 Then
            _Error = "Musíte specifikovat příjemce pro automatickou změnu řešitele."
        End If
       
        If _Error <> "" Then
            Return False
        End If
        If _cDL.Save(cRec, lisB08, lisB11, lisB10) Then
            _LastSavedPID = _cDL.LastSavedRecordPID
            Return True
        Else
            _Error = _cDL.ErrorMessage
            Return False
        End If
    End Function

    Public Function GetList_Allb09IDs() As IEnumerable(Of BO.b09WorkflowCommandCatalog) Implements Ib06WorkflowStepBL.GetList_Allb09IDs
        Return _cDL.GetList_Allb09IDs()
    End Function
    Public Function GetList_B10(intPID As Integer) As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding) Implements Ib06WorkflowStepBL.GetList_B10
        Return _cDL.GetList_B10(intPID)
    End Function
    Public Function GetList_B08(intPID As Integer) As IEnumerable(Of BO.b08WorkflowReceiverToStep) Implements Ib06WorkflowStepBL.GetList_B08
        Return _cDL.GetList_B08(intPID)
    End Function
    Public Function GetList_B11(intPID As Integer) As IEnumerable(Of BO.b11WorkflowMessageToStep) Implements Ib06WorkflowStepBL.GetList_B11
        Return _cDL.GetList_B11(intPID)
    End Function

    

    Private Function ValidateWorkflowStepBeforeRun(intRecordPID As Integer, x29id As BO.x29IdEnum, cB06 As BO.b06WorkflowStep, strComment As String, strUploadGUID As String, bolManualStep As Boolean, lisNominee As List(Of BO.x69EntityRole_Assign)) As Boolean
        If cB06.PID = 0 Then
            _Error = "Musíte zvolit z nabídky konkrétní krok!"
        End If
        If cB06.b06IsCommentRequired And Trim(strComment) = "" Then
            _Error = "Krok [" & cB06.b06Name & "] vyžaduje zapsat komentář!" : Return False
        End If
        If cB06.b06IsNomineeRequired Then
            If lisNominee Is Nothing Then
                _Error = "Chybí nominace nového řešitele." : Return False                
            End If
            If lisNominee.Count = 0 Then
                _Error = "Chybí nominace nového řešitele." : Return False
            End If
        End If
        If Not lisNominee Is Nothing Then
            For Each c In lisNominee
                If c.j02ID <> 0 And c.j11ID <> 0 Then
                    _Error = "V jednom řádku nominace může být buď pouze osoba nebo pouze tým." : Return False
                End If
            Next
        End If

        If _Error <> "" Then Return False
        If cB06.b06IsRunOneInstanceOnly Then
            'krok lze spustit u akce pouze jednou
            Dim lisHistory As IEnumerable(Of BO.b05Workflow_History) = Me.Factory.b05Workflow_HistoryBL.GetList(intRecordPID, x29id)
            If lisHistory.Where(Function(p) p.b06ID = cB06.PID).Count > 0 Then
                _Error = "Krok [" & cB06.b06Name & "] je povoleno spouštět pouze jednou!"
            End If
        End If
        If _Error <> "" Then Return False

        If cB06.b06ValidateBeforeRunSQL <> "" Then
            'spuštění kroku je podmíněno splněním SQL dotazu
            If _cDL.GetBeforeRunWorkflowSQLResult(intRecordPID, cB06) <> 1 Then
                _Error = cB06.b06ValidateBeforeErrorMessage
                Return False
            End If

        End If



        If _Error <> "" Then
            Return False
        Else
            Return True
        End If
    End Function


    Function RunWorkflowStep(cB06 As BO.b06WorkflowStep, intRecordPID As Integer, x29id As BO.x29IdEnum, strComment As String, strUploadGUID As String, bolManualStep As Boolean, lisNominee As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ib06WorkflowStepBL.RunWorkflowStep
        _Error = ""
        If Not ValidateWorkflowStepBeforeRun(intRecordPID, x29id, cB06, strComment, strUploadGUID, bolManualStep, lisNominee) Then
            Return False
        End If
        If Not lisNominee Is Nothing Then
            For Each cX69 In lisNominee
                cX69.x67ID = cB06.x67ID_Nominee
                If cX69.j02ID = 0 And cX69.j11ID = 0 Then
                    _Error = "V nominaci chybí specifikace osoby nebo týmu osob." : Return False
                End If
            Next
        End If
        If cB06.x67ID_Direct <> 0 And (cB06.j11ID_Direct <> 0 Or cB06.b02ID_LastReceiver_ReturnTo <> 0) Then
            cB06.x67ID_Nominee = cB06.x67ID_Direct
            'automatická změna řešitele
            If cB06.j11ID_Direct <> 0 Then
                'řešitelem bude explicitně určený tým
                lisNominee = New List(Of BO.x69EntityRole_Assign)
                Dim cX69 As New BO.x69EntityRole_Assign
                cX69.x67ID = cB06.x67ID_Direct
                cX69.j11ID = cB06.j11ID_Direct
                lisNominee.Add(cX69)
            End If
            If cB06.b02ID_LastReceiver_ReturnTo <> 0 Then
                'vrátit řešiteli, který byl poslední ve statusu b02ID_LastReceiver_ReturnTo
                lisNominee = _cDL.GetListOfLastWorkflowX69(cB06.x67ID_Direct, intRecordPID, cB06.b02ID_LastReceiver_ReturnTo)

            End If
        End If
        If Not lisNominee Is Nothing And cB06.x67ID_Nominee <> 0 Then
            Dim lisAll As List(Of BO.x69EntityRole_Assign) = Me.Factory.x67EntityRoleBL.GetList_x69(x29id, intRecordPID).Where(Function(p) p.x67ID <> cB06.x67ID_Nominee).ToList
            For Each cX69 In lisNominee
                lisAll.Add(cX69)
            Next
            lisNominee = lisAll
        End If

        Dim intCurB02ID As Integer = 0, bolStopAutoNotification As Boolean = False, intJ02ID_Owner As Integer = 0, intP41ID_Ref As Integer = 0

        Select Case x29id
            Case BO.x29IdEnum.p41Project
                Dim cRec As BO.p41Project = Me.Factory.p41ProjectBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : bolStopAutoNotification = cRec.p41IsNoNotify
                intJ02ID_Owner = cRec.j02ID_Owner
                If Not lisNominee Is Nothing Then
                    Me.Factory.p41ProjectBL.Save(cRec, Nothing, Nothing, lisNominee, Nothing)
                End If
            Case BO.x29IdEnum.p56Task
                Dim cRec As BO.p56Task = Me.Factory.p56TaskBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : bolStopAutoNotification = cRec.p56IsNoNotify
                intJ02ID_Owner = cRec.j02ID_Owner : intP41ID_Ref = cRec.p41ID
                If Not lisNominee Is Nothing Then
                    Me.Factory.p56TaskBL.Save(cRec, lisNominee, Nothing, "")
                End If
            Case BO.x29IdEnum.p91Invoice
                Dim cRec As BO.p91Invoice = Me.Factory.p91InvoiceBL.Load(intRecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner : intP41ID_Ref = cRec.p41ID_First
                intCurB02ID = cRec.b02ID
            Case BO.x29IdEnum.p28Contact
                Dim cRec As BO.p28Contact = Me.Factory.p28ContactBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID
                intJ02ID_Owner = cRec.j02ID_Owner
                If Not lisNominee Is Nothing Then
                    Me.Factory.p28ContactBL.Save(cRec, Nothing, Nothing, Nothing, lisNominee, Nothing)
                End If

            Case BO.x29IdEnum.o23Doc
                Dim cRec As BO.o23Doc = Me.Factory.o23DocBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID
                intJ02ID_Owner = cRec.j02ID_Owner
                If Not lisNominee Is Nothing Then
                    Me.Factory.o23DocBL.Save(cRec, 0, lisNominee, Nothing, Nothing, "")
                End If
        End Select

        If cB06.b02ID_Target <> 0 Then
            _cDL.SaveStatusMove(intRecordPID, x29id, cB06, cB06.b02ID_Target, bolManualStep, strComment)
        End If


        Dim intB07ID As Integer = 0
        Dim c As New BO.b07Comment
        c.b07RecordPID = intRecordPID
        c.x29ID = x29id
        c.b07Value = strComment
        c.b07WorkflowInfo = cB06.NameWithTargetStatus
        If Me.Factory.b07CommentBL.Save(c, strUploadGUID, Nothing) Then
            intB07ID = Me.Factory.b07CommentBL.LastSavedPID
        End If

        Dim cB05 As New BO.b05Workflow_History
        With cB05
            .b05RecordPID = intRecordPID
            .x29ID = x29id
            .b06ID = cB06.PID
            .b07ID = intB07ID
            If cB06.b02ID_Target <> 0 Then
                .b02ID_From = intCurB02ID
                .b02ID_To = cB06.b02ID_Target
            End If

            .b05IsManualStep = bolManualStep
        End With
        Me.Factory.b05Workflow_HistoryBL.Save(cB05)
        If cB06.b06RunSQL <> "" Then
            _cDL.RunSQL(cB06.b06RunSQL, intRecordPID)
        End If

        _cDL.RunB09Commands(intRecordPID, x29id, cB06.PID)

        Dim lisB10 As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding) = GetList_B10(cB06.PID).Where(Function(p) p.x18ID <> 0) 'zakládání dokumentů

        If lisB10.Count > 0 Then
            Dim objects As List(Of Object) = GetObjects(intRecordPID, x29id, 0)
            Dim cM As New BO.clsMergeContent

            For Each cB10 In lisB10
                Dim cDoc As New BO.o23Doc
                cDoc.o23Name = cB10.o23Name
                cDoc.o23Name = cM.MergeContent(objects, cDoc.o23Name, "")
                cDoc.x23ID = Factory.x18EntityCategoryBL.Load(cB10.x18ID).x23ID
                Dim lisX20 As List(Of BO.x20EntiyToCategory) = Factory.x18EntityCategoryBL.GetList_x20(cB10.x18ID).Where(Function(p) p.x29ID = x29id).ToList

                Dim lisX19 As New List(Of BO.x19EntityCategory_Binding)
                For Each cX20 In lisX20
                    Dim cc As New BO.x19EntityCategory_Binding
                    cc.x20ID = cX20.x20ID
                    cc.x19RecordPID = intRecordPID
                    lisX19.Add(cc)
                Next
                Factory.o23DocBL.Save(cDoc, cB10.x18ID, Nothing, lisX19, lisX20.Select(Function(p) p.x20ID).ToList, "")
            Next
        End If
        If cB06.f02ID <> 0 Then
            'založit složku
            Factory.f01FolderBL.CreateUpdateFolder(intRecordPID, cB06.f02ID)
        End If

      
        If bolStopAutoNotification Then
            If Not cB06.b06IsManualStep Then bolStopAutoNotification = False 'automatické kroky musí mít vždy zapnutou notifikaci
            If cB06.b06IsKickOffStep Then bolStopAutoNotification = True
        End If

        If Not bolStopAutoNotification Then
            'test případné mailové notifikace
            Dim objects As List(Of Object) = GetObjects(intRecordPID, x29id, intP41ID_Ref)


            Handle_Notification(cB06, intRecordPID, x29id, intJ02ID_Owner, objects, intP41ID_Ref, strComment)
        End If
        Return True
    End Function

    Private Function GetObjects(intRecordPID As Integer, x29id As BO.x29IdEnum, intP41ID_Ref As Integer) As List(Of Object)
        Dim objects As New List(Of Object)
        Select Case x29id
            Case BO.x29IdEnum.p56Task
                objects.Add(Factory.p56TaskBL.Load(intRecordPID))
            Case BO.x29IdEnum.p41Project
                objects.Add(Factory.p41ProjectBL.Load(intRecordPID))
            Case BO.x29IdEnum.p28Contact
                objects.Add(Factory.p28ContactBL.Load(intRecordPID))
            Case BO.x29IdEnum.p91Invoice
                objects.Add(Factory.p91InvoiceBL.Load(intRecordPID))
            
            Case BO.x29IdEnum.o23Doc
                objects.Add(Factory.o23DocBL.Load(intRecordPID))
        End Select
        If intP41ID_Ref <> 0 Then objects.Add(Factory.p41ProjectBL.Load(intP41ID_Ref))
        Return objects
    End Function
    ''Private Sub CreateDirectories(intRecordPID As Integer, cB06 As BO.b06WorkflowStep, x29id As BO.x29IdEnum)
    ''    _Error = ""
    ''    With cB06
    ''        If Trim(.b06CreateDirectory) = "" Then Return
    ''    End With

    ''    Dim objects As List(Of Object) = GetObjects(intRecordPID, x29id, 0)
    ''    Dim cM As New BO.clsMergeContent, strDirs As String = cB06.b06CreateDirectory
    ''    strDirs = cM.MergeContent(objects, strDirs, "")

    ''    Try
    ''        For Each strDir As String In BO.BAS.ConvertDelimitedString2List(strDirs, ";")
    ''            If Not System.IO.Directory.Exists(strDir) Then
    ''                System.IO.Directory.CreateDirectory(strDir)
    ''            End If
    ''        Next


    ''    Catch ex As Exception
    ''        _Error += " | " & ex.Message
    ''    End Try

    ''End Sub
  

    Private Sub Handle_Notification(cB06 As BO.b06WorkflowStep, intRecordPID As Integer, x29id As BO.x29IdEnum, intJ02ID_Owner As Integer, objects As List(Of Object), intP41ID_Ref As Integer, strComment As String)
        Dim lisB11 As IEnumerable(Of BO.b11WorkflowMessageToStep) = GetList_B11(cB06.PID)
        If lisB11.Count = 0 Then Return 'ke kroku nejsou definovány notifikační události

        Dim strLinkUrl As String = Factory.GetRecordLinkUrl(BO.BAS.GetDataPrefix(x29id), intRecordPID)
        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Factory.x67EntityRoleBL.GetList_x69(x29id, intRecordPID)
        Dim lisX69Ref As IEnumerable(Of BO.x69EntityRole_Assign) = Nothing
        If intP41ID_Ref <> 0 Then
            lisX69Ref = Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, intP41ID_Ref)
        End If
        For Each c In lisB11
            Dim mrs As New List(Of BO.PersonOrTeam)

            If c.b11IsRecordOwner Then
                mrs.Add(New BO.PersonOrTeam(intJ02ID_Owner, 0))
            End If
            If c.x67ID <> 0 Then
                If lisX69.Where(Function(p) p.x67ID = c.x67ID).Count > 0 Then
                    Dim cRole As BO.x69EntityRole_Assign = lisX69.Where(Function(p) p.x67ID = c.x67ID)(0)
                    mrs.Add(New BO.PersonOrTeam(cRole.j02ID, cRole.j11ID))
                End If
                If Not lisX69Ref Is Nothing Then
                    If lisX69Ref.Where(Function(p) p.x67ID = c.x67ID).Count > 0 Then
                        Dim cRole As BO.x69EntityRole_Assign = lisX69Ref.Where(Function(p) p.x67ID = c.x67ID)(0)
                        mrs.Add(New BO.PersonOrTeam(cRole.j02ID, cRole.j11ID))
                    End If
                End If
            End If
            If c.j11ID > 0 Then
                mrs.Add(New BO.PersonOrTeam(0, c.j11ID))
            End If
            If c.j04ID <> 0 Then
                Dim mq As New BO.myQueryJ03
                mq.j04ID = c.j04ID
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                For Each cJ03 In Factory.j03UserBL.GetList(mq).Where(Function(p) p.j02ID <> 0)
                    mrs.Add(New BO.PersonOrTeam(cJ03.j02ID, 0))
                Next
            End If

            If mrs.Count > 0 Then
                Dim j02ids As List(Of Integer) = mrs.Where(Function(p) p.j02ID <> 0).Select(Function(p) p.j02ID).ToList
                Dim j11ids As List(Of Integer) = mrs.Where(Function(p) p.j11ID <> 0).Select(Function(p) p.j11ID).ToList
                Dim lisReceivers As List(Of BO.x43MailQueue_Recipient) = Factory.j02PersonBL.GetEmails_j02_join_j11(j02ids.ToList, j11ids.ToList)
                If lisReceivers.Count > 0 Then
                    Dim cMerge As New BO.clsMergeContent
                    Dim cB65 As BO.b65WorkflowMessage = Factory.b65WorkflowMessageBL.Load(c.b65ID)
                    Dim mes As New Rebex.Mail.MailMessage
                    mes.From.Add(New Rebex.Mime.Headers.MailAddress(Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress"), "MARKTIME robot"))

                    mes.BodyText = cMerge.MergeContent(objects, cB65.b65MessageBody, strLinkUrl)
                    If mes.BodyText.IndexOf("#RolesInline#") > 0 Or mes.BodyText.IndexOf("[%RolesInline%]") > 0 Then
                        mes.BodyText = Replace(mes.BodyText, "[%RolesInline%]", "#RolesInline#", , , CompareMethod.Text)
                        Select Case x29id
                            Case BO.x29IdEnum.p56Task
                                mes.BodyText = Replace(mes.BodyText, "#RolesInline#", Factory.p56TaskBL.GetRolesInline(intRecordPID))
                            Case BO.x29IdEnum.o23Doc
                                mes.BodyText = Replace(mes.BodyText, "#RolesInline#", Factory.o23DocBL.GetRolesInline(intRecordPID))
                            Case BO.x29IdEnum.p41Project
                                mes.BodyText = Replace(mes.BodyText, "#RolesInline#", Factory.p41ProjectBL.GetRolesInline(intRecordPID))
                            Case BO.x29IdEnum.p28Contact
                                mes.BodyText = Replace(mes.BodyText, "#RolesInline#", Factory.p28ContactBL.GetRolesInline(intRecordPID))
                        End Select

                    End If
                    mes.BodyText = Replace(mes.BodyText, "#comment#", strComment, , , CompareMethod.Text)

                    mes.Subject = cB65.b65MessageSubject
                    If mes.Subject.IndexOf("[") > 0 Then mes.Subject = cMerge.MergeContent(objects, cB65.b65MessageSubject, strLinkUrl)

                    For Each cEmail In lisReceivers
                        'pro každého jedna zpráva
                        Dim lisTo As New List(Of BO.x43MailQueue_Recipient)
                        lisTo.Add(cEmail)

                        Factory.x40MailQueueBL.SaveMessageToQueque(mes, lisTo, x29id, intRecordPID, BO.x40StateENUM.InQueque, cB06.o40ID)
                    Next

                End If
            End If
            If c.b11IsRecordCreatorByEmail Then
                'odpovědět na e-mail
                Dim cMerge As New BO.clsMergeContent
                Dim cB65 As BO.b65WorkflowMessage = Factory.b65WorkflowMessageBL.Load(c.b65ID)
                Dim strBody As String = cMerge.MergeContent(objects, cB65.b65MessageBody, strLinkUrl)
                strBody = Replace(strBody, "#comment#", strComment, , , CompareMethod.Text)
                Factory.x40MailQueueBL.SendAnswer2Ticket(strBody, x29id, intRecordPID)
            End If
        Next

    End Sub


    Public Function GetPossibleWorkflowSteps4Person(strRecordPrefix As String, intRecordPID As Integer, intJ02ID As Integer) As List(Of BO.WorkflowStepPossible4User) Implements Ib06WorkflowStepBL.GetPossibleWorkflowSteps4Person
        'vrací okruh možných workflow kroků pro záznam intRecordPID/strRecordPrefix
        Dim ret As New List(Of BO.WorkflowStepPossible4User), strLogin As String = Factory.SysUser.j03Login
        If intJ02ID = 0 Then intJ02ID = Factory.SysUser.j02ID
        If intJ02ID <> Factory.SysUser.j02ID Then
            Dim cUser As BO.j03User = Factory.j03UserBL.LoadByJ02ID(intJ02ID)
            strLogin = cUser.j03Login
        End If
        Dim x29id As BO.x29IdEnum = BO.BAS.GetX29FromPrefix(strRecordPrefix)
        Dim curPerson As BO.j02Person = Factory.j02PersonBL.Load(intJ02ID)
        Dim lisPersonJ11 As IEnumerable(Of BO.j11Team) = Factory.j02PersonBL.GetList_j11(Factory.SysUser.j02ID)

        Dim intCurB02ID As Integer = 0, intP41ID As Integer = 0, intRecOwnerID As Integer = 0, strRecUserInsert As String = ""
        Select Case strRecordPrefix
            Case "p41"
                Dim cRec As BO.p41Project = Factory.p41ProjectBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : intRecOwnerID = cRec.j02ID_Owner : strRecUserInsert = cRec.UserInsert
            Case "p56"
                Dim cRec As BO.p56Task = Factory.p56TaskBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : intP41ID = cRec.p41ID : intRecOwnerID = cRec.j02ID_Owner : strRecUserInsert = cRec.UserInsert
            
            Case "p28"
                Dim cRec As BO.p28Contact = Factory.p28ContactBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : intRecOwnerID = cRec.j02ID_Owner : strRecUserInsert = cRec.UserInsert
            Case "p91"
                Dim cRec As BO.p91Invoice = Factory.p91InvoiceBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : intRecOwnerID = cRec.j02ID_Owner : strRecUserInsert = cRec.UserInsert
            Case "o23"
                Dim cRec As BO.o23Doc = Factory.o23DocBL.Load(intRecordPID)
                intCurB02ID = cRec.b02ID : intRecOwnerID = cRec.j02ID_Owner : strRecUserInsert = cRec.UserInsert
        End Select

        Dim lisB06 As IEnumerable(Of BO.b06WorkflowStep) = GetList(0).Where(Function(p As BO.b06WorkflowStep) p.b06IsManualStep = True And p.b02ID = intCurB02ID)

        Dim lisX69 As List(Of BO.x69EntityRole_Assign) = Factory.x67EntityRoleBL.GetList_x69(x29id, intRecordPID).ToList
        If strRecordPrefix = "p56" And intP41ID <> 0 Then
            'ještě doplnit projektové role
            Dim lisX69_p41 As IEnumerable(Of BO.x69EntityRole_Assign) = Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, intP41ID)
            For Each c In lisX69_p41
                lisX69.Add(c)
            Next
        End If
        For Each cB06 As BO.b06WorkflowStep In lisB06
            Dim lisB08 As IEnumerable(Of BO.b08WorkflowReceiverToStep) = Factory.b06WorkflowStepBL.GetList_B08(cB06.PID)
            Dim bolOK As Boolean = False
            If lisB08.Where(Function(p) p.j04ID = Factory.SysUser.j04ID).Count > 0 Then bolOK = True
            If strRecUserInsert = Factory.SysUser.j03Login Then
                If lisB08.Where(Function(p) p.b08IsRecordCreator = True).Count > 0 Then bolOK = True 'zakladatel záznamu
            End If
            If intRecOwnerID = Factory.SysUser.j02ID Then
                If lisB08.Where(Function(p) p.b08IsRecordOwner = True).Count > 0 Then bolOK = True 'vlastník záznamu
            End If
            For Each c In lisX69
                If lisB08.Where(Function(p) p.x67ID = c.x67ID And c.j02ID <> 0 And c.j02ID = curPerson.PID).Count > 0 Then bolOK = True
                If lisB08.Where(Function(p) p.x67ID = c.x67ID And c.j07ID <> 0 And c.j07ID = curPerson.j07ID).Count > 0 Then bolOK = True
                For Each cc In lisPersonJ11
                    If lisB08.Where(Function(p) p.x67ID = c.x67ID And c.j11ID <> 0 And c.j11ID = cc.PID).Count > 0 Then bolOK = True
                Next
            Next
            For Each cc In lisPersonJ11
                If lisB08.Where(Function(p) p.j11ID <> 0 And p.j11ID = cc.PID).Count > 0 Then bolOK = True
            Next

            If bolOK Then
                Dim c As New BO.WorkflowStepPossible4User
                c.b06ID = cB06.PID
                c.b06Name = cB06.b06Name

                Dim strName As String = cB06.b06Name
                If cB06.b02ID_Target <> 0 Then
                    strName += "->" & cB06.TargetStatus
                End If
                c.RadioListText = strName

                ret.Add(c)
            End If
        Next
        Return ret
    End Function

    Public Function GetAutoWorkflowSQLResult(intRecordPID As Integer, cB06 As BO.b06WorkflowStep) As Integer Implements Ib06WorkflowStepBL.GetAutoWorkflowSQLResult
        Return _cDL.GetAutoWorkflowSQLResult(intRecordPID, cB06)
    End Function
End Class
