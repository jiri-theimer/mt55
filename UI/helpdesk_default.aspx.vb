Public Class helpdesk_default
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub helpdesk_default_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            Master.SiteMenuValue = "dashboard"
            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID
            With Master.Factory.j03UserBL
                .InhaleUserParams("helpdesk_default-gridquery")

                basUI.SelectDropdownlistValue(Me.cbxGridQuery, .GetUserParam("helpdesk_default-gridquery"))
            End With



            Dim mq As New BO.myQueryP41
            mq.Closed = BO.BooleanQueryMode.FalseQuery
            mq.j02ID_ContactPerson = Master.Factory.SysUser.j02ID
            Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
            Me.p41ID.DataSource = lis
            Me.p41ID.DataBind()
            Me.p57ID.DataSource = Master.Factory.p57TaskTypeBL.GetList(New BO.myQuery).Where(Function(p) p.p57IsHelpdesk = True)
            Me.p57ID.DataBind()

            RefreshGrid()
        End If
    End Sub

    Private Sub RefreshGrid()
        Dim mq As New BO.myQueryP56
        Select Case Me.cbxGridQuery.SelectedValue
            Case "open"
                mq.Closed = BO.BooleanQueryMode.FalseQuery
            Case "close"
                mq.Closed = BO.BooleanQueryMode.TrueQuery
            Case "all"
                mq.Closed = BO.BooleanQueryMode.NoQuery
        End Select
        mq.j02ID_Owner = Master.Factory.SysUser.j02ID

        Dim lis As IEnumerable(Of BO.p56Task) = Master.Factory.p56TaskBL.GetList(mq).Where(Function(p) p.p57IsHelpdesk = True)
        rpGrid.DataSource = lis
        rpGrid.DataBind()
        lblHeaderMy.Text = BO.BAS.OM2(lblHeaderMy.Text, lis.Count.ToString)
    End Sub

    Private Sub helpdesk_default_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Not Me.p41ID.SelectedItem Is Nothing Then

        End If
    End Sub

    

    Private Sub cmdSaveTicket_Click(sender As Object, e As EventArgs) Handles cmdSaveTicket.Click
        If Me.p57ID.Items.Count = 0 Then
            Master.Notify("V systému chybí nastavit minimálně jeden typ úkolu s povolením zapisovat na helpdesk!", NotifyLevel.WarningMessage)
            Return
        End If
        If Me.p41ID.Items.Count = 0 Then
            Master.Notify("Pro váš osobní profil chybí v systému vazba na minimálně jeden projekt. Bez vazby na projekt nemůžete zapisovat požadavky!", NotifyLevel.WarningMessage)
            Return
        End If
        upload1.TryUploadhWaitingFilesOnClientSide()
        Dim cRec As New BO.p56Task
        With cRec
            .p41ID = BO.BAS.IsNullInt(Me.p41ID.SelectedValue)
            .p57ID = BO.BAS.IsNullInt(Me.p57ID.SelectedValue)
            .p56Description = Me.p56Description.Text
            .p56Name = Me.p57ID.SelectedItem.Text & " (" & Master.Factory.SysUser.Person & ")"
        End With
        If Master.Factory.p56TaskBL.Save(cRec, Nothing, Nothing, upload1.GUID) Then
            ReloadPage()
        Else
            Master.Notify(Master.Factory.p56TaskBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub

    Private Sub cmdCreateTicket_Click(sender As Object, e As EventArgs) Handles cmdCreateTicket.Click
        panTicket.Visible = True
        Me.p41ID.Focus()
    End Sub

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub

    Private Sub cmdClearTicket_Click(sender As Object, e As EventArgs) Handles cmdClearTicket.Click
        panTicket.Visible = False

    End Sub

    Private Sub cbxGridQuery_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGridQuery.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("helpdesk_default-gridquery", Me.cbxGridQuery.SelectedValue)
        RefreshGrid()
    End Sub

    Private Sub rpGrid_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpGrid.ItemDataBound
        Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
        With CType(e.Item.FindControl("link1"), HyperLink)
            .Text = cRec.p56Code
            .NavigateUrl = "helpdesk_ticket.aspx?pid=" & cRec.PID.ToString & "&backpage=" & Server.UrlEncode("helpdesk_default.aspx")
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        With CType(e.Item.FindControl("b02Color"), Label)
            If cRec.b02Color <> "" Then .Style.Item("background-color") = cRec.b02Color
        End With
        With CType(e.Item.FindControl("p57Name"), Label)
            .Text = cRec.p57Name
            .Font.Strikeout = cRec.IsClosed
        End With
        CType(e.Item.FindControl("Project"), Label).Text = cRec.Client & " - " & cRec.p41Name
        With CType(e.Item.FindControl("b02Name"), Label)
            .Text = cRec.b02Name
            .Font.Strikeout = cRec.IsClosed
        End With
        CType(e.Item.FindControl("p56DateInsert"), Label).Text = BO.BAS.FD(cRec.DateInsert, True, True)
        With CType(e.Item.FindControl("cmdWorkflowDialog"), HyperLink)
            .NavigateUrl = "javascript:wd(" & cRec.PID.ToString & ")"
        End With
    End Sub

    Private Sub cmdRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdRefreshOnBehind.Click
        ReloadPage()
    End Sub

    Private Sub ReloadPage()
        Response.Redirect("helpdesk_default.aspx")
    End Sub
End Class