Imports Telerik.Web.UI

Public Class admin_workflow
    Inherits System.Web.UI.Page

    Private Class IDS
        Public Property Prefix As String
        Public Property OrigPID As Integer
        Public Property NewPID As Integer
        Public Sub New(strPrefix As String, intOrigPID As Integer, intNewPID As Integer)
            Me.Prefix = strPrefix
            Me.OrigPID = intOrigPID
            Me.NewPID = intNewPID
        End Sub
    End Class
    Public Property CurrentB01ID As Integer
        Get
            Return BO.BAS.IsNullInt(cbxB01ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.cbxB01ID, value.ToString)
        End Set
    End Property
    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            If Me.hidcurx29id.Value = "" Then Return BO.x29IdEnum.p41Project
            Return CType(Me.hidcurx29id.Value, BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidcurx29id.Value = CInt(value).ToString
        End Set
    End Property
    Public Property CurrentB02ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidcurb02id.Value)
        End Get
        Set(value As Integer)
            hidcurb02id.Value = value.ToString
        End Set
    End Property
    Public Property CurrentB06ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidcurb06id.Value)
        End Get
        Set(value As Integer)
            hidcurb06id.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Workflow návrhář"
                .SiteMenuValue = "admin_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_Admin)

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("admin_workflow-b01id")
                    .Add("admin_workflow-showb65")
                    .Add("admin_workflow-include_nonactual")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
            End With

            Me.cbxB01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList()
            Me.cbxB01ID.DataBind()

            Me.CurrentB01ID = BO.BAS.IsNullInt(Request.Item("b01id"))
            With Master.Factory.j03UserBL
                If Me.CurrentB01ID = 0 Then
                    Me.CurrentB01ID = BO.BAS.IsNullInt(.GetUserParam("admin_workflow-b01id"))
                End If

                Me.chkIncludeNonActual.Checked = BO.BAS.BG(.GetUserParam("admin_workflow-include_nonactual", "0"))
            End With
            ''With Master
            ''    .ActivateContextMenu(True)
            ''    .AddContextMenu("Nový stav", "javascript:b02_new()", True)
            ''    .AddContextMenu("Detail vybraného stavu", "javascript:b02_edit()", True, , , "b02_edit")

            ''    .AddContextMenu("Nový krok", "javascript:b06_new(true)", True, , , "b06_new")


            ''    .AddContextMenu("Hlavička šablony", "javascript:b01_edit()", True)
            ''    '.AddContextMenu("Kopírovat", "javascript:b01_clone()", True)
            ''    .AddContextMenu("Export do XML", "javascript:b01_export()", True)
            ''    .AddContextMenu("Import z XML", "javascript:b01_import()", True)
            ''End With

            

            RefreshRecord()


        End If
    End Sub
    Private Sub RefreshState()

        If Me.CurrentB02ID = 0 Then
            cmdNewB06.Visible = False
        Else
            cmdNewB06.Visible = True
        End If
    End Sub


    

    Private Sub RefreshRecord()
        Me.CurrentB02ID = 0
        If Me.CurrentB01ID = 0 Then
            Return
        End If
        Dim cRec As BO.b01WorkflowTemplate = Master.Factory.b01WorkflowTemplateBL.Load(Me.CurrentB01ID)
        Me.CurrentX29ID = cRec.x29ID
        RefreshB65List(cRec)

        RefreshTree()
    End Sub

    Private Sub RefreshB65List(cRec As BO.b01WorkflowTemplate)

        Dim lis As IEnumerable(Of BO.b65WorkflowMessage) = Master.Factory.b65WorkflowMessageBL.GetList(Me.CurrentB01ID, New BO.myQuery)

        rpB65.DataSource = lis
        rpB65.DataBind()
        ph1.Text = BO.BAS.OM2(ph1.Text, lis.Count.ToString)
    End Sub
    

    Private Sub rpB65_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpB65.ItemDataBound
        Dim cRec As BO.b65WorkflowMessage = CType(e.Item.DataItem, BO.b65WorkflowMessage)
        With CType(e.Item.FindControl("b65name"), HyperLink)
            .Text = cRec.b65Name
            .NavigateUrl = "javascript:b65_edit(" & cRec.PID.ToString & ")"
        End With
        CType(e.Item.FindControl("b65MessageSubject"), Label).Text = cRec.b65MessageSubject
    End Sub

    

    Private Sub RefreshTree()
        Dim myQuery As New BO.myQuery
        If Me.chkIncludeNonActual.Checked Then
            myQuery.Closed = BO.BooleanQueryMode.NoQuery
        End If
        Dim lisB02 As IEnumerable(Of BO.b02WorkflowStatus) = Master.Factory.b02WorkflowStatusBL.GetList(Me.CurrentB01ID)
        Dim lisB06 As IEnumerable(Of BO.b06WorkflowStep) = Master.Factory.b06WorkflowStepBL.GetList(Me.CurrentB01ID)
        With tree1
            .Clear()
            For Each cRec As BO.b02WorkflowStatus In lisB02
                Dim intB02ID As Integer = cRec.PID
                If Me.CurrentB02ID = 0 Then
                    Me.CurrentB02ID = intB02ID
                End If
                Dim strImgB02 As String = "Images/a14.gif"
               
                Dim n As New RadTreeNode(cRec.b02Name, "b02-" & intB02ID.ToString)
                If cRec.b02Code <> "" Then n.Text += " (" & cRec.b02Code & ")"
                n.NavigateUrl = "javascript:b02_click(" & intB02ID.ToString & ")"
                n.ImageUrl = strImgB02
                n.ToolTip = "Workflow stav [" & cRec.b02Code & "]"
                If cRec.IsClosed Then n.Font.Strikeout = True
                .AddItem(n)

                For Each cB06 As BO.b06WorkflowStep In lisB06.Where(Function(p As BO.b06WorkflowStep) p.b02ID = intB02ID)
                    Dim strName As String = cB06.b06Name
                    Dim strIMG As String = "Images/a13.gif"
                    If cB06.b02ID_Target <> 0 Then
                        strName += "->" & cB06.TargetStatus
                        strIMG = "Images/a12.gif"
                    End If
                    If cB06.b06IsKickOffStep Then
                        strIMG = "Images/star.png"
                    End If
                    n = New RadTreeNode(strName, "b06-" & cB06.PID.ToString)
                    If Not cB06.b06IsManualStep Then
                        n.ForeColor = Drawing.Color.Brown    'neuživatelský krok
                        n.Font.Italic = True
                    End If
                    
                    n.ImageUrl = strIMG
                    n.ToolTip = "Workflow krok"
                    n.NavigateUrl = "javascript:b06_click(" & cB06.PID.ToString & ")"
                    If cB06.IsClosed Then n.Font.Strikeout = True

                    .AddItem(n, "b02-" & intB02ID.ToString)
                Next
            Next
            .SelectedValue = "b02-" & Me.CurrentB02ID.ToString
            .ExpandAll()
        End With

        RefreshB02Record()
        ''If lisB02.Where(Function(p) p.b02IsDefaultStatus = True).Count = 0 Then
        ''    Master.Notify("Pozor, tato workflow šablona zatím nemá nastavený výchozí (startovací) stav!", 1)
        ''End If
    End Sub

    Private Sub RefreshB02Record()
        panB02Rec.Visible = False
        If Me.CurrentB02ID = 0 Then
            Return
        End If

        panB02Rec.Visible = True
        Dim cRec As BO.b02WorkflowStatus = Master.Factory.b02WorkflowStatusBL.Load(Me.CurrentB02ID)
        With cRec
            ab02name.Text = .b02Name
            ab02name.NavigateUrl = "javascript:b02_edit(" & .PID.ToString & ")"
            ''Master.ChangeContextMenuItem("b02_edit", "Detail stavu [" & .b02Name & "]", "javascript:b02_edit(" & .PID.ToString & ")")
            ''Master.ChangeContextMenuItem("b06_new", "Nový krok v rámci stavu [" & .b02Name & "]", "javascript:b06_new()")
            b02ident.Text = .b02Code
            If .b02Color <> "" Then
                b02ident.Style.Item("background-color") = .b02Color
            Else
                b02ident.Style.Item("background-color") = ""
            End If
        End With
        
        Dim lisB06 As IEnumerable(Of BO.b06WorkflowStep) = Master.Factory.b06WorkflowStepBL.GetList(Me.CurrentB01ID).Where(Function(p) p.b02ID = cRec.PID)
        rpB02_B06.DataSource = lisB06
        rpB02_B06.DataBind()


        ''Dim lisB10 As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding) = Master.Factory.b02WorkflowStatusBL.GetList_B10(cRec.PID)
        ''rpB10.DataSource = lisB10
        ''rpB10.DataBind()

    End Sub

    Private Sub cmdRefreshOnBehind_Click(sender As Object, e As System.EventArgs) Handles cmdRefreshOnBehind.Click
        Select Case hidcurflag.Value
            Case "b01-save"
                Response.Redirect("admin_workflow.aspx?b01id=" & Me.CurrentB01ID.ToString)
            Case "b01-delete"
                Master.Factory.j03UserBL.SetUserParam("admin_workflow-b01id", "")
                Response.Redirect("admin_workflow.aspx")
            Case "b02-save", "b06-save", "b06-delete"
                RefreshTree()
            Case "b02-change"  'výběr jiného stavu
                RefreshB02Record()
            Case "b06-change"   'výběr jiného kroku
                Dim cRec As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(Me.CurrentB06ID)
                Me.CurrentB02ID = cRec.b02ID
                RefreshB02Record()
            Case "b65-save", "b65-delete"
                Dim cRec As BO.b01WorkflowTemplate = Master.Factory.b01WorkflowTemplateBL.Load(Me.CurrentB01ID)
                RefreshB65List(cRec)
                RefreshB02Record()
            Case Else
                RefreshRecord()
        End Select
        hidcurflag.Value = ""
    End Sub

    Private Sub rpB02_B06_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpB02_B06.ItemDataBound
        Dim cRec As BO.b06WorkflowStep = CType(e.Item.DataItem, BO.b06WorkflowStep)
        With CType(e.Item.FindControl("b06name"), HyperLink)
            .Text = cRec.b06Name
            If cRec.TargetStatus <> "" Then .Text += "->" & cRec.TargetStatus
            .NavigateUrl = "javascript:b06_edit(" & cRec.PID.ToString & ")"
            
            If cRec.PID = Me.CurrentB06ID Then
                .Font.Bold = True
            End If
            If Not cRec.b06IsManualStep Then
                .ForeColor = Drawing.Color.Brown    'neuživatelský krok
                .Font.Italic = True
            End If
            If cRec.IsClosed Then
                .Font.Strikeout = True
            End If
        End With
        With CType(e.Item.FindControl("imgB06"), Image)
            If cRec.b02ID_Target <> 0 Then
                .ImageUrl = "Images/a12.gif"
            Else
                .ImageUrl = "Images/a13.gif"
            End If
            If cRec.b06IsKickOffStep Then
                .ImageUrl = "Images/star.png"
            End If
        End With

    End Sub

    
    Private Sub admin_workflow_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        RefreshState()
    End Sub

    Private Sub chkIncludeNonActual_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkIncludeNonActual.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("admin_workflow-include_nonactual", BO.BAS.GB(chkIncludeNonActual.Checked))
        RefreshRecord()
    End Sub

    Private Sub rpB10_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpB10.ItemDataBound
        Dim cRec As BO.b10WorkflowCommandCatalog_Binding = CType(e.Item.DataItem, BO.b10WorkflowCommandCatalog_Binding)
        CType(e.Item.FindControl("b09name"), Label).Text = cRec.b09Name
    End Sub

    Private Sub cbxB01ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxB01ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("admin_workflow-b01id", Me.CurrentB01ID.ToString)
        RefreshRecord()
    End Sub

    Private Sub cmdGenerateDump_Click(sender As Object, e As EventArgs) Handles cmdGenerateDump.Click
        Dim pars As New List(Of BO.PluginDbParameter)
        Dim par As New BO.PluginDbParameter("b01id", Me.CurrentB01ID)
        pars.Add(par)
        Dim s As String = "SELECT * from b01WorkflowTemplate where b01ID=@b01id"
        s += ";SELECT * FROM b02WorkflowStatus WHERE b01ID=@b01id"
        s += ";SELECT * FROM b65WorkflowMessage WHERE b01ID=@b01id"
        s += ";SELECT * FROM b06WorkflowStep WHERE b02ID IN (SELECT b02ID FROM b02WorkflowStatus where b01ID=@b01id)"
        s += ";SELECT * from b08WorkflowReceiverToStep WHERE b06ID IN (SELECT a.b06ID FROM b06WorkflowStep a INNER JOIN b02WorkflowStatus b ON a.b02ID=b.b02ID WHERE b.b01ID=@b01id)"
        s += ";SELECT * from b11WorkflowMessageToStep WHERE b06ID IN (SELECT a.b06ID FROM b06WorkflowStep a INNER JOIN b02WorkflowStatus b ON a.b02ID=b.b02ID WHERE b.b01ID=@b01id)"
        s += ";SELECT * from b10WorkflowCommandCatalog_Binding WHERE b06ID IN (SELECT a.b06ID FROM b06WorkflowStep a INNER JOIN b02WorkflowStatus b ON a.b02ID=b.b02ID WHERE b.b01ID=@b01id)"

        Dim ds As System.Data.DataSet = Master.Factory.pluginBL.GetDataSet(s, pars, "b01WorkflowTemplate,b02WorkflowStatus,b65WorkflowMessage,b06WorkflowStep,b08WorkflowReceiverToStep,b11WorkflowMessageToStep,b10WorkflowCommandCatalog_Binding")


        ds.WriteXml(Master.Factory.x35GlobalParam.TempFolder & "\workflow_b01id_" & Me.CurrentB01ID.ToString & ".xml", XmlWriteMode.WriteSchema)
        ''Master.Notify("Šablona vyexportována do TEMpu.")
        Response.Redirect("binaryfile.aspx?tempfile=workflow_b01id_" & Me.CurrentB01ID.ToString & ".xml")
        
    End Sub


    Private Sub cmdImportXML_Click(sender As Object, e As EventArgs) Handles cmdImportXML.Click
        Dim lisFiles As List(Of String) = basUI.Get_Uploaded_Files(Me.upload1, Master.Factory.x35GlobalParam.TempFolder)
        If lisFiles.Count = 0 Then
            Master.Notify("Na vstupu chybí nahraný XML soubor.", NotifyLevel.ErrorMessage)
            Return
        End If
        Dim strFile As String = lisFiles(0)
        Dim ds As New DataSet
        ds.ReadXml(strFile)

        Dim lisIDS As New List(Of IDS)


        Dim dt As DataTable = FindDataTable(ds, "b01WorkflowTemplate")
        Dim cB01 As New BO.b01WorkflowTemplate
        With cB01
            .x29ID = dt.Rows(0).Item("x29ID")
            .b01Name = dt.Rows(0).Item("b01Name") & " KOPIE"
            .b01Code = dt.Rows(0).Item("b01Code") & ""
        End With
        With Master.Factory.b01WorkflowTemplateBL
            If .Save(cB01) Then
                cB01 = .Load(.LastSavedPID)
            End If
        End With

        dt = FindDataTable(ds, "b65WorkflowMessage")
        For Each dbRow As DataRow In dt.Rows
            Dim c As New BO.b65WorkflowMessage
            c.b01ID = cB01.PID
            c.b65Name = dbRow.Item("b65Name") & ""
            c.b65MessageBody = dbRow.Item("b65MessageBody") & ""
            c.b65MessageSubject = dbRow.Item("b65MessageSubject") & ""
            If Master.Factory.b65WorkflowMessageBL.Save(c) Then
                lisIDS.Add(New IDS("b65", dbRow.Item("b65ID"), Master.Factory.b65WorkflowMessageBL.LastSavedPID))
            End If
        Next

        dt = FindDataTable(ds, "b02WorkflowStatus")
        For Each dbRow As DataRow In dt.Rows
            Dim c As New BO.b02WorkflowStatus
            c.b01ID = cB01.PID
            c.b02Code = dbRow.Item("b02Code") & ""
            c.b02Name = dbRow.Item("b02Name")
            c.b02Ordinary = BO.BAS.IsNullInt(dbRow.Item("b02Ordinary"))
            c.b02Color = dbRow.Item("b02Color") & ""
            If Not dbRow.Item("b02IsRecordReadOnly4Owner") Is System.DBNull.Value Then c.b02IsRecordReadOnly4Owner = dbRow.Item("b02IsRecordReadOnly4Owner")
            If Master.Factory.b02WorkflowStatusBL.Save(c) Then
                lisIDS.Add(New IDS("b02", dbRow.Item("b02ID"), Master.Factory.b02WorkflowStatusBL.LastSavedPID))
            End If
        Next

        Dim dtB08 As DataTable = FindDataTable(ds, "b08WorkflowReceiverToStep")
        Dim dtB10 As DataTable = FindDataTable(ds, "b10WorkflowCommandCatalog_Binding")
        Dim dtB11 As DataTable = FindDataTable(ds, "b11WorkflowMessageToStep")

        dt = FindDataTable(ds, "b06WorkflowStep")
        For Each dbRow As DataRow In dt.Rows
            Dim c As New BO.b06WorkflowStep
            c.b02ID = lisIDS.First(Function(p) p.Prefix = "b02" And p.OrigPID = dbRow.Item("b02ID")).NewPID
            If Not dbRow.Item("b02ID_LastReceiver_ReturnTo") Is System.DBNull.Value Then
                c.b02ID_LastReceiver_ReturnTo = BO.BAS.IsNullInt(lisIDS.First(Function(p) p.Prefix = "b02" And p.OrigPID = dbRow.Item("b02ID_LastReceiver_ReturnTo")).NewPID)
            End If
            If Not dbRow.Item("b02ID_Target") Is System.DBNull.Value Then
                c.b02ID_Target = BO.BAS.IsNullInt(lisIDS.First(Function(p) p.Prefix = "b02" And p.OrigPID = dbRow.Item("b02ID_Target")).NewPID())
            End If

            c.b06Code = dbRow.Item("b06Code") & ""
            
            c.b06IsCommentRequired = dbRow.Item("b06IsCommentRequired")
            c.b06IsKickOffStep = dbRow.Item("b06IsKickOffStep")
            c.b06IsManualStep = dbRow.Item("b06IsManualStep")
            c.b06IsNominee = dbRow.Item("b06IsNominee")
            c.b06IsNomineeRequired = dbRow.Item("b06IsNomineeRequired")
            c.b06IsRunOneInstanceOnly = dbRow.Item("b06IsRunOneInstanceOnly")
            c.b06Name = dbRow.Item("b06Name") & ""
            c.b06Ordinary = dbRow.Item("b06Ordinary")
            c.b06RunSQL = dbRow.Item("b06RunSQL") & ""
            c.b06ValidateAutoMoveSQL = dbRow.Item("b06ValidateAutoMoveSQL") & ""
            c.b06ValidateBeforeErrorMessage = dbRow.Item("b06ValidateBeforeErrorMessage") & ""
            c.b06ValidateBeforeRunSQL = dbRow.Item("b06ValidateBeforeRunSQL") & ""
            c.j11ID_Direct = BO.BAS.IsNullInt(dbRow.Item("j11ID_Direct"))
            c.x67ID_Direct = BO.BAS.IsNullInt(dbRow.Item("x67ID_Direct"))
            c.x67ID_Nominee = BO.BAS.IsNullInt(dbRow.Item("x67ID_Nominee"))

            Dim lisB08 As New List(Of BO.b08WorkflowReceiverToStep)
            For Each row As DataRow In dtB08.Rows
                Dim cc As New BO.b08WorkflowReceiverToStep
                ''cc.b06ID = row.Item("b06ID")
                cc.j04ID = TestRecordExistenct("j04", BO.BAS.IsNullInt(row.Item("j04id")))
                cc.j11ID = TestRecordExistenct("j11", BO.BAS.IsNullInt(row.Item("j11id")))
                cc.x67ID = TestRecordExistenct("x67", BO.BAS.IsNullInt(row.Item("x67id")))
                If Not row.Item("b08IsRecordCreator") Is System.DBNull.Value Then cc.b08IsRecordCreator = row.Item("b08IsRecordCreator")
                If Not row.Item("b08IsRecordOwner") Is System.DBNull.Value Then cc.b08IsRecordOwner = row.Item("b08IsRecordOwner")
                lisB08.Add(cc)
            Next
            Dim lisB10 As New List(Of BO.b10WorkflowCommandCatalog_Binding)
            For Each row As DataRow In dtB10.Rows
                Dim cc As New BO.b10WorkflowCommandCatalog_Binding
                ''cc.b06ID = row.Item("b06ID")
                cc.b09ID = BO.BAS.IsNullInt(row.Item("b09ID"))
                cc.b10Worksheet_DateFlag = BO.BAS.IsNullInt(row.Item("b10Worksheet_DateFlag"))
                cc.b10Worksheet_HoursFlag = BO.BAS.IsNullInt(row.Item("b10Worksheet_HoursFlag"))
                cc.b10Worksheet_p72ID = BO.BAS.IsNullInt(row.Item("b10Worksheet_p72ID"))
                cc.b10Worksheet_PersonFlag = BO.BAS.IsNullInt(row.Item("b10Worksheet_PersonFlag"))
                cc.b10Worksheet_ProjectFlag = BO.BAS.IsNullInt(row.Item("b10Worksheet_ProjectFlag"))
                cc.b10Worksheet_Text = row.Item("b10Worksheet_Text") & ""
                cc.p31ID_Template = TestRecordExistenct("p31", BO.BAS.IsNullInt(row.Item("p31ID_Template")))
                lisB10.Add(cc)
            Next
            Dim lisB11 As New List(Of BO.b11WorkflowMessageToStep)
            For Each row As DataRow In dtB11.Rows
                If Not row.Item("b65ID") Is System.DBNull.Value Then
                    Dim cc As New BO.b11WorkflowMessageToStep
                    If lisIDS.Where(Function(p) p.Prefix = "b65" And p.OrigPID = row.Item("b65ID")).Count > 0 Then
                        cc.b65ID = lisIDS.First(Function(p) p.Prefix = "b65" And p.OrigPID = row.Item("b65ID")).NewPID
                        If Not row.Item("b11IsRecordCreator") Is System.DBNull.Value Then cc.b11IsRecordCreator = row.Item("b11IsRecordCreator")
                        If Not row.Item("b11IsRecordOwner") Is System.DBNull.Value Then cc.b11IsRecordCreator = row.Item("b11IsRecordOwner")
                        cc.j02ID = TestRecordExistenct("j02", BO.BAS.IsNullInt(row.Item("j02id")))
                        cc.j11ID = TestRecordExistenct("j11", BO.BAS.IsNullInt(row.Item("j11ID")))
                        cc.x67ID = TestRecordExistenct("x67", BO.BAS.IsNullInt(row.Item("x67ID")))
                        cc.j04ID = TestRecordExistenct("j04", BO.BAS.IsNullInt(row.Item("j04ID")))
                        lisB11.Add(cc)
                    End If

                End If

            Next

            If Master.Factory.b06WorkflowStepBL.Save(c, lisB08, lisB11, lisB10) Then
                ''lisIDS.Add(New IDS("b06", row.Item("b06ID"), Master.Factory.b06WorkflowStepBL.LastSavedPID))
            End If
        Next

        Response.Redirect("admin_workflow.aspx?b01id=" & cB01.PID.ToString)

    End Sub

    Private Function TestRecordExistenct(strPrefix As String, intPID As Integer) As Integer
        If intPID = 0 Then Return 0
        Select Case strPrefix
            Case "j04"
                If Master.Factory.j04UserRoleBL.Load(intPID) Is Nothing Then intPID = 0
            Case "j11"
                If Master.Factory.j11TeamBL.Load(intPID) Is Nothing Then intPID = 0
            Case "j02"
                If Master.Factory.j02PersonBL.Load(intPID) Is Nothing Then intPID = 0
            Case "x67"
                If Master.Factory.x67EntityRoleBL.Load(intPID) Is Nothing Then intPID = 0
            Case "p31"
                If Master.Factory.p31WorksheetBL.Load(intPID) Is Nothing Then intPID = 0
        End Select

        Return intPID
    End Function

    Private Function FindDataTable(ds As DataSet, strTableName As String) As DataTable
        For Each dt As DataTable In ds.Tables
            If LCase(dt.TableName) = LCase(strTableName) Then Return dt
        Next
        Return Nothing
    End Function
End Class