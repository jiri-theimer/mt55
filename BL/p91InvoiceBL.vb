Public Interface Ip91InvoiceBL
    Inherits IFMother
    Function Create(cCreate As BO.p91Create) As Integer
    Function Load(intPID As Integer) As BO.p91Invoice
    Function LoadByCode(strCode As String) As BO.p91Invoice
    Function LoadCreditNote(intPID As Integer) As BO.p91Invoice
    Function LoadMyLastCreated() As BO.p91Invoice
    Function LoadLastCreatedByClient(intP28ID As Integer) As BO.p91Invoice
    Function Delete(intPID As Integer, strGUID As String) As Boolean
    Function GetList(mq As BO.myQueryP91) As IEnumerable(Of BO.p91Invoice)
    Function GetListAsDR(myQuery As BO.myQueryP91) As SqlClient.SqlDataReader
    Function Update(cRec As BO.p91Invoice, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean
    Function GetGridDataSource(myQuery As BO.myQueryP91) As DataTable
    Function ChangeVat(intP91ID As Integer, x15id As BO.x15IdEnum, dblNewVatRate As Double) As Boolean
    Function ChangeCurrency(intP91ID As Integer, intJ27ID As Integer) As Boolean
    Function ConvertFromDraft(intP91ID As Integer) As Boolean
    Function SaveP94(cRec As BO.p94Invoice_Payment) As Boolean
    Function DeleteP94(intP94ID As Integer, intP91ID As Integer) As Boolean
    Function GetList_p94(intPID As Integer) As IEnumerable(Of BO.p94Invoice_Payment)
    Function LoadP94ByCode(strP94Code As String) As BO.p94Invoice_Payment
    Function GetVirtualCount(myQuery As BO.myQueryP91) As Integer
    Function GetSumRow(myQuery As BO.myQueryP91) As BO.p91InvoiceSum
    Function SaveP99(intP91ID As Integer, intP90ID As Integer, intP82ID As Integer) As Boolean
    Function DeleteP99(intP91ID As Integer, intP90ID As Integer) As Boolean
    Function CreateCreditNote(intP91ID As Integer, intP92ID_CreditNote As Integer) As Integer
    Function RecalcFPR(d1 As Date, d2 As Date, Optional intP51ID As Integer = 0) As Boolean
    Function InhaleRecordDisposition(cRec As BO.p91Invoice) As BO.p91RecordDisposition
    Function GetGridFooterSums(myQuery As BO.myQueryP91, strSumFields As String) As DataTable
    Sub ClearExchangeDateAndRecalc(intP91ID As Integer)
End Interface

Class p91InvoiceBL
    Inherits BLMother
    Implements Ip91InvoiceBL
    Private WithEvents _cDL As DL.p91InvoiceDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p91InvoiceDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Create(cCreate As BO.p91Create) As Integer Implements Ip91InvoiceBL.Create
        With cCreate
            If .p92ID = 0 Then _Error = "Chybí typ faktury."
            If .p28ID = 0 Then _Error = "Chybí příjemce (odběratel) faktury."
            If .TempGUID = "" Then _Error = "GUID is missing."
            If .p28ID <> 0 Then
                If Factory.p28ContactBL.Load(.p28ID).p28IsDraft Then _Error = "Fakturu nelze vystavit pro DRAFT klienta."
            End If
            If Not Factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator) Then
                .IsDraft = True   'násilné rozhodnutí o draftu, protože uživatel nemá oprávnění vystavovat faktury za celou firmu
            End If
            If _Error <> "" Then Return 0
        End With
        Dim lis As IEnumerable(Of BO.p85TempBox) = Me.Factory.p85TempBoxBL.GetList(cCreate.TempGUID, False)
        If lis.Count = 0 Then
            _Error = "Na vstupu do faktury není ani jeden worksheet úkon." : Return 0
        End If
        If Not Me.RaiseAppEvent_TailoringTestBeforeSave(cCreate, Nothing, "p91_beforecreate") Then Return 0

        Dim intP91ID As Integer = _cDL.Create(cCreate)
        If intP91ID <> 0 Then
            Me.RaiseAppEvent_TailoringAfterSave(intP91ID, "p91_aftercreate")

            Me.RaiseAppEvent(BO.x45IDEnum.p91_new, intP91ID)
            Return intP91ID
        Else
            Return 0
        End If
    End Function
    Public Function Update(cRec As BO.p91Invoice, lisX69 As List(Of BO.x69EntityRole_Assign), lisFF As List(Of BO.FreeField)) As Boolean Implements Ip91InvoiceBL.Update
        With cRec
            If .p92ID = 0 Then _Error = "Chybí typ faktury." : Return False
            If Trim(.p91Client) = "" Then
                If .p28ID = 0 Then _Error = "Chybí příjemce (odběratel) faktury." : Return False
                If .o38ID_Primary = 0 Then
                    If Factory.p28ContactBL.GetList_o37(.p28ID).Where(Function(p) p.o36ID = BO.o36IdEnum.InvoiceAddress).Count > 0 Then
                        'klient má v profilu uvedenou fakturační adresu, tak jí budeme vyžadovat
                        _Error = "Chybí fakturační adresa klienta."
                    End If
                End If
            End If
            If _Error <> "" Then Return False
        End With
        If _cDL.Update(cRec, lisX69, lisFF) Then
            Me.RaiseAppEvent_TailoringAfterSave(cRec.PID, "p91_afterupdate")
            Me.RaiseAppEvent(BO.x45IDEnum.p91_update, cRec.PID)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.p91Invoice Implements Ip91InvoiceBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByCode(strCode As String) As BO.p91Invoice Implements Ip91InvoiceBL.LoadByCode
        Return _cDL.LoadByCode(strCode)
    End Function
    Public Function LoadMyLastCreated() As BO.p91Invoice Implements Ip91InvoiceBL.LoadMyLastCreated
        Return _cDL.LoadMyLastCreated()
    End Function
    Public Function LoadLastCreatedByClient(intP28ID As Integer) As BO.p91Invoice Implements Ip91InvoiceBL.LoadLastCreatedByClient
        Return _cDL.LoadLastCreatedByClient(intP28ID)
    End Function
    Public Function LoadCreditNote(intPID As Integer) As BO.p91Invoice Implements Ip91InvoiceBL.LoadCreditNote
        Return _cDL.LoadCreditNote(intPID)
    End Function

    Public Function Delete(intPID As Integer, strGUID As String) As Boolean Implements Ip91InvoiceBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, intPID) 'úschova kvůli logování historie
        If _cDL.Delete(intPID, strGUID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p91_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(mq As BO.myQueryP91) As IEnumerable(Of BO.p91Invoice) Implements Ip91InvoiceBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetListAsDR(myQuery As BO.myQueryP91) As SqlClient.SqlDataReader Implements Ip91InvoiceBL.GetListAsDR
        Return _cDL.GetListAsDR(myQuery)
    End Function
    Public Function ChangeVat(intP91ID As Integer, x15id As BO.x15IdEnum, dblNewVatRate As Double) As Boolean Implements Ip91InvoiceBL.ChangeVat
        If intP91ID = 0 Then _Error = "Na vstupu chybí ID faktury." : Return False
        Return _cDL.ChangeVat(intP91ID, x15id, dblNewVatRate)
    End Function
    Public Function ChangeCurrency(intP91ID As Integer, intJ27ID As Integer) As Boolean Implements Ip91InvoiceBL.ChangeCurrency
        If intP91ID = 0 Or intJ27ID = 0 Then _Error = "Na vstupu chybí ID faktury nebo měna." : Return False
        Dim cRec As BO.p91Invoice = Load(intP91ID)
        If cRec.j27ID = intJ27ID Then _Error = "Cílová měna je shodná s aktuální měnou faktury." : Return False
        Return _cDL.ChangeCurrency(intP91ID, intJ27ID)
    End Function
    Public Function ConvertFromDraft(intP91ID As Integer) As Boolean Implements Ip91InvoiceBL.ConvertFromDraft
        Return _cDL.ConvertFromDraft(intP91ID)
    End Function
    Public Function SaveP94(cRec As BO.p94Invoice_Payment) As Boolean Implements Ip91InvoiceBL.SaveP94
        If cRec.p94Amount = 0 Then
            _Error = "Částka je nula." : Return False
        End If
        Return _cDL.SaveP94(cRec)
    End Function
    Public Function DeleteP94(intP94ID As Integer, intP91ID As Integer) As Boolean Implements Ip91InvoiceBL.DeleteP94
        Return _cDL.DeleteP94(intP94ID, intP91ID)
    End Function
    Public Function GetList_p94(intPID As Integer) As IEnumerable(Of BO.p94Invoice_Payment) Implements Ip91InvoiceBL.GetList_p94
        Return _cDL.GetList_p94(intPID)
    End Function
    Public Function LoadP94ByCode(strP94Code As String) As BO.p94Invoice_Payment Implements Ip91InvoiceBL.LoadP94ByCode
        Return _cDL.LoadP94ByCode(strP94Code)
    End Function
    Public Function GetVirtualCount(myQuery As BO.myQueryP91) As Integer Implements Ip91InvoiceBL.GetVirtualCount
        Return _cDL.GetVirtualCount(myQuery)
    End Function
    Public Function GetSumRow(myQuery As BO.myQueryP91) As BO.p91InvoiceSum Implements Ip91InvoiceBL.GetSumRow
        Return _cDL.GetSumRow(myQuery)
    End Function
    Public Function SaveP99(intP91ID As Integer, intP90ID As Integer, intP82ID As Integer) As Boolean Implements Ip91InvoiceBL.SaveP99
        If intP82ID = 0 Then
            _Error = "Na vstupu chybí ID úhrady (p82ID)." : Return False
        End If
        If Factory.p90ProformaBL.GetList_p99(0, 0, intP82ID).Count > 0 Then
            _Error = "Tato úhrada již byla dříve spárována s daňovou fakturou" : Return False
        End If


        Return _cDL.SaveP99(intP91ID, intP90ID, intP82ID)
    End Function
    Public Function DeleteP99(intP91ID As Integer, intP90ID As Integer) As Boolean Implements Ip91InvoiceBL.DeleteP99
        Return _cDL.DeleteP99(intP91ID, intP90ID)
    End Function
    Public Function CreateCreditNote(intP91ID As Integer, intP92ID_CreditNote As Integer) As Integer Implements Ip91InvoiceBL.CreateCreditNote
        If intP91ID = 0 Then
            _Error = "Na vstupu chybí ID faktury." : Return 0
        End If
        If intP92ID_CreditNote = 0 Then
            _Error = "Na vstupu chybí ID typu opravného dokladu." : Return 0
        End If
        Return _cDL.CreateCreditNote(intP91ID, intP92ID_CreditNote)
    End Function
    Public Function RecalcFPR(d1 As Date, d2 As Date, Optional intP51ID As Integer = 0) As Boolean Implements Ip91InvoiceBL.RecalcFPR
        If d1 > d2 Then
            _Error = "Časové Období pro přepočet není korektní." : Return False
        End If
        Return _cDL.RecalcFPR(d1, d2, intP51ID)
    End Function

    Public Function InhaleRecordDisposition(cRec As BO.p91Invoice) As BO.p91RecordDisposition Implements Ip91InvoiceBL.InhaleRecordDisposition
        Dim c As New BO.p91RecordDisposition

        If cRec.j02ID_Owner = Factory.SysUser.j02ID Or Factory.SysUser.IsAdmin Then
            c.OwnerAccess = True : c.ReadAccess = True  'je vlastník nebo admin
            Return c
        End If
        If Factory.TestPermission(BO.x53PermValEnum.GR_P91_Owner) Then
            c.OwnerAccess = True : c.ReadAccess = True  'má globální roli vlastit všechny faktury
            Return c
        End If

        If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p91Invoice, cRec.PID, BO.x53PermValEnum.IR_P91_Owner, False) Then
            'k faktuře má explicitně přidělené právo vlastníka
            c.OwnerAccess = True : c.ReadAccess = True
            Return c
        End If
        If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p91Invoice, cRec.PID, BO.x53PermValEnum.IR_P91_Reader, True) Then
            'k faktuře má explicitně přidělené právo čtenáře
            c.ReadAccess = True
            Return c
        End If
        If Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.p41Project, cRec.p41ID_First, BO.x53PermValEnum.PR_P91_Reader, False) Then
            'v projektové roli má oprávnění ke čtení faktur v projektu
            c.ReadAccess = True
            Return c
        End If

        Return c
    End Function
    Public Function GetGridDataSource(myQuery As BO.myQueryP91) As DataTable Implements Ip91InvoiceBL.GetGridDataSource
        Return _cDL.GetGridDataSource(myQuery)
    End Function
    Public Function GetGridFooterSums(myQuery As BO.myQueryP91, strSumFields As String) As DataTable Implements Ip91InvoiceBL.GetGridFooterSums
        Return _cDL.GetGridFooterSums(myQuery, strSumFields)
    End Function

    Public Sub ClearExchangeDateAndRecalc(intP91ID As Integer) Implements Ip91InvoiceBL.ClearExchangeDateAndRecalc
        _cDL.ClearExchangeDate(intP91ID)
        _cDL.RecalcAmount(intP91ID)
    End Sub
End Class
