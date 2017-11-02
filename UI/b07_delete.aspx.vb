Public Class b07_delete
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub b07_delete_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid is missing")

                Dim cRec As BO.b07Comment = .Factory.b07CommentBL.Load(.DataPID)
                If Not .Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                    If cRec.j02ID_Owner <> .Factory.SysUser.j02ID Then .StopPage("Musíte být vlastníkem záznamu nebo admin.")
                End If
                Dim mq As New BO.myQueryB07
                mq.b07ID_Parent = .DataPID
                If .Factory.b07CommentBL.GetList(mq).Count > 0 Then
                    .StopPage("Nelze odstranit, protože se k tomuto komentáři odkazují podřízené komentáře.")
                End If

                .AddToolbarButton("Nenávratně odstranit záznam komentáře/přílohy", "ok", , "Images/delete.png")
            End With
            
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Master.Factory.b07CommentBL.Delete(Master.DataPID) Then
                Master.CloseAndRefreshParent("b07-delete")
            Else
                Master.Notify(Master.Factory.b07CommentBL.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End If
    End Sub
End Class