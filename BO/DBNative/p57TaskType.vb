Public Class p57TaskType
    Inherits BOMother
    Public Property b01ID As Integer
    Public Property x38ID As Integer
    Public Property p57Name As String
    Public Property p57Code As String
    Public Property p57IsDefault As Boolean
    Public Property p57IsHelpdesk As Boolean
    Public Property p57Ordinary As Integer

    Public Property p57IsEntry_Receiver As Boolean
    Public Property p57IsEntry_Budget As Boolean
    Public Property p57IsEntry_Priority As Boolean
    Public Property p57IsEntry_CompletePercent As Boolean
    Public Property p57Caption_PlanFrom As String
    Public Property p57Caption_PlanUntil As String

    Public Property p57PlanDatesEntryFlag As Integer
    '0 (NULL) - Termín není povinný, plánované zahájení se nabízí k vyplnění
    '1-Termín je povinné zadat
    '2-Nenabízet plánované zahájení
    '3-Plánované zahájení je povinné zadat
    '4-Plánované zahájení je povinné a v kalendáři zobrazovat jako událost od-do


    Private Property _b01Name As String
    Public ReadOnly Property b01Name As String
        Get
            Return _b01Name
        End Get
    End Property
End Class
