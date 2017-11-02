Public Class OtherQueryItem
    Public Property pid As Integer
    Public Property Text As String
    Public Property IsClosed As Boolean
    Public Sub New(intPID As Integer, strText As String)
        Me.pid = intPID
        Me.Text = strText
    End Sub
End Class
Public Enum j70ScrollingFlagENUM
    NoScrolling = 0
    Scrolling = 1
    StaticHeaders = 2
End Enum
Public Enum j70PageLayoutFlagENUM
    _None = 0
    LeftRight = 1
    OnlyOne = 3
    TopBottom = 2
End Enum
Public Class j70QueryTemplate
    Inherits BOMother
    Public Property j70Name As String
    Public Property x29ID As x29IdEnum
    Public Property j03ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property j70IsSystem As Boolean
    Public Property j70BinFlag As Integer
    Public Property j70IsNegation As Boolean

    Public Property j70ColumnNames As String
    Public Property j70OrderBy As String
    Public Property j70IsFilteringByColumn As Boolean
    Public Property j70ScrollingFlag As j70ScrollingFlagENUM = j70ScrollingFlagENUM.StaticHeaders
    Public Property j70MasterPrefix As String
    Public Property j70PageLayoutFlag As j70PageLayoutFlagENUM = j70PageLayoutFlagENUM._None
    Private Property _Mark As String
    Public ReadOnly Property NameWithMark As String
        Get
            ''Return Trim(j70Name & " " & _Mark)

            Select Case Me.j70PageLayoutFlag
                Case j70PageLayoutFlagENUM.OnlyOne, j70PageLayoutFlagENUM.TopBottom
                    Return RTrim("[" & j70Name & "] " & _Mark)
                Case Else
                    Return RTrim(j70Name & " " & _Mark)
            End Select
        End Get
    End Property
    Public ReadOnly Property NameWithCreator As String
        Get
            Return j70Name & " (" & Me.UserInsert & ")"
        End Get
    End Property

    
End Class
