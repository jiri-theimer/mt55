Public Interface Ip95InvoiceRowBL
    Inherits IFMother
    Function Save(cRec As BO.p95InvoiceRow) As Boolean
    Function Load(intPID As Integer) As BO.p95InvoiceRow
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p95InvoiceRow)
End Interface
Class p95InvoiceRowBL
    Inherits BLMother
    Implements Ip95InvoiceRowBL
    Private WithEvents _cDL As DL.p95InvoiceRowDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p95InvoiceRowDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p95InvoiceRow) As Boolean Implements Ip95InvoiceRowBL.Save
        With cRec
            If Trim(.p95Name) = "" Then _Error = "Chybí název!" : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p95InvoiceRow Implements Ip95InvoiceRowBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip95InvoiceRowBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p95InvoiceRow) Implements Ip95InvoiceRowBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
