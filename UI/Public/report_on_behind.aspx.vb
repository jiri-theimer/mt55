Imports Telerik.Reporting
Public Class report_on_behind
    Inherits System.Web.UI.Page

    Private _factory As BL.Factory

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _factory = New BL.Factory(, "mtservice")
        If _factory.SysUser Is Nothing Then
            lblMessage.Text = "Service user is not inhaled!"
            Return
        End If
        Dim intX31ID As Integer = BO.BAS.IsNullInt(Request.Item("x31id"))
        If intX31ID = 0 Then
            lblMessage.Text = "x31id is missing!"
            Return
        End If
        Dim cRec As BO.x31Report = _factory.x31ReportBL.Load(intX31ID)
        If cRec Is Nothing Then
            lblMessage.Text = "Report instance is null!" : Return
        End If

        If cRec.x31FormatFlag <> BO.x31FormatFlagENUM.Telerik Then
            lblMessage.Text = "Report format is not TRDX!" : Return
        End If
        Dim strRepFullPath As String = _factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName
        Dim cRep As New clsReportOnBehind()

        Dim strOutputFileName As String = cRep.GenerateReport2Temp(_factory, strRepFullPath)

        lblMessage.Text = Now.ToString & " | Generated report: " & strOutputFileName

        'sestava je vygenerována v temp složce

        SendReportByMail(strOutputFileName)
    End Sub

   

    Private Sub SendReportByMail(strOutputFileName As String)
        Dim message As New Rebex.Mail.MailMessage
        With message
            .BodyText = "Obsah zprávy: " & strOutputFileName
            .Subject = "Předmět zprávy: " & strOutputFileName
            '.AddOneFile2FullPath(_factory.x35GlobalParam.TempFolder & "\" & strOutputFileName)
        End With
        Dim recipients As New List(Of BO.x43MailQueue_Recipient)
        'Dim cX43 As New BO.x43MailQueue_Recipient("jiri.theimer@gmail.com", "TIMER", BO.x43RecipientIdEnum.recTO)
        'recipients.Add(cX43)
        'cX43 = New BO.x43MailQueue_Recipient("info@marktime.sk", "SK", BO.x43RecipientIdEnum.recBCC)
        'recipients.Add(cX43)
        'cX43 = New BO.x43MailQueue_Recipient("jtheimer@marktime.cz", "", BO.x43RecipientIdEnum.recCC)
        'recipients.Add(cX43)
        'cX43 = New BO.x43MailQueue_Recipient("jiri.theimer@itelligence.cz", "", BO.x43RecipientIdEnum.recTO)
        'recipients.Add(cX43)

        With _factory.x40MailQueueBL
            Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, BO.x29IdEnum.j02Person, _factory.SysUser.j02ID, BO.x40StateENUM.InQueque, 0)
            If intMessageID > 0 Then
                If Not .SendMessageFromQueque(intMessageID) Then
                    Response.Write(.ErrorMessage)
                End If
            Else
                Response.Write(.ErrorMessage)
            End If
        End With
      
    End Sub
End Class