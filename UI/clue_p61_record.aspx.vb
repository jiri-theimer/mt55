Public Class clue_p61_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p61ActivityCluster = Master.Factory.p61ActivityClusterBL.Load(intPID)
        With cRec
            Me.ph1.Text = .p61Name
        End With

        Dim mq As New BO.myQueryP32
        mq.p61ID = intPID
        
        Me.rp1.DataSource = Master.Factory.p32ActivityBL.GetList(mq).OrderBy(Function(p) p.p34Name).ThenBy(Function(p) p.p32Name)
        Me.rp1.DataBind()


    End Sub
End Class