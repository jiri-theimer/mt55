Public Enum x40StateENUM
    _NotSpecified = 0
    InQueque = 1
    IsError = 2
    IsProceeded = 3
    IsStopped = 4
    WaitOnConfirm = 5
End Enum

Public Class x40MailQueue
    Inherits BOMother
    Public Property x29ID As BO.x29IdEnum
    Public Property x40State As x40StateENUM = x40StateENUM.InQueque
    Public Property x40RecordPID As Integer
    Public Property j03ID_Sys As Integer
    Public Property o40ID As Integer

    Public Property x40Subject As String
    Public Property x40Body As String
    Public Property x40IsHtmlBody As Boolean

    Public Property x40SenderName As String
    Public Property x40SenderAddress As String
    Public Property x40Recipient As String
    
    Public Property x40CC As String
    
    Public Property x40BCC As String
    
    Public Property x40Attachments As String


    Public Property x40WhenProceeded As Date?

    Public Property x40ErrorMessage As String

    Public Property x40IsAutoNotification As Boolean
    Public Property x40MessageID As String
    Public Property x40ArchiveFolder As String



    Public ReadOnly Property StatusAlias As String
        Get
            Select Case Me.x40State
                Case x40StateENUM.InQueque : Return "Odesílá se"
                Case x40StateENUM.IsError : Return "Chyba"
                Case x40StateENUM.IsProceeded : Return "Odesláno"
                Case x40StateENUM.IsStopped : Return "Zastaveno"
                Case x40StateENUM.WaitOnConfirm : Return "Čeká na odeslání"
                Case Else : Return "?"""
            End Select
        End Get
    End Property
    Public ReadOnly Property StatusColor As String
        Get
            Select Case Me.x40State
                Case x40StateENUM.InQueque : Return "#996633"
                Case x40StateENUM.IsError : Return "#ff0000"
                Case x40StateENUM.IsProceeded : Return "#008000"
                Case x40StateENUM.IsStopped : Return "#ff66ff"
                Case x40StateENUM.WaitOnConfirm : Return "#0000ff"
                Case Else : Return "?"""
            End Select
        End Get
    End Property
    Public ReadOnly Property Context As String
        Get
            Return BO.BAS.GetX29EntityAlias(x29ID, False)
        End Get
    End Property
End Class
