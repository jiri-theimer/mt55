Public Class membership_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub membership_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("verify") = "0"

        End If
    End Sub


    Private Sub cmdVerify_Click(sender As Object, e As EventArgs) Handles cmdVerify.Click
        lblError.Text = ""
        If Me.txt1.Text = "caligula" & Format(Now, "ddHH") Then
            ViewState("verify") = "1"
            RefreshList()
        Else
            ViewState("verify") = "0"
            lblError.Text = "Heslo není správné."
        End If
        RefreshState()
    End Sub

    Private Sub RefreshList()
        Dim lisJ03 As IEnumerable(Of BO.j03User) = Master.Factory.j03UserBL.GetList(New BO.myQueryJ03).Where(Function(p) p.j03IsSystemAccount = False)
        rp1.DataSource = lisJ03
        rp1.DataBind()
    End Sub

    Private Sub RefreshState()
        If ViewState("verify") = "1" Then
            panUI.Visible = True
        Else
            panUI.Visible = False
        End If
        panVerify.Visible = Not panUI.Visible
    End Sub

    Private Sub membership_recovery_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        RefreshState()
    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(CInt(e.CommandArgument))
        Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(cRec.j02ID)

        Dim strEmail As String = cRec.j03Login & "@marktime.cz", strPWD As String = "A100000XX."
        If Not cJ02 Is Nothing Then
            strEmail = cJ02.j02Email
        End If

        basMemberShip.RecoveryAccount(cRec.j03Login, strEmail, strPWD)
        Dim xx As String = basMemberShip.RecoveryPassword(cRec.j03Login)
        CType(e.Item.FindControl("lblNewPassword"), Label).Text = "Nové heslo: " & xx

        txtReport.Text += vbCrLf & cRec.j03Login & ": " & xx
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.j03User = CType(e.Item.DataItem, BO.j03User)
        With CType(e.Item.FindControl("cmdRecoveryAccount"), Button)
            .CommandArgument = cRec.PID.ToString
        End With
    End Sub
End Class