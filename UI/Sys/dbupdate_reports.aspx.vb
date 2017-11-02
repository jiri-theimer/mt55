Imports System.Xml

Public Class dbupdate_reports
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private _dir As String
    Private Class RecordINI
        Public x31FileName As String
        Public x31FormatFlag As Integer
        Public x31name As String
        Public personalpage As Boolean
        Public j25id As Integer
        Public x29id As Integer
        Public x31PluginFlag As Integer
    End Class

    Private Sub dbupdate_reports_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        _dir = BO.ASS.GetApplicationRootFolder & "\sys\reports"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.NoMenu = True
        If Not Page.IsPostBack Then
            If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\sys\reports\reports.ini") Then
                Master.StopPage("Chybí konfigurační soubor: \sys\reports\reports.ini")
            End If
            If Master.Factory.j11TeamBL.GetList().Where(Function(p) p.j11IsAllPersons = True).Count = 0 Then
                Me.lblError.Text = "Nelze definovat přístupová práva k sestavě (j11TeamBL)."
            End If
            If Master.Factory.x67EntityRoleBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.x31Report).Count = 0 Then
                Me.lblError.Text += " # Nelze definovat přístupová práva k sestavě (x67EntityRoleBL)."
            End If
            If Master.Factory.o13AttachmentTypeBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.x31Report).Count = 0 Then
                Me.lblError.Text += " # Nelze definovat přístupová práva k sestavě (o13AttachmentTypeBL)."
            End If

            Dim strDIR As String = BO.ASS.GetApplicationRootFolder & "\sys\reports"
            Dim cF As New BO.clsFile
            Dim lis As List(Of String) = cF.GetFileListFromDir(strDIR, "*.*")


            rp1.DataSource = lis
            rp1.DataBind()


            If BO.ASS.GetConfigVal("dbupdate-dbs") <> "" Then
                Dim names As List(Of String) = BO.BAS.ConvertDelimitedString2List(BO.ASS.GetConfigVal("dbupdate-dbs"), ";")
                If names.Count > 0 Then
                    panMultiDbs.Visible = True
                    For Each s In names
                        Me.dbs.Items.Add(s)
                    Next
                End If


            End If
        End If
    End Sub


    Private Sub HandleImport()
        Dim strDIR As String = BO.ASS.GetApplicationRootFolder & "\sys\reports"
        Dim cF As New BO.clsFile
        Dim lis As List(Of String) = cF.GetFileListFromDir(strDIR, "*.*")

        Dim lisX31Pre As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(New BO.myQuery)

        Dim cRole As New BO.x69EntityRole_Assign()
        cRole.j11ID = Master.Factory.j11TeamBL.GetList().Where(Function(p) p.j11IsAllPersons = True)(0).PID
        cRole.x67ID = Master.Factory.x67EntityRoleBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.x31Report)(0).PID
        Dim intO13ID As Integer = Master.Factory.o13AttachmentTypeBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.x31Report)(0).PID
        For Each strFileName In lis.Where(Function(p) Trim(p) <> "")
            Dim cRI As RecordINI = LoadRecordINI(strFileName)
            If Not cRI Is Nothing Then
                Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.LoadByFilename(cRI.x31FileName)
                If cRec Is Nothing Then
                    'zjistit, zda neexistuje podle shody v x31Code
                    cRec = Master.Factory.x31ReportBL.LoadByCode(Replace(cRI.x31FileName, ".trdx", "", , , CompareMethod.Text).Replace(".aspx", ""))
                End If
                If cRec Is Nothing Then
                    cRec = New BO.x31Report
                    cRec.j25ID = cRI.j25id
                    If lisX31Pre.Count < 5 Then
                        'úvodní import sestav
                        cRec.ValidFrom = DateSerial(2017, 1, 1)
                    End If
                End If
                With cRec
                    .x29ID = CType(cRI.x29id, BO.x29IdEnum)
                    .x31Code = cRI.x31FileName.Replace(".trdx", "").Replace(".aspx", "")
                    .x31FormatFlag = CType(cRI.x31FormatFlag, BO.x31FormatFlagENUM)
                    .x31IsUsableAsPersonalPage = cRI.personalpage
                    .x31Name = cRI.x31name
                    If .x31FormatFlag = BO.x31FormatFlagENUM.ASPX Then
                        .x31PluginFlag = CType(cRI.x31PluginFlag, BO.x31PluginFlagENUM)
                        If .x31PluginFlag = BO.x31PluginFlagENUM._AfterEntityMenu Then .x31PluginHeight = 30

                        If .x31PluginFlag = BO.x31PluginFlagENUM._AfterEntityMenu Then
                            cF.CopyFile(strDIR & "\" & strFileName, BO.ASS.GetApplicationRootFolder & "\Plugins\" & strFileName)
                        End If
                    End If
                End With
                If cF.FileExist(strDIR & "\" & strFileName) Then
                    Dim s As String = cF.GetFileContents(strDIR & "\" & strFileName, , False)
                    If s.IndexOf("331=331") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.p31
                    If s.IndexOf("141=141") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.p41
                    If s.IndexOf("328=328") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.p28
                    If s.IndexOf("391=391") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.p91
                    If s.IndexOf("356=356") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.p56
                    If s.IndexOf("102=102") > 0 Then cRec.x31QueryFlag = BO.x31QueryFlagENUM.j02
                    If s.IndexOf("@datfrom") > 0 Or s.IndexOf("@datuntil") > 0 Then cRec.x31IsPeriodRequired = True
                End If
                
                Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
                lisX69.Add(cRole)
                Dim strUploadGUID As String = BO.BAS.GetGUID
                If Not Master.Factory.o27AttachmentBL.UploadAndSaveOnFile2Temp(strUploadGUID, strDIR & "\" & strFileName, intO13ID) Then
                    WE(strFileName & " | " & Master.Factory.o27AttachmentBL.ErrorMessage)
                End If


                If Not Master.Factory.x31ReportBL.Save(cRec, strUploadGUID, lisX69) Then
                    WE(strFileName & " | " & Master.Factory.x31ReportBL.ErrorMessage)
                End If
            End If
        Next


    End Sub

    Private Sub WE(strError As String)
        Me.lblError.Text += "<hr>" & strError
    End Sub

    Private Function LoadRecordINI(strFileName As String) As RecordINI
        Dim cINI As New clsINIFile()
        Dim strFind As String = cINI.read(_dir & "\reports.ini", LCase(strFileName), "x31name")
        If strFind <> "" Then
            Dim c As New RecordINI
            c.x31FileName = strFileName
            c.x31FormatFlag = BO.BAS.IsNullInt(cINI.read(_dir & "\reports.ini", LCase(strFileName), "x31FormatFlag"))
            If c.x31FormatFlag = 3 Then
                c.x31PluginFlag = BO.BAS.IsNullInt(cINI.read(_dir & "\reports.ini", LCase(strFileName), "x31PluginFlag"))
            End If
            c.x31name = strFind
            c.j25id = BO.BAS.IsNullInt(cINI.read(_dir & "\reports.ini", LCase(strFileName), "j25id"))
            c.personalpage = BO.BAS.BG((cINI.read(_dir & "\reports.ini", LCase(strFileName), "personalpage")))
            c.x29id = BO.BAS.IsNullInt(cINI.read(_dir & "\reports.ini", LCase(strFileName), "x29id"))
            Return c
        Else
            Return Nothing
        End If
    End Function

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click
        Me.lblError.Text = ""
        HandleImport()
        HandleImportX55()
        If Me.lblError.Text = "" Then
            Master.Notify("Aktualizace byla dokončena", NotifyLevel.InfoMessage)
            cmdGo.Visible = False
        Else
            Master.Notify("Aktualizace byla dokončena s chybami", NotifyLevel.WarningMessage)
        End If
        cmdDefPage.Visible = True

    End Sub

    Private Sub HandleImportX55()
        Dim xml As New XmlDocument()
        xml.Load(BO.ASS.GetApplicationRootFolder & "\sys\reports\gadgets.xml")
        Dim records As XmlNodeList = xml.SelectNodes("/x55HtmlSnippet/record")

        For Each record As XmlNode In records
            Dim strCode As String = GetNodeContent(record, "x55code")
            If strCode <> "" Then
                Dim cRec As BO.x55HtmlSnippet = Master.Factory.x55HtmlSnippetBL.LoadByCode(strCode)
                If cRec Is Nothing Then
                    cRec = New BO.x55HtmlSnippet
                    cRec.x55Code = strCode
                End If
                With cRec
                    .x55IsSystem = True
                    .x55Content = GetNodeContent(record, "x55content")
                    .x55Name = GetNodeContent(record, "x55name")
                    .x55RecordSQL = GetNodeContent(record, "x55recordsql")
                    .x55Height = GetNodeContent(record, "x55height")
                    Dim intTypeFlag As Integer = BO.BAS.IsNullInt(GetNodeContent(record, "x55typeflag"))
                    If intTypeFlag > 0 Then
                        .x55TypeFlag = CType(intTypeFlag, BO.x55TypeENUM)
                    End If
                End With
                Dim x56 As XmlNode = record.SelectSingleNode("x56")
                Dim lisX56 As New List(Of BO.x56SnippetProperty)

                If Not x56 Is Nothing Then
                    For Each n As XmlNode In x56.ChildNodes
                        Dim cX56 As New BO.x56SnippetProperty
                        cX56.x56ControlPropertyName = n.Name
                        cX56.x56ControlPropertyValue = n.InnerXml
                        If Not n.Attributes("x56control") Is Nothing Then
                            cX56.x56Control = n.Attributes("x56control").Value
                        End If

                        lisX56.Add(cX56)
                    Next
                End If

                If Not Master.Factory.x55HtmlSnippetBL.Save(cRec, lisX56) Then
                    WE(strCode & " | " & Master.Factory.x55HtmlSnippetBL.ErrorMessage)
                End If

            End If

        Next
    End Sub

    Private Function GetNodeContent(nodeMaster As XmlNode, strNodeSlaveName As String) As String
        Dim xx As XmlNode = nodeMaster.SelectSingleNode(strNodeSlaveName)

        If Not xx Is Nothing Then
            Return Trim(xx.InnerXml)
        Else
            Return ""
        End If
    End Function

    Private Sub cmdDefPage_Click(sender As Object, e As EventArgs) Handles cmdDefPage.Click
        Response.Redirect("../default.aspx")
    End Sub

    
    Private Sub cmdGoDbs_Click(sender As Object, e As EventArgs) Handles cmdGoDbs.Click
        Dim s As String = String.Format("server=Sql.mycorecloud.net\MARKTIME;database={0};uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;", Me.dbs.SelectedItem.Text)
        Master.Factory.ChangeConnectString(s)
        HandleImport()
        With Master.Factory.x31ReportBL.GetList(New BO.myQuery).OrderByDescending(Function(p) p.PID)(0)
            lblDbsMessage.Text = .x31Name & " (" & .DateInsert.ToString & ")"
        End With

    End Sub

    Private Sub dbs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dbs.SelectedIndexChanged
        lblDbsMessage.Text = ""
    End Sub
End Class