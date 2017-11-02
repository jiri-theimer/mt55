Public Interface Ic26HolidayBL
    Inherits IFMother
    Function Save(cRec As BO.c26Holiday) As Boolean
    Function Load(intPID As Integer) As BO.c26Holiday
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.c26Holiday)

End Interface
Class c26HolidayBL
    Inherits BLMother
    Implements Ic26HolidayBL
    Private WithEvents _cDL As DL.c26HolidayDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.c26HolidayDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.c26Holiday) As Boolean Implements Ic26HolidayBL.Save
        With cRec
            If Trim(.c26Name) = "" Then _Error = "Chybí název svátku." : Return False
            If BO.BAS.IsNullDBDate(.c26Date) Is Nothing Then _Error = "Datum je prázdné." : Return False
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.c26Holiday Implements Ic26HolidayBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ic26HolidayBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.c26Holiday) Implements Ic26HolidayBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
