Public Class mobile_p31_list
    Inherits System.Web.UI.UserControl
    Private _lastP31Date As Date? = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Private Sub rpP31_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP31.ItemDataBound
        Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)
        Dim bolShowTotals As Boolean = False

        With CType(e.Item.FindControl("p31Date"), Label)
            If _lastP31Date Is Nothing Then
                bolShowTotals = True
            Else
                If _lastP31Date = cRec.p31Date Then
                    .Visible = False
                    CType(e.Item.FindControl("trDate"), HtmlTableRow).Style.Item("display") = "none"
                Else
                    bolShowTotals = True
                End If
            End If
            If bolShowTotals Then
                .Text = BO.BAS.FD(cRec.p31Date)
                Dim lis As IEnumerable(Of BO.p31Worksheet) = CType(rpP31.DataSource, IEnumerable(Of BO.p31Worksheet))
                CType(e.Item.FindControl("Pocet"), Label).Text = "(" & lis.Where(Function(p) p.p31Date = cRec.p31Date).Count.ToString & "x)"
                CType(e.Item.FindControl("Hodiny"), Label).Text = BO.BAS.FN(lis.Where(Function(p) p.p31Date = cRec.p31Date).Sum(Function(p) p.p31Hours_Orig)) & " h."
            End If
        End With
        With CType(e.Item.FindControl("cmdEdit"), HyperLink)
            If cRec.p71ID > BO.p71IdENUM.Nic Or cRec.p91ID > 0 Then
                .Visible = False
            Else
                ''.Text = "<img border='0' src='Images/fe.png' />"
                .NavigateUrl = "javascript:hardrefresh('edit'," & cRec.PID.ToString & ")"
            End If
        End With


        With CType(e.Item.FindControl("Project"), Label)
            If cRec.p41NameShort <> "" Then
                .Text = cRec.p41NameShort
            Else
                .Text = cRec.p41Name
            End If
            If cRec.p28ID_Client <> 0 Then
                .Text = BO.BAS.OM3(cRec.ClientName, 15) & " - " & .Text
            End If
        End With
        With CType(e.Item.FindControl("Task"), Label)
            If cRec.p56ID <> 0 Then
                .Text = cRec.p56Name
            Else
                .Visible = False
            End If
        End With
        With CType(e.Item.FindControl("ContactPerson"), Label)
            If cRec.j02ID_ContactPerson <> 0 Then
                .Text = cRec.ContactPerson
            Else
                .Visible = False
            End If
        End With
        With CType(e.Item.FindControl("p32Name"), Label)
            .Text = cRec.p32Name
            If Not cRec.p32IsBillable Then .ForeColor = Drawing.Color.Red
        End With
        With CType(e.Item.FindControl("p31Value_Orig"), Label)
            .Text = BO.BAS.FN(cRec.p31Value_Orig)
            If Not cRec.p32IsBillable Then .ForeColor = Drawing.Color.Red
        End With
        CType(e.Item.FindControl("p31Text"), Label).Text = BO.BAS.CrLfText2Html(cRec.p31Text)

        _lastP31Date = cRec.p31Date
    End Sub

    Public Sub RefreshData(lis As IEnumerable(Of BO.p31Worksheet), strHeader As String)
        lblListP31ListHeader.Text = strHeader
        rpP31.DataSource = lis
        rpP31.DataBind()
    End Sub
End Class