Public Enum myQueryP56_SpecificQuery
    _NotSpecified = 0
    AllowedForWorksheetEntry = 1    'pouze úkoly, kam lze zapisovat worksheet
    AllowedForRead = 2              'pouze úkoly, ke kterým má právo na čtení

End Enum

Public Enum myQueryP56_QuickQuery
    _NotSpecified = 0
    OpenTasks = 1
    Removed2Bin = 2
    WaitingOnApproval = 3
    WaitingOnInvoice = 5
    Is_PlanUntil = 6
    Is_OverPlanUtil = 7
    Is_PlanFrom = 8
    Is_PlanHours = 9
    Is_PlanExpenses = 10
    Is_OverPlanHours = 11
    Is_OverPlanEpenses = 12
    Is_RecurMother = 13
End Enum
Public Class myQueryP56
    Inherits myQuery

    Public Property p57ID As Integer
    Public Property p41ID As Integer
    Public Property p41IDs As List(Of Integer)
    Public Property IsIncludeChildProjects As Boolean = False  'zda zahrnout pod-projekty
    Public Property o22ID As Integer
    Public Property b02ID As Integer
    Public Property p28ID As Integer
    Public Property j70ID As Integer
    Public Property j02ID As Integer    'bráno z pohledu, kde je daná osoba příjemcem úkolu - nic víc
    Public Property j02IDs As List(Of Integer)  'bráno z pohledu, kde je daná osoba příjemcem úkolu - nic víc
    Public Property Owners As List(Of Integer)  'vlastníci úkolů

    Public Property p56PlanFrom_D1 As Date?
    Public Property p56PlanFrom_D2 As Date?
    Public Property p56PlanUntil_D1 As Date?
    Public Property p56PlanUntil_D2 As Date?

    Public SpecificQuery As myQueryP56_SpecificQuery = myQueryP56_SpecificQuery._NotSpecified
    Public Property j02ID_ExplicitQueryFor As Integer = 0   'ID osoby, z jejíhož pohledu se mají odfiltrovat úkoly, pokud je 0, pak přihlášený uživatel

    Public Property x18Value As String  'filtrování podle kategorií
    Public Property o51IDs As List(Of Integer)  'filtrování podle štítků
    Public Property TerminNeniVyplnen As BooleanQueryMode = BooleanQueryMode.NoQuery

    Public Property IsRecurrenceMother As BO.BooleanQueryMode = BooleanQueryMode.NoQuery
    Public Property IsRecurrenceChild As BO.BooleanQueryMode = BooleanQueryMode.NoQuery
    Public Property p56RecurMotherID As Integer
    Public Property IsShowTagsInColumn As Boolean   'platí pouze GetList bez datareader
End Class
