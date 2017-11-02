Public Class contactpersons
    Inherits System.Web.UI.UserControl
    Private Property _ShowAsLink As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Property IsShowClueTip As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsShowClueTip.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsShowClueTip.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Sub FillData(lisJ02 As IEnumerable(Of BO.j02Person), bolShowAsLink As Boolean)
        _ShowAsLink = bolShowAsLink
        rpP30.DataSource = lisJ02
        rpP30.DataBind()

    End Sub

    Private Sub rpP30_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP30.ItemDataBound
        Dim cRec As BO.j02Person = CType(e.Item.DataItem, BO.j02Person)
        With CType(e.Item.FindControl("j02Email"), HyperLink)
            If cRec.j02Email <> "" Then
                .Text = cRec.j02Email
                .NavigateUrl = "mailto:" & cRec.j02Email

            Else
                .Visible = False
            End If
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        With CType(e.Item.FindControl("linkPerson"), HyperLink)
            .Text = cRec.FullNameDesc
            If _ShowAsLink Then
                .NavigateUrl = "j02_framework.aspx?pid=" & cRec.PID.ToString
            Else
                If cRec.j02IsInvoiceEmail Then .ForeColor = Drawing.Color.Green
                .CssClass = "valbold"
            End If

            If cRec.IsClosed Then .Font.Strikeout = True

        End With
        With CType(e.Item.FindControl("j02JobTitle"), Label)
            .Text = cRec.j02JobTitle
        End With
        With CType(e.Item.FindControl("j02Mobile"), Label)
            .Text = cRec.j02Mobile
        End With
        If Me.IsShowClueTip Then
            CType(e.Item.FindControl("linkPP1"), HyperLink).Attributes("onclick") = "RCM('j02'," & cRec.PID.ToString & ",this)"
        Else
            e.Item.FindControl("linkPP1").Visible = False
        End If

    End Sub
End Class