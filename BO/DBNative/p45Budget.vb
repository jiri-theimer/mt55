Public Class p45Budget
    Inherits BOMother
    Public Property p45Name As String
    Public Property p41ID As Integer
    Public Property p45VersionIndex As Integer
    Public Property p45PlanFrom As Date
    Public Property p45PlanUntil As Date

    Public ReadOnly Property VersionWithName As String
        Get
            Dim strOcas As String = ""
            If Now >= Me.ValidFrom And Now <= Me.ValidUntil Then strOcas = " (aktuální verze)"
            If Me.p45Name = "" Then
                Return String.Format("#{0}", Me.p45VersionIndex.ToString) & strOcas
            Else
                Return String.Format("#{0} [{1}]", Me.p45VersionIndex.ToString, Me.p45Name) & strOcas
            End If
        End Get
    End Property
End Class
