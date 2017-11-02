Imports Telerik.Reporting

Public Class report_framework_detail1
    Inherits System.Web.UI.Page

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

    Private Sub report_framework_detail1_CommitTransaction(sender As Object, e As EventArgs) Handles Me.CommitTransaction

    End Sub
   
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
                    .Add("report_framework_detail1-period")
                    .Add("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString)
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                    period1.SelectedValue = .GetUserParam("report_framework_detail1-period")


                End With

                cmdSetting.Visible = .Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
            End With

            Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
            If cRec.QueryX29ID > BO.x29IdEnum._NotSpecified Then
                Me.hidQueryPrefix.Value = Left(cRec.QueryX29ID.ToString, 3)
                SetupJ70Combo(BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString)), cRec.QueryX29ID)
            End If

            RenderReport(cRec, True)

        End If
    End Sub

    Private Sub Handle_FirstRun(bolPeriodQuery As Boolean)
        divReportViewer.Visible = False
        panFirstRun.Visible = True
        If bolPeriodQuery Then
            cmdRunReport.Text = "Vygenerovat náhled sestavy podle zvoleného filtru období"
        End If
    End Sub

    
    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("report_framework_detail1-period", Me.period1.SelectedValue)
        RenderReport()
    End Sub

    Private Sub RenderReport(Optional cRec As BO.x31Report = Nothing, Optional bolFirstRun As Boolean = False)
        cmdPdfExport.Enabled = False : linkPrint.Enabled = False : linkMail.Enabled = False

        If cRec Is Nothing Then cRec = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        If cRec Is Nothing Then
            Master.StopPage("Nelze načíst sestavu.<hr>" & Master.Factory.x31ReportBL.ErrorMessage)
            Return
        End If
        lblHeader.Text = cRec.x31Name
        If cRec.x31FormatFlag <> BO.x31FormatFlagENUM.Telerik Then
            Master.StopPage("NOT TRDX format.")
        End If
        If cRec.QueryX29ID > BO.x29IdEnum._NotSpecified And bolFirstRun Then
            Handle_FirstRun(False)
            Return
        End If
        Dim strRepFullPath As String = Master.Factory.x35GlobalParam.UploadFolder
        If cRec.ReportFolder <> "" Then
            strRepFullPath += "\" & cRec.ReportFolder
        End If
        strRepFullPath += "\" & cRec.ReportFileName

        Dim cF As New BO.clsFile
        If Not cF.FileExist(strRepFullPath) Then
            Master.Notify("XML soubor šablony tiskové sestavy nelze načíst.", 2)
            Return
        End If

        Dim strXmlContent As String = cF.GetFileContents(strRepFullPath, , False), bolPeriod As Boolean = False


        Dim xmlRepSource As New Telerik.Reporting.XmlReportSource()
        xmlRepSource.Xml = strXmlContent


        If Me.CurrentJ70ID > 0 Then
            Dim strW As String = Master.Factory.j70QueryTemplateBL.GetSqlWhere(Me.CurrentJ70ID)
            Select Case cRec.QueryX29ID
                Case BO.x29IdEnum.p41Project
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "141=141", strW)
                Case BO.x29IdEnum.p28Contact
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "328=328", strW)
                Case BO.x29IdEnum.p91Invoice
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "391=391", strW)
                Case BO.x29IdEnum.p31Worksheet
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "331=331", strW)
                Case BO.x29IdEnum.p56Task
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "356=356", strW)
                Case BO.x29IdEnum.j02Person
                    xmlRepSource.Xml = Replace(xmlRepSource.Xml, "102=102", strW)
            End Select

        End If


        If strXmlContent.IndexOf("@datfrom") > 0 Or strXmlContent.IndexOf("@datuntil") > 0 Or cRec.x31IsPeriodRequired Then
            If bolFirstRun Then
                Handle_FirstRun(True)
                Return
            End If
            period1.Visible = True
            'uriReportSource.Parameters.Add(New Parameter("datFrom", period1.DateFrom))
            'uriReportSource.Parameters.Add(New Parameter("datUntil", period1.DateUntil))

            xmlRepSource.Parameters.Add(New Parameter("datfrom", period1.DateFrom))
            xmlRepSource.Parameters.Add(New Parameter("datuntil", period1.DateUntil))

        Else
            period1.Visible = False
        End If
        divReportViewer.Visible = True
        panFirstRun.Visible = False
        ''If intJ70ID <> 0 And strXmlContent.IndexOf("1=1") > 0 Then
        ''    'externí filtr z návrháře
        ''    Dim strW As String = Master.Factory.j70SavedBasicQueryBL.CompleteReportSqlQuerySource(intJ70ID, "xa")
        ''    If strW <> "" Then
        ''        Dim strInSQL As String = "a.a01ID IN (SELECT xa.a01ID FROM a01Event xa WHERE " & strW & ")"
        ''        xmlRepSource.Xml = Replace(xmlRepSource.Xml, "1=1", strInSQL)
        ''    End If

        ''End If
        ''If strXmlContent.IndexOf("2=2") > 0 Then
        ''    Dim strW As String = GetLimitWHERE_ByUser() 'externí podmínka s omezením na aplikační roli
        ''    If strW <> "" Then
        ''        Dim strInSQL As String = "a.a01ID IN (SELECT xz.a01ID FROM a01Event xz WHERE " & strW & ")"
        ''        xmlRepSource.Xml = Replace(xmlRepSource.Xml, "2=2", strInSQL)
        ''    End If

        ''End If

        ''lblQuery.Visible = bolQueryJ70
        ''query_j70id.Visible = bolQueryJ70

        
        xmlRepSource.Xml = Replace(xmlRepSource.Xml, "Name=" & Chr(34) & "report1" & Chr(34), "Name=" & Chr(34) & GetOutputExportFileName(cRec) & Chr(34))




        rv1.ReportSource = xmlRepSource

        cmdPdfExport.Enabled = True : linkPrint.Enabled = True : linkMail.Enabled = True
    End Sub
    Private Function GetOutputExportFileName(Optional cRec As BO.x31Report = Nothing) As String
        If cRec Is Nothing Then cRec = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        Dim strExportName As String = cRec.x31ExportFileNameMask
        If strExportName = "" Then strExportName = cRec.x31Name
        ''strExportName += "_" & Format(Now, "yyyyMMddHHmmss")
        Return BO.BAS.Prepare4FileName(strExportName)
    End Function

    Private Sub report_framework_detail1_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        'lblHeader.Visible = Not Me.j70ID.Visible
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
        j70ID.Items.Insert(0, "--Pojmenovaný filtr--")
        basUI.SelectDropdownlistValue(Me.j70ID, intDef.ToString)

        Me.j70ID.Visible = True : Me.cmdQuery.Visible = True : Me.clue_query.Visible = True
    End Sub
 
    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("report_framework_detail-j70id-" & Me.CurrentX31ID.ToString, Me.CurrentJ70ID.ToString)
        RenderReport()
    End Sub

   
    Private Sub cmdRunReport_Click(sender As Object, e As EventArgs) Handles cmdRunReport.Click
        divReportViewer.Visible = True
        panFirstRun.Visible = False
        RenderReport(, False)
    End Sub

    Private Sub cmdPdfExport_Click(sender As Object, e As EventArgs) Handles cmdPdfExport.Click

        hidOutputFullPathPdf.Value = GenerateOnePDF2Temp(Me.CurrentX31ID, GenerateOnePDF2Temp(Me.CurrentX31ID, GetOutputExportFileName() & ".pdf"))

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
End Class