Public Class p30Contact_Person
    Inherits j02Person
    Public Property p28ID As Integer
    Public Property p41ID As Integer
    Public Property j02ID As Integer
    Public Property p27ID As Integer

    Private Property _p27Name As String
    Public ReadOnly Property p27Name As String
        Get
            Return _p27Name
        End Get
    End Property

    Private Property _p28CompanyName As String
    Public ReadOnly Property p28CompanyName As String
        Get
            Return _p28CompanyName
        End Get
    End Property
    Private Property _p28Name As String
    Public ReadOnly Property p28Name As String
        Get
            Return _p28Name
        End Get
    End Property
    Private Property _p41Name As String
    Private Property _p41Code As String
    Public ReadOnly Property Project As String
        Get
            Return _p41Name & " (" & _p41Code & ")"
        End Get
    End Property
    Public Property IsSetAsDeleted As Boolean

    ''Public Property p30IsDefaultInWorksheet As Boolean
    ''Public Property p30IsDefaultInInvoice As Boolean

End Class
