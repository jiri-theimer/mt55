' NOTE: You can use the "Rename" command on the context menu to change the class name "mtdefault" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select mtdefault.svc or mtdefault.svc.vb at the Solution Explorer and start debugging.
Imports System.ServiceModel

Class mtService
    Implements ImtService
    
    Private _factory As BL.Factory
    Private _mqDef As BO.myQuery
    Public Sub New()
        _mqDef = New BO.myQuery
        _mqDef.Closed = BO.BooleanQueryMode.NoQuery
    End Sub



    Private Function res(bolSucceess As Boolean, Optional intPID As Integer = 0, Optional strErrorMessage As String = "", Optional strSuccessMessage As String = "") As BO.ServiceResult
        Dim c As New BO.ServiceResult
        c.PID = intPID
        c.ErrorMessage = strErrorMessage
        c.SuccessMessage = strSuccessMessage
        c.IsSuccess = bolSucceess
        Return c
    End Function

    Private Function VerifyUser(strLogin As String, strPassword As String) As BO.j03UserSYS
        Dim b As Boolean = Membership.ValidateUser(strLogin, strPassword)
        If Not b Then
            Throw New FaultException("Heslo nebo uživatelské jméno (login) je chybné.")
            'Return res(False, , "Heslo nebo uživatelské jméno (login) je chybné.")
        End If
        _factory = New BL.Factory(, strLogin)
        If _factory.SysUser Is Nothing Then
            Throw New FaultException("Účet uživatele nebyl nalezen v MARKTIME databázi.")
        End If
        With _factory.SysUser
            If .IsClosed Then
                Throw New FaultException("Uzavřený uživatelský účet pro přihlašování.")
            End If
        End With
        Return _factory.SysUser
    End Function
    Public Function Ping(strLogin As String, strPassword As String) As Boolean Implements ImtService.Ping
        VerifyUser(strLogin, strPassword)
        Return True
    End Function

    Public Function SaveTask(intPID As Integer, strExternalPID As String, fields As Dictionary(Of String, Object), receivers As List(Of BO.x69EntityRole_Assign), uploadedTempFiles As List(Of String), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveTask
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()

        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        Dim cRec As New BO.p56Task

        If intPID <> 0 Then cRec = _factory.p56TaskBL.Load(intPID)
        If cRec Is Nothing Then
            sr.ErrorMessage = "record not found" : Return sr
        End If
        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try

        Next
        If _factory.p56TaskBL.Save(cRec, receivers, Nothing, "") Then
            sr.PID = _factory.p56TaskBL.LastSavedPID
            sr.IsSuccess = True
        Else
            sr.ErrorMessage = _factory.p56TaskBL.ErrorMessage
            sr.IsSuccess = False
        End If

        Return sr
    End Function

    Public Function LoadTaskExtended(intPID As Integer, strLogin As String, strPassword As String) As BO.p56TaskWsExtended Implements ImtService.LoadTaskExtended
        VerifyUser(strLogin, strPassword)
        Dim ret As New BO.p56TaskWsExtended

        Dim c As BO.p56Task = _factory.p56TaskBL.Load(intPID)
        If c Is Nothing Then
            ret.ErrorMessage = "Nelze načíst záznam." : Return ret
        End If

        ret.ConvertFromOrig(c)
        ret.IsSuccess = True
        Return ret

    End Function
    Public Function LoadTask(intPID As Integer, strLogin As String, strPassword As String) As BO.p56Task Implements ImtService.LoadTask
        VerifyUser(strLogin, strPassword)
        Return _factory.p56TaskBL.Load(intPID)

    End Function
    Public Function LoadTaskByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p56Task Implements ImtService.LoadTaskByExternalPID
        VerifyUser(strLogin, strPassword)
        Return _factory.p56TaskBL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function ListTasks4WorksheetEnty(intP41ID As Integer, intJ02ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.p56Task) Implements ImtService.ListTasks4WorksheetEnty
        VerifyUser(strLogin, strPassword)
        Dim mq As New BO.myQueryP56
        mq.p41ID = intP41ID
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForWorksheetEntry
        mq.j02ID_ExplicitQueryFor = intJ02ID

        Return _factory.p56TaskBL.GetList(mq)
    End Function
    Public Function ListProjects(intP28ID As Integer, bolWorksheetEnty As Boolean, strLogin As String, strPassword As String) As IEnumerable(Of BO.p41Project) Implements ImtService.ListProjects
        VerifyUser(strLogin, strPassword)

        Dim mq As New BO.myQueryP41
        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.p28ID = intP28ID
        If bolWorksheetEnty Then
            mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        Else
            mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
        End If
        Return _factory.p41ProjectBL.GetList(mq)
    End Function
    Public Function ListClients(strLogin As String, strPassword As String) As IEnumerable(Of BO.p28Contact) Implements ImtService.ListClients
        VerifyUser(strLogin, strPassword)
        Dim mq As New BO.myQueryP28

        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead

        Return _factory.p28ContactBL.GetList(mq)

    End Function
    
    

    Public Function ListTaskTypes(strLogin As String, strPassword As String) As IEnumerable(Of BO.p57TaskType) Implements ImtService.ListTaskTypes
       VerifyUser(strLogin, strPassword)
        Return _factory.p57TaskTypeBL.GetList(_mqDef)
    End Function
    Public Function ListPriorities(strLogin As String, strPassword As String) As IEnumerable(Of BO.p59Priority) Implements ImtService.ListPriorities
        VerifyUser(strLogin, strPassword)

        Return _factory.p59PriorityBL.GetList(_mqDef)
    End Function
    Public Function ListSheets(strLogin As String, strPassword As String) As IEnumerable(Of BO.p34ActivityGroup) Implements ImtService.ListSheets
        VerifyUser(strLogin, strPassword)

        Return _factory.p34ActivityGroupBL.GetList(_mqDef)
    End Function
    Public Function ListSheets4Project(intP41ID As Integer, intJ02ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.p34ActivityGroup) Implements ImtService.ListSheets4Project
        VerifyUser(strLogin, strPassword)
        Dim cP41 As BO.p41Project = _factory.p41ProjectBL.Load(intP41ID)

        Return _factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(intP41ID, cP41.p42ID, cP41.j18ID, intJ02ID)
    End Function
    Public Function ListActivities(strLogin As String, strPassword As String) As IEnumerable(Of BO.p32Activity) Implements ImtService.ListActivities
        VerifyUser(strLogin, strPassword)

        Dim mq As New BO.myQueryP32
        Return _factory.p32ActivityBL.GetList(mq)
    End Function

    Public Function ListPersons(strLogin As String, strPassword As String) As IEnumerable(Of BO.j02Person) Implements ImtService.ListPersons
        VerifyUser(strLogin, strPassword)

        Dim mq As New BO.myQueryJ02
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Return _factory.j02PersonBL.GetList(mq)
    End Function
    Public Function ListContactPersons(strLogin As String, strPassword As String, intP28ID As Integer, intP41ID As Integer) As IEnumerable(Of BO.j02Person) Implements ImtService.ListContactPersons
        VerifyUser(strLogin, strPassword)
        Dim mq As New BO.myQueryJ02
        mq.p28ID = intP28ID
        mq.p41ID = intP41ID
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
        Return _factory.j02PersonBL.GetList(mq)
    End Function
    Public Function LoadPerson(intPID As Integer, strLogin As String, strPassword As String) As BO.j02Person Implements ImtService.LoadPerson
        VerifyUser(strLogin, strPassword)
        If intPID = 0 Then
            Dim cJ03 As BO.j03User = _factory.j03UserBL.LoadByLogin(strLogin)
            intPID = cJ03.j02ID
        End If
        Return _factory.j02PersonBL.Load(intPID)
    End Function
    Public Function LoadPersonByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.j02Person Implements ImtService.LoadPersonByExternalPID
        VerifyUser(strLogin, strPassword)
        Return _factory.j02PersonBL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function ListPersonTeams(strLogin As String, strPassword As String) As IEnumerable(Of BO.j11Team) Implements ImtService.ListPersonTeams
        VerifyUser(strLogin, strPassword)
        Return _factory.j11TeamBL.GetList(_mqDef)
    End Function
    Public Function ListRoles(strLogin As String, strPassword As String) As IEnumerable(Of BO.x67EntityRole) Implements ImtService.ListRoles
        VerifyUser(strLogin, strPassword)
        Return _factory.x67EntityRoleBL.GetList(_mqDef)
    End Function
    Public Function LoadProject(intPID As Integer, strLogin As String, strPassword As String) As BO.p41Project Implements ImtService.LoadProject
        VerifyUser(strLogin, strPassword)
        Return _factory.p41ProjectBL.Load(intPID)
    End Function
    Public Function LoadProjectByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p41Project Implements ImtService.LoadProjectByExternalPID
        VerifyUser(strLogin, strPassword)
        Return _factory.p41ProjectBL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function LoadClient(intPID As Integer, strLogin As String, strPassword As String) As BO.p28Contact Implements ImtService.LoadClient
        VerifyUser(strLogin, strPassword)
        Return _factory.p28ContactBL.Load(intPID)
    End Function
    Public Function LoadClientByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p28Contact Implements ImtService.LoadClientByExternalPID
        VerifyUser(strLogin, strPassword)
        Return _factory.p28ContactBL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function SaveWorksheet(intPID As Integer, fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveWorksheet
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        Dim cRec As New BO.p31WorksheetEntryInput
        cRec.SetPID(intPID)

        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try
        Next
        With cRec
            If .p34ID = 0 And .p32ID <> 0 Then  'dohledat sešit, pokud na vstupu chybí
                Dim cP32 As BO.p32Activity = _factory.p32ActivityBL.Load(.p32ID)
                .p34ID = cP32.p34ID
            End If

            
            If .p31HoursEntryflag = BO.p31HoursEntryFlagENUM.NeniCas Then
                'ověřit, zda se jedná o hodiny
                If .p34ID <> 0 Then
                    Dim cP34 As BO.p34ActivityGroup = _factory.p34ActivityGroupBL.Load(.p34ID)
                    If cP34.p33ID = BO.p33IdENUM.Cas Then .p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Hodiny
                End If
                If .TimeFrom <> "" Or .TimeUntil <> "" Then .p31HoursEntryflag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo
            End If
            If .p31HoursEntryflag <> BO.p31HoursEntryFlagENUM.NeniCas Then
                If Not .ValidateEntryTime(5, False) Then
                    sr.ErrorMessage = .ErrorMessage : Return sr
                End If
            End If
        End With
        
        If _factory.p31WorksheetBL.SaveOrigRecord(cRec, Nothing) Then
            sr.PID = _factory.p31WorksheetBL.LastSavedPID
            sr.IsSuccess = True
        Else
            sr.ErrorMessage = _factory.p31WorksheetBL.ErrorMessage
            sr.IsSuccess = False
        End If

        Return sr
    End Function

    Public Function SaveProject(intPID As Integer, fields As Dictionary(Of String, Object), receivers As List(Of BO.x69EntityRole_Assign), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveProject
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        Dim cRec As New BO.p41Project

        If intPID <> 0 Then cRec = _factory.p41ProjectBL.Load(intPID)
        If cRec Is Nothing Then
            sr.ErrorMessage = "record not found" : Return sr
        End If
        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try
        Next
        If _factory.p41ProjectBL.Save(cRec, Nothing, Nothing, receivers, Nothing) Then
            sr.PID = _factory.p41ProjectBL.LastSavedPID
            sr.IsSuccess = True
        Else
            sr.ErrorMessage = _factory.p41ProjectBL.ErrorMessage
            sr.IsSuccess = False
        End If

        Return sr
    End Function
    Public Function SaveClient(intPID As Integer, fields As Dictionary(Of String, Object), addresses As List(Of BO.o37Contact_Address), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveClient
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        Dim cRec As New BO.p28Contact

        If intPID <> 0 Then cRec = _factory.p28ContactBL.Load(intPID)
        If cRec Is Nothing Then
            sr.ErrorMessage = "record not found" : Return sr
        End If
        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try
        Next
        Try
            If _factory.p28ContactBL.Save(cRec, addresses, Nothing, Nothing, Nothing, Nothing) Then
                sr.PID = _factory.p28ContactBL.LastSavedPID
                sr.IsSuccess = True
            Else
                sr.ErrorMessage = _factory.p28ContactBL.ErrorMessage
                sr.IsSuccess = False
            End If
        Catch ex As Exception
            Throw New FaultException(ex.Message)

        End Try


        Return sr
    End Function
    Public Function ListWorkflowStatuses(intB01ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.b02WorkflowStatus) Implements ImtService.ListWorkflowStatuses
        VerifyUser(strLogin, strPassword)
        Return _factory.b02WorkflowStatusBL.GetList(intB01ID)
    End Function
    Public Function ListWorkflowSteps(intB01ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.b06WorkflowStep) Implements ImtService.ListWorkflowSteps
        VerifyUser(strLogin, strPassword)
        Return _factory.b06WorkflowStepBL.GetList(intB01ID)
    End Function
    Public Function ListPossibleWorkflowSteps(intRecordPID As Integer, strRecordPrefix As String, intJ02ID As Integer, strLogin As String, strPassword As String) As List(Of BO.WorkflowStepPossible4User) Implements ImtService.ListPossibleWorkflowSteps
        VerifyUser(strLogin, strPassword)
        Return _factory.b06WorkflowStepBL.GetPossibleWorkflowSteps4Person(strRecordPrefix, intRecordPID, intJ02ID)
    End Function

    Public Function SavePerson(intPID As Integer, fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SavePerson
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        Dim cRec As New BO.j02Person

        If intPID <> 0 Then cRec = _factory.j02PersonBL.Load(intPID)
        If cRec Is Nothing Then
            sr.ErrorMessage = "record not found" : Return sr
        End If
        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try
        Next
        Try
            If _factory.j02PersonBL.Save(cRec, Nothing) Then
                sr.PID = _factory.j02PersonBL.LastSavedPID
                sr.IsSuccess = True
            Else
                sr.ErrorMessage = _factory.j02PersonBL.ErrorMessage
                sr.IsSuccess = False
            End If
        Catch ex As Exception
            Throw New FaultException(ex.Message)

        End Try
        Return sr
    End Function

    Public Function SaveContactPerson(intJ02ID As Integer, intP28ID As Integer, strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveContactPerson
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If intJ02ID = 0 Or intP28ID = 0 Then
            sr.ErrorMessage = "intJ02ID or intP28ID is nothing" : Return sr
        End If
        Dim lis As IEnumerable(Of BO.p30Contact_Person) = _factory.p30Contact_PersonBL.GetList(intP28ID, 0, 0)
        Dim cRec As New BO.p30Contact_Person

        If lis.Where(Function(p) p.j02ID = intJ02ID).Count > 0 Then
            cRec = lis.Where(Function(p) p.j02ID = intJ02ID).First
        Else
            cRec.p28ID = intP28ID
            cRec.j02ID = intJ02ID
        End If
        Try
            If _factory.p30Contact_PersonBL.Save(cRec) Then
                sr.PID = _factory.p30Contact_PersonBL.LastSavedPID
                sr.IsSuccess = True
            Else
                sr.ErrorMessage = _factory.p30Contact_PersonBL.ErrorMessage
                sr.IsSuccess = False
            End If
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try
        Return sr
    End Function
    Public Function DeleteContactPerson(intJ02ID As Integer, intP28ID As Integer, strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.DeleteContactPerson
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        If intJ02ID = 0 Or intP28ID = 0 Then
            sr.ErrorMessage = "intJ02ID or intP28ID is nothing" : Return sr
        End If
        Dim lis As IEnumerable(Of BO.p30Contact_Person) = _factory.p30Contact_PersonBL.GetList(intP28ID, 0, intJ02ID)
        If lis.Count = 0 Then
            sr.ErrorMessage = "Vazba této osoby na klienta neexistuje." : Return sr
        End If
        Try
            If _factory.p30Contact_PersonBL.Delete(lis(0).PID) Then
                sr.IsSuccess = True
            Else
                sr.ErrorMessage = _factory.p30Contact_PersonBL.ErrorMessage
                sr.IsSuccess = False
            End If
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try
        Return sr
    End Function

    Public Function UploadBinaryToTempFile(chunkBytes As Byte(), intPartZeroIndex As Integer, intTotalSize As Integer, strArchiveFileName As String, strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.UploadBinaryToTempFile
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult()
        Dim cF As New BO.clsFile

        Dim strPath As String = _factory.x35GlobalParam.TempFolder & "\" & strArchiveFileName
        If intPartZeroIndex = 0 Then
            If cF.FileExist(strPath) Then
                cF.DeleteFile(strPath)
            End If
        End If

        cF.AppendAllBytesToFile(strPath, chunkBytes)
        sr.IsSuccess = True
        Return sr

        
    End Function
    Public Function ListDocTypes(strLogin As String, strPassword As String) As IEnumerable(Of BO.x18EntityCategory) Implements ImtService.ListDocTypes
        VerifyUser(strLogin, strPassword)

        Return _factory.x18EntityCategoryBL.GetList(_mqDef)
    End Function

    Public Function SaveExternalObject2Temp(strPrefix As String, strTempGUID As String, strExternalPID As String, uploadedTempFiles As List(Of String), fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveExternalObject2Temp
        VerifyUser(strLogin, strPassword)
        Dim sr As New BO.ServiceResult(), bolOK As Boolean

        If Not uploadedTempFiles Is Nothing Then
            Dim cF As New BO.clsFile    'temp nahrané soubory
            For Each strFileName As String In uploadedTempFiles
                Dim c As New BO.p85TempBox
                c.p85GUID = strTempGUID
                c.p85Prefix = "o27"
                c.p85OtherKey1 = 1  'o13id=1
                c.p85FreeText01 = strFileName
                c.p85FreeText02 = strFileName
                Dim a() As String = Split(strFileName, "_xxx_")
                If UBound(a) > 0 Then
                    c.p85FreeText01 = a(1)  'uživatelský název souboru
                End If
                c.p85FreeNumber01 = cF.GetFileSize(_factory.x35GlobalParam.TempFolder & "\" & strFileName)
                c.p85FreeText03 = cF.GetContentType(_factory.x35GlobalParam.TempFolder & "\" & strFileName)
                If Right(LCase(strFileName), 3) = "msg" Then
                    c.p85FreeText04 = "MS-Outlook"
                    c.p85FreeText01 = "MS-OUTLOOK-MESSAGE.msg"
                End If

                bolOK = _factory.p85TempBoxBL.Save(c)
            Next
        End If
        If Not fields Is Nothing Then
            For Each fld In fields
                Dim c As New BO.p85TempBox
                c.p85GUID = strTempGUID
                c.p85FreeText01 = strExternalPID
                c.p85FreeText02 = fld.Key
                c.p85Prefix = strPrefix
                c.p85FreeText03 = LCase(fld.Value.GetType().Name)
                If Not fld.Value Is Nothing Then
                    If Not fld.Value Is System.DBNull.Value Then
                        Select Case LCase(fld.Value.GetType().Name)
                            Case "date", "datetime"
                                c.p85FreeDate01 = fld.Value
                            Case "int32"
                                c.p85OtherKey1 = fld.Value
                            Case "double"
                                c.p85FreeFloat01 = fld.Value
                            Case "boolean"
                                c.p85FreeBoolean01 = fld.Value
                            Case Else
                                c.p85Message = fld.Value

                        End Select
                    End If
                End If
                bolOK = _factory.p85TempBoxBL.Save(c)
            Next

        End If

        sr.IsSuccess = bolOK

        Return sr

    End Function

    Public Function SaveDocument(intPID As Integer, strExternalPID As String, uploadedTempFiles As List(Of String), fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult Implements ImtService.SaveDocument
        VerifyUser(strLogin, strPassword)

        Dim sr As New BO.ServiceResult()
        If fields Is Nothing Then
            sr.ErrorMessage = "fields is nothing" : Return sr
        End If
        If intPID = 0 And Not _factory.o23DocBL.LoadByExternalPID(strExternalPID) Is Nothing Then
            sr.ErrorMessage = "V MARKTIME již existuje dokument k této Outlook položce! Upravovat nebo odstranit tento dokument lze pouze v MARKTIME rozhraní." : Return sr
        End If
        Dim strGUID As String = ""
        If Not uploadedTempFiles Is Nothing Then
            Dim cF As New BO.clsFile
            'temp nahrané soubory
            For Each strFileName As String In uploadedTempFiles
                If strGUID = "" Then strGUID = BO.BAS.GetGUID()
                Dim c As New BO.p85TempBox
                c.p85GUID = strGUID
                c.p85Prefix = "o27"
                c.p85OtherKey1 = 1  'o13id=1
                c.p85FreeText02 = strFileName
                c.p85FreeNumber01 = cF.GetFileSize(_factory.x35GlobalParam.TempFolder & "\" & strFileName)
                c.p85FreeText03 = cF.GetContentType(_factory.x35GlobalParam.TempFolder & "\" & strFileName)
                If Right(LCase(strFileName), 3) = "msg" Then
                    c.p85FreeText01 = "MS-Outlook"
                End If

                _factory.p85TempBoxBL.Save(c)
            Next
        End If

        Dim cRec As New BO.o23Doc
        If intPID = 0 Then
            cRec.o23GUID = strGUID
            cRec.o23ExternalPID = strExternalPID
        Else
            cRec = _factory.o23DocBL.Load(intPID)
        End If
        If cRec Is Nothing Then
            sr.ErrorMessage = "record not found" : Return sr
        End If
        For Each c In fields
            Try
                BO.BAS.SetPropertyValue(cRec, c.Key, c.Value)
            Catch ex As Exception
                sr.ErrorMessage = "Property [" & c.Key & "], error: " & ex.Message : Return sr
            End Try
        Next

        Try
            If _factory.o23DocBL.Save(cRec, Nothing, Nothing, Nothing, Nothing, strGUID) Then
                sr.PID = _factory.o23DocBL.LastSavedPID
                sr.IsSuccess = True
            Else
                sr.ErrorMessage = _factory.o23DocBL.ErrorMessage
                sr.IsSuccess = False
            End If
        Catch ex As Exception
            Throw New FaultException(ex.Message)

        End Try
        Return sr
    End Function

    Public Function ListComboSource(strDataPrefix As String, strFlag As String, bolFirstEmptyRow As Boolean, intParentPID As Integer, strLogin As String, strPassword As String) As List(Of BO.ComboSource) Implements ImtService.ListComboSource
        Dim s As String = "", ret As New List(Of BO.ComboSource)
        If strDataPrefix = "" Then Return ret
        VerifyUser(strLogin, strPassword)

        Select Case strDataPrefix
            Case "p28"
                Dim mq As New BO.myQueryP28
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                mq.SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead
               
                Dim lis As IEnumerable(Of BO.p28Contact) = _factory.p28ContactBL.GetList(mq)
                For Each c In lis
                    Dim cc As New BO.ComboSource
                    cc.pid = c.PID : cc.ItemText = c.p28Name : cc.ItemFlag = ""
                    ret.Add(cc)
                Next
                Select Case strFlag
                    Case "1", "2"    'klienti pro zápis worksheet                        
                        Dim mqP41 As New BO.myQueryP41
                        mqP41.Closed = BO.BooleanQueryMode.FalseQuery
                        If strFlag = "1" Then
                            mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry  'zapisování úkonů
                        End If
                        If strFlag = "2" Then
                            mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead  'projekt klienta
                        End If
                        Dim p28ids As List(Of Integer) = _factory.p41ProjectBL.GetList(mqP41).Select(Function(p) p.p28ID_Client).Distinct.ToList
                        For Each intP28ID As Integer In p28ids
                            If Not ret.Find(Function(p) p.pid = intP28ID) Is Nothing Then
                                ret.Find(Function(p) p.pid = intP28ID).ItemFlag = "1"
                            End If
                        Next
                        ret.RemoveAll(Function(p) p.ItemFlag = "")
                End Select
            Case "p41"
                Dim mq As New BO.myQueryP41, intMask As Integer = _factory.SysUser.j03ProjectMaskIndex
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
                If intParentPID > 0 Then mq.p28ID = intParentPID
                Select Case strFlag
                    Case "1"    'projekty pro zápis worksheet, intParentPID=p28ID
                        mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
                    Case "2"    'Projekty, kde je minimálně jeden otevřený úkol
                        mq.QuickQuery = BO.myQueryP41_QuickQuery.WithOpenTasks
                    Case Else    'všechny otevřené projekty
                End Select
                Dim lis As IEnumerable(Of BO.p41Project) = _factory.p41ProjectBL.GetList(mq)
                If intParentPID = -1 Then lis = lis.Where(Function(p) p.p28ID_Client = 0) 'projekty bez klienta
                For Each c In lis
                    Dim cc As New BO.ComboSource
                    cc.pid = c.PID : cc.ItemFlag = ""
                    If intParentPID = 0 Then
                        cc.ItemText = c.ProjectWithMask(intMask)
                    Else
                        cc.ItemText = c.PrefferedName & " [" & c.p41Code & "]"
                    End If
                    ret.Add(cc)
                Next
            Case "p91"
                Dim mq As New BO.myQueryP91
                mq.SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                If intParentPID > 0 Then mq.p28ID = intParentPID
                Dim lis As IEnumerable(Of BO.p91Invoice) = _factory.p91InvoiceBL.GetList(mq)
                For Each c In lis
                    Dim cc As New BO.ComboSource
                    cc.pid = c.PID
                    If intParentPID = 0 Then
                        cc.ItemText = c.p91Code & " - " & c.p91Client & " (" & BO.BAS.FN(c.p91Amount_TotalDue) & " " & c.j27Code
                    Else
                        cc.ItemText = c.p91Code & " - " & BO.BAS.FD(c.p91DateSupply) & " (" & BO.BAS.FN(c.p91Amount_TotalDue) & " " & c.j27Code
                    End If

                    ret.Add(cc)
                Next
            Case "j02"
                Dim mq As New BO.myQueryJ02
                mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                Dim lis As IEnumerable(Of BO.j02Person) = _factory.j02PersonBL.GetList(mq)
                For Each c In lis
                    Dim cc As New BO.ComboSource
                    cc.pid = c.PID
                    cc.ItemText = c.FullNameDesc
                    ret.Add(cc)
                Next
            Case "p56"
                Dim mq As New BO.myQueryP56
                mq.p41ID = intParentPID
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
                Dim lis As IEnumerable(Of BO.p56Task) = _factory.p56TaskBL.GetList(mq, True)
                For Each c In lis
                    Dim cc As New BO.ComboSource
                    cc.pid = c.PID
                    cc.ItemText = c.p57Name & ": " & c.p56Name & " ..." & c.ReceiversInLine

                    ret.Add(cc)
                Next
            Case Else

        End Select
        If bolFirstEmptyRow Then
            Dim cc As New BO.ComboSource
            cc.pid = 0
            ret.Insert(0, cc)
        End If
        Return ret

    End Function

    Public Function LoadMsOfficeBinding(strEntryID As String, strLogin As String, strPassword As String) As BO.MsOfficeBinding Implements ImtService.LoadMsOfficeBinding
        If strEntryID = "" Then Return Nothing
        VerifyUser(strLogin, strPassword)
        'Return _factory.o23DocBL.LoadMsOfficeBinding(strEntryID)
        Return Nothing
    End Function
End Class
