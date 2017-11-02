Public Class j90LoginAccessLog
    Public Property pid As Integer
    Public Property j90Date As Date
    Public Property j03ID As Integer
    Public Property j90IsDomainTrusted As Boolean
    Public Property j90ClientBrowser As String
    Public Property j90Platform As String
    Public Property j90UserHostAddress As String
    Public Property j90UserHostName As String
    Public Property j90AppClient As String
    Public Property j90IsMobileDevice As Boolean
    Public Property j90MobileDevice As String
    Public Property j90ScreenPixelsHeight As Integer
    Public Property j90ScreenPixelsWidth As Integer
    Public Property j90DomainUserName As String
    Public Property j90RequestURL As String

    Public ReadOnly Property ScreenResolution As String
        Get
            If j90ScreenPixelsHeight > 0 Then
                Return j90ScreenPixelsWidth.ToString & " x " & j90ScreenPixelsHeight.ToString
            Else
                Return ""
            End If
        End Get
    End Property

End Class
