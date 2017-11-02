Public Class f01Folder
    Inherits BOMother
    Public Property f02ID As Integer
    Public Property f01RecordPID As Integer
    Public Property f01Name As String

    Private Property _f02Name As String
    Public ReadOnly Property f02Name As String
        Get
            Return _f02Name
        End Get
    End Property
    Private Property _f02SysFlag As BO.f02SysFlagENUM
    Public ReadOnly Property f02SysFlag As BO.f02SysFlagENUM
        Get
            Return _f02SysFlag
        End Get
    End Property
    Private Property _f02RootPath As String
    Public ReadOnly Property f02RootPath As String
        Get
            Return _f02RootPath
        End Get
    End Property
End Class
