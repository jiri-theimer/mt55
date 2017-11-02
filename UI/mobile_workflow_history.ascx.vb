Public Class mobile_workflow_history
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(factory As BL.Factory, x29id As BO.x29IdEnum, intRecordPID As Integer, Optional intSelectedB07ID As Integer = 0)
        Dim mq As New BO.myQueryB07
        mq.RecordDataPID = intRecordPID
        mq.x29id = x29id
        Dim lisB07 As IEnumerable(Of BO.b07Comment) = factory.b07CommentBL.GetList(mq)
        rp1.DataSource = lisB07
        rp1.DataBind()

    End Sub
End Class