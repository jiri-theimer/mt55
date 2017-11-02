Public Class mobile_start
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
    Private Property _lastP41ID As Integer

    Private Sub mobile_start_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim lisPars As New List(Of String)
            With lisPars
                .Add("mobile_start-scope")
                
            End With
            Master.MenuPrefix = "home"
            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)
                basUI.SelectDropdownlistValue(Me.cbxScope, .GetUserParam("mobile_start-scope", "1"))
            End With
            If Request.Item("w") <> "" And Request.Item("h") <> "" Then
                basUI.Write2AccessLog(Master.Factory, True, Request, Request.Item("w"), Request.Item("h"))
            End If

            RefreshTasks()


        End If
    End Sub
    Private Sub RefreshTasks()
        Dim mq As New BO.myQueryP56
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TopRecordsOnly = 100
        Select Case cbxScope.SelectedValue
            Case "1"
                mq.j02ID = Master.Factory.SysUser.j02ID
            Case "2"
                mq.j02ID_Owner = Master.Factory.SysUser.j02ID
        End Select

        Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq).OrderBy(Function(p) p.p41Name).ThenBy(Function(p) p.p41ID)
        rp1.DataSource = lis
        rp1.DataBind()
        If lis.Count > 0 Then
            panP56.Visible = True
            CountP56.Text = lis.Count.ToString
            If lis.Count = 100 Then
                Me.CountP56.Text = "Podmínce vyhovuje více než 100 úkolů!"
            End If
        Else
            panP56.Visible = False
        End If
    End Sub

    

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
        If cRec.p41ID = _lastP41ID Then
            e.Item.FindControl("trRow").Visible = False
        End If
        _lastP41ID = cRec.p41ID

    End Sub

    Private Sub cbxScope_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxScope.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("mobile_start-scope", Me.cbxScope.SelectedValue)
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("mobile_start.aspx")
    End Sub
End Class