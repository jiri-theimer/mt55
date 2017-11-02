Public Class BOMotherFT
    Protected Property _pid As Integer
    Protected _ValidFrom As Date = Now
    Protected _ValidUntil As Date = DateSerial(3000, 1, 1)

    Public ReadOnly Property PID As Integer
        Get
            Return _pid
        End Get
    End Property
    Public Property ValidFrom As Date
        Get
            Return _ValidFrom
        End Get
        Set(value As Date)
            _ValidFrom = value
        End Set
    End Property
    Public Property ValidUntil As Date
        Get
            Return _ValidUntil
        End Get
        Set(value As Date)
            _ValidUntil = value
        End Set
    End Property
    Public Overridable ReadOnly Property IsClosed As Boolean
        Get
            If _ValidFrom <= Now And _ValidUntil >= Now Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Public Sub ClearPID()
        _pid = 0
    End Sub
    Public Sub SetPID(intNewPID As Integer)
        _pid = intNewPID
    End Sub
End Class
