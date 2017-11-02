Public Class entity_modal_approving
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Public Class StatColumn
        Public Property Value As String
        Public Property SelectField As String
        Public Property StatColType As String = "S"
        Public Property StatColC As String = "0"
        Public Property GroupField As String

        Public Property OrderByField As String
        Public Property HeaderText As String

    End Class
    Private Class StructureSQL
        Public Property stringH As String
        Public Property stringF As String
        Public Property stringG As String
        Public Property stringT As String
        Public Property stringC As String
        Public Property stringOF As String
    End Class

   
    Public Class CommandRow
        Public Property PID As Integer
        Public Property Name As String
        Public Property RowCount As Integer
        Public Sub New(intPID As Integer, strName As String, intRowCount As Integer)
            Me.PID = intPID
            Me.Name = strName
            Me.RowCount = intRowCount
        End Sub
    End Class
    Public Property CurrentInputPIDs As String
        Get
            Return Me.hidInputPIDS.Value
        End Get
        Set(value As String)
            Me.hidInputPIDS.Value = value
        End Set
    End Property
    
    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return DirectCast(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property

    Private Sub entity_modal_approving_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
            If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified Then
                Master.StopPage("prefix is missing")
            End If
            If Request.Item("aw") <> "" Then
                Me.hidMasterAW.Value = Replace(Server.UrlDecode(Request.Item("aw")), "xxx", "=")
            End If
            With Master
                .AddToolbarButton("Nastavení", "setting", 0, "Images/arrow_down_menu.png", False)
                .AddToolbarButton("Vystavit fakturu", "continue_invoice", , "Images/continue.png", False, "javascript:invoice()", , True)
                .AddToolbarButton("Přejít do schvalování", "continue", , "Images/continue.png", False, "javascript:approve_all()", , True)
                .RadToolbar.FindItemByValue("setting").CssClass = "show_hide1"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Request.Item("pids") <> "" Then
                    Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Request.Item("pids")).Distinct.ToList
                    Select Case pids.Count
                        Case 0
                        Case 1
                            Master.DataPID = pids(0)
                        Case Is > 100
                            .StopPage("Najednou je možné vybrat maximállně 100 položek.")
                        Case Else
                            Me.CurrentInputPIDs = String.Join(",", pids)
                    End Select
                End If
                If .DataPID = 0 And Me.CurrentInputPIDs = "" Then .StopPage("pid or pids is missing")
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("periodcombo-custom_query")
                    .Add("p31_grid-period")
                    .Add("entity_framework_detail_approving-subgrid")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col1")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col2")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col3")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col1x")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col2x")
                    .Add(Me.CurrentPrefix & "-entity_framework_detail_approving-col3x")
                    .Add("entity_framework_detail_approving-j27ids")
                    .Add("entity_framework_detail_approving-chkCommandsJ02")
                    .Add("entity_framework_detail_approving-chkCommandsP34")
                    .Add("entity_framework_detail_approving-chkCommandsP41")
                    .Add("entity_framework_detail_approving-approving-level")
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("p31_grid-period")

                    basUI.SelectDropdownlistValue(Me.col1, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col1"))
                    basUI.SelectDropdownlistValue(Me.col2, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col2"))
                    basUI.SelectDropdownlistValue(Me.col3, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col3"))
                    basUI.SelectDropdownlistValue(Me.col1x, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col1x", "p31ApprovingSet"))
                    basUI.SelectDropdownlistValue(Me.col2x, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col2x", "p34"))
                    basUI.SelectDropdownlistValue(Me.col3x, .GetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col3x"))
                    Me.chkCommandsJ02.Checked = BO.BAS.BG(.GetUserParam("entity_framework_detail_approving-chkCommandsJ02", "0"))
                    Me.chkCommandsP34.Checked = BO.BAS.BG(.GetUserParam("entity_framework_detail_approving-chkCommandsP34", "1"))
                    Me.chkCommandsP41.Checked = BO.BAS.BG(.GetUserParam("entity_framework_detail_approving-chkCommandsP41", "0"))


                    If col1.SelectedValue = "" And col2.SelectedValue = "" And col3.SelectedValue = "" Then
                        Select Case Me.CurrentX29ID
                            Case BO.x29IdEnum.p41Project, BO.x29IdEnum.p56Task
                                col1.SelectedValue = "j02"
                                col2.SelectedValue = "p32"
                            Case BO.x29IdEnum.p28Contact
                                col1.SelectedValue = "p41"
                                col2.SelectedValue = "j02"
                            Case BO.x29IdEnum.j02Person
                                col1.SelectedValue = "p28"
                                col2.SelectedValue = "p41"
                                col3.SelectedValue = "p34"
                        End Select
                    End If
                    Dim lisJ27 As List(Of String) = Split(.GetUserParam("entity_framework_detail_approving-j27ids", "2,3")).ToList
                    basUI.CheckItems(Me.j27ids, lisJ27)

                End With


            End With
            Select Case Request.Item("scope")
                Case "20" : cbxApprovingLevel.SelectedValue = "0"
                Case "21" : cbxApprovingLevel.SelectedValue = "1"
                Case "22" : cbxApprovingLevel.SelectedValue = "2"
                Case Else
                    basUI.SelectDropdownlistValue(Me.cbxApprovingLevel, Master.Factory.j03UserBL.GetUserParam("entity_framework_detail_approving-approving-level"))
            End Select

            RefreshRecord()


        End If
        
        
    End Sub

    Private Sub RefreshRecord()
        ViewState("can_create_invoice") = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P91_Creator, BO.x53PermValEnum.GR_P91_Draft_Creator)

        If Not Master.Factory.SysUser.IsApprovingPerson Then
            Handle_NoPermissions()
            Return
        End If
        Dim bolCanApprove As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver)

        Dim intFirstPID As Integer = Master.DataPID
        If Me.CurrentInputPIDs <> "" Then
            intFirstPID = BO.BAS.ConvertPIDs2List(Me.CurrentInputPIDs)(0)
        End If
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(intFirstPID)
                If cRec Is Nothing Then NoRec()
                Master.HeaderText = cRec.FullName
                Master.HeaderIcon = "Images/project_32.png"
                Dim bolCanCreateDraftInvoice As Boolean = False
                If Not bolCanApprove Then
                    Dim cDisp As BO.p41RecordDisposition = Master.Factory.p41ProjectBL.InhaleRecordDisposition(cRec)
                    bolCanCreateDraftInvoice = cDisp.p91_DraftCreate
                    If cDisp.x67IDs.Count > 0 Then
                        Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = Master.Factory.x67EntityRoleBL.GetList_o28(cDisp.x67IDs)
                        If lisO28.Where(Function(p) p.o28PermFlag = BO.o28PermFlagENUM.CistASchvalovatVProjektu Or p.o28PermFlag = BO.o28PermFlagENUM.CistAEditASchvalovatVProjektu).Count > 0 Then
                            bolCanApprove = True
                        End If
                    End If
                End If
                If Not ViewState("can_create_invoice") Then
                    ViewState("can_create_invoice") = bolCanCreateDraftInvoice
                End If
            Case BO.x29IdEnum.p28Contact
                Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(intFirstPID)
                If cRec Is Nothing Then NoRec()
                Master.HeaderText = cRec.p28Name
                Master.HeaderIcon = "Images/contact_32.png"
                If Not bolCanApprove Then
                    Dim mq As New BO.myQueryP41
                    mq.p28ID = cRec.PID
                    mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForApproving
                    Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
                    If lisP41.Count > 0 Then
                        bolCanApprove = True
                    End If
                End If
            Case BO.x29IdEnum.p56Task
                Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(intFirstPID)
                If cRec Is Nothing Then NoRec()
                Master.HeaderText = cRec.FullName
                Master.HeaderIcon = "Images/task_32.png"
            Case BO.x29IdEnum.j02Person
                Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(intFirstPID)
                If cRec Is Nothing Then NoRec()
                If Not cRec.j02IsIntraPerson Then
                    Master.StopPage("U kontaktních osob nelze aplikovat schvalování.", False)
                End If
                Master.HeaderText = cRec.FullNameAsc
                Master.HeaderIcon = "Images/person_32.png"
        End Select

        ''If ViewState("can_create_invoice") Then
        ''    tabs1.FindTabByValue("3").Visible = True
        ''Else
        ''    tabs1.FindTabByValue("3").Visible = False
        ''End If
        ''RadMultiPage1.FindPageViewByID("tri").Visible = tabs1.FindTabByValue("3").Visible

        If Not bolCanApprove Then
            Handle_NoPermissions()
        End If
    End Sub

    Private Sub Handle_NoPermissions()
        Master.StopPage("Nemáte oprávnění pro schvalování úkonů.")

    End Sub





    Private Sub NoRec()
        Master.StopPage("Record not found.")
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        If Me.hidHardRefreshFlag.Value = "" And Me.hidHardRefreshPID.Value = "" Then Return

        Select Case Me.hidHardRefreshFlag.Value

           


            Case Else
                ReloadPage()
        End Select
        Me.hidHardRefreshFlag.Value = ""
        Me.hidHardRefreshPID.Value = ""
    End Sub

    
    Private Sub ReloadPage()
        Response.Redirect("entity_modal_approving.aspx?prefix=" & Me.CurrentPrefix & "&pid=" & Master.DataPID.ToString & "&pids=" & Me.CurrentInputPIDs)
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-period", Me.period1.SelectedValue)

    End Sub

    Private Sub entity_framework_detail_approving_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        RefreshResult()

        If ViewState("can_create_invoice") Then
            With tabs1.Tabs
                If .Item(0).Text.IndexOf("(0)") > 0 And .Item(1).Text.IndexOf(">0") > 0 Then
                    .Item(1).Selected = True
                    RadMultiPage1.FindPageViewByID("dva").Selected = True
                End If
            End With
        End If
    End Sub



    Private Function InhaleSqlStructure(bolApproved As Boolean) As StructureSQL
        Dim lis As List(Of StatColumn) = GetSelectedColumns(bolApproved)
        If lis.Select(Function(p) p.Value).Count <> lis.Select(Function(p) p.Value).Distinct.Count Then
            Master.Notify("Agregační sloupec může být vybrán pouze jednou.", NotifyLevel.WarningMessage)
            Return Nothing
        End If
        If basUI.GetCheckedItems(j27ids).Count = 0 Then
            Master.Notify("Musíte zaškrtnout minimálně jednu měnu.", NotifyLevel.WarningMessage)
            Return Nothing
        Else
            Master.Factory.j03UserBL.SetUserParam("entity_framework_detail_approving-j27ids", String.Join(",", basUI.GetCheckedItems(Me.j27ids)))
        End If
        If lis.Count = 0 Then
            Master.Notify("Musíte vybrat minimálně jeden agregační sloupec.", NotifyLevel.WarningMessage)
            Return Nothing
        Else
            If Not bolApproved Then
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col1", col1.SelectedValue)
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col2", col2.SelectedValue)
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col3", col3.SelectedValue)
            Else
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col1x", col1x.SelectedValue)
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col2x", col2x.SelectedValue)
                Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix & "-entity_framework_detail_approving-col3x", col3x.SelectedValue)
            End If
            
        End If
        Dim strT As String = lis(0).StatColType, strH As String = lis(0).HeaderText, strF As String = lis(0).SelectField
        Dim strC As String = lis(0).StatColC, strG As String = lis(0).GroupField

        For i As Integer = 1 To lis.Count - 1
            strT += "|" & lis(i).StatColType
            strH += "|" & lis(i).HeaderText
            strF += "," & lis(i).SelectField
            strC += "|" & lis(i).StatColC
            strG += "," & lis(i).GroupField
        Next
        Dim strOF As String = strF
        Dim strTA As String = strT, strHA As String = strH, strCA As String = strC, strFA As String = strF
        strT += "|N" : strTA += "|N|N|N|N"
        strH += "|Hodiny" : strHA += "|Hodiny k fakturaci|Hodiny do paušálu|Hodiny k odpisu|Hodiny fakt.později"
        strC += "|11" : strCA += "|11|11|11|11"
        strF += ",sum(p31Hours_Orig)" : strFA += ",sum(case when a.p72ID_AfterApprove=4 then p31Hours_Approved_Billing end),sum(case when a.p72ID_AfterApprove=6 THEN p31Hours_Orig end),sum(case when a.p72ID_AfterApprove IN (2,3) THEN p31Hours_Orig end),sum(case when a.p72ID_AfterApprove=7 THEN p31Hours_Approved_Billing end)"

        For Each intJ27ID As Integer In basUI.GetCheckedItems(Me.j27ids)
            strT += "|N" : strTA += "|N"
            strH += "|Částka bez DPH [" & Me.j27ids.Items.FindByValue(intJ27ID.ToString).Text & "]"
            strHA += "|Částka k fakturaci [" & Me.j27ids.Items.FindByValue(intJ27ID.ToString).Text & "]"
            strC += "|11" : strCA += "|11"
            strF += ",sum(case when a.j27ID_Billing_Orig=" & intJ27ID.ToString & " THEN p31Amount_WithoutVat_Orig END)"
            strFA += ",sum(case when a.p72ID_AfterApprove=4 AND a.j27ID_Billing_Orig=" & intJ27ID.ToString & " THEN p31Amount_WithoutVat_Approved END)"

        Next
        strT += "|I" : strTA += "|I"
        strH += "|Počet úkonů" : strHA += "|Počet úkonů"
        strC += "|11" : strCA += "|11"
        strF += ",COUNT(p31ID)" : strFA += ",COUNT(p31ID)"

        Dim c As New StructureSQL
        c.stringT = strT
        c.stringH = strH
        c.stringC = strC
        c.stringF = strF
        c.stringOF = strOF
        c.stringG = strG
        If bolApproved Then
            c.stringF = strFA
            c.stringH = strHA
            c.stringC = strCA
            c.stringT = strTA
        End If

        Return c
    End Function


    Private Sub RefreshResult()
        Dim cS1 As StructureSQL = InhaleSqlStructure(False)
        Dim cS2 As StructureSQL = InhaleSqlStructure(True)
        If cS1 Is Nothing Or cS2 Is Nothing Then Return

        Dim pars As New List(Of BO.PluginDbParameter)
        Dim s As String = "SELECT " & cS1.stringF & " FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID LEFT OUTER JOIN p28contact p28 ON p41.p28ID_Client=p28.p28ID"
        s += " INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID"
        s += " WHERE " & GetSqlWhere(False, pars)
        s += " GROUP BY " & cS1.stringG
        s += " ORDER BY " & cS1.stringOF


        With plugin1
            .ColHeaders = cS1.stringH
            .ColFlexSubtotals = cS1.stringC
            .ColTypes = cS1.stringT
            For Each par In pars
                .AddDbParameter(par.Name, par.Value)
            Next
            .GenerateTable(Master.Factory, s)
        End With


        Dim sA As String = "SELECT " & cS2.stringF & " FROM p31Worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID LEFT OUTER JOIN p28contact p28 ON p41.p28ID_Client=p28.p28ID"
        sA += " INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        sA += " LEFT OUTER JOIN p56Task p56 ON a.p56ID=p56.p56ID"
        pars = New List(Of BO.PluginDbParameter)
        sA += " WHERE " & GetSqlWhere(True, pars)
        sA += " GROUP BY " & cS2.stringG
        sA += " ORDER BY " & cS2.stringOF

        With plugin2
            .ColHeaders = cS2.stringH
            .ColFlexSubtotals = cS2.stringC
            .ColTypes = cS2.stringT
            For Each par In pars
                .AddDbParameter(par.Name, par.Value)
            Next
            .GenerateTable(Master.Factory, sA)
            If .GeneratedRowsCount > 0 Then
                tlb2.FindItemByValue("cmdReApprove").Visible = True
                tlb2.Visible = True
            Else
                tlb2.FindItemByValue("cmdReApprove").Visible = False
                tlb2.Visible = False
            End If
            tlb2.FindItemByValue("cmdClearApprove").Visible = tlb2.FindItemByValue("cmdReApprove").Visible
            If .GeneratedRowsCount > 0 Then
                tlb2.FindItemByValue("cmdCreateP91").Visible = ViewState("can_create_invoice")
                tabs1.Tabs(1).Text = BO.BAS.OM2(tabs1.Tabs(1).Text, ">0")
            Else
                tlb2.FindItemByValue("cmdCreateP91").Visible = False
                tabs1.Tabs(1).Text = BO.BAS.OM2(tabs1.Tabs(1).Text, "0")
            End If
            Master.HideShowToolbarButton("continue_invoice", tlb2.FindItemByValue("cmdCreateP91").Visible)
            
        End With
        tlb2.FindItemByValue("cmdAppendP91").Visible = tlb2.FindItemByValue("cmdCreateP91").Visible
        
        
        Dim lis As List(Of StatColumn) = GetSelectedColumns(False)
        RenderDynamicCommands(lis)

    End Sub
    Private Function GetSqlWhere(bolApprovedQuery As Boolean, ByRef pars As List(Of BO.PluginDbParameter)) As String
        Dim s As String = ""
        If Not bolApprovedQuery Then
            s = "a.p71ID IS NULL AND a.p91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil"   'rozpracovanost
        Else
            s = "a.p71ID=1 AND a.p91ID IS NULL AND getdate() BETWEEN a.p31ValidFrom AND a.p31ValidUntil"         'už schválené
            If Me.cbxApprovingLevel.SelectedValue <> "" Then s += " AND a.p31ApprovingLevel=" & Me.cbxApprovingLevel.SelectedValue
        End If
        If Me.hidMasterAW.Value <> "" Then s += " AND " & Me.hidMasterAW.Value
        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                If Me.CurrentInputPIDs <> "" Then
                    s += " AND a.p41ID IN (" & Me.CurrentInputPIDs & ")"
                Else
                    s += " AND a.p41ID=@p41id"
                    pars.Add(New BO.PluginDbParameter("p41id", Master.DataPID))
                End If
                
            Case BO.x29IdEnum.j02Person
                If Me.CurrentInputPIDs <> "" Then
                    s += " AND a.j02ID IN (" & Me.CurrentInputPIDs & ")"
                Else
                    s += " AND a.j02ID=@j02id"
                    pars.Add(New BO.PluginDbParameter("j02id", Master.DataPID))
                End If
            Case BO.x29IdEnum.p28Contact
                If Me.CurrentInputPIDs <> "" Then
                    s += " AND p41.p28ID_Client IN (" & Me.CurrentInputPIDs & ")"
                Else
                    s += " AND p41.p28ID_Client=@p28id"
                    pars.Add(New BO.PluginDbParameter("p28id", Master.DataPID))
                End If
            Case BO.x29IdEnum.p56Task
                If Me.CurrentInputPIDs <> "" Then
                    s += " AND a.p56ID IN (" & Me.CurrentInputPIDs & ")"
                Else
                    s += " AND a.p56ID=@p56id"
                    pars.Add(New BO.PluginDbParameter("p56id", Master.DataPID))
                End If
        End Select
        If period1.SelectedValue <> "" Then
            s += " AND a.p31Date BETWEEN @d1 AND @d2"
            pars.Add(New BO.PluginDbParameter("d1", period1.DateFrom))
            pars.Add(New BO.PluginDbParameter("d2", period1.DateUntil))
        End If


        If Not bolApprovedQuery Then
            If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Approver) Then
                pars.Add(New BO.PluginDbParameter("j02id_query", Master.Factory.SysUser.j02ID))
                'nemá práva schvalovat veškerý worksheet v systému
                s += " AND a.p31ID IN ("
                s += "SELECT p31.p31ID FROM p31Worksheet p31 INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag IN (3,4) AND x67.x29ID=141 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query))) scope ON p31.p41ID=scope.x69RecordPID AND p32.p34ID=scope.p34ID"
                s += ")"
            End If
            's += " AND getdate() BETWEEN p41.p41ValidFrom AND p41.p41ValidUntil"
        Else
            If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Reader) Then
                pars.Add(New BO.PluginDbParameter("j02id_query", Master.Factory.SysUser.j02ID))
                'ošetřit čtecí práva
                s += " AND a.p31ID IN ("
                s += "SELECT p31.p31ID FROM p31Worksheet p31 INNER JOIN p32Activity p32 ON p31.p32ID=p32.p32ID INNER JOIN (SELECT x69.x69RecordPID,o28.p34ID FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID INNER JOIN o28ProjectRole_Workload o28 ON x67.x67ID=o28.x67ID WHERE o28.o28PermFlag>0 AND x67.x29ID=141 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query))) scope ON p31.p41ID=scope.x69RecordPID AND p32.p34ID=scope.p34ID"
                s += ")"
            End If
        End If
        Return s
    End Function

    Private Function GetSelectedColumns(bolApproved As Boolean) As List(Of StatColumn)
        Dim lis As New List(Of StatColumn), x As Integer = 0
        Dim cols As New List(Of String)
        If Not bolApproved Then
            If col1.SelectedValue <> "" Then cols.Add(col1.SelectedValue)
            If col2.SelectedValue <> "" Then cols.Add(col2.SelectedValue)
            If col3.SelectedValue <> "" Then cols.Add(col3.SelectedValue)
        Else
            If col1x.SelectedValue <> "" Then cols.Add(col1x.SelectedValue)
            If col2x.SelectedValue <> "" Then cols.Add(col2x.SelectedValue)
            If col3x.SelectedValue <> "" Then cols.Add(col3x.SelectedValue)
        End If
        
        For Each s In cols
            Dim c As New StatColumn
            c.Value = s
            c.SelectField = "min(" & GetSelectField(s) & ")"

            c.GroupField = GetGroupByField(s)
            If Not bolApproved Then
                c.HeaderText = col1.Items.FindByValue(s).Text
            Else
                c.HeaderText = col1x.Items.FindByValue(s).Text
            End If

            c.OrderByField = c.SelectField
            If x < cols.Count - 1 Then
                c.StatColC = "1"
            End If
            lis.Add(c)
            x += 1
        Next
        Return lis
    End Function

    Private Function GetSelectField(strPrefix As String) As String
        Select Case strPrefix
            Case "p28" : Return "p28Name"
            Case "p41" : Return "isnull(p41NameShort,p41Name)"
            Case "j02" : Return "j02LastName+' '+j02Firstname"
            Case "p56" : Return "p56Name+' ('+isnull(p56Code,'')+')'"
            Case "p34" : Return "p34Name"
            Case "p32" : Return "p32Name"
            Case "month" : Return "convert(varchar(7), p31Date, 126)"
            Case "day" : Return "convert(varchar(10), p31Date, 111)"
            Case "p31ApprovingSet" : Return "p31ApprovingSet"
            Case Else : Return ""
        End Select
    End Function
    Private Function GetGroupByField(strPrefix As String) As String
        Select Case strPrefix
            Case "p28" : Return "p41.p28ID_Client"
            Case "p41" : Return "a.p41ID"
            Case "j02" : Return "a.j02ID"
            Case "p56" : Return "a.p56ID"
            Case "p34" : Return "p32.p34ID"
            Case "p32" : Return "a.p32ID"
            Case "month" : Return "convert(varchar(7), p31Date, 126)"
            Case "day" : Return "convert(varchar(10), p31Date, 111)"
            Case "p31ApprovingSet" : Return "a.p31ApprovingSet"
            Case Else : Return ""
        End Select
    End Function

    

   

    Private Sub RenderDynamicCommands(cols As List(Of StatColumn))
        Dim mq As New BO.myQueryP31
        mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForDoApprove
        If period1.SelectedValue <> "" Then
            mq.DateFrom = period1.DateFrom
            mq.DateUntil = period1.DateUntil
        End If
        mq.MG_AdditionalSqlWHERE = Me.hidMasterAW.Value

        Select Case Me.CurrentX29ID
            Case BO.x29IdEnum.p41Project
                mq.p41ID = Master.DataPID
                If Me.CurrentInputPIDs <> "" Then mq.p41IDs = BO.BAS.ConvertPIDs2List(Me.CurrentInputPIDs)
            Case BO.x29IdEnum.j02Person
                mq.j02ID = Master.DataPID
                If Me.CurrentInputPIDs <> "" Then mq.j02IDs = BO.BAS.ConvertPIDs2List(Me.CurrentInputPIDs)
            Case BO.x29IdEnum.p28Contact
                mq.p28ID_Client = Master.DataPID
                If Me.CurrentInputPIDs <> "" Then mq.p28IDs_Client = BO.BAS.ConvertPIDs2List(Me.CurrentInputPIDs)
            Case BO.x29IdEnum.p56Task
                If Me.CurrentInputPIDs <> "" Then
                    mq.p56IDs = BO.BAS.ConvertPIDs2List(Me.CurrentInputPIDs)
                Else
                    mq.p56IDs = New List(Of Integer)
                    mq.p56IDs.Add(Master.DataPID)
                End If

        End Select
        For i As Integer = 1 To tlb1.Items.Count - 1
            tlb1.Items.RemoveAt(1)
        Next


        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        'Dim qry = From p In lisP31 Select p.j02ID, p.Person Distinct

        'Dim qry2 = From p In lisP31 Group By p.j02ID Into Hovado = Group, Count()

        Dim lis As New List(Of CommandRow)

        If Me.chkCommandsJ02.Checked And Me.chkCommandsJ02.Visible Then
            'tlačítka za osoby
            For Each sada In lisP31.GroupBy(Function(p) p.j02ID)
                Dim cmd As New Telerik.Web.UI.RadToolBarButton(String.Format("{0}<span class='badge1'>{1}x</span>", sada(0).Person, sada.Count))
                cmd.NavigateUrl = "javascript:approve_j02(" & sada(0).j02ID.ToString & ")"
                cmd.Value = sada(0).j02ID.ToString
                cmd.ImageUrl = "Images/approve.png"
                tlb1.Items.Add(cmd)
            Next
        End If

        'If Me.CurrentX29ID <> BO.x29IdEnum.j02Person Then

        'Else

        '    'tlačítka za klienty
        '    For Each sada In lisP31.GroupBy(Function(p) p.p28ID_Client)
        '        lis.Add(New CommandRow(sada(0).p28ID_Client, sada(0).p28Name, sada.Count))
        '    Next
        '    If lis.Count > 1 Then
        '        rpCommandP28.DataSource = lis
        '        rpCommandP28.DataBind()
        '    End If
        'End If

        If Me.chkCommandsP34.Checked And Me.chkCommandsP34.Visible Then
            For Each sada In lisP31.GroupBy(Function(p) p.p34ID)
                Dim cmd As New Telerik.Web.UI.RadToolBarButton(String.Format("{0} <span class='badge1'>{1}x</span>", sada(0).p34Name, sada.Count))
                cmd.NavigateUrl = "javascript:approve_p34(" & sada(0).p34ID.ToString & ")"
                cmd.Value = sada(0).j02ID.ToString
                cmd.ImageUrl = "Images/approve.png"
                tlb1.Items.Add(cmd)
            Next
           
        End If

        If lisP31.Count > 0 Then
            tlb1.Visible = True
            Master.HideShowToolbarButton("continue", True)
            Master.RenameToolbarButton("continue", String.Format("Přejít do schvalování rozpracovanosti {0}", lisP31.Count.ToString & "x"))
        Else
            tlb1.Visible = False
            Master.HideShowToolbarButton("continue", False)
        End If
        tabs1.Tabs(0).Text = BO.BAS.OM2(tabs1.Tabs(0).Text, lisP31.Count.ToString)


    End Sub


    Private Sub chkCommandsJ02_CheckedChanged(sender As Object, e As EventArgs) Handles chkCommandsJ02.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_framework_detail_approving-chkCommandsJ02", BO.BAS.GB(Me.chkCommandsJ02.Checked))

    End Sub

    Private Sub chkCommandsP34_CheckedChanged(sender As Object, e As EventArgs) Handles chkCommandsP34.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_framework_detail_approving-chkCommandsP34", BO.BAS.GB(Me.chkCommandsP34.Checked))

    End Sub

    Private Sub chkCommandsP41_CheckedChanged(sender As Object, e As EventArgs) Handles chkCommandsP41.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("entity_framework_detail_approving-chkCommandsP41", BO.BAS.GB(Me.chkCommandsP41.Checked))

    End Sub

    
   
  
    

    Private Sub cbxApprovingLevel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxApprovingLevel.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_framework_detail_approving-approving-level", Me.cbxApprovingLevel.SelectedValue)
    End Sub
End Class