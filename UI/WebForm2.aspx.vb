Public Class WebForm2
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim x As Double = 0
        x = 2 / 0

        Dim y As Integer = 0
        Dim a1 As Integer = 748444444
        Dim a2 As Integer = 288888899

        y = a1 * a2

    End Sub

End Class