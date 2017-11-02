Public Interface Ip98Invoice_Round_Setting_TemplateBL
    Inherits IFMother
    Function Save(cRec As BO.p98Invoice_Round_Setting_Template, lisP97 As List(Of BO.p97Invoice_Round_Setting)) As Boolean
    Function Load(intPID As Integer) As BO.p98Invoice_Round_Setting_Template
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p98Invoice_Round_Setting_Template)
    Function GetList_P97(intP98ID As Integer) As IEnumerable(Of BO.p97Invoice_Round_Setting)

End Interface

Class p98Invoice_Round_Setting_TemplateBL
    Inherits BLMother
    Implements Ip98Invoice_Round_Setting_TemplateBL
    Private WithEvents _cDL As DL.p98Invoice_Round_Setting_TemplateDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p98Invoice_Round_Setting_TemplateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p98Invoice_Round_Setting_Template, lisP97 As List(Of BO.p97Invoice_Round_Setting)) As Boolean Implements Ip98Invoice_Round_Setting_TemplateBL.Save
        With cRec
            If Trim(.p98Name) = "" Then _Error = "Chybí název." : Return False
            If Not .p98IsDefault Then
                If GetList().Where(Function(p) p.p98IsDefault = True And p.PID <> cRec.PID).Count = 0 Then
                    _Error = "Jedno zaokrouhlovací pravidlo musí být nastavené jako [Výchozí]." : Return False
                End If
            End If
            If lisP97.Count = 0 Then
                _Error = "V pravidle musí být minimálně jeden řádek (měna)." : Return False
            End If
            If lisP97.GroupBy(Function(p) p.j27ID).Count = 1 And lisP97.Count > 1 Then
                _Error = "Pro jednu měnu může existovat pouze jeden řádek." : Return False
            End If
        End With

        Return _cDL.Save(cRec, lisP97)
    End Function
    Public Function Load(intPID As Integer) As BO.p98Invoice_Round_Setting_Template Implements Ip98Invoice_Round_Setting_TemplateBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip98Invoice_Round_Setting_TemplateBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p98Invoice_Round_Setting_Template) Implements Ip98Invoice_Round_Setting_TemplateBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_P97(intP98ID As Integer) As IEnumerable(Of BO.p97Invoice_Round_Setting) Implements Ip98Invoice_Round_Setting_TemplateBL.GetList_P97
        Return _cDL.GetList_P97(intP98ID)
    End Function
End Class
