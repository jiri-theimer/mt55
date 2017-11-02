Public MustInherit Class BOMother
    Protected Property _pid As Integer
    Protected Property _DateUpdate As Date?
    Protected Property _DateInsert As Date?
    Protected Property _UserUpdate As String
    Protected Property _UserInsert As String
    Protected Property _ValidFrom As Date = Now
    Protected Property _ValidUntil As Date = DateSerial(3000, 1, 1)
   
    Public Property PID As Integer
        Get
            Return _pid
        End Get
        Set(value As Integer)
            'hodnota PID se nesmí přepisovat - SET je tu pouze kvůli web service
        End Set
    End Property
    
    Public Property DateUpdate As Date?
        Get
            If _DateUpdate Is Nothing Then Return _DateInsert
            Return _DateUpdate
        End Get
        Set(value As Date?)
            'SET je tu pouze kvůli web service
        End Set
    End Property
    Public Property DateInsert As Date?
        Get
            Return _DateInsert
        End Get
        Set(value As Date?)
            'SET je tu pouze kvůli web service
        End Set
    End Property
    Public Property UserUpdate As String
        Get
            If _UserUpdate = "" Then Return _UserInsert
            Return _UserUpdate
        End Get
        Set(value As String)
            'SET je tu pouze kvůli web service
        End Set
    End Property
    Public Property UserInsert As String
        Get
            Return _UserInsert
        End Get
        Set(value As String)
            'SET je tu pouze kvůli web service
        End Set
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
    Public Overridable Property IsClosed As Boolean
        Get
            If _ValidFrom <= Now And _ValidUntil >= Now Then
                Return False
            Else
                If _ValidFrom.AddMinutes(-5) <= Now And _ValidUntil >= Now Then
                    Return False    'o 5 minut to může být kvůli oddělenému DB a APP serveru
                End If
                Return True
            End If
        End Get
        Set(value As Boolean)
            'SET je tu pouze kvůli web service
        End Set
    End Property
    Public ReadOnly Property Timestamp As String
        Get
            Dim s As String = "Založeno: <span style='color:black;'>" & _UserInsert & "/" & BO.BAS.FD(_DateInsert, True) & "</span>"
            If _DateInsert < _DateUpdate Then
                s += " | Aktualizováno: <span style='color:black;'>" & _UserUpdate & "/" & BO.BAS.FD(_DateUpdate, True) & "</span>"
            End If
            Return s
        End Get
    End Property
    Public ReadOnly Property TimestampPlainText As String
        Get
            Dim s As String = "Založeno: " & _UserInsert & "/" & BO.BAS.FD(_DateInsert, True)
            If _DateInsert < _DateUpdate Then
                s += " | Poslední aktualizace: " & _UserUpdate & "/" & BO.BAS.FD(_DateUpdate, True)
            End If
            Return s
        End Get
    End Property
    Public Sub ClearPID()
        _pid = 0
    End Sub
    Public Sub SetPID(intNewPID As Integer)
        _pid = intNewPID
    End Sub
    Public Sub SetUserInsert(strNewUserInsert As String)
        _UserInsert = strNewUserInsert
        _UserUpdate = strNewUserInsert
    End Sub
End Class
