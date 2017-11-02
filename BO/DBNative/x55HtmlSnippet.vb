Public Enum x55TypeENUM
    DynamicHtml = 1
    StaticHtml = 2
    ExternalPage = 3
End Enum
Public Class x55HtmlSnippet
    Inherits BOMother
    Public Property x55TypeFlag As x55TypeENUM
    Public Property x55Name As String
    Public Property x55Content As String
    Public Property x55Code As String
    Public Property x55Ordinary As Integer
    Public Property x55RecordSQL As String
    Public Property x55Height As String
    Public Property x55Width As String
    Public Property x55IsSystem As Boolean

    Public ReadOnly Property NameWithCode As String
        Get
            If x55Code = "" Then
                Return Me.x55Name
            Else
                Return Me.x55Name & " (" & Me.x55Code & ")"
            End If

        End Get
    End Property
End Class
