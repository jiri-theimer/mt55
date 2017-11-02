Public Class p31DataSourceForTimeline
    Public Property j02ID As Integer
    Public Property p41ID As Integer
    Public Property p28ID As Integer
    Public Property Project As String
    Public Property Client As String
    Public Property p34ID As Integer
    Public Property p32ID As Integer
    Public Property p32Name As String
    Public Property p34Name As String
    Public Property p34Color As String
    Public Property p32Color As String
    Public Property p31Date As Date
    Public Property Hours_Orig As Double
    Public Property p31ID_max As Integer
    Public Property p31ID_min As Integer

    Public ReadOnly Property ClientAndProject As String
        Get
            If Me.Client = "" Then
                Return Me.Project
            Else
                Return Me.Client & " - " & Me.Project
            End If
        End Get
    End Property
End Class
