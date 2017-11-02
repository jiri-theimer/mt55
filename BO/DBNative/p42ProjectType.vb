Public Enum p42ArchiveFlagENUM
    NoLimit = 0
    NoArchive_Waiting_Invoice = 1
    NoArchive_Waiting_Approve = 2
End Enum
Public Enum p42ArchiveFlagP31ENUM
    EditingOnly = 1   'pouze rozpracované úkony
    EditingOrApproved = 2 'rozpracované nebo schválené
    NoRecords = 3           'žádné
End Enum
Public Class p42ProjectType
    Inherits BOMother
    Public Property b01ID As Integer
    Public Property f02ID As Integer
    Public Property x38ID As Integer
    Public Property x38ID_Draft As Integer
    Public Property p42Name As String
    Public Property p42Code As String
    Public Property p42IsDefault As Boolean
    Public Property p42Ordinary As Integer

    Public Property p42ArchiveFlag As p42ArchiveFlagENUM = p42ArchiveFlagENUM.NoLimit
    Public Property p42ArchiveFlagP31 As p42ArchiveFlagP31ENUM = p42ArchiveFlagP31ENUM.EditingOrApproved

    Public Property p42IsModule_p31 As Boolean
    Public Property p42IsModule_o23 As Boolean
    Public Property p42IsModule_p56 As Boolean
    Public Property p42IsModule_p45 As Boolean
    Public Property p42IsModule_o22 As Boolean
    Public Property p42IsModule_p48 As Boolean
    Public Property p42SubgridO23Flag As Integer = 0

    Private Property _b01Name As String
    Public ReadOnly Property b01Name As String
        Get
            Return _b01Name
        End Get
    End Property
End Class
