Imports Winnovative.ExcelLib

Public Class report_framework_detail4
    Inherits System.Web.UI.Page
    Private Property _defOutputFileName As String

    Public Property CurrentX31ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidCurX31ID.Value)
        End Get
        Set(value As Integer)
            hidCurX31ID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentJ70ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.j70ID, value.ToString)
        End Set
    End Property

    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                Me.CurrentX31ID = BO.BAS.IsNullInt(Request.Item("x31id"))
                If Me.CurrentX31ID = 0 Then
                    .StopPage("x31id missing.")
                End If
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("periodcombo-custom_query")
                    .Add("report_framework_detail4-period")
                    .Add("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString)
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("report_framework_detail4-period")


                End With

                cmdSetting.Visible = .Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
            End With
            Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
            If cRec.QueryX29ID > BO.x29IdEnum._NotSpecified Then
                Me.hidQueryPrefix.Value = Left(cRec.QueryX29ID.ToString, 3)
                SetupJ70Combo(BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString)), cRec.QueryX29ID)
            Else
                Me.j70ID.Visible = False : Me.cmdQuery.Visible = False : Me.clue_query.Visible = False
            End If

            If InhaleReport() = "" Then
                cmdGenerate.Visible = False
            End If
        End If
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("report_framework_detail4-period", Me.period1.SelectedValue)
    End Sub

    Private Function InhaleReport() As String
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        If cRec Is Nothing Then
            Master.StopPage("Nelze načíst sestavu.<hr>" & Master.Factory.x31ReportBL.ErrorMessage)
            Return ""
        End If
        lblHeader.Text = cRec.x31Name
        If cRec.x31FormatFlag <> BO.x31FormatFlagENUM.XLSX Then
            Master.StopPage("NOT XLSX format.") : Return ""
        End If
        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName

        Dim cF As New BO.clsFile
        If Not cF.FileExist(strRepFullPath) Then
            Master.Notify("XLSX soubor šablony tiskové sestavy nelze načíst.", 2)
            Return ""
        End If
        _defOutputFileName = cRec.x31ExportFileNameMask
        If _defOutputFileName = "" Then _defOutputFileName = cRec.x31Name
        _defOutputFileName += "_" & Format(Now, "yyyyMMddHHmmss")

        Return strRepFullPath




    End Function

    Private Function GenerateXLS(strSourceXlsFullPath As String, bolGenerateCsvFile As Boolean, Optional strCsvDelimiter As String = ";") As String
        Dim cXLS As New clsExportToXls(Master.Factory)
        cXLS.DefaultOutputFileName = _defOutputFileName

        Dim sheetDef As ExcelWorksheet = cXLS.LoadSheet(strSourceXlsFullPath, 0, "marktime_definition")
        If sheetDef Is Nothing Then
            Master.Notify("XLS soubor neobsahuje sešit s názvem [marktime_definition].", NotifyLevel.ErrorMessage)
            Return ""
        End If
        Dim sheetData As ExcelWorksheet = Nothing
        Dim book As ExcelWorkbook = cXLS.LoadWorkbook(strSourceXlsFullPath)
        For i As Integer = 0 To book.Worksheets.Count - 1
            If LCase(book.Worksheets(i).Name) <> "marktime_definition" Then
                sheetData = book.Worksheets(i)
                Exit For
            End If
        Next

        If sheetData Is Nothing Then
            Master.Notify("XLS soubor neobsahuje volný datový sešit.", NotifyLevel.ErrorMessage)
            Return ""
        End If
        For i = 1 To 1000
            Select Case LCase(sheetDef.Item(i, 1).Text)
                Case "x31name", "x31code"
                Case Else
                    Dim sh As ExcelWorksheet = sheetData
                    Dim strList As String = sheetDef.Item(i, 3).Value
                    If strList <> "" Then
                        sh = book.Worksheets.Item(strList)
                    End If
                    Dim strSQL As String = sheetDef.Item(i, 2).Value
                    If Me.CurrentJ70ID > 0 Then
                        Dim strW As String = Master.Factory.j70QueryTemplateBL.GetSqlWhere(Me.CurrentJ70ID)
                        Select Case Me.hidQueryPrefix.Value
                            Case "p41"
                                strSQL = Replace(strSQL, "141=141", strW)
                            Case "p28"
                                strSQL = Replace(strSQL, "328=328", strW)
                            Case "p91"
                                strSQL = Replace(strSQL, "391=391", strW)
                            Case "p31"
                                strSQL = Replace(strSQL, "331=331", strW)
                            Case "p56"
                                strSQL = Replace(strSQL, "356=356", strW)
                            Case "j02"
                                strSQL = Replace(strSQL, "102=102", strW)
                        End Select
                    End If

                    Dim strRange As String = sheetDef.Item(i, 1).Text
                    If strSQL <> "" And strRange <> "" Then
                        Dim pars As New List(Of BO.PluginDbParameter)
                        pars.Add(New BO.PluginDbParameter("datfrom", period1.DateFrom))
                        pars.Add(New BO.PluginDbParameter("datuntil", period1.DateUntil))
                        Dim intRow As Integer = sh.Item(strRange).TopRowIndex
                        Dim intCol As Integer = sh.Item(strRange).LeftColumnIndex
                        Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(strSQL, pars)
                        cXLS.MergeSheetWithDataTable(sh, dt, intRow, intCol)
                    End If

            End Select
        Next
        book.Worksheets.RemoveWorksheet("marktime_definition")
        Dim strResult As String = cXLS.SaveAsFile(sheetData, bolGenerateCsvFile, strCsvDelimiter)
        Dim cF As New BO.clsFile
        Return cF.GetNameFromFullpath(strResult)
    End Function

    Private Sub report_framework_detail4_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        If Me.j70ID.Visible Then
            basUIMT.RenderQueryCombo(Me.j70ID)
            With Me.j70ID
                If .SelectedIndex > 0 Then
                    .ToolTip = .SelectedItem.Text
                    Me.clue_query.Attributes("rel") = "clue_quickquery.aspx?j70id=" & .SelectedValue
                Else
                    Me.clue_query.Visible = False
                End If
            End With
        End If
    End Sub

    

    Private Sub SetupJ70Combo(intDef As Integer, x29id As BO.x29IdEnum)
        Dim mq As New BO.myQuery
        j70ID.DataSource = Master.Factory.j70QueryTemplateBL.GetList(mq, x29id)
        j70ID.DataBind()
        j70ID.Items.Insert(0, "--Bez filtrování--")
        basUI.SelectDropdownlistValue(Me.j70ID, intDef.ToString)

    End Sub

    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString, Me.CurrentJ70ID.ToString)
    End Sub

    Private Sub cmdGenerate_Click(sender As Object, e As EventArgs) Handles cmdGenerate.Click
        Dim strTempFile As String = GenerateXLS(InhaleReport(), False)
        If strTempFile <> "" Then
            Response.Redirect("binaryfile.aspx?tempfile=" & strTempFile)
        End If
    End Sub
    Private Sub cmdGenerateCSV_Click(sender As Object, e As EventArgs) Handles cmdGenerateCSV.Click
        Dim strTempFile As String = GenerateXLS(InhaleReport(), True, cbxDelimiter.SelectedValue)
        If strTempFile <> "" Then
            Response.Redirect("binaryfile.aspx?tempfile=" & strTempFile)
        End If
    End Sub
End Class