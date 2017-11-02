Public Class j03_messages
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub j03_messages_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .HeaderText = "Upozornění ze systému"

            End With


            RefreshRecord()


        End If
    End Sub

    Private Sub RefreshRecord()
        rpO22.DataSource = Master.Factory.o22MilestoneBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        rpO22.DataBind()
        If rpO22.Items.Count = 0 Then
            boxO22.Visible = False
        Else
            boxO22.Visible = True
        End If

        rpO23.DataSource = Master.Factory.o23DocBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        rpO23.DataBind()
        If rpO23.Items.Count = 0 Then
            boxO23.Visible = False
        Else
            boxO23.Visible = True
        End If

        rpP56.DataSource = Master.Factory.p56TaskBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        rpP56.DataBind()
        If rpP56.Items.Count = 0 Then
            boxP56.Visible = False
        Else
            boxP56.Visible = True
        End If

        rpP39.DataSource = Master.Factory.p40WorkSheet_RecurrenceBL.GetList_forMessagesDashboard(Master.Factory.SysUser.j02ID)
        rpP39.DataBind()
        If rpP39.Items.Count = 0 Then
            boxP39.Visible = False
        Else
            boxP39.Visible = True
        End If
    End Sub

    Private Sub rpO22_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO22.ItemDataBound
        Dim cRec As BO.o22Milestone = CType(e.Item.DataItem, BO.o22Milestone)
        With CType(e.Item.FindControl("cmdO22"), HyperLink)
            .Text = cRec.NameWithDate
            .NavigateUrl = "javascript: o22_detail(" & cRec.PID.ToString & ")"
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        CType(e.Item.FindControl("Owner"), Label).Text = cRec.Owner
        With CType(e.Item.FindControl("Project"), Label)
            Select Case cRec.x29ID
                Case BO.x29IdEnum.p41Project
                    .Text = cRec.Project
                Case BO.x29IdEnum.p28Contact
                    .Text = cRec.Contact

            End Select
        End With
        

    End Sub

    Private Sub rpP56_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP56.ItemDataBound
        Dim cRec As BO.p56Task = CType(e.Item.DataItem, BO.p56Task)
        With CType(e.Item.FindControl("cmdP56"), HyperLink)
            .Text = cRec.NameWithTypeAndCode
            .NavigateUrl = "javascript: p56_detail(" & cRec.PID.ToString & ")"
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        CType(e.Item.FindControl("Owner"), Label).Text = cRec.Owner
        CType(e.Item.FindControl("Project"), Label).Text = cRec.Client & " - " & cRec.p41Name
       
    End Sub

    Private Sub rpP39_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP39.ItemDataBound
        Dim cRec As BO.p39WorkSheet_Recurrence_Plan = CType(e.Item.DataItem, BO.p39WorkSheet_Recurrence_Plan)
        With CType(e.Item.FindControl("cmdProject"), HyperLink)
            .Text = cRec.p41Name
            .NavigateUrl = "javascript: p41_detail(" & cRec.p41ID.ToString & ")"
        End With
        CType(e.Item.FindControl("p39DateCreate"), Label).Text = BO.BAS.FD(cRec.p39DateCreate, True)
        CType(e.Item.FindControl("p39Date"), Label).Text = BO.BAS.FD(cRec.p39Date)
        CType(e.Item.FindControl("p39Text"), Label).Text = cRec.p39Text

    End Sub

    Private Sub rpO23_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO23.ItemDataBound
        Dim cRec As BO.o23Doc = CType(e.Item.DataItem, BO.o23Doc)
        With CType(e.Item.FindControl("cmdO23"), HyperLink)
            .Text = cRec.x23Name & ": " & cRec.o23Name
            .NavigateUrl = "javascript: o23_detail(" & cRec.PID.ToString & ")"
            If cRec.IsClosed Then .Font.Strikeout = True
        End With

        With CType(e.Item.FindControl("Project"), Label)
            .Text = "????"
        End With
    End Sub
End Class