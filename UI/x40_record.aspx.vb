Public Class x40_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub x40_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("Na vstupu chybí ID zprávy.")
                .HeaderText = "Poštovní zpráva"
                .HeaderIcon = "Images/email_32.png"
                cmdDelete.Visible = .Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
            End With


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        cmdChangeStateOnNeedConfirm.Visible = False
        Dim cRec As BO.x40MailQueue = Master.Factory.x40MailQueueBL.Load(Master.DataPID)

        cmdMSG.Visible = False : cmdEML.Visible = False
        With cRec
            If .x40MessageID <> "" And .x40ArchiveFolder <> "" Then

                cmdMSG.Visible = True : cmdEML.Visible = True
                Dim mail As New Rebex.Mail.MailMessage
                mail.Load(Master.Factory.x35GlobalParam.UploadFolder & "\" & cRec.x40ArchiveFolder & "\" & cRec.x40MessageID & ".eml")
                rpAtt.DataSource = mail.Attachments
                rpAtt.DataBind()
            End If
            Me.DateInsert.Text = BO.BAS.FD(.DateInsert)
            Me.x40SenderName.Text = .x40SenderName & " (" & .x40SenderAddress & ")"
            Me.x40Recipient.Text = .x40Recipient
            Me.x40CC.Text = .x40CC
            Me.x40BCC.Text = .x40BCC

            Me.x40Subject.Text = .x40Subject
            Me.x40Body.Text = BO.BAS.CrLfText2Html(.x40Body)
            Me.x40ErrorMessage.Text = .x40ErrorMessage
            Me.x40State.Text = .StatusAlias

            Select Case .x40State
                Case BO.x40StateENUM.InQueque
                    Me.x40State.Text += " (každých 5 minut systém pošle 10 zpráv)"
                    cmdChangeState.Text = "Změnit stav zprávy na [Zastaveno]" : cmdChangeState.CommandArgument = "4"
                    cmdChangeStateOnNeedConfirm.Visible = True
                Case BO.x40StateENUM.IsError
                    Me.x40State.Text += " (při pokusu o odeslání došlo k chybě)"
                    Me.x40State.ForeColor = Drawing.Color.Red
                    cmdChangeState.Text = "Změnit stav zprávy na [Odeslat]" : cmdChangeState.CommandArgument = "1"
                    cmdChangeStateOnNeedConfirm.Visible = True
                Case BO.x40StateENUM.IsProceeded
                    Me.x40State.Text += " (zpracováno v čase: " & BO.BAS.FD(.x40WhenProceeded, True) & ")"
                    cmdChangeState.Visible = False
                Case BO.x40StateENUM.IsStopped
                    Me.x40State.ForeColor = Drawing.Color.Magenta
                    cmdChangeState.Text = "Změnit stav zprávy na [Odeslat]" : cmdChangeState.CommandArgument = "1"
                    cmdChangeStateOnNeedConfirm.Visible = True
                Case BO.x40StateENUM.WaitOnConfirm
                    Me.x40State.Text += " (čeká se, až někdo odešle zprávu)"
                    cmdChangeState.Text = "Změnit stav zprávy na [Odeslat]" : cmdChangeState.CommandArgument = "1"
                    cmdChangeStateOnNeedConfirm.Visible = False
            End Select
            Me.Timestamp.Text = .Timestamp
        End With


    End Sub

    Private Sub cmdChangeState_Click(sender As Object, e As EventArgs) Handles cmdChangeState.Click
        Dim newState As BO.x40StateENUM = BO.x40StateENUM._NotSpecified
        Select Case Me.cmdChangeState.CommandArgument
            Case "4"
                newState = BO.x40StateENUM.IsStopped
            Case "1"
                newState = BO.x40StateENUM.InQueque
            Case "5"
                newState = BO.x40StateENUM.WaitOnConfirm
        End Select
        If Master.Factory.x40MailQueueBL.UpdateMessageState(Master.DataPID, newState) Then
            Master.CloseAndRefreshParent("save")
        End If
    End Sub

    Private Sub cmdChangeStateOnNeedConfirm_Click(sender As Object, e As EventArgs) Handles cmdChangeStateOnNeedConfirm.Click
        If Master.Factory.x40MailQueueBL.UpdateMessageState(Master.DataPID, BO.x40StateENUM.WaitOnConfirm) Then
            Master.CloseAndRefreshParent("save")
        Else
            Master.Notify(Master.Factory.x40MailQueueBL.ErrorMessage)
        End If
    End Sub

    Private Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
        With Master.Factory.x40MailQueueBL
            If .Delete(Master.DataPID) Then
                Master.CloseAndRefreshParent("delete")
            Else
                Master.Notify(Master.Factory.x40MailQueueBL.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub cmdEML_Click(sender As Object, e As EventArgs) Handles cmdEML.Click
        Response.Redirect("binaryfile.aspx?prefix=x40-eml&pid=" & Master.DataPID.ToString)
    End Sub

    Private Sub cmdMSG_Click(sender As Object, e As EventArgs) Handles cmdMSG.Click
        Response.Redirect("binaryfile.aspx?prefix=x40-msg&pid=" & Master.DataPID.ToString)
    End Sub

    Private Sub rpAtt_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpAtt.ItemDataBound
        Dim cRec As Rebex.Mail.Attachment = CType(e.Item.DataItem, Rebex.Mail.Attachment)
        With CType(e.Item.FindControl("linkAtt"), HyperLink)
            .Text = cRec.DisplayName
            If .Text = "" Then .Text = cRec.FileName
            .Text += " (" & BO.BAS.FormatFileSize(cRec.GetContentLength) & ")"
            .NavigateUrl = "binaryfile.aspx?prefix=x40-eml&pid=" & Master.DataPID.ToString & "&att=" & cRec.FileName
        End With
        Dim cF As New BO.clsFile

        CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/Files/" & BO.BAS.GetFileExtensionIcon(Right(cRec.FileName, 4))
    End Sub
End Class