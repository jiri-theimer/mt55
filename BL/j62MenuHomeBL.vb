
Public Interface Ij62MenuHomeBL
    Inherits IFMother
    Function Save(cRec As BO.j62MenuHome, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
    Function Load(intPID As Integer) As BO.j62MenuHome
    Function Load_j60(intPID As Integer) As BO.j60MenuTemplate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(intJ60ID As Integer, mq As BO.myQuery) As IEnumerable(Of BO.j62MenuHome)
    Function GetList_J60() As IEnumerable(Of BO.j60MenuTemplate)
    Function Delete_J60(intJ60ID As Integer) As Boolean
    Function Save_j60(cRec As BO.j60MenuTemplate, intJ60ID_Clone_Orig As Integer) As Boolean
End Interface

Class j62MenuHomeBL
    Inherits BLMother
    Implements Ij62MenuHomeBL
    Private WithEvents _cDL As DL.j62MenuHomeDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j62MenuHomeDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j62MenuHome, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ij62MenuHomeBL.Save
        With cRec
            If .x29ID = BO.x29IdEnum._NotSpecified Then _Error = "Chybí Modul." : Return False
            If Trim(.j62Name) = "" Then _Error = "Chybí název položky." : Return False
            
            If Not .j62IsSeparator Then
                If Trim(.j62Url) = "" Then _Error = "Chybí URL odkaz menu položky nebo zaškrtněte, že položka je pouze oddělovačem." : Return False

            End If


        End With

        Return _cDL.Save(cRec, lisX69)
    End Function
    Public Function Load(intPID As Integer) As BO.j62MenuHome Implements Ij62MenuHomeBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij62MenuHomeBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(intJ60ID As Integer, mq As BO.myQuery) As IEnumerable(Of BO.j62MenuHome) Implements Ij62MenuHomeBL.GetList
        Return _cDL.GetList(intJ60ID, mq)
    End Function
    Public Function GetList_J60() As IEnumerable(Of BO.j60MenuTemplate) Implements Ij62MenuHomeBL.GetList_J60
        Return _cDL.GetList_J60()
    End Function
    Public Function Delete_J60(intJ60ID As Integer) As Boolean Implements Ij62MenuHomeBL.Delete_J60
        Return _cDL.Delete_J60(intJ60ID)
    End Function
    Public Function Save_j60(cRec As BO.j60MenuTemplate, intJ60ID_Clone_Orig As Integer) As Boolean Implements Ij62MenuHomeBL.Save_j60
        Return _cDL.Save_j60(cRec, intJ60ID_Clone_Orig)
    End Function
    Public Function Load_j60(intPID As Integer) As BO.j60MenuTemplate Implements Ij62MenuHomeBL.Load_j60
        Return _cDL.Load_j60(intPID)
    End Function
End Class
