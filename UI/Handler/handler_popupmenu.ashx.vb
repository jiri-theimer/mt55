Imports System.Web
Imports System.Web.Services

Public Class handler_popupmenu
    Implements System.Web.IHttpHandler
    Private _lis As New List(Of BO.ContextMenuItem)
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "application/json"


        Dim factory As BL.Factory = Nothing
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            FinalRW(context, "factory is nothing") : Return
        End If

        Dim strPREFIX As String = Trim(context.Request.Item("prefix"))
        Dim intPID As Integer = BO.BAS.IsNullInt(Trim(context.Request.Item("pid")))
        Dim strFlag As String = Trim(context.Request.Item("flag"))

        If strPREFIX = "" Then
            FinalRW(context, "prefix is missing") : Return
        End If
        If Left(strPREFIX, 5) = "admin" Then
            HandleAdminMenu(strPREFIX, intPID, factory) 'číselníky za administrace
            FinalRW(context, "") : Return
        End If
        ''CI("flag: " & strFlag, "", True)

        Select Case strPREFIX
            Case "newrec"
                RenderNewRecMenu(factory)  'hlavní menu - odkazy k založení nového záznamu
           
            Case "p31"
                HandleP31(intPID, factory, strFlag)

            Case "p56"
                HandleP56(intPID, factory, strFlag)
            Case "p28"
                HandleP28(intPID, factory, strFlag)
            Case "p41"
                HandleP41(intPID, factory, strFlag)
            Case "p91"
                HandleP91(intPID, factory, strFlag)
            Case "o23"
                HandleO23(intPID, factory, strFlag)
            Case "j02"
                HandleJ02(intPID, factory, strFlag)
            Case "p90"
                HandleP90(intPID, factory)
            Case "p51"
                HandleP51(intPID, factory)
            Case "x40"
                HandleX40(intPID, factory)
            Case Else
                CI("Nezpracovatelný PREFIX", "")
        End Select

        
        Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer
        Dim s As String = jss.Serialize(_lis)
        context.Response.Write(s)


    End Sub
   
    Private Sub HandleP56(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.p56Task = factory.p56TaskBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return

        Dim cDisp As BO.p56RecordDisposition = factory.p56TaskBL.InhaleRecordDisposition(cRec)
        If Not cDisp.ReadAccess Then CI("Nemáte přístup k tomuto úkolu.", "", True) : Return
        If strFlag <> "pagemenu" Then
            If factory.SysUser.j04IsMenu_Task Then
                REL(cRec.FullName, "p56_framework.aspx?pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
            Else
                CI(cRec.FullName, "", True, "Images/information.png")
            End If
        End If

        If cDisp.OwnerAccess Then
            SEP()
            CI("Upravit kartu úkolu", "p56_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
            
        End If
        SEP()
        CI("[NOVÝ]", "", , "Images/new4menu.png")
        If cDisp.OwnerAccess Then
            CI("Kopírovat", "p56_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png", True)    'pod nový
        End If

        CI("Založit úkol", "p56_record.aspx?pid=0&masterprefix=p41&masterpid=0", , "Images/new4menu.png", True)   'pod nový

        If factory.SysUser.j04IsMenu_Notepad Then
            CI("Vytvořit dokument", "o23_record.aspx?masterprefix=p56&masterpid=" & cRec.PID.ToString, , "Images/notepad.png", True)    'pod nový
        End If

        Dim cP41 As BO.p41Project = factory.p41ProjectBL.Load(cRec.p41ID)

        If cDisp.P31_Create Then
            SEP()
            CI("[ZAPSAT WORKSHEET]", "p31_record.aspx?p56id=" & intPID.ToString, cRec.IsClosed, "Images/worksheet.png")
            If Not cRec.IsClosed Then
                Dim lisP34 As IEnumerable(Of BO.p34ActivityGroup) = factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(cP41.PID, cP41.p42ID, cP41.j18ID, factory.SysUser.j02ID)
                For Each c In lisP34
                    CI(c.p34Name, "p31_record.aspx?pid=0&p56id=" & cRec.PID.ToString & "&p34id=" + c.PID.ToString, , "Images/worksheet.png", True)
                Next
            End If
        End If

        Dim cDispP41 As BO.p41RecordDisposition = factory.p41ProjectBL.InhaleRecordDisposition(cP41)
        Dim mq As New BO.myQueryP31
        mq.p56IDs = BO.BAS.ConvertInt2List(cRec.PID)
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
        Dim cSum As BO.p31WorksheetSum = factory.p31WorksheetBL.LoadSumRow(mq, True, True)
        Dim intWIPx As Integer = 0, intAPPx As Integer = 0
        With cSum
            intWIPx = .WaitingOnApproval_Hours_Count + .WaitingOnApproval_Other_Count
            intAPPx = .WaitingOnInvoice_Hours_Count + .WaitingOnInvoice_Other_Count
        End With
        If intWIPx > 0 Or intAPPx > 0 Then
            Dim bolCanApprove As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver)
            If Not bolCanApprove And cDispP41.x67IDs.Count > 0 Then
                Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = factory.x67EntityRoleBL.GetList_o28(cDispP41.x67IDs)
                If lisO28.Where(Function(p) p.o28PermFlag = BO.o28PermFlagENUM.CistASchvalovatVProjektu Or p.o28PermFlag = BO.o28PermFlagENUM.CistAEditASchvalovatVProjektu).Count > 0 Then
                    bolCanApprove = True
                End If
            End If
            If bolCanApprove Then
                SEP()
                Dim ss As String = String.Format("Schválit rozpracované úkony ({0}x)", intWIPx)
                If intWIPx = 0 And intAPPx > 0 Then ss = String.Format("Přes-schválit ({0}x) schválené", intAPPx)
                If intWIPx > 0 And intAPPx > 0 Then ss = String.Format("Schválit ({0}x)/přes-schválit ({1}x)", intWIPx, intAPPx)
                CI(ss, "entity_modal_approving.aspx?prefix=p56&pid=" & intPID.ToString, , "Images/approve.png", False, True)
            End If
        End If

        SEP()
        CI("Posunout/Doplnit", "workflow_dialog.aspx?prefix=p56&pid=" & intPID.ToString, , "Images/workflow.png")


        If factory.SysUser.j04IsMenu_Project And cRec.p41ID > 0 Then
            CI("[ODKAZ]", "", , "Images/link.png")

            REL(cP41.PrefferedName, "p41_framework.aspx?pid=" & cRec.p41ID.ToString, "_top", "Images/project.png", True)
            If cP41.p28ID_Client > 0 And factory.SysUser.j04IsMenu_Contact Then
                REL(cP41.Client, "p28_framework.aspx?pid=" & cP41.p28ID_Client.ToString, "_top", "Images/contact.png", True)
            End If
        End If
        If factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot) Then
            SEP()
            REL("Statistiky úkolu", "p31_sumgrid.aspx?masterprefix=p56&masterpid=" & cRec.PID.ToString, "_top", "Images/pivot.png")
        End If
        SEP()
        CI("Tisková sestava", "report_modal.aspx?prefix=p56&pid=" & intPID.ToString, , "Images/report.png")
        CI("Odeslat e-mail", "sendmail.aspx?prefix=p56&pid=" & cRec.PID.ToString, , "Images/email.png")

        SEP()
        CI("[DALŠÍ]", "", , "Images/more.png")
        CI("Oštítkovat", "tag_binding.aspx?prefix=p56&pids=" & intPID.ToString, , "Images/tag.png", True)


        If cRec.b01ID = 0 Then CI("Doplnit poznámku, komentář, přílohu", "b07_create.aspx?masterprefix=p56&masterpid=" & cRec.PID.ToString, , "Images/comment.png", True)

        REL("Historie odeslané pošty", "x40_framework.aspx?masterprefix=p56&masterpid=" & cRec.PID.ToString, "_top", "Images/email.png", True)
        If cDisp.OwnerAccess Then
            CI("Historie záznamu", "entity_timeline.aspx?prefix=p56&pid=" & cRec.PID.ToString, , "Images/event.png", True)
        End If

        CI("Plugin", "plugin_modal.aspx?prefix=p56&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)


    End Sub
    Private Sub HandleP91(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.p91Invoice = factory.p91InvoiceBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return

        Dim cDisp As BO.p91RecordDisposition = factory.p91InvoiceBL.InhaleRecordDisposition(cRec)
        If cDisp.ReadAccess Then
            If strFlag <> "pagemenu" Then
                REL(cRec.p92Name & ": " & cRec.p91Code, "p91_framework.aspx?pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
            End If

            If cDisp.OwnerAccess Then
                SEP()
                CI("Upravit kartu dokladu", "p91_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
            End If
            If cRec.b01ID > 0 Then
                SEP()
                CI("Posunout/doplnit", "workflow_dialog.aspx?prefix=p91&pid=" & cRec.PID.ToString, , "Images/workflow.png")
            End If

            SEP()
            CI("[AKCE]", "", , "Images/wizard.png")
            CI("Zapsat úhradu", "p91_pay.aspx?pid=" & intPID.ToString, , "Images/payment.png", True)
            CI("Převést fakturu na jinou měnu", "p91_change_currency.aspx?pid=" & intPID.ToString, , "Images/recalc.png", True)
            CI("Převést fakturu na jinou DPH sazbu", "p91_change_vat.aspx?pid=" & intPID.ToString, , "Images/recalc.png", True)            
            CI("Spárovat fakturu s úhradou zálohy", "p91_proforma.aspx?pid=" & intPID.ToString, , "Images/proforma.png", True)            
            If cRec.p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice Then
                CI("Vytvořit k faktuře opravný doklad", "p91_creditnote.aspx?pid=" & intPID.ToString, , "Images/correction_down.gif", True)
            End If


            SEP()
            CI("[ODKAZ]", "", , "Images/link.png")
            If cRec.p91ID_CreditNoteBind > 0 Then                
                Dim cP91 As BO.p91Invoice = factory.p91InvoiceBL.Load(cRec.p91ID_CreditNoteBind)
                REL(cP91.p92Name & ": " & cP91.p91Code, "p91_framework.aspx?pid=" & cP91.PID.ToString, "_top", "Images/invoice.png", True)   'pod odkaz
            Else
                If cRec.p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice Then
                    Dim cP91 As BO.p91Invoice = factory.p91InvoiceBL.LoadCreditNote(cRec.PID)
                    If Not cP91 Is Nothing Then                        
                        REL(cP91.p92Name & ": " & cP91.p91Code, "p91_framework.aspx?pid=" & cP91.PID.ToString, "_top", "Images/correction_down.gif", True)   'pod odkaz
                    End If
                End If
            End If
            
            If factory.SysUser.j04IsMenu_Contact And cRec.p28ID > 0 Then
                REL(cRec.p28Name, "p28_framework.aspx?pid=" & cRec.p28ID.ToString, "_top", "Images/contact.png", True)   'pod odkaz
            End If
            If factory.SysUser.j04IsMenu_Project And cRec.p41ID_First > 0 Then
                REL(cRec.p41Name, "p41_framework.aspx?pid=" & cRec.p41ID_First.ToString, "_top", "Images/project.png", True) 'pod odkaz
            End If
            If factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot) Then
                SEP()
                REL("Statistiky faktury", "p31_sumgrid.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString, "_top", "Images/pivot.png")
            End If
            SEP()
            Dim cP92 As BO.p92InvoiceType = factory.p92InvoiceTypeBL.Load(cRec.p92ID)
            With cP92
                If .x31ID_Invoice > 0 Then
                    CI("Sestava dokladu", "report_modal.aspx?x31id=" & .x31ID_Invoice.ToString & "&prefix=p91&pid=" & intPID.ToString, , "Images/report.png")
                End If
                If .x31ID_Attachment > 0 Then
                    CI("Sestava přílohy", "report_modal.aspx?x31id=" & .x31ID_Attachment.ToString & "&prefix=p91&pid=" & intPID.ToString, , "Images/report.png")
                End If
            End With
            CI("Tisková sestava", "report_modal.aspx?prefix=p91&pid=" & intPID.ToString, , "Images/report.png")
            CI("Odeslat e-mail", "sendmail.aspx?prefix=p91&pid=" & cRec.PID.ToString, , "Images/email.png")
        Else
            CI(cRec.p92Name & ": " & cRec.p91Code, "", True, "Images/information.png")
        End If

        SEP()
        CI("[DALŠÍ]", "", , "Images/more.png")
        CI("Oštítkovat", "tag_binding.aspx?prefix=p91&pids=" & intPID.ToString, , "Images/tag.png", True)
        If cDisp.OwnerAccess Then
            CI("Import úhrad z ABO souboru", "p91_pay_aboimport.aspx", , "Images/payment.png", True)
        End If
        If cDisp.ReadAccess Then
            CI("Export do účetnictví POHODA", "p91_export2pohoda.aspx?pid=" & cRec.PID.ToString, , "Images/license.png", True)
        End If
        If factory.SysUser.j04IsMenu_Notepad Then
            CI("Vytvořit dokument", "o23_record.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString, , "Images/notepad.png", True)
        End If


        If cRec.b01ID = 0 Then CI("Doplnit poznámku, komentář, přílohu", "b07_create.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString, , "Images/comment.png", True)


        REL("Historie odeslané pošty", "x40_framework.aspx?masterprefix=p91&masterpid=" & cRec.PID.ToString, "_top", "Images/email.png", True)
        If cDisp.OwnerAccess Then
            CI("Historie záznamu", "entity_timeline.aspx?prefix=p91&pid=" & cRec.PID.ToString, , "Images/event.png", True)
        End If

        CI("Plugin", "plugin_modal.aspx?prefix=p91&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)
        CI("Čárový kód", "barcode.aspx?prefix=p91&pid=" & cRec.PID.ToString, , "Images/barcode.png", True)

    End Sub
    Private Sub HandleP31(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.p31Worksheet = factory.p31WorksheetBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return
        Dim strMasterPrefix As String = Left(strFlag, 3)
        Select Case strFlag
            Case "p31_approving_step3"  'schvalování
                CI("Fakturovat", "javascript:ContextMenu_Batch4(" & cRec.PID.ToString & ")", , "Images/a14.gif")
                SEP()
                CI("Zahrnout do paušálu", "javascript:ContextMenu_Batch6(" & cRec.PID.ToString & ")", , "Images/a16.gif")
                SEP()                
                CI("Viditelný odpis", "javascript:ContextMenu_Batch2(" & cRec.PID.ToString & ")", , "Images/a12.gif")
                CI("Skrytý odpis", "javascript:ContextMenu_Batch3(" & cRec.PID.ToString & ")", , "Images/a13.gif")
                SEP()
                CI("Fakturovat později", "javascript:ContextMenu_Batch7(" & cRec.PID.ToString & ")", , "Images/a17.gif")
                SEP()
                CI("Vyčistit schválování (bude rozpracováno)", "javascript:ContextMenu_BatchClear(" & cRec.PID.ToString & ")", , "Images/clear.png")
                If cRec.p33ID = BO.p33IdENUM.Cas Then
                    SEP()
                    CI("Rozdělit úkon na 2 kusy", "javascript:ContextMenu_Split(" & cRec.PID.ToString & ")", , "Images/split.png")
                End If

                Return
            Case "p91_framework_detail"    'položky faktury
        End Select


        Dim cDisp As BO.p31WorksheetDisposition = factory.p31WorksheetBL.InhaleRecordDisposition(intPID)
        If cDisp.RecordDisposition = BO.p31RecordDisposition._NoAccess Then CI("K úkonu nemáte přístup.", "", True) : Return

        Select Case cDisp.RecordState
            Case BO.p31RecordState.Editing
                If cDisp.RecordDisposition = BO.p31RecordDisposition.CanEdit Or cDisp.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Then
                    CI("Upravit úkon", "p31_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
                    CI("Kopírovat", "p31_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
                    If cRec.p33ID = BO.p33IdENUM.Cas Then
                        CI("Rozdělit úkon na 2 kusy", "p31_record_split.aspx?pid=" & intPID.ToString, , "Images/split.png")
                    End If
                   
                End If
                ''Dim bolCanApproveAll As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver)
                If Not cRec.IsClosed And (cDisp.RecordDisposition = BO.p31RecordDisposition.CanApprove Or cDisp.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit) Then
                    SEP()
                    CI("[SCHVÁLIT]", "", , "Images/approve.png")
                    CI("Schvalovací dialog", "p31_approving_step2.aspx?pids=" & intPID.ToString, , "Images/approve.png", True, True)

                    CI("Fakturovat", "javascript:ContextMenu_Approve(4," & cRec.PID.ToString & ")", , "Images/a14.gif", True)
                    CI("Zahrnout do paušálu", "javascript:ContextMenu_Approve(6," & cRec.PID.ToString & ")", , "Images/a16.gif", True)
                    CI("Viditelný odpis", "javascript:ContextMenu_Approve(2," & cRec.PID.ToString & ")", , "Images/a12.gif", True)
                    CI("Skrytý odpis", "javascript:ContextMenu_Approve(3," & cRec.PID.ToString & ")", , "Images/a13.gif", True)
                    CI("Fakturovat později", "javascript:ContextMenu_Approve(7," & cRec.PID.ToString & ")", , "Images/a17.gif", True)

                End If
            Case BO.p31RecordState.Approved
                CI("Detail úkonu", "p31_record.aspx?pid=" & intPID.ToString, , "Images/zoom.png")

                If cDisp.RecordDisposition = BO.p31RecordDisposition.CanEdit Or cDisp.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Or cRec.j02ID = factory.SysUser.j02ID Then
                    CI("Kopírovat", "p31_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
                End If
                If cDisp.RecordDisposition = BO.p31RecordDisposition.CanApprove Or cDisp.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Then
                    SEP()
                    CI("[PŘE-SCHVÁLIT]", "", , "Images/approve.png")
                    CI("Schvalovací dialog", "p31_approving_step2.aspx?pids=" & intPID.ToString, , "Images/approve.png", True, True)
                    CI("Fakturovat", "javascript:ContextMenu_Approve(4," & cRec.PID.ToString & ")", , "Images/a14.gif", True)
                    CI("Zahrnout do paušálu", "javascript:ContextMenu_Approve(6," & cRec.PID.ToString & ")", , "Images/a16.gif", True)
                    CI("Viditelný odpis", "javascript:ContextMenu_Approve(2," & cRec.PID.ToString & ")", , "Images/a12.gif", True)
                    CI("Skrytý odpis", "javascript:ContextMenu_Approve(3," & cRec.PID.ToString & ")", , "Images/a13.gif", True)
                    CI("Fakturovat později", "javascript:ContextMenu_Approve(7," & cRec.PID.ToString & ")", , "Images/a17.gif", True)
                    CI("Vyčistit schvalování", "javascript:ContextMenu_Approve(0," & cRec.PID.ToString & ")", , "Images/clear.png", True)

                    If factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator) Then
                        SEP()
                        CI("[FAKTUROVAT schválené]", "", , "Images/billing.png")
                        CI("Fakturovat (nová faktura)", "p91_create_step1.aspx?nogateway=1&prefix=p31&pid=" & intPID.ToString, , , True, True)
                        CI("Přidat do existující faktury", "p91_add_worksheet.aspx?p31ids=" & intPID.ToString, , , True, True)
                    End If

                End If
            Case BO.p31RecordState.Invoiced
                If strMasterPrefix = "p91" Then
                    CI("Upravit úkon", "p31_record_AI.aspx?pid=" & intPID.ToString, , "Images/zoom.png")
                Else
                    CI("Detail úkonu", "p31_record.aspx?pid=" & intPID.ToString, , "Images/zoom.png")
                End If

                If cDisp.RecordDisposition = BO.p31RecordDisposition.CanEdit Or cRec.j02ID = factory.SysUser.j02ID Then
                    SEP()
                    CI("Kopírovat", "p31_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
                End If

        End Select

        SEP()
        CI("[ODKAZ]", "", , "Images/link.png")
        If cRec.j02ID_ContactPerson > 0 Then
            Dim c As BO.j02Person = factory.j02PersonBL.Load(cRec.j02ID_ContactPerson)
            If factory.SysUser.j04IsMenu_People Then
                REL(c.FullNameDescWithJobTitle, "j02_framework.aspx?pid=" & cRec.j02ID_ContactPerson.ToString, "_top", "Images/contactperson.png", True)
            Else
                CI(c.FullNameDescWithJobTitle, "", True, "Images/contactperson.png", True)
            End If
        End If
        If factory.SysUser.j04IsMenu_Notepad Then
            Dim mqO23 As New BO.myQueryO23(0)
            mqO23.Record_x29ID = BO.x29IdEnum.p31Worksheet
            mqO23.RecordPID = cRec.PID
            mqO23.MyRecordsDisponible = True
            Dim lisO23 As IEnumerable(Of BO.o23Doc) = factory.o23DocBL.GetList(mqO23)
            For Each c In lisO23
                REL(c.NameWithComboName, "o23_fixwork.aspx?x18id=" & c.x18ID.ToString & "&pid=" & c.PID.ToString, "_top", "Images/notepad.png", True)
            Next
        End If

        If cRec.p56ID > 0 Then
            If factory.SysUser.j04IsMenu_Task Then REL(cRec.p56Name, "p56_framework.aspx?pid=" & cRec.p56ID.ToString, "_top", "Images/task.png", True) 'pod odkaz
        End If
        If factory.SysUser.j04IsMenu_Project Then
            Dim ss As String = cRec.p41NameShort
            If ss = "" Then ss = cRec.p41Name
            If cRec.p28ID_Client > 0 Then ss = cRec.ClientName & " - " & ss
            REL(ss, "p41_framework.aspx?pid=" & cRec.p41ID.ToString, "_top", "Images/project.png", True) 'pod odkaz
        End If
        If factory.SysUser.j04IsMenu_Contact And cRec.p28ID_Client > 0 Then
            REL(cRec.ClientName, "p28_framework.aspx?pid=" & cRec.p28ID_Client.ToString, "_top", "Images/contact.png", True) 'pod odkaz
        End If

        If factory.SysUser.j04IsMenu_Invoice And cRec.p91ID > 0 Then
            Dim cP91 As BO.p91Invoice = factory.p91InvoiceBL.Load(cRec.p91ID)
            REL(cP91.p92Name & ": " & cP91.p91Code, "p91_framework.aspx?pid=" & cRec.p91ID.ToString, "_top", "Images/invoice.png", True)  'odkaz
        End If
        If factory.SysUser.j04IsMenu_People Then
            REL(cRec.Person, "j02_framework.aspx?pid=" & cRec.j02ID.ToString, "_top", "Images/person.png", True) 'pod odkaz
        End If
        SEP()
        CI("Oštítkovat", "tag_binding.aspx?prefix=p31&pids=" & intPID.ToString, , "Images/tag.png")
    End Sub
    Private Sub HandleP28(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.p28Contact = factory.p28ContactBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen", "", True) : Return
        Dim cDisp As BO.p28RecordDisposition = factory.p28ContactBL.InhaleRecordDisposition(cRec)
        If Not cDisp.ReadAccess Then CI("Ke klientovi nemáte oprávnění.", "", True) : Return

        If factory.SysUser.j04IsMenu_Contact Then
            If strFlag <> "pagemenu" Then
                REL(cRec.p28Name, "p28_framework.aspx?pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
            End If
        Else
            CI(cRec.p28Name, "", True, "Images/information.png")
        End If

        If cDisp.OwnerAccess Then
            SEP()
            CI("Upravit kartu klienta", "p28_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")

        End If
        SEP()
        CI("[NOVÝ]", "", , "Images/new4menu.png")
        If cDisp.OwnerAccess Then CI("Kopírovat klienta", "p28_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png", True) 'pod nový
        If factory.TestPermission(BO.x53PermValEnum.GR_P28_Creator, BO.x53PermValEnum.GR_P28_Draft_Creator) Then
            CI("Založit klienta", "javascript:record_new();", , "Images/contact.png", True)  'pod nový
        End If
        If cRec.p28SupplierFlag = BO.p28SupplierFlagENUM.ClientAndSupplier Or cRec.p28SupplierFlag = BO.p28SupplierFlagENUM.ClientOnly Then
            If factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator, BO.x53PermValEnum.GR_P41_Draft_Creator) Then
                CI("Založit pro klienta nový projekt", "p41_create.aspx?p28id=" & cRec.PID.ToString, , "Images/project.png", True)   'pod nový
            End If
        End If
        If factory.SysUser.j04IsMenu_Notepad Then
            CI("Vytvořit dokument", "o23_record.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, , "Images/notepad.png", True)    'pod nový
        End If
        If Not cRec.IsClosed Then CI("Vytvořit kalendářovou událost", "o22_record.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, , "Images/calendar.png", True) 'pod nový
        If cRec.b02ID = 0 Then CI("Doplnit přílohu, komentář, poznámku", "b07_create.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, , "Images/comment.png", True) 'pod nový

        

        Dim bolCanInvoice As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator), bolInvoicing As Boolean = False

        If (factory.SysUser.IsApprovingPerson Or bolCanInvoice) And cRec.p28SupplierFlag <> BO.p28SupplierFlagENUM.NotClientNotSupplier Then
            Dim mq As New BO.myQueryP31
            mq.p28ID_Client = cRec.PID
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            Dim cSum As BO.p31WorksheetSum = factory.p31WorksheetBL.LoadSumRow(mq, True, True)

            Dim intWIPx As Integer = 0, intAPPx As Integer = 0
            With cSum
                intWIPx = .WaitingOnApproval_Hours_Count + .WaitingOnApproval_Other_Count
                intAPPx = .WaitingOnInvoice_Hours_Count + .WaitingOnInvoice_Other_Count
            End With
            If intWIPx > 0 Or intAPPx > 0 Then
                SEP()
                Dim ss As String = String.Format("Schválit rozpracované úkony ({0}x)", intWIPx)
                If intWIPx = 0 And intAPPx > 0 Then ss = String.Format("Přes-schválit ({0}x) schválené", intAPPx)
                If intWIPx > 0 And intAPPx > 0 Then ss = String.Format("Schválit ({0}x)/přes-schválit ({1}x)", intWIPx, intAPPx)


                CI(ss, "entity_modal_approving.aspx?prefix=p28&pid=" & intPID.ToString, , "Images/approve.png", False, True)

                If bolCanInvoice Then CI("[FAKTUROVAT]", "", , "Images/billing.png") : bolInvoicing = True
                If bolCanInvoice And intAPPx > 0 Then
                    CI(String.Format("Fakturovat schválené úkony ({0}x)", intAPPx.ToString), "p91_create_step1.aspx?nogateway=1&prefix=p28&pid=" & intPID.ToString, , "Images/billing.png", True, True)
                End If
                If bolCanInvoice And intAPPx = 0 And intWIPx > 0 Then
                    CI(String.Format("Fakturovat bez schvalování ({0}x+{1}x)", intWIPx, intAPPx), "entity_modal_invoicing.aspx?prefix=p28&pids=" & intPID.ToString, , "Images/billing_blue.png", True, True)
                End If
                If factory.SysUser.j04IsMenu_Invoice Then
                    If cSum.Last_p91ID > 0 Then
                        REL(String.Format("Poslední faktura: {0}", factory.p91InvoiceBL.Load(cSum.Last_p91ID).p91Code), "p91_framework.aspx?pid=" & cSum.Last_p91ID.ToString, "_top", "Images/link.png", True)
                    Else
                        CI("Klient zatím nefakturován", "", True, , True)
                    End If

                End If
            End If
        End If
        If bolCanInvoice Then
            If Not bolInvoicing Then CI("[FAKTUROVAT]", "", , "Images/invoice.png")
            CI("Fakturovat jednou částkou bez úkonů", "p91_create_step1.aspx?quick=1&prefix=p28&pid=" & intPID.ToString, , "Images/billing_green.png", True, True)
        End If

        
        If cRec.b02ID > 0 Then
            SEP()
            CI("Posunout/doplnit", "workflow_dialog.aspx?prefix=p28&pid=" & cRec.PID.ToString, , "Images/workflow.png")
        End If

        Dim lisJ02 As IEnumerable(Of BO.j02Person) = factory.p30Contact_PersonBL.GetList_J02(cRec.PID, 0, True)
        If cRec.p28ParentID > 0 Or lisJ02.Count > 0 Then
            CI("[ODKAZ]", "", , "Images/link.png")
            If cRec.p28ParentID > 0 Then
                Dim cParent As BO.p28Contact = factory.p28ContactBL.Load(cRec.p28ParentID)
                REL(cParent.p28Name, "p28_framework.aspx?pid=" & cRec.p28ParentID.ToString, "_top", "Images/tree.png", True)
            End If
            Dim intTOP As Integer = lisJ02.Count : If intTOP > 10 Then intTOP = 10
            For Each c In lisJ02.Take(intTOP)
                REL(c.FullNameDescWithJobTitle, "j02_framework.aspx?pid=" & c.PID.ToString, "_top", "Images/contactperson.png", True)
            Next
        End If

        If factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot) Then
            SEP()
            REL("Statistiky klienta", "p31_sumgrid.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, "_top", "Images/pivot.png")
        End If
        SEP()
        CI("Tisková sestava", "report_modal.aspx?prefix=p28&pid=" & intPID.ToString, , "Images/report.png")
        CI("Odeslat e-mail", "sendmail.aspx?prefix=p28&pid=" & cRec.PID.ToString, , "Images/email.png")

     

        SEP()
        CI("[DALŠÍ]", "", , "Images/more.png")
        CI("Oštítkovat", "tag_binding.aspx?prefix=p28&pids=" & intPID.ToString, , "Images/tag.png", True)

        REL("Kalendář klienta", "entity_scheduler.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, "_top", "Images/calendar.png", True)




        REL("Historie odeslané pošty", "x40_framework.aspx?masterprefix=p28&masterpid=" & cRec.PID.ToString, "_top", "Images/email.png", True)
        If cDisp.OwnerAccess Then
            CI("Historie záznamu", "entity_timeline.aspx?prefix=p28&pid=" & cRec.PID.ToString, , "Images/event.png", True)
        End If

        CI("Plugin", "plugin_modal.aspx?prefix=p28&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)
        CI("Čárový kód", "barcode.aspx?prefix=p28&pid=" & cRec.PID.ToString, , "Images/barcode.png", True)


    End Sub
    Private Sub HandleP41(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.p41Project = factory.p41ProjectBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return
        If factory.SysUser.j04IsMenu_Project Then
            If strFlag <> "pagemenu" Then
                REL(cRec.PrefferedName, "p41_framework.aspx?pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
            End If
            
        Else
            CI(cRec.PrefferedName, "", True, "Images/information.png")
        End If

        Dim cDisp As BO.p41RecordDisposition = factory.p41ProjectBL.InhaleRecordDisposition(cRec)
        Dim cP42 As BO.p42ProjectType = factory.p42ProjectTypeBL.Load(cRec.p42ID)

        If cDisp.OwnerAccess Then
            SEP()
            CI("Upravit kartu projektu", "p41_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")

        End If
        SEP()
        CI("[NOVÝ]", "", , "Images/new4menu.png")
        If cDisp.OwnerAccess Then CI("Kopírovat projekt", "p41_create.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png", True)
        If factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator, BO.x53PermValEnum.GR_P41_Draft_Creator) Then
            CI("Založit projekt", "p41_create.aspx?client_family=1&pid=" & intPID.ToString, , "Images/project.png", True) 'pod NOVÝ
            CI("Založit pod-projekt", "p41_create.aspx?client_family=1&pid=" & cRec.PID & "&create_parent=1", , "Images/tree.png", True)    'pod nový
        End If
        If cP42.p42IsModule_o23 Then
            CI("Vytvořit dokument", "o23_record.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, , "Images/notepad.png", True)    'pod nový
        End If
        If Not cRec.IsClosed Then
            If cP42.p42IsModule_p56 Then CI("Vytvořit úkol", "p56_record.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, , "Images/task.png", True) 'pod nový
            If cP42.p42IsModule_o22 Then CI("Vytvořit kalendářovou událost", "o22_record.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, , "Images/calendar.png", True) 'pod nový
        End If
        If cDisp.OwnerAccess Then
            CI("Vytvořit předpis opakované odměny/úkonu", "p40_record.aspx?p41id=" & cRec.PID.ToString, , "Images/worksheet_recurrence.png", True)
        End If
        
        If cRec.b01ID = 0 Then CI("Doplnit přílohu, poznámku, komentář", "b07_create.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, , "Images/comment.png", True) 'pod nový


        If Not cDisp.ReadAccess Then CI("Nemáte přístup k tomuto projektu.", "", True) : Return
        If cP42.p42IsModule_p31 Then
            If Not (cRec.IsClosed Or cRec.p41IsDraft) Then
                SEP()
                CI("[ZAPSAT WORKSHEET]", "p31_record.aspx?pid=0&p41id=" & cRec.PID.ToString, , "Images/worksheet.png")

                Dim lisP34 As IEnumerable(Of BO.p34ActivityGroup) = factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(cRec.PID, cRec.p42ID, cRec.j18ID, factory.SysUser.j02ID)
                For Each c In lisP34
                    CI(c.p34Name, "p31_record.aspx?pid=0&p41id=" & cRec.PID.ToString & "&p34id=" + c.PID.ToString, , "Images/worksheet.png", True)
                Next
            End If

            Dim mq As New BO.myQueryP31
            mq.p41ID = cRec.PID
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            Dim cSum As BO.p31WorksheetSum = factory.p31WorksheetBL.LoadSumRow(mq, True, True)
            Dim intWIPx As Integer = 0, intAPPx As Integer = 0
            With cSum
                intWIPx = .WaitingOnApproval_Hours_Count + .WaitingOnApproval_Other_Count
                intAPPx = .WaitingOnInvoice_Hours_Count + .WaitingOnInvoice_Other_Count
            End With

            Dim bolCanInvoice As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator), bolInvoicing As Boolean = False
            If intWIPx > 0 Or intAPPx > 0 Then
                Dim bolCanApprove As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver)
                If Not bolCanApprove And cDisp.x67IDs.Count > 0 Then
                    Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = factory.x67EntityRoleBL.GetList_o28(cDisp.x67IDs)
                    If lisO28.Where(Function(p) p.o28PermFlag = BO.o28PermFlagENUM.CistASchvalovatVProjektu Or p.o28PermFlag = BO.o28PermFlagENUM.CistAEditASchvalovatVProjektu).Count > 0 Then
                        bolCanApprove = True
                    End If
                End If
                If bolCanApprove Or bolCanInvoice Then SEP()
                If bolCanApprove And (intWIPx > 0 Or intAPPx > 0) Then
                    Dim ss As String = String.Format("Schválit rozpracované úkony ({0}x)", intWIPx)
                    If intWIPx = 0 And intAPPx > 0 Then ss = String.Format("Přes-schválit ({0}x) schválené", intAPPx)
                    If intWIPx > 0 And intAPPx > 0 Then ss = String.Format("Schválit ({0}x)/přes-schválit ({1}x)", intWIPx, intAPPx)
                    CI(ss, "entity_modal_approving.aspx?prefix=p41&pid=" & intPID.ToString, , "Images/approve.png", False, True)
                End If
                If bolCanInvoice Then CI("[FAKTUROVAT", "", , "Images/billing.png") : bolInvoicing = True
                If bolCanInvoice And intAPPx > 0 Then
                    CI(String.Format("Fakturovat schválené úkony ({0}x)", intAPPx.ToString), "p91_create_step1.aspx?nogateway=1&prefix=p41&pid=" & intPID.ToString, , "Images/billing.png", True) : bolInvoicing = True
                End If
                If bolCanInvoice And intAPPx = 0 And intWIPx > 0 Then
                    CI(String.Format("Fakturovat bez schvalování ({0}x+{1}x)", intWIPx, intAPPx), "entity_modal_invoicing.aspx?prefix=p41&pids=" & intPID.ToString, , "Images/billing_blue.png", True) : bolInvoicing = True
                End If

                If factory.SysUser.j04IsMenu_Invoice Then
                    If cSum.Last_p91ID > 0 Then
                        REL(String.Format("Poslední faktura: {0}", factory.p91InvoiceBL.Load(cSum.Last_p91ID).p91Code), "p91_framework.aspx?pid=" & cSum.Last_p91ID.ToString, "_top", "Images/link.png", True)
                    Else
                        CI("Projekt zatím nefakturován", "", True, , True)
                    End If

                End If
            End If
            If bolCanInvoice And Not cRec.p41IsDraft Then
                If Not bolInvoicing Then CI("Fakturovat", "", , "Images/billing.png")
                CI("Fakturovat jednou částkou bez úkonů", "p91_create_step1.aspx?quick=1&prefix=p41&pid=" & intPID.ToString, , "Images/billing_green.png", True)
            End If


        End If
        

        
        If cRec.b01ID > 0 Then
            SEP()
            CI("Posunout/doplnit", "workflow_dialog.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/workflow.png")
        End If


        If cP42.p42IsModule_p31 Then
            If cDisp.P31_RecalcRates Or cDisp.P31_Move2Bin Or cDisp.P31_MoveToOtherProject Then
                SEP()
                CI("[AKCE]", "", , "Images/wizard.png")
                If cDisp.P31_RecalcRates Then CI("Přepočítat sazby rozpr. hodin", "p31_recalc.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/recalc.png", True)
                If cDisp.P31_Move2Bin Then CI("Přesunout do/z archivu nevyfakturované úkony", "p31_move2bin.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/bin.png", True)
                If cDisp.P31_MoveToOtherProject Then CI("Přesunout rozpr. na jiný projekt", "p31_move2project.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/cut.png", True)
            End If
        End If

        SEP()
        CI("[ODKAZ]", "", , "Images/link.png")
        If cRec.p41ParentID > 0 And factory.SysUser.j04IsMenu_Project Then        
            Dim cParent As BO.p41Project = factory.p41ProjectBL.Load(cRec.p41ParentID)
            REL(cParent.PrefferedName, "p41_framework.aspx?pid=" & cRec.p41ParentID.ToString, "_top", "Images/tree.png", True)   'pod odkaz
        End If
        If factory.SysUser.j04IsMenu_Contact And cRec.p28ID_Client > 0 Then
            REL(cRec.Client, "p28_framework.aspx?pid=" & cRec.p28ID_Client.ToString, "_top", "Images/contact.png", True) 'pod odkaz
        End If
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = factory.p30Contact_PersonBL.GetList_J02(0, cRec.PID, True)
        Dim intTOP As Integer = lisJ02.Count : If intTOP > 10 Then intTOP = 10
        For Each c In lisJ02.Take(intTOP)
            REL(c.FullNameDescWithJobTitle, "j02_framework.aspx?pid=" & c.PID.ToString, "_top", "Images/contactperson.png", True)
        Next
        
        If factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot) Then
            SEP()
            REL("Statistiky projektu", "p31_sumgrid.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, "_top", "Images/pivot.png")
        End If        
        SEP()
        CI("Tisková sestava", "report_modal.aspx?prefix=p41&pid=" & intPID.ToString, , "Images/report.png")
        CI("Odeslat e-mail", "sendmail.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/email.png")
        
        

        'If strFlag = "pagemenu" Then
        SEP()
        CI("[DALŠÍ]", "", , "Images/more.png")
        CI("Oštítkovat", "tag_binding.aspx?prefix=p41&pids=" & intPID.ToString, , "Images/tag.png", True)
        If cDisp.OwnerAccess Then
            CI("Kontaktní osoby projektu", "p30_binding.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, , "Images/person.png", True)
        End If

        If factory.p41ProjectBL.IsMyFavouriteProject(cRec.PID) Then
            CI("Vyřadit z mých oblíbených projektů", "javascript:Handle_Project_Favourite(" & cRec.PID.ToString & ")", , "Images/favourite_clear.png", True)
        Else
            CI("Zařadit mezi mé oblíbené projekty", "javascript:Handle_Project_Favourite(" & cRec.PID.ToString & ")", , "Images/favourite_add.png", True)
        End If

        If cDisp.OwnerAccess Then
            CI("Nastavit jako opakovaný projekt", "p41_recurrence.aspx?pid=" & cRec.PID.ToString, , "Images/recurrence.png", True)
        End If
        

        If cP42.p42IsModule_o22 Then
            REL("Kalendář projektu", "entity_scheduler.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, "_top", "Images/calendar.png", True)
        End If
        If cP42.p42IsModule_p48 Then
            REL("Operativní plán projektu", "p48_framework.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, "_top", "Images/oplan.png", True)
        End If
        REL("Historie odeslané pošty", "x40_framework.aspx?masterprefix=p41&masterpid=" & cRec.PID.ToString, "_top", "Images/email.png", True)
        If cDisp.OwnerAccess Then
            CI("Historie záznamu", "entity_timeline.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/event.png", True)
        End If
        CI("Plugin", "plugin_modal.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)
        CI("Čárový kód", "barcode.aspx?prefix=p41&pid=" & cRec.PID.ToString, , "Images/barcode.png", True)


        'End If
    End Sub
    Private Sub HandleO23(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.o23Doc = factory.o23DocBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return
        Dim cDisp As BO.o23RecordDisposition = factory.o23DocBL.InhaleDisposition(cRec)
        If Not cDisp.ReadAccess Then CI("Nemáte přístup k tomuto dokumentu.", "", True) : Return
        Dim cX18 As BO.x18EntityCategory = factory.x18EntityCategoryBL.Load(cRec.x18ID)
        If strFlag <> "pagemenu" Then
            If factory.SysUser.j04IsMenu_Notepad Then
                Select Case strFlag
                    Case "o23_fixwork"
                        REL("Přejít do obecného přehledu", "entity_framework.aspx?prefix=o23&pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
                    Case ""
                        REL("Přejít do pevného přehledu", "o23_fixwork.aspx?pid=" & intPID.ToString & "&x18id=" & cRec.x18ID.ToString, "_top", "Images/fullscreen.png")
                    Case Else
                        REL("Přejít do pevného přehledu", "o23_fixwork.aspx?pid=" & intPID.ToString & "&x18id=" & cRec.x18ID.ToString, "_top", "Images/fullscreen.png")
                        REL("Přejít do obecného přehledu", "entity_framework.aspx?prefix=o23&pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
                End Select


            Else
                CI(cRec.o23Name, "", True, "Images/information.png")
            End If
        End If
        

        If cDisp.OwnerAccess Then
            SEP()
            If cX18.x18IsManyItems Then
                CI("Upravit kartu dokumentu", "o23_record.aspx?pid=" & intPID.ToString & "&x18id=" & cRec.x18ID.ToString, , "Images/edit.png")
            Else
                CI("Upravit kartu kategorie", "o23_record.aspx?pid=" & intPID.ToString & "&x18id=" & cRec.x18ID.ToString, , "Images/edit.png")
            End If

            CI("Kopírovat", "o23_record.aspx?clone=1&pid=" & intPID.ToString & "&x18id=" & cRec.x18ID.ToString, , "Images/copy.png")
        End If
        If cX18.x18UploadFlag = BO.x18UploadENUM.FileSystemUpload And cRec.o23LockedFlag <> BO.o23LockedTypeENUM.LockAllFiles Then
            Dim mq As New BO.myQueryO27
            mq.Record_x29ID = BO.x29IdEnum.o23Doc
            mq.Record_PID = cRec.PID
            Dim lis As IEnumerable(Of BO.o27Attachment) = factory.o27AttachmentBL.GetList(mq)
            If lis.Count > 0 Then
                SEP()
                CI(String.Format("Souborové přílohy ({0}x)", lis.Count), "fileupload_preview.aspx?prefix=o23&pid=" & cRec.PID.ToString, , "Images/attachment.png")
            End If
        End If
        If cX18.b01ID <> 0 Then
            CI("Posunout/Doplnit", "workflow_dialog.aspx?prefix=o23&pid=" & intPID.ToString, , "Images/workflow.png")
        Else
            CI("Nahrát přílohu, doplnit poznámku/komentář", "b07_create.aspx?masterprefix=o23&masterpid=" & cRec.PID.ToString, , "Images/comment.png")
        End If

        SEP()
        CI("Tisková sestava", "report_modal.aspx?prefix=o23&pid=" & intPID.ToString, , "Images/report.png")
        CI("Odeslat e-mail", "sendmail.aspx?prefix=o23&pid=" & cRec.PID.ToString, , "Images/email.png")

        SEP()
        CI("Oštítkovat", "tag_binding.aspx?prefix=o23&pids=" & intPID.ToString, , "Images/tag.png")
        If strFlag = "pagemenu" Then
            SEP()
            CI("Plugin", "plugin_modal.aspx?prefix=o23&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)
            CI("Čárový kód", "barcode.aspx?prefix=o23&pid=" & cRec.PID.ToString, , "Images/barcode.png", True)
        End If

        
    End Sub
    Private Sub HandleX40(intPID As Integer, factory As BL.Factory)
        Dim cRec As BO.x40MailQueue = factory.x40MailQueueBL.Load(intPID)
        CI(cRec.StatusAlias, "", True, "Images/information.png")
        SEP()
        CI("Detail", "x40_record.aspx?pid=" & intPID.ToString, , "Images/zoom.png")

        If cRec.x40MessageID <> "" And cRec.x40ArchiveFolder <> "" Then
            SEP()
            REL("EML formát zprávy", "binaryfile.aspx?prefix=x40-eml&pid=" & cRec.PID.ToString, "", "Images/email.png")
            REL("Otevřít v MS-OUTLOOK", "binaryfile.aspx?prefix=x40-msg&pid=" & cRec.PID.ToString, "", "Images/outlook.png")
        End If

    End Sub
    Private Sub HandleP51(intPID As Integer, factory As BL.Factory)
        Dim cRec As BO.p51PriceList = factory.p51PriceListBL.Load(intPID)
        If Not factory.TestPermission(BO.x53PermValEnum.GR_P51_Admin) Then
            CI("Chybí oprávnění pro správu ceníků.", "", True) : Return
        Else
            CI(cRec.p51Name, "", True, "Images/information.png")
        End If
        SEP()
        CI("Upravit ceník", "p51_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
        CI("Kopírovat", "p51_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
        If cRec.p51IsCustomTailor Then
            SEP()
            Dim mqP28 As New BO.myQueryP28, b As Boolean = False
            mqP28.p51ID = intPID
            Dim lisP28 As IEnumerable(Of BO.p28Contact) = factory.p28ContactBL.GetList(mqP28)
            For Each c In lisP28
                REL(c.p28Name, "p28_framework.aspx?pid=" & c.PID.ToString, "_top", "Images/contact.png")
            Next
            Dim mqP41 As New BO.myQueryP41
            mqP41.p51ID = intPID
            mqP41.TopRecordsOnly = 10
            Dim lisP41 As IEnumerable(Of BO.p41Project) = factory.p41ProjectBL.GetList(mqP41)
            For Each c In lisP41
                REL(c.FullName, "p41_framework.aspx?pid=" & c.PID.ToString, "_top", "Images/project.png")
            Next

        End If
        If cRec.p51ID_Master > 0 Then
            SEP()
            CI(String.Join("MASTER ceník: {0}", cRec.p51Name_Master), "p51_record.aspx?pid=" & cRec.p51ID_Master.ToString, , "Images/edit.png")
        End If
        If cRec.p51IsInternalPriceList And factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
            SEP()
            REL("Nastavení interních ceníků", "admin_framework.aspx?prefix=p50", "_top", "Images/setting.png")
        End If
    End Sub

    Private Sub HandleJ02(intPID As Integer, factory As BL.Factory, strFlag As String)
        Dim cRec As BO.j02Person = factory.j02PersonBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return
        If strFlag <> "pagemenu" Then
            If factory.SysUser.j04IsMenu_People Then
                REL(cRec.FullNameDesc, "j02_framework.aspx?pid=" & intPID.ToString, "_top", "Images/fullscreen.png")
            Else
                CI(cRec.FullNameDesc, "", True, "Images/information.png")
            End If
        End If

        If factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
            SEP()
            CI("Upravit kartu osoby", "j02_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")

           
        End If
        CI("[NOVÝ]", "", , "Images/new4menu.png")
        If factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
            CI("Kopírovat osobu", "j02_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png", True) 'pod nový
            CI("Založit osobu", "j02_record.aspx?pid=0", , "Images/person.png", True)
        End If
        If factory.SysUser.j04IsMenu_Notepad Then
            CI("Vytvořit dokument", "o23_record.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, , "Images/notepad.png", True)    'pod nový
        End If
        If Not cRec.IsClosed Then CI("Vytvořit kalendářovou událost", "o22_record.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, , "Images/calendar.png", True) 'pod nový
        CI("Doplnit přílohu, komentář, poznámku", "b07_create.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, , "Images/comment.png", True)  'pod nový

        If cRec.j02IsIntraPerson Then
            If factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver) Then
                SEP()
                If cRec.j02IsIntraPerson Then CI("Schvalovat práci osoby", "entity_modal_approving.aspx?prefix=j02&pid=" & cRec.PID.ToString, , "Images/approve.png", , True)
            End If
            If factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot) Then
                SEP()
                REL("Statistiky osoby", "p31_sumgrid.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, "_top", "Images/pivot.png")
            End If
            SEP()
            CI("Tisková sestava", "report_modal.aspx?prefix=j02&pid=" & intPID.ToString, , "Images/report.png")
        Else
            CI("[ODKAZ]", "", , "Images/link.png")
            Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = factory.p30Contact_PersonBL.GetList(0, 0, intPID)
            For Each c In lisP30
                If c.p41ID <> 0 Then
                    REL(c.Project, "p41_framework.aspx?pid=" & c.p41ID.ToString, "_top", "Images/project.png", True)
                End If
                If c.p28ID <> 0 Then
                    REL(c.p28Name, "p28_framework.aspx?pid=" & c.p28ID.ToString, "_top", "Images/contact.png", True)
                End If
            Next
          
        End If
        If cRec.j02Email <> "" Then
            SEP()
            CI("Odeslat e-mail", "sendmail.aspx?prefix=j02&pid=" & cRec.PID.ToString, , "Images/email.png")
        End If




        If cRec.j02IsIntraPerson Then
            CI("[DALŠÍ]", "", , "Images/more.png")
            If factory.SysUser.j04IsMenu_People Then
                CI("Oštítkovat", "tag_binding.aspx?prefix=j02&pids=" & intPID.ToString, , "Images/tag.png", True)
            End If

            If factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                CI("Přepočítat sazby rozpracovaných hodin", "p31_recalc.aspx?prefix=j02&pid=" & cRec.PID.ToString, , "Images/recalc.png", True)
            End If


            REL("Kalendář osoby", "entity_scheduler.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, "_top", "Images/calendar.png", True)


            If factory.TestPermission(BO.x53PermValEnum.GR_P48_Creator) Then
                REL("Operativní plán osoby", "cmdP48", "p48_framework.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, "Images/oplan.png", True)
            End If
            CI("Osobní plány", "j02_personalplan.aspx?j02id=" & cRec.PID.ToString, , "Images/plan.png", True, True)

            REL("Historie odeslané pošty", "x40_framework.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString, "_top", "Images/email.png", True)
            CI("Historie záznamu", "entity_timeline.aspx?prefix=j02&pid=" & cRec.PID.ToString, , "Images/event.png", True)

            CI("Plugin", "plugin_modal.aspx?prefix=j02&pid=" & cRec.PID.ToString, , "Images/plugin.png", True)

        End If


    End Sub
    Private Sub HandleP90(intPID As Integer, factory As BL.Factory)
        Dim cRec As BO.p90Proforma = factory.p90ProformaBL.Load(intPID)
        If cRec Is Nothing Then CI("Záznam nebyl nalezen.", "", True) : Return
        If Not factory.TestPermission(BO.x53PermValEnum.GR_P90_Reader) Then
            CI("Chybí oprávnění ke čtení záloh.", "", True) : Return
        End If
        CI(cRec.p89Name & ": " & cRec.p90Code, "", True, "Images/information.png")
        If factory.TestPermission(BO.x53PermValEnum.GR_P90_Owner) Or cRec.j02ID_Owner = factory.SysUser.j02ID Then
            SEP()
            CI("Upravit kartu zálohy", "p90_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
            CI("Kopírovat", "p90_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
        End If
        SEP()
        CI("Tisková sestava", "report_modal.aspx?prefix=p90&pid=" & intPID.ToString, , "Images/report.png")
        If factory.SysUser.j04IsMenu_Contact And cRec.p28ID > 0 Then
            SEP()
            REL(cRec.p28Name, "p28_framework.aspx?pid=" & cRec.p28ID.ToString, "_top", "Images/contact.png")
        End If
        Dim lis As IEnumerable(Of BO.p99Invoice_Proforma) = factory.p90ProformaBL.GetList_p99(0, cRec.PID, 0)
        If lis.Count > 0 Then SEP()
        For Each c In lis
            REL(String.Join("Spárováno: {0}", c.p91Code), "p91_framework.aspx?pid=" & c.p91ID.ToString, "_top", "Images/invoice.png")
        Next


        SEP()
        CI("Oštítkovat", "tag_binding.aspx?prefix=p90&pids=" & intPID.ToString, , "Images/tag.png")
    End Sub
    Private Sub HandleAdminMenu(strPrefix As String, intPID As Integer, factory As BL.Factory)
        Dim a() As String = Split(strPrefix, "-")
        If Not factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
            CI("Nedisponujete oprávněním administrátora.", "", True) : Return
        End If

        CI("Upravit záznam", Right(a(1), 3) & "_record.aspx?pid=" & intPID.ToString, , "Images/edit.png")
        CI("Kopírovat", Right(a(1), 3) & "_record.aspx?clone=1&pid=" & intPID.ToString, , "Images/copy.png")
    End Sub
    Private Sub RenderNewRecMenu(factory As BL.Factory)
        With factory.SysUser

            If .j04IsMenu_Worksheet Then
                CI("[ZAPSAT WORKSHEET]", "p31_record.aspx?pid=0", , "Images/worksheet.png")
                Dim lis As IEnumerable(Of BO.p34ActivityGroup) = factory.p34ActivityGroupBL.GetList_WorksheetEntry_InAllProjects(factory.SysUser.j02ID)
                If lis.Count > 0 And lis.Count <= 10 Then
                    For Each c In lis
                        CI(c.p34Name, "p31_record.aspx?pid=0&p34id=" + c.PID.ToString, , "Images/worksheet.png", True)
                    Next
                End If
                
            End If
            If .j04IsMenu_Contact Then
                If factory.TestPermission(BO.x53PermValEnum.GR_P28_Creator, BO.x53PermValEnum.GR_P28_Draft_Creator) Then
                    CI(Resources.common.Klient, "p28_record.aspx?pid=0&hrjs=hardrefresh_menu", , "Images/contact.png")
                End If
            End If
            If .j04IsMenu_Project Then
                If factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator, BO.x53PermValEnum.GR_P41_Draft_Creator) Then
                    CI(Resources.common.Projekt, "p41_create.aspx?hrjs=hardrefresh_menu", , "Images/project.png")
                End If
            End If

            If factory.SysUser.j04IsMenu_Notepad Then
                CI(Resources.common.Dokument, "select_doctype.aspx?hrjs=hardrefresh_menu", , "Images/notepad.png")
            End If
            If factory.SysUser.j04IsMenu_Task Then
                CI(Resources.common.Ukol, "p56_record.aspx?masterprefix=p41&masterpid=0&hrjs=hardrefresh_menu", , "Images/task.png")
            End If
            CI("Událost v kalendáři", "o22_record.aspx?hrjs=hardrefresh_menu", , "Images/event.png")

            ''If .j04IsMenu_Invoice Then
            ''    If factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator) Then
            ''        CI(Resources.common.Faktura, "p91_create_step1.aspx?prefix=p28&hrjs=hardrefresh_menu", , "Images/invoice.png")
            ''    End If
            ''End If
            ''If factory.TestPermission(BO.x53PermValEnum.GR_P90_Create) Then
            ''    CI(Resources.common.ZalohovaFaktura, "p90_record.aspx?pid=0&hrjs=hardrefresh_menu", , "Images/proforma.png")
            ''End If

            If factory.SysUser.IsAdmin Or factory.SysUser.j04IsMenu_People Then
                If factory.SysUser.IsAdmin Then
                    CI(Resources.common.Osoba, "", , "Images/person.png")
                End If
                If factory.SysUser.IsAdmin Then
                    CI("Interní osoba (uživatel)", "j02_record.aspx?pid=0&hrjs=hardrefresh_menu", , "Images/person.png", True)
                End If
                CI("Kontaktní osoba klienta/projektu", "j02_record.aspx?pid=0&hrjs=hardrefresh_menu&iscontact=1", , "Images/contactperson.png", True)
            End If
            
        End With


    End Sub

    Private Sub FinalRW(context As HttpContext, strMessage As String)
        If strMessage <> "" Then
            CI(strMessage, "", True)
        End If
        Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer
        Dim s As String = jss.Serialize(_lis)
        context.Response.Write(s)
    End Sub
    Private Sub SEP()
        Dim c As New BO.ContextMenuItem
        c.IsSeparator = True
        _lis.Add(c)
    End Sub
    Private Sub CI(strText As String, strURL As String, Optional bolDisabled As Boolean = False, Optional strImageUrl As String = "", Optional bolChild As Boolean = False, Optional bolTopWindow As Boolean = False)
        Dim c As New BO.ContextMenuItem
        If Len(strText) > 35 Then strText = Left(strText, 35) & "..."
        c.Text = strText
        If strURL <> "" Then
            If strURL.IndexOf("javascript") >= 0 Then
                c.NavigateUrl = strURL
            Else
                If bolTopWindow Then
                    c.NavigateUrl = "javascript:contMenu(" & Chr(34) & strURL & Chr(34) & ",true)"
                Else
                    c.NavigateUrl = "javascript:contMenu(" & Chr(34) & strURL & Chr(34) & ",false)"
                End If
            End If
            
        End If
        c.IsDisabled = bolDisabled
        c.ImageUrl = strImageUrl
        c.IsChildOfPrevious = bolChild
        _lis.Add(c)

    End Sub
    Private Sub REL(strText As String, strURL As String, strTarget As String, Optional strImageUrl As String = "", Optional bolChild As Boolean = False)
        Dim c As New BO.ContextMenuItem
        If Len(strText) > 35 Then strText = Left(strText, 35) & "..."
        c.Text = strText
        c.NavigateUrl = "javascript:contReload(" & Chr(34) & strURL & Chr(34) & "," & Chr(34) & strTarget & Chr(34) & ")"
        c.ImageUrl = strImageUrl
        c.Target = strTarget
        c.IsChildOfPrevious = bolChild
        _lis.Add(c)
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class