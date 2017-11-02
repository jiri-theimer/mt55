Public Enum x45IDEnum
    p41_new = 14101
    p41_update = 14102
    p41_bin = 14103
    p41_restore = 14104
    p41_delete = 14105
    p41_limithours_over = 14110
    p41_limitfee_over = 14111

    p28_new = 32801
    p28_update = 32802
    p28_bin = 32803
    p28_restore = 32804
    p28_delete = 32805
    p28_limithours_over = 32810
    p28_limitfee_over = 32811

    p51_new = 35101
    p51_update = 35102
    p51_bin = 35103
    p51_restore = 35104
    p51_delete = 35105

    p36_new = 33601
    p36_update = 33602
    p36_delete = 33605

    p91_new = 39101
    p91_update = 39102
    p91_bin = 39103
    p91_restore = 39104
    p91_delete = 39105

    p90_new = 39001
    p90_update = 39002
    p90_bin = 39003
    p90_restore = 39004
    p90_delete = 39005

    p56_new = 35601
    p56_update = 35602
    p56_bin = 35603
    p56_restore = 35604
    p56_delete = 35605
    p56_remind = 35606

    o23_new = 22301
    o23_update = 22302
    o23_bin = 22303
    o23_restore = 22304
    o23_delete = 22305
    o23_remind = 22306

    o22_new = 22201
    o22_update = 22202
    o22_bin = 22303
    o22_restore = 22204
    o22_delete = 22205
    o22_remind = 22206

    j02_new = 10201
    j02_update = 10202
    j02_bin = 10203
    j02_restore = 10204
    j02_delete = 10205

    p30_new = 33001
    p30_delete = 33005

    b07_new = 60701
End Enum

Public Class x45Event
    Inherits BOMother
    Public Property x45ID As x45IDEnum
    Public Property x45Name As String
    Public Property x45Code As String
    Public Property x45MessageTemplate As String
    Public Property x45Ordinary As Integer
    Public Property x45IsAllowNotification As Boolean
    Public Property x45IsManualReceiver As Boolean
    Public Property x45IsReference As Boolean

    Public ReadOnly Property NameWithCode As String
        Get
            Return Me.x45Name & " (" & Me.x45Code & ")"
        End Get
    End Property

    Public ReadOnly Property Prefix As String
        Get
            Return Left(Me.x45ID.ToString, 3)
        End Get
    End Property
    Public ReadOnly Property x29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(Me.Prefix)
        End Get
    End Property
    Public ReadOnly Property IsDeleteEvent As Boolean
        Get
            If Me.x45ID.ToString.IndexOf("delete") > 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
End Class
