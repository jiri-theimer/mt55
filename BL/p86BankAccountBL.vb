Public Interface Ip86BankAccountBL
    Inherits IFMother
    Function Save(cRec As BO.p86BankAccount) As Boolean
    Function Load(intPID As Integer) As BO.p86BankAccount
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing, Optional intP93ID As Integer = 0) As IEnumerable(Of BO.p86BankAccount)

End Interface

Class p86BankAccountBL
    Inherits BLMother
    Implements Ip86BankAccountBL
    Private WithEvents _cDL As DL.p86BankAccountDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p86BankAccountDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p86BankAccount) As Boolean Implements Ip86BankAccountBL.Save
        With cRec
            If Trim(.p86Name) = "" Then _Error = "Chybí název účtu." : Return False
            If Trim(.p86BankAccount) = "" Then _Error = "Chybí číslo účtu." : Return False
        End With
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Dim lis As IEnumerable(Of BO.p86BankAccount) = GetList(mq).Where(Function(p) p.PID <> cRec.PID)
        If lis.Where(Function(p) LCase(Trim(p.p86BankAccount) & p.p86BankCode) = LCase(Trim(cRec.p86BankAccount & cRec.p86BankCode))).Count > 0 Then
            _Error = "Číslo účtu [" & cRec.p86BankAccount & "/" & cRec.p86BankCode & "] je již nastaveno v jiném účtu." : Return False
        End If
        If cRec.p86IBAN <> "" Then
            If lis.Where(Function(p) LCase(Trim(p.p86IBAN)) = LCase(Trim(cRec.p86IBAN))).Count > 0 Then
                _Error = "IBAN kód [" & cRec.p86IBAN & "] je již nastaven v jiném bankovním účtu." : Return False
            End If
        End If
        ''If cRec.p86SWIFT <> "" Then
        ''    If lis.Where(Function(p) LCase(Trim(p.p86SWIFT)) = LCase(Trim(cRec.p86SWIFT))).Count > 0 Then
        ''        _Error = "SWIFT kód [" & cRec.p86SWIFT & "] je již nastaven v jiném bankovním účtu." : Return False
        ''    End If
        ''End If
        

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.p86BankAccount Implements Ip86BankAccountBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip86BankAccountBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing, Optional intP93ID As Integer = 0) As IEnumerable(Of BO.p86BankAccount) Implements Ip86BankAccountBL.GetList
        Return _cDL.GetList(mq, intP93ID)
    End Function
End Class
