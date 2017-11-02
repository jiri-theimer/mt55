
Public Class j04UserRole
    Inherits BOMother
    Public Property j60ID As Integer
    Public Property j04Name As String
    Public Property j04Aspx_PersonalPage As String
    Public Property j04Aspx_PersonalPage_Mobile As String
    Public Property j04Aspx_OneProjectPage As String
    Public Property j04Aspx_OneContactPage As String
    Public Property j04Aspx_OneInvoicePage As String
    Public Property j04Aspx_OnePersonPage As String

    Public Property x67ID As Integer
    Public Property j04IsMenu_Worksheet As Boolean
    Public Property j04IsMenu_Project As Boolean
    Public Property j04IsMenu_Contact As Boolean
    Public Property j04IsMenu_People As Boolean
    Public Property j04IsMenu_Report As Boolean
    Public Property j04IsMenu_Invoice As Boolean
    Public Property j04IsMenu_Proforma As Boolean
    Public Property j04IsMenu_MyProfile As Boolean = True
    Public Property j04IsMenu_Task As Boolean
    Public Property j04IsMenu_More As Boolean = True
    Public Property j04IsMenu_Notepad As Boolean
    Public Property j04DashboardHtml As String  'na míru vytvořené HTML do dashboard stránky j03_mypage_greeting.aspx
    Private Property _x67RoleValue As String


    Public ReadOnly Property RoleValue As String
        Get
            Return _x67RoleValue
        End Get
    End Property

    Public Function TestPermission(intNeededPermissionValue As BO.x53PermValEnum) As Boolean
        If Mid(_x67RoleValue, intNeededPermissionValue, 1) = "1" Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
