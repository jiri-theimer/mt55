Public Enum myQueryP91_SpecificQuery
    _NotSpecified = 0
    AllowedForRead = 2              'pouze faktury, ke kterým má právo na čtení

End Enum
Public Enum myQueryP91_PeriodType
    p91DateSupply = 1
    p91DateMaturity = 2
    p91Date = 3
End Enum
Public Enum myQueryP91_QuickQuery
    _NotSpecified = 0
    OpenInvoices = 1
    Removed2Bin = 2
    InMaturity = 3
    DebtAfterMaturity = 4
    WithDebt = 15
    IsDraft = 5
    IsOficialCode = 6
    BoundWithProforma = 7
    BoundWithCreditNote = 8
    Is_p91RoundFitAmount = 9
    Is_p91Amount_WithoutVat_Standard = 10
    Is_p91Amount_WithoutVat_Low = 11
    Is_p91Amount_WithoutVat_None = 12
    Is_ExchangeRate = 13
    WithOverhead = 14
End Enum
Public Class myQueryP91
    Inherits myQuery

    Public Property p92ID As Integer
    Public Property p41ID As Integer
    Public Property p56ID As Integer
    Public Property o38ID As Integer
    Public Property j02ID As Integer
    Public Property p28ID As Integer
    Public Property j27ID As Integer
    Public Property b02ID As Integer
    Public Property j70ID As Integer
    Public Property p93ID As Integer

    Public Property SpecificQuery As myQueryP91_SpecificQuery = myQueryP91_SpecificQuery._NotSpecified
    Public Property j02ID_ExplicitQueryFor As Integer = 0   'ID osoby, z jejíhož pohledu se mají odfiltrovat faktury, pokud je 0, pak přihlášený uživatel
    Public Property PeriodType As myQueryP91_PeriodType = myQueryP91_PeriodType.p91DateSupply
    Public Property QuickQuery As myQueryP91_QuickQuery = myQueryP91_QuickQuery._NotSpecified
    Public Property x18Value As String  'filtrování podle kategorií
    Public Property o51IDs As List(Of Integer)  'filtrování podle štítků
End Class
