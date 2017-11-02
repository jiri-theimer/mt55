Public Interface Ij70QueryTemplateBL
    Inherits IFMother
    Function Save(cRec As BO.j70QueryTemplate, lisJ71 As List(Of BO.j71QueryTemplate_Item), lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
    Function Load(intPID As Integer) As BO.j70QueryTemplate
    Function LoadSystemTemplate(x29id As BO.x29IdEnum, intJ03ID As Integer, Optional strMasterPrefix As String = "")
    Function Delete(intPID As Integer) As Boolean
    Function GetList(myQuery As BO.myQuery, _x29id As BO.x29IdEnum, Optional strMasterPrefix As String = "", Optional onlyQuery As BO.BooleanQueryMode = BO.BooleanQueryMode.NoQuery) As IEnumerable(Of BO.j70QueryTemplate)
    Function GetList_j71(intPID As Integer) As IEnumerable(Of BO.j71QueryTemplate_Item)
    Function GetList_OtherQueryItem(x29id As BO.x29IdEnum) As List(Of BO.OtherQueryItem)

    Function GetSqlWhere(intJ70ID As Integer) As String
    
    Sub Setupj71TempList(intPID As Integer, strGUID As String)

    Function CheckDefaultTemplate(x29id As BO.x29IdEnum, intJ03ID As Integer, Optional strMasterPrefix As String = "") As Boolean

    Function ColumnsPallete(x29id As BO.x29IdEnum) As List(Of BO.GridColumn)
    Function GroupByPallet(x29id As BO.x29IdEnum) As List(Of BO.GridGroupByColumn)
End Interface

Class j70QueryTemplateBL
    Inherits BLMother
    Implements Ij70QueryTemplateBL
    Private WithEvents _cDL As DL.j70QueryTemplateDL
    Private _x29id As BO.x29IdEnum
    Private _lisX67 As IEnumerable(Of BO.x67EntityRole) = Nothing

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j70QueryTemplateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function Delete(intPID As Integer) As Boolean Implements Ij70QueryTemplateBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(myQuery As BO.myQuery, _x29id As BO.x29IdEnum, Optional strMasterPrefix As String = "", Optional onlyQuery As BO.BooleanQueryMode = BO.BooleanQueryMode.NoQuery) As System.Collections.Generic.IEnumerable(Of BO.j70QueryTemplate) Implements Ij70QueryTemplateBL.GetList
        Return _cDL.GetList(myQuery, _x29id, strMasterPrefix)
    End Function


    Public Function Load(intPID As Integer) As BO.j70QueryTemplate Implements Ij70QueryTemplateBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadSystemTemplate(x29id As BO.x29IdEnum, intJ03ID As Integer, Optional strMasterPrefix As String = "") Implements Ij70QueryTemplateBL.LoadSystemTemplate
        Dim c As BO.j70QueryTemplate = _cDL.LoadSystemTemplate(x29id, intJ03ID, strMasterPrefix)
        If c Is Nothing Then
            If CheckDefaultTemplate(BO.x29IdEnum.p31Worksheet, _cUser.PID, strMasterPrefix) Then
                c = _cDL.LoadSystemTemplate(x29id, intJ03ID, strMasterPrefix)
            End If
        End If
        Return c
    End Function
   
    Public Function Save(cRec As BO.j70QueryTemplate, lisJ71 As List(Of BO.j71QueryTemplate_Item), lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ij70QueryTemplateBL.Save
        With cRec
            If .j03ID = 0 Then .j03ID = _cUser.PID
            If Trim(.j70Name) = "" Then _Error = "Chybí název šablony filtru."
            If .x29ID = BO.x29IdEnum._NotSpecified Then _Error = "Chybí x29ID."
            If Trim(.j70ColumnNames) = "" Then
                _Error = "Přehled musí obsahovat minimálně jeden sloupec."
            End If
        End With

        If _Error <> "" Then Return False

        Return _cDL.Save(cRec, lisJ71, lisX69)

    End Function

    Public Sub Setupj71TempList(intPID As Integer, strGUID As String) Implements Ij70QueryTemplateBL.Setupj71TempList
        _cDL.Setupj71TempList(intPID, strGUID)
    End Sub

    
    Public Function GetList_j71(intPID As Integer) As IEnumerable(Of BO.j71QueryTemplate_Item) Implements Ij70QueryTemplateBL.GetList_j71
        Return _cDL.GetList_j71(intPID)
    End Function


    Public Function GetList_OtherQueryItem(x29id As BO.x29IdEnum) As List(Of BO.OtherQueryItem) Implements Ij70QueryTemplateBL.GetList_OtherQueryItem
        Dim lis As New List(Of BO.OtherQueryItem)
        Select Case x29id
            Case BO.x29IdEnum.p41Project
                lis.Add(New BO.OtherQueryItem(3, "Obsahují rozpracované úkony, které čekají na schvalování"))
                lis.Add(New BO.OtherQueryItem(5, "Obsahují schválené úkony, které čekají na fakturaci"))
                lis.Add(New BO.OtherQueryItem(15, "Obsahují vyfakturované úkony"))
                lis.Add(New BO.OtherQueryItem(4, "Došlo k překročení limitu rozpracovanosti"))
                lis.Add(New BO.OtherQueryItem(7, "Obsahují úkoly (otevřené nebo uzavřené)"))
                lis.Add(New BO.OtherQueryItem(6, "Obsahují otevřené úkoly"))
                lis.Add(New BO.OtherQueryItem(9, "Mají vazbu na minimálně jeden dokument"))
                lis.Add(New BO.OtherQueryItem(10, "Obsahují opakované paušální odměny"))
                lis.Add(New BO.OtherQueryItem(11, "Projekty v režimu DRAFT"))
                lis.Add(New BO.OtherQueryItem(12, "Projekty mimo režim DRAFT"))
                lis.Add(New BO.OtherQueryItem(13, "Není přiřazen ceník k projektu ani ke klientovi projektu"))
                lis.Add(New BO.OtherQueryItem(14, "Je přiřazen ceník k projektu nebo ke klientovi projektu"))
                lis.Add(New BO.OtherQueryItem(16, "Přiřazena minimálně jedna kontaktní osoba"))
                lis.Add(New BO.OtherQueryItem(17, "Bez přiřazení kontaktních osob"))
                lis.Add(New BO.OtherQueryItem(18, "Má nadřízený projekt"))
                lis.Add(New BO.OtherQueryItem(19, "Má pod-projekty"))
                lis.Add(New BO.OtherQueryItem(20, "Moje oblíbené projekty"))
                lis.Add(New BO.OtherQueryItem(21, "Vyplněna fakturační poznámka projektu"))
                lis.Add(New BO.OtherQueryItem(22, "Je matkou opakovaných projektů"))

            Case BO.x29IdEnum.p28Contact
                lis.Add(New BO.OtherQueryItem(3, "Projekty klienta obsahují rozpracované úkony, které čekají na schvalování"))
                lis.Add(New BO.OtherQueryItem(5, "Projektovy klienta obsahují schválené úkony, které čekají na fakturaci"))
                lis.Add(New BO.OtherQueryItem(4, "Došlo k překročení limitu rozpracovanosti (buď limit na projektech nebo limit samotného klienta)"))
                lis.Add(New BO.OtherQueryItem(18, "Je klientem minimálně jednoho projektu"))
                lis.Add(New BO.OtherQueryItem(19, "Není klientem ani u jednoho projektu"))
                lis.Add(New BO.OtherQueryItem(16, "Přiřazena minimálně jedna kontaktní osoba"))
                lis.Add(New BO.OtherQueryItem(17, "Bez přiřazení kontaktních osob"))
                lis.Add(New BO.OtherQueryItem(20, "Mají vazbu na minimálně jeden dokument"))
                lis.Add(New BO.OtherQueryItem(21, "Vystupuje jako dodavatel"))
                lis.Add(New BO.OtherQueryItem(28, "Ani klient, ani dodavatel"))
                lis.Add(New BO.OtherQueryItem(22, "Duplicitní klienti podle prvních 25 písmen v názvu"))
                lis.Add(New BO.OtherQueryItem(23, "Duplicitní klienti podle IČ"))
                lis.Add(New BO.OtherQueryItem(24, "Duplicitní klienti podle DIČ"))
                lis.Add(New BO.OtherQueryItem(25, "Má nadřízeného klienta"))
                lis.Add(New BO.OtherQueryItem(26, "Má pod sebou podřízené klienty"))
                lis.Add(New BO.OtherQueryItem(27, "Nastavena režijní fakturační přirážka"))
                lis.Add(New BO.OtherQueryItem(29, "Vyplněna fakturační poznámka klienta"))
                lis.Add(New BO.OtherQueryItem(30, "Zařazen do monitoringu insolvence"))
            Case BO.x29IdEnum.j02Person
                lis.Add(New BO.OtherQueryItem(6, "Pouze interní osoby"))
                lis.Add(New BO.OtherQueryItem(7, "Pouze kontaktní osoby"))
                lis.Add(New BO.OtherQueryItem(3, "Existují rozpracované úkony, které čekají na schvalování"))
                lis.Add(New BO.OtherQueryItem(5, "Existují schválené úkony, které čekají na fakturaci"))
            Case BO.x29IdEnum.p91Invoice
                lis.Add(New BO.OtherQueryItem(3, "Ve splatnosti"))
                lis.Add(New BO.OtherQueryItem(4, "Neuhrazené po splatnosti"))
                lis.Add(New BO.OtherQueryItem(15, "Neuhrazené"))
                lis.Add(New BO.OtherQueryItem(7, "Svázané se zálohou"))
                lis.Add(New BO.OtherQueryItem(8, "Svázané s opravným dokladem"))
                lis.Add(New BO.OtherQueryItem(5, "Pouze DRAFT faktury"))
                lis.Add(New BO.OtherQueryItem(6, "Pouze faktury s oficiálním číslem"))
                lis.Add(New BO.OtherQueryItem(9, "Obsahuje haléřové zaokrouhlení"))
                lis.Add(New BO.OtherQueryItem(10, "Obsahuje základ se základní sazbou DPH"))
                lis.Add(New BO.OtherQueryItem(11, "Obsahuje základ se sníženou sazbou DPH"))
                lis.Add(New BO.OtherQueryItem(12, "Obsahuje základ s nulovou sazbou DPH"))
                lis.Add(New BO.OtherQueryItem(13, "Obsahuje přepočet podle měnového kurzu"))
                lis.Add(New BO.OtherQueryItem(14, "Nastavena režijní fakturační přirážka"))
            Case BO.x29IdEnum.p31Worksheet
                lis.Add(New BO.OtherQueryItem(1, "Rozpracovanost, čeká na schvalování"))
                lis.Add(New BO.OtherQueryItem(2, "Schváleno, čeká na fakturaci"))
                lis.Add(New BO.OtherQueryItem(3, "Vyfakturováno"))
                lis.Add(New BO.OtherQueryItem(4, "Přesunuto do archivu"))
                lis.Add(New BO.OtherQueryItem(6, "Přiřazená kontaktní osoba"))
                lis.Add(New BO.OtherQueryItem(7, "Přiřazen dokument"))
                lis.Add(New BO.OtherQueryItem(8, "Vyplněná výchozí korekce úkonu"))
                lis.Add(New BO.OtherQueryItem(9, "Přiřazen úkol"))
                lis.Add(New BO.OtherQueryItem(10, "Přiřazen dodavatel"))
                lis.Add(New BO.OtherQueryItem(11, "Vazba na rozpočet"))
                lis.Add(New BO.OtherQueryItem(12, "Vyplněn kód dokladu"))
                lis.Add(New BO.OtherQueryItem(13, "vygenerováno automaticky robotem"))
            Case BO.x29IdEnum.p56Task
                lis.Add(New BO.OtherQueryItem(3, "Obsahují rozpracované úkony, které čekají na schvalování"))
                lis.Add(New BO.OtherQueryItem(5, "Obsahují schválené úkony, které čekají na fakturaci"))
                lis.Add(New BO.OtherQueryItem(6, "Vyplněn termín dokončení úkolu"))
                lis.Add(New BO.OtherQueryItem(7, "Je po termínu dokončení úkolu"))
                lis.Add(New BO.OtherQueryItem(8, "Vyplněno plánované zahájení úkolu"))
                lis.Add(New BO.OtherQueryItem(9, "Vyplněn plán/limit hodin"))
                lis.Add(New BO.OtherQueryItem(10, "Vyplněn plán/limit výdajů"))
                lis.Add(New BO.OtherQueryItem(11, "Došlo k překročení plánu/limitu hodin"))
                lis.Add(New BO.OtherQueryItem(12, "Došlo k překročení plánu/limitu výdajů"))
                lis.Add(New BO.OtherQueryItem(13, "Je matkou opakovaných úkolů"))
            Case BO.x29IdEnum.o23Doc
                lis.Add(New BO.OtherQueryItem(3, "Dokument byl svázán s projektem"))
                lis.Add(New BO.OtherQueryItem(4, "Dokument čeká na vazbu s projektem"))
                lis.Add(New BO.OtherQueryItem(5, "Dokument byl svázán s klientem"))
                lis.Add(New BO.OtherQueryItem(6, "Dokument čeká na vazbu s klientem"))
                lis.Add(New BO.OtherQueryItem(7, "Dokument byl svázán s fakturou"))
                lis.Add(New BO.OtherQueryItem(8, "Dokument čeká na vazbu s fakturou"))
                lis.Add(New BO.OtherQueryItem(9, "Dokument byl svázán s worksheet úkonem"))
                lis.Add(New BO.OtherQueryItem(10, "Dokument čeká na vazbu s úkonem"))
                lis.Add(New BO.OtherQueryItem(11, "Dokument byl svázán s osobou"))
                lis.Add(New BO.OtherQueryItem(12, "Dokument čeká na vazbu s osobou"))
        End Select
        Return lis
    End Function

    Public Function GetSqlWhere(intJ70ID As Integer) As String Implements Ij70QueryTemplateBL.GetSqlWhere
        Return _cDL.GetSqlWhere(intJ70ID)
    End Function

    Public Function CheckDefaultTemplate(x29id As BO.x29IdEnum, intJ03ID As Integer, Optional strMasterPrefix As String = "") As Boolean Implements Ij70QueryTemplateBL.CheckDefaultTemplate
        If Not _cDL.LoadSystemTemplate(x29id, intJ03ID, strMasterPrefix) Is Nothing Then Return True 'systém šablona již existuje

        Dim c As New BO.j70QueryTemplate
        c.x29ID = x29id
        c.j70IsSystem = True
        c.j70Name = BL.My.Resources.common.VychoziDatovyPrehled
        c.j70MasterPrefix = strMasterPrefix
        c.j70ScrollingFlag = BO.j70ScrollingFlagENUM.NoScrolling

        If c.j70MasterPrefix = "" Or (x29id = BO.x29IdEnum.p31Worksheet And c.j70MasterPrefix = "p31_grid") Or c.j70MasterPrefix = "p31_framework" Then
            c.j70IsFilteringByColumn = True 'pro hlavní přehledy nahodit sloupcový auto-filter
            c.j70ScrollingFlag = BO.j70ScrollingFlagENUM.StaticHeaders    'pro hlavní přehledy nastavit ukotvení záhlaví
        End If
        If c.j70MasterPrefix <> "" Then
            c.j70IsFilteringByColumn = False
        End If
        If c.j70MasterPrefix = "approving_step3" Then
            c.j70ScrollingFlag = BO.j70ScrollingFlagENUM.StaticHeaders  'schvalování úkonů
        End If

        Select Case x29id
            Case BO.x29IdEnum.p31Worksheet
                Select Case strMasterPrefix
                    Case "j02", "j02-p31"
                        c.j70Name = String.Format(BL.My.Resources.common.VychoziPrehledOsoby, "osoby")
                        c.j70ColumnNames = "p31Date,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "j02-approved"
                        c.j70Name = "Schválené úkony osoby"
                        c.j70ColumnNames = "p31Date,ClientName,p41Name,p32Name,p31Hours_Orig,p31Amount_WithoutVat_Orig,p31Hours_Approved_Billing,p31Amount_WithoutVat_Approved,p31Text"
                    Case "j02-time"
                        c.j70Name = "Hodiny osoby"
                        c.j70ColumnNames = "p31Date,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "j02-expense", "j02-fee"
                        c.j70Name = "Peněžní úkony osoby"
                        c.j70ColumnNames = "p31Date,ClientName,p41Name,p32Name,p31Amount_WithoutVat_Orig,p31VatRate_Orig,p31Amount_WithVat_Orig,p31Text"
                    Case "j02-kusovnik"
                        c.j70Name = "Kusovníkové úkony osoby"
                        c.j70ColumnNames = "p31Date,ClientName,p41Name,p32Name,p31Value_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p28", "p28-p31"
                        c.j70Name = My.Resources.common.VychoziPrehledKlienta
                        c.j70ColumnNames = "p31Date,Person,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p28-approved"
                        c.j70Name = "Schválené úkony klienta"
                        c.j70ColumnNames = "p31Date,Person,p41Name,p32Name,p31Hours_Orig,p31Amount_WithoutVat_Orig,p31Hours_Approved_Billing,p31Rate_Billing_Approved,p31Amount_WithoutVat_Approved,p31Text"
                    Case "p28-time"
                        c.j70Name = "Hodiny klienta"
                        c.j70ColumnNames = "p31Date,Person,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p28-expense", "p28-fee"
                        c.j70Name = "Peněžní úkony klienta"
                        c.j70ColumnNames = "p31Date,p41Name,p32Name,p31Amount_WithoutVat_Orig,p31VatRate_Orig,p31Amount_WithVat_Orig,p31Text"
                    Case "p28-kusovnik"
                        c.j70Name = "Kusovníkové úkony klienta"
                        c.j70ColumnNames = "p31Date,p41Name,p32Name,p31Value_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p41", "p41-p31"
                        c.j70Name = My.Resources.common.VychoziPrehledProjektu
                        c.j70ColumnNames = "p31Date,Person,p34Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p41-approved"
                        c.j70Name = "Schválené úkony projektu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Hours_Orig,p31Amount_WithoutVat_Orig,p31Hours_Approved_Billing,p31Rate_Billing_Approved,p31Amount_WithoutVat_Approved,p31Text"
                    Case "p41-time"
                        c.j70Name = "Hodiny projektu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p41-expense", "p41-fee"
                        c.j70Name = "Peněžní úkonu projektu"
                        c.j70ColumnNames = "p31Date,Person,p34Name,p32Name,p31Amount_WithoutVat_Orig,p31VatRate_Orig,p31Amount_WithVat_Orig,p31Text"
                    Case "p41-kusovnik"
                        c.j70Name = "Kusovníkové úkony projektu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Value_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p56", "p56-p31"
                        c.j70Name = "Úkony vybraného úkolu"
                        c.j70ColumnNames = "p31Date,Person,p34Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p56-approved"

                        c.j70Name = "Schválené úkony úkolu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Hours_Orig,p31Amount_WithoutVat_Orig,p31Hours_Approved_Billing,p31Rate_Billing_Approved,p31Amount_WithoutVat_Approved,p31Text"

                    Case "p56-time"
                        c.j70Name = "Hodiny v úkolu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "p56-expense", "p56-fee"
                        c.j70Name = "Peněžní úkony v úkolu"
                        c.j70ColumnNames = "p31Date,Person,p34Name,p32Name,p31Amount_WithoutVat_Orig,p31VatRate_Orig,p31Amount_WithVat_Orig,p31Text"
                    Case "p56-kusovnik"
                        c.j70Name = "Kusovníkové úkony v úkolu"
                        c.j70ColumnNames = "p31Date,Person,p32Name,p31Value_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"

                    Case "p91"
                        c.j70Name = My.Resources.common.VychoziPrehledFaktury
                        c.j70ColumnNames = "p31Date,Person,p41Name,p32Name,p31Hours_Invoiced,p31Rate_Billing_Invoiced,p31Amount_WithoutVat_Invoiced,p31VatRate_Invoiced,p31Amount_WithVat_Invoiced,p31Text"
                    Case "approving_step3"  'schvalovací rozhraní
                        c.j70Name = "Rozhraní pro schvalování úkonů | Příprava k fakturaci"
                        c.j70ColumnNames = "p31Date,Person,p41Name,p32Name,p31Hours_Orig,p31Hours_Approved_Billing,p31Rate_Billing_Orig,p31Amount_WithoutVat_Approved,p31Text"
                    Case "p31_grid"
                        c.j70Name = My.Resources.common.VychoziPrehled
                        c.j70ColumnNames = "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                    Case "mobile_grid"
                        c.j70ColumnNames = "p31Date,Person,p41Name,p31Value_Orig,p31Text"
                    Case Else
                        c.j70ColumnNames = "p31Date,Person,ClientName,p41Name,p32Name,p31Hours_Orig,p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig,p31Text"
                End Select

            Case BO.x29IdEnum.p41Project
                Select Case strMasterPrefix
                    Case "p31_framework"
                        c.j70Name = My.Resources.common.VychoziPrehledZapisovaniUkonu
                    Case "mobile_grid"
                        c.j70ColumnNames = "FullName"
                    Case "p28"
                        c.j70Name = "Projekty klienta"
                        c.j70ColumnNames = "p41Name,p41Code,p42Name"    'projekty v záložce pod klientem
                    Case "p41"
                        c.j70Name = "Pod-projekty"
                        c.j70ColumnNames = "p41Code,p41Name,p42Name"    'podřízené projekty
                End Select
                If c.j70ColumnNames = "" Then c.j70ColumnNames = "Client,p41Name"
            Case BO.x29IdEnum.p28Contact
                c.j70ColumnNames = "p28Name"
            Case BO.x29IdEnum.p91Invoice

                Select Case strMasterPrefix
                    Case "p41"
                        c.j70Name = "Výchozí přehled v detailu projektu"
                        c.j70ColumnNames = "p91Code,p91DateSupply,p91Amount_WithoutVat,p91Amount_Debt"
                    Case "p28"
                        c.j70Name = "Výchozí přehled v detailu klienta"
                        c.j70ColumnNames = "p91Code,p91DateSupply,p91Amount_WithoutVat,p91Amount_Debt"
                    Case "j02"
                        c.j70Name = "Výchozí přehled v detailu osoby"
                        c.j70ColumnNames = "p91Code,p28Name,p91DateSupply,p91Amount_WithoutVat,p91Amount_Debt"
                    Case "mobile_grid"
                        c.j70ColumnNames = "p91Code,p91Client,p91Amount_WithoutVat"
                    Case Else
                        c.j70ColumnNames = "p91Code,p28Name,p91Amount_WithoutVat,p91Amount_Debt"
                End Select
            Case BO.x29IdEnum.j02Person
                c.j70ColumnNames = "FullNameDesc"
            Case BO.x29IdEnum.p56Task
                Select Case strMasterPrefix
                    Case "j02"
                        c.j70Name = My.Resources.common.VychoziPrehledOsoby
                        c.j70ColumnNames = "p57Name,Client,p41Name,p56Name,p56PlanUntil,ReceiversInLine,Hours_Orig,Owner"
                    Case "p28"
                        c.j70Name = My.Resources.common.VychoziPrehledKlienta
                        c.j70ColumnNames = "p57Name,p56Name,p56PlanUntil,ReceiversInLine,Hours_Orig"
                    Case "p41"
                        c.j70Name = My.Resources.common.VychoziPrehled
                        c.j70ColumnNames = "p57Name,p56Name,b02Name,ReceiversInLine,Hours_Orig"
                    Case "p31_framework"
                        c.j70Name = My.Resources.common.VychoziPrehledZapisovaniUkonu
                        c.j70ColumnNames = "p56Name"
                    Case "mobile_grid"
                        c.j70Name = My.Resources.common.VychoziPrehled
                        c.j70ColumnNames = "p56Name"
                    Case Else
                        c.j70ColumnNames = "ClientAndProject,p56Name,ReceiversInLine,b02Name"
                End Select
            Case BO.x29IdEnum.o23Doc
                Select Case strMasterPrefix
                    Case "p41"
                        c.j70ColumnNames = "x18Name,o23Name"
                    Case "mobile_grid"
                        c.j70ColumnNames = "x18Name,o23Name"
                    Case Else
                        c.j70ColumnNames = "x18Name,x23Code,o23Name"
                End Select

        End Select
        c.j03ID = intJ03ID
        Return Save(c, Nothing, Nothing)
    End Function


    Public Function GetColumns(x29id As BO.x29IdEnum) As List(Of BO.GridColumn) Implements Ij70QueryTemplateBL.ColumnsPallete
        _x29id = x29id
        Dim lis As New List(Of BO.GridColumn)

        Select Case _x29id
            Case BO.x29IdEnum.p31Worksheet
                InhaleP31ColList(lis)
            Case BO.x29IdEnum.p41Project
                InhaleP41ColList(lis)
            Case BO.x29IdEnum.p28Contact
                InhaleP28ColList(lis)
            Case BO.x29IdEnum.p91Invoice
                InhaleP91ColList(lis)
            Case BO.x29IdEnum.j02Person
                InhaleJ02ColList(lis)
            Case BO.x29IdEnum.p56Task
                InhaleP56ColList(lis)
            Case BO.x29IdEnum.o23Doc
                InhaleO23ColList(lis)
        End Select

        Return lis


    End Function

    Private Sub AppendRoles(x29id As BO.x29IdEnum, strRefField As String, strTreeGroup As String, ByRef lis As List(Of BO.GridColumn))
        Dim mq As New BO.myQuery

        If _lisX67 Is Nothing Then _lisX67 = Factory.x67EntityRoleBL.GetList(mq)
        Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = _lisX67.Where(Function(p) p.x29ID = x29id)

        For Each c In lisX67
            Select Case x29id
                Case BO.x29IdEnum.p28Contact
                    lis.Add(AGC(c.x67Name, "Role_x67_" & c.PID.ToString, BO.cfENUM.AnyString, , "dbo.p28_getonerole_inline(" & strRefField & "," & c.PID.ToString & ")", , , strTreeGroup))
                Case BO.x29IdEnum.p41Project
                    lis.Add(AGC(c.x67Name, "Role_x67_" & c.PID.ToString, BO.cfENUM.AnyString, , "dbo.p41_get_one_role_inline(" & strRefField & "," & c.PID.ToString & ")", , , strTreeGroup))
                Case BO.x29IdEnum.p56Task
                    lis.Add(AGC(c.x67Name, "Role_x67_" & c.PID.ToString, BO.cfENUM.AnyString, , "dbo.p56_get_one_role_inline(" & strRefField & "," & c.PID.ToString & ")", , , strTreeGroup))
            End Select
        Next
      
    End Sub
    Private Sub AppendFreeFields(x29id As BO.x29IdEnum, ByRef lis As List(Of BO.GridColumn))
        Dim lisX28 As IEnumerable(Of BO.x28EntityField) = Factory.x28EntityFieldBL.GetList(x29id, -1, True)
        For Each c In lisX28
            If c.x28Flag = BO.x28FlagENUM.UserField Then
                If c.x23ID = 0 Then
                    Select Case c.x24ID
                        Case BO.x24IdENUM.tString
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.AnyString, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                        Case BO.x24IdENUM.tDate
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.DateOnly, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                        Case BO.x24IdENUM.tDateTime
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.DateTime, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                        Case BO.x24IdENUM.tDecimal
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.Numeric, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                        Case BO.x24IdENUM.tInteger
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.Numeric0, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                        Case BO.x24IdENUM.tBoolean
                            lis.Add(AGC(c.x28Name, c.x28Field, BO.cfENUM.Checkbox, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                    End Select
                Else
                    'text combo položky
                    lis.Add(AGC(c.x28Name, c.x28Field & "Text", BO.cfENUM.AnyString, , , , , "Uživatelské pole", c.x28Pivot_SelectSql, c.x28Pivot_GroupBySql))
                End If
            End If
            If c.x28Flag = BO.x28FlagENUM.GridField Then
                Dim col As New BO.GridColumn(c.x29ID, c.x28Name, c.x28Grid_Field)
                col.ColumnDBName = c.x28Grid_SqlSyntax
                col.SqlSyntax_FROM = c.x28Grid_SqlFrom
                Select Case c.x24ID
                    Case BO.x24IdENUM.tDate : col.ColumnType = BO.cfENUM.DateOnly
                    Case BO.x24IdENUM.tDateTime : col.ColumnType = BO.cfENUM.DateTime
                    Case BO.x24IdENUM.tDecimal : col.ColumnType = BO.cfENUM.Numeric
                    Case BO.x24IdENUM.tInteger : col.ColumnType = BO.cfENUM.Numeric0
                    Case BO.x24IdENUM.tBoolean : col.ColumnType = BO.cfENUM.Checkbox
                End Select
                col.Pivot_SelectSql = c.x28Pivot_SelectSql
                col.Pivot_GroupBySql = c.x28Pivot_GroupBySql
                lis.Add(col)
            End If
        Next
        Dim lisX18 As IEnumerable(Of BO.x18EntityCategory) = Factory.x18EntityCategoryBL.GetList(, x29id, -1).Where(Function(p) p.x18IsManyItems = False), x As Integer = 0
        For Each c In lisX18
            x += 1
            Dim strSql As String = "dbo.stitek_hodnoty(" & c.PID.ToString & "," & CInt(x29id).ToString & ",a." & BO.BAS.GetDataPrefix(x29id) & "ID)"
            lis.Add(AGC(Left(c.x18Name, 20), "tag" & x.ToString & "_" & c.PID.ToString, BO.cfENUM.AnyString, , strSql, , , "Kategorie (typ dokumentu)", , ))
        Next
        Select Case x29id
            Case BO.x29IdEnum.p41Project
                lisX18 = Factory.x18EntityCategoryBL.GetList(, BO.x29IdEnum.p28Contact, -1).Where(Function(p) p.x18IsManyItems = False)
                For Each c In lisX18
                    x += 1
                    Dim strSql As String = "dbo.stitek_hodnoty(" & c.PID.ToString & ",328,a.p28ID_Client)"
                    lis.Add(AGC(Left(c.x18Name, 20), "tag" & x.ToString & "_" & c.PID.ToString, BO.cfENUM.AnyString, , strSql, , , "Kategorie klienta projektu", , ))
                Next
            Case BO.x29IdEnum.p31Worksheet
                lisX18 = Factory.x18EntityCategoryBL.GetList(, BO.x29IdEnum.p41Project, -1).Where(Function(p) p.x18IsManyItems = False)
                For Each c In lisX18
                    x += 1
                    Dim strSql As String = "dbo.stitek_hodnoty(" & c.PID.ToString & ",141,a.p41ID)"
                    lis.Add(AGC(Left(c.x18Name, 20), "tag" & x.ToString & "_" & c.PID.ToString, BO.cfENUM.AnyString, , strSql, , , "Kategorie projektu", , ))
                Next
                lisX18 = Factory.x18EntityCategoryBL.GetList(, BO.x29IdEnum.p28Contact, -1).Where(Function(p) p.x18IsManyItems = False)
                For Each c In lisX18
                    x += 1
                    Dim strSql As String = "dbo.stitek_hodnoty(" & c.PID.ToString & ",328,p41.p28ID_Client)"
                    lis.Add(AGC(Left(c.x18Name, 20), "tag" & x.ToString & "_" & c.PID.ToString, BO.cfENUM.AnyString, , strSql, , , "Kategorie klienta projektu", , ))
                Next
        End Select
    End Sub

    Private Sub InhaleP41ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC(My.Resources.common.NazevProjektu, "p41Name", , , "a.p41Name"))
            .Add(AGC(My.Resources.common.Kod, "p41Code", , , "a.p41Code"))

            .Add(AGC("DRAFT", "p41IsDraft", BO.cfENUM.Checkbox, , "a.p41IsDraft"))
            .Add(AGC(My.Resources.common.KlientProjektu, "Client", , , "p28client.p28Name"))
            .Add(AGC(My.Resources.common.KlientPlusProjekt, "FullName", , True, "isnull(p28client.p28Name+char(32),'')+a.p41Name"))
            .Add(AGC(My.Resources.common.Zkratka, "p41NameShort", , , "a.p41NameShort"))
            .Add(AGC(My.Resources.common.TypProjektu, "p42Name"))
            .Add(AGC(My.Resources.common.Stredisko, "j18Name"))
            .Add(AGC(My.Resources.common.FakturacniCenik, "p51Name_Billing", , , "p51billing.p51Name"))
            .Add(AGC(My.Resources.common.NakladovyCenik, "p51Name_Internal", , , "p51internal.p51Name", , "LEFT OUTER JOIN p51PriceList p51internal ON a.p51ID_Internal=p51internal.p51ID"))
            .Add(AGC(My.Resources.common.TypFaktury, "p92Name"))
            .Add(AGC("Fakturační poznámka", "p41BillingMemo", , , "a.p41BillingMemo"))
            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(141,a.p41ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(141,a.p41ID)"))
            .Add(AGC("Otevřené úkoly", "PendingTasks", , , "dbo.p41_get_tasks_inline_html(a.p41ID)"))

            .Add(AGC(My.Resources.common.PlanStart, "p41PlanFrom", BO.cfENUM.DateOnly, , "a.p41PlanFrom"))
            .Add(AGC(My.Resources.common.PlanEnd, "p41PlanUntil", BO.cfENUM.DateOnly, , "a.p41PlanUntil"))
            .Add(AGC(My.Resources.common.LimitHodin, "p41LimitHours_Notification", BO.cfENUM.Numeric, , "a.p41LimitHours_Notification", True))
            .Add(AGC(My.Resources.common.LimitniHonorar, "p41LimitFee_Notification", BO.cfENUM.Numeric, , "a.p41LimitFee_Notification", True))

            .Add(AGC("Odběratel faktury", "InvoiceClient", , , "p28billing.p28Name", , "LEFT OUTER JOIN p28Contact p28billing ON a.p28ID_Billing=p28billing.p28ID"))
            .Add(AGC("Klastr aktivit", "Cluster", , , "p61.p61Name", , "LEFT OUTER JOIN p61ActivityCluster p61 ON a.p61ID=p61.p61ID"))

            .Add(AGC("Stromový název", "p41TreePath", , True, "a.p41TreePath", , , "Strom"))
            .Add(AGC("Odkaz na pod-projekty", "ChildProjectsInline", , , "dbo.p41_get_childs_inline_html(a.p41ID)", , , "Strom"))
            .Add(AGC("Nadřízený projekt", "ParentProject", , True, "parent.ParentName", , "LEFT OUTER JOIN (select p41ID as ParentPID,isnull(p41NameShort,p41Name) as ParentName FROM p41Project) parent ON a.p41ParentID=parent.ParentPID", "Strom"))
            .Add(AGC("Strom index", "p41TreeIndex", , True, "a.p41TreeIndex", , , "Strom"))
            .Add(AGC("Strom level", "p41TreeLevel", , True, "a.p41TreeLevel", , , "Strom"))
            .Add(AGC(My.Resources.common.VlastnikZaznamu, "Owner", , , "j02owner.j02LastName+char(32)+j02owner.j02FirstName", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozeno, "p41DateInsert", BO.cfENUM.DateTime, , "a.p41DateInsert", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozil, "p41UserInsert", , , "a.p41UserInsert", , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizace, "p41DateUpdate", BO.cfENUM.DateTime, , "a.p41DateUpdate", , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizoval, "p41UserUpdate", , , "a.p41UserUpdate", , , "Záznam"))
            .Add(AGC(My.Resources.common.ExterniKod, "p41ExternalPID", , , "a.p41ExternalPID"))
            If Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                .Add(AGC("Vykázané hodiny", "Vykazano_Hodiny", BO.cfENUM.Numeric2, , "alfa.Vykazano_Hodiny", True, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vykázáno"))
                .Add(AGC("Vykázané výdaje", "Vykazano_Vydaje", BO.cfENUM.Numeric2, , "alfa.Vykazano_Vydaje", True, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vykázáno"))
                .Add(AGC("Vyfakturované hodiny", "Vyfakturovano_Hodiny", BO.cfENUM.Numeric2, , "alfa.Vyfakturovano_Hodiny", True, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vyfakturováno"))
                .Add(AGC("Vyfakturovaná částka", "Vyfakturovano_Castka", BO.cfENUM.Numeric2, , "alfa.Vyfakturovano_Celkem_Domestic", True, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vyfakturováno"))
                .Add(AGC("Faktura naposledy", "Vyfakturovano_Naposledy_Kdy", BO.cfENUM.DateOnly, , "alfa.Vyfakturovano_Naposledy_Kdy", False, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vyfakturováno"))
                .Add(AGC("Faktura první", "Vyfakturovano_Poprve_Kdy", BO.cfENUM.DateOnly, , "alfa.Vyfakturovano_Poprve_Kdy", False, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vyfakturováno"))
                .Add(AGC("Počet faktur", "Vyfakturovano_PocetFaktur", BO.cfENUM.Numeric0, , "alfa.Vyfakturovano_PocetFaktur", False, "LEFT OUTER JOIN tview_p41_worksheet(@dp31f1,@dp31f2) alfa ON a.p41ID=alfa.p41ID", "Vyfakturováno"))

                .Add(AGC("Rozpracované hodiny", "WIP_Hodiny", BO.cfENUM.Numeric2, , "beta.Hodiny", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracováno bez DPH", "WIP_Castka", BO.cfENUM.Numeric2, , "beta.Castka_Celkem", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracovanoý honorář", "WIP_Honorar", BO.cfENUM.Numeric2, , "beta.Honorar", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracovanoý honorář CZK", "WIP_Honorar_CZK", BO.cfENUM.Numeric2, , "beta.Honorar_CZK", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracovaný honorář EUR", "WIP_Honorar_EUR", BO.cfENUM.Numeric2, , "beta.Honorar_EUR", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje", "WIP_Vydaje", BO.cfENUM.Numeric2, , "beta.Vydaje", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje CZK", "WIP_Vydaje_CZK", BO.cfENUM.Numeric2, , "beta.Vydaje_CZK", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje EUR", "WIP_Vydaje_EUR", BO.cfENUM.Numeric2, , "beta.Vydaje_EUR", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály", "WIP_Odmeny", BO.cfENUM.Numeric2, , "beta.Odmeny", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály CZK", "WIP_Odmeny_CZK", BO.cfENUM.Numeric2, , "beta.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály EUR", "WIP_Odmeny_EUR", BO.cfENUM.Numeric2, , "beta.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p41_wip(@dp31f1,@dp31f2) beta ON a.p41ID=beta.p41ID", "Rozpracováno"))

                .Add(AGC("Nevyfakturované hodiny", "NI_Hodiny", BO.cfENUM.Numeric2, , "gama.Hodiny", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturováno bez DPH", "NI_Castka", BO.cfENUM.Numeric2, , "gama.Castka_Celkem", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honorář", "NI_Honorar", BO.cfENUM.Numeric2, , "gama.Honorar", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honorář CZK", "NI_Honorar_CZK", BO.cfENUM.Numeric2, , "gama.Honorar_CZK", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honodář EUR", "NI_Honorar_EUR", BO.cfENUM.Numeric2, , "gama.Honorar_EUR", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované výdaje", "NI_Vydaje", BO.cfENUM.Numeric2, , "gama.Vydaje", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály", "NI_Odmeny", BO.cfENUM.Numeric2, , "gama.Odmeny", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály CZK", "NI_Odmeny_CZK", BO.cfENUM.Numeric2, , "gama.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály EUR", "NI_Odmeny_EUR", BO.cfENUM.Numeric2, , "gama.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p41_notinvoiced(@dp31f1,@dp31f2) gama ON a.p41ID=gama.p41ID", "Nevyfakturováno"))

                .Add(AGC("Schválené hodiny", "AP_Hodiny", BO.cfENUM.Numeric2, , "omega.Hodiny", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schváleno bez DPH", "AP_Castka", BO.cfENUM.Numeric2, , "omega.Castka_Celkem", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválený honorář", "AP_Honorar", BO.cfENUM.Numeric2, , "omega.Honorar", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválený honorář CZK", "AP_Honorar_CZK", BO.cfENUM.Numeric2, , "omega.Honorar_CZK", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválený honorář EUR", "AP_Honorar_EUR", BO.cfENUM.Numeric2, , "omega.Honorar_EUR", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválené výdaje", "AP_Vydaje", BO.cfENUM.Numeric2, , "omega.Vydaje", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválené paušály", "AP_Odmeny", BO.cfENUM.Numeric2, , "omega.Odmeny", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválené paušály CZK", "AP_Odmeny_CZK", BO.cfENUM.Numeric2, , "omega.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))
                .Add(AGC("Schválené paušály EUR", "AP_Odmeny_EUR", BO.cfENUM.Numeric2, , "omega.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p41_approved(@dp31f1,@dp31f2) omega ON a.p41ID=omega.p41ID", "Schváleno"))

            End If
        End With
        AppendRoles(BO.x29IdEnum.p41Project, "a.p41ID", "Projektová a klientská role", lis)
        AppendRoles(BO.x29IdEnum.p28Contact, "a.p28ID_Client", "Projektová a klientská role", lis)
        AppendFreeFields(BO.x29IdEnum.p41Project, lis)

    End Sub
    Private Sub InhaleP28ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC(My.Resources.common.Nazev, "p28Name", , , "a.p28Name"))
            .Add(AGC(My.Resources.common.Spolecnost, "p28CompanyName", , , "a.p28CompanyName"))
            .Add(AGC(My.Resources.common.Kod, "p28Code", , , "a.p28Code"))
            .Add(AGC(My.Resources.common.KodDodavatele, "p28SupplierID", , , "a.p28SupplierID"))
            .Add(AGC(My.Resources.common.IC, "p28RegID", , , "a.p28RegID"))
            .Add(AGC(My.Resources.common.DIC, "p28VatID", , , "a.p28VatID"))
            .Add(AGC(My.Resources.common.Typ, "p29Name"))
            .Add(AGC("Fakturační poznámka", "p28BillingMemo", , , "a.p28BillingMemo"))
            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(328,a.p28ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(328,a.p28ID)"))


            ''.Add(AGC("Nadřízený klient", "ParentContact", , , "p28parent.p28Name", , "LEFT OUTER JOIN p28Contact p28parent ON a.p28ParentID=p28parent.p28ID"))
            .Add(AGC("Město", "Adress1_City", , , "pa.o38City", , "LEFT OUTER JOIN view_PrimaryAddress pa ON a.p28ID=pa.p28ID"))
            .Add(AGC("Ulice", "Adress1_Street", , , "pa.o38Street", , "LEFT OUTER JOIN view_PrimaryAddress pa ON a.p28ID=pa.p28ID"))
            .Add(AGC("PSČ", "Adress1_ZIP", , , "pa.o38ZIP", , "LEFT OUTER JOIN view_PrimaryAddress pa ON a.p28ID=pa.p28ID"))
            .Add(AGC("Stát", "Adress1_Country", , , "pa.o38Country", , "LEFT OUTER JOIN view_PrimaryAddress pa ON a.p28ID=pa.p28ID"))
            .Add(AGC(My.Resources.common.Zkratka, "p28CompanyShortName", , , "a.p28CompanyShortName"))
            .Add(AGC("Otevřené projekty", "OtevreneProjekty", BO.cfENUM.Numeric0, , "op.PocetOtevrenychProjektu", , "LEFT OUTER JOIN view_p28_projects op ON a.p28ID=op.p28ID"))
            .Add(AGC(My.Resources.common.FakturacniCenik, "p51Name_Billing", , , "p51billing.p51Name"))
            .Add(AGC(My.Resources.common.NakladovyCenik, "p51Name_Internal", , , "p51internal.p51Name"))
            .Add(AGC(My.Resources.common.TypFaktury, "p92Name"))
            .Add(AGC("Fakturační jazyk", "p87Name"))

            .Add(AGC(My.Resources.common.LimitHodin, "p28LimitHours_Notification", BO.cfENUM.Numeric, , "a.p28LimitHours_Notification", True))
            .Add(AGC(My.Resources.common.LimitniHonorar, "p28LimitFee_Notification", BO.cfENUM.Numeric, , "a.p28LimitFee_Notification", True))

            .Add(AGC(My.Resources.common.VlastnikZaznamu, "Owner", , , "j02owner.j02LastName+char(32)+j02owner.j02FirstName", , , "Záznam"))
            .Add(AGC("Kontaktní média", "KontaktniMedia", , , "dbo.p28_medias_inline(a.p28ID)"))
            .Add(AGC("Fakturační e-mail", "FakturacniEmail", , , "dbo.p28_medias_inline_invoice(a.p28ID)"))
            .Add(AGC("Kontaktní osoby", "KontaktniOsoby", , , "dbo.p28_ko_inline(a.p28ID)"))

            '.Add(AGC("1.kontaktní osoba", "KontaktniOsoba", , , "ko.Person", , "LEFT OUTER JOIN view_p28_contactpersons ko ON a.p28ID=ko.p28ID"))

            .Add(AGC("Fakt.kontaktní osoba", "FakturacniKontaktniOsoba", , , "fko.Person", , "LEFT OUTER JOIN view_p28_contactpersons_invoice fko ON a.p28ID=fko.p28ID"))
            .Add(AGC("Stromový název", "p28TreePath", , True, "a.p28TreePath", , , "Strom"))
            .Add(AGC("Odkaz na podřízené klienty", "ChildContactsInline", , , "dbo.p28_get_childs_inline_html(a.p28ID)", , , "Strom"))
            .Add(AGC("Strom index", "p28TreeIndex", , True, "a.p28TreeIndex", , , "Strom"))
            .Add(AGC("Strom level", "p28TreeLevel", , True, "a.p28TreeLevel", , , "Strom"))
            .Add(AGC(My.Resources.common.Zalozeno, "p28DateInsert", BO.cfENUM.DateTime, , "a.p28DateInsert", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozil, "p28UserInsert", , , "a.p28UserInsert", , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizace, "p28DateUpdate", BO.cfENUM.DateTime, , "a.p28DateUpdate", , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizoval, "p28UserUpdate", , , "a.p28UserUpdate", , , "Záznam"))
            .Add(AGC("Externí kód", "p28ExternalPID", , , "a.p28ExternalPID"))

            If Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                .Add(AGC("Vykázané hodiny", "Vykazano_Hodiny", BO.cfENUM.Numeric2, , "alfa.Vykazano_Hodiny", True, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vykázáno"))
                .Add(AGC("Vykázané výdaje", "Vykazano_Vydaje", BO.cfENUM.Numeric2, , "alfa.Vykazano_Vydaje", True, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vykázáno"))
                .Add(AGC("Vyfakturované hodiny", "Vyfakturovano_Hodiny", BO.cfENUM.Numeric2, , "alfa.Vyfakturovano_Hodiny", True, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vyfakturováno"))
                .Add(AGC("Vyfakturovaná částka", "Vyfakturovano_Castka", BO.cfENUM.Numeric2, , "alfa.Vyfakturovano_Celkem_Domestic", True, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vyfakturováno"))
                .Add(AGC("Faktura naposledy", "Vyfakturovano_Naposledy_Kdy", BO.cfENUM.DateOnly, , "alfa.Vyfakturovano_Naposledy_Kdy", False, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vyfakturováno"))
                .Add(AGC("Faktura poprvé", "Vyfakturovano_Poprve_Kdy", BO.cfENUM.DateOnly, , "alfa.Vyfakturovano_Poprve_Kdy", False, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vyfakturováno"))
                .Add(AGC("Počet faktur", "Vyfakturovano_PocetFaktur", BO.cfENUM.Numeric0, , "alfa.Vyfakturovano_PocetFaktur", False, "LEFT OUTER JOIN tview_p28_worksheet(@dp31f1,@dp31f2) alfa ON a.p28ID=alfa.p28ID", "Vyfakturováno"))

                .Add(AGC("Rozpracované hodiny", "WIP_Hodiny", BO.cfENUM.Numeric2, , "beta.Hodiny", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracováno bez DPH", "WIP_Castka", BO.cfENUM.Numeric2, , "beta.Castka_Celkem", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpraoovaný honorář", "WIP_Honorar", BO.cfENUM.Numeric2, , "beta.Honorar", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracovaný honorář CZK", "WIP_Honorar_CZK", BO.cfENUM.Numeric2, , "beta.Honorar_CZK", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracovaný honorář EUR", "WIP_Honorar_EUR", BO.cfENUM.Numeric2, , "beta.Honorar_EUR", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje", "WIP_Vydaje", BO.cfENUM.Numeric2, , "beta.Vydaje", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje CZK", "WIP_Vydaje_CZK", BO.cfENUM.Numeric2, , "beta.Vydaje_CZK", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované výdaje EUR", "WIP_Vydaje_EUR", BO.cfENUM.Numeric2, , "beta.Vydaje_EUR", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály", "WIP_Odmeny", BO.cfENUM.Numeric2, , "beta.Odmeny", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály CZK", "WIP_Odmeny_CZK", BO.cfENUM.Numeric2, , "beta.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))
                .Add(AGC("Rozpracované paušály EUR", "WIP_Odmeny_EUR", BO.cfENUM.Numeric2, , "beta.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p28_wip(@dp31f1,@dp31f2) beta ON a.p28ID=beta.p28ID", "Rozpracováno"))


                .Add(AGC("Nevyfakturované hodiny", "NI_Hodiny", BO.cfENUM.Numeric2, , "gama.Hodiny", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturováno bez DPH", "NI_Castka", BO.cfENUM.Numeric2, , "gama.Castka_Celkem", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honorář", "NI_Honorar", BO.cfENUM.Numeric2, , "gama.Honorar", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honorář CZK", "NI_Honorar_CZK", BO.cfENUM.Numeric2, , "gama.Honorar_CZK", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturovaný honodář EUR", "NI_Honorar_EUR", BO.cfENUM.Numeric2, , "gama.Honorar_EUR", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované výdaje", "NI_Vydaje", BO.cfENUM.Numeric2, , "gama.Vydaje", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály", "NI_Odmeny", BO.cfENUM.Numeric2, , "gama.Odmeny", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály CZK", "NI_Odmeny_CZK", BO.cfENUM.Numeric2, , "gama.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))
                .Add(AGC("Nevyfakturované paušály EUR", "NI_Odmeny_EUR", BO.cfENUM.Numeric2, , "gama.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p28_notinvoiced(@dp31f1,@dp31f2) gama ON a.p28ID=gama.p28ID", "Nevyfakturováno"))

                .Add(AGC("Schválené hodiny", "AP_Hodiny", BO.cfENUM.Numeric2, , "omega.Hodiny", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schváleno bez DPH", "AP_Castka", BO.cfENUM.Numeric2, , "omega.Castka_Celkem", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválený honorář", "AP_Honorar", BO.cfENUM.Numeric2, , "omega.Honorar", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválený honorář CZK", "AP_Honorar_CZK", BO.cfENUM.Numeric2, , "omega.Honorar_CZK", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválený honorář EUR", "AP_Honorar_EUR", BO.cfENUM.Numeric2, , "omega.Honorar_EUR", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválené výdaje", "AP_Vydaje", BO.cfENUM.Numeric2, , "omega.Vydaje", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválené paušály", "AP_Odmeny", BO.cfENUM.Numeric2, , "omega.Odmeny", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválené paušály CZK", "AP_Odmeny_CZK", BO.cfENUM.Numeric2, , "omega.Odmeny_CZK", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))
                .Add(AGC("Schválené paušály EUR", "AP_Odmeny_EUR", BO.cfENUM.Numeric2, , "omega.Odmeny_EUR", True, "LEFT OUTER JOIN tview_p28_approved(@dp31f1,@dp31f2) omega ON a.p28ID=omega.p28ID", "Schváleno"))

            End If
        End With
        AppendRoles(BO.x29IdEnum.p28Contact, "a.p28ID", "Klientské role", lis)
        AppendFreeFields(BO.x29IdEnum.p28Contact, lis)
    End Sub
    Private Sub InhaleJ02ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC("Příjmení+jméno", "FullNameDesc", , , "a.j02LastName+char(32)+a.j02FirstName"))
            .Add(AGC("Jméno+příjmení", "FullNameAsc", , , "a.j02FirstName+char(32)+a.j02LastName"))
            .Add(AGC("Jméno", "j02FirstName"))
            .Add(AGC("Příjmení", "j02LastName"))
            .Add(AGC("Titul", "j02TitleBeforeName"))
            .Add(AGC("E-mail", "j02Email"))


            .Add(AGC("Pozice", "j07Name"))
            .Add(AGC("Osobní číslo (kód)", "j02Code"))
            .Add(AGC("Fond", "c21Name"))
            .Add(AGC("Středisko", "j18Name"))

            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(102,a.j02ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(102,a.j02ID)"))

            .Add(AGC("Mobil", "j02Mobile"))
            .Add(AGC("Pevná", "j02Phone"))
            .Add(AGC("Oslovení", "j02Salutation"))
            .Add(AGC("Kancelář", "j02Office"))

            
            .Add(AGC("Pozice KO", "j02JobTitle", , , , , , "Kontaktní osoba klienta/projektu"))
            .Add(AGC("Klient vazba", "VazbaKlient", , , "dbo.j02_clients_inline(a.j02ID)", , , "Kontaktní osoba klienta/projektu"))
            .Add(AGC("Fakturační adresa?", "j02IsInvoiceEmail", BO.cfENUM.Checkbox, , , , , "Kontaktní osoba klienta/projektu"))

            .Add(AGC("Interní osoba", "j02IsIntraPerson", BO.cfENUM.Checkbox))
            .Add(AGC("Založeno", "j02DateInsert", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC("Založil", "j02UserInsert", , , , , , "Záznam"))
            .Add(AGC("Aktualizace", "j02DateUpdate", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC("Aktualizoval", "j02UserUpdate", , , , , , "Záznam"))
            .Add(AGC("Externí kód", "j02ExternalPID"))

            .Add(AGC("Rozpracované hodiny", "WIP_Hodiny", BO.cfENUM.Numeric2, , "beta.Hodiny", True, "LEFT OUTER JOIN tview_j02_wip(@dp31f1,@dp31f2) beta ON a.j02ID=beta.j02ID", "Rozpracováno"))
            .Add(AGC("Rozpraoovaný honorář", "WIP_Honorar", BO.cfENUM.Numeric2, , "beta.Honorar", True, "LEFT OUTER JOIN tview_j02_wip(@dp31f1,@dp31f2) beta ON a.j02ID=beta.j02ID", "Rozpracováno"))
            .Add(AGC("Rozpracovaný honorář CZK", "WIP_Honorar_CZK", BO.cfENUM.Numeric2, , "beta.Honorar_CZK", True, "LEFT OUTER JOIN tview_j02_wip(@dp31f1,@dp31f2) beta ON a.j02ID=beta.j02ID", "Rozpracováno"))
            .Add(AGC("Rozpracovaný honorář EUR", "WIP_Honorar_EUR", BO.cfENUM.Numeric2, , "beta.Honorar_EUR", True, "LEFT OUTER JOIN tview_j02_wip(@dp31f1,@dp31f2) beta ON a.j02ID=beta.j02ID", "Rozpracováno"))

            .Add(AGC("Nevyfakturované hodiny", "NI_Hodiny", BO.cfENUM.Numeric2, , "gama.Hodiny", True, "LEFT OUTER JOIN tview_j02_notinvoiced(@dp31f1,@dp31f2) gama ON a.j02ID=gama.j02ID", "Nevyfakturováno"))
            .Add(AGC("Nevyfakturovaný honorář", "NI_Honorar", BO.cfENUM.Numeric2, , "gama.Honorar", True, "LEFT OUTER JOIN tview_j02_notinvoiced(@dp31f1,@dp31f2) gama ON a.j02ID=gama.j02ID", "Nevyfakturováno"))
            .Add(AGC("Nevyfakturovaný honorář CZK", "NI_Honorar_CZK", BO.cfENUM.Numeric2, , "gama.Honorar_CZK", True, "LEFT OUTER JOIN tview_j02_notinvoiced(@dp31f1,@dp31f2) gama ON a.j02ID=gama.j02ID", "Nevyfakturováno"))
            .Add(AGC("Nevyfakturovaný honodář EUR", "NI_Honorar_EUR", BO.cfENUM.Numeric2, , "gama.Honorar_EUR", True, "LEFT OUTER JOIN tview_j02_notinvoiced(@dp31f1,@dp31f2) gama ON a.j02ID=gama.j02ID", "Nevyfakturováno"))

        End With
        AppendFreeFields(BO.x29IdEnum.j02Person, lis)
    End Sub
    Private Sub InhaleP31ColList(ByRef lis As List(Of BO.GridColumn))
        Dim bolHideRatesColumns As Boolean = Not Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates)   'zda uživatel nemá právo vidět sazby a fakturační údaje


        With lis
            .Add(AGC(My.Resources.common.Datum, "p31Date", BO.cfENUM.DateOnly, , , , , "Datum a čas", "convert(varchar(10), a.p31Date, 126)", "convert(varchar(10), a.p31Date, 126)"))
            .Add(AGC("Rok", "Rok", BO.cfENUM.Numeric0, , "Year(a.p31date)", , , "Datum a čas", "Year(a.p31date)", "Year(a.p31date)"))
            .Add(AGC("Měsíc", "Mesic", , , "convert(varchar(7),a.p31Date,126)", , , "Datum a čas", "convert(varchar(7),a.p31Date,126)", "convert(varchar(7),a.p31Date,126)"))
            .Add(AGC("Týden", "Tyden", , , "convert(varchar(4),year(a.p31Date))+'-'+convert(varchar(10),DATEPART(week,a.p31Date))", , , "Datum a čas", "convert(varchar(4),year(a.p31Date))+'-'+convert(varchar(10),DATEPART(week,a.p31Date))", "convert(varchar(4),year(a.p31Date))+'-'+convert(varchar(10),DATEPART(week,a.p31Date))"))
            .Add(AGC("Rok faktury", "RokFaktury", BO.cfENUM.Numeric0, , "Year(p91.p91DateSupply)", , , "Datum a čas", "Year(p91.p91DateSupply)", "Year(p91.p91DateSupply)"))
            .Add(AGC("Měsíc faktury", "MesicFaktury", , , "convert(varchar(7),p91.p91DateSupply,126)", , , "Datum a čas", "convert(varchar(7),p91.p91DateSupply,126)", "convert(varchar(7),p91.p91DateSupply,126)"))

            .Add(AGC(My.Resources.common.TimeFrom, "TimeFrom", , False, "p31DateTimeFrom_Orig", , , "Datum a čas"))
            .Add(AGC(My.Resources.common.TimeUntil, "TimeUntil", , False, "p31DateTimeUntil_Orig", , , "Datum a čas"))

            .Add(AGC("Jméno", "Person", , , "j02.j02LastName+char(32)+j02.j02FirstName", , , "Osoba", "min(j02.j02LastName+char(32)+j02.j02FirstName)", "a.j02ID"))
            .Add(AGC("Pozice osoby", "PoziceOsoby", , True, "j07.j07Name", , "LEFT OUTER JOIN j07PersonPosition j07 ON j02.j07ID=j07.j07ID", "Osoba", "min(j07.j07Name)", "j02.j07ID"))
            .Add(AGC("Středisko osoby", "StrediskoOsoby", , True, "j18_j02.j18Name", , "LEFT OUTER JOIN j18Region j18_j02 ON j02.j18ID=j18_j02.j18ID", "Osoba", "min(j18_j02.j18Name)", "j02.j18ID"))


            .Add(AGC(My.Resources.common.p32Name, "p32Name", , , , , , "Aktivita", "min(p32Name)", "a.p32ID"))
            .Add(AGC(My.Resources.common.FA, "p32IsBillable", BO.cfENUM.Checkbox, , , , , "Aktivita", "min(convert(int,p32.p32IsBillable))", "p32.p32IsBillable"))
            .Add(AGC(My.Resources.common.Sesit, "p34Name", , , , , , "Aktivita", "min(p34Name)", "p32.p34ID"))
            .Add(AGC(My.Resources.common.FakturacniOddil, "p95Name", , , , , , "Aktivita", "min(p95.p95Name)", "p32.p95ID"))

            .Add(AGC(My.Resources.common.Projekt, "p41Name", , , "isnull(p41NameShort,p41Name)", , , "Projekt", "min(p41Name)", "a.p41ID"))
            '.Add(AGC("Klient+Projekt", "ClientAndProject", , , "isnull(p28Client.p28Name+'-','')+p41Name", , , "Projekt", "min(isnull(p28Client.p28Name+' - ','')+p41Name)", "a.p41ID"))
            .Add(AGC("Klient+Projekt", "ClientAndProject", , , "isnull(p28client.p28Name+char(32)+isnull(p41NameShort,p41Name),isnull(p41NameShort,p41Name))", , , "Projekt", "min(isnull(p28Client.p28Name+' - ','')+p41Name)", "a.p41ID"))

            .Add(AGC("Stromový název", "p41TreePath", , , "isnull(p41TreePath,p41Name)", , , "Projekt", "min(p41TreePath)", "a.p41ID"))

            .Add(AGC(My.Resources.common.KodProjektu, "p41Code", , , , , , "Projekt", "min(p41Code)", "a.p41ID"))
            .Add(AGC(My.Resources.common.KlientProjektu, "ClientName", , , "p28Client.p28Name", , , "Projekt", "min(p28Client.p28Name)", "p41.p28ID_Client"))
            .Add(AGC("Středisko projektu", "StrediskoProjektu", , True, "j18_p41.j18Name", , "LEFT OUTER JOIN j18Region j18_p41 ON p41.j18ID=j18_p41.j18ID", "Projekt", "min(j18_p41.j18Name)", "p41.j18ID"))
            .Add(AGC("Typ projektu", "TypProjektu", , True, "p42.p42Name", , "LEFT OUTER JOIN p42ProjectType p42 ON p41.p42ID=p42.p42ID", "Projekt", "min(p42Name)", "p41.p42ID"))

            .Add(AGC(My.Resources.common.NazevUkolu, "p56Name", , , , , , "Úkol", "min(p56Name)", "a.p56ID"))
            .Add(AGC(My.Resources.common.KodUkolu, "p56Code", , , , , , "Úkol", "min(p56Code)", "a.p56ID"))

            .Add(AGC("Text", "p31Text"))
            .Add(AGC(My.Resources.common.Dodavatel, "SupplierName", , , "supplier.p28Name", , , , "min(supplier.p28Name)", "a.p28ID_Supplier"))
            .Add(AGC(My.Resources.common.KodDokladu, "p31Code"))
            .Add(AGC(My.Resources.common.KontaktniOsoba, "ContactPerson", , , "cp.j02LastName+char(32)+cp.j02FirstName"))

            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(331,a.p31ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(331,a.p31ID)"))

            .Add(AGC(My.Resources.common.VykazanaHodnota, "p31Value_Orig", BO.cfENUM.Numeric2, , , True, , "Vykázáno"))
            .Add(AGC(My.Resources.common.VykazaneHodiny, "p31Hours_Orig", BO.cfENUM.Numeric2, , , True, , "Vykázáno"))
            .Add(AGC(My.Resources.common.VykazaneHodinyHHMM, "p31HHMM_Orig", , , "p31Hours_Orig", , , "Vykázáno"))
            If Not bolHideRatesColumns Then
                .Add(AGC(My.Resources.common.VychoziSazba, "p31Rate_Billing_Orig", BO.cfENUM.Numeric2, , , , , "Vykázáno", "p31Rate_Billing_Orig", "p31Rate_Billing_Orig"))
                .Add(AGC(My.Resources.common.CastkaBezDPH, "p31Amount_WithoutVat_Orig", BO.cfENUM.Numeric2, , , True, , "Vykázáno"))

                .Add(AGC("Částka vč. DPH", "p31Amount_WithVat_Orig", BO.cfENUM.Numeric2, , , True, , "Vykázáno"))
                .Add(AGC(My.Resources.common.NakladovaSazba, "p31Rate_Internal_Orig", BO.cfENUM.Numeric2, , , , , "Nákladová cena", "p31Rate_Internal_Orig", "p31Rate_Internal_Orig"))
                .Add(AGC(My.Resources.common.NakladovaCastka, "p31Amount_Internal", BO.cfENUM.Numeric2, , , True, , "Nákladová cena"))

                .Add(AGC("Bez DPH dle fixního kurzu", "p31Amount_WithoutVat_FixedCurrency", BO.cfENUM.Numeric2, , , True, , "Vykázáno"))

                .Add(AGC("Náklad", "Vykazano_Naklad", BO.cfENUM.Numeric2, , "p31_ocas.Vykazano_Naklad", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))
                .Add(AGC("Odhad výnosu", "Vykazano_Vynos", BO.cfENUM.Numeric2, , "p31_ocas.Vykazano_Vynos", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))
                .Add(AGC("Odhad zisku", "Vykazano_Zisk", BO.cfENUM.Numeric2, , "p31_ocas.Vykazano_Zisk", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))

                .Add(AGC("Vyfakturovaný výnos", "Vyfakturovano_Vynos", BO.cfENUM.Numeric2, , "p31_ocas.Vyfakturovano_Vynos", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))
                .Add(AGC("Vyfakturovaný výnos x Kurz", "Vyfakturovano_Vynos_Domestic", BO.cfENUM.Numeric2, , "p31_ocas.Vyfakturovano_Vynos_Domestic", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))
                .Add(AGC("Zisk po fakturaci", "Vyfakturovano_Zisk", BO.cfENUM.Numeric2, , "p31_ocas.Vyfakturovano_Zisk", True, "LEFT OUTER JOIN dbo.view_p31_ocas p31_ocas ON a.p31ID=p31_ocas.p31ID", "Výsledovka"))
            End If

            .Add(AGC("Číslo faktury", "p91Code", , , , , , "Vyfakturováno", "min(p91Code)", "a.p91ID"))
            .Add(AGC("Vyfakt.status", "p70Name", , , , , , "Vyfakturováno", "min(p70Name)", "a.p70ID"))

            .Add(AGC("Kalk/počet", "p31Calc_Pieces", BO.cfENUM.Numeric2, , , , , "Vykázáno"))
            .Add(AGC("Kalk/cena 1 ks", "p31Calc_PieceAmount", BO.cfENUM.Numeric2, , , , , "Vykázáno"))

            .Add(AGC(My.Resources.common.SazbaDPH, "p31VatRate_Orig", BO.cfENUM.Numeric0, , , , , "Vykázáno", "p31VatRate_Orig", "p31VatRate_Orig"))
            .Add(AGC(My.Resources.common.Mena, "j27Code_Billing_Orig", , , "j27billing_orig.j27Code", , , "Vykázáno", "min(j27billing_orig.j27Code)", "a.j27ID_Billing_Orig"))

            .Add(AGC(My.Resources.common.Schvaleno, "p71Name", , , , , , "Schváleno", "min(p71Name)", "a.p71ID"))
            .Add(AGC(My.Resources.common.NavrhFakturacnihoStatusu, "approve_p72Name", , , "p72approve.p72Name", , , "Schváleno"))
            .Add(AGC(My.Resources.common.SchvalenaHodnota, "p31Value_Approved_Billing", BO.cfENUM.Numeric2, , , True, , "Schváleno"))
            .Add(AGC(My.Resources.common.SchvaleneHodiny, "p31Hours_Approved_Billing", BO.cfENUM.Numeric2, , , True, , "Schváleno"))
            .Add(AGC(My.Resources.common.SchvalenoHHMM, "p31HHMM_Approved_Billing", , , , , , "Schváleno"))
            .Add(AGC(My.Resources.common.SchvalenaSazba, "p31Rate_Billing_Approved", BO.cfENUM.Numeric2, , , , , "Schváleno"))
            .Add(AGC("Schváleno interně", "p31Value_Approved_Internal", BO.cfENUM.Numeric2, , , True, , "Schváleno"))
            If Not bolHideRatesColumns Then .Add(AGC(My.Resources.common.SchvalenoBezDPH, "p31Amount_WithoutVat_Approved", BO.cfENUM.Numeric2, , , True, , "Schváleno"))
            If Not bolHideRatesColumns Then .Add(AGC(My.Resources.common.SchvalenoVcDPH, "p31Amount_WithVat_Approved", BO.cfENUM.Numeric2, , , True, , "Schváleno"))
            .Add(AGC("Úroveň schvalování", "p31ApprovingLevel", BO.cfENUM.Numeric0, , , , , "Schváleno"))
            .Add(AGC(My.Resources.common.SchvalenoKdy, "p31Approved_When", BO.cfENUM.DateTime, , , , , "Schváleno"))
            .Add(AGC(My.Resources.common.BillingDavka, "p31ApprovingSet", , , , , , "Schváleno"))


            .Add(AGC(My.Resources.common.VyfakturovanaHodnota, "p31Value_Invoiced", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))
            .Add(AGC(My.Resources.common.VyfakturovaneHodiny, "p31Hours_Invoiced", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))
            .Add(AGC("Vyfakt.HH:mm", "p31HHMM_Invoiced", , , , , , "Vyfakturováno"))
            If Not bolHideRatesColumns Then
                .Add(AGC(My.Resources.common.VyfakturovanaSazba, "p31Rate_Billing_Invoiced", BO.cfENUM.Numeric2, , , , , "Vyfakturováno"))
                .Add(AGC(My.Resources.common.VyfakturovanoBezDPH, "p31Amount_WithoutVat_Invoiced", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))
                .Add(AGC(My.Resources.common.VyfakturovanoVcDPH, "p31Amount_WithVat_Invoiced", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))
                .Add(AGC("Vyfakt.sazba DPH", "p31VatRate_Invoiced", BO.cfENUM.Numeric0, , , , , "Vyfakturováno"))
                .Add(AGC("Vyfakt.bez DPH x Kurz", "p31Amount_WithoutVat_Invoiced_Domestic", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))
                .Add(AGC("Měna faktury", "j27Code_Billing_Invoice", , , "j27billing_invoice.j27Code", , "LEFT OUTER JOIN j27Currency j27billing_invoice ON a.j27ID_Billing_Invoiced=j27billing_invoice.j27ID", "Vyfakturováno", "min(j27billing_invoice.j27Code)", "a.j27ID_Billing_Invoiced"))
            End If
            .Add(AGC("Hodiny v paušálu", "p31Value_FixPrice", BO.cfENUM.Numeric2, , , True, , "Vyfakturováno"))


            .Add(AGC("Typ úhrady", "TypUhrady", , True, "j19.j19Name", , "LEFT OUTER JOIN j19PaymentType j19 ON a.j19ID=j19.j19ID"))


            .Add(AGC(My.Resources.common.VlastnikZaznamu, "Owner", , False, "j02owner.j02LastName+char(32)+j02owner.j02FirstName", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozeno, "p31DateInsert", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozil, "p31UserInsert", , , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizace, "p31DateUpdate", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizoval, "p31UserUpdate", , , , , , "Záznam"))
        End With
        AppendRoles(BO.x29IdEnum.p41Project, "a.p41ID", "Projektová a klientská role", lis)
        AppendRoles(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Projektová a klientská role", lis)
        AppendRoles(BO.x29IdEnum.p56Task, "a.p56ID", "Úkolová role", lis)
        AppendFreeFields(BO.x29IdEnum.p31Worksheet, lis)
    End Sub
    Private Sub InhaleP91ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC("Číslo", "p91Code"))

            .Add(AGC("Název klienta (vazba)", "p28Name", , , "p28client.p28Name"))
            .Add(AGC("Firma klienta (vazba)", "p28CompanyName", , , "p28client.p28CompanyName"))
            .Add(AGC("Klient ve faktuře", "p91Client"))
            .Add(AGC("Klient projektu", "PClient", , , "projectclient.p28Name", , "LEFT OUTER JOIN p28Contact projectclient ON p41.p28ID_Client=projectclient.p28ID"))

            .Add(AGC("Měna", "j27Code"))
            .Add(AGC("Typ faktury", "p92Name"))
            .Add(AGC("Projekt", "p41Name", , , "isnull(p41NameShort,p41Name)"))
            .Add(AGC("DPH region", "j17Name"))

            .Add(AGC("Bez dph", "p91Amount_WithoutVat", BO.cfENUM.Numeric, , , True))
            .Add(AGC("Bez dph x Kurz", "WithoutVat_Krat_Kurz", BO.cfENUM.Numeric, , "dbo.my_iif1(a.j27ID,a.j27ID_Domestic,p91Amount_WithoutVat,p91Amount_WithoutVat*p91ExchangeRate)", True))
            .Add(AGC("Dluh", "p91Amount_Debt", BO.cfENUM.Numeric, , , True))
            .Add(AGC("Dluh x Kurz", "Debt_Krat_Kurz", BO.cfENUM.Numeric, , "dbo.my_iif1(a.j27ID,a.j27ID_Domestic,p91Amount_Debt,p91Amount_Debt*p91ExchangeRate)", True))
            .Add(AGC("Celkem", "p91Amount_TotalDue", BO.cfENUM.Numeric, , , True))
            .Add(AGC("Celkem x Kurz", "p91Amount_TotalDue_Krat_Kurz", BO.cfENUM.Numeric, , "dbo.my_iif1(a.j27ID,a.j27ID_Domestic,p91Amount_TotalDue,p91Amount_TotalDue*p91ExchangeRate)", True))
            .Add(AGC("Celk.dph", "p91Amount_Vat", BO.cfENUM.Numeric, , , True))
            .Add(AGC("Vč.dph", "p91Amount_WithVat", BO.cfENUM.Numeric, , , True))
            .Add(AGC("Haléřové zaokrouhlení", "p91RoundFitAmount", BO.cfENUM.Numeric))
            .Add(AGC("Uhrazené zálohy", "p91ProformaBilledAmount", BO.cfENUM.Numeric))
            ''.Add(AGC("Bez dph CZK", "WithoutVat_CZK", BO.cfENUM.Numeric, , , True))
            ''.Add(AGC("Bez dph EUR", "WithoutVat_EUR", BO.cfENUM.Numeric, , , True))
            ''.Add(AGC("Dluh CZK", "Debt_CZK", BO.cfENUM.Numeric, , , True))
            ''.Add(AGC("Dluh EUR", "Debt_EUR", BO.cfENUM.Numeric, , , True))


            .Add(AGC("Datum", "p91Date", BO.cfENUM.DateOnly))
            .Add(AGC("Plnění", "p91DateSupply", BO.cfENUM.DateOnly))
            .Add(AGC("Splatnost", "p91DateMaturity", BO.cfENUM.DateOnly))
            .Add(AGC("Dnů po splatnosti", "DnuPoSplatnosti", BO.cfENUM.Numeric0, , "dbo.my_iif1(a.p91Amount_Debt,0,null,datediff(day,p91DateMaturity,dbo.get_today()))", False))
            .Add(AGC("Datum úhrady", "p91DateBilled", BO.cfENUM.DateOnly))
            .Add(AGC("Aktuální stav", "b02Name"))



            .Add(AGC("Text", "p91Text1"))

            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(391,a.p91ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(391,a.p91ID)"))

            .Add(AGC("Ulice klienta", "p91ClientAddress1_Street"))
            .Add(AGC("Město klienta", "p91ClientAddress1_City"))
            .Add(AGC("PSČ klienta", "p91ClientAddress1_ZIP"))
            .Add(AGC("Stát klienta", "p91ClientAddress1_Country"))
            .Add(AGC("Kontaktní osoba", "p91ClientPerson"))
            .Add(AGC("IČ klienta", "p91Client_RegID"))
            .Add(AGC("DIČ klienta", "p91Client_VatID"))

            .Add(AGC("Čas odeslání", "VomKdyOdeslano", BO.cfENUM.DateTime, , "vom.Kdy_Odeslano", , "LEFT OUTER JOIN view_p91_sendbyemail vom ON a.p91ID=vom.p91ID", "Elektronicky odesláno"))
            .Add(AGC("Stav odeslání", "VomStav", , , "vom.AktualniStav", , "LEFT OUTER JOIN view_p91_sendbyemail vom ON a.p91ID=vom.p91ID", "Elektronicky odesláno"))
            .Add(AGC("Komu odesláno", "VomKomu", , , "vom.Komu", , "LEFT OUTER JOIN view_p91_sendbyemail vom ON a.p91ID=vom.p91ID", "Elektronicky odesláno"))
            .Add(AGC("Vloženo do fronty", "VomDateInsert", BO.cfENUM.DateTime, , "vom.Kdy_Zahajeno", , "LEFT OUTER JOIN view_p91_sendbyemail vom ON a.p91ID=vom.p91ID", "Elektronicky odesláno"))

            .Add(AGC("Vlastník záznamu", "Owner", , , "j02owner.j02LastName+char(32)+j02owner.j02FirstName"))
            .Add(AGC("Založeno", "p91DateInsert", BO.cfENUM.DateTime))
            .Add(AGC("Založil", "p91UserInsert"))
            .Add(AGC("Aktualizace", "p91DateUpdate", BO.cfENUM.DateTime))
            .Add(AGC("Aktualizoval", "p91UserUpdate"))
        End With
        AppendRoles(BO.x29IdEnum.p41Project, "a.p41ID_First", "Projektová role", lis)
        AppendRoles(BO.x29IdEnum.p28Contact, "p41.p28ID_Client", "Role klienta projektu", lis)
        AppendFreeFields(BO.x29IdEnum.p91Invoice, lis)
    End Sub

    Private Sub InhaleP56ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC(My.Resources.common.Kod, "p56Code"))
            .Add(AGC(My.Resources.common.Typ, "p57Name"))
            .Add(AGC(My.Resources.common.Nazev, "p56Name"))
            .Add(AGC("Aktuální stav", "b02Name"))
            .Add(AGC("Klient+projekt", "ClientAndProject", , , "isnull(p28client.p28Name+char(32)+isnull(p41NameShort,p41Name),isnull(p41NameShort,p41Name))", , , "Projekt"))
            .Add(AGC(My.Resources.common.Klient, "Client", , , "p28client.p28Name", , , "Projekt"))
            .Add(AGC(My.Resources.common.Projekt, "p41Name", , , "isnull(p41NameShort,p41Name)", , , "Projekt"))
            .Add(AGC(My.Resources.common.KodProjektu, "p41Code", , , , , , "Projekt"))
            .Add(AGC(My.Resources.common.Prijemce, "ReceiversInLine", , , "dbo.p56_getroles_inline(a.p56ID)"))

            .Add(AGC(My.Resources.common.Termin, "p56PlanUntil", BO.cfENUM.DateTime, , , , , "Plán úkolu"))
            .Add(AGC(My.Resources.common.PlanStart, "p56PlanFrom", BO.cfENUM.DateTime, , , , , "Plán úkolu"))

            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(356,a.p56ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(356,a.p56ID)"))

            .Add(AGC("Hotovo%", "p56CompletePercent", BO.cfENUM.Numeric0))

            .Add(AGC(My.Resources.common.PrioritaZadavatele, "p59NameSubmitter", , , "p59submitter.p59Name"))

            .Add(AGC("Hodnocení", "p56RatingValue", BO.cfENUM.Numeric0))
            .Add(AGC("Připomenutí", "p56ReminderDate", BO.cfENUM.DateTime))

            .Add(AGC(My.Resources.common.PlanHodin, "p56Plan_Hours", BO.cfENUM.Numeric2, , , True, , "Plán úkolu"))
            .Add(AGC(My.Resources.common.PlanVydaju, "p56Plan_Expenses", BO.cfENUM.Numeric2, , , True, , "Plán úkolu"))
            .Add(AGC(My.Resources.common.VykazaneHodiny, "Hours_Orig", BO.cfENUM.Numeric2, , "p31.Hours_Orig", True, , "Vykázáno"))
            .Add(AGC(My.Resources.common.VykazaneVydaje, "Expenses_Orig", BO.cfENUM.Numeric2, , "p31.Expenses_Orig", True, , "Vykázáno"))

            .Add(AGC(My.Resources.common.VlastnikZaznamu, "Owner", , , "j02owner.j02LastName+char(32)+j02owner.j02FirstName", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozeno, "p56DateInsert", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozil, "p56UserInsert", , , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizace, "p56DateUpdate", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizoval, "p56UserUpdate", , , , , , "Záznam"))
            .Add(AGC(My.Resources.common.ExterniKod, "p56ExternalPID"))
        End With
        AppendRoles(BO.x29IdEnum.p56Task, "a.p56ID", "Role v úkolu", lis)
        AppendFreeFields(BO.x29IdEnum.p56Task, lis)
    End Sub
    Private Sub InhaleO23ColList(ByRef lis As List(Of BO.GridColumn))
        With lis
            .Add(AGC("Typ dokumentu", "x18Name"))
            .Add(AGC("Název", "o23Name"))
            .Add(AGC("Kód", "o23Code"))
            .Add(AGC("Aktuální stav", "b02Name"))
            
            .Add(AGC("Příjemci", "ReceiversInLine", , , "dbo.o23_getroles_inline(a.o23ID)"))

            .Add(AGC(My.Resources.common.Stitky, "TagsHtml", , , "dbo.tag_values_inline_html(223,a.o23ID)"))
            .Add(AGC(My.Resources.common.Stitky + " (text)", "TagsText", , , "dbo.tag_values_inline(223,a.o23ID)"))

            .Add(AGC("Datum", "o23FreeDate01", BO.cfENUM.DateTime))
            .Add(AGC("Připomenutí", "o23ReminderDate", BO.cfENUM.DateTime))

            .Add(AGC(My.Resources.common.VlastnikZaznamu, "Owner", , , "j02owner.j02LastName+char(32)+j02owner.j02FirstName", , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozeno, "o23DateInsert", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Zalozil, "o23UserInsert", , , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizace, "o23DateUpdate", BO.cfENUM.DateTime, , , , , "Záznam"))
            .Add(AGC(My.Resources.common.Aktualizoval, "o23UserUpdate", , , , , , "Záznam"))
        End With
        AppendRoles(BO.x29IdEnum.o23Doc, "a.o23ID", "Role v dokumentu", lis)

        Dim lisX18 As IEnumerable(Of BO.x18EntityCategory) = Factory.x18EntityCategoryBL.GetList(, BO.x29IdEnum._NotSpecified, -1), x As Integer = 0
        Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Factory.x18EntityCategoryBL.GetList_x16(0)        ''.Where(Function(p) p.x16IsGridField = True)
        For Each cX18 In lisX18
            For Each c In lisX16.Where(Function(p) p.x18ID = cX18.PID).OrderBy(Function(p) p.x16Ordinary)
                ''Dim strSql As String = "iif(x18.x18ID=" & c.x18ID.ToString & "," & c.x16Field & ",null)" 'kvůli tomu, aby do gridu každý sloupce vstupoval s unikátním názvem
                Dim strSql As String = "dbo.my_iif2_string(x18.x18ID," & c.x18ID.ToString & "," & c.x16Field & ",null)" 'kvůli tomu, aby do gridu každý sloupce vstupoval s unikátním názvem
                Select Case c.FieldType
                    Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime
                        strSql = "dbo.my_iif2_date(x18.x18ID," & c.x18ID.ToString & "," & c.x16Field & ",null)"
                    Case BO.x24IdENUM.tDecimal, BO.x24IdENUM.tInteger
                        strSql = "dbo.my_iif2_number(x18.x18ID," & c.x18ID.ToString & "," & c.x16Field & ",null)"
                    Case BO.x24IdENUM.tBoolean
                        strSql = "dbo.my_iif2_bit(x18.x18ID," & c.x18ID.ToString & "," & c.x16Field & ",null)"
                End Select
                lis.Add(AGC(c.x16Name, "x16_field" & c.x16ID.ToString, c.GridColumnType, , strSql, , , cX18.x18Name))
            Next
            
        Next


    End Sub



    Private Function AGC(strHeader As String, strName As String, Optional colType As BO.cfENUM = BO.cfENUM.AnyString, Optional bolSortable As Boolean = True, Optional strDBName As String = "", Optional bolShowTotals As Boolean = False, Optional strSqlSyntax_FROM As String = "", Optional strTreeGroup As String = "", Optional strPivotSelectSql As String = "", Optional strPivotGroupBySql As String = "")
        Dim col As BO.GridColumn
        col = New BO.GridColumn(_x29id, strHeader, strName, colType)
        With col            
            .IsSortable = bolSortable
            .ColumnDBName = strDBName
            .IsShowTotals = bolShowTotals
            .SqlSyntax_FROM = strSqlSyntax_FROM
            .TreeGroup = strTreeGroup
            .Pivot_SelectSql = strPivotSelectSql
            .Pivot_GroupBySql = strPivotGroupBySql
        End With
        Return col
    End Function


    Public Function GroupByPallet(x29id As BO.x29IdEnum) As List(Of BO.GridGroupByColumn) Implements Ij70QueryTemplateBL.GroupByPallet
        Dim lis As New List(Of BO.GridGroupByColumn)
        lis.Add(New BO.GridGroupByColumn(My.Resources.common.BezSouhrnu, "", "", ""))
        Select Case x29id
            Case BO.x29IdEnum.p41Project
                lis.Add(New BO.GridGroupByColumn("Klient", "Client", "a.p28ID_Client", "min(p28client.p28Name)"))
                lis.Add(New BO.GridGroupByColumn("Typ projektu", "p42Name", "a.p42ID", "min(p42.p42Name)"))
                lis.Add(New BO.GridGroupByColumn("Středisko", "j18Name", "a.j18ID", "min(j18.j18Name)"))
                ''lis.Add(New BO.GridGroupByColumn("DRAFT", "p41IsDraft", "a.p41IsDraft", "a.p41IsDraft"))


            Case BO.x29IdEnum.p28Contact
                lis.Add(New BO.GridGroupByColumn("Typ klienta", "p29Name", "a.p29ID", "min(p29.p29Name)"))
                lis.Add(New BO.GridGroupByColumn("Typ faktury", "p92Name", "a.p92ID", "min(p92.p92Name)"))
                lis.Add(New BO.GridGroupByColumn("Fakturační jazyk", "p87Name", "a.p87ID", "min(p87Name)"))
                lis.Add(New BO.GridGroupByColumn("Vlastník záznamu", "Owner", "a.j02ID_Owner", "min(j02owner.j02LastName+' '+j02owner.j02FirstName)"))
                lis.Add(New BO.GridGroupByColumn("DRAFT", "p28IsDraft", "a.p28IsDraft", "a.p28IsDraft"))
            Case BO.x29IdEnum.o23Doc
                lis.Add(New BO.GridGroupByColumn("Typ dokumentu", "x18Name", "x23.x18ID", "min(x18Name)"))
                lis.Add(New BO.GridGroupByColumn("Aktuální stav", "b02Name", "a.b02ID", "min(b02.b02Name)"))
                lis.Add(New BO.GridGroupByColumn("Vlastník záznamu", "Owner", "a.j02ID_Owner", "min(j02owner.j02LastName+' '+j02owner.j02FirstName)"))
                lis.Add(New BO.GridGroupByColumn("DRAFT", "o23IsDraft", "a.o23IsDraft", "a.o23IsDraft"))
            Case BO.x29IdEnum.p31Worksheet
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Sesit, "p34Name", "p32.p34ID", "min(p34.p34Name)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.p32Name, "p32Name", "a.p32ID", "min(p34.p34Name+' - '+p32.p32Name)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Osoba, "Person", "a.j02ID", "min(j02.j02LastName+' '+j02.j02Firstname)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.KlientProjektu, "ClientName", "p41.p28ID_Client", "min(p28Client.p28Name)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Projekt, "p41Name", "a.p41ID", "min(isnull(p28Client.p28Name+' - ','')+isnull(p41.p41NameShort,p41.p41Name))"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Faktura, "p91Code", "a.p91ID", "min(p91.p91Code)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Schvaleno, "p71Name", "a.p71ID", "min(p71.p71Name)"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.FakturacniStatus, "p70Name", "a.p70ID", "min(p70.p70Name)"))
                lis.Add(New BO.GridGroupByColumn("Úkol", "p56Name", "a.p56ID", "min(p56.p56Name+' ('+p41.p41Name+')')"))
                lis.Add(New BO.GridGroupByColumn(My.Resources.common.Dodavatel, "SupplierName", "a.p28ID_Supplier", "min(supplier.p28Name)"))
                lis.Add(New BO.GridGroupByColumn("Billing dávka", "p31ApprovingSet", "a.p31ApprovingSet", "min(a.p31ApprovingSet)"))
            Case BO.x29IdEnum.p56Task
                lis.Add(New BO.GridGroupByColumn("Typ úkolu", "p57Name", "a.p57ID", "min(p57.p57Name)"))
                lis.Add(New BO.GridGroupByColumn("Klient", "Client", "p41.p28ID_Client", "min(p28client.p28Name)"))
                lis.Add(New BO.GridGroupByColumn("Projekt", "ProjectCodeAndName", "a.p41ID", "min(isnull(p28client.p28Name+' - ','')+isnull(p41NameShort,p41Name))"))
                lis.Add(New BO.GridGroupByColumn("Aktuální stav", "b02Name", "a.b02ID", "min(b02.b02Name)"))
                lis.Add(New BO.GridGroupByColumn("Priorita zadavatele", "p59NameSubmitter", "a.p59ID_Submitter", "min(p59submitter.p59name)"))
                lis.Add(New BO.GridGroupByColumn("Příjemce", "ReceiversInLine", "", ""))
                lis.Add(New BO.GridGroupByColumn("Vlastník záznamu", "Owner", "a.j02ID_Owner", "min(j02owner.j02LastName+' '+j02owner.j02FirstName)"))
            Case BO.x29IdEnum.j02Person
                lis.Add(New BO.GridGroupByColumn("Pozice", "j07Name", "a.j07ID", "min(j07Name)"))
                lis.Add(New BO.GridGroupByColumn("Pracovní fond", "c21Name", "a.c21ID", "min(c21Name)"))
                lis.Add(New BO.GridGroupByColumn("Středisko", "j18Name", "a.j18ID", "min(j18Name)"))
                lis.Add(New BO.GridGroupByColumn("Interní osoba", "j02IsIntraPerson", "j02IsIntraPerson", "j02IsIntraPerson"))
            Case BO.x29IdEnum.p91Invoice
                lis.Add(New BO.GridGroupByColumn("Klient", "p28Name", "a.p28ID", "min(p28Name)"))
                lis.Add(New BO.GridGroupByColumn("Měna", "j27Code", "a.j27ID", "min(j27Code)"))
                lis.Add(New BO.GridGroupByColumn("Typ faktury", "p92Name", "a.p92ID", "min(p92Name)"))
                lis.Add(New BO.GridGroupByColumn("DRAFT", "p91IsDraft", "p91IsDraft", "p91IsDraft"))
        End Select
        Return lis
    End Function
End Class
