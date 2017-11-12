Public Class pokus3
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        

        Response.Write("lama@marktime50: " & BO.BAS.ParseDbNameFromCloudLogin("lama@marktime50"))
        Response.Write("<hr>")
        Response.Write("iveta@cleverapp.cz: " & BO.BAS.ParseDbNameFromCloudLogin("iveta@cleverapp.cz"))
        Response.Write("<hr>")
        Response.Write("jiri.theimer@cleverapp.cz: " & BO.BAS.ParseDbNameFromCloudLogin("jiri.theimer@cleverapp.cz"))
        Response.Write("<hr>")
        Response.Write("jiri.theimer@cleverapp: " & BO.BAS.ParseDbNameFromCloudLogin("jiri.theimer@cleverapp"))
        Response.Write("<hr>")
        Response.Write("jiri.theimer: " & BO.BAS.ParseDbNameFromCloudLogin("jiri.theimer"))
        Response.Write("<hr>")
        Response.Write("theimer: " & BO.BAS.ParseDbNameFromCloudLogin("theimer"))
    End Sub

End Class