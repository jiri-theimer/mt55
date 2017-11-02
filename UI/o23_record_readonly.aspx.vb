Public Class o23_record_readonly1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        rec1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing")
            RefreshRecord()
        End If
    End Sub


    Private Sub RefreshRecord()
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(cRec.x18ID)


        rec1.FillData(cRec, cX18, True)

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.o23Doc, cRec.PID)
        roles1.RefreshData(lisX69, cRec.PID)
        comments1.RefreshData(Master.Factory, BO.x29IdEnum.o23Doc, cRec.PID)
    End Sub
End Class