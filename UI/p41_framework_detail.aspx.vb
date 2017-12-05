Public Class p41_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm

    Private Sub p41_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        menu1.DataPrefix = "p41"
        ff1.Factory = Master.Factory
        p31summary1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        folder1.Factory = Master.Factory
    End Sub

    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cal1.factory = Master.Factory
        'výchozí stránka z entity_framework přehledu (levý panel)
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "p41"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Me.menu1.PageSource = "2" Then
                    .IsHideAllRecZooms = True
                    ''If Request.Item("tab") <> "" Then
                    ''    .Factory.j03UserBL.SetUserParam("p41_framework_detail-tab", Request.Item("tab"))
                    ''End If
                End If


                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p41_framework_detail-pid")
                    .Add("p41_framework_detail-tab")
                    .Add("p41_menu-remember-tab")
                    .Add("p41_menu-tabskin")
                    ''.Add("p41_menu-menuskin")
                    .Add("p41_framework_detail-chkFFShowFilledOnly")
                    .Add("p41_framework_detail_pos")
                    .Add("p41_menu-x31id-plugin")
                    .Add("p41_menu-show-level1")
                    .Add("myscheduler-firstday")
                    .Add("myscheduler-tasksnoterm")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                    'jedná se o výchozí stránku projektu, která se zároveň stará o automatické přesměrování na další projektové stránky
                    Dim intPID As Integer = Master.DataPID
                    If intPID = 0 Then
                        intPID = BO.BAS.IsNullInt(.GetUserParam("p41_framework_detail-pid", "O"))
                        If intPID = 0 Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p41")
                    Else
                        If intPID <> BO.BAS.IsNullInt(.GetUserParam("p41_framework_detail-pid", "O")) Then
                            .SetUserParam("p41_framework_detail-pid", intPID.ToString)
                        End If
                    End If

                    Dim strTab As String = Request.Item("tab")

                    If strTab = "" And .GetUserParam("p41_menu-remember-tab", "0") = "1" Then
                        strTab = .GetUserParam("p41_framework_detail-tab")  'záložka je ukotvená
                    End If
                    
                    Select Case strTab
                        Case "p31", "time", "expense", "fee", "kusovnik"
                            Server.Transfer("entity_framework_rec_p31.aspx?masterprefix=p41&masterpid=" & intPID.ToString & "&p31tabautoquery=" & strTab & "&source=" & menu1.PageSource, False)
                        Case "o23", "p91", "p56", "p41"
                            Server.Transfer("entity_framework_rec_" & strTab & ".aspx?masterprefix=p41&masterpid=" & intPID.ToString & "&source=" & menu1.PageSource, False)
                        Case "budget"
                            Server.Transfer("p41_framework_rec_budget.aspx?pid=" & intPID.ToString & "&source=" & menu1.PageSource, False)
                        Case Else
                            'zůstat na BOARD stránce
                    End Select

                    Master.DataPID = intPID
                    cal1.FirstDayMinus = BO.BAS.IsNullInt(.GetUserParam("myscheduler-firstday", "-1"))
                    cal1.ShowTasksNoTerm = BO.BAS.BG(.GetUserParam("myscheduler-tasksnoterm", "1"))
                    ''menu1.MenuSkin = .GetUserParam("p41_menu-menuskin")
                    menu1.TabSkin = .GetUserParam("p41_menu-tabskin")
                    menu1.x31ID_Plugin = .GetUserParam("p41_menu-x31id-plugin")
                    If .GetUserParam("p41_menu-remember-tab", "0") = "1" Then
                        menu1.LockedTab = .GetUserParam("p41_framework_detail-tab")
                    End If

                    Me.chkFFShowFilledOnly.Checked = BO.BAS.BG(.GetUserParam("p41_framework_detail-chkFFShowFilledOnly", "0"))

                End With
            End With


        End If

        RefreshRecord()
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p41")

        Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(cRec.p42ID)
        Dim cRecSum As BO.p41ProjectSum = Master.Factory.p41ProjectBL.LoadSumRow(cRec.PID)
        Dim cDisp As BO.p41RecordDisposition = Master.Factory.p41ProjectBL.InhaleRecordDisposition(cRec)

        menu1.p41_RefreshRecord(cRec, cRecSum, "board", cDisp)
        Handle_Permissions(cRec, cP42, cDisp)

        Dim cClient As BO.p28Contact = Nothing

        With cRec
            Me.boxCoreTitle.Text = .p42Name & " (" & .p41Code & ")"
            If .b02ID <> 0 Then
                Me.boxCoreTitle.Text += ": " & .b02Name
            End If
            Me.Owner.Text = .Owner : Me.linkTimestamp.Text = .UserInsert & "/" & .DateInsert
            If cDisp.OwnerAccess Then
                Me.linkTimestamp.ToolTip = "CHANGE-LOG"
                Me.linkTimestamp.NavigateUrl = "javascript:changelog()"
            End If

            Me.Project.Text = .p41Name & " <span style='color:gray;padding-left:10px;'>" & .p41Code & "</span>"
            Select Case .p41TreeLevel
                Case 1 : Me.Project.ForeColor = basUIMT.TreeColorLevel1
                Case Is > 1 : Me.Project.ForeColor = basUIMT.TreeColorLevel2

            End Select

            If .p41NameShort <> "" Then
                Me.Project.Text += "<div style='color:green;'>" & .p41NameShort & "</div>"
            End If

            If .p28ID_Client > 0 Then
                Me.Client.Text = .Client : Me.Client.Visible = True
                If Master.Factory.SysUser.j04IsMenu_Contact Then
                    Me.Client.NavigateUrl = "p28_framework.aspx?pid=" & .p28ID_Client.ToString
                End If
                cClient = Master.Factory.p28ContactBL.Load(.p28ID_Client)
                Me.pmClient.Attributes.Item("onclick") = "RCM('p28'," & .p28ID_Client.ToString & ",this)"
                Me.clue_client.Attributes("rel") = "clue_p28_record.aspx?pid=" & .p28ID_Client.ToString
            Else
                Me.clue_client.Visible = False : Me.Client.Visible = False : pmClient.Visible = False
            End If
            If .p28ID_Billing > 0 Then
                lblClientBilling.Visible = True : Me.ClientBilling.Visible = True
                Dim cClientBilling As BO.p28Contact = Master.Factory.p28ContactBL.Load(.p28ID_Billing)
                Me.ClientBilling.Text = cClientBilling.p28Name
                Me.ClientBilling.NavigateUrl = "p28_framework.aspx?pid=" & .p28ID_Billing.ToString
            End If
            If .j18ID > 0 Then
                Me.clue_j18name.Attributes("rel") = "clue_j18_record.aspx?pid=" & .j18ID.ToString
            Else
                Me.clue_j18name.Visible = False
            End If
            If .p61ID > 0 Then
                Me.p61Name.Text = Master.Factory.p61ActivityClusterBL.Load(.p61ID).p61Name
                Me.clue_p61Name.Attributes("rel") = "clue_p61_record.aspx?pid=" & .p61ID.ToString : clue_p61Name.Visible = True
            Else
                lblP61Name.Visible = False : Me.p61Name.Visible = False : clue_p61Name.Visible = False
            End If
            Me.p42Name.Text = .p42Name
            Me.clue_p42name.Attributes("rel") = "clue_p42_record.aspx?pid=" & .p42ID.ToString
            lblJ18Name.Visible = False : Me.j18Name.Visible = False
            If .j18ID > 0 Then
                lblJ18Name.Visible = True : Me.j18Name.Visible = True
                Me.j18Name.Text = .j18Name
            End If
            If .b01ID > 0 Then
                Me.trWorkflow.Visible = True
                Me.b02Name.Text = .b02Name
            Else
                Me.trWorkflow.Visible = False
            End If

            If Not (.p41PlanFrom Is Nothing Or .p41PlanUntil Is Nothing) Then
                Me.PlanPeriod.Text = "<b style='color:green;'>" & BO.BAS.FD(.p41PlanFrom.Value) & "</b> - <b style='color:red;'>" & BO.BAS.FD(.p41PlanUntil.Value) & "</b>"
                If DateDiff(DateInterval.Day, .p41PlanFrom.Value, .p41PlanUntil.Value) < 750 Then
                    Me.PlanPeriod.Text += " [" & DateDiff(DateInterval.Day, .p41PlanFrom.Value, .p41PlanUntil.Value).ToString & "d.]"
                End If
                trPlan.Visible = True
            Else
                trPlan.Visible = False

            End If

            Me.imgDraft.Visible = .p41IsDraft
            If .p41IsDraft Then imgRecord.ImageUrl = "Images/draft.png"
            If .p65ID > 0 Then
                RenderRecurrence(cRec)
            Else
                panRecurrence.Controls.Clear()
            End If
            If cRec.p41ParentID <> 0 Then
                RenderTree(cRec, cRecSum)
            End If
            ''If .p41ParentID <> 0 Then
            ''    Me.trParent.Visible = True
            ''    Me.ParentProject.NavigateUrl = "p41_framework.aspx?pid=" & .p41ParentID.ToString
            ''    Me.ParentProject.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ParentID)
            ''End If
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) And .p41BillingMemo <> "" Then
                boxBillingMemo.Visible = True
                Me.p41BillingMemo.Text = BO.BAS.CrLfText2Html(.p41BillingMemo)
                If Not cClient Is Nothing Then
                    If cClient.p28BillingMemo <> "" Then
                        Me.p41BillingMemo.Text += "<hr>" & String.Format("Fakturační poznámka klienta: {0}", BO.BAS.CrLfText2Html(cClient.p28BillingMemo))
                    End If
                End If
            Else
                boxBillingMemo.Visible = False
            End If

        End With



        If cP42.p42IsModule_p31 Then
            RefreshBillingLanguage(cRec, cClient)
            RefreshPricelist(cRec, cClient)
            ''RefreshOtherBillingSetting(cRec, cClient)
        Else
            trP51.Visible = False
        End If




        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, cRec.PID)
        Me.roles_project.RefreshData(lisX69, cRec.PID)


        
        If cRecSum.p30_Exist Then
            Dim lisP30 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(cRec.p28ID_Client, Master.DataPID, False)
            If lisP30.Count > 0 Then
                Me.boxP30.Visible = True
                Me.persons1.FillData(lisP30, Master.Factory.SysUser.j04IsMenu_People)
                With Me.boxP30Title
                    .Text = BO.BAS.OM2(.Text, lisP30.Count.ToString)
                   
                End With
            Else
                cRecSum.p30_Exist = False
            End If
        End If
        Me.boxP30.Visible = cRecSum.p30_Exist

        

        If cP42.p42IsModule_p31 And boxP31Summary.Visible Then

            Me.linkLastInvoice.Text = cRecSum.Last_Invoice
            If cRecSum.Last_p91ID > 0 Then
                Me.linkLastInvoice.Text = cRecSum.Last_Invoice
                If Master.Factory.SysUser.j04IsMenu_Invoice Then
                    Me.linkLastInvoice.NavigateUrl = "p91_framework.aspx?pid=" & cRecSum.Last_p91ID.ToString
                End If
            End If
            Me.Last_WIP_Worksheet.Text = cRecSum.Last_Wip_Worksheet
            If cRec.p41LimitFee_Notification > 0 Or cRec.p41LimitHours_Notification > 0 Or cRecSum.p31_Wip_Time_Count > 0 Or cRecSum.p31_Wip_Expense_Count > 0 Or cRecSum.p31_Wip_Fee_Count > 0 Or cRecSum.p31_Approved_Time_Count > 0 Or cRecSum.p31_Approved_Expense_Count > 0 Then
                Dim mq As New BO.myQueryP31
                mq.p41ID = cRec.PID
                mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
                Dim cWorksheetSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, True)
                If cWorksheetSum.RowsCount = 0 Then
                    boxP31Summary.Visible = False
                Else
                    p31summary1.RefreshData(cWorksheetSum, "p41", Master.DataPID, Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates), cRec.p41LimitHours_Notification, cRec.p41LimitFee_Notification)
                End If
            Else
                p31summary1.Visible = False
                If cRecSum.Last_Invoice = "" And cRecSum.Last_Wip_Worksheet = "" Then Me.boxP31Summary.Visible = False
            End If
        End If




        Dim lisFF As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p41Project, Master.DataPID, cRec.p42ID)
        If lisFF.Count > 0 Then
            ff1.FillData(lisFF, Not Me.chkFFShowFilledOnly.Checked)
        Else
            boxFF.Visible = False
        End If
        If cP42.p42SubgridO23Flag = 0 And cP42.p42IsModule_o23 Then
            labels1.RefreshData(Master.Factory, BO.x29IdEnum.p41Project, cRec.PID)
            boxX18.Visible = labels1.ContainsAnyData
        Else
            boxX18.Controls.Clear()
            boxX18.Visible = False
        End If
        

        If cRecSum.is_My_Favourite Then
            imgFavourite.ImageUrl = "Images/favourite.png"
            imgFavourite.Visible = True
        End If

        RefreshP40(cRecSum)

        If cRecSum.b07_Count > 0 Or cRec.b01ID <> 0 Then
            comments1.Visible = True
            comments1.RefreshData(Master.Factory, BO.x29IdEnum.p41Project, cRec.PID)
        Else
            comments1.Visible = False
        End If

        If cRec.o25ID_Calendar > 0 Or cRecSum.o22_Actual_Count > 0 Or cRecSum.p56_Actual_Count > 0 Then
            cal1.o25ID = cRec.o25ID_Calendar
            cal1.Visible = True
            cal1.RecordPID = Master.DataPID
            cal1.RefreshData(Today)
        Else
            cal1.Visible = False
        End If

        If cRecSum.o52_Exist Then
            tags1.RefreshData(cRec.PID)
        Else
            tags1.RecordPID = cRec.PID
        End If
        If cRecSum.f01_Exist Then
            folder1.RefreshData(cRec.PID)
        End If

        'RefreshP64(cRecSum)

    End Sub

    Private Sub RenderRecurrence(cRec As BO.p41Project)
        panRecurrence.Visible = True
        imgRecord.ImageUrl = "Images/recurrence.png"
        With cRec
            Me.RecurrenceType.Text = Master.Factory.p65RecurrenceBL.Load(.p65ID).p65Name
            If .p41IsStopRecurrence Then Me.RecurrenceType.Text += " <img src='Images/exclaim.png'/>" Else Me.RecurrenceType.Text += " <img src='Images/flame.png'/>"

            Me.p41RecurNameMask.Text = .p41RecurNameMask
            Me.p41RecurBaseDate.Text = BO.BAS.FD(.p41RecurBaseDate)
            Dim mq As New BO.myQueryP41
            mq.p41RecurMotherID = .PID
            Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq).OrderByDescending(Function(p) p.PID)
            Dim cP65 As BO.p65Recurrence = Master.Factory.p65RecurrenceBL.Load(cRec.p65ID)
            Dim datNextBaseDate = Master.Factory.p65RecurrenceBL.CalculateNextBaseDate(cP65, cRec.p41RecurBaseDate)
            If lis.Count > 0 Then
                LastChild.Text = lis(0).PrefferedName
                LastChild.NavigateUrl = "p41_framework.aspx?pid=" & lis(0).PID.ToString
                datNextBaseDate = Master.Factory.p65RecurrenceBL.CalculateNextBaseDate(cP65, lis(0).p41RecurBaseDate)
            End If
            Dim c As BO.RecurrenceCalculation = Master.Factory.p65RecurrenceBL.CalculateDates(cP65, datNextBaseDate)

            lblNextGen.Text = BO.BAS.FD(c.DatGen, , True) & "<span style='color:#FF8C00;' title='Rozhodné datum'> " & Format(c.DatBase, "dd.MM.yyyy") & "</span>"
            If Year(c.DatPlanUntil) > 2000 Then
                lblNextGen.Text += "<span style='color:green;' title='Plánovaný termín dokončení'> " & Format(c.DatPlanUntil, "dd.MM.yyyy") & "</span>"
            End If
            If c.DatGen <= Now Then lblNextGen.Text += " - > " & BO.BAS.FD(Now)
        End With

    End Sub
  

    Private Sub RefreshP40(cRecSum As BO.p41ProjectSum)
        If cRecSum.p40_Exist Then
            Dim lisP40 As IEnumerable(Of BO.p40WorkSheet_Recurrence) = Master.Factory.p40WorkSheet_RecurrenceBL.GetList(Master.DataPID, 0)
            rpP40.DataSource = lisP40
            rpP40.DataBind()
        Else
            cRecSum.p40_Exist = False
        End If
        boxP40.Visible = cRecSum.p40_Exist
    End Sub


    Private Sub Handle_Permissions(cRec As BO.p41Project, cP42 As BO.p42ProjectType, cDisp As BO.p41RecordDisposition)

        With Master.Factory

            Me.p31summary1.Visible = False
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                If cP42.p42IsModule_p31 And menu1.IsExactApprovingPerson Then Me.p31summary1.Visible = True
            End If


        End With
        With cDisp
            boxP30.Visible = .OwnerAccess
            If cRec.p41TreeNext > cRec.p41TreePrev And .OwnerAccess Then
                linkBatchUpdateChilds.Visible = True
            End If

        End With


        panDraftCommands.Visible = False
        If cRec.b02ID = 0 And cRec.p41IsDraft And cDisp.OwnerAccess Then
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P41_Creator) Then panDraftCommands.Visible = True 'pokud je vlastník a má právo zakládat ostré projekty a projekt nemá workflow šablonu
        End If



    End Sub

    Private Sub RefreshBillingLanguage(cRec As BO.p41Project, cClient As BO.p28Contact)
        imgFlag_Project.Visible = False : imgFlag_Client.Visible = False
        With cRec
            If .p87ID > 0 Then
                Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(.p87ID)
                If cP87.p87Icon <> "" Then
                    imgFlag_Project.Visible = True
                    imgFlag_Project.ImageUrl = "Images/flags/" & cP87.p87Icon
                End If

            End If
            If .p87ID_Client > 0 Then
                If Not cClient Is Nothing Then
                    Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(.p87ID_Client)
                    If cP87.p87Icon <> "" Then
                        imgFlag_Client.Visible = True
                        imgFlag_Client.ImageUrl = "Images/flags/" & cP87.p87Icon
                    End If
                End If
            End If
        End With
    End Sub
    Private Sub RefreshPricelist(cRec As BO.p41Project, cClient As BO.p28Contact)
        Me.clue_p51id_billing.Visible = False : Me.p51Name_Billing.Visible = False : Me.lblX51_Message.Text = ""
        With cRec
            If .p51ID_Billing > 0 Then
                Me.p51Name_Billing.Visible = True : clue_p51id_billing.Visible = True
                Me.p51Name_Billing.Text = .p51Name_Billing
                If .p51Name_Billing.IndexOf(cRec.p41Name) >= 0 Then
                    'sazby na míru
                    p51Name_Billing.Text = "Tento projekt má sazby na míru"
                End If
                Me.clue_p51id_billing.Attributes("rel") = "clue_p51_record.aspx?pid=" & .p51ID_Billing.ToString
            Else
                If Not cClient Is Nothing Then
                    With cClient
                        If .p51ID_Billing > 0 Then
                            Me.lblX51_Message.Text = "(dědí se z klienta)"
                            Me.p51Name_Billing.Visible = True : clue_p51id_billing.Visible = True
                            Me.p51Name_Billing.Text = .p51Name_Billing
                            Me.clue_p51id_billing.Attributes("rel") = "clue_p51_record.aspx?pid=" & .p51ID_Billing.ToString
                        End If
                    End With
                End If
            End If
        End With
        If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            Me.clue_p51id_billing.Visible = False  'uživatel nemá oprávnění vidět sazby
        End If
        If Me.p51Name_Billing.Text = "" And lblX51_Message.Text = "" Then trP51.Visible = False
    End Sub

    Private Sub chkFFShowFilledOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkFFShowFilledOnly.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p41_framework_detail-chkFFShowFilledOnly", BO.BAS.GB(Me.chkFFShowFilledOnly.Checked))
        ReloadPage()
    End Sub

    Private Sub rpP40_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP40.ItemDataBound
        Dim cRec As BO.p40WorkSheet_Recurrence = CType(e.Item.DataItem, BO.p40WorkSheet_Recurrence)
        With CType(e.Item.FindControl("p40Name"), HyperLink)
            .Text = cRec.p40Name & " (" & cRec.p34Name & "): " & BO.BAS.FN(cRec.p40Value) & ",-"
            .NavigateUrl = "javascript:p40_record(" & cRec.PID.ToString & ")"
        End With
        With CType(e.Item.FindControl("clue_p40"), HyperLink)
            If cRec.p56ID = 0 Then
                .Attributes("rel") = "clue_p40_record.aspx?pid=" & cRec.PID.ToString
            Else
                .Visible = False
            End If

        End With
        With CType(e.Item.FindControl("linkChrono"), HyperLink)
            If cRec.p56ID = 0 Then
                .NavigateUrl = "javascript:p40_chrono(" & cRec.PID.ToString & ")"
            Else
                '.NavigateUrl = "javascript:contMenu('p56_record.aspx?pid=" & cRec.p56ID.ToString & "',false)"
                .NavigateUrl = "p56_framework.aspx?pid=" & cRec.p56ID.ToString
                .Target = "_top"
                .CssClass = "value_link"
                .Text = cRec.Task
            End If

        End With
    End Sub



    Private Sub ReloadPage()
        Response.Redirect("p41_framework_detail.aspx?pid=" & Master.DataPID.ToString & "&source=" & menu1.PageSource)
    End Sub

    Private Sub RenderTree(cRec As BO.p41Project, cRecSum As BO.p41ProjectSum)
        If Not tree1.IsEmpty Then Return
        tree1.Visible = True
        Dim c As BO.p41Project = Master.Factory.p41ProjectBL.LoadTreeTop(cRec.p41TreeIndex)
        If c Is Nothing Then Return
        Dim mq As New BO.myQueryP41
        mq.TreeIndexFrom = c.p41TreePrev
        mq.TreeIndexUntil = c.p41TreeNext
        'Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq).Where(Function(p) (p.p41TreeNext > p.p41TreePrev And p.p41TreeLevel < cRec.p41TreeLevel) Or p.PID = cRec.PID).OrderBy(Function(p) p.p41TreeIndex)
        Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq).OrderBy(Function(p) p.p41TreeIndex)
        If lis.Count > 20 Then
            lis = lis.Where(Function(p) (p.p41TreeNext > p.p41TreePrev And p.p41TreeLevel < cRec.p41TreeLevel) Or p.PID = cRec.PID)
        End If
        For Each c In lis
            Dim n As Telerik.Web.UI.RadTreeNode = tree1.AddItem(c.PrefferedName, c.PID.ToString, "p41_framework.aspx?pid=" & c.PID.ToString, c.p41ParentID.ToString, "Images/tree.png", , "_top")
            If menu1.PageSource = "navigator" Then
                n.Target = "" : n.NavigateUrl = "p41_framework_detail.aspx?pid=" & c.PID.ToString
            End If
            If c.p41TreeLevel = 1 Then n.ForeColor = basUIMT.TreeColorLevel1
            If c.p41TreeLevel > 1 Then n.ForeColor = basUIMT.TreeColorLevel2
            If c.IsClosed Then n.Font.Strikeout = True
            If c.PID = cRec.PID Then n.Font.Bold = True : n.ImageUrl = "Images/ok.png" : n.NavigateUrl = "" : n.Selected = True
        Next
        tree1.ExpandAll()
    End Sub

    Private Sub cmdConvertDraft2Normal_Click(sender As Object, e As EventArgs) Handles cmdConvertDraft2Normal.Click
        With Master.Factory.p41ProjectBL
            If .ConvertFromDraft(Master.DataPID) Then
                ReloadPage()
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    
  

   
End Class