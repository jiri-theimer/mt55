Public Enum ModuleEnum
    System = 1
    Billing = 2
    Worksheet = 3
    SMTP = 4
    Other = 99
End Enum
Public Enum UserAuthenticationModeEnum
    MixedMode = 1
    WindowsOnly = 2
    AnonymousOnly = 3
End Enum

Public Class x35GlobalParam
    Inherits BOMother
    Public Property x35ModuleFlag As ModuleEnum
    Public Property x35Key As String
    Public Property x35Value As String
    Public Property x35Description As String
    Public Property x35Ordinary As Integer
End Class
