Public Class b07Comment
    Inherits BOMother
    Public Property x29ID As BO.x29IdEnum
    Public Property b07RecordPID As Integer
    Public Property j02ID_Owner As Integer
    Public Property o43ID As Integer        'načtená e-mail zpráva přes IMAP
    Public Property b07Value As String
    Public Property b07WorkflowInfo As String

    Public Property b07ID_Parent As Integer

    Private Property _b07TreeOrder As Integer
    Public ReadOnly Property b07TreeOrder As Integer
        Get
            Return _b07TreeOrder
        End Get
    End Property
    Private Property _b07TreeLevel As Integer
    Public ReadOnly Property b07TreeLevel As Integer
        Get
            Return _b07TreeLevel
        End Get
    End Property

    Private Property _b07TreePrev As Integer
    Public ReadOnly Property b07TreePrev As Integer
        Get
            Return _b07TreePrev
        End Get
    End Property
    Private Property _b07TreeNext As Integer
    Public ReadOnly Property b07TreeNext As Integer
        Get
            Return _b07TreeNext
        End Get
    End Property

    Private Property _Author As String
    Public ReadOnly Property Author As String
        Get
            Return _Author
        End Get
    End Property
    Private Property _Avatar As String
    Public ReadOnly Property Avatar As String
        Get
            Return _Avatar
        End Get
    End Property

    ''Private Property _o27ID As Integer
    ''Public ReadOnly Property o27ID As Integer
    ''    Get
    ''        Return _o27ID
    ''    End Get
    ''End Property
    ''Private Property _o27OriginalFileName As String
    ''Public ReadOnly Property o27OriginalFileName As String
    ''    Get
    ''        Return _o27OriginalFileName
    ''    End Get
    ''End Property
    Private Property _o43Attachments As String
    Public ReadOnly Property o43Attachments As String
        Get
            Return _o43Attachments
        End Get
    End Property
End Class
