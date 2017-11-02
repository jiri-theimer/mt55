Public Class SetupDataGrid
    Public Property cJ70 As BO.j70QueryTemplate
    Public Property PageSize As Integer = 20
    Public Property AllowCustomPaging As Boolean
    Public Property AllowMultiSelect As Boolean
    Public Property AllowMultiSelectCheckboxSelector As Boolean = True
    Public Property FilterSetting As String
    Public Property FilterExpression As String
    Public Property SortExpression As String
    Public Property SysColumnWidth As Integer = 16
    Public Property ContextMenuWidth As Integer = 16
    Public Property strMasterPrefix As String

End Class
