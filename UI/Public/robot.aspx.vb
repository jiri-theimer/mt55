Public Class robot
    Inherits System.Web.UI.Page
    Private Property _Factory As BL.Factory
    Private Property _BatchGuid As String = ""
    Private Property _curNow As Date = Now
    Private _lisP53 As IEnumerable(Of BO.p53VatRate) = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _BatchGuid = Format(Now, "dd.MM.yyyy HH:mm:ss")

        If Not Page.IsPostBack Then
            If Request.Item("blank") = "1" Then panModal.Visible = True
            log4net.LogManager.GetLogger("robotlog").Info("Start")
            _Factory = New BL.Factory(, BO.ASS.GetConfigVal("robot_account", "admin"))
            If _Factory.SysUser Is Nothing Then
                log4net.LogManager.GetLogger("robotlog").Info("Service user is not inhaled!")
                Response.Write("Service user not exists!")
                Return
            End If

            _curNow = Now
            Dim bolNowExplicit As Boolean = False
            If Request.Item("now") <> "" Then
                bolNowExplicit = True
                _curNow = BO.BAS.ConvertString2Date(Request.Item("now"))
            End If

            


            Handle_MailQueque()

            Handle_ImapRobot()

            Handle_o22Reminder()
            Handle_p56Reminder()

            Handle_ScheduledReports()

            Handle_SqlTasks()

            Handle_AutoWorkflowSteps()

            If IsTime4Run(BO.j91RobotTaskFlag.RecurrenceP41, 60) Or Request.Item("recur") = "1" Or bolNowExplicit Then   'opakované worksheet úkony, projekty a úkoly stačí jednou za hodinu
                Handle_Recurrence_p41()
                Handle_Recurrence_p56()
                Handle_p40Queue()   'opakované paušální úkony
            End If




            If _curNow > Today.AddHours(15) And _curNow < Today.AddHours(19) Then
                If IsTime4Run(BO.j91RobotTaskFlag.CnbKurzy, 60) Then
                    Handle_CnbKurzy()
                End If
            End If

            If BO.ASS.GetConfigVal("autobackup", "1") = "1" And Now > Today.AddDays(1).AddMinutes(-60) Then
                'zbývá 60 minut do půlnoci na zálohování
                If IsTime4Run(BO.j91RobotTaskFlag.DbBackup, 60 * 5) Then  'stačí jednou za 5 hodin
                    Handle_DbBackup()
                End If
            End If

            If (Now > Today.AddMinutes(2 * 60) And Now < Today.AddMinutes(3 * 60 + 20)) Or Request.Item("ping") = "1" Then
                'mezi druhou a čtvrtou hodinou ráno vyčistit temp tabulky
                If IsTime4Run(BO.j91RobotTaskFlag.ClearTemp, 60 * 8) Then
                    _Factory.p85TempBoxBL.Recovery_ClearCompleteTemp()
                    WL(BO.j91RobotTaskFlag.ClearTemp, "", "Clear TEMP")
                End If
                If IsTime4Run(BO.j91RobotTaskFlag.CentralPing, 60 * 8) Then
                    Handle_CentralPing()
                End If

            End If

            log4net.LogManager.GetLogger("robotlog").Info("End")

            If Request.Item("now") = "" Then
                Me.lblMessage.Text = Format(Now, "dd.MM.yyyy HH:mm:ss") & " - robot spuštěn."
            Else
                Me.lblMessage.Text = String.Format("Robot spuštěn pro den {0}.", Request.Item("now"))
            End If

            If Request.Item("backup") = "1" Then
                Handle_DbBackup()
            End If
        End If

    End Sub

    Private Sub Handle_MailQueque()
        Dim mq As New BO.myQueryX40
        mq.x40State = BO.x40StateENUM.InQueque
        mq.TopRecordsOnly = 10

        Dim lisX40 As IEnumerable(Of BO.x40MailQueue) = _Factory.x40MailQueueBL.GetList(mq)
        WL(BO.j91RobotTaskFlag.MailQueue, "", String.Format("Počet zpráv k odeslání:{0}", lisX40.Count))

        If lisX40.Count > 0 Then
            've frontě čekají smtp zprávy k odeslání - maximálně 10 zpráv najednou
            For Each cMessage In lisX40
                _Factory.x40MailQueueBL.SendMessageFromQueque(cMessage)

            Next
        End If
    End Sub

    Private Sub Handle_Recurrence_p41()
        Dim mq As New BO.myQueryP41
        mq.IsRecurrenceMother = BO.BooleanQueryMode.TrueQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Dim lisMothers As IEnumerable(Of BO.p41Project) = _Factory.p41ProjectBL.GetList(mq).Where(Function(p) p.p41IsStopRecurrence = False)
        WL(BO.j91RobotTaskFlag.RecurrenceP41, "", String.Format("Počet projektů (matek): {0}", lisMothers.Count))
        If lisMothers.Count = 0 Then Return
        mq = New BO.myQueryP41
        mq.IsRecurrenceChild = BO.BooleanQueryMode.TrueQuery
        Dim lisChilds As IEnumerable(Of BO.p41Project) = _Factory.p41ProjectBL.GetList(mq)
        Dim lisP65 As IEnumerable(Of BO.p65Recurrence) = _Factory.p65RecurrenceBL.GetList(New BO.myQuery)

        For Each c In lisMothers
            Dim cP65 As BO.p65Recurrence = lisP65.First(Function(p) p.PID = c.p65ID)
            Dim cRD As BO.RecurrenceCalculation = _Factory.p65RecurrenceBL.CalculateDates(cP65, _curNow)
            'Dim datBase As Date = DateSerial(Year(Now), Month(Now), 1), bolNeed2Gen As Boolean = False
            'If cP65.p65RecurFlag = BO.RecurrenceType.Year Then
            '    datBase = DateSerial(Year(Now), 1, 1)
            'End If
            'If cP65.p65RecurFlag = BO.RecurrenceType.Quarter Then
            '    Select Case Month(Now)
            '        Case 1, 2, 3 : datBase = DateSerial(Year(Now), 1, 1)
            '        Case 4, 5, 6 : datBase = DateSerial(Year(Now), 4, 1)
            '        Case 7, 8, 9 : datBase = DateSerial(Year(Now), 7, 1)
            '        Case Else : datBase = DateSerial(Year(Now), 12, 1)
            '    End Select

            'End If
            'Dim datGen As Date = datBase.AddMonths(cP65.p65RecurGenToBase_M).AddDays(cP65.p65RecurGenToBase_D)
            If lisChilds.Where(Function(p) p.p41RecurMotherID = c.PID And p.p41RecurBaseDate = cRD.DatBase).Count = 0 And cRD.DatGen <= _curNow And cRD.DatBase > c.p41RecurBaseDate Then
                'potomek ještě neexistuje a nastal čas ho vygenerovat
                'Dim datPlanUntil As Date = Nothing
                'If cP65.p65IsPlanUntil Then datPlanUntil = cRD.DatBase.AddMonths(cP65.p65RecurPlanUntilToBase_M).AddDays(cP65.p65RecurPlanUntilToBase_D)

                Dim cNew As New BO.p41Project, intMotherPID As Integer = c.PID
                cNew = c
                With cNew
                    .p41RecurMotherID = intMotherPID
                    .p41RecurBaseDate = cRD.DatBase
                    .p65ID = 0 : .p41Code = "" : .ValidFrom = _curNow
                    If c.p41RecurNameMask <> "" Then .p41Name = c.p41RecurNameMask
                    .p41ParentID = c.PID   'potom bude podřízený projekt matce
                    .p41PlanUntil = cRD.DatPlanUntil
                    .p41PlanFrom = cRD.DatPlanFrom
                    .SetPID(0)
                    'If cP65.p65IsPlanFrom Or cP65.p65IsPlanUntil Then
                    '    .p41PlanFrom = cRD.DatBase.AddMonths(cP65.p65RecurPlanFromToBase_M).AddDays(cP65.p65RecurPlanFromToBase_D)
                    'Else
                    '    .p41PlanFrom = Nothing
                    'End If

                End With
                Dim lisFF As List(Of BO.FreeField) = _Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p41Project, intMotherPID, c.p42ID)
                If _Factory.p41ProjectBL.Save(cNew, Nothing, Nothing, _Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, intMotherPID).ToList, lisFF) Then
                    WL(BO.j91RobotTaskFlag.RecurrenceP41, "", "Mother: " & c.FullName & ", child: " & _Factory.p41ProjectBL.LastSavedPID.ToString & "/" & cNew.p41Name)
                Else
                    WL(BO.j91RobotTaskFlag.RecurrenceP41, "Mother: " & c.FullName & ", child: " & cNew.p41Name & ": " & _Factory.p41ProjectBL.ErrorMessage, "")
                End If

            End If
        Next
    End Sub
    Private Sub Handle_Recurrence_p56()
        Dim mq As New BO.myQueryP56
        mq.IsRecurrenceMother = BO.BooleanQueryMode.TrueQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Dim lisMothers As IEnumerable(Of BO.p56Task) = _Factory.p56TaskBL.GetList(mq).Where(Function(p) p.p56IsStopRecurrence = False)
        WL(BO.j91RobotTaskFlag.RecurrenceP56, "", String.Format("Počet úkolů (matek): {0}", lisMothers.Count))
        If lisMothers.Count = 0 Then Return
        mq = New BO.myQueryP56
        mq.IsRecurrenceChild = BO.BooleanQueryMode.TrueQuery
        Dim lisChilds As IEnumerable(Of BO.p56Task) = _Factory.p56TaskBL.GetList(mq)
        Dim lisP65 As IEnumerable(Of BO.p65Recurrence) = _Factory.p65RecurrenceBL.GetList(New BO.myQuery)

        For Each c In lisMothers
            Dim cP65 As BO.p65Recurrence = lisP65.First(Function(p) p.PID = c.p65ID)
            Dim cRD As BO.RecurrenceCalculation = _Factory.p65RecurrenceBL.CalculateDates(cP65, _curNow)
            
            If lisChilds.Where(Function(p) p.p56RecurMotherID = c.PID And p.p56RecurBaseDate = cRD.DatBase).Count = 0 And cRD.DatGen <= _curNow And cRD.DatBase > c.p56RecurBaseDate Then
                'potomek ještě neexistuje a nastal čas ho vygenerovat
                'ještě musí platit, že rozhodné datum potomka je větší než rozhodné datum matky

                Dim cNew As New BO.p56Task, intMotherPID As Integer = c.PID
                cNew = c
                With cNew
                    .p56RecurMotherID = intMotherPID
                    .p56RecurBaseDate = cRD.DatBase
                    .p65ID = 0 : .p56Code = "" : .ValidFrom = _curNow
                    If c.p56RecurNameMask <> "" Then .p56Name = c.p56RecurNameMask
                    .p56PlanUntil = cRD.DatPlanUntil
                    .p56PlanFrom = cRD.DatPlanFrom
                    .SetPID(0)


                End With
                Dim lisFF As List(Of BO.FreeField) = _Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p56Task, intMotherPID, c.p57ID)
                If _Factory.p56TaskBL.Save(cNew, _Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p56Task, intMotherPID).ToList, lisFF, "") Then
                    Dim intP56ID As Integer = _Factory.p56TaskBL.LastSavedPID
                    Dim lisP40 As IEnumerable(Of BO.p40WorkSheet_Recurrence) = _Factory.p40WorkSheet_RecurrenceBL.GetList(c.p41ID, c.PID).Where(Function(p) p.IsClosed = False)
                    If lisP40.Count > 0 Then
                        'K potomkům matky se má vygenerovat worksheet úkon
                        For Each cP40 In lisP40
                            Dim intP31ID As Integer = SaveP31OrigRecord(cP40, Now, "", _Factory.p56TaskBL.Load(intP56ID))
                            If intP31ID = 0 Then
                                WL(BO.j91RobotTaskFlag.RecurrenceP56, "", "SaveP31OrigRecord,p56ID=" & intP56ID.ToString & ", ERROR=" & _Factory.p31WorksheetBL.ErrorMessage)
                            End If
                        Next
                    End If

                    WL(BO.j91RobotTaskFlag.RecurrenceP56, "", "Mother: " & c.FullName & ", child: " & intP56ID.ToString & "/" & cNew.p41Name)
                Else
                    WL(BO.j91RobotTaskFlag.RecurrenceP56, "Mother: " & c.FullName & ", child: " & cNew.p56Name & ": " & _Factory.p56TaskBL.ErrorMessage, "")
                End If

            End If
        Next
    End Sub

    Private Function SaveP31OrigRecord(cRec As BO.p40WorkSheet_Recurrence, p31Date As Date, strText As String, cTask As BO.p56Task) As Integer
        If _lisP53 Is Nothing Then
            _lisP53 = _Factory.p53VatRateBL.GetList(New BO.myQuery)
        End If
        Dim cP34 As BO.p34ActivityGroup = _Factory.p34ActivityGroupBL.Load(cRec.p34ID)
        Dim cP31 As New BO.p31WorksheetEntryInput
        With cP31
            If cTask Is Nothing Then
                .p31Text = strText
                .p31Date = p31Date
            Else
                .p56ID = cTask.PID
                .p31Date = cTask.p56RecurBaseDate
                .p31Text = _Factory.ftBL.get_ParsedText_With_Period(cRec.p40Text, .p31Date, 0)
            End If
            .j02ID = cRec.j02ID
            .p41ID = cRec.p41ID
            .p34ID = cRec.p34ID
            .p32ID = cRec.p32ID
            .Value_Orig = CStr(cRec.p40Value)
            .Value_Orig_Entried = CStr(cRec.p40Value)
            If cP34.p33ID = BO.p33IdENUM.PenizeBezDPH Or cP34.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                .j27ID_Billing_Orig = cRec.j27ID
                If cRec.x15ID > BO.x15IdEnum.BezDPH Then
                    Dim lisVR As IEnumerable(Of BO.p53VatRate) = _lisP53.Where(Function(p) p.j27ID = cRec.j27ID And p.x15ID = cRec.x15ID)
                    If lisVR.Count > 0 Then
                        .VatRate_Orig = lisVR(0).p53Value
                    End If
                End If

                .Amount_WithoutVat_Orig = cRec.p40Value
                If cP34.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                    .Amount_Vat_Orig = .VatRate_Orig / 100 * cRec.p40Value
                    .Amount_WithVat_Orig = .Amount_Vat_Orig + .Amount_WithoutVat_Orig
                End If

            End If

        End With
        Dim bol As Boolean = _Factory.p31WorksheetBL.SaveOrigRecord(cP31, Nothing)
        If bol Then
            Return _Factory.p31WorksheetBL.LastSavedPID
        Else
            Return 0
        End If
    End Function
    Private Sub Handle_p40Queue()
        Dim lisP40 As IEnumerable(Of BO.p40WorkSheet_Recurrence) = _Factory.p40WorkSheet_RecurrenceBL.GetList_WaitingForGenerate(_curNow)
        WL(BO.j91RobotTaskFlag.p40, "", String.Format("Počet p40 záznamů :{0}", lisP40.Count))
        If lisP40.Count = 0 Then Return

        For Each cRec In lisP40
            Dim cP39 As BO.p39WorkSheet_Recurrence_Plan = _Factory.p40WorkSheet_RecurrenceBL.LoadP39_FirstWaiting(cRec.PID, _curNow)
            If Not cP39 Is Nothing Then
                'vygenerovat úkon
                Dim intP31ID As Integer = SaveP31OrigRecord(cRec, cP39.p39Date, cP39.p39Text, Nothing)
                ''Dim cP34 As BO.p34ActivityGroup = _Factory.p34ActivityGroupBL.Load(cRec.p34ID)
                ''Dim cP31 As New BO.p31WorksheetEntryInput
                ''With cP31
                ''    .j02ID = cRec.j02ID
                ''    .p41ID = cRec.p41ID
                ''    .p34ID = cRec.p34ID
                ''    .p32ID = cRec.p32ID
                ''    .p31Text = cP39.p39Text
                ''    .p31Date = cP39.p39Date
                ''    .Value_Orig = CStr(cRec.p40Value)
                ''    .Value_Orig_Entried = CStr(cRec.p40Value)
                ''    If cP34.p33ID = BO.p33IdENUM.PenizeBezDPH Or cP34.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                ''        .j27ID_Billing_Orig = cRec.j27ID
                ''        If cRec.x15ID > BO.x15IdEnum.BezDPH Then
                ''            Dim lisVR As IEnumerable(Of BO.p53VatRate) = lisP53.Where(Function(p) p.j27ID = cRec.j27ID And p.x15ID = cRec.x15ID)
                ''            If lisVR.Count > 0 Then
                ''                .VatRate_Orig = lisVR(0).p53Value
                ''            End If
                ''        End If

                ''        .Amount_WithoutVat_Orig = cRec.p40Value
                ''        If cP34.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                ''            .Amount_Vat_Orig = .VatRate_Orig / 100 * cRec.p40Value
                ''            .Amount_WithVat_Orig = .Amount_Vat_Orig + .Amount_WithoutVat_Orig
                ''        End If

                ''    End If

                ''End With
                ''Dim bol As Boolean = _Factory.p31WorksheetBL.SaveOrigRecord(cP31, Nothing)
                If intP31ID > 0 Then
                    WL(BO.j91RobotTaskFlag.p40, "", "p40-new robot worksheet record,p39ID=" & cP39.p39ID.ToString & ", p31ID=" & intP31ID.ToString)

                    _Factory.p40WorkSheet_RecurrenceBL.Update_p31Instance(cP39.p39ID, intP31ID, "")
                Else
                    WL(BO.j91RobotTaskFlag.p40, "", "p40-new robot worksheet record,p39ID=" & cP39.p39ID.ToString & ", ERROR=" & _Factory.p31WorksheetBL.ErrorMessage)

                    _Factory.p40WorkSheet_RecurrenceBL.Update_p31Instance(cP39.p39ID, 0, _Factory.p31WorksheetBL.ErrorMessage)
                End If
            End If

        Next
    End Sub

    Public Sub Handle_o22Reminder()
        _Factory.o22MilestoneBL.Handle_Reminder()
    End Sub
    Public Sub Handle_o23Reminder()
        _Factory.o23DocBL.Handle_Reminder()
    End Sub
    Public Sub Handle_p56Reminder()
        _Factory.p56TaskBL.Handle_Reminder()
    End Sub

    Public Sub Handle_ImapRobot()
        Dim lis As IEnumerable(Of BO.o41InboxAccount) = _Factory.o41InboxAccountBL.GetList(New BO.myQuery)
        WL(BO.j91RobotTaskFlag.ImapImport, "", String.Format("IMAP robot, Počet imap účtů: {0}", lis.Count))

        For Each c In lis
            _Factory.o42ImapRuleBL.HandleWaitingImapMessages(c)
        Next

    End Sub

    Public Sub Handle_CnbKurzy()
        Dim datImport As Date = DateSerial(Year(Now), Month(Now), Day(Now))

        Dim lisM62 As IEnumerable(Of BO.m62ExchangeRate) = _Factory.m62ExchangeRateBL.GetList().Where(Function(p) p.m62RateType = BO.m62RateTypeENUM.InvoiceRate And p.m62Date = datImport And p.UserInsert = "robot")
        WL(BO.j91RobotTaskFlag.CnbKurzy, "", "CNB import měnových kurzů.")
        If lisM62.Count = 0 Then
            _Factory.m62ExchangeRateBL.ImportRateList_CNB(datImport)
        End If
    End Sub
    Public Sub Handle_CentralPing()
        Dim strGUID As String = _Factory.x35GlobalParam.GetValueString("AppScope")
        Dim strName As String = _Factory.x35GlobalParam.GetValueString("AppName")
        Dim s As String = "SELECT count(*) as PocetZaznamu,max(p31DateInsert) as PosledniZapis,count(distinct case when p31dateinsert between dateadd(day,-14,getdate()) and getdate() then j02id end) as PocetZapisovacu FROM p31Worksheet"
        Dim pars As New List(Of BO.PluginDbParameter)
        Dim dt As DataTable = _Factory.pluginBL.GetDataTable(s, pars)

        s = "http://www.marktime50.net/mtrc/ping.aspx?guid=" & strGUID & "&name=" & Server.HtmlEncode(strName) & "&verze=" & BO.ASS.GetUIVersion(True)
        For Each row As DataRow In dt.Rows
            s += "&poslednizapis=" & BO.BAS.FD(row.Item("PosledniZapis")) & "&pocetzaznamu=" & row.Item("PocetZaznamu").ToString & "&pocetzapisovacu=" & row.Item("PocetZapisovacu").ToString
        Next


        Dim rq As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(s)
        Dim rs As System.Net.HttpWebResponse = rq.GetResponse()

        WL(BO.j91RobotTaskFlag.CentralPing, "", String.Format("Central PING, name={0}", strName))

    End Sub

    Public Sub Handle_ScheduledReports()
        Dim lis As IEnumerable(Of BO.x31Report) = _Factory.x31ReportBL.GetList(New BO.myQuery).Where(Function(p) p.x31IsScheduling = True And p.x31SchedulingReceivers <> "")
        WL(BO.j91RobotTaskFlag.ScheduledReports, "", String.Format("Počet sestav: {0}", lis.Count))

        For Each c In lis
            If _Factory.x31ReportBL.IsWaiting4AutoGenerate(c) Then

                Dim strRepFullPath As String = _Factory.x35GlobalParam.UploadFolder
                If c.ReportFolder <> "" Then
                    strRepFullPath += "\" & c.ReportFolder
                End If
                strRepFullPath += "\" & c.ReportFileName
                Dim cRep As New clsReportOnBehind()
                Dim strOutputFileName As String = cRep.GenerateReport2Temp(_Factory, strRepFullPath)

                Dim message As New Rebex.Mail.MailMessage
                With message
                    .BodyText = "Automaticky generovaná zpráva ze systému MARKTIME." & vbCrLf & vbCrLf & "Report: " & c.x31Name & vbCrLf & vbCrLf & vbCrLf & "Pozdrav posílá MARKTIME robot!"
                    .From.Add(New Rebex.Mime.Headers.MailAddress(_Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress"), "MARKTIME robot"))
                    .Subject = BO.BAS.OM3(c.x31Name, 30) & " | MARKTIME REPORT"
                    .Attachments.Add(New Rebex.Mail.Attachment(_Factory.x35GlobalParam.TempFolder & "\" & strOutputFileName))
                End With
                c.x31SchedulingReceivers = Replace(c.x31SchedulingReceivers, ",", ";")
                Dim a() As String = Split(c.x31SchedulingReceivers, ";")
                Dim recipients As New List(Of BO.x43MailQueue_Recipient)
                For i = 0 To UBound(a)
                    _Factory.x40MailQueueBL.AppendRecipient(recipients, a(i), "")
                Next
                With _Factory.x40MailQueueBL
                    Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, BO.x29IdEnum.x31Report, c.PID, BO.x40StateENUM.InQueque, 0)
                    If intMessageID > 0 Then
                        _Factory.x31ReportBL.UpdateLastScheduledRun(c.PID, Now)
                        If Not .SendMessageFromQueque(intMessageID) Then
                            WL(BO.j91RobotTaskFlag.ScheduledReports, .ErrorMessage, "")
                            Response.Write(.ErrorMessage)
                        Else
                            WL(BO.j91RobotTaskFlag.ScheduledReports, "", String.Format("Odeslán report ", message.Subject))
                        End If
                    Else
                        WL(BO.j91RobotTaskFlag.ScheduledReports, .ErrorMessage, "")
                        Response.Write(.ErrorMessage)
                    End If
                End With
            End If
        Next

    End Sub

    Private Sub cmdRunNow_Click(sender As Object, e As EventArgs) Handles cmdRunNow.Click
        Response.Redirect("robot.aspx?blank=1&now=" & Format(Me.datNow.SelectedDate, "dd.MM.yyyy"))
    End Sub

    Public Sub Handle_SqlTasks()
        Dim lis As IEnumerable(Of BO.x48SqlTask) = _Factory.x48SqlTaskBL.GetList(New BO.myQuery)
        WL(BO.j91RobotTaskFlag.SqlTasks, "", String.Format("Počet sql úloh k zpracování: {0}", lis.Count))
        For Each c In lis
            If _Factory.x48SqlTaskBL.IsWaiting4AutoGenerate(c) Then
                WL(BO.j91RobotTaskFlag.SqlTasks, "", "SqlTask: " & c.x48Name)

                Dim dt As DataTable = _Factory.pluginBL.GetDataTable(c.x48Sql, Nothing)
                If _Factory.pluginBL.ErrorMessage <> "" Then
                    WL(BO.j91RobotTaskFlag.SqlTasks, _Factory.pluginBL.ErrorMessage, "")

                    Continue For 'Chyba v SQL -> jít na další úlohu nebo končit
                End If
                If c.x48MailBody = "" And c.x48MailSubject = "" Then
                    WL(BO.j91RobotTaskFlag.SqlTasks, "", "Sql result: " & dt.Rows.Count.ToString & " rows.")

                    Continue For 'Nemá se posílat mail zpráva ->jít na další úlohu nebo končit


                End If

                Dim strRepFullPath As String = _Factory.x35GlobalParam.UploadFolder, cX31 As BO.x31Report = Nothing

                If c.x31ID <> 0 Then
                    cX31 = _Factory.x31ReportBL.Load(c.x31ID)
                    If cX31.ReportFolder <> "" Then
                        strRepFullPath += "\" & cX31.ReportFolder
                    End If
                    strRepFullPath += "\" & cX31.ReportFileName
                End If
                For Each dr As DataRow In dt.Rows   'kolik řádků sql výstupu, tolik mail zpráv
                    Dim strOutputPdfFileName As String = ""
                    If Not cX31 Is Nothing Then
                        Dim cRep As New clsReportOnBehind()
                        If c.x48TaskOutputFlag = BO.x48TaskOutputFlagENUM.PIDsTable Then
                            cRep.Query_RecordPID = dr(0)
                        End If
                        strOutputPdfFileName = cRep.GenerateReport2Temp(_Factory, strRepFullPath)

                    End If


                    'odeslat mail zprávu:
                    Dim message As New Rebex.Mail.MailMessage
                    With message
                        .From.Add(New Rebex.Mime.Headers.MailAddress(_Factory.x35GlobalParam.GetValueString("SMTP_SenderAddress"), "MARKTIME robot"))
                        .Subject = c.x48MailSubject
                        .BodyText = c.x48MailBody
                        If strOutputPdfFileName <> "" Then
                            .Attachments.Add(New Rebex.Mail.Attachment(_Factory.x35GlobalParam.TempFolder & "\" & strOutputPdfFileName))
                        End If
                    End With
                    c.x48MailTo = Replace(c.x48MailTo, ",", ";")

                    Dim a() As String = Split(c.x48MailTo, ";")
                    Dim recipients As New List(Of BO.x43MailQueue_Recipient)
                    For i = 0 To UBound(a)
                        Dim cc As New BO.x43MailQueue_Recipient()
                        cc.x43Email = a(i)
                        If c.x48TaskOutputFlag = BO.x48TaskOutputFlagENUM.RunSql Then
                            cc.x43RecipientFlag = BO.x43RecipientIdEnum.recTO
                        Else
                            cc.x43RecipientFlag = BO.x43RecipientIdEnum.recBCC
                        End If
                        recipients.Add(cc)
                    Next
                    If c.x29ID = BO.x29IdEnum.j02Person Then
                        Dim cJ02 As BO.j02Person = _Factory.j02PersonBL.Load(dr(0))
                        Dim cc As New BO.x43MailQueue_Recipient()
                        cc.x43Email = cJ02.j02Email
                        cc.x43RecipientFlag = BO.x43RecipientIdEnum.recTO
                        recipients.Add(cc)
                    End If
                    If recipients.Count > 0 Then
                        With _Factory.x40MailQueueBL
                            Dim intMessageID As Integer = .SaveMessageToQueque(message, recipients, c.x29ID, c.PID, BO.x40StateENUM.InQueque, 0)
                            If intMessageID > 0 Then

                                If Not .SendMessageFromQueque(intMessageID) Then
                                    log4net.LogManager.GetLogger("robotlog").Error(.ErrorMessage)
                                End If
                            Else
                                log4net.LogManager.GetLogger("robotlog").Error(.ErrorMessage)
                            End If
                        End With
                    End If

                Next


                _Factory.x48SqlTaskBL.UpdateLastScheduledRun(c.PID, Now)


            End If
        Next

    End Sub

    Private Sub Handle_DbBackup()
        Dim cBL As New BL.SysObjectBL()
        Dim strDir As String = BO.ASS.GetConfigVal("backupdir", ""), bolTestFileSystem As Boolean = False
        If strDir = "" Then
            strDir = _Factory.x35GlobalParam.UploadFolder & "\dbBackup"
            If Not System.IO.Directory.Exists(strDir) Then
                Try
                    System.IO.Directory.CreateDirectory(strDir)
                Catch ex As Exception
                    WL(BO.j91RobotTaskFlag.DbBackup, "Handle_DbBackup: " & ex.Message, "")
                    Return
                End Try
            End If
            bolTestFileSystem = True
        End If

        Dim strBackupFileName As String = "MARKTIME50_Backup_day" & Weekday(Now, Microsoft.VisualBasic.FirstDayOfWeek.Monday).ToString & ".bak"

        If bolTestFileSystem Then
            If System.IO.File.Exists(strDir & "\" & strBackupFileName) Then
                If System.IO.File.Exists(strDir & "\backup.info") Then
                    Dim cF As New BO.clsFile
                    Dim strDat As String = cF.GetFileContents(strDir & "\backup.info", , , True)
                    If strDat = Format(Now, "dd.MM.yyyy") Then
                        Return  'záloha pro tento den již existuje
                    Else
                        Try
                            System.IO.File.Delete(strDir & "\" & strBackupFileName)
                        Catch ex As Exception
                            WL(BO.j91RobotTaskFlag.DbBackup, "Nelze odstranit starý backup soubor: " & strDir & "\" & strBackupFileName, "")
                            Return
                        End Try
                    End If
                End If
            End If
        End If


        Dim strRet As String = cBL.CreateDbBackup(, strDir, strBackupFileName, bolTestFileSystem)
        If strRet <> strBackupFileName Then
            WL(BO.j91RobotTaskFlag.DbBackup, "Handle_DbBackup: " & strRet, "")
        Else
            If bolTestFileSystem Then
                Dim cF As New BO.clsFile
                cF.SaveText2File(strDir & "\backup.info", Format(Now, "dd.MM.yyyy"))

            End If
        End If

        strBackupFileName = "MARKTIME50_MEMBERSHIP_Backup_day" & Weekday(Now, Microsoft.VisualBasic.FirstDayOfWeek.Monday).ToString & ".bak"
        If bolTestFileSystem Then
            If System.IO.File.Exists(strDir & "\" & strBackupFileName) Then
                System.IO.File.Delete(strDir & "\" & strBackupFileName)
            End If
        End If

        strRet = cBL.CreateDbBackup(System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationServices").ToString, strDir, strBackupFileName, bolTestFileSystem)
    End Sub

    Private Sub WL(Task As BO.j91RobotTaskFlag, strError As String, strInfo As String)
        Dim c As New BO.j91RobotLog
        c.j91BatchGuid = _BatchGuid
        c.j91Account = _Factory.SysUser.j03Login
        c.j91Date = Now
        c.j91ErrorMessage = strError
        c.j91InfoMessage = strInfo
        c.j91TaskFlag = Task

        _Factory.ftBL.AppendRobotLog(c)
        If strError <> "" Then
            log4net.LogManager.GetLogger("robotlog").Error("Robot task " & CInt(Task).ToString & ": " & strError)
        End If
        ''If strInfo <> "" Then
        ''    log4net.LogManager.GetLogger("robotlog").Info(strInfo)
        ''End If
    End Sub

    Private Function IsTime4Run(TaskFlag As BO.j91RobotTaskFlag, dblMinIntervalMinutes As Double) As Boolean
        Dim c As BO.j91RobotLog = _Factory.ftBL.GetLastRobotRun(TaskFlag)
        If c Is Nothing Then Return True
        If c.j91Date.AddMinutes(dblMinIntervalMinutes) > _curNow Then
            Return False
        End If
        Return True
    End Function

    Private Sub Handle_AutoWorkflowSteps()
        WL(BO.j91RobotTaskFlag.AutoWorkflowSteps, "", "Handle_AutoWorkflowSteps")
        For Each c In _Factory.b01WorkflowTemplateBL.GetList(New BO.myQuery)
            Handle_AutoWorkflowSteps_OneTemplate(c)
        Next
    End Sub
    Private Sub Handle_AutoWorkflowSteps_OneTemplate(cB01 As BO.b01WorkflowTemplate)
        Dim lisB06 As IEnumerable(Of BO.b06WorkflowStep) = _Factory.b06WorkflowStepBL.GetList(cB01.PID).Where(Function(p) p.b06ValidateAutoMoveSQL <> "")
        For Each cB06 In lisB06
            Dim pids As New List(Of Integer)
            Select Case cB01.x29ID
                Case BO.x29IdEnum.p56Task
                    Dim mq As New BO.myQueryP56
                    mq.b02ID = cB06.b02ID
                    mq.MG_SelectPidFieldOnly = True
                    pids = _Factory.p56TaskBL.GetList(mq).Select(Function(p) p.PID).ToList
                Case BO.x29IdEnum.p41Project
                    Dim mq As New BO.myQueryP41
                    mq.b02ID = cB06.b02ID
                    mq.MG_SelectPidFieldOnly = True
                    pids = _Factory.p41ProjectBL.GetList(mq).Select(Function(p) p.PID).ToList
                Case BO.x29IdEnum.p91Invoice
                    Dim mq As New BO.myQueryP91
                    mq.b02ID = cB06.b02ID
                    mq.MG_SelectPidFieldOnly = True
                    pids = _Factory.p91InvoiceBL.GetList(mq).Select(Function(p) p.PID).ToList
                Case BO.x29IdEnum.p28Contact
                    Dim mq As New BO.myQueryP28
                    mq.b02ID = cB06.b02ID
                    mq.MG_SelectPidFieldOnly = True
                    pids = _Factory.p28ContactBL.GetList(mq).Select(Function(p) p.PID).ToList
                Case BO.x29IdEnum.o23Doc
                    Dim mq As New BO.myQueryO23(0)
                    mq.b02IDs = BO.BAS.ConvertInt2List(cB06.b02ID)
                    mq.MG_SelectPidFieldOnly = True
                    pids = _Factory.o23DocBL.GetList(mq).Select(Function(p) p.PID).ToList
            End Select
            If pids.Count > 0 Then
                For Each intPID As Integer In pids
                    If _Factory.b06WorkflowStepBL.GetAutoWorkflowSQLResult(intPID, cB06) = 1 Then
                        'podmínka automaticky spuštěného kroku splněna
                        WL(BO.j91RobotTaskFlag.AutoWorkflowSteps, "", String.Format("Handle_AutoWorkflowSteps: RecordPID={0}, b06Name={1}", intPID, cB06.b06Name))
                        _Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, intPID, cB06.x29id, "", "", False, Nothing)
                    End If
                Next
            End If

        Next

    End Sub
End Class