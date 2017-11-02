Imports Telerik.Web.UI
Public Class sumgrid_designer
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _lisDD As List(Of BO.GridColumn) = Nothing
    
    Private Sub sumgrid_designer_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            hidMasterPID.Value = Request.Item("masterpid")
            hidMasterPrefix.Value = Request.Item("masterprefix")
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .neededPermission = BO.x53PermValEnum.GR_P31_Pivot


                .AddToolbarButton("Uložit a spustit statistiku", "ok", , "Images/ok.png")


            End With
            Me.j70ID.DataSource = Master.Factory.j70QueryTemplateBL.GetList(New BO.myQuery, BO.x29IdEnum.p31Worksheet)
            Me.j70ID.DataBind()
            Me.j70ID.Items.Insert(0, "--Pojmenovaný filtr--")
            SetupJ77Combo()

            RefreshRecord()
        End If

    End Sub

    Private Sub SetupGroupByCombo(strDD1 As String, strDD2 As String)
        Dim lisAllCols As List(Of BO.GridColumn) = Me.lisDD.Where(Function(p) p.Pivot_SelectSql <> "").ToList
        Me.dd1.Items.Clear() : Me.dd2.Items.Clear()

        For Each c In lisAllCols.Where(Function(p) p.TreeGroup <> "").Select(Function(p) p.TreeGroup).Distinct
            Dim n As New RadComboBoxItem(c, "group" & c)
            n.Enabled = False
            Me.dd1.Items.Add(n)
        Next

        For i = lisAllCols.Count - 1 To 0 Step -1
            Dim c As BO.GridColumn = lisAllCols(i)
            Dim n As New RadComboBoxItem(c.ColumnHeader, c.ColumnName)
            Select Case c.ColumnType
                Case BO.cfENUM.DateTime, BO.cfENUM.DateTime
                    n.ImageUrl = "Images/type_datetime.png"
                Case BO.cfENUM.DateOnly
                    n.ImageUrl = "Images/type_date.png"
                Case BO.cfENUM.Numeric, BO.cfENUM.Numeric2, BO.cfENUM.Numeric0
                    n.ImageUrl = "Images/type_number.png"
                Case BO.cfENUM.AnyString
                    n.ImageUrl = "Images/type_text.png"
                Case BO.cfENUM.Checkbox
                    n.ImageUrl = "Images/type_checkbox.png"
            End Select

            If c.TreeGroup = "" Then
                Me.dd1.Items.Add(n)
            Else
                Dim y As Integer = Me.dd1.FindItemIndexByValue("group" & c.TreeGroup)
                If y > -1 Then
                    Me.dd1.Items.Insert(y + 1, n)
                End If



            End If
        Next
        For i As Integer = 0 To Me.dd1.Items.Count - 1 - 1
            Dim n As New RadComboBoxItem()
            With Me.dd1.Items(i)
                n.ImageUrl = .ImageUrl
                n.Text = .Text
                n.Value = .Value
                n.Enabled = .Enabled
            End With
            Me.dd2.Items.Add(n)
        Next
        Me.dd2.Items.Insert(0, "")
        If strDD1 <> "" Then Me.dd1.SelectedValue = strDD1
        If strDD2 <> "" Then Me.dd2.SelectedValue = strDD2
    End Sub
    Private Sub SetupSumColsSetting(strDefSumCols As String, strDefAddCols As String)
        Dim lisAllSums As List(Of BO.PivotSumField) = Master.Factory.j77WorksheetStatTemplateBL.ColumnsPallete()

        sumsSource.Items.Clear()
        sumsDest.Items.Clear()
        For Each c In lisAllSums
            Dim it As New RadListBoxItem(c.Caption, c.FieldTypeID.ToString)
            Select Case c.GP
                Case 10 : it.ImageUrl = "Images/a01_stats.png"
                Case 14 : it.ImageUrl = "Images/a14_stats.png"
                Case 13 : it.ImageUrl = "Images/a13_stats.png"
                Case 16 : it.ImageUrl = "Images/a16_stats.png"
                Case 17 : it.ImageUrl = "Images/a17_stats.png"
                Case 24 : it.ImageUrl = "Images/a24_stats.png"
                Case 23 : it.ImageUrl = "Images/a23_stats.png"
                Case 26 : it.ImageUrl = "Images/a26_stats.png"
                Case 30 : it.ImageUrl = "Images/a02_stats.png"
                Case 50 : it.ImageUrl = "Images/a50_stats.png"
                Case 60 : it.ImageUrl = "Images/statement.png"
                Case Else
                    it.ImageUrl = "Images/a00_stats.png"
            End Select
            'Select Case c.ColumnType
            '    Case BO.cfENUM.DateTime, BO.cfENUM.DateTime
            '        it.ImageUrl = "Images/type_datetime.png"
            '    Case BO.cfENUM.DateOnly
            '        it.ImageUrl = "Images/type_date.png"
            '    Case BO.cfENUM.Numeric, BO.cfENUM.Numeric2, BO.cfENUM.Numeric0
            '        it.ImageUrl = "Images/type_number.png"
            '    Case BO.cfENUM.AnyString
            '        it.ImageUrl = "Images/type_text.png"
            '    Case BO.cfENUM.Checkbox
            '        it.ImageUrl = "Images/type_checkbox.png"
            'End Select

            sumsSource.Items.Add(it)
            
        Next

        If strDefSumCols <> "" Then
            Dim a() As String = Split(strDefSumCols, ",")
            For Each s In a
                Dim it As RadListBoxItem = sumsSource.FindItem(Function(p) p.Value = s)
                If Not it Is Nothing Then
                    sumsSource.Transfer(it, sumsSource, sumsDest)
                    sumsSource.ClearSelection()
                    sumsDest.ClearSelection()
                End If
            Next
            sumsSource.ClearSelection()
        End If

        colsSource.Items.Clear()
        colsDest.Items.Clear()
        For Each c As RadComboBoxItem In Me.dd1.Items
            Dim it As New RadListBoxItem(c.Text, c.Value)
            it.ImageUrl = c.ImageUrl
            it.Enabled = c.Enabled
            colsSource.Items.Add(it)
        Next

        If strDefAddCols <> "" Then
            Dim a() As String = Split(strDefAddCols, ",")
            For Each s In a
                Dim b() As String = Split(s, "-")
                Dim it As RadListBoxItem = colsSource.FindItem(Function(p) p.Value = b(0))
                If Not it Is Nothing Then
                    If UBound(b) > 0 Then
                        it.Text += " (" & UCase(b(1)) & ")"
                    Else
                        it.Text += " (ALL)"
                    End If
                    colsSource.Transfer(it, colsSource, colsDest)
                    colsSource.ClearSelection()
                    colsDest.ClearSelection()
                End If
            Next
            colsSource.ClearSelection()
        End If
    End Sub
    Private ReadOnly Property lisDD As List(Of BO.GridColumn)
        Get
            If _lisDD Is Nothing Then _lisDD = Master.Factory.j70QueryTemplateBL.ColumnsPallete(BO.x29IdEnum.p31Worksheet)
            Return _lisDD
        End Get
    End Property
    Private ReadOnly Property CurrentDD1 As BO.GridColumn
        Get
            If Me.dd1.SelectedValue = "" Then Return Nothing
            Return Me.lisDD.Where(Function(p) p.ColumnName = Me.dd1.SelectedValue).First()
        End Get
    End Property
    Private ReadOnly Property CurrentDD2 As BO.GridColumn
        Get
            If Me.dd2.SelectedValue = "" Then Return Nothing
            Return lisDD.Where(Function(p) p.ColumnName = Me.dd2.SelectedValue).First()
        End Get
    End Property
    Private ReadOnly Property CurrentSumFields_PIVOT As List(Of BO.PivotSumField)
        Get
            Dim lis As New List(Of BO.PivotSumField), a() As String = Split(GetSumPIDsInLine(), ",")
            If GetSumPIDsInLine() = "" Then Return lis
            For i As Integer = 0 To UBound(a)
                lis.Add(New BO.PivotSumField(CType(a(i), BO.PivotSumFieldType)))
            Next
            Return lis
        End Get
    End Property
    Private ReadOnly Property CurrentColFields_PIVOT As List(Of BO.GridColumn)
        Get
            Dim lis As New List(Of BO.GridColumn)
            If GetColPIDsInLine() = "" Then Return lis
            Dim a() As String = Split(GetColPIDsInLine(), ",")
            For i As Integer = 0 To UBound(a)
                Dim b() As String = Split(a(i), "-")
                Dim c As BO.GridColumn = Me.lisDD.Where(Function(p) p.ColumnName = b(0)).First
                If UBound(b) > 0 Then
                    c.MyTag = b(1)
                Else
                    c.MyTag = "all"
                End If

                lis.Add(c)
            Next
            Return lis
        End Get
    End Property


    Private Function GetSumPIDsInLine() As String
        Dim s As String = ""
        For Each it As RadListBoxItem In sumsDest.Items
            If s = "" Then
                s = it.Value
            Else
                s += "," & it.Value
            End If
        Next
        Return s
    End Function
    Private Function GetColPIDsInLine() As String
        Dim s As String = ""
        For Each it As RadListBoxItem In colsDest.Items
            If s = "" Then
                s = it.Value
            Else
                s += "," & it.Value
            End If
            If it.Text.IndexOf("(ALL)") > 0 Then
                s += "-all"
            End If
            If it.Text.IndexOf("(MAX)") > 0 Then
                s += "-max"
            End If
            If it.Text.IndexOf("(MIN)") > 0 Then
                s += "-min"
            End If
        Next
        Return s
    End Function

    Private Sub SetupJ77Combo()
        Dim lisJ77 As IEnumerable(Of BO.j77WorksheetStatTemplate) = Master.Factory.j77WorksheetStatTemplateBL.GetList(New BO.myQuery)
        j77ID.DataSource = lisJ77
        j77ID.DataBind()
        Me.j77ID.Items.Insert(0, "--Výchozí WORKSHEET statistika--")
        basUI.SelectDropdownlistValue(Me.j77ID, Master.DataPID.ToString)
    End Sub
    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If hidIsOwner.Value = "1" Then
                If Not SaveChanges() Then
                    Return
                End If
            End If
            Master.Factory.j03UserBL.SetUserParam("p31_sumgrid-j77id", Master.DataPID.ToString)
            Master.CloseAndRefreshParent("sumgrid_designer")
        End If
    End Sub
    Private Sub cmdDelete_Click(sender As Object, e As ImageClickEventArgs) Handles cmdDelete.Click
        
        If Master.Factory.j77WorksheetStatTemplateBL.Delete(Master.DataPID) Then
            Master.DataPID = 0
            Master.CloseAndRefreshParent("sumgrid_designer")
        Else
            Master.Notify(Master.Factory.j77WorksheetStatTemplateBL.ErrorMessage, 2)
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim strDefDD1 As String = "", strDefDD2 As String = "", strDefSumCols As String = "", strDefAddCols As String = ""
        If Me.j70ID.Items.Count > 0 Then Me.j70ID.SelectedIndex = 0

        If Master.DataPID = 0 Then
            hidIsOwner.Value = "1"
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p31-j70id")
                .Add(hidMasterPrefix.Value & "p31_sumgrid-dd1")
                .Add(hidMasterPrefix.Value & "p31_sumgrid-dd2")
                .Add(hidMasterPrefix.Value & "p31_sumgrid-sumcols")
                .Add(hidMasterPrefix.Value & "p31_sumgrid-addcols")
            End With

            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)

                Select Case Me.hidMasterPrefix.Value
                    Case "j02" : strDefDD1 = "ClientName"
                    Case Else
                        strDefDD1 = "Person"
                End Select
                strDefDD1 = .GetUserParam(hidMasterPrefix.Value & "p31_sumgrid-dd1", strDefDD1)
                strDefDD2 = .GetUserParam(hidMasterPrefix.Value & "p31_sumgrid-dd2")
                strDefSumCols = .GetUserParam(hidMasterPrefix.Value & "p31_sumgrid-sumcols", "1")
                strDefAddCols = .GetUserParam(hidMasterPrefix.Value & "p31_sumgrid-addcols")

            End With
        Else
            If Master.DataPID > -1 Then
                Dim cRec As BO.j77WorksheetStatTemplate = Master.Factory.j77WorksheetStatTemplateBL.Load(Master.DataPID)
                With cRec
                    Me.j77Name.Text = .j77Name
                    strDefDD1 = .j77DD1
                    strDefDD2 = .j77DD2
                    strDefSumCols = .j77SumFields
                    strDefAddCols = .j77ColFields
                    Me.lblTimeStamp.Text = .Timestamp
                    basUI.SelectDropdownlistValue(Me.j70ID, .j70ID.ToString)
                    If .j02ID_Owner = Master.Factory.SysUser.j02ID Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                        hidIsOwner.Value = "1"
                    Else
                        hidIsOwner.Value = "0"
                    End If
                    Me.j77Ordinary.Value = .j77Ordinary
                    basUI.SelectDropdownlistValue(Me.j77TabQueryFlag, .j77TabQueryFlag)
                End With
                roles1.InhaleInitialData(cRec.PID)
            End If
        End If

        SetupGroupByCombo(strDefDD1, strdefdd2)

        SetupSumColsSetting(strDefSumCols, strDefAddCols)
    End Sub

    Private Sub j77ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j77ID.SelectedIndexChanged
        Master.DataPID = BO.BAS.IsNullInt(Me.j77ID.SelectedValue)
        RefreshRecord()
    End Sub

    Private Sub cmdNew_Click(sender As Object, e As ImageClickEventArgs) Handles cmdNew.Click
        Me.j77Name.Text = ""
        Me.j77Name.Focus()
        Me.hidIsOwner.Value = "1"
        Me.j70ID.SelectedIndex = 0
        j77Ordinary.Value = 0

        If Me.j77ID.Items.FindByValue("-1") Is Nothing Then
            Me.j77ID.Items.Insert(0, New ListItem("--Založit novou pojmenovanou šablonu--", "-1"))
        End If
        Me.j77ID.SelectedValue = "-1"
        Master.DataPID = BO.BAS.IsNullInt(Me.j77ID.SelectedValue)

    End Sub

    Private Function SaveChanges() As Boolean
        If Master.DataPID = 0 Then
            With Master.Factory.j03UserBL
                .SetUserParam(hidMasterPrefix.Value & "p31_sumgrid-dd1", Me.dd1.SelectedValue)
                .SetUserParam(hidMasterPrefix.Value & "p31_sumgrid-dd2", Me.dd2.SelectedValue)
                .SetUserParam(hidMasterPrefix.Value & "p31_sumgrid-sumcols", GetSumPIDsInLine())
                .SetUserParam(hidMasterPrefix.Value & "p31_sumgrid-addcols", GetColPIDsInLine())
            End With
            Return True
        Else
            roles1.SaveCurrentTempData()
            Dim cRec As New BO.j77WorksheetStatTemplate
            cRec.j02ID_Owner = Master.Factory.SysUser.j02ID
            cRec.j03ID = Master.Factory.SysUser.PID

            If Master.DataPID > -1 Then
                cRec = Master.Factory.j77WorksheetStatTemplateBL.Load(Master.DataPID)
            End If
            With cRec
                .j77Name = Me.j77Name.Text
                .j77DD1 = Me.dd1.SelectedValue
                .j77DD2 = Me.dd2.SelectedValue
                .j77SumFields = GetSumPIDsInLine()
                .j77ColFields = GetColPIDsInLine()
                .j70ID = BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
                .j77Ordinary = BO.BAS.IsNullInt(Me.j77Ordinary.Value)
                .j77TabQueryFlag = Me.j77TabQueryFlag.SelectedValue
            End With
            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return False
            End If
            With Master.Factory.j77WorksheetStatTemplateBL
                If .Save(cRec, lisX69) Then
                    Master.DataPID = .LastSavedPID
                    Return True
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                    Return False
                End If
            End With
            
        End If
        
    End Function

    Private Sub sumgrid_designer_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Master.DataPID = 0 Then
            Me.j77Name.Visible = False
        Else
            Me.j77Name.Visible = True
        End If
        lblName.Visible = Me.j77Name.Visible
        panRoles.Visible = Me.j77Name.Visible
        panJ70ID.Visible = Me.j77Name.Visible

        cmdDelete.Visible = False
        Me.j77Name.Enabled = False
        panRoles.Enabled = False
        colsDest.Enabled = False
        sumsDest.Enabled = False
        Me.dd1.Enabled = False
        Me.dd2.Enabled = False
        panJ70ID.Enabled = False

        If Me.hidIsOwner.Value = "1" Then
            Me.j77Name.Enabled = True
            panRoles.Enabled = True
            colsDest.Enabled = True
            sumsDest.Enabled = True
            Me.dd1.Enabled = True
            Me.dd2.Enabled = True
            panJ70ID.Enabled = True
            Master.RenameToolbarButton("ok", "Uložit a spustit statistiku")
            If Master.DataPID > 0 Then
                cmdDelete.Visible = True
            End If
        Else
            Master.RenameToolbarButton("ok", "Spustit statistiku")
        End If
    End Sub

    Private Sub cbxMaxMinAll_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxMaxMinAll.SelectedIndexChanged
        If colsDest.SelectedItem Is Nothing Then
            Master.Notify("Není vybrán sloupec.", NotifyLevel.WarningMessage)
        Else
            Dim s As String = lisDD.Where(Function(p) p.ColumnName = colsDest.SelectedValue).First.ColumnHeader
            colsDest.SelectedItem.Text = s & " (" & UCase(cbxMaxMinAll.SelectedValue) & ")"
            cbxMaxMinAll.SelectedIndex = 0
        End If
    End Sub
End Class