Public Enum cfENUM
    AnyString = 1
    DateOnly = 2
    DateTime = 3
    Checkbox = 4
    Numeric = 6
    Numeric0 = 7
    Numeric2 = 8
    Numeric3 = 9
    TimeOnly = 5

End Enum

Public Class GridColumn
    Public Property x29ID As BO.x29IdEnum
    Public Property ColumnType As cfENUM
    Public Property ColumnHeader As String
    Public Property ColumnName As String
    Public Property IsSortable As Boolean = True
    Public Property ColumnDBName As String
    Public Property IsShowTotals As Boolean = False
    Public Property IsAllowFiltering As Boolean = True
    Public Property DrillDownDBName As String
    Public Property SqlSyntax_FROM As String
    Public Property TreeGroup As String
    Public Property Pivot_SelectSql As String
    Public Property Pivot_GroupBySql As String
    Public Property MyTag As String


    Public Sub New(colX29ID As BO.x29IdEnum, strHeader As String, strName As String, Optional colType As cfENUM = cfENUM.AnyString, Optional bolSortable As Boolean = True)
        Me.x29ID = colX29ID
        Me.ColumnHeader = strHeader
        Me.ColumnName = strName
        Me.ColumnType = colType
        Me.IsSortable = bolSortable
    End Sub


    Public ReadOnly Property ColumnSqlSyntax_OrderBy As String
        Get
            If Me.ColumnDBName <> "" Then
                Return Me.ColumnDBName
            Else
                Return Me.ColumnName
            End If
        End Get
    End Property
    Public ReadOnly Property ColumnSqlSyntax_Select As String
        Get
            If Me.ColumnDBName = "" Then
                Return Me.ColumnName
            Else
                Return Me.ColumnDBName & " AS " & Me.ColumnName
            End If
        End Get
    End Property
    Public ReadOnly Property DrillDownSqlSyntax_Select As String
        Get
            Return Me.DrillDownDBName & " AS " & Me.ColumnName
        End Get
    End Property
    
End Class

Public Class GridGroupByColumn
    Public Property ColumnHeader As String
    Public Property ColumnField As String
    Public Property FieldSqlGroupBy As String
    Public Property AggregateSQL As String

    Public Sub New(strHeader As String, strField As String, strFieldSqlGroupBy As String, strAggregateSQL As String)
        Me.ColumnHeader = strHeader
        Me.ColumnField = strField
        Me.FieldSqlGroupBy = strFieldSqlGroupBy
        Me.AggregateSQL = strAggregateSQL
    End Sub

    Public ReadOnly Property LinqQueryField As String
        Get
            If Me.FieldSqlGroupBy.IndexOf(".") > 0 Then
                Dim a() As String = Split(FieldSqlGroupBy, ".")
                Return a(1)
            Else
                Return Me.FieldSqlGroupBy
            End If

        End Get
    End Property

    
End Class
