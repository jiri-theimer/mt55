Imports Telerik.Web.UI
Public Class approving_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private Property _needFilterIsChanged As Boolean = False

    Public Property CurrentPrefix As String
        Get
            Return Me.hidCurPrefix.Value
        End Get
        Set(value As String)
            Me.hidCurPrefix.Value = value
        End Set
    End Property
    Public ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            Return BO.BAS.GetX29FromPrefix(Me.hidCurPrefix.Value)
        End Get
    End Property
    
    Private Sub approving_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        query1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Schvalovat, připravit fakturační podklady"
                .SiteMenuValue = "p31_approving"

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("approving_framework-j70id")
                    .Add("approving_framework-j27id")
                    .Add("p31_grid-period")
                    .Add("p31_grid-periodtype")
                    .Add("periodcombo-custom_query")

                    .Add("approving_framework-prefix")
                    .Add("approving_framework-pagesize")
                    .Add("approving_framework-scope")
                    .Add("approving_framework-groupby-p41")
                    .Add("approving_framework-groupby-j02")
                    .Add("approving_framework-groupby-p28")
                    .Add("approving_framework-kusovnik")


                    .Add("approving_framework-filter_setting-p41")
                    .Add("approving_framework-filter_sql-p41")
                    .Add("approving_framework-filter_setting-p28")
                    .Add("approving_framework-filter_sql-p28")
                    .Add("approving_framework-filter_setting-j02")
                    .Add("approving_framework-filter_sql-j02")
                    
                    .Add("approving_framework-chkFirstLastCount")
                    .Add("approving_framework-chkShowTags")
                    .Add("approving_framework-cbxScrollingFlag")
                    .Add("x18_querybuilder-value-p41-approve")
                    .Add("x18_querybuilder-text-p41-approve")
                    .Add("x18_querybuilder-value-p28-approve")
                    .Add("x18_querybuilder-text-p28-approve")
                    .Add("x18_querybuilder-value-j02-approve")
                    .Add("x18_querybuilder-text-j02-approve")
                    .Add("x18_querybuilder-value-p56-approve")
                    .Add("x18_querybuilder-text-p56-approve")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    Me.chkFirstLastCount.Checked = BO.BAS.BG(.GetUserParam("approving_framework-chkFirstLastCount", "0"))
                    Me.chkShowTags.Checked = BO.BAS.BG(.GetUserParam("approving_framework-chkShowTags", "0"))
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("p31_grid-period")
                    If Request.Item("prefix") = "" Then
                        Me.CurrentPrefix = .GetUserParam("approving_framework-prefix", "p41")
                    Else
                        Me.CurrentPrefix = Request.Item("prefix")
                    End If

                    Me.tabs1.FindTabByValue(Me.CurrentPrefix).Selected = True
                    basUI.SelectDropdownlistValue(Me.cbxPaging, .GetUserParam("approving_framework-pagesize", "50"))
                    basUI.SelectDropdownlistValue(Me.cbxScope, .GetUserParam("approving_framework-scope", "1"))
                    basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("approving_framework-groupby-" & Me.CurrentPrefix, ""))
                    Me.chkKusovnik.Checked = BO.BAS.BG(.GetUserParam("approving_framework-kusovnik", "0"))
                    basUI.SelectRadiolistValue(Me.cbxScrollingFlag, .GetUserParam("approving_framework-cbxScrollingFlag", "2"))
                    hidX18_value.Value = .GetUserParam("x18_querybuilder-value-" & Me.CurrentPrefix & "-approve")
                    Me.x18_querybuilder_info.Text = .GetUserParam("x18_querybuilder-text-" & Me.CurrentPrefix & "-approve")

                End With
                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.p28Contact
                        Me.cbxGroupBy.Items.FindByValue("Client").Enabled = False
                    Case BO.x29IdEnum.j02Person
                        Me.cbxGroupBy.Items.FindByValue("Client").Enabled = False
                        menu1.FindItemByValue("bin").Visible = False
                       
                End Select

                panExport.Visible = .Factory.TestPermission(BO.x53PermValEnum.GR_GridTools)
                query1.AllowSettingButton = panExport.Visible

            End With
            With Master.Factory.j03UserBL
                Dim strJ70ID As String = Request.Item("j70id")
                If strJ70ID = "" Then strJ70ID = .GetUserParam("approving_framework-j70id")
                query1.RefreshData(BO.BAS.IsNullInt(strJ70ID))
                SetupGrid(.GetUserParam("approving_framework-filter_setting-" + Me.CurrentPrefix), .GetUserParam("approving_framework-filter_sql-" + Me.CurrentPrefix))
            End With

        End If
        
    End Sub

    
    Private Sub SetupGrid(strFilterSetting As String, strFilterExpression As String)
        With grid1
            .ClearColumns()
            .AllowMultiSelect = True
            .AddCheckboxSelector()
            .PageSize = BO.BAS.IsNullInt(Me.cbxPaging.SelectedValue)
            If cbxScrollingFlag.SelectedValue <> "0" Then
                .radGridOrig.ClientSettings.Scrolling.AllowScroll = True
            End If
            If cbxScrollingFlag.SelectedValue = "2" Then
                .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = True
            End If
            ''.AddSystemColumn(16)
            .AddContextMenuColumn(16)

            .radGridOrig.ShowFooter = True

            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p41Project
                    ''.AddSystemColumn(25)
                    If Me.cbxGroupBy.SelectedValue <> "Client" Then
                        .AddColumn("Client", "Klient")
                    End If

                    .AddColumn("Project", "Projekt")

                Case BO.x29IdEnum.p28Contact
                    ''.AddSystemColumn(25)
                    .AddColumn("Client", "Klient")
                Case BO.x29IdEnum.j02Person
                    .AddColumn("Person", "Osoba")
                Case BO.x29IdEnum.p56Task
                    .AddColumn("Client", "Klient")
                    .AddColumn("Project", "Projekt")
                    .AddColumn("Task", "Úkol")
            End Select
            If Me.cbxGroupBy.SelectedValue <> "j27Code" Then
                .AddColumn("j27Code", "")
            End If

            If Me.cbxScope.SelectedValue = "1" Then
                .AddColumn("rozpracovano_hodiny", "Hodiny", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("rozpracovano_honorar", "Honorář", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("rozpracovano_vydaje", "Výdaje", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("rozpracovano_odmeny", "Fixní odměny", BO.cfENUM.Numeric2, , , , , True)
                If Me.chkKusovnik.Checked Then
                    .AddColumn("rozpracovano_kusovnik_honorar", "Honorář z kusovníku", BO.cfENUM.Numeric2, , , , , True)
                End If
                If chkFirstLastCount.Checked Then
                    .AddColumn("rozpracovano_prvni", "První", BO.cfENUM.DateOnly)
                    .AddColumn("rozpracovano_posledni", "Poslední", BO.cfENUM.DateOnly)
                End If
                .AddColumn("rozpracovano_pocet", "Počet", BO.cfENUM.Numeric0, , , , , True)
            End If
            If Left(Me.cbxScope.SelectedValue, 1) = "2" Then
                .AddColumn("schvaleno_hodiny_fakturovat", "Hodiny k fakturaci", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("schvaleno_honorar_fakturovat", "Honorář", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("schvaleno_vydaje_fakturovat", "Výdaje", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("schvaleno_odmeny_fakturovat", "Fixní odměny", BO.cfENUM.Numeric2, , , , , True)
                If Me.chkKusovnik.Checked Then
                    .AddColumn("schvaleno_kusovnik_honorar", "Honorář z kusovníku", BO.cfENUM.Numeric2, , , , , True)
                End If
                .AddColumn("schvaleno_hodiny_pausal", "Hodiny v paušálu", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("schvaleno_hodiny_odpis", "Odepsané hodiny", BO.cfENUM.Numeric2, , , , , True)
                .AddColumn("schvaleno_hodiny_pozdeji", "Hodiny fakturovat později", BO.cfENUM.Numeric2, , , , , True)
                
                If Me.chkFirstLastCount.Checked Then
                    .AddColumn("schvaleno_prvni", "První", BO.cfENUM.DateOnly)
                    .AddColumn("schvaleno_posledni", "Poslední", BO.cfENUM.DateOnly)
                End If
                .AddColumn("schvaleno_pocet", "Počet", BO.cfENUM.Numeric0, , , , , True)
            End If
            If chkShowTags.Checked Then
                .AddColumn("TagsInlineHtml", "", , False, , , , , False)
            End If
            .SetFilterSetting(strFilterSetting, strFilterExpression)
        End With
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With

        With grid1.radGridOrig.ClientSettings.Scrolling
            .AllowScroll = True
            '.UseStaticHeaders = True
            Dim intHeight As Integer = Request.Browser.ScreenPixelsHeight * 2 - 100
            intHeight = intHeight - 110
            .ScrollHeight = Unit.Parse(intHeight.ToString & "px")
        End With

    End Sub

    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With grid1.radGridOrig.MasterTableView
            .GroupByExpressions.Clear()
            If strGroupField = "" Then Return
            .ShowGroupFooter = True
            Dim GGE As New GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strFieldHeader

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)
        End With
    End Sub

    Private Sub RefreshRecord()

    End Sub

    Private Sub grid1_FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String) Handles grid1.FilterCommand
        _needFilterIsChanged = True
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.ApprovingFramework = CType(e.Item.DataItem, BO.ApprovingFramework)
        If cbxScope.SelectedValue = "1" Then
            If Not cRec.rozpracovano_odmeny Is Nothing Then
                dataItem("rozpracovano_odmeny").ForeColor = Drawing.Color.Blue
            End If
            If Not cRec.rozpracovano_vydaje Is Nothing Then
                dataItem("rozpracovano_vydaje").ForeColor = Drawing.Color.Brown
            End If
        End If
        If Left(cbxScope.SelectedValue, 1) = "2" Then
            If Not cRec.schvaleno_odmeny_fakturovat Is Nothing Then
                dataItem("schvaleno_odmeny_fakturovat").ForeColor = Drawing.Color.Blue
            End If
            If Not cRec.schvaleno_vydaje_fakturovat Is Nothing Then
                dataItem("schvaleno_vydaje_fakturovat").ForeColor = Drawing.Color.Brown
            End If
        End If
        ''Select Case Me.CurrentX29ID
        ''    Case BO.x29IdEnum.p41Project
        ''        dataItem("systemcolumn").Text = "<a href='p41_framework.aspx?pid=" & cRec.PID.ToString & "'><img src='Images/fullscreen.png'/></a>"
        ''    Case BO.x29IdEnum.p28Contact
        ''        dataItem("systemcolumn").Text = "<a href='p28_framework.aspx?pid=" & cRec.PID.ToString & "'><img src='Images/fullscreen.png'/></a>"
        ''End Select

        With dataItem.Item("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('" & cRec.Prefix & "','" & cRec.PID.ToString & "',this,'approving_framework')" & Chr(34) & "></a>"
        End With

    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        If _needFilterIsChanged Then
            With Master.Factory.j03UserBL
                .SetUserParam("approving_framework-filter_setting-" + Me.CurrentPrefix, grid1.GetFilterSetting())
                .SetUserParam("approving_framework-filter_sql-" + Me.CurrentPrefix, grid1.GetFilterExpression())
            End With
        End If

        Dim mq As New BO.myQueryP31
        If Me.cbxScope.SelectedValue = "1" Then
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForDoApprove
        End If
        If Left(Me.cbxScope.SelectedValue, 1) = "2" Then
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForReApprove
            Select Case Me.cbxScope.SelectedValue
                Case "20" : mq.p31ApprovingLevel = 0
                Case "21" : mq.p31ApprovingLevel = 1
                Case "22" : mq.p31ApprovingLevel = 2
            End Select

        End If
        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil
        mq.j70ID = query1.CurrentJ70ID

        Dim lis As IEnumerable(Of BO.ApprovingFramework) = Master.Factory.p31WorksheetBL.GetList_ApprovingFramework(Me.CurrentX29ID, mq, hidX18_value.Value, chkShowTags.Checked)
        Dim qry = From p In lis Select p.j27ID, p.j27Code Distinct

        With Me.cbxJ27ID
            Dim ss As String = ""
            If Not Page.IsPostBack Then
                ss = Master.Factory.j03UserBL.GetUserParam("approving_framework-j27id")
            Else
                If .SelectedIndex > 0 Then ss = .SelectedValue
            End If
            If qry.Count > 1 Then
                .Items.Clear()
                Dim s As New List(Of String)
                For Each row In qry
                    .Items.Add(New ListItem(row.j27Code, row.j27ID.ToString))
                    s.Add(row.j27Code)
                Next
                .Items.Insert(0, New ListItem(String.Join("+", s), ""))
                If ss <> "" Then basUI.SelectDropdownlistValue(Me.cbxJ27ID, ss)

                If .SelectedValue <> "" Then
                    Dim intJ27ID As Integer = BO.BAS.IsNullInt(.SelectedValue)
                    lis = lis.Where(Function(p) p.j27ID = intJ27ID)
                End If
            Else
                .Visible = False
            End If
        End With
        
        grid1.DataSource = lis
    End Sub

    Private Sub approving_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.CurrentQuery.Text = ""
        
        If hidX18_value.Value <> "" Then
            Me.CurrentQuery.Text += "<img src='Images/query.png' style='margin-left:20px;'/><img src='Images/label.png'/>" & Me.x18_querybuilder_info.Text
            cmdClearX18.Visible = True
        Else
            cmdClearX18.Visible = False
        End If
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        If Me.cbxScope.SelectedValue = "1" Then
            lblHeader.Text = "Schvalovat"

        Else
            lblHeader.Text = "Fakturovat"
            With Me.cbxScope
                Select Case .SelectedValue
                    Case "20" : .ToolTip = "Schváleno na úrovni #0"
                    Case "21" : .ToolTip = "Schváleno na úrovni #1"
                    Case "22" : .ToolTip = "Schváleno na úrovni #2"
                    Case Else : .ToolTip = ""
                End Select
            End With
        End If
        If cbxJ27ID.SelectedIndex > 0 Then
            cbxJ27ID.BackColor = basUI.ColorQueryRGB
        Else
            cbxJ27ID.BackColor = Nothing
        End If
    End Sub

    
    Private Sub ReloadPage()
        Response.Redirect("approving_framework.aspx")
    End Sub

    

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("p31_grid-period", Me.period1.SelectedValue)
        grid1.Rebind(False)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-pagesize", Me.cbxPaging.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub cmdHardRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdHardRefreshOnBehind.Click
        Select Case Me.hidHardRefreshFlag.Value
            Case "pdf", "xls", "doc"
                basUIMT.Handle_GridTelerikExport(grid1, Me.hidHardRefreshFlag.Value)
            Case "j70-run", "x18_querybuilder"
                ReloadPage()
            Case Else
                grid1.Rebind(False)
        End Select


        Me.hidHardRefreshFlag.Value = ""
    End Sub

    Private Sub cbxScope_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxScope.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-scope", Me.cbxScope.SelectedValue)
        ReloadPage()

    End Sub

    Private Sub cbxGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGroupBy.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-groupby-" & Me.CurrentPrefix, Me.cbxGroupBy.SelectedValue)
        ReloadPage()

    End Sub

    Private Sub chkKusovnik_CheckedChanged(sender As Object, e As EventArgs) Handles chkKusovnik.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-kusovnik", BO.BAS.GB(Me.chkKusovnik.Checked))
        ReloadPage()

    End Sub

    

    Private Sub chkFirstLastCount_CheckedChanged(sender As Object, e As EventArgs) Handles chkFirstLastCount.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-chkFirstLastCount", BO.BAS.GB(Me.chkFirstLastCount.Checked))
        ReloadPage()
    End Sub

    Private Sub cbxScrollingFlag_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxScrollingFlag.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-cbxScrollingFlag", Me.cbxScrollingFlag.SelectedValue)
        ReloadPage()
    End Sub
    Private Sub cmdClearX18_Click(sender As Object, e As ImageClickEventArgs) Handles cmdClearX18.Click
        With Master.Factory.j03UserBL
            .SetUserParam("x18_querybuilder-value-" & Me.CurrentPrefix & "-approve", "")
            .SetUserParam("x18_querybuilder-text-" & Me.CurrentPrefix & "-approve", "")
        End With
        ReloadPage()
    End Sub
    
    Private Sub cbxJ27ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxJ27ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-j27id", Me.cbxJ27ID.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub chkShowTags_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowTags.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("approving_framework-chkShowTags", BO.BAS.GB(Me.chkShowTags.Checked))
        ReloadPage()
    End Sub
End Class