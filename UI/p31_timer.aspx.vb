Public Class p31_timer
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub p31_timer_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        gridP31.Factory = Master.Factory
        Master.HelpTopicID = "p31_timer"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        timer1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            Master.SiteMenuValue = "p31_timer"
            With Master.Factory.j03UserBL
                .InhaleUserParams("p31_timer-grid")
                Me.chkShowP31Grid.Checked = BO.BAS.BG(.GetUserParam("p31_timer-grid", "0"))
            End With
            If Me.chkShowP31Grid.Checked Then
                SetupGrid()
            Else
                gridP31.Visible = False
            End If
        End If

    End Sub
    Private Sub SetupGrid()
        With gridP31
            .Visible = True
            .MasterDataPID = Master.Factory.SysUser.j02ID
            .ExplicitDateFrom = Today
            .ExplicitDateUntil = Today
            .RecalcVirtualRowCount()
            .Rebind(False)
        End With
    End Sub

    Private Sub p31_timer_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblItemsHeader.Text = BO.BAS.OM2(Me.lblItemsHeader.Text, timer1.RowsCount.ToString)
    End Sub

    Private Sub chkShowP31Grid_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowP31Grid.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p31_timer-grid", BO.BAS.GB(Me.chkShowP31Grid.Checked))
        Response.Redirect("p31_timer.aspx")
    End Sub
End Class