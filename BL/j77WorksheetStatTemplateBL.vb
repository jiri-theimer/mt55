Public Interface Ij77WorksheetStatTemplateBL
    Inherits IFMother
    Function Save(cRec As BO.j77WorksheetStatTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
    Function Load(intPID As Integer) As BO.j77WorksheetStatTemplate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(myQuery As BO.myQuery) As IEnumerable(Of BO.j77WorksheetStatTemplate)


    Function ColumnsPallete() As List(Of BO.PivotSumField)
End Interface
Class j77WorksheetStatTemplateBL
    Inherits BLMother
    Implements Ij77WorksheetStatTemplateBL
    Private WithEvents _cDL As DL.j77WorksheetStatTemplateDL
    Private _x29id As BO.x29IdEnum

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j77WorksheetStatTemplateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function Delete(intPID As Integer) As Boolean Implements Ij77WorksheetStatTemplateBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(myQuery As BO.myQuery) As System.Collections.Generic.IEnumerable(Of BO.j77WorksheetStatTemplate) Implements Ij77WorksheetStatTemplateBL.GetList
        Return _cDL.GetList(myQuery)
    End Function


    Public Function Load(intPID As Integer) As BO.j77WorksheetStatTemplate Implements Ij77WorksheetStatTemplateBL.Load
        Return _cDL.Load(intPID)
    End Function
    
    Public Function Save(cRec As BO.j77WorksheetStatTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ij77WorksheetStatTemplateBL.Save
        With cRec
            If .j03ID = 0 Then .j03ID = _cUser.PID
            If .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
            If Trim(.j77Name) = "" Then _Error = "Chybí název šablony."
        End With

        If _Error <> "" Then Return False

        Return _cDL.Save(cRec, lisX69)

    End Function

    Public Function ColumnsPallete() As List(Of BO.PivotSumField) Implements Ij77WorksheetStatTemplateBL.ColumnsPallete
        Dim bolHideRatesColumns As Boolean = Not Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates)   'zda uživatel nemá právo vidět sazby a fakturační údaje

        Dim lis As New List(Of BO.PivotSumField)

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Orig))
        If Not bolHideRatesColumns Then
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_Orig))
        End If
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_WIP))
        If Not bolHideRatesColumns Then
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Orig))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_WIP))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_WIP))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Approved))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.NI_HoursFee))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Invoiced))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Invoiced_Domestic))
        End If

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Approved_Billing))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.NI_Hours))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Approved_FixedPrice))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Approved_WriteOff))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Approved_InvoiceLater))

        If Not bolHideRatesColumns Then
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_Approved))
        End If

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Invoiced))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Invoiced_FixedPrice))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Invoiced_WriteOff))

        If Not bolHideRatesColumns Then
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_Invoiced))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_Invoiced_Domestic))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_WithoutVat_FixedCurrency))

            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Fees))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Fees_WIP))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Fees_Approved))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.NI_Fees))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Fees_Invoiced))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Fees_Invoiced_Domestic))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Expenses))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Expenses_WIP))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Expenses_Approved))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.NI_Expenses))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Expenses_Invoiced))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.Expenses_Invoiced_Domestic))

            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Internal))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_HoursFee_Internal_Approved))

            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_Approved_Internal))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Amount_Internal_Approved))
        End If

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Hours_BIN))

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Value_Orig))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Value_Approved_Billing))
        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Value_Invoiced))

        lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.p31Value_FixPrice))

        If Not bolHideRatesColumns Then
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.SI_Income1))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.SI_Profit1))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.SI_Profit2))
            lis.Add(New BO.PivotSumField(BO.PivotSumFieldType.SI_Profit3))
        End If
        Return lis

    End Function

End Class
