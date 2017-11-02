Public Enum x53PermValEnum
    'Oprávnění globální aplikační role
    GR_Admin = 1                        'Administrátor systému (menu Administrace)
    GR_P28_Creator = 2                   'Vytvářet klienty
    GR_P28_Reader = 3                   'Čtenář všech klientů
    GR_P28_Owner = 4                    'Vlastnická práva ke všem klientům
    GR_P41_Creator = 5                   'Vytvářet projekty
    GR_P41_Reader = 6                   'Čtenář všech projektů
    GR_P41_Owner = 7                    'Vlastnická práva ke všem projektům
    GR_P51_Admin = 8                    'Správce ceníků sazeb
    GR_P91_Owner = 9                    'Vlastnická práva ke všem fakturám
    GR_P91_Reader = 10                  'Čtenář všech faktur
    GR_P31_Pivot = 11                 'Worksheet PIVOT
    GR_P31_Approve_P72 = 12             'Jako schvalovatel může navrhovat fakturační status úkonu
    GR_P31_Approve_SourceRecord = 13    'Jako schvalovatel může upravovat zdrojový úkon
    GR_P36_Admin = 14                   'Uzamykat worksheet období
    GR_P31_ApprovingDialog = 15           'Jako schvalovatel přístup do schvalovacího dialogu
    GR_P31_Trimm = 16                   'Navrhovat fakturační korekce v úkonu
    GR_P31_AllowRates = 17              'Přístup k billing informacím o worksheet úkonu (sazby a fakturační údaje)
    GR_P90_Create = 18                  'vytvářet zálohové faktury
    GR_P90_Reader = 19                  'Vlastnická práva ke všem zálohovým fakturám
    GR_P90_Owner = 20                   'Čtenář všech zálohových faktur
    GR_P31_Reader = 21                  'Čtenář všech worksheet záznamů
    GR_P31_Owner = 22                   'Vlastnická práva všech worksheet záznamů
    GR_P31_Approver = 23                'Oprávnění schvalovat všechny worksheet úkony v databázi
    GR_P91_Creator = 24                'Oprávnění vystavovat faktury za všechny projekty
    GR_P31_EditAsNonOwner = 25          'Pokud je osobou worksheet úkonu a není vlastníkem, má i přesto vlastnická práva k úkonu
    GR_P56_Creator = 26                  'vytvářet úkoly ve všech projektech
    GR_P48_Creator = 27                    'zapisovat operativní plán
    GR_P48_Reader = 28                    'Čtenář všech operativních plánů
    GR_P31_Creator = 29                   'Oprávnění zapisovat worksheet do jakéhokoliv projektu bez ohledu na projektovou roli
    GR_O22_Creator = 30                   'Zapisovat události do kalendáře
    GR_P41_Draft_Creator = 31                'Vytvářet DRAFT projekty
    GR_P28_Draft_Creator = 32                'Vytvářet DRAFT klienty
    ''GR_O23_Draft_Creator = 33                'Vytvářet DRAFT dokumenty
    GR_P91_Draft_Creator = 34                'Oprávnění vystavovat DRAFT faktury za všechny projekty
    ''GR_O10_Creator = 35                   'Zapisovat články na nástěnku
    GR_X31_Personal = 36                  'Přístup k osobním tiskovým sestavám
    GR_GridTools = 37                   'Pokročilé nástroje v datových přehledech (vytvářet pojmenované filtry/návrhář sloupců/export do XLS/PDF/DOC)
    GR_Navigator = 38                     'Přístup do nástroje NAVIGATOR
    GR_X18_Admin = 39                     'Správce dokumentů
    'Oprávnění projektové role
    PR_P41_Owner = 1                    'Oprávnění vlastníka projektu
    ''PR_P41_Roles = 2                    'Obsazovat projektové role v projektu
    PR_P56_Creator = 3                   'Vytvářet úlohy
    ''PR_P56_Owner = 5                    'Oprávnění vlastníka ke všem úkolům v projektu
    PR_P91_Creator = 6                   'V rámci projektu vystavovat DRAFT faktury
    PR_P91_Reader = 7                     'Přístup k vystaveným fakturám projektu
    PR_P31_RecalcRates = 8              'Zpětný přepočet sazeb úkonů v projektu
    PR_P31_MoveToOtherProject = 9       'Hromadně přesouvat  rozpracovanost na jiný projekt
    PR_P31_LimitPerBudget = 10            'Limit vykázatelných hodin kontrolovat striktně podle rozpočtu projektu
    PR_P56_Bin = 11                     'Uzavírat všechny úkoly v projektu (přesouvat do koše)
    PR_P31_Recurrence = 12              'Definovat šablony opakovaných worksheet úkonů/paušálů
    PR_P31_Move2Bin = 13                'hromadně přesouvat rozpracovanost do koše
    PR_P47_Owner = 14                  'vytvářet kapacitní plán projektu
    PR_P45_Owner = 15                  'vytvářet a upravovat rozpočet projektu
    PR_P45_Reader = 16                    'číst rozpočet projektu

   

    'Oprávnění role ve faktuře
    IR_P91_Reader = 1                   'Číst záznam faktury
    IR_P91_Owner = 2                   'Číst+Upravovat záznam faktury

    'Oprávnění role v zálohové faktuře
    PF_P90_Reader = 1                   'Číst záznam zálohové faktury
    PF_P90_Owner = 2                   'Číst+Upravovat záznam zálohové faktury

    'Oprávnění role v úkolu
    TR_P31_Creator = 1                   'Práva zapisovat worksheet v úkolu
    TR_P56_Owner = 2                    'Oprávnění vlastníka úkolu
    TR_P56_Bin = 3                      'Právo uzavírat úkoly

    'Oprávnění role v dokumentu
    DR_O23_Reader = 1                    'Oprávnění číst dokument
    DR_O23_Owner = 2                   'Oprávnění vlastníka dokumentu
    ''DR_O23_File_Appender = 3              'právo dohrávat do dokumentu další přílohy a komentáře
    DR_O23_Comments = 4                 'Zapisovat komentáře mimo workflow dokumentu
    DR_O23_Files_Lock1 = 5                 'Uzamykat/odemykat přístup k souborům dokumentu

    'Oprávnění role v klientovi
    CR_P28_Reader = 1                    'Přístup ke klientovi
    CR_P28_Owner = 2                   'Oprávnění vlastníka klienta

    'Oprávnění role pro položky štítku
    X18_ReaderItems = 1                 'Číst všechny dokumenty daného typu
    X18_CreateItems = 2                 'Vytvářet dokumenty v daném typu
    X18_OwnerItems = 3                  'Vytvářet, upravovat a číst dokumenty daného typu + zamykat přístup k přílohám
    X18_ReadAndUpload = 4               'Číst vše + nahrávat přílohy/komentáře
End Enum
Public Class x53Permission
    Inherits BOMotherFT
    Public Property x29ID As x29IdEnum
    Public Property x53Name As String
    Public Property x53Code As String
    Public Property x53Value As x53PermValEnum
    Public Property x53Ordinary As Integer
    Public Property x53Description As String

End Class
