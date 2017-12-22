Imports Telerik.Reporting
Public Class clsReportOnBehind
    Public Property Query_DateFrom As Date = DateSerial(1900, 1, 1)
    Public Property Query_DateUntil As Date = DateSerial(3000, 1, 1)
    Public Property Query_RecordPID As Integer = 0
    Public Property OtherParams As Dictionary(Of String, String) = Nothing


    Public Function GenerateReport2Temp(factory As BL.Factory, strFullPathTRDX As String, Optional strFormat As String = "PDF", Optional strOutputFileName As String = "") As String

        Dim settings As New System.Xml.XmlReaderSettings()
        settings.IgnoreWhitespace = True

        ''Dim fileStream As New System.IO.StreamReader(strFullPathTRDX)
        
        If BO.ASS.GetConfigVal("cloud") = "1" Then
            Dim cF As New BO.clsFile
            Dim strXML As String = cF.GetFileContents(strFullPathTRDX, , False)
            Dim strDbConString As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationPrimary").ToString
            strDbConString = Replace(strDbConString, "cloud-db-template", BO.BAS.ParseDbNameFromCloudLogin(factory.SysUser.j03Login), , 1, CompareMethod.Binary)
            strXML = Replace(strXML, "ApplicationPrimary", strDbConString)

            'strFullPathTRDX = factory.x35GlobalParam.TempFolder & "\" & factory.SysUser.PID.ToString & "rep.trdx"
            Randomize()
            strFullPathTRDX = factory.x35GlobalParam.TempFolder & "\" & factory.SysUser.PID.ToString & "_" & (CInt(Rnd() * 1000000)).ToString & ".trdx"
            cF.SaveText2File(strFullPathTRDX, strXML, , , False)
            ''fileStream = New System.IO.StreamReader(strFullPathTRDX)
        End If



        ''Dim xmlReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(fileStream, settings)
        Dim xmlReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(strFullPathTRDX, settings)

        Dim xmlSerializer As New XmlSerialization.ReportXmlSerializer()

        Dim report As Telerik.Reporting.Report = DirectCast(xmlSerializer.Deserialize(xmlReader), Telerik.Reporting.Report)



        Dim reportProcessor As New Processing.ReportProcessor()
        Dim instanceReportSource As New InstanceReportSource()
        instanceReportSource.ReportDocument = report
        instanceReportSource.Parameters.Add(New Parameter("datfrom", Me.Query_DateFrom))
        instanceReportSource.Parameters.Add(New Parameter("datuntil", Me.Query_DateUntil))
        instanceReportSource.Parameters.Add(New Parameter("pid", Me.Query_RecordPID))

        If Not Me.OtherParams Is Nothing Then
            For Each par In Me.OtherParams
                instanceReportSource.Parameters.Add(New Parameter(par.Key, par.Value))
            Next
        End If


        Dim result As Processing.RenderingResult = reportProcessor.RenderReport(strFormat, instanceReportSource, Nothing)
        Dim b As Boolean = False
        If strOutputFileName = "" Then strOutputFileName = "MARKTIME_REPORT_" & BO.BAS.GetGUID & "." & strFormat

        Using fs As New System.IO.FileStream(factory.x35GlobalParam.TempFolder & "\" & strOutputFileName, System.IO.FileMode.Create)
            fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length)
            fs.Close()
            b = True
        End Using
        If b Then

            Return strOutputFileName
        Else
            Return ""
        End If


    End Function
End Class
