Public Class o42_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o42_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "IMAP pravidlo"


            End With

            RefreshRecord()

            If Master.DataPID = 0 Then

            End If

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        Me.o41ID.DataSource = Master.Factory.o41InboxAccountBL.GetList(New BO.myQuery)
        Me.o41ID.DataBind()
        Me.p57ID.DataSource = Master.Factory.p57TaskTypeBL.GetList(New BO.myQuery)
        Me.p57ID.DataBind()
        Me.x18ID.DataSource = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery).Where(Function(p) p.x18IsManyItems = True)
        Me.x18ID.DataBind()

        If Master.DataPID = 0 Then
            Me.j02ID_Owner_Default.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner_Default.Text = Master.Factory.SysUser.PersonDesc
            Handle_ChangeTarget()
            Return
        End If

        Dim cRec As BO.o42ImapRule = Master.Factory.o42ImapRuleBL.Load(Master.DataPID)
        With cRec
            Me.o42Name.Text = .o42Name
            Me.o42SenderAddress.Text = .o42SenderAddress
            Me.o42IsUse_To.Checked = .o42IsUse_To
            Me.o42IsUse_CC.Checked = .o42IsUse_CC
            Me.o42Name.Text = .o42Name
            Me.o41ID.SelectedValue = .o41ID.ToString
            Me.p57ID.SelectedValue = .p57ID.ToString
            Me.o41ID.SelectedValue = .o41ID.ToString
            Me.x18ID.SelectedValue = .x18ID.ToString
            If .p57ID <> 0 Then
                Me.opgTarget.SelectedValue = "p56"
            End If
            If .x18ID <> 0 Then
                Me.opgTarget.SelectedValue = "o23"
            End If
            Handle_ChangeTarget()
            Me.x67ID.SelectedValue = .x67ID.ToString
            Me.p41ID_Default.Value = .p41ID_Default.ToString
            If .p41ID_Default <> 0 Then
                Me.p41ID_Default.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID_Default)
            End If
            Me.j02ID_Owner_Default.Value = .j02ID_Owner_Default.ToString
            If .j02ID_Owner_Default <> 0 Then
                Me.j02ID_Owner_Default.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.j02Person, .j02ID_Owner_Default)
            End If
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp

          
        End With
    End Sub
    Private Sub Handle_ChangeTarget()
        Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList()
        Select Case Me.opgTarget.SelectedValue
            Case "p56"
                lisX67 = lisX67.Where(Function(p) p.x29ID = BO.x29IdEnum.p56Task)
            Case "o23"
                lisX67 = lisX67.Where(Function(p) p.x29ID = BO.x29IdEnum.o23Doc)
        End Select
        Dim s As String = Me.x67ID.SelectedValue
        Me.x67ID.DataSource = lisX67
        Me.x67ID.DataBind()
        Me.x67ID.SelectedValue = s
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o42ImapRuleBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o42-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o42ImapRuleBL
            Dim cRec As BO.o42ImapRule = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o42ImapRule)
            With cRec
                .o42Name = Me.o42Name.Text
                .o42IsUse_To = Me.o42IsUse_To.Checked
                .o42IsUse_CC = Me.o42IsUse_CC.Checked
                .o42SenderAddress = Me.o42SenderAddress.Text
                .p57ID = BO.BAS.IsNullInt(Me.p57ID.SelectedValue)
                .x18ID = BO.BAS.IsNullInt(Me.x18ID.SelectedValue)
                .o41ID = BO.BAS.IsNullInt(Me.o41ID.SelectedValue)
                .j02ID_Owner_Default = BO.BAS.IsNullInt(Me.j02ID_Owner_Default.Value)
                .p41ID_Default = BO.BAS.IsNullInt(Me.p41ID_Default.Value)
                .x67ID = BO.BAS.IsNullInt(Me.x67ID.SelectedValue)

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("o42-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub o42_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.x18ID.Visible = False : Me.p57ID.Visible = False
        Select Case Me.opgTarget.SelectedValue
            Case "p56"
                lblBind.Text = "Typ zakládaného úkolu:"
                lblDefaultOwner.Text = "Výchozí vlastník zakládaného úkolu:"
                Me.p57ID.Visible = True


            Case "o23"
                Me.lblBind.Text = "Typ zakládaného dokumentu:"
                lblDefaultOwner.Text = "Výchozí vlastník zakládaného dokumentu:"
                Me.x18ID.Visible = True
        End Select
    End Sub

    Private Sub opgTarget_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgTarget.SelectedIndexChanged
        Handle_ChangeTarget()
    End Sub
End Class