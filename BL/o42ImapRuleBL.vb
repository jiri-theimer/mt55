Imports Rebex.Net
Imports Independentsoft.Msg

Public Interface Io42ImapRuleBL
    Inherits IFMother
    Function Save(cRec As BO.o42ImapRule) As Boolean
    Function Load(intPID As Integer) As BO.o42ImapRule
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o42ImapRule)
    Sub HandleWaitingImapMessages(cInbox As BO.o41InboxAccount)
    Function LoadHistoryByMessageGUID(strMessageGUID As String) As BO.o43ImapRobotHistory
    Function LoadHistoryByID(intO43ID As Integer) As BO.o43ImapRobotHistory
    Function LoadHistoryByRecordGUID(strGUID As String) As BO.o43ImapRobotHistory

    Sub ChangeRecordGuidInHistory(intO43ID As Integer, strNewGUID As String)
    Function Connect(cInbox As BO.o41InboxAccount) As Boolean
    Function LoadMailMessageFromHistory(intO43ID As Integer) As Rebex.Mail.MailMessage
    Function GetList_o43(mq As BO.myQuery) As IEnumerable(Of BO.o43ImapRobotHistory)
End Interface
Class o42ImapRuleBL
    Inherits BLMother
    Implements Io42ImapRuleBL
    Private WithEvents _cDL As DL.o42ImapRuleDL
    Private WithEvents _client As Imap


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o42ImapRuleDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o42ImapRule) As Boolean Implements Io42ImapRuleBL.Save
        _Error = ""
        With cRec
            If .o41ID = 0 Then _Error = "Chybí IMAP účet." : Return False
            If Trim(.o42Name) = "" Then _Error = "Název pravidla je povinné pole." : Return False
            If .j02ID_Owner_Default = 0 Then _Error = "Chybí výchozí vlastník pravidlem zakládaného záznamu"
            If .p57ID = 0 And .x18ID = 0 Then _Error = "Chybí [Typ úkolu] nebo [Typ dokumentu]."
            If .x67ID = 0 Then _Error = "Chybí specifikace role, kterou obdrží osoby v novém záznamu"
            If .p57ID <> 0 And .p41ID_Default = 0 Then
                _Error = "K typu úkolu musíte specifikovat výchozí projekt."
            End If
        End With
        If _Error <> "" Then Return False

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o42ImapRule Implements Io42ImapRuleBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io42ImapRuleBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o42ImapRule) Implements Io42ImapRuleBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function LoadHistoryByMessageGUID(strMessageGUID As String) As BO.o43ImapRobotHistory Implements Io42ImapRuleBL.LoadHistoryByMessageGUID
        Return _cDL.LoadHistoryByMessageGUID(strMessageGUID)
    End Function
    Public Function LoadHistoryByID(intO43ID As Integer) As BO.o43ImapRobotHistory Implements Io42ImapRuleBL.LoadHistoryByID
        Return _cDL.LoadHistoryByID(intO43ID)
    End Function
    Public Function LoadHistoryByRecordGUID(strGUID As String) As BO.o43ImapRobotHistory Implements Io42ImapRuleBL.LoadHistoryByRecordGUID
        Return _cDL.LoadHistoryByRecordGUID(strGUID)
    End Function
    'dále už je funkcionalita pro práci s IMAPem
    Public Function Connect(cInbox As BO.o41InboxAccount) As Boolean Implements Io42ImapRuleBL.Connect
        If cInbox Is Nothing Then Return False
        If _client Is Nothing Then _client = New Imap

        With cInbox
            Try
                Select Case .o41SslModeFlag
                    Case BO.SslModeENUM._NoSSL
                        If .o41Port = "" Then
                            _client.Connect(.o41Server)
                        Else
                            _client.Connect(.o41Server, CInt(.o41Port))
                        End If
                    Case BO.SslModeENUM.Implicit
                        _client.Settings.SslAcceptAllCertificates = True
                        If .o41Port <> "" Then
                            _client.Connect(.o41Server, BO.BAS.IsNullInt(.o41Port), SslMode.Implicit)
                        Else
                            _client.Connect(.o41Server, SslMode.Implicit)
                        End If
                    Case BO.SslModeENUM.Explicit
                        _client.Settings.SslAcceptAllCertificates = True
                        If .o41Port <> "" Then
                            _client.Connect(.o41Server, BO.BAS.IsNullInt(.o41Port), SslMode.Explicit)
                        Else
                            _client.Connect(.o41Server, SslMode.Explicit)
                        End If
                End Select
            Catch ex As Exception
                _Error = .o41Server & "<br>" & ex.Message
                Return False
            End Try

            If .o41Login <> "" Then
                Try
                    _client.Login(.o41Login, .DecryptedPassword)
                Catch ex As Rebex.Net.ImapException
                    _Error = ex.Message
                    _client = Nothing
                    Return False
                End Try

                If Not _client.IsAuthenticated() Then
                    _Error = "Bad login or password!"
                    _client.Disconnect()
                    Return False
                End If
            End If
            

            
            Try
                _client.SelectFolder(.o41Folder)
            Catch ex As Exception
                _Error = ex.Message
                _client.Disconnect()
                Return False
            End Try

            Return True

        End With

    End Function

    Private Sub Disconnect()
        If _client Is Nothing Then Return
        _client.Disconnect()
    End Sub

    Public Sub HandleWaitingImapMessages(cInbox As BO.o41InboxAccount) Implements Io42ImapRuleBL.HandleWaitingImapMessages
        If Not Connect(cInbox) Then Return
        W2L("Inbox account: " & cInbox.o41Name & ": " & cInbox.o41Login)
        Dim colINFO As ImapMessageCollection
        colINFO = _client.Search(ImapSearchParameter.HasFlagsNoneOf(ImapMessageFlags.Deleted Or ImapMessageFlags.Seen))  'seznam všech nových zpráv (nepřečtených)


        If colINFO.Count > 2000 Then
            W2L("Total messages count in IMAP account: " & colINFO.Count.ToString & ", current user: " & _cUser.j03Login)
            Dim xx As New ImapMessageSet()
            xx.AddRange(colINFO.Count - 500, colINFO.Count)
            colINFO = _client.GetMessageList(xx, ImapListFields.UniqueId)
        End If

        W2L("FETCH messages count: " & colINFO.Count.ToString & ", current user: " & _cUser.j03Login)


        Dim lisItems4Delete As New List(Of ImapMessageInfo)
        Dim intKolikMaxNajednou As Integer = 5

        For i As Integer = 0 To colINFO.Count - 1

            Dim imi As ImapMessageInfo = Nothing, bolGoON As Boolean = False
            Try
                imi = _client.GetMessageInfo(colINFO(i).UniqueId, ImapListFields.FullHeaders)

                bolGoON = True
                If imi.Flags = ImapMessageFlags.Deleted Then bolGoON = False

            Catch ex As Exception
                W2L(" _client.GetMessageInfo", ex)
            End Try
            If bolGoON And Not cInbox.o41DateImportAfter Is Nothing Then
                'kontrola vůči stáří zprávy
                Try
                    If imi.Date.UniversalTime <= cInbox.o41DateImportAfter Then bolGoON = False
                Catch ex As Exception
                    'If imi.Date.LocalTime <= _account.Date_ImportFrom Then bolGoON = False

                End Try

            End If
            If bolGoON Then
                'kontrola vůči DB, zda nebyla naimportována zpráva již dříve
                Dim strID As String = imi.MessageId.Id
                If strID = "" Then strID = imi.UniqueId
                Dim cO43 As BO.o43ImapRobotHistory = LoadHistoryByMessageGUID(strID)
                If Not cO43 Is Nothing Then
                    'zpráva byla dříve již zpracována
                    bolGoON = False
                    W2L("Message " & strID & " exists IN o43ImapRobotHistory history! Import stopped.")
                    ''If Math.Abs(cO43.o43Length - imi.Length) < 10 Then
                    ''    bolGoON = False 'na imi:UniqueID není zcela spolehnutí - test ještě podle délky zprávy
                    ''End If
                End If
            End If
            If bolGoON Then
                If ImportOneMessage(imi, cInbox) Then

                    lisItems4Delete.Add(imi)

                End If
            End If
            If i > intKolikMaxNajednou Then
                Exit For  'dosažen limit maximálního počtu najednou naimportovaných zpráv
            End If
        Next

        If lisItems4Delete.Count > 0 Then
            'odstranit naimportované zprávy ze zdrojového imap účtu, pokud platí o41IsDeleteMesageAfterImport=1
            For Each c In lisItems4Delete
                Dim strUniqueID As String = c.UniqueId
                Try
                    If cInbox.o41IsDeleteMesageAfterImport Then
                        _client.DeleteMessage(strUniqueID)  'nastaví pouze flag DELETED
                        _client.Purge() 'trvale odstranit zprávu  z inboxu
                    Else
                        'nastavit FLAG SEEN
                        _client.SetMessageFlags(strUniqueID, ImapFlagAction.Add, ImapMessageFlags.Seen)


                        Dim cO43 As BO.o43ImapRobotHistory = LoadHistoryByMessageGUID(strUniqueID)
                        If Not cO43 Is Nothing Then
                            Dim cB07 As BO.b07Comment = Factory.b07CommentBL.LoadByO43ID(cO43.o43ID)
                            If Not cB07 Is Nothing Then
                                _client.SetMessageFlags(strUniqueID, ImapFlagAction.Add, ImapMessageFlags.Keywords, "marktime-b07id", cB07.PID.ToString)
                            End If

                        End If
                      
                        
                    End If

                   

                    W2L("Message " & strUniqueID & " deleted in IMAP account.")
                Catch ex As Exception
                    W2L("Error in deleting message: " & strUniqueID & " )", ex)
                End Try

            Next

        End If
    End Sub

    Private Function ImportOneMessage(ByVal imi As ImapMessageInfo, cInbox As BO.o41InboxAccount) As Boolean
        If imi.IsDeleted Then Return False 'zprávy označené jako DELETED se ignorují
        Dim message As Rebex.Mail.MailMessage, cO43 As New BO.o43ImapRobotHistory

        Dim att As Rebex.Mail.Attachment
        Dim addr As Rebex.Mime.Headers.MailAddress, strFromAddress As String = ""

        '----------test mail adresy odesílatele-------------
        If imi.From.Count > 0 Then
            addr = imi.From.Item(0)
            strFromAddress = addr.Address
            cO43.o43FROM = strFromAddress
            cO43.o43FROM_DisplayName = imi.From.Item(0).DisplayName
        End If

        Dim receivers As New List(Of String)
        Dim lisCC_Persons As New List(Of BO.j02Person), lisCC_Projects As New List(Of BO.p41Project), lisCC_Clients As New List(Of BO.p28Contact)
        Dim lisCC_Teams As New List(Of BO.j11Team)
        Dim cJ02_FROM As BO.j02Person = Factory.j02PersonBL.LoadByEmail(cO43.o43FROM)
        For i As Integer = 0 To imi.CC.Count - 1
            receivers.Add(imi.CC(i).Address)
            If i = 0 Then cO43.o43CC = imi.CC(i).Address Else cO43.o43CC += ";" & imi.CC(i).Address
        Next
        For i As Integer = 0 To imi.To.Count - 1
            receivers.Add(imi.To(i).Address)
            If i = 0 Then cO43.o43TO = imi.To(i).Address Else cO43.o43TO += ";" & imi.To(i).Address
        Next
        For Each strAddress As String In receivers  ''.Where(Function(p) p <> cInbox.o41Login)
            Dim cJ02 As BO.j02Person = Factory.j02PersonBL.LoadByImapRobotAddress(strAddress)
            If cJ02 Is Nothing Then cJ02 = Factory.j02PersonBL.LoadByEmail(strAddress) 'u osob zkusit i normální adresu j02Email
            Dim cJ11 As BO.j11Team = Nothing
            If Not cJ02 Is Nothing Then
                lisCC_Persons.Add(cJ02)
            Else
                cJ11 = Factory.j11TeamBL.LoadByImapRobotAddress(strAddress)
                If Not cJ11 Is Nothing Then lisCC_Teams.Add(cJ11)
            End If
            If cJ02 Is Nothing And cJ11 Is Nothing Then
                Dim cP41 As BO.p41Project = Factory.p41ProjectBL.LoadByImapRobotAddress(strAddress)
                If Not cP41 Is Nothing Then lisCC_Projects.Add(cP41)
                If cP41 Is Nothing Then
                    Dim cP28 As BO.p28Contact = Factory.p28ContactBL.LoadByImapRobotAddress(strAddress)
                    If Not cP28 Is Nothing Then lisCC_Clients.Add(cP28)
                End If
            End If
        Next


        Dim strUploadFolder As String = Factory.x35GlobalParam.UploadFolder & "\IMAP\" & Year(Now).ToString & "\" & Month(Now).ToString
        If Not System.IO.Directory.Exists(strUploadFolder) Then
            System.IO.Directory.CreateDirectory(strUploadFolder)
        End If
        Dim strGUID As String = BO.BAS.GetGUID
        With cO43
            .o41ID = cInbox.PID
            .o43ImapArchiveFolder = Year(Now).ToString & "\" & Month(Now).ToString
            .o43DateImport = Now
            .o43DateMessage = imi.Date.LocalTime
            .o43RecordGUID = strGUID
            .o43MessageGUID = imi.UniqueId
            ''If imi.MessageId.Id <> "" Then .o43MessageGUID = imi.MessageId.Id
            .o43Subject = imi.Subject
            .o43Length = imi.Length

        End With


        W2L("Message GUID: " & imi.UniqueId & " is new to load, record GUID: " & strGUID)
        If imi.From.Count > 0 Then
            W2L("From: " & imi.From.Item(0).Address & " " & imi.From.Item(0).DisplayName & " | " & Format(imi.Date.LocalTime, "dd.MM.yyyy HH:mm ddd"))
        End If
        W2L("Subject: " & imi.Subject)


        Try
            message = _client.GetMailMessage(imi.UniqueId)
            With message
                If .HasBodyHtml Then
                    Dim cHTML As New BO.clsHandleHtml()
                    cO43.o43Body_Html = cHTML.ToFopCZ(.BodyHtml)
                End If
                If .HasBodyText Then
                    cO43.o43Body_PlainText = .BodyText
                    ''    cO43.o43Body_PlainText = Trim(.BodyText).Replace("<", "[").Replace(">", "]")
                End If
            End With

            ''Dim reply As Rebex.Mail.MailMessage = message.CreateReply("ja@seznam.cz", Rebex.Mail.ReplyBodyTransformation.None, True)
            ''reply.BodyText = "ahoj.\n" & message.BodyText
          


        Catch ex As Exception
            W2L("message ID " & imi.UniqueId, ex)
            Return False
        End Try



        With message
            For Each att In .Attachments
                Dim strOrigFileName As String = att.FileName
                Dim strArchiveFileName As String = strGUID & "_" & strOrigFileName
                att.Save(strUploadFolder & "\" & strArchiveFileName)  'uloží soubor do IMAP upload složky
                If cO43.o43Attachments = "" Then
                    cO43.o43Attachments += att.FileName
                Else
                    cO43.o43Attachments += "; " & att.FileName
                End If
                UploadFile2Temp(att.FileName, strArchiveFileName, strGUID, strUploadFolder) 'uložit přílohy do TEMP složky, hodí se pokud se bude jednat o o23 dokument

            Next
        End With

        'uložit zprávu do EML formátu
        message.Save(strUploadFolder & "\" & strGUID & ".eml")
        cO43.o43EmlFileName = strGUID & ".eml"

        If SaveAsMsg(message, strGUID, strUploadFolder) Then
            'uloženo do MSG formátu
            cO43.o43MsgFileName = strGUID & ".msg"
        End If

        If message.InReplyTo.Count > 0 Then
            'odpověď na zprávu
            Dim cOrigMessage As BO.x40MailQueue = Me.Factory.x40MailQueueBL.LoadByMessageID(message.InReplyTo(0).Id)
            If cOrigMessage Is Nothing Then
                'ještě zkusit najít přes References
                If Not message.Headers.Item("References") Is Nothing Then
                    Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(message.Headers.Item("References").Value.ToString, "<")
                    For Each strMesID As String In lis
                        strMesID = Trim(Replace(strMesID, ">", ""))
                        cOrigMessage = Me.Factory.x40MailQueueBL.LoadByMessageID(strMesID)
                        If Not cOrigMessage Is Nothing Then Exit For
                    Next

                End If
            End If
            If Not cOrigMessage Is Nothing Then
                'odpovídá se na zprávu odeslanou z MT
                With cOrigMessage
                    If .x29ID = BO.x29IdEnum.p56Task Then
                        cO43.p56ID = .x40RecordPID
                    End If
                    If .x29ID = BO.x29IdEnum.o23Doc Then
                        cO43.o23ID = .x40RecordPID
                    End If
                End With
            End If
        End If

        Dim intO43ID As Integer = _cDL.InsertImport2History(cO43), bolAnswer As Boolean = False
        cO43 = LoadHistoryByID(intO43ID)

        If cO43.p56ID > 0 Or cO43.o23ID > 0 Then
            bolAnswer = True
            'vazba na úkol nebo dokument existuje -> není třeba zakládat nový úkol/dokument, jedná se odpověď
            Dim cB07 As New BO.b07Comment
            With cO43
                If .p56ID > 0 Then
                    cB07.x29ID = BO.x29IdEnum.p56Task : cB07.b07RecordPID = .p56ID
                End If
                If .o23ID > 0 Then
                    cB07.x29ID = BO.x29IdEnum.o23Doc : cB07.b07RecordPID = .o23ID
                End If
                cB07.o43ID = intO43ID
                If .o43Body_PlainText <> "" Then
                    cB07.b07Value = .o43Body_PlainText
                Else
                    cB07.b07Value = .o43Body_Html
                End If
                cB07.j02ID_Owner = .j02ID_Owner
            End With
            Factory.b07CommentBL.Save(cB07, "", Nothing)
        Else
            'nový úkol nebo nový dokument
            bolAnswer = False
            Dim lisO42 As IEnumerable(Of BO.o42ImapRule) = GetList(New BO.myQuery).Where(Function(p) p.o41ID = cInbox.PID)
            For Each c In lisO42
                If c.p57ID <> 0 Then
                    cO43.p56ID = CreateP56Record(cO43, c, cInbox, cJ02_FROM, lisCC_Persons, lisCC_Projects, lisCC_Clients, lisCC_Teams)
                End If
                If c.x18ID <> 0 Then
                    cO43.o23ID = CreateO23Record(cO43, c, cInbox, cJ02_FROM, lisCC_Persons, lisCC_Projects, lisCC_Clients, lisCC_Teams)
                End If
            Next
            _cDL.UpdateHistoryBind(intO43ID, cO43.p56ID, cO43.o23ID)
        End If

        Dim strBodyForward As String = message.From(0).Address & " " & message.From(0).DisplayName & vbCrLf & vbCrLf & message.Subject & ": " & vbCrLf & vbCrLf & message.BodyText
        Dim recepientsForward As New List(Of BO.x43MailQueue_Recipient), x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified, intRecordPID As Integer = 0

        x29ID = BO.x29IdEnum.o43ImapRobotHistory
        intRecordPID = cO43.o43ID
        If cO43.p56ID > 0 Then
            x29ID = BO.x29IdEnum.p56Task
            intRecordPID = cO43.p56ID
            strBodyForward = "ÚKOL: " & Factory.GetRecordLinkUrl("p56", intRecordPID) & vbCrLf & strBodyForward
        End If
        If cO43.o23ID > 0 Then
            x29ID = BO.x29IdEnum.o23Doc
            intRecordPID = cO43.o23ID
            strBodyForward = "DOKUMENT: " & Factory.GetRecordLinkUrl("o23", intRecordPID) & vbCrLf & strBodyForward
        End If

        If cO43.p56ID = 0 And cO43.o23ID = 0 Then
            'zpráva bez vazby
            If cInbox.o41ForwardEmail_UnBound <> "" Then
                Factory.x40MailQueueBL.AppendRecipient(recepientsForward, cInbox.o41ForwardEmail_UnBound, "")
            End If
        Else
            If bolAnswer Then
                Select Case cInbox.o41ForwardFlag_Answer
                    Case BO.o41ForwardENUM.EmailAddress
                        If cInbox.o41ForwardEmail_Answer <> "" Then
                            Factory.x40MailQueueBL.AppendRecipient(recepientsForward, cInbox.o41ForwardEmail_Answer, "")
                        End If
                    Case BO.o41ForwardENUM.EntityRoles
                        For Each c In Factory.x67EntityRoleBL.GetEmails_j02_join_j11(x29ID, intRecordPID)
                            Factory.x40MailQueueBL.AppendRecipient(recepientsForward, c.x43Email, c.x43DisplayName)
                        Next
                End Select
            Else
                Select Case cInbox.o41ForwardFlag_New
                    Case BO.o41ForwardENUM.EmailAddress
                        If cInbox.o41ForwardEmail_New <> "" Then
                            Factory.x40MailQueueBL.AppendRecipient(recepientsForward, cInbox.o41ForwardEmail_New, "")
                        End If
                    Case BO.o41ForwardENUM.EntityRoles
                        For Each c In Factory.x67EntityRoleBL.GetEmails_j02_join_j11(x29ID, intRecordPID)
                            Factory.x40MailQueueBL.AppendRecipient(recepientsForward, c.x43Email, c.x43DisplayName)
                        Next
                End Select
            End If
        End If
        If recepientsForward.Count > 0 Then
            

           
            Factory.x40MailQueueBL.ForwardMessageToQueue("Upozornění na příchozí MARKTIME poštu", strBodyForward, recepientsForward, intO43ID, x29ID, intRecordPID)
        End If



        If cO43.o43ErrorMessage = "" Then
            Return True
        Else
            Return False
        End If


    End Function

    Private Function CreateP56Record(ByRef cO43 As BO.o43ImapRobotHistory, cRule As BO.o42ImapRule, cInbox As BO.o41InboxAccount, cJ02_FROM As BO.j02Person, lisJ02 As List(Of BO.j02Person), lisP41 As List(Of BO.p41Project), lisP28 As List(Of BO.p28Contact), lisJ11 As List(Of BO.j11Team)) As Integer
        Dim c As New BO.p56Task
        c.p57ID = cRule.p57ID
        c.o43ID = cO43.o43ID
        c.p56Name = cO43.o43Subject
        c.p56Description = cO43.o43Body_PlainText
        c.p56Description = Trim(c.p56Description).Replace("<", "[").Replace(">", "]")

        If lisP41.Count > 0 Then
            c.p41ID = lisP41(0).PID
        Else
            If lisP28.Count > 0 Then
                'vzít první projekt klienta
                Dim mq As New BO.myQueryP41
                mq.p28ID = lisP28(0).PID
                Dim lis As IEnumerable(Of BO.p41Project) = Factory.p41ProjectBL.GetList(mq)
                If lis.Count > 0 Then c.p41ID = lis(0).PID
            End If
        End If
        If c.p41ID = 0 Then
            'projekt chybí, je třeba ho vzít z IMAP pravidla (výchozí projekt)
            c.p41ID = cRule.p41ID_Default
        End If
        If Not cJ02_FROM Is Nothing Then
            c.j02ID_Owner = cJ02_FROM.PID
        Else
            c.j02ID_Owner = cRule.j02ID_Owner_Default   'vlastník úkolu chybí, je třeba ho vzít z IMAP pravidla
        End If
        Dim cJ03 As BO.j03User = Factory.j03UserBL.LoadByJ02ID(c.j02ID_Owner)
        If Not cJ03 Is Nothing Then
            'odesílatel mailu má zavedený účet
            c.SetUserInsert(cJ03.j03Login)
        Else
            c.SetUserInsert("imap-robot")
        End If

        Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
        For Each person In lisJ02
            Dim role As New BO.x69EntityRole_Assign()
            role.j02ID = person.PID
            role.x67ID = cRule.x67ID
            lisX69.Add(role)
        Next
        For Each team In lisJ11
            Dim role As New BO.x69EntityRole_Assign()
            role.j11ID = team.PID
            role.x67ID = cRule.x67ID
            lisX69.Add(role)
        Next

        With Factory.p56TaskBL
            If .Save(c, lisX69, Nothing, "") Then
                Dim intP56ID As Integer = .LastSavedPID

                Dim cB07 As New BO.b07Comment
                cB07.o43ID = cO43.o43ID
                cB07.x29ID = BO.x29IdEnum.p56Task
                cB07.b07RecordPID = intP56ID
                If cO43.o43Body_PlainText <> "" Then
                    cB07.b07Value = cO43.o43Body_PlainText
                Else
                    cB07.b07Value = cO43.o43Body_Html
                End If
                cB07.j02ID_Owner = c.j02ID_Owner
                Factory.b07CommentBL.Save(cB07, "", Nothing)

                Return intP56ID
            Else
                cO43.o43ErrorMessage = "Chyba při pokusu o založení úkolu: " & .ErrorMessage
                W2L("Chyba při pokusu o založení úkolu: " & .ErrorMessage)
                Return 0
            End If
        End With

    End Function


    Private Function SaveAsMsg(ByVal mes As Rebex.Mail.MailMessage, strGUID As String, strArchiveFolder As String) As Boolean
        Dim msg As New Message()
        With msg
            .Encoding = System.Text.Encoding.GetEncoding(1250)
            .Subject = mes.Subject
            .DateCompleted = mes.Date.LocalTime
            If mes.HasBodyHtml Then
                ''.BodyHtmlText = cHTML.ToFopCZ(mes.BodyHtml)
            End If
            If mes.HasBodyText Then
                .Body = mes.BodyText
            End If


            .SenderEmailAddress = mes.From.Item(0).Address
            .SenderName = mes.From.Item(0).DisplayName
            .SenderAddressType = "SMTP"

            .MessageFlags = New Independentsoft.Msg.MessageFlag() {Independentsoft.Msg.MessageFlag.Read}

            Dim rec As New Recipient
            Try
                rec.EmailAddress = mes.To.Item(0).Address
                rec.DisplayName = mes.To.Item(0).DisplayName
            Catch ex As Exception

            End Try



            rec.AddressType = "SMTP"
            rec.RecipientType = Independentsoft.Msg.RecipientType.To
            .Recipients.Add(rec)

            Try
                .ReceivedByEmailAddress = mes.To.Item(0).Address
                .ReceivedByEmailAddress = mes.To.Item(0).Address
            Catch ex As Exception

            End Try


            Dim att As Rebex.Mail.Attachment
            For Each att In mes.Attachments

                Dim strFileName As String = BO.BAS.RemoveDiacritism(att.FileName)
                .Attachments.Add(New Independentsoft.Msg.Attachment(strFileName, att.GetContentStream()))
            Next
            Try

                .Save(strArchiveFolder & "\" & strGUID & ".msg")
                Return True

            Catch ex As Exception
                _Error = ex.Message
                Return False
            End Try
        End With

    End Function

    Private Sub W2L(strMessage2Log As String, Optional ex As Exception = Nothing)
        Dim s As String = strMessage2Log
        If Not ex Is Nothing Then
            s += vbCrLf & vbCrLf & ex.Message
        End If
        log4net.LogManager.GetLogger("imaplog").Info(strMessage2Log)
    End Sub

    ''Private Function InsertImport2History(cHistory As BO.o43ImapRobotHistory) As Integer
    ''    Return _cDL.InsertImport2History(cHistory)

    ''    ''Dim cB07 As New BO.b07Comment
    ''    ''cB07.o43ID = intO43ID
    ''    ''If cHistory.o43Body_PlainText <> "" Then
    ''    ''    cB07.b07Value = cHistory.o43Body_PlainText
    ''    ''Else
    ''    ''    cB07.b07Value = cHistory.o43Body_Html
    ''    ''End If
    ''    ''cB07.j02ID_Owner = cHistory.j02ID_Owner


    ''    ''If intO43ID <> 0 And cHistory.p56ID <> 0 Then
    ''    ''    'propsat do úkolu informaci o vazbě na IMAP zdroj
    ''    ''    Factory.p56TaskBL.UpdateImapSource(cHistory.p56ID, intO43ID)

    ''    ''    cB07.x29ID = BO.x29IdEnum.p56Task
    ''    ''    cB07.b07RecordPID = cHistory.p56ID
    ''    ''    Factory.b07CommentBL.Save(cB07, "", Nothing)
    ''    ''End If
    ''    ''If intO43ID <> 0 And cHistory.o23ID <> 0 Then
    ''    ''    'propsat do dokumentu informaci o vazbě na IMAP zdroj
    ''    ''    Factory.o23DocBL.UpdateImapSource(cHistory.o23ID, intO43ID)

    ''    ''    cB07.x29ID = BO.x29IdEnum.o23Doc
    ''    ''    cB07.b07RecordPID = cHistory.o23ID
    ''    ''    Factory.b07CommentBL.Save(cB07, "", Nothing)
    ''    ''End If

    ''    ''Return intO43ID
    ''End Function
    Public Sub ChangeRecordGuidInHistory(intO43ID As Integer, strNewGUID As String) Implements Io42ImapRuleBL.ChangeRecordGuidInHistory
        _cDL.ChangeRecordGuidInHistory(intO43ID, strNewGUID)
    End Sub


    Private Function CreateO23Record(ByRef cO43 As BO.o43ImapRobotHistory, cRule As BO.o42ImapRule, cInbox As BO.o41InboxAccount, cJ02_FROM As BO.j02Person, lisJ02 As List(Of BO.j02Person), lisP41 As List(Of BO.p41Project), lisP28 As List(Of BO.p28Contact), lisJ11 As List(Of BO.j11Team)) As Integer
        Dim cx18 As BO.x18EntityCategory = Factory.x18EntityCategoryBL.Load(cRule.x18ID)

        Dim c As New BO.o23Doc
        c.x23ID = cx18.x23ID
        c.o23Name = cO43.o43Subject
        c.o43ID = cO43.o43ID
        c.o23FreeDate01 = cO43.o43DateMessage

        Dim strPlainText As String = Trim(cO43.o43Body_PlainText).Replace("<", "[").Replace(">", "]")

        With Factory.o23DocBL
            If .Save(c, 0, Nothing, Nothing, Nothing, cO43.o43RecordGUID) Then
                Dim intO23ID As Integer = .LastSavedPID

                .SaveHtmlContent(intO23ID, cO43.o43Body_Html, strPlainText)

                Dim cB07 As New BO.b07Comment
                cB07.o43ID = cO43.o43ID
                cB07.x29ID = BO.x29IdEnum.o23Doc
                cB07.b07RecordPID = intO23ID
                If cO43.o43Body_PlainText <> "" Then
                    cB07.b07Value = cO43.o43Body_PlainText
                Else
                    cB07.b07Value = cO43.o43Body_Html
                End If
                cB07.j02ID_Owner = c.j02ID_Owner
                Factory.b07CommentBL.Save(cB07, "", Nothing)

                Return intO23ID
            Else
                cO43.o43ErrorMessage = "Chyba při pokusu o založení dokumentu: " & .ErrorMessage
                W2L("Chyba při pokusu o založení dokumentu: " & .ErrorMessage)
                Return 0
            End If
        End With

    End Function

    Private Sub UploadFile2Temp(strOrigFileName As String, strArchiveFileName As String, strGUID As String, strImapUploadFolder As String)
        Dim cF As New BO.clsFile
        cF.CopyFile(strImapUploadFolder & "\" & strArchiveFileName, Factory.x35GlobalParam.TempFolder & "\" & strArchiveFileName)

        Dim strContentType As String = ""
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = strGUID
        cRec.p85Prefix = "o27"
        cRec.p85OtherKey1 = 1   'o13ID

        cRec.p85FreeText01 = strOrigFileName
        cRec.p85FreeText02 = strArchiveFileName
        cRec.p85FreeText03 = cF.GetContentType(Factory.x35GlobalParam.TempFolder & "\" & strArchiveFileName)
        cRec.p85FreeText05 = Right(strOrigFileName, 4)
        cRec.p85FreeNumber01 = cF.GetFileSize(Factory.x35GlobalParam.TempFolder & "\" & strArchiveFileName)

        Factory.p85TempBoxBL.Save(cRec)
    End Sub

    Public Function LoadMailMessageFromHistory(intO43ID As Integer) As Rebex.Mail.MailMessage Implements Io42ImapRuleBL.LoadMailMessageFromHistory
        Dim cO43 As BO.o43ImapRobotHistory = Factory.o42ImapRuleBL.LoadHistoryByID(intO43ID)
        Dim cInbox As BO.o41InboxAccount = Factory.o41InboxAccountBL.Load(cO43.o41ID)
        If Not Connect(cInbox) Then
            Return Nothing
        End If
        Try
            Return _client.GetMailMessage(cO43.o43MessageGUID)
        Catch ex As Exception
            Dim strPath As String = Me.Factory.x35GlobalParam.UploadFolder & "\" & cO43.o43ImapArchiveFolder & "\" & cO43.o43EmlFileName
            If System.IO.File.Exists(strPath) Then
                Dim mail As New Rebex.Mail.MailMessage
                mail.Load(strPath)
                Return mail            
            End If
            Return Nothing
        End Try



    End Function

    Public Function GetList_o43(mq As BO.myQuery) As IEnumerable(Of BO.o43ImapRobotHistory) Implements Io42ImapRuleBL.GetList_o43
        Return _cDL.GetList_o43(mq)
    End Function
End Class
