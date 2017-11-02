Public Class imap_record
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(cRec As BO.o43ImapRobotHistory)
        If cRec Is Nothing Then
            cmdEML.Text = "????" : cmdMSG.Text = "????" : Return
        End If
        Me.hidGUID.Value = cRec.o43RecordGUID
        With cRec
            Me.Sender.Text = "<a href='mailto:" & .o43FROM & "'>" & .o43FROM & "</a> " & .o43FROM_DisplayName
            Me.Timestamp.Text = BO.BAS.FD(.o43DateMessage, True, True)
            Me.Subject.Text = .o43Subject

            cmdMSG.NavigateUrl = "binaryfile.aspx?format=msg&prefix=o43&guid=" & .o43RecordGUID
            cmdEML.NavigateUrl = "binaryfile.aspx?format=eml&prefix=o43&guid=" & .o43RecordGUID
            If .o43Attachments <> "" And cRec.p56ID <> 0 Then
                Dim a() As String = Split(.o43Attachments, ";") 'attachmenty z poštovní zprávy se zobrazují pouze u úkolu. U dokumentu jsou přílohy v rámci o27
                rpIMAPAttachments.DataSource = a
                rpIMAPAttachments.DataBind()
                lblAttHeader.Text = BO.BAS.OM2(Me.lblAttHeader.Text, rpIMAPAttachments.Items.Count.ToString)
            Else
                lblAttHeader.Visible = False
            End If
        End With

        
    End Sub

    Private Sub rpIMAPAttachments_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpIMAPAttachments.ItemDataBound
        Dim s As String = Trim(CType(e.Item.DataItem, String))
        With CType(e.Item.FindControl("att1"), HyperLink)
            .Text = s
            .NavigateUrl = "binaryfile.aspx?prefix=o43&guid=" & Me.hidGUID.Value & "&att=" & s
        End With
        CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/Files/" & BO.BAS.GetFileExtensionIcon(Right(s, 4))
    End Sub
End Class