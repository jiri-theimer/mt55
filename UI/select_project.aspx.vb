Public Class select_project
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub select_project_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("oper") = Request.Item("oper")
            ViewState("ocas") = basUI.GetCompleteQuerystring(Request)
            Select Case ViewState("oper")
                Case "createtask"
                    Me.p41ID.Flag = "createtask"
                Case "createp49"
                    Me.p41ID.Flag = "createp49"
                Case "createo22", "createp64"
                    Me.p41ID.Flag = ""
                Case Else

            End Select
            If Request.Item("masterprefix") = "p41" Then
                If BO.BAS.IsNullInt(Request.Item("masterpid")) > 0 Then
                    p41ID.Value = Request.Item("masterpid")
                    p41ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, BO.BAS.IsNullInt(Request.Item("masterpid")))
                End If
            End If
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                .AddToolbarButton("Pokračovat", "continue", 0, "Images/continue.png")
            End With
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "continue" Then
            Dim intP41ID As Integer = BO.BAS.IsNullInt(Me.p41ID.Value)
            If intP41ID = 0 Then
                Master.Notify("Musíte vybrat projekt!", NotifyLevel.WarningMessage)
                Return
            End If
            Dim strURL As String = ""
            Select Case ViewState("oper")
                Case "createtask"
                    strURL = "p56_record.aspx?p41id=" & intP41ID.ToString
                Case "createp49"
                    strURL = "p49_record.aspx?p41id=" & intP41ID.ToString
                Case "createo22"
                    strURL = "o22_record.aspx?masterprefix=p41&masterpid=" & intP41ID.ToString
                Case "createp64"
                    strURL = "p64_record.aspx?p41id=" & intP41ID.ToString
                Case Else
                    Master.Notify("Neznámá operace: " & ViewState("oper"), NotifyLevel.ErrorMessage)
                    Return
            End Select
            If ViewState("ocas") <> "" Then
                strURL += "&" & ViewState("ocas")
            End If
            Server.Transfer(strURL, False)
        End If
    End Sub
End Class