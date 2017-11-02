Public Class p28_address
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub FillData(lisO37 As IEnumerable(Of BO.o37Contact_Address))
        rpO37.DataSource = lisO37
        rpO37.DataBind()
    End Sub
End Class