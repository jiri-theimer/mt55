Public Enum BooleanQueryMode As Integer
    NoQuery = -1
    FalseQuery = 0
    TrueQuery = 1
End Enum
Public Class myQuery
    Public Property PIDs As List(Of Integer)


    Public Property j02ID_Owner As Integer
    
    Public Property Closed As BooleanQueryMode = BooleanQueryMode.FalseQuery

    Public Property DateFrom As Date = DateSerial(1900, 1, 1)
    Public Property DateUntil As Date = DateSerial(3000, 1, 1)

    Public Property DateInsertFrom As Date?
    Public Property DateInsertUntil As Date?

    Public Property p31Date_D1 As Date?
    Public Property p31Date_D2 As Date?
    
    Public Property TopRecordsOnly As Integer
    Public Property SearchExpression As String
    Public Property ColumnFilteringExpression As String

    Public Property MyRecordsDisponible As Boolean
    Public Property MyRecordsToBeInformed As Boolean            'kde jsem čtenář

    Public Property MG_PageSize As Integer
    Public Property MG_CurrentPageIndex As Integer
    Public Property MG_SortString As String
    Public Property MG_PageMovePrevOnly As Boolean          'zda vyrobit rychlý SQL pro grid, kde lze pohybovat pouze dopředu a dozadu o po stránce
    Public Property MG_AdditionalSqlFROM As String          'rozšíření FROM SQL klauzule
    Public Property MG_AdditionalSqlWHERE As String         'dodatečná WHERE klauzule
    Public Property MG_GridSqlColumns As String             'sloupce v gridu
    Public Property MG_GridGroupByField As String           'pole souhrnů v gridu

    Public Property MG_SelectPidFieldOnly As Boolean

    Public Sub AddItemToPIDs(intPID As Integer)
        If Me.PIDs Is Nothing Then Me.PIDs = New List(Of Integer)
        Me.PIDs.Add(intPID)
    End Sub
End Class
