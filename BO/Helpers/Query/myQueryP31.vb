
Public Enum myQueryP31_QuickQuery
    _NotSpecified = 0
    Editing = 1     'rozpracované
    Approved = 2    'prošlo schvalováním
    Invoiced = 3    'prošlo fakturací
    MovedToBin = 4  'přesunuto do koše
    EditingOrMovedToBin = 5   'rozpracované nebo v koši
    EditingOrApproved = 14        'rozpracované nebo schválené a čekající na fakturaci
    Is_ContactPerson = 6  's vazbou na kontaktní osobu
    Is_Document = 7           's vazbou na dokument
    Is_Corrention = 8     'vyplněná výchozí korekce úkonu
    Is_Task = 9           's vazbou na úkol
    Is_Supplier = 10      's vazbou na dodavatele
    Is_Budget = 11
    Is_p31Code = 12
    Is_GeneratedByRobot = 13  'vygenerováno robotem
End Enum
Public Enum myQueryP31_SpecificQuery
    _NotSpecified = 0
    AllowedForRead = 1          'pouze úkony, ke kterým mám právo na čtení
    AllowedForDoApprove = 2     'pouze rozpracované úkony, které mohu schvalovat
    AllowedForReApprove = 3       'schválené úkony, které dosud nejsou vyfakturovány
    AllowedForCreateInvoice = 4     'pouze úkony, které mohu vyfakturovat
End Enum
Public Enum myQueryP31_Period
    p31Date = 1
    p31DateInsert = 2
    p91Date = 3
    p91DateSupply = 4
End Enum
Public Class myQueryP31
    Inherits myQuery
    Public Property j70ID As Integer
    Public Property p41ID As Integer
    Public Property p41IDs As List(Of Integer)
    Public Property IncludeChildProjects As Boolean
    Public Property p28ID_Client As Integer
    Public Property p28IDs_Client As List(Of Integer)
    Public Property p28ID_Supplier As Integer?
    Public Property p31ApprovingLevel As Integer?

    Public Property p56IDs As List(Of Integer)
    Public Property o22ID As Integer
    Public Property j02ID As Integer
    Public Property j02IDs As List(Of Integer)
    Public Property p34ID As Integer
    Public Property p34IDs As List(Of Integer)
    Public Property p32ID As Integer
    Public Property j27ID_Billing_Orig As Integer

    Public Property p33IDs As List(Of Integer)

    Public Property p91ID As Integer
    Public Property p91IDs As List(Of Integer)

    Public Property p70ID As p70IdENUM?
    Public Property p71ID As p71IdENUM?
    Public Property p72ID As p72IdENUM?

    Public Property p49ID As Integer
    Public Property o23ID As Integer

    Public Property Billable As BooleanQueryMode = BooleanQueryMode.NoQuery

    Public Property QuickQuery As myQueryP31_QuickQuery = myQueryP31_QuickQuery._NotSpecified
    Public Property SpecificQuery As myQueryP31_SpecificQuery = myQueryP31_SpecificQuery._NotSpecified
    Public Property j02ID_ExplicitQueryFor As Integer

    Public Property TabAutoQuery As String  'možné hodnoty: time, expense, fee, kusovnik
    Public Property PeriodType As myQueryP31_Period = myQueryP31_Period.p31Date
    Public Property x18Value As String  'filtrování podle kategorií
    Public Property o51IDs As List(Of Integer)  'filtrování podle štítků
End Class
