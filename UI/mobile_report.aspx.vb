Imports Telerik.Reporting

Public Class mobile_report
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
    Private Property _x29id As BO.x29IdEnum

    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            If _x29id = BO.x29IdEnum._NotSpecified Then _x29id = CType(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
            Return _x29id
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public Property CurrentX31ID As Integer
        Get
            If Me.x31ID.Items.Count = 0 Then Return 0
            Return BO.BAS.IsNullInt(Me.x31ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.x31ID, value.ToString)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Request.Item("h") <> "" Then
                rv1.Height = Unit.Parse((BO.BAS.IsNullInt(Request.Item("h")) - 200).ToString & "px")
            End If
            If Request.Item("prefix") <> "" Then
                Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
            End If

            With Master
                .MenuPrefix = "report"
                If Request.Item("prefix") = "j02" Then
                    .MenuPrefix = "report_personal"
                End If
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID <> 0 Then
                    nav1.Visible = True
                    With Me.RecordHeader
                        .Text = Master.Factory.GetRecordCaption(Me.CurrentX29ID, Master.DataPID)
                        .NavigateUrl = "mobile_" & Me.CurrentPrefix & "_framework.aspx?pid=" & Master.DataPID.ToString
                        If Me.CurrentPrefix = "j02" Then .NavigateUrl = "mobile_start.aspx"
                    End With
                Else
                    nav1.Visible = False
                End If
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("periodcombo-custom_query")
                    .Add("mobile_report-x31id-" & Me.CurrentPrefix)
                    .Add("mobile_report-period-" & Len(Me.CurrentPrefix).ToString)
                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"), , True)
                    period1.SelectedValue = .GetUserParam("mobile_report-period-" & Len(Me.CurrentPrefix).ToString)

                    Dim strDefX31ID As String = Request.Item("x31id")
                    If strDefX31ID = "" Then strDefX31ID = .GetUserParam("mobile_report-x31id-" & Me.CurrentPrefix)
                    SetupX31Combo(strDefX31ID)
                    If Me.CurrentX31ID <> 0 Then
                        If Request.Item("x31id") = "" Then
                            cmdRunDefaultReport.Visible = True
                            Me.period1.Visible = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID).x31IsPeriodRequired
                        Else
                            RenderReport()
                        End If
                    End If
                End With
            End With

        End If

    End Sub


    Private Sub SetupX31Combo(strDefX31ID As String)
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Dim lisX31 As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(mq)
        If Me.CurrentPrefix <> "" Then
            lisX31 = lisX31.Where(Function(p) p.x29ID = Me.CurrentX29ID And (p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik Or p.x31FormatFlag = BO.x31FormatFlagENUM.DOCX))
        Else
            lisX31 = lisX31.Where(Function(p) p.x29ID = BO.x29IdEnum._NotSpecified And p.x31FormatFlag = BO.x31FormatFlagENUM.Telerik)
        End If
        Me.x31ID.DataSource = lisX31
        Me.x31ID.DataBind()
        Me.x31ID.Items.Insert(0, New ListItem("--Vyberte tiskovou sestavu--", ""))
        If strDefX31ID <> "" Then Me.x31ID.SelectedValue = strDefX31ID
        If lisX31.Count = 0 Then
            Master.Notify("V katalogu šablon sestav není žádná položka pro tento typ entity.", NotifyLevel.InfoMessage)

        End If

    End Sub

    Private Sub mobile_report_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            If .SelectedValue <> "" Then
                .BackColor = basUI.ColorQueryRGB
            Else
                .BackColor = Nothing
            End If
        End With
        With Me.x31ID
            If .SelectedValue <> "" Then
                .BackColor = System.Drawing.Color.Yellow
            Else
                .BackColor = Nothing
            End If
        End With
        If Me.CurrentX31ID = 0 Or cmdRunDefaultReport.Visible Then
            rv1.Visible = False
        Else
            rv1.Visible = True
        End If
    End Sub

    Private Sub RenderReport()
        cmdRunDefaultReport.Visible = False
        period1.Visible = False
        If Me.CurrentX31ID = 0 Then
            Return
        End If

        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        If cRec Is Nothing Then
            Master.StopPage("Nelze načíst sestavu.<hr>" & Master.Factory.x31ReportBL.ErrorMessage)
            Return
        End If
        period1.Visible = cRec.x31IsPeriodRequired
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

        If cRec.x31FormatFlag = BO.x31FormatFlagENUM.DOCX Then
            'DOCX
            rv1.Visible = False
            Dim cDoc As New clsMergeDOC(Master.Factory)
            If Trim(cRec.x31DocSqlSourceTabs) <> "" Then
                Dim a() As String = Split(Trim(cRec.x31DocSqlSourceTabs), "||")
                For i As Integer = 0 To UBound(a)
                    Dim b() As String = Split(a(i) & "|", "|")
                    cDoc.AddMergeRegion(b(0), b(1), b(2))
                Next
            End If
            Dim strRet As String = cDoc.MergeReport(cRec, Master.DataPID, "pdf")
            If strRet = "" Then
                Master.Notify(cDoc.ErrorMessage, NotifyLevel.ErrorMessage)
                Return
            Else
                cmdDocMergeResult.Visible = True : cmdDocMergeResult.NavigateUrl = "binaryfile.aspx?tempfile=" & strRet

            End If
            Return
        End If

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

        rv1.ReportSource = xmlRepSource
    End Sub

    Private Sub x31ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x31ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("mobile_report-x31id-" & Me.CurrentPrefix, Me.x31ID.SelectedValue)

        RenderReport()
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("mobile_report-period-" & Len(Me.CurrentPrefix).ToString, Me.period1.SelectedValue)
        RenderReport()
    End Sub

    Private Sub cmdRunDefaultReport_Click(sender As Object, e As EventArgs) Handles cmdRunDefaultReport.Click
        RenderReport()
    End Sub
End Class