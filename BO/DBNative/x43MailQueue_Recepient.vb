Public Enum x43RecipientIdEnum
    recTO = 1
    recCC = 2
    recBCC = 3
End Enum
Public Class x43MailQueue_Recipient
    Public Property x40ID As Integer
    Public Property x43DisplayName As String
    Public Property x43Email As String
    Public Property x43RecipientFlag As x43RecipientIdEnum


    'Public Sub New(Optional strEmail As String = "", Optional strDisplayName As String = Nothing, Optional recipientType As x43RecipientIdEnum = x43RecipientIdEnum.recTO)
    '    If Not String.IsNullOrEmpty(strEmail) Then
    '        Me.x43Email = strEmail
    '    End If

    '    If Not String.IsNullOrEmpty(strDisplayName) Then
    '        Me.x43DisplayName = strDisplayName
    '    End If

    '    Me.x43RecipientFlag = recipientType

    'End Sub
End Class
