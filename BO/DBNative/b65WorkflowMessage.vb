Public Class b65WorkflowMessage
    Inherits BOMother
    Public Property b01ID As Integer
    Public Property b65Name As String
    Public Property b65MessageSubject As String
    Public Property b65MessageBody As String
    
    Private Property _b01Name As String

    Public ReadOnly Property b01Name As String
        Get
            Return _b01Name
        End Get

    End Property

End Class
