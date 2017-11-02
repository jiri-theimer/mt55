
Public Class b05Workflow_History
    Inherits BOMother
    Public Property j03ID_Sys As Integer
    Public Property b05RecordPID As Integer
    Public x29ID As BO.x29IdEnum
    Public Property b06ID As Integer
    Public Property b02ID_From As Integer
    Public Property b02ID_To As Integer
    Public Property b07ID As Integer
    Public Property b05IsManualStep As Boolean
    Public Property b05IsCommentOnly As Boolean
    Public Property b05Comment As String
    Public Property b05ErrorMessage As String
    Public Property b05SQL As String

    Private Property _b06Name As String
    Private Property _b02name_from As String
    Private Property _b02name_to As String

    Private Property _j03login As String
    Private Property _j02lastname As String
    Private Property _j02firstname As String
    Private Property _o27ID As Integer


    Public ReadOnly Property b06Name As String
        Get
            Return _b06Name
        End Get
    End Property
  
    Public ReadOnly Property Person As String
        Get
            If _j02lastname = "" Then
                Return _j03login
            Else
                Return _j02lastname & " " & _j02firstname
            End If
        End Get
    End Property

    Public ReadOnly Property StatusMove As String
        Get
            If Me.b02ID_From = Me.b02ID_To Then
                Return ""
            Else
                Return _b02name_from & " -> " & _b02name_to
            End If
        End Get
    End Property
    Public ReadOnly Property StatusMoveHtml As String
        Get
            If Me.b02ID_From = Me.b02ID_To Then
                Return ""
            Else
                Return _b02name_from & " -> <span style='color:red;'>" & _b02name_to & "</span>"
            End If
        End Get
    End Property
    Public ReadOnly Property b02Name_From As String
        Get
            Return _b02name_from
        End Get
    End Property
    Public ReadOnly Property b02Name_To As String
        Get
            Return _b02name_to
        End Get
    End Property
   
   
    Public ReadOnly Property o27ID As Integer
        Get
            Return _o27ID
        End Get
    End Property
   
End Class
