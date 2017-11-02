Imports System.Data
Imports System.Web.UI.HtmlControls


Public Interface IPluginSupportBL
    Inherits IFMother
    Function CreateDataTableIntoString(tab As BO.PluginDataTable) As String
    Function MergeRecordSQL(strSQL As String, strTemplateContent As String) As String
    Function GetDataTable(strSQL As String, pars As List(Of BO.PluginDbParameter)) As DataTable

    Function GetDataSet(strSQL As String, pars As List(Of BO.PluginDbParameter), Optional strTablesNameList As String = "") As DataSet

    ReadOnly Property RowsCount As Integer
End Interface
Class PluginSupportBL
    Inherits BLMother
    Implements IPluginSupportBL
    Private WithEvents _cDL As DL.PluginSupportDL
    Private m_tab As HtmlTable
    Private m_tabTemp As HtmlTable
    Private _cTab As BO.PluginDataTable
    Private m_bolIsGrandTotalRendering As Boolean
    Private Property _RowsCount As Integer
    Private Property _NeedHtmlDecode As Boolean = False

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.PluginSupportDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public ReadOnly Property RowsCount As Integer Implements IPluginSupportBL.RowsCount
        Get
            Return _RowsCount
        End Get
    End Property

    Public Function GetDataTable(strSQL As String, pars As List(Of BO.PluginDbParameter)) As DataTable Implements IPluginSupportBL.GetDataTable
        Dim intRows As Integer = 0
        Dim ds As DataSet = Nothing
        If LCase(strSQL).IndexOf(" procedure ") > 0 Then
            ds = _cDL.GetDatasetBySP(strSQL, intRows, pars)
        Else
            ds = _cDL.GetDataset(strSQL, intRows, pars)
        End If
        _RowsCount = intRows
        Return ds.Tables(0)

    End Function
    Public Function GetDataSet(strSQL As String, pars As List(Of BO.PluginDbParameter), Optional strTablesNameList As String = "") As DataSet Implements IPluginSupportBL.GetDataSet
        Dim intRows As Integer = 0
        Dim ds As DataSet = Nothing
        If LCase(strSQL).IndexOf(" procedure ") > 0 Then
            ds = _cDL.GetDatasetBySP(strSQL, intRows, pars)
        Else
            ds = _cDL.GetDataset(strSQL, intRows, pars)
        End If
        _RowsCount = intRows
        If strTablesNameList <> "" Then
            Dim a() As String = Split(strTablesNameList, ",")
            For i = 0 To ds.Tables.Count - 1
                ds.Tables(i).TableName = a(i)
            Next
        End If
        Return ds

    End Function
    
    Public Function MergeRecordSQL(strSQL As String, strTemplateContent As String) As String Implements IPluginSupportBL.MergeRecordSQL
        Dim cM As New BO.clsMergeContent
        Dim fields As List(Of String) = cM.GetAllMergeFieldsInContent(strTemplateContent)
        If fields.Count = 0 Then Return strTemplateContent

        Dim intRows As Integer = 0
        Dim par As New BO.PluginDbParameter("pid", _cUser.j02ID)
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(par)
        Dim ds As DataSet = _cDL.GetDataset(strSQL, intRows, pars)
        For Each dbRow As DataRow In ds.Tables(0).Rows
            For Each strField As String In fields
                Dim val As Object = dbRow.Item(strField)
                Dim strReplace As String = ""
                If Not val Is Nothing Then
                    Select Case val.GetType.ToString
                        Case "System.String"
                            strReplace = val
                        Case "System.DateTime"
                            strReplace = BO.BAS.FD(val, True)
                        Case "System.Double"
                            strReplace = BO.BAS.FN(val)
                        Case "System.Boolean"
                            If val Then strReplace = "ANO" Else strReplace = "NE"
                        Case "System.Int32"
                            strReplace = BO.BAS.FNI(val)
                        Case Else
                            strReplace = val.ToString
                    End Select
                End If
                If strReplace <> "" Then
                    strTemplateContent = Replace(strTemplateContent, "[%" & strField & "%]", strReplace, , , CompareMethod.Text)
                End If
            Next
        Next
        For Each field In fields    'nahradit zbylá prázdná pole
            If strTemplateContent.IndexOf("[%" & field & "%]") > 0 Then
                strTemplateContent = Replace(strTemplateContent, "[%" & field & "%]", "")
            End If
        Next
        Return strTemplateContent
    End Function

    Public Function CreateDataTableIntoString(tableDefinition As BO.PluginDataTable) As String Implements IPluginSupportBL.CreateDataTableIntoString
        _cTab = tableDefinition
        m_tab = New HtmlTable()

        Generate_HtmlTable(tableDefinition)

        If _Error <> "" Then
            Return "<p>" & _Error & "</p>"
        End If
        If _RowsCount = 0 And tableDefinition.NoDataMessage <> "" Then
            Return "<p>" & tableDefinition.NoDataMessage & "</p>"
        End If

        Dim sw As New System.IO.StringWriter()
        Dim xx As New System.Web.UI.HtmlTextWriter(sw)


        m_tab.RenderControl(New System.Web.UI.HtmlTextWriter(sw))

        Dim s As String = sw.ToString
        
        Dim sb As System.Text.StringBuilder = sw.GetStringBuilder

        Dim x As Integer = s.IndexOf("<tr>")
        If x > 0 Then
            If _cTab.TableCaption <> "" Then
                sb.Insert(x - 1, "<caption>" & _cTab.TableCaption & "</caption><thead>")
            Else
                sb.Insert(x - 1, "<thead>")
            End If
        End If


        x = sb.ToString.IndexOf("</tr>")
        If x > 0 Then
            sb.Insert(x + Len("</tr>"), "</thead><tbody>")
        End If
        x = sb.ToString.IndexOf("</table>")
        If x > 0 Then
            sb.Insert(x - 1, "</tbody>")
        End If

        If _NeedHtmlDecode Then
            Return System.Net.WebUtility.HtmlDecode(sb.ToString)
        Else
            Return sb.ToString
        End If
        

        'Return sb.ToString
    End Function
    Public Sub Generate_HtmlTable(tableDefinition As BO.PluginDataTable)
        If tableDefinition.SQL = "" Then
            _Error = "SQL is missing." : Return
        End If
        _RowsCount = 0

        Dim ds As DataSet = _cDL.GetDataset(tableDefinition.SQL, _RowsCount, tableDefinition.DbParameters)
        If _cDL.ErrorMessage <> "" Then
            Return
        End If
        m_tab.Style.Item("border-collapse") = "collapse"

        If _cTab.TableCssClass <> "" Then m_tab.Attributes.Item("class") = _cTab.TableCssClass
        If _cTab.TableID <> "" Then m_tab.Attributes.Item("id") = _cTab.TableID
        RenderGridHeaders()
        RenderData(ds)
        CorrectGridHeaders()


    End Sub

    Private Sub RenderGridHeaders()
        If _cTab.ColHeaders = "" Then Return
        Dim tr As New HtmlTableRow, th As HtmlTableCell
        m_tab.Rows.Add(tr)
        Dim a() As String = Split(_cTab.ColHeaders, "|")
        Dim t() As String = Split(_cTab.ColTypes, "|")
        Dim i As Integer

        For i = 0 To UBound(a)  'standardní tabulka
            th = New HtmlTableCell("th")

            th.InnerText = a(i)
            th.Attributes.Item("ID") = "HEADER"
            If t(i) = "N" Or t(i) = "T" Or t(i) = "C" Or t(i) = "I" Then
                th.Attributes.Item("id") = "HEADER-NUMERIC"
            End If
            tr.Cells.Add(th)
        Next

        If _cTab.PivotColHeaders > "" Then
            tr = New HtmlTableRow
            m_tab.Rows.Add(tr)
            For i = 0 To _cTab.ColsCountWithoutPivot - 1
                th = New HtmlTableCell
                th.InnerText = a(i)
                th.Attributes.Item("id") = "HEADER"
                If t(i) = "N" Or t(i) = "T" Or t(i) = "C" Or t(i) = "I" Then
                    th.Attributes.Item("id") = "HEADER-NUMERIC"
                End If
                tr.Cells.Add(th)
            Next
            Dim aH() As String = Split(_cTab.PivotColHeaders, "|"), x As Integer
            For i = _cTab.ColsCountWithoutPivot To UBound(a)
                th = New HtmlTableCell
                If x > UBound(aH) Then x = 0
                th.InnerText = aH(x)
                th.Attributes.Item("id") = "cellPivot"

                x += 1
                tr.Cells.Add(th)
            Next


        End If
    End Sub
    Private Sub CorrectGridHeaders()
        If _cTab.PivotColHeaders = "" Then Return
        Dim i As Integer, tr0 As HtmlTableRow = m_tab.Rows(0), tr1 As HtmlTableRow = m_tab.Rows(1), bolColspan As Boolean
        For i = 0 To _cTab.ColsCountWithoutPivot - 1
            tr1.Cells.Remove(tr1.Cells(0))
            tr0.Cells(i).RowSpan = 2
        Next
        Dim aH() As String = Split(_cTab.PivotColHeaders, "|"), j As Integer
        For i = _cTab.ColsCountWithoutPivot To tr0.Cells.Count - 1 Step UBound(aH) + 1
            bolColspan = True

            tr0.Cells(i).ColSpan = UBound(aH) + 1
            For j = 0 To UBound(aH) - 1
                tr0.Cells(i + j + 1).InnerText = "4remove"
            Next
        Next
        Dim b As Boolean = True
        While b
            b = False
            For i = 0 To tr0.Cells.Count - 1
                If tr0.Cells(i).InnerText = "4remove" Then
                    tr0.Cells.Remove(tr0.Cells(i))
                    b = True
                    Exit For
                End If
            Next

        End While

    End Sub

    Private Sub RenderData(ByVal ds As DataSet)
        Dim tr As HtmlTableRow, td As HtmlTableCell, dbRow As DataRow, i As Integer, lngRow As Integer
        Dim strTDClass As String
        Dim aT() As String = Split(UCase(_cTab.ColTypes), "|")
        Dim lngColumns As Integer = ds.Tables(0).Columns.Count
        If UBound(aT) + 1 < lngColumns Then lngColumns = UBound(aT) + 1
        Dim xx As Integer, intTrClass As Integer = -1
        'If _cTab.TrClassColumn <> "" Then
        '    intTrClass = IsNullInt(Me.TrClassColumn)
        'End If
        Dim aColCSS() As String = Split(_cTab.ColClasses, "|")


        For Each dbRow In ds.Tables(0).Rows
            tr = New HtmlTableRow
            'If intTrClass > -1 Then
            '    'explicitní TR styly gridu
            '    tr.CssClass = dbRow.Item(intTrClass).ToString
            'End If
            lngRow += 1
            xx += 1
           

            For i = 0 To lngColumns - 1
                td = New HtmlTableCell

                Select Case aT(i)
                    Case "N", "I", "T"
                        td.InnerText = dbRow.Item(i) & ""
                        td.Attributes.Item("id") = "CELL-NUMERIC"
                    Case "C"

                        td.InnerText = dbRow.Item(i) & ""
                        td.Attributes.Item("id") = "CELL-CURRENCY"

                    Case "D"
                        td.Attributes.Item("id") = "CELL-DATE"
                        If _cTab.FormatDate = "" Then
                            td.InnerText = BO.BAS.FD(dbRow.Item(i))
                        Else
                            td.InnerText = Format(dbRow.Item(i), _cTab.FormatDate)
                        End If

                    Case "DT", "DTX"
                        td.Attributes.Item("id") = "CELL-DATE"
                        If _cTab.FormatDateTime = "" Then
                            If aT(i) = "DTX" Then
                                td.InnerText = BO.BAS.FD(dbRow.Item(i), True, True)
                            Else
                                td.InnerText = BO.BAS.FD(dbRow.Item(i), True)
                            End If

                        Else
                            td.InnerText = Format(dbRow.Item(i), _cTab.FormatDateTime)
                        End If

                    Case "SX"
                        td.Attributes.Item("id") = "CELL-SX"
                        td.InnerText = dbRow.Item(i) & ""
                    Case "HTML"
                        td.InnerHtml = dbRow.Item(i) & ""
                    Case "B"
                        If dbRow.Item(i) Is System.DBNull.Value Then
                            td.InnerText = ""
                        Else
                            td.InnerText = Format(dbRow.Item(i), "Yes/No")
                        End If
                  
                    Case Else
                        td.InnerText = dbRow.Item(i) & ""
                End Select
                If td.InnerText.IndexOf("[[") >= 0 Then
                    td.InnerText = Replace(td.InnerText, "[[", "<") 'html element
                    td.InnerText = Replace(td.InnerText, "]]", ">") 'html element
                    _NeedHtmlDecode = True
                End If
                If _cTab.IsExcelFormat Then
                    'xls formát čísel
                    If aT(i) = "C" Or aT(i) = "N" Or aT(i) = "I" Then
                        'td.CssClass = ""
                        td.Attributes.Item("x:num") = "'" & Replace(Replace(dbRow.Item(i).ToString, " ", ""), ",", ".") & "'"
                        td.Style.Item("mso-number-format") = "Fixed"
                    End If
                End If
                If UBound(aColCSS) > 0 Then
                    strTDClass = aColCSS(i)
                    td.Attributes.Item("class") = strTDClass
                End If


                tr.Cells.Add(td)
            Next
            m_tab.Rows.Add(tr)
        Next

        RenderSubtotals()
        RenderGrandTotal()
        FormatNumberColumns()
    End Sub

    Private Sub FormatNumberColumns()
        Dim a() As String = Split(_cTab.ColTypes, "|"), i As Integer, strFormat As String, lngCol As Integer, x As Integer
        Dim tr As HtmlTableRow

        Dim strExplicitCulture As String = _cTab.CultureCode
        Dim nfi As System.Globalization.NumberFormatInfo = Nothing
        If strExplicitCulture <> "" Then
            nfi = New System.Globalization.CultureInfo(strExplicitCulture, False).NumberFormat
        End If
        Dim cTime As New BO.clsTime

        For i = 0 To UBound(a)
            strFormat = ""
            Select Case UCase(a(i))
                Case "N" : strFormat = "Standard"
                Case "C" : strFormat = "Currency"
                Case "G" : strFormat = "G"
                Case "I" : strFormat = "I"
                Case "T" : strFormat = "TIME"

            End Select
            Dim intFirstFormattedRow As Integer = 1
            If _cTab.ColHeaders = "" Then intFirstFormattedRow = 0

            If strFormat <> "" And strFormat <> "TIME" Then
                lngCol = i : x = 0
                For Each tr In m_tab.Rows
                    x += 1
                    If x > intFirstFormattedRow Then
                        If strExplicitCulture = "" Then
                            If strFormat = "I" Then
                                If IsNumeric(tr.Cells(lngCol).InnerText) Then tr.Cells(lngCol).InnerText = CLng(tr.Cells(lngCol).InnerText).ToString
                            Else
                                tr.Cells(lngCol).InnerText = Format(tr.Cells(lngCol).InnerText, strFormat)
                            End If
                        Else
                            Dim s As String = tr.Cells(lngCol).InnerText
                            If IsNumeric(s) Then
                                tr.Cells(lngCol).InnerText = CDec(s).ToString(_cTab.FormatNumber, nfi)
                            End If
                        End If
                    End If
                Next
            End If
            If strFormat = "TIME" Then
                lngCol = i : x = 0
                For Each tr In m_tab.Rows
                    x += 1
                    If x > 1 And IsNumeric(tr.Cells(lngCol).InnerText) Then tr.Cells(lngCol).InnerText = cTime.ShowAsHHMM(tr.Cells(lngCol).InnerText)
                Next
            End If
        Next


    End Sub

    Private Sub RenderSubtotals()
        If _cTab.ColFlexSubtotals = "" Or InStr(_cTab.ColFlexSubtotals, "1") < 0 Then
            'v sestavě nejsou souhrny - ohlídat akorát celkové součty
            Return
        End If
        m_tabTemp = New HtmlTable

        Dim a() As String = Split(_cTab.ColFlexSubtotals, "|"), i As Integer, lngCol As Integer
        Dim str As String, strLast As String = "", tr As HtmlTableRow, lngRow As Integer
        Dim lngSubtotalOrder As Integer, lngRows As Integer
        Dim trLast As HtmlTableRow = Nothing, xx As Long, bolCreateSubtotal As Boolean, j As Integer, intStartRow As Integer = 1
        If _cTab.PivotColHeaders > "" Then intStartRow = 2
        

        For i = 0 To UBound(a)
            If a(i) = "1" Then
                lngCol = i
                lngRow = 0
                lngSubtotalOrder += 1
                lngRows = m_tab.Rows.Count
                If lngRows > 1 Then strLast = m_tab.Rows(intStartRow).Cells(lngCol).InnerText
                While lngRow < lngRows
                    tr = m_tab.Rows(lngRow)

                    bolCreateSubtotal = False
                    If lngRow > intStartRow Then 'id mají vyplněné subtotal rows
                        str = tr.Cells(lngCol).InnerText
                        If (str <> strLast) And tr.ID & "" = "" And trLast.ID & "" = "" Then
                            bolCreateSubtotal = True
                        End If
                        If lngSubtotalOrder > 1 And Not bolCreateSubtotal Then
                            If tr.ID & "" <> "" And trLast.ID & "" = "" Then
                                bolCreateSubtotal = True
                            End If
                        End If

                        If bolCreateSubtotal Then
                            CreateSubtotalRow(lngRow, lngSubtotalOrder, lngCol)

                            lngRows = m_tab.Rows.Count  'nutno obnovovat celkový počet řádků
                            lngRow = lngRow + 1
                        End If


                        If tr.ID & "" = "" Then
                            strLast = str
                        Else
                            strLast = ""
                        End If

                    End If
                    trLast = tr
                    lngRow = lngRow + 1
                    xx += 1
                    If xx > 20000 Then Exit While
                End While
                If lngSubtotalOrder = 1 Then
                    trLast = m_tab.Rows(lngRows - 1)
                    CreateSubtotalRow(lngRows - 1, lngSubtotalOrder, lngCol, True)
                End If

                For j = 0 To UBound(a)
                    If a(j) = "11" Then
                        'součtový sloupec
                        RenderTotals(j, lngSubtotalOrder)
                    End If
                Next
            End If
        Next

        lngCol = -1
        lngRows = m_tab.Rows.Count

        If _cTab.IsHideRepeatedGroupValues Then
            a = Split(_cTab.ColHideRepeatedValues, "|")
            For i = 0 To UBound(a)
                If a(i) = "1" Then
                    lngCol = i : j = 0 : str = "" : strLast = ""
                    For lngRow = 0 To lngRows - 2
                        tr = m_tab.Rows(lngRow)
                        If Left(tr.ID, 8) <> "subtotal" Then
                            str = m_tab.Rows(lngRow).Cells(lngCol).InnerText
                            If str = strLast Then

                                m_tab.Rows(lngRow).Cells(lngCol).InnerText = ""

                            End If
                            j += 1
                            strLast = str
                        End If

                    Next

                End If
            Next

        End If

    End Sub

    Private Function CreateSubtotalRow(ByVal lngIndex As Integer, ByVal lngSubtotalOrder As Integer, ByVal lngStartCol As Integer, Optional ByVal bolAddSubtotalAtEnd As Boolean = False) As HtmlTableRow
        Dim trNew As New HtmlTableRow
        trNew.ID = "subtotal" & lngSubtotalOrder

        Dim i As Integer, td As HtmlTableCell, tdNew As HtmlTableCell
        For Each td In m_tab.Rows(lngIndex).Cells
            tdNew = New HtmlTableCell
            tdNew.Attributes.Item("id") = "CELL-SUBTOTAL"
            If lngStartCol = i Then
                If Not bolAddSubtotalAtEnd Then
                    tdNew.InnerText = _cTab.TOTAL_Caption & " " & m_tab.Rows(lngIndex - 1).Cells(i).InnerText
                Else
                    tdNew.InnerText = _cTab.TOTAL_Caption & " " & m_tab.Rows(lngIndex).Cells(i).InnerText
                End If

            End If


            trNew.Cells.Add(tdNew)
            i = i + 1
        Next
        If Not bolAddSubtotalAtEnd Then

            m_tab.Rows.Insert(lngIndex, trNew)
        Else
            m_tab.Rows.Add(trNew)
        End If
        Return trNew
    End Function

    Private Sub RenderTotals(ByVal lngCol As Integer, ByVal lngSubtotalOrder As Integer)
        Dim tr As HtmlTableRow, td As HtmlTableCell, dbl As Double, dblTotal As Double, lngRow As Integer = 0, intZeroRow As Integer = 0
        If _cTab.IsWithoutTABLE_ELEMENT Then intZeroRow = -1
        For Each tr In m_tab.Rows
            If lngRow > intZeroRow Then
                td = tr.Cells(lngCol)
                td.Attributes.Item("id") = "CELL-TOTAL"
                dbl = 0
                If tr.ID & "" = "" Then
                    If IsNumeric(td.InnerText) Then dbl = CDbl(td.InnerText)
                    dblTotal = dblTotal + dbl
                Else
                    If tr.ID = "subtotal" & lngSubtotalOrder Then
                        td.InnerText = dblTotal
                        td.Style.Item("text-align") = "right"
                        If _cTab.IsExcelFormat Then
                            td.Attributes.Item("x:num") = "'" & Replace(Replace(td.InnerText, " ", ""), ",", ".") & "'"
                            td.Style.Item("mso-number-format") = "Fixed"
                        End If

                    End If
                    dblTotal = 0
                End If

            End If
            lngRow += 1
        Next
    End Sub
    Private Sub RenderGrandTotal()
        m_bolIsGrandTotalRendering = False
        If Not _cTab.IsShowGrandTotals Then Return

        Dim trNew As New HtmlTableRow, tdNew As HtmlTableCell, td As HtmlTableCell


        For Each td In m_tab.Rows(m_tab.Rows.Count - 1).Cells
            tdNew = New HtmlTableCell
            tdNew.Attributes.Item("id") = "CELL-GRANDTOTAL"

            tdNew.InnerText = " "
            trNew.Attributes.Item("id") = "grandtotal"
            trNew.Cells.Add(tdNew)
        Next


        Dim a() As String = Split(_cTab.ColFlexSubtotals, "|"), i As Integer, tr As HtmlTableRow, bolIsSubtotal As Boolean
        For i = 0 To UBound(a)
            If a(i) = "1" Then bolIsSubtotal = True
        Next
        Dim dblTotal As Double, dbl As Double, bolGoOn As Boolean, bolIsGrandTotal As Boolean, strCell As String, x As Integer

        For i = 0 To UBound(a)
            dblTotal = 0
            ''Try
            ''    If Not _cTab.IsExcelFormat Then trNew.Cells(i).CssClass = Me.CssClass_TD_FOOTER
            ''Catch ex As Exception

            ''    Return
            ''End Try

            If a(i) = "11" Then
                If _cTab.IsWithoutTABLE_ELEMENT Then
                    x = 1
                Else
                    x = 0
                End If
                For Each tr In m_tab.Rows
                    bolGoOn = False : x += 1
                    If bolIsSubtotal Then
                        If tr.ID = "subtotal1" Then bolGoOn = True
                    Else
                        If x > 1 Then bolGoOn = True
                    End If
                    If bolGoOn Then
                        dbl = 0
                        strCell = tr.Cells(i).InnerText
                        If IsNumeric(strCell) Then dbl = CDbl(strCell)
                        dblTotal = dblTotal + dbl
                        bolIsGrandTotal = True
                    End If
                Next
                trNew.Cells(i).InnerText = dblTotal.ToString
                trNew.Cells(i).Style.Item("text-align") = "right"
                If _cTab.IsExcelFormat Then
                    trNew.Cells(i).Attributes.Item("x:num") = "'" & Replace(Replace(trNew.Cells(i).InnerText, " ", ""), ",", ".") & "'"
                    trNew.Cells(i).Style.Item("mso-number-format") = "Fixed"
                End If

            End If
        Next
        If bolIsGrandTotal Then

            trNew.Cells(0).InnerText = _cTab.GRANDTOTAL_CAPTION

            m_tab.Rows.Add(trNew)
            m_bolIsGrandTotalRendering = True
        End If

    End Sub

  
End Class
