Public Class import_object
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub import_object_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory
        io1.Factory = Master.Factory
        Master.SiteMenuValue = "dashboard"
        If Not Page.IsPostBack Then
            If Request.Item("prefix") = "" Or Request.Item("guid") = "" Then
                Master.StopPage("Na vstupu chybí identifikace vstupního objektu (prefix a guid).")
                Return
            End If

            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID
            hidGUID.Value = Request.Item("guid")
            hidPrefix.Value = Request.Item("prefix")
            Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID.Value)
            If lis.Count = 0 Then
                Master.StopPage("Objekt pro tento klíč neexistuje.")
            End If
            If lis.Where(Function(p) p.p85FreeText04 = "MS-Outlook").Count > 0 Then
                linkMSG.NavigateUrl = "binaryfile.aspx?tempfile=" & lis.Where(Function(p) p.p85FreeText04 = "MS-Outlook")(0).p85FreeText02
            Else
                linkMSG.Visible = False
            End If
            If lis.Where(Function(p) p.p85FreeText01 = "p41id").Count > 0 Then
                hidP41ID.Value = lis.Where(Function(p) p.p85FreeText01 = "p41id")(0).p85OtherKey1.ToString
            End If

            Dim strURL As String = ""
            tabs1.Tabs(1).Visible = False : RadMultiPage1.PageViews(1).Visible = False
            io1.InhaleObjectRecord(hidGUID.Value, hidPrefix.Value, True)
            Select Case hidPrefix.Value
                Case "p31"
                    tabs1.Tabs(0).Text = "Zapsat worksheet úkon"
                    tabs1.Tabs(0).ImageUrl = "Images/worksheet.png"
                    strURL = "p31_record.aspx?pid=0&guid_import=" & hidGUID.Value
                    If hidP41ID.Value <> "" Then
                        strURL += "&p41id=" & hidP41ID.Value
                    End If
                    Me.Subject.Text = BO.BAS.FD(io1.FindDate("p31Date"), False, True)
                    Me.Body.Text = BO.BAS.CrLfText2Html(io1.FindString("p31Text"))
                Case "o23"
                    tabs1.Tabs(1).Visible = True : RadMultiPage1.PageViews(1).Visible = True
                    tabs1.Tabs(0).Text = "Vytvořit dokument"
                    panO23.Visible = True
                    Dim lisX18 As IEnumerable(Of BO.x18EntityCategory) = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery, BO.x29IdEnum._NotSpecified).Where(Function(p) p.x18IsManyItems = True)
                    Me.x18ID.DataSource = lisX18
                    Me.x18ID.DataBind()
                    If lisX18.Count = 0 Then
                        Master.Notify("V databázi ani jeden mně dostupný typ dokumentu.")
                    End If

                    Me.b07Value.Text = io1.FindString("o23Name")
                    Me.b07Value_Mail.Text = io1.FindString("o23Name")
                    If Me.b07Value.Text <> "" And io1.FindString("o23BigText") <> "" Then Me.b07Value.Text += vbCrLf & vbCrLf
                    Me.b07Value.Text += io1.FindString("o23BigText")
                    Me.opgBodyFormat.Items.FindByValue("1").Text += " (" & Len(Me.b07Value.Text).ToString & " znaků)"
                    If io1.FindString("HTMLBody") <> "" Then
                        Dim cF As New BO.clsFile
                        Me.b07BodyHTML.Text = cF.GetFileContents(Master.Factory.x35GlobalParam.TempFolder & "\" & io1.FindString("HTMLBody"))
                        opgBodyFormat.Items.FindByValue("2").Text += " (" & Len(Me.b07BodyHTML.Text).ToString & " znaků)"
                    Else
                        opgBodyFormat.Items.FindByValue("2").Enabled = False
                    End If
                    If Not io1.IsMailMessage() Then
                        opgBodyFormat.Items.FindByValue("3").Enabled = False
                        opgBodyFormat.Items.FindByValue("1").Selected = True
                    End If
                    panObject.Visible = False
                    'Me.Subject.Text = io1.FindString("o23Name")
                    'Me.Body.Text = BO.BAS.CrLfText2Html(io1.FindString("o23BigText"))
                    io1.PrepareTempFileUpload(upload1.GUID)
                    uploadlist1.RefreshData_TEMP()
                Case "p28"
                    tabs1.Tabs(0).Text = "Založit klienta"
                    tabs1.Tabs(0).ImageUrl = "Images/contact.png"
                    strURL = "p28_record.aspx?pid=0&guid_import=" & hidGUID.Value
                    Me.Subject.Text = io1.FindString("p28CompanyName")
                Case "p56"
                    tabs1.Tabs(0).Text = "Vytvořit úkol"
                    tabs1.Tabs(0).ImageUrl = "Images/task.png"
                    strURL = "p56_record.aspx?pid=0&masterprefix=p41&masterpid=0&guid_import=" & hidGUID.Value
                    If hidP41ID.Value <> "" Then
                        strURL += "&p41id=" & hidP41ID.Value
                    End If
                    Me.Subject.Text = io1.FindString("p56Name")
                    Me.Body.Text = BO.BAS.CrLfText2Html(io1.FindString("p56Description"))
            End Select
            hidPopupUrl.Value = strURL
        End If

    End Sub

    Private Sub x18ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x18ID.SelectedIndexChanged
        hidPopupUrl.Value = "o23_record.aspx?x18id=" & Me.x18ID.SelectedValue & "&guid_import=" & Me.hidGUID.Value
        If hidP41ID.Value <> "" Then
            hidPopupUrl.Value += "&p41id=" & hidP41ID.Value
        End If
    End Sub

    Private Sub import_object_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If hidPopupUrl.Value <> "" Then
            cmdPopup.Visible = True
        Else
            cmdPopup.Visible = False
        End If

        search_p41.Visible = False : search_p28.Visible = False
        Select Case Me.opgSearch.SelectedValue
            Case "p41"
                search_p41.Visible = True
            Case "p28"
                search_p28.Visible = True
        End Select
        Me.panBodyHTML.Visible = False : Me.b07Value.Visible = False : Me.b07Value_Mail.Visible = False

        Select Case Me.opgBodyFormat.SelectedValue
            Case "1"
                Me.b07Value.Visible = True
            Case "2"
                Me.panBodyHTML.Visible = True
            Case "3"    'pošta 1:1
                Me.b07Value_Mail.Visible = True
        End Select
       
    End Sub

    Private Sub cmdSaveB07_Click(sender As Object, e As EventArgs) Handles cmdSaveB07.Click
        Dim cRec As New BO.b07Comment
        With cRec

            Select Case Me.opgSearch.SelectedValue
                Case "p41"
                    .x29ID = BO.x29IdEnum.p41Project
                    .b07RecordPID = BO.BAS.IsNullInt(search_p41.SelectedValue)
                Case "p28"
                    .x29ID = BO.x29IdEnum.p28Contact
                    .b07RecordPID = BO.BAS.IsNullInt(search_p28.SelectedValue)
            End Select
            Select Case Me.opgBodyFormat.SelectedValue
                Case "1"
                    .b07Value = Me.b07Value.Text
                Case "2"
                    .b07Value = Me.b07BodyHTML.Text
                Case "3"
                    .b07Value = Me.b07Value_Mail.Text

            End Select
            If .b07RecordPID = 0 Then
                Master.Notify("Musíte vybrat projekt nebo klienta.", NotifyLevel.ErrorMessage)
                Return
            End If
            If Trim(.b07Value) = "" Then
                Master.Notify("Chybí název/obsah komentáře.", NotifyLevel.ErrorMessage)
                Return
            End If
        End With


        With Master.Factory.b07CommentBL
            If .Save(cRec, upload1.GUID, Nothing) Then
                Response.Redirect(Me.opgSearch.SelectedValue & "_framework.aspx?pid=" & cRec.b07RecordPID.ToString)
            Else
                Master.Notify(Master.Factory.b07CommentBL.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub
End Class