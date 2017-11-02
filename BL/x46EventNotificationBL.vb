Public Interface Ix46EventNotificationBL
    Inherits IFMother
    Function Save(cRec As BO.x46EventNotification) As Boolean
    Function Load(intPID As Integer) As BO.x46EventNotification
    Function Delete(intPID As Integer) As Boolean
    Function GetList(myQuery As BO.myQuery, Optional intJ02ID As Integer = 0, Optional intX45ID As Integer = 0) As IEnumerable(Of BO.x46EventNotification)
    Sub GenerateNotifyMessages(cX47 As BO.x47EventLog, Optional lisExplicitNotifyReceivers As List(Of BO.PersonOrTeam) = Nothing)

End Interface

Class x46EventNotificationBL
    Inherits BLMother
    Implements Ix46EventNotificationBL
    Private WithEvents _cDL As DL.x46EventNotificationDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x46EventNotificationDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x46EventNotification) As Boolean Implements Ix46EventNotificationBL.Save
        With cRec
            Dim cEvent As BO.x45Event = Me.Factory.ftBL.LoadX45(.x45ID)

            If Not .x46IsUseSystemTemplate And Len(Trim(.x46MessageTemplate)) < 10 Then
                _Error = "Rozsah notifikační zprávy je nedostatečný." : Return False
            End If
            If .x46IsUseSystemTemplate Then
                If Trim(cEvent.x45MessageTemplate) = "" Then
                    _Error = "Událost nemá definovaný výchozí text notifikační zprávy. Obsah zprávy proto musíte napsat ručně."
                    Return False
                End If
            End If
            If Trim(.x46MessageSubject) = "" Then
                _Error = "Chybí předmět zprávy." : Return False
            End If
            If .x46IsForAllRoles Then .x67ID = 0
            If .x46IsForAllReferenceRoles Then .x67ID_Reference = 0
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.x46EventNotification Implements Ix46EventNotificationBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix46EventNotificationBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(myQuery As BO.myQuery, Optional intJ02ID As Integer = 0, Optional intX45ID As Integer = 0) As IEnumerable(Of BO.x46EventNotification) Implements Ix46EventNotificationBL.GetList
        Return _cDL.GetList(myQuery, intJ02ID, intX45ID)
    End Function

    Public Sub GenerateNotifyMessages(cX47 As BO.x47EventLog, Optional lisExplicitNotifyReceivers As List(Of BO.PersonOrTeam) = Nothing) Implements Ix46EventNotificationBL.GenerateNotifyMessages
        Dim lisX46 As IEnumerable(Of BO.x46EventNotification) = GetList(New BO.myQuery, , CInt(cX47.x45ID))
        If cX47.x29ID_Reference > BO.x29IdEnum._NotSpecified Then
            'odfiltrovat notifikace pouze na ty, které se týkají refereční entity nebo všech referencí
            lisX46 = lisX46.Where(Function(p) p.x29ID_Reference = cX47.x29ID_Reference Or p.x29ID_Reference = BO.x29IdEnum._NotSpecified)
        End If
        If lisX46.Count = 0 Then
            Return  'k události nejsou nadefinovány žádné notifikace
        End If
        Dim cX45 As BO.x45Event = Me.Factory.ftBL.LoadX45(CInt(cX47.x45ID))
        Dim objects As New List(Of Object), intJ02ID_Owner As Integer = 0, intJ02ID_Owner_Reference As Integer = 0, objectReference As Object = Nothing
        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Factory.x67EntityRoleBL.GetList_x69(cX47.x29ID, cX47.x47RecordPID)
        Dim mrs As New List(Of BO.PersonOrTeam)
        If Not lisExplicitNotifyReceivers Is Nothing Then
            mrs = lisExplicitNotifyReceivers    'příjemci notifikace jsou ručně předáni
        End If

        Dim cRoles As New BO.Roles4Notification

        Select Case cX45.x29ID
            Case BO.x29IdEnum.p41Project
                Dim cRec As BO.p41Project = Factory.p41ProjectBL.Load(cX47.x47RecordPID)            
                intJ02ID_Owner = cRec.j02ID_Owner
                objects.Add(cRec)
                If cX47.x45ID = BO.x45IDEnum.p41_limitfee_over Or cX47.x45ID = BO.x45IDEnum.p41_limithours_over Then
                    Dim mq As New BO.myQueryP31
                    mq.p41ID = cX47.x47RecordPID
                    objects.Add(Factory.p31WorksheetBL.LoadSumRow(mq, True, True))
                End If
                cRoles.RolesInLine = Factory.p41ProjectBL.GetRolesInline(cRec.PID)

            Case BO.x29IdEnum.p56Task
                Dim cRec As BO.p56Task = Factory.p56TaskBL.Load(cX47.x47RecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner
                objects.Add(cRec)
                cRoles.RolesInLine = Factory.p56TaskBL.GetRolesInline(cRec.PID)
            Case BO.x29IdEnum.p91Invoice
                Dim cRec As BO.p91Invoice = Factory.p91InvoiceBL.Load(cX47.x47RecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner
                objects.Add(cRec)
            Case BO.x29IdEnum.p28Contact
                Dim cRec As BO.p28Contact = Factory.p28ContactBL.Load(cX47.x47RecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner
                objects.Add(cRec)
                If cX47.x45ID = BO.x45IDEnum.p28_limitfee_over Or cX47.x45ID = BO.x45IDEnum.p28_limithours_over Then
                    Dim mq As New BO.myQueryP31
                    mq.p28ID_Client = cX47.x47RecordPID
                    objects.Add(Factory.p31WorksheetBL.LoadSumRow(mq, True, True))
                End If
                cRoles.RolesInLine = Factory.p28ContactBL.GetRolesInline(cRec.PID)
            Case BO.x29IdEnum.j02Person
                objects.Add(Factory.j02PersonBL.Load(cX47.x47RecordPID))
            Case BO.x29IdEnum.p51PriceList
                objects.Add(Factory.p51PriceListBL.Load(cX47.x47RecordPID))
                ''Case BO.x29IdEnum.b07Comment
                ''    Dim cRec As BO.b07Comment = Factory.b07CommentBL.Load(cX47.x47RecordPID)
                ''    intJ02ID_Owner = cRec.j02ID_Owner
                ''    objects.Add(cRec)
                ''    With cRec
                ''        If .x29ID = BO.x29IdEnum.p41Project Then objectReference = Factory.p41ProjectBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.p28Contact Then objectReference = Factory.p28ContactBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.p56Task Then objectReference = Factory.p56TaskBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.p91Invoice Then objectReference = Factory.p91InvoiceBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.o22Milestone Then objectReference = Factory.o22MilestoneBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.p31Worksheet Then objectReference = Factory.p31WorksheetBL.Load(.b07RecordPID)
                ''        If .x29ID = BO.x29IdEnum.o23Doc Then
                ''            Dim c As BO.o23Doc = Factory.o23DocBL.Load(.b07RecordPID)
                ''            If c.o23IsEncrypted Then
                ''                c.o23BigText = "Obsah je zašifrovaný."
                ''            End If
                ''            objectReference = c
                ''        End If
                ''    End With
            Case BO.x29IdEnum.o23Doc
                Dim cRec As BO.o23Doc = Factory.o23DocBL.Load(cX47.x47RecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner
                If cRec.o23IsEncrypted Then
                    cRec.o23BigText = "Obsah je zašifrovaný."
                End If
                objects.Add(cRec)
                cRoles.RolesInLine = Factory.o23DocBL.GetRolesInline(cRec.PID)
            Case BO.x29IdEnum.o22Milestone
                Dim cRec As BO.o22Milestone = Factory.o22MilestoneBL.Load(cX47.x47RecordPID)
                intJ02ID_Owner = cRec.j02ID_Owner
                objects.Add(cRec)
                With cRec
                    If .p41ID > 0 Then objectReference = Factory.p41ProjectBL.Load(.p41ID)
                    If .p28ID > 0 Then objectReference = Factory.p28ContactBL.Load(.p28ID)
                    If .j02ID > 0 Then objectReference = Factory.j02PersonBL.Load(.j02ID)
                    If .p56ID > 0 Then objectReference = Factory.p56TaskBL.Load(.p56ID)
                    If .p91ID > 0 Then objectReference = Factory.p91InvoiceBL.Load(.p91ID)
                End With
                If lisExplicitNotifyReceivers Is Nothing Then
                    'notifikovat všechny osoby k události
                    Dim lisO20 As IEnumerable(Of BO.o20Milestone_Receiver) = Factory.o22MilestoneBL.GetList_o20(cRec.PID)
                    For Each c In lisO20
                        mrs.Add(New BO.PersonOrTeam(c.j02ID, c.j11ID))
                    Next
                End If

            Case BO.x29IdEnum.p36LockPeriod
                objects.Add(Factory.p36LockPeriodBL.Load(cX47.x47RecordPID))
            Case Else

        End Select

        objects.Add(cRoles)

        If Not objectReference Is Nothing Then
            intJ02ID_Owner_Reference = objectReference.j02ID_Owner
            objects.Add(objectReference)
        End If

        Dim mes As New Rebex.Mail.MailMessage
        mes.From.Add(New Rebex.Mime.Headers.MailAddress(Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress"), "MARKTIME robot"))
        
        Dim strLinkUrl As String = Factory.GetRecordLinkUrl(BO.BAS.GetDataPrefix(cX45.x29ID), cX47.x47RecordPID)

        For Each c In lisX46
            Dim strMergedSubject As String = c.x46MessageSubject, strMergedBody As String = c.x46MessageTemplate
            If lisExplicitNotifyReceivers Is Nothing Then
                If c.x46IsForRecordOwner Then
                    mrs.Add(New BO.PersonOrTeam(intJ02ID_Owner, 0))
                End If
                If c.x46IsForAllRoles Then
                    For Each cRole In lisX69
                        mrs.Add(New BO.PersonOrTeam(cRole.j02ID, cRole.j11ID))
                    Next
                Else
                    If c.x67ID <> 0 Then
                        If lisX69.Where(Function(p) p.x67ID = c.x67ID).Count > 0 Then
                            Dim cRole As BO.x69EntityRole_Assign = lisX69.Where(Function(p) p.x67ID = c.x67ID)(0)
                            mrs.Add(New BO.PersonOrTeam(cRole.j02ID, cRole.j11ID))
                        End If
                    End If
                End If
                If c.j02ID > 0 Then
                    mrs.Add(New BO.PersonOrTeam(c.j02ID, 0))
                End If
                If c.j11ID > 0 Then
                    mrs.Add(New BO.PersonOrTeam(0, c.j11ID))
                End If
                If cX47.x29ID_Reference > BO.x29IdEnum._NotSpecified And cX47.x47RecordPID_Reference > 0 Then
                    'příjemcem zprávy může být i osoba se vztahem k referenční entitě
                    If c.x46IsForRecordOwner_Reference And intJ02ID_Owner_Reference > 0 Then
                        mrs.Add(New BO.PersonOrTeam(intJ02ID_Owner_Reference, 0))    'vlastník referenčního záznamu
                    End If
                End If
            End If
            

            If mrs.Count > 0 Then
                'existují příjemci události
             
                Dim j02ids As List(Of Integer) = mrs.Where(Function(p) p.j02ID <> 0).Select(Function(p) p.j02ID).ToList
                Dim j11ids As List(Of Integer) = mrs.Where(Function(p) p.j11ID <> 0).Select(Function(p) p.j11ID).ToList
                If c.x46IsExcludeAuthor Then    'vyloučit z příjemců zprávy autora události
                    If j02ids.Exists(Function(p) p = _cUser.j02ID) Then j02ids.Remove(_cUser.j02ID)
                End If
                Dim lisReceivers As List(Of BO.x43MailQueue_Recipient) = Factory.j02PersonBL.GetEmails_j02_join_j11(j02ids.ToList, j11ids.ToList)

                If lisReceivers.Count > 0 Then
                    'zkompletovat zprávu a odeslat do mail fronty
                    Dim cMerge As New BO.clsMergeContent
                    mes.BodyText = cMerge.MergeContent(objects, strMergedBody, strLinkUrl)
                    mes.Subject = strMergedSubject
                    If cX45.x29ID = BO.x29IdEnum.o22Milestone Then
                        'přiložit ICS soubor
                        Dim strICS As String = Factory.o22MilestoneBL.CreateICalendarTempFullPath(cX47.x47RecordPID)
                        If strICS <> "" Then
                            mes.Attachments.Add(New Rebex.Mail.Attachment(strICS))
                        End If
                    End If

                    CompleteMessages(cX47, mes, lisReceivers)
                End If

            End If
        Next

    End Sub

    
    Private Sub CompleteMessages(cX47 As BO.x47EventLog, message As Rebex.Mail.MailMessage, lisReceivers As List(Of BO.x43MailQueue_Recipient))
        Dim x29ID_Message As BO.x29IdEnum = cX47.x29ID, intRecordPID_Message As Integer = cX47.x47RecordPID
        If cX47.x29ID = BO.x29IdEnum.b07Comment Then
            x29ID_Message = cX47.x29ID_Reference
            intRecordPID_Message = cX47.x47RecordPID_Reference
        End If

        For Each cReceiver In lisReceivers
            'každému poslat zprávu individuálně
            Dim lisTo As New List(Of BO.x43MailQueue_Recipient)
            lisTo.Add(cReceiver)

            Factory.x40MailQueueBL.SaveMessageToQueque(message, lisTo, x29ID_Message, intRecordPID_Message, BO.x40StateENUM.InQueque, 0)
        Next
    End Sub
    

   

End Class
