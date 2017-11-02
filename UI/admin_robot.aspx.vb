Public Class admin_robot
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub admin_robot_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "admin_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderText = "Nastavení spouštění robota"
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With
            lblHostHelp.Text = "Příklad host url: " & Context.Request.Url.GetLeftPart(UriPartial.Authority)


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        With Master.Factory.x35GlobalParam
            Me.robot_host.Text = .GetValueString("robot_host")
            If Me.robot_host.Text <> "" Then
                Me.chkUseRobot.Checked = True
                basUI.SelectDropdownlistValue(Me.robot_cache_timeout, .GetValueString("robot_cache_timeout", "300"))
            Else
                Me.chkUseRobot.Checked = False
            End If
            

        End With
    End Sub

    Private Sub admin_robot_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panRec.Visible = Me.chkUseRobot.Checked
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            With Master.Factory.x35GlobalParam
                If Me.chkUseRobot.Checked Then
                    If Trim(Me.robot_host.Text) = "" Then
                        Master.Notify("Chybí specifikace host URL.", NotifyLevel.ErrorMessage)
                        Return
                    End If
                Else
                    Me.robot_host.Text = ""
                End If
                Dim cRec As BO.x35GlobalParam = .Load("robot_host")
                cRec.x35Value = Me.robot_host.Text
                .Save(cRec)

                cRec = .Load("robot_cache_timeout")
                cRec.x35Value = Me.robot_cache_timeout.SelectedValue
                .Save(cRec)


            End With
            Master.CloseAndRefreshParent("robot")
        End If
    End Sub

    Private Sub chkUseRobot_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseRobot.CheckedChanged
        If Me.chkUseRobot.Checked And Me.robot_host.Text = "" Then
            Me.robot_host.Text = Context.Request.Url.GetLeftPart(UriPartial.Authority)
        End If
    End Sub
End Class