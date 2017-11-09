Imports Telerik.Web.UI
Public Class entity_menu
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Public Property DataPrefix As String
        Get
            Return hidDataPrefix.Value
        End Get
        Set(value As String)
            hidDataPrefix.Value = value
        End Set
    End Property
  
    Public Property DataPID As Integer
    


    Public Property CurrentTab As String
        Get
            If tabs1.Tabs.Count = 0 Then Return ""
            If tabs1.SelectedTab Is Nothing Then
                tabs1.SelectedIndex = 0
            End If
            Return tabs1.SelectedTab.Value
        End Get
        Set(value As String)
            If tabs1.FindTabByValue(value) Is Nothing Then Return
            tabs1.FindTabByValue(value).Selected = True
        End Set
    End Property
    Public Property LockedTab As String
        Get
            Return hidLockedTab.Value
        End Get
        Set(value As String)
            hidLockedTab.Value = value
        End Set
    End Property
   
    Public ReadOnly Property IsExactApprovingPerson As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsCanApprove.Value)
        End Get
    End Property
   
    Public Property TabSkin As String
        Get
            Return tabs1.Skin
        End Get
        Set(value As String)
            If value = "" Then value = "Default"
            tabs1.Skin = value
        End Set
    End Property
    
    ''Public Property ShowLevel1 As Boolean
    ''    Get
    ''        Return FNO("level1").Visible
    ''    End Get
    ''    Set(value As Boolean)
    ''        FNO("level1").Visible = value
    ''    End Set
    ''End Property
    Public Property x31ID_Plugin As String
        Get
            Return Me.hidPlugin.Value
        End Get
        Set(value As String)
            Me.hidPlugin.Value = value
        End Set
    End Property
    Public ReadOnly Property PageSource As String
        Get
            Return Me.hidSource.Value
        End Get
    End Property

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Page.IsPostBack Then
            Me.hidSource.Value = Request.Item("source")
            ''Me.hidParentWidth.Value = Request.Item("parentWidth")
            
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Me.Factory.SysUser.OneProjectPage <> "" Then
                Server.Transfer(basUI.AddQuerystring2Page(Me.Factory.SysUser.OneProjectPage, "pid=" & Me.DataPID.ToString))
            End If
            If Request.Item("savetab") = "1" Then
                Me.Factory.j03UserBL.SetUserParam(Me.DataPrefix & "_framework_detail-tab", Request.Item("tab"))
            End If

            ''FNO("saw").NavigateUrl = Me.DataPrefix & "_framework_detail.aspx?saw=1"
        End If
        ''Dim cbx As RadComboBox = CType(FNO("search").FindControl("cbxSearch"), RadComboBox)
        Dim cbx As New RadComboBox()
        With cbx
            .DropDownWidth = Unit.Parse("400px")
            .RenderMode = RenderMode.Auto
            .EnableTextSelection = True
            .MarkFirstMatch = True
            .EnableLoadOnDemand = True
            .Width = Unit.Parse("200px")
            .Style.Item("margin-top") = "5px"
            .OnClientItemsRequesting = "cbxSearch_OnClientItemsRequesting"
            .OnClientSelectedIndexChanged = "cbxSearch_OnClientSelectedIndexChanged"
            .WebServiceSettings.Method = "LoadComboData"
            .Text = "Hledat..."
            ''.OnClientFocus = "cbxSearch_OnClientFocus"
        End With

        Dim s As String = "", strTop As String = "5px"
        If hidSource.Value = "3" Then strTop = "44px"
        Select Case Me.DataPrefix
            Case "p28"
                s = "<img src='Images/contact_32.png' style='position:absolute;left:5px;top:" & strTop & ";'/>"
                cbx.WebServiceSettings.Path = "~/Services/contact_service.asmx"
                cbx.ToolTip = "Hledat klienta"
                If Factory.SysUser.j03PageMenuFlag = 0 Then imgPM.ImageUrl = "Images/contact_32.png"
            Case "j02"
                s = "<img src='Images/person_32.png' style='position:absolute;left:5px;top:" & strTop & ";'/>"
                cbx.WebServiceSettings.Path = "~/Services/person_service.asmx"
                cbx.ToolTip = "Hledat osobu"
                If Factory.SysUser.j03PageMenuFlag = 0 Then imgPM.ImageUrl = "Images/person_32.png"
            Case "p56"
                s = "<img src='Images/task_32.png' style='position:absolute;left:5px;top:" & strTop & ";'/>"
                cbx.WebServiceSettings.Path = "~/Services/task_service.asmx"
                cbx.ToolTip = "Hledat úkol"
                If Factory.SysUser.j03PageMenuFlag = 0 Then imgPM.ImageUrl = "Images/task_32.png"

            Case "o23"
                s = "<img src='Images/notepad_32.png' style='position:absolute;left:5px;top:" & strTop & ";'/>"
                cbx.WebServiceSettings.Path = "~/Services/doc_service.asmx"
                cbx.ToolTip = "Hledat dokument"
                If Factory.SysUser.j03PageMenuFlag = 0 Then imgPM.ImageUrl = "Images/notepad_32.png"
            Case "p41"
                s = "<img src='Images/project_32.png' style='position:absolute;left:5px;top:" & strTop & ";'/>"
                cbx.WebServiceSettings.Path = "~/Services/project_service.asmx"
                cbx.ToolTip = "Hledat projekt"
                If Factory.SysUser.j03PageMenuFlag = 0 Then imgPM.ImageUrl = "Images/project_32.png"
        End Select
        If hidSource.Value = "3" Then
            cmdGo2Grid.Visible = True
        Else
            cmdGo2Grid.Visible = False
        End If
        If hidSource.Value = "2" Or hidSource.Value = "1" Then
            'sb1.Visible = False
            'sb1.ashx = ""
            'FNO("searchbox").Visible = False
        Else
            'FNO("searchbox").Controls.Add(cbx)
        End If
        ''If Factory.SysUser.j03PageMenuFlag = 1 Then
        ''    'klasické menu
        ''    If hidSource.Value = "2" Then
        ''        'panel nahoře a dole
        ''    Else
        ''        place0.Controls.Add(New LiteralControl(s))
        ''    End If

        ''End If
        


        
        Handle_PluginBellowMenu()
        tabs1.DataBind()

    End Sub
    Private Sub Handle_PluginBellowMenu()
        If Me.x31ID_Plugin = "" Then Return
        If Me.hidPlugin_FileName.Value = "" Then
            Dim cX31 As BO.x31Report = Factory.x31ReportBL.Load(CInt(Me.x31ID_Plugin))
            If Not cX31 Is Nothing Then
                Me.hidPlugin_FileName.Value = cX31.ReportFileName
                Me.hidPlugin_Height.Value = cX31.x31PluginHeight.ToString
                If Me.hidPlugin_Height.Value = "0" Then Me.hidPlugin_Height.Value = "30"
            End If
        End If
        If Me.hidPlugin_FileName.Value = "" Then Return

        place1.Controls.Add(New LiteralControl("<iframe id='fraPlugin' width='100%' height='" & Me.hidPlugin_Height.Value & "px' frameborder='0' src='Plugins/" & Me.hidPlugin_FileName.Value & "?pid=" & Me.DataPID.ToString & "'></iframe>"))
    End Sub
    Public Sub p41_RefreshRecord(cRec As BO.p41Project, cRecSum As BO.p41ProjectSum, strTabValue As String, Optional cDisp As BO.p41RecordDisposition = Nothing)
        If cRec Is Nothing Then Return
        Me.DataPID = cRec.PID
        If cDisp Is Nothing Then cDisp = Me.Factory.p41ProjectBL.InhaleRecordDisposition(cRec)
        Dim cP42 As BO.p42ProjectType = Me.Factory.p42ProjectTypeBL.Load(cRec.p42ID)
        p41_SetupTabs(cRecSum, cP42, cDisp)
        Handle_TestIfCanApproveOrInvoice(cRec, cP42, cDisp)

        If hidSource.Value = "2" Then
            panPM1.Controls.Clear()
            panPM1.Visible = False
            ShowHideMenu(False, False)
        Else

            ShowHideMenu(False, True)

            pm1.Attributes.Item("onclick") = "RCM('p41', " & cRec.PID.ToString & ", this, 'pagemenu')"
            With linkPM
                .Text = cRec.FullName
                If Len(.Text) > 70 Then
                    .ToolTip = cRec.FullName
                    .Text = Left(.Text, 70) & "..."
                End If
                .Text += " <span class='lbl'>[" & cRec.p42Name & ": " & cRec.p41Code & "]</span>"
                .Attributes.Item("onclick") = "RCM('p41', " & cRec.PID.ToString & ", this, 'pagemenu')"
            End With
            Handle_ContextMenuBlackWhite(cRec.IsClosed)
        End If
        


        Me.CurrentTab = strTabValue


        Handle_SelectedTab()

    End Sub
    Private Sub Handle_ContextMenuBlackWhite(bolRecIsClosed As Boolean)
        If bolRecIsClosed Then panPM1.Style.Item("background-color") = "black" : linkPM.Style.Item("color") = "white"
    End Sub

    Private Sub HighLight_LockedTab(strTab As String)
        For Each t As RadTab In tabs1.Tabs
            If t.Selected Then
                t.FindControl("cmdLock").Visible = True
                CType(t.FindControl("cmdLock"), HtmlButton).Attributes.Item("onclick") = "lockTabs('" & t.Value & "')"
            Else
                t.FindControl("cmdLock").Visible = False
            End If


        Next
        If strTab = "" Then Return
        Dim mytab As RadTab = tabs1.FindTabByValue(strTab)
        If Not mytab Is Nothing Then
            mytab.ImageUrl = "Images/lock.png"
            If mytab.Selected Then
                CType(mytab.FindControl("cmdLock"), HtmlButton).Visible = False
            End If

        End If
    End Sub

    Private Sub Handle_TestIfCanApproveOrInvoice(cRec As BO.p41Project, cP42 As BO.p42ProjectType, cDisp As BO.p41RecordDisposition)
        Dim bolCanApproveOrInvoice As Boolean = Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver, BO.x53PermValEnum.GR_P91_Creator)
        If cP42.p42IsModule_p31 Then
            If Not bolCanApproveOrInvoice Then bolCanApproveOrInvoice = Me.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Draft_Creator)
            If bolCanApproveOrInvoice = False And cDisp.x67IDs.Count > 0 Then
                Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = Me.Factory.x67EntityRoleBL.GetList_o28(cDisp.x67IDs)
                If lisO28.Where(Function(p) p.o28PermFlag = BO.o28PermFlagENUM.CistASchvalovatVProjektu Or p.o28PermFlag = BO.o28PermFlagENUM.CistAEditASchvalovatVProjektu).Count > 0 Then
                    bolCanApproveOrInvoice = True
                End If
            End If
            
        End If
        hidIsCanApprove.Value = BO.BAS.GB(bolCanApproveOrInvoice)
    End Sub

    

    


    ''Private Sub hmi(strMenuValue As String, bolVisible As Boolean)
    ''    Dim mi As NavigationNode = FNO(strMenuValue)
    ''    If mi Is Nothing Then Return
    ''    mi.Visible = bolVisible
    ''End Sub
    ''Private Function ami(strText As String, strValue As String, strURL As String, strImg As String, miParent As NavigationNode, Optional strToolTip As String = "", Optional bolSeparatorBefore As Boolean = False, Optional strTarget As String = "") As NavigationNode
    ''    If bolSeparatorBefore And Not miParent Is Nothing Then
    ''        ''Dim sep As New RadMenuItem()
    ''        ''sep.IsSeparator = True
    ''        ''miParent.Nodes.Add(sep)
    ''    End If
    ''    Dim mi As New NavigationNode(strText)
    ''    mi.NavigateUrl = strURL
    ''    mi.ID = strValue
    ''    mi.ImageUrl = strImg
    ''    mi.ToolTip = strToolTip
    ''    mi.Target = strTarget
    ''    If Not miParent Is Nothing Then
    ''        miParent.Nodes.Add(mi)
    ''    Else
    ''        menu1.Nodes.Insert(menu1.Nodes.Count - 1, mi)
    ''    End If

    ''    Return mi
    ''End Function


    Private Sub cti(strName As String, strX61Code As String)
        Dim cX61 As New BO.x61PageTab
        cX61.x61Code = strX61Code
        Dim tab As New RadTab(strName, strX61Code)
        tabs1.Tabs.Add(tab)

        tab.NavigateUrl = cX61.GetPageUrl(Me.DataPrefix, Me.DataPID, Me.hidIsCanApprove.Value) & "&tab=" & strX61Code & "&source=" & Me.hidSource.Value
        tab.Attributes.Item("myurl") = tab.NavigateUrl
        If tabs1.Tabs.Count = 0 Then tab.Selected = True

        
    End Sub
    Private Sub p41_SetupTabs(crs As BO.p41ProjectSum, cP42 As BO.p42ProjectType, cDisp As BO.p41RecordDisposition)
        tabs1.Tabs.Clear()
        Dim s As String = ""
        cti("Projekt", "board")
        If cP42.p42IsModule_p31 Then
            ''s = "Summary" : cti(s, "summary")
            s = "Worksheet"
            If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                With crs
                    If .p31_Wip_Time_Count > 0 Or .p31_Wip_Expense_Count > 0 Or .p31_Wip_Fee_Count > 0 Then
                        s += "<span class='badge1wip'>" & .p31_Wip_Time_Count.ToString & "+" & .p31_Wip_Expense_Count.ToString & "+" & .p31_Wip_Fee_Count.ToString & "</span>"
                    End If
                End With
            End If
            cti(s, "p31")

            s = "Hodiny"
            If crs.p31_Wip_Time_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Time_Count.ToString & "</span>"
            If crs.p31_Approved_Time_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Time_Count.ToString & "</span>"
            cti(s, "time")

            s = "Výdaje"
            If crs.p31_Wip_Expense_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Expense_Count.ToString & "</span>"
            If crs.p31_Approved_Expense_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Expense_Count.ToString & "</span>"
            cti(s, "expense")
            If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                s = "Odměny"
                If crs.p31_Wip_Fee_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Fee_Count.ToString & "</span>"
                If crs.p31_Approved_Fee_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Fee_Count.ToString & "</span>"
                cti(s, "fee")
                If cDisp.p91_Read Then
                    s = "Faktury"
                    If crs.p91_Count > 0 Then s += "<span class='badge1tab'>" & crs.p91_Count.ToString & "</span>"
                    cti(s, "p91")
                End If
            End If
        End If
        If crs.childs_Count > 0 Then
            s = "Pod-projekty<span class='badge1tab'>" & crs.childs_Count.ToString & "</span>"
            cti(s, "p41")
        End If
        If cP42.p42IsModule_p56 Then
            s = "Úkoly"
            If crs.p56_Actual_Count > 0 Then s += "<span class='badge1tab'>" & crs.p56_Actual_Count.ToString & "</span>"
            cti(s, "p56")
        End If
        If cP42.p42IsModule_p45 Then
            If cDisp.p45_Read Then
                s = "Rozpočet"
                If crs.p45_Count > 0 Then s += "<span class='badge1tab'>" & crs.p45_Count.ToString & "</span>"
                cti(s, "budget")
            End If
        End If
        If cP42.p42IsModule_o23 Then
            s = "Dokumenty"
            If crs.o23_Count > 0 Then s += "<span class='badge1tab'>" & crs.o23_Count.ToString & "</span>"
            cti(s, "o23")
        End If

    End Sub

    Private Sub RemoveTab(strTabValue As String)
        With tabs1.Tabs
            If Not .FindTabByValue(strTabValue) Is Nothing Then
                .Remove(.FindTabByValue(strTabValue))
                If .Count > 0 Then tabs1.SelectedIndex = 0

            End If
        End With
    End Sub
    Private Sub Handle_SelectedTab()

        If tabs1.SelectedTab Is Nothing And tabs1.Tabs.Count > 0 Then
            'pokud není označená záložka, pak skočit na první
            Dim c As New BO.x61PageTab
            c.x61Code = "board"
            Server.Transfer(c.GetPageUrl(Me.DataPrefix, Me.DataPID, "") & "&tab=board&source=" & Me.hidSource.Value, False)
        End If
        If Not tabs1.SelectedTab Is Nothing Then
            tabs1.SelectedTab.NavigateUrl = ""
            tabs1.SelectedTab.Style.Item("cursor") = "default"
        End If
    End Sub
    Private Sub ShowHideMenu(bolPopupMenu As Boolean, bolContextMenu As Boolean)
       
        If bolContextMenu Then
            panPM1.Visible = True
        Else
            panPM1.Controls.Clear()
            panPM1.Visible = False
        End If
    End Sub
    Public Sub p28_RefreshRecord(cRec As BO.p28Contact, cRecSum As BO.p28ContactSum, strTabValue As String, Optional cDisp As BO.p28RecordDisposition = Nothing)
        If cRec Is Nothing Then Return
        Me.DataPID = cRec.PID
        If cDisp Is Nothing Then cDisp = Me.Factory.p28ContactBL.InhaleRecordDisposition(cRec)
        p28_SetupTabs(cRecSum)

        If hidSource.Value = "2" Then
            ShowHideMenu(False, False)
        Else
            ShowHideMenu(False, True)
            pm1.Attributes.Item("onclick") = "RCM('p28', " & cRec.PID.ToString & ", this, 'pagemenu')"
            With linkPM
                .Text = cRec.p28Name & " <span class='lbl'>[" & cRec.p28Code & "]</span>"
                ''.NavigateUrl = "p28_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value
                .Attributes.Item("onclick") = "RCM('p28', " & cRec.PID.ToString & ", this, 'pagemenu')"
            End With
            Handle_ContextMenuBlackWhite(cRec.IsClosed)
        End If


        

        Me.CurrentTab = strTabValue
        Handle_SelectedTab()
    End Sub

    Private Sub p28_SetupTabs(crs As BO.p28ContactSum)
        tabs1.Tabs.Clear()
        cti("Klient", "board")
        Dim bolAllowRates As Boolean = Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates)
        Dim lisX61 As IEnumerable(Of BO.x61PageTab) = Me.Factory.j03UserBL.GetList_PageTabs(Me.Factory.SysUser.PID, BO.x29IdEnum.p28Contact)
        For Each c In lisX61
            Dim s As String = c.x61Name, bolGo As Boolean = True
            Select Case c.x61Code
                Case "p31"
                    If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                        With crs
                            If .p31_Wip_Time_Count > 0 Or .p31_Wip_Expense_Count > 0 Or .p31_Wip_Fee_Count > 0 Then
                                s += "<span class='badge1wip'>" & .p31_Wip_Time_Count.ToString & "+" & .p31_Wip_Expense_Count.ToString & "+" & .p31_Wip_Fee_Count.ToString & "</span>"
                            End If
                        End With
                    End If
                Case "time"
                    If crs.p31_Wip_Time_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Time_Count.ToString & "</span>"
                    If crs.p31_Approved_Time_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Time_Count.ToString & "</span>"
                Case "kusovnik"
                    If crs.p31_Wip_Kusovnik_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Kusovnik_Count.ToString & "</span>"
                    If crs.p31_Approved_Kusovnik_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Kusovnik_Count.ToString & "</span>"
                Case "expense"
                    If crs.p31_Wip_Expense_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Expense_Count.ToString & "</span>"
                    If crs.p31_Approved_Expense_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Expense_Count.ToString & "</span>"
                Case "fee"
                    If crs.p31_Wip_Fee_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Fee_Count.ToString & "</span>"
                    If crs.p31_Approved_Fee_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Fee_Count.ToString & "</span>"
                    bolGo = bolAllowRates
                Case "p91"
                    If crs.p91_Count > 0 Then s += "<span class='badge1tab'>" & crs.p91_Count.ToString & "</span>"
                    bolGo = Me.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Reader)
                Case "p56"
                    If crs.p56_Actual_Count > 0 Then s += "<span class='badge1tab'>" & crs.p56_Actual_Count.ToString & "</span>"
                Case "o23"
                    If crs.o23_Count > 0 Then s += "<span class='badge1tab'>" & crs.o23_Count.ToString & "</span>"
                Case "p41"
                    s += "<span class='badge1tab'>" & crs.p41_Actual_Count.ToString & "+" & crs.p41_Closed_Count.ToString & "</span>"
                Case "p90"
                    If crs.p90_Count > 0 Then s += "<span class='badge1tab'>" & crs.p90_Count.ToString & "</span>"
                    bolGo = Me.Factory.TestPermission(BO.x53PermValEnum.GR_P90_Reader)
            End Select

            If bolGo Then cti(s, c.x61Code)
        Next
       
    End Sub

    Private Sub Handle_NoAccess(strMessage As String)
        Response.Redirect("stoppage.aspx?err=1&message=" & Server.UrlEncode(strMessage), True)
    End Sub

    

    Public Sub j02_RefreshRecord(cRec As BO.j02Person, cRecSum As BO.j02PersonSum, strTabValue As String)
        If cRec Is Nothing Then Return
        Me.DataPID = cRec.PID

        j02_SetupTabs(cRec, cRecSum)
        If hidSource.Value = "2" Then
            panPM1.Controls.Clear()
            panPM1.Visible = False
            ShowHideMenu(False, False)
        Else
            ShowHideMenu(False, True)

            pm1.Attributes.Item("onclick") = "RCM('j02'," & cRec.PID.ToString & ",this,'pagemenu')"
            With linkPM
                .Text = cRec.FullNameDesc
                If cRec.j07ID <> 0 Then .Text += " <span class='lbl'>[" & cRec.j07Name & "]</span>"
                If cRec.j02JobTitle <> "" Then .Text += " <span class='lbl'>[" & cRec.j02JobTitle & "]</span>"
                If Not cRec.j02IsIntraPerson Then .Font.Italic = True
                ''.NavigateUrl = "j02_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value
                .Attributes.Item("onclick") = "RCM('j02', " & cRec.PID.ToString & ", this, 'pagemenu')"
            End With
            Handle_ContextMenuBlackWhite(cRec.IsClosed)
        End If

        


        Me.CurrentTab = strTabValue
        Handle_SelectedTab()
    End Sub

    

    Private Sub j02_SetupTabs(cRec As BO.j02Person, crs As BO.j02PersonSum)
        tabs1.Tabs.Clear()
        If cRec.j02IsIntraPerson Then
            cti(cRec.FullNameAsc, "board")
        Else
            cti(System.String.Format("{0} (kontaktní osoba)", cRec.FullNameAsc), "board")
            Return
        End If
        Dim lisX61 As IEnumerable(Of BO.x61PageTab) = Me.Factory.j03UserBL.GetList_PageTabs(Me.Factory.SysUser.PID, BO.x29IdEnum.j02Person)
        For Each c In lisX61
            Dim s As String = c.x61Name
            Select Case c.x61Code
                Case "p31"
                    If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                        With crs
                            If .p31_Wip_Time_Count > 0 Or .p31_Wip_Expense_Count > 0 Or .p31_Wip_Fee_Count > 0 Then
                                s += "<span class='badge1wip'>" & .p31_Wip_Time_Count.ToString & "+" & .p31_Wip_Expense_Count.ToString & "+" & .p31_Wip_Fee_Count.ToString & "</span>"
                            End If
                        End With
                    End If
                Case "time"
                    If crs.p31_Wip_Time_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Time_Count.ToString & "</span>"
                    If crs.p31_Approved_Time_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Time_Count.ToString & "</span>"
                Case "expense"
                    If crs.p31_Wip_Expense_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Expense_Count.ToString & "</span>"
                    If crs.p31_Approved_Expense_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Expense_Count.ToString & "</span>"
                Case "fee"
                    If crs.p31_Wip_Fee_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Fee_Count.ToString & "</span>"
                    If crs.p31_Approved_Fee_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Fee_Count.ToString & "</span>"
                Case "kusovnik"
                    If crs.p31_Wip_Kusovnik_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Kusovnik_Count.ToString & "</span>"
                    If crs.p31_Approved_Kusovnik_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Kusovnik_Count.ToString & "</span>"
                Case "p91"
                    If crs.p91_Count > 0 Then s += "<span class='badge1tab'>" & crs.p91_Count.ToString & "</span>"
                Case "p56"
                    If crs.p56_Actual_Count > 0 Then s += "<span class='badge1tab'>" & crs.p56_Actual_Count.ToString & "</span>"
                Case "o23"
                    If crs.o23_Count > 0 Then s += "<span class='badge1tab'>" & crs.o23_Count.ToString & "</span>"
            End Select

            cti(s, c.x61Code)
        Next


    End Sub

    Public Sub p56_RefreshRecord(cRec As BO.p56Task, crs As BO.p56TaskSum, cP41 As BO.p41Project, strTabValue As String, Optional cDisp As BO.p56RecordDisposition = Nothing)
        If cRec Is Nothing Then Return
        Me.DataPID = cRec.PID

        Dim cP42 As BO.p42ProjectType = Me.Factory.p42ProjectTypeBL.Load(cP41.p42ID)

        cti("Úkol", "board")
        Dim s As String = ""
        If cP42.p42IsModule_p31 Then
            s = "Worksheet"
            If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                With crs
                    If .p31_Wip_Time_Count > 0 Or .p31_Wip_Expense_Count > 0 Or .p31_Wip_Fee_Count > 0 Then
                        s += "<span class='badge1wip'>" & .p31_Wip_Time_Count.ToString & "+" & .p31_Wip_Expense_Count.ToString & "+" & .p31_Wip_Fee_Count.ToString & "</span>"
                    End If
                End With
            End If
            cti(s, "p31")
            s = "Hodiny"
            If crs.p31_Wip_Time_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Time_Count.ToString & "</span>"
            If crs.p31_Approved_Time_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Time_Count.ToString & "</span>"
            cti(s, "time")
            s = "Výdaje"
            If crs.p31_Wip_Expense_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Expense_Count.ToString & "</span>"
            If crs.p31_Approved_Expense_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Expense_Count.ToString & "</span>"
            cti(s, "expense")
            If Me.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                s = "Odměny"
                If crs.p31_Wip_Fee_Count > 0 Then s += "<span class='badge1wip'>" & crs.p31_Wip_Fee_Count.ToString & "</span>"
                If crs.p31_Approved_Fee_Count > 0 Then s += "<span class='badge1approved'>" & crs.p31_Approved_Fee_Count.ToString & "</span>"
                cti(s, "fee")
            End If
            If Me.Factory.SysUser.j04IsMenu_Invoice Then
                s = "Faktury"
                If crs.p91_Count > 0 Then s += "<span class='badge1tab'>" & crs.p91_Count.ToString & "</span>"
                cti(s, "p91")
            End If
        End If
        s = "Dokumenty"
        If crs.o23_Count > 0 Then s += "<span class='badge1tab'>" & crs.o23_Count.ToString & "</span>"
        cti(s, "o23")

        If hidSource.Value = "2" Then
            ShowHideMenu(False, False)
        Else
            ShowHideMenu(False, True)

            pm1.Attributes.Item("onclick") = "RCM('p56'," & cRec.PID.ToString & ",this,'pagemenu')"
            With linkPM
                .Text = BO.BAS.OM3(cRec.FullName, 70)
                ''.NavigateUrl = "p56_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value
                .Attributes.Item("onclick") = "RCM('p56', " & cRec.PID.ToString & ", this, 'pagemenu')"
            End With
            Handle_ContextMenuBlackWhite(cRec.IsClosed)
        End If

        

        Me.CurrentTab = strTabValue
        Handle_SelectedTab()


    End Sub

    



    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Select Case hidSource.Value
            Case "1"
               
            Case "2"
                

            Case "3"

        End Select
        HighLight_LockedTab(hidLockedTab.Value)
    End Sub

    ''Private Function FNO(strValue As String) As NavigationNode
    ''    If Not menu1.Visible Then Return New NavigationNode()
    ''    Return menu1.GetAllNodes.First(Function(p) p.ID = strValue)
    ''End Function

   
  
End Class