Public Class SetupDataGrid
    Public Property factory As BL.Factory
    Public Property grid As UI.datagrid
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
    Public Property MasterPrefix As String


   


    Public Sub New(cFactory As BL.Factory, cGrid As UI.datagrid, cJ70Template As BO.j70QueryTemplate)
        Me.factory = cFactory
        Me.grid = cGrid
        Me.cJ70 = cJ70Template
    End Sub
End Class

Public Class PreparedDataGrid
    Public Property Cols As String       'návratová hodnota
    Public Property AdditionalFROM As String 'návratová hodnota
    Public Property SumCols As String    'návratová hodnota
End Class
