Imports Telerik.Reporting
Public Class report_modal
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Public Property CurrentX31ID As Integer
        Get
            If Me.x31ID.Items.Count = 0 Then Return 0
            Return BO.BAS.IsNullInt(Me.x31ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.x31ID, value.ToString)
        End Set
    End Property
    Public Property MultiPIDs As String
        Get
            Return Me.hidPIDS.Value
        End Get
        Set(value As String)
            Me.hidPIDS.Value = value
        End Set
    End Property

    Private Sub InhaleLic()
        ceTe.DynamicPDF.Document.AddLicense("DPS50NPDFHMIDOmx0oPeZ+nF3z6VFeFQHDwPiglUKQ/xyRdT8Uvdb5Moivhseqj3bxlt//+w6FtkfFfsGjYwOAJOXNbss5x7huJQ")
    End Sub

    Private Sub report_modal_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub
    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return DirectCast(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
            If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified Then
                Master.StopPage("prefix missing")
            End If
            If Me.CurrentX29ID = BO.x29IdEnum.j02Person Then
                If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_X31_Personal) Then Master.StopPage("Chybí oprávnění k osobním sestavám.")
            End If
            ViewState("guid") = BO.BAS.GetGUID
            If Request.Item("pids") <> "" Then
                Me.MultiPIDs = Request.Item("pids")
            Else
                If Request.Item("pid") <> "" Then
                    If Request.Item("pid").IndexOf(",") > 0 Then Me.MultiPIDs = Request.Item("pid")
                End If
            End If
            With Master
                If Me.MultiPIDs = "" Then
                    .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                Else
                    If Me.MultiPIDs.IndexOf(",") < 0 Then
                        .DataPID = BO.BAS.IsNullInt(Me.MultiPIDs)
                        Me.MultiPIDs = ""
                    End If
                End If
                ''If .DataPID = 0 And Request.Item("guid") = "" Then .StopPage("pid missing")
                If .DataPID = 0 And Me.MultiPIDs = "" And Request.Item("guid") = "" Then .StopPage("pid missing")
                If .Factory.SysUser.IsAdmin Then
                    .AddToolbarButton(Resources.report_modal.NastaveniSablony, "setting", "0", "Images/setting.png", False, "javascript:x31_record()")
                End If
                .AddToolbarButton("PDF merge", "merge", 0, "Images/merge.png", False)

                If Me.MultiPIDs = "" Then
                    .AddToolbarButton("PDF náhled", "pdf", 0, "Images/pdf.png", True)
                Else
                    .AddToolbarButton("PDF export", "pdf-export", 0, "Images/pdf.png", True)
                End If

                .AddToolbarButton(Resources.report_modal.OdeslatPostou, "mail", 0, "Images/email.png")
                .AddToolbarButton(Resources.report_modal.Tisk, "print", 0, "Images/report.png", False, "javascript:rvprint()")
                .RadToolbar.FindItemByValue("merge").CssClass = "show_hide1"

            End With
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("periodcombo-custom_query")
                .Add("report_modal-x31id-" & Me.CurrentPrefix)
                .Add("report_modal-period")
                .Add("report_modal-x31id-merge1-" & Me.CurrentPrefix)
                .Add("report_modal-x31id-merge2-" & Me.CurrentPrefix)
                .Add("report_modal-x31id-merge3-" & Me.CurrentPrefix)
            End With
            With Master
                If .DataPID <> 0 Then .HeaderText = "Report | " & .Factory.GetRecordCaption(Me.CurrentX29ID, .DataPID)
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("report_modal-period")

                    Dim strDefX31ID As String = Request.Item("x31id")
                    If strDefX31ID = "" Then strDefX31ID = .GetUserParam("report_modal-x31id-" & Me.CurrentPrefix)
                    SetupX31Combo(strDefX31ID)
                    strDefX31ID = .GetUserParam("report_modal-x31id-merge1-" & Me.CurrentPrefix)
                    If strDefX31ID <> "" Then basUI.SelectDropdownlistValue(Me.x31ID_Merge1, strDefX31ID)
                    strDefX31ID = .GetUserParam("report_modal-x31id-merge2-" & Me.CurrentPrefix)
                    If strDefX31ID <> "" Then basUI.SelectDropdownlistValue(Me.x31ID_Merge2, strDefX31ID)
                    strDefX31ID = .GetUserParam("report_modal-x31id-merge3-" & Me.CurrentPrefix)
                    If strDefX31ID <> "" Then basUI.SelectDropdownlistValue(Me.x31ID_Merge3, strDefX31ID)
                End With
            End With



            InhaleOtherInputParameters()
            If Me.MultiPIDs <> "" Then
                Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.MultiPIDs)
                If pids.Count > 100 Then Master.StopPage("Generovat hromadně sestavu lze maximálně pro 100 záznamů.")
                period1.Visible = True
                Master.HideShowToolbarButton("mail", False) 'hromadný tisk nemá poštu
                Master.HideShowToolbarButton("print", False)    'hromadný tisk nemá tisk


                Select Case Me.CurrentX29ID
                    Case BO.x29IdEnum.p28Contact
                        Dim mq As New BO.myQueryP28
                        mq.PIDs = pids
                        Me.multiple_records.Text = String.Join("<hr>", Master.Factory.p28ContactBL.GetList(mq).Select(Function(p) p.p28Name))
                    Case BO.x29IdEnum.p41Project
                        Dim mq As New BO.myQueryP41
                        mq.PIDs = pids
                        Me.multiple_records.Text = String.Join("<hr>", Master.Factory.p41ProjectBL.GetList(mq).Select(Function(p) p.FullName))
                    Case BO.x29IdEnum.p91Invoice
                        Dim mq As New BO.myQueryP91
                        mq.PIDs = pids
                        Me.multiple_records.Text = String.Join("<hr>", Master.Factory.p91InvoiceBL.GetList(mq).Select(Function(p) p.p91Code))
                    Case BO.x29IdEnum.j02Person
                        Dim mq As New BO.myQueryJ02
                        mq.PIDs = pids
                        Me.multiple_records.Text = String.Join("<hr>", Master.Factory.j02PersonBL.GetList(mq).Select(Function(p) p.FullNameDesc))
                    Case BO.x29IdEnum.o23Doc
                        Dim mq As New BO.myQueryO23(0)
                        mq.PIDs = pids
                        Me.multiple_records.Text = String.Join("<hr>", Master.Factory.o23DocBL.GetList(mq).Select(Function(p) p.NameWithCode))
                End Select
                Me.multiple_records.Text = "<b style='color:blue'>Pro tisk hromadné sestavy je k dispozici pouze PDF výstup. Náhled k tisku je možný pouze pro jeden záznam.</b><hr>" & Me.multiple_records.Text
            Else
                RenderReport()
                multiple_records.Visible = False
            End If

        End If
    End Sub

    Private Sub MultiPidsGeneratePDF()
        InhaleLic()
        Dim doc1 As New ceTe.DynamicPDF.Merger.MergeDocument()
        doc1.Author = "MARKTIME"

        Dim a() As String = Split(Me.MultiPIDs, ",")
        For Each strPID As String In a
            Master.DataPID = CInt(strPID)
            Dim strPdfFileName As String = GenerateOnePDF2Temp(Me.CurrentX31ID, ViewState("guid") & "_" & strPID & ".pdf")
            doc1.Append(Master.Factory.x35GlobalParam.TempFolder & "\" & strPdfFileName)
        Next
        Master.DataPID = 0

        doc1.DrawToWeb(Master.Factory.GetRecordFileName(BO.x29IdEnum._NotSpecified, 0, "pdf", False, Me.CurrentX31ID), True)
        ''doc1.DrawToWeb("MARKTIME_REPORT_MULTIPLE.pdf", True)


    End Sub

    Private Sub SetupX31Combo(strDefX31ID As String)
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Dim lisX31 As List(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(mq).Where(Function(p) p.x29ID = Me.CurrentX29ID And (p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik Or p.x31FormatFlag = BO.x31FormatFlagENUM.DOCX Or p.x31FormatFlag = BO.x31FormatFlagENUM.XLSX)).ToList
        If Me.CurrentX29ID = BO.x29IdEnum.o23Doc Then
            Dim cO23 As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
            Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(cO23.x18ID)
            Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(cX18.x18ReportCodes, ","), lisX31Q As New List(Of BO.x31Report)
            If lis.Count = 0 Then
                Master.StopPage(String.Format("Pro typ dokumentu [{0}] není nastavena šablona tiskové sestavy.", cX18.x18Name))
            Else
                For Each strCode As String In lis
                    If lisX31.Where(Function(p) LCase(p.x31Code) = LCase(strCode)).Count > 0 Then
                        lisX31Q.Add(lisX31.Where(Function(p) LCase(p.x31Code) = LCase(strCode)).First)
                    End If
                Next
                lisX31 = lisX31Q
            End If

        Else

        End If
        Me.x31ID.DataSource = lisX31
        Me.x31ID.DataBind()
        If strDefX31ID <> "" Then Me.x31ID.SelectedValue = strDefX31ID
        If lisX31.Count = 0 Then
            Master.Notify("V katalogu šablon sestav není žádná položka pro tento typ entity.", NotifyLevel.InfoMessage)

        End If
        With Me.x31ID_Merge1
            .DataSource = lisX31
            .DataBind()
            .Items.Insert(0, "")
        End With
        With Me.x31ID_Merge2
            .DataSource = lisX31
            .DataBind()
            .Items.Insert(0, "")
        End With
        With Me.x31ID_Merge3
            .DataSource = lisX31
            .DataBind()
            .Items.Insert(0, "")
        End With
    End Sub


    Private Sub RenderReport()
        If Me.CurrentX31ID = 0 Or Me.MultiPIDs <> "" Then Return

        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        If cRec Is Nothing Then
            Master.StopPage("Nelze načíst sestavu.<hr>" & Master.Factory.x31ReportBL.ErrorMessage)
            Return
        End If
        Master.HeaderText = cRec.x31Name
        Me.x31ID.ToolTip = String.Format("Kód sestavy: {0}", cRec.x31Code)
        
        If Not (cRec.x31FormatFlag = BO.x31FormatFlagENUM.Telerik Or cRec.x31FormatFlag = BO.x31FormatFlagENUM.DOCX Or cRec.x31FormatFlag = BO.x31FormatFlagENUM.XLSX) Then
            Master.StopPage("NOT TRDX/DOCX/XLSX format.")
        End If
        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName

        Dim cF As New BO.clsFile
        If Not cF.FileExist(strRepFullPath) Then
            Master.Notify("XML/DOCX/XLSX soubor šablony tiskové sestavy nelze načíst.", 2)
            Return
        End If

        cmdDocMergeResult.Visible = False : opgDocResultType.Visible = False : cmdXlsResult.Visible = False
        If cRec.x31FormatFlag = BO.x31FormatFlagENUM.DOCX Then
            'DOCX
            rv1.Visible = False
            If Me.MultiPIDs = "" Then Master.HideShowToolbarButton("pdf", False)
            If Me.MultiPIDs <> "" Then Master.HideShowToolbarButton("pdf-export", False)
            Master.HideShowToolbarButton("merge", False)
            Dim cDoc As New clsMergeDOC(Master.Factory)
            If Trim(cRec.x31DocSqlSourceTabs) <> "" Then
                Dim a() As String = Split(Trim(cRec.x31DocSqlSourceTabs), "||")
                For i As Integer = 0 To UBound(a)
                    Dim b() As String = Split(a(i) & "|", "|")
                    cDoc.AddMergeRegion(b(0), b(1), b(2))
                Next
            End If
            Dim strRet As String = cDoc.MergeReport(cRec, Master.DataPID, Me.opgDocResultType.SelectedValue)
            If strRet = "" Then
                Master.Notify(cDoc.ErrorMessage, NotifyLevel.ErrorMessage)
                Return
            Else
                cmdDocMergeResult.Visible = True : cmdDocMergeResult.NavigateUrl = "binaryfile.aspx?tempfile=" & strRet
                opgDocResultType.Visible = True
            End If
            Return
        End If
        If cRec.x31FormatFlag = BO.x31FormatFlagENUM.XLSX Then
            'XLSX
            rv1.Visible = False
            If Me.MultiPIDs = "" Then Master.HideShowToolbarButton("pdf", False)
            If Me.MultiPIDs <> "" Then Master.HideShowToolbarButton("pdf-export", False)
            Master.HideShowToolbarButton("merge", False)
            period1.Visible = cRec.x31IsPeriodRequired
            Dim strRet As String = GenerateXLS(strRepFullPath)
            If strRet <> "" Then
                cmdXlsResult.Visible = True : cmdXlsResult.NavigateUrl = "binaryfile.aspx?tempfile=" & strRet
            End If
            Return
        End If
        If Me.MultiPIDs = "" Then Master.HideShowToolbarButton("pdf", True)
        If Me.MultiPIDs <> "" Then Master.HideShowToolbarButton("pdf-export", True)
        Master.HideShowToolbarButton("merge", True)
        rv1.Visible = True
        

        Dim strXmlContent As String = cF.GetFileContents(strRepFullPath, , False), bolPeriod As Boolean = False


        Dim xmlRepSource As New Telerik.Reporting.XmlReportSource()
        xmlRepSource.Xml = strXmlContent
        If strXmlContent.IndexOf("@datfrom") > 0 Or strXmlContent.IndexOf("@datuntil") > 0 Or cRec.x31IsPeriodRequired Then
            period1.Visible = True
            xmlRepSource.Parameters.Add(New Parameter("datfrom", period1.DateFrom))
            xmlRepSource.Parameters.Add(New Parameter("datuntil", period1.DateUntil))
        Else
            period1.Visible = False
        End If
        If Master.DataPID <> 0 Then
            xmlRepSource.Parameters.Add(New Parameter("pid", Master.DataPID))
        End If
        
        If strXmlContent.IndexOf("@langindex") > 0 Then            
            Select Case Me.CurrentX29ID
                Case BO.x29IdEnum.p41Project
                    Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                    xmlRepSource.Parameters.Add(New Parameter("langindex", IIf(cP41.p87ID <> 0, cP41.p87ID, cP41.p87ID_Client)))
                Case BO.x29IdEnum.p28Contact
                    Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
                    xmlRepSource.Parameters.Add(New Parameter("langindex", cP28.p87ID))
            End Select
        End If


        If Not ViewState("params") Is Nothing Then
            Dim params As Dictionary(Of String, String) = CType(ViewState("params"), Dictionary(Of String, String))
            For Each par In params
                xmlRepSource.Parameters.Add(New Parameter(par.Key, par.Value))
            Next
        End If
        Dim strExportName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "", False, cRec.PID)
        xmlRepSource.Xml = Replace(xmlRepSource.Xml, "Name=" & Chr(34) & "report1" & Chr(34), "Name=" & Chr(34) & strExportName & Chr(34))


        rv1.ReportSource = xmlRepSource
    End Sub

    Private Sub InhaleOtherInputParameters()
        ViewState("params") = Nothing

        Dim a As New Dictionary(Of String, String)

        With Request.QueryString
            For i As Integer = 0 To .Count - 1
                Select Case LCase(.GetKey(i))
                    Case "x29id", "pid", "x31pid", "x31id", "prefix"
                    Case Else
                        a.Add(.GetKey(i), .Item(i))

                End Select
            Next
        End With
        ViewState("params") = a
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("report_modal-period", Me.period1.SelectedValue)
        RenderReport()
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        If Me.MultiPIDs <> "" Then
            MultiPidsGeneratePDF()
        Else
            RenderReport()
        End If

    End Sub

    Private Sub cmdRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdRefreshOnBehind.Click
        RenderReport()
    End Sub

    Private Sub x31ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x31ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-" & Me.CurrentPrefix, Me.x31ID.SelectedValue)
        RenderReport()

    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "mail"
                Dim strURL As String = "sendmail.aspx?x31id=" & Me.CurrentX31ID.ToString & "&prefix=" & Me.CurrentPrefix & "&pid=" & Master.DataPID.ToString
                If period1.SelectedValue <> "" Then
                    strURL += "&datfrom=" & BO.BAS.FD(period1.DateFrom) & "&datuntil=" & BO.BAS.FD(period1.DateUntil)
                End If
                Server.Transfer(strURL)
            Case "pdf-export"
                MultiPidsGeneratePDF()
                
                
            Case "pdf"
                Dim strOutputFileName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "pdf", False, Me.CurrentX31ID)

                hidOutputFullPathPdf.Value = GenerateOnePDF2Temp(Me.CurrentX31ID, GenerateOnePDF2Temp(Me.CurrentX31ID, strOutputFileName))

        End Select
    End Sub

    Private Sub GenerateMerge(bolForceDownload As Boolean, bolSendByMail As Boolean)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge1-" & Me.CurrentPrefix, Me.x31ID_Merge1.SelectedValue)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge2-" & Me.CurrentPrefix, Me.x31ID_Merge2.SelectedValue)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge3-" & Me.CurrentPrefix, Me.x31ID_Merge3.SelectedValue)
        Dim reps As New List(Of String)

        Dim s As String = ""
        If Me.CurrentX31ID > 0 Then
            s = GenerateOnePDF2Temp(Me.CurrentX31ID)
            If s <> "" Then reps.Add(s)
        End If
        If Me.x31ID_Merge1.SelectedValue <> "" Then
            s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge1.SelectedValue))
            If s <> "" Then reps.Add(s)
        End If
        If Me.x31ID_Merge2.SelectedValue <> "" Then
            s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge2.SelectedValue))
            If s <> "" Then reps.Add(s)
        End If
        If Me.x31ID_Merge3.SelectedValue <> "" Then
            s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge3.SelectedValue))
            If s <> "" Then reps.Add(s)
        End If
        If reps.Count = 0 Then Return

        InhaleLic()

        Dim doc1 As New ceTe.DynamicPDF.Merger.MergeDocument()
        doc1.Author = "MARKTIME"
        For Each strFile As String In reps
            doc1.Append(Master.Factory.x35GlobalParam.TempFolder & "\" & strFile)
        Next

        If bolSendByMail Then
            'Dim strFileName As String = BO.BAS.GetGUID & ".pdf"
            Dim strFileName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "pdf", False, Me.CurrentX31ID)
            'doc1.Draw(Master.Factory.x35GlobalParam.TempFolder & "\" & strFileName)
            doc1.Draw(Master.Factory.x35GlobalParam.TempFolder & "\" & strFileName)

            Server.Transfer("sendmail.aspx?prefix=" & Me.CurrentPrefix & "&pid=" & Master.DataPID.ToString & "&tempfile=" & strFileName, False)
            Return
        End If

        Dim strExportName As String = Master.Factory.GetRecordFileName(Me.CurrentX29ID, Master.DataPID, "pdf", False, Me.CurrentX31ID)
        doc1.DrawToWeb(strExportName & ".pdf", bolForceDownload)

    End Sub
    Private Sub MultiPidsGenerateMerge(bolForceDownload As Boolean, bolSendByMail As Boolean)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge1-" & Me.CurrentPrefix, Me.x31ID_Merge1.SelectedValue)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge2-" & Me.CurrentPrefix, Me.x31ID_Merge2.SelectedValue)
        Master.Factory.j03UserBL.SetUserParam("report_modal-x31id-merge3-" & Me.CurrentPrefix, Me.x31ID_Merge3.SelectedValue)

        InhaleLic()
        Dim doc1 As New ceTe.DynamicPDF.Merger.MergeDocument()
        doc1.Author = "MARKTIME"

        Dim a() As String = Split(Me.MultiPIDs, ",")
        For Each strPID As String In a
            Master.DataPID = CInt(strPID)
            
            Dim reps As New List(Of String)
            Dim s As String = ""
            If Me.CurrentX31ID > 0 Then
                s = GenerateOnePDF2Temp(Me.CurrentX31ID)
                If s <> "" Then reps.Add(s)
            End If
            If Me.x31ID_Merge1.SelectedValue <> "" Then
                s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge1.SelectedValue))
                If s <> "" Then reps.Add(s)
            End If
            If Me.x31ID_Merge2.SelectedValue <> "" Then
                s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge2.SelectedValue))
                If s <> "" Then reps.Add(s)
            End If
            If Me.x31ID_Merge3.SelectedValue <> "" Then
                s = GenerateOnePDF2Temp(CInt(Me.x31ID_Merge3.SelectedValue))
                If s <> "" Then reps.Add(s)
            End If
            

            For Each strFile As String In reps
                doc1.Append(Master.Factory.x35GlobalParam.TempFolder & "\" & strFile)
            Next
        Next

        Master.DataPID = 0

        ''If bolSendByMail Then
        ''    Dim strFileName As String = BO.BAS.GetGUID & ".pdf"
        ''    doc1.Draw(Master.Factory.x35GlobalParam.TempFolder & "\" & strFileName)
        ''    Server.Transfer("sendmail.aspx?prefix=" & Me.CurrentPrefix & "&pid=" & Master.DataPID.ToString & "&tempfile=" & strFileName, False)
        ''End If
        doc1.DrawToWeb("MARKTIME_REPORT_MULTIPLE.pdf", bolForceDownload)

    End Sub

    Private Function GenerateOnePDF2Temp(intX31ID As Integer, Optional strOutputFileName As String = "") As String
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(intX31ID)
        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName
        Dim cRep As New clsReportOnBehind()
        cRep.Query_RecordPID = Master.DataPID
        If Not ViewState("params") Is Nothing Then
            cRep.OtherParams = ViewState("params")
        End If
        If period1.Visible Then
            cRep.Query_DateFrom = period1.DateFrom
            cRep.Query_DateUntil = period1.DateUntil
        End If


        strOutputFileName = cRep.GenerateReport2Temp(Master.Factory, strRepFullPath, , strOutputFileName)
        If strOutputFileName = "" Then
            Master.Notify("Chyba při generování PDF.", NotifyLevel.ErrorMessage) : Return ""
        End If

        Return strOutputFileName
    End Function

    Private Sub cmdMergePDF_Download_Click(sender As Object, e As EventArgs) Handles cmdMergePDF_Download.Click
        If Me.MultiPIDs <> "" Then
            MultiPidsGenerateMerge(True, False)
        Else
            GenerateMerge(True, False)
        End If

    End Sub

    Private Sub cmdMergePDF_Preview_Click(sender As Object, e As EventArgs) Handles cmdMergePDF_Preview.Click
        If Me.MultiPIDs <> "" Then
            MultiPidsGenerateMerge(False, False)
        Else
            GenerateMerge(False, False)
        End If

    End Sub

    Private Sub report_modal_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If period1.Visible And period1.SelectedValue <> "" Then
            period1.BackColor = basUI.ColorQueryRGB
        Else
            period1.BackColor = Nothing
        End If
        If rv1.Visible Then
            If Page.Culture.IndexOf("Czech") < 0 And Page.Culture.IndexOf("Če") < 0 Then
                With rv1.Resources
                    .ExportSelectFormatText = ""
                    .TogglePageLayoutToolTip = ""
                    .NextPageToolTip = ""
                    .PrintToolTip = ""
                    .PreviousPageToolTip = ""
                    .RefreshToolTip = ""
                    .LastPageToolTip = ""
                    .FirstPageToolTip = ""
                End With
            End If
        End If
    End Sub

    Private Sub cmdMergePDF_SendMail_Click(sender As Object, e As EventArgs) Handles cmdMergePDF_SendMail.Click
        If Me.MultiPIDs <> "" Then
            MultiPidsGenerateMerge(True, True)
        Else
            GenerateMerge(True, True)
        End If

    End Sub

    Private Sub opgDocResultType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgDocResultType.SelectedIndexChanged
        RenderReport()
    End Sub


    Private Function GenerateXLS(strSourceXlsFullPath As String) As String
        Dim cXLS As New clsExportToXls(Master.Factory)
        Dim sheetDef As Winnovative.ExcelLib.ExcelWorksheet = cXLS.LoadSheet(strSourceXlsFullPath, 0, "marktime_definition")
        If sheetDef Is Nothing Then
            Master.Notify("XLS soubor neobsahuje sešit s názvem [marktime_definition].", NotifyLevel.ErrorMessage)
            Return ""
        End If
        Dim sheetData As Winnovative.ExcelLib.ExcelWorksheet = Nothing
        Dim book As Winnovative.ExcelLib.ExcelWorkbook = cXLS.LoadWorkbook(strSourceXlsFullPath)
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
        Dim dtRecord As DataTable = Nothing
        For i = 1 To 1000
            Select Case LCase(sheetDef.Item(i, 1).Text)
                Case "x31name", "x31code"
                Case "recordsql"
                    Dim strSQL As String = sheetDef.Item(i, 2).Value
                    Dim pars As New List(Of BO.PluginDbParameter)
                    pars.Add(New BO.PluginDbParameter("pid", Master.DataPID))
                    pars.Add(New BO.PluginDbParameter("datfrom", period1.DateFrom))
                    pars.Add(New BO.PluginDbParameter("datuntil", period1.DateUntil))
                    dtRecord = Master.Factory.pluginBL.GetDataTable(strSQL, pars)
                Case Else
                    Dim strRange As String = sheetDef.Item(i, 1).Text
                    If strRange <> "" Then
                        Dim intRow As Integer = sheetData.Item(strRange).TopRowIndex
                        Dim intCol As Integer = sheetData.Item(strRange).LeftColumnIndex
                        Select Case LCase(sheetDef.Item(i, 3).Text)
                            Case "field"
                                If Not dtRecord Is Nothing Then
                                    Try
                                        sheetData.Item(intRow, intCol).Value = dtRecord.Rows(0).Item(sheetDef(i, 2).Value)
                                    Catch ex As Exception
                                        sheetData.Item(intRow, intCol).Value = "N/A"
                                    End Try
                                End If
                            Case Else
                                Dim strSQL As String = sheetDef.Item(i, 2).Value
                                Dim pars As New List(Of BO.PluginDbParameter)
                                pars.Add(New BO.PluginDbParameter("pid", Master.DataPID))
                                pars.Add(New BO.PluginDbParameter("datfrom", period1.DateFrom))
                                pars.Add(New BO.PluginDbParameter("datuntil", period1.DateUntil))
                                Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(strSQL, pars)
                                If Master.Factory.pluginBL.ErrorMessage <> "" Then
                                    sheetData.Item(intRow, intCol).AddComment(Master.Factory.pluginBL.ErrorMessage)
                                    sheetData.Item(intRow, intCol).Value = "N/A"
                                Else
                                    cXLS.MergeSheetWithDataTable(sheetData, dt, intRow, intCol)

                                End If

                        End Select
                    End If
                    
            End Select
        Next
        book.Worksheets.RemoveWorksheet("marktime_definition")
        Dim strResult As String = cXLS.SaveAsFile(sheetData, False)
        Dim cF As New BO.clsFile
        Return cF.GetNameFromFullpath(strResult)
    End Function
End Class