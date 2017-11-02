Public Class sendmail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _isChangeJ61ID As Boolean = False
    Private Property _color As System.Drawing.Color = System.Drawing.Color.SkyBlue


    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return DirectCast(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property
    Private Sub sendmail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.Factory.SysUser.j02ID)
            If cJ02.o40ID = 0 Then
                txtSender.Text = Master.Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress")
                If txtSender.Text <> Master.Factory.SysUser.PersonEmail Then
                    lblReplyTo.Text = String.Format("REPLY adresa: {0}", Master.Factory.SysUser.PersonEmail)
                End If
            Else
                txtSender.Text = cJ02.j02Email

            End If

           

            If Request.Item("prefix") <> "" Then
                Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
                Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Master.DataPID = 0 Then Master.StopPage("pid missing")
                Me.cmdInsertLink.Visible = True
                If Me.CurrentX29ID = BO.x29IdEnum.j02Person And Master.DataPID <> Master.Factory.SysUser.j02ID Then
                    If Master.DataPID <> 0 Then Me.txtTo.Text = Master.Factory.j02PersonBL.Load(Master.DataPID).j02Email
                End If

            Else
                Me.CurrentX29ID = BO.x29IdEnum.j02Person
                Master.DataPID = Master.Factory.SysUser.j02ID
            End If

            Me.EntityContext.Text = BO.BAS.GetX29EntityAlias(Me.CurrentX29ID, False) & ": " & Master.Factory.GetRecordCaption(Me.CurrentX29ID, Master.DataPID, True)
            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID

            SetupTemplates()
            If Me.CurrentPrefix <> "" Then
                Me.txtSubject.Text = Master.Factory.GetRecordCaption(Me.CurrentX29ID, Master.DataPID)
            End If
            Select Case Me.CurrentPrefix
                Case "p28"
                    hidMasterPrefix_p30.Value = "p28"
                    hidMasterPID_p30.Value = Master.DataPID.ToString
                Case "p41"
                    hidMasterPrefix_p30.Value = "p41"
                    hidMasterPID_p30.Value = Master.DataPID.ToString

                Case "p91"
                    Dim cP91 As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
                    hidMasterPID_p30.Value = cP91.p28ID.ToString
                    hidMasterPrefix_p30.Value = "p28"
                    Me.txtSubject.Text = String.Format("Faktura {0} | {1}", cP91.p91Code, cP91.p91Client)
                    Dim tos As New List(Of String)
                    Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(cP91.p28ID).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email And p.o32IsDefaultInInvoice = True)
                    For Each c In lisO32
                        tos.Add(c.o32Value)
                    Next
                    Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(cP91.p28ID, cP91.p41ID_First, False).Where(Function(p) p.j02IsInvoiceEmail = True)
                    For Each c In lisJ02
                        tos.Add(c.j02Email)
                    Next
                    
                    If cP91.j02ID_ContactPerson <> 0 Then
                        Dim s As String = Master.Factory.j02PersonBL.Load(cP91.j02ID_ContactPerson).j02Email
                        If s <> "" Then tos.Add(s)
                    End If
                    If tos.Count > 0 Then
                        Me.txtTo.Text = String.Join(",", tos.Distinct)
                    End If
                    If cP91.p28ID <> 0 Then
                        Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(cP91.p28ID)
                        If cP28.j61ID_Invoice <> 0 Then
                            basUI.SelectDropdownlistValue(Me.j61ID, cP28.j61ID_Invoice)
                            Handle_ChangeJ61ID()
                        End If
                    End If
                    

            End Select
            If Me.hidMasterPrefix_p30.Value <> "" Then
                linkNewPerson.Text = BO.BAS.OM2(Me.linkNewPerson.Text, Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(hidMasterPrefix_p30.Value), BO.BAS.IsNullInt(hidMasterPID_p30.Value), False))
            End If

            With Master
                .HeaderText = "Odeslat poštovní zprávu"
                .HeaderIcon = "Images/email_32.png"
                .AddToolbarButton("Zařadit k odeslání", "queue", , "Images/queue.png", , , , True)
                .AddToolbarButton("Odeslat zprávu", "ok", , "Images/email.png", , , , True)
            End With
            SetupCombos()
            If Me.CurrentX29ID = BO.x29IdEnum.p91Invoice And Me.txtTo.Text = "" And Me.cbxAddContactPerson.Items.Count > 1 Then
                Dim s As String = Me.cbxAddContactPerson.Items(1).Value
                If Not IsNumeric(s) Then
                    Me.txtTo.Text = s
                Else
                    Me.txtTo.Text = Master.Factory.j02PersonBL.Load(CInt(s)).j02Email
                End If
            End If
            If Me.txtBody.Text = "" Then
                If Me.CurrentX29ID > BO.x29IdEnum._NotSpecified And Me.CurrentX29ID <> BO.x29IdEnum.j02Person Then
                    Me.txtBody.Text = vbCrLf & vbCrLf & "Přímý odkaz: " & Master.Factory.GetRecordLinkUrl(Me.CurrentPrefix, Master.DataPID)
                End If
                
            End If
            If Master.Factory.SysUser.j02ID > 0 Then
                Dim strSignature As String = Master.Factory.j02PersonBL.Load(Master.Factory.SysUser.j02ID).j02EmailSignature
                If strSignature <> "" Then
                    Me.txtBody.Text += vbCrLf & vbCrLf & strSignature
                End If
            End If


            
            If Request.Item("x31id") <> "" Then
                GenerateReportOnBehind(BO.BAS.IsNullInt(Request.Item("x31id")))
            End If
            If Request.Item("tempfile") <> "" Then
                PrepareTempFile(Request.Item("tempfile"))

            End If
        End If
    End Sub
    
    Private Sub SetupTemplates()
        Me.j61ID.DataSource = Master.Factory.j61TextTemplateBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
        Me.j61ID.DataBind()
        Me.j61ID.Items.Insert(0, "--Vyberte pojmenovanou šablonu zprávy/textu--")

        
    End Sub

    Private Sub SetupCombos()
        Dim mq As New BO.myQueryJ02
        mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq)
        With Me.cbxAddPerson
            If lisJ02.Count < 100 Then
                .DataSource = lisJ02
                .DataBind()
                .Items.Insert(0, "--Osoba (interní)--")
            Else
                .Visible = False
            End If
        End With
        mq = New BO.myQueryJ02
        mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p91Invoice
                mq.p91ID = Master.DataPID
            Case BO.x29IdEnum.p28Contact
                mq.p28ID = Master.DataPID
            Case BO.x29IdEnum.p41Project
                mq.p41ID = Master.DataPID
            Case Else
                mq.IntraPersons = BO.myQueryJ02_IntraPersons.NonIntraOnly
        End Select
        lisJ02 = Master.Factory.j02PersonBL.GetList(mq)

        With Me.cbxAddContactPerson
            If lisJ02.Count < 100 Then
                .DataSource = lisJ02
                .DataBind()
                .Items.Insert(0, "--Kontaktní osoba--")
                
            End If
            Dim intP28ID As Integer = 0
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p28Contact : intP28ID = Master.DataPID
                Case BO.x29IdEnum.p91Invoice
                    Dim cP91 As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
                    intP28ID = cP91.p28ID
            End Select
            If intP28ID > 0 Then
                Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(intP28ID).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email)
                For Each c In lisO32
                    Me.cbxAddContactPerson.Items.Add(New ListItem(c.o32Value & " " & c.o32Description, c.o32Value))
                Next
            End If        
        End With

        With Me.cbxAddPosition
            .DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
            .DataBind()
            .Items.Insert(0, "--Pozice osoby--")
        End With
        With Me.cbxAddTeam
            .DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            .DataBind()
            .Items.Insert(0, "--Tým osob--")
        End With
        
    End Sub

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Or strButtonValue = "queue" Then
            Dim message As New Rebex.Mail.MailMessage
            With message
                .BodyText = Trim(Me.txtBody.Text)
                If BO.BAS.TestEMailAddress(Me.txtSender.Text, "") Then
                    .From.Add(New Rebex.Mime.Headers.MailAddress(Me.txtSender.Text, Master.Factory.SysUser.Person & " via MARKTIME"))
                End If
                If .From.Count > 0 Then
                    If .From(0).Address <> Master.Factory.SysUser.PersonEmail Then
                        .ReplyTo = Master.Factory.SysUser.PersonEmail
                    End If
                End If
                .Subject = Me.txtSubject.Text
                Master.Factory.x40MailQueueBL.CompleteMailAttachments(message, upload1.GUID)
                
            End With
            Me.txtTo.Text = Replace(Replace(Me.txtTo.Text, " ", ""), ";", ",")
            Dim a() As String = Split(Trim(Me.txtTo.Text), ",")
            Dim recipients As New List(Of BO.x43MailQueue_Recipient)
            If Me.txtTo.Text <> "" Then
                For Each strEmail As String In a
                    Dim cX43 As New BO.x43MailQueue_Recipient()
                    cX43.x43Email = strEmail
                    cX43.x43RecipientFlag = BO.x43RecipientIdEnum.recTO
                    recipients.Add(cX43)
                Next
            End If
            Me.txtCC.Text = Replace(Replace(Me.txtCC.Text, " ", ""), ";", ",")
            If Me.txtCC.Text <> "" Then
                a = Split(Me.txtCC.Text, ",")
                For Each strEmail As String In a
                    Dim cX43 As New BO.x43MailQueue_Recipient()
                    cX43.x43Email = strEmail
                    cX43.x43RecipientFlag = BO.x43RecipientIdEnum.recCC
                    recipients.Add(cX43)
                Next
            End If

            Me.txtBCC.Text = Replace(Replace(Me.txtBCC.Text, " ", ""), ";", ",")
            If Me.txtBCC.Text <> "" Then
                a = Split(Me.txtBCC.Text, ",")
                For Each strEmail As String In a
                    Dim cX43 As New BO.x43MailQueue_Recipient()
                    cX43.x43Email = strEmail
                    cX43.x43RecipientFlag = BO.x43RecipientIdEnum.recBCC
                    recipients.Add(cX43)
                Next
            End If
            If Me.CurrentX29ID = BO.x29IdEnum.o22Milestone Then
                Dim strPath As String = Master.Factory.o22MilestoneBL.CreateICalendarTempFullPath(Master.DataPID)
                If strPath <> "" Then message.Attachments.Add(New Rebex.Mail.Attachment(strPath))
            End If

            Dim messageStatus As BO.x40StateENUM = BO.x40StateENUM.InQueque
            If strButtonValue = "queue" Then messageStatus = BO.x40StateENUM.WaitOnConfirm
            With Master.Factory.x40MailQueueBL

                Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, Me.CurrentX29ID, Master.DataPID, messageStatus, 0)
                If intMessageID > 0 Then
                    If messageStatus = BO.x40StateENUM.InQueque Then
                        'rovnou odeslat
                        If .SendMessageFromQueque(intMessageID) Then
                            Master.CloseAndRefreshParent("send-mail")
                        Else
                            Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        End If
                    Else
                        'zpráva zařazena k potvrzení
                        Master.CloseAndRefreshParent("save-mail")
                    End If
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If




            End With

        End If
    End Sub

    Private Sub cbxAddPerson_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAddPerson.SelectedIndexChanged
        If Me.cbxAddPerson.SelectedIndex > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.AddItemToPIDs(CInt(Me.cbxAddPerson.SelectedValue))
            AR(mq)
            Me.cbxAddPerson.SelectedIndex = 0
        End If
    End Sub

    Private Sub AR(mq As BO.myQueryJ02, Optional strDirectMail As String = "")
        If strDirectMail <> "" Then
            If Me.txtTo.Text = "" Then
                Me.txtTo.Text = strDirectMail
            Else
                If Me.txtTo.Text.IndexOf(strDirectMail) < 0 Then
                    Me.txtTo.Text += "," & strDirectMail
                End If
            End If
        End If
        If mq Is Nothing Then Return
        Dim lis As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq).Where(Function(p) p.j02Email <> "")
        For Each c In lis
            If Me.txtTo.Text = "" Then
                Me.txtTo.Text = c.j02Email
            Else
                If Me.txtTo.Text.IndexOf(c.j02Email) < 0 Then
                    Me.txtTo.Text += "," & c.j02Email
                End If

            End If
        Next

    End Sub

    Private Sub cbxAddPosition_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAddPosition.SelectedIndexChanged
        If Me.cbxAddPosition.SelectedIndex > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.j07ID = CInt(Me.cbxAddPosition.SelectedValue)
            AR(mq)
            Me.cbxAddPosition.SelectedIndex = 0
        End If
    End Sub

    Private Sub cbxAddTeam_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAddTeam.SelectedIndexChanged
        If Me.cbxAddTeam.SelectedIndex > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.j11ID = CInt(Me.cbxAddTeam.SelectedValue)
            Dim cRec As BO.j11Team = Master.Factory.j11TeamBL.Load(CInt(Me.cbxAddTeam.SelectedValue))
            If cRec.j11Email <> "" Then mq = Nothing
            AR(mq, cRec.j11Email)
            Me.cbxAddTeam.SelectedIndex = 0
        End If
    End Sub

    Private Sub GenerateReportOnBehind(intX31ID As Integer)
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(intX31ID)
        If cRec Is Nothing Then
            Master.Notify("Nelze načíst šablonu sestavy.", NotifyLevel.ErrorMessage)
            Return
        End If
        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName
        Dim cRep As New clsReportOnBehind()
        If Request.Item("datfrom") <> "" Then
            cRep.Query_DateFrom = BO.BAS.ConvertString2Date(Request.Item("datfrom"))
        End If
        If Request.Item("datuntil") <> "" Then
            cRep.Query_DateUntil = BO.BAS.ConvertString2Date(Request.Item("datuntil"))
        End If
        If cRec.x29ID > BO.x29IdEnum._NotSpecified And BO.BAS.IsNullInt(Request.Item("pid")) <> 0 Then
            'kontextová sestava - je třeba zjistit parametr @pid
            cRep.Query_RecordPID = BO.BAS.IsNullInt(Request.Item("pid"))
        End If

        Dim strOutputFileName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "pdf", False, intX31ID)
        strOutputFileName = cRep.GenerateReport2Temp(Master.Factory, strRepFullPath, , strOutputFileName)
        If strOutputFileName = "" Then
            Master.Notify("Chyba při generování PDF.", NotifyLevel.ErrorMessage) : Return
        End If
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p91Invoice
                Dim cP91 As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
                Me.txtSubject.Text = String.Format("Faktura {0} | {1}", cP91.p91Code, cP91.p91Client)
            Case Else
                Me.txtSubject.Text = cRec.x31Name
        End Select


        Dim cTemp As New BO.p85TempBox(), cF As New BO.clsFile
        Dim lisO13 As IEnumerable(Of BO.o13AttachmentType) = Master.Factory.o13AttachmentTypeBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.x40MailQueue)
        With cTemp
            If lisO13.Count > 0 Then
                .p85OtherKey1 = lisO13(0).PID
                .p85FreeText06 = lisO13(0).o13Name
            End If
            .p85GUID = upload1.GUID
            .p85FreeText01 = strOutputFileName
            .p85FreeText02 = strOutputFileName
            .p85FreeText03 = "application/pdf"
            .p85FreeText04 = "PDF report"
            .p85FreeNumber01 = cF.GetFileSize(Master.Factory.x35GlobalParam.TempFolder & "\" & strOutputFileName)
        End With
        Master.Factory.p85TempBoxBL.Save(cTemp)
        uploadlist1.RefreshData_TEMP()
    End Sub

    Private Sub cbxAddContactPerson_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAddContactPerson.SelectedIndexChanged
        If Me.cbxAddContactPerson.SelectedIndex > 0 Then
            If Not IsNumeric(Me.cbxAddContactPerson.SelectedValue) Then
                'pouze kontaktní médium
                AR(Nothing, Me.cbxAddContactPerson.SelectedValue)
                Return
            End If
            Dim mq As New BO.myQueryJ02
            mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
            mq.AddItemToPIDs(CInt(Me.cbxAddContactPerson.SelectedValue))
            AR(mq)
            Me.cbxAddContactPerson.SelectedIndex = 0
        End If
    End Sub

    Private Sub j61ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j61ID.SelectedIndexChanged
        Handle_ChangeJ61ID()
    End Sub

    Private Sub Handle_ChangeJ61ID()
        Dim intJ61ID As Integer = BO.BAS.IsNullInt(Me.j61ID.SelectedValue), strLINK As String = Master.Factory.GetRecordLinkUrl(Me.CurrentPrefix, Master.DataPID)
        If intJ61ID <> 0 Then
            _isChangeJ61ID = True
            Dim c As BO.j61TextTemplate = Master.Factory.j61TextTemplateBL.Load(intJ61ID)
            Dim cRoles As New BO.Roles4Notification
            If Me.CurrentX29ID > BO.x29IdEnum._NotSpecified And ((c.j61MailSubject & c.j61PlainTextBody).IndexOf("]") > 0) Then
                Dim cM As New BO.clsMergeContent(), objects As New List(Of Object)
                If c.j61PlainTextBody.IndexOf("#RolesInline#") > 0 Then
                    c.j61PlainTextBody = Replace(c.j61PlainTextBody, "#RolesInline#", "[%RolesInLine%]", , , CompareMethod.Text)
                End If
                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.p41Project
                        objects.Add(Master.Factory.p41ProjectBL.Load(Master.DataPID))
                        cRoles.RolesInLine = Master.Factory.p41ProjectBL.GetRolesInline(Master.DataPID)
                    Case BO.x29IdEnum.p28Contact
                        objects.Add(Master.Factory.p28ContactBL.Load(Master.DataPID))
                        cRoles.RolesInLine = Master.Factory.p28ContactBL.GetRolesInline(Master.DataPID)
                    Case BO.x29IdEnum.p91Invoice
                        objects.Add(Master.Factory.p91InvoiceBL.Load(Master.DataPID))
                    Case BO.x29IdEnum.j02Person
                        objects.Add(Master.Factory.j02PersonBL.Load(Master.DataPID))
                    Case BO.x29IdEnum.p56Task
                        objects.Add(Master.Factory.j02PersonBL.Load(Master.DataPID))
                        cRoles.RolesInLine = Master.Factory.p56TaskBL.GetRolesInline(Master.DataPID)
                    Case BO.x29IdEnum.o23Doc
                        objects.Add(Master.Factory.o23DocBL.Load(Master.DataPID))
                        cRoles.RolesInLine = Master.Factory.o23DocBL.GetRolesInline(Master.DataPID)
                End Select
                objects.Add(cRoles)
                c.j61PlainTextBody = cM.MergeContent(objects, c.j61PlainTextBody, strLINK)
                c.j61MailSubject = cM.MergeContent(objects, c.j61MailSubject, strLINK)
            End If
            If c.j61PlainTextBody <> "" Then
                Me.txtBody.Text = c.j61PlainTextBody : Me.txtBody.BackColor = _color

            End If
            If c.j61MailSubject <> "" Then
                Me.txtSubject.Text = c.j61MailSubject : Me.txtSubject.BackColor = _color
            Else
                If Me.txtSubject.BackColor = _color Then Me.txtSubject.Text = "" : Me.txtSubject.BackColor = Nothing
            End If
            If c.j61MailTO <> "" Then
                Me.txtTo.Text = c.j61MailTO : Me.txtTo.BackColor = _color
            Else
                If Me.txtTo.BackColor = _color Then Me.txtTo.Text = "" : Me.txtTo.BackColor = Nothing
            End If
            If c.j61MailCC <> "" Then
                Me.txtCC.Text = c.j61MailCC : Me.txtCC.BackColor = _color
            Else
                If Me.txtCC.BackColor = _color Then Me.txtCC.Text = "" : Me.txtCC.BackColor = Nothing
            End If
            If c.j61MailBCC <> "" Then
                Me.txtBCC.Text = c.j61MailBCC : Me.txtBCC.BackColor = _color
            Else
                If Me.txtBCC.BackColor = _color Then Me.txtBCC.Text = "" : Me.txtBCC.BackColor = Nothing
            End If
        End If
    End Sub

    Private Sub sendmail_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Not _isChangeJ61ID Then
            Me.txtBody.BackColor = Nothing
            Me.txtSubject.BackColor = Nothing
            Me.txtBCC.BackColor = Nothing
            Me.txtCC.BackColor = Nothing
            Me.txtTo.BackColor = Nothing
        End If
        If BO.BAS.IsNullInt(Me.j61ID.SelectedValue) = 0 Then
            cmdEdit.Visible = False
        Else
            cmdEdit.Visible = True
        End If
        cmdClone.Visible = cmdEdit.Visible
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        SetupTemplates()
        If Me.hidHardRefreshPID.Value <> "" Then
            basUI.SelectDropdownlistValue(Me.j61ID, Me.hidHardRefreshPID.Value)
            Handle_ChangeJ61ID()
        End If
        hidHardRefreshPID.Value = "" : Me.hidHardRefreshFlag.Value = ""
    End Sub


    Private Sub PrepareTempFile(strTempFileName As String)
        ''Dim strOutputFileName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "pdf", False, 0)

        Dim cTemp As New BO.p85TempBox(), cF As New BO.clsFile
        Dim lisO13 As IEnumerable(Of BO.o13AttachmentType) = Master.Factory.o13AttachmentTypeBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.x40MailQueue)
        With cTemp
            If lisO13.Count > 0 Then
                .p85OtherKey1 = lisO13(0).PID
                .p85FreeText06 = lisO13(0).o13Name
            End If
            .p85GUID = upload1.GUID
            ''.p85FreeText01 = strOutputFileName
            .p85FreeText01 = strTempFileName
            .p85FreeText02 = strTempFileName
            .p85FreeText03 = cF.GetContentType(Master.Factory.x35GlobalParam.TempFolder & "\" & strTempFileName)
            .p85FreeNumber01 = cF.GetFileSize(Master.Factory.x35GlobalParam.TempFolder & "\" & strTempFileName)
        End With
        Master.Factory.p85TempBoxBL.Save(cTemp)
        uploadlist1.RefreshData_TEMP()
    End Sub

    
    Private Sub cmdInsertLink_Click(sender As Object, e As EventArgs) Handles cmdInsertLink.Click
        Me.txtBody.Text += vbCrLf & vbCrLf & "Přímý odkaz: " & Master.Factory.GetRecordLinkUrl(Me.CurrentPrefix, Master.DataPID)
    End Sub
End Class