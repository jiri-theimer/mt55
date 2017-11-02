Public Class x38_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x38_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení číselné řady"
            End With
            


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then
            Me.x38Scale.Value = 4
            Return
        Else
            Me.x29ID.Enabled = False
        End If

        Dim cRec As BO.x38CodeLogic = Master.Factory.x38CodeLogicBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            Me.x38Name.Text = .x38Name
            If .x38Scale < 3 Or .x38Scale > 6 Then
                Me.x38Scale.Value = 4
            Else
                Me.x38Scale.Value = .x38Scale
            End If
            Me.x38IsUseDbPID.Checked = .x38IsUseDbPID
            Me.x38ConstantBeforeValue.Text = .x38ConstantBeforeValue
            Me.x38ConstantAfterValue.Text = .x38ConstantAfterValue
            basUI.SelectRadiolistValue(Me.x38EditModeFlag, CInt(.x38EditModeFlag).ToString)
            Me.x38MaskSyntax.Text = .x38MaskSyntax
            Me.x38IsDraft.Checked = .x38IsDraft
            Master.Timestamp = .Timestamp
            Me.x38ExplicitIncrementStart.Value = .x38ExplicitIncrementStart


        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x38CodeLogicBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x38-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x38CodeLogicBL
            Dim cRec As BO.x38CodeLogic = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x38CodeLogic)
            With cRec
                .x29ID = BO.BAS.IsNullInt(Me.x29ID.SelectedValue)
                .x38Name = Me.x38Name.Text
                .x38EditModeFlag = DirectCast(CInt(Me.x38EditModeFlag.SelectedValue), BO.x38EditModeFlagENUM)
                .x38Scale = BO.BAS.IsNullInt(Me.x38Scale.Value)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .x38ConstantBeforeValue = Me.x38ConstantBeforeValue.Text
                .x38ConstantAfterValue = Me.x38ConstantAfterValue.Text
                .x38MaskSyntax = Me.x38MaskSyntax.Text
                .x38IsDraft = Me.x38IsDraft.Checked
                .x38ExplicitIncrementStart = BO.BAS.IsNullInt(x38ExplicitIncrementStart.Value)
                .x38IsUseDbPID = Me.x38IsUseDbPID.Checked
            End With
            

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID


                Master.CloseAndRefreshParent("x38-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub x38_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Select Case Me.x29ID.SelectedValue
            Case "391", "390", "141", "328", "223"
                Me.x38IsDraft.Visible = True
            Case Else
                Me.x38IsDraft.Visible = False
        End Select
        Me.tabMore.Visible = Not Me.x38IsUseDbPID.Checked

    End Sub
End Class