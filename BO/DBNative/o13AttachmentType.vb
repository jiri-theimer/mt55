Public Class o13AttachmentType
    Inherits BOMother
    Public Property x29ID As x29IdEnum
    Public Property o13Name As String
    Public Property o13IsUniqueArchiveFileName As Boolean
    Public Property o13IsArchiveFolderWithPeriodSuffix As Boolean
    Public Property o13ArchiveFolder As String

    Private Property _x29Name As String
    Public ReadOnly Property x29Name As String
        Get
            Return _x29Name
        End Get
    End Property
End Class
