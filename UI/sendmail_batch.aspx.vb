Public Class sendmail_batch
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

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

            Me.txtSubject.Text = c.j61PlainTextBody
            Me.txtBody.Text = c.j61MailSubject
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
        'Select Case Me.CurrentPrefix
        '    Case "p28"
        '        Dim mq As New BO.myQueryP28
        '        mq.PIDs = pids
        '        Dim lis As IEnumerable(Of BO.p28Contact) = Master.Factory.p28ContactBL.GetList(mq)
        '    Case "p41"
        '        hidMasterPrefix_p30.Value = "p41"
        '        hidMasterPID_p30.Value = Master.DataPID.ToString
        '    Case "p31"
        '        Dim cP31 As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        '        Me.txtSubject.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, Master.DataPID, True)
        '        If Me.txtTo.Text = "" Then
        '            Me.txtTo.Text = Master.Factory.j02PersonBL.Load(cP31.j02ID).j02Email
        '        End If
        '        Me.txtBody.Text = ""
        '    Case "p91"
        '        Dim cP91 As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
        '        hidMasterPID_p30.Value = cP91.p28ID.ToString
        '        hidMasterPrefix_p30.Value = "p28"
        '        Me.txtSubject.Text = String.Format("Faktura {0} | {1}", cP91.p91Code, cP91.p91Client)
        '        Dim tos As New List(Of String)
        '        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(cP91.p28ID).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email And p.o32IsDefaultInInvoice = True)
        '        For Each c In lisO32
        '            tos.Add(c.o32Value)
        '        Next
        '        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(cP91.p28ID, cP91.p41ID_First, False).Where(Function(p) p.j02IsInvoiceEmail = True)
        '        For Each c In lisJ02
        '            tos.Add(c.j02Email)
        '        Next

        '        If cP91.j02ID_ContactPerson <> 0 Then
        '            Dim s As String = Master.Factory.j02PersonBL.Load(cP91.j02ID_ContactPerson).j02Email
        '            If s <> "" Then tos.Add(s)
        '        End If
        '        If tos.Count > 0 Then
        '            Me.txtTo.Text = String.Join(",", tos.Distinct)
        '        End If
        '        If cP91.p28ID <> 0 Then
        '            Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(cP91.p28ID)
        '            If cP28.j61ID_Invoice <> 0 Then
        '                basUI.SelectDropdownlistValue(Me.j61ID, cP28.j61ID_Invoice)
        '                Handle_ChangeJ61ID()
        '            End If
        '        End If


        'End Select
    End Sub
End Class