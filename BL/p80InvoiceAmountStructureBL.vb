Public Interface Ip80InvoiceAmountStructureBL
    Inherits IFMother
    Function Save(cRec As BO.p80InvoiceAmountStructure) As Boolean
    Function Load(intPID As Integer) As BO.p80InvoiceAmountStructure
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p80InvoiceAmountStructure)

End Interface
Class p80InvoiceAmountStructureBL
    Inherits BLMother
    Implements Ip80InvoiceAmountStructureBL
    Private WithEvents _cDL As DL.p80InvoiceAmountStructureDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p80InvoiceAmountStructureDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p80InvoiceAmountStructure) As Boolean Implements Ip80InvoiceAmountStructureBL.Save
        With cRec
            If Trim(.p80Name) = "" Then _Error = "Chybí název." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p80InvoiceAmountStructure Implements Ip80InvoiceAmountStructureBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip80InvoiceAmountStructureBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p80InvoiceAmountStructure) Implements Ip80InvoiceAmountStructureBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
