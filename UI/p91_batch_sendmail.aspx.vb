Public Class p91_batch_sendmail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private Property _lisX31 As IEnumerable(Of BO.x31Report)
    Private Property _lisJ61 As IEnumerable(Of BO.j61TextTemplate)


    Private Sub p91_batch_print_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            System.Threading.Thread.Sleep(3000) 'počkat 3 sekundy než se na klientovi uloží seznam PIDů
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.LoadByGUID("p91_batch_sendmail-pids-" & Master.Factory.SysUser.PID.ToString)
            If cRec Is Nothing Then
                Master.StopPage("pids is missing.")
            End If
            If cRec.p85Message = "" Then
                Master.StopPage("Na vstupu chybí vybrané záznamy faktur.")
            End If
            ViewState("pids") = cRec.p85Message
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p91_batch_sendmail-body")
                .Add("p91_batch_sendmail-chkAtt")
                .Add("p91_batch_sendmail-subject")
            End With
            With Master
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                .HeaderText = "Hromadné odeslání faktur elektronickou poštou"
                .HeaderIcon = "Images/email_32.png"
                .AddToolbarButton("Zařadit do fronty k odeslání", "ok", , "Images/ok.png", , , , True)
            End With

            With Master.Factory.j03UserBL
                Me.txtBody.Text = .GetUserParam("p91_batch_sendmail-body")
                Me.txtSubjectPre.Text = .GetUserParam("p91_batch_sendmail-subject")
                Me.chkAtt.Checked = BO.BAS.BG(.GetUserParam("p91_batch_sendmail-chkAtt", "1"))
            End With

            Dim mqP91 As New BO.myQueryP91
            mqP91.PIDs = BO.BAS.ConvertPIDs2List(cRec.p85Message)
            Dim lis As IEnumerable(Of BO.p91Invoice) = Master.Factory.p91InvoiceBL.GetList(mqP91)

            _lisX31 = Master.Factory.x31ReportBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p91Invoice And p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik Or p.x31FormatFlag = BO.x31FormatFlagENUM.DOCX)

            rp1.DataSource = lis
            rp1.DataBind()
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        If _lisJ61 Is Nothing Then _lisJ61 = Master.Factory.j61TextTemplateBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p91Invoice)
        Dim cRec As BO.p91Invoice = CType(e.Item.DataItem, BO.p91Invoice)
        CType(e.Item.FindControl("p91ID"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("p91Code"), Label).Text = cRec.p91Code
        CType(e.Item.FindControl("txtSubject"), TextBox).Text = String.Format("Faktura č. {0}", cRec.p91Code)
        With CType(e.Item.FindControl("rep1"), DropDownList)
            If .Items.Count = 0 Then
                .DataSource = _lisX31
                .DataBind()
                .Items.Insert(0, "")
            End If
        End With
        With CType(e.Item.FindControl("rep2"), DropDownList)
            If .Items.Count = 0 Then
                .DataSource = _lisX31
                .DataBind()
                .Items.Insert(0, "")
            End If
        End With
        With CType(e.Item.FindControl("j61ID"), DropDownList)
            If .Items.Count = 0 Then
                .DataSource = _lisJ61
                .DataBind()
                .Items.Insert(0, "Výchozí text")
            End If
        End With
        If cRec.p28ID <> 0 Then
            Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(cRec.p28ID)
            If cP28.j61ID_Invoice <> 0 Then
                basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j61ID"), DropDownList), cP28.j61ID_Invoice.ToString)
            End If
        End If
        Dim cP92 As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(cRec.p92ID)
        If cP92.x31ID_Invoice > 0 Then basUI.SelectDropdownlistValue(CType(e.Item.FindControl("rep1"), DropDownList), cP92.x31ID_Invoice.ToString)
        If cP92.x31ID_Attachment > 0 Then basUI.SelectDropdownlistValue(CType(e.Item.FindControl("rep2"), DropDownList), cP92.x31ID_Attachment.ToString)


        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(cRec.p28ID).Where(Function(p) p.o33ID = BO.o33FlagEnum.Email And p.o32IsDefaultInInvoice = True)
        Dim tos As New List(Of String)
        For Each c In lisO32
            tos.Add(c.o32Value)
        Next
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(cRec.p28ID, cRec.p41ID_First, False).Where(Function(p) p.j02IsInvoiceEmail = True)
        For Each c In lisJ02
            tos.Add(c.j02Email)
        Next
        If cRec.j02ID_ContactPerson <> 0 Then
            Dim s As String = Master.Factory.j02PersonBL.Load(cRec.j02ID_ContactPerson).j02Email
            If s <> "" Then tos.Add(s)
        End If
        If tos.Count > 0 Then
            CType(e.Item.FindControl("receiver"), email_receiver).Text = String.Join(",", tos.Distinct)
        End If

    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            For Each ri As RepeaterItem In rp1.Items
                Dim strCode As String = CType(ri.FindControl("p91Code"), Label).Text
                Dim strTo As String = Trim(CType(ri.FindControl("receiver"), email_receiver).Text)
                If strTo = "" Then
                    Master.Notify(String.Format("U faktury {0} chybí e-mail příjemce zprávy.", strCode), NotifyLevel.ErrorMessage) : Return
                End If
            Next
            With Master.Factory.j03UserBL
                .SetUserParam("p91_batch_sendmail-body", Me.txtBody.Text)
                .SetUserParam("p91_batch_sendmail-subject", Me.txtSubjectPre.Text)
                .SetUserParam("p91_batch_sendmail-chkAtt", BO.BAS.GB(Me.chkAtt.Checked))
            End With
            Dim strErrs As String = "", x As Integer = 0
            
            For Each ri As RepeaterItem In rp1.Items
                Dim message As New Rebex.Mail.MailMessage
                Dim intJ61ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("j61ID"), DropDownList).SelectedValue)
                Dim intP91ID As Integer = CInt(CType(ri.FindControl("p91ID"), HiddenField).Value)

                With message
                    If intJ61ID = 0 Then
                        .BodyText = Trim(Me.txtBody.Text)
                    Else
                        'text ze šablony
                        Dim cM As New BO.clsMergeContent(), objects As New List(Of Object)
                        objects.Add(Master.Factory.p91InvoiceBL.Load(intP91ID))
                        .BodyText = cM.MergeContent(objects, Master.Factory.j61TextTemplateBL.Load(intJ61ID).j61PlainTextBody, Master.Factory.GetRecordLinkUrl("p91", intP91ID))
                    End If

                    .Subject = Trim(Me.txtSubjectPre.Text & " " & CType(ri.FindControl("txtSubject"), TextBox).Text)
                    .ReplyTo = Master.Factory.SysUser.PersonEmail
                End With


                Dim strTo As String = CType(ri.FindControl("receiver"), email_receiver).Text
                Dim intRep1 As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("rep1"), DropDownList).SelectedValue)
                Dim intRep2 As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("rep2"), DropDownList).SelectedValue)
                Dim strP91Code As String = CType(ri.FindControl("p91Code"), Label).Text

                If intRep1 <> 0 Then
                    Dim strOutputFile As String = Master.Factory.GetRecordFileName(BO.x29IdEnum.p91Invoice, intP91ID, "pdf", False, intRep1)
                    message.Attachments.Add(New Rebex.Mail.Attachment(GenerateReportOnBehind(intRep1, intP91ID, strOutputFile)))

                End If
                If intRep2 <> 0 And intRep2 <> intRep1 And chkAtt.Checked = True Then
                    Dim strOutputFile As String = Master.Factory.GetRecordFileName(BO.x29IdEnum.p91Invoice, intP91ID, "pdf", False, intRep2)
                    message.Attachments.Add(New Rebex.Mail.Attachment(GenerateReportOnBehind(intRep2, intP91ID, strOutputFile)))

                End If

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
                    Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, BO.x29IdEnum.p91Invoice, intP91ID, BO.x40StateENUM.InQueque, 0)
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

    Private Function GenerateReportOnBehind(intX31ID As Integer, intDataPID As Integer, strOutputFileName As String) As String
        'vrací plnou cestu na vygenerovaný PDF dokument
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(intX31ID)
        If cRec Is Nothing Then Return ""

        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName
        Dim cRep As New clsReportOnBehind()
        cRep.Query_RecordPID = intDataPID

        strOutputFileName = cRep.GenerateReport2Temp(Master.Factory, strRepFullPath, , strOutputFileName)
        If strOutputFileName = "" Then Return ""


        ''Dim cTemp As New BO.p85TempBox(), cF As New BO.clsFile, strGUID As String = BO.BAS.GetGUID
        ''Dim lisO13 As IEnumerable(Of BO.o13AttachmentType) = Master.Factory.o13AttachmentTypeBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.x40MailQueue)
        ''With cTemp
        ''    If lisO13.Count > 0 Then
        ''        .p85OtherKey1 = lisO13(0).PID
        ''        .p85FreeText06 = lisO13(0).o13Name
        ''    End If
        ''    .p85GUID = strGUID
        ''    .p85FreeText01 = strOutputFileName
        ''    .p85FreeText02 = strOutputFileName
        ''    .p85FreeText03 = "application/pdf"
        ''    .p85FreeText04 = "PDF report"
        ''    .p85FreeNumber01 = cF.GetFileSize(Master.Factory.x35GlobalParam.TempFolder & "\" & strOutputFileName)
        ''End With
        ''Master.Factory.p85TempBoxBL.Save(cTemp)

        Return Master.Factory.x35GlobalParam.TempFolder & "\" & strOutputFileName
    End Function

    Private Sub p91_batch_sendmail_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        For Each ri As RepeaterItem In rp1.Items
            ri.FindControl("rep2").Visible = chkAtt.Checked
        Next
    End Sub
End Class