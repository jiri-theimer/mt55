Public Class j62MenuHome
    Inherits BOMother
    Public Property j60ID As Integer
    Public Property j62ParentID As Integer
    Public Property j62Name As String
    Public Property j62Name_ENG As String
    Public Property x29ID As BO.x29IdEnum
    Public Property j70ID As Integer
    Public Property x31ID As Integer
    Public Property j62Url As String
    Public Property j62Target As String
    Public Property j62GridGroupBy As String
    Public Property j62Ordinary As Integer
    Public Property j62IsSeparator As Boolean
    Public Property j62ImageUrl As String
    Public Property j62Tag As String

    Private Property _j62TreeIndex As Integer
    Public ReadOnly Property j62TreeIndex As Integer
        Get
            Return _j62TreeIndex
        End Get
    End Property
    Private Property _j62TreePrev As Integer
    Public ReadOnly Property j62TreePrev As Integer
        Get
            Return _j62TreePrev
        End Get
    End Property
    Private Property _j62TreeNext As Integer
    Public ReadOnly Property j62TreeNext As Integer
        Get
            Return _j62TreeNext
        End Get
    End Property
    Private Property _j62TreeLevel As Integer
    Public ReadOnly Property j62TreeLevel As Integer
        Get
            Return _j62TreeLevel
        End Get
    End Property

    Public ReadOnly Property TreeMenuItem As String
        Get
            If _j62TreeLevel <= 1 Then
                Return Me.j62Name
            Else
                Return Space(_j62TreeLevel * 3).Replace(" ", "-") + Me.j62Name
            End If
        End Get
    End Property
    Private Property _x29Name As String
    Public ReadOnly Property x29Name As String
        Get
            If Me.x29ID = x29IdEnum.j03User Then Return ""
            Return _x29Name
        End Get
    End Property

End Class
