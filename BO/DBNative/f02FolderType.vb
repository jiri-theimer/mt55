Public Enum f02SysFlagENUM
    FileSystem = 1
    Alfresco = 2
    DropBox = 3
    OneDrive = 4
End Enum

Public Class f02FolderType
    Inherits BOMother
    Public Property f02Name As String
    Public Property f02SysFlag As f02SysFlagENUM = f02SysFlagENUM.FileSystem
    Public Property x29ID As BO.x29IdEnum = x29IdEnum._NotSpecified
    Public Property f02ParentID As Integer
    Public Property f02Mask As String
    Public Property f02RootPath As String

End Class
