Public Interface Io40SmtpAccountBL
    Inherits IFMother
    Function Save(cRec As BO.o40SmtpAccount) As Boolean
    Function Load(intPID As Integer) As BO.o40SmtpAccount
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o40SmtpAccount)
    Function SetGlobalDefaultSmtpAccount(intO40ID As Integer) As Boolean
End Interface
Class o40SmtpAccountBL
    Inherits BLMother
    Implements Io40SmtpAccountBL
    Private WithEvents _cDL As DL.o40SmtpAccountDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o40SmtpAccountDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o40SmtpAccount) As Boolean Implements Io40SmtpAccountBL.Save
        With cRec
            If Trim(.o40Name) = "" Or Trim(.o40EmailAddress) = "" Or Trim(.o40Server) = "" Then
                _Error = "Název, e-mail adresa a server jsou povinná pole!" : Return False
            End If
            If Not .o40IsVerify Then
                .o40Login = ""
                .o40Password = ""
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o40SmtpAccount Implements Io40SmtpAccountBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io40SmtpAccountBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o40SmtpAccount) Implements Io40SmtpAccountBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function SetGlobalDefaultSmtpAccount(intO40ID As Integer) As Boolean Implements Io40SmtpAccountBL.SetGlobalDefaultSmtpAccount
        'intO40ID=0->globální smtp účet bude podle web.config
        Return _cDL.SetGlobalDefaultSmtpAccount(intO40ID)
    End Function

End Class
