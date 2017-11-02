
Public Interface Io41InboxAccountBL
    Inherits IFMother
    Function Save(cRec As BO.o41InboxAccount) As Boolean
    Function Load(intPID As Integer) As BO.o41InboxAccount
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o41InboxAccount)
End Interface
Class o41InboxAccountBL
    Inherits BLMother
    Implements Io41InboxAccountBL
    Private WithEvents _cDL As DL.o41InboxAccountDL
    

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o41InboxAccountDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o41InboxAccount) As Boolean Implements Io41InboxAccountBL.Save
        With cRec
            If Trim(.o41Name) = "" Or Trim(.o41Login) = "" Or Trim(.o41Server) = "" Then
                _Error = "Název, login a server jsou povinná pole!" : Return False
            End If
            If .o41ForwardFlag_Answer <> BO.o41ForwardENUM.EmailAddress Then
                .o41ForwardEmail_Answer = ""
            End If
            If .o41ForwardFlag_New <> BO.o41ForwardENUM.EmailAddress Then
                .o41ForwardEmail_New = ""
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o41InboxAccount Implements Io41InboxAccountBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io41InboxAccountBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o41InboxAccount) Implements Io41InboxAccountBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
