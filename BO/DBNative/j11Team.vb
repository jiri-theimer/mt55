Public Class j11Team
    Inherits BOMother
    Public Property j11Name As String
    Public Property j11IsAllPersons As Boolean
    Public Property j11Email As String
    Public Property j11DomainAccount As String
    Public Property j11RobotAddress As String
    Public ReadOnly Property NameWithEmail As String
        Get
            If Me.j11Email = "" Then
                Return Me.j11Name
            Else
                Return Me.j11Name & " (" & Me.j11Email & ")"
            End If
        End Get
    End Property
End Class
