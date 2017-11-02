Public Class barcode
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub barcode_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidRecordPID.Value = Request.Item("pid")
            hidRecordPrefix.Value = Request.Item("prefix")
            If hidRecordPrefix.Value <> "" Then
                lblHeader.Text = Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.hidRecordPrefix.Value), BO.BAS.IsNullInt(hidRecordPID.Value))
            End If

            Dim lisPars As New List(Of String)
            With lisPars
                .Add("barcode-type")
                .Add("barcode-show-text")
                .Add("barcode-show-checksum")

            End With

            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)
                basUI.SelectDropdownlistValue(Me.cbxType, .GetUserParam("barcode-type", "23"))
                chkShowTextBellow.Checked = BO.BAS.BG(.GetUserParam("barcode-show-text", "0"))
                chkShowChecksum.Checked = BO.BAS.BG(.GetUserParam("barcode-show-checksum", "0"))

            End With
            If Me.hidRecordPrefix.Value <> "" Then
                txt1.Text = hidRecordPrefix.Value & "|" & hidRecordPID.Value
            End If
            With Master.Factory
                Select Case Me.hidRecordPrefix.Value
                    Case "j02"
                        Dim c As BO.j02Person = .j02PersonBL.Load(BO.BAS.IsNullInt(Me.hidRecordPID.Value))
                        txt1.Text += "|" & c.FullNameAsc
                    Case "p41"
                        Dim c As BO.p41Project = .p41ProjectBL.Load(BO.BAS.IsNullInt(Me.hidRecordPID.Value))
                        txt1.Text += "|" & c.FullName
                    Case "p28"
                        Dim c As BO.p28Contact = .p28ContactBL.Load(BO.BAS.IsNullInt(Me.hidRecordPID.Value))
                        txt1.Text += "|" & c.p28Name
                    Case "p91"
                        Dim c As BO.p91Invoice = .p91InvoiceBL.Load(BO.BAS.IsNullInt(Me.hidRecordPID.Value))
                        txt1.Text += "|" & c.p91Code & "|" & c.p91Client
                End Select
            End With


            RenderBC()
        End If
    End Sub


    Private Sub RenderBC()
        If Trim(txt1.Text) = "" Then txt1.Text = "1234"
        bc1.Text = txt1.Text
        bc1.Type = CType(CInt(Me.cbxType.SelectedValue), Telerik.Web.UI.BarcodeType)
        ''bc1.Type = DirectCast([Enum].Parse(GetType(Telerik.Web.UI.BarcodeType), cbxType.SelectedItem.Text, True), Telerik.Web.UI.BarcodeType)
        If bc1.Type <> Telerik.Web.UI.BarcodeType.QRCode And bc1.Type <> Telerik.Web.UI.BarcodeType.PDF417 Then
            bc1.ShowText = Me.chkShowTextBellow.Checked
            bc1.ShowChecksum = Me.chkShowChecksum.Checked
        Else
            bc1.ShowText = False
            bc1.ShowChecksum = False
        End If

    End Sub

    Private Sub barcode_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Select Case bc1.Type
            Case Telerik.Web.UI.BarcodeType.QRCode
                chkShowTextBellow.Visible = False
                chkShowChecksum.Visible = False
                txt1.TextMode = TextBoxMode.MultiLine
                txt1.Style.Item("height") = "70px"
            Case Else
                chkShowTextBellow.Visible = True
                chkShowChecksum.Visible = True
                txt1.TextMode = TextBoxMode.SingleLine
                txt1.Style.Item("height") = ""
        End Select
        If hidRecordPrefix.Value = "" Then
            trFormat.Visible = False
        Else
            trFormat.Visible = True
        End If
    End Sub

    Private Sub cbxType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxType.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("barcode-type", Me.cbxType.SelectedValue)
        RenderBC()

    End Sub

    Private Sub chkShowChecksum_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowChecksum.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("barcode-show-checksum", BO.BAS.GB(Me.chkShowChecksum.Checked))
        RenderBC()
    End Sub

    Private Sub chkShowTextBellow_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowTextBellow.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("barcode-show-text", BO.BAS.GB(Me.chkShowTextBellow.Checked))
        RenderBC()
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        RenderBC()
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        RenderBC()
        Dim s As String = BO.BAS.Prepare4FileName(Me.txt1.Text) & ".png"
        bc1.GetImage().Save(Master.Factory.x35GlobalParam.TempFolder & "\" & s, System.Drawing.Imaging.ImageFormat.Png)
        Response.Redirect("binaryfile.aspx?tempfile=" & s)

    End Sub
End Class