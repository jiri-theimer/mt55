Public Class p41RecordDisposition
    Public Property ReadAccess As Boolean
    Public Property OwnerAccess As Boolean
    Public Property P56_Create As Boolean
    Public Property P31_RecalcRates As Boolean
    Public Property P31_Move2Bin As Boolean
    Public Property P31_MoveToOtherProject As Boolean
    Public Property p91_Read As Boolean
    Public Property p91_DraftCreate As Boolean
    Public Property p47_Owner As Boolean

    Public Property p45_Owner As Boolean
    Public Property p45_Read As Boolean

    Public Property x67IDs As List(Of Integer)  'seznam projektových rolí, které daná osoba v projektu má přiřazené
End Class
