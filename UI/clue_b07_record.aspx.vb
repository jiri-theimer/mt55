Public Class clue_b07_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.b07Comment = Master.Factory.b07CommentBL.Load(Master.DataPID)
        comments1.RefreshOneCommentRecord(Master.Factory, Master.DataPID)

    End Sub
End Class