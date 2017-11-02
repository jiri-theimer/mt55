Public Class p41_batch_childs
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p41_batch_childs_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("p41_batch_childs")
            End With
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderText = .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .DataPID)
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With
            SetupGrid()
            RefreshRecord
        End If
    End Sub
    Private Sub SetupGrid()
        With Me.grid1
            .AddColumn("Client", "Klient projektu", , False, , , , , False)
            .AddColumn("p41TreePath", "Projekt", , False, , , , , False)
        End With


    End Sub

    Private Sub RefreshRecord()
        With Master.Factory.p41ProjectBL
            Dim cRec As BO.p41Project = .Load(Master.DataPID)
            Dim cDisp As BO.p41RecordDisposition = .InhaleRecordDisposition(cRec)
            If Not cDisp.OwnerAccess Then Master.StopPage("Nedisponujete vlastnickým oprávněním k projektu.")
        End With
        
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            With Master.Factory.p41ProjectBL
                If .BatchUpdate_TreeChilds(Master.DataPID, Me.chkRoles.Checked, Me.chkP28ID.Checked, Me.chkP87ID.Checked, Me.chkP51ID.Checked, Me.chkP92ID.Checked, Me.chkJ18ID.Checked, False, Me.chkValidity.Checked) Then
                    Master.CloseAndRefreshParent("p41-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With

        End If
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        With Master.Factory.p41ProjectBL
            Dim cRec As BO.p41Project = .Load(Master.DataPID)
            Dim mq As New BO.myQueryP41
            With mq
                .TreeIndexFrom = cRec.p41TreePrev
                .TreeIndexUntil = cRec.p41TreeNext
                .Closed = BO.BooleanQueryMode.NoQuery
            End With
            grid1.DataSource = .GetList(mq).Where(Function(p) p.PID <> Master.DataPID).OrderBy(Function(p) p.p41TreeIndex)
        End With
        



    End Sub
End Class