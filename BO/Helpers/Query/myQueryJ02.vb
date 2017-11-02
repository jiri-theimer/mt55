Public Enum myQueryJ02_SpecificQuery
    _NotSpecified = 0
    AllowedForWorksheetEntry = 1    'pouze osoby, za které lze zapisovat worksheet
    AllowedForRead = 2              'pouze osoby, ke kterým má právo na čtení
    AllowedForP48Entry = 3            'pouze osoby, za které lze vytvářet operativní plán
    

End Enum
Public Enum myQueryJ02_QuickQuery
    _NotSpecified = 0
    OpenPersons = 1
    Removed2Bin = 2
    WaitingOnApproval = 3
    WaitingOnInvoice = 5
    IntraPersonsOnly = 6
    NonIntraPersonsOnly = 7
    WithAnyTask = 8
    WithOpenTask = 9
End Enum
Public Enum myQueryJ02_IntraPersons
    _NotSpecified = 0
    IntraOnly = 1       'pouze personální zdroje firmy
    NonIntraOnly = 2  'mimo personálních zdrojů firmy
End Enum
Public Class myQueryJ02
    Inherits myQuery
    Public Property j04ID As Integer
    Public Property j11ID As Integer
    Public Property j07ID As Integer
    Public Property p41ID As Integer
    Public Property j70ID As Integer
    Public Property p28ID As Integer
    Public Property p91ID As Integer

    Public SpecificQuery As myQueryJ02_SpecificQuery = myQueryJ02_SpecificQuery._NotSpecified
    Public IntraPersons As myQueryJ02_IntraPersons = myQueryJ02_IntraPersons.IntraOnly
    Public QuickQuery As myQueryJ02_QuickQuery = myQueryJ02_QuickQuery._NotSpecified
    Public Property x18Value As String  'filtrování podle kategorií (dokumentů)
    Public Property o51IDs As List(Of Integer)  'filtrování podle štítků
End Class
