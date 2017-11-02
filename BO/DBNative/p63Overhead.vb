Public Class p63Overhead
    Inherits BOMother
    Public Property p32ID As Integer
    Public Property p63Name As String
    Public Property p63PercentRate As Double
    Public Property p63IsIncludeTime As Boolean
    Public Property p63IsIncludeFees As Boolean
    Public Property p63IsIncludeExpense As Boolean
    Private Property _p32Name As String

    Public ReadOnly Property p32Name As String
        Get
            Return _p32Name
        End Get
    End Property

    Public ReadOnly Property NameWithRate As String
        Get
            Return Me.p63Name & " [" & Me.p63PercentRate.ToString & "%]"
        End Get
    End Property
End Class
