Public Class p31_record_router
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_record_router_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim s As String = Request.Item("pids")
            If s = "" Then s = Request.Item("pid")
            If s = "" Then
                Master.StopPage("pid/pids missing")
            End If
            
            Dim a() As String = Split(s, ",")
            If UBound(a) = 0 And BO.BAS.IsNullInt(s) > 0 Then Response.Redirect("p31_record.aspx?pid=" & s)


            Dim mq As New BO.myQueryP31
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
            For i = 0 To UBound(a)
                mq.AddItemToPIDs(CInt(a(i)))
            Next
            rp1.DataSource = Master.Factory.p31WorksheetBL.GetList(mq)
            rp1.DataBind()

        End If
    End Sub

End Class