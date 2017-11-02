Public Enum b06NomineeFlagENUM
    _AllPersons = 0
    MastersOnly = 1
End Enum
Public Class b06WorkflowStep
    Inherits BOMother
    Public Property b02ID As Integer
    Public Property b02ID_Target As Integer
    Public Property f02ID As Integer
    Public Property b06Code As String
    Public Property b06Name As String
    Public Property b06IsManualStep As Boolean
    Public Property b06Ordinary As Integer
    Public Property b06IsCommentRequired As Boolean
    Public Property b06IsKickOffStep As Boolean
    Public Property b06IsNominee As Boolean
    Public Property b06IsNomineeRequired As Boolean
    Public Property x67ID_Nominee As Integer
    Public Property b06NomineeFlag As b06NomineeFlagENUM = b06NomineeFlagENUM._AllPersons
    Public Property x67ID_Direct As Integer
    Public Property j11ID_Direct As Integer
    Public Property b02ID_LastReceiver_ReturnTo As Integer
    Public Property b06RunSQL As String
    Public Property b06ValidateBeforeRunSQL As String
    Public Property b06ValidateAutoMoveSQL As String
    Public Property b06ValidateBeforeErrorMessage As String
    Public Property b06IsRunOneInstanceOnly As Boolean  'zda je povoleno spustit krok pouze jednou
 


    Private Property _TargetStatus As String

    Public ReadOnly Property TargetStatus As String
        Get
            Return _TargetStatus
        End Get
    End Property

    Public ReadOnly Property NameWithTargetStatus As String
        Get
            If Me.TargetStatus <> "" Then
                Return Me.b06Name & " -> " & Me.TargetStatus
            Else
                Return Me.b06Name
            End If

        End Get
    End Property

    Private Property _x29ID As Integer
    Public ReadOnly Property x29id As BO.x29IdEnum
        Get
            Return CType(_x29ID, BO.x29IdEnum)
        End Get
    End Property

    Private Property _o40ID As Integer
    Public ReadOnly Property o40ID As Integer
        Get
            Return _o40ID
        End Get
    End Property
End Class
