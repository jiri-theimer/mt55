Public Class x18_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord
    Private _lisX20 As IEnumerable(Of BO.x20EntiyToCategory) = Nothing

    Private Sub x18_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public ReadOnly Property CurrentX23ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.x23ID.SelectedValue)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Me.b01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.o23Doc)
            Me.b01ID.DataBind()

            hidGUID_x16.Value = BO.BAS.GetGUID()
            ''hidGUID_x17.Value = BO.BAS.GetGUID()
            hidGUID_x20.Value = BO.BAS.GetGUID()
            With Master
                .HeaderIcon = "Images/label_32.png"
                .HeaderText = "Typ dokumentu"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Not .Factory.TestPermission(BO.x53PermValEnum.GR_X18_Admin) Then
                    .StopPage("Pro správu dokumentů nemáte oprávnění.")
                    'If .DataPID <> 0 Then
                    '    Server.Transfer("x18_items.aspx?pid=" & .DataPID.ToString, False)
                    'Else

                    'End If
                End If
                .neededPermission = BO.x53PermValEnum.GR_X18_Admin


                Dim lis As IEnumerable(Of BO.x23EntityField_Combo) = .Factory.x23EntityField_ComboBL.GetList(New BO.myQuery)
                Me.x23ID.DataSource = lis
                Me.x23ID.DataBind()

                Me.x31ID_Plugin.DataSource = .Factory.x31ReportBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.o23Doc And p.x31FormatFlag = BO.x31FormatFlagENUM.ASPX)
                Me.x31ID_Plugin.DataBind()
                Me.x31ID_Plugin.Items.Insert(0, "")
            End With

            RefreshRecord()

            Handle_Change_IsManyItems()

            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.x18Name.Text += " KOPIE"
                Me.x23ID.SelectedValue = ""
            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
            Return
        End If
        _lisX20 = Master.Factory.x18EntityCategoryBL.GetList_x20(Master.DataPID)

        Dim cRec As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(Master.DataPID)
        With cRec
            Me.x23ID.SelectedValue = .x23ID.ToString
            Me.x18Name.Text = .x18Name
            Me.x18NameShort.Text = .x18NameShort
            Me.x18Ordinary.Value = .x18Ordinary

            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.b01ID.SelectedValue = .b01ID.ToString

            Me.x18IsColors.Checked = .x18IsColors
            If .x18IsManyItems Then
                Me.x18IsManyItems.SelectedValue = "1"
            Else
                Me.x18IsManyItems.SelectedValue = "0"
            End If
            Me.x18IsClueTip.Checked = .x18IsClueTip
            Me.x18Icon.Text = .x18Icon
            Me.x18Icon32.Text = .x18Icon32
            Me.x18ReportCodes.Text = .x18ReportCodes
            Me.x18IsCalendar.Checked = .x18IsCalendar

            basUI.SelectDropdownlistValue(Me.x18CalendarFieldStart, .x18CalendarFieldStart)
            basUI.SelectDropdownlistValue(Me.x18CalendarFieldEnd, .x18CalendarFieldEnd)
            basUI.SelectDropdownlistValue(Me.x18CalendarFieldSubject, .x18CalendarFieldSubject)
            basUI.SelectDropdownlistValue(Me.x18CalendarResourceField, .x18CalendarResourceField)
            Master.Timestamp = .Timestamp

           
            basUI.SelectDropdownlistValue(Me.x18GridColsFlag, CInt(.x18GridColsFlag).ToString)
            basUI.SelectDropdownlistValue(Me.x18EntryNameFlag, CInt(.x18EntryNameFlag).ToString)
            basUI.SelectDropdownlistValue(Me.x18EntryCodeFlag, CInt(.x18EntryCodeFlag).ToString)
            basUI.SelectDropdownlistValue(Me.x18EntryOrdinaryFlag, CInt(.x18EntryOrdinaryFlag).ToString)
            basUI.SelectDropdownlistValue(Me.x18DashboardFlag, CInt(.x18DashboardFlag).ToString)
            basUI.SelectRadiolistValue(Me.x18UploadFlag, CInt(.x18UploadFlag).ToString)

            basUI.SelectDropdownlistValue(Me.x18MaxOneFileSize, .x18MaxOneFileSize.ToString)
            Me.x18AllowedFileExtensions.Text = .x18AllowedFileExtensions
            Me.x18IsAllowEncryption.Checked = .x18IsAllowEncryption

            basUI.SelectDropdownlistValue(Me.x31ID_Plugin, .x31ID_Plugin.ToString)

            roles1.InhaleInitialData(.PID)
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With



        Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Master.Factory.x18EntityCategoryBL.GetList_x16(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(hidGUID_x16.Value)
        For Each c In lisX16
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = hidGUID_x16.Value
                .p85Prefix = "x16"
                .p85DataPID = c.x16ID
                .p85OtherKey1 = c.x16Ordinary
                .p85FreeText01 = c.x16Field
                .p85FreeText02 = c.x16Name
                .p85FreeText03 = c.x16NameGrid
                .p85Message = c.x16DataSource
                .p85FreeBoolean01 = c.x16IsEntryRequired
                .p85FreeBoolean02 = c.x16IsGridField
                .p85FreeBoolean03 = c.x16IsFixedDataSource
                .p85FreeNumber01 = c.x16TextboxWidth
                .p85FreeNumber02 = c.x16TextboxHeight
                .p85FreeText04 = c.x16Format
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempX16()
        ''Dim lisX17 As IEnumerable(Of BO.x17EntityCategory_Folder) = Master.Factory.x18EntityCategoryBL.GetList_x17(Master.DataPID)
        ''Master.Factory.p85TempBoxBL.Truncate(hidGUID_x17.Value)
        ''For Each c In lisX17
        ''    Dim cTemp As New BO.p85TempBox
        ''    With cTemp
        ''        .p85GUID = hidGUID_x17.Value
        ''        .p85Prefix = "x17"
        ''        .p85DataPID = c.x17ID
        ''        .p85FreeText01 = c.x17Path
        ''        .p85FreeNumber02 = c.x17Ordinary
        ''        .p85OtherKey1 = c.j11ID_Read
        ''        .p85OtherKey2 = c.j11ID_CreateFiles
        ''        .p85OtherKey3 = c.j11ID_FullControl
        ''    End With
        ''    Master.Factory.p85TempBoxBL.Save(cTemp)
        ''Next
        ''RefreshTempX17()
        Dim lisX20 As IEnumerable(Of BO.x20EntiyToCategory) = Master.Factory.x18EntityCategoryBL.GetList_x20(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(hidGUID_x20.Value)
        For Each c In lisX20
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = hidGUID_x20.Value
                .p85Prefix = "x20"
                .p85DataPID = c.x20ID
                .p85OtherKey1 = c.x29ID
                .p85FreeText01 = c.x20Name
                .p85OtherKey2 = CInt(c.x20EntryModeFlag)
                .p85OtherKey3 = CInt(c.x20GridColumnFlag)
                .p85OtherKey6 = CInt(c.x20EntityPageFlag)
                .p85OtherKey7 = c.x20EntityTypePID
                .p85OtherKey8 = c.x29ID_EntityType
                .p85FreeBoolean01 = c.x20IsEntryRequired
                .p85FreeBoolean02 = c.x20IsMultiSelect
                .p85FreeBoolean03 = c.x20IsClosed
                .p85FreeNumber01 = c.x20Ordinary
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempX20()

       
    End Sub
    Private Sub RefreshTempX16()
        rpX16.DataSource = Master.Factory.p85TempBoxBL.GetList(hidGUID_x16.Value)
        rpX16.DataBind()
    End Sub
    ''Private Sub RefreshTempX17()
    ''    rpX17.DataSource = Master.Factory.p85TempBoxBL.GetList(hidGUID_x17.Value)
    ''    rpX17.DataBind()
    ''End Sub
    Private Sub RefreshTempX20()
        rpX20.DataSource = Master.Factory.p85TempBoxBL.GetList(hidGUID_x20.Value)
        rpX20.DataBind()
    End Sub
    
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x18EntityCategoryBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x18-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()
        SaveTempX16()
        ''SaveTempX17()
        SaveTempX20()

        If Master.DataPID = 0 And opg1.SelectedValue = "2" Then
            If Me.x23ID.SelectedValue = "" Then
                Master.Notify("Musíte vybrat datový zdroj dokumentů z nabídky.")
                Return
            End If
        End If
        Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()


        Dim lisX16 As New List(Of BO.x16EntityCategory_FieldSetting)
        For Each cTMP In Master.Factory.p85TempBoxBL.GetList(hidGUID_x16.Value)
            Dim c As New BO.x16EntityCategory_FieldSetting
            With cTMP
                c.x16Field = .p85FreeText01
                c.x16Name = .p85FreeText02
                c.x16NameGrid = .p85FreeText03
                c.x16Ordinary = .p85OtherKey1
                c.x16IsEntryRequired = .p85FreeBoolean01
                c.x16IsGridField = .p85FreeBoolean02
                c.x16DataSource = .p85Message
                c.x16IsFixedDataSource = .p85FreeBoolean03
                c.x16TextboxWidth = .p85FreeNumber01
                c.x16TextboxHeight = .p85FreeNumber02
                c.x16Format = .p85FreeText04
            End With
            lisX16.Add(c)
        Next
        ''Dim lisX17 As New List(Of BO.x17EntityCategory_Folder)
        ''For Each cTMP In Master.Factory.p85TempBoxBL.GetList(hidGUID_x17.Value)
        ''    Dim c As New BO.x17EntityCategory_Folder
        ''    With cTMP

        ''        c.x17Path = .p85FreeText01
        ''        c.x17Ordinary = .p85FreeNumber02
        ''        c.j11ID_Read = .p85OtherKey1
        ''        c.j11ID_CreateFiles = .p85OtherKey2
        ''        c.j11ID_FullControl = .p85OtherKey3
        ''    End With
        ''    lisX17.Add(c)
        ''Next
        Dim lisX20 As New List(Of BO.x20EntiyToCategory)
        For Each cTMP In Master.Factory.p85TempBoxBL.GetList(hidGUID_x20.Value)
            Dim c As New BO.x20EntiyToCategory
            c.x20ID = cTMP.p85DataPID
            c.x18ID = Master.DataPID
            c.x29ID = cTMP.p85OtherKey1
            c.x20Name = cTMP.p85FreeText01
            c.x20IsEntryRequired = cTMP.p85FreeBoolean01
            c.x20IsMultiSelect = cTMP.p85FreeBoolean02
            c.x20EntryModeFlag = cTMP.p85OtherKey2
            c.x20GridColumnFlag = cTMP.p85OtherKey3
            c.x20EntityPageFlag = cTMP.p85OtherKey6
            c.x20IsClosed = cTMP.p85FreeBoolean03
            c.x20Ordinary = cTMP.p85FreeNumber01
            c.x20EntityTypePID = cTMP.p85OtherKey7
            c.x29ID_EntityType = cTMP.p85OtherKey8
            lisX20.Add(c)
        Next


        With Master.Factory.x18EntityCategoryBL
            Dim cRec As BO.x18EntityCategory = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x18EntityCategory)
            cRec.x18Name = Me.x18Name.Text
            cRec.x18NameShort = Me.x18NameShort.Text
            cRec.x18Ordinary = BO.BAS.IsNullInt(Me.x18Ordinary.Value)
            cRec.x23ID = Me.CurrentX23ID
            cRec.x18Icon = Me.x18Icon.Text
            cRec.x18Icon32 = Me.x18Icon32.Text
            cRec.x18IsClueTip = Me.x18IsClueTip.Checked
            cRec.x18ReportCodes = Me.x18ReportCodes.Text
            cRec.b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)

            cRec.x18IsColors = Me.x18IsColors.Checked
            If Me.x18IsManyItems.SelectedValue = "1" Then
                cRec.x18IsManyItems = True
            Else
                cRec.x18IsManyItems = False
            End If

            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil
            cRec.j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
            cRec.x18GridColsFlag = CType(x18GridColsFlag.SelectedValue, BO.x18GridColsENUM)
            cRec.x18EntryNameFlag = CType(x18EntryNameFlag.SelectedValue, BO.x18EntryNameENUM)
            cRec.x18EntryCodeFlag = CType(x18EntryCodeFlag.SelectedValue, BO.x18EntryCodeENUM)
            cRec.x18EntryOrdinaryFlag = CType(x18EntryOrdinaryFlag.SelectedValue, BO.x18EntryOrdinaryENUM)
            cRec.x18IsCalendar = Me.x18IsCalendar.Checked
            cRec.x18CalendarFieldStart = Me.x18CalendarFieldStart.SelectedValue
            cRec.x18CalendarFieldEnd = Me.x18CalendarFieldEnd.SelectedValue
            cRec.x18CalendarFieldSubject = Me.x18CalendarFieldSubject.SelectedValue
            cRec.x18CalendarResourceField = Me.x18CalendarResourceField.SelectedValue
            cRec.x18DashboardFlag = CType(x18DashboardFlag.SelectedValue, BO.x18DashboardENUM)
            cRec.x18UploadFlag = CType(Me.x18UploadFlag.SelectedValue, BO.x18UploadENUM)
            cRec.x18AllowedFileExtensions = Me.x18AllowedFileExtensions.Text
            cRec.x18MaxOneFileSize = BO.BAS.IsNullInt(Me.x18MaxOneFileSize.SelectedValue)
            cRec.x18IsAllowEncryption = Me.x18IsAllowEncryption.Checked
            cRec.x31ID_Plugin = BO.BAS.IsNullInt(Me.x31ID_Plugin.SelectedValue)

            If .Save(cRec, lisX20, lisX69, lisX16) Then
                Master.DataPID = .LastSavedPID

                Master.CloseAndRefreshParent("x18-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub x18_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Master.DataPID = 0 Then
            opg1.Visible = True
            If opg1.SelectedValue = "2" Then
                Me.x23ID.Visible = True
            Else
                Me.x23ID.Visible = False
            End If
            Me.lblX23ID.Visible = Me.x23ID.Visible
        Else
            opg1.Visible = False
            Me.x23ID.Enabled = False
        End If
        If rpX16.Items.Count > 0 Then
            Me.x18GridColsFlag.Visible = True
        Else
            Me.x18GridColsFlag.Visible = False
        End If
        Me.lblx18CalendarFieldEnd.Visible = Me.x18IsCalendar.Checked
        Me.lblx18CalendarFieldStart.Visible = Me.x18IsCalendar.Checked
        Me.x18CalendarFieldStart.Visible = Me.x18IsCalendar.Checked
        Me.x18CalendarFieldEnd.Visible = Me.x18IsCalendar.Checked
        Me.x18CalendarFieldSubject.Visible = Me.x18IsCalendar.Checked
        Me.lblx18CalendarFieldSubject.Visible = Me.x18IsCalendar.Checked
        Me.x18CalendarResourceField.Visible = Me.x18IsCalendar.Checked
        Me.lblx18CalendarResourceField.Visible = Me.x18IsCalendar.Checked
       
        If Me.x18UploadFlag.SelectedValue = "1" Then
            Me.panFileSystem.Visible = True
        Else
            Me.panFileSystem.Visible = False
        End If
    End Sub

    'Private Sub Handle_ChangeX29ID()
    '    Dim mq As New BO.myQuery, lis As New List(Of BO.x22EntiyCategory_Binding)
    '    mq.Closed = BO.BooleanQueryMode.NoQuery
    '    For Each intX29ID As Integer In basUI.GetCheckedItems(Me.x29IDs)
    '        Select Case CType(intX29ID, BO.x29IdEnum)
    '            Case BO.x29IdEnum.p28Contact
    '                For Each c In Master.Factory.p29ContactTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 329, c.p29Name & " (Klient)"))
    '                Next
    '            Case BO.x29IdEnum.p41Project
    '                For Each c In Master.Factory.p42ProjectTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 342, c.p42Name & " (Projekt)"))
    '                Next
    '            Case BO.x29IdEnum.j02Person
    '                For Each c In Master.Factory.j07PersonPositionBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 107, c.j07Name & " (Osoba)"))
    '                Next
    '            Case BO.x29IdEnum.o23Notepad
    '                For Each c In Master.Factory.o24NotepadTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 224, c.o24Name & "(Dokument)"))
    '                Next
    '            Case BO.x29IdEnum.p91Invoice
    '                For Each c In Master.Factory.p92InvoiceTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 392, c.p92Name & " (Faktura)"))
    '                Next
    '            Case BO.x29IdEnum.p90Proforma
    '                For Each c In Master.Factory.p89ProformaTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 389, c.p89Name & " (Zálohová faktura)"))
    '                Next
    '            Case BO.x29IdEnum.p31Worksheet
    '                For Each c In Master.Factory.p34ActivityGroupBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 334, c.p34Name & " (Worksheet)"))
    '                Next                    
    '            Case BO.x29IdEnum.p56Task
    '                For Each c In Master.Factory.p57TaskTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 357, c.p57Name & " (Úkol)"))
    '                Next
    '            Case BO.x29IdEnum.o22Milestone
    '                For Each c In Master.Factory.o21MilestoneTypeBL.GetList(mq)
    '                    lis.Add(x22rec(c.PID, 221, c.o21Name & " (Událost v kalendáři)"))
    '                Next
    '            Case Else
    '        End Select

    '    Next
    '    rp1.DataSource = lis
    '    rp1.DataBind()
    'End Sub
    'Private Function x22rec(intEntityTypePID As Integer, intX29ID As Integer, strEntityTypeAlias As String) As BO.x22EntiyCategory_Binding
    '    Dim c As New BO.x22EntiyCategory_Binding
    '    c.x18ID = Master.DataPID
    '    c.x22EntityTypePID = intEntityTypePID
    '    c.x29ID_EntityType = intX29ID
    '    c.EntityTypeAlias = strEntityTypeAlias
    '    Return c
    'End Function

    
    

    
    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click

    End Sub

    

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub rpX16_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpX16.ItemCommand
        SaveTempX16()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then

            End If
        End If
        RefreshTempX16()
    End Sub

    Private Sub cmdNewX16_Click(sender As Object, e As EventArgs) Handles cmdNewX16.Click
        SaveTempX16()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = hidGUID_x16.Value
        cRec.p85Prefix = "x16"
        cRec.p85FreeBoolean02 = True
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempX16()
    End Sub

    Private Sub rpX16_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX16.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With

        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("x16Field"), DropDownList), .p85FreeText01)
            CType(e.Item.FindControl("x16Name"), TextBox).Text = .p85FreeText02
            CType(e.Item.FindControl("x16NameGrid"), TextBox).Text = .p85FreeText03
            CType(e.Item.FindControl("x16Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value = .p85OtherKey1
            CType(e.Item.FindControl("x16IsEntryRequired"), CheckBox).Checked = .p85FreeBoolean01
            CType(e.Item.FindControl("x16IsGridField"), CheckBox).Checked = .p85FreeBoolean02
            CType(e.Item.FindControl("x16IsFixedDataSource"), CheckBox).Checked = .p85FreeBoolean03
            CType(e.Item.FindControl("x16DataSource"), TextBox).Text = .p85Message
            CType(e.Item.FindControl("x16TextboxWidth"), TextBox).Text = .p85FreeNumber01
            CType(e.Item.FindControl("x16TextboxHeight"), TextBox).Text = .p85FreeNumber02
            CType(e.Item.FindControl("x16Format"), TextBox).Text = .p85FreeText04
        End With
    End Sub
    Private Sub SaveTempX16()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID_x16.Value)
        For Each ri As RepeaterItem In rpX16.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85FreeText01 = CType(ri.FindControl("x16Field"), DropDownList).SelectedValue
                .p85FreeText02 = CType(ri.FindControl("x16Name"), TextBox).Text
                .p85FreeText03 = CType(ri.FindControl("x16NameGrid"), TextBox).Text
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("x16Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value)
                .p85FreeBoolean01 = CType(ri.FindControl("x16IsEntryRequired"), CheckBox).Checked
                .p85FreeBoolean02 = CType(ri.FindControl("x16IsGridField"), CheckBox).Checked
                .p85FreeBoolean03 = CType(ri.FindControl("x16IsFixedDataSource"), CheckBox).Checked
                .p85Message = CType(ri.FindControl("x16DataSource"), TextBox).Text
                .p85FreeNumber01 = BO.BAS.IsNullInt(CType(ri.FindControl("x16TextboxWidth"), TextBox).Text)
                .p85FreeNumber02 = BO.BAS.IsNullInt(CType(ri.FindControl("x16TextboxHeight"), TextBox).Text)
                .p85FreeText04 = CType(ri.FindControl("x16Format"), TextBox).Text
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub
    ''Private Sub SaveTempX17()
    ''    Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID_x17.Value)
    ''    For Each ri As RepeaterItem In rpX17.Items
    ''        Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
    ''        Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
    ''        With cRec
    ''            .p85FreeText01 = CType(ri.FindControl("x17Path"), TextBox).Text
    ''            .p85FreeNumber02 = BO.BAS.IsNullInt(CType(ri.FindControl("x17Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value)
    ''        End With
    ''        Master.Factory.p85TempBoxBL.Save(cRec)
    ''    Next
    ''End Sub
    Private Sub rpX20_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpX20.ItemCommand
        SaveTempX20()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            Dim intX20ID As Integer = cRec.p85DataPID
            If intX20ID <> 0 Then
                If Master.Factory.x18EntityCategoryBL.GetList_X19(BO.BAS.ConvertInt2List(intX20ID), False).Count > 0 Then
                    Master.Notify("Tato vazba  již obsahuje data. Buď daná data nejdřív odstraníte nebo vazbu nastavte jako uzavřenou.", NotifyLevel.WarningMessage)
                    Return
                End If
            End If
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then

            End If
        End If
        RefreshTempX20()
    End Sub

    Private Sub rpX20_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX20.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        Dim cbx0 As DropDownList = CType(e.Item.FindControl("x29ID"), DropDownList)
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            CType(e.Item.FindControl("x20ID"), HiddenField).Value = .p85DataPID.ToString
            basUI.SelectDropdownlistValue(cbx0, .p85OtherKey1.ToString)
            CType(e.Item.FindControl("x20Name"), TextBox).Text = .p85FreeText01
            CType(e.Item.FindControl("x20IsEntryRequired"), CheckBox).Checked = .p85FreeBoolean01
            CType(e.Item.FindControl("x20IsMultiselect"), CheckBox).Checked = .p85FreeBoolean02
            CType(e.Item.FindControl("x20IsClosed"), CheckBox).Checked = .p85FreeBoolean03
            CType(e.Item.FindControl("x20Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value = .p85FreeNumber01


            Dim cbxEntityType As DropDownList = CType(e.Item.FindControl("x20EntityTypePID"), DropDownList)
            If cbxEntityType.Items.Count = 0 Then
                FillComboEntityType(cbxEntityType, .p85OtherKey1, CType(e.Item.FindControl("x29ID_EntityType"), HiddenField))
            End If
            basUI.SelectDropdownlistValue(cbxEntityType, .p85OtherKey7.ToString)


            Dim cbx1 As DropDownList = CType(e.Item.FindControl("x20EntryModeFlag"), DropDownList), cbx2 As DropDownList = CType(e.Item.FindControl("x20GridColumnFlag"), DropDownList)
            Dim cbx3 As DropDownList = CType(e.Item.FindControl("x20EntityPageFlag"), DropDownList)

            basUI.SelectDropdownlistValue(cbx1, .p85OtherKey2.ToString)
            basUI.SelectDropdownlistValue(cbx2, .p85OtherKey3.ToString)
            basUI.SelectDropdownlistValue(cbx3, cRec.p85OtherKey6.ToString)
            Dim strName As String = Me.x18Name.Text
            If Trim(Me.x18NameShort.Text) <> "" Then strName = Me.x18NameShort.Text

            cbx2.Items.FindByValue("2").Text = String.Format("Sloupec [{0}] v přehledu dokumentů", IIf(cRec.p85FreeText01 = "", cbx0.SelectedItem.Text, cRec.p85FreeText01))
            Select Case CType(cRec.p85OtherKey1, BO.x29IdEnum)
                Case BO.x29IdEnum.p28Contact
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě klienta", strName)
                    cbx1.Items.FindByValue("2").Text = String.Format("Vazbu vyplňovat vyhledavačem klienta v záznamu dokumentu [{0}]", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu klientů", strName)

                Case BO.x29IdEnum.p41Project
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě projektu", strName)
                    cbx1.Items.FindByValue("2").Text = String.Format("Vazbu vyplňovat vyhledavačem projektu v záznamu dokumentu [{0}]", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu projektů", strName)
                Case BO.x29IdEnum.j02Person
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě osoby", strName)
                    cbx1.Items.FindByValue("2").Text = String.Format("Vazbu vyplňovat vyhledavačem osoby v záznamu dokumentu [{0}]", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu osob", strName)
                Case BO.x29IdEnum.p56Task
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě úkolu", strName)
                    cbx1.Items.FindByValue("2").Text = String.Format("Vazbu vyplňovat vyhledavačem úkolu v záznamu dokumentu [{0}]", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu úkolů", strName)
                Case BO.x29IdEnum.o23Doc
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě dokumentu", strName)
                    cbx1.Items.FindByValue("2").Text = String.Format("Vazbu vyplňovat vyhledavačem dokumentu v záznamu dokumentu [{0}]", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu dokumentů", strName)
                Case BO.x29IdEnum.p31Worksheet
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] ve formuláři pro zápis worksheet úkonu", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu worksheet úkonů", strName)
                    cbx2.Items.FindByValue("2").Enabled = False
                    cbx2.Items.FindByValue("3").Enabled = False
                   
                Case BO.x29IdEnum.o22Milestone
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě kalendářové události", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Pole [{0}] v kalendáři událostí", strName)
                    cbx2.Items.FindByValue("2").Enabled = False
                    cbx2.Items.FindByValue("3").Enabled = False
                    
                Case BO.x29IdEnum.p91Invoice
                    cbx1.Items.FindByValue("1").Text = String.Format("Vazbu vyplňovat z combo nabídky dokumentů [{0}] v kartě faktury", strName)
                    cbx2.Items.FindByValue("1").Text = String.Format("Sloupec [{0}] v přehledu faktur", strName)
                    cbx2.Items.FindByValue("2").Enabled = False
                    cbx2.Items.FindByValue("3").Enabled = False
            End Select


            'CType(e.Item.FindControl("x29ID_EntityType"), HiddenField).Value = .p85OtherKey5.ToString
        End With
       
        ''With CType(e.Item.FindControl("chkEntityType"), CheckBox)
        ''    .Text = cRec.EntityTypeAlias
        ''    If Not _lisX20 Is Nothing Then
        ''        If _lisX20.Where(Function(p) p.x22EntityTypePID = cRec.x22EntityTypePID And p.x29ID_EntityType = cRec.x29ID_EntityType).Count > 0 Then
        ''            .Checked = True
        ''        End If
        ''        If _lisX22.Where(Function(p) p.x22EntityTypePID = cRec.x22EntityTypePID And p.x29ID_EntityType = cRec.x29ID_EntityType And p.x22IsEntryRequired = True).Count > 0 Then
        ''            CType(e.Item.FindControl("x22IsEntryRequired"), CheckBox).Checked = True
        ''        End If
        ''    End If
        ''End With
    End Sub
    Private Sub SaveTempX20()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID_x20.Value)
        For Each ri As RepeaterItem In rpX20.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = CInt(CType(ri.FindControl("x29ID"), DropDownList).SelectedValue)
                .p85FreeText01 = CType(ri.FindControl("x20Name"), TextBox).Text
                .p85OtherKey2 = CInt(CType(ri.FindControl("x20EntryModeFlag"), DropDownList).SelectedValue)
                .p85OtherKey3 = CInt(CType(ri.FindControl("x20GridColumnFlag"), DropDownList).SelectedValue)
                .p85OtherKey6 = CInt(CType(ri.FindControl("x20EntityPageFlag"), DropDownList).SelectedValue)
                .p85OtherKey7 = BO.BAS.IsNullInt(CType(ri.FindControl("x20EntityTypePID"), DropDownList).SelectedValue)
                If .p85OtherKey7 <> 0 Then
                    .p85OtherKey8 = BO.BAS.IsNullInt(CType(ri.FindControl("x29ID_EntityType"), HiddenField).Value)
                Else
                    .p85OtherKey8 = 0
                End If
                .p85FreeBoolean01 = CType(ri.FindControl("x20IsEntryRequired"), CheckBox).Checked
                .p85FreeBoolean02 = CType(ri.FindControl("x20IsMultiselect"), CheckBox).Checked
                .p85FreeBoolean03 = CType(ri.FindControl("x20IsClosed"), CheckBox).Checked
                .p85FreeNumber01 = BO.BAS.IsNullInt(CType(ri.FindControl("x20Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value)
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub cmdAddX20_Click(sender As Object, e As EventArgs) Handles cmdAddX20.Click
        If Me.x29ID_addX20.SelectedValue = "" Then
            Master.Notify("Musíte vybrat entitu.", NotifyLevel.WarningMessage)
            Return
        End If
        SaveTempX20()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = hidGUID_x20.Value
        cRec.p85Prefix = "x20"
        cRec.p85OtherKey1 = Me.x29ID_addX20.SelectedValue
        ''cRec.p85OtherKey7 = BO.BAS.IsNullInt(Me.x20EntityTypePID_addX20.SelectedValue)

        Master.Factory.p85TempBoxBL.Save(cRec)

        RefreshTempX20()
        x29ID_addX20.SelectedIndex = 0
    End Sub

    Private Sub FillComboEntityType(cbx As DropDownList, intX29ID As Integer, ctlX29 As HiddenField)
        If intX29ID = 0 Then
            cbx.DataSource = Nothing
            cbx.DataBind()
            Return
        End If
        Dim mq As New BO.myQuery, lis As New List(Of ListItem)
        mq.Closed = BO.BooleanQueryMode.NoQuery

        Select Case CType(intX29ID, BO.x29IdEnum)
            Case BO.x29IdEnum.p28Contact
                For Each c In Master.Factory.p29ContactTypeBL.GetList(mq)
                    lis.Add(New ListItem(c.p29Name, c.PID.ToString))
                Next
                ctlX29.Value = "329"
            Case BO.x29IdEnum.p41Project
                For Each c In Master.Factory.p42ProjectTypeBL.GetList(mq)
                    lis.Add(New ListItem(c.p42Name, c.PID.ToString))
                Next
                ctlX29.Value = "342"
            Case BO.x29IdEnum.j02Person
                For Each c In Master.Factory.j07PersonPositionBL.GetList(mq)
                    lis.Add(New ListItem(c.j07Name, c.PID.ToString))
                Next
                ctlX29.Value = "107"
            Case BO.x29IdEnum.o23Doc
                For Each c In Master.Factory.x18EntityCategoryBL.GetList(mq)
                    lis.Add(New ListItem(c.x18Name, c.PID.ToString))
                Next
                ctlX29.Value = "918"
            Case BO.x29IdEnum.p91Invoice
                For Each c In Master.Factory.p92InvoiceTypeBL.GetList(mq)
                    lis.Add(New ListItem(c.p92Name, c.PID.ToString))
                Next
                ctlX29.Value = "392"
            Case BO.x29IdEnum.p31Worksheet
                For Each c In Master.Factory.p34ActivityGroupBL.GetList(mq)
                    lis.Add(New ListItem(c.p34Name, c.PID.ToString))
                Next
                ctlX29.Value = "334"
            Case BO.x29IdEnum.p56Task
                For Each c In Master.Factory.p57TaskTypeBL.GetList(mq)
                    lis.Add(New ListItem(c.p57Name, c.PID.ToString))
                Next
                ctlX29.Value = "357"
            Case BO.x29IdEnum.o22Milestone
                For Each c In Master.Factory.o21MilestoneTypeBL.GetList(mq)
                    lis.Add(New ListItem(c.o21Name, c.PID.ToString))
                Next
                ctlX29.Value = "221"
            
            Case Else
        End Select

        With cbx
            .Items.Clear()
            .Items.Add(New ListItem("--Omezit na typ--", ""))
            For Each c In lis
                .Items.Add(New ListItem(c.Text, c.Value))
            Next
        End With





    End Sub

    Private Sub x18IsManyItems_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x18IsManyItems.SelectedIndexChanged
        Handle_Change_IsManyItems()
    End Sub

    Private Sub Handle_Change_IsManyItems()
        If Master.DataPID = 0 And Me.x18IsManyItems.SelectedValue = "1" Then
            Me.x18UploadFlag.SelectedValue = "1"
        End If
    End Sub

    ''Private Sub cmdAddX17_Click(sender As Object, e As EventArgs) Handles cmdAddX17.Click
    ''    SaveTempX17()
    ''    Dim cRec As New BO.p85TempBox()
    ''    cRec.p85GUID = hidGUID_x17.Value
    ''    cRec.p85Prefix = "x17"
    ''    Master.Factory.p85TempBoxBL.Save(cRec)
    ''    RefreshTempX17()
    ''End Sub

    ''Private Sub rpX17_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpX17.ItemCommand
    ''    SaveTempX17()
    ''    Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
    ''    If e.CommandName = "delete" Then
    ''        If Master.Factory.p85TempBoxBL.Delete(cRec) Then

    ''        End If
    ''    End If
    ''    RefreshTempX17()
    ''End Sub

    ''Private Sub rpX17_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX17.ItemDataBound
    ''    Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

    ''    With CType(e.Item.FindControl("del"), ImageButton)
    ''        .CommandArgument = cRec.PID.ToString
    ''        .CommandName = "delete"
    ''    End With

    ''    With cRec
    ''        CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
    ''        CType(e.Item.FindControl("x17Path"), TextBox).Text = .p85FreeText01
    ''        CType(e.Item.FindControl("x17Ordinary"), Telerik.Web.UI.RadNumericTextBox).Value = .p85FreeNumber02
    ''    End With
    ''End Sub
End Class