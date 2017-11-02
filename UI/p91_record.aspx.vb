Public Class p91_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p91_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        roles1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master

                .HeaderIcon = "Images/invoice_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    .StopPage("pid missing")
                End If
                .AddToolbarButton("Odstranit", "mydelete", 2, "Images/delete.png")

                Me.j17ID.DataSource = .Factory.j17CountryBL.GetList()
                Me.j17ID.DataBind()
                Me.p92ID.DataSource = .Factory.p92InvoiceTypeBL.GetList(New BO.myQuery)
                Me.p92ID.DataBind()
                Me.p98ID.DataSource = .Factory.p98Invoice_Round_Setting_TemplateBL.GetList()
                Me.p98ID.DataBind()
                Me.p63ID.DataSource = .Factory.p63OverheadBL.GetList(New BO.myQuery)
                Me.p63ID.DataBind()
                Me.p80ID.DataSource = .Factory.p80InvoiceAmountStructureBL.GetList(New BO.myQuery)
                Me.p80ID.DataBind()
               
            End With


            RefreshRecord()
            Master.IsRecordDeletable = False

            If Request.Item("tab") <> "" Then
                With RadTabStrip1.FindTabByValue(Request.Item("tab"))
                    Try
                        .Selected = True
                        If Not RadMultiPage1.FindPageViewByID(Request.Item("tab")) Is Nothing Then
                            RadMultiPage1.FindPageViewByID(Request.Item("tab")).Selected = True
                        End If
                    Catch ex As Exception
                    End Try
                End With
            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
        With cRec
            Master.HeaderText = "Nastavení faktury " & .p91Code
            Me.p91Code.Text = .p91Code
            Me.p91Code.NavigateUrl = "javascript:recordcode()"
            Me.j17ID.SelectedValue = .j17ID.ToString
            Me.p98ID.SelectedValue = .p98ID.ToString
            Me.p63ID.SelectedValue = .p63ID.ToString
            Me.p80ID.SelectedValue = .p80ID.ToString
            Me.p92ID.SelectedValue = .p92ID.ToString
            Me.p28ID.Value = .p28ID.ToString
            Me.p28ID.Text = .p28Name
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.p91text1.Text = .p91Text1
            Me.p91text2.Text = .p91Text2
            Me.p91Date.SelectedDate = .p91Date
            Me.p91DateMaturity.SelectedDate = .p91DateMaturity
            Me.p91DateSupply.SelectedDate = .p91DateSupply
            Me.p91Datep31_From.selecteddate = .p91Datep31_From
            Me.p91Datep31_Until.SelectedDate = .p91Datep31_Until

            Me.p91Client.Text = .p91Client
            Me.p91ClientPerson.Text = .p91ClientPerson
            Me.p91ClientPerson_Salutation.Text = .p91ClientPerson_Salutation
            Me.p91Client_RegID.Text = .p91Client_RegID
            Me.p91Client_VatID.Text = .p91Client_VatID
            Me.p91ClientAddress1_City.Text = .p91ClientAddress1_City
            Me.p91ClientAddress1_Street.Text = .p91ClientAddress1_Street
            Me.p91ClientAddress1_ZIP.Text = .p91ClientAddress1_ZIP
            Me.p91ClientAddress1_Country.Text = .p91ClientAddress1_Country
            Me.p91ClientAddress2.Text = .p91ClientAddress2

            InhaleAddresses()
            If .p28ID <> 0 Then
                Me.o38ID_Primary.SelectedValue = .o38ID_Primary.ToString
                Me.o38ID_Delivery.SelectedValue = .o38ID_Delivery.ToString

                Dim mq As New BO.myQueryJ02
                mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
                mq.p28ID = .p28ID
                Me.j02ID_ContactPerson.DataSource = Master.Factory.j02PersonBL.GetList(mq).ToList
                Me.j02ID_ContactPerson.DataBind()
                Me.j02ID_ContactPerson.SelectedValue = .j02ID_ContactPerson.ToString

            End If

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp & " <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"

        End With
        roles1.InhaleInitialData(cRec.PID)

        Handle_FF()
        
    End Sub

    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p91Invoice, Master.DataPID, BO.BAS.IsNullInt(Me.p92ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p91Invoice, BO.BAS.IsNullInt(Me.p92ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "p91Invoice_FreeField", Master.DataPID)
                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)
            End If
        End With
    End Sub

    Private Sub InhaleAddresses()
        Dim intP28ID As Integer = BO.BAS.IsNullInt(Me.p28ID.Value)
        Dim lis As IEnumerable(Of BO.o38Address) = Master.Factory.p28ContactBL.GetList_o37(intP28ID)
        Me.o38ID_Primary.DataSource = lis
        Me.o38ID_Primary.DataBind()
        If lis.Count > 0 Then
            Me.o38ID_Primary.SelectedIndex = 1
        End If
        Me.o38ID_Delivery.DataSource = lis
        Me.o38ID_Delivery.DataBind()
    End Sub
    ''Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
    ''    With Master.Factory.p91InvoiceBL
    ''        If .Delete(Master.DataPID) Then
    ''            Master.DataPID = 0
    ''            Master.CloseAndRefreshParent("p91-delete")
    ''        Else
    ''            Master.Notify(.ErrorMessage, 2)
    ''        End If
    ''    End With
    ''End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()

        With Master.Factory.p91InvoiceBL
            Dim cRec As BO.p91Invoice = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p91Invoice)
            With cRec
                .p92ID = BO.BAS.IsNullInt(Me.p92ID.SelectedValue)
                .j17ID = BO.BAS.IsNullInt(Me.j17ID.SelectedValue)
                .p98ID = BO.BAS.IsNullInt(Me.p98ID.SelectedValue)
                .p63ID = BO.BAS.IsNullInt(Me.p63ID.SelectedValue)
                .p80ID = BO.BAS.IsNullInt(Me.p80ID.SelectedValue)
                .p28ID = BO.BAS.IsNullInt(Me.p28ID.Value)
                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)
                .o38ID_Primary = BO.BAS.IsNullInt(Me.o38ID_Primary.SelectedValue)
                .o38ID_Delivery = BO.BAS.IsNullInt(Me.o38ID_Delivery.SelectedValue)
                .j02ID_ContactPerson = BO.BAS.IsNullInt(Me.j02ID_ContactPerson.SelectedValue)
                .p91Text1 = Me.p91text1.Text
                .p91Text2 = Me.p91text2.Text
                .p91Date = Me.p91Date.SelectedDate
                .p91DateMaturity = Me.p91DateMaturity.SelectedDate
                .p91DateSupply = Me.p91DateSupply.SelectedDate
                .p91Datep31_From = BO.BAS.IsNullDBDate(Me.p91Datep31_From.SelectedDate)
                .p91Datep31_Until = BO.BAS.IsNullDBDate(Me.p91Datep31_Until.SelectedDate)


                .p91Client = Me.p91Client.Text
                .p91ClientPerson = Me.p91ClientPerson.Text
                .p91ClientPerson_Salutation = Me.p91ClientPerson_Salutation.Text
                .p91Client_RegID = Me.p91Client_RegID.Text
                .p91Client_VatID = Me.p91Client_VatID.Text
                .p91ClientAddress1_City = Me.p91ClientAddress1_City.Text
                .p91ClientAddress1_Street = Me.p91ClientAddress1_Street.Text
                .p91ClientAddress1_ZIP = Me.p91ClientAddress1_ZIP.Text
                .p91ClientAddress1_Country = Me.p91ClientAddress1_Country.Text
                .p91ClientAddress2 = Me.p91ClientAddress2.Text

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()
            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If

            If .Update(cRec, lisX69, lisFF) Then
                Master.DataPID = .LastSavedPID
                Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p91Invoice, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                Master.CloseAndRefreshParent("p91-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p28ID_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p28ID.AutoPostBack_SelectedIndexChanged
        InhaleAddresses()
    End Sub

    Private Sub p92ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p92ID.NeedMissingItem
        Dim cRec As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p92Name
    End Sub

    Private Sub p92ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p92ID.SelectedIndexChanged
        Handle_FF()
    End Sub

    Private Sub cmdLoadClient_Click(sender As Object, e As EventArgs) Handles cmdLoadClient.Click
        Dim intP28ID As Integer = BO.BAS.IsNullInt(Me.p28ID.Value), intO38ID As Integer = BO.BAS.IsNullInt(Me.o38ID_Primary.SelectedValue)
        If intP28ID = 0 Or intO38ID = 0 Then
            Master.Notify("Musíte vybrat klienta a jeho primární adresu.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(intP28ID), cA As BO.o38Address = Master.Factory.o38AddressBL.Load(intO38ID)
        With cRec
            Me.p91Client.Text = IIf(.p28CompanyName = "", .p28Name, .p28CompanyName)
            Me.p91Client_VatID.Text = .p28VatID
            Me.p91Client_RegID.Text = .p28RegID
        End With
        With cA
            Me.p91ClientAddress1_Street.Text = .o38Street
            Me.p91ClientAddress1_City.Text = .o38City
            Me.p91ClientAddress1_ZIP.Text = .o38ZIP
            Me.p91ClientAddress1_Country.Text = .o38Country
        End With
        If Me.o38ID_Delivery.SelectedValue <> "" Then
            cA = Master.Factory.o38AddressBL.Load(BO.BAS.IsNullInt(Me.o38ID_Delivery.SelectedValue))
            Me.p91ClientAddress2.Text = cA.FullAddressWithBreaks
        Else
            Me.p91ClientAddress2.Text = ""
        End If
        If Me.j02ID_ContactPerson.SelectedValue <> "" Then
            Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(CInt(Me.j02ID_ContactPerson.SelectedValue))
            Me.p91ClientPerson.Text = cJ02.FullNameAsc
            Me.p91ClientPerson_Salutation.Text = cJ02.j02Salutation
        End If
    End Sub

    Private Sub j17ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j17ID.NeedMissingItem
        Dim cRec As BO.j17Country = Master.Factory.j17CountryBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.j17Name
    End Sub

    Private Sub p63ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p63ID.NeedMissingItem
        Dim cRec As BO.p63Overhead = Master.Factory.p63OverheadBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.NameWithRate
    End Sub

    Private Sub p80ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p80ID.NeedMissingItem
        Dim cRec As BO.p80InvoiceAmountStructure = Master.Factory.p80InvoiceAmountStructureBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.p80Name
    End Sub
    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "mydelete" Then
            Server.Transfer("p91_delete.aspx?pid=" & Master.DataPID.ToString)
        End If
    End Sub
End Class