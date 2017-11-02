Public Class p28_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p28_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p28_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        roles1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            ViewState("guid_o37") = BO.BAS.GetGUID()
            ViewState("guid_o32") = BO.BAS.GetGUID()
            ViewState("guid_p30") = BO.BAS.GetGUID()

            With Master
                .HeaderIcon = "Images/contact_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Or .IsRecordClone Then
                    'oprávnění pro zakládání nových kontaktů
                    .neededPermission = BO.x53PermValEnum.GR_P28_Creator
                    .neededPermissionIfSecond = BO.x53PermValEnum.GR_P28_Draft_Creator
                End If
                With .Factory.j03UserBL
                    .InhaleUserParams("p28_record-chkWhisper")
                    Me.chkWhisper.Checked = BO.BAS.BG(.GetUserParam("p28_record-chkWhisper", "1"))
                End With

            End With
            Me.p29ID.DataSource = Master.Factory.p29ContactTypeBL.GetList(New BO.myQuery)
            Me.p29ID.DataBind()

            basUI.SetupP87Combo(Master.Factory, Me.p87ID)

            SetupPriceList()
            
            Me.p63ID.DataSource = Master.Factory.p63OverheadBL.GetList(New BO.myQuery)
            Me.p63ID.DataBind()
            Me.j61ID_Invoice.DataSource = Master.Factory.j61TextTemplateBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p91Invoice)
            Me.j61ID_Invoice.DataBind()
            Me.j61ID_Invoice.Items.Insert(0, "")
            
            If Me.p92id.Visible Then
                Me.p92id.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice)
                Me.p92id.DataBind()
            End If
           
            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If

            If Master.DataPID = 0 Then
                Master.HeaderText = "Založit klienta"

            End If

        End If



    End Sub
    Private Sub SetupPriceList()
        Dim lis As IEnumerable(Of BO.p51PriceList) = Master.Factory.p51PriceListBL.GetList(New BO.myQuery)
        Me.p51ID_Billing.DataSource = lis.Where(Function(p) p.p51IsInternalPriceList = False And p.p51IsMasterPriceList = False And p.p51IsCustomTailor = False)
        Me.p51ID_Billing.DataBind()
        Me.p51ID_Internal.DataSource = lis.Where(Function(p) p.p51IsInternalPriceList = True And p.p51IsMasterPriceList = False And p.p51IsCustomTailor = False)
        Me.p51ID_Internal.DataBind()
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            TryInhaleInitialData()
            Handle_FF()
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
            Me.p28IsDraft.Visible = True
            Return
        Else
            Me.p28IsDraft.Visible = False
        End If

        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("Záznam klienta nelze načíst.")
        Dim cDisp As BO.p28RecordDisposition = Master.Factory.p28ContactBL.InhaleRecordDisposition(cRec)
        If Not cDisp.OwnerAccess Then
            'oprávnění pro editaci klienta
            Master.StopPage("Pro klienta nedisponujete vlastnickým (editačním) oprávněním.")
        End If
        With cRec
            Master.HeaderText = "Klient | " & .p28Name
            Me.p29ID.SelectedValue = .p29ID.ToString
            Handle_FF()
            Me.p28IsCompany.SelectedValue = BO.BAS.GB(.p28IsCompany)
            Me.p28Code.Text = .p28Code
            Me.p28Code.NavigateUrl = "javascript:recordcode()"
            If .p28IsCompany Then
                Me.p28CompanyName.Text = .p28CompanyName
                Me.p28CompanyShortName.Text = .p28CompanyShortName
            Else
                Me.p28TitleAfterName.SetText(.p28TitleAfterName)
                Me.p28TitleBeforeName.SetText(.p28TitleBeforeName)
                Me.p28FirstName.Text = .p28FirstName
                Me.p28LastName.Text = .p28LastName
            End If
            Me.p28RegID.Text = .p28RegID
            Me.p28VatID.Text = .p28VatID

            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.p51ID_Billing.SelectedValue = .p51ID_Billing.ToString
            If .p51ID_Billing > 0 Then
                Dim cP51 As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(.p51ID_Billing)
                If cP51.p51IsCustomTailor Then
                    Me.hidP51ID_Tailor.Value = cP51.PID.ToString
                    Me.opgPriceList.SelectedValue = "3"
                    If Not Page.IsPostBack Then
                        If Master.IsRecordClone Then
                            Me.p51ID_Billing.SelectedValue = ""
                            opgPriceList.SelectedIndex = 0
                        End If
                    End If
                    
                Else
                    Me.opgPriceList.SelectedValue = "2"
                End If
            Else
                Me.opgPriceList.SelectedValue = "1"
            End If
            Me.p51ID_Internal.SelectedValue = .p51ID_Internal.ToString
            Me.p63ID.SelectedValue = .p63ID.ToString
            basUI.SelectDropdownlistValue(Me.j61ID_Invoice, .j61ID_Invoice.ToString)

            Me.p87ID.SelectedValue = .p87ID.ToString
            Me.p92id.SelectedValue = .p92ID.ToString
            Me.p28InvoiceDefaultText1.Text = .p28InvoiceDefaultText1
            Me.p28InvoiceDefaultText2.Text = .p28InvoiceDefaultText2
            Me.p28InvoiceMaturityDays.Value = .p28InvoiceMaturityDays

            Me.p28LimitHours_Notification.Value = .p28LimitHours_Notification
            Me.p28LimitFee_Notification.Value = .p28LimitFee_Notification
            If .p28LimitHours_Notification > 0 Or .p28LimitFee_Notification > 0 Then Me.chkDefineLimits.Checked = True Else Me.chkDefineLimits.Checked = False


            Me.p28CompanyShortName.Text = .p28CompanyShortName
            Me.p28RobotAddress.Text = .p28RobotAddress
            basUI.SelectRadiolistValue(Me.p28SupplierFlag, .p28SupplierFlag)
            Me.p28SupplierID.Text = .p28SupplierID
            Me.p28ExternalPID.Text = .p28ExternalPID
            Master.Timestamp = .Timestamp & " <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"

            If .p28ParentID <> 0 Then
                Me.p28ParentID.Value = .p28ParentID
                Me.p28ParentID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .p28ParentID, True)
            End If
            Me.p28BillingMemo.Text = .p28BillingMemo
            Me.p28Pohoda_VatCode.Text = .p28Pohoda_VatCode
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid_o32"))
        For Each c In lisO32
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid_o32")
                .p85DataPID = c.PID
                If Not Page.IsPostBack Then
                    If Master.IsRecordClone Then
                        .p85DataPID = 0
                    End If
                End If
                
                .p85OtherKey1 = c.o33ID
                .p85FreeText01 = c.o32Value
                .p85FreeText02 = c.o32Description
                .p85FreeBoolean01 = c.o32IsDefaultInInvoice
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempO32()

        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(Master.DataPID, 0, False)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid_p30"))
        For Each c In lisJ02
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid_p30")
                .p85DataPID = c.PID
                .p85OtherKey1 = c.PID
                .p85FreeText01 = c.FullNameDescWithJobTitle
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempP30()
        basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInWorksheet, cRec.j02ID_ContactPerson_DefaultInWorksheet.ToString)
        basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInInvoice, cRec.j02ID_ContactPerson_DefaultInInvoice.ToString)

        Dim lisO37 As IEnumerable(Of BO.o37Contact_Address) = Master.Factory.p28ContactBL.GetList_o37(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid_o37"))
        For Each c In lisO37
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid_o37")
                .p85DataPID = c.PID
                .p85OtherKey1 = c.o36ID
                .p85OtherKey2 = c.o38ID
                .p85FreeText01 = c.o38City
                .p85FreeText02 = c.o38Street
                .p85FreeText03 = c.o38ZIP
                .p85FreeText04 = c.o38Country
                .p85FreeText05 = c.o38Name
                .p85FreeText09 = c.o38AresID
                If Not Page.IsPostBack Then
                    If Master.IsRecordClone Then
                        .p85OtherKey2 = 0
                        .p85DataPID = 0
                        .p85FreeText09 = ""
                    End If
                End If
                

            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next

        roles1.InhaleInitialData(cRec.PID)
        tags1.RefreshData(cRec.PID)
        
        RefreshTempO37()

        
    End Sub
    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p28Contact, Master.DataPID, BO.BAS.IsNullInt(Me.p29ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p28Contact, BO.BAS.IsNullInt(Me.p29ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "p28Contact_FreeField", Master.DataPID)
                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)

            End If
        End With
    End Sub
    Private Sub rpO37_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO37.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        If Me.hidDistinctCountries.Value = "" Then
            Me.hidDistinctCountries.Value = ";" & String.Join(";", Master.Factory.o38AddressBL.GetList_DistinctCountries())
        End If
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("o36id"), DropDownList), .p85OtherKey1.ToString)
            CType(e.Item.FindControl("o38City"), TextBox).Text = .p85FreeText01
            CType(e.Item.FindControl("o38Street"), TextBox).Text = .p85FreeText02
            CType(e.Item.FindControl("o38ZIP"), TextBox).Text = .p85FreeText03
            With CType(e.Item.FindControl("o38Country"), UI.datacombo)
                .SetText(cRec.p85FreeText04())
                .DefaultValues = Me.hidDistinctCountries.Value
            End With

            CType(e.Item.FindControl("o38Name"), TextBox).Text = .p85FreeText05
        End With
    End Sub

    Private Sub RefreshTempO37()
        rpO37.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o37"))
        rpO37.DataBind()
        
    End Sub
    Private Sub RefreshTempO32()
        rpO32.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o32"))
        rpO32.DataBind()
    End Sub
    Private Sub RefreshTempP30()
        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30"))
        rpP30.DataSource = lis
        rpP30.DataBind()
        Dim s As String = ""
        With Me.j02ID_ContactPerson_DefaultInInvoice
            If .Items.Count = 0 Then s = "" Else s = .SelectedValue
            .DataSource = lis
            .DataBind()
            .Items.Insert(0, "")
        End With
        basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInInvoice, s)
        With Me.j02ID_ContactPerson_DefaultInWorksheet
            If .Items.Count = 0 Then s = "" Else s = .SelectedValue
            .DataSource = lis
            .DataBind()
            .Items.Insert(0, "")
        End With
        basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInWorksheet, s)

        
    End Sub
   

    Private Sub p28_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        panCompany.Visible = False : panPerson.Visible = False
        If Me.p28IsCompany.SelectedValue = "1" Then
            panCompany.Visible = True
        Else
            panPerson.Visible = True
        End If
        p28CompanyShortName.Visible = Not panPerson.Visible
        lblp28CompanyShortName.Visible = Not panPerson.Visible
        If rpO37.Items.Count > 0 Then panO37.Visible = True Else panO37.Visible = False
        If rpO32.Items.Count > 0 Then panO32.Visible = True Else panO32.Visible = False
        Me.panLimits.Visible = Me.chkDefineLimits.Checked
        RefreshState_Pricelist()

        Select Case Me.p28SupplierFlag.SelectedValue
            Case "2", "3"
                lblSupplierID.Visible = True : p28SupplierID.Visible = True
            Case Else
                lblSupplierID.Visible = False : p28SupplierID.Visible = False
        End Select
        If rpP30.Items.Count > 0 Then
            tabDefaultPerson.Visible = True
        Else
            tabDefaultPerson.Visible = False
        End If
    End Sub
    Private Sub RefreshState_Pricelist()
        lblP51ID_Billing.Visible = True : Me.p51ID_Billing.Visible = True
        Select Case Me.opgPriceList.SelectedValue
            Case "1"
                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False
                cmdNewP51.Visible = False
                cmdEditP51.Visible = False
            Case "2"
                Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(0)"
                Me.cmdNewP51.Visible = True : cmdEditP51.Visible = True
                If Me.p51ID_Billing.SelectedValue <> "" Then
                    cmdEditP51.NavigateUrl = "javascript:p51_edit(" & Me.p51ID_Billing.SelectedValue & ")"
                    cmdEditP51.Visible = True
                Else
                    cmdEditP51.Visible = False
                End If

            Case "3"
                If BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value) <> 0 Then
                    cmdEditP51.NavigateUrl = "javascript:p51_edit(" & Me.hidP51ID_Tailor.Value & ")"
                    Me.cmdNewP51.Visible = False
                    cmdEditP51.Visible = True
                Else
                    Me.cmdNewP51.Visible = True
                    Me.cmdNewP51.NavigateUrl = "javascript:p51_billing_add(1)"
                    cmdEditP51.Visible = False
                End If

                lblP51ID_Billing.Visible = False
                Me.p51ID_Billing.Visible = False

        End Select
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p28ContactBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p28-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub SaveTempO37()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o37"))
        For Each ri As RepeaterItem In rpO37.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("o36id"), DropDownList).SelectedValue)
                .p85FreeText01 = CType(ri.FindControl("o38City"), TextBox).Text
                .p85FreeText02 = CType(ri.FindControl("o38Street"), TextBox).Text
                .p85FreeText03 = CType(ri.FindControl("o38ZIP"), TextBox).Text
                .p85FreeText04 = CType(ri.FindControl("o38Country"), UI.datacombo).Text
                .p85FreeText05 = CType(ri.FindControl("o38Name"), TextBox).Text
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub
    Private Sub SaveTempO32()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o32"))
        For Each ri As RepeaterItem In rpO32.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("o33id"), DropDownList).SelectedValue)
                .p85FreeText01 = CType(ri.FindControl("o32Value"), TextBox).Text
                .p85FreeText02 = CType(ri.FindControl("o32Description"), TextBox).Text
                .p85FreeBoolean01 = CType(ri.FindControl("o32IsDefaultInInvoice"), CheckBox).Checked
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub
    

    
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveTempO37()
        SaveTempO32()

        roles1.SaveCurrentTempData()

        With Master.Factory.p28ContactBL
            Dim cRec As BO.p28Contact = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p28Contact)
            With cRec
                If .PID = 0 Then .p28IsDraft = Me.p28IsDraft.Checked
                .p28IsCompany = BO.BAS.BG(Me.p28IsCompany.SelectedValue)
                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
                .p29ID = BO.BAS.IsNullInt(Me.p29ID.SelectedValue)
                .p63ID = BO.BAS.IsNullInt(Me.p63ID.SelectedValue)
                .j61ID_Invoice = BO.BAS.IsNullInt(Me.j61ID_Invoice.SelectedValue)
                .p28ParentID = BO.BAS.IsNullInt(Me.p28ParentID.Value)
                If .p28IsCompany Then
                    .p28CompanyName = Me.p28CompanyName.Text
                    .p28CompanyShortName = Me.p28CompanyShortName.Text
                Else
                    .p28TitleBeforeName = Me.p28TitleBeforeName.Text
                    .p28FirstName = Me.p28FirstName.Text
                    .p28LastName = Me.p28LastName.Text
                    .p28TitleAfterName = Me.p28TitleAfterName.Text
                End If
                .p28RegID = Me.p28RegID.Text
                .p28VatID = Me.p28VatID.Text
                .p28InvoiceMaturityDays = BO.BAS.IsNullInt(Me.p28InvoiceMaturityDays.Value)
                Select Case Me.opgPriceList.SelectedValue
                    Case "1"
                        .p51ID_Billing = 0
                    Case "2"
                        .p51ID_Billing = BO.BAS.IsNullInt(Me.p51ID_Billing.SelectedValue)
                    Case "3"
                        .p51ID_Billing = BO.BAS.IsNullInt(Me.hidP51ID_Tailor.Value)
                End Select
                If Me.opgPriceList.SelectedValue <> "1" And .p51ID_Billing = 0 Then
                    Master.Notify("Chybí ceník sazeb.", NotifyLevel.WarningMessage) : Return
                End If
                .p87ID = BO.BAS.IsNullInt(Me.p87ID.SelectedValue)
                .p92ID = BO.BAS.IsNullInt(Me.p92id.SelectedValue)
                .p28InvoiceDefaultText1 = Me.p28InvoiceDefaultText1.Text
                .p28InvoiceDefaultText2 = Me.p28InvoiceDefaultText2.Text
                .p51ID_Internal = BO.BAS.IsNullInt(Me.p51ID_Internal.SelectedValue)
                .p28RobotAddress = Me.p28RobotAddress.Text
                .p28ExternalPID = Me.p28ExternalPID.Text
                .p28SupplierFlag = CInt(Me.p28SupplierFlag.SelectedValue)
                If .p28SupplierFlag = BO.p28SupplierFlagENUM.ClientAndSupplier Or .p28SupplierFlag = BO.p28SupplierFlagENUM.SupplierOnly Then
                    .p28SupplierID = Me.p28SupplierID.Text
                End If

                If Me.chkDefineLimits.Checked Then
                    .p28LimitHours_Notification = BO.BAS.IsNullNum(Me.p28LimitHours_Notification.Value)
                    .p28LimitFee_Notification = BO.BAS.IsNullNum(Me.p28LimitFee_Notification.Value)
                Else
                    .p28LimitHours_Notification = 0 : .p28LimitFee_Notification = 0
                End If
                .p28BillingMemo = Trim(Me.p28BillingMemo.Text)
                .p28Pohoda_VatCode = Me.p28Pohoda_VatCode.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .j02ID_ContactPerson_DefaultInWorksheet = BO.BAS.IsNullInt(Me.j02ID_ContactPerson_DefaultInWorksheet.SelectedValue)
                .j02ID_ContactPerson_DefaultInInvoice = BO.BAS.IsNullInt(Me.j02ID_ContactPerson_DefaultInInvoice.SelectedValue)
            End With

            Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o37"), True)
            Dim lisO37 As New List(Of BO.o37Contact_Address)
            For Each cTMP In lisTEMP
                Dim c As New BO.o37Contact_Address
                With cTMP
                    c.IsSetAsDeleted = .p85IsDeleted
                    c.o36ID = .p85OtherKey1
                    c.o38ID = .p85OtherKey2
                    c.o38City = .p85FreeText01
                    c.o38Street = .p85FreeText02
                    c.o38ZIP = .p85FreeText03
                    c.o38Country = .p85FreeText04
                    c.o38Name = .p85FreeText05
                    c.o38AresID = .p85FreeText09
                End With
                lisO37.Add(c)
            Next
            lisTEMP = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o32"), True)
            Dim lisO32 As New List(Of BO.o32Contact_Medium)
            For Each cTMP In lisTEMP
                Dim c As New BO.o32Contact_Medium
                With cTMP
                    c.SetPID(.p85DataPID)
                    c.IsSetAsDeleted = .p85IsDeleted
                    c.o33ID = .p85OtherKey1
                    c.o32Value = .p85FreeText01
                    c.o32Description = .p85FreeText02
                    c.o32IsDefaultInInvoice = .p85FreeBoolean01
                End With
                lisO32.Add(c)
            Next

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If
            Dim lisP30 As List(Of BO.p30Contact_Person) = Nothing
            lisTEMP = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30"))
            For Each cTMP In lisTEMP
                If lisP30 Is Nothing Then lisP30 = New List(Of BO.p30Contact_Person)
                Dim c As New BO.p30Contact_Person
                c.j02ID = cTMP.p85OtherKey1
                lisP30.Add(c)
            Next


            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()


            If .Save(cRec, lisO37, lisO32, lisP30, lisX69, lisFF) Then
                Dim bolNew As Boolean = Master.IsRecordNew
                Master.DataPID = .LastSavedPID

                Master.Factory.o51TagBL.SaveBinding("p28", Master.DataPID, tags1.Geto51IDs())
                If Not bolNew Or ff1.GetX20IDs.Count > 0 Then
                    Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p28Contact, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                End If

                If bolNew Then
                    Master.CloseAndRefreshParent("p28-create")
                Else
                    Master.CloseAndRefreshParent("p28-save")
                End If

            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub rpO37_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpO37.ItemCommand
        SaveTempO37()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If cRec.p85DataPID > 0 Then
                Dim mq As New BO.myQueryP91
                mq.o38ID = cRec.p85DataPID
                Dim lisTest As IEnumerable(Of BO.p91Invoice) = Master.Factory.p91InvoiceBL.GetList(mq)
                If lisTest.Count > 0 Then
                    Master.Notify("Tuto adresu není možné odstranit, protože má vazbu na " & lisTest.Count.ToString & " klientských faktur (první z nich: " & lisTest(0).p91Code & ").", NotifyLevel.WarningMessage)
                    Return
                End If
            End If

            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempO37()
            End If
        End If
    End Sub

    
    Private Sub cmdAddO37_Click(sender As Object, e As EventArgs) Handles cmdAddO37.Click
        SaveTempO37()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid_o37")
        cRec.p85OtherKey1 = BO.o36IdEnum.InvoiceAddress
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempO37()
    End Sub

    Private Sub p29ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p29ID.NeedMissingItem
        Dim cRec As BO.p29ContactType = Master.Factory.p29ContactTypeBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.p29Name
        End If
    End Sub

    Private Sub p51ID_Billing_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID_Billing.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.NameWithCurr
        End If
    End Sub

    Private Sub rpO32_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpO32.ItemCommand
        SaveTempO32()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then

            End If
        End If
        RefreshTempO32()
    End Sub

    Private Sub rpO32_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rpO32.ItemCreated
        AddHandler CType(e.Item.FindControl("o33id"), DropDownList).SelectedIndexChanged, AddressOf Me.o33id_OnChange
    End Sub
    Private Sub o33id_OnChange(sender As Object, e As EventArgs)
        Dim cbx As DropDownList = DirectCast(sender, DropDownList)
        If cbx.SelectedValue = "2" Then
            cbx.FindControl("o32IsDefaultInInvoice").Visible = True
        Else
            cbx.FindControl("o32IsDefaultInInvoice").Visible = False
        End If
    End Sub

    Private Sub rpO32_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO32.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With

        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("o33id"), DropDownList), .p85OtherKey1.ToString)
            CType(e.Item.FindControl("o32Value"), TextBox).Text = .p85FreeText01
            CType(e.Item.FindControl("o32Description"), TextBox).Text = .p85FreeText02
            CType(e.Item.FindControl("o32IsDefaultInInvoice"), CheckBox).Checked = .p85FreeBoolean01
            If .p85OtherKey1 = 2 Then
                e.Item.FindControl("o32IsDefaultInInvoice").Visible = True
            Else
                e.Item.FindControl("o32IsDefaultInInvoice").Visible = False
            End If

        End With
    End Sub

    Private Sub cmdAddO32_Click(sender As Object, e As EventArgs) Handles cmdAddO32.Click
        SaveTempO32()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid_o32")
        cRec.p85OtherKey1 = BO.o33FlagEnum.Email
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempO32()
    End Sub



    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click
        Dim strPID As String = Me.HardRefreshPID.Value
        
        Select Case Me.HardRefreshFlag.Value
            Case "p51-save"
                SetupPriceList()
                Me.p51ID_Billing.SelectedValue = strPID
                Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strPID))
                If cRec.p51IsCustomTailor Then
                    hidP51ID_Tailor.Value = cRec.PID.ToString
                    Me.opgPriceList.SelectedValue = "3"
                Else
                    hidP51ID_Tailor.Value = ""
                    Me.opgPriceList.SelectedValue = "2"
                End If
                RefreshState_Pricelist()
            Case "p51-delete"
                SetupPriceList()
            Case "j02-save" 'kontaktní osoba
                Dim intJ02ID As Integer = BO.BAS.IsNullInt(strPID)
                Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(intJ02ID)
                If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30")).Where(Function(p) p.p85OtherKey1 = intJ02ID).Count = 0 Then
                    Dim cTemp As New BO.p85TempBox
                    cTemp.p85GUID = ViewState("guid_p30")
                    cTemp.p85OtherKey1 = c.PID
                    cTemp.p85FreeText01 = c.FullNameDesc
                    Master.Factory.p85TempBoxBL.Save(cTemp)
                Else

                End If
                RefreshTempP30()
            Case "j02-delete"   'kontaktní osoba
                Dim intJ02ID As Integer = BO.BAS.IsNullInt(strPID)
                If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30")).Where(Function(p) p.p85OtherKey1 = intJ02ID).Count > 0 Then
                    Master.Factory.p85TempBoxBL.Delete(Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30")).Where(Function(p) p.p85OtherKey1 = intJ02ID)(0))
                End If
                RefreshTempP30()
        End Select

        Me.HardRefreshPID.Value = ""
        Me.HardRefreshFlag.Value = ""
    End Sub
    Private Sub p51ID_Billing_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p51ID_Billing.SelectedIndexChanged
        RefreshState_Pricelist()
    End Sub

    Private Sub cmdARES_Click(sender As Object, e As EventArgs) Handles cmdARES.Click
        If Trim(Me.p28RegID.Text) = "" Then
            Master.Notify("Musíte vyplnit IČ.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim cAres As New clsAresImport()
        Dim cRec As BO.AresRecord = cAres.LoadAresRecord(Trim(Replace(Me.p28RegID.Text, " ", "")))
        If cRec Is Nothing Then
            Master.Notify("ARES záznam nebylo možné načíst, chyba: " & cAres.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            p28IsCompany.SelectedIndex = 0
            Me.p28CompanyName.Text = cRec.Company
            If Me.p28VatID.Text = "" Then Me.p28VatID.Text = cRec.DIC
            SaveTempO37()
            Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o37"))
            Dim cTemp As New BO.p85TempBox()
            If lisTemp.Where(Function(p) p.p85FreeText09 = cRec.ID_adresy).Count > 0 Then
                cTemp = lisTemp(0)
            End If
            With cTemp
                .p85GUID = ViewState("guid_o37")
                .p85OtherKey1 = BO.o36IdEnum.InvoiceAddress
                .p85FreeText01 = cRec.City
                .p85FreeText02 = cRec.Street
                .p85FreeText03 = cRec.PostCode
                .p85FreeText04 = cRec.Country
                .p85FreeText09 = cRec.ID_adresy
            End With

            Master.Factory.p85TempBoxBL.Save(cTemp)
            RefreshTempO37()

        End If
    End Sub

  

    
    Private Sub p51ID_Internal_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID_Internal.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p51Name
    End Sub

    Private Sub p29ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p29ID.SelectedIndexChanged
        Handle_FF()
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub TryInhaleInitialData()
        Dim cRecLast As BO.p28Contact = Master.Factory.p28ContactBL.LoadMyLastCreated()
        If cRecLast Is Nothing Then Return
        With cRecLast
            Me.p29ID.SelectedValue = .p29ID.ToString
            Me.p28IsCompany.SelectedValue = BO.BAS.GB(.p28IsCompany)
            Me.p28InvoiceMaturityDays.Value = .p28InvoiceMaturityDays
            roles1.InhaleInitialData(.PID)
        End With

    End Sub

    Private Sub p63ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p63ID.NeedMissingItem
        Dim cRec As BO.p63Overhead = Master.Factory.p63OverheadBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.NameWithRate
    End Sub

    Private Sub cmdVIES_Click(sender As Object, e As EventArgs) Handles cmdVIES.Click
        Dim strVAT As String = Trim(Me.p28VatID.Text)
        If strVAT = "" Then
            Master.Notify("Musíte vyplnit DIČ.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim bolValid As Boolean = False, strName As String = "", strAddress = ""
        Try
            Dim c As New VatService.checkVatPortTypeClient
            c.checkVat(Left(strVAT, 2), Right(strVAT, Len(strVAT) - 2), bolValid, strName, strAddress)
            If Not bolValid Then
                Master.Notify("Neplatné DIČ", NotifyLevel.ErrorMessage)
                Return
            End If
        Catch ex As Exception
            Master.Notify("VIES web service, Error: " & ex.Message, NotifyLevel.ErrorMessage)
            Return
        End Try
        Me.p28VatID.Text = strVAT
        p28IsCompany.SelectedIndex = 0
        Me.p28CompanyName.Text = strName

        SaveTempO37()
        Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_o37"))
        Dim cTemp As New BO.p85TempBox()
        If lisTemp.Where(Function(p) p.p85FreeText09 = "vies").Count > 0 Then
            cTemp = lisTemp(0)
        End If
        Dim a() As String = Split(strAddress & Chr(10) & Chr(10), Chr(10))
        If Len(strAddress) > 10 Then
            With cTemp
                .p85GUID = ViewState("guid_o37")
                .p85OtherKey1 = BO.o36IdEnum.InvoiceAddress
                .p85FreeText01 = a(1) 'město na druhém místě
                .p85FreeText02 = a(0)   'ulice na prvním místě

                Select Case Left(strVAT, 2)
                    Case "CZ"
                        .p85FreeText04 = "Česká republika"
                        If Len(a(2)) >= 6 Then .p85FreeText03 = Trim(Left(a(2), 6)) 'psč
                    Case "SK"
                        .p85FreeText04 = "Slovenská republika"
                        .p85FreeText03 = Trim(Left(a(1), 6)) 'psč u slováků brát z ulice
                        .p85FreeText01 = Trim(Replace(.p85FreeText01, Left(a(1), 6), ""))  'město
                    Case "DE" : .p85FreeText04 = "Bundesrepublik Deutschland"
                    Case "AT" : .p85FreeText04 = "Republik Österreich"
                    Case Else

                End Select

                .p85FreeText09 = "vies"
            End With

            Master.Factory.p85TempBoxBL.Save(cTemp)
        Else
            Master.Notify("Subjekt byl nalezen v registru VIES, ale adresu nelze načíst.", NotifyLevel.InfoMessage)
        End If
        
        RefreshTempO37()
    End Sub

    Private Sub chkWhisper_CheckedChanged(sender As Object, e As EventArgs) Handles chkWhisper.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p28_record-chkWhisper", BO.BAS.GB(Me.chkWhisper.Checked))
    End Sub

    Private Sub rpP30_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP30.ItemCommand
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            Master.Factory.p85TempBoxBL.Delete(cRec)
        End If
        RefreshTempP30()
    End Sub

    Private Sub rpP30_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP30.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), Button)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        CType(e.Item.FindControl("cmdDeletePerson"), HtmlButton).Attributes.Item("onclick") = "j02_delete(" & cRec.p85OtherKey1.ToString & ")"

        Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(cRec.p85OtherKey1)
        If Not c Is Nothing Then
            With cRec
                CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
                basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p27id"), DropDownList), .p85OtherKey2.ToString)
                With CType(e.Item.FindControl("linkPerson"), HyperLink)
                    .Text = c.FullNameAsc
                    .NavigateUrl = "javascript:j02_record(" & c.PID.ToString & ")"
                    If c.IsClosed Then .Font.Strikeout = True
                    If c.j02IsInvoiceEmail Then .ForeColor = Drawing.Color.Green
                End With

                CType(e.Item.FindControl("j02Email"), Label).Text = c.j02Email
                ''CType(e.Item.FindControl("j02Email"), HyperLink).NavigateUrl = "mailto:" & c.j02Email
                CType(e.Item.FindControl("j02Mobile"), Label).Text = c.j02Mobile
                CType(e.Item.FindControl("j02JobTitle"), Label).Text = c.j02JobTitle

                CType(e.Item.FindControl("clue_j02"), HyperLink).Attributes("rel") = "clue_j02_record.aspx?pid=" & .p85OtherKey1.ToString

            End With
        Else
            e.Item.FindControl("clue_j02").Visible = False
            e.Item.FindControl("cmdDeletePerson").Visible = False
            e.Item.FindControl("del").Visible = False
        End If
        
    End Sub

    Private Sub j02ID_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles j02ID.AutoPostBack_SelectedIndexChanged
        Dim intJ02ID As Integer = BO.BAS.IsNullInt(Me.j02ID.Value)
        If intJ02ID = 0 Then Return
        Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(intJ02ID)
        If Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p30")).Where(Function(p) p.p85OtherKey1 = intJ02ID).Count > 0 Then
            Master.Notify(String.Format("{0} již je v seznamu kontaktní osob klienta.", c.FullNameAsc), NotifyLevel.WarningMessage)
            Return
        Else
            Dim cTemp As New BO.p85TempBox
            cTemp.p85GUID = ViewState("guid_p30")
            cTemp.p85OtherKey1 = c.PID
            cTemp.p85FreeText01 = c.FullNameDesc
            Master.Factory.p85TempBoxBL.Save(cTemp)
        End If
        RefreshTempP30()
        Me.j02ID.Text = ""
        Me.j02ID.Value = ""
    End Sub
End Class