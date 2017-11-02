Imports Telerik.Web.UI

Public Class datagrid
    Inherits System.Web.UI.UserControl
    Public Event NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs)
    Public Event ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs)
    Public Event NeedFooterSource(footerItem As GridFooterItem, footerDatasource As Object)
    Public Event DetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs)
    Public Event ItemCommand(sender As Object, e As GridCommandEventArgs, strPID As String)
    Public Event SortCommand(SortExpression As String, strOwnerTableName As String)
    Public Event FilterCommand(strFilterFunction As String, strFilterColumn As String, strFilterPattern As String)
    Public Property Skin As String
        Get
            Return grid1.Skin
        End Get
        Set(value As String)
            grid1.Skin = value
        End Set
    End Property
    
    Public Property OnRowSelected As String
        Get
            Return grid1.ClientSettings.ClientEvents.OnRowSelected
        End Get
        Set(value As String)
            grid1.ClientSettings.ClientEvents.OnRowSelected = value

        End Set
    End Property
    Public Property OnRowDblClick As String
        Get
            Return grid1.ClientSettings.ClientEvents.OnRowDblClick
        End Get
        Set(value As String)
            grid1.ClientSettings.ClientEvents.OnRowDblClick = value
        End Set
    End Property

    Public Property PageSize As Integer
        Get
            Return grid1.MasterTableView.PageSize
        End Get
        Set(ByVal value As Integer)
            grid1.MasterTableView.PageSize = value
        End Set
    End Property
    Public Property AllowCustomPaging As Boolean
        Get
            Return grid1.AllowCustomPaging
        End Get
        Set(value As Boolean)
            grid1.AllowCustomPaging = value
        End Set
    End Property
    Public Property AllowCustomSorting As Boolean
        Get
            Return grid1.MasterTableView.AllowCustomSorting
        End Get
        Set(value As Boolean)
            grid1.MasterTableView.AllowCustomSorting = value
        End Set
    End Property
    Public Property VirtualRowCount As Integer
        Get
            Return grid1.MasterTableView.VirtualItemCount
        End Get
        Set(value As Integer)
            grid1.MasterTableView.VirtualItemCount = value
            If value > 100000 Then
                grid1.PagerStyle.Mode = GridPagerMode.NumericPages

            Else
                grid1.PagerStyle.Mode = GridPagerMode.NextPrevAndNumeric
            End If
        End Set
    End Property
    Public Property AllowFilteringByColumn As Boolean
        Get
            Return grid1.MasterTableView.AllowFilteringByColumn
        End Get
        Set(value As Boolean)
            grid1.MasterTableView.AllowFilteringByColumn = value
        End Set
    End Property
    Public Property DataKeyNames As String
        Get
            Return String.Join(",", grid1.MasterTableView.DataKeyNames)
        End Get
        Set(ByVal value As String)
            grid1.MasterTableView.DataKeyNames = Split(value, ",")
        End Set
    End Property
    Public Property ClientDataKeyNames As String
        Get
            Return String.Join(",", grid1.MasterTableView.ClientDataKeyNames)
        End Get
        Set(ByVal value As String)
            grid1.MasterTableView.ClientDataKeyNames = Split(value, ",")
        End Set
    End Property

    Public Property radGridOrig As RadGrid
        Get
            Return grid1
        End Get
        Set(ByVal value As RadGrid)
            grid1 = value
        End Set
    End Property
    Public Overridable Property DataSource As IEnumerable
        Get
            Return grid1.DataSource
        End Get
        Set(ByVal value As IEnumerable)
            grid1.DataSource = value

        End Set
    End Property
    Public Overridable Property DataSourceDataTable As DataTable
        Get
            Return grid1.DataSource
        End Get
        Set(ByVal value As DataTable)
            grid1.DataSource = value

        End Set
    End Property
    Public Property AllowMultiSelect As Boolean
        Get
            Return grid1.AllowMultiRowSelection
        End Get
        Set(value As Boolean)
            grid1.AllowMultiRowSelection = value
        End Set
    End Property
    Public Property PagerAlwaysVisible As Boolean
        Get
            Return grid1.PagerStyle.AlwaysVisible
        End Get
        Set(value As Boolean)
            grid1.PagerStyle.AlwaysVisible = value
        End Set
    End Property

    Public Function GetSelectedPIDs() As List(Of Integer)
        Dim lis As New List(Of Integer)
        Dim ie As IEnumerable(Of GridDataItem) = grid1.MasterTableView.GetSelectedItems.AsEnumerable()
        If ie Is Nothing Then Return lis
        For Each it As GridDataItem In ie
            lis.Add(it.GetDataKeyValue("pid"))
        Next
        Return lis
    End Function
    Public Function GetAllPIDs() As List(Of Integer)
        Dim lis As New List(Of Integer)
        For Each it As GridDataItem In grid1.MasterTableView.Items
            lis.Add(it.GetDataKeyValue("pid"))
        Next
        Return lis
    End Function
    Public ReadOnly Property RowsCount As Integer
        Get
            Return grid1.MasterTableView.Items.Count
        End Get
    End Property

    Public Overridable Sub Rebind(bolKeepSelectedItems As Boolean, Optional intExplicitSelectedPID As Integer = 0)
        Dim ie As IEnumerable(Of GridDataItem) = Nothing
        If grid1.MasterTableView.DetailTables.Count > 0 Then
            grid1.MasterTableView.DetailTables(0).Rebind()
            For Each di As GridDataItem In grid1.MasterTableView.Items
                If di.Expanded Then
                    ''If bolKeepSelectedItems And intExplicitSelectedPID = 0 Then ie = di.ChildItem.NestedTableViews(0).GetSelectedItems.AsEnumerable()
                    di.Expanded = False
                    ''If bolKeepSelectedItems Then
                    ''    If intExplicitSelectedPID <> 0 Then
                    ''        For Each it As GridDataItem In di.ChildItem.NestedTableViews(0).Items
                    ''            If it.GetDataKeyValue("pid") = intExplicitSelectedPID Then
                    ''                it.Selected = True
                    ''                Return
                    ''            End If
                    ''        Next
                    ''    Else
                    ''        For Each it As GridDataItem In ie
                    ''            it.Selected = True
                    ''        Next
                    ''    End If

                    ''End If
                End If
            Next
            Return
        End If

        If bolKeepSelectedItems And intExplicitSelectedPID = 0 Then ie = grid1.MasterTableView.GetSelectedItems.AsEnumerable()

        grid1.Rebind()

        If bolKeepSelectedItems Then
            If intExplicitSelectedPID <> 0 Then
                For Each it As GridDataItem In grid1.MasterTableView.Items
                    If it.GetDataKeyValue("pid") = intExplicitSelectedPID Then
                        it.Selected = True
                        Return
                    End If
                Next
            Else
                For Each it As GridDataItem In ie
                    it.Selected = True
                Next
            End If

        End If
    End Sub

    Public Overloads Sub SelectRecords(lisDataPIDs As List(Of Integer))
        For Each x As Integer In lisDataPIDs
            For Each it As GridDataItem In grid1.MasterTableView.Items
                If it.GetDataKeyValue("pid") = x Then
                    it.Selected = True : Exit For
                End If
            Next
        Next

    End Sub
    Public Overloads Sub SelectRecords(intOnePID As Integer)
        If intOnePID = 0 Then Return
        For Each it As GridDataItem In grid1.MasterTableView.Items
            If it.GetDataKeyValue("pid") = intOnePID Then
                it.Selected = True : Exit For

            End If
        Next
    End Sub

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        SetupGrid()
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

        End If
    End Sub

    Public Sub ClearColumns()
        grid1.Columns.Clear()
        grid1.MasterTableView.GroupByExpressions.Clear()
    End Sub

    Private Sub SetupGrid()


        grid1.PagerStyle.PageSizeLabelText = ""
        If Page.Culture.IndexOf("Czech") >= 0 Or Page.Culture.IndexOf("Če") >= 0 Then
            With grid1.PagerStyle
                .LastPageToolTip = "Poslední strana"
                .FirstPageToolTip = "První strana"
                .PrevPageToolTip = "Předchozí strana"
                .NextPageToolTip = "Další strana"
                .PagerTextFormat = "{4} Strana {0}/{1}, {2} - {3} z {5}"
            End With
            With grid1.SortingSettings
                .SortToolTip = "Klikněte zde pro třídění"
                .SortedDescToolTip = "Setříděno sestupně"
                .SortedAscToolTip = "Setříděno vzestupně"
            End With
            With grid1.GroupingSettings
                .CollapseTooltip = "Sbalit řádky"
                .ExpandTooltip = "Rozbalit řádky"
            End With
        End If


        


        With grid1.MasterTableView

            .NoMasterRecordsText = Resources.common.Grid_ZadneZaznamy
        End With
    End Sub
    Public Sub AddCheckboxSelector()
        Dim col As New GridClientSelectColumn()
        grid1.MasterTableView.Columns.Add(col)
        col.HeaderStyle.Width = Unit.Parse("25px")
        ''col.ItemStyle.Width = Unit.Parse("25px")

    End Sub
    Public Sub AddButton(strText As String, strCommandName As String, strHeaderText As String, Optional strImageUrl As String = "")
        Dim cmd As New GridButtonColumn
        grid1.MasterTableView.Columns.Add(cmd)
        With cmd
            .CommandName = strCommandName
            .UniqueName = strCommandName
            .Text = strText
            .HeaderText = strHeaderText
            .ImageUrl = strImageUrl
            .ItemStyle.Width = Unit.Parse("16px")
            .HeaderStyle.Width = Unit.Parse("16px")
        End With
    End Sub
    Public Sub AddLink(strText As String, strCommandName As String, strHeaderText As String, Optional strImageUrl As String = "", Optional strNavigateUrl As String = "", Optional strField As String = "")
        Dim cmd As New GridHyperLinkColumn

        grid1.MasterTableView.Columns.Add(cmd)
        With cmd
            .UniqueName = strCommandName
            .Text = strText
            .HeaderText = strHeaderText
            .ImageUrl = strImageUrl
            .ItemStyle.Width = Unit.Parse("16px")
            .HeaderStyle.Width = Unit.Parse("16px")
            .NavigateUrl = strNavigateUrl
            .DataTextField = strField
        End With
    End Sub

    Public Sub AddTextboxColumn(strField As String, strHeader As String, strColumnEditorID As String, bolAllowSorting As Boolean, Optional ByVal strUniqueName As String = "")
        Dim col As New GridBoundColumn

        grid1.MasterTableView.Columns.Add(col)
        col.HeaderText = strHeader
        col.DataField = strField
        col.ColumnEditorID = strColumnEditorID
        If strUniqueName <> "" Then
            col.UniqueName = strUniqueName
            col.SortExpression = strUniqueName
        End If
        col.AllowSorting = bolAllowSorting
    End Sub
    Public Sub AddNumbericTextboxColumn(strField As String, strHeader As String, strColumnEditorID As String, bolAllowSorting As Boolean, Optional ByVal strUniqueName As String = "")
        Dim col As New GridNumericColumn

        grid1.MasterTableView.Columns.Add(col)
        col.HeaderText = strHeader
        col.DataField = strField
        col.ColumnEditorID = strColumnEditorID
        If strUniqueName <> "" Then
            col.UniqueName = strUniqueName
            col.SortExpression = strUniqueName
        End If
        col.AllowSorting = bolAllowSorting
    End Sub


    Public Sub AddColumn(ByVal strField As String, ByVal strHeader As String, Optional ByVal colformat As BO.cfENUM = BO.cfENUM.AnyString, Optional ByVal bolAllowSorting As Boolean = True, Optional ByVal bolVisible As Boolean = True, Optional ByVal strUniqueName As String = "", Optional strHeaderTooltip As String = "", Optional bolShowTotals As Boolean = False, Optional bolAllowFiltering As Boolean = True, Optional strGroupName As String = "", Optional gtv As GridTableView = Nothing)
        Select Case colformat
            Case BO.cfENUM.Checkbox
                AddCheckboxColumn(strField, strHeader, bolAllowSorting, bolVisible)
                Return
            Case Else
        End Select
        Dim col As New GridBoundColumn
        If Not gtv Is Nothing Then
            gtv.Columns.Add(col)
        Else
            grid1.MasterTableView.Columns.Add(col)
        End If

        col.HeaderText = strHeader
        col.DataField = strField
        col.HeaderTooltip = strHeaderTooltip
        col.ReadOnly = True
        If strUniqueName <> "" Then
            col.UniqueName = strUniqueName
            col.SortExpression = strUniqueName
        End If
        col.AllowSorting = bolAllowSorting
        col.Visible = bolVisible
        col.AllowFiltering = bolAllowFiltering
        If colformat = BO.cfENUM.AnyString And bolAllowFiltering Then
            col.AutoPostBackOnFilter = True
            col.CurrentFilterFunction = GridKnownFunction.Contains
        End If
        If strGroupName <> "" Then col.ColumnGroupName = strGroupName
        Select Case colformat
            Case BO.cfENUM.DateOnly
                col.DataFormatString = "{0:dd.MM.yyyy}"
                col.DataType = Type.GetType("System.DateTime")

            Case BO.cfENUM.DateTime
                col.DataFormatString = "{0:dd.MM.yyyy HH:mm}"
            Case BO.cfENUM.Numeric, BO.cfENUM.Numeric2
                'col.DataFormatString = "{0:F2}"
                col.DataFormatString = "{0:###,##0.00}"
                col.DataType = System.Type.GetType("System.Double")
                col.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                If bolShowTotals Then
                    'col.DefaultInsertValue = "sum"
                    col.Aggregate = GridAggregateFunction.Sum

                End If
                col.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
            Case BO.cfENUM.Numeric3
                col.DataFormatString = "{0:###,##0.000}"
                col.DataType = System.Type.GetType("System.Double")
                col.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                If bolShowTotals Then
                    'col.DefaultInsertValue = "sum"
                    col.Aggregate = GridAggregateFunction.Sum
                End If
                col.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
            Case BO.cfENUM.Numeric0
                col.DataType = System.Type.GetType("System.Int32")
                col.DataFormatString = "{0:F0}"
                col.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                If bolShowTotals Then
                    col.Aggregate = GridAggregateFunction.Sum
                End If
                col.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
            Case BO.cfENUM.TimeOnly
                col.DataFormatString = "{0:HH:mm}"

        End Select

    End Sub
    Private Sub AddCheckboxColumn(ByVal strField As String, ByVal strHeader As String, Optional ByVal bolAllowSorting As Boolean = True, Optional ByVal bolVisible As Boolean = True)
        Dim col As GridCheckBoxColumn
        col = New GridCheckBoxColumn

        grid1.MasterTableView.Columns.Add(col)
        col.HeaderText = strHeader
        col.DataField = strField
        col.AllowSorting = bolAllowSorting
        col.Visible = bolVisible
    End Sub

    Public Sub AddSystemColumn(ByVal intWidth As Integer, Optional strFieldName As String = "systemcolumn", Optional gtv As GridTableView = Nothing)
        Dim col As GridBoundColumn
        col = New GridBoundColumn
        If Not gtv Is Nothing Then
            gtv.Columns.Add(col)
        Else
            grid1.MasterTableView.Columns.Add(col)
        End If
        col.DataField = strFieldName
        col.AllowFiltering = False
        col.AllowSorting = False
        col.HeaderStyle.Width = Unit.Parse(intWidth.ToString & "px")
        col.Exportable = False
        col.ReadOnly = True
        col.ItemStyle.CssClass = "systemcolumn"
    End Sub
    Public Sub AddContextMenuColumn(ByVal intWidth As Integer, Optional gtv As GridTableView = Nothing)
        Dim col As GridBoundColumn
        col = New GridBoundColumn
        If Not gtv Is Nothing Then
            gtv.Columns.Add(col)
        Else
            grid1.MasterTableView.Columns.Add(col)
        End If
        col.UniqueName = "pm1"
        col.AllowFiltering = False
        col.AllowSorting = False
        col.HeaderStyle.Width = Unit.Parse(intWidth.ToString & "px")
        col.Exportable = False
        col.ReadOnly = True
        col.ItemStyle.CssClass = "systemcolumn"
    End Sub

    Private Sub grid1_BiffExporting(sender As Object, e As Telerik.Web.UI.GridBiffExportingEventArgs) Handles grid1.BiffExporting
        For i = 1 To e.ExportStructure.Tables(0).Columns.Count
            e.ExportStructure.Tables(0).Columns(i).Style.HorizontalAlign = HorizontalAlign.Left

        Next

    End Sub




    Private Sub grid1_DataBound(sender As Object, e As System.EventArgs) Handles grid1.DataBound
        If Not grid1.ShowFooter Then Return
        If grid1.MasterTableView.GetItems(GridItemType.Footer).Count = 0 Then Return
        Dim footerItem As GridFooterItem = grid1.MasterTableView.GetItems(GridItemType.Footer)(0)
        RaiseEvent NeedFooterSource(footerItem, Nothing)

    End Sub

    Private Sub grid1_DetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs) Handles grid1.DetailTableDataBind
        RaiseEvent DetailTableDataBind(sender, e)
    End Sub

    

    

    Private Sub grid1_Init(sender As Object, e As EventArgs) Handles grid1.Init
        If grid1.AllowFilteringByColumn Then Return
        Dim menu As GridFilterMenu = grid1.FilterMenu
        Dim i As Integer = 0
        With menu.Items
            While i < .Count
                If Page.Culture.IndexOf("Czech") >= 0 Or Page.Culture.IndexOf("Če") >= 0 Then
                    With .Item(i)
                        Select Case .Text
                            Case "NoFilter" : .Text = "Nefiltrovat" : i += 1
                            Case "Contains" : .Text = "Obsahuje" : i += 1
                            Case "EqualTo" : .Text = "Je rovno" : i += 1
                            Case "GreaterThan" : .Text = "Je větší než" : i += 1
                            Case "LessThan" : .Text = "Je menší než" : i += 1
                            Case "IsNull" : .Text = "Je prázdné" : i += 1
                            Case "NotIsNull" : .Text = "Není prázdné" : i += 1
                            Case "StartsWith" : .Text = "Začíná na" : i += 1
                            Case Else
                                menu.Items.RemoveAt(i)
                        End Select
                    End With
                Else
                    With .Item(i)
                        Select Case .Text
                            Case "NoFilter", "Contains", "EqualTo", "GreaterThan", "LessThan", "IsNull", "NotIsNull", "StartsWith"
                                i += 1
                            Case Else
                                menu.Items.RemoveAt(i)
                        End Select
                    End With
                End If
                
            End While
        End With

    End Sub



    Private Sub grid1_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles grid1.ItemCommand
        If Not (TypeOf e.Item Is GridPagerItem Or TypeOf e.Item Is GridHeaderItem) Then
            RaiseEvent ItemCommand(sender, e, grid1.Items.Item(e.Item.ItemIndex).GetDataKeyValue("pid"))
        End If
        If e.CommandName = RadGrid.FilterCommandName Then
            Dim filterPair As Pair = DirectCast(e.CommandArgument, Pair)
            Dim ctl As Control = (CType(e.Item, GridFilteringItem))(filterPair.Second.ToString()).Controls(0)
            If TypeOf ctl Is TextBox Then
                RaiseEvent FilterCommand(filterPair.First, filterPair.Second, CType(ctl, TextBox).Text)
            End If
            If TypeOf ctl Is CheckBox Then
                RaiseEvent FilterCommand(filterPair.First, filterPair.Second, BO.BAS.GB(CType(ctl, CheckBox).Checked))
            End If

        End If

        If e.CommandName = RadGrid.ExpandCollapseCommandName Then
            Dim item As GridItem
            For Each item In e.Item.OwnerTableView.Items
                If item.Expanded AndAlso Not item Is e.Item Then
                    item.Expanded = False
                End If
            Next item
        End If
    End Sub

    Private Sub grid1_ItemCreated(sender As Object, e As GridItemEventArgs) Handles grid1.ItemCreated
        If TypeOf e.Item Is GridFilteringItem Then
            For Each col As GridColumn In grid1.MasterTableView.Columns
                DirectCast(e.Item, GridFilteringItem)(col.UniqueName).HorizontalAlign = col.HeaderStyle.HorizontalAlign
                If Not String.IsNullOrEmpty(grid1.MasterTableView.FilterExpression) Then
                    If Not String.IsNullOrEmpty(col.CurrentFilterValue) Or col.CurrentFilterFunction = GridKnownFunction.IsNull Or col.CurrentFilterFunction = GridKnownFunction.NotIsNull Then
                        DirectCast(e.Item, GridFilteringItem)(col.UniqueName).BackColor = basUI.ColorQueryRGB
                    End If
                End If
            Next
            ''If Not String.IsNullOrEmpty(grid1.MasterTableView.FilterExpression) Then
            ''    For Each col As GridColumn In grid1.MasterTableView.Columns
            ''        If Not String.IsNullOrEmpty(col.CurrentFilterValue) Or col.CurrentFilterFunction = GridKnownFunction.IsNull Or col.CurrentFilterFunction = GridKnownFunction.NotIsNull Then
            ''            DirectCast(e.Item, GridFilteringItem)(col.UniqueName).BackColor = Drawing.Color.Red
            ''        End If
            ''    Next
            ''End If
        End If
        If grid1.IsExporting Then
            If TypeOf e.Item Is GridDataItem OrElse TypeOf e.Item Is GridHeaderItem Then
                e.Item.Cells(0).Visible = False
            End If
        End If
    End Sub



    Private Sub grid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles grid1.ItemDataBound
        If TypeOf e.Item Is GridPagerItem Then
            If Not e.Item.FindControl("PageSizeComboBox") Is Nothing Then
                e.Item.FindControl("PageSizeComboBox").Visible = False
            End If

        End If


        RaiseEvent ItemDataBound(sender, e)
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        RaiseEvent NeedDataSource(sender, e)

    End Sub

    Public Function GenerateFooterItemString(cSum As Object) As String
        Dim lis As New List(Of String)
        For Each col In grid1.MasterTableView.Columns
            If TypeOf col Is GridBoundColumn Then
                If col.Aggregate = GridAggregateFunction.Sum Then
                    Dim o As Object = BO.BAS.GetPropertyValue(cSum, col.DataField)
                    If Not o Is Nothing Then
                        lis.Add(col.DataField & ";" & BO.BAS.FN(o))
                        ''s += "|" & col.DataField & ";" & BO.BAS.FN(o)
                    End If
                End If
            End If
        Next
        'Return BO.BAS.OM1(s)
        Return String.Join("|", lis)
    End Function
    Public Function CompleteFooterString(dt As DataTable, strSumFields As String, Optional intFirstSumColZeroIndex As Integer = 1) As String
        Dim lis As New List(Of String)

        For i As Integer = intFirstSumColZeroIndex To dt.Columns.Count - 1
            If Not dt.Rows(0).Item(i) Is System.DBNull.Value Then lis.Add(dt.Columns(i).ColumnName & ";" & BO.BAS.FN(dt.Rows(0).Item(i)))
        Next
        Return String.Join("|", lis)
    End Function
    

    Public Sub ParseFooterItemString(footerItem As GridFooterItem, strFooterString As String)
        If strFooterString = "" Then Return
        Dim a() As String = Split(strFooterString, "|")
        With grid1.MasterTableView.Columns
            For Each strPair As String In a
                Dim b() As String = Split(strPair, ";")
                Dim col As GridColumn = .FindByDataField(b(0))
                If Not col Is Nothing Then footerItem.Item(col).Text = b(1)
            Next
        End With
    End Sub

    Private Sub grid1_PageIndexChanged(sender As Object, e As GridPageChangedEventArgs) Handles grid1.PageIndexChanged

    End Sub

    

    

    Private Sub grid1_SortCommand(sender As Object, e As GridSortCommandEventArgs) Handles grid1.SortCommand
        Select Case e.NewSortOrder
            Case GridSortOrder.Ascending
                RaiseEvent SortCommand(e.SortExpression, e.Item.OwnerTableView.Name)
            Case GridSortOrder.Descending
                RaiseEvent SortCommand(e.SortExpression & " DESC", e.Item.OwnerTableView.Name)
            Case GridSortOrder.None
                RaiseEvent SortCommand("", e.Item.OwnerTableView.Name)
        End Select

    End Sub

    Public Function GetFilterExpressionCompleteSql() As String
        With grid1.MasterTableView
            If .FilterExpression = "" Then
                Return ""
            Else
                Dim s As New List(Of String)
                For Each col As GridColumn In .Columns
                    If Not String.IsNullOrEmpty(col.CurrentFilterValue) Or col.CurrentFilterFunction = GridKnownFunction.IsNull Or col.CurrentFilterFunction = GridKnownFunction.NotIsNull Then
                        Dim strVal As String = col.CurrentFilterValue, strOPER As String = ""
                        Dim strColType As String = col.DataTypeName
                        If TypeOf col Is GridBoundColumn Then
                            If CType(col, GridBoundColumn).DataFormatString.IndexOf("dd.") > 0 Then strColType = "System.DateTime"
                        End If
                        Select Case strColType
                            Case "System.Double", "System.Int"
                                If Not IsNumeric(strVal) Then strVal = "0"
                                strVal = Replace(strVal, ",", ".")
                            Case "System.DateTime"
                                If strVal.IndexOf(".") > 0 Then
                                    Dim d As Date? = BO.BAS.ConvertString2Date(strVal)
                                    If Not d Is Nothing Then
                                        strVal = "'" & Month(d).ToString & "/" & Day(d).ToString & "/" & Year(d).ToString & "'"
                                    End If
                                Else
                                    Dim a() As String = Split(strVal & "///", "/")
                                    If Not IsNumeric(a(0)) Then a(0) = "12"
                                    If Not IsNumeric(a(1)) Then a(1) = "31"
                                    If Not IsNumeric(a(2)) Then a(2) = "1900"
                                    strVal = "'" & a(0) & "/" & a(1) & "/" & a(2) & "'"
                                End If

                            Case "System.String"
                                Select Case col.CurrentFilterFunction
                                    Case GridKnownFunction.Contains
                                        strVal = "'%" & strVal & "%'"
                                    Case GridKnownFunction.StartsWith
                                        strVal = "'" & strVal & "%'"
                                    Case Else
                                        strVal = BO.BAS.GS(strVal)
                                End Select
                            Case "System.Boolean"
                                strVal = Replace(strVal, "True", "1")
                                strVal = Replace(strVal, "False", "0")
                        End Select
                        Select Case col.CurrentFilterFunction
                            Case GridKnownFunction.Contains, GridKnownFunction.StartsWith, GridKnownFunction.EndsWith : strOPER = "LIKE"
                            Case GridKnownFunction.EqualTo : strOPER = "="
                            Case GridKnownFunction.GreaterThan : strOPER = ">"
                            Case GridKnownFunction.GreaterThanOrEqualTo : strOPER = ">="
                            Case GridKnownFunction.LessThan : strOPER = "<"
                            Case GridKnownFunction.LessThanOrEqualTo : strOPER = "<="
                            Case GridKnownFunction.IsNull
                                strOPER = "IS NULL" : strVal = ""
                            Case GridKnownFunction.NotIsNull
                                strOPER = "IS NOT NULL" : strVal = ""
                        End Select
                        s.Add(col.UniqueName & " " & strOPER & " " & strVal)

                    End If

                Next
                Return System.String.Join(" AND ", s)
            End If
        End With
    End Function

    Public Function GetFilterExpression() As String
        With grid1.MasterTableView
            If .FilterExpression = "" Then
                Return ""
            Else
                If .FilterExpression.IndexOf(",") >= 0 Then .FilterExpression = .FilterExpression.Replace(",", ".")
                .FilterExpression = .FilterExpression.Replace("True", "1")
                ''If strReplaceDbField <> "" Then
                ''    Dim matches As MatchCollection = Regex.Matches(.FilterExpression, "\[.*?\]")
                ''    For Each m As Match In matches
                ''        'Dim strField As String = Replace(m.Value, "[", "").Replace("]", "")
                ''        .FilterExpression = Replace(.FilterExpression, m.Value, strReplaceDbField)
                ''    Next
                ''End If
                

                Return .FilterExpression

            End If
        End With
    End Function
    Public Sub ClearFilter()
        For Each col As GridColumn In grid1.MasterTableView.Columns
            col.CurrentFilterValue = ""
            col.CurrentFilterFunction = GridKnownFunction.NoFilter
        Next
        grid1.MasterTableView.FilterExpression = ""
    End Sub
    Public Function GetFilterSetting() As String
        Dim s As New List(Of String)
        For Each col As GridColumn In grid1.MasterTableView.Columns
            If Not String.IsNullOrEmpty(col.CurrentFilterValue) Or col.CurrentFilterFunction = GridKnownFunction.IsNull Or col.CurrentFilterFunction = GridKnownFunction.NotIsNull Then
                s.Add(col.UniqueName & "##" & col.CurrentFilterFunction & "##" & col.CurrentFilterValue)
            End If

        Next
        Return System.String.Join("|", s)
    End Function
    Public Sub SetFilterSetting(strSetting As String, strFilterExpression As String)
        If strSetting = "" Then Return
        Dim i As Integer = strSetting.IndexOf("$$")
        If i >= 0 Then
            strSetting = Split(strSetting, "$$")(1)
        End If
        Dim a() As String = Split(strSetting, "|")

        For Each s In a
            Dim b() As String = Split(s, "##")
            With grid1.MasterTableView.Columns
                Try
                    Dim col As GridColumn = .FindByUniqueName(b(0))
                    If Not col Is Nothing Then
                        col.CurrentFilterValue = b(2)
                        col.CurrentFilterFunction = DirectCast(CInt(b(1)), GridKnownFunction)
                    End If
                Catch ex As Exception
                    'sloupec neexistuje, raději vyčistíme celý filtr
                    strFilterExpression = ""
                End Try
            End With
        Next

        grid1.MasterTableView.FilterExpression = strFilterExpression

    End Sub

   
End Class