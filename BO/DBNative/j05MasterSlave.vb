Public Enum j05Disposition_p31ENUM
    _NotSpecified = 0
    Cist = 1
    CistAEdit = 2
    CistASchvalovat = 3
    CistAEditASchvalovat = 4
End Enum
Public Enum j05Disposition_p48ENUM
    _NotSpecified = 0
    Cist = 1
    CistAEdit = 2
End Enum
Public Class j05MasterSlave
    Inherits BOMother
    Public Property j02ID_Master As Integer
    Public Property j02ID_Slave As Integer
    Public Property j11ID_Slave As Integer
    Public Property j05Disposition_p31 As j05Disposition_p31ENUM = j05Disposition_p31ENUM._NotSpecified
    Public Property j05IsCreate_p31 As Boolean
    Public Property j05Disposition_p48 As j05Disposition_p48ENUM = j05Disposition_p48ENUM._NotSpecified
    Public Property j05IsCreate_p48 As Boolean



    Private Property _PersonMaster As String
    Public ReadOnly Property PersonMaster As String
        Get
            Return _PersonMaster
        End Get
    End Property
    Private Property _PersonSlave As String
    Public ReadOnly Property PersonSlave As String
        Get
            Return _PersonSlave
        End Get
    End Property
    Private Property _TeamSlave As String
    Public ReadOnly Property TeamSlave As String
        Get
            Return _TeamSlave
        End Get
    End Property

    
End Class
