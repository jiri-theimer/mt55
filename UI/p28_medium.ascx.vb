Public Class p28_medium
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub FillData(lisO32 As IEnumerable(Of BO.o32Contact_Medium))
        rpO32.DataSource = lisO32
        rpO32.DataBind()

    End Sub

    Private Sub rpO32_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO32.ItemDataBound
        Dim cRec As BO.o32Contact_Medium = CType(e.Item.DataItem, BO.o32Contact_Medium)
        With CType(e.Item.FindControl("aLink"), HyperLink)
            Select Case cRec.o33ID
                Case BO.o33FlagEnum.Email
                    .Text = cRec.o32Value
                    .NavigateUrl = "mailto:" & cRec.o32Value
                Case BO.o33FlagEnum.URL
                    .Text = cRec.o32Value

                    If cRec.o32Value.IndexOf("http:") = -1 And cRec.o32Value.IndexOf("ftp:") = -1 And cRec.o32Value.IndexOf("https:") = -1 And cRec.o32Value.IndexOf("ftps:") = -1 Then
                        .NavigateUrl = "http://" & cRec.o32Value
                    Else
                        .NavigateUrl = cRec.o32Value
                    End If


                    .Target = "_blank"
                Case BO.o33FlagEnum.SKYPE
                    .Text = cRec.o32Value
                    .NavigateUrl = "skype:" & cRec.o32Value
                Case Else
                    .Visible = False
                    CType(e.Item.FindControl("o32Value"), Label).Text = cRec.o32Value
            End Select
        End With
        
    End Sub
End Class