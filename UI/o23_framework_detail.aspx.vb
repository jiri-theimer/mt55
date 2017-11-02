Public Class o23_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Public Property CurrentX18ID As Integer
        Get
            Return BO.BAS.IsNullInt(hidX18ID.Value)
        End Get
        Set(value As Integer)
            hidX18ID.Value = value.ToString
        End Set
    End Property
    

    Private Sub o23_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        rec1.Factory = Master.Factory

        tags1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            ViewState("verified") = ""
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                hidSource.Value = Request.Item("source")
                hidX18ID.Value = Request.Item("x18id")
                If Me.CurrentX18ID = 0 And .DataPID = 0 Then
                    .StopPage("")
                End If
                ''Dim lisPars As New List(Of String)
                ''With lisPars
                ''    .Add("o23_framework_detail-pid")
                ''End With
            End With

            RefreshRecord()
        End If


    End Sub
    ''Private Function FNO(strValue As String) As Telerik.Web.UI.NavigationNode
    ''    Return menu1.GetAllNodes.First(Function(p) p.ID = strValue)
    ''End Function
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        If cRec Is Nothing Then Return
       
        ''If Me.CurrentX18ID = 0 Then Me.CurrentX18ID = Master.Factory.x18EntityCategoryBL.LoadByX23ID(cRec.x23ID).PID
        Me.CurrentX18ID = cRec.x18ID
        Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(Me.CurrentX18ID)

        If cX18.x18Icon32 <> "" Then
            imgIcon32.ImageUrl = cX18.x18Icon32
        Else
            If cX18.x18IsManyItems Then imgIcon32.ImageUrl = "Images/notepad_32.png"
        End If

        Dim cDisp As BO.o23RecordDisposition = Master.Factory.o23DocBL.InhaleDisposition(cRec, cX18)
        If Not cDisp.ReadAccess Then Master.StopPage("Nemáte přístup k tomuto dokumentu.")

        pm1.Attributes.Item("onclick") = "RCM('o23'," & cRec.PID.ToString & ",this,'pagemenu')"
        With linkPM
            .Text = cRec.NameWithComboName
            If Len(.Text) > 70 Then .Text = Left(.Text, 70) & "..."
            ''.NavigateUrl = "o23_framework_detail.aspx?pid=" & cRec.PID.ToString & "&source=" & Me.hidSource.Value
            .Attributes.Item("onclick") = "RCM('o23'," & cRec.PID.ToString & ",this,'pagemenu')"
        End With
        If cRec.IsClosed Then panPM1.Style.Item("background-color") = "black" : linkPM.Style.Item("color") = "white"

        ''If Not cX18.x18IsManyItems Then
        ''    FNO("record").Text = "ZÁZNAM KATEGORIE"
        ''End If
        If cX18.b01ID <> 0 Then
            hidB01ID.Value = cX18.b01ID.ToString
            ''FNO("cmdWorkflow").Text = "Posunout stav/doplnit"
            ''FNO("cmdWorkflow").ImageUrl = "Images/workflow.png"
            ''FNO("cmdWorkflow").NavigateUrl = "javascript:workflow()"
        Else
            hidB01ID.Value = ""
            ''FNO("cmdWorkflow").Text = "Nahrát přílohu dokumentu, zapsat komentář"
            ''FNO("cmdWorkflow").ImageUrl = "Images/comment.png"
            ''FNO("cmdWorkflow").NavigateUrl = "javascript:b07_create()"
        End If


        
        ''FNO("cmdNew").Visible = cDisp.CreateItem
        ''FNO("cmdClone").Visible = cDisp.CreateItem
        ''FNO("cmdWorkflow").Visible = cDisp.UploadAndComment
        cmdLockUnlock.Visible = cDisp.LockUnlockFiles_Flag1
        If cRec.o23LockedFlag = BO.o23LockedTypeENUM.LockAllFiles Then
            cmdLockUnlock.Text = "Odemknout přístup k přílohám"
            comments1.AttachmentIsReadonly = True
        Else
            cmdLockUnlock.Text = "Uzamknout přístup k přílohám"
            comments1.AttachmentIsReadonly = False
        End If

        ''If cRec.IsClosed Then menu1.Skin = "Black"
        Dim bolShowContent As Boolean = True
        If cRec.o23IsEncrypted Then
            If ViewState("verified") <> "1" Then
                panEncrypted.Visible = True
                bolShowContent = False
            End If
        End If
        If bolShowContent Then
            rec1.Visible = True
            rec1.FillData(cRec, cX18, True)
            panEncrypted.Visible = False
        Else
            rec1.Visible = False
            ''menu1.Enabled = False
            linkPM.Enabled = False
        End If


        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.o23Doc, cRec.PID)
        roles1.RefreshData(lisX69, cRec.PID)
        
        comments1.RefreshData(Master.Factory, BO.x29IdEnum.o23Doc, Master.DataPID)

        ''FNO("reload").NavigateUrl = "o23_framework_detail.aspx?pid=" & Master.DataPID.ToString & "&x18id=" & Me.CurrentX18ID.ToString


        If cX18.x18UploadFlag = BO.x18UploadENUM.FileSystemUpload Then
            ''Fileupload_list__readonly.LockFlag = CInt(cRec.o23LockedFlag)
            Dim mq As New BO.myQueryO27
            mq.Record_x29ID = BO.x29IdEnum.o23Doc
            mq.Record_PID = cRec.PID
            Dim lisO27 As IEnumerable(Of BO.o27Attachment) = Master.Factory.o27AttachmentBL.GetList(mq)

            '' Me.Fileupload_list__readonly.RefreshData(mq)
            With Me.filesPreview
                If lisO27.Count > 0 Then
                    .Text = BO.BAS.OM2(.Text, lisO27.Count.ToString)
                    .NavigateUrl = "javascript:file_preview('o23'," & Master.DataPID.ToString & ")"
                Else
                    cmdLockUnlock.Visible = False
                End If

            End With
        Else
            panUpload.Visible = False
        End If
        
        tags1.RefreshData(cRec.PID)
    End Sub


    Private Sub o23_framework_detail_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ''With FNO("fs")
        ''    If hidSource.Value = "3" Then
        ''        .ImageUrl = "Images/fullscreen.png"
        ''        .Text = "Přepnout do přehledu"
        ''        .Width = Nothing
        ''    Else
        ''        .ImageUrl = "Images/open_in_new_window.png"
        ''        .Text = "Otevřít v nové záložce"
        ''    End If

        ''End With
        'If hidSource.Value = "2" Then

        '    ''menu1.Skin = "Metro"
        '    ''imgIcon32.Visible = False

        '    ''FNO("reload").Visible = False
        'Else

        '    ''FNO("reload").Visible = True
        'End If
        'If hidSource.Value = "3" Then

        '    ''imgIcon32.Style.Item("top") = "44px"
        'Else

        'End If
    End Sub

    Private Sub cmdLockUnlock_Click(sender As Object, e As EventArgs) Handles cmdLockUnlock.Click
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        If cRec.o23LockedFlag = BO.o23LockedTypeENUM.LockAllFiles Then
            Master.Factory.o23DocBL.UnLockFilesInDocument(cRec)
        Else
            Master.Factory.o23DocBL.LockFilesInDocument(cRec, BO.o23LockedTypeENUM.LockAllFiles)
        End If
        RefreshRecord()
    End Sub

    Private Sub cmdDecrypt_Click(sender As Object, e As EventArgs) Handles cmdDecrypt.Click
        With Master.Factory.o23DocBL
            Dim cRec As BO.o23Doc = .Load(Master.DataPID)
            Dim strPWD As String = .DecryptString(cRec.o23Password)

            If txtPassword.Text <> strPWD Then
                ViewState("verified") = "0"
                Master.Notify("Heslo není správné", NotifyLevel.WarningMessage)
                Me.txtPassword.Focus()
            Else
                ViewState("verified") = "1"
                RefreshRecord()
                Dim cTemp As New BO.p85TempBox
                cTemp.p85GUID = Master.Factory.SysUser.j02ID.ToString & "-" & Master.DataPID.ToString
                cTemp.p85FreeDate01 = Now
                Master.Factory.p85TempBoxBL.Save(cTemp)
            End If
        End With
        
        

    End Sub
End Class