Public Class p38ActivityTag
    Inherits BOMother
    Public Property p38Name As String
    Public Property p38Code As String
    Public Property p38Ordinary As Integer

    Public ReadOnly Property CodeWithName As String
        Get
            If Me.p38Code = "" Then
                Return Me.p38Name
            Else
                Return Me.p38Code & " " & Me.p38Name
            End If
        End Get
    End Property
End Class
