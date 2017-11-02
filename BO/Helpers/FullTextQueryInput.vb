Public Class FullTextQueryInput
    Public Property SearchExpression As String
    Public Property IncludeMain As Boolean
    Public Property IncludeWorksheet As Boolean
    Public Property IncludeTask As Boolean
    Public Property IncludeInvoice As Boolean
    Public Property IncludeDocument As Boolean
    Public Property DateFrom As Date?
    Public Property DateUntil As Date?

    Public Property TopRecs As Integer = 50

End Class
