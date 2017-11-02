Public Class j61TextTemplate
    Inherits BOMother

    Public Property j02ID_Owner As Integer
    Public Property j61Name As String
    Public Property x29ID As BO.x29IdEnum
    Public Property j61HtmlBody As String
    Public Property j61PlainTextBody As String
    Public Property j61MailSubject As String
    Public Property j61Ordinary As Integer

    Public Property j61MailTO As String
    Public Property j61MailCC As String
    Public Property j61MailBCC As String

    Private Property _x29Name As String
    Public ReadOnly Property x29Name As String
        Get
            Return _x29Name
        End Get
    End Property
    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property
End Class
