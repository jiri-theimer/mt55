Public Class clue_x67_record
    Inherits System.Web.UI.Page
  

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.x67EntityRole = Master.Factory.x67EntityRoleBL.Load(intPID)
        With cRec
            Me.ph1.Text = .x67Name


        End With
        If BO.BAS.IsNullInt(Request.Item("j02id")) > 0 Then
            Me.Person.Text = Master.Factory.j02PersonBL.Load(BO.BAS.IsNullInt(Request.Item("j02id"))).FullNameDesc
        End If
        If BO.BAS.IsNullInt(Request.Item("j11id")) > 0 Then
            Dim c As BO.j11Team = Master.Factory.j11TeamBL.Load(BO.BAS.IsNullInt(Request.Item("j11id")))
            If Not c.j11IsAllPersons Then
                Me.Team.Text = String.Join("<br>", Master.Factory.j11TeamBL.GetList_BoundJ12(BO.BAS.IsNullInt(Request.Item("j11id"))).Select(Function(p) p.FullNameDesc))
                Me.Team.Text = "<b>" & c.j11Name & "</b>:<br>" & Me.Team.Text
            Else
                Me.Team.Text = c.j11Name
            End If



        End If

        If cRec.x29ID = BO.x29IdEnum.p41Project Or cRec.x29ID = BO.x29IdEnum.j18Region Then
            Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = Master.Factory.x67EntityRoleBL.GetList_o28(BO.BAS.ConvertInt2List(intPID))
            rpO28.DataSource = lisO28
            rpO28.DataBind()
        Else
            Me.panWorksheet.Visible = False
        End If
        


        Dim lisX53 As IEnumerable(Of BO.x53Permission) = Master.Factory.x67EntityRoleBL.GetList_BoundX53(intPID)
        rpX53.DataSource = lisX53
        rpX53.DataBind()


    End Sub

    Private Sub rpO28_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO28.ItemDataBound
        Dim cRec As BO.o28ProjectRole_Workload = CType(e.Item.DataItem, BO.o28ProjectRole_Workload)
        CType(e.Item.FindControl("p34Name"), Label).Text = cRec.p34Name
        If cRec.o28EntryFlag > BO.o28EntryFlagENUM.NemaPravoZapisovatWorksheet Then
            e.Item.FindControl("img1").Visible = True
        Else
            e.Item.FindControl("img1").Visible = False
        End If
        With CType(e.Item.FindControl("PermFlag"), Label)
            Select Case cRec.o28PermFlag
                Case BO.o28PermFlagENUM.PouzeVlastniWorksheet
                    .Text = "Pouze vlastní worksheet"
                Case BO.o28PermFlagENUM.CistVseVProjektu
                    .Text = "V rámci projektu číst úkony všechny lidí"
                Case BO.o28PermFlagENUM.CistASchvalovatVProjektu
                    .Text = "Číst a schvalovat všechny úkony v rámci projektu"
                Case BO.o28PermFlagENUM.CistAEditVProjektu
                    .Text = "Číst a upravovat všechny úkony v rámci projektu"
                Case BO.o28PermFlagENUM.CistAEditASchvalovatVProjektu
                    .Text = "Plný přístup - číst, upravovat a schvalovat všechny úkony v rámci projektu"
            End Select
        End With
        

    End Sub
End Class