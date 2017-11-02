Public Class kickoff_after1
    Inherits System.Web.UI.Page
    Private _Factory As BL.Factory

    Private Sub kickoff_after1_Init(sender As Object, e As EventArgs) Handles Me.Init
        _Factory = New BL.Factory(, "mtservice")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.lblError.Text = ""

            Me.j27id.DataSource = _Factory.ftBL.GetList_J27()
            Me.j27id.DataBind()

            If Request.Item("akce") = "filtry" Then
                Filtry()
            End If
            If Request.Item("akce") = "svatky" Then
                Svatky()
            End If
        End If
    End Sub

    Private Sub PracovniCas(intP95ID As Integer)
        Dim cP34 As New BO.p34ActivityGroup
        With cP34
            .p34Name = "Klientské hodiny"
            .p34Code = "TB"
            .p33ID = BO.p33IdENUM.Cas
            .p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaJePovinna
            .p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj
        End With

        If Not _Factory.p34ActivityGroupBL.Save(cP34) Then
            WE(_Factory.p34ActivityGroupBL.ErrorMessage)
        Else
            Dim intP34ID As Integer = _Factory.p34ActivityGroupBL.LastSavedPID
            CP32(intP34ID, "Jednání s klientem", True, True, intP95ID)
            CP32(intP34ID, "Příprava na jednání", True, True, intP95ID)
            CP32(intP34ID, "Ztráta času cestováním", True, True, intP95ID)

            CP32(intP34ID, "Telefonování s klientem", True, True, intP95ID)
            CP32(intP34ID, "Studium podkladů", True, True, intP95ID)
            CP32(intP34ID, "Vedení projektu", True, True, intP95ID)
            CP32(intP34ID, "Překlady", True, True, intP95ID)
            CP32(intP34ID, "Zápis z jednání", True, True, intP95ID)
            CP32(intP34ID, "Telekonference", True, True, intP95ID)
            CP32(intP34ID, "Kompletování dokumentů", True, True, intP95ID)
            CP32(intP34ID, "Návrh dokumentů", True, True, intP95ID)
            Select Case Me.cbxBusiness.SelectedValue
                Case "AK"
                    CP32(intP34ID, "Právní stanovisko", True, True, intP95ID)
                    CP32(intP34ID, "Sepis žaloby", True, True, intP95ID)
                    CP32(intP34ID, "Návrh k insolvenci", True, True, intP95ID)
                Case "IT"
                    CP32(intP34ID, "Servisní výjezd", True, True, intP95ID)
                    CP32(intP34ID, "Vzdálená správa", True, True, intP95ID)
                    CP32(intP34ID, "Analýza", True, True, intP95ID)
                    CP32(intP34ID, "Konfigurace SW", True, True, intP95ID)
                    CP32(intP34ID, "Programování ASP.NET", True, True, intP95ID)
                    CP32(intP34ID, "Programování SQL", True, True, intP95ID)
                    CP32(intP34ID, "Grafické práce", True, True, intP95ID)
                    CP32(intP34ID, "Testování", True, True, intP95ID)
                    CP32(intP34ID, "Poskytování školení", True, True, intP95ID)
                    CP32(intP34ID, "Psaní dokumentace/helpů", True, True, intP95ID)
                    CP32(intP34ID, "Instalace nové verze/upgrade", True, True, intP95ID)
                    CP32(intP34ID, "Tvorba tiskových sestav", True, True, intP95ID)
                Case "UCTO"
                    CP32(intP34ID, "Mzdové účetnictví", True, True, intP95ID)
                    CP32(intP34ID, "Zastupování na úřadech", True, True, intP95ID)
                    CP32(intP34ID, "Roční uzávěrky", True, True, intP95ID)
                    CP32(intP34ID, "Daňové poradenství", True, True, intP95ID)
                    CP32(intP34ID, "Práce auditora", True, True, intP95ID)
                    CP32(intP34ID, "Pořizování dokladů", True, True, intP95ID)
                    CP32(intP34ID, "Pravidelná hlášení", True, True, intP95ID)
                Case "MEDIA"
                    CP32(intP34ID, "Autorský dohled", True, True, intP95ID)
                    CP32(intP34ID, "Ilustrace/Kresba", True, True, intP95ID)
                    CP32(intP34ID, "Kalkulace", True, True, intP95ID)
                    CP32(intP34ID, "DTP práce", True, True, intP95ID)
                    CP32(intP34ID, "Monitoring", True, True, intP95ID)
                    CP32(intP34ID, "Vyhledávání článků", True, True, intP95ID)
                    CP32(intP34ID, "Tvorba textů", True, True, intP95ID)
                    CP32(intP34ID, "Jazyková korekce", True, True, intP95ID)
                    CP32(intP34ID, "Vazba článků", True, True, intP95ID)

                Case "PRO"
                    CP32(intP34ID, "Projekční práce", True, True, intP95ID)
                    CP32(intP34ID, "Kompletace", True, True, intP95ID)
                    CP32(intP34ID, "Dozor stavby", True, True, intP95ID)
                    CP32(intP34ID, "Reklamace", True, True, intP95ID)
                    CP32(intP34ID, "HIP", True, True, intP95ID)

            End Select
            CP32(intP34ID, "Ostatní fakturovatelné", True, True, intP95ID, 100)
            CP32(intP34ID, "Ostatní nefakturovatelné", False, True, intP95ID, 101)
        End If
    End Sub
    Private Sub NePracovniCas()
        Dim cP34 As New BO.p34ActivityGroup
        With cP34
            .p34Name = "Interní čas"
            .p34Code = "TN"
            .p33ID = BO.p33IdENUM.Cas
            .p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaJePovinna
            .p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj
            .p34Ordinary = 1
        End With
        If Not _Factory.p34ActivityGroupBL.Save(cP34) Then
            WE(_Factory.p34ActivityGroupBL.ErrorMessage)
        Else
            Dim intP34ID As Integer = _Factory.p34ActivityGroupBL.LastSavedPID
            CP32(intP34ID, "Dovolená", False, False, 0)
            CP32(intP34ID, "E-maily", False, True, 0)
            CP32(intP34ID, "Interní organizace", False, True, 0)
            CP32(intP34ID, "Lékař", False, False, 0)
            CP32(intP34ID, "Nemoc", False, False, 0)
            CP32(intP34ID, "Oběd", False, True, 0)
            CP32(intP34ID, "Samo-studium", False, True, 0)
            CP32(intP34ID, "Příprava nabídky", False, True, 0)
            CP32(intP34ID, "Akviziční činnosti", False, True, 0)
            CP32(intP34ID, "Překlady", False, True, 0)
            CP32(intP34ID, "Prostoje", False, True, 0)
            CP32(intP34ID, "Pochůzka", False, True, 0)
            CP32(intP34ID, "Pošta", False, True, 0)
            CP32(intP34ID, "Školení/seminář", False, True, 0)
            CP32(intP34ID, "Služení cesta", False, True, 0)
            CP32(intP34ID, "Vykazování", False, True, 0)
            CP32(intP34ID, "Ostatní", False, True, 100)
        End If
    End Sub
    Private Sub Vydaje(intP95ID As Integer)
        Dim cP34 As New BO.p34ActivityGroup
        With cP34
            .p34Name = "Výdaje"
            .p34Code = "EX"
            .p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu
            .p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaJePovinna
            .p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj
            .p34Ordinary = 2
        End With
        If Not _Factory.p34ActivityGroupBL.Save(cP34) Then
            WE(_Factory.p34ActivityGroupBL.ErrorMessage)
        Else
            Dim intP34ID As Integer = _Factory.p34ActivityGroupBL.LastSavedPID
            CP32(intP34ID, "Cestovní náklady", True, True, intP95ID)
            CP32(intP34ID, "Kurýr", True, True, intP95ID)
            CP32(intP34ID, "Notářské úkony", True, True, intP95ID)
            CP32(intP34ID, "Překlady", True, True, intP95ID)
            CP32(intP34ID, "Soudní a správní poplatky", True, True, intP95ID)
            CP32(intP34ID, "Subdodávky", True, True, intP95ID)
            CP32(intP34ID, "Nákup licencí", True, True, intP95ID)
            CP32(intP34ID, "Kancelářské potřeby", True, True, intP95ID)
            CP32(intP34ID, "Ostatní fakturovatelné", True, True, intP95ID, 100)
            CP32(intP34ID, "Ostatní nefakturovatelné", False, True, intP95ID, 101)
        End If
    End Sub
    Private Sub FixniOdmeny(intP95ID As Integer)
        Dim cP34 As New BO.p34ActivityGroup
        With cP34
            .p34Name = "Pevné (paušální) odměny"
            .p34Code = "FEE"
            .p33ID = BO.p33IdENUM.PenizeBezDPH
            .p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaJePovinna
            .p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem
            .p34Ordinary = 4
        End With
        If Not _Factory.p34ActivityGroupBL.Save(cP34) Then
            WE(_Factory.p34ActivityGroupBL.ErrorMessage)
        Else
            Dim intP34ID As Integer = _Factory.p34ActivityGroupBL.LastSavedPID
            CP32(intP34ID, "Pevně domluvená odměna", True, True, intP95ID, -1)
            CP32(intP34ID, "Paušální (opakovaná) odměna", True, True, intP95ID)
            CP32(intP34ID, "Sleva", True, True, intP95ID)
            CP32(intP34ID, "Přirážka", True, True, intP95ID)
            CP32(intP34ID, "Software Maintenance", True, True, intP95ID)
            CP32(intP34ID, "Ostatní", True, True, intP95ID, 100)

        End If
    End Sub
    Private Sub CP32(intP34ID As Integer, strName As String, bolBillable As Boolean, bolTextRequired As Boolean, intP95ID As Integer, Optional intOrdinary As Integer = 0)
        Dim c As New BO.p32Activity
        c.p34ID = intP34ID
        c.p32Name = strName
        c.p32IsBillable = bolBillable
        c.p32IsTextRequired = bolTextRequired
        c.p32Ordinary = intOrdinary
        c.p95ID = intP95ID
        _Factory.p32ActivityBL.Save(c)
    End Sub

    Private Sub WE(strError As String)
        Me.lblError.Text += "<hr>" & strError
    End Sub

    Private Sub TypyProjektu()
        Dim lisP34 As IEnumerable(Of BO.p34ActivityGroup) = _Factory.p34ActivityGroupBL.GetList(New BO.myQuery)
        Dim lisX38 As IEnumerable(Of BO.x38CodeLogic) = _Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p41Project)
        If lisX38.Count = 0 Then
            WE("Nedošlo k vygenerování nastavení číselných řad.")
            Return
        End If
        Dim cRec As New BO.p42ProjectType
        With cRec
            .p42Name = "Klientský"
            .p42IsDefault = True
            .p42Code = "FP"
            .x38ID = lisX38(0).PID
        End With
        Dim lis As New List(Of BO.p43ProjectType_Workload)
        For Each cP34 In lisP34.Where(Function(p) p.p34Code <> "TN")
            Dim c As New BO.p43ProjectType_Workload()
            c.p34ID = cP34.PID
            lis.Add(c)
        Next
        _Factory.p42ProjectTypeBL.Save(cRec, lis)

        lis = New List(Of BO.p43ProjectType_Workload)
        cRec = New BO.p42ProjectType
        With cRec
            .p42Name = "Interní"
            .p42Code = "NP"
            .x38ID = lisX38(0).PID
        End With
        For Each cP34 In lisP34.Where(Function(p) p.p34Code = "TN" Or p.p34Code = "EX")
            Dim c As New BO.p43ProjectType_Workload()
            c.p34ID = cP34.PID
            lis.Add(c)
        Next
        _Factory.p42ProjectTypeBL.Save(cRec, lis)
    End Sub
    Private Sub CreateJ07(strName As String, intOrdinary As Integer)
        Dim c As New BO.j07PersonPosition
        c.j07Name = strName
        c.j07Ordinary = intOrdinary
        _Factory.j07PersonPositionBL.Save(c)
    End Sub
    Private Sub Pozice()
        CreateJ07("Partner", 1)
        CreateJ07("Senior", 2)
        If Me.cbxBusiness.SelectedValue = "AK" Then
            CreateJ07("Advokát", 3)
        Else
            CreateJ07("Konzultant", 3)
        End If
        CreateJ07("Office", 8)
        CreateJ07("Student", 10)
    End Sub
    Private Sub Svatky()
        CC26(1, 1, "Nový rok")
        CC26(6, 4, "Velikonoční pondělí")
        CC26(1, 5, "Svátek práce")
        CC26(8, 5, "Den vítězství")
        CC26(5, 7, "Den slovanských věrozvěstů Cyrila a Metoděje")
        CC26(6, 7, "Den upálení mistra Jana Husa")
        CC26(28, 9, "Den české státnosti")
        CC26(28, 10, "Den vzniku samostatného československého státu")
        CC26(17, 11, "Den boje za svobodu a demokracii")
        CC26(24, 12, "Štědrý den")
        CC26(25, 12, "1. svátek vánoční")
        CC26(26, 12, "2. svátek vánoční")
    End Sub
    Private Sub CC26(den As Integer, mesic As Integer, strName As String)
        Dim c As New BO.c26Holiday()
        c.c26Date = DateSerial(Year(Now), mesic, den)
        c.c26Name = strName
        _Factory.c26HolidayBL.Save(c)

        c = New BO.c26Holiday()
        c.c26Date = DateSerial(Year(Now) + 1, mesic, den)
        c.c26Name = strName
        _Factory.c26HolidayBL.Save(c)
    End Sub
    Private Sub Fondy()
        Dim c As New BO.c21FondCalendar()
        c.c21Name = "FULL TIME"
        c.c21Day1_Hours = 8
        c.c21Day2_Hours = 8
        c.c21Day3_Hours = 8
        c.c21Day4_Hours = 8
        c.c21Day5_Hours = 8
        c.c21ScopeFlag = BO.c21ScopeFlagENUM.Basic
        _Factory.c21FondCalendarBL.Save(c)
        Dim intC21ID As Integer = _Factory.c21FondCalendarBL.LastSavedPID


        c = New BO.c21FondCalendar
        c.c21Name = "Po+St (2 dny v týdnu)"
        c.c21Day1_Hours = 8
        c.c21Day3_Hours = 8
        c.c21ScopeFlag = BO.c21ScopeFlagENUM.Basic
        _Factory.c21FondCalendarBL.Save(c)
        
        Dim lis As IEnumerable(Of BO.j02Person) = _Factory.j02PersonBL.GetList(New BO.myQueryJ02)
        For Each person In lis
            person.c21ID = intC21ID
            _Factory.j02PersonBL.Save(person, Nothing)
        Next
    End Sub

    Private Sub ProjektoveRole()
        'výchozí situace je, že role jsou již založené, pouze aktualizovat o28
        Dim cRole As BO.x67EntityRole = _Factory.x67EntityRoleBL.Load(3)
        Dim lisP34 As IEnumerable(Of BO.p34ActivityGroup) = _Factory.p34ActivityGroupBL.GetList(New BO.myQuery)

        Dim lisO28 As New List(Of BO.o28ProjectRole_Workload)

        For Each sesit In lisP34
            Dim c As New BO.o28ProjectRole_Workload
            c.p34ID = sesit.PID
            c.o28EntryFlag = BO.o28EntryFlagENUM.ZapisovatDoProjektuIDoUloh
            c.o28PermFlag = BO.o28PermFlagENUM.CistASchvalovatVProjektu
            lisO28.Add(c)
        Next
        _Factory.x67EntityRoleBL.SaveO28(cRole.PID, lisO28)

        cRole = _Factory.x67EntityRoleBL.Load(5)
        lisO28 = New List(Of BO.o28ProjectRole_Workload)
        For Each sesit In lisP34
            Dim c As New BO.o28ProjectRole_Workload
            c.p34ID = sesit.PID
            c.o28EntryFlag = BO.o28EntryFlagENUM.ZapisovatDoProjektuIDoUloh
            c.o28PermFlag = BO.o28PermFlagENUM.PouzeVlastniWorksheet
            lisO28.Add(c)
        Next
        _Factory.x67EntityRoleBL.SaveO28(cRole.PID, lisO28)
    End Sub

    Private Sub Rezije()
        Dim intJ02ID As Integer = _Factory.j02PersonBL.GetList(New BO.myQueryJ02)(0).PID

        Dim c As New BO.p28Contact
        c.p28CompanyName = Me.txtCompany.Text
        c.p28IsCompany = True
        c.p28RegID = Me.txtIC.Text
        c.p28VatID = Me.txtDIC.Text
        c.j02ID_Owner = intJ02ID
        c.p28CompanyShortName = "_Kancelář"

        Dim lisO37 As New List(Of BO.o37Contact_Address)
        Dim cc As New BO.o37Contact_Address
        With cc
            .o38Street = Me.txtStreet.Text
            .o38City = Me.txtCity.Text
            .o38ZIP = Me.txtPostCode.Text
            .o36ID = BO.o36IdEnum.InvoiceAddress
        End With
        lisO37.Add(cc)

        If _Factory.p28ContactBL.Save(c, lisO37, Nothing, Nothing, Nothing, Nothing) Then
            Dim intP28ID As Integer = _Factory.p28ContactBL.LastSavedPID
            Dim intP42ID As Integer = _Factory.p42ProjectTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p42Code = "NP")(0).PID
            CreateP41(intP28ID, "Interní projekt", intP42ID, intJ02ID)

        Else
            WE(_Factory.p28ContactBL.ErrorMessage)
        End If
    End Sub
    Private Sub Projekty()
        Dim intJ02ID As Integer = _Factory.j02PersonBL.GetList(New BO.myQueryJ02)(0).PID
        Dim c As New BO.p28Contact
        c.p28CompanyName = Me.txtClient.Text
        c.p28IsCompany = True
        c.j02ID_Owner = intJ02ID

        If _Factory.p28ContactBL.Save(c, Nothing, Nothing, Nothing, Nothing, Nothing) Then
            Dim intP28ID As Integer = _Factory.p28ContactBL.LastSavedPID
            Dim intP42ID As Integer = _Factory.p42ProjectTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p42Code = "FP")(0).PID
            If Trim(Me.txtProject1.Text) = "" Then Me.txtProject1.Text = "General"
            CreateP41(intP28ID, Me.txtProject1.Text, intP42ID, intJ02ID)
            If Trim(Me.txtProject2.Text) <> "" Then
                CreateP41(intP28ID, Me.txtProject2.Text, intP42ID, intJ02ID)
            End If

        Else
            WE(_Factory.p28ContactBL.ErrorMessage)
        End If
    End Sub

    Private Sub CreateP41(intP28ID As Integer, strName As String, intP42ID As Integer, intJ02ID As Integer)
        Dim cP41 As New BO.p41Project
        cP41.p42ID = intP42ID
        cP41.p41Name = strName
        cP41.p28ID_Client = intP28ID
        cP41.j02ID_Owner = intJ02ID
        Dim lisRoles As New List(Of BO.x69EntityRole_Assign)
        Dim role As New BO.x69EntityRole_Assign
        role.j02ID = intJ02ID
        role.x67ID = 3
        lisRoles.Add(role)
        role = New BO.x69EntityRole_Assign
        role.j11ID = GetJ11ID_All()    'tým = všichni
        role.x67ID = 5
        lisRoles.Add(role)
        _Factory.p41ProjectBL.Save(cP41, Nothing, Nothing, lisRoles, Nothing)
    End Sub
    Private Sub TypyMilniku()
        CO21(BO.x29IdEnum.p41Project, "Lhůta")
        CO21(BO.x29IdEnum.p41Project, "Schůzka s klientem")
        CO21(BO.x29IdEnum.p41Project, "Kontrolní den", 10)
        CO21(BO.x29IdEnum.p41Project, "Vypršení licence", 10)
        CO21(BO.x29IdEnum.p41Project, "Ostatní", 100)

        CO21(BO.x29IdEnum.p28Contact, "Schůzka s klientem")
        CO21(BO.x29IdEnum.p28Contact, "Ostatní", 100)

        CO21(BO.x29IdEnum.j02Person, "Narozeniny")
        CO21(BO.x29IdEnum.j02Person, "Termín školení")
        CO21(BO.x29IdEnum.j02Person, "Ostatní", 100)
    End Sub
    Private Sub CO21(x29id As BO.x29IdEnum, strName As String, Optional intOrdinary As Integer = 0)
        Dim c As New BO.o21MilestoneType
        c.x29ID = x29id
        c.o21Name = strName
        c.o21Flag = BO.o21FlagEnum.DeadlineOrMilestone
        c.o21Ordinary = intOrdinary
        _Factory.o21MilestoneTypeBL.Save(c)
    End Sub
    Private Sub TypyUkolu()
        CP57("Úkol")
        ''CP57("Servisní výjezd")
        ''CP57("Ostatní", 100)
    End Sub
    Private Sub CP57(strName As String, Optional intOrdinary As Integer = 0)
        Dim c As New BO.p57TaskType
        c.p57Name = strName
        c.p57Ordinary = intOrdinary
        c.p57IsEntry_Budget = True
        c.p57IsEntry_CompletePercent = True
        c.p57IsEntry_Priority = True
        c.p57IsEntry_Receiver = True

        If _Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p56Task).Count > 0 Then
            c.x38ID = _Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p56Task)(0).PID
        End If

        _Factory.p57TaskTypeBL.Save(c)
    End Sub

    Private Sub NastaveniFakturace()
        Dim c As New BO.p93InvoiceHeader
        c.p93Name = Me.txtCompany.Text
        c.p93Company = Me.txtCompany.Text
        c.p93RegID = Me.txtIC.Text
        c.p93VatID = Me.txtDIC.Text
        c.p93Street = Me.txtStreet.Text
        c.p93City = Me.txtCity.Text
        c.p93Zip = Me.txtPostCode.Text

        Dim ba As New BO.p86BankAccount
        ba.p86Name = "Firemní účet"
        ba.p86BankCode = Me.txtBankCode.Text
        ba.p86BankAccount = Me.txtBankAccount.Text
        _Factory.p86BankAccountBL.Save(ba)
        Dim intP86ID As Integer = _Factory.p86BankAccountBL.LastSavedPID

        Dim lisP88 As New List(Of BO.p88InvoiceHeader_BankAccount)
        Dim cP88 As New BO.p88InvoiceHeader_BankAccount
        cP88.j27ID = CInt(Me.j27id.SelectedValue)
        cP88.p86ID = intP86ID
        lisP88.Add(cP88)
        _Factory.p93InvoiceHeaderBL.Save(c, lisP88)

        _Factory.x35GlobalParam.UpdateValue("j27ID_Invoice", Me.j27id.SelectedValue)
        _Factory.x35GlobalParam.UpdateValue("j27ID_Domestic", Me.j27id.SelectedValue)
        _Factory.x35GlobalParam.UpdateValue("Round2Minutes", "5")

        
    End Sub

    Private Sub DphSazby()
        Dim intJ27ID As Integer = BO.BAS.IsNullInt(Me.j27id.SelectedValue)

        CreateP53(BO.x15IdEnum.BezDPH, 0, intJ27ID)
        CreateP53(BO.x15IdEnum.SnizenaSazba, BO.BAS.IsNullNum(Me.txtVatRateLow.Text), intJ27ID)
        CreateP53(BO.x15IdEnum.ZakladniSazba, BO.BAS.IsNullNum(Me.txtVatRateStandard.Text), intJ27ID)

        If intJ27ID = 2 Then
            CreateP53(BO.x15IdEnum.BezDPH, 0, 3)
        End If
    End Sub

    Private Sub CreateP53(x15id As BO.x15IdEnum, dblValue As Double, intJ27ID As Integer)
        Dim cP53 As New BO.p53VatRate
        cP53.x15ID = x15id
        cP53.j27ID = intJ27ID
        cP53.p53Value = dblValue
        cP53.ValidFrom = DateSerial(Year(Now), 1, 1)
        _Factory.p53VatRateBL.Save(cP53)
    End Sub

    Private Function CreateP95(strName As String) As Integer
        Dim c As New BO.p95InvoiceRow
        c.p95Name = strName

        If _Factory.p95InvoiceRowBL.Save(c) Then
            Return _Factory.p95InvoiceRowBL.LastSavedPID
        End If
    End Function

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click
        Me.lblError.Text = ""
        If Me.cbxBusiness.SelectedItem Is Nothing Then
            WE("Musíte vybrat odvětví podnikání.")
            Return
        End If
        If Trim(Me.txtCompany.Text) = "" Then
            WE("Musíte vyplnit název vaší společnosti.")
            Return
        End If
        If Trim(Me.txtClient.Text) = "" Then
            WE("Musíte vyplnit název jednoho z vašich klientů.")
            Return
        End If
        If _Factory.p34ActivityGroupBL.GetList(New BO.myQuery).Count = 0 Then
            Dim intP95ID As Integer = CreateP95("Pracovní čas")
            PracovniCas(intP95ID)
            NePracovniCas()
            intP95ID = CreateP95("Výdaje")
            Vydaje(intP95ID)
            intP95ID = CreateP95("Odměny")
            FixniOdmeny(intP95ID)
        End If
        If _Factory.p42ProjectTypeBL.GetList(New BO.myQuery).Count = 0 Then
            TypyProjektu()
        End If
        If _Factory.j07PersonPositionBL.GetList().Count = 0 Then
            Pozice()
        End If
        If _Factory.c26HolidayBL.GetList().Count = 0 Then
            Svatky()
        End If
        If _Factory.c21FondCalendarBL.GetList().Count = 0 Then
            Fondy()
        End If

        If _Factory.x67EntityRoleBL.GetList_o28(BO.BAS.ConvertInt2List(3)).Count = 0 Then
            ProjektoveRole()
        End If
        If _Factory.p28ContactBL.GetList(New BO.myQueryP28).Count = 0 Then
            Rezije()
            Projekty()
        End If
        If _Factory.o21MilestoneTypeBL.GetList(New BO.myQuery).Count = 0 Then
            TypyMilniku()
        End If
        If _Factory.p57TaskTypeBL.GetList().Count = 0 Then
            TypyUkolu()
        End If
        If _Factory.p53VatRateBL.GetList(New BO.myQuery).Count = 0 Then
            DphSazby()
        End If
        If _Factory.p93InvoiceHeaderBL.GetList().Count = 0 Then
            NastaveniFakturace()
        End If

        Filtry()


        If Me.lblError.Text = "" Then
            Response.Redirect("kickoff_after2.aspx")
        End If

    End Sub

    Private Sub Filtry()
        Dim strColumnNames As String = "Client,p41Name"
        CreateQuery("Můj seznam oblíbených", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 20)
        CreateQuery("Projekty s kontaktní osobou", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 16)
        CreateQuery("Není přiřazen ceník sazeb", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 13)
        CreateQuery("Projekty v režimu DRAFT", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 11)
        CreateQuery("Projekty s opakovanou paušální odměnou", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 10)
        CreateQuery("Matky opakovaných projektů", BO.x29IdEnum.p41Project, 0, strColumnNames, "_other", 22)
        CreateQuery("Překročen limit rozpracovanosti", BO.x29IdEnum.p41Project, 0, "Client,p41Name,p41LimitHours_Notification,p41LimitFee_Notification,WIP_Hodiny,WIP_Castka", "_other", 4)
        CreateQuery("Projekty s vystavenou fakturou", BO.x29IdEnum.p41Project, 0, "Client,p41Name,Vyfakturovano_PocetFaktur,Vyfakturovano_Naposledy_Kdy", "_other", 15)
        CreateQuery("Stav fakturace", BO.x29IdEnum.p41Project, 0, "Client,p41Name,NI_Castka,Vyfakturovano_Naposledy_Kdy")
        CreateQuery("Schváleno, čeká na fakturaci", BO.x29IdEnum.p41Project, 0, "Client,p41Name,AP_Hodiny,AP_Castka", "_other", 5)
        CreateQuery("Rozpracováno, čeká na schválení", BO.x29IdEnum.p41Project, 0, "Client,p41Name,WIP_Hodiny,WIP_Castka", "_other", 3)
        CreateQuery("Nevyfakturováno", BO.x29IdEnum.p41Project, 0, "Client,p41Name,NI_Hodiny,NI_Castka", "_other", 3)
        CreateQuery("Otevřené úkoly", BO.x29IdEnum.p41Project, 0, "Client,p41Name,PendingTasks", "_other", 6)
        CreateQuery("Naposledy založené", BO.x29IdEnum.p41Project, 0, "Client,p41Name,p41DateInsert,p41UserInsert", , , , "a.p41DateInsert DESC")
        CreateQuery("Štítky", BO.x29IdEnum.p41Project, 0, "Client,p41Name,TagsHtml")
        CreateQuery("Strom", BO.x29IdEnum.p41Project, 0, "Client,p41TreePath")
        CreateQuery("Projekty v archivu", BO.x29IdEnum.p41Project, 2, strColumnNames)
        CreateQuery("Otevřené projekty", BO.x29IdEnum.p41Project, 1, strColumnNames)


        strColumnNames = "p28Name"
        CreateQuery("Otevření klienti", BO.x29IdEnum.p28Contact, 1, strColumnNames)
        CreateQuery("Klienti v archivu", BO.x29IdEnum.p28Contact, 2, strColumnNames)
        CreateQuery("Strom", BO.x29IdEnum.p28Contact, 0, "p28TreePath")
        CreateQuery("Štítky", BO.x29IdEnum.p28Contact, 0, "p28Name,TagsHtml")
        CreateQuery("Naposledy založené", BO.x29IdEnum.p28Contact, 0, "p28Name,p28DateInsert,p28UserInsert", , , , "a.p28DateInsert DESC")
        CreateQuery("Stav fakturace", BO.x29IdEnum.p28Contact, 0, "p28Name,NI_Castka,Vyfakturovano_Naposledy_Kdy")
        CreateQuery("Nevyfakturovano", BO.x29IdEnum.p28Contact, 0, "p28Name,NI_Hodiny,NI_Castka", "_other", 35)
        CreateQuery("Rozpracováno, čeká na schválení", BO.x29IdEnum.p28Contact, 0, "p28Name,WIP_Hodiny,WIP_Castka", "_other", 3)
        CreateQuery("Schváleno, čeká na fakturaci", BO.x29IdEnum.p28Contact, 0, "p28Name,AP_Hodiny,AP_Castka", "_other", 5)
        CreateQuery("Kontaktní osoby klienta", BO.x29IdEnum.p28Contact, 0, "p28Name,KontaktniOsoby")
        CreateQuery("Kontaktní média klienta", BO.x29IdEnum.p28Contact, 0, "p28Name,KontaktniMedia")
        CreateQuery("Fakturační e-mail", BO.x29IdEnum.p28Contact, 0, "p28Name,FakturacniEmail")
        CreateQuery("Klient+podřízení klienti", BO.x29IdEnum.p28Contact, 0, "p28Name,ChildContactsInline")
        
        strColumnNames = "FullNameDesc"
        CreateQuery("Nevyfakturované hodiny", BO.x29IdEnum.j02Person, 0, "FullNameDesc,NI_Hodiny", "_other", 35)
        CreateQuery("Rozpracované hodiny", BO.x29IdEnum.j02Person, 0, "FullNameDesc,WIP_Hodiny", "_other", 3)
        CreateQuery("Štítky", BO.x29IdEnum.j02Person, 0, "FullNameDesc,TagsHtml")
        CreateQuery("Osoby v archivu", BO.x29IdEnum.j02Person, 2, strColumnNames)
        CreateQuery("Kontaktní osoby", BO.x29IdEnum.j02Person, 0, "FullNameDesc,VazbaKlient", "_other", 7)
        CreateQuery("Interní osoby", BO.x29IdEnum.j02Person, 1, strColumnNames, "_other", 6)


        strColumnNames = "p91Code,p91Client,p91Amount_WithoutVat,j27Code"

        CreateQuery("S nulovou sazbou DPH", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 12)
        CreateQuery("S přepotem  měnového kurzu", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 13)
        CreateQuery("S haléřovým zaokrouhlením", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 9)
        CreateQuery("Svázané s opravným dokladem", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 8)
        CreateQuery("Svázané se zálohou", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 7)
        CreateQuery("Stav odesílání", BO.x29IdEnum.p91Invoice, 0, "p91Code,p91Client,VomStav,VomKomu")
        CreateQuery("S oficiálním číslem", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 6)
        CreateQuery("DRAFT doklady", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 5)
        CreateQuery("Ve splatnosti", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 3)
        CreateQuery("Neuhrazené po splatnosti", BO.x29IdEnum.p91Invoice, 0, strColumnNames, "_other", 4)

        strColumnNames = "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
        CreateQuery("Vygenerováno automaticky robotem", BO.x29IdEnum.p31Worksheet, 0, strColumnNames, "_other", 13)
        CreateQuery("Přiřazen úkol", BO.x29IdEnum.p31Worksheet, 0, strColumnNames, "_other", 9)
        CreateQuery("Přiřazena kontaktní osoba", BO.x29IdEnum.p31Worksheet, 0, strColumnNames, "_other", 6)
        CreateQuery("Štítky", BO.x29IdEnum.p31Worksheet, 0, "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Amount_WithoutVat_Orig,p31Text,TagsHtml")
        CreateQuery("Přesunuto do archivu", BO.x29IdEnum.p31Worksheet, 0, strColumnNames, "_other", 4)
        CreateQuery("Výsledovka", BO.x29IdEnum.p31Worksheet, 0, "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,j27Code_Billing_Orig,p31Text,Vykazano_Naklad,Vykazano_Vynos,Vykazano_Zisk,Vyfakturovano_Vynos,Vyfakturovano_Zisk")
        CreateQuery("Vyfakturováno", BO.x29IdEnum.p31Worksheet, 0, "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Invoiced,p31Amount_WithoutVat_Invoiced,p31Text,p91Code", "_other", 3)
        CreateQuery("Schváleno, čeká na fakturaci", BO.x29IdEnum.p31Worksheet, 0, "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Hours_Approved_Billing,p31Amount_WithoutVat_Approved,j27Code_Billing_Orig,p31Text", "_other", 2)
        CreateQuery("Rozpracovanost, čeká na schvalování", BO.x29IdEnum.p31Worksheet, 0, "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text", "_other", 1)
        CreateQuery("Pevné (paušální) odměny", BO.x29IdEnum.p31Worksheet, 0, "p31Date,ClientName,p41Name,Person,p32Name,p31Amount_WithoutVat_Orig,j27Code_Billing_Orig,p31Text", "p34id", 4, "Pevné (paušální) odměny")
        CreateQuery("Výdaje", BO.x29IdEnum.p31Worksheet, 0, "p31Date,ClientName,p41Name,Person,p32Name,p31Amount_WithoutVat_Orig,p31VatRate_Orig,j27Code_Billing_Orig,p31Text", "p34id", 3, "Výdaje")

        strColumnNames = "ClientAndProject,p56Name,b02Name"
        CreateQuery("Matky opakovaných úkolů", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 13)
        CreateQuery("Je po termínu dokončení", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 7)
        CreateQuery("Vyplněn termín dokončení", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 6)
        CreateQuery("Vyplněn plán/limit hodin", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 9)
        CreateQuery("Schválené úkony, čeká na fakturaci", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 5)
        CreateQuery("Rozpracované, čeká na schvalování", BO.x29IdEnum.p56Task, 0, strColumnNames, "_other", 3)
        CreateQuery("Uzavřené úkoly (v archivu)", BO.x29IdEnum.p56Task, 2, strColumnNames)
        CreateQuery("Otevřené úkoly", BO.x29IdEnum.p56Task, 1, strColumnNames)








    End Sub

    Private Function Findj71RecordName(x29id As BO.x29IdEnum, intJ71RecordPID As Integer) As String
        If _Factory.j70QueryTemplateBL.GetList_OtherQueryItem(x29id).Where(Function(p) p.pid = intJ71RecordPID).Count > 0 Then
            Return _Factory.j70QueryTemplateBL.GetList_OtherQueryItem(x29id).Where(Function(p) p.pid = intJ71RecordPID)(0).Text
        Else
            Return ""
        End If
    End Function

    Private Function GetJ11ID_All() As Integer
        Dim lisJ11 As IEnumerable(Of BO.j11Team) = _Factory.j11TeamBL.GetList()
        If lisJ11.Where(Function(p) p.j11IsAllPersons = True).Count > 0 Then
            Return lisJ11.Where(Function(p) p.j11IsAllPersons = True)(0).PID
        Else
            Return lisJ11(0).PID
        End If
    End Function
    Private Sub CreateQuery(strJ70Name As String, x29ID As BO.x29IdEnum, intBinFlag As Integer, strColumnNames As String, Optional strJ71Field As String = "", Optional intJ71RecordPID As Integer = 0, Optional strJ71RecordName As String = "", Optional strJ70OrderBy As String = "")
        Dim mqJ02 As New BO.myQueryJ02
        mqJ02.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
        mqJ02.Closed = BO.BooleanQueryMode.FalseQuery
        Dim intJ02ID As Integer = 0
        Try
            intJ02ID = _Factory.j02PersonBL.GetList(mqJ02)(0).PID
        Catch ex As Exception
            Response.Write(ex.Message & "<hr>CreateQuery start")
            Return
        End Try
        Dim mq As New BO.myQueryJ03
        mq.j02ID = intJ02ID
        Dim intJ03ID As Integer = 0
        Try
            intJ03ID = _Factory.j03UserBL.GetList(mq)(0).PID
        Catch ex As Exception
            Response.Write(ex.Message & "<hr>intJ02ID: " & intJ02ID.ToString)
            Return
        End Try

        
        Dim c As New BO.j70QueryTemplate
        c.j70Name = strJ70Name
        c.j70BinFlag = intBinFlag
        c.j02ID_Owner = intJ02ID
        c.j03ID = intJ03ID
        c.x29ID = x29ID
        c.j70ColumnNames = strColumnNames
        c.j70OrderBy = strJ70OrderBy
        c.j70ScrollingFlag = BO.j70ScrollingFlagENUM.StaticHeaders
        c.j70IsFilteringByColumn = True

        Dim lisJ71 As New List(Of BO.j71QueryTemplate_Item)
        If strJ71Field <> "" Then
            ''Dim cI As New BO.j71QueryTemplate_Item
            ''cI.j71ValueType = "combo"
            ''cI.j71RecordPID = intJ71RecordPID
            ''cI.j71Field = strJ71Field
            ''If strJ71Field = "_other" Then
            ''    cI.j71FieldLabel = "Různé"
            ''    cI.j71RecordName = Findj71RecordName(x29ID, cI.j71RecordPID)
            ''Else
            ''    If strJ71Field = "p34id" Then cI.j71FieldLabel = "Sešit"
            ''    cI.j71RecordName = strJ71RecordName
            ''End If
            ''lisJ71.Add(cI)
            Select Case intJ71RecordPID
                Case 35
                    lisJ71.Add(CreateJ71(x29ID, strJ71Field, 3, strJ71RecordName))
                    lisJ71.Add(CreateJ71(x29ID, strJ71Field, 5, strJ71RecordName))
                Case Else
                    lisJ71.Add(CreateJ71(x29ID, strJ71Field, intJ71RecordPID, strJ71RecordName))
            End Select

        End If

        Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
        Dim cJ As New BO.x69EntityRole_Assign
        cJ.x67ID = _Factory.x67EntityRoleBL.GetList(New BO.myQuery).First(Function(p) p.x29ID = BO.x29IdEnum.j70QueryTemplate).PID

        cJ.j11ID = GetJ11ID_All()
        lisX69.Add(cJ)
        _Factory.j70QueryTemplateBL.Save(c, lisJ71, lisX69)
    End Sub

    Private Function CreateJ71(x29ID As BO.x29IdEnum, strJ71Field As String, intJ71RecordPID As Integer, strJ71RecordName As String) As BO.j71QueryTemplate_Item
        Dim cI As New BO.j71QueryTemplate_Item
        cI.j71ValueType = "combo"
        cI.j71RecordPID = intJ71RecordPID
        cI.j71Field = strJ71Field
        If strJ71Field = "_other" Then
            cI.j71FieldLabel = "Různé"
            cI.j71RecordName = Findj71RecordName(x29ID, cI.j71RecordPID)
        Else
            If strJ71Field = "p34id" Then cI.j71FieldLabel = "Sešit"
            cI.j71RecordName = strJ71RecordName
        End If
        Return cI

    End Function
End Class