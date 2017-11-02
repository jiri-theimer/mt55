Public Class clsToolBarButton
    Public Property Text As String
    Public Property Value As String
    Public Property Index As Integer
    Public Property ImageURL As String
    Public Property AutoPostback As Boolean = True
    Public Property NavigateURL As String
    Public Property Target As String
    Public Property ShowLoading As Boolean

    Public Property GroupText As String

    Public Sub New(strText As String, strValue As String)
        Me.Text = strText
        Me.Value = strValue
    End Sub

End Class
