Public Class p65_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p65_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Pravidlo opakovaného projektu a úkolu"
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p65Recurrence = Master.Factory.p65RecurrenceBL.Load(Master.DataPID)
        With cRec
            basUI.SelectRadiolistValue(Me.p65RecurFlag, BO.BAS.IsNullInt(.p65RecurFlag).ToString)
            Me.p65Name.Text = .p65Name
            Me.p65IsPlanFrom.Checked = .p65IsPlanFrom
            Me.p65IsPlanUntil.Checked = .p65IsPlanUntil

            Me.p65RecurGenToBase_D.Value = .p65RecurGenToBase_D
            Me.p65RecurGenToBase_M.Value = .p65RecurGenToBase_M

            Me.p65RecurPlanFromToBase_D.Value = .p65RecurPlanFromToBase_D
            Me.p65RecurPlanFromToBase_M.Value = .p65RecurPlanFromToBase_M

            Me.p65RecurPlanFromToBase_D.Value = .p65RecurPlanFromToBase_D
            Me.p65RecurPlanFromToBase_D.Value = .p65RecurPlanFromToBase_D

            Me.p65RecurPlanUntilToBase_D.Value = .p65RecurPlanUntilToBase_D
            Me.p65RecurPlanUntilToBase_M.Value = .p65RecurPlanUntilToBase_M

            Master.Timestamp = .Timestamp



            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p65RecurrenceBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p65-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p65RecurrenceBL
            Dim cRec As BO.p65Recurrence = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p65Recurrence)
            cRec.p65Name = Me.p65Name.Text
            cRec.p65RecurFlag = CInt(Me.p65RecurFlag.SelectedValue)
            cRec.p65IsPlanFrom = Me.p65IsPlanFrom.Checked
            cRec.p65IsPlanUntil = Me.p65IsPlanUntil.Checked

            cRec.p65RecurPlanFromToBase_D = BO.BAS.IsNullInt(Me.p65RecurPlanFromToBase_D.Value)
            cRec.p65RecurPlanFromToBase_M = BO.BAS.IsNullInt(Me.p65RecurPlanFromToBase_M.Value)

            cRec.p65RecurGenToBase_D = BO.BAS.IsNullInt(Me.p65RecurGenToBase_D.Value)
            cRec.p65RecurGenToBase_M = BO.BAS.IsNullInt(Me.p65RecurGenToBase_M.Value)

            cRec.p65RecurPlanUntilToBase_D = BO.BAS.IsNullInt(Me.p65RecurPlanUntilToBase_D.Value)
            cRec.p65RecurPlanUntilToBase_M = BO.BAS.IsNullInt(Me.p65RecurPlanUntilToBase_M.Value)

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p65-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p65_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblp65RecurPlanFromToBase_D.Visible = Me.p65IsPlanFrom.Checked
        lblp65RecurPlanFromToBase_M.Visible = Me.p65IsPlanFrom.Checked
        p65RecurPlanFromToBase_D.Visible = Me.p65IsPlanFrom.Checked
        p65RecurPlanFromToBase_M.Visible = Me.p65IsPlanFrom.Checked

        lblp65RecurPlanUntilToBase_D.Visible = Me.p65IsPlanUntil.Checked
        lblp65RecurPlanUntilToBase_M.Visible = Me.p65IsPlanUntil.Checked
        p65RecurPlanUntilToBase_D.Visible = Me.p65IsPlanUntil.Checked
        p65RecurPlanUntilToBase_M.Visible = Me.p65IsPlanUntil.Checked
    End Sub
End Class