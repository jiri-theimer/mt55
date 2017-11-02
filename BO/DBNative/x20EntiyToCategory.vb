Public Enum x20EntryModeENUM
    Combo = 1
    InsertUpdateWithoutCombo = 2
    ExternalByWorkflow = 3
End Enum
Public Enum x20GridColumnENUM
    EntityColumn = 1
    CategoryColumn = 2
    Both = 3
    _None = 4
End Enum
Public Enum x20EntityPageENUM
    Label = 1
    Hyperlink = 2
    HyperlinkPlusNew = 3
    NotUsed = 9
End Enum

Public Class x20EntiyToCategory
    Public Property x20ID As Integer
    Public Property x18ID As Integer
    Public Property x29ID As Integer
    Public Property x20Name As String
    Public Property x20IsMultiSelect As Boolean
    Public Property x20IsEntryRequired As Boolean
    Public Property x20EntityTypePID As Integer
    Public Property x29ID_EntityType As Integer
    Public Property x20EntryModeFlag As x20EntryModeENUM = x20EntryModeENUM.Combo
    Public Property x20GridColumnFlag As x20GridColumnENUM = x20GridColumnENUM.EntityColumn

    Public Property EntityTypeAlias As String   'pomocný atribut - není v SQL
    Public Property x20IsClosed As Boolean
    Public Property x20Ordinary As Integer
    Public Property x20EntityPageFlag As x20EntityPageENUM = x20EntityPageENUM.Label

End Class
