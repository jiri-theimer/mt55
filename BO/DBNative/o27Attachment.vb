Public Class o27Attachment
    Inherits BOMother
    Public Property o13ID As Integer
    
    Public Property x31ID As Integer
    Public Property x40ID As Integer
    Public Property x50ID As Integer
   
    Public Property b07ID As Integer

    Public Property o27Name As String
    Public Property o27OriginalFileName As String

    Public Property o27ContentType As String
    Public Property o27GUID As String
    
    Private Property _o13Name As String
    Private Property _o13ArchiveFolder As String
    Private Property _o27ArchiveFileName As String
    Private Property _o27FileExtension As String
    Private Property _o27ArchiveFolder As String
    Public Property Version As Integer      'Pracovní atribut kvůli dynamickému on-fly verze souboru
    Public Property ShowVersion As Boolean  'Pracovní atribut kvůli dynamickému on-fly verze souboru
    Private Property _o27FileSize As Integer

    Public ReadOnly Property o27FileSize As Integer
        Get
            Return _o27FileSize
        End Get
    End Property

    Public ReadOnly Property o27ArchiveFolder As String
        Get
            Return _o27ArchiveFolder
        End Get
    End Property

    Public ReadOnly Property o13Name As String
        Get
            Return _o13Name
        End Get
    End Property
    Public ReadOnly Property o27ArchiveFileName As String
        Get
            Return _o27ArchiveFileName
        End Get
    End Property
    Public ReadOnly Property o27FileExtension As String
        Get
            Return _o27FIleExtension
        End Get
    End Property
    Public ReadOnly Property o13ArchiveFolder As String
        Get
            Return _o13ArchiveFolder
        End Get
    End Property
    Public Function GetFullPath(strRootFolder As String) As String
        If Me.o27ArchiveFolder = "" Then
            Return strRootFolder & "\" & Me.o27ArchiveFileName
        Else
            Return strRootFolder & "\" & Me.o27ArchiveFolder & "\" & Me.o27ArchiveFileName
        End If
    End Function
End Class
