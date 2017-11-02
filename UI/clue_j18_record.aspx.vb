Public Class clue_j18_record
    Inherits System.Web.UI.Page
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.j18Region = Master.Factory.j18RegionBL.Load(intPID)
        With cRec
            Me.ph1.Text = .j18Name
            

        End With

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.j18Region, cRec.PID)
        Me.roles_region.RefreshData(lisX69, cRec.PID)


    End Sub
End Class