Public Class help
    Inherits System.Web.UI.Page
   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        upl1.Factory = Master.Factory

        If Not Page.IsPostBack Then

            If Request.Item("page") <> "" Then
                ViewState("page") = Request.Item("page")
                ViewState("pid") = "0"
                Dim cRec As BO.x50Help = Master.Factory.x50HelpBL.LoadByAspx(Request.Item("page"))
                If Not cRec Is Nothing Then
                    If cRec.x50ExternalURL <> "" Then
                        Me.x50ExternalURL.NavigateUrl = cRec.x50ExternalURL
                    Else
                        Me.x50ExternalURL.Visible = False
                    End If
                    lblFormHeader.Text = cRec.x50Name
                    ViewState("pid") = cRec.PID.ToString
                    place1.Controls.Add(New LiteralControl(cRec.x50Html))

                    Dim mq As New BO.myQueryO27
                    mq.x50ID = cRec.PID
                    upl1.RefreshData(mq)


                Else
                    Me.x50ExternalURL.Visible = False
                    place1.Controls.Add(New LiteralControl("<div>Obsah nápovědy pro tuto oblast není k dispozici.</div><div><img src='Images/Context/directions.jpg'/></div>"))
                End If
            End If

            cmdNew.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
            
        End If
    End Sub

    Private Sub cmdNew_Click(sender As Object, e As EventArgs) Handles cmdNew.Click
        Server.Transfer("x50_record.aspx?pid=" & ViewState("pid") & "&page=" & ViewState("page"))
    End Sub
End Class