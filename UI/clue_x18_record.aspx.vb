Public Class clue_x18_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
        End If
    End Sub

    Private Sub RefreshRecord(intPID As Integer)
        If intPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(intPID)
        With cRec
            Me.ph1.Text = .x18Name
            
            With Me.x18AllowedFileExtensions
                .Text = cRec.x18AllowedFileExtensions
                If .Text = "" Then .Text = "[bez omezení]"
            End With

            If .x18MaxOneFileSize > 0 Then
                Me.x18MaxOneFileSize.Text = BO.BAS.FormatFileSize(.x18MaxOneFileSize)
            Else
                Me.x18MaxOneFileSize.Text = "[maximum, co povolí server]"
            End If


            
        End With



    End Sub
End Class