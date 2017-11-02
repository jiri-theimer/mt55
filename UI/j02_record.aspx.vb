Public Class j02_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j02_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "j02_record"


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            Me.hidGUID.Value = Request.Item("guid")
            If Request.Item("iscontact") = "1" Then
                'režim zakládání kontaktní osoby
                Me.j02IsIntraPerson.SelectedValue = "0"
                Me.j02IsIntraPerson.Enabled = False
                
            Else
                Master.neededPermission = BO.x53PermValEnum.GR_Admin
            End If
            With Master
                .HeaderIcon = "Images/person_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Osobní profil"

                Me.c21ID.DataSource = .Factory.c21FondCalendarBL.GetList(New BO.myQuery)
                Me.c21ID.DataBind()
                Me.j17ID.DataSource = .Factory.j17CountryBL.GetList(New BO.myQuery)
                Me.j17ID.DataBind()
                Me.j18ID.DataSource = Master.Factory.j18RegionBL.GetList(New BO.myQuery)
                Me.j18ID.DataBind()
                Me.o40ID.DataSource = .Factory.o40SmtpAccountBL.GetList(New BO.myQuery)
                Me.o40ID.DataBind()
                Me.j02TimesheetEntryDaysBackLimit_p34IDs.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery).Where(Function(p) p.p33ID = BO.p33IdENUM.Cas Or p.p33ID = BO.p33IdENUM.Kusovnik)
                Me.j02TimesheetEntryDaysBackLimit_p34IDs.DataBind()
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        j07ID.DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
        j07ID.DataBind()


        If Master.DataPID = 0 Then
            Handle_FF()
            Return
        End If

        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        With cRec
            Me.j02Email.Text = .j02Email
            Me.j02Code.Text = .j02Code
            Me.j02FirstName.Text = .j02FirstName
            Me.j02LastName.Text = .j02LastName
            Me.j02TitleAfterName.SetText(.j02TitleAfterName)
            Me.j02TitleBeforeName.SetText(.j02TitleBeforeName)
            Me.j02Mobile.Text = .j02Mobile
            Me.j02Phone.Text = .j02Phone
            Me.j07ID.SelectedValue = .j07ID.ToString
            Handle_FF()
            Me.j17ID.SelectedValue = .j17ID.ToString
            Me.c21ID.SelectedValue = .c21ID.ToString
            Me.j18ID.SelectedValue = .j18ID.ToString
            Me.o40ID.SelectedValue = .o40ID.ToString
            Me.j02Office.Text = .j02Office
            Me.j02Salutation.Text = .j02Salutation
            Me.j02EmailSignature.Text = .j02EmailSignature
            Me.j02IsIntraPerson.SelectedValue = BO.BAS.GB(.j02IsIntraPerson)
            Me.j02JobTitle.SetText(.j02JobTitle)
            Me.j02RobotAddress.Text = .j02RobotAddress
            Me.j02ExternalPID.Text = .j02ExternalPID
            Me.j02DomainAccount.Text = .j02DomainAccount
            Me.j02IsInvoiceEmail.Checked = .j02IsInvoiceEmail
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            basUI.SelectDropdownlistValue(Me.j02TimesheetEntryDaysBackLimit, .j02TimesheetEntryDaysBackLimit.ToString)
            If .j02TimesheetEntryDaysBackLimit_p34IDs <> "" Then
                Dim lis As List(Of String) = BO.BAS.ConvertPIDs2List(.j02TimesheetEntryDaysBackLimit_p34IDs).Select(Function(p) p.ToString).ToList
                Me.j02TimesheetEntryDaysBackLimit_p34IDs.SelectCheckboxItems(lis)

            End If
            basUI.SelectDropdownlistValue(Me.j02WorksheetAccessFlag, CInt(.j02WorksheetAccessFlag).ToString)
            basUI.SelectDropdownlistValue(Me.p72ID_NonBillable, CInt(.p72ID_NonBillable).ToString)
            Me.j02AvatarImage.Value = .j02AvatarImage
            Master.Timestamp = .Timestamp & " <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"

            
            tags1.RefreshData(cRec.PID)

            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.j02Person, Master.DataPID, BO.BAS.IsNullInt(Me.j07ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.j02Person, BO.BAS.IsNullInt(Me.j07ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "j02Person_FreeField", Master.DataPID)
                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)

            End If
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j02PersonBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j02-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Master.DataPID = 0 And panCreateContactPerson.Visible And Me.j02IsIntraPerson.SelectedValue = "0" And hidGUID.Value = "" Then
            If Me.p28ID.Value = "" And Me.p41ID.Value = "" Then
                Master.Notify("Pro kontaktní osobu musíte vybrat klienta nebo projekt.", NotifyLevel.WarningMessage) : Return
            End If
            If Me.p28ID.Value <> "" And Me.p41ID.Value <> "" Then
                Master.Notify("Vyberte buď klienta nebo projekt, ale nikoliv obojí dohromady.", NotifyLevel.WarningMessage) : Return
            End If
        End If
        With Master.Factory.j02PersonBL
            Dim cRec As BO.j02Person = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j02Person)
            With cRec
                .j07ID = BO.BAS.IsNullInt(j07ID.SelectedValue)
                .j17ID = BO.BAS.IsNullInt(j17ID.SelectedValue)
                .c21ID = BO.BAS.IsNullInt(c21ID.SelectedValue)
                .j18ID = BO.BAS.IsNullInt(Me.j18ID.SelectedValue)
                .o40ID = BO.BAS.IsNullInt(Me.o40ID.SelectedValue)
                .j02FirstName = j02FirstName.Text
                .j02LastName = j02LastName.Text
                .j02TitleBeforeName = j02TitleBeforeName.Text
                .j02TitleAfterName = j02TitleAfterName.Text
                .j02Code = Me.j02Code.Text
                .j02Mobile = j02Mobile.Text
                .j02Phone = j02Phone.Text
                .j02Office = Me.j02Office.Text
                .j02Salutation = Me.j02Salutation.Text
                .j02Email = j02Email.Text
                .j02EmailSignature = j02EmailSignature.Text
                .j02IsIntraPerson = BO.BAS.BG(Me.j02IsIntraPerson.SelectedValue)
                .j02JobTitle = Me.j02JobTitle.Text
                .j02RobotAddress = Me.j02RobotAddress.Text
                .j02ExternalPID = Me.j02ExternalPID.Text
                .j02TimesheetEntryDaysBackLimit = BO.BAS.IsNullInt(Me.j02TimesheetEntryDaysBackLimit.SelectedValue)
                .j02TimesheetEntryDaysBackLimit_p34IDs = String.Join(",", Me.j02TimesheetEntryDaysBackLimit_p34IDs.GetAllCheckedValues)
                .j02WorksheetAccessFlag = BO.BAS.IsNullInt(Me.j02WorksheetAccessFlag.SelectedValue)
                .p72ID_NonBillable = BO.BAS.IsNullInt(Me.p72ID_NonBillable.SelectedValue)
                .j02AvatarImage = Me.j02AvatarImage.Value
                .j02DomainAccount = Me.j02DomainAccount.Text
                .j02IsInvoiceEmail = Me.j02IsInvoiceEmail.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil

            End With

            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()

            If .Save(cRec, lisFF) Then
                Master.DataPID = .LastSavedPID
                Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.j02Person, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                Master.Factory.o51TagBL.SaveBinding("j02", Master.DataPID, tags1.Geto51IDs())
                If Me.hidGUID.Value <> "" Then
                    Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
                    Dim cTemp As New BO.p85TempBox
                    cTemp.p85GUID = Me.hidGUID.Value

                    cTemp.p85DataPID = Master.DataPID
                    cTemp.p85FreeText01 = c.FullNameAsc
                    Master.Factory.p85TempBoxBL.Save(cTemp)
                End If
                If cRec.PID = 0 And panCreateContactPerson.Visible And cRec.j02IsIntraPerson = False And hidGUID.Value = "" Then
                    'kontaktní osoba
                    Dim cP30 As New BO.p30Contact_Person
                    cP30.j02ID = Master.DataPID
                    If BO.BAS.IsNullInt(Me.p28ID.Value, 0) > 0 Then
                        cP30.p28ID = CInt(Me.p28ID.Value)
                    End If
                    If BO.BAS.IsNullInt(Me.p41ID.Value, 0) > 0 Then
                        cP30.p41ID = CInt(Me.p41ID.Value)
                    End If
                    Master.Factory.p30Contact_PersonBL.Save(cP30)
                    Master.CloseAndRefreshParent("j02-save")
                    Return
                End If
                If cRec.PID = 0 And cRec.j02IsIntraPerson = True Then
                    Server.Transfer("j03_create.aspx?j02id=" & Master.DataPID.ToString)
                Else
                    Master.CloseAndRefreshParent("j02-save")
                End If

            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

   
    Private Sub j07ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j07ID.NeedMissingItem
        Dim cRec As BO.j07PersonPosition = Master.Factory.j07PersonPositionBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        strAddMissingItemText = cRec.j07Name
    End Sub

    Private Sub c21ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles c21ID.NeedMissingItem
        Dim cRec As BO.c21FondCalendar = Master.Factory.c21FondCalendarBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.c21Name
    End Sub

    Private Sub j17ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j17ID.NeedMissingItem
        Dim cRec As BO.j17Country = Master.Factory.j17CountryBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.j17Name
    End Sub

    Private Sub j18ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j18ID.NeedMissingItem
        Dim cRec As BO.j18Region = Master.Factory.j18RegionBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.j18Name
    End Sub

    Private Sub j02_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Dim b As Boolean = BO.BAS.BG(Me.j02IsIntraPerson.SelectedValue)
        lblJ07ID.Visible = b : Me.j07ID.Visible = b
        lblJ18ID.Visible = b : Me.j18ID.Visible = b
        lblC21ID.Visible = b : Me.c21ID.Visible = b
        trJ17ID.Visible = b
        trO40.Visible = b
        lblj02EmailSignature.Visible = b : Me.j02EmailSignature.Visible = b

        Me.panCreateContactPerson.Visible = False
        If Not b And hidGUID.Value = "" And Master.DataPID = 0 Then
            Me.panCreateContactPerson.Visible = True
        End If


        trJobTitle.Visible = Not b
        j02IsInvoiceEmail.Visible = Not b
        lblDomain.Visible = b
        j02DomainAccount.Visible = b

        If Me.j02IsIntraPerson.SelectedValue = "1" Then

            RadTabStrip1.Tabs.FindTabByValue("other").Style.Item("display") = "block"
        Else

            RadTabStrip1.Tabs.FindTabByValue("other").Style.Item("display") = "none"
        End If
        If Me.j02AvatarImage.Value <> "" Then
            imgAvatar.ImageUrl = "Plugins/Avatar/" & Me.j02AvatarImage.Value
            cmdDeleteAvatar.Visible = True
        Else
            imgAvatar.ImageUrl = "Images/nophoto.png"
            cmdDeleteAvatar.Visible = False
        End If
        cmdUploadAvatar.Visible = Not cmdDeleteAvatar.Visible

    End Sub

    Private Sub cmdUploadAvatar_Click(sender As Object, e As EventArgs) Handles cmdUploadAvatar.Click
        Dim strErr As String = ""
        Dim strJ02AvatarImage As String = basUIMT.UploadAvatarImage(upload1, Master.Factory.SysUser.j02ID, strErr)
        If strJ02AvatarImage = "" Then
            Master.Notify(strErr, NotifyLevel.ErrorMessage)
        Else
            Me.j02AvatarImage.Value = strJ02AvatarImage
            Master.Notify("Obrázek se uloží do osobního profilu až stisknutím tlačítka [Uložit změny].")
        End If

        

    End Sub

    Private Sub cmdDeleteAvatar_Click(sender As Object, e As EventArgs) Handles cmdDeleteAvatar.Click
        Me.j02AvatarImage.Value = ""
        Master.Notify("Změna se uloží do osobního profilu až stisknutím tlačítka [Uložit změny].")
    End Sub
End Class