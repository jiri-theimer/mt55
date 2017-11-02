Public Interface Ip93InvoiceHeaderBL
    Inherits IFMother
    Function Save(cRec As BO.p93InvoiceHeader, lisP88 As List(Of BO.p88InvoiceHeader_BankAccount)) As Boolean
    Function Load(intPID As Integer) As BO.p93InvoiceHeader
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p93InvoiceHeader)
    Function GetList_p88(intPID As Integer) As IEnumerable(Of BO.p88InvoiceHeader_BankAccount)

End Interface


Class p93InvoiceHeaderBL
    Inherits BLMother
    Implements Ip93InvoiceHeaderBL
    Private WithEvents _cDL As DL.p93InvoiceHeaderDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p93InvoiceHeaderDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p93InvoiceHeader, lisP88 As List(Of BO.p88InvoiceHeader_BankAccount)) As Boolean Implements Ip93InvoiceHeaderBL.Save
        With cRec
            If Trim(.p93Name) = "" Then _Error = "Chybí název hlavičky." : Return False

        End With
        If lisP88.Where(Function(p) p.j27ID = 0 Or p.p86ID = 0).Count > 0 Then
            _Error = "V nastavení bankovních účtů je nevyplněná měna nebo účet." : Return False
        End If
        If Not lisP88 Is Nothing Then
            For Each c In lisP88.GroupBy(Function(p) p.j27ID)
                If c.Count > 1 Then
                    
                    _Error = "Pro měnu [" & Me.Factory.ftBL.LoadJ27(c(0).j27ID).j27Code & "] není možné definovat více bankovních účtů." : Return False
                End If
            Next
        End If

        Return _cDL.Save(cRec, lisP88)
    End Function
    Public Function Load(intPID As Integer) As BO.p93InvoiceHeader Implements Ip93InvoiceHeaderBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip93InvoiceHeaderBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p93InvoiceHeader) Implements Ip93InvoiceHeaderBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_p88(intPID As Integer) As IEnumerable(Of BO.p88InvoiceHeader_BankAccount) Implements Ip93InvoiceHeaderBL.GetList_p88
        Return _cDL.GetList_p88(intPID)
    End Function
End Class
