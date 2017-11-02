
Public Interface Ix23EntityField_ComboBL
    Inherits IFMother
    Function Save(cRec As BO.x23EntityField_Combo) As Boolean
    Function Load(intPID As Integer) As BO.x23EntityField_Combo
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x23EntityField_Combo)

End Interface
Class x23EntityField_ComboBL
    Inherits BLMother
    Implements Ix23EntityField_ComboBL
    Private WithEvents _cDL As DL.x23EntityField_ComboDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x23EntityField_ComboDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x23EntityField_Combo) As Boolean Implements Ix23EntityField_ComboBL.Save
        With cRec
            If Trim(.x23Name) = "" Then _Error = "Chybí název číselníku." : Return False
            .x23DataSource = Trim(.x23DataSource)
            If .x23DataSource = "" Then
                .x23DataSourceFieldTEXT = "" : .x23DataSourceFieldPID = "" : .x23DataSourceTable = ""
            Else
                If .x23DataSourceFieldPID = "" Then
                    _Error = "Chybí specifikace doplňujících atributů k externímu datovému zdroji." : Return False
                End If
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.x23EntityField_Combo Implements Ix23EntityField_ComboBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ix23EntityField_ComboBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.x23EntityField_Combo) Implements Ix23EntityField_ComboBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
