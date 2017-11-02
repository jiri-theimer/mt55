Public Interface Ip92InvoiceTypeBL
    Inherits IFMother
    Function Save(cRec As BO.p92InvoiceType) As Boolean
    Function Load(intPID As Integer) As BO.p92InvoiceType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p92InvoiceType)

End Interface
Class p92InvoiceTypeBL
    Inherits BLMother
    Implements Ip92InvoiceTypeBL
    Private WithEvents _cDL As DL.p92InvoiceTypeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p92InvoiceTypeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p92InvoiceType) As Boolean Implements Ip92InvoiceTypeBL.Save
        With cRec
            If Trim(.p92Name) = "" Then _Error = "Chybí název typu." : Return False
            If .x38ID = 0 Then _Error = "Chybí specifikace číselné řady." : Return False
            If .j27ID = 0 Then _Error = "Chybí specifikace výchozí měny faktury." : Return False
        End With

        If _cDL.Save(cRec) Then
            Factory.p53VatRateBL.TestAndSetupVatExistence(cRec.x15ID, cRec.j27ID)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.p92InvoiceType Implements Ip92InvoiceTypeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip92InvoiceTypeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p92InvoiceType) Implements Ip92InvoiceTypeBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
