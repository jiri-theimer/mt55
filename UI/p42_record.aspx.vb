Public Class p42_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p42_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Typ projektu"

               
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
        Me.p34ids.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery)
        Me.p34ids.DataBind()
        Dim lisX38 As IEnumerable(Of BO.x38CodeLogic) = Master.Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p41Project)
        Me.x38ID.DataSource = lisX38.Where(Function(p) p.x38IsDraft = False)
        Me.x38ID.DataBind()
        Me.x38ID_Draft.DataSource = lisX38.Where(Function(p) p.x38IsDraft = True)
        Me.x38ID_Draft.DataBind()
        Me.b01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project)
        Me.b01ID.DataBind()
        Me.f02ID.DataSource = Master.Factory.f01FolderBL.GetList_f02(New BO.myQuery).Where(Function(p) p.f02ParentID = 0)
        Me.f02ID.DataBind()


        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(Master.DataPID)
        With cRec
            Me.p42Name.Text = .p42Name
            Me.p42Ordinary.Value = .p42Ordinary
            Me.p42Code.Text = .p42Code
            Me.b01ID.SelectedValue = .b01ID.ToString
            Me.x38ID.SelectedValue = .x38ID.ToString
            Me.x38ID_Draft.SelectedValue = .x38ID_Draft.ToString
            Me.f02ID.SelectedValue = .f02ID.ToString
            Me.p42IsDefault.Checked = .p42IsDefault
            Me.p42IsModule_p31.Checked = .p42IsModule_p31
            Me.p42IsModule_o23.Checked = .p42IsModule_o23
            Me.p42IsModule_p56.Checked = .p42IsModule_p56
            Me.p42IsModule_p45.Checked = .p42IsModule_p45
            Me.p42IsModule_o22.Checked = .p42IsModule_o22
            Me.p42IsModule_p48.Checked = .p42IsModule_p48
            basUI.SelectDropdownlistValue(Me.p42SubgridO23Flag, .p42SubgridO23Flag.ToString)
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp

            basUI.SelectDropdownlistValue(Me.p42ArchiveFlag, CInt(.p42ArchiveFlag).ToString)
            basUI.SelectDropdownlistValue(Me.p42ArchiveFlagP31, CInt(.p42ArchiveFlagP31).ToString)

            Dim lis As IEnumerable(Of BO.p43ProjectType_Workload) = Master.Factory.p42ProjectTypeBL.GetList_p43(Master.DataPID)
            basUI.CheckItems(Me.p34ids, lis.Select(Function(p) p.p34ID).ToList)
            
            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p42ProjectTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p42-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p42ProjectTypeBL
            Dim cRec As BO.p42ProjectType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p42ProjectType)
            cRec.p42Name = Me.p42Name.Text
            cRec.p42Ordinary = BO.BAS.IsNullInt(Me.p42Ordinary.Value)
            cRec.p42Code = Me.p42Code.Text
            cRec.b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)
            cRec.x38ID = BO.BAS.IsNullInt(Me.x38ID.SelectedValue)
            cRec.x38ID_Draft = BO.BAS.IsNullInt(Me.x38ID_Draft.SelectedValue)
            cRec.f02ID = BO.BAS.IsNullInt(Me.f02ID.SelectedValue)
            cRec.p42IsDefault = Me.p42IsDefault.Checked
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil
            cRec.p42ArchiveFlag = CInt(Me.p42ArchiveFlag.SelectedValue)
            cRec.p42ArchiveFlagP31 = CInt(Me.p42ArchiveFlagP31.SelectedValue)
            cRec.p42IsModule_p31 = Me.p42IsModule_p31.Checked
            cRec.p42IsModule_o23 = Me.p42IsModule_o23.Checked
            cRec.p42IsModule_p56 = Me.p42IsModule_p56.Checked
            cRec.p42IsModule_p45 = Me.p42IsModule_p45.Checked
            cRec.p42IsModule_o22 = Me.p42IsModule_o22.Checked
            cRec.p42IsModule_p48 = Me.p42IsModule_p48.Checked
            cRec.p42SubgridO23Flag = CInt(Me.p42SubgridO23Flag.SelectedValue)

            Dim mq As New BO.myQuery
            mq.AddItemToPIDs(-1)
            For Each intP34ID As Integer In basUI.GetCheckedItems(Me.p34ids)
                mq.AddItemToPIDs(intP34ID)
            Next
            Dim lisP43 As New List(Of BO.p43ProjectType_Workload)
            For Each intP34ID As Integer In basUI.GetCheckedItems(Me.p34ids)
                Dim cP43 As New BO.p43ProjectType_Workload
                cP43.p34ID = intP34ID
                lisP43.Add(cP43)
            Next
            If .Save(cRec, lisP43) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p42-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    
   
    Private Sub p42_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panP34IDs.Visible = Me.p42IsModule_p31.Checked
        Me.p42SubgridO23Flag.Visible = Me.p42IsModule_o23.Checked
    End Sub
End Class