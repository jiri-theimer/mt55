Public Class SysDbObject
    Public ID As Integer
    Public Name As String
    Public Content As String
    Public xType As String
    Public crDate As Date
End Class


Public Class TableColumn
    Public ID As Integer
    Public TableName As String
    Public Name As String
    Public DBType As String
    Public IsNullable As Boolean
    Public IsComputed As Boolean
    Public Formula As String
    Public Size As Integer
End Class
