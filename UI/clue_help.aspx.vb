Public Class clue_help
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Request.Item("pid") = "" Or Request.Item("prefix") = "" Then
                Master.StopPage("PID or PREFIX is missing.")
            End If
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Select Case Request.Item("prefix")
                Case "x28"
                    Dim c As BO.x28EntityField = Master.Factory.x28EntityFieldBL.Load(Master.DataPID)
                    Me.lblContent.Text = BO.BAS.CrLfText2Html(c.x28HelpText)
            End Select
        End If
    End Sub

End Class