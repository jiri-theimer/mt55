Public Class o43ImapRobotHistory
    Public Property o43ID As Integer
    Public Property o43ImapArchiveFolder As String
    Public Property o43DateImport As Date
    Public Property o43DateMessage As Date
    Public Property o41ID As Integer
    Public Property p56ID As Integer
    Public Property o23ID As Integer
    Public Property o43RecordGUID As String
    Public Property o43MessageGUID As String
    Public Property o43Message As String
    Public Property o43Subject As String
    Public Property o43Body_PlainText As String
    Public Property o43Body_Html As String
    Public Property o43FROM As String
    Public Property o43FROM_DisplayName As String
    Public Property o43CC As String
    Public Property o43BCC As String
    Public Property o43TO As String
    Public Property j02ID_Owner As Integer
    Public Property o43Attachments As String

    Public Property o43Length As Integer

    Public Property o43ErrorMessage As String
    Public Property o43EmlFileName As String
    Public Property o43MsgFileName As String

    Private Property _o41Name As String
    Public ReadOnly Property o41Name As String
        Get
            Return _o41Name
        End Get
    End Property

    Public ReadOnly Property EntityName As String
        Get
            If Me.p56ID > 0 Then
                Return "ÚKOL"
            End If
            If Me.o23ID > 0 Then
                Return "DOKUMENT"
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property pid As Integer
        Get
            Return Me.o43ID
        End Get
    End Property
End Class
