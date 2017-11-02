Public Class p34_after_create
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p34_after_create_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p32_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin

                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    .StopPage("pid is missing.")
                End If
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With

            p42ids.DataSource = Master.Factory.p42ProjectTypeBL.GetList(New BO.myQuery)
            p42ids.DataBind()

            Dim lisX67 As IEnumerable(Of BO.x67EntityRole) = Master.Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p41Project)

            rp1.DataSource = lisX67
            rp1.DataBind()
           
            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(Master.DataPID)
        Master.HeaderText = "Dokončit nastavení sešitu [" & cRec.p34Name & "]"

        Me.p34Name.Text = cRec.p34Name
        Me.p33Name.Text = cRec.p33Name

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.x67EntityRole = CType(e.Item.DataItem, BO.x67EntityRole)


        With cRec
            CType(e.Item.FindControl("_x67name"), Label).Text = cRec.x67Name
           
            CType(e.Item.FindControl("_x67id"), HiddenField).Value = cRec.PID.ToString

        End With
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            For Each intP42ID As Integer In basUI.GetCheckedItems(Me.p42ids)
                With Master.Factory.p42ProjectTypeBL
                    Dim cRec As BO.p42ProjectType = .Load(intP42ID)
                    Dim lisP43 As List(Of BO.p43ProjectType_Workload) = .GetList_p43(intP42ID).ToList
                    Dim cP43 As New BO.p43ProjectType_Workload
                    cP43.p34ID = Master.DataPID
                    lisP43.Add(cP43)
                    .Save(cRec, lisP43)
                End With
            Next
            For Each ri As RepeaterItem In rp1.Items
                With Master.Factory.x67EntityRoleBL
                    Dim cRec As BO.x67EntityRole = .Load(CInt(CType(ri.FindControl("_x67id"), HiddenField).Value))

                    Dim lisO28 As List(Of BO.o28ProjectRole_Workload) = .GetList_o28(BO.BAS.ConvertInt2List(cRec.PID)).ToList
                    Dim cO28 As New BO.o28ProjectRole_Workload
                    cO28.p34ID = Master.DataPID
                    cO28.o28EntryFlag = CInt(CType(ri.FindControl("_o28entryflag"), DropDownList).SelectedValue)
                    cO28.o28PermFlag = CInt(CType(ri.FindControl("_o28permflag"), DropDownList).SelectedValue)
                    cO28.x67ID = cRec.PID
                    lisO28.Add(cO28)
                    .SaveO28(cRec.PID, lisO28)

                    Dim cChild As BO.x67EntityRole = .LoadChild(cRec.PID)
                    If Not cChild Is Nothing Then
                        .SaveO28(cChild.PID, lisO28)    'přidat ještě do projektových skupin (regionu)
                    End If

                End With

            Next
            Master.CloseAndRefreshParent("p34-save")
        End If
    End Sub
End Class