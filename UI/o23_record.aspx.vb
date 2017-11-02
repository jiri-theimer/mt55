Imports Telerik.Web.UI

Public Class o23_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord
    Private Property _curRec As BO.o23Doc
    Private Property _loadingLastX20ID As Integer


    Public ReadOnly Property CurrentX18ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidX18ID.Value)
        End Get
    End Property
    Public ReadOnly Property CurrentX20ID As Integer
        Get
            If opgX20ID.SelectedItem Is Nothing Then Return 0
            Return BO.BAS.IsNullInt(opgX20ID.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            If hidX29ID.Value = "" Then
                Return BO.x29IdEnum._NotSpecified
            Else
                Return CType(CInt(hidX29ID.Value), BO.x29IdEnum)
            End If
        End Get
    End Property

    Private Sub o23_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        io1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID
            
            With Master
                .HeaderIcon = "Images/label_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Dokument"
                hidGUID_x19.Value = BO.BAS.GetGUID()
                Me.x23ID.DataSource = .Factory.x23EntityField_ComboBL.GetList(New BO.myQuery)
                Me.x23ID.DataBind()

                If Request.Item("x23id") <> "" Then
                    Me.x23ID.SelectedValue = Request.Item("x23id")
                End If
                If Request.Item("x18id") <> "" Then
                    hidX18ID.Value = Request.Item("x18id")
                    Dim c As BO.x18EntityCategory = .Factory.x18EntityCategoryBL.Load(Me.CurrentX18ID)
                    If c.x31ID_Plugin <> 0 Then
                        'přesměrovat na ASPX pluginu
                        Dim cPlugin As BO.x31Report = .Factory.x31ReportBL.Load(c.x31ID_Plugin)
                        Response.Redirect("/Plugins/" & cPlugin.x31FileName & "?blank=1&pid=" & .DataPID.ToString & "&x18id=" & c.PID.ToString & "&clone=" & Request.Item("clone"), True)
                        Return
                    End If
                    Handle_Permissions(c)

                    Me.x23ID.SelectedValue = c.x23ID.ToString
                    Me.hidx18CalendarFieldStart.Value = c.x18CalendarFieldStart
                    Me.hidx18CalendarFieldEnd.Value = c.x18CalendarFieldEnd
                    Me.hidJavascriptFile.Value = c.x18JavascriptFile

                    .HeaderText = c.x18Name
                    If c.x18Icon32 <> "" Then .HeaderIcon = c.x18Icon32
                    Me.x18Name.Text = c.x18Name

                    panColors.Visible = c.x18IsColors
                    If Not c.x18IsColors Then
                        o23BackColor.Preset = Telerik.Web.UI.ColorPreset.None
                        o23BackColor.Items.Clear()
                        o23ForeColor.Preset = Telerik.Web.UI.ColorPreset.None
                        o23ForeColor.Items.Clear()

                    End If
                    Me.o23IsEncrypted.Visible = c.x18IsAllowEncryption
                    'If Master.DataPID = 0 And c.x18UploadFlag = BO.x18UploadENUM.FileSystemUpload Then
                    If c.x18UploadFlag = BO.x18UploadENUM.FileSystemUpload Then
                        panUpload.Visible = True
                    Else
                        panUpload.Visible = False
                    End If

                    If c.x18EntryNameFlag = BO.x18EntryNameENUM.NotUsed Then
                        Me.lblName.Visible = False : Me.o23Name.Visible = False
                    End If
                    Select Case c.x18EntryCodeFlag
                        Case BO.x18EntryCodeENUM.NotUsed
                            lblo23Code.Visible = False : Me.o23Code.Visible = False
                        Case BO.x18EntryCodeENUM.AutoP41, BO.x18EntryCodeENUM.AutoX18
                            If Master.DataPID = 0 Then
                                Me.o23Code.Visible = False : lblo23Code.Visible = False
                            Else
                                Me.o23Code.Enabled = False
                            End If

                    End Select
                    If c.x18EntryOrdinaryFlag = BO.x18EntryOrdinaryENUM.NotUsed Then
                        lblOrdinary.Visible = False : o23Ordinary.Visible = False
                    End If
                Else
                    If Not (Request.Item("source") = "x18_items" Or Request.Item("source") = "x18_record") Then
                        .neededPermission = BO.x53PermValEnum.GR_Admin
                    End If
                End If
                If Me.x23ID.SelectedIndex > 0 Then
                    lblX23ID.Visible = False
                    Me.x23ID.Visible = False
                End If




            End With


            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.o23Code.Text = ""
                Me.o23Name.Text += " KOPIE"
                filesPreview.Visible = False
            End If

            If Request.Item("guid_import") <> "" Then
                io1.InhaleObjectRecord(Request.Item("guid_import"), "o23", False)
                io1.PrepareTempFileUpload(upload1.GUID)
                uploadlist1.RefreshData_TEMP()
                panUpload.Visible = True
            End If
        End If
        If Not panHtmlEditor.Visible Then
            panHtmlEditor.Controls.Clear()
        End If
        If Not panUpload.Visible Then
            panUpload.Controls.Clear()
        End If
    End Sub

    

    Private Sub Handle_Permissions(c As BO.x18EntityCategory)
        Dim cDisp As BO.x18RecordDisposition = Master.Factory.x18EntityCategoryBL.InhaleDisposition(c)
        With cDisp
            If Master.DataPID = 0 Then
                If Not .CreateItem Then
                    Master.StopPage(String.Format("Nemáte oprávnění zakládat nové dokumenty pro typ [{0}].", c.x18Name), True)
                End If
            Else
                If Not .OwnerItems Then
                    Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
                    If cRec.j02ID_Owner <> Master.Factory.SysUser.j02ID Then
                        'není vlastníkem záznamu
                        If .ReadItems Then
                            Server.Transfer("o23_record_readonly.aspx?pid=" & Master.DataPID.ToString, True)
                        Else
                            Master.StopPage("Nemáte oprávnění číst tento dokument v rámci typu [{0}].", True)

                        End If
                    Else
                        'je vlastníkem
                        Dim bolTested As Boolean = False
                        If cRec.b02ID <> 0 Then
                            Dim cB02 As BO.b02WorkflowStatus = Master.Factory.b02WorkflowStatusBL.Load(cRec.b02ID)
                            If cB02.b02IsRecordReadOnly4Owner Then  'aktuální workflow status má nastaveno, že vlastník záznam má pouze readonly přístup
                                Server.Transfer("o23_record_readonly.aspx?pid=" & Master.DataPID.ToString, True)
                            End If
                        End If
                    End If
                End If
            End If


        End With
    End Sub

    Private Sub RefreshRecord()
        If Me.CurrentX18ID = 0 And Master.DataPID = 0 Then
            Server.Transfer("select_doctype.aspx")
            Return
        End If
        Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(Me.CurrentX18ID)
        lisX20X18 = lisX20X18.Where(Function(p) p.x20IsClosed = False And p.x20EntryModeFlag = BO.x20EntryModeENUM.InsertUpdateWithoutCombo).OrderBy(Function(p) p.x20IsMultiSelect).ThenBy(Function(p) p.x29ID)   'omezit pouze na otevřené vazby + vazby vyplňované přes záznam položky kategorie
        With opgX20ID
            .DataSource = lisX20X18
            .DataBind()
            Dim intDefListIndex As Integer = 0
            If Master.DataPID = 0 Then
                Dim defUsedVals As New List(Of String)
                If Request.Item("masterprefix") <> "" And Request.Item("masterpid") <> "" Then 'předvyplnit nový záznam odkazem na entitu
                    Dim intX29ID As Integer = CInt(BO.BAS.GetX29FromPrefix(Request.Item("masterprefix")))
                    If lisX20X18.Where(Function(p) p.x29ID = intX29ID).Count > 0 Then
                        .SelectedValue = lisX20X18.Where(Function(p) p.x29ID = intX29ID)(0).x20ID.ToString
                        Handle_Changex20ID()
                        Handle_CreateTempX19Reocrd(BO.BAS.IsNullInt(Request.Item("masterpid")))
                        defUsedVals.Add(.SelectedValue)
                    End If
                End If
                If lisX20X18.Where(Function(p) p.x29ID = 102).Count > 0 And Request.Item("masterprefix") <> "j02" Then   'předvyplnit nový záznam vazbou na přihlášeného usera
                    'předvyplnit vazbu osoby na přihlášeného uživatele
                    .SelectedValue = lisX20X18.Where(Function(p) p.x29ID = 102)(0).x20ID.ToString
                    Handle_Changex20ID()
                    Handle_CreateTempX19Reocrd(Master.Factory.SysUser.j02ID)
                    defUsedVals.Add(.SelectedValue)
                End If
                If defUsedVals.Count > 0 Then
                    For i As Integer = 0 To .Items.Count - 1
                        Dim ii As Integer = i
                        If defUsedVals.Where(Function(p) p = .Items(ii).Value).Count = 0 Then   'najít první neobsazený druh vazby, kterou přednastavit k vyplnění
                            intDefListIndex = i : Exit For
                        End If
                    Next
                End If
            End If



            If .Items.Count > 0 Then
                .SelectedIndex = intDefListIndex

                Handle_Changex20ID()
            Else
                panX20.Visible = False
            End If
        End With

        If Master.DataPID = 0 Then
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
            _curRec = New BO.o23Doc
            RefreshUserFields()

            If Request.Item("t1") <> "" And Request.Item("t2") <> "" Then
                Dim dt1 As New BO.DateTimeByQuerystring(Request.Item("t1")), dt2 As New BO.DateTimeByQuerystring(Request.Item("t2"))
                If hidx18CalendarFieldStart.Value <> "" Then
                    For Each ri As RepeaterItem In rpX16.Items
                        Dim strF As String = CType(ri.FindControl("x16Field"), HiddenField).Value
                        If strF = hidx18CalendarFieldStart.Value Then
                            CType(ri.FindControl("txtFF_Date"), Telerik.Web.UI.RadDateTimePicker).SelectedDate = dt1.DateOnly
                        End If
                        If strF = hidx18CalendarFieldEnd.Value Then
                            CType(ri.FindControl("txtFF_Date"), Telerik.Web.UI.RadDateTimePicker).SelectedDate = dt2.DateOnly
                        End If
                    Next
                End If

            End If

            Return      'konec pro režim nového záznamu
        End If

        _curRec = Master.Factory.o23DocBL.Load(Master.DataPID)
        If _curRec.o23IsEncrypted Then Handle_EncryptedRecord()


        With _curRec
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.x23ID.SelectedValue = .x23ID.ToString
            Me.o23Name.Text = .o23Name
            Me.o23Ordinary.Value = .o23Ordinary
            Me.o23Code.Text = .o23Code
            Me.o23ArabicCode.Text = .o23ArabicCode
            Master.Timestamp = .Timestamp
            If panColors.Visible Then
                basUI.SetColorToPicker(Me.o23BackColor, .o23BackColor)
                basUI.SetColorToPicker(Me.o23ForeColor, .o23ForeColor)
            End If

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Me.o23IsEncrypted.Checked = .o23IsEncrypted
        End With

        If panUpload.Visible And _curRec.o23LockedFlag <> BO.o23LockedTypeENUM.LockAllFiles Then
            Dim mq As New BO.myQueryO27
            mq.Record_x29ID = BO.x29IdEnum.o23Doc
            mq.Record_PID = _curRec.PID
            Dim lis As IEnumerable(Of BO.o27Attachment) = Master.Factory.o27AttachmentBL.GetList(mq)
            With Me.filesPreview
                If lis.Count > 0 Then
                    .Visible = True
                    .Text = BO.BAS.OM2(.Text, lis.Count.ToString)
                    .NavigateUrl = "javascript:file_preview('o23'," & Master.DataPID.ToString & ")"
                End If
            End With
        End If

        RefreshUserFields()
        If panX20.Visible Then
            Dim lisX19 As IEnumerable(Of BO.x19EntityCategory_Binding) = Master.Factory.x18EntityCategoryBL.GetList_X19(Master.DataPID, lisX20X18.Select(Function(p) p.x20ID).ToList, True)
            Master.Factory.p85TempBoxBL.Truncate(hidGUID_x19.Value)
            For Each c In lisX19
                Dim cTemp As New BO.p85TempBox
                With cTemp
                    .p85GUID = hidGUID_x19.Value
                    .p85Prefix = "x19"
                    .p85DataPID = c.PID
                    .p85OtherKey1 = c.x19RecordPID
                    .p85OtherKey2 = c.x20ID
                    .p85OtherKey3 = c.x29ID
                    .p85FreeBoolean01 = c.x20IsMultiselect
                    If c.x20Name <> "" Then
                        .p85FreeText01 = c.x20Name
                    Else
                        .p85FreeText01 = BO.BAS.GetX29EntityAlias(CType(c.x29ID, BO.x29IdEnum), False)
                    End If

                    .p85FreeText02 = c.RecordAlias


                End With
                Master.Factory.p85TempBoxBL.Save(cTemp)
            Next
            RefreshTempX19()
        End If

        If panHtmlEditor.Visible Then
            With Master.Factory.o23DocBL
                If _curRec.o23IsEncrypted Then
                    Me.o23HtmlContent.Content = .DecryptString(.LoadHtmlContent(Master.DataPID))
                Else
                    Me.o23HtmlContent.Content = .LoadHtmlContent(Master.DataPID)
                End If
            End With
        End If
        tags1.RefreshData(Master.DataPID)

    End Sub

    Private Sub Handle_EncryptedRecord()
        If Not _curRec.o23IsEncrypted Then Return

        Dim cTemp As BO.p85TempBox = Master.Factory.p85TempBoxBL.LoadByGUID(Master.Factory.SysUser.j02ID.ToString & "-" & Master.DataPID.ToString)
        If cTemp Is Nothing Then
            Dim s As String = "<hr><a href='o23_fixwork.aspx?pid=" & _curRec.PID.ToString & "&x18id=" & _curRec.x18ID.ToString & "' target='_top'>Stránka dokumentu s možností zadat heslo k zašifrovanému obsahu</a>"
            Master.StopPage("Nezdařilo se ověření zašifrovaného obsahu." & s)
        Else
            Master.Factory.o23DocBL.DecryptRecord(_curRec)
        End If
    End Sub
    Private Sub RefreshTempX19()
        rpX19.DataSource = Master.Factory.p85TempBoxBL.GetList(hidGUID_x19.Value).OrderBy(Function(p) p.p85FreeBoolean01).ThenBy(Function(p) p.p85OtherKey2)
        rpX19.DataBind()
    End Sub
    Private Sub RefreshUserFields()
        Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Master.Factory.x18EntityCategoryBL.GetList_x16(Me.CurrentX18ID)
        Dim bolHTML As Boolean = False
        If lisX16.Where(Function(p) p.x16Field = "o23HtmlContent").Count > 0 Then
            bolHTML = True
            lisX16 = lisX16.Where(Function(p) p.x16Field <> "o23HtmlContent")
        End If

        If lisX16.Count > 0 Then
            If lisX16.Where(Function(p) LCase(p.x16Field).IndexOf("date") > 0).Count = 0 Then
                Me.SharedCalendar.Visible = False
            End If
            rpX16.DataSource = lisX16
            rpX16.DataBind()

        Else
            panX16.Visible = False
            Me.SharedCalendar.Visible = False
        End If

        panHtmlEditor.Visible = bolHTML

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o23DocBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o23-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Me.o23IsEncrypted.Checked Then
            If Not TestPassword() Then Return
        End If
        With Master.Factory.o23DocBL
            Dim cRec As BO.o23Doc = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o23Doc)
            cRec.o23Name = Me.o23Name.Text
            cRec.o23Ordinary = BO.BAS.IsNullInt(Me.o23Ordinary.Value)
            cRec.x23ID = BO.BAS.IsNullInt(Me.x23ID.SelectedValue)
            cRec.o23Code = Me.o23Code.Text
            cRec.o23ArabicCode = Me.o23ArabicCode.Text
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil
            cRec.j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
            If panColors.Visible Then
                cRec.o23BackColor = basUI.GetColorFromPicker(Me.o23BackColor)
                cRec.o23ForeColor = basUI.GetColorFromPicker(Me.o23ForeColor)
            End If
            If Not InhaleUserFieldValues(cRec) Then
                Return
            End If

            cRec.o23IsEncrypted = Me.o23IsEncrypted.Checked
            If cRec.o23IsEncrypted Then
                .EncryptRecord(cRec)
            End If
            If Me.o23password.Text <> "" Then
                cRec.o23Password = .EncryptString(Me.o23password.Text)
            End If



            Dim lisX19 As List(Of BO.x19EntityCategory_Binding) = Nothing
            If panX20.Visible Then
                lisX19 = New List(Of BO.x19EntityCategory_Binding)
                For Each cTMP In Master.Factory.p85TempBoxBL.GetList(hidGUID_x19.Value)
                    Dim c As New BO.x19EntityCategory_Binding
                    With cTMP
                        c.x19RecordPID = .p85OtherKey1
                        c.x20ID = .p85OtherKey2
                        c.o23ID = Master.DataPID
                    End With
                    lisX19.Add(c)
                Next
            End If

            If .Save(cRec, Me.CurrentX18ID, Nothing, lisX19, GetX20IDs(), "") Then
                Master.DataPID = .LastSavedPID
                If panHtmlEditor.Visible Then
                    If cRec.o23IsEncrypted Then
                        .SaveHtmlContent(Master.DataPID, .EncryptString(Me.o23HtmlContent.Content))
                    Else
                        .SaveHtmlContent(Master.DataPID, Me.o23HtmlContent.Content)
                    End If

                End If
                Master.Factory.o51TagBL.SaveBinding("o23", Master.DataPID, tags1.Geto51IDs())
                If panUpload.Visible Then
                    Dim lisTempUpload As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(upload1.GUID, False)
                    If uploadlist1.ItemsCount > 0 Then
                        Dim cB07 As New BO.b07Comment
                        cB07.x29ID = BO.x29IdEnum.o23Doc
                        cB07.b07RecordPID = Master.DataPID
                        Master.Factory.b07CommentBL.Save(cB07, upload1.GUID, Nothing)
                    End If
                End If
                
                Master.CloseAndRefreshParent("o23-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Private Function GetX20IDs() As List(Of Integer)
        Dim lis As New List(Of Integer)
        For Each li As ListItem In opgX20ID.Items
            lis.Add(CInt(li.Value))
        Next
        Return lis
    End Function

    Private Sub o23_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Master.DataPID <> 0 Then
            Me.x23ID.Enabled = False
            If Me.o23Code.Visible Then
                cmdChangeCode.Visible = Not Me.o23Code.Enabled
            End If
        Else
            Me.x23ID.Enabled = True
            cmdChangeCode.Visible = False
        End If
        basUIMT.RenderQueryCombo(Me.cbxType)
        Me.panPassword.Visible = Me.o23IsEncrypted.Checked
        If Page.IsPostBack And panUpload.Visible Then
            upload1.TryUploadhWaitingFilesOnClientSide()
        End If
        If hidJavascriptFile.Value <> "" Then
            Dim strFile As String = BO.ASS.GetApplicationRootFolder & "\Plugins\" & hidJavascriptFile.Value
            Dim cF As New BO.clsFile
            place_x18JavascriptFile.Controls.Add(New LiteralControl(cF.GetFileContents(strFile, , False)))
        End If
      
    End Sub

    Private Function InhaleUserFieldValues(ByRef cRec As BO.o23Doc) As Boolean
        Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Master.Factory.x18EntityCategoryBL.GetList_x16(Me.CurrentX18ID)
        Dim errs As New List(Of String)
        For Each ri As RepeaterItem In rpX16.Items
            Dim intX16ID As Integer = CInt(CType(ri.FindControl("x16ID"), HiddenField).Value)
            Dim c As BO.x16EntityCategory_FieldSetting = lisX16.First(Function(p) p.x16ID = intX16ID)
            Dim val As Object = Nothing
            Select Case c.FieldType
                Case BO.x24IdENUM.tString
                    If ri.FindControl("cbxFF").Visible Then
                        val = CType(ri.FindControl("cbxFF"), RadComboBox).Text
                    Else
                        val = CType(ri.FindControl("txtFF_Text"), TextBox).Text
                    End If
                    If val = "" Then val = Nothing
                Case BO.x24IdENUM.tDecimal
                    val = CType(ri.FindControl("txtFF_Number"), RadNumericTextBox).DbValue
                Case BO.x24IdENUM.tBoolean
                    val = CType(ri.FindControl("chkFF"), CheckBox).Checked
                Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime
                    With CType(ri.FindControl("txtFF_Date"), RadDateTimePicker)
                        If .IsEmpty Then
                            val = Nothing
                        Else
                            val = .DbSelectedDate
                        End If
                    End With
            End Select
            BO.BAS.SetPropertyValue(cRec, c.x16Field, val)
            If c.x16IsEntryRequired And val Is Nothing Then
                If c.FieldType <> BO.x24IdENUM.tBoolean Then
                    errs.Add(String.Format("Pole [{0}] je povinné k vyplnění.", c.x16Name))
                End If
            End If
        Next
        If errs.Count = 0 Then
            Return True
        Else
            Master.Notify(String.Join("<hr>", errs), NotifyLevel.ErrorMessage)
            Return False
        End If
    End Function

    Private Sub rpX16_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rpX16.ItemCreated
        With CType(e.Item.FindControl("txtFF_Date"), RadDateTimePicker)
            .SharedCalendar = Me.SharedCalendar
        End With

    End Sub

    Private Sub rpX16_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX16.ItemDataBound
        Dim cRec As BO.x16EntityCategory_FieldSetting = CType(e.Item.DataItem, BO.x16EntityCategory_FieldSetting)

        e.Item.FindControl("txtFF_Number").Visible = False
        e.Item.FindControl("txtFF_Text").Visible = False
        e.Item.FindControl("chkFF").Visible = False
        e.Item.FindControl("txtFF_Date").Visible = False
        e.Item.FindControl("cbxFF").Visible = False

        CType(e.Item.FindControl("x16IsEntryRequired"), HiddenField).Value = BO.BAS.GB(cRec.x16IsEntryRequired)


        With CType(e.Item.FindControl("x16Name"), Label)
            Select Case cRec.FieldType
                Case BO.x24IdENUM.tDecimal : .Text = "<img src='Images/type_number.png'/> "
                Case BO.x24IdENUM.tBoolean : .Text = "<img src='Images/type_checkbox.png'/> "
                Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime : .Text = "<img src='Images/type_date.png'/> "
                Case Else : .Text = "<img src='Images/type_text.png'/> "
            End Select
            .Text += cRec.x16Name & ":"
            If cRec.x16IsEntryRequired Then
                .ForeColor = Drawing.Color.Red
                .Text = .Text & "*"
            End If
        End With

        CType(e.Item.FindControl("x16Field"), HiddenField).Value = cRec.x16Field

        CType(e.Item.FindControl("x16ID"), HiddenField).Value = cRec.x16ID.ToString


        Dim curValue As Object = BO.BAS.GetPropertyValue(_curRec, cRec.x16Field)
        If curValue Is System.DBNull.Value Then curValue = Nothing

        Select Case cRec.FieldType
            Case BO.x24IdENUM.tString
                CType(e.Item.FindControl("hidType"), HiddenField).Value = "string"
                If cRec.x16DataSource <> "" Then
                    e.Item.FindControl("cbxFF").Visible = True
                    If cRec.x16DataSource.IndexOf("|") > 0 And cRec.x16DataSource.IndexOf(vbCrLf) > 0 Then
                        'checkbox-list
                        With CType(e.Item.FindControl("cbxFF"), RadComboBox)
                            .Enabled = False
                            .Text = curValue
                            If .Text <> "" Then .ToolTip = .Text
                        End With

                        With CType(e.Item.FindControl("cmdChklist"), HtmlButton)
                            .Visible = True
                            .Attributes.Item("onclick") = "chklist(" & cRec.x16ID.ToString & ",'" & e.Item.FindControl("cbxFF").ClientID & "')"
                        End With
                    Else
                        Dim a() As String = Split(cRec.x16DataSource, ";")
                        With CType(e.Item.FindControl("cbxFF"), RadComboBox)
                            .AllowCustomText = Not cRec.x16IsFixedDataSource
                            If .AllowCustomText Then .Items.Add(New RadComboBoxItem(" "))
                            For i As Integer = 0 To UBound(a)
                                .Items.Add(New RadComboBoxItem(a(i)))
                            Next
                            If Not curValue Is Nothing Then
                                If .AllowCustomText Then
                                    .Text = curValue
                                Else
                                    Try
                                        .Items.FindItemByText(curValue).Selected = True
                                    Catch ex As Exception
                                    End Try
                                End If
                            End If
                            If cRec.x16TextboxWidth > 0 Then
                                .Style.Item("width") = cRec.x16TextboxWidth.ToString & "px"
                            End If
                        End With
                    End If
                    
                Else
                    With CType(e.Item.FindControl("txtFF_Text"), TextBox)
                        .Visible = True
                        If cRec.x16TextboxWidth > 40 Then
                            .Style.Item("width") = cRec.x16TextboxWidth.ToString & "px"
                        End If
                        If cRec.x16TextboxHeight > 20 Then
                            .Style.Item("height") = cRec.x16TextboxHeight.ToString & "px"
                            .TextMode = TextBoxMode.MultiLine
                        End If

                        If Not curValue Is Nothing Then
                            .Text = curValue
                        End If
                    End With
                End If

            Case BO.x24IdENUM.tDecimal
                CType(e.Item.FindControl("hidType"), HiddenField).Value = "decimal"
                With CType(e.Item.FindControl("txtFF_Number"), RadNumericTextBox)
                    .Visible = True
                    .NumberFormat.DecimalDigits = 2
                    If Not curValue Is Nothing Then .Value = CDbl(curValue)
                End With

            Case BO.x24IdENUM.tBoolean
                CType(e.Item.FindControl("hidType"), HiddenField).Value = "boolean"
                e.Item.FindControl("x16Name").Visible = False
                With CType(e.Item.FindControl("chkFF"), CheckBox)
                    .Visible = True
                    .Text = cRec.x16Name
                    If Not curValue Is Nothing Then
                        .Checked = curValue
                    End If
                End With
            Case BO.x24IdENUM.tDateTime
                CType(e.Item.FindControl("hidType"), HiddenField).Value = "datetime"

                With CType(e.Item.FindControl("txtFF_Date"), RadDateTimePicker)
                    ''.SharedCalendar = Me.SharedCalendar
                    .Visible = True
                    .MinDate = DateSerial(1900, 1, 1)
                    .MaxDate = DateSerial(2100, 1, 1)
                    If cRec.x16Format = "" Then cRec.x16Format = "dd.MM.yyyy"
                    .DateInput.DisplayDateFormat = cRec.x16Format
                    .DateInput.DateFormat = cRec.x16Format
                    If LCase(cRec.x16Format).IndexOf("hh") > 0 Then                        
                        .TimePopupButton.Visible = True
                        ''.TimeView.Visible = True
                        .Width = Unit.Parse("170px")
                    Else
                        .TimePopupButton.Visible = False
                        ''.TimeView.Visible = False
                        .Width = Unit.Parse("130px")
                    End If

                    'Select Case cRec.TypeName
                    '    Case "datetime"
                    '        .DateFormat = "dd.MM.yyyy HH:mm"
                    '    Case "time"
                    '        .DateFormat = "HH:mm"
                    '    Case Else
                    '        .DateFormat = "dd.MM.yyyy"
                    'End Select

                    If Not curValue Is Nothing Then
                        If Year(curValue) > 1900 Then .SelectedDate = curValue
                    End If
                End With
        End Select
    End Sub
    Private Function LoadX20Record() As BO.x20EntiyToCategory
        Dim lis As IEnumerable(Of BO.x20EntiyToCategory) = Master.Factory.x18EntityCategoryBL.GetList_x20(Me.CurrentX18ID).Where(Function(p) p.x20ID = Me.CurrentX20ID)
        If lis.Count = 0 Then
            Return Nothing
        Else
            Return lis(0)
        End If
    End Function

    Private Sub Handle_Changex20ID()
        If Me.opgX20ID.SelectedItem Is Nothing Then Return
        
        Dim cX20 As BO.x20EntiyToCategory = LoadX20Record(), sT As String = "--Všechny typy--"
        If cX20 Is Nothing Then Return
        hidX29ID.Value = cX20.x29ID.ToString
        Dim lisPars As New List(Of String)

        cbx1.Visible = True
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                cbxType.DataTextField = "p42Name" : sT = "--Všechny typy projektů--"
                cbxType.DataSource = Master.Factory.p42ProjectTypeBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/project_service.asmx" : cbx1.Text = "Hledat projekt..."
            Case BO.x29IdEnum.p28Contact
                cbxType.DataTextField = "p29Name" : sT = "--Všechny typy klientů--"
                cbxType.DataSource = Master.Factory.p29ContactTypeBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/contact_service.asmx" : cbx1.Text = "Hledat klienta..."
            Case BO.x29IdEnum.p91Invoice
                cbxType.DataTextField = "p92Name" : sT = "--Všechny typy faktur--"
                cbxType.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/invoice_service.asmx" : cbx1.Text = "Hledat fakturu..."
            Case BO.x29IdEnum.p56Task
                cbxType.DataTextField = "p57Name" : sT = "--Všechny typy úkolů--"
                cbxType.DataSource = Master.Factory.p57TaskTypeBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/task_service.asmx" : cbx1.Text = "Hledat úkol..."
            Case BO.x29IdEnum.j02Person
                cbxType.DataTextField = "j07Name" : sT = "--Všichni vč.kontaktních osob--"
                cbxType.DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/person_service.asmx" : cbx1.Text = "Hledat osobu..."
            Case BO.x29IdEnum.o23Doc
                cbxType.DataTextField = "x18Name" : sT = "--Všechny typy dokumentů--"
                cbxType.DataSource = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery)
                cbx1.WebServiceSettings.Path = "~/Services/doc_service.asmx" : cbx1.Text = "Hledat dokument..."
            Case Else
                Me.cbx1.Visible = False
                Master.Notify("Pro tuto entitu nelze použít hledací combo!", NotifyLevel.ErrorMessage)
        End Select
        If cX20.x20Name <> "" Then
            cbx1.Text = String.Format("Hledat [{0}]...", cX20.x20Name)
        End If
        If cbxType.DataTextField <> "" Then
            Dim strKey As String = Me.hidX18ID.Value & "-cbxType-" & Me.cbxType.DataTextField
            lisPars.Add(strKey)
            Master.Factory.j03UserBL.InhaleUserParams(lisPars)

            With cbxType
                Try
                    .DataBind()
                Catch ex As Exception

                End Try

                .Visible = True

                .Items.Insert(0, New ListItem(sT, ""))
            End With

            If Me.CurrentX29ID = BO.x29IdEnum.j02Person Then
                cbxType.Items.Insert(0, New ListItem("--Pouze interní osoby--", "-1"))
            End If
            basUI.SelectDropdownlistValue(Me.cbxType, Master.Factory.j03UserBL.GetUserParam(strKey))
            
        End If
    End Sub

    Private Sub cbx1_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles cbx1.SelectedIndexChanged
        Dim intRecordPID As Integer = BO.BAS.IsNullInt(e.Value)
        If intRecordPID = 0 Then
            Master.Notify("Musíte vybrat záznam!", NotifyLevel.WarningMessage)
            Return
        End If
        Handle_CreateTempX19Reocrd(intRecordPID)


        cbx1.SelectedValue = ""
        cbx1.Text = ""

    End Sub

    Private Sub Handle_CreateTempX19Reocrd(intRecordPID As Integer)
        If intRecordPID = 0 Then Return
        Dim cX20 As BO.x20EntiyToCategory = LoadX20Record()
        Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID_x19.Value)
        If lisTemp.Where(Function(p) p.p85OtherKey1 = intRecordPID And p.p85OtherKey2 = Me.CurrentX20ID).Count > 0 Then
            Master.Notify("Tato vazba již existuje!", NotifyLevel.WarningMessage)
            Return
        End If

        Dim cRec As New BO.p85TempBox()

        If Not cX20.x20IsMultiSelect Then
            If lisTemp.Where(Function(p) p.p85OtherKey2 = Me.CurrentX20ID).Count > 0 Then
                'automaticky nahradit původní vazbu za tuto, protože není MULTI-SELECT
                cRec = lisTemp.Where(Function(p) p.p85OtherKey2 = Me.CurrentX20ID)(0)
            End If
        End If

        With cRec
            .p85GUID = hidGUID_x19.Value
            .p85Prefix = "x19"
            .p85OtherKey1 = intRecordPID
            .p85OtherKey2 = Me.CurrentX20ID
            .p85OtherKey3 = BO.BAS.IsNullInt(hidX29ID.Value)
            .p85FreeBoolean01 = cX20.x20IsMultiSelect
            If cX20.x20Name <> "" Then
                .p85FreeText01 = cX20.x20Name
            Else
                .p85FreeText01 = BO.BAS.GetX29EntityAlias(Me.CurrentX29ID, False)
            End If
            .p85FreeText02 = Master.Factory.GetRecordCaption(Me.CurrentX29ID, intRecordPID)
        End With

        Master.Factory.p85TempBoxBL.Save(cRec)

        RefreshTempX19()
    End Sub

    Private Sub opgX20ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgX20ID.SelectedIndexChanged
        Handle_Changex20ID()
    End Sub

    Private Sub rpX19_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpX19.ItemCommand
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then

            End If
        End If
        RefreshTempX19()
    End Sub

    Private Sub rpX19_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX19.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        With CType(e.Item.FindControl("Entity"), Label)
            If cRec.p85OtherKey2 <> _loadingLastX20ID Then
                .Text = cRec.p85FreeText01 & ":"
            End If
        End With
        With CType(e.Item.FindControl("RecordAlias"), Label)
            .Text = cRec.p85FreeText02
            .ToolTip = cRec.p85FreeText01
        End With
        _loadingLastX20ID = cRec.p85OtherKey2
    End Sub

    Private Sub cmdChangeCode_Click(sender As Object, e As EventArgs) Handles cmdChangeCode.Click
        Me.o23Code.Enabled = True
        Me.o23Code.Focus()
        Master.Notify("Hodnota kódu se uloží až po uložení celého záznamu tlačítkem [Uložit změny].")
        Me.o23ArabicCode.Text = ""
    End Sub

   
    Private Sub cbxType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxType.SelectedIndexChanged
        Dim strKey As String = Me.hidX18ID.Value & "-cbxType-" & Me.cbxType.DataTextField
        Master.Factory.j03UserBL.SetUserParam(strKey, cbxType.SelectedValue)
    End Sub

    Private Function TestPassword() As Boolean
        If Len(o23password.Text) < 4 And Len(o23password.Text) > 0 Then
            Master.Notify("Minimální délka hesla jsou 4 znaky.", 2)
            Return False
        End If
        If o23password.Text <> txtVerify.Text And (txtVerify.Text <> "" Or o23password.Text <> "") Then
            Master.Notify("Heslo nesouhlasí s ověřením.", 2)
            Return False
        End If
       
        If Len(o23password.Text) < 4 Then
            If Master.IsRecordNew Then
                Master.Notify("Zadejte heslo (minimálně 4 znaky).", 2)
                o23password.Focus()
                Return False
            Else
                Dim cRecCurrent As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
                If Not cRecCurrent.o23IsEncrypted Then    'dokument dříve nebyl zašifrovaný a nyní má být a heslo je nedostatečné
                    Master.Notify("Zadejte heslo (minimálně 4 znaky).", 2)
                    o23password.Focus()
                    Return False
                End If
            End If

        End If

        Return True
    End Function

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub
End Class