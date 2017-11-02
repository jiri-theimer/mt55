Public Class p90Proforma
    Inherits BOMother
    Public Property j27ID As Integer
    Public Property p89ID As Integer
    Public Property p28ID As Integer
    Public Property j19ID As Integer
    Public Property j02ID_Owner As Integer
    Public Property p90Code As String
    Public Property p90IsDraft As Boolean
    Public Property p90Date As Date
    Public Property p90DateBilled As Date?
    Public Property p90DateMaturity As Date?
    Public Property p90Amount_WithoutVat As Double
    Public Property p90Amount_Vat As Double
    Public Property p90Amount_Billed As Double
    Public Property p90VatRate As Double
    Public Property p90Amount As Double
    Public Property p90Amount_Debt As Double
    Public Property p90Text1 As String
    Public Property p90Text2 As String
    Public Property p90TextDPP As String
    Public Property TagsInlineHtml As String

    Private Property _p82ID As Integer
    Public ReadOnly Property p82ID As Integer
        Get
            Return _p82ID
        End Get
    End Property
    Private Property _p82Code As String
    Public ReadOnly Property p82Code As String
        Get
            Return _p82Code
        End Get
    End Property

    Private Property _j27Code As String
    Public ReadOnly Property j27Code As String
        Get
            Return _j27Code
        End Get
    End Property

    Private Property _p28Name As String
    Public ReadOnly Property p28Name As String
        Get
            Return _p28Name
        End Get
    End Property

    Private Property _p89Name As String
    Public ReadOnly Property p89Name As String
        Get
            Return _p89Name
        End Get
    End Property

    Private Property _Owner As String
    Public ReadOnly Property Owner As String
        Get
            Return _Owner
        End Get
    End Property

    Public ReadOnly Property CodeWithClient As String
        Get
            Return Me.p90Code & " - " & _p28Name
        End Get
    End Property
End Class
