Public Enum x38EditModeFlagENUM
    NotEditable = 1
    RecordOwnerOnly = 2
    AdminOnly = 3
End Enum
Public Class x38CodeLogic
    Inherits BOMother
    Public Property x29ID As x29IdEnum
    Public Property x38Name As String
    Public Property x38EditModeFlag As x38EditModeFlagENUM = x38EditModeFlagENUM.AdminOnly
    Public Property x38MaskSyntax As String
    Public Property x38ConstantBeforeValue As String
    Public Property x38ConstantAfterValue As String
    Public Property x38Scale As Integer
    Public Property x38IsDraft As Boolean
    Public Property x38Description As String
    Public Property x38ExplicitIncrementStart As Integer
    Public Property x38IsUseDbPID As Boolean



    Public ReadOnly Property CodeLogicInfo As String
        Get
            If Me.x38IsUseDbPID Then Return ""

            If Me.x38MaskSyntax = "" Then
                Return Me.x38ConstantBeforeValue & Right("000000001", Me.x38Scale) & " - " & Me.x38ConstantBeforeValue & Right("99999999999", Me.x38Scale)
            Else
                Return "Specifické pravidlo pro generování kódu záznamu"
            End If

        End Get
    End Property
End Class
