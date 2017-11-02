Public Class o51Tag
    Inherits BOMother
    Public Property o51Name As String
    Public Property j02ID_Owner As Integer
    Public Property o51ScopeFlag As Integer = 0
    Public Property o51IsP41 As Boolean
    Public Property o51IsP28 As Boolean
    Public Property o51IsP56 As Boolean
    Public Property o51IsP91 As Boolean
    Public Property o51IsP90 As Boolean
    Public Property o51IsJ02 As Boolean
    Public Property o51IsO23 As Boolean
    Public Property o51IsP31 As Boolean
    Public Property o51BackColor As String
    Public Property o51ForeColor As String
    Public Property o51Description As String

    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
End Class
