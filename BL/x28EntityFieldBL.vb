Public Interface Ix28EntityFieldBL
    Inherits IFMother
    Function Save(cRec As BO.x28EntityField, lisX26 As List(Of BO.x26EntityField_Binding)) As Boolean
    Function Load(intPID As Integer) As BO.x28EntityField
    Function LoadByField(strField As String) As BO.x28EntityField
    Function LoadByQueryField(strField As String) As BO.x28EntityField
    Function Delete(intPID As Integer) As Boolean
    Function GetList(x29id As BO.x29IdEnum, intEntityType As Integer, bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField)
    Function GetList(x28FieldNames As List(Of String), bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField)
    Function GetList(intX23ID As Integer) As IEnumerable(Of BO.x28EntityField)
    Function GetListWithValues(x29id As BO.x29IdEnum, intRecordPID As Integer, intEntityType As Integer, Optional strTempGUID As String = "") As List(Of BO.FreeField)
    Function GetList_x26(intX28ID As Integer) As IEnumerable(Of BO.x26EntityField_Binding)
End Interface
Class x28EntityFieldBL
    Inherits BLMother
    Implements Ix28EntityFieldBL
    Private WithEvents _cDL As DL.x28EntityFieldDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x28EntityFieldDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.x28EntityField, lisX26 As List(Of BO.x26EntityField_Binding)) As Boolean Implements Ix28EntityFieldBL.Save
        With cRec
            If Trim(.x28Name) = "" Then _Error = "Chybí název/popisek pole." : Return False
            If .x24ID = BO.x24IdENUM.tNone Then _Error = "Chybí typ pole." : Return False
            Select Case .x28Flag
                Case BO.x28FlagENUM.UserField
                    If .x23ID <> 0 And .x24ID <> BO.x24IdENUM.tInteger Then
                        _Error = "Pole s vazbou na combo seznam musí být formátu INTEGER." : Return False
                    End If

                    If .x28Field = "" Then
                        Dim cX24 As BO.x24DataType = Factory.ftBL.GetList_X24().Where(Function(p) p.x24ID = .x24ID)(0)
                        .x28Field = _cDL.FindFirstUsableField(BO.BAS.GetDataPrefix(cRec.x29ID), cX24, .x23ID)
                        If .x28Field = "" Then
                            _Error = "Systém nedokázal najít volné fyzické pole pro toto uživatelské pole." : Return False
                        End If
                    End If
                Case BO.x28FlagENUM.GridField
                    If Trim(.x28Grid_Field) = "" And Trim(.x28Query_Field) = "" Then _Error = "Chybná specifikace pole." : Return False
                    If .x28Grid_Field.IndexOf(".") > 0 Or .x28Grid_Field.IndexOf("[") > 0 Or .x28Grid_Field.IndexOf("[") > 0 Then
                        _Error = "Pole obsahuje zakázané znaky." : Return False
                    End If
                   
            End Select
            
            
            If .x28IsAllEntityTypes Then
                lisX26 = New List(Of BO.x26EntityField_Binding)
            Else
                If Not lisX26 Is Nothing Then
                    If lisX26.Count = 0 Then
                        _Error = "Musíte zaškrtnout minimálně jeden typ entity." : Return False
                    End If
                End If
            End If
        End With


        Return _cDL.Save(cRec, lisX26)
    End Function
    Public Function Load(intPID As Integer) As BO.x28EntityField Implements Ix28EntityFieldBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByField(strField As String) As BO.x28EntityField Implements Ix28EntityFieldBL.LoadByField
        Return _cDL.LoadByField(strField)
    End Function
    Public Function LoadByQueryField(strField As String) As BO.x28EntityField Implements Ix28EntityFieldBL.LoadByQueryField
        Return _cDL.LoadByQueryField(strField)
    End Function

    Public Function Delete(intPID As Integer) As Boolean Implements Ix28EntityFieldBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Overloads Function GetList(x29id As BO.x29IdEnum, intEntityType As Integer, bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField) Implements Ix28EntityFieldBL.GetList
        Return _cDL.GetList(x29id, intEntityType, bolTestUserAccess)
    End Function
    Public Overloads Function GetList(x28FieldNames As List(Of String), bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField) Implements Ix28EntityFieldBL.GetList
        Return _cDL.GetList(x28FieldNames, bolTestUserAccess)
    End Function
    Public Overloads Function GetList(intX23ID As Integer) As IEnumerable(Of BO.x28EntityField) Implements Ix28EntityFieldBL.GetList
        Return _cDL.GetList(intX23ID)
    End Function
    Public Function GetListWithValues(x29id As BO.x29IdEnum, intRecordPID As Integer, intEntityType As Integer, Optional strTempGUID As String = "") As List(Of BO.FreeField) Implements Ix28EntityFieldBL.GetListWithValues
        Return _cDL.GetListWithValues(x29id, intRecordPID, intEntityType, strTempGUID)


    End Function
    Public Function GetList_x26(intX28ID As Integer) As IEnumerable(Of BO.x26EntityField_Binding) Implements Ix28EntityFieldBL.GetList_x26
        Return _cDL.GetList_x26(intX28ID)
    End Function
End Class
