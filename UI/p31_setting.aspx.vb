Public Class p31_setting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_setting_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("p33id") = Request.Item("p33id")
            If ViewState("p33id") = "" Then ViewState("p33id") = "1"
            With Master
                .HeaderText = "Worksheet nastavení | " & .Factory.SysUser.Person
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("OK", "ok", , "Images/ok.png")
                Dim lisPars As New List(Of String)
                lisPars.Add("p31_default_HoursEntryFlag")
                lisPars.Add("p31_HoursInputInterval")
                lisPars.Add("p31_HoursInputFormat")
                lisPars.Add("p31_TimeInputInterval")
                lisPars.Add("p31_TimeInput_Start")
                lisPars.Add("p31_TimeInput_End")
                lisPars.Add("p31_PreFillP32ID")
                .Factory.j03UserBL.InhaleUserParams(lisPars)
            End With

            With Master.Factory.j03UserBL
                Select Case .GetUserParam("p31_default_HoursEntryFlag", "1")
                    Case "1"
                        Me.opgHoursEntryFlag.SelectedValue = "1"    'hodiny
                    Case "2"
                        Me.opgHoursEntryFlag.SelectedValue = "2"    'minuty
                    Case "3"
                        Me.opgHoursEntryFlag.SelectedValue = "1"
                        Me.chkShowTimeInterval.Checked = True
                End Select
                basUI.SelectDropdownlistValue(Me.p31_HoursInputInterval, .GetUserParam("p31_HoursInputInterval", "30"))
                basUI.SelectDropdownlistValue(Me.p31_TimeInputInterval, .GetUserParam("p31_TimeInputInterval", "30"))
                basUI.SelectDropdownlistValue(Me.p31_TimeInput_Start, .GetUserParam("p31_TimeInput_Start", "8"))
                basUI.SelectDropdownlistValue(Me.p31_TimeInput_End, .GetUserParam("p31_TimeInput_End", "19"))
                basUI.SelectDropdownlistValue(Me.p31_HoursInputFormat, .GetUserParam("p31_HoursInputFormat", "dec"))
                Me.p31_PreFillP32ID.Checked = BO.BAS.BG(.GetUserParam("p31_PreFillP32ID", "1"))
            End With

           
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick

        If strButtonValue = "ok" Then
            Dim s As String = Me.opgHoursEntryFlag.SelectedValue
            If s = "1" And Me.chkShowTimeInterval.Checked Then
                s = "3"
            End If
            With Master.Factory.j03UserBL
                .SetUserParam("p31_default_HoursEntryFlag", s)
                .SetUserParam("p31_HoursInputInterval", Me.p31_HoursInputInterval.SelectedValue)
                .SetUserParam("p31_HoursInputFormat", Me.p31_HoursInputFormat.SelectedValue)
                .SetUserParam("p31_TimeInputInterval", Me.p31_TimeInputInterval.SelectedValue)
                .SetUserParam("p31_TimeInput_Start", Me.p31_TimeInput_Start.SelectedValue)
                .SetUserParam("p31_TimeInput_End", Me.p31_TimeInput_End.SelectedValue)
                .SetUserParam("p31_PreFillP32ID", BO.BAS.GB(Me.p31_PreFillP32ID.Checked))
            End With




            Master.CloseAndRefreshParent("p31-setting", Me.opgHoursEntryFlag.SelectedValue)
        End If
    End Sub

    Private Sub p31_setting_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.opgHoursEntryFlag.SelectedValue = "2" Then
            chkShowTimeInterval.Visible = False

        Else
            chkShowTimeInterval.Visible = True
        End If

    End Sub
End Class