Public Class mobile_p56_create
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p41id.Value)
        End Get
        Set(value As Integer)
            Me.p41id.Value = value.ToString
            If value = 0 Then
                linkCurProject.Visible = False
            Else
                linkCurProject.Visible = True
            End If
        End Set
    End Property

    Private Sub mobile_p56_create_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        With Me.p41id.radComboBoxOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
        End With
        receiver1.Factory = Master.Factory
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            If Request.Item("source") <> "" Then
                If Not Request.UrlReferrer Is Nothing Then Me.hidRef.Value = Request.UrlReferrer.PathAndQuery
            End If
            Me.p56PlanUntil.SelectedDate = Now.AddDays(2)

            If BO.BAS.IsNullInt(Request.Item("p41id")) <> 0 Then
                Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
                InhaleProject()
            End If
            Me.p57ID.DataSource = Master.Factory.p57TaskTypeBL.GetList(New BO.myQuery)
            Me.p57ID.DataBind()
            Me.x67ID.DataSource = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p56Task)
            Me.x67ID.DataBind()
            If Me.x67ID.Items.Count = 1 Then Me.x67ID.Visible = False

            If Me.p57ID.Items.Count = 1 Then
                panType.Visible = False
            End If
            Me.receiver1.AddReceiver(Master.Factory.SysUser.j02ID, 0, True)
        End If
    End Sub
    Private Sub InhaleProject()
        If Me.CurrentP41ID = 0 Then Return
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
        Me.p41id.Text = cRec.ProjectWithMask(Master.Factory.SysUser.j03ProjectMaskIndex)
    End Sub

    Private Sub mobile_p56_create_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.CurrentP41ID = 0 Then
            panRecord.Visible = False
            Me.lblRecordHeader.Text = "Vyberte projekt"
        Else
            panRecord.Visible = True
            Me.lblRecordHeader.Text = "Vytvořit úkol"
            Me.linkCurProject.Text = "<img src='Images/project.png' /> " & Me.p41id.Text
            Me.linkCurProject.NavigateUrl = "mobile_p41_framework.aspx?pid=" & Me.CurrentP41ID.ToString
        End If
    End Sub

    Private Sub cmdAddReceiver_Click(sender As Object, e As EventArgs) Handles cmdAddReceiver.Click
        Me.receiver1.AddReceiver(0, 0, True)
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        receiver1.SaveTemp()
        With Master.Factory.p56TaskBL
            Dim cRec As New BO.p56Task
            With cRec
                .p41ID = Me.CurrentP41ID
                .p56Name = Me.p56Name.Text

                .p57ID = BO.BAS.IsNullInt(Me.p57ID.SelectedValue)
               
                If Not Me.p56PlanFrom.SelectedDate Is Nothing Then .p56PlanFrom = BO.BAS.IsNullDBDate(Me.p56PlanFrom.SelectedDate)
                .p56PlanUntil = BO.BAS.IsNullDBDate(Me.p56PlanUntil.SelectedDate)
                .p56IsNoNotify = Me.p56IsNoNotify.Checked

                .p56Description = Me.p56Description.Text
                

                .ValidFrom = Now
                .ValidUntil = DateSerial(3000, 1, 1)
            End With

            Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
            For Each r In receiver1.GetList()
                Dim c As New BO.x69EntityRole_Assign
                c.j02ID = r.j02ID
                c.j11ID = r.j11ID
                c.x67ID = BO.BAS.IsNullInt(Me.x67ID.SelectedValue)
                lisX69.Add(c)
            Next
           
            If .Save(cRec, lisX69, Nothing, "") Then
                Master.DataPID = .LastSavedPID
                Redirect2Source()
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Private Sub Redirect2Source()
        If Me.hidRef.Value = "" Then
            Response.Redirect("mobile_p56_create.aspx")
        Else
            Response.Redirect(Me.hidRef.Value)
        End If

    End Sub
    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Redirect2Source()
    End Sub

    Private Sub p41id_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41id.AutoPostBack_SelectedIndexChanged
        InhaleProject()
    End Sub
End Class