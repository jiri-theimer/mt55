Public Class ChangePasswordSuccess
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub ChangePasswordSuccess_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Master.Factory.SysUser.j03IsMustChangePassword Then
                Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.Factory.SysUser.PID)
                cRec.j03IsMustChangePassword = False
                Master.Factory.j03UserBL.Save(cRec)
                Response.Redirect("default.aspx")
            End If
            If Not Master.Factory.SysUser.j03PasswordExpiration Is Nothing Then
                If Master.Factory.SysUser.j03PasswordExpiration < Now Then
                    Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.Factory.SysUser.PID)
                    cRec.j03PasswordExpiration = DateAdd(DateInterval.Month, 6, Today)
                    Master.Factory.j03UserBL.Save(cRec)
                    Master.Notify(System.String.Format("Nové heslo expiruje za 1/2 roku dne: {0}.", BO.BAS.FD(cRec.j03PasswordExpiration)), NotifyLevel.InfoMessage)
                End If
            End If
        End If
    End Sub

End Class