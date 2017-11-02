Public Enum TimeFormat_ReadENUM
    HHmm = 1
    Decadic = 2
End Enum
Public Class j03UserSYS
    Inherits j03User

    Public Property j04IsMenu_Worksheet As Boolean
    Public Property j04IsMenu_Project As Boolean
    Public Property j04IsMenu_Contact As Boolean
    Public Property j04IsMenu_People As Boolean
    Public Property j04IsMenu_Report As Boolean
    Public Property j04IsMenu_Invoice As Boolean
    Public Property j04IsMenu_Proforma As Boolean
    Public Property j04IsMenu_Notepad As Boolean
    Public Property j04IsMenu_Task As Boolean
    Public Property j04IsMenu_MyProfile As Boolean
    Public Property j60ID As Integer        'ID hlavního menu

    Private Property _IsApprovingPerson As Boolean    'detekce, zda uživatel může potenciálně schvalovat nějaký worksheet
    Private Property _IsMasterPerson As Boolean       'detekce, zda osoba má pod sebou nějaké podřízené
    
    Private Property _RoleValue As String
    Private Property _PersonalPage As String
    Public Property OneProjectPage As String
    Public Property OneContactPage As String
    Public Property OneInvoicePage As String
    Public Property OnePersonPage As String

    Private Property _MessagesCount As Integer  'počet zpráv, na které systém upozorňuje uživatele
    Private Property _j11IDs As String          'seznam týmů osoby

    Public Property ExplicitConnectString   'pro předávání jiného db connect stringu

   
    Public ReadOnly Property IsApprovingPerson As Boolean
        Get
            Return _IsApprovingPerson
        End Get
    End Property
    Public ReadOnly Property IsMasterPerson As Boolean
        Get
            Return _IsMasterPerson
        End Get
    End Property
    Public ReadOnly Property PersonalPage As String
        Get
            Return _PersonalPage
        End Get
    End Property
    

    Public ReadOnly Property RoleValue As String
        Get
            Return _RoleValue
        End Get
    End Property
    Public ReadOnly Property IsAdmin As Boolean
        Get
            If _RoleValue.Substring(BO.x53PermValEnum.GR_Admin - 1, 1) = "1" Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property TimeFormat4Read As TimeFormat_ReadENUM
        Get
            Return TimeFormat_ReadENUM.Decadic
        End Get
    End Property

    
    Public ReadOnly Property MessagesCount As Integer
        Get
            Return _MessagesCount
        End Get
    End Property
    Public ReadOnly Property j11IDs As String
        Get
            Return _j11IDs
        End Get
    End Property
End Class
