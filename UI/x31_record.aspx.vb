Public Class x31_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x31_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public Property CurrentFormat As BO.x31FormatFlagENUM
        Get
            Return CType(Me.x31FormatFlag.SelectedValue, BO.x31FormatFlagENUM)
        End Get
        Set(value As BO.x31FormatFlagENUM)
            basUI.SelectRadiolistValue(Me.x31FormatFlag, CInt(value).ToString)
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory
        roles1.Factory = Master.Factory
        
        If Not Page.IsPostBack Then
            ViewState("upload_changed") = "0"
            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID
            Select Case Request.Item("prefix")
                Case "plugin_x31"
                    Me.CurrentFormat = BO.x31FormatFlagENUM.ASPX
                Case "docx_x31"
                    Me.CurrentFormat = BO.x31FormatFlagENUM.DOCX
                Case Else
                    Me.CurrentFormat = BO.x31FormatFlagENUM.Telerik
            End Select
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/report_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Šablona tiskové sestavy nebo pluginu"

            End With
            Me.j25ID.DataSource = Master.Factory.j25ReportCategoryBL.GetList(New BO.myQuery)
            Me.j25ID.DataBind()


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If



        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Dim lis As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(New BO.myQuery).OrderByDescending(Function(p) p.PID)
            If lis.Count > 0 Then
                roles1.InhaleInitialData(lis(0).PID)
            End If
            Return
        End If

        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record is missing.")

        With cRec
            Master.HeaderText = .x31Name
            Me.x31Name.Text = .x31Name
            Me.x31Code.Text = .x31Code
            Me.x31Ordinary.Value = .x31Ordinary
            Me.j25ID.SelectedValue = .j25ID.ToString
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)

            Me.CurrentFormat = .x31FormatFlag
            If Me.CurrentFormat = BO.x31FormatFlagENUM.ASPX Then
                basUI.SelectDropdownlistValue(Me.x31PluginFlag, BO.BAS.IsNullInt(.x31PluginFlag).ToString)
                Me.x31PluginHeight.Value = .x31PluginHeight
            End If
            Me.x31IsPeriodRequired.Checked = .x31IsPeriodRequired
            Me.x31IsUsableAsPersonalPage.Checked = .x31IsUsableAsPersonalPage
            Me.x31DocSqlSource.Text = .x31DocSqlSource
            Me.x31DocSqlSourceTabs.Text = .x31DocSqlSourceTabs
            Me.x31ExportFileNameMask.Text = .x31ExportFileNameMask
            basUI.SelectDropdownlistValue(Me.x31QueryFlag, CInt(.x31QueryFlag).ToString)
            Me.x31IsScheduling.Checked = .x31IsScheduling
            If .x31IsScheduling Then
                Me.x31RunInTime.Text = .x31RunInTime
                Me.x31IsRunInDay1.Checked = .x31IsRunInDay1
                Me.x31IsRunInDay2.Checked = .x31IsRunInDay2
                Me.x31IsRunInDay3.Checked = .x31IsRunInDay3
                Me.x31IsRunInDay4.Checked = .x31IsRunInDay4
                Me.x31IsRunInDay5.Checked = .x31IsRunInDay5
                Me.x31IsRunInDay6.Checked = .x31IsRunInDay6
                Me.x31IsRunInDay7.Checked = .x31IsRunInDay7
                Me.x31SchedulingReceivers.Text = .x31SchedulingReceivers
                Me.x31LastScheduledRun.Text = BO.BAS.FD(.x31LastScheduledRun, True, True)
            End If

            Master.Factory.o27AttachmentBL.CopyRecordsToTemp(upload1.GUID, Master.DataPID, BO.x29IdEnum.x31Report)
            uploadlist1.RefreshData_TEMP()
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        roles1.InhaleInitialData(cRec.PID)
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x31ReportBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x31-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()
        With Master.Factory.x31ReportBL
            Dim cRec As BO.x31Report = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x31Report)

            With cRec
                .x31FormatFlag = Me.CurrentFormat
                If .x31FormatFlag = BO.x31FormatFlagENUM.ASPX Then
                    .x31PluginFlag = BO.BAS.IsNullInt(Me.x31PluginFlag.SelectedValue)
                    .x31PluginHeight = BO.BAS.IsNullInt(Me.x31PluginHeight.Value)
                End If
                .x29ID = DirectCast(BO.BAS.IsNullInt(Me.x29ID.SelectedValue), BO.x29IdEnum)
                .j25ID = BO.BAS.IsNullInt(Me.j25ID.SelectedValue)
                .x31Name = Me.x31Name.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .x31Code = Me.x31Code.Text
                .x31Ordinary = BO.BAS.IsNullInt(Me.x31Ordinary.Value)
                .x31IsPeriodRequired = Me.x31IsPeriodRequired.Checked
                .x31IsUsableAsPersonalPage = Me.x31IsUsableAsPersonalPage.Checked
                .x31DocSqlSource = Me.x31DocSqlSource.Text
                .x31DocSqlSourceTabs = Me.x31DocSqlSourceTabs.Text
                .x31ExportFileNameMask = Me.x31ExportFileNameMask.Text
                .x31QueryFlag = BO.BAS.IsNullInt(Me.x31QueryFlag.SelectedValue)

                .x31IsScheduling = Me.x31IsScheduling.Checked
                .x31IsRunInDay1 = Me.x31IsRunInDay1.Checked
                .x31IsRunInDay2 = Me.x31IsRunInDay2.Checked
                .x31IsRunInDay3 = Me.x31IsRunInDay3.Checked
                .x31IsRunInDay4 = Me.x31IsRunInDay4.Checked
                .x31IsRunInDay5 = Me.x31IsRunInDay5.Checked
                .x31IsRunInDay6 = Me.x31IsRunInDay6.Checked
                .x31IsRunInDay7 = Me.x31IsRunInDay7.Checked
                .x31RunInTime = Me.x31RunInTime.Text
                .x31SchedulingReceivers = Me.x31SchedulingReceivers.Text
            End With
            If Me.uploadlist1.ItemsCount = 0 Then
                Master.Notify("Chybí soubor šablony sestavy", NotifyLevel.WarningMessage)
                Return
            End If

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If
            Dim strSendGUID As String = ""
            If ViewState("upload_changed") = "1" Then
                strSendGUID = upload1.GUID
            End If
            If .Save(cRec, strSendGUID, lisX69) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x31-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    

  
    Private Sub upload1_AfterUploadOneFile(strFulllPath As String, strErrorMessage As String) Handles upload1.AfterUploadOneFile
        ViewState("upload_changed") = "1"
        Me.uploadlist1.RefreshData_TEMP()

        Dim cF As New BO.clsFile
        Select Case Right(LCase(strFulllPath), 4)
            Case "trdx"
                Me.CurrentFormat = BO.x31FormatFlagENUM.Telerik
                Dim s As String = cF.GetFileContents(strFulllPath, , False)
                If s.IndexOf("331=331") > 0 Then Me.x31QueryFlag.SelectedValue = "331"
                If s.IndexOf("141=141") > 0 Then Me.x31QueryFlag.SelectedValue = "141"
                If s.IndexOf("328=328") > 0 Then Me.x31QueryFlag.SelectedValue = "328"
                If s.IndexOf("391=391") > 0 Then Me.x31QueryFlag.SelectedValue = "391"
                If s.IndexOf("356=356") > 0 Then Me.x31QueryFlag.SelectedValue = "356"
                If s.IndexOf("102=102") > 0 Then Me.x31QueryFlag.SelectedValue = "102"
                If s.IndexOf("@datfrom") > 0 Or s.IndexOf("@datuntil") > 0 Then Me.x31IsPeriodRequired.Checked = True
            Case ".docx"
                Me.CurrentFormat = BO.x31FormatFlagENUM.DOCX
            Case "aspx"
                Me.CurrentFormat = BO.x31FormatFlagENUM.ASPX
            Case "xlsx"
                Me.CurrentFormat = BO.x31FormatFlagENUM.XLSX
        End Select

        Me.x31Code.Text = cF.GetNameFromFullpath(strFulllPath).Replace(cF.GetFileExtension(strFulllPath), "")
        If Me.x31Name.Text = "" Then
            Me.x31Name.Text = Me.x31Code.Text
        End If
        If Me.CurrentFormat = BO.x31FormatFlagENUM.XLSX Then
            Dim cXLS As New clsExportToXls(Master.Factory)
            Dim sheet As Winnovative.ExcelLib.ExcelWorksheet = cXLS.LoadSheet(strFulllPath, 0, "marktime_definition")
            If Not cXLS.FindPropertyValueInColumn(sheet, 1, "x31Name") Is Nothing Then
                Me.x31Name.Text = cXLS.FindPropertyValueInColumn(sheet, 1, "x31Name").ToString
            End If
        End If
    End Sub

    
    Private Sub upload1_ErrorUpload(strFileName As String, strError As String) Handles upload1.ErrorUpload
        ViewState("upload_changed") = "1"
        Master.Notify(strError, NotifyLevel.WarningMessage)
    End Sub

    Private Sub j25ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j25ID.NeedMissingItem
        Dim cRec As BO.j25ReportCategory = Master.Factory.j25ReportCategoryBL.Load(CInt(strFoundedMissingItemValue))
        strAddMissingItemText = cRec.j25Name
    End Sub

    Private Sub x31_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.x31FormatFlag.SelectedItem.Attributes.Item("style") = "font-weight:bold;color:blue;"
        x31IsPeriodRequired.Visible = True : x31IsUsableAsPersonalPage.Visible = True : panDocFormat.Visible = False : Me.x31QueryFlag.Visible = True
        Me.panScheduling.Visible = False
        x31PluginFlag.Visible = False : x31PluginHeight.Visible = False
        If Me.x29ID.SelectedValue = "" Then
            x31IsUsableAsPersonalPage.Visible = True
        Else
            x31IsUsableAsPersonalPage.Visible = False
        End If

        Select Case Me.CurrentFormat
            Case BO.x31FormatFlagENUM.DOCX
                x31IsPeriodRequired.Visible = False : x31IsUsableAsPersonalPage.Visible = False : panDocFormat.Visible = True : Me.x31QueryFlag.Visible = False
            Case BO.x31FormatFlagENUM.ASPX
                Me.x31QueryFlag.Visible = False
                x31PluginFlag.Visible = True
                If Me.x31PluginFlag.SelectedValue = "1" Then
                    x31PluginHeight.Visible = True            
                End If
            Case BO.x31FormatFlagENUM.Telerik
                If Me.x29ID.SelectedValue = "" Then
                    Me.panScheduling.Visible = True
                End If
        End Select
        panIsScheduling.Visible = Me.x31IsScheduling.Checked
        If Me.x31LastScheduledRun.Text = "" Then cmdClearLastScheduledRun.Visible = False
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub uploadlist1_AfterSetAsDeleted(cFile As BO.p85TempBox) Handles uploadlist1.AfterSetAsDeleted
        ViewState("upload_changed") = "1"
    End Sub

    Private Sub cmdClearLastScheduledRun_Click(sender As Object, e As EventArgs) Handles cmdClearLastScheduledRun.Click
        Master.Factory.x31ReportBL.UpdateLastScheduledRun(Master.DataPID, Nothing)
        Me.x31LastScheduledRun.Text = ""
    End Sub
End Class