Public Class alertbox1
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub alertbox1_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            lblQuestion.Text = Server.HtmlDecode(Request.Item("q"))

            Master.AddToolbarButton("ANO", "ok", , "Images/ok.png", False, Server.HtmlDecode(Request.Item("a")), "_top")

        End If
    End Sub

    
    
End Class