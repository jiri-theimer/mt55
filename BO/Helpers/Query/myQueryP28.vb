Public Enum myQueryP28_SpecificQuery
    _NotSpecified = 0

    AllowedForRead = 2              'pouze kontakty, ke kterým má právo na čtení

End Enum
Public Enum myQueryP28_QuickQuery
    _NotSpecified = 0
    OpenClients = 1
    Removed2Bin = 2
    WaitingOnApproval = 3
    OverWorksheetLimit = 4
    WaitingOnInvoice = 5
    ProjectClient = 6
    ProjectInvoiceReceiver = 7
    DraftClients = 11
    NonDraftCLients = 12
    WithContactPersons = 16
    WithoutContactPersons = 17
    WithProjects = 18
    WithoutProjects = 19
    WIthNotepad = 20
    SupplierSide = 21
    DuplicityInCompanyName = 22
    DuplicityRegID = 23
    DuplicityVatID = 24
    WithParentContact = 25
    WithChildContact = 26
    WithOverHead = 27
    NotClientNotSupplier = 28
    WithBillingMemo = 29
    IsirMonitoring = 30
    WithAnyInvoice = 31
    WithOpenProjects = 32
End Enum
Public Class myQueryP28
    Inherits myQuery

    Public Property p29ID As Integer
    Public Property j02ID As Integer
    Public Property b02ID As Integer
    Public Property j70ID As Integer
    Public Property p51ID As Integer
    Public Property p28ParentID As Integer
    Public Property CanBeSupplier As BooleanQueryMode = BooleanQueryMode.NoQuery
    Public Property CanBeClient As BooleanQueryMode = BooleanQueryMode.NoQuery
    Public QuickQuery As myQueryP28_QuickQuery = myQueryP28_QuickQuery._NotSpecified
    Public SpecificQuery As myQueryP28_SpecificQuery = myQueryP28_SpecificQuery._NotSpecified
    Public Property j02ID_ExplicitQueryFor As Integer = 0   'ID osoby, z jejíhož pohledu se mají odfiltrovat klienty, pokud je 0, pak přihlášený uživatel

    Public Property TreeIndexFrom As Integer
    Public Property TreeIndexUntil As Integer
    Public Property p28TreeLevel As Integer = -1
    Public Property x18Value As String  'filtrování podle kategorií
    Public Property o51IDs As List(Of Integer)  'filtrování podle štítků
End Class
