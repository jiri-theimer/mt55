Public Class p28_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm

    Private Sub p28_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        menu1.DataPrefix = "p28"
        p31summary1.Factory = Master.Factory
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory
    End Sub

   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cal1.factory = Master.Factory()

        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "p28"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Me.menu1.PageSource = "2" Then
                    .IsHideAllRecZooms = True
                    ''If Request.Item("tab") <> "" Then
                    ''    .Factory.j03UserBL.SetUserParam("p41_framework_detail-tab", Request.Item("tab"))
                    ''End If
                End If

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p28_framework_detail-pid")
                    .Add("p28_framework_detail-tab")
                    .Add("p28_menu-remember-tab")
                    .Add("p28_menu-tabskin")
                    ''.Add("p28_menu-menuskin")
                    .Add("p28_framework_detail-chkFFShowFilledOnly")
                    .Add("p28_framework_detail_pos")
                    .Add("p28_menu-x31id-plugin")
                    .Add("p28_menu-show-level1")
                    .Add("p28_menu-show-cal1")
                    .Add("myscheduler-firstday")
                End With

                Dim intPID As Integer = Master.DataPID
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    'úvodní dispečerská stránka
                    If intPID = 0 Then
                        intPID = BO.BAS.IsNullInt(.GetUserParam("p28_framework_detail-pid", "O"))
                        If intPID = 0 Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p28")
                    Else
                        If intPID <> BO.BAS.IsNullInt(.GetUserParam("p28_framework_detail-pid", "O")) Then
                            .SetUserParam("p28_framework_detail-pid", intPID.ToString)
                        End If
                    End If
                    Dim strTab As String = Request.Item("tab")
                    If strTab = "" And .GetUserParam("p28_menu-remember-tab", "0") = "1" Then strTab = .GetUserParam("p28_framework_detail-tab", "board")
                    Select Case strTab
                        Case "p31", "time", "expense", "fee", "kusovnik"
                            Server.Transfer("entity_framework_rec_p31.aspx?masterprefix=p28&masterpid=" & intPID.ToString & "&p31tabautoquery=" & strTab & "&source=" & menu1.PageSource, False)
                        Case "o23", "p91", "p56", "p41"
                            Server.Transfer("entity_framework_rec_" & strTab & ".aspx?masterprefix=p28&masterpid=" & intPID.ToString & "&source=" & menu1.PageSource, False)
                        Case "p90"
                            Server.Transfer("p28_framework_detail_p90.aspx?masterpid=" & intPID.ToString & "&source=" & menu1.PageSource, False)
                        Case Else
                            'zůstat zde na BOARD stránce
                    End Select
                    cal1.FirstDayMinus = BO.BAS.IsNullInt(.GetUserParam("myscheduler-firstday", "-1"))
                    hidCal1ShallBeActive.Value = .GetUserParam("p28_menu-show-cal1", "1")
                    menu1.TabSkin = .GetUserParam("p28_menu-tabskin")
                    ''menu1.MenuSkin = .GetUserParam("p28_menu-menuskin")
                    menu1.x31ID_Plugin = .GetUserParam("p28_menu-x31id-plugin")
                    If .GetUserParam("p28_menu-remember-tab", "0") = "1" Then
                        menu1.LockedTab = .GetUserParam("p28_framework_detail-tab")
                    End If
                    Me.chkFFShowFilledOnly.Checked = BO.BAS.BG(.GetUserParam("p28_framework_detail-chkFFShowFilledOnly", "0"))
                End With
                Master.DataPID = intPID
            End With


        End If
        RefreshRecord()
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
        If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=p28")

        Dim cRecSum As BO.p28ContactSum = Master.Factory.p28ContactBL.LoadSumRow(cRec.PID)
        Dim cDisp As BO.p28RecordDisposition = Master.Factory.p28ContactBL.InhaleRecordDisposition(cRec)

        Handle_Permissions(cRec, cDisp)

        menu1.p28_RefreshRecord(cRec, cRecSum, "board", cDisp)

        With cRec
            If .p29ID <> 0 Then Me.boxCoreTitle.Text = .p29Name
            Me.boxCoreTitle.Text += " (" & .p28Code & ")"
            If .b02ID <> 0 Then
                Me.boxCoreTitle.Text += ": " & .b02Name
            End If
            Me.Owner.Text = .Owner : Me.linkTimestamp.Text = .UserInsert & "/" & .DateInsert
            If cDisp.OwnerAccess Then
                Me.linkTimestamp.ToolTip = "CHANGE-LOG"
                Me.linkTimestamp.NavigateUrl = "javascript:changelog()"
            End If
            Me.Contact.Text = .p28Name
            If .p28Code <> "" Then
                Me.Contact.Text += " <span style='color:gray;padding-left:10px;'>" & .p28Code & "</span>"
            End If
            If .p28TreeLevel = 1 Then Me.Contact.ForeColor = basUIMT.TreeColorLevel1
            If .p28TreeLevel > 1 Then Me.Contact.ForeColor = basUIMT.TreeColorLevel2


            If .p28CompanyShortName > "" Then
                Me.Contact.Text += "<div style='color:green;'>" & .p28CompanyName & "</div>"
            End If
            If .p28RegID <> "" Or .p28VatID <> "" Or .p28Person_BirthRegID <> "" Then
                Dim cM As New BO.SubjectMonitoring(cRec)
                If .p28RegID <> "" Then
                    Me.linkIC.Visible = True
                    Me.linkIC.Text = .p28RegID
                    Me.linkIC.NavigateUrl = cM.JusticeUrl : Me.linkIC.ToolTip = cM.JusticeName
                    Me.linkARES.NavigateUrl = cM.AresUrl
                    If Me.linkARES.NavigateUrl <> "" Then Me.linkARES.Visible = True
                End If
                If .p28VatID <> "" Then
                    Me.linkDIC.Visible = True
                    Me.linkDIC.Text = .p28VatID
                    Me.linkDIC.NavigateUrl = "javascript:vat_info('" & .p28VatID & "')"
                End If
                If cM.IsirUrl > "" Then
                    Me.linkISIR.Visible = True : Me.linkISIR.NavigateUrl = cM.IsirUrl
                End If


            Else
                trICDIC.Visible = False
            End If

            If .p29ID > 0 Then
                Me.p29Name.Text = "[" & .p29Name & "]"
            End If
            imgDraft.Visible = .p28IsDraft
            If .p28IsDraft Then imgRecord.ImageUrl = "Images/draft.png"
            ''If .p28ParentID <> 0 Then
            ''    Me.trParent.Visible = True
            ''    Me.ParentContact.NavigateUrl = "p28_framework.aspx?pid=" & .p28ParentID.ToString
            ''    Me.ParentContact.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .p28ParentID)
            ''End If
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) And .p28BillingMemo <> "" Then
                boxBillingMemo.Visible = True
                Me.p28BillingMemo.Text = BO.BAS.CrLfText2Html(.p28BillingMemo)
            Else
                boxBillingMemo.Visible = False
            End If
        End With

        RefreshBillingLanguage(cRec)

        RefreshPricelist(cRec)

        ''RefreshProjectList(cRec)


        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p28Contact, cRec.PID)
        Me.roles1.RefreshData(lisX69, cRec.PID)
        If Me.roles1.RowsCount = 0 Then panRoles.Visible = False

        Dim lisO37 As IEnumerable(Of BO.o37Contact_Address) = Master.Factory.p28ContactBL.GetList_o37(cRec.PID)
        Dim lisO32 As IEnumerable(Of BO.o32Contact_Medium) = Master.Factory.p28ContactBL.GetList_o32(cRec.PID)
        If lisO37.Count > 0 Or lisO32.Count > 0 Then
            Me.boxO37.Visible = True
            If lisO37.Count > 0 Then Me.address1.FillData(lisO37)
            If lisO32.Count > 0 Then Me.medium1.FillData(lisO32)
        Else
            Me.boxO37.Visible = False
        End If


        If cRecSum.p30_Exist Then
            Dim lisP30 As IEnumerable(Of BO.j02Person) = Master.Factory.p30Contact_PersonBL.GetList_J02(Master.DataPID, 0, True)
            If lisP30.Count > 0 Then
                Me.boxO37.Visible = True
                Me.persons1.FillData(lisP30, Master.Factory.SysUser.j04IsMenu_People)
            Else
                cRecSum.p30_Exist = False
            End If
        End If


        If cRecSum.Last_p91ID > 0 Then
            Me.linkLastInvoice.Text = cRecSum.Last_Invoice
            If Master.Factory.SysUser.j04IsMenu_Invoice Then
                Me.linkLastInvoice.NavigateUrl = "p91_framework.aspx?pid=" & cRecSum.Last_p91ID.ToString
            End If
        End If
        

        Me.Last_WIP_Worksheet.Text = cRecSum.Last_Wip_Worksheet
        If p31summary1.Visible Then
            If cRecSum.p31_Approved_Time_Count > 0 Or cRecSum.p31_Wip_Time_Count > 0 Or cRecSum.p31_Wip_Expense_Count > 0 Or cRecSum.p31_Wip_Fee_Count > 0 Or cRecSum.p31_Approved_Expense_Count > 0 Then
                Dim mq As New BO.myQueryP31
                mq.p28ID_Client = cRec.PID
                Dim cWorksheetSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, True)
                If cWorksheetSum.RowsCount = 0 Then
                    boxP31Summary.Visible = False
                Else
                    p31summary1.RefreshData(cWorksheetSum, "p28", Master.DataPID, Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates), 0, 0)
                End If
            Else
                p31summary1.Visible = False
                If cRecSum.Last_Invoice = "" And cRecSum.Last_Wip_Worksheet = "" Then Me.boxP31Summary.Visible = False
            End If
        End If
        

        If cRec.b02ID > 0 Then
            Me.trWorkflow.Visible = True
            Me.b02Name.Text = cRec.b02Name
        Else
            Me.trWorkflow.Visible = False
        End If

        labels1.RefreshData(Master.Factory, BO.x29IdEnum.p28Contact, cRec.PID)
        boxX18.Visible = labels1.ContainsAnyData

        Dim lisFF As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p28Contact, Master.DataPID, cRec.p29ID)
        If lisFF.Count > 0 Then
            ff1.FillData(lisFF, Not Me.chkFFShowFilledOnly.Checked)
            chkFFShowFilledOnly.Visible = True
        Else
            chkFFShowFilledOnly.Visible = False
        End If
        If cRec.p28ParentID <> 0 Then
            RenderTree(cRec, cRecSum)
        End If

        If cRecSum.o48_Exist Then linkISIR_Monitoring.Text = "ANO" : linkISIR.Font.Bold = True Else linkISIR_Monitoring.Text = "NE"


        If cRecSum.b07_Count > 0 Or cRec.b02ID > 0 Then
            comments1.Visible = True
            comments1.RefreshData(Master.Factory, BO.x29IdEnum.p28Contact, cRec.PID)
        Else
            comments1.Visible = False
        End If

        If cRecSum.p41_Actual_Count > 0 And Master.Factory.SysUser.j04IsMenu_Project Then
            boxP41.Visible = True
            RefreshProjectList(cRec, cRecSum.p41_Actual_Count)
        Else
            boxP41.Visible = False
        End If

        If hidCal1ShallBeActive.Value = "1" Then
            cal1.RecordPID = Master.DataPID
            If cRecSum.p56_Actual_Count > 0 Or cRecSum.o22_Actual_Count > 0 Then
                cal1.RefreshData(Today)
                cal1.RefreshTasksWithoutDate(True)
            Else
                cal1.Visible = False
            End If
        Else
            cal1.Visible = False
        End If
        If cRecSum.o52_Exist Then
            tags1.RefreshData(cRec.PID)
        Else
            tags1.RecordPID = cRec.PID
        End If

       
    End Sub

    Private Sub RenderTree(cRec As BO.p28Contact, cRecSum As BO.p28ContactSum)
        tree1.Visible = True
        Dim c As BO.p28Contact = Master.Factory.p28ContactBL.LoadTreeTop(cRec.p28TreeIndex)
        If c Is Nothing Then Return
        Dim mq As New BO.myQueryP28
        mq.TreeIndexFrom = c.p28TreePrev
        mq.TreeIndexUntil = c.p28TreeNext
        Dim lis As IEnumerable(Of BO.p28Contact) = Master.Factory.p28ContactBL.GetList(mq).Where(Function(p) (p.p28TreeNext > p.p28TreePrev And p.p28TreeLevel < cRec.p28TreeLevel) Or p.PID = cRec.PID).OrderBy(Function(p) p.p28TreeIndex)
        For Each c In lis
            Dim n As Telerik.Web.UI.RadTreeNode = tree1.AddItem(c.p28Name, c.PID.ToString, "p28_framework.aspx?pid=" & c.PID.ToString, c.p28ParentID.ToString, "Images/tree.png", , "_top")
            If c.p28TreeLevel = 1 Then n.ForeColor = basUIMT.TreeColorLevel1
            If c.p28TreeLevel > 1 Then n.ForeColor = basUIMT.TreeColorLevel2
            If c.IsClosed Then n.Font.Strikeout = True
        Next
        tree1.ExpandAll()
    End Sub

    Private Sub Handle_Permissions(cRec As BO.p28Contact, cDisp As BO.p28RecordDisposition)

        If Not cDisp.ReadAccess Then
            Master.StopPage("Nedisponujete přístupovým oprávněním ke klientovi.")
        End If

        Me.p31summary1.Visible = False
        If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            If menu1.IsExactApprovingPerson Then Me.p31summary1.Visible = True
        End If



        If cRec.b02ID = 0 And cRec.p28IsDraft And cDisp.OwnerAccess Then
            panDraftCommands.Visible = True 'pokud je vlastník a projekt nemá workflow šablonu
        Else
            panDraftCommands.Visible = False
        End If


    End Sub

    Private Sub RefreshPricelist(cRec As BO.p28Contact)
        Me.clue_p51id_billing.Visible = False : Me.p51Name_Billing.Visible = False : lblX51.Visible = False
        With cRec
            If .p51ID_Billing > 0 Then
                Me.p51Name_Billing.Visible = True : clue_p51id_billing.Visible = True : lblX51.Visible = True
                Me.p51Name_Billing.Text = .p51Name_Billing
                Try
                    If .p51Name_Billing.IndexOf(cRec.p28Name) >= 0 Then
                        Me.p51Name_Billing.Text = "Sazby na míru"
                    End If
                    Me.clue_p51id_billing.Attributes("rel") = "clue_p51_record.aspx?pid=" & .p51ID_Billing.ToString
                Catch ex As Exception

                End Try


            End If
        End With
        If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
            Me.clue_p51id_billing.Visible = False  'uživatel nemá oprávnění vidět sazby
        End If

    End Sub
    Private Sub RefreshBillingLanguage(cRec As BO.p28Contact)
        imgFlag_Contact.Visible = False
        With cRec
            If .p87ID > 0 Then
                Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(.p87ID)
                If cP87.p87Icon <> "" Then
                    imgFlag_Contact.Visible = True
                    imgFlag_Contact.ImageUrl = "Images/flags/" & cP87.p87Icon
                End If

            End If
        End With
    End Sub



   


    Private Sub ReloadPage()
        Response.Redirect("p28_framework_detail.aspx?pid=" & Master.DataPID.ToString & "&source=" & menu1.PageSource)
    End Sub


    Private Sub chkFFShowFilledOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkFFShowFilledOnly.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p28_framework_detail-chkFFShowFilledOnly", BO.BAS.GB(Me.chkFFShowFilledOnly.Checked))
        ReloadPage()
    End Sub

    Private Sub cmdConvertDraft2Normal_Click(sender As Object, e As EventArgs) Handles cmdConvertDraft2Normal.Click
        With Master.Factory.p28ContactBL
            If .ConvertFromDraft(Master.DataPID) Then
                ReloadPage()
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub RefreshProjectList(cRec As BO.p28Contact, intAllOpenProjects As Integer)
        Dim mq As New BO.myQueryP41
        mq.p28ID = cRec.PID
        mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
        mq.Closed = BO.BooleanQueryMode.FalseQuery


        Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq).OrderBy(Function(p) p.IsClosed).ThenByDescending(Function(p) p.PID)
        If lis.Count = 0 Then
            boxP41.Visible = False : Return
        Else
            boxP41.Visible = True
            Dim intClosed As Integer = lis.Where(Function(p) p.IsClosed = True).Count
            Dim intOpened As Integer = lis.Count - intClosed
            Dim s As String = ""
            With boxP41Title
                If intClosed > 0 Then
                    ''.Text = .Text & " (" & intOpened.ToString & IIf(intClosed > 0, "+" & intClosed.ToString, "") & ")"
                    s = "<span class='badge1'>" & intOpened.ToString & IIf(intClosed > 0, "+" & intClosed.ToString, "") & "</span>"
                Else
                    ''.Text = .Text & " (" & intOpened.ToString & ")"
                    s = "<span class='badge1'>" & intOpened.ToString & "</span>"
                End If

                ''.Text = "<a href='javascript:projects()'>" & .Text & s & "</a>"
            End With

        End If
        ''If Not Me.chkShowBoxP41.Checked Then Return

        If lis.Count > 25 Then
            boxP41Title.Text = BO.BAS.OM4(boxP41Title.Text, ": " & lis.Count.ToString & ", z toho prvních 25:", "")
            lis = lis.Take(25) 'omezit na maximálně 25+1

        End If
        rpP41.DataSource = lis.OrderBy(Function(p) p.IsClosed).ThenBy(Function(p) p.p41Name)
        rpP41.DataBind()

    End Sub
    Private Sub rpP41_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP41.ItemDataBound

        Dim cRec As BO.p41Project = CType(e.Item.DataItem, BO.p41Project)
        With CType(e.Item.FindControl("aProject"), HyperLink)
            If cRec.p41NameShort > "" Then
                .Text = cRec.p41NameShort
            Else
                .Text = cRec.p41Name
            End If
            .Text += " (" & cRec.p41Code & ")"
            If cRec.IsClosed Then .Font.Strikeout = True : .ForeColor = Drawing.Color.Gray
            If Master.Factory.SysUser.j04IsMenu_Project Then
                .NavigateUrl = "p41_framework.aspx?pid=" & cRec.PID.ToString


                
            End If
        End With
        With CType(e.Item.FindControl("linkPP1"), HyperLink)
            .Attributes.Item("onclick") = "RCM('p41'," & cRec.PID.ToString & ",this)"
        End With
        ''CType(e.Item.FindControl("clue_project"), HyperLink).Attributes.Item("rel") = "clue_p41_record.aspx?pid=" & cRec.PID.ToString


    End Sub

End Class