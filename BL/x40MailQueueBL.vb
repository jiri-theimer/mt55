'Imports System.Net.Mail
Imports Rebex.Net
Imports Rebex.Mail

Public Interface Ix40MailQueueBL
    Inherits IFMother

    Function SaveMessageToQueque(message As MailMessage, recipients As List(Of BO.x43MailQueue_Recipient), x29id As BO.x29IdEnum, intRecordPID As Integer, status As BO.x40StateENUM, intExplicitO40ID As Integer) As Integer
    Function SendMessageFromQueque(cRec As BO.x40MailQueue) As Boolean
    Function SendMessageFromQueque(intX40ID As Integer) As Boolean
    Function UpdateMessageState(intX40ID As Integer, NewState As BO.x40StateENUM) As Boolean
    Function Load(intPID As Integer) As BO.x40MailQueue
    Function LoadByMessageID(strMessageID As String) As BO.x40MailQueue
    Function LoadByEntity(intRecordPID As Integer, x29ID As BO.x29IdEnum) As BO.x40MailQueue
    Function Delete(intPID As Integer) As Boolean
    Function GetList(myQuery As BO.myQueryX40) As IEnumerable(Of BO.x40MailQueue)
    ''Function GetList_AllHisMessages(intJ03ID_Sender As Integer, intJ02ID_Person As Integer, Optional intTopRecs As Integer = 500) As IEnumerable(Of BO.x40MailQueue)
    'Function SendMessageWithoutQueque(strRecipient As String, strBody As String, strSubject As String) As Boolean
    Function CompleteMailAttachments(ByRef message As MailMessage, strUploadGUID As String) As String
    Function SendAnswer2Ticket(strBody As String, cRec2Answer As BO.b07Comment) As Boolean
    Function SendAnswer2Ticket(strBody As String, intO43ID As Integer, x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean
    Function SendAnswer2Ticket(strBody As String, x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean
    Function CreateRecipients(strEmail As String, strDisplayName As String, Optional side As BO.x43RecipientIdEnum = BO.x43RecipientIdEnum.recTO) As List(Of BO.x43MailQueue_Recipient)
    Sub AppendRecipient(ByRef lis2Append As List(Of BO.x43MailQueue_Recipient), strEmail As String, strDisplayName As String, Optional side As BO.x43RecipientIdEnum = BO.x43RecipientIdEnum.recTO)
    Function ForwardMessageToQueue(strSubject As String, strBody As String, recipients As List(Of BO.x43MailQueue_Recipient), intO43ID As Integer, x29id As BO.x29IdEnum, intRecordPID As Integer) As Integer
    Function TestConnect(strSmtpServer As String, strSmtpLogin As String, strSmtpPassword As String, intPort As Integer, sslM As BO.SslModeENUM, smtpSpecAuth As BO.smtpAuthenticationENUM) As Boolean
End Interface

Class x40MailQueueBL
    Inherits BLMother
    Implements Ix40MailQueueBL
    Private WithEvents _cDL As DL.x40MailQueueDL
    
    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x40MailQueueDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Load(intPID As Integer) As BO.x40MailQueue Implements Ix40MailQueueBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByMessageID(strMessageID As String) As BO.x40MailQueue Implements Ix40MailQueueBL.LoadByMessageID
        Return _cDL.LoadByMessageID(strMessageID)
    End Function
    Public Function LoadByEntity(intRecordPID As Integer, x29ID As BO.x29IdEnum) As BO.x40MailQueue Implements Ix40MailQueueBL.LoadByEntity
        Return _cDL.LoadByEntity(intRecordPID, x29ID)
    End Function

    Public Function Delete(intPID As Integer) As Boolean Implements Ix40MailQueueBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(myQuery As BO.myQueryX40) As IEnumerable(Of BO.x40MailQueue) Implements Ix40MailQueueBL.GetList
        Return _cDL.GetList(myQuery)
    End Function
    Public Function CompleteMailAttachments(ByRef message As MailMessage, strUploadGUID As String) As String Implements Ix40MailQueueBL.CompleteMailAttachments
        If strUploadGUID = "" Then Return ""
        Dim lis As New List(Of String)
        Dim lisTempUpload As IEnumerable(Of BO.p85TempBox) = Me.Factory.p85TempBoxBL.GetList(strUploadGUID), strTempDir As String = Me.Factory.x35GlobalParam.TempFolder
        For Each cTMP In lisTempUpload
            If message.Attachments.Where(Function(p) p.FileName = strTempDir & "\" & cTMP.p85FreeText02).Count = 0 Then
                Dim att As New Rebex.Mail.Attachment(strTempDir & "\" & cTMP.p85FreeText02, cTMP.p85FreeText01)
                message.Attachments.Add(att)
                lis.Add(cTMP.p85FreeText01)
            End If
        Next
        Return String.Join(", ", lis)

        'If Me.Factory.o27AttachmentBL.UploadAndSaveUserControl(lisTempUpload, BO.x29IdEnum.x40MailQueue, cX40.PID) Then

        'End If
    End Function
    Public Function SaveMessageToQueue(mes As MailMessage, recipients As List(Of BO.x43MailQueue_Recipient), x29id As BO.x29IdEnum, intRecordPID As Integer, status As BO.x40StateENUM, intExplicitO40ID As Integer) As Integer Implements Ix40MailQueueBL.SaveMessageToQueque
        _Error = ""
        If recipients Is Nothing Then recipients = New List(Of BO.x43MailQueue_Recipient)
        If recipients.Count = 0 Then
            _Error = "Poštovní zpráva musí mít minimálně jednoho příjemce." : Return 0
        End If
        With mes
            .DefaultCharset = System.Text.Encoding.UTF8
            .Headers.Add("marktime-prefix", BO.BAS.GetDataPrefix(x29id))
            .Headers.Add("marktime-pid", intRecordPID.ToString)

            If Trim(.Subject) = "" And Trim(.BodyText) = "" And Trim(.BodyHtml) = "" Then
                _Error = "Předmět zprávy i text zprávy jsou prázdné." : Return 0
            End If
            If Not String.IsNullOrEmpty(.BodyText) Then
                If .BodyText.IndexOf("--") > 0 Then
                    .BodyText = Replace(.BodyText, "[!--", "<!--")
                    .BodyText = Replace(.BodyText, "--]", "-->")
                End If
            End If
            If .From.Count = 0 Then
                .From.Add(New Rebex.Mime.Headers.MailAddress(Me.Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress"), Me.Factory.SysUser.Person & " via MARKTIME"))
            End If
        End With

        Dim cX40 As New BO.x40MailQueue()
        With cX40
            .o40ID = intExplicitO40ID
            .x40State = status
            .x29ID = x29id
            .x40RecordPID = intRecordPID
            If mes.HasBodyHtml Then
                .x40Body = mes.BodyHtml
            Else
                .x40Body = mes.BodyText
            End If
            .x40IsHtmlBody = mes.HasBodyHtml
            .x40Subject = mes.Subject
            If mes.From.Count > 0 Then
                .x40SenderName = mes.From(0).DisplayName
                .x40SenderAddress = mes.From(0).Address
            End If

            .j03ID_Sys = _cUser.PID
        End With
        'nejdříve uložit x40 záznam do databáze
        If _cDL.Save(cX40, recipients) Then
            cX40 = _cDL.Load(_cDL.LastSavedRecordPID)
        Else
            _Error = _cDL.ErrorMessage
            Return Nothing
        End If
        For Each att In mes.Attachments
            Dim s As String = att.DisplayName
            If s = "" Then s = att.FileName
            If cX40.x40Attachments = "" Then
                cX40.x40Attachments = s
            Else
                cX40.x40Attachments += ", " & s
            End If
        Next

        For Each c In recipients
            Select Case c.x43RecipientFlag
                Case BO.x43RecipientIdEnum.recTO
                    mes.To.Add(c.x43Email)
                    cX40.x40Recipient += ", " & c.x43Email
                Case BO.x43RecipientIdEnum.recCC
                    mes.CC.Add(c.x43Email)
                    cX40.x40CC += ", " & c.x43Email
                Case BO.x43RecipientIdEnum.recBCC
                    mes.Bcc.Add(c.x43Email)
                    cX40.x40BCC += ", " & c.x43Email
            End Select

        Next

        'uložit mailmessage do souboru
        cX40.x40ArchiveFolder = "x40\" & Year(Now).ToString & "\" & Right("0" & Month(Now).ToString, 2)
        cX40.x40MessageID = mes.MessageId.Id
        Dim strDir As String = Me.Factory.x35GlobalParam.UploadFolder & "\" & cX40.x40ArchiveFolder
        If Not System.IO.Directory.Exists(strDir) Then
            System.IO.Directory.CreateDirectory(strDir)
        End If
        mes.Save(strDir & "\" & mes.MessageId.Id & ".eml", MailFormat.Mime)

        With cX40
            .x40Recipient = BO.BAS.OM1(.x40Recipient)
            .x40BCC = BO.BAS.OM1(.x40BCC)
            .x40CC = BO.BAS.OM1(.x40CC)
        End With

        If _cDL.Save(cX40, Nothing) Then
            Return _cDL.LastSavedRecordPID
        Else
            Return 0
        End If

    End Function


    ''Public Function SendMessageWithoutQueque(strRecipient As String, strBody As String, strSubject As String) As Boolean Implements Ix40MailQueueBL.SendMessageWithoutQueque
    ''    'Dim mail As MailMessage = New MailMessage()
    ''    Dim mail As New MailMessage

    ''    With mail
    ''        .DefaultCharset = System.Text.Encoding.UTF8
    ''        .BodyText = strBody
    ''        .Subject = strSubject
    ''        .To = strRecipient
    ''    End With

    ''    Dim strIsUseWebConfigSetting As String = Me.Factory.x35GlobalParam.GetValueString("IsUseWebConfigSetting", "1")
    ''    If strIsUseWebConfigSetting = "0" Then
    ''        'odeslat podle nastavení mimo web.config
    ''        Dim smtp As New Smtp
    ''        With smtp
    ''            Try
    ''                .Connect(Me.Factory.x35GlobalParam.GetValueString("SMTP_Server"))
    ''                .Login(Me.Factory.x35GlobalParam.GetValueString("SMTP_Login"), Me.Factory.x35GlobalParam.GetValueString("SMTP_Password"))
    ''                .Send(mail)
    ''                Return True
    ''            Catch ex As Exception
    ''                _Error = ex.Message
    ''                Return False
    ''            End Try

    ''        End With

    ''    Else
    ''        'odeslat podle web.config nastavení
    ''        Try
    ''            Smtp.Send(mail, SmtpConfiguration.Default)
    ''            Return True
    ''        Catch ex As Exception
    ''            _Error = ex.Message
    ''            Return False
    ''        End Try

    ''    End If
    ''End Function
    Public Overloads Function SendMessageFromQueque(intX40ID As Integer) As Boolean Implements Ix40MailQueueBL.SendMessageFromQueque
        Dim cRec As BO.x40MailQueue = Load(intX40ID)
        If cRec Is Nothing Then Return False
        Return SendMessageFromQueque(cRec)
    End Function

    Private Function InhaleMailMessageFromDb(cRec As BO.x40MailQueue) As MailMessage
        Dim mail As New MailMessage
        Dim recipients As IEnumerable(Of BO.x43MailQueue_Recipient) = _cDL.GetList_Recipients(cRec.PID)

        Dim strGlobalSenderAddress As String = Me.Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress")
        Dim strSenderIsUser As String = Me.Factory.x35GlobalParam.GetValueString("SMTP_SenderIsUser")
        Dim strSenderName As String = cRec.x40SenderName
        If strSenderName <> "" Then
            If strSenderName.IndexOf(" via ") < 0 Then
                strSenderName += " via MARKTIME"
            End If
        End If
        
        With mail
            .DefaultCharset = System.Text.Encoding.UTF8
            If strSenderIsUser = "1" Then
                .From.Add(New Rebex.Mime.Headers.MailAddress(cRec.x40SenderAddress, strSenderName))
            Else
                .From.Add(New Rebex.Mime.Headers.MailAddress(strGlobalSenderAddress, strSenderName))
            End If
            If cRec.x40IsHtmlBody Then
                .BodyHtml = cRec.x40Body
            Else
                .BodyText = cRec.x40Body
            End If
            .Subject = cRec.x40Subject
        End With

        For Each c In recipients.Where(Function(p) p.x43Email <> "")
            Try
                Select Case c.x43RecipientFlag
                    Case BO.x43RecipientIdEnum.recTO
                        mail.To.Add(New Rebex.Mime.Headers.MailAddress(c.x43Email, c.x43DisplayName))

                    Case BO.x43RecipientIdEnum.recBCC
                        mail.Bcc.Add(New Rebex.Mime.Headers.MailAddress(c.x43Email, c.x43DisplayName))
                    Case BO.x43RecipientIdEnum.recCC
                        mail.CC.Add(New Rebex.Mime.Headers.MailAddress(c.x43Email, c.x43DisplayName))
                End Select
            Catch ex As Exception
                'chyba v mail adrese příjemce
            End Try

        Next
        Dim mqO27 As New BO.myQueryO27
        mqO27.x40ID = cRec.PID
        Dim lisO27 As IEnumerable(Of BO.o27Attachment) = Factory.o27AttachmentBL.GetList(mqO27)
        If lisO27.Count > 0 Then
            Dim strRootFolder As String = Me.Factory.x35GlobalParam.UploadFolder
            For Each cO27 In lisO27
                Dim strPath As String = cO27.GetFullPath(strRootFolder)

                Dim att As New Attachment(strPath)
                att.ContentDisposition.FileName = cO27.o27OriginalFileName

                mail.Attachments.Add(att)
                If cO27.o27OriginalFileName.IndexOf(".ics") > 0 Then
                    'Dim mimeType As System.Net.Mime.ContentType = New System.Net.Mime.ContentType("text/calendar; method=REQUEST")
                    'Dim icalView As New Rebex.Mail.AlternateView(strPath, mimeType)
                    Dim icalView As New AlternateView(strPath)

                    icalView.SetContentFromFile(strPath, "text/calendar")

                    icalView.TransferEncoding = Net.Mime.TransferEncoding.SevenBit
                    mail.AlternateViews.Add(icalView)
                End If
            Next
        End If

        Return mail
    End Function
    Public Overloads Function SendMessageFromQueque(cRec As BO.x40MailQueue) As Boolean Implements Ix40MailQueueBL.SendMessageFromQueque
        _Error = ""
        Dim mail As New MailMessage
        If cRec.x40MessageID <> "" And cRec.x40ArchiveFolder <> "" Then
            'načíst mail z EML souboru
            If System.IO.File.Exists(Me.Factory.x35GlobalParam.UploadFolder & "\" & cRec.x40ArchiveFolder & "\" & cRec.x40MessageID & ".eml") Then
                mail.Load(Me.Factory.x35GlobalParam.UploadFolder & "\" & cRec.x40ArchiveFolder & "\" & cRec.x40MessageID & ".eml")
            End If
        End If
        If mail.BodyText = "" And mail.BodyHtml <> "" And mail.Subject = "" Then
            mail = InhaleMailMessageFromDb(cRec)    'zpráva načíst z databáze
        End If

        
        Dim bolSucceeded As Boolean = False, bolUseWebConfig As Boolean = True, strSmtpServer As String = Me.Factory.x35GlobalParam.GetValueString("SMTP_Server"), strSmtpLogin As String = "", strSmtpPassword As String = "", intPort As Integer = 0, sslM As BO.SslModeENUM = BO.SslModeENUM._NoSSL
        Dim smtpSpecAuth As BO.smtpAuthenticationENUM = BO.smtpAuthenticationENUM._Auto, bolPersonalSMTP As Boolean = False
        If cRec.o40ID = 0 Then  'na vstupu již může být předán jiný SMTP účet
            If Me.Factory.x35GlobalParam.GetValueString("IsUseWebConfigSetting", "1") = "0" And BO.BAS.IsNullInt(strSmtpServer) <> 0 Then
                cRec.o40ID = BO.BAS.IsNullInt(strSmtpServer)
            End If
            If _cUser.j02ID <> 0 Then
                Dim cPerson As BO.j02Person = Me.Factory.j02PersonBL.Load(_cUser.j02ID)
                If cPerson.o40ID <> 0 Then
                    'osoba má vlastní SMTP účet
                    cRec.o40ID = cPerson.o40ID : bolPersonalSMTP = True
                End If
            End If
        End If
        
        If cRec.o40ID <> 0 Then
            bolUseWebConfig = False
            Dim cO40 As BO.o40SmtpAccount = Me.Factory.o40SmtpAccountBL.Load(cRec.o40ID)

            strSmtpServer = cO40.o40Server
            intPort = BO.BAS.IsNullInt(cO40.o40Port)
            sslM = cO40.o40SslModeFlag
            smtpSpecAuth = cO40.o40SmtpAuthentication

            If cO40.o40IsVerify Then
                strSmtpLogin = cO40.o40Login
                strSmtpPassword = cO40.DecryptedPassword()
            End If
            cRec.x40SenderAddress = cO40.o40EmailAddress
            If bolPersonalSMTP Then
                cRec.x40SenderName = cO40.o40Name   'odesílá se z uživatelovo mail účtu
            Else
                cRec.x40SenderName = _cUser.Person & " via MARKTIME"  'odesílá se z centrálního smtp účtu
            End If


            mail.From.Clear()
            mail.From.Add(New Rebex.Mime.Headers.MailAddress(cO40.o40EmailAddress, cO40.o40Name))
        End If
        ''Dim credentials As GssApiProvider = Rebex.Net.GssApiProvider.GetSspiProvider("Ntlm", Nothing, Nothing, Nothing, Nothing)

        If Not bolUseWebConfig Then
            Dim smtp As New Smtp
            With smtp
                Try
                    Select Case sslM
                        Case BO.SslModeENUM._NoSSL
                            If intPort = 0 Then
                                .Connect(strSmtpServer)
                            Else
                                .Connect(strSmtpServer, intPort)
                            End If
                        Case BO.SslModeENUM.Implicit
                            If intPort = 0 Then
                                .Connect(strSmtpServer, Rebex.Net.SslMode.Implicit)
                            Else
                                .Connect(strSmtpServer, intPort, Rebex.Net.SslMode.Implicit)
                            End If
                        Case BO.SslModeENUM.Explicit
                            If intPort = 0 Then
                                .Connect(strSmtpServer, Rebex.Net.SslMode.Explicit)
                            Else
                                .Connect(strSmtpServer, intPort, Rebex.Net.SslMode.Explicit)
                            End If
                    End Select
                    If strSmtpLogin <> "" And strSmtpPassword <> "" Then
                        ''Select Case smtpSpecAuth
                        ''    Case BO.smtpAuthenticationENUM.Ntlm
                        ''        Dim credentials As GssApiProvider = Rebex.Net.GssApiProvider.GetSspiProvider("Ntlm", Nothing, strSmtpLogin, strSmtpPassword, Nothing)
                        ''        .Login(credentials)
                        ''    Case BO.smtpAuthenticationENUM._Auto
                        ''        .Login(strSmtpLogin, strSmtpPassword)
                        ''    Case Else
                        ''        .Login(strSmtpLogin, strSmtpPassword, CType(smtpSpecAuth, SmtpAuthentication))

                        ''End Select
                        If smtpSpecAuth = BO.smtpAuthenticationENUM._Auto Then
                            .Login(strSmtpLogin, strSmtpPassword)
                        Else
                            .Login(strSmtpLogin, strSmtpPassword, CType(smtpSpecAuth, SmtpAuthentication))
                        End If
                    Else
                        ''Select Case smtpSpecAuth
                        ''    Case BO.smtpAuthenticationENUM.Ntlm
                        ''        Dim credentials As GssApiProvider = Rebex.Net.GssApiProvider.GetSspiProvider("Ntlm", Nothing, Nothing, Nothing, Nothing)
                        ''        .Login(credentials)
                        ''    Case BO.smtpAuthenticationENUM._Auto
                        ''        'žádný login
                        ''    Case Else
                        ''        .Login(CType(smtpSpecAuth, SmtpAuthentication))

                        ''End Select
                        If smtpSpecAuth = BO.smtpAuthenticationENUM._Auto Then
                            'žádný login
                        Else
                            .Login(CType(smtpSpecAuth, SmtpAuthentication))
                        End If
                    End If
                    .Send(mail)
                    .Disconnect()
                    bolSucceeded = True
                Catch ex As Exception
                    _Error = ex.Message
                    bolSucceeded = False
                End Try

            End With
        Else
            Try
                Smtp.Send(mail, SmtpConfiguration.Default)
                bolSucceeded = True
            Catch ex As Exception
                _Error = ex.Message
                bolSucceeded = False
            End Try
        End If
        If bolSucceeded Then
            log4net.LogManager.GetLogger("smtplog").Info("Sender: " & mail.From(0).Address & vbCrLf & "Message recipients: " & cRec.x40Recipient & vbCrLf & "Message subject: " & cRec.x40Subject & vbCrLf & "Message body: " & cRec.x40Body)

            For i As Integer = 0 To mail.Attachments.Count - 1
                Try
                    log4net.LogManager.GetLogger("smtplog").Info("Message attachment: " & mail.Attachments(i).ContentDisposition.FileName)
                Catch ex As Exception

                End Try

            Next
            cRec.x40State = BO.x40StateENUM.IsProceeded
            cRec.x40WhenProceeded = Now
            cRec.x40ErrorMessage = ""
        Else
            bolSucceeded = False
            cRec.x40State = BO.x40StateENUM.IsError
            cRec.x40ErrorMessage = _Error
            log4net.LogManager.GetLogger("smtplog").Error("Error: " & _Error & vbCrLf & "Sender: " & mail.From(0).Address & "/" & vbCrLf & "Message recipients: " & cRec.x40Recipient & vbCrLf & "Message subject: " & cRec.x40Subject & vbCrLf & "Message body: " & cRec.x40Body)

        End If

        _cDL.Save(cRec, Nothing)

        Return bolSucceeded

    End Function
    Public Function UpdateMessageState(intX40ID As Integer, NewState As BO.x40StateENUM) As Boolean Implements Ix40MailQueueBL.UpdateMessageState
        If intX40ID = 0 Then _Error = "ID zprávy je nula." : Return False
        If Not (NewState = BO.x40StateENUM.InQueque Or NewState = BO.x40StateENUM.IsStopped Or NewState = BO.x40StateENUM.WaitOnConfirm) Then
            _Error = "Změnit stav lze pouze na [Zastaveno], [Mail fronta] nebo [Čeká na potvrzení]." : Return False
        End If
        Return _cDL.UpdateMessageState(intX40ID, NewState)
    End Function

    Function ForwardMessageToQueue(strSubject As String, strBody As String, recipients As List(Of BO.x43MailQueue_Recipient), intO43ID As Integer, x29id As BO.x29IdEnum, intRecordPID As Integer) As Integer Implements Ix40MailQueueBL.ForwardMessageToQueue
        'fce vrací x40ID
        _Error = ""
        If intO43ID = 0 Then Return 0
        Dim original As Rebex.Mail.MailMessage = Me.Factory.o42ImapRuleBL.LoadMailMessageFromHistory(intO43ID)
        If original Is Nothing Then Return 0

        Dim forward As New MailMessage()
        If strSubject = "" Then
            forward.Subject = "FW: " & original.Subject
        Else
            forward.Subject = strSubject
        End If

        forward.BodyText = strBody
        'and add the original e-mail as an attachment
        forward.Attachments.Add(New Attachment(original))

        Return SaveMessageToQueue(forward, recipients, x29id, intRecordPID, BO.x40StateENUM.InQueque, 0)

    End Function
    Function SendAnswer2Ticket(strBody As String, intO43ID As Integer, x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean Implements Ix40MailQueueBL.SendAnswer2Ticket
        If intO43ID = 0 Then
            _Error = "Nelze najít zprávu, na kterou poslat odpověď (o43id)"
            Return False
        End If
        Dim original As Rebex.Mail.MailMessage = Me.Factory.o42ImapRuleBL.LoadMailMessageFromHistory(intO43ID)
        Dim reply As New Rebex.Mail.MailMessage
        If Not (original.MessageId Is Nothing) Then
            reply.InReplyTo.Add(original.MessageId)
        End If
        reply.Headers.Add("marktime-prefix", BO.BAS.GetDataPrefix(x29id))
        reply.Headers.Add("marktime-pid", intRecordPID.ToString)
        Dim intB01ID As Integer = 0, intO40ID As Integer = 0
        If x29id = BO.x29IdEnum.p56Task Then
            intB01ID = Me.Factory.p56TaskBL.Load(intRecordPID).b01ID
        End If
        If intB01ID <> 0 Then
            intO40ID = Me.Factory.b01WorkflowTemplateBL.Load(intB01ID).o40ID
        End If


        reply.To = original.From
        reply.CC = original.CC
        reply.MessageId = New Rebex.Mime.Headers.MessageId
        reply.Subject = "RE: " & original.Subject
        reply.BodyText = strBody

        Dim recipients As List(Of BO.x43MailQueue_Recipient) = CreateRecipients(original.From(0).Address, original.From(0).DisplayName)

        Dim intX40ID As Integer = SaveMessageToQueue(reply, recipients, x29id, intRecordPID, BO.x40StateENUM.InQueque, intO40ID)
        Return SendMessageFromQueque(intX40ID)
    End Function
    Function CreateRecipients(strEmail As String, strDisplayName As String, Optional side As BO.x43RecipientIdEnum = BO.x43RecipientIdEnum.recTO) As List(Of BO.x43MailQueue_Recipient) Implements Ix40MailQueueBL.CreateRecipients
        Dim recipients As New List(Of BO.x43MailQueue_Recipient)
        Dim cR As New BO.x43MailQueue_Recipient
        cR.x43Email = strEmail
        cR.x43DisplayName = strDisplayName
        cR.x43RecipientFlag = side
        recipients.Add(cR)
        Return recipients
    End Function
    Sub AppendRecipient(ByRef lis2Append As List(Of BO.x43MailQueue_Recipient), strEmail As String, strDisplayName As String, Optional side As BO.x43RecipientIdEnum = BO.x43RecipientIdEnum.recTO) Implements Ix40MailQueueBL.AppendRecipient
        Dim c As New BO.x43MailQueue_Recipient
        c.x43Email = strEmail
        c.x43DisplayName = strDisplayName
        c.x43RecipientFlag = side
        lis2Append.Add(c)
    End Sub
    Function SendAnswer2Ticket(strBody As String, x29id As BO.x29IdEnum, intRecordPID As Integer) As Boolean Implements Ix40MailQueueBL.SendAnswer2Ticket
        Dim mq As New BO.myQueryB07 'najít poslední odpověď žadatele načtenou přes IMAP
        mq.x29id = x29id
        mq.RecordDataPID = intRecordPID
        Dim lis As IEnumerable(Of BO.b07Comment) = Factory.b07CommentBL.GetList(mq).Where(Function(p) p.o43ID <> 0).OrderByDescending(Function(p) p.o43ID)
        Dim intO43ID As Integer = 0
        If lis.Count > 0 Then
            Return SendAnswer2Ticket(strBody, lis(0))   'nalezena poslední odpověď
        Else
            Select Case x29id
                Case BO.x29IdEnum.p56Task
                    intO43ID = Factory.p56TaskBL.Load(intRecordPID).o43ID
                Case BO.x29IdEnum.o23Doc
                    intO43ID = Factory.o23DocBL.Load(intRecordPID).o43ID
                Case Else
                    Return False
            End Select
            Return SendAnswer2Ticket(strBody, intO43ID, x29id, intRecordPID)
        End If

    End Function
    Function SendAnswer2Ticket(strBody As String, cRec2Answer As BO.b07Comment) As Boolean Implements Ix40MailQueueBL.SendAnswer2Ticket
        Return SendAnswer2Ticket(strBody, cRec2Answer.o43ID, cRec2Answer.x29ID, cRec2Answer.b07RecordPID)
    End Function

    Function TestConnect(strSmtpServer As String, strSmtpLogin As String, strSmtpPassword As String, intPort As Integer, sslM As BO.SslModeENUM, smtpSpecAuth As BO.smtpAuthenticationENUM) As Boolean Implements Ix40MailQueueBL.TestConnect
        Dim smtp As New Smtp
        With smtp
            Try
                Select Case sslM
                    Case BO.SslModeENUM._NoSSL
                        If intPort = 0 Then
                            .Connect(strSmtpServer)
                        Else
                            .Connect(strSmtpServer, intPort)
                        End If
                    Case BO.SslModeENUM.Implicit
                        .Settings.SslAcceptAllCertificates = True
                        If intPort = 0 Then
                            .Connect(strSmtpServer, SslMode.Implicit)
                        Else
                            .Connect(strSmtpServer, intPort, SslMode.Implicit)
                        End If
                    Case BO.SslModeENUM.Explicit
                        .Settings.SslAcceptAllCertificates = True
                        If intPort = 0 Then
                            .Connect(strSmtpServer, SslMode.Explicit)
                        Else
                            .Connect(strSmtpServer, intPort, SslMode.Explicit)
                        End If
                End Select

                If strSmtpLogin <> "" And strSmtpPassword <> "" Then
                    If smtpSpecAuth = BO.smtpAuthenticationENUM._Auto Then
                        .Login(strSmtpLogin, strSmtpPassword)
                    Else
                        .Login(strSmtpLogin, strSmtpPassword, CType(smtpSpecAuth, SmtpAuthentication))
                    End If
                Else
                    If smtpSpecAuth > BO.smtpAuthenticationENUM._Auto Then
                        .Login(CType(smtpSpecAuth, SmtpAuthentication))
                    End If
                End If

                .Disconnect()
                Return True
            Catch ex As Exception
                _Error = ex.Message
                Return False
            End Try

        End With
    End Function
End Class
