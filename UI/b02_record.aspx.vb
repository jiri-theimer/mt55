Public Class b02_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub b02_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/workflow_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Workflow stav"
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If



        End If
    End Sub

    Private Sub RefreshRecord()
        Me.b01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList()
        Me.b01ID.DataBind()

        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.b02WorkflowStatus = Master.Factory.b02WorkflowStatusBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record is missing.")

        With cRec
            Me.b02IsRecordReadOnly4Owner.Checked = .b02IsRecordReadOnly4Owner
            Me.b02Name.Text = .b02Name
            Me.b02Code.Text = .b02Code
            Me.b01ID.SelectedValue = .b01ID.ToString
            Me.b02Ordinary.Value = .b02Ordinary
            basUI.SetColorToPicker(Me.b02Color, .b02Color)

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.b02WorkflowStatusBL
            If .Delete(Master.DataPID) Then
                Master.CloseAndRefreshParent("b02-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.b02WorkflowStatusBL
            Dim cRec As BO.b02WorkflowStatus = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.b02WorkflowStatus)

            With cRec
                .b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)
                .b02Name = Me.b02Name.Text
                .b02Ordinary = BO.BAS.IsNullInt(Me.b02Ordinary.Value)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .b02Code = Me.b02Code.Text
                .b02Color = basUI.GetColorFromPicker(Me.b02Color)
                .b02IsRecordReadOnly4Owner = Me.b02IsRecordReadOnly4Owner.Checked
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("b02-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class