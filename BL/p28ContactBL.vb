Public Interface Ip28ContactBL
    Inherits IFMother
    Function Save(cRec As BO.p28Contact, lisO37 As List(Of BO.o37Contact_Address), lisO32 As List(Of BO.o32Contact_Medium), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
    Function Load(intPID As Integer) As BO.p28Contact
    Function LoadByRegID(strRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
    Function LoadByVatID(strVatID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
    Function LoadByPersonBirthRegID(strBirthRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
    Function LoadBySupplierID(strSupplierID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact
    Function LoadByExternalPID(strExternalPID As String) As BO.p28Contact
    Function LoadByImapRobotAddress(strRobotAddress) As BO.p28Contact
    Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p28Contact
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryP28) As IEnumerable(Of BO.p28Contact)
    Function GetGridDataSource(myQuery As BO.myQueryP28) As DataTable
    Function GetVirtualCount(myQuery As BO.myQueryP28) As Integer
    Function GetList_o37(intPID As Integer) As IEnumerable(Of BO.o37Contact_Address)
    Function GetList_o32(intPID As Integer) As IEnumerable(Of BO.o32Contact_Medium)

    Function InhaleRecordDisposition(cRec As BO.p28Contact) As BO.p28RecordDisposition
    Function InhaleRecordDisposition(intPID As Integer) As BO.p28RecordDisposition
    Sub UpdateSelectedRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP28ID As Integer)
    Sub ClearSelectedRole(intX67ID As Integer, intP28ID As Integer)
    Function ConvertFromDraft(intPID As Integer) As Boolean
    Function LoadMyLastCreated() As BO.p28Contact
    Function HasChildRecords(intPID As Integer) As Boolean
    Function LoadSumRow(intPID As Integer) As BO.p28ContactSum
    Function GetGridFooterSums(myQuery As BO.myQueryP28, strSumFields As String) As DataTable
    Function AppendOrRemove_IsirMoniting(intP28ID As Integer, bolRemove As Boolean) As Boolean
    Function LoadO48Record(intP28ID As Integer) As BO.o48IsirMonitoring
    Function GetRolesInline(intPID As Integer) As String
End Interface
Class p28ContactBL
    Inherits BLMother
    Implements Ip28ContactBL
    Private WithEvents _cDL As DL.p28ContactDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p28ContactDL(ServiceUser)
        _cUser = ServiceUser
    End Sub

    Public Function Load(intPID As Integer) As BO.p28Contact Implements Ip28ContactBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadMyLastCreated() As BO.p28Contact Implements Ip28ContactBL.LoadMyLastCreated
        Return _cDL.LoadMyLastCreated()
    End Function
    Public Function LoadByRegID(strRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact Implements Ip28ContactBL.LoadByRegID
        Return _cDL.LoadByRegID(strRegID, intP28ID_Exclude)
    End Function
    Public Function LoadByVatID(strVatID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact Implements Ip28ContactBL.LoadByVatID
        Return _cDL.LoadByVatID(strVatID, intP28ID_Exclude)
    End Function
    Public Function LoadByPersonBirthRegID(strBirthRegID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact Implements Ip28ContactBL.LoadByPersonBirthRegID
        Return _cDL.LoadByPersonBirthRegID(strBirthRegID, intP28ID_Exclude)
    End Function
    Public Function LoadBySupplierID(strSupplierID As String, Optional intP28ID_Exclude As Integer = 0) As BO.p28Contact Implements Ip28ContactBL.LoadBySupplierID
        Return _cDL.LoadBySupplierID(strSupplierID, intP28ID_Exclude)
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress) As BO.p28Contact Implements Ip28ContactBL.LoadByImapRobotAddress
        Return _cDL.LoadByImapRobotAddress(strRobotAddress)
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.p28Contact Implements Ip28ContactBL.LoadByExternalPID
        Return _cDL.LoadByExternalPID(strExternalPID)
    End Function

    Private Function ValidateBeforeSave(ByRef cRec As BO.p28Contact, lisO37 As List(Of BO.o37Contact_Address), lisO32 As List(Of BO.o32Contact_Medium), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
        If cRec.PID = 0 Then
            With Factory
                If .TestPermission(BO.x53PermValEnum.GR_P28_Creator) Then
                    'může založit oficiální klienty i draft klienty
                Else
                    If .TestPermission(BO.x53PermValEnum.GR_P28_Draft_Creator) Then
                        cRec.p28IsDraft = True  'násilně založit klienta jako draft, protože na oficiální záznam uživatel právo nemá
                    Else
                        _Error = "Vaše oprávnění nedovoluje zakládat klienty (ani v DRAFT režimu)." : Return False
                    End If
                End If
            End With
        End If
        With cRec
            If .p28SupplierFlag = BO.p28SupplierFlagENUM.ClientAndSupplier Or .p28SupplierFlag = BO.p28SupplierFlagENUM.SupplierOnly Then
                If Trim(.p28SupplierID) = "" Then _Error = "Musíte vyplnit kód dodavatele." : Return False
            Else
                .p28SupplierID = ""
            End If
            If .p28SupplierFlag = BO.p28SupplierFlagENUM.NotClientNotSupplier And cRec.PID <> 0 Then
                Dim mqP41 As New BO.myQueryP41
                mqP41.p28ID = cRec.PID
                If Factory.p41ProjectBL.GetVirtualCount(mqP41) > 0 Then
                    _Error = "Klient má vazbu na minimálně jeden projekt." : Return False
                End If
            End If
            If .p28SupplierID <> "" Then
                If Not LoadBySupplierID(.p28SupplierID, .PID) Is Nothing Then
                    _Error = String.Format("Kód dodavatele [{0}] je již vyplněn u jiného subjektu ({1})!", .p28SupplierID, LoadBySupplierID(.p28SupplierID, .PID).p28Name)
                    Return False
                End If
            End If
            If .p28IsCompany Then
                If .p28CompanyName = "" Then _Error = "Chybí název společnosti!" : Return False
            Else
                If .p28LastName = "" Then _Error = "Chybí příjmení!" : Return False
            End If
            If .j02ID_Owner = 0 Then _Error = "Chybí vlastník záznamu." : Return False
            If Trim(.p28RegID) <> "" Then
                If Not LoadByRegID(.p28RegID, .PID) Is Nothing Then
                    _Error = String.Format("IČ [{0}] je již uvedeno u jiného klienta ({1})!", .p28RegID, LoadByRegID(.p28RegID, .PID).p28Name)
                    Return False
                End If
            End If
            If .p28ParentID <> 0 Then
                If .p28ParentID = .PID Then _Error = "Nadřízený záznam se musí lišit od podřízeného." : Return False
            End If
            'If Trim(.p28VatID) <> "" Then
            '    .p28VatID = Trim(.p28VatID)
            '    If Not LoadByVatID(.p28VatID, .PID) Is Nothing Then
            '        _Error = BO.BAS.GLX("DIČ [" & .p28RegID & "] je již uvedeno u jiného kontaktu ([%1%])!", LoadByVatID(.p28VatID, .PID).p28Name)
            '        Return False
            '    End If
            'End If
            If Trim(.p28Person_BirthRegID) <> "" Then
                If Not LoadByPersonBirthRegID(.p28Person_BirthRegID, .PID) Is Nothing Then
                    _Error = String.Format("RČ [{0}] je již uvedeno u jiného klienta ({%1%})!", .p28Person_BirthRegID, LoadByPersonBirthRegID(.p28Person_BirthRegID, .PID).p28Name)
                    Return False
                End If
            End If
            If Len(.p28BillingMemo) > 1000 Then
                _Error = "Obsah fakturační poznámky je příliš dlouhý (nad 1.000 znaků). Pro tak dlouhé fakturační poznámky nebo souborové přílohy klienta využívejte modul DOKUMENTY." : Return False
            End If
        End With
        If Not lisFF Is Nothing Then
            If Not BL.BAS.ValidateFF(lisFF) Then
                _Error = BL.BAS.ErrorMessage : Return False
            End If
        End If
        If Not lisO32 Is Nothing Then
            If lisO32.Where(Function(p) p.IsSetAsDeleted = False And Trim(p.o32Value) = "" And Trim(p.o32Description) = "").Count > 0 Then
                _Error = "Kontaktní média klienta obsahují položku s nevyplněnou adresou (číslem) i poznámkou!" : Return False
            End If
        End If
        Return True
    End Function
    Public Function Save(cRec As BO.p28Contact, lisO37 As List(Of BO.o37Contact_Address), lisO32 As List(Of BO.o32Contact_Medium), lisP30 As List(Of BO.p30Contact_Person), lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean Implements Ip28ContactBL.Save
        With cRec
            If .PID = 0 And .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
            If .p28IsCompany Then .p28CompanyName = Trim(.p28CompanyName) Else .p28LastName = Trim(.p28LastName)
            If Trim(.p28RegID) <> "" Then .p28RegID = Trim(.p28RegID)
            If Trim(.p28Person_BirthRegID) <> "" Then .p28Person_BirthRegID = Trim(.p28Person_BirthRegID)
        End With

        If Not ValidateBeforeSave(cRec, lisO37, lisO32, lisP30, lisX69, lisFF) Then
            Return False
        End If
        If Not Me.RaiseAppEvent_TailoringTestBeforeSave(cRec, lisFF, "p28_beforesave") Then Return False

        If _cDL.Save(cRec, lisO37, lisO32, lisP30, lisX69, lisFF, _LastSavedPID) Then
            Me.RaiseAppEvent_TailoringAfterSave(_LastSavedPID, "p28_aftersave")
            Dim cP29 As BO.p29ContactType = Me.Factory.p29ContactTypeBL.Load(cRec.p29ID)
            Dim intB01ID As Integer = 0
            If Not cP29 Is Nothing Then intB01ID = cP29.b01ID
            If cRec.PID = 0 Then
                If intB01ID > 0 Then InhaleDefaultWorkflowMove(_LastSavedPID, intB01ID) 'je třeba nahodit výchozí workflow stav
                Me.RaiseAppEvent(BO.x45IDEnum.p28_new, _LastSavedPID)
            Else
                If intB01ID > 0 And cRec.b02ID = 0 Then InhaleDefaultWorkflowMove(cRec.PID, intB01ID) 'chybí hodnota workflow stavu
                Me.RaiseAppEvent(BO.x45IDEnum.p28_update, _LastSavedPID)
            End If
            Return True
        Else
            Return False
        End If

    End Function
    Private Sub InhaleDefaultWorkflowMove(intP28ID As Integer, intB01ID As Integer)
        Dim cB06 As BO.b06WorkflowStep = Me.Factory.b06WorkflowStepBL.LoadKickOffStep(intB01ID)
        If cB06 Is Nothing Then Return
        Me.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, intP28ID, BO.x29IdEnum.p28Contact, "", "", False, Nothing)
    End Sub

    Public Function Delete(intPID As Integer) As Boolean Implements Ip28ContactBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, intPID)
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p28_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetList(mq As BO.myQueryP28) As IEnumerable(Of BO.p28Contact) Implements Ip28ContactBL.GetList
        Return _cDL.GetList(mq)
    End Function

    Public Function GetVirtualCount(myQuery As BO.myQueryP28) As Integer Implements Ip28ContactBL.GetVirtualCount
        Return _cDL.GetVirtualCount(myQuery)
    End Function

    Public Function GetList_o37(intPID As Integer) As IEnumerable(Of BO.o37Contact_Address) Implements Ip28ContactBL.GetList_o37
        Return _cDL.GetList_o37(intPID)
    End Function
    Public Function GetList_o32(intPID As Integer) As IEnumerable(Of BO.o32Contact_Medium) Implements Ip28ContactBL.GetList_o32
        Return _cDL.GetList_o32(intPID)
    End Function

   

    Public Overloads Function InhaleRecordDisposition(intPID As Integer) As BO.p28RecordDisposition Implements Ip28ContactBL.InhaleRecordDisposition
        Return InhaleRecordDisposition(Load(intPID))
    End Function
    Public Overloads Function InhaleRecordDisposition(cRec As BO.p28Contact) As BO.p28RecordDisposition Implements Ip28ContactBL.InhaleRecordDisposition
        Dim c As New BO.p28RecordDisposition

        If cRec.j02ID_Owner = Factory.SysUser.j02ID Or Factory.SysUser.IsAdmin Then
            c.OwnerAccess = True : c.ReadAccess = True  'je vlastník nebo admin
            Return c
        End If
        If Factory.TestPermission(BO.x53PermValEnum.GR_P28_Owner) Then
            c.OwnerAccess = True : c.ReadAccess = True  'má globální roli vlastit všechny klienty
            Return c
        End If

        If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p28Contact, cRec.PID, BO.x53PermValEnum.CR_P28_Owner, False) Then
            'ke klientovi má vlastnická oprávnění
            c.OwnerAccess = True : c.ReadAccess = True
            Return c
        End If
        If Factory.TestPermission(BO.x53PermValEnum.GR_P28_Reader) Then
            c.ReadAccess = True  'má globální roli vlastit všechny klienty
            Return c
        End If

        If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p28Contact, cRec.PID, BO.x53PermValEnum.CR_P28_Reader, True) Then
            'ke klientovi má přístup číst
            c.ReadAccess = True
            Return c
        End If

        Return c
    End Function

    Public Sub UpdateSelectedRole(intX67ID As Integer, lisX69 As List(Of BO.x69EntityRole_Assign), intP28ID As Integer) Implements Ip28ContactBL.UpdateSelectedRole
        _cDL.UpdateSelectedRole(intX67ID, lisX69, intP28ID)
    End Sub
    Public Sub ClearSelectedRole(intX67ID As Integer, intP28ID As Integer) Implements Ip28ContactBL.ClearSelectedRole
        _cDL.ClearSelectedRole(intX67ID, intP28ID)
    End Sub
    Public Function ConvertFromDraft(intPID As Integer) As Boolean Implements Ip28ContactBL.ConvertFromDraft
        Return _cDL.ConvertFromDraft(intPID)
    End Function

    Public Function GetGridDataSource(myQuery As BO.myQueryP28) As DataTable Implements Ip28ContactBL.GetGridDataSource
        Return _cDL.GetGridDataSource(myQuery)
    End Function
    Public Function HasChildRecords(intPID As Integer) As Boolean Implements Ip28ContactBL.HasChildRecords
        Return _cDL.HasChildRecords(intPID)
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.p28ContactSum Implements Ip28ContactBL.LoadSumRow
        Return _cDL.LoadSumRow(intPID)
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP28, strSumFields As String) As DataTable Implements Ip28ContactBL.GetGridFooterSums
        Return _cDL.GetGridFooterSums(myQuery, strSumFields)
    End Function
    Public Function AppendOrRemove_IsirMoniting(intP28ID As Integer, bolRemove As Boolean) As Boolean Implements Ip28ContactBL.AppendOrRemove_IsirMoniting
        Return _cDL.AppendOrRemove_IsirMoniting(intP28ID, bolRemove)
    End Function
    Public Function LoadO48Record(intP28ID As Integer) As BO.o48IsirMonitoring Implements Ip28ContactBL.LoadO48Record
        Return _cDL.LoadO48Record(intP28ID)
    End Function
    Public Function LoadTreeTop(intCurTreeIndex As Integer) As BO.p28Contact Implements Ip28ContactBL.LoadTreeTop
        Return _cDL.LoadTreeTop(intCurTreeIndex)
    End Function
    Public Function GetRolesInline(intPID As Integer) As String Implements Ip28ContactBL.GetRolesInline
        Return _cDL.GetRolesInline(intPID)
    End Function
End Class
