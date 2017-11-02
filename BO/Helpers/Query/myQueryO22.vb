Public Enum myQueryO22_SpecificQuery
    _NotSpecified = 0
    AllowedForRead = 2              'pouze milníky, ke kterým má právo na čtení
End Enum

Public Class myQueryO22
    Inherits myQuery
    Public SpecificQuery As myQueryO22_SpecificQuery = myQueryO22_SpecificQuery._NotSpecified
    Public Property p41ID As Integer
    Public Property IsIncludeChildProjects As Boolean = False
    Public Property p28ID As Integer
    Public Property j02IDs As List(Of Integer)
    Public Property p41IDs As List(Of Integer)
    Public Property p91ID As Integer
    Public Property o21ID As Integer

   

End Class
