Public Class Factory
    Private Property _cUser As BO.j03UserSYS  'přihlášený uživatel, který volá factory
    Private Property _p31 As Ip31WorksheetBL
    Private Property _f01 As If01FolderBL
    Private Property _p36 As Ip36LockPeriodBL
    Private Property _b07 As Ib07CommentBL
    Private Property _j04 As Ij04UserRoleBL
    Private Property _j05 As Ij05MasterSlaveBL
    Private Property _p11 As Ip11AttendanceBL
    Private Property _o51 As Io51TagBL
    Private Property _p40 As Ip40WorkSheet_RecurrenceBL
    Private Property _p41 As Ip41ProjectBL
    Private Property _p45 As Ip45BudgetBL
    Private Property _p47 As Ip47CapacityPlanBL
    Private Property _p48 As Ip48OperativePlanBL
    Private Property _p49 As Ip49FinancialPlanBL
    Private Property _p28 As Ip28ContactBL
    Private Property _p29 As Ip29ContactTypeBL
    Private Property _o21 As Io21MilestoneTypeBL

    Private Property _o22 As Io22MilestoneBL
    Private Property _o23 As Io23DocBL
    Private Property _p51 As Ip51PriceListBL
    Private Property _p50 As Ip50OfficePriceListBL
    Private Property _p56 As Ip56TaskBL
    Private Property _j02 As Ij02PersonBL
    Private Property _j11 As Ij11TeamBL
    Private Property _o27 As Io27AttachmentBL
    Private Property _o38 As Io38AddressBL
    Private Property _p30 As Ip30Contact_PersonBL
    Private Property _j03 As Ij03UserBL
    Private Property _j07 As Ij07PersonPositionBL
    Private Property _p35 As Ip35UnitBL
    Private Property _o13 As Io13AttachmentTypeBL
    Private Property _p98 As Ip98Invoice_Round_Setting_TemplateBL

    Private Property _j18 As Ij18RegionBL
    Private Property _x31 As Ix31ReportBL
    Private Property _x35 As Ix35GlobalParamBL
    Private Property _x40 As Ix40MailQueueBL
    Private Property _p32 As Ip32ActivityBL
    Private Property _p34 As Ip34ActivityGroupBL
    Private Property _p61 As Ip61ActivityClusterBL
    Private Property _p53 As Ip53VatRateBL
    Private Property _p42 As Ip42ProjectTypeBL
    Private Property _p57 As Ip57TaskTypeBL
    Private Property _p59 As Ip59PriorityBL
    Private Property _x67 As Ix67EntityRoleBL
    Private Property _p95 As Ip95InvoiceRowBL
    Private Property _p85 As Ip85TempBoxBL

    Private Property _j77 As Ij77WorksheetStatTemplateBL
    Private Property _j70 As Ij70QueryTemplateBL
    Private Property _j25 As Ij25ReportCategoryBL
    Private Property _j17 As Ij17CountryBL
    Private Property _p92 As Ip92InvoiceTypeBL
    Private Property _p89 As Ip89ProformaTypeBL
    Private Property _p91 As Ip91InvoiceBL
    Private Property _p90 As Ip90ProformaBL
    Private Property _p93 As Ip93InvoiceHeaderBL
    Private Property _p86 As Ip86BankAccountBL
    Private Property _c21 As Ic21FondCalendarBL
    Private Property _c26 As Ic26HolidayBL
    Private Property _x27 As Ix27EntityFieldGroupBL
    Private Property _x28 As Ix28EntityFieldBL
    Private Property _x23 As Ix23EntityField_ComboBL
    Private Property _x18 As Ix18EntityCategoryBL
    Private Property _x38 As Ix38CodeLogicBL
    Private Property _b01 As Ib01WorkflowTemplateBL
    Private Property _b06 As Ib06WorkflowStepBL
    Private Property _b02 As Ib02WorkflowStatusBL
    Private Property _b05 As Ib05Workflow_HistoryBL
    Private Property _b65 As Ib65WorkflowMessageBL
    Private Property _x47 As Ix47EventLogBL
    Private Property _x46 As Ix46EventNotificationBL
    Private Property _x50 As Ix50HelpBL
    Private Property _x55 As Ix55HtmlSnippetBL
    Private Property _x58 As Ix58UserPageBL
    Private Property _m62 As Im62ExchangeRateBL
    Private Property _o41 As Io41InboxAccountBL
    Private Property _o40 As Io40SmtpAccountBL
    Private Property _o42 As Io42ImapRuleBL
    Private Property _j61 As Ij61TextTemplateBL
    Private Property _j62 As Ij62MenuHomeBL
    Private Property _p63 As Ip63OverheadBL
    Private Property _p65 As Ip65RecurrenceBL
    Private Property _p80 As Ip80InvoiceAmountStructureBL
    Private Property _x48 As Ix48SqlTaskBL
    Private Property _ft As IFtBL
    Private Property _plugin As IPluginSupportBL
    Private Property _copymanager As IDataCopyManagerBL

    Public Sub New(Optional sysUser As BO.j03UserSYS = Nothing, Optional strLogin As String = "")
        If sysUser Is Nothing And strLogin <> "" Then
            Dim cBL As BL.Ij03UserBL = New BL.j03UserBL(Nothing)
            sysUser = cBL.LoadSysProfile(strLogin)

        End If
        _cUser = sysUser
    End Sub

    Public ReadOnly Property SysUser As BO.j03UserSYS
        Get
            Return _cUser
        End Get
    End Property
    Public Sub ChangeConnectString(strNewConstring As String)
        Dim objType As Type = Me.GetType()
        For Each pInfo In objType.GetProperties(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.IgnoreCase)
            If Left(pInfo.Name, 1) = "_" And pInfo.Name <> "_cUser" Then
                pInfo.SetValue(Me, Nothing, Nothing)
            End If
        Next
        _cUser.ExplicitConnectString = strNewConstring
        
    End Sub

    Public Overloads Function TestPermission(intNeededPermissionValue As BO.x53PermValEnum) As Boolean
        If _cUser Is Nothing Then Return False
        Return BO.BAS.TestPermission(_cUser, intNeededPermissionValue)
    End Function
    Public Overloads Function TestPermission(intNeededPermissionValue As BO.x53PermValEnum, OrSecondPermission As BO.x53PermValEnum) As Boolean
        If _cUser Is Nothing Then Return False
        Return BO.BAS.TestPermission(_cUser, intNeededPermissionValue, OrSecondPermission)
    End Function

    Public ReadOnly Property o13AttachmentTypeBL As Io13AttachmentTypeBL
        Get
            If _o13 Is Nothing Then _o13 = New o13AttachmentTypeBL(_cUser)
            Return _o13
        End Get
    End Property
    Public ReadOnly Property o27AttachmentBL As Io27AttachmentBL
        Get
            If _o27 Is Nothing Then _o27 = New o27AttachmentBL(_cUser)
            Return _o27
        End Get
    End Property
   
    Public ReadOnly Property p31WorksheetBL As Ip31WorksheetBL
        Get
            If _p31 Is Nothing Then _p31 = New p31WorksheetBL(_cUser)
            Return _p31
        End Get
    End Property
   
    Public ReadOnly Property j02PersonBL As Ij02PersonBL
        Get
            If _j02 Is Nothing Then _j02 = New j02PersonBL(_cUser)
            Return _j02
        End Get
    End Property
    Public ReadOnly Property j04UserRoleBL As Ij04UserRoleBL
        Get
            If _j04 Is Nothing Then _j04 = New j04UserRoleBL(_cUser)
            Return _j04
        End Get
    End Property
    Public ReadOnly Property j05MasterSlaveBL As Ij05MasterSlaveBL
        Get
            If _j05 Is Nothing Then _j05 = New j05MasterSlaveBL(_cUser)
            Return _j05
        End Get
    End Property
    Public ReadOnly Property j11TeamBL As Ij11TeamBL
        Get
            If _j11 Is Nothing Then _j11 = New j11TeamBL(_cUser)
            Return _j11
        End Get
    End Property
    Public ReadOnly Property p11AttendanceBL As Ip11AttendanceBL
        Get
            If _p11 Is Nothing Then _p11 = New p11AttendanceBL(_cUser)
            Return _p11
        End Get
    End Property
    Public ReadOnly Property o51TagBL As Io51TagBL
        Get
            If _o51 Is Nothing Then _o51 = New o51TagBL(_cUser)
            Return _o51
        End Get
    End Property
    Public ReadOnly Property p40WorkSheet_RecurrenceBL As Ip40WorkSheet_RecurrenceBL
        Get
            If _p40 Is Nothing Then _p40 = New p40WorkSheet_RecurrenceBL(_cUser)
            Return _p40
        End Get
    End Property
    Public ReadOnly Property o41InboxAccountBL As Io41InboxAccountBL
        Get
            If _o41 Is Nothing Then _o41 = New o41InboxAccountBL(_cUser)
            Return _o41
        End Get
    End Property
    Public ReadOnly Property o40SmtpAccountBL As Io40SmtpAccountBL
        Get
            If _o40 Is Nothing Then _o40 = New o40SmtpAccountBL(_cUser)
            Return _o40
        End Get
    End Property
    Public ReadOnly Property o42ImapRuleBL As Io42ImapRuleBL
        Get
            If _o42 Is Nothing Then _o42 = New o42ImapRuleBL(_cUser)
            Return _o42
        End Get
    End Property
    Public ReadOnly Property p41ProjectBL As Ip41ProjectBL
        Get
            If _p41 Is Nothing Then _p41 = New p41ProjectBL(_cUser)
            Return _p41
        End Get
    End Property
    Public ReadOnly Property p45BudgetBL As Ip45BudgetBL
        Get
            If _p45 Is Nothing Then _p45 = New p45BudgetBL(_cUser)
            Return _p45
        End Get
    End Property
    Public ReadOnly Property p47CapacityPlanBL As Ip47CapacityPlanBL
        Get
            If _p47 Is Nothing Then _p47 = New p47CapacityPlanBL(_cUser)
            Return _p47
        End Get
    End Property
    Public ReadOnly Property p48OperativePlanBL As Ip48OperativePlanBL
        Get
            If _p48 Is Nothing Then _p48 = New p48OperativePlanBL(_cUser)
            Return _p48
        End Get
    End Property
    Public ReadOnly Property p49FinancialPlanBL As Ip49FinancialPlanBL
        Get
            If _p49 Is Nothing Then _p49 = New p49FinancialPlanBL(_cUser)
            Return _p49
        End Get
    End Property
    Public ReadOnly Property p51PriceListBL As Ip51PriceListBL
        Get
            If _p51 Is Nothing Then _p51 = New p51PriceListBL(_cUser)
            Return _p51
        End Get
    End Property
    Public ReadOnly Property p50OfficePriceListBL As Ip50OfficePriceListBL
        Get
            If _p50 Is Nothing Then _p50 = New p50OfficePriceListBL(_cUser)
            Return _p50
        End Get

    End Property
    Public ReadOnly Property j07PersonPositionBL As Ij07PersonPositionBL
        Get
            If _j07 Is Nothing Then _j07 = New j07PersonPositionBL(_cUser)
            Return _j07
        End Get
    End Property
    Public ReadOnly Property p35UnitBL As Ip35UnitBL
        Get
            If _p35 Is Nothing Then _p35 = New p35UnitBL(_cUser)
            Return _p35
        End Get
    End Property
    
    Public ReadOnly Property j18RegionBL As Ij18RegionBL
        Get
            If _j18 Is Nothing Then _j18 = New j18RegionBL(_cUser)
            Return _j18
        End Get
    End Property
    Public ReadOnly Property j17CountryBL As Ij17CountryBL
        Get
            If _j17 Is Nothing Then _j17 = New j17CountryBL(_cUser)
            Return _j17
        End Get
    End Property
    Public ReadOnly Property p30Contact_PersonBL As Ip30Contact_PersonBL
        Get
            If _p30 Is Nothing Then _p30 = New p30Contact_PersonBL(_cUser)
            Return _p30
        End Get
    End Property
    Public ReadOnly Property c21FondCalendarBL As Ic21FondCalendarBL
        Get
            If _c21 Is Nothing Then _c21 = New c21FondCalendarBL(_cUser)
            Return _c21
        End Get
    End Property
    Public ReadOnly Property c26HolidayBL As Ic26HolidayBL
        Get
            If _c26 Is Nothing Then _c26 = New c26HolidayBL(_cUser)
            Return _c26
        End Get
    End Property
   
    Public ReadOnly Property p98Invoice_Round_Setting_TemplateBL As Ip98Invoice_Round_Setting_TemplateBL
        Get
            If _p98 Is Nothing Then _p98 = New p98Invoice_Round_Setting_TemplateBL(_cUser)
            Return _p98
        End Get
    End Property
    Public ReadOnly Property j25ReportCategoryBL As Ij25ReportCategoryBL
        Get
            If _j25 Is Nothing Then _j25 = New j25ReportCategoryBL(_cUser)
            Return _j25
        End Get
    End Property
    Public ReadOnly Property p28ContactBL As Ip28ContactBL
        Get
            If _p28 Is Nothing Then _p28 = New p28ContactBL(_cUser)
            Return _p28
        End Get
    End Property
    Public ReadOnly Property b07CommentBL As Ib07CommentBL
        Get
            If _b07 Is Nothing Then _b07 = New b07CommentBL(_cUser)
            Return _b07
        End Get
    End Property
    Public ReadOnly Property p56TaskBL As Ip56TaskBL
        Get
            If _p56 Is Nothing Then _p56 = New p56TaskBL(_cUser)
            Return _p56
        End Get
    End Property
    Public ReadOnly Property o38AddressBL As Io38AddressBL
        Get
            If _o38 Is Nothing Then _o38 = New o38AddressBL(_cUser)
            Return _o38
        End Get
    End Property
    Public ReadOnly Property j03UserBL As Ij03UserBL
        Get
            If _j03 Is Nothing Then _j03 = New j03UserBL(_cUser)
            Return _j03
        End Get
    End Property
    Public ReadOnly Property x31ReportBL As Ix31ReportBL
        Get
            If _x31 Is Nothing Then _x31 = New x31ReportBL(_cUser)
            Return _x31
        End Get
    End Property
    Public ReadOnly Property x35GlobalParam As Ix35GlobalParamBL
        Get
            If _x35 Is Nothing Then _x35 = New x35GlobalParamBL(_cUser)
            Return _x35
        End Get
    End Property
    Public ReadOnly Property x40MailQueueBL As Ix40MailQueueBL
        Get
            If _x40 Is Nothing Then _x40 = New x40MailQueueBL(_cUser)
            Return _x40
        End Get
    End Property
    Public ReadOnly Property p32ActivityBL As Ip32ActivityBL
        Get
            If _p32 Is Nothing Then _p32 = New p32ActivityBL(_cUser)
            Return _p32
        End Get
    End Property
    Public ReadOnly Property p36LockPeriodBL As Ip36LockPeriodBL
        Get
            If _p36 Is Nothing Then _p36 = New p36LockPeriodBL(_cUser)
            Return _p36
        End Get
    End Property
    Public ReadOnly Property f01FolderBL As If01FolderBL
        Get
            If _f01 Is Nothing Then _f01 = New f01FolderBL(_cUser)
            Return _f01
        End Get
    End Property
    Public ReadOnly Property p34ActivityGroupBL As Ip34ActivityGroupBL
        Get
            If _p34 Is Nothing Then _p34 = New p34ActivityGroupBL(_cUser)
            Return _p34
        End Get
    End Property
    Public ReadOnly Property p61ActivityClusterBL As Ip61ActivityClusterBL
        Get
            If _p61 Is Nothing Then _p61 = New p61ActivityClusterBL(_cUser)
            Return _p61
        End Get
    End Property
    Public ReadOnly Property p53VatRateBL As Ip53VatRateBL
        Get
            If _p53 Is Nothing Then _p53 = New p53VatRateBL(_cUser)
            Return _p53
        End Get
    End Property
    Public ReadOnly Property p42ProjectTypeBL As Ip42ProjectTypeBL
        Get
            If _p42 Is Nothing Then _p42 = New p42ProjectTypeBL(_cUser)
            Return _p42
        End Get
    End Property
    Public ReadOnly Property p57TaskTypeBL As Ip57TaskTypeBL
        Get
            If _p57 Is Nothing Then _p57 = New p57TaskTypeBL(_cUser)
            Return _p57
        End Get
    End Property

    Public ReadOnly Property p59PriorityBL As Ip59PriorityBL
        Get
            If _p59 Is Nothing Then _p59 = New p59PriorityBL(_cUser)
            Return _p59
        End Get
    End Property
    Public ReadOnly Property p29ContactTypeBL As Ip29ContactTypeBL
        Get
            If _p29 Is Nothing Then _p29 = New p29ContactTypeBL(_cUser)
            Return _p29
        End Get
    End Property
  
   
    Public ReadOnly Property o21MilestoneTypeBL As Io21MilestoneTypeBL
        Get
            If _o21 Is Nothing Then _o21 = New o21MilestoneTypeBL(_cUser)
            Return _o21
        End Get
    End Property
    Public ReadOnly Property o22MilestoneBL As Io22MilestoneBL
        Get
            If _o22 Is Nothing Then _o22 = New o22MilestoneBL(_cUser)
            Return _o22
        End Get
    End Property
    Public ReadOnly Property x67EntityRoleBL As Ix67EntityRoleBL
        Get
            If _x67 Is Nothing Then _x67 = New x67EntityRoleBL(_cUser)
            Return _x67
        End Get
    End Property
    Public ReadOnly Property p95InvoiceRowBL As Ip95InvoiceRowBL
        Get
            If _p95 Is Nothing Then _p95 = New p95InvoiceRowBL(_cUser)
            Return _p95
        End Get
    End Property
    Public ReadOnly Property p92InvoiceTypeBL As Ip92InvoiceTypeBL
        Get
            If _p92 Is Nothing Then _p92 = New p92InvoiceTypeBL(_cUser)
            Return _p92
        End Get
    End Property
    Public ReadOnly Property p89ProformaTypeBL As Ip89ProformaTypeBL
        Get
            If _p89 Is Nothing Then _p89 = New p89ProformaTypeBL(_cUser)
            Return _p89
        End Get
    End Property
    Public ReadOnly Property p91InvoiceBL As Ip91InvoiceBL
        Get
            If _p91 Is Nothing Then _p91 = New p91InvoiceBL(_cUser)
            Return _p91
        End Get
    End Property
    Public ReadOnly Property p90ProformaBL As Ip90ProformaBL
        Get
            If _p90 Is Nothing Then _p90 = New p90ProformaBL(_cUser)
            Return _p90
        End Get
    End Property
    Public ReadOnly Property p93InvoiceHeaderBL As Ip93InvoiceHeaderBL
        Get
            If _p93 Is Nothing Then _p93 = New p93InvoiceHeaderBL(_cUser)
            Return _p93
        End Get
    End Property
    Public ReadOnly Property p86BankAccountBL As Ip86BankAccountBL
        Get
            If _p86 Is Nothing Then _p86 = New p86BankAccountBL(_cUser)
            Return _p86
        End Get
    End Property
    Public ReadOnly Property x27EntityFieldGroupBL As Ix27EntityFieldGroupBL
        Get
            If _x27 Is Nothing Then _x27 = New x27EntityFieldGroupBL(_cUser)
            Return _x27
        End Get
    End Property
    Public ReadOnly Property x28EntityFieldBL As Ix28EntityFieldBL
        Get
            If _x28 Is Nothing Then _x28 = New x28EntityFieldBL(_cUser)
            Return _x28
        End Get
    End Property
    Public ReadOnly Property x23EntityField_ComboBL As Ix23EntityField_ComboBL
        Get
            If _x23 Is Nothing Then _x23 = New x23EntityField_ComboBL(_cUser)
            Return _x23
        End Get
    End Property
    Public ReadOnly Property o23DocBL As Io23DocBL
        Get
            If _o23 Is Nothing Then _o23 = New o23DocBL(_cUser)
            Return _o23
        End Get
    End Property
    Public ReadOnly Property x18EntityCategoryBL As Ix18EntityCategoryBL
        Get
            If _x18 Is Nothing Then _x18 = New x18EntityCategoryBL(_cUser)
            Return _x18
        End Get
    End Property
    Public ReadOnly Property x50HelpBL As Ix50HelpBL
        Get
            If _x50 Is Nothing Then _x50 = New x50HelpBL(_cUser)
            Return _x50
        End Get
    End Property
    Public ReadOnly Property x55HtmlSnippetBL As Ix55HtmlSnippetBL
        Get
            If _x55 Is Nothing Then _x55 = New x55HtmlSnippetBL(_cUser)
            Return _x55
        End Get
    End Property
    Public ReadOnly Property x58UserPage As Ix58UserPageBL
        Get
            If _x58 Is Nothing Then _x58 = New x58UserPageBL(_cUser)
            Return _x58
        End Get
    End Property
    Public ReadOnly Property x47EventLogBL As Ix47EventLogBL
        Get
            If _x47 Is Nothing Then _x47 = New x47EventLogBL(_cUser)
            Return _x47
        End Get
    End Property
    Public ReadOnly Property x46EventNotificationBL As Ix46EventNotificationBL
        Get
            If _x46 Is Nothing Then _x46 = New x46EventNotificationBL(_cUser)
            Return _x46
        End Get
    End Property
    Public ReadOnly Property m62ExchangeRateBL As Im62ExchangeRateBL
        Get
            If _m62 Is Nothing Then _m62 = New m62ExchangeRateBL(_cUser)
            Return _m62
        End Get
    End Property
    Public ReadOnly Property p85TempBoxBL As Ip85TempBoxBL
        Get
            If _p85 Is Nothing Then _p85 = New p85TempBoxBL(_cUser)
            Return _p85
        End Get
    End Property
    Public ReadOnly Property x38CodeLogicBL As Ix38CodeLogicBL
        Get
            If _x38 Is Nothing Then _x38 = New x38CodeLogicBL(_cUser)
            Return _x38
        End Get
    End Property
    Public ReadOnly Property b01WorkflowTemplateBL As Ib01WorkflowTemplateBL
        Get
            If _b01 Is Nothing Then _b01 = New b01WorkflowTemplateBL(_cUser)
            Return _b01
        End Get
    End Property
    Public ReadOnly Property b65WorkflowMessageBL As Ib65WorkflowMessageBL
        Get
            If _b65 Is Nothing Then _b65 = New b65WorkflowMessageBL(_cUser)
            Return _b65
        End Get
    End Property
    Public ReadOnly Property b06WorkflowStepBL As Ib06WorkflowStepBL
        Get
            If _b06 Is Nothing Then _b06 = New b06WorkflowStepBL(_cUser)
            Return _b06
        End Get
    End Property
    Public ReadOnly Property b02WorkflowStatusBL As Ib02WorkflowStatusBL
        Get
            If _b02 Is Nothing Then _b02 = New b02WorkflowStatusBL(_cUser)
            Return _b02
        End Get
    End Property
    Public ReadOnly Property b05Workflow_HistoryBL As Ib05Workflow_HistoryBL
        Get
            If _b05 Is Nothing Then _b05 = New b05Workflow_HistoryBL(_cUser)
            Return _b05
        End Get
    End Property
  
    Public ReadOnly Property j77WorksheetStatTemplateBL As Ij77WorksheetStatTemplateBL
        Get
            If _j77 Is Nothing Then _j77 = New j77WorksheetStatTemplateBL(_cUser)
            Return _j77
        End Get
    End Property
    Public ReadOnly Property j70QueryTemplateBL As Ij70QueryTemplateBL
        Get
            If _j70 Is Nothing Then _j70 = New j70QueryTemplateBL(_cUser)
            Return _j70
        End Get
    End Property
    Public ReadOnly Property j61TextTemplateBL As Ij61TextTemplateBL
        Get
            If _j61 Is Nothing Then _j61 = New j61TextTemplateBL(_cUser)
            Return _j61
        End Get
    End Property
    Public ReadOnly Property p63OverheadBL As Ip63OverheadBL
        Get
            If _p63 Is Nothing Then _p63 = New p63OverheadBL(_cUser)
            Return _p63
        End Get
    End Property
    Public ReadOnly Property p65RecurrenceBL As Ip65RecurrenceBL
        Get
            If _p65 Is Nothing Then _p65 = New p65RecurrenceBL(_cUser)
            Return _p65
        End Get
    End Property
  
    Public ReadOnly Property p80InvoiceAmountStructureBL As Ip80InvoiceAmountStructureBL
        Get
            If _p80 Is Nothing Then _p80 = New p80InvoiceAmountStructureBL(_cUser)
            Return _p80
        End Get
    End Property
    Public ReadOnly Property j62MenuHomeBL As Ij62MenuHomeBL
        Get
            If _j62 Is Nothing Then _j62 = New j62MenuHomeBL(_cUser)
            Return _j62
        End Get
    End Property
    Public ReadOnly Property x48SqlTaskBL As Ix48SqlTaskBL
        Get
            If _x48 Is Nothing Then _x48 = New x48SqlTaskBL(_cUser)
            Return _x48
        End Get
    End Property
    Public ReadOnly Property ftBL As IFtBL
        Get
            If _ft Is Nothing Then _ft = New FtBL(_cUser)
            Return _ft
        End Get
    End Property
    Public ReadOnly Property pluginBL As IPluginSupportBL
        Get
            If _plugin Is Nothing Then _plugin = New PluginSupportBL(_cUser)
            Return _plugin
        End Get
    End Property
    Public ReadOnly Property copyManagerBL As IDataCopyManagerBL
        Get
            If _copymanager Is Nothing Then _copymanager = New DataCopyManagerBL(_cUser)
            Return _copymanager
        End Get
    End Property

    Public Function GetRecordCaption(x29id As BO.x29IdEnum, intRecordPID As Integer, Optional bolExcludeAfterPIPE As Boolean = False) As String
        If Not bolExcludeAfterPIPE Then
            Return x47EventLogBL.GetObjectAlias(x29id, intRecordPID)
        Else
            Dim s As String = x47EventLogBL.GetObjectAlias(x29id, intRecordPID)
            If s.IndexOf("|") > 0 Then s = Left(s, s.IndexOf("|") - 1)
            Return s
        End If

    End Function
    Public Function GetRecordLinkUrl(strPrefix As String, intRecordPID As Integer)
        Return x35GlobalParam.GetValueString("AppHost") & "/dr.aspx?prefix=" & strPrefix & "&pid=" & intRecordPID.ToString
    End Function

    Public Function GetRecordFileName(x29id As BO.x29IdEnum, intRecordPID As Integer, strFileSuffix As String, bolAppendTimestamp As Boolean, intX31ID As Integer) As String
        Dim s As String = ""
        If x29id > BO.x29IdEnum._NotSpecified And intRecordPID <> 0 And x29id <> BO.x29IdEnum.x31Report Then
            s = x47EventLogBL.GetObjectAlias(x29id, intRecordPID)
        End If
        If intX31ID > 0 Then
            Dim cX31 As BO.x31Report = x31ReportBL.Load(intX31ID)
            If Not cX31 Is Nothing Then
                If cX31.x31ExportFileNameMask = "" Then
                    s = cX31.x31Name & "_" & s
                Else
                    s = cX31.x31ExportFileNameMask & "_" & s
                End If
            End If
        End If

        s = BO.BAS.Prepare4FileName(s)

        If s = "" Then s = BO.BAS.GetGUID()
        If Right(s, 1) = "_" Then s = Left(s, Len(s) - 1)
        If bolAppendTimestamp Then s += "_" & Format(Now, "yyyy-mm-dd-HHmm")
        If strFileSuffix = "" Then
            Return s
        Else
            Return s & "." & strFileSuffix
        End If




    End Function


End Class
