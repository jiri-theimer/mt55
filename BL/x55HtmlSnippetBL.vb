Public Interface Ix55HtmlSnippetBL
    Inherits IFMother
    Function Save(cRec As BO.x55HtmlSnippet, lisX56 As List(Of BO.x56SnippetProperty)) As Boolean
    Function Load(intPID As Integer) As BO.x55HtmlSnippet
    Function LoadByCode(strCode As String) As BO.x55HtmlSnippet
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x55HtmlSnippet)
    Function GetList_Properties(intPID As Integer) As IEnumerable(Of BO.x56SnippetProperty)

End Interface
Class x55HtmlSnippetBL
    Inherits BLMother
    Implements Ix55HtmlSnippetBL
    Private WithEvents _cDL As DL.x55HtmlSnippetDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x55HtmlSnippetDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x55HtmlSnippet, lisX56 As List(Of BO.x56SnippetProperty)) As Boolean Implements Ix55HtmlSnippetBL.Save
        With cRec
            If Trim(.x55Name) = "" Then _Error = "Chybí název." : Return False
        End With

        Return _cDL.Save(cRec, lisX56)
    End Function
    Public Function Load(intPID As Integer) As BO.x55HtmlSnippet Implements Ix55HtmlSnippetBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByCode(strCode As String) As BO.x55HtmlSnippet Implements Ix55HtmlSnippetBL.LoadByCode
        Return _cDL.LoadByCode(strCode)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix55HtmlSnippetBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x55HtmlSnippet) Implements Ix55HtmlSnippetBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_Properties(intPID As Integer) As IEnumerable(Of BO.x56SnippetProperty) Implements Ix55HtmlSnippetBL.GetList_Properties
        Return _cDL.GetList_Properties(intPID)
    End Function
End Class
