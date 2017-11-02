Public Class admin_p87
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub admin_p87_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderText = "Nastavení dalších fakturačních jazyků"
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("OK", "ok", , "Images/ok.png")
            End With


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim lis As IEnumerable(Of BO.p87BillingLanguage) = Master.Factory.ftBL.GetList_P87(New BO.myQuery)

        Dim cF As New BO.clsFile

        Dim files As List(Of String) = cF.GetFileListFromDir(BO.ASS.GetApplicationRootFolder() & "\Images\flags", "*.gif")
        For Each c In lis
            CType(panRec.FindControl("p87lang" & c.p87LangIndex.ToString), TextBox).Text = c.p87Name
            CType(panRec.FindControl("p87icon" & c.p87LangIndex.ToString), DropDownList).DataSource = files
            CType(panRec.FindControl("p87icon" & c.p87LangIndex.ToString), DropDownList).DataBind()
            basUI.SelectDropdownlistValue(CType(panRec.FindControl("p87icon" & c.p87LangIndex.ToString), DropDownList), c.p87Icon)
        Next
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim lis As List(Of BO.p87BillingLanguage) = Master.Factory.ftBL.GetList_P87(New BO.myQuery).ToList
            For Each c In lis
                c.p87Name = CType(panRec.FindControl("p87lang" & c.p87LangIndex.ToString), TextBox).Text
                c.p87Icon = CType(panRec.FindControl("p87icon" & c.p87LangIndex.ToString), DropDownList).SelectedValue
            Next

            With Master.Factory.ftBL
                If .SaveP87(lis) Then
                    Master.CloseAndRefreshParent()
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With
        End If
    End Sub

    Private Sub admin_p87_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        For i As Integer = 1 To 4
            If CType(panRec.FindControl("p87icon" & i.ToString), DropDownList).SelectedValue <> "" Then
                CType(panRec.FindControl("img" & i.ToString), Image).Visible = True
                CType(panRec.FindControl("img" & i.ToString), Image).ImageUrl = "Images/flags/" & CType(panRec.FindControl("p87icon" & i.ToString), DropDownList).SelectedValue
            Else
                CType(panRec.FindControl("img" & i.ToString), Image).Visible = False
            End If
        Next
    End Sub
End Class