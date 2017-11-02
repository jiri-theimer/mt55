Public Class query_builder
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _lastField As String

    Private Class myItem
        Public Property x29ID As BO.x29IdEnum
        Public Property Field As String
        Public Property Text As String

        Public Property x24ID As BO.x24IdENUM = BO.x24IdENUM.tNone


        Public Sub New(x29ID As BO.x29IdEnum, strField As String, strText As String, Optional x24ID As BO.x24IdENUM = BO.x24IdENUM.tNone)
            Me.Text = strText
            Me.x29ID = x29ID
            Me.Field = strField
            Me.x24ID = x24ID
        End Sub
    End Class
    
    
    Public Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
        Set(value As String)
            Me.hidPrefix.Value = value
        End Set
    End Property
    Public ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(Me.CurrentPrefix)
        End Get
    End Property
    
    Public ReadOnly Property CurrentIsOwner As Boolean
        Get
            Return BO.BAS.BG(hidIsOwner.Value)
        End Get
    End Property
    Private Sub query_builder_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            ViewState("x36key") = Request.Item("x36key")
            Me.CurrentPrefix = Request.Item("prefix")
            hidModeFlag.Value = Request.Item("modeflag")    '1-filtr + sloupce, 2-pouze filtr, 3-pouze sloupce
            If hidModeFlag.Value = "" Then hidModeFlag.Value = "1"
            hidMasterprefixFlag.Value = Request.Item("masterprefixflag")
            ViewState("masterprefix") = Request.Item("masterprefix")
            If ViewState("x36key") = "" Then ViewState("x36key") = Me.CurrentPrefix & "-j70id"
            With Master
                .neededPermission = BO.x53PermValEnum.GR_GridTools
                .Factory.j03UserBL.InhaleUserParams(CStr(ViewState("x36key")))

                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Request.Item("j70id") <> "" Then
                    .DataPID = BO.BAS.IsNullInt(Request.Item("j70id"))
                End If
                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam(CStr(ViewState("x36key"))))
                End If

                .AddToolbarButton("Uložit a spustit", "run", , "Images/ok.png")

            End With

            SetupQuery()
            SetupJ70Combo()

            SetupCols()
            RefreshRecord()

            Select Case Request.Item("tab")
                Case "query"
                    RadTabStrip1.SelectedIndex = 0
                Case "columns"
                    RadTabStrip1.SelectedIndex = 1
                Case Else
                    RadTabStrip1.SelectedIndex = 1
            End Select
            Select Case hidModeFlag.Value
                Case "2"
                    RadTabStrip1.Tabs(1).Visible = False
                    RadMultiPage1.PageViews(1).Visible = False
                    RadTabStrip1.SelectedIndex = 0
                    RadMultiPage1.SelectedIndex = 0
                    lblName.Text = "Název:"
                    lblJ70ID.Text = "Pojmenovaný filtr:"
                Case "3"
                    RadTabStrip1.Tabs(0).Visible = False
                    RadMultiPage1.PageViews(0).Visible = False
                    RadTabStrip1.SelectedIndex = 1
                    RadMultiPage1.SelectedIndex = 1

            End Select
           
        End If
    End Sub

    Private Sub SetupQuery()
        Dim lis As New List(Of myItem)
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                ph1.Text = "Přehled projektů"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.p42ProjectType, "p42id", "Typ projektu"))
                lis.Add(New myItem(BO.x29IdEnum.j18Region, "j18id", "Středisko projektu"))
                lis.Add(New myItem(BO.x29IdEnum.p92InvoiceType, "p92id", "Výchozí typ faktury projektu"))
                lis.Add(New myItem(BO.x29IdEnum.p51PriceList, "p51id_billing", "Fakturační ceník projektu"))
                lis.Add(New myItem(BO.x29IdEnum.p87BillingLanguage, "p87id", "Fakturační jazyk projektu"))
                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id_owner", "Vlastník záznamu projektu"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id", "Obsazení projektové role"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id-j11id", "Obsazení role přes tým"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p28id_client", "Klient projektu"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p28id_billing", "Odběratel faktury"))
                lis.Add(New myItem(BO.x29IdEnum.p29ContactType, "p28client.p29id", "Typ klienta projektu"))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41Name", "Název projektu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41NameShort", "Zkrácený název projektu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41Code", "Kód projektu", BO.x24IdENUM.tString))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41BillingMemo", "Fakturační poznámka projektu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41InvoiceDefaultText1", "Výchozí text faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41InvoiceMaturityDays", "Výchozí počet dní splatnosti", BO.x24IdENUM.tDecimal))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41PlanFrom", "Plánované datum zahájení", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41PlanUntil", "Plánované datum dokončení", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41LimitHours_Notification", "Limitní objem rozpracovaných hodin", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41LimitFee_Notification", "Limitní honorář z rozpracovaných hodin", BO.x24IdENUM.tDecimal))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41DateInsert", "Datum založení projektu", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41IsDraft", "Jedná se o DRAFT záznam?", BO.x24IdENUM.tBoolean))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p41ExternalPID", "Externí kód projektu", BO.x24IdENUM.tString))
            Case BO.x29IdEnum.p28Contact
                ph1.Text = "Přehled klientů"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.p29ContactType, "p29id", "Typ klienta"))
                lis.Add(New myItem(BO.x29IdEnum.p92InvoiceType, "p92id", "Výchozí typ faktury klienta"))
                lis.Add(New myItem(BO.x29IdEnum.p51PriceList, "p51id_billing", "Fakturační ceník klienta"))
                lis.Add(New myItem(BO.x29IdEnum.p87BillingLanguage, "p87id", "Fakturační jazyk klienta"))
                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id_owner", "Vlastník záznamu klienta"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id", "Obsazení klientské role"))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28Name", "Název klienta", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28NameShort", "Zkrácený název klienta", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28Code", "Kód klienta", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28IsCompany", "Právnická osoba?", BO.x24IdENUM.tBoolean))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28RegID", "IČ klienta", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28VatID", "DIČ klienta", BO.x24IdENUM.tString))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28BillingMemo", "Fakturační poznámka klienta", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28InvoiceDefaultText1", "Výchozí text faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28InvoiceMaturityDays", "Výchozí počet dní splatnosti", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28LimitHours_Notification", "Limitní objem rozpracovaných hodin", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28LimitFee_Notification", "Limitní honorář z rozpracovaných hodin", BO.x24IdENUM.tDecimal))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28IsDraft", "Jedná se o DRAFT záznam?", BO.x24IdENUM.tBoolean))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28DateInsert", "Datum založení klienta", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28ExternalPID", "Externí kód klienta", BO.x24IdENUM.tString))
            Case BO.x29IdEnum.p56Task
                ph1.Text = "Přehled úkolů"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.p57TaskType, "p57id", "Typ úkolu"))                
                lis.Add(New myItem(BO.x29IdEnum.b02WorkflowStatus, "b02id", "Aktuální workflow stav"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id", "Obsazení role v úkolu"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id-j11id", "Obsazení role přes tým"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Klient projektu"))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56Name", "Název úkolu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56Code", "Kód úkolu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p28ExternalPID", "Externí kód úkolu", BO.x24IdENUM.tString))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56PlanUntil", "Termín (plánované datum dokončení)", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56PlanFrom", "Plánované datum zahájení", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56Description", "Podrobný popis úkolu", BO.x24IdENUM.tString))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56Plan_Hours", "Plán/limit hodin", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56Plan_Expenses", "Plán/limit výdajů", BO.x24IdENUM.tDecimal))

                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id_owner", "Vlastník/autor úkolu"))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56DateInsert", "Datum založení úkolu", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p56ExternalPID", "Externí kód úkolu", BO.x24IdENUM.tString))
            Case BO.x29IdEnum.o23Doc
                ph1.Text = "Přehled dokumentů"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.x18EntityCategory, "x18ID", "Typ dokumentu"))
                lis.Add(New myItem(BO.x29IdEnum.b02WorkflowStatus, "b02id", "Aktuální workflow stav"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id", "Role příjemce (čtenáře) dokumentu"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id-j11id", "Obsazení role přes tým"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "a.p28ID", "Klient přiřazený k dokumentu"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Klient projektu svázaného s dokumentem"))

                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id_owner", "Vlastník/autor záznamu"))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.o23DateInsert", "Datum založení dokumentu", BO.x24IdENUM.tDateTime))
            Case BO.x29IdEnum.p91Invoice
                ph1.Text = "Přehled faktur"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.p92InvoiceType, "p92id", "Typ faktury"))
                lis.Add(New myItem(BO.x29IdEnum.j27Currency, "j27id", "Měna faktury"))

                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "a.p28ID", "Klient faktury (vazba)"))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Client", "Název klienta faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Klient vyfakturovaného projektu"))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Client_RegID", "IČ klienta faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Client_VatID", "DIČ klienta faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.p29ContactType, "p28client.p29id", "Typ klienta faktury"))
                lis.Add(New myItem(BO.x29IdEnum.j18Region, "p41.j18id", "Středisko fakturovaného projektu"))


                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id_owner", "Vlastník záznamu faktury"))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91DateInsert", "Datum založení faktury", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91DateSupply", "Datum plnění faktury", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91DateMaturity", "Datum splatnosti faktury", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91DateBilled", "Datum poslední úhrady", BO.x24IdENUM.tDateTime))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91IsDraft", "Jedná se o DRAFT záznam?", BO.x24IdENUM.tBoolean))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Code", "Číslo faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Amount_TotalDue", "Částka celkem", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Amount_Vat", "DPH celkem", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91ProformaAmount", "Částka záloh", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91RoundFitAmount", "Částka haléřového zaokrouhlení", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Text1", "Text faktury", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p91Text2", "Technický text faktury", BO.x24IdENUM.tString))
               

            Case BO.x29IdEnum.j02Person
                ph1.Text = "Přehled lidí"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.j07PersonPosition, "j07id", "Pozice"))
                lis.Add(New myItem(BO.x29IdEnum.c21FondCalendar, "c21id", "Pracovní fond"))
                lis.Add(New myItem(BO.x29IdEnum.j11Team, "j11id", "Tým osob"))
                lis.Add(New myItem(BO.x29IdEnum.j18Region, "j18id", "Středisko osoby"))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02IsIntraPerson", "Je osobou s uživatelským účtem?", BO.x24IdENUM.tBoolean))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02Email", "E-mail adresa", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02Code", "Kód/osobní číslo", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02Office", "Kancelář", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02Salutation", "Oslovení pro korespondenci", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.j02DateInsert", "Datum založení osoby", BO.x24IdENUM.tDateTime))
            Case BO.x29IdEnum.p31Worksheet
                ph1.Text = "WORKSHEET přehled"

                lis.Add(New myItem(BO.x29IdEnum._NotSpecified, "_other", "Různé"))
                lis.Add(New myItem(BO.x29IdEnum.p34ActivityGroup, "p32.p34id", "Sešit"))
                lis.Add(New myItem(BO.x29IdEnum.p32Activity, "p32id", "Aktivita"))
                lis.Add(New myItem(BO.x29IdEnum.j02Person, "j02id", "Osoba"))
                lis.Add(New myItem(BO.x29IdEnum.j02Person, "a.j02ID_ContactPerson", "Kontaktní osoba"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Klient projektu"))
                lis.Add(New myItem(BO.x29IdEnum.System, "p41.p41Name", "Název projektu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.p42ProjectType, "p41.p42ID", "Typ projektu"))
                lis.Add(New myItem(BO.x29IdEnum.j18Region, "p41.j18id", "Středisko projektu"))
                lis.Add(New myItem(BO.x29IdEnum.p28Contact, "a.p28ID_Supplier", "Dodavatel"))
                lis.Add(New myItem(BO.x29IdEnum.p71ApproveStatus, "p71id", "Schváleno"))
                lis.Add(New myItem(BO.x29IdEnum.p72PreBillingStatus, "p72ID_AfterApprove", "Návrh fakturačního statusu schvalovatelem"))
                lis.Add(New myItem(BO.x29IdEnum.p70BillingStatus, "p70id", "Fakturační status"))
                lis.Add(New myItem(BO.x29IdEnum.j27Currency, "j27ID_Billing_Orig", "Měna úkonu"))
                lis.Add(New myItem(BO.x29IdEnum.p95InvoiceRow, "p32.p95id", "Fakturační oddíl"))
                lis.Add(New myItem(BO.x29IdEnum.x67EntityRole, "x67id", "Obsazení projektové role"))
                lis.Add(New myItem(BO.x29IdEnum.j19PaymentType, "j19id", "Typ úhrady"))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31Hours_Orig", "Vykázané hodiny", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31Rate_Billing_Orig", "Výchozí fakturační sazba", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31Rate_Internal_Orig", "Nákladová hodinová sazba", BO.x24IdENUM.tDecimal))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31Text", "Podrobný popis úkonu", BO.x24IdENUM.tString))
                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31ApprovingSet", "Dávka v rámci schvalování", BO.x24IdENUM.tString))

                lis.Add(New myItem(BO.x29IdEnum.System, "a.p31DateInsert", "Datum založení úkonu", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "p91.p91DateSupply", "Zdanitelné plnění faktury", BO.x24IdENUM.tDateTime))
                lis.Add(New myItem(BO.x29IdEnum.System, "p91.p91Date", "Datum faktury", BO.x24IdENUM.tDateTime))
        End Select

        lis.Add(New myItem(BO.x29IdEnum.o23Doc, "o23id", "Kategorie"))

        Dim lisFF As IEnumerable(Of BO.x28EntityField) = Master.Factory.x28EntityFieldBL.GetList(Me.CurrentX29ID, -1, True).Where(Function(p) p.x28Flag = BO.x28FlagENUM.UserField Or p.x28Query_Field <> "")
        For Each cField In lisFF
            If cField.x28Query_Field <> "" Then
                lis.Add(New myItem(BO.x29IdEnum.System, cField.x28Query_Field, cField.x28Name))
            Else
                lis.Add(New myItem(BO.x29IdEnum.System, cField.x28Field, cField.x28Name))
            End If

        Next
        For Each c In lis
            Me.cbxQueryField.Items.Add(New ListItem(c.Text, CInt(c.x29ID).ToString & "-" & c.Field & "--" & CInt(c.x24ID).ToString))

        Next
        cbxQueryField.Items.Insert(0, "--Vyberte filtrovací pole--")
    End Sub

    Private Sub Handle_ChangeField()
        cbxItemsExtension.Visible = False

        ''If Me.cbxQueryField.SelectedItem Is Nothing Then Return
        If Me.cbxQueryField.SelectedIndex = 0 Then Return

        Dim a() As String = Split(Me.cbxQueryField.SelectedValue, "-")
        Dim x29id As BO.x29IdEnum = CType(CInt(a(0)), BO.x29IdEnum)
        Dim strField As String = a(1), mq As New BO.myQuery, strFieldAfter As String = ""
        If UBound(a) > 1 Then strFieldAfter = a(2)

        mq.Closed = BO.BooleanQueryMode.NoQuery
        Me.panQueryItems.Visible = True : Me.panQueryNonItems.Visible = False : Me.panQueryPeriod.Visible = False : Me.panQueryString.Visible = False

        Select Case x29id
            Case BO.x29IdEnum.System
                Dim cField As BO.x28EntityField = Master.Factory.x28EntityFieldBL.LoadByField(strField)
                If cField Is Nothing Then cField = Master.Factory.x28EntityFieldBL.LoadByQueryField(strField)
                If cField Is Nothing Then
                    cField = New BO.x28EntityField
                    cField.x28Name = cbxQueryField.SelectedItem.Text
                    cField.x24ID = CType(CInt(a(3)), BO.x24IdENUM)
                End If
                'uživatelské - volné pole
                If cField.x23ID <> 0 Or cField.x24ID = BO.x24IdENUM.tBoolean Then
                    'číselníkové (combo) pole
                    If cField.x23ID <> 0 Then
                        Me.cbxItems.DataTextField = "o23Name"
                        Me.cbxItems.DataSource = Master.Factory.o23DocBL.GetList(New BO.myQueryO23(cField.x23ID))
                    Else
                        Me.cbxItems.Clear()
                        Me.cbxItems.AddOneComboItem("1", "ANO")
                        Me.cbxItems.AddOneComboItem("0", "NE")
                    End If

                Else
                    'nečíselníkové pole
                    Me.panQueryItems.Visible = False
                    Select Case cField.x24ID
                        Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime, BO.x24IdENUM.tTime
                            panQueryPeriod.Visible = True
                            Dim bolEnglish As Boolean = False
                            ''If Page.Culture.IndexOf("Czech") < 0 Then bolEnglish = True

                            period1.FillData(Master.Factory.ftBL.GetList_X21_NonDB(False, bolEnglish), "", "--Vlastní období--")
                        Case BO.x24IdENUM.tString
                            panQueryString.Visible = True
                        Case Else
                            Me.panQueryNonItems.Visible = True
                    End Select


                End If
                
            Case BO.x29IdEnum._NotSpecified
                Dim lis As List(Of BO.OtherQueryItem) = Master.Factory.j70QueryTemplateBL.GetList_OtherQueryItem(Me.CurrentX29ID)

                Me.cbxItems.DataTextField = "Text"
                Me.cbxItems.DataSource = lis


            Case BO.x29IdEnum.p92InvoiceType
                Me.cbxItems.DataTextField = "p92Name"
                Me.cbxItems.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(mq)
            Case BO.x29IdEnum.p42ProjectType
                Me.cbxItems.DataTextField = "p42Name"
                Me.cbxItems.DataSource = Master.Factory.p42ProjectTypeBL.GetList(mq)
            Case BO.x29IdEnum.p29ContactType
                Me.cbxItems.DataTextField = "p29Name"
                Me.cbxItems.DataSource = Master.Factory.p29ContactTypeBL.GetList(mq)
            Case BO.x29IdEnum.p57TaskType
                Me.cbxItems.DataTextField = "p57Name"
                Me.cbxItems.DataSource = Master.Factory.p57TaskTypeBL.GetList(mq)
            
            Case BO.x29IdEnum.x18EntityCategory
                Me.cbxItems.DataTextField = "x18Name"
                Me.cbxItems.DataSource = Master.Factory.x18EntityCategoryBL.GetList(mq)
            Case BO.x29IdEnum.b02WorkflowStatus
                Me.cbxItems.DataTextField = "NameWithb01Name"
                Me.cbxItems.DataSource = Master.Factory.b02WorkflowStatusBL.GetList(0).Where(Function(p) p.x29ID = Me.CurrentX29ID)
            Case BO.x29IdEnum.j18Region
                Me.cbxItems.DataTextField = "j18Name"
                Me.cbxItems.DataSource = Master.Factory.j18RegionBL.GetList(mq)
            Case BO.x29IdEnum.j02Person
                Me.cbxItems.DataTextField = "FullNameDesc"
                Dim mqJ02 As New BO.myQueryJ02
                mqJ02.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
                mqJ02.Closed = BO.BooleanQueryMode.NoQuery
                Me.cbxItems.DataSource = Master.Factory.j02PersonBL.GetList(mqJ02)
            Case BO.x29IdEnum.p87BillingLanguage
                Me.cbxItems.DataTextField = "p87Name"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_P87()
            Case BO.x29IdEnum.j27Currency
                Me.cbxItems.DataTextField = "j27Code"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_J27()
            Case BO.x29IdEnum.j07PersonPosition
                Me.cbxItems.DataTextField = "j07Name"
                Me.cbxItems.DataSource = Master.Factory.j07PersonPositionBL.GetList(mq)
            Case BO.x29IdEnum.j11Team
                Me.cbxItems.DataTextField = "j11Name"
                Me.cbxItems.DataSource = Master.Factory.j11TeamBL.GetList(mq).Where(Function(p) p.j11IsAllPersons = False)
            Case BO.x29IdEnum.c21FondCalendar
                Me.cbxItems.DataTextField = "c21Name"
                Me.cbxItems.DataSource = Master.Factory.c21FondCalendarBL.GetList(mq)
            Case BO.x29IdEnum.p34ActivityGroup
                Me.cbxItems.DataTextField = "p34Name"
                Me.cbxItems.DataSource = Master.Factory.p34ActivityGroupBL.GetList(mq)
            Case BO.x29IdEnum.p32Activity
                Dim mqP32 As New BO.myQueryP32
                mqP32.Closed = BO.BooleanQueryMode.NoQuery
                Me.cbxItems.DataTextField = "NameWithSheet"
                Me.cbxItems.DataSource = Master.Factory.p32ActivityBL.GetList(mqP32).OrderBy(Function(p) p.p34ID).ThenBy(Function(p) p.p32Ordinary).ThenBy(Function(p) p.p32Name)
            Case BO.x29IdEnum.p95InvoiceRow
                Me.cbxItems.DataTextField = "p95Name"
                Me.cbxItems.DataSource = Master.Factory.p95InvoiceRowBL.GetList(mq)
            Case BO.x29IdEnum.j19PaymentType
                Me.cbxItems.DataTextField = "j19Name"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_j19(mq)
            Case BO.x29IdEnum.p71ApproveStatus
                Me.cbxItems.DataTextField = "p71Name"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_P71()
            Case BO.x29IdEnum.p72PreBillingStatus
                Me.cbxItems.DataTextField = "p72Name"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_P72()
            Case BO.x29IdEnum.p70BillingStatus
                Me.cbxItems.DataTextField = "p70Name"
                Me.cbxItems.DataSource = Master.Factory.ftBL.GetList_P70()
            Case BO.x29IdEnum.p51PriceList
                Me.cbxItems.DataTextField = "p51Name"
                Me.cbxItems.DataSource = Master.Factory.p51PriceListBL.GetList(mq).Where(Function(p) p.p51IsMasterPriceList = False And p.p51IsInternalPriceList = False And p.p51IsCustomTailor = False)
            Case BO.x29IdEnum.x67EntityRole
                Me.cbxItems.DataTextField = "x67Name"
                Dim x29Role As BO.x29IdEnum = Me.CurrentX29ID
                If Me.CurrentX29ID = BO.x29IdEnum.p31Worksheet Then x29Role = BO.x29IdEnum.p41Project
                Me.cbxItems.DataSource = Master.Factory.x67EntityRoleBL.GetList(mq).Where(Function(p) p.x29ID = x29Role)
                Me.cbxItemsExtension.Visible = True
                If strFieldAfter = "j11id" Then
                    Me.cbxItemsExtension.DataTextField = "j11Name"
                    Me.cbxItemsExtension.DataSource = Master.Factory.j11TeamBL.GetList()
                    Me.cbxItemsExtension.DataBind()
                Else
                    Me.cbxItemsExtension.DataTextField = "FullNameDesc"
                    Dim mqJ02 As New BO.myQueryJ02
                    mqJ02.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
                    mqJ02.Closed = BO.BooleanQueryMode.NoQuery
                    Me.cbxItemsExtension.DataSource = Master.Factory.j02PersonBL.GetList(mqJ02)
                    Me.cbxItemsExtension.DataBind()
                    Me.cbxItemsExtension.AddOneComboItem("-1", "Aktuálně přihlášený uživatel", 1)
                End If

            Case BO.x29IdEnum.p28Contact
                Me.cbxItems.DataTextField = "p28Name"
                Dim mqP28 As New BO.myQueryP28
                mqP28.Closed = BO.BooleanQueryMode.NoQuery
                Select Case strField
                    Case "p28id_client"
                        mqP28.QuickQuery = BO.myQueryP28_QuickQuery.ProjectClient
                    Case "p28id_billing"
                        mqP28.QuickQuery = BO.myQueryP28_QuickQuery.ProjectInvoiceReceiver
                End Select
                Me.cbxItems.DataSource = Master.Factory.p28ContactBL.GetList(mqP28)
            Case BO.x29IdEnum.o23Doc 'štítky
                Me.cbxItems.DataTextField = "NameWithComboName"
                Me.cbxItems.DataSource = Master.Factory.x18EntityCategoryBL.GetList_O23(Me.CurrentX29ID)
            Case Else
                Me.cbxItems.DataSource = Nothing
        End Select

        If panQueryItems.Visible Then
            If cbxItemsExtension.Visible Then
                cbxItems.AllowCheckboxes = False
            Else
                cbxItems.AllowCheckboxes = True
            End If
            Me.cbxItems.DataBind()
        End If

    End Sub

  
    Private Sub cbxQueryField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxQueryField.SelectedIndexChanged
        Handle_ChangeField()
    End Sub

    Private Sub cbxItems_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles cbxItems.ItemDataBound
        Try
            If e.Item.DataItem.IsClosed Then
                e.Item.Font.Strikeout = True
            End If
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub SaveTempQueryItem()
        Dim a() As String = Split(Me.cbxQueryField.SelectedValue, "-")
        Dim x29id As BO.x29IdEnum = CType(CInt(a(0)), BO.x29IdEnum)
        Dim strField As String = a(1), mq As New BO.myQuery, bolCiselnik As Boolean = True
        Dim intj71RecordPID As Integer, strj71ValueFrom As String = "", strj71ValueUntil As String = ""
        Dim strJ71ValueType As String = "combo", strj71RecordName As String = ""
        Dim strJ71StringOperator As String = "", strJ71ValueString As String = "", intX28ID As Integer = 0

        If x29id = BO.x29IdEnum.System Then
            'uživatelské - volné pole
            Dim cField As BO.x28EntityField = Master.Factory.x28EntityFieldBL.LoadByField(strField)
            If cField Is Nothing Then cField = Master.Factory.x28EntityFieldBL.LoadByQueryField(strField)
            If cField Is Nothing Then
                cField = New BO.x28EntityField
                cField.x28Name = cbxQueryField.SelectedItem.Text
                cField.x24ID = CType(CInt(a(3)), BO.x24IdENUM)
                intX28ID = 0
            Else
                intX28ID = cField.PID
            End If
            Select Case cField.x24ID
                Case BO.x24IdENUM.tBoolean
                    strJ71ValueType = "boolean"
                Case BO.x24IdENUM.tDecimal, BO.x24IdENUM.tInteger
                    strJ71ValueType = "number"
                    If Me.panQueryNonItems.Visible Then
                        If Not IsNumeric(Trim(Me.j71ValueFrom.Text)) Then
                            Master.Notify("[Hodnota od] musí být číslo.", NotifyLevel.WarningMessage) : Return
                        End If
                        If Not IsNumeric(Trim(Me.j71ValueUntil.Text)) Then
                            Master.Notify("[Hodnota do] musí být číslo.", NotifyLevel.WarningMessage) : Return
                        End If
                    End If

                Case BO.x24IdENUM.tString
                    strJ71ValueType = "string"
                    strJ71StringOperator = Me.cbxStringOperator.SelectedValue
                    strJ71ValueString = Trim(Me.txtStringValue.Text)
                    If strJ71ValueString = "" And Me.cbxStringOperator.SelectedValue <> "EMPTY" And Me.cbxStringOperator.SelectedValue <> "NOTEMPTY" Then
                        Master.Notify("Musíte zadat hodnotu.", NotifyLevel.WarningMessage)
                        Return
                    End If
            End Select
        End If

        If Me.panQueryNonItems.Visible Then
            strj71ValueFrom = Trim(Me.j71ValueFrom.Text)
            strj71ValueUntil = Trim(Me.j71ValueUntil.Text)
            strj71RecordName = strj71ValueFrom & " - " & strj71ValueUntil
        End If
        If Me.panQueryString.Visible Then
            strJ71ValueType = "string"
            strj71RecordName = Me.cbxStringOperator.SelectedItem.Text & ": " & strJ71ValueString
        End If
        If Me.panQueryPeriod.Visible Then
            'filtrování podle datumu
            strj71ValueFrom = Format(period1.DateFrom, "dd.MM.yyyy")
            strj71ValueUntil = Format(period1.DateUntil, "dd.MM.yyyy")
            If Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85FreeText04 = strField).Count > 0 Then
                Master.Notify("Toto datumové pole již bylo do filtru zařazeno.", NotifyLevel.WarningMessage)
                Return
            End If
            strJ71ValueType = "date"
            strj71RecordName = strj71ValueFrom & " - " & strj71ValueUntil
        End If
        If Me.panQueryItems.Visible Then
            'číselník
            Dim items As List(Of String) = Nothing
            If Me.cbxItems.AllowCheckboxes Then
                items = Me.cbxItems.GetAllCheckedValues()
                If items.Count = 0 Then
                    Master.Notify("Musíte zaškrtnout alespoň jednu položku.", NotifyLevel.WarningMessage)
                    Return
                End If
            Else
                If Me.cbxItems.SelectedValue = "" Then
                    Master.Notify("Musíte vybrat hodnotu.", NotifyLevel.WarningMessage)
                    Return
                End If
                items = BO.BAS.ConvertDelimitedString2List(Me.cbxItems.SelectedValue, ",")
            End If
            If Me.cbxItemsExtension.Visible Then
                If Me.cbxItemsExtension.GetAllCheckedValues.Count = 0 Then
                    Master.Notify("Musíte zaškrtnout alespoň jednu položku.", NotifyLevel.WarningMessage)
                    Return
                End If
            End If

            For Each strItem In items
                intj71RecordPID = BO.BAS.IsNullInt(strItem)
                strj71RecordName = cbxItems.RadCombo.Items.FindItemByValue(strItem).Text

                If Me.cbxItemsExtension.Visible Then
                    For Each strExtItem In Me.cbxItemsExtension.GetAllCheckedValues
                        Dim intExtensionValue As Integer = BO.BAS.IsNullInt(strExtItem), strExtensionAlias As String = ""
                        If intExtensionValue <> 0 Then
                            strExtensionAlias = cbxItemsExtension.RadCombo.Items.FindItemByValue(strExtItem).Text
                        End If
                        If Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85OtherKey1 = intj71RecordPID And p.p85FreeText04 = strField And p.p85OtherKey3 = intExtensionValue).Count > 0 Then
                            ''Master.Notify("Tato hodnota již byla do filtru zařazena.", NotifyLevel.WarningMessage)
                        Else
                            SaveTempValue(intj71RecordPID, strj71RecordName, x29id, strField, cbxQueryField.SelectedItem.Text, intExtensionValue, strExtensionAlias, "", "", strJ71ValueType, "", "", intX28ID)
                        End If
                    Next

                Else
                    If Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85OtherKey1 = intj71RecordPID And p.p85FreeText04 = strField And p.p85OtherKey3 = 0).Count > 0 Then
                        ''Master.Notify("Tato hodnota již byla do filtru zařazena.", NotifyLevel.WarningMessage)
                    Else
                        SaveTempValue(intj71RecordPID, strj71RecordName, x29id, strField, cbxQueryField.SelectedItem.Text, 0, "", "", "", strJ71ValueType, "", "", intX28ID)
                    End If
                End If
               
            Next
            
        Else
            SaveTempValue(intj71RecordPID, strj71RecordName, x29id, strField, cbxQueryField.SelectedItem.Text, 0, "", strj71ValueFrom, strj71ValueUntil, strJ71ValueType, strJ71StringOperator, strJ71ValueString, intX28ID)
        End If
       
        
        RefreshJ71TempList()
    End Sub

    Private Sub SaveTempValue(intj71RecordPID As Integer, strj71RecordName As String, x29id As BO.x29IdEnum, strQueryField As String, strQueryField_Alias As String, intExtensionValue As Integer, strExtensionAlias As String, strj71ValueFrom As String, strj71ValueUntil As String, strJ71ValueType As String, strJ71StringOperator As String, strJ71ValueString As String, intX28ID As Integer)
        Dim cRec As New BO.p85TempBox
        With cRec
            .p85GUID = ViewState("guid")
            .p85OtherKey1 = intj71RecordPID
            .p85FreeText01 = strj71RecordName

            .p85OtherKey2 = CInt(x29id)
            .p85FreeText03 = strQueryField_Alias
            .p85FreeText04 = strQueryField

            If intExtensionValue <> 0 Then
                .p85OtherKey3 = intExtensionValue
                .p85FreeText02 = strExtensionAlias
            End If

            .p85FreeText05 = strj71ValueFrom
            .p85FreeText06 = strj71ValueUntil
            .p85FreeText07 = strJ71ValueType
            .p85FreeText08 = strJ71StringOperator
            .p85FreeText09 = strJ71ValueString
            .p85Message = strQueryField_Alias
            .p85OtherKey4 = intX28ID
        End With
        Master.Factory.p85TempBoxBL.Save(cRec)
    End Sub
    Private Sub RefreshJ71TempList()
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).OrderBy(Function(p) p.p85FreeText04).ThenBy(Function(p) p.p85OtherKey2).ToList

        rpJ71.DataSource = lisTMP
        rpJ71.DataBind()

    End Sub
    Private Sub rpJ71_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpJ71.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString

        If _lastField <> cRec.p85FreeText04 Then
            If cRec.p85FreeText03 = "System" Then cRec.p85FreeText03 = cRec.p85Message
            CType(e.Item.FindControl("x29Name"), Label).Text = cRec.p85FreeText03 & ":"
            e.Item.FindControl("nebo").Visible = False
        Else
            e.Item.FindControl("nebo").Visible = True
            
        End If
        CType(e.Item.FindControl("x29id"), HiddenField).Value = cRec.p85OtherKey2.ToString
        CType(e.Item.FindControl("j71Field"), HiddenField).Value = cRec.p85FreeText04
        If Not cbxQueryField.Items.FindByValue(cRec.p85OtherKey2.ToString & "-" & cRec.p85FreeText04) Is Nothing Then
            CType(e.Item.FindControl("x29Name"), Label).Text = cbxQueryField.Items.FindByValue(cRec.p85OtherKey2.ToString & "-" & cRec.p85FreeText04).Text & ":"
        End If

        CType(e.Item.FindControl("j71RecordName"), Label).Text = cRec.p85FreeText01
        If cRec.p85FreeText05 <> "" Or cRec.p85FreeText06 <> "" Then
            CType(e.Item.FindControl("j71RecordName"), Label).Text = cRec.p85FreeText05 & " - " & cRec.p85FreeText06
        End If
        CType(e.Item.FindControl("j71RecordPID"), HiddenField).Value = cRec.p85OtherKey1.ToString

        CType(e.Item.FindControl("j71RecordName_Extension"), Label).Text = cRec.p85FreeText02
        CType(e.Item.FindControl("j71RecordPID_Extension"), HiddenField).Value = cRec.p85OtherKey3.ToString
        If Not Me.CurrentIsOwner Then
            e.Item.FindControl("del").Visible = False
        End If
        _lastField = cRec.p85FreeText04
    End Sub
    Private Sub rpJ71_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rpJ71.ItemCommand
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(CType(e.Item.FindControl("p85id"), HiddenField).Value))
        If Master.Factory.p85TempBoxBL.Delete(cRec) Then
            RefreshJ71TempList()

        End If
    End Sub

    Private Sub query_builder_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        RefreshState()
    End Sub
    Private Sub RefreshState()
      
        If rpJ71.Items.Count > 0 Then
            panJ71.Visible = True
        Else
            panJ71.Visible = False
        End If
        cmdClear.Visible = panJ71.Visible

        Dim bolIsSystem As Boolean = BO.BAS.BG(Me.j70IsSystem.Value)
        lblName.Visible = Not bolIsSystem
        Me.j70Name.Visible = Not bolIsSystem

        If cmdDelete.Visible Then
            cmdDelete.Visible = Not bolIsSystem
        End If

        If Not Me.CurrentIsOwner Then
            Master.RenameToolbarButton("run", "Spustit přehled")
            panRoles.Visible = False
            cmdClear.Visible = False
            cmdDelete.Visible = False
            lblName.Visible = False
            Me.j70Name.Visible = False
            Me.panQueryCondition.Visible = False
            Me.opgBin.Enabled = False
            cmdSave.Visible = False
        Else
            Master.RenameToolbarButton("run", "Uložit a spustit přehled")
            panRoles.Visible = Not bolIsSystem
            Me.panQueryCondition.Visible = True
            Me.opgBin.Enabled = True
            cmdSave.Visible = True
        End If

        Select Case Me.cbxStringOperator.SelectedValue
            Case "EMPTY", "NOTEMPTY"
                Me.txtStringValue.Visible = False
            Case Else
                Me.txtStringValue.Visible = True
        End Select

        If rpJ71.Items.Count > 0 Then
            Me.RadTabStrip1.Tabs(0).ForeColor = Drawing.Color.Red
        Else
            Me.RadTabStrip1.Tabs(0).ForeColor = Nothing
        End If
        If roles1.RowsCount > 0 Then
            Me.RadTabStrip1.Tabs(2).ForeColor = Drawing.Color.Red
        Else
            Me.RadTabStrip1.Tabs(2).ForeColor = Nothing
        End If
        Me.RadTabStrip1.Tabs(1).Text = String.Format("Nastavení sloupců ({0})", lt1.Items.Count)
    End Sub

    Private Sub SetupJ70Combo()
        Dim mq As BO.myQuery = Nothing
        Dim onlyQuery As BO.BooleanQueryMode = BO.BooleanQueryMode.NoQuery
        If hidModeFlag.Value = "2" Then onlyQuery = BO.BooleanQueryMode.TrueQuery
        Dim s As String = ViewState("masterprefix")
        If hidMasterprefixFlag.Value = "1" And ViewState("masterprefix") <> "" Then s = "-1"
        Dim lisJ70 As IEnumerable(Of BO.j70QueryTemplate) = Master.Factory.j70QueryTemplateBL.GetList(mq, Me.CurrentX29ID, s, onlyQuery)
        If ViewState("masterprefix") <> "" Then
            lisJ70 = lisJ70.Where(Function(p) p.j70MasterPrefix = ViewState("masterprefix") Or (p.j70MasterPrefix = "" And p.j70IsSystem = False))
        End If
        If lisJ70.Count = 0 Then
            'uživatel zatím nemá žádný filtr - založit první j70IsSystem=1
            Dim c As New BO.j70QueryTemplate
            c.j70IsSystem = True
            c.j70Name = "Můj výchozí přehled"
            c.j03ID = Master.Factory.SysUser.PID
            c.x29ID = Me.CurrentX29ID
            If Not Master.Factory.j70QueryTemplateBL.Save(c, New List(Of BO.j71QueryTemplate_Item), Nothing) Then
                Master.Notify(Master.Factory.j70QueryTemplateBL.ErrorMessage, NotifyLevel.ErrorMessage)

            End If
            
            lisJ70 = Master.Factory.j70QueryTemplateBL.GetList(mq, Me.CurrentX29ID)

        End If
        j70ID.DataSource = lisJ70
        j70ID.DataBind()
        basUI.SelectDropdownlistValue(Me.j70ID, Master.DataPID.ToString)
    End Sub


    Private Sub ClearQuery()
        
        With Master.Factory.p85TempBoxBL
            .Truncate(ViewState("guid"))
        End With
        RefreshJ71TempList()

    End Sub

    Private Sub cmdClear_Click(sender As Object, e As EventArgs) Handles cmdClear.Click
        ClearQuery()
    End Sub

    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        Master.DataPID = BO.BAS.IsNullInt(j70ID.SelectedValue)
        If Master.DataPID = 0 Then
            j70Name.Text = ""
            Me.j70IsSystem.Value = "0"
        End If
        roles1.RefreshGUID()
        RefreshRecord()

    End Sub

    Private Sub RefreshRecord()
        Master.DataPID = BO.BAS.IsNullInt(j70ID.SelectedValue)
        cmdDelete.Visible = False : cmdNew.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
        If Master.DataPID = 0 Then
            If j70ID.Items.Count = 0 Then
                j70Name.Text = "Můj výchozí přehled"
            End If
            j70Name.Enabled = True
            ClearQuery()
            Return
        End If
        Dim cRec As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(Master.DataPID)
        With cRec
            j70Name.Text = .j70Name
            Me.j70IsNegation.Checked = .j70IsNegation
            cmdDelete.Visible = True

            lblTimeStamp.Text = .Timestamp
            
            Me.j70IsSystem.Value = BO.BAS.GB(.j70IsSystem)
        End With
        roles1.InhaleInitialData(cRec.PID)

        If cRec.j03ID = Master.Factory.SysUser.PID Or Master.Factory.SysUser.IsAdmin Then
            hidIsOwner.Value = "1"
        Else
            hidIsOwner.Value = "0"
        End If

        basUI.SelectDropdownlistValue(Me.opgBin, cRec.j70BinFlag.ToString)
        basUI.SelectRadiolistValue(Me.j70PageLayoutFlag, CInt(cRec.j70PageLayoutFlag).ToString)
        Master.Factory.j70QueryTemplateBL.Setupj71TempList(Master.DataPID, ViewState("guid"))
        RefreshJ71TempList()

        RefreshRecord_Columns(cRec)
    End Sub

    

    Private Sub cmdDelete_Click(sender As Object, e As ImageClickEventArgs) Handles cmdDelete.Click
        If Master.DataPID = 0 Then Return

        If Master.Factory.j70QueryTemplateBL.Delete(Master.DataPID) Then
            SetupJ70Combo()
            RefreshRecord()
        Else
            Master.Notify(Master.Factory.j70QueryTemplateBL.ErrorMessage, 2)
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "run" Then
            If Not Me.CurrentIsOwner Then
                SubmitQuery()
                Return
            End If
            roles1.SaveCurrentTempData()
            If SaveChanges() Then
                SubmitQuery()
            End If
            ''Dim cRec As BO.j70QueryTemplate = IIf(Master.DataPID = 0, New BO.j70QueryTemplate, Master.Factory.j70QueryTemplateBL.Load(Master.DataPID))
            ''With cRec
            ''    .x29ID = Me.CurrentX29ID
            ''    .j70Name = j70Name.Text
            ''    .j70IsNegation = Me.j70IsNegation.Checked
            ''    .j70BinFlag = BO.BAS.IsNullInt(Me.opgBin.SelectedValue)

            ''    Dim s As String = ""
            ''    For Each it As Telerik.Web.UI.RadListBoxItem In lt1.Items
            ''        s += "," & it.Value
            ''    Next
            ''    .j70ColumnNames = BO.BAS.OM1(s)

            ''    .j70OrderBy = GetOrderBy()
            ''    .j70IsFilteringByColumn = Me.j70IsFilteringByColumn.Checked
            ''    .j70ScrollingFlag = BO.BAS.IsNullInt(Me.j70ScrollingFlag.SelectedValue)
            ''End With

            ''Dim lisJ71 As List(Of BO.j71QueryTemplate_Item) = GetList_j71()

            ''Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            ''If roles1.ErrorMessage <> "" Then
            ''    Master.Notify(roles1.ErrorMessage, 2)
            ''    Return
            ''End If


            ''If Master.Factory.j70QueryTemplateBL.Save(cRec, lisJ71, lisX69) Then
            ''    Master.DataPID = Master.Factory.j70QueryTemplateBL.LastSavedPID
            ''    SubmitQuery()
            ''Else
            ''    Master.Notify(Master.Factory.j70QueryTemplateBL.ErrorMessage, NotifyLevel.ErrorMessage)
            ''End If
        End If
    End Sub
    Private Function SaveChanges() As Boolean
        roles1.SaveCurrentTempData()
        If Trim(j70Name.Text) = "" Then
            Master.Notify("Chybí specifikace jména přehledu.", NotifyLevel.ErrorMessage)
            Return False
        End If
        If lt1.Items.Count = 0 Then
            Master.Notify("Přehled musí mít minimálně jeden sloupec.", NotifyLevel.ErrorMessage)
            Return False
        End If
        Dim cRec As BO.j70QueryTemplate = IIf(Master.DataPID = 0, New BO.j70QueryTemplate, Master.Factory.j70QueryTemplateBL.Load(Master.DataPID))
        With cRec
            .x29ID = Me.CurrentX29ID
            .j70Name = j70Name.Text
            .j70IsNegation = Me.j70IsNegation.Checked
            .j70BinFlag = BO.BAS.IsNullInt(Me.opgBin.SelectedValue)

            Dim s As String = ""
            For Each it As Telerik.Web.UI.RadListBoxItem In lt1.Items
                s += "," & it.Value
            Next
            .j70ColumnNames = BO.BAS.OM1(s)

            .j70OrderBy = GetOrderBy()
            .j70IsFilteringByColumn = Me.j70IsFilteringByColumn.Checked
            .j70ScrollingFlag = BO.BAS.IsNullInt(Me.j70ScrollingFlag.SelectedValue)
            .j70PageLayoutFlag = BO.BAS.IsNullInt(Me.j70PageLayoutFlag.SelectedValue)
        End With

        Dim lisJ71 As List(Of BO.j71QueryTemplate_Item) = GetList_j71()

        Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
        If roles1.ErrorMessage <> "" Then
            Master.Notify(roles1.ErrorMessage, 2)
            Return False
        End If


        If Master.Factory.j70QueryTemplateBL.Save(cRec, lisJ71, lisX69) Then
            Master.DataPID = Master.Factory.j70QueryTemplateBL.LastSavedPID
            Return True
        Else
            Master.Notify(Master.Factory.j70QueryTemplateBL.ErrorMessage, NotifyLevel.ErrorMessage)
            Return False
        End If
    End Function

    Private Function GetList_j71() As List(Of BO.j71QueryTemplate_Item)
        Dim lisTMP As List(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).ToList
        Dim lisJ71 As New List(Of BO.j71QueryTemplate_Item)
        For Each cTMP In lisTMP
            Dim c As New BO.j71QueryTemplate_Item
            c.j71RecordPID = cTMP.p85OtherKey1
            c.j71RecordName = cTMP.p85FreeText01

            c.x29ID = cTMP.p85OtherKey2
            c.j71Field = cTMP.p85FreeText04
            c.j71RecordPID_Extension = cTMP.p85OtherKey3
            c.j71RecordName_Extension = cTMP.p85FreeText02
            c.j71ValueFrom = cTMP.p85FreeText05
            c.j71ValueUntil = cTMP.p85FreeText06
            c.j71ValueType = cTMP.p85FreeText07
            c.j71StringOperator = cTMP.p85FreeText08
            c.j71ValueString = cTMP.p85FreeText09
            c.j71FieldLabel = cTMP.p85Message
            c.x28ID = cTMP.p85OtherKey4
            If c.x28ID <> 0 Then
                'x28ID
                Dim cX28 As BO.x28EntityField = Master.Factory.x28EntityFieldBL.Load(c.x28ID)
                c.j71SqlExpression = cX28.x28Query_SqlSyntax
            End If
            lisJ71.Add(c)
        Next
        Return lisJ71
    End Function

    Private Sub SubmitQuery()
        Master.Factory.j03UserBL.SetUserParam(CStr(ViewState("x36key")), Master.DataPID.ToString)
        If ViewState("x36key") = Me.CurrentPrefix & "-j70id" Then
            Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "_framework-sort", "")
        End If
        If ViewState("x36key") = Me.CurrentPrefix & "_subgrid-j70id" Then
            Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "_subgrid-sort", "")
        End If
       
        Master.CloseAndRefreshParent("j70-run")
    End Sub
    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    
    Private Sub cmdAdd2Query_Click(sender As Object, e As EventArgs) Handles cmdAdd2Query.Click
        SaveTempQueryItem()
    End Sub

    Private Sub cmdAdd2QueryNonItems_Click(sender As Object, e As EventArgs) Handles cmdAdd2QueryNonItems.Click
        SaveTempQueryItem()
    End Sub

    Private Sub cmdAdd2QueryPeriod_Click(sender As Object, e As EventArgs) Handles cmdAdd2QueryPeriod.Click
        SaveTempQueryItem()
    End Sub

    Private Sub cmdAdd2QueryString_Click(sender As Object, e As EventArgs) Handles cmdAdd2QueryString.Click
        SaveTempQueryItem()
    End Sub

   

    Private Sub SetupCols()
        tr1.Nodes.Clear()


        Dim lisAllCols As List(Of BO.GridColumn) = Master.Factory.j70QueryTemplateBL.ColumnsPallete(Me.CurrentX29ID)
        For Each c In lisAllCols.Where(Function(p) p.TreeGroup <> "").Select(Function(p) p.TreeGroup).Distinct
            Dim n As New Telerik.Web.UI.RadTreeNode(c, c)
            n.ImageUrl = "Images/folder.png"
            tr1.Nodes.Add(n)
        Next
        For Each c In lisAllCols
            Dim n As New Telerik.Web.UI.RadTreeNode(c.ColumnHeader, c.ColumnName)

            Select Case c.ColumnType
                Case BO.cfENUM.DateTime, BO.cfENUM.DateTime
                    n.ImageUrl = "Images/type_datetime.png"
                Case BO.cfENUM.DateOnly
                    n.ImageUrl = "Images/type_date.png"
                Case BO.cfENUM.Numeric, BO.cfENUM.Numeric2, BO.cfENUM.Numeric0
                    n.ImageUrl = "Images/type_number.png"
                Case BO.cfENUM.AnyString
                    n.ImageUrl = "Images/type_text.png"
                Case BO.cfENUM.Checkbox
                    n.ImageUrl = "Images/type_checkbox.png"
            End Select
            If Not c.ColumnName Is Nothing Then
                If c.ColumnName.IndexOf("Free") > 0 Then n.ForeColor = Drawing.Color.Green
                If Left(c.ColumnName, 3) = "tag" Then n.ImageUrl = "Images/label.png"
            End If

            If c.TreeGroup = "" Then
                tr1.Nodes.Add(n)
            Else
                tr1.FindNodeByValue(c.TreeGroup).Nodes.Add(n)
            End If

        Next

        Me.cbxOrderBy1.DataSource = lisAllCols.Where(Function(p) p.IsSortable = True)
        Me.cbxOrderBy1.DataBind()
        Me.cbxOrderBy1.Items.Insert(0, "")
        Me.cbxOrderBy2.DataSource = lisAllCols.Where(Function(p) p.IsSortable = True)
        Me.cbxOrderBy2.DataBind()
        Me.cbxOrderBy2.Items.Insert(0, "")

        
    End Sub
    Private Sub RefreshRecord_Columns(cRec As BO.j70QueryTemplate)

        lt1.Items.Clear()
        With cRec



            Master.DataPID = .PID

            For Each s In Split(.j70ColumnNames, ",")
                If s <> "" Then
                    Dim strField As String = Trim(s)
                    Dim n As Telerik.Web.UI.RadTreeNode = tr1.FindNodeByValue(strField)
                    Handle_Add(n)
                End If
            Next
            If .j70OrderBy <> "" Then
                Dim a() As String = Split(.j70OrderBy, " , ")
                If a(0).IndexOf("DESC") > 0 Then
                    Me.cbxOrderBy1Dir.SelectedValue = "DESC"
                    basUI.SelectDropdownlistValue(Me.cbxOrderBy1, Replace(a(0), " DESC", ""))
                Else
                    basUI.SelectDropdownlistValue(Me.cbxOrderBy1, a(0))
                End If
                If UBound(a) > 0 Then
                    If a(1).IndexOf("DESC") > 0 Then
                        Me.cbxOrderBy2Dir.SelectedValue = "DESC"
                        basUI.SelectDropdownlistValue(Me.cbxOrderBy2, Replace(a(1), " DESC", ""))
                    Else
                        basUI.SelectDropdownlistValue(Me.cbxOrderBy2, a(1))
                    End If
                End If
            End If
            basUI.SelectRadiolistValue(Me.j70ScrollingFlag, CInt(.j70ScrollingFlag).ToString)
            Me.j70IsFilteringByColumn.Checked = cRec.j70IsFilteringByColumn


        End With


        RefreshTreeState()
    End Sub

    Private Sub Handle_Add(n As Telerik.Web.UI.RadTreeNode)
        If n Is Nothing Then Return
        If n.ImageUrl = "Images/folder.png" Then Return
        If n.Value = "" Then Return

        If Not lt1.FindItemByValue(n.Value) Is Nothing Then
            Return
        End If
        Dim it As New Telerik.Web.UI.RadListBoxItem(n.Text, n.Value)
        it.ToolTip = n.ToolTip
        it.ForeColor = n.ForeColor
        it.ImageUrl = n.ImageUrl

        lt1.Items.Add(it)

    End Sub

    Private Sub RefreshTreeState(Optional strSelectValue As String = "")
        For Each n In tr1.GetAllNodes()
            n.Enabled = True
        Next
        For i As Integer = 0 To lt1.Items.Count - 1
            Dim n As Telerik.Web.UI.RadTreeNode = tr1.FindNodeByValue(lt1.Items(i).Value)
            If Not n Is Nothing Then
                n.Enabled = False

            End If
        Next
        If strSelectValue <> "" Then
            If Not tr1.FindNodeByValue(strSelectValue) Is Nothing Then
                tr1.FindNodeByValue(strSelectValue).Selected = True
            End If
        End If
        If lt1.Items.Count > 0 Then
            cmdRemove.Enabled = True
        Else
            cmdRemove.Enabled = False
        End If
    End Sub

   
   
    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        Handle_Add(tr1.SelectedNode)

        RefreshTreeState()
        With lt1.Items
            For i As Integer = 0 To .Count - 1
                .Item(i).Selected = False
            Next
            If .Count > 0 Then
                lt1.Items(.Count - 1).Selected = True
            End If
        End With

    End Sub

    Private Sub cmdRemove_Click(sender As Object, e As EventArgs) Handles cmdRemove.Click
        If lt1.SelectedItem Is Nothing Then Return
        Dim strVal As String = lt1.SelectedItem.Value, x As Integer = lt1.SelectedIndex
        lt1.Items.Remove(lt1.SelectedItem)
        RefreshTreeState(strVal)
        If lt1.Items.Count > x Then
            lt1.SelectedIndex = x
        Else
            lt1.SelectedIndex = 0
        End If
    End Sub

    Private Function GetOrderBy() As String
        Dim s As String = ""
        If Me.cbxOrderBy1.SelectedValue <> "" Then
            s = Me.cbxOrderBy1.SelectedValue
            If Me.cbxOrderBy1Dir.SelectedValue <> "" Then
                s += " DESC"
            End If
        End If
        If Me.cbxOrderBy2.SelectedValue <> "" Then
            If s = "" Then
                s = Me.cbxOrderBy2.SelectedValue
            Else
                s += " , " & Me.cbxOrderBy2.SelectedValue       'musí tam být před a za čárkou mezera, protože pole bohužel mohou obsahovat čárky
            End If

            If Me.cbxOrderBy2Dir.SelectedValue <> "" Then
                s += " DESC"
            End If
        End If
        Return s
    End Function

    Private Sub cmdSaveNewTemplate_Click(sender As Object, e As EventArgs) Handles cmdSaveNewTemplate.Click
        If hidNewJ70Name.Value <> "" Then
            roles1.SaveCurrentTempData()

            Dim c As New BO.j70QueryTemplate
            With c
                .x29ID = Me.CurrentX29ID
                .j70Name = Trim(Me.hidNewJ70Name.Value)
                .j70IsNegation = Me.j70IsNegation.Checked
                .j70BinFlag = BO.BAS.IsNullInt(Me.opgBin.SelectedValue)
                .j70IsSystem = False

                Dim s As String = ""
                For Each it As Telerik.Web.UI.RadListBoxItem In lt1.Items
                    s += "," & it.Value
                Next
                .j70ColumnNames = BO.BAS.OM1(s)

                .j70OrderBy = GetOrderBy()
                .j70IsFilteringByColumn = Me.j70IsFilteringByColumn.Checked
                .j70ScrollingFlag = BO.BAS.IsNullInt(Me.j70ScrollingFlag.SelectedValue)
            End With

            If Master.Factory.j70QueryTemplateBL.Save(c, GetList_j71(), roles1.GetData4Save()) Then
                Master.DataPID = Master.Factory.j70QueryTemplateBL.LastSavedPID

                SetupJ70Combo()

                ''SetupCols()
                RefreshRecord()
            Else
                Master.Notify(Master.Factory.j70QueryTemplateBL.ErrorMessage)
            End If

            hidNewJ70Name.Value = ""
        End If
        
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As ImageClickEventArgs) Handles cmdSave.Click
        If SaveChanges() Then
            Me.j70ID.SelectedItem.Text = Me.j70Name.Text
        End If
    End Sub
End Class