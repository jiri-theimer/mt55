Public Class b09WorkflowCommandCatalog
    Public Property b09ID As Integer
    Public Property x29ID As x29IdEnum
    Public Property b09Code As String
    Public Property b09Name As String
    Public Property b09Ordinary As Integer
    Public Property b09SQL As String
    
    Public ReadOnly Property FullName As String
        Get
            Return b09Name & " [" & b09Code & "]"
        End Get
    End Property

End Class
