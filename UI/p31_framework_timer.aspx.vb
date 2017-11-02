Public Class p31_framework_timer
    Inherits System.Web.UI.Page
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        timer1.Factory = Master.Factory

    End Sub

End Class