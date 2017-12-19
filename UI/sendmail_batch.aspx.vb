Public Class sendmail_batch
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Class EmailReceiver
        Public Property PID As Integer
        Public Property Prefix As String
        Public Property RecordName As String
        Public Property Emails As String
        Public Sub New(intPID As Integer, strPrefix As String)
            Me.PID = intPID
            Me.Prefix = strPrefix
        End Sub
    End Class

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

    Private Sub sendmail_batch_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            If Request.Item("prefix") = "" Then Master.StopPage("prefix missing.") : Return
            Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))

            System.Threading.Thread.Sleep(3000) 'počkat 3 sekundy než se na klientovi uloží seznam PIDů
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.LoadByGUID(Me.CurrentPrefix & "_batch_sendmail-pids-" & Master.Factory.SysUser.PID.ToString)
            If cRec Is Nothing Then Master.StopPage("pids is missing.")
            If cRec.p85Message = "" Then Master.StopPage("Na vstupu chybí vybrané záznamy faktur.")
            hidPIDs.Value = cRec.p85Message





            Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.Factory.SysUser.j02ID)
            If cJ02.o40ID = 0 Then
                txtSender.Text = Master.Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress")
                If txtSender.Text <> Master.Factory.SysUser.PersonEmail Then
                    lblReplyTo.Text = String.Format("REPLY adresa: {0}", Master.Factory.SysUser.PersonEmail)
                End If
            Else
                txtSender.Text = cJ02.j02Email

            End If

            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID

            SetupTemplates()

            With Master
                .HeaderText = "Hromadně odeslat elektronickou poštu"
                .HeaderIcon = "Images/email_32.png"
                .AddToolbarButton("Zařadit do fronty k odeslání", "ok", , "Images/ok.png", , , , True)
            End With

            RefreshList()
        End If


    End Sub

    Private Sub SetupTemplates()
        Me.j61ID.DataSource = Master.Factory.j61TextTemplateBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
        Me.j61ID.DataBind()
        Me.j61ID.Items.Insert(0, "--Vyberte pojmenovanou šablonu zprávy/textu--")


    End Sub

    Private Sub j61ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j61ID.SelectedIndexChanged
        Handle_ChangeJ61ID()

    End Sub
    Private Sub Handle_ChangeJ61ID()
        Dim intJ61ID As Integer = BO.BAS.IsNullInt(Me.j61ID.SelectedValue)
        If intJ61ID <> 0 Then

            Dim c As BO.j61TextTemplate = Master.Factory.j61TextTemplateBL.Load(intJ61ID)

            Me.txtBody.Text = c.j61PlainTextBody
            Me.txtSubject.Text = c.j61MailSubject
        End If
    End Sub

    Private Sub sendmail_batch_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
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

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub


    Private Sub RefreshList()
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(hidPIDs.Value)
        Dim recs As New List(Of EmailReceiver)
        Select Me.CurrentPrefix
            Case "p28"
                Dim mq As New BO.myQueryP28
                mq.PIDs = pids
                Dim lis As IEnumerable(Of BO.p28Contact) = Master.Factory.p28ContactBL.GetList(mq)
                For Each c In lis
                    Dim cc As New EmailReceiver(c.PID, "p28")
                    cc.RecordName = c.p28Name
                    cc.Emails = String.Join(",", Master.Factory.p28ContactBL.GetList_o32(c.PID).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email).Select(Function(p) p.o32Value))

                    Dim s As String = String.Join(",", Master.Factory.p30Contact_PersonBL.GetList_J02(c.PID, 0, False).Where(Function(p) p.j02Email <> "").Select(Function(p) p.j02Email))
                    If s <> "" Then
                        If cc.Emails = "" Then cc.Emails = s Else cc.Emails += "," & s
                    End If
                    recs.Add(cc)
                Next
            Case "j02"
                Dim mq As New BO.myQueryJ02
                mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
                mq.PIDs = pids
                Dim lis As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq)
                For Each c In lis
                    Dim cc As New EmailReceiver(c.PID, "j02")
                    cc.RecordName = c.FullNameDesc
                    cc.Emails = c.j02Email
                    recs.Add(cc)
                Next
            Case "p41"
                Dim mq As New BO.myQueryP41
                mq.PIDs = pids
                Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
                For Each c In lis
                    Dim cc As New EmailReceiver(c.PID, "p41")
                    cc.RecordName = c.FullName
                    If c.p28ID_Client > 0 Then
                        cc.Emails = String.Join(",", Master.Factory.p28ContactBL.GetList_o32(c.p28ID_Client).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email).Select(Function(p) p.o32Value))
                    End If
                    
                    Dim s As String = String.Join(",", Master.Factory.p30Contact_PersonBL.GetList_J02(c.p28ID_Client, c.PID, False).Where(Function(p) p.j02Email <> "").Select(Function(p) p.j02Email))
                    If s <> "" Then
                        If cc.Emails = "" Then cc.Emails = s Else cc.Emails += "," & s
                    End If
                    recs.Add(cc)
                Next


        End Select

        rp1.DataSource = recs
        rp1.DataBind()

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As EmailReceiver = CType(e.Item.DataItem, EmailReceiver)
        CType(e.Item.FindControl("EntityRecord"), Label).Text = cRec.RecordName
        CType(e.Item.FindControl("Emails"), TextBox).Text = cRec.Emails
        CType(e.Item.FindControl("PID"), HiddenField).Value = cRec.PID.ToString
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Trim(Me.txtBody.Text) = "" Then
                Master.Notify("Chybí obsah (BODY) zprávy.", NotifyLevel.ErrorMessage) : Return
            End If
            If Trim(Me.txtSubject.Text) = "" Then
                Master.Notify("Chybí předmět (SUBJECT) zprávy.", NotifyLevel.ErrorMessage) : Return
            End If
            For Each ri As RepeaterItem In rp1.Items
                Dim strName As String = CType(ri.FindControl("EntityRecord"), Label).Text
                Dim strTo As String = Trim(CType(ri.FindControl("Emails"), TextBox).Text)
                If strTo = "" Then
                    Master.Notify(String.Format("U záznamu [{0}] chybí e-mail příjemce zprávy.", strName), NotifyLevel.ErrorMessage) : Return
                End If
            Next

            Dim strErrs As String = "", x As Integer = 0

            For Each ri As RepeaterItem In rp1.Items
                Dim message As New Rebex.Mail.MailMessage

                Dim intPID As Integer = CInt(CType(ri.FindControl("PID"), HiddenField).Value)

                With message
                    If Me.txtBody.Text.IndexOf("%]") > 0 Or Me.txtSubject.Text.IndexOf("%]") > 0 Then
                        Dim cM As New BO.clsMergeContent(), objects As New List(Of Object)
                        Select Case Me.CurrentPrefix
                            Case "p28"
                                objects.Add(Master.Factory.p28ContactBL.Load(intPID))
                            Case "p41"
                                objects.Add(Master.Factory.p41ProjectBL.Load(intPID))
                            Case "j02"
                                objects.Add(Master.Factory.j02PersonBL.Load(intPID))
                        End Select

                        .BodyText = cM.MergeContent(objects, Me.txtBody.Text, "")
                        .Subject = cM.MergeContent(objects, Me.txtSubject.Text, "")
                    Else
                        .BodyText = Trim(Me.txtBody.Text)
                        .Subject = Trim(Me.txtSubject.Text)
                    End If

                    .ReplyTo = Master.Factory.SysUser.PersonEmail
                End With


                Dim strTo As String = CType(ri.FindControl("Emails"), TextBox).Text

                Dim recipients As New List(Of BO.x43MailQueue_Recipient)
                strTo = Replace(strTo, ",", ";")
                Dim lisTo As List(Of String) = BO.BAS.ConvertDelimitedString2List(strTo, ";")
                For Each strOneTo As String In lisTo
                    Dim cX43 As New BO.x43MailQueue_Recipient()
                    cX43.x43Email = strOneTo
                    cX43.x43RecipientFlag = BO.x43RecipientIdEnum.recTO
                    recipients.Add(cX43)
                Next



                With Master.Factory.x40MailQueueBL
                    .CompleteMailAttachments(message, upload1.GUID)
                    Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, Me.CurrentX29ID, intPID, BO.x40StateENUM.InQueque, 0)
                    If intMessageID = 0 Then
                        strErrs += "<hr>" & .ErrorMessage
                    Else
                        x += 1
                    End If
                End With

            Next
            If strErrs = "" Then
                Master.CloseAndRefreshParent()
            Else
                If x > 0 Then
                    strErrs = String.Format("Počet zpráv k odeslání bez chyby: {0}.", x) & strErrs
                End If
                Master.Notify(strErrs, NotifyLevel.WarningMessage)
            End If
        End If
    End Sub
End Class