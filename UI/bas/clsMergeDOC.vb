Imports Aspose.Words

Public Class clsMergeRegion
    Public Property SqlData As String
    Public Property SqlNoData As String
    Public Property RegionName As String

End Class
Public Class clsMergeDOC
    Private Property _Error As String
    Private _factory As BL.Factory
    Private _MergeRegions As List(Of clsMergeRegion)
    
    Public Sub New(factory As BL.Factory)
        factory.x35GlobalParam.InhaleParams("AsposeWordsLicense")
        Dim strLicFile As String = factory.x35GlobalParam.GetValueString("AsposeWordsLicense")
        If strLicFile = "" Then strLicFile = BO.ASS.GetApplicationRootFolder() & "\bin\ZI.dll"

        Dim license As New Aspose.Words.License()

        license.SetLicense(strLicFile)

        _factory = factory
    End Sub
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Private Function SaveDoc(ByVal doc As Document, ByVal strDestFile As String, ByVal strDestFormat As String) As Boolean
        Try
            Select Case LCase(strDestFormat)
                Case "pdf"
                    doc.Save(strDestFile, SaveFormat.Pdf)
                Case "docx"
                    doc.Save(strDestFile, SaveFormat.Docx)
                Case "doc"
                    doc.Save(strDestFile, SaveFormat.Doc)
                Case "odt"
                    doc.Save(strDestFile, SaveFormat.Odt)
                Case "rtf"
                    doc.Save(strDestFile, SaveFormat.Rtf)
                Case "htm"
                    doc.Save(strDestFile, SaveFormat.Html)
                Case Else
                    Return False
            End Select


            If System.IO.File.Exists(strDestFile) Then Return True Else Return False
        Catch ex As Exception

            W2L(strDestFile & vbCrLf & ex.Message)
            Return False
        End Try
    End Function
    Private Sub AppendDoc(ByVal dstDoc As Document, ByVal srcDoc As Document)
        For i As Integer = 0 To srcDoc.Sections.Count - 1
            'First need to import the section into the destination document,
            'this translates any document specific lists or styles.
            Dim section As Section = CType(dstDoc.ImportNode(srcDoc.Sections(i), True), Section)
            dstDoc.Sections.Add(section)
        Next i
    End Sub

    Private Sub MergeImage(ByVal strSourceDocFile As String, ByVal strImageFile As String, ByVal strMergeField As String, ByVal strDestDocFile As String)
        Dim doc As New Document(strSourceDocFile)
        Dim bd As New DocumentBuilder(doc)
        If bd.MoveToMergeField(strMergeField) Then
            bd.InsertImage(strImageFile)
            doc.Save(strDestDocFile)
        End If
    End Sub

    Private Sub W2L(ByVal strText As String)
        log4net.LogManager.GetLogger("debuglog").Error(strText)
    End Sub

    Private Function MailMerge(ByVal strSourceDocFullPath As String, intRecordPID As Integer, ByVal dt As DataTable, ByVal strDestFile As String, Optional ByVal strDestFormat As String = "pdf") As Boolean
        Dim doc As Document = Nothing
        Try
            doc = New Document(strSourceDocFullPath)
        Catch ex As Exception
            _Error = ex.Message
            W2L(_Error)
            Return False
        End Try

        Dim dstDoc As Document = doc.Clone()
        dstDoc.Sections.Clear()
        Dim dbRow As DataRow, strErr As String = "", bolSignatureImage As Boolean = False, strImageFullPath As String = ""
        If Not dt Is Nothing Then
            bolSignatureImage = dt.Columns.Contains("image1_fullpath")
            For Each dbRow In dt.Rows
                If bolSignatureImage Then
                    strImageFullPath = dbRow.Item("image1_fullpath") & ""
                End If
                Dim rowDoc As Document = doc.Clone()
                rowDoc.MailMerge.Execute(dbRow)

                MergeInnerTable(rowDoc, intRecordPID)

                AppendDoc(dstDoc, rowDoc)

            Next
            If strImageFullPath <> "" Then
                Dim bd As New DocumentBuilder(dstDoc)
                If bd.MoveToMergeField("image1") Then
                    bd.InsertImage(strImageFullPath)
                End If
            End If
        End If


        If SaveDoc(dstDoc, strDestFile, strDestFormat) Then
            Return True
        End If


        _Error = strErr
        Return False
    End Function

    

    Public Function MergeReport(cX31 As BO.x31Report, intRecordPID As Integer, Optional ByVal strExplicitDestFormat As String = "") As String
        If strExplicitDestFormat = "" Then strExplicitDestFormat = "pdf"

        Dim strSQL As String = Replace(cX31.x31DocSqlSource, "#pid#", intRecordPID.ToString, , , CompareMethod.Text)
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("pid", intRecordPID))
        Dim dt As DataTable = _factory.pluginBL.GetDataTable(strSQL, pars)
        _factory.x35GlobalParam.InhaleParams("Upload_Folder")
        Dim strRepFullPath As String = _factory.x35GlobalParam.UploadFolder
        If cX31.ReportFolder <> "" Then
            strRepFullPath += "\" & cX31.ReportFolder
        End If
        strRepFullPath += "\" & cX31.ReportFileName
        Dim strDestFileName As String = BO.BAS.GetGUID() & "." & strExplicitDestFormat
        Dim strDestFileFullPath As String = _factory.x35GlobalParam.TempFolder & "\" & strDestFileName
        If MailMerge(strRepFullPath, intRecordPID, dt, strDestFileFullPath, strExplicitDestFormat) Then
            Return strDestFileName
        Else
            Return ""
        End If

    End Function

    Private Sub MergeInnerTable(ByRef rowDoc As Document, intRecordPID As Integer)
        If _MergeRegions Is Nothing Then Return
        For Each cTab In _MergeRegions
            Dim strSQL As String = Replace(cTab.SqlData, "#pid#", intRecordPID.ToString, , , CompareMethod.Text)
            Dim pars As New List(Of BO.PluginDbParameter)
            pars.Add(New BO.PluginDbParameter("pid", intRecordPID))

            Dim dt As DataTable = _factory.pluginBL.GetDataTable(strSQL, pars)
            If _factory.pluginBL.ErrorMessage <> "" Then
                _Error = _factory.pluginBL.ErrorMessage
                Return
            End If
            If dt.Rows.Count = 0 And cTab.SqlNoData = "" Then
                For i As Integer = 0 To dt.Columns.Count - 1
                    If cTab.SqlNoData = "" Then
                        cTab.SqlNoData = "SELECT NULL as " & dt.Columns(i).ColumnName
                    Else
                        cTab.SqlNoData += ",NULL as " & dt.Columns(i).ColumnName
                    End If
                Next
            End If
            If dt.Rows.Count = 0 Then
                pars = New List(Of BO.PluginDbParameter)

                dt = _factory.pluginBL.GetDataTable(cTab.SqlNoData, pars)
            End If
            dt.TableName = cTab.RegionName
            rowDoc.MailMerge.ExecuteWithRegions(dt)
            
            
            ''rowDoc.MailMerge.ExecuteWithRegions(dt.CreateDataReader(), cTab.RegionName)
        Next

    End Sub
    Public Sub AddMergeRegion(ByVal strTabRegionName As String, ByVal strSQL As String, Optional ByVal strSQL_NoData As String = "")
        If _MergeRegions Is Nothing Then
            _MergeRegions = New List(Of clsMergeRegion)
        End If
        _MergeRegions.Add(New clsMergeRegion())
        With _MergeRegions(_MergeRegions.Count - 1)
            .RegionName = strTabRegionName
            .SqlData = strSQL
            .SqlNoData = strSQL_NoData
        End With
       
    End Sub
End Class
