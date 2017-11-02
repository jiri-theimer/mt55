Public Class ChangePassword
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub ChangePassword_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
   

    Private Sub ChangePassword_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Master.Factory.SysUser.j03IsMustChangePassword Then
                Master.Notify("Ve vašem uživatelském profilu je nastaveno, že si musíte změnit přístupové heslo.", NotifyLevel.InfoMessage)
            End If
            If Not Master.Factory.SysUser.j03PasswordExpiration Is Nothing Then
                If Master.Factory.SysUser.j03PasswordExpiration < Now Then
                    Master.Notify(System.String.Format("Vaše přístupové heslo expirovalo dne {0} a proto si ho musíte okamžitě změnit.", BO.BAS.FD(Master.Factory.SysUser.j03PasswordExpiration)), NotifyLevel.InfoMessage)
                End If
            End If
        End If
    End Sub

    Private Sub ChangeUserPassword_ChangingPassword(sender As Object, e As LoginCancelEventArgs) Handles ChangeUserPassword.ChangingPassword
        With Me.ChangeUserPassword
            If .NewPassword = .CurrentPassword Then
                Master.Notify("Nové heslo se musí lišit od původního.", NotifyLevel.WarningMessage)
                e.Cancel = True
            End If
        End With

    End Sub
End Class