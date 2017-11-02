Imports Telerik.Web.UI

Public Class admin_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _bolNeedExportList As Boolean = False
    Private Property _needFilterIsChanged As Boolean = False


    Private Sub admin_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        Master.HelpTopicID = "admin_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("prefix") = Request.Item("prefix")
            hidGo2Pid.Value = Request.Item("go2pid")
            With Master
                .PageTitle = "Nastavení systému"
                .SiteMenuValue = "admin_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_Admin)

                ViewState("page") = Right(ViewState("prefix"), 3) & "_record.aspx"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("admin_framework-prefix")
                    .Add(ViewState("prefix") & "-pagesize")
                    .Add(ViewState("prefix") & "-query-validity")
                    .Add("admin_framework-chkShowCustomTailor")
                    .Add(ViewState("prefix") & "-admin_framework-search")
                    .Add("admin_framework-filter_setting-" & ViewState("prefix"))
                    .Add("admin_framework-filter_sql-" & ViewState("prefix"))
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                

            End With
            With Master.Factory.j03UserBL
                cbxPaging.SelectedValue = .GetUserParam(ViewState("prefix") & "-pagesize", "20")
                Me.txtSearch.Text = .GetUserParam(ViewState("prefix") & "-admin_framework-search")
                basUI.SelectDropdownlistValue(Me.query_validity, .GetUserParam(ViewState("prefix") & "-query-validity"))
            End With
            menu1.FindItemByValue("refresh").NavigateUrl = "admin_framework.aspx?prefix=" & ViewState("prefix")

            SetupTreeMenu()
            If ViewState("prefix") = "" Then
                RefreshRecord()
                panelmenu1.SelectedValue = "dashboard"
            Else
                With Master.Factory.j03UserBL
                    SetupGrid(.GetUserParam("admin_framework-filter_setting-" & ViewState("prefix")), .GetUserParam("admin_framework-filter_sql-" & ViewState("prefix")))
                End With
                panelmenu1.SelectedValue = ViewState("prefix")
            End If
            Select Case ViewState("prefix")
                Case "m62"
                    Dim cmd As New RadMenuItem("Import kurzů z ČNB", "javascript: sw_master('m62_import_setting.aspx','Images/setting.png',false)")
                    cmd.PostBack = False
                    menu1.Items.Add(cmd)
                
            End Select

            Dim cF As New BO.clsFile
            If Not cF.FileExist(BO.ASS.GetApplicationRootFolder & "\Plugins\company_logo.png") Then
                'zkopírovat výchozí logo obrázek do company_logo.png
                If cF.FileExist(BO.ASS.GetApplicationRootFolder & "\Plugins\company_logo_default.png") Then
                    cF.CopyFile(BO.ASS.GetApplicationRootFolder & "\Plugins\company_logo_default.png", BO.ASS.GetApplicationRootFolder & "\Plugins\company_logo.png")
                End If
            End If
            imgLogoPreview.DataValue = cF.GetBinaryContent(BO.ASS.GetApplicationRootFolder & "\Plugins\company_logo.png")

        End If
    End Sub
    Private Sub admin_framework_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        Dim b As Boolean = False
        If ViewState("prefix") <> "" Then
            b = True
            
        End If
        lblPath.Text = panelmenu1.GetSelectedPath()


        panDashboard.Visible = Not b
        panGRID.Visible = b
        cbxPaging.Visible = b
        query_validity.Visible = b
        With menu1
            .Visible = b
        End With

        If ViewState("prefix") <> "" Then
            menu1.FindItemByValue("new").Visible = True

            Select Case ViewState("prefix")
                Case "x28", "j05", "c26", "p36"
                    query_validity.Visible = False
                Case Else
                    query_validity.Visible = True
            End Select
        End If
        If Trim(txtSearch.Text) = "" Then
            txtSearch.Style.Item("background-color") = ""
        Else
            txtSearch.Style.Item("background-color") = basUI.ColorQueryCSS
        End If
        If query_validity.SelectedIndex > 0 Then
            query_validity.BackColor = basUI.ColorQueryRGB
        Else
            query_validity.BackColor = Nothing
        End If

        If BO.BAS.IsNullInt(Me.hidGo2Pid.Value) > 0 Then
            Dim intPID As Integer = CInt(hidGo2Pid.Value) : hidGo2Pid.Value = ""
            grid1.SelectRecords(intPID)
            If grid1.radGridOrig.SelectedItems.Count = 0 And grid1.radGridOrig.MasterTableView.PageCount > 1 Then
                Dim x As Integer = 0
                While grid1.radGridOrig.MasterTableView.PageCount > grid1.radGridOrig.CurrentPageIndex
                    grid1.radGridOrig.CurrentPageIndex += 1
                    grid1.Rebind(True, intPID)
                    If grid1.radGridOrig.SelectedItems.Count > 0 Then
                        Exit While
                    End If
                    x += 1
                    If x > 1000 Then Exit While
                End While

            End If
            If grid1.radGridOrig.SelectedItems.Count > 0 Then hiddatapid.Value = intPID.ToString
        End If
    End Sub

    Private Sub SetupTreeMenu()
        With Me.panelmenu1

            .AddItem("Možnosti", "dashboard", "admin_framework.aspx", , "Images/settings.png")
            .AddItem("Uživatelé", "user", , , "Images/user.png")
            .AddItem("Uživatelské účty", "j03", NU("j03"), "user")
            .AddItem("Aplikační role", "j04", NU("j04"), "user")

            .AddItem("Osoby", "person", , , "Images/person.png")
            .AddItem("Osobní profily", "j02", NU("j02"), "person")
            .AddItem("Pozice osob", "j07", NU("j07"), "person")
            .AddItem("Týmy osob", "j11", NU("j11"), "person")
            .AddItem("Nadřízení/podřízení", "j05", NU("j05"), "person")
            .AddItem("Osobní plány", "p66", "javascript:p66_plan()", "person")
            .AddItem("Pracovní fondy", "c21", NU("c21"), "person")
            .AddItem("Dny svátků", "c26", NU("c26"), "person")


            '.AddItem("Časové fondy", "j11", NU("j11"), "user")

            .AddItem("Worksheet", "p31", , , "Images/worksheet.png")            
            .AddItem("Sešity", "p34", NU("p34"), "p31")
            .AddItem("Aktivity", "p32", NU("p32"), "p31")
            .AddItem("Klastry aktivit", "p61", NU("p61"), "p31")
            .AddItem("Uzamknutá období", "p36", NU("p36"), "p31")
            .AddItem("Kusovníkové jednotky", "p35", NU("p35"), "p31")
            .AddItem("Nastavení interních ceníků", "p50", NU("p50"), "p31")

            .AddItem("Projekty", "p41", , , "Images/project.png")
            .AddItem("Typy projektů", "p42", NU("p42"), "p41")
            .AddItem("Projektové role", "p41_x67", NU("p41_x67"), "p41")

            .AddItem("Klienti", "p28", , , "Images/contact.png")
            .AddItem("Typy klientů", "p29", NU("p29"), "p28")
            .AddItem("Role v klientovi", "p28_x67", NU("p28_x67"), "p28")

            


            .AddItem("Dokumenty", "o23", , , "Images/notepad.png")
            .AddItem("Typy dokumentů", "o23_x18", "x18_framework.aspx?source=admin", "o23")
            .AddItem("Role v dokumentu", "o23_x67", NU("o23_x67"), "o23")


            .AddItem("Fakturace", "p91", , , "Images/billing.png")
            .AddItem("Typy faktur", "p92", NU("p92"), "p91")
            .AddItem("DPH sazby", "p53", NU("p53"), "p91")

            .AddItem("Bankovní účty", "p86", NU("p86"), "p91")
            .AddItem("Vystavovatelé faktur", "p93", NU("p93"), "p91")

            .AddItem("Měnové kurzy", "m62", NU("m62"), "p91")
            .AddItem("Fakturační oddíly", "p95", NU("p95"), "p91")
            .AddItem("Zaokrouhlovací pravidla", "p98", NU("p98"), "p91")
            .AddItem("Struktury rozpisu částky faktury", "p80", NU("p80"), "p91")
            .AddItem("Režijní přirážky k fakturaci", "p63", NU("p63"), "p91")
            .AddItem("Role ve faktuře", "p91_x67", NU("p91_x67"), "p91")

            .AddItem("Typy záloh", "p89", NU("p89"), "p91")


            .AddItem("Úkoly", "p56", , , "Images/task.png")
            .AddItem("Typy úkolů", "p57", NU("p57"), "p56")
            .AddItem("Role v úkolu", "p56_x67", NU("p56_x67"), "p56")
            .AddItem("Priority úkolu", "p59", NU("p59"), "p56")
            .AddItem("Kalendářové události", "o22", , , "Images/calendar.png")
            .AddItem("Typy událostí pro projekt", "p41_o21", NU("p41_o21"), "o22")
            .AddItem("Typy událostí pro klienta", "p28_o21", NU("p28_o21"), "o22")
            .AddItem("Typy událostí pro osobní profil", "j02_o21", NU("j02_o21"), "o22")
            ''.AddItem("Nepersonální zdroje", "j23", NU("j23"), "o22")
            ''.AddItem("Typy neper. zdrojů", "j24", NU("j24"), "o22")

            .AddItem("Tiskové sestavy a pluginy", "reporting", , , "Images/report.png")
            .AddItem("Šablony sestav", "rep_x31", NU("rep_x31"), "reporting")
            .AddItem("Šablony pluginů", "plugin_x31", NU("plugin_x31"), "reporting")
            .AddItem("Kategorie sestav a pluginů", "j25", NU("j25"), "reporting")
            '.AddItem("Úkoly", "p56", , , "Images/task.png")
            '.AddItem("Ostatní", "other", , , "Images/settings.png")
            '.AddItem("SMTP server", "smtp", , "other")
           


            

            .AddItem("Pošta/SMTP/IMAP", "smtp_imap", , , "Images/email.png")
            .AddItem("Výchozí aplikační poštovní účet", "admin_smtp", "javascript:sw_master('admin_smtp.aspx','Images/setting_32.png')", "smtp_imap")
            .AddItem("SMTP účty", "o40", NU("o40"), "smtp_imap")

            .AddItem("Historie/Fronta odeslaných zpráv", "x40", "x40_framework.aspx", "smtp_imap")

            .AddItem("IMAP účty", "o41", NU("o41"), "smtp_imap")
            .AddItem("IMAP pravidla", "o42", NU("o42"), "smtp_imap")

            .AddItem("Notifikační pravidla", "x46", NU("x46"), "smtp_imap")

            .AddItem("Uživatelská pole", "ff", , , "Images/form.png")
            .AddItem("Katalog polí", "x28", NU("x28"), "ff")

            .AddItem("Skupiny uživatelských polí", "x27", NU("x27"), "ff")

            .AddItem("Ostatní nastavení", "other", , , "Images/more.png")

            .AddItem("Číselné řady", "x38", NU("x38"), "other")
            .AddItem("Střediska", "j18", NU("j18"), "other")
            .AddItem("Pravidla opakovaných úkolů a projektů", "p65", NU("p65"), "other")
            .AddItem("Regiony", "j17", NU("j17"), "other")
            .AddItem("Textové šablony", "j61", NU("j61"), "other")
            .AddItem("Plánovač úloh", "x48", NU("x48"), "other")
            .AddItem("Struktura aplikačního menu", "menu", "admin_menu.aspx", "other")
            .AddItem("Návrhář workflow šablon", "workflow", "admin_workflow.aspx", "other")

            .AddItem("Gadget nastavení", "x55", NU("x55"), "other")




        End With
    End Sub

    Private Sub RefreshRecord()
        

        With Master.Factory.x35GlobalParam
            '.InhaleParams(lis)
            ''Me.lblLicCompany.Text = .GetValueString("License_Company")


            Me.AppName.Text = .GetValueString("AppName")
            Me.version.Text = BO.ASS.GetUIVersion
            'Me.lblLicExpiration.DataValue = .GetValueString("License_Expiration")
            Me.Round2Minutes.Text = .GetValueString("Round2Minutes")
            Me.DefMaturityDays.Text = .GetValueString("DefMaturityDays")
            Dim intJ27ID As Integer = BO.BAS.IsNullInt(.GetValueString("j27ID_Domestic"))
            If intJ27ID > 0 Then
                Me.j27ID_Domestic.Text = Master.Factory.ftBL.LoadJ27(intJ27ID).j27Code
            End If
            If .GetValueInteger("p32ID_CreditNote") <> 0 Then
                Dim cP32 As BO.p32Activity = Master.Factory.p32ActivityBL.Load(.GetValueInteger("p32ID_CreditNote"))
                If Not cP32 Is Nothing Then Me.p32ID_CreditNote.Text = cP32.NameWithSheet
            End If

            Me.Upload_Folder.Text = .GetValueString("Upload_Folder")
            If Me.Upload_Folder.Text <> "" Then Me.Upload_Folder.Text = "*******************" & Right(Me.Upload_Folder.Text, 5)
            Me.robot_host.Text = .GetValueString("robot_host")
            If Me.robot_host.Text = "" Then
                Me.robot_host.Text = "Robot není nastaven!"

     
            End If
            Me.robot_cache_lastrequest.Text = .GetValueString("robot_cache_lastrequest")


            Select Case .UserAuthenticationMode
                Case BO.UserAuthenticationModeEnum.MixedMode
                    Me.UserAuthenticationMode.Text = "Přihlašování povoleno aplikačně i přes Windows doménu"
                Case BO.UserAuthenticationModeEnum.AnonymousOnly
                    Me.UserAuthenticationMode.Text = "Přihlašování povoleno pouze přes aplikační db účtů"
                Case BO.UserAuthenticationModeEnum.WindowsOnly
                    Me.UserAuthenticationMode.Text = "Přihlašování povoleno pouze přes Windows doménu"
            End Select
        End With
        RefreshP87()

        Dim mq As New BO.myQueryJ03
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Me.lblRealUsersCount.Text = Master.Factory.j03UserBL.GetList(mq).Count.ToString
    End Sub

    Private Sub RefreshP87()
        Dim lis As IEnumerable(Of BO.p87BillingLanguage) = Master.Factory.ftBL.GetList_P87(New BO.myQuery)
        For Each c In lis
            CType(panP87.FindControl("BillingLang" & c.p87LangIndex.ToString), Label).Text = c.p87Name
            If c.p87Icon <> "" Then
                CType(panP87.FindControl("p87Icon" & c.p87LangIndex.ToString), Image).Visible = True
                CType(panP87.FindControl("p87Icon" & c.p87LangIndex.ToString), Image).ImageUrl = "Images/flags/" & c.p87Icon
            Else
                CType(panP87.FindControl("p87Icon" & c.p87LangIndex.ToString), Image).Visible = False
            End If
        Next
    End Sub



    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)

        Dim bolSearch As Boolean = False
        grid1.ClearColumns()

        With grid1
            .PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
            .radGridOrig.ShowFooter = False
            .AddContextMenuColumn(16)
            
            Select Case ViewState("prefix")
                Case "j02"
                    bolSearch = True
                    .AddColumn("FullNameDesc", "Jméno")
                    .AddColumn("j02Email", "E-mail")
                    .AddColumn("j07Name", "Pozice")
                    .AddColumn("c21Name", "Fond")
                    .AddColumn("j18Name", "Středisko")


                Case "j03"
                    bolSearch = True
                    .AddColumn("j03Login", "Login")
                    .AddColumn("Person", "Osoba")
                    .AddColumn("j04Name", "Aplikační role")
                Case "j04"
                    .AddColumn("j04Name", "Název role")
                Case "j05"
                    .AddColumn("PersonMaster", "Nadřízený")
                    .AddColumn("PersonSlave", "Podřízený (osoba)")
                    .AddColumn("TeamSlave", "Podřízený (tým)")
                Case "j07"
                    .AddColumn("j07Name", "Název pozice")
                    .AddColumn("j07Ordinary", "#")
                Case "j17"
                    .AddColumn("j17Name", "Název")
                    .AddColumn("j17Ordinary", "#", BO.cfENUM.Numeric0)
                Case "j18"
                    .AddColumn("j18Name", "Název střediska")
                    .AddColumn("j18Ordinary", "#", BO.cfENUM.Numeric0)
               
                Case "j61"
                    .AddColumn("j61Name", "Název šablony")
                    .AddColumn("Owner", "Vlastník")
                    .AddColumn("x29Name", "Entita")
                    ''Case "j62"
                    ''    .AddColumn("TreeMenuItem", "Název položky menu")
                    ''    .AddColumn("x29Name", "Modul")
                    ''    .AddColumn("j62Tag", "Tag")
                    ''    .AddColumn("j62Ordinary", "#", BO.cfENUM.Numeric0)
                Case "c21"
                    .AddColumn("c21Name", "Název pracovního fondu")
                    .AddColumn("c21Ordinary", "#", BO.cfENUM.Numeric0)
                Case "c26"
                    .AddColumn("c26Name", "Název svátku")
                    .AddColumn("c26Date", "Den", BO.cfENUM.DateOnly)
                    .AddColumn("j17Name", "Region")
                Case "j11"
                    .AddColumn("j11Name", "Název týmu")
                    ''Case "j23"
                    ''    .AddColumn("j24Name", "Typ zdroje")
                    ''    .AddColumn("j23Name", "Název")
                    ''    .AddColumn("j23Code", "Kód")
                    ''    .AddColumn("j23Ordinary", "#")
                    ''Case "j24"
                    ''    .AddColumn("j24Name", "Název typu zdroje")
                    ''    .AddColumn("j24Ordinary", "#")
                Case "p50"
                    .AddColumn("Binding", "Druh sazby")
                    .AddColumn("p51Name", "Název ceníku")
                    .AddColumn("ValidFrom", "Platí od", BO.cfENUM.DateTime)
                    .AddColumn("ValidUntil", "Platí do", BO.cfENUM.DateTime)
                Case "j25"
                    .AddColumn("j25Name", "Název kategorie")
                    .AddColumn("j25Ordinary", "#")
                Case "rep_x31", "plugin_x31"
                    .AddColumn("FormatName", "Formát")
                    .AddColumn("x31Name", "Název sestavy")
                    .AddColumn("x31FileName", "Soubor šablony")
                    '.AddColumn("j25Name", "Kategorie")
                    .AddColumn("x29Name", "Kontext")
                    .AddColumn("x31Ordinary", "#")
                Case "x38"
                    .AddColumn("x38Name", "Název")
                    .AddColumn("CodeLogicInfo", "")
                Case "x55"
                    .AddColumn("x55Name", "Název")
                    .AddColumn("flag", "")
                    .AddColumn("x55Code", "Kód")
                Case "p95"
                    .AddColumn("p95Name", "Název fakturačního oddílu")
                    .AddColumn("p95Code", "Kód")
                    .AddColumn("p95Ordinary", "#")
                Case "p98"
                    .AddColumn("p98Name", "Název pravidla")
                    .AddColumn("p98IsDefault", "Výchozí pravidlo", BO.cfENUM.Checkbox)
                Case "p63"
                    .AddColumn("NameWithRate", "Název pravidla")
                    .AddColumn("p32Name", "Aktivita")
                Case "p65"
                    .AddColumn("p65Name", "Název pravidla")
                Case "p80"
                    .AddColumn("p80Name", "Název pravidla")
                    .AddColumn("p80IsFeeSeparate", "Pevné odměny 1:1", BO.cfENUM.Checkbox)
                    .AddColumn("p80IsExpenseSeparate", "Výdaje 1:1", BO.cfENUM.Checkbox)
                    .AddColumn("p80IsTimeSeparate", "Čas 1:1", BO.cfENUM.Checkbox)
                Case "p92"
                    .AddColumn("p92Name", "Název")
                    .AddColumn("j27Code", "Cílová měna")
                    .AddColumn("p93Name", "Vystavovatel")
                    .AddColumn("j17Name", "DPH region")
                Case "p89"
                    .AddColumn("p89Name", "Název")
                    '.AddColumn("j27Code", "Cílová měna")
                    .AddColumn("p93Name", "Vystavovatel")
                Case "o41"
                    .AddColumn("o41Name", "Název účtu")
                    .AddColumn("o41Login", "Login")
                    .AddColumn("o41Server", "Server")
                Case "o40"
                    .AddColumn("o40Name", "Název účtu")
                    .AddColumn("o40EmailAddress", "E-mail adresa")
                    .AddColumn("o40Server", "SMTP server")
                Case "o42"
                    .AddColumn("o42Name", "Název pravidla")
                Case "p93"
                    .AddColumn("p93Name", "Název hlavičky")
                    .AddColumn("p93Company", "Firma")
                Case "p86"
                    .AddColumn("p86Name", "Název účtu")
                    .AddColumn("p86BankName", "Název banky")
                    .AddColumn("p86BankAccount", "Číslo účtu")
                    .AddColumn("p86BankCode", "Kód banky")
                    .AddColumn("p86SWIFT", "SWIFT")
                    .AddColumn("p86IBAN", "IBAN")
                Case "m62"
                    .AddColumn("RateType", "Typ kurzu")
                    .AddColumn("m62Date", "Datum kurzu", BO.cfENUM.DateOnly)
                    .AddColumn("m62Rate", "Kurz", BO.cfENUM.Numeric3)
                    .AddColumn("j27Code_Slave", "Cílová měna")
                    .AddColumn("j27Code_Master", "Zdrojová měna")

                Case "p32"
                    bolSearch = True
                    .AddColumn("p32name", "Název aktivity")
                    .AddColumn("p34name", "Sešit")
                    .AddColumn("p32Code", "Kód aktivity")
                    .AddColumn("p32IsBillable", "Fakturovatelné", BO.cfENUM.Checkbox)
                    .AddColumn("p95Name", "Fakt.oddíl")
                    .AddColumn("p32Color", "Barva")
                    .AddColumn("p32Ordinary", "#", BO.cfENUM.Numeric0)
                Case "p34"
                    .AddColumn("p34name", "Název sešitu")
                    .AddColumn("p34Code", "Kód")
                    .AddColumn("p33Name", "Vstupní data")
                    .AddColumn("p34Color", "Barva")
                    .AddColumn("p34Ordinary", "#", BO.cfENUM.Numeric0)
                Case "p61"
                    .AddColumn("p61Name", "Název klastru")
                Case "p36"
                    .AddColumn("p36DateFrom", "Od", BO.cfENUM.DateOnly)
                    .AddColumn("p36DateUntil", "Do", BO.cfENUM.DateOnly)
                    .AddColumn("Person", "Osoba")
                    .AddColumn("j11Name", "Tým osob")
                    .AddColumn("p36IsAllSheets", "Všechny sešity", BO.cfENUM.Checkbox)
                Case "p42"
                    .AddColumn("p42name", "Název")
                    .AddColumn("b01Name", "Workflow šablona")
                    .AddColumn("p42Ordinary", "#", BO.cfENUM.Numeric0)
                Case "p35"
                    .AddColumn("p35Name", "Název")
                    .AddColumn("p35Code", "Kód")
                Case "p57"
                    .AddColumn("p57Name", "Název")
                    .AddColumn("b01Name", "Workflow šablona")
                    .AddColumn("p57Ordinary", "#", BO.cfENUM.Numeric0)
                  
                Case "p59"
                    .AddColumn("p59Name", "Název priority")
                    .AddColumn("p59Ordinary", "#", BO.cfENUM.Numeric0)
                Case "p29"
                    .AddColumn("p29Name", "Název")
                    .AddColumn("p29Ordinary", "#", BO.cfENUM.Numeric0)
                
                    'Case "x23"
                    '    .AddColumn("x23Name", "Název")
                    '    .AddColumn("x23Ordinary", "#", BO.cfENUM.Numeric0)
                
                Case "x27"
                    .AddColumn("x27Name", "Název skupiny")
                    .AddColumn("x27Ordinary", "#", BO.cfENUM.Numeric0)
                Case "x28"
                    .AddColumn("x29Name", "Entita")
                    .AddColumn("x28Name", "Název pole")
                    .AddColumn("x27Name", "Skupina")
                    .AddColumn("x28Field", "Fyzický název pole")
                    ''.AddColumn("x23Name", "Combo seznam")
                    .AddColumn("x28IsRequired", "Povinné", BO.cfENUM.Checkbox)
                    .AddColumn("x28Ordinary", "#", BO.cfENUM.Numeric0)
                Case "p41_o21", "p28_o21", "j02_o21"
                    .AddSystemColumn(16)
                    .AddColumn("o21Name", "Název")

                    .AddColumn("o21Ordinary", "#", BO.cfENUM.Numeric0)
                Case "x46"
                    .AddColumn("x45Name", "Událost")
                    .AddColumn("x46MessageSubject", "Předmět zprávy")
                    .AddColumn("Person", "Příjemce (osoba)")
                    .AddColumn("j11Name", "Příjemce (tým osob)")
                    .AddColumn("x67Name", "Příjemce (role)")
                    .AddColumn("x29NameSingle", "Referenční entita")
                    ''Case "x40"
                    ''    .AddSystemColumn(20, "UserInsert")
                    ''    .AddColumn("DateUpdate", "Čas", BO.cfENUM.DateTime)
                    ''    .AddColumn("x40State", "Stav")
                    ''    .AddColumn("x40SenderName", "Odesílatel")
                    ''    '.AddColumn("x40SenderAddress", "Adresa")
                    ''    .AddColumn("x40Recipient", "Příjemce")
                    ''    .AddColumn("x40Subject", "Předmět zprávy")
                    ''    .AddColumn("x40WhenProceeded", "Zpracováno", BO.cfENUM.DateTime)
                    ''    .AddColumn("x40ErrorMessage", "Chyba")

                Case "p53"
                    .AddColumn("x15Name", "Hladina")
                    .AddColumn("p53Value", "Hodnota sazby", BO.cfENUM.Numeric)
                    .AddColumn("j27Code", "Měna")
                    .AddColumn("j17Name", "DPH region")
                    .AddColumn("ValidFrom", "Platnost od", BO.cfENUM.DateTime)
                    .AddColumn("ValidUntil", "Platnost do", BO.cfENUM.DateOnly)
                Case "p41_x67", "p56_x67", "x31_x67", "p28_x67", "p91_x67", "p90_x67", "o23_x67"
                    .AddColumn("x67Name", "Název role")
                    .AddColumn("x67Ordinary", "#", BO.cfENUM.Numeric0)
                Case "x48"
                    .AddColumn("x48Name", "Název úlohy")
            End Select
            Select Case ViewState("prefix")
                Case "p35"
                Case Else
                    .AddColumn("DateUpdate", "Posl.aktualizace", BO.cfENUM.DateTime)
                    .AddColumn("UserUpdate", "Aktualizoval")
            End Select

            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With

        cmdSearch.Visible = bolSearch
        txtSearch.Visible = bolSearch
    End Sub


    Private Function NU(ByVal strValue As String) As String
        Return "javascript:lp('" & strValue & "')"
    End Function

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        With dataItem("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('admin-" & ViewState("prefix") & "','" & dataItem.GetDataKeyValue("pid").ToString & "',this)" & Chr(34) & "></a>"
        End With

        Select Case ViewState("prefix")
            Case "p41_o21", "p28_o21", "j02_o21"
                Dim cRec As BO.o21MilestoneType = CType(e.Item.DataItem, BO.o21MilestoneType)
                Select Case cRec.o21Flag
                    Case BO.o21FlagEnum.DeadlineOrMilestone
                        dataItem("systemcolumn").CssClass = "o21_1"
                    Case BO.o21FlagEnum.EventFromUntil
                        dataItem("systemcolumn").CssClass = "o21_2"
                    Case BO.o21FlagEnum.MemoOnly
                        dataItem("systemcolumn").CssClass = "o21_3"
                End Select
                ''Case "x40"
                ''    basUIMT.x40_grid_Handle_ItemDataBound(sender, e)
            Case "x55"
                Dim cRec As BO.x55HtmlSnippet = CType(e.Item.DataItem, BO.x55HtmlSnippet)
                Select Case cRec.x55TypeFlag
                    Case BO.x55TypeENUM.DynamicHtml
                        dataItem("flag").Text = "Dynamické HTML"
                    Case BO.x55TypeENUM.ExternalPage
                        dataItem("flag").Text = "Externí HTML stránka"
                    Case BO.x55TypeENUM.StaticHtml
                        dataItem("flag").Text = "Statické HTML"
                End Select
                If cRec.x55IsSystem Then dataItem.ForeColor = Drawing.Color.CornflowerBlue
            Case "p34"
                Dim cRec As BO.p34ActivityGroup = CType(e.Item.DataItem, BO.p34ActivityGroup)
                If cRec.p34Color <> "" Then
                    dataItem("p34Color").Style.Item("background-color") = cRec.p34Color
                End If
                Select Case cRec.p33ID
                    Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                        If cRec.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
                            dataItem.ForeColor = Drawing.Color.Blue
                        Else
                            dataItem.ForeColor = Drawing.Color.Brown
                        End If
                    Case BO.p33IdENUM.Kusovnik
                        dataItem.ForeColor = Drawing.Color.Green
                End Select
            Case "p32"
                Dim cRec As BO.p32Activity = CType(e.Item.DataItem, BO.p32Activity)
                If cRec.p32Color <> "" Then
                    dataItem("p32Color").Style.Item("background-color") = cRec.p32Color
                End If
                Select Case cRec.p33ID
                    Case 1
                        If cRec.p32ManualFeeFlag = 1 Then
                            dataItem.Font.Italic = True
                        End If
                    Case 3
                        dataItem.ForeColor = Drawing.Color.Green
                    Case 2, 5
                        If cRec.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
                            dataItem.ForeColor = Drawing.Color.Blue
                        Else
                            dataItem.ForeColor = Drawing.Color.Brown
                        End If
                End Select
                
            Case "o40"
                Dim cRec As BO.o40SmtpAccount = CType(e.Item.DataItem, BO.o40SmtpAccount)
                If cRec.o40IsGlobalDefault Then
                    dataItem.BackColor = System.Drawing.Color.SkyBlue
                End If
        End Select
     


        If dataItem.GetDataKeyValue("IsClosed") Then dataItem.Font.Strikeout = True

      
    End Sub
    Private Function VQ() As BO.BooleanQueryMode
        Select Case Me.query_validity.SelectedValue
            Case "1" : Return BO.BooleanQueryMode.FalseQuery
            Case "2" : Return BO.BooleanQueryMode.TrueQuery
            Case Else : Return BO.BooleanQueryMode.NoQuery
        End Select
    End Function
    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("admin_framework-filter_setting-" & ViewState("prefix"), grid1.GetFilterSetting())
                .SetUserParam("admin_framework-filter_sql-" & ViewState("prefix"), grid1.GetFilterExpression())
            End With
        End If
        grid1.DataKeyNames = "pid,IsClosed"
        Dim mqDef As New BO.myQuery
        mqDef.Closed = VQ()
        Dim lisGRID As IEnumerable(Of Object) = Nothing
        With Master.Factory
            Select Case ViewState("prefix")
                Case "j02"
                    Dim mq As New BO.myQueryJ02
                    mq.Closed = VQ()
                    mq.SearchExpression = txtSearch.Text
                    Dim lis As IEnumerable(Of BO.j02Person) = .j02PersonBL.GetList(mq)
                    grid1.DataSource = lis
                Case "j03"
                    Dim mq As New BO.myQueryJ03
                    mq.Closed = VQ()
                    mq.SearchExpression = txtSearch.Text
                    Dim lis As IEnumerable(Of BO.j03User) = .j03UserBL.GetList(mq)
                    grid1.DataSource = lis
                Case "j04"
                    Dim lis As IEnumerable(Of BO.j04UserRole) = .j04UserRoleBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "j05"
                    Dim lis As IEnumerable(Of BO.j05MasterSlave) = .j05MasterSlaveBL.GetList(0, 0, 0)
                    grid1.DataSource = lis
                Case "j07"
                    Dim lis As IEnumerable(Of BO.j07PersonPosition) = .j07PersonPositionBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "j17"
                    Dim lis As IEnumerable(Of BO.j17Country) = .j17CountryBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "j18"
                    Dim lis As IEnumerable(Of BO.j18Region) = .j18RegionBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "c21"
                    Dim lis As IEnumerable(Of BO.c21FondCalendar) = .c21FondCalendarBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "c26"
                    Dim lis As IEnumerable(Of BO.c26Holiday) = .c26HolidayBL.GetList(mqDef)
                    grid1.DataSource = lis
                    'Case "j24"
                    '    Dim lis As IEnumerable(Of BO.j24NonPersonType) = .j24NonePersonTypeBL.GetList(mqDef)
                    '    grid1.DataSource = lis
                    '    ''Case "j23"
                    '    ''    Dim lis As IEnumerable(Of BO.j23NonPerson) = .j23NonPersonBL.GetList(mqDef)
                    '    ''    grid1.DataSource = lis
                Case "j25"
                    Dim lis As IEnumerable(Of BO.j25ReportCategory) = .j25ReportCategoryBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "j11"
                    Dim lis As IEnumerable(Of BO.j11Team) = .j11TeamBL.GetList(mqDef).Where(Function(p) p.j11IsAllPersons = False)
                    grid1.DataSource = lis
                Case "p32"
                    Dim mq As New BO.myQueryP32
                    mq.Closed = VQ()
                    mq.SearchExpression = txtSearch.Text
                    Dim lis As IEnumerable(Of BO.p32Activity) = .p32ActivityBL.GetList(mq)
                    grid1.DataSource = lis
                Case "p34"
                    Dim lis As IEnumerable(Of BO.p34ActivityGroup) = .p34ActivityGroupBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p61"
                    grid1.DataSource = .p61ActivityClusterBL.GetList(mqDef)
                Case "p36"
                    Dim lis As IEnumerable(Of BO.p36LockPeriod) = .p36LockPeriodBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p35"
                    Dim lis As IEnumerable(Of BO.p35Unit) = .p35UnitBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p42"
                    Dim lis As IEnumerable(Of BO.p42ProjectType) = .p42ProjectTypeBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p57"
                    Dim lis As IEnumerable(Of BO.p57TaskType) = .p57TaskTypeBL.GetList(mqDef)
                    grid1.DataSource = lis
                    lisGRID = lis
                Case "p29"
                    Dim lis As IEnumerable(Of BO.p29ContactType) = .p29ContactTypeBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p50"
                    Dim lis As IEnumerable(Of BO.p50OfficePriceList) = .p50OfficePriceListBL.GetList(mqDef)
                    grid1.DataSource = lis
              
                Case "p41_o21", "p28_o21", "j02_o21"
                    Dim x29id As BO.x29IdEnum = BO.BAS.GetX29FromPrefix(Left(ViewState("prefix"), 3))
                    Dim lis As IEnumerable(Of BO.o21MilestoneType) = .o21MilestoneTypeBL.GetList(mqDef).Where(Function(p) p.x29ID = x29id)
                    grid1.DataSource = lis


                Case "p41_x67", "p56_x67", "x31_x67", "p28_x67", "p91_x67", "p90_x67", "o23_x67"
                    Dim lis As IEnumerable(Of BO.x67EntityRole) = .x67EntityRoleBL.GetList(mqDef).Where(Function(p) p.x29ID = BO.BAS.GetX29FromPrefix(Left(ViewState("prefix"), 3)))
                    grid1.DataSource = lis

                Case "p95"
                    Dim lis As IEnumerable(Of BO.p95InvoiceRow) = .p95InvoiceRowBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "o41"
                    Dim lis As IEnumerable(Of BO.o41InboxAccount) = .o41InboxAccountBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "o40"
                    Dim lis As IEnumerable(Of BO.o40SmtpAccount) = .o40SmtpAccountBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "o42"
                    Dim lis As IEnumerable(Of BO.o42ImapRule) = .o42ImapRuleBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p98"
                    Dim lis As IEnumerable(Of BO.p98Invoice_Round_Setting_Template) = .p98Invoice_Round_Setting_TemplateBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p63"
                    Dim lis As IEnumerable(Of BO.p63Overhead) = .p63OverheadBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p65"
                    Dim lis As IEnumerable(Of BO.p65Recurrence) = .p65RecurrenceBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p80"
                    Dim lis As IEnumerable(Of BO.p80InvoiceAmountStructure) = .p80InvoiceAmountStructureBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p92"
                    Dim lis As IEnumerable(Of BO.p92InvoiceType) = .p92InvoiceTypeBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p89"
                    Dim lis As IEnumerable(Of BO.p89ProformaType) = .p89ProformaTypeBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p93"
                    Dim lis As IEnumerable(Of BO.p93InvoiceHeader) = .p93InvoiceHeaderBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p86"
                    Dim lis As IEnumerable(Of BO.p86BankAccount) = .p86BankAccountBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "p53"
                    Dim lis As IEnumerable(Of BO.p53VatRate) = .p53VatRateBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "m62"
                    Dim lis As IEnumerable(Of BO.m62ExchangeRate) = .m62ExchangeRateBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "j61"
                    Dim lis As IEnumerable(Of BO.j61TextTemplate) = .j61TextTemplateBL.GetList(mqDef)
                    grid1.DataSource = lis
                    ''Case "j62"
                    ''    Dim lis As IEnumerable(Of BO.j62MenuHome) = .j62MenuHomeBL.GetList(0, mqDef)
                    ''    grid1.DataSource = lis
               
                Case "p59"
                    Dim lis As IEnumerable(Of BO.p59Priority) = .p59PriorityBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "x38"
                    Dim lis As IEnumerable(Of BO.x38CodeLogic) = .x38CodeLogicBL.GetList(BO.x29IdEnum._NotSpecified)
                    grid1.DataSource = lis
                Case "x55"
                    Dim lis As IEnumerable(Of BO.x55HtmlSnippet) = .x55HtmlSnippetBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "x27"
                    Dim lis As IEnumerable(Of BO.x27EntityFieldGroup) = .x27EntityFieldGroupBL.GetList(mqDef)
                    grid1.DataSource = lis
                Case "x28"
                    Dim lis As IEnumerable(Of BO.x28EntityField) = .x28EntityFieldBL.GetList(BO.x29IdEnum._NotSpecified, -1, False)
                    grid1.DataSource = lis
                    ''Case "x23"
                    ''    Dim lis As IEnumerable(Of BO.x23EntityField_Combo) = .x23EntityField_ComboBL.GetList(mqDef)
                    ''    grid1.DataSource = lis
               
                
                Case "x46"
                    Dim lis As IEnumerable(Of BO.x46EventNotification) = .x46EventNotificationBL.GetList(mqDef)
                    grid1.DataSource = lis
                    ''Case "x40"
                    ''    Dim mq As New BO.myQueryX40
                    ''    mq.SearchExpression = grid1.GetFilterExpression()
                    ''    mq.TopRecordsOnly = 500
                    ''    Dim lis As IEnumerable(Of BO.x40MailQueue) = .x40MailQueueBL.GetList(mq)
                    ''    grid1.DataSource = lis
                Case "plugin_x31"
                    Dim lis As IEnumerable(Of BO.x31Report) = .x31ReportBL.GetList(mqDef).Where(Function(p) p.x31FormatFlag = BO.x31FormatFlagENUM.ASPX)
                    grid1.DataSource = lis
                Case "rep_x31"
                    Dim lis As IEnumerable(Of BO.x31Report) = .x31ReportBL.GetList(mqDef).Where(Function(p) p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik Or p.x31FormatFlag = BO.x31FormatFlagENUM.DOCX Or p.x31FormatFlag = BO.x31FormatFlagENUM.XLSX)
                    grid1.DataSource = lis
                Case "x48"
                    Dim lis As IEnumerable(Of BO.x48SqlTask) = .x48SqlTaskBL.GetList(mqDef)
                    grid1.DataSource = lis
            End Select
        End With

        If _bolNeedExportList Then
            Dim cXLS As New clsExportToXls(Master.Factory)
            Dim strFields As String = "", strHeaders As String = ""
            For Each c As Object In grid1.radGridOrig.Columns
                If strFields = "" Then
                    strFields = c.DataField
                    strHeaders = c.HeaderText
                Else
                    strFields += "|" & c.DataField
                    strHeaders += "|" & c.HeaderText
                End If
            Next
            strFields += "|UserInsert|DateInsert|UserUpdate|DateUpdate|PID"
            strHeaders += "|Založil kdo|Založeno kdy|Aktualizoval kdo|Aktualizováno kdy|PID"

            Dim strFileName As String = cXLS.ExportGenericData(grid1.DataSource, strFields, strHeaders)
            If strFileName = "" Then
                Master.Notify(cXLS.ErrorMessage, NotifyLevel.ErrorMessage)
            Else
                Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
            End If
        
        End If
        
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam(ViewState("prefix") & "-pagesize", cbxPaging.SelectedValue)

        grid1.PageSize = BO.BAS.IsNullInt(cbxPaging.SelectedItem.Text)
        If grid1.radGridOrig.CurrentPageIndex > 0 Then grid1.radGridOrig.CurrentPageIndex = 0
        grid1.Rebind(True)

    End Sub

    

    Private Sub menu1_ItemClick(sender As Object, e As RadMenuEventArgs) Handles menu1.ItemClick
        If e.Item.Value = "export" Then
            ExportGridData()
        End If
    End Sub

    Private Sub ExportGridData()
        
        _bolNeedExportList = True
        grid1.Rebind(True)

       
    End Sub

   
    

   
    Private Sub cmdSearch_Click(sender As Object, e As ImageClickEventArgs) Handles cmdSearch.Click
        Handle_RunSearch()
    End Sub
    Private Sub Handle_RunSearch()
        Master.Factory.j03UserBL.SetUserParam(ViewState("prefix") & "-admin_framework-search", txtSearch.Text)

        grid1.Rebind(False)

        txtSearch.Focus()
    End Sub

    Private Sub query_validity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles query_validity.SelectedIndexChanged

        Master.Factory.j03UserBL.SetUserParam(ViewState("prefix") & "-query-validity", query_validity.SelectedValue)

        grid1.Rebind(False)
    End Sub

    Private Sub cmdExpandAll_Click(sender As Object, e As ImageClickEventArgs) Handles cmdExpandAll.Click
        panelmenu1.ExpandAll()
    End Sub

    Private Sub cmdCollapseAll_Click(sender As Object, e As ImageClickEventArgs) Handles cmdCollapseAll.Click
        panelmenu1.CollapseAll()
    End Sub
End Class