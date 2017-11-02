
Public Interface Ij61TextTemplateBL
    Inherits IFMother
    Function Save(cRec As BO.j61TextTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
    Function Load(intPID As Integer) As BO.j61TextTemplate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.j61TextTemplate)
    Function InhaleRecordDisposition(cRec As BO.j61TextTemplate) As BO.RecordDisposition
End Interface
Class j61TextTemplateBL
    Inherits BLMother
    Implements Ij61TextTemplateBL
    Private WithEvents _cDL As DL.j61TextTemplateDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j61TextTemplateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j61TextTemplate, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ij61TextTemplateBL.Save
        With cRec
            If .x29ID = BO.x29IdEnum._NotSpecified Then _Error = "Chybí druh entity." : Return False
            If Trim(.j61Name) = "" Then _Error = "Chybí název šablony." : Return False
            If Trim(.j61PlainTextBody) = "" Then _Error = "Chybí text obsahu šablony." : Return False
            If .j02ID_Owner = 0 Then _Error = "Chybí vlastník záznamu." : Return False

        End With

        Return _cDL.Save(cRec, lisX69)
    End Function
    Public Function Load(intPID As Integer) As BO.j61TextTemplate Implements Ij61TextTemplateBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij61TextTemplateBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.j61TextTemplate) Implements Ij61TextTemplateBL.GetList
        Return _cDL.GetList(mq)
    End Function

    Public Function InhaleRecordDisposition(cRec As BO.j61TextTemplate) As BO.RecordDisposition Implements Ij61TextTemplateBL.InhaleRecordDisposition
        Dim c As New BO.RecordDisposition
        If Factory.SysUser.IsAdmin Or cRec.j02ID_Owner = Factory.SysUser.j02ID Then
            c.ReadAccess = True : c.OwnerAccess = True
            Return c
        End If
        If Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.j61TextTemplate, cRec.PID).Count > 0 Then
            c.ReadAccess = True 'ručně lze dát poznámce roli pouze na čtení
        End If

        Return c
    End Function
End Class
