Public Class o51_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o51_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Štítek"
            End With
           

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.o51Tag = Master.Factory.o51TagBL.Load(Master.DataPID)
        With cRec
            Me.o51Name.Text = .o51Name
           
            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)

            If .o51ScopeFlag = 1 Then
                o51ScopeFlag.Checked = True
            Else
                o51ScopeFlag.Checked = False
                Me.o51IsJ02.Checked = .o51IsJ02
                Me.o51IsO23.Checked = .o51IsO23
                Me.o51IsP28.Checked = .o51IsP28
                Me.o51IsP31.Checked = .o51IsP31
                Me.o51IsP41.Checked = .o51IsP41
                Me.o51IsP56.Checked = .o51IsP56
                Me.o51IsP90.Checked = .o51IsP90
                Me.o51IsP91.Checked = .o51IsP91
            End If

            basUI.SetColorToPicker(Me.o51BackColor, .o51BackColor)
            basUI.SetColorToPicker(Me.o51ForeColor, .o51ForeColor)
        End With

    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o51TagBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o51-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        
        With Master.Factory.o51TagBL
            Dim cRec As BO.o51Tag = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o51Tag)
            With cRec
                .o51Name = Me.o51Name.Text

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil

                If o51ScopeFlag.Checked Then
                    .o51ScopeFlag = 1
                Else
                    .o51ScopeFlag = 0
                    .o51IsJ02 = Me.o51IsJ02.Checked
                    .o51IsO23 = Me.o51IsO23.Checked
                    .o51IsP28 = Me.o51IsP28.Checked
                    .o51IsP31 = Me.o51IsP31.Checked
                    .o51IsP41 = Me.o51IsP41.Checked
                    .o51IsP56 = Me.o51IsP56.Checked
                    .o51IsP90 = Me.o51IsP90.Checked
                    .o51IsP91 = Me.o51IsP91.Checked
                End If
                .o51BackColor = basUI.GetColorFromPicker(Me.o51BackColor)
                .o51ForeColor = basUI.GetColorFromPicker(Me.o51ForeColor)
            End With



           
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("o51-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub o51_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        panEntities.Visible = Not Me.o51ScopeFlag.Checked
    End Sub
End Class