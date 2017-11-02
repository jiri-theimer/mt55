Public Class x46_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x46_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
           
            With Master
                .HeaderIcon = "Images/setting_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení notifikačního pravidla"

            End With

            Me.x45ID.DataSource = Master.Factory.ftBL.GetList_X45(New BO.myQuery).Where(Function(p) p.x45IsAllowNotification = True)
            Me.x45ID.DataBind()

            Me.j11ID.DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            Me.j11ID.DataBind()

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If

        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If
        Dim cRec As BO.x46EventNotification = Master.Factory.x46EventNotificationBL.Load(Master.DataPID)
        With cRec
            Me.x45ID.SelectedValue = CInt(.x45ID).ToString
            RefreshState()

            Me.j02ID.Value = .j02ID.ToString
            Me.j02ID.Text = .Person
            Me.j11ID.SelectedValue = .j11ID.ToString
            basUI.SelectDropdownlistValue(Me.x29ID_Reference, CInt(.x29ID_Reference).ToString)
            Me.x67ID.SelectedValue = .x67ID.ToString
            Me.x46IsExcludeAuthor.Checked = .x46IsExcludeAuthor
            Me.x46IsForRecordOwner.Checked = .x46IsForRecordOwner
            Me.x46IsForRecordOwner_Reference.Checked = .x46IsForRecordOwner_Reference
            Me.x46IsUseSystemTemplate.Checked = .x46IsUseSystemTemplate
            Me.x46MessageTemplate.Text = .x46MessageTemplate
            Me.x46MessageSubject.Text = .x46MessageSubject
            Me.x46IsForAllRoles.Checked = .x46IsForAllRoles
            Me.x46IsForAllReferenceRoles.Checked = .x46IsForAllReferenceRoles

            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x46EventNotificationBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x46-delete")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        Server.Transfer("x46_record.aspx?pid=" & Master.DataPID.ToString)
    End Sub
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x46EventNotificationBL
            Dim cRec As BO.x46EventNotification = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x46EventNotification)
            With cRec
                .x45ID = CType(BO.BAS.IsNullInt(Me.x45ID.SelectedValue), BO.x45IDEnum)
                .j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
                .j11ID = BO.BAS.IsNullInt(Me.j11ID.SelectedValue)
                .x67ID = BO.BAS.IsNullInt(Me.x67ID.SelectedValue)
                .x29ID_Reference = CType(BO.BAS.IsNullInt(Me.x29ID_Reference.SelectedValue), BO.x29IdEnum)
                .x46IsExcludeAuthor = Me.x46IsExcludeAuthor.Checked
                .x46IsForRecordOwner = Me.x46IsForRecordOwner.Checked
                .x46IsForRecordOwner_Reference = Me.x46IsForRecordOwner_Reference.Checked
                .x46MessageSubject = Me.x46MessageSubject.Text
                .x46MessageTemplate = Me.x46MessageTemplate.Text
                .x46IsUseSystemTemplate = Me.x46IsUseSystemTemplate.Checked
                .x46IsForAllReferenceRoles = Me.x46IsForAllReferenceRoles.Checked
                .x46IsForAllRoles = Me.x46IsForAllRoles.Checked

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x46-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub x46_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        RefreshState()
        
       
    End Sub

    Private Sub RefreshState()
        Me.panReference.Visible = False : Me.x29ID_Reference.Enabled = True : lblMessage.Text = ""
        Me.x67ID.Visible = False : Me.lblX67ID.Visible = False
        Dim strLastX67ID As String = Me.x67ID.SelectedValue
        With Me.x29ID_Reference.Items
            .FindByValue("331").Enabled = True
            .FindByValue("223").Enabled = True
            .FindByValue("222").Enabled = True
        End With
        If Me.x45ID.SelectedIndex > 0 Then
            Dim cX45 As BO.x45Event = Master.Factory.ftBL.LoadX45(BO.BAS.IsNullInt(Me.x45ID.SelectedValue))
            Me.x45MessageTemplate.Text = cX45.x45MessageTemplate
            Me.panReference.Visible = cX45.x45IsReference
            Me.panReceiver.Visible = Not cX45.x45IsManualReceiver
            If cX45.x45IsReference Then
                Select Case cX45.x29ID
                    Case BO.x29IdEnum.p56Task
                        Me.x29ID_Reference.Enabled = False
                        Me.x29ID_Reference.SelectedValue = "141"
                    Case BO.x29IdEnum.o22Milestone, BO.x29IdEnum.o23Doc
                        With Me.x29ID_Reference.Items
                            .FindByValue("331").Enabled = False
                            .FindByValue("223").Enabled = False
                            .FindByValue("222").Enabled = False
                        End With
                End Select
                
            End If
            If Not cX45.x45IsManualReceiver Then
                lblReceiverMessage.Text = ""
                If cX45.x29ID > BO.x29IdEnum._NotSpecified Then
                    Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = cX45.x29ID)
                    Me.x67ID.DataSource = lisX67
                    Me.x67ID.DataBind()
                    Me.x67ID.SelectedValue = strLastX67ID
                    If lisX67.Count > 0 Then
                        Me.x67ID.Visible = True : Me.lblX67ID.Visible = True
                    End If
                    Dim lisX29 As IEnumerable(Of BO.x29Entity) = Master.Factory.ftBL.GetList_X29().Where(Function(p) p.PID = CInt(cX45.x29ID))
                    If lisX29.Count > 0 Then
                        Me.x46IsForRecordOwner.Text = BO.BAS.OM2(Me.x46IsForRecordOwner.Text, lisX29(0).x29NameSingle)
                    End If
                End If
                Select Case cX45.x29ID
                    Case BO.x29IdEnum.o22Milestone
                        Me.lblMessage.Text = "Automatickým příjemcem této události budou všechny zapojené osoby do dané kalendářové události (milníku)."
                    Case BO.x29IdEnum.b07Comment
                        Me.lblMessage.Text = "V případě reakce na komentář bude automatickým příjemcem autor původního komentáře."
                End Select
            Else
                lblReceiverMessage.Text = "Příjemce notifikace určuje ručně uživatel, který vyvolal tuto událost."
            End If
            x46IsExcludeAuthor.Visible = Me.panReceiver.Visible
            x46IsForRecordOwner.Visible = Me.panReceiver.Visible
            x46IsForRecordOwner_Reference.Visible = Me.panReceiver.Visible
            x46IsForAllReferenceRoles.Visible = Me.panReceiver.Visible
            x67ID_Reference.Visible = Me.panReceiver.Visible
            lblx67ID_Reference.Visible = Me.panReceiver.Visible
        End If

        If Me.x29ID_Reference.SelectedValue <> "" And Me.panReference.Visible Then
            Dim s As String = Me.x67ID_Reference.SelectedValue
            Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = CInt(Me.x29ID_Reference.SelectedValue))
            Me.x67ID_Reference.DataSource = lisX67
            Me.x67ID_Reference.DataBind()
            Me.x67ID_Reference.SelectedValue = s
            If lisX67.Count > 0 And Me.panReceiver.Visible Then
                Me.x67ID_Reference.Visible = True
            Else
                Me.x67ID_Reference.Visible = False
            End If
        End If
        Me.x46MessageTemplate.Visible = Not Me.x46IsUseSystemTemplate.Checked
        Me.x45MessageTemplate.Visible = Not Me.x46MessageTemplate.Visible
        Me.x46IsForAllRoles.Visible = Me.x67ID.Visible
        If Me.panReceiver.Visible Then
            Me.x46IsForAllReferenceRoles.Visible = Me.x67ID_Reference.Visible



            If Me.x46IsForAllRoles.Visible Then
                Me.x67ID.Visible = Not Me.x46IsForAllRoles.Checked
            End If
            If Me.x46IsForAllReferenceRoles.Visible Then
                Me.x67ID_Reference.Visible = Not Me.x46IsForAllReferenceRoles.Checked
            End If
        End If
    End Sub
End Class