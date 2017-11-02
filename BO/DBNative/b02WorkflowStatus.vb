Public Class b02WorkflowStatus
    Inherits BOMother
    Public Property b01ID As Integer
    Public Property b02Name As String
    Public Property b02Code As String
    Public Property b02Color As String
    Public Property b02IsRecordReadOnly4Owner As Boolean

    Public Property b02Ordinary As Integer

    Public Property b02IsDurationSLA As Boolean
    Public Property b02TimeOut_Total As Integer       'timeout v celkových hodinách trvání
    Public Property b02TimeOut_SLA As Integer       'timeout v SLA hodinách trvání

    Private Property _x29ID As Integer
    Public ReadOnly Property x29ID As BO.x29IdEnum
        Get
            Return CType(_x29ID, BO.x29IdEnum)
        End Get
    End Property
    Private Property _b01Name As String
    Public ReadOnly Property NameWithb01Name As String
        Get
            Return _b01Name & " - " & Me.b02Name
        End Get
    End Property
End Class
