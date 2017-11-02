Public Class x20_join_x18
    Inherits BO.x18EntityCategory
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
    Public Property x20EntityPageFlag As x20EntityPageENUM = x20EntityPageENUM.Label

    Public Property EntityTypeAlias As String   'pomocný atribut - není v SQL
    Public Property x20IsClosed As Boolean
    Public Property x20Ordinary As Integer

    Public ReadOnly Property BindName As String
        Get
            If Me.x20Name = "" Then
                Return BO.BAS.GetX29EntityAlias(CType(Me.x29ID, BO.x29IdEnum), False)
            Else
                Return Me.x20Name
            End If
        End Get
    End Property

End Class
