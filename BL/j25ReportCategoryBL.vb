Public Interface Ij25ReportCategoryBL
    Inherits IFMother
    Function Save(cRec As BO.j25ReportCategory) As Boolean
    Function Load(intPID As Integer) As BO.j25ReportCategory
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j25ReportCategory)

End Interface

Class j25ReportCategoryBL
    Inherits BLMother
    Implements Ij25ReportCategoryBL
    Private WithEvents _cDL As DL.j25ReportCategoryDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j25ReportCategoryDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j25ReportCategory) As Boolean Implements Ij25ReportCategoryBL.Save
        With cRec
            If Trim(.j25Name) = "" Then _Error = "Chybí název." : Return False

        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.j25ReportCategory Implements Ij25ReportCategoryBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij25ReportCategoryBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j25ReportCategory) Implements Ij25ReportCategoryBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
