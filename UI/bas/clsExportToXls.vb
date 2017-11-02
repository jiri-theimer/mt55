Imports Winnovative.ExcelLib

Public Class clsExportToXls
    Private Property _Error As String
    Private _factory As BL.Factory
    Public Property DefaultOutputFileName As String = ""
    Public Sub New(factory As BL.Factory)
        _factory = factory
    End Sub
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Function CreateSheet() As ExcelWorksheet
        Dim book As New ExcelWorkbook(ExcelWorkbookFormat.Xlsx_2007), bol97Format As Boolean = False
        With book
            .LicenseKey = "Jg0XBhcVBhUQEwYQCBYGFRcIFxQIHx8fHw=="
            .DefaultFontName = "Calibri"
            .DefaultFontSizePoints = 11
            .DefaultRowHeightPoints = 15
        End With

        With book.DocumentProperties
            .Subject = "MARKTIME Data Export"
            .Comments = Format(Now, "dd.MM.yyyy HH:mm")
        End With
        
        Return book.Worksheets(0)
    End Function

    Public Function LoadWorkbook(strFullPathXLS As String) As ExcelWorkbook
        Dim book As New ExcelWorkbook(strFullPathXLS)
        book.LicenseKey = "Jg0XBhcVBhUQEwYQCBYGFRcIFxQIHx8fHw=="
        Return book
    End Function
    Public Function LoadSheet(strFullPathXLS As String, intSheetIndex As Integer, Optional strSheetName As String = "") As ExcelWorksheet
        Dim book As New ExcelWorkbook(strFullPathXLS), bol97Format As Boolean = False
        book.LicenseKey = "Jg0XBhcVBhUQEwYQCBYGFRcIFxQIHx8fHw=="
        Dim sheet As ExcelWorksheet = book.Worksheets(intSheetIndex)
        If strSheetName <> "" Then
            Try
                sheet = book.Worksheets(strSheetName)
            Catch ex As Exception

            End Try
        End If


        Return sheet
    End Function
    Private Sub RenderHeaders(strHeaders As String, sheet As ExcelWorksheet, Optional strComments As String = "", Optional intRow As Integer = 1)
        Dim a() As String = Split(strHeaders, "|")
        Dim ac() As String = Split(strComments, "|")
        For i As Integer = 0 To UBound(a)
            sheet.Item(intRow, i + 1).Value = a(i)
            If UBound(ac) >= i Then
                If ac(i) <> "" Then
                    With sheet.Item(intRow, i + 1).DataValidator
                        .ShowInputMessage = True
                        .InputMessageText = ac(i)
                    End With
                End If

            End If
        Next
    End Sub
    Private Sub RenderDataRow(obj As Object, strProperties As String, sheet As ExcelWorksheet, intRow As Integer, Optional intStartColumn As Integer = 1)
        Dim x As Integer = intStartColumn
        For Each s In Split(strProperties, "|")
            Dim o As Object = Nothing
            If TypeOf obj Is DataRow Then
                Try
                    o = CType(obj, DataRow).Item(s)
                Catch ex As Exception
                End Try

            Else
                o = BO.BAS.GetPropertyValue(obj, s)
            End If

            If Not o Is Nothing Then

                Select Case o.GetType.ToString
                    Case "System.DateTime"
                        sheet.Item(intRow, x).Style.Number.NumberFormatString = "dd.MM.yyyy"
                        If Not BO.BAS.IsNullDBDate(o) Is Nothing Then
                            sheet.Item(intRow, x).Value = o
                            'sheet.Item(intRow, x).DateTimeValue = o
                        End If

                    Case "System.Double"
                        sheet.Item(intRow, x).Style.Number.NumberFormatString = "#,##0.00"
                        sheet.Item(intRow, x).NumberValue = o
                        'Case "System.Integer"
                        '    sheet.Item(intRow, x).NumberValue = o
                    Case Else

                        sheet.Item(intRow, x).Value = o


                End Select

            End If
            x += 1
        Next
    End Sub

    Public Function FindPropertyValueInColumn(sheet As ExcelWorksheet, intColumn As Integer, strPropertyName As String) As Object
        Dim i As Integer = 1
        For i = 1 To 10000
            If LCase(sheet.Item(i, intColumn).Text) = LCase(strPropertyName) Then
                Return sheet.Item(i, intColumn + 1).Value
            End If
        Next
        Return Nothing
    End Function
    Public Function ExportDataGrid(lis As IEnumerable(Of Object), cJ70 As BO.j70QueryTemplate) As String
        Dim sheet As ExcelWorksheet = CreateSheet(), strFields As String = "", strHeaders As String = ""

        Dim lisCols As List(Of BO.GridColumn) = _factory.j70QueryTemplateBL.ColumnsPallete(cJ70.x29ID)
        For Each s In Split(cJ70.j70ColumnNames, ",")
            Dim strField As String = Trim(s)

            Dim c As BO.GridColumn = lisCols.Find(Function(p) p.ColumnName = strField)
            If Not c Is Nothing Then
                strFields += "|" & c.ColumnName
                strHeaders += "|" & c.ColumnHeader

            End If

        Next
        strFields = BO.BAS.OM1(strFields)
        strHeaders = BO.BAS.OM1(strHeaders)

        RenderHeaders(strHeaders, sheet)

        Dim x As Integer = 2
        For Each c In lis
            RenderDataRow(c, strFields, sheet, x)
            x += 1
        Next
        Return SaveAsFile(sheet, False)
    End Function
   

    Public Function ExportGenericData(lis As IEnumerable(Of Object), strProperties As String, strHeaders As String) As String
        Dim sheet As ExcelWorksheet = CreateSheet()

        Dim lisCols As List(Of BO.GridColumn) = _factory.j70QueryTemplateBL.ColumnsPallete(BO.x29IdEnum.p31Worksheet)
        
       

        RenderHeaders(strHeaders, sheet)

        Dim x As Integer = 2
        For Each c In lis
            RenderDataRow(c, strProperties, sheet, x)
            x += 1
        Next
        Return SaveAsFile(sheet, False)
    End Function

    Public Sub MergeSheetWithDataTable(ByRef sheet As ExcelWorksheet, dt As DataTable, intStartRow As Integer, intStartColumn As Integer)
        Dim dbRow As DataRow
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim row As Integer = intStartRow
            For Each dbRow In dt.Rows
                Select Case dt.Columns(i).DataType.ToString
                    Case "System.Double"
                        If Not dbRow(i) Is System.DBNull.Value Then
                            sheet.Item(row, intStartColumn + i).NumberValue = dbRow(i)
                        End If
                    Case Else
                        sheet.Item(row, intStartColumn + i).Value = dbRow(i)
                End Select
                row += 1
            Next

        Next

    End Sub

    
    Public Function SaveAsFile(sheet As ExcelWorksheet, bolGenerateCsvFile As Boolean, Optional strCsvDelimiter As String = ";") As String

        Dim strDir As String = _factory.x35GlobalParam.TempFolder
        If strDir = "" Then
            _Error = "TEMP folder is not defined!"
            Return ""
        End If


        Dim strGUID As String = "MARKTIME_" & BO.BAS.GetGUID()
        If Me.DefaultOutputFileName <> "" Then
            strGUID = BO.BAS.Prepare4FileName(Me.DefaultOutputFileName)
        End If
        Dim strFullPath As String = strDir & "\" & strGUID & ".xlsx"
        If bolGenerateCsvFile Then
            strFullPath = strDir & "\" & strGUID & ".csv"
        End If
        Try
            If bolGenerateCsvFile Then
                sheet.Workbook.SaveToCsv(0, strFullPath, strCsvDelimiter, System.Text.Encoding.UTF8)
            Else
                sheet.Workbook.Save(strFullPath)
            End If
            sheet.Workbook.Close()
            If bolGenerateCsvFile Then
                Return strGUID & ".csv"
            Else
                Return strGUID & ".xlsx"
            End If

        Catch ex As Exception
            _Error = ex.Message
            Return ""
        End Try
    End Function
End Class
