Public Class x28_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord
    Private _lisX26 As IEnumerable(Of BO.x26EntityField_Binding) = Nothing

    Private Sub x28_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/setting_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Uživatelské pole"

            End With

            RefreshRecord()
            If Master.IsRecordNew And Request.Item("prefix") <> "" Then
                Dim strPrefix As String = Left(Request.Item("prefix"), 3)


            End If
            If Master.IsRecordClone Then
                Master.DataPID = 0
                x28field.Text = "?"
            End If
        End If

    End Sub

    Private Sub RefreshRecord()
        Me.x28NotPublic_j04IDs.DataSource = Master.Factory.j04UserRoleBL.GetList()
        Me.x28NotPublic_j04IDs.DataBind()
        Me.x28NotPublic_j07IDs.DataSource = Master.Factory.j07PersonPositionBL.GetList()
        Me.x28NotPublic_j07IDs.DataBind()

        x27ID.DataSource = Master.Factory.x27EntityFieldGroupBL.GetList(New BO.myQuery)
        x27ID.DataBind()
        x24id.DataSource = Master.Factory.ftBL.GetList_X24(New BO.myQuery)
        x24id.DataBind()
        Me.x23ID.DataSource = Master.Factory.x23EntityField_ComboBL.GetList(New BO.myQuery).Where(Function(p) p.x23DataSource <> "")
        Me.x23ID.DataBind()

        Handle_ChangeX29ID()

        If Master.DataPID = 0 Then Return

        _lisX26 = Master.Factory.x28EntityFieldBL.GetList_x26(Master.DataPID)

        Dim cRec As BO.x28EntityField = Master.Factory.x28EntityFieldBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.x28Flag, CInt(.x28Flag).ToString)
            Me.x28IsAllEntityTypes.Checked = .x28IsAllEntityTypes
            x29ID.SelectedValue = BO.BAS.IsNullInt(.x29ID).ToString
            Handle_ChangeX29ID()
            x27ID.SelectedValue = BO.BAS.IsNull(.x27ID)
            x28Name.Text = .x28Name
            x24id.SelectedValue = CInt(.x24ID).ToString
            x23ID.SelectedValue = .x23ID.ToString
            x28TextboxWidth.Value = .x28TextboxWidth
            x28TextboxHeight.Value = .x28TextboxHeight
            x28DataSource.Text = .x28DataSource
            x28IsFixedDataSource.Checked = .x28IsFixedDataSource
            x28Ordinary.Value = .x28Ordinary
            x28field.Text = .x28Field
            x28IsRequired.Checked = .x28IsRequired
            x28IsPublic.Checked = .x28IsPublic
            If Not .x28IsPublic Then
                basUI.CheckItems(Me.x28NotPublic_j04IDs, BO.BAS.ConvertPIDs2List(.x28NotPublic_j04IDs, ","))
                basUI.CheckItems(Me.x28NotPublic_j07IDs, BO.BAS.ConvertPIDs2List(.x28NotPublic_j07IDs, ","))
            End If
            Master.Timestamp = .Timestamp
            Me.x28Grid_Field.Text = .x28Grid_Field
            Me.x28Grid_SqlFrom.Text = .x28Grid_SqlFrom
            Me.x28Grid_SqlSyntax.Text = .x28Grid_SqlSyntax
            Me.x28Pivot_SelectSql.Text = .x28Pivot_SelectSql
            Me.x28Pivot_GroupBySql.Text = .x28Pivot_GroupBySql
            Me.x28HelpText.Text = .x28HelpText
            Me.x28Query_Field.Text = .x28Query_Field
            Me.x28Query_SqlSyntax.Text = .x28Query_SqlSyntax
        End With


    End Sub

    Private Sub x28_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        RefreshState()
    End Sub
    Private Sub RefreshState()
        x24id.Enabled = Master.IsRecordNew
        x29ID.Enabled = Master.IsRecordNew

        Dim b As Boolean = False
        If Me.x23ID.SelectedValue <> "" Then
            If Me.x24id.SelectedValue <> "1" Then Me.x24id.SelectedValue = "1"
            Me.x24id.Enabled = False
        Else
            Me.x24id.Enabled = True
        End If
       
        If x24id.SelectedValue = "2" Then b = True 'string

        x28IsFixedDataSource.Visible = b
        x28DataSource.Visible = b
        lblx28DataSource.Visible = b
        lblItemsListDelimiter.Visible = b
        lblx28TextboxHeight.Visible = b : x28TextboxHeight.Visible = b
        lblx28TextboxWidth.Visible = b : x28TextboxWidth.Visible = b

        Me.panEntityTypes.Visible = Not Me.x28IsAllEntityTypes.Checked
        Me.x28IsRequired.Visible = Me.x28IsAllEntityTypes.Checked

        Me.tabPivot.Visible = False

        Select Case CType(Me.x29ID.SelectedValue, BO.x29IdEnum)
            Case BO.x29IdEnum.p41Project : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny typy projektů"
            Case BO.x29IdEnum.p28Contact : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny typy klientů"
            Case BO.x29IdEnum.j02Person : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny pozice osob"
            Case BO.x29IdEnum.p91Invoice : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny typy faktur"
            Case BO.x29IdEnum.o23Doc : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny typy dokumentů"
            Case BO.x29IdEnum.p31Worksheet
                Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny worksheet sešity"
                tabPivot.Visible = True
            Case BO.x29IdEnum.p56Task : Me.x28IsAllEntityTypes.Text = "Pole je použitelné pro všechny typy úkolů"
        End Select
        panPublic.Visible = Not Me.x28IsPublic.Checked

        If Me.x28Flag.SelectedValue = "1" Then
            tabFlag1.Visible = True
            panHelp.Visible = True
        Else
            tabFlag1.Visible = False
            panHelp.Visible = False
        End If
        tabFlag2.Visible = Not tabFlag1.Visible
        If Master.DataPID <> 0 Then
            Me.x28Flag.Enabled = False

        End If
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x28EntityFieldBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x28-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x28EntityFieldBL
            Dim cRec As BO.x28EntityField = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x28EntityField)
            With cRec
                .x28Flag = CInt(Me.x28Flag.SelectedValue)
                .x28Name = x28Name.Text
                .x24ID = BO.BAS.IsNullInt(x24id.SelectedValue)

                .x28Ordinary = BO.BAS.IsNullInt(x28Ordinary.Value)
                .x28TextboxHeight = BO.BAS.IsNullInt(x28TextboxHeight.Value)
                .x28TextboxWidth = BO.BAS.IsNullInt(x28TextboxWidth.Value)
                .x28DataSource = x28DataSource.Text
                .x28IsFixedDataSource = x28IsFixedDataSource.Checked
                .x28IsRequired = Me.x28IsRequired.Checked
                .x28IsAllEntityTypes = Me.x28IsAllEntityTypes.Checked

                .x29ID = BO.BAS.IsNullInt(x29ID.SelectedValue)
                .x27ID = BO.BAS.IsNullInt(x27ID.SelectedValue)
                .x23ID = BO.BAS.IsNullInt(Me.x23ID.SelectedValue)
                .x28IsPublic = Me.x28IsPublic.Checked
                If Me.x28IsPublic.Checked Then
                    .x28NotPublic_j04IDs = "" : .x28NotPublic_j07IDs = ""
                Else
                    .x28NotPublic_j04IDs = String.Join(",", basUI.GetCheckedItems(Me.x28NotPublic_j04IDs))
                    .x28NotPublic_j07IDs = String.Join(",", basUI.GetCheckedItems(Me.x28NotPublic_j07IDs))
                End If

                .x28Grid_Field = Me.x28Grid_Field.Text
                .x28Grid_SqlFrom = Me.x28Grid_SqlFrom.Text
                .x28Grid_SqlSyntax = Me.x28Grid_SqlSyntax.Text
                .x28Pivot_SelectSql = Me.x28Pivot_SelectSql.Text
                .x28Pivot_GroupBySql = Me.x28Pivot_GroupBySql.Text
                .x28HelpText = Me.x28HelpText.Text
                .x28Query_SqlSyntax = Me.x28Query_SqlSyntax.Text
                .x28Query_Field = Me.x28Query_Field.Text
            End With

            Dim lisX26 As New List(Of BO.x26EntityField_Binding)
            For Each ri As RepeaterItem In rp1.Items
                If CType(ri.FindControl("chkEntityType"), CheckBox).Checked Then
                    Dim c As New BO.x26EntityField_Binding
                    c.x26EntityTypePID = CInt(CType(ri.FindControl("x26EntityTypePID"), HiddenField).Value)
                    c.x29ID_EntityType = CInt(CType(ri.FindControl("x29ID_EntityType"), HiddenField).Value)
                    c.x28ID = Master.DataPID
                    c.x26IsEntryRequired = CType(ri.FindControl("x26IsEntryRequired"), CheckBox).Checked
                    lisX26.Add(c)
                End If
                
            Next
            If .Save(cRec, lisX26) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent()
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub x24id_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles x24id.SelectedIndexChanged
        RefreshState()
    End Sub

    Private Sub Handle_ChangeX29ID()
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Select Case CType(Me.x29ID.SelectedValue, BO.x29IdEnum)
            Case BO.x29IdEnum.p28Contact
                rp1.DataSource = Master.Factory.p29ContactTypeBL.GetList(mq)
            Case BO.x29IdEnum.p41Project
                rp1.DataSource = Master.Factory.p42ProjectTypeBL.GetList(mq)
            Case BO.x29IdEnum.j02Person
                rp1.DataSource = Master.Factory.j07PersonPositionBL.GetList(mq)
            Case BO.x29IdEnum.o23Doc
                rp1.DataSource = Master.Factory.x18EntityCategoryBL.GetList(mq)
            Case BO.x29IdEnum.p91Invoice           
                rp1.DataSource = Master.Factory.p92InvoiceTypeBL.GetList(mq)
            Case BO.x29IdEnum.p90Proforma
                rp1.DataSource = Master.Factory.p89ProformaTypeBL.GetList(mq)
            Case BO.x29IdEnum.p31Worksheet
                rp1.DataSource = Master.Factory.p34ActivityGroupBL.GetList(mq)
            Case BO.x29IdEnum.p56Task
                rp1.DataSource = Master.Factory.p57TaskTypeBL.GetList(mq)
            Case Else
                Return
        End Select

        rp1.DataBind()
    End Sub

    Private Sub x29ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x29ID.SelectedIndexChanged
        Handle_ChangeX29ID()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim strName As String = "", intPID As Integer, x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified
        If TypeOf e.Item.DataItem Is BO.p42ProjectType Then
            Dim cRec As BO.p42ProjectType = CType(e.Item.DataItem, BO.p42ProjectType)
            strName = cRec.p42Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p42ProjectType
        End If
        If TypeOf e.Item.DataItem Is BO.p92InvoiceType Then
            Dim cRec As BO.p92InvoiceType = CType(e.Item.DataItem, BO.p92InvoiceType)
            strName = cRec.p92Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p92InvoiceType
        End If
        If TypeOf e.Item.DataItem Is BO.p29ContactType Then
            Dim cRec As BO.p29ContactType = CType(e.Item.DataItem, BO.p29ContactType)
            strName = cRec.p29Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p29ContactType
        End If
        If TypeOf e.Item.DataItem Is BO.j07PersonPosition Then
            Dim cRec As BO.j07PersonPosition = CType(e.Item.DataItem, BO.j07PersonPosition)
            strName = cRec.j07Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.j07PersonPosition
        End If
       
        If TypeOf e.Item.DataItem Is BO.p34ActivityGroup Then
            Dim cRec As BO.p34ActivityGroup = CType(e.Item.DataItem, BO.p34ActivityGroup)
            strName = cRec.p34Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p34ActivityGroup
        End If
        If TypeOf e.Item.DataItem Is BO.p89ProformaType Then
            Dim cRec As BO.p89ProformaType = CType(e.Item.DataItem, BO.p89ProformaType)
            strName = cRec.p89Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p89ProformaType
        End If
        If TypeOf e.Item.DataItem Is BO.p57TaskType Then
            Dim cRec As BO.p57TaskType = CType(e.Item.DataItem, BO.p57TaskType)
            strName = cRec.p57Name : intPID = cRec.PID : x29ID = BO.x29IdEnum.p57TaskType
        End If
        Dim intX29ID As Integer = CInt(x29ID)
        CType(e.Item.FindControl("x26EntityTypePID"), HiddenField).Value = intPID.ToString
        CType(e.Item.FindControl("x29ID_EntityType"), HiddenField).Value = intX29ID.ToString
        With CType(e.Item.FindControl("chkEntityType"), CheckBox)
            .Text = strName
            If Not _lisX26 Is Nothing Then
                If _lisX26.Where(Function(p) p.x26EntityTypePID = intPID And p.x29ID_EntityType = intX29ID).Count > 0 Then
                    .Checked = True
                End If
                If _lisX26.Where(Function(p) p.x26EntityTypePID = intPID And p.x29ID_EntityType = intX29ID And p.x26IsEntryRequired = True).Count > 0 Then
                    CType(e.Item.FindControl("x26IsEntryRequired"), CheckBox).Checked = True
                End If
            End If
        End With
        
    End Sub
End Class