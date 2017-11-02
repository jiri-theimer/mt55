Public Class BOMotherNN
    Protected Property _pid As Integer
  
    Public ReadOnly Property PID As Integer
        Get
            Return _pid
        End Get
    End Property

    Public Sub ClearPID()
        _pid = 0
    End Sub
    Public Sub SetPID(intNewPID As Integer)
        _pid = intNewPID
    End Sub
End Class
