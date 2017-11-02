Public Class BAS
    Private Shared Property _Error As String
    Public Shared ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property

    Public Shared Function ValidateFF(lisFF As List(Of BO.FreeField)) As Boolean
        Dim bolIsRequired As Boolean = False, lisX26 As IEnumerable(Of BO.x26EntityField_Binding) = Nothing
        For Each c In lisFF
            bolIsRequired = c.x28IsRequired
            If Not bolIsRequired Then
                If lisX26 Is Nothing Then

                End If
            End If
            If bolIsRequired Then
                Select Case c.x24ID
                    Case BO.x24IdENUM.tInteger, BO.x24IdENUM.tDecimal
                        If BO.BAS.IsNullNum(c.DBValue) = 0 Then
                            _Error = "Pole [" & c.x28Name & "] je povinné k vyplnění."
                            Return False
                        End If
                    Case BO.x24IdENUM.tString
                        If c.DBValue Is Nothing Then
                            _Error = "Pole [" & c.x28Name & "] je povinné k vyplnění."
                            Return False
                        End If
                        If Trim(c.DBValue.ToString) = "" Then
                            _Error = "Pole [" & c.x28Name & "] je povinné k vyplnění."
                            Return False
                        End If
                    Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime
                        If BO.BAS.IsNullDBDate(c.DBValue) Is Nothing Then
                            _Error = "Pole [" & c.x28Name & "] je povinné k vyplnění."
                            Return False
                        End If
                End Select
            End If
        Next
        Return True
    End Function

End Class
