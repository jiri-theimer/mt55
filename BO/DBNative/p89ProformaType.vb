Public Class p89ProformaType
    Inherits BOMother
    Public Property p93ID As Integer
    Public Property x31ID As Integer
    Public Property x31ID_Payment As Integer
    Public Property j27ID As Integer
    Public Property j19ID As Integer
    Public Property p89Name As String
    Public Property p89Code As String
    Public Property p89IsDefault As Boolean
    Public Property x38ID As Integer
    Public Property x38ID_Draft As Integer
    Public Property x38ID_Payment As Integer
    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property

    Private Property _p93Name As String
    Public ReadOnly Property p93Name As String
        Get
            Return _p93Name
        End Get
    End Property
End Class
