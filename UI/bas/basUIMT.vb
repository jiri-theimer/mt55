Imports Telerik.Web.UI
Public Class basUIMT
    Public Shared ReadOnly Property TreeColorLevel1 As System.Drawing.Color
        Get
            Return Drawing.Color.MediumVioletRed
        End Get
    End Property
    Public Shared ReadOnly Property TreeColorLevel2 As System.Drawing.Color
        Get
            Return Drawing.Color.BlueViolet
        End Get
    End Property

    Public Shared Function PrepareDataGrid(cS As SetupDataGrid) As PreparedDataGrid
        If cS.cJ70.j70ScrollingFlag = BO.j70ScrollingFlagENUM.Scrolling Then cS.cJ70.j70ScrollingFlag = BO.j70ScrollingFlagENUM.StaticHeaders
        Dim lisSqlSEL As New List(Of String) 'vrací Sql SELECT syntaxi pro datový zdroj GRIDu
        Dim lisSqlSumCols As New List(Of String)
        Dim lisSqlFROM As New List(Of String)   'další nutné SQL FROM klauzule
        With cS.grid
            .ClearColumns()
            .AllowMultiSelect = cS.AllowMultiSelect
            .DataKeyNames = "pid"
            .AllowCustomSorting = True



            .AllowCustomPaging = cS.AllowCustomPaging
            'If cS.AllowMultiSelect And cS.AllowMultiSelectCheckboxSelector Then .AddCheckboxSelector()
            If cS.AllowMultiSelectCheckboxSelector Then .AddCheckboxSelector()


            .PageSize = cS.PageSize
            If cS.SysColumnWidth > 0 Then
                .AddSystemColumn(cS.SysColumnWidth)
            End If
            If cS.ContextMenuWidth > 0 Then
                .AddContextMenuColumn(cS.ContextMenuWidth)
            End If

            .radGridOrig.PagerStyle.Mode = Telerik.Web.UI.GridPagerMode.NextPrevAndNumeric
            .AllowFilteringByColumn = cS.cJ70.j70IsFilteringByColumn
            Select Case cS.cJ70.j70ScrollingFlag
                Case BO.j70ScrollingFlagENUM.Scrolling
                    .radGridOrig.ClientSettings.Scrolling.AllowScroll = True
                    .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = False
                Case BO.j70ScrollingFlagENUM.StaticHeaders
                    .radGridOrig.ClientSettings.Scrolling.AllowScroll = True
                    .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = True
                Case Else
                    .radGridOrig.ClientSettings.Scrolling.AllowScroll = False
                    .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = False
            End Select

            .radGridOrig.MasterTableView.Name = "grid"
            If cS.SortExpression <> "" Then .radGridOrig.MasterTableView.SortExpressions.AddSortExpression(cS.SortExpression)


            Dim lisCols As List(Of BO.GridColumn) = cS.factory.j70QueryTemplateBL.ColumnsPallete(cS.cJ70.x29ID), bolMobile As Boolean = False
            Select Case Left(cS.MasterPrefix, 3)
                Case "p41"
                    lisCols = lisCols.Where(Function(p) p.TreeGroup <> "Projekt" And p.TreeGroup <> "Project").ToList   'nezobrazovat sloupce projektu, když uživatel stojí v pod-přehledu v rámci projektu
                Case "j02"
                    lisCols = lisCols.Where(Function(p) p.TreeGroup <> "Osoba" And p.TreeGroup <> "Person").ToList   'nezobrazovat sloupce projektu, když uživatel stojí v pod-přehledu v rámci projektu
                Case "p28"
                    lisCols = lisCols.Where(Function(p) p.ColumnName <> "ClientName" And p.TreeGroup <> "Project").ToList   'nezobrazovat sloupce projektu, když uživatel stojí v pod-přehledu v rámci projektu

            End Select
            If cS.cJ70.j70MasterPrefix = "mobile_grid" Then
                bolMobile = True
            End If
            Dim intIndex As Integer = 0
            For Each s In Split(cS.cJ70.j70ColumnNames, ",")
                Dim strField As String = Trim(s)

                Dim c As BO.GridColumn = lisCols.Find(Function(p) p.ColumnName = strField)

                If Not c Is Nothing Then
                    .AddColumn(c.ColumnName, c.ColumnHeader, c.ColumnType, c.IsSortable, , c.ColumnDBName, , c.IsShowTotals, c.IsAllowFiltering)

                    lisSqlSEL.Add(c.ColumnSqlSyntax_Select)
                    If c.IsShowTotals Then
                        If c.ColumnDBName <> "" Then
                            lisSqlSumCols.Add("sum(" & c.ColumnDBName & ") as " & c.ColumnName)
                        Else
                            lisSqlSumCols.Add("sum(" & c.ColumnName & ") as " & c.ColumnName)
                        End If
                    End If

                    If c.SqlSyntax_FROM <> "" Then lisSqlFROM.Add(c.SqlSyntax_FROM)
                End If
                intIndex += 1
            Next
            cS.grid.SetFilterSetting(cS.FilterSetting, cS.FilterExpression)

            If cS.cJ70.j70OrderBy <> "" Then
                Dim a() As String = Split(cS.cJ70.j70OrderBy, ",")
                For i As Integer = 0 To UBound(a)
                    Dim strC As String = a(i)
                    If strC.IndexOf(".") > 0 Then
                        strC = Split(strC, ".")(1)
                    End If
                    Dim c As BO.GridColumn = lisCols.Find(Function(p) p.ColumnName = strC And p.SqlSyntax_FROM <> "")
                    If Not c Is Nothing Then
                        lisSqlFROM.Add(c.SqlSyntax_FROM)
                    End If
                Next
            End If
        End With

        Dim cRet As New PreparedDataGrid

        If lisSqlFROM.Count > 0 Then cRet.AdditionalFROM = String.Join(" ", lisSqlFROM.Distinct)
        cRet.SumCols = String.Join("|", lisSqlSumCols)
        cRet.Cols = String.Join(",", lisSqlSEL)

        Return cRet

    End Function
    Public Shared Function SetupDataGrid(factory As BL.Factory, grid As UI.datagrid, cJ70 As BO.j70QueryTemplate, intPageSize As Integer, bolCustomPaging As Boolean, bolAllowMultiSelect As Boolean, Optional bolMultiSelectCheckboxSelector As Boolean = True, Optional strFilterSetting As String = "", Optional strFilterExpression As String = "", Optional strSortExpression As String = "", Optional ByRef strGetAdditionalFROM As String = "", Optional intSysColumnWidth As Integer = 16, Optional ByRef strGetSumCols As String = "", Optional strMasterPrefix As String = "") As String
        Dim cSetup As New SetupDataGrid(factory, grid, cJ70)
        With cSetup
            .PageSize = intPageSize
            .AllowCustomPaging = bolCustomPaging
            .AllowMultiSelect = bolAllowMultiSelect
            .AllowMultiSelectCheckboxSelector = bolAllowMultiSelect
            .FilterSetting = strFilterSetting
            .FilterExpression = strFilterExpression
            .SortExpression = strSortExpression
            .SysColumnWidth = intSysColumnWidth
            .MasterPrefix = strMasterPrefix
        End With

        Dim cRet As PreparedDataGrid = PrepareDataGrid(cSetup)
        strGetAdditionalFROM = cRet.AdditionalFROM
        strGetSumCols = cRet.SumCols
        Return cRet.Cols
    End Function
    Public Shared Sub MakeDockZonesUserFriendly(rdl As RadDockLayout, bolLockedInteractivity As Boolean)

        For Each zone In rdl.RegisteredZones
            zone.BorderStyle = BorderStyle.Solid
            zone.BorderColor = Drawing.Color.Silver
            Dim dh As DockHandle = DockHandle.TitleBar
            If bolLockedInteractivity Then
                dh = DockHandle.None
                zone.MinHeight = Nothing
                zone.MinWidth = Nothing
                zone.BorderStyle = BorderStyle.None
            End If
            For Each d In zone.Docks
                d.DockHandle = dh

            Next
        Next
    End Sub

    Public Shared Sub p91_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolDT As Boolean, Optional strMobileLinkColumn As String = "")
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        If bolDT Then
            Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
            If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu
            With dataItem("pm1")
                .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p91','" & cRec.Item("pid").ToString & "',this)" & Chr(34) & "></a>"
            End With

            With cRec
                If .Item("IsDraft") Then
                    dataItem("systemcolumn").CssClass = "draft"
                Else
                    If .Item("Debt") > 10 Then
                        If .Item("Maturity") >= Today Then
                            dataItem("systemcolumn").CssClass = "p91_yellow"
                        Else
                            If Math.Abs(.Item("TotalDue") - .Item("Debt")) < 50 Then
                                dataItem("systemcolumn").CssClass = "p91_red"
                            Else
                                dataItem("systemcolumn").CssClass = "p91_pink"
                            End If
                        End If
                    End If
                End If
                If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
                If cRec.Item("InvoiceType") = BO.p92InvoiceTypeENUM.CreditNote Then
                    'dobropis - opravný doklad
                    dataItem("systemcolumn").CssClass = "p91_creditnote"
                End If
                If strMobileLinkColumn <> "" Then
                    dataItem(strMobileLinkColumn).Text = "<a style='color:blue;text-decoration:underline;' href='javascript:re(" & cRec.Item("pid").ToString & ")'>" & dataItem(strMobileLinkColumn).Text & "</a>"
                End If
            End With
        Else
            Dim cRec As BO.p91Invoice = CType(e.Item.DataItem, BO.p91Invoice)
            With cRec
                If .p91IsDraft Then
                    dataItem("systemcolumn").CssClass = "draft"
                Else
                    If .p91Amount_Debt > 10 Then
                        If .p91DateMaturity >= Today Then
                            dataItem("systemcolumn").CssClass = "p91_yellow"
                        Else
                            If Math.Abs(.p91Amount_TotalDue - .p91Amount_Debt) < 50 Then
                                dataItem("systemcolumn").CssClass = "p91_red"
                            Else
                                dataItem("systemcolumn").CssClass = "p91_pink"
                            End If
                        End If
                    End If
                End If
                If cRec.IsClosed Then dataItem.Font.Strikeout = True
                If cRec.p92InvoiceType = BO.p92InvoiceTypeENUM.CreditNote Then
                    'dobropis - opravný doklad
                    dataItem("systemcolumn").CssClass = "p91_creditnote"
                End If
            End With
        End If

    End Sub
    Public Shared Sub x40_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs)
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.x40MailQueue = CType(e.Item.DataItem, BO.x40MailQueue)
        dataItem("UserInsert").Text = ""  'náhrada za systemcolumn
        If cRec.x40Attachments > "" Then
            dataItem("UserInsert").CssClass = "attachment"

        End If
        dataItem("x40State").Text = cRec.StatusAlias
        dataItem.Style.Item("color") = cRec.StatusColor
        With dataItem.Item("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('x40','" & cRec.PID.ToString & "',this)" & Chr(34) & "></a>"
        End With
        
    End Sub
    Public Shared Sub o43_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs)
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        Dim cRec As BO.o43ImapRobotHistory = CType(e.Item.DataItem, BO.o43ImapRobotHistory)
        dataItem("UserInsert").Text = ""  'náhrada za systemcolumn
        If cRec.o43Attachments > "" Then
            dataItem("UserInsert").CssClass = "attachment"

        End If
        dataItem("UserInsert").Text = "<a style='float:right;' title='Otevřít v MS-OUTLOOK' href='binaryfile.aspx?prefix=o43&format=msg&pid=" & cRec.o43ID.ToString & "'><img src='Images/files/msg_24.png'/></a>"
        dataItem("UserInsert").Text += "<a style='float:right;' title='EML formát zprávy' href='binaryfile.aspx?prefix=o43&format=eml&pid=" & cRec.o43ID.ToString & "'><img src='Images/files/eml_24.png'/></a>"

        If cRec.p56ID > 0 Then
            dataItem("EntityName").Text = "<a href='p56_framework.aspx?pid=" & cRec.p56ID.ToString & "'>ÚKOL</a>"
        End If
        If cRec.o23ID > 0 Then
            dataItem("EntityName").Text = "<a href='o23_fixwork.aspx?pid=" & cRec.o23ID.ToString & "'>DOKUMENT</a>"
        End If
    End Sub

    Public Shared Sub p56_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolShowClueTip As Boolean, bolDT As Boolean, strMobileLinkColumn As String, strContextMenuFlag As String)
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)

        If bolDT Then
            Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
            If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu
            If bolShowClueTip Then

            End If
            With dataItem("pm1")
                .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p56','" & cRec.Item("pid").ToString & "',this)" & Chr(34) & "></a>"
            End With
            With cRec
                If .Item("IsClosed") Then
                    dataItem.Font.Strikeout = True
                Else
                    If Not .Item("p56PlanUntil_Grid") Is System.DBNull.Value Then
                        If Now > .Item("p56PlanUntil_Grid") Then dataItem("systemcolumn").CssClass = "overtime"
                    End If
                End If
                If BO.BAS.IsNullInt(.Item("b02ID_Grid")) > 0 Then
                    If .Item("b02Color_Grid") & "" <> "" Then dataItem("systemcolumn").Style.Item("background-color") = .Item("b02Color_Grid")
                End If
                If strMobileLinkColumn <> "" Then
                    dataItem(strMobileLinkColumn).Text = "<a style='color:blue;text-decoration:underline;' href='javascript:re(" & cRec.Item("pid").ToString & ")'>" & dataItem(strMobileLinkColumn).Text & "</a>"
                End If
                If Not .Item("o43ID") Is System.DBNull.Value Then
                    dataItem("systemcolumn").CssClass = "imap"
                End If
                If Not cRec.Item("p65ID") Is System.DBNull.Value Then
                    dataItem("systemcolumn").CssClass = "recurrence"
                End If
            End With
        Else
            Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
            If bolShowClueTip Then
                With dataItem("systemcolumn")
                    .Text = "<a class='reczoom' title='Detail úkolu' rel='clue_p56_record.aspx?pid=" & cRec.PID.ToString & "'>i</a>"
                End With
            End If
            With cRec
                If .IsClosed Then
                    dataItem.Font.Strikeout = True
                Else
                    If Not .p56PlanUntil Is Nothing Then
                        If Now > .p56PlanUntil Then dataItem("systemcolumn").CssClass = "overtime"
                    End If
                End If
                If .b02ID > 0 Then
                    If .b02Color <> "" Then dataItem("systemcolumn").Style.Item("background-color") = .b02Color
                End If
            End With
        End If


    End Sub
    Public Shared Sub o23_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolShowClueTip As Boolean, strMobileLinkColumn As String, strContextMenuFlag As String)
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)

        Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
        If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu
        If bolShowClueTip Then
            With dataItem("systemcolumn")
                .Text = "<a class='reczoom' title='Detail dokumentu' rel='clue_o23_record.aspx?pid=" & cRec.Item("pid").ToString & "' style='margin-left:-10px;'>i</a>"
            End With
        End If
        With cRec
            With dataItem("pm1")
                .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('o23','" & cRec.Item("pid").ToString & "',this,'" & strContextMenuFlag & "')" & Chr(34) & "></a>"
            End With
            ''If .Item("IsO27") Then
            ''    Dim s As String = "<a href='fileupload_preview.aspx?prefix=o23&pid=" & cRec.Item("pid").ToString & "' target='_blank' title='Dokument má přílohy'><img src='Images/attachment.png'/></a>"
            ''    If bolShowClueTip Then
            ''        With dataItem.Cells
            ''            If .Count >= 5 Then
            ''                .Item(4).Text = s & .Item(4).Text
            ''            End If
            ''        End With
            ''    Else
            ''        dataItem("systemcolumn").Text += s
            ''    End If

            ''End If
            If .Item("IsDraft") Then dataItem("systemcolumn").CssClass = "draft"
            If .Item("o23IsEncrypted") Then dataItem("systemcolumn").CssClass = "spy"
            If CType(BO.BAS.IsNullInt(.Item("o23LockedFlag")), BO.o23LockedTypeENUM) > BO.o23LockedTypeENUM._NotSpecified Then dataItem("systemcolumn").CssClass = "locked"


            If BO.BAS.IsNullInt(.Item("b02ID")) > 0 Then
                If .Item("b02Color") & "" <> "" Then dataItem("systemcolumn").Style.Item("background-color") = .Item("b02Color")
            Else
                If .Item("IsClosed") Then dataItem.Font.Strikeout = True
            End If
            'If strMobileLinkColumn <> "" Then
            '    dataItem(strMobileLinkColumn).Text = "<a style='color:blue;text-decoration:underline;' href='javascript:re(" & cRec.Item("pid").ToString & ")'>" & dataItem(strMobileLinkColumn).Text & "</a>"
            'End If
        End With

    End Sub
    Public Shared Sub p41_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolDT As Boolean, strMobileLinkColumn As String, strContextMenuFlag As String)
        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        If bolDT Then
            Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)

            If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu

            If cRec.Item("IsDraft") Then dataItem("systemcolumn").CssClass = "draft"
            If Not cRec.Item("j13ID") Is System.DBNull.Value Then dataItem("systemcolumn").CssClass = "favourite"
            If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
            If strMobileLinkColumn <> "" Then
                dataItem(strMobileLinkColumn).Text = "<a style='color:blue;text-decoration:underline;' href='javascript:re(" & cRec.Item("pid").ToString & ")'>" & dataItem(strMobileLinkColumn).Text & "</a>"
            End If
            If Not cRec.Item("TreeLevel") Is System.DBNull.Value Then
                Select Case cRec.Item("TreeLevel")
                    Case 1 : dataItem.ForeColor = TreeColorLevel1
                    Case Is > 1 : dataItem.ForeColor = TreeColorLevel2
                End Select
            End If
            If Not cRec.Item("p65ID") Is System.DBNull.Value Then
                dataItem("systemcolumn").CssClass = "recurrence"
            End If
            With dataItem("pm1")

                .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p41','" & cRec.Item("pid").ToString & "',this,'" & strContextMenuFlag & "')" & Chr(34) & "></a>"
            End With


        Else
            Dim cRec As BO.p41Project = CType(e.Item.DataItem, BO.p41Project)
            If cRec.IsClosed Then dataItem.Font.Strikeout = True
            If cRec.p41IsDraft Then dataItem("systemcolumn").CssClass = "draft"
        End If


    End Sub

    Public Shared Sub j02_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs)
        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)

        Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
        If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu
        With dataItem("pm1")
            .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('j02','" & cRec.Item("pid").ToString & "',this)" & Chr(34) & "></a>"
        End With
        If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
        If Not cRec.Item("IsIntraPerson") Then
            dataItem.Font.Italic = True
        End If
    End Sub
    Public Shared Sub p28_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolDT As Boolean, Optional strMobileLinkColumn As String = "")
        If Not TypeOf e.Item Is GridDataItem Then Return
        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        If bolDT Then
            Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
            If cRec.Item(0) Is System.DBNull.Value Then Return 'chybné SQL datového přehledu
            With dataItem("pm1")
                .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p28','" & cRec.Item("pid").ToString & "',this)" & Chr(34) & "></a>"
            End With
            If cRec.Item("IsClosed") Then dataItem.Font.Strikeout = True
            If cRec.Item("IsDraft") Then dataItem("systemcolumn").CssClass = "draft"
            If cRec.Item("SupplierFlag") = 4 Then dataItem.Font.Italic = 4
            If strMobileLinkColumn <> "" Then
                dataItem(strMobileLinkColumn).Text = "<a style='color:blue;text-decoration:underline;' href='javascript:re(" & cRec.Item("pid").ToString & ")'>" & dataItem(strMobileLinkColumn).Text & "</a>"
            End If
            If Not cRec.Item("ParentID") Is System.DBNull.Value Then dataItem.ForeColor = TreeColorLevel1
        Else
            Dim cRec As BO.p28Contact = CType(e.Item.DataItem, BO.p28Contact)

            If cRec.IsClosed Then dataItem.Font.Strikeout = True
            If cRec.p28IsDraft Then dataItem("systemcolumn").CssClass = "draft"

            If cRec.p28CompanyShortName > "" Then dataItem.ToolTip = cRec.p28CompanyName
        End If

    End Sub

    Private Shared Sub p31_grid_Handle_ItemDataBound_Engine(dataItem As GridDataItem, p31Date As Date, p72ID_AfterTrimming As BO.p72IdENUM, P72ID_AfterApprove As BO.p72IdENUM, p70ID As BO.p70IdENUM, p71ID As BO.p71IdENUM, bolIsClosed As Boolean, intO23ID_First As Integer, intP49ID As Integer, p33ID As BO.p33IdENUM, p31Hours_Trimmed As Double, p31Hours_Orig As Double, p34IncomeStatementFlag As BO.p34IncomeStatementFlagENUM)
        If p31Date > Now Then dataItem("systemcolumn").CssClass = "future" 'záznam do budoucna vizuálně zvýrazňovat jako plán

        Select Case p72ID_AfterTrimming
            Case BO.p72IdENUM.SkrytyOdpis, BO.p72IdENUM.ViditelnyOdpis, BO.p72IdENUM.ZahrnoutDoPausalu
                dataItem("systemcolumn").CssClass = "corr_236"
            Case BO.p72IdENUM.Fakturovat
                If p31Hours_Trimmed < p31Hours_Orig Then
                    dataItem("systemcolumn").CssClass = "corr_4_down"
                End If
                If p31Hours_Trimmed > p31Hours_Orig Then
                    dataItem("systemcolumn").CssClass = "corr_4_up"
                End If
        End Select
        Select Case P72ID_AfterApprove
            Case BO.p72IdENUM.Fakturovat : dataItem("systemcolumn").CssClass = "a14"
            Case BO.p72IdENUM.FakturovatPozdeji : dataItem("systemcolumn").CssClass = "a17"
            Case BO.p72IdENUM.ZahrnoutDoPausalu : dataItem("systemcolumn").CssClass = "a16"
            Case BO.p72IdENUM.ViditelnyOdpis : dataItem("systemcolumn").CssClass = "a12"
            Case BO.p72IdENUM.SkrytyOdpis : dataItem("systemcolumn").CssClass = "a13"
            Case Else
                If p71ID = BO.p71IdENUM.Neschvaleno Then dataItem("systemcolumn").CssClass = "a20"

        End Select

        Select Case p70ID
            Case BO.p70IdENUM.Vyfakturovano : dataItem("systemcolumn").CssClass = "a4"
            Case BO.p70IdENUM.ZahrnutoDoPausalu : dataItem("systemcolumn").CssClass = "a6"
            Case BO.p70IdENUM.ViditelnyOdpis : dataItem("systemcolumn").CssClass = "a2"
            Case BO.p70IdENUM.SkrytyOdpis : dataItem("systemcolumn").CssClass = "a3"
            
        End Select
        If intO23ID_First > 0 And intP49ID = 0 Then dataItem("systemcolumn").Text += "<img src='Images/attachment.png' width='12px' height='12px'/>"
        If bolIsClosed Then dataItem.Font.Strikeout = True

        Select Case p33ID
            Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                If p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
                    dataItem.ForeColor = Drawing.Color.Blue  'příjmy
                Else
                    dataItem.ForeColor = Drawing.Color.Brown    'výdaje
                End If
                If intP49ID > 0 Then
                    If p71ID > BO.p71IdENUM.Nic Then
                        If intO23ID_First = 0 Then
                            dataItem("systemcolumn").Text += "<img src='Images/finplan.png' style='width:12px;height:12px;padding-left:7px;'/>"
                        Else
                            dataItem("systemcolumn").Text += "<img src='Images/finplan_attachment.png' style='width:12px;height:12px;padding-left:7px;'/>"
                        End If
                    Else
                        If intO23ID_First = 0 Then
                            dataItem("systemcolumn").Text += "<img src='Images/finplan.png' style='width:12px;height:12px'/>"
                        Else
                            dataItem("systemcolumn").Text += "<img src='Images/finplan_attachment.png' style='width:12px;height:12px'/>"
                        End If
                    End If
                End If
            Case BO.p33IdENUM.Kusovnik
                dataItem.ForeColor = Drawing.Color.Green  'kusovník
            Case Else
        End Select
    End Sub
    Public Shared Sub p31_grid_Handle_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs, bolDT As Boolean, bolMobile As Boolean, strContextMenuFlag As String)
        If Not TypeOf e.Item Is GridDataItem Then Return

        Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
        If bolDT Then
            Dim cRec As System.Data.DataRowView = CType(e.Item.DataItem, System.Data.DataRowView)
            If cRec.Item(0) Is System.DBNull.Value Then Return
            With cRec
                p31_grid_Handle_ItemDataBound_Engine(dataItem, .Item("p31Date_Grid"), BO.BAS.IsNullInt(.Item("p72ID_AfterTrimming")), BO.BAS.IsNullInt(.Item("p72ID_AfterApprove")), BO.BAS.IsNullInt(.Item("p70ID")), BO.BAS.IsNullInt(.Item("p71ID")), .Item("IsClosed"), BO.BAS.IsNullInt(.Item("o23ID_First")), BO.BAS.IsNullInt(.Item("p49ID")), .Item("p33ID"), BO.BAS.IsNullNum(.Item("p31Hours_Trimmed_Grid")), BO.BAS.IsNullNum(.Item("p31Hours_Orig_Grid")), .Item("p34IncomeStatementFlag"))
            End With
            If bolMobile Then
                dataItem("mob").Text = "<a href='javascript:re(" & cRec.Item("pid").ToString & ")'><img src='Images/fe.png'></a>"
            Else
                With dataItem("pm1")
                    .Text = "<a class='pp1' onclick=" & Chr(34) & "RCM('p31','" & cRec.Item("pid").ToString & "',this,'" & strContextMenuFlag & "')" & Chr(34) & "></a>"
                End With
            End If

        Else

            Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)
            With cRec
                p31_grid_Handle_ItemDataBound_Engine(dataItem, .p31Date, .p72ID_AfterTrimming, .p72ID_AfterApprove, .p70ID, .p71ID, .IsClosed, .o23ID_First, .p49ID, .p33ID, .p31Hours_Trimmed, .p31Hours_Orig, .p34IncomeStatementFlag)
            End With



        End If

    End Sub
    
    Public Shared Sub RenderHeaderMenu(bolRecordIsClosed As Boolean, panMenuContainer As Panel, menu As Telerik.Web.UI.RadMenu)
        If bolRecordIsClosed Then

            panMenuContainer.BackColor = Drawing.Color.Black
            For i As Integer = 0 To menu.Items.Count - 1
                menu.Items(i).Style.Item("background") = "black !important"
                menu.Items(i).Style.Item("color") = "white !important"
            Next

        Else

            panMenuContainer.BackColor = Nothing
        End If
    End Sub
    Public Overloads Shared Sub RenderLevelLink(cmdLevelLink As HyperLink, strText As String, strURL As String, bolIsClosed As Boolean)
        With cmdLevelLink
            If strText = "" Then
                .Visible = False
                Return
            Else
                .Visible = True
            End If
            If bolIsClosed Then
                .Font.Strikeout = True

            Else

            End If
            If strText.Length > 40 Then
                .Text = Left(strText, 40) & "..."
                .ToolTip = strText
                .Style.Item("font-size") = "120%"
            Else
                .Text = strText
            End If

            .NavigateUrl = strURL
        End With

    End Sub
    Public Overloads Shared Sub RenderLevelLink(menuLevel1 As Telerik.Web.UI.RadMenuItem, strText As String, strURL As String, bolIsClosed As Boolean)
        With menuLevel1
            .Font.Underline = True
            .Font.Bold = True
            If bolIsClosed Then
                .Font.Strikeout = True

            Else

            End If
            If strText.Length > 40 Then
                .Text = Left(strText, 40) & "..."
                .ToolTip = strText

            Else
                .Text = strText
            End If

            .NavigateUrl = strURL
        End With

    End Sub
    Public Overloads Shared Sub RenderLevelLink(menuLevel1 As Telerik.Web.UI.NavigationNode, strText As String, strURL As String, bolIsClosed As Boolean)
        With menuLevel1
            .Font.Underline = True
            .Font.Bold = True
            If bolIsClosed Then
                .Font.Strikeout = True

            Else

            End If
            If strText.Length > 40 Then
                .Text = Left(strText, 40) & "..."
                .ToolTip = strText

            Else
                .Text = strText
            End If

            .NavigateUrl = strURL
        End With

    End Sub


    Public Shared Sub RenderQueryCombo(cbx As DropDownList)
        With cbx
            If .SelectedIndex > 0 Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub

    Public Shared Sub RenderQuickqueryLink(cmdClueQuickQuery As HyperLink, intQuickQueryValue As Integer, intBinQueryValue As Integer)
        With cmdClueQuickQuery
            If intQuickQueryValue > 0 Then
                .BackColor = basUI.ColorQueryRGB
                '.ForeColor = Drawing.Color.White
            Else
                .BackColor = Nothing
                .ForeColor = Nothing
            End If
            Select Case intBinQueryValue
                Case 1
                    .Text = "<img src='Images/query.png'/>Filtr<img src='Images/ok.png'/>"
                Case 2
                    .Text = "<img src='Images/query.png'/>Filtr<img src='Images/bin.png'/>"
                Case Else
                    .Text = "<img src='Images/query.png'/>Filtr"
            End Select
        End With
    End Sub

    
    Public Overloads Shared Function QueryProjectListByTop10(factory As BL.Factory, intJ02ID As Integer) As IEnumerable(Of BO.p41Project)
        'vybere z projektů TOP 10 podle naposledy zapisovaných úkonů
        Dim mqP31 As New BO.myQueryP31
        mqP31.j02ID = intJ02ID
        mqP31.TopRecordsOnly = 100
        mqP31.MG_SortString = "p31dateinsert desc"
        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = factory.p31WorksheetBL.GetList(mqP31)
        Dim p41ids As New List(Of Integer)
        If lisP31.Count > 0 Then
            If lisP31.Select(Function(p) p.p41ID).Distinct.Count > 10 Then
                For Each c In lisP31
                    If p41ids.Where(Function(p) p = c.p41ID).Count = 0 Then
                        p41ids.Add(c.p41ID)
                    End If
                    If p41ids.Count >= 10 Then Exit For
                Next
            Else
                p41ids = lisP31.Select(Function(p) p.p41ID).Distinct.ToList
            End If
        Else
            p41ids.Add(-1)
        End If
        Dim mqP41 As New BO.myQueryP41
        mqP41.PIDs = p41ids
        mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        Return factory.p41ProjectBL.GetList(mqP41)
    End Function
    Public Overloads Shared Function QueryProjectListByTop10(factory As BL.Factory, intJ02ID As Integer, strCols As String, strGroupField As String) As DataTable
        'vybere z projektů TOP 10 podle naposledy zapisovaných úkonů
        Dim mqP31 As New BO.myQueryP31
        mqP31.j02ID = intJ02ID
        mqP31.TopRecordsOnly = 100
        mqP31.MG_SortString = "p31dateinsert desc"
        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = factory.p31WorksheetBL.GetList(mqP31)
        Dim p41ids As New List(Of Integer)
        If lisP31.Count > 0 Then
            If lisP31.Select(Function(p) p.p41ID).Distinct.Count > 10 Then
                For Each c In lisP31
                    If p41ids.Where(Function(p) p = c.p41ID).Count = 0 Then
                        p41ids.Add(c.p41ID)
                    End If
                    If p41ids.Count >= 10 Then Exit For
                Next
            Else
                p41ids = lisP31.Select(Function(p) p.p41ID).Distinct.ToList
            End If
        Else
            p41ids.Add(-1)
        End If
        Dim mqP41 As New BO.myQueryP41
        mqP41.PIDs = p41ids
        mqP41.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        mqP41.MG_GridSqlColumns = strCols
        mqP41.MG_GridGroupByField = strGroupField
        Return factory.p41ProjectBL.GetGridDataSource(mqP41)
    End Function
    Public Shared Sub RenderSawMenuItemAsGrid(MenuItem As Telerik.Web.UI.RadMenuItem, strPrefix As String)
        With MenuItem
            .ToolTip = "Datový přehled"
            .Text = "<img src='Images/grid.png'/>"
            .NavigateUrl = "entity_framework.aspx?prefix=" & strPrefix
            .Target = ""
        End With
    End Sub
    Public Shared Sub Handle_GridTelerikExport(grid1 As UI.datagrid, strFormat As String)
        With grid1
            If .radGridOrig.ClientSettings.Scrolling.AllowScroll Then
                .radGridOrig.ClientSettings.Scrolling.AllowScroll = False
                .radGridOrig.ClientSettings.Scrolling.UseStaticHeaders = False
            End If
            For Each c As GridColumn In .radGridOrig.Columns
                If c.ColumnType <> "GridBoundColumn" Then
                    c.Display = False
                    c.Visible = False
                End If

            Next
           
            .Page.Response.ClearHeaders()
            .Page.Response.Cache.SetCacheability(HttpCacheability.[Private])
            .PageSize = 2000


            .Rebind(False)
            Select Case strFormat
                Case "xls"
                    .radGridOrig.ExportToExcel()
                Case "pdf"
                    With .radGridOrig.ExportSettings.Pdf
                        If grid1.radGridOrig.Columns.Count > 4 Then
                            .PageWidth = Unit.Parse("297mm")
                            .PageHeight = Unit.Parse("210mm")
                        Else
                            .PageHeight = Unit.Parse("297mm")
                            .PageWidth = Unit.Parse("210mm")
                        End If
                    End With
                    .radGridOrig.ExportToPdf()
                Case "doc"
                    .radGridOrig.ExportToWord()
            End Select

            
        End With
    End Sub
    Public Shared Function UploadAvatarImage(upl1 As Telerik.Web.UI.RadUpload, intJ02ID As Integer, ByRef strRetErr As String) As String
        strRetErr = ""
        For Each invalidFile As Telerik.Web.UI.UploadedFile In upl1.InvalidFiles
            strRetErr = "Nevhodný Avatar soubor: " & invalidFile.FileName
            Return ""
        Next
        If upl1.UploadedFiles.Count = 0 Then
            strRetErr = "Musíte vybrat soubor obrázku na vašem PC."
            Return ""
        End If
        If Not System.IO.Directory.Exists(BO.ASS.GetApplicationRootFolder & "\Plugins\Avatar") Then
            Try
                System.IO.Directory.CreateDirectory(BO.ASS.GetApplicationRootFolder & "\Plugins\Avatar")
            Catch ex As Exception
                strRetErr = ex.Message
                Return False
            End Try
        End If
        For Each validFile As Telerik.Web.UI.UploadedFile In upl1.UploadedFiles
            Try
                Dim strFile As String = BO.ASS.GetApplicationRootFolder & "\Plugins\Avatar\" & intJ02ID.ToString & "_" & validFile.FileName

                validFile.SaveAs(strFile, True)
                Return intJ02ID.ToString & "_" & validFile.FileName

            Catch ex As Exception
                strRetErr = ex.Message
                Return ""
            End Try

        Next
        
        Return ""
    End Function
End Class
