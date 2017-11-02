Public Class b06_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return CType(Me.hidcurx29id.Value, BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidcurx29id.Value = CInt(value).ToString
        End Set
    End Property
    Public ReadOnly Property CurrentB01ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidB01ID.Value)
        End Get
    End Property
    Private Sub b06_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid_b11") = BO.BAS.GetGUID()
            ViewState("guid_b10") = BO.BAS.GetGUID()
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                '.HeaderIcon = "Images/workflow_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Workflow krok"
            End With
            ViewState("b02id") = Request.Item("b02id")

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
            If BO.BAS.IsNullInt(ViewState("b02id")) = 0 Then Master.StopPage("b02id is missing!")

           
        End If

        RefreshState()
    End Sub
    Private Sub Handle_InhaleB02(cB02 As BO.b02WorkflowStatus)
        hidB01ID.Value = cB02.b01ID.ToString
        hidcurx29id.Value = CInt(cB02.x29ID).ToString
    End Sub

    Private Sub RefreshState()

        
        Dim b As Boolean = b06IsManualStep.Checked
        


        b06IsCommentRequired.Visible = b
        RadTabStrip1.Tabs(1).Visible = b
        RadPageView2.Visible = b

    End Sub

    Private Sub RefreshRecord()
        Dim myQuery As New BO.myQuery
        If ViewState("b02id") = "" And Master.DataPID <> 0 Then
            Dim c As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(Master.DataPID)
            ViewState("b02id") = c.b02ID.ToString
        End If
        Dim cB02 As BO.b02WorkflowStatus = Master.Factory.b02WorkflowStatusBL.Load(ViewState("b02id"))
        Handle_InhaleB02(cB02)

        b02ID_Target.DataSource = Master.Factory.b02WorkflowStatusBL.GetList(cB02.b01ID).Where(Function(p As BO.b02WorkflowStatus) p.PID <> BO.BAS.IsNullInt(ViewState("b02id")))
        b02ID_Target.DataBind()
        b02ID_Target.ChangeItemText("", "--Krok bez změny stavu--")

        b02ID_LastReceiver_ReturnTo.DataSource = Master.Factory.b02WorkflowStatusBL.GetList(cB02.b01ID)
        b02ID_LastReceiver_ReturnTo.DataBind()
        b02ID_LastReceiver_ReturnTo.Items.Insert(0, "")
        
        cbxAddB09ID.DataSource = Master.Factory.b06WorkflowStepBL.GetList_Allb09IDs().Where(Function(p) p.x29ID = Me.CurrentX29ID)
        cbxAddB09ID.DataBind()
        cbxAddB09ID.Items.Insert(0, "--Vyberte příkaz--")


        Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project Or p.x29ID = Me.CurrentX29ID).OrderBy(Function(p) p.x29ID)
        If Me.CurrentX29ID = BO.x29IdEnum.o23Doc Then
            lisX67 = lisX67.Where(Function(p) p.x29ID = Me.CurrentX29ID)    'pro dokumenty nenabízet projektové role
        End If
        chklX67IDs_B08.DataSource = lisX67
        chklX67IDs_B08.DataBind()
        chklX67IDs_B08.Items.Add(New ListItem("Vlastník záznamu", "b08IsRecordOwner"))
        chklX67IDs_B08.Items.Add(New ListItem("Zakladatel záznamu", "b08IsRecordCreator"))
        chklJ11IDs_B08.DataSource = Master.Factory.j11TeamBL.GetList().Where(Function(p) p.j11IsAllPersons = False)
        chklJ11IDs_B08.DataBind()
        chklJ04IDs_B08.DataSource = Master.Factory.j04UserRoleBL.GetList()
        chklJ04IDs_B08.DataBind()

        Me.x67ID_Nominee.DataSource = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
        Me.x67ID_Nominee.DataBind()
        Me.x67ID_Nominee.Items.Insert(0, "")
        Me.x67ID_Direct.DataSource = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
        Me.x67ID_Direct.DataBind()
        Me.x67ID_Direct.Items.Insert(0, "--Krok bez změny řešitele--")
        Me.j11ID_Direct.DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
        Me.j11ID_Direct.DataBind()
        Me.j11ID_Direct.Items.Insert(0, "")
        If Master.DataPID = 0 Then
           
            Return
        End If

        Dim cRec As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(Master.DataPID)
        With cRec
            b02ID_Target.SelectedValue = .b02ID_Target.ToString
            b06Name.Text = .b06Name
            b06Ordinary.Value = .b06Ordinary
            b06IsManualStep.Checked = .b06IsManualStep
            b06IsCommentRequired.Checked = .b06IsCommentRequired
            b06RunSQL.Text = .b06RunSQL
            b06ValidateAutoMoveSQL.Text = .b06ValidateAutoMoveSQL
            b06ValidateBeforeErrorMessage.Text = .b06ValidateBeforeErrorMessage
            b06ValidateBeforeRunSQL.Text = .b06ValidateBeforeRunSQL
            Me.b06IsKickOffStep.Checked = .b06IsKickOffStep

            Me.b06IsRunOneInstanceOnly.Checked = .b06IsRunOneInstanceOnly
            Me.b06IsNominee.Checked = .b06IsNominee
            Me.b06IsNomineeRequired.Checked = .b06IsNomineeRequired
            basUI.SelectDropdownlistValue(Me.x67ID_Nominee, .x67ID_Nominee.ToString)
            basUI.SelectDropdownlistValue(Me.x67ID_Direct, .x67ID_Direct.ToString)
            basUI.SelectDropdownlistValue(Me.j11ID_Direct, .j11ID_Direct.ToString)
            basUI.SelectDropdownlistValue(Me.b02ID_LastReceiver_ReturnTo, .b02ID_LastReceiver_ReturnTo.ToString)
            If Me.x67ID_Direct.SelectedIndex > 0 Or Me.j11ID_Direct.SelectedIndex > 0 Then
                Me.chkDirectNominee.Checked = True
            Else
                Me.chkDirectNominee.Checked = False
            End If

            basUI.SelectDropdownlistValue(Me.b06NomineeFlag, CInt(.b06NomineeFlag).ToString)

            Master.Timestamp = .Timestamp
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)


        End With
        Dim lisB08 As List(Of BO.b08WorkflowReceiverToStep) = Master.Factory.b06WorkflowStepBL.GetList_B08(Master.DataPID).ToList
        basUI.CheckItems(Me.chklJ04IDs_B08, lisB08.Where(Function(p) p.j04ID <> 0).Select(Function(p) p.j04ID).ToList)
        basUI.CheckItems(Me.chklJ11IDs_B08, lisB08.Where(Function(p) p.j11ID <> 0).Select(Function(p) p.j11ID).ToList)
        basUI.CheckItems(Me.chklX67IDs_B08, lisB08.Where(Function(p) p.x67ID <> 0).Select(Function(p) p.x67ID).ToList)
        If lisB08.Where(Function(p) p.b08IsRecordOwner).Count > 0 Then
            Me.chklX67IDs_B08.Items.FindByValue("b08IsRecordOwner").Selected = True
        End If
        If lisB08.Where(Function(p) p.b08IsRecordCreator).Count > 0 Then
            Me.chklX67IDs_B08.Items.FindByValue("b08IsRecordCreator").Selected = True
        End If
       

        Dim lisB11 As List(Of BO.b11WorkflowMessageToStep) = Master.Factory.b06WorkflowStepBL.GetList_B11(Master.DataPID).ToList
        For Each c In lisB11
            Dim cTMP As New BO.p85TempBox
            cTMP.p85GUID = ViewState("guid_b11")
            cTMP.p85OtherKey1 = c.x67ID
            cTMP.p85OtherKey2 = c.b65ID
            cTMP.p85OtherKey3 = c.j04ID
            cTMP.p85OtherKey4 = c.j11ID
            cTMP.p85DataPID = c.b11ID
            cTMP.p85FreeBoolean01 = c.b11IsRecordOwner
            cTMP.p85FreeBoolean02 = c.b11IsRecordCreator
            cTMP.p85FreeBoolean03 = c.b11IsRecordCreatorByEmail
            Master.Factory.p85TempBoxBL.Save(cTMP)
        Next
        RefreshTempListB11()
       

        Dim lisB10 As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding) = Master.Factory.b06WorkflowStepBL.GetList_B10(Master.DataPID)
        For Each c In lisB10
            Dim cTMP As New BO.p85TempBox
            With cTMP
                .p85GUID = ViewState("guid_b10")
                .p85OtherKey1 = c.b09ID
                .p85FreeText01 = c.b09Name
                If c.p31ID_Template <> 0 Then
                    .p85Prefix = "p31"
                    .p85OtherKey2 = c.p31ID_Template
                    .p85OtherKey3 = c.b10Worksheet_p72ID
                    .p85FreeNumber01 = CInt(c.b10Worksheet_ProjectFlag)
                    .p85FreeNumber02 = CInt(c.b10Worksheet_PersonFlag)
                    .p85FreeNumber03 = CInt(c.b10Worksheet_DateFlag)
                    .p85FreeText01 = c.b10Worksheet_Text
                    .p85OtherKey4 = CInt(c.b10Worksheet_HoursFlag)
                End If
                If c.x18ID <> 0 Then
                    .p85Prefix = "o23"
                    .p85OtherKey2 = c.x18ID
                    .p85FreeText02 = c.o23Name
                End If

            End With
            


            cTMP.p85DataPID = c.b10ID
            Master.Factory.p85TempBoxBL.Save(cTMP)
        Next
        RefreshTempListB10()

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.b06WorkflowStepBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("b06-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()

    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.b06WorkflowStepBL
            Dim cRec As BO.b06WorkflowStep = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.b06WorkflowStep)
            With cRec
                .b06Name = b06Name.Text
                .b06IsCommentRequired = b06IsCommentRequired.Checked
                .b06Ordinary = BO.BAS.IsNullInt(Me.b06Ordinary.Value)
                .b06IsManualStep = b06IsManualStep.Checked
                .b06IsKickOffStep = Me.b06IsKickOffStep.Checked
                .b06IsNominee = Me.b06IsNominee.Checked
                .b06IsNomineeRequired = Me.b06IsNomineeRequired.Checked
                .x67ID_Nominee = BO.BAS.IsNullInt(Me.x67ID_Nominee.SelectedValue)
                .b06NomineeFlag = BO.BAS.IsNullInt(Me.b06NomineeFlag.SelectedValue)
                If Me.chkDirectNominee.Checked Then
                    .x67ID_Direct = BO.BAS.IsNullInt(Me.x67ID_Direct.SelectedValue)
                    .j11ID_Direct = BO.BAS.IsNullInt(Me.j11ID_Direct.SelectedValue)
                Else
                    .x67ID_Direct = 0 : .j11ID_Direct = 0
                End If
                .b02ID_LastReceiver_ReturnTo = BO.BAS.IsNullInt(Me.b02ID_LastReceiver_ReturnTo.SelectedValue)

                .b06RunSQL = b06RunSQL.Text
                .b06ValidateAutoMoveSQL = b06ValidateAutoMoveSQL.Text
                .b06ValidateBeforeErrorMessage = b06ValidateBeforeErrorMessage.Text
                .b06ValidateBeforeRunSQL = b06ValidateBeforeRunSQL.Text



                If Master.DataPID = 0 Then
                    .b02ID = BO.BAS.IsNullInt(ViewState("b02id"))
                End If
                .b02ID_Target = BO.BAS.IsNullInt(b02ID_Target.SelectedValue)

                .b06IsRunOneInstanceOnly = Me.b06IsRunOneInstanceOnly.Checked


                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            Dim lisB08 As New List(Of BO.b08WorkflowReceiverToStep)
            For Each li As ListItem In chklJ04IDs_B08.Items
                If li.Selected Then
                    Dim c As New BO.b08WorkflowReceiverToStep
                    c.j04ID = CInt(li.Value)
                    lisB08.Add(c)
                End If
            Next
            For Each li As ListItem In chklJ11IDs_B08.Items
                If li.Selected Then
                    Dim c As New BO.b08WorkflowReceiverToStep
                    c.j11ID = CInt(li.Value)
                    lisB08.Add(c)
                End If
            Next
            For Each li As ListItem In chklX67IDs_B08.Items
                If li.Selected Then
                    Dim c As New BO.b08WorkflowReceiverToStep
                    Select Case li.Value
                        Case "b08IsRecordOwner"
                            c.b08IsRecordOwner = True
                        Case "b08IsRecordCreator"
                            c.b08IsRecordCreator = True
                        Case Else
                            c.x67ID = CInt(li.Value)
                    End Select
                    lisB08.Add(c)
                End If
            Next

            Dim lisB11 As New List(Of BO.b11WorkflowMessageToStep)
            For Each ri As RepeaterItem In rpB11.Items
                Dim c As New BO.b11WorkflowMessageToStep
                Select Case CType(ri.FindControl("x67ID"), DropDownList).SelectedValue
                    Case "b11IsRecordOwner"
                        c.b11IsRecordOwner = True
                    Case "b11IsRecordCreator"
                        c.b11IsRecordCreator = True
                    Case "b11IsRecordCreatorByEmail"
                        c.b11IsRecordCreatorByEmail = True
                    Case Else
                        c.x67ID = BO.BAS.IsNullInt(CType(ri.FindControl("x67ID"), DropDownList).SelectedValue)
                End Select
                c.j04ID = BO.BAS.IsNullInt(CType(ri.FindControl("j04id"), DropDownList).SelectedValue)
                c.j11ID = BO.BAS.IsNullInt(CType(ri.FindControl("j11id"), DropDownList).SelectedValue)
                c.b65ID = BO.BAS.IsNullInt(CType(ri.FindControl("b65id"), DropDownList).SelectedValue)
                lisB11.Add(c)
            Next



            Dim lisB10 As New List(Of BO.b10WorkflowCommandCatalog_Binding)
            For Each cTMP In Master.Factory.p85TempBoxBL.GetList(ViewState("guid_b10"))
                Dim c As New BO.b10WorkflowCommandCatalog_Binding
                c.b09ID = cTMP.p85OtherKey1
                If cTMP.p85Prefix = "p31" Then
                    c.p31ID_Template = cTMP.p85OtherKey2
                    c.b10Worksheet_ProjectFlag = CInt(cTMP.p85FreeNumber01)
                    c.b10Worksheet_PersonFlag = CInt(cTMP.p85FreeNumber02)
                    c.b10Worksheet_DateFlag = CInt(cTMP.p85FreeNumber03)
                    c.b10Worksheet_p72ID = cTMP.p85OtherKey3
                    c.b10Worksheet_Text = cTMP.p85FreeText01
                    c.b10Worksheet_HoursFlag = CInt(cTMP.p85OtherKey4)
                End If
                If cTMP.p85Prefix = "o23" Then
                    c.x18ID = cTMP.p85OtherKey2
                    c.o23Name = cTMP.p85FreeText02
                End If
                lisB10.Add(c)
            Next

            If .Save(cRec, lisB08, lisB11, lisB10) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("b06-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub b06IsManualStep_CheckedChanged(sender As Object, e As System.EventArgs) Handles b06IsManualStep.CheckedChanged
        RefreshState()
    End Sub

    
    Private Sub cmdNewB11_Click(sender As Object, e As System.EventArgs) Handles cmdNewB11.Click
        SaveB11Tempbox()
        Dim cRec As New BO.p85TempBox()
        With cRec
            .p85GUID = ViewState("guid_b11")
        End With
        If Master.Factory.p85TempBoxBL.Save(cRec) Then
            SaveB11Tempbox()
            RefreshTempListB11()
        End If
    End Sub
    Private Sub RefreshTempListB11()
        rpB11.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_b11"))
        rpB11.DataBind()

    End Sub
    Private Sub SaveB11Tempbox()
        For Each ri As RepeaterItem In rpB11.Items
            Dim strP85ID As String = CType(ri.FindControl("p85id"), HiddenField).Value
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(strP85ID))
            Select Case CType(ri.FindControl("x67id"), DropDownList).SelectedValue
                Case "b11IsRecordOwner"
                    cRec.p85FreeBoolean01 = True
                Case "b11IsRecordCreator"
                    cRec.p85FreeBoolean02 = True
                Case "b11IsRecordCreatorByEmail"
                    cRec.p85FreeBoolean03 = True
                Case Else
                    cRec.p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("x67id"), DropDownList).SelectedValue)
            End Select
            cRec.p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("b65id"), DropDownList).SelectedValue)
            cRec.p85OtherKey3 = BO.BAS.IsNullInt(CType(ri.FindControl("j04id"), DropDownList).SelectedValue)
            cRec.p85OtherKey4 = BO.BAS.IsNullInt(CType(ri.FindControl("j11id"), DropDownList).SelectedValue)
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub rpB11_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rpB11.ItemCommand
        Dim strP85ID As String = CType(e.Item.FindControl("p85id"), HiddenField).Value

        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(strP85ID))
        If Master.Factory.p85TempBoxBL.Delete(cRec) Then
            Saveb11Tempbox()
            RefreshTempListb11()
        End If
    End Sub

    Private Sub rpB11_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpB11.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            Dim lis1 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project Or p.x29ID = Me.CurrentX29ID).OrderBy(Function(p) p.x29ID)
            With CType(e.Item.FindControl("x67id"), DropDownList)
                .DataSource = lis1
                .DataBind()
                .Items.Add(New ListItem("Vlastník záznamu", "b11IsRecordOwner"))
                .Items.Add(New ListItem("Zakladatel záznamu", "b11IsRecordCreator"))
                If Me.CurrentX29ID = BO.x29IdEnum.p56Task Then
                    .Items.Add(New ListItem("Odpověď na e-mail zakladateli úkolu", "b11IsRecordCreatorByEmail"))
                End If
                If Me.CurrentX29ID = BO.x29IdEnum.o23Doc Then
                    .Items.Add(New ListItem("Odpověď na e-mail zakladateli dokumentu", "b11IsRecordCreatorByEmail"))
                End If
                .Items.Insert(0, "---Vyberte kontextovou roli---")
                If cRec.p85FreeBoolean01 Then
                    .Items.FindByValue("b11IsRecordOwner").Selected = True
                End If
                If cRec.p85FreeBoolean02 Then
                    .Items.FindByValue("b11IsRecordCreator").Selected = True
                End If
                If cRec.p85FreeBoolean03 Then
                    .Items.FindByValue("b11IsRecordCreatorByEmail").Selected = True
                End If
                Try
                    If Not cRec.p85FreeBoolean01 Or cRec.p85FreeBoolean02 Then
                        .SelectedValue = cRec.p85OtherKey1.ToString
                    End If
                Catch ex As Exception
                End Try
            End With

            Dim mq As New BO.myQuery
            Dim lis2 As IEnumerable(Of BO.b65WorkflowMessage) = Master.Factory.b65WorkflowMessageBL.GetList(Me.CurrentB01ID, New BO.myQuery)
            With CType(e.Item.FindControl("b65id"), DropDownList)
                .DataSource = lis2
                .DataBind()
                .Items.Insert(0, "---Vyberte šablonu notifikační zprávy---")
                Try
                    .SelectedValue = cRec.p85OtherKey2.ToString
                Catch ex As Exception
                End Try
            End With
            mq = New BO.myQuery
            Dim lis3 As IEnumerable(Of BO.j04UserRole) = Master.Factory.j04UserRoleBL.GetList(mq)
            With CType(e.Item.FindControl("j04id"), DropDownList)
                .DataSource = lis3
                .DataBind()
                .Items.Insert(0, "---Vyberte aplikační roli---")
                Try
                    .SelectedValue = cRec.p85OtherKey3.ToString
                Catch ex As Exception
                End Try
            End With
            mq = New BO.myQuery
            Dim lis4 As IEnumerable(Of BO.j11Team) = Master.Factory.j11TeamBL.GetList(mq).Where(Function(p) p.j11IsAllPersons = False)
            With CType(e.Item.FindControl("j11id"), DropDownList)
                .DataSource = lis4
                .DataBind()
                .Items.Insert(0, "---Vyberte tým osob---")
                Try
                    .SelectedValue = cRec.p85OtherKey4.ToString
                Catch ex As Exception
                End Try
            End With
        End With
    End Sub

    

    
    

    Private Sub RefreshTempListB10()
        rpB10.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_b10"))
        rpB10.DataBind()

    End Sub

   
   

    Private Sub b06_record_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        If Me.cbxAddB09ID.SelectedValue <> "" Then
            Me.cmdAddB09.Visible = True
        Else
            Me.cmdAddB09.Visible = False
        End If
        Me.b06IsRunOneInstanceOnly.Visible = Not Me.b06IsManualStep.Checked
        Me.panNominee.Visible = Me.b06IsNominee.Checked
        Me.panDirectNominee.Visible = Me.chkDirectNominee.Checked
    End Sub

    Private Sub rpB10_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rpB10.ItemCommand
        Dim strP85ID As String = CType(e.Item.FindControl("p85id"), HiddenField).Value
        If e.CommandName = "delete" Then
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(strP85ID))
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempListB10()
            End If
        
        End If
        
    End Sub

    Private Sub rpB10_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpB10.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            CType(e.Item.FindControl("b09id"), HiddenField).Value = .p85OtherKey1.ToString
            CType(e.Item.FindControl("b09Name"), Label).Text = .p85FreeText01
            If cRec.p85Prefix = "p31" Then
                'vzorový worksheet záznam
                With CType(e.Item.FindControl("WorksheetTemplate"), Label)
                    .Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p31Worksheet, cRec.p85OtherKey2, False)
                    Select Case CType(cRec.p85FreeNumber01, BO.b10Worksheet_ProjectENUM)
                        Case BO.b10Worksheet_ProjectENUM.ProjectInTemplate
                            .Text += "<br>Projekt úkonu převzít ze vzoru."
                        Case BO.b10Worksheet_ProjectENUM.WorkflowContext
                            .Text += "<br>Projekt úkonu odvodit z workflow záznamu."
                    End Select
                    Select Case CType(cRec.p85FreeNumber02, BO.b10Worksheet_PersonENUM)
                        Case BO.b10Worksheet_PersonENUM.PersonInTemplate
                            .Text += "<br>Osobu úkonu převzít ze vzoru."
                        Case BO.b10Worksheet_PersonENUM.WorkflowContext
                            .Text += "<br>Osobu úkonu odvodit z workflow záznamu."
                        Case BO.b10Worksheet_PersonENUM.WorkflowCreator
                            .Text += "<br>Osoba úkonu bude autor workflow záznamu."
                    End Select
                    Select Case CType(cRec.p85FreeNumber03, BO.b10Worksheet_DateENUM)
                        Case BO.b10Worksheet_DateENUM.DateInTemplate
                            .Text += "<br>Datum úkonu převzít ze vzoru."
                        Case BO.b10Worksheet_DateENUM.DateContext
                            .Text += "<br>Datum úkonu odvodit z workflow záznamu."
                        Case BO.b10Worksheet_DateENUM.Today
                            .Text += "<br>Datum úkonu bude TODAY."
                    End Select
                    Select Case CType(cRec.p85OtherKey4, BO.b10Worksheet_HoursENUM)
                        Case BO.b10Worksheet_HoursENUM.HoursInTemplate
                            .Text += "<br>Hodiny nebo peníze úkonu přesně podle vzoru"
                        Case BO.b10Worksheet_HoursENUM.HoursPerFund
                            .Text += "<br>Hodiny odvodit podle pracovního kalendáře (pouze pro pracovní dny)"
                    End Select
                    If cRec.p85FreeText01 <> "" Then
                        .Text += "<br>Text úkonu bude: <i>" & cRec.p85FreeText01 & "</i></b>"
                    End If
                    If cRec.p85OtherKey3 <> 0 Then
                        .Text += "<br>Schválit statusem: " & Master.Factory.ftBL.GetList_P72().Where(Function(p) p.PID = cRec.p85OtherKey3)(0).p72Name
                    End If
                End With

            End If
            If cRec.p85Prefix = "o23" Then
                With CType(e.Item.FindControl("DocumentTemplate"), Label)
                    .Text = "Typ dokumentu: "
                    If cRec.p85OtherKey2 <> 0 Then .Text += Master.Factory.x18EntityCategoryBL.Load(cRec.p85OtherKey2).x18Name
                    .Text += ", název: " & cRec.p85FreeText02
                End With
                
            End If
            

        End With
    End Sub

   
    Private Sub cbxAddB09ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAddB09ID.SelectedIndexChanged
        panWorksheetTemplate.Visible = False : panDoc.Visible = False

        Dim intB09ID As Integer = BO.BAS.IsNullInt(Me.cbxAddB09ID.SelectedValue)
        If intB09ID = 0 Then
            Return
        End If

        Dim cRec As BO.b09WorkflowCommandCatalog = Master.Factory.b06WorkflowStepBL.GetList_Allb09IDs().Where(Function(p) p.b09ID = intB09ID)(0)
        Select Case cRec.b09Code
            Case "p31_create"
                panWorksheetTemplate.Visible = True
                'Select Case Me.CurrentX29ID
                '    Case BO.x29IdEnum.p56Task
                '        b10Worksheet_ProjectFlag.Items.FindByValue("2").Text=
                'End Select
            Case "o23_create"
                If Me.x18ID.Items.Count = 0 Then
                    Me.x18ID.DataSource = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery, Me.CurrentX29ID)
                    Me.x18ID.DataBind()
                End If
                panDoc.Visible = True
        End Select
    End Sub
    Private Sub cmdAddB09_Click(sender As Object, e As System.EventArgs) Handles cmdAddB09.Click
        Dim intB09ID As Integer = BO.BAS.IsNullInt(Me.cbxAddB09ID.SelectedValue)
        If intB09ID = 0 Then
            Master.Notify("Musíte vybrat příkaz z nabídky.", 2) : Return
        End If
        If panWorksheetTemplate.Visible And BO.BAS.IsNullInt(Me.p31ID_Template.SelectedValue) = 0 Then
            Master.Notify("Musíte vybrat vzorový worksheet záznam.")
            Return
        End If
        If panDoc.Visible And Me.x18ID.SelectedValue = "" Then
            Master.Notify("Musíte vybrat typ dokumentu.")
            Return
        End If
        Dim cRec As BO.b09WorkflowCommandCatalog = Master.Factory.b06WorkflowStepBL.GetList_Allb09IDs().Where(Function(p) p.b09ID = intB09ID)(0)

        Dim cTMP As New BO.p85TempBox
        cTMP.p85GUID = ViewState("guid_b10")
        cTMP.p85OtherKey1 = intB09ID
        cTMP.p85FreeText01 = cRec.b09Name
        If panWorksheetTemplate.Visible Then
            cTMP.p85Prefix = "p31"
            cTMP.p85OtherKey2 = BO.BAS.IsNullInt(Me.p31ID_Template.SelectedValue)
            cTMP.p85FreeNumber01 = CInt(b10Worksheet_ProjectFlag.SelectedValue)
            cTMP.p85FreeNumber02 = CInt(b10Worksheet_PersonFlag.SelectedValue)
            cTMP.p85FreeNumber03 = CInt(b10Worksheet_DateFlag.SelectedValue)
            cTMP.p85OtherKey3 = BO.BAS.IsNullInt(Me.b10Worksheet_p72ID.SelectedValue)
            cTMP.p85OtherKey4 = BO.BAS.IsNullInt(Me.b10Worksheet_HoursFlag.SelectedValue)
            cTMP.p85FreeText01 = b10Worksheet_Text.Text
        End If
        If panDoc.Visible Then
            cTMP.p85Prefix = "o23"
            cTMP.p85OtherKey2 = BO.BAS.IsNullInt(Me.x18ID.SelectedValue)
            cTMP.p85FreeText02 = o23Name.Text
        End If

        Master.Factory.p85TempBoxBL.Save(cTMP)
        RefreshTempListB10()
    End Sub

End Class