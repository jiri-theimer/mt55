Public Class FreeField
    Inherits x28EntityField
    Public Property DBValue As Object
    Public Property ComboText As String
    Public Property IsExternalDataSource As Boolean

    Public Sub SetTypeFromName(strTypeName As String)
        _TypeName = strTypeName
        Select Case LCase(strTypeName)
            Case "boolean"
                Me.x24ID = x24IdENUM.tBoolean
            Case "date"
                Me.x24ID = x24IdENUM.tDate
            Case "datetime"
                Me.x24ID = x24IdENUM.tDateTime
            Case "decimal"
                Me.x24ID = x24IdENUM.tDecimal
            Case "integer"
                Me.x24ID = x24IdENUM.tInteger
            Case Else
                Me.x24ID = x24IdENUM.tString
        End Select
    End Sub
End Class
