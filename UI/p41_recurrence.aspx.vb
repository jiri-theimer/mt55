Public Class p41_recurrence
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p41_recurrence_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderText = .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .DataPID)
                
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With

            Me.p65ID.DataSource = Master.Factory.p65RecurrenceBL.GetList(New BO.myQuery)
            Me.p65ID.DataBind()
            Me.p65ID.Items.Insert(0, "--Projekt není matkou opakovaných projektů--")

            RefreshRecord()
        End If
    End Sub
    Private Sub RefreshRecord()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        With crec
            Me.p41RecurNameMask.Text = .p41RecurNameMask
            basUI.SelectDropdownlistValue(Me.p65ID, .p65ID.ToString)
            Me.p41IsStopRecurrence.Checked = .p41IsStopRecurrence
            If Not .p41RecurBaseDate Is Nothing Then Me.p41RecurBaseDate.SelectedDate = .p41RecurBaseDate
        End With
    End Sub
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
            With cRec
                .p65ID = BO.BAS.IsNullInt(Me.p65ID.SelectedValue)
                If .p65ID <> 0 Then
                    .p41RecurBaseDate = Me.p41RecurBaseDate.SelectedDate
                    .p41RecurNameMask = Me.p41RecurNameMask.Text
                    .p41IsStopRecurrence = Me.p41IsStopRecurrence.Checked
                End If
            End With

            With Master.Factory.p41ProjectBL
                If .Save(cRec, Nothing, Nothing, Nothing, Nothing) Then
                    
                    Master.CloseAndRefreshParent("p41-save")
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With
        End If
    End Sub

    Private Sub p41_recurrence_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.p65ID.SelectedIndex > 0 Then
            panRecurrence.Visible = True
        Else
            panRecurrence.Visible = False
        End If
    End Sub
End Class