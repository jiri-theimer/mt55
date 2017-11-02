Public Enum o28EntryFlagENUM
    NemaPravoZapisovatWorksheet = 0
    ZapisovatDoProjektuIDoUloh = 1
    'ZapisovatDoProjektuADoUlohKdeJeResitelem = 2
    'ZapisovatPouzeDoUlohKdeJeResitelem = 3
    ZapisovatDoProjektuNadrizenym = 4
End Enum
Public Enum o28PermFlagENUM
    PouzeVlastniWorksheet = 0
    CistVseVProjektu = 1
    CistAEditVProjektu = 2
    CistASchvalovatVProjektu = 3
    CistAEditASchvalovatVProjektu = 4
End Enum

Public Class o28ProjectRole_Workload
    Inherits BOMotherNN
    Public Property p34ID As Integer
    Public Property x67ID As Integer
    Public Property o28EntryFlag As o28EntryFlagENUM
    Public Property o28PermFlag As o28PermFlagENUM

    Private Property _x67Name As String
    Public ReadOnly Property x67Name As String
        Get
            Return _x67Name
        End Get
    End Property

    Private Property _p34Name As String
    Public ReadOnly Property p34Name As String
        Get
            Return _p34Name
        End Get
    End Property
End Class
