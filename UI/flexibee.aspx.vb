Imports Newtonsoft.Json
Imports System.Net.ServicePointManager

Public Class flexibee
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private _lisP31 As IEnumerable(Of BO.p31Worksheet)
    Private Sub flexibee_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                If BO.ASS.GetConfigVal("flexibee_server") = "" Then
                    .StopPage("Chybí nastavení pro komunikaci s FlexiBee API.")
                End If
            End With

        End If
    End Sub

    Private Sub RefreshFaktury()
        System.Net.ServicePointManager.ServerCertificateValidationCallback = Function() True

        Dim strURL As String = BO.ASS.GetConfigVal("flexibee_server") & "/faktura-prijata.json"
        strURL += "?limit=" & cbxTopRecs.SelectedValue & "&use-internal-id=true&order=id@D"
        ''strURL += "&detail=full"

        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(strURL)
        request.ContentType = "application/json"
        request.Credentials = New System.Net.NetworkCredential(BO.ASS.GetConfigVal("flexibee_login"), BO.ASS.GetConfigVal("flexibee_pwd"))

        Dim rs As System.Net.HttpWebResponse = Nothing

        Try
            rs = request.GetResponse()
        Catch ex As Net.WebException
            If ex.Status = Net.WebExceptionStatus.TrustFailure Then

            End If
            Master.Notify(ex.Message, NotifyLevel.ErrorMessage)
            rs.Close()
            Return
        End Try

        Dim mq As New BO.myQueryP31
        mq.QuickQuery = BO.myQueryP31_QuickQuery.Is_p31Code
        _lisP31 = Master.Factory.p31WorksheetBL.GetList(mq)

        Dim reader As New IO.StreamReader(rs.GetResponseStream())

        Dim s As String = reader.ReadToEnd()
        s = Replace(s, "faktura-prijata", "fakturaprijata")
        s = Replace(s, "firma@", "firma")
        s = Replace(s, "mena@", "mena")
        s = Replace(s, "stavUhrK@", "stavUhrK")
        s = Replace(s, "@version", "version")

        Dim result As BO.FlexiBee_Rootobject = JsonConvert.DeserializeObject(Of BO.FlexiBee_Rootobject)(s)


        rp1.DataSource = result.winstrom.fakturaprijata
        rp1.DataBind()

        'For Each faktura In result.winstrom.fakturaprijata
        '    Me.TextBox1.Text += vbCrLf & "ID: " & faktura.id & ", cena: " & faktura.sumCelkem & ", datum: " & faktura.datVyst & ", popis: " & faktura.popis & ", fima ID: " & faktura.firmainternalId & ", last-changed: " & faktura.lastUpdate
        'Next

        reader.Close()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim c As BO.FakturaPrijata = CType(e.Item.DataItem, BO.FakturaPrijata)
        CType(e.Item.FindControl("hidID"), HiddenField).Value = c.id

        CType(e.Item.FindControl("kod"), Label).Text = c.kod
        CType(e.Item.FindControl("popis"), Label).Text = c.popis
        CType(e.Item.FindControl("firma"), Label).Text = Replace(c.firma, "code:", "")

        CType(e.Item.FindControl("datVyst"), Label).Text = BO.BAS.FD(c.datVyst)
        CType(e.Item.FindControl("datSplat"), Label).Text = BO.BAS.FD(c.datSplat)
        CType(e.Item.FindControl("lastUpdate"), Label).Text = BO.BAS.FD(c.lastUpdate, True, True)
        CType(e.Item.FindControl("sumCelkem"), Label).Text = BO.BAS.FN(c.sumCelkem) & " " & Replace(c.mena, "code:", "")

        With CType(e.Item.FindControl("linkExport"), HyperLink)
            .NavigateUrl = "javascript:export_record(" & c.id & ")"
        End With
        With CType(e.Item.FindControl("linkP31"), HyperLink)
            If _lisP31.Where(Function(p) p.p31Code = c.kod).Count > 0 Then
                .NavigateUrl = "javascript:contMenu('p31_record.aspx?pid=" & _lisP31.Where(Function(p) p.p31Code = c.kod)(0).PID.ToString & "',false)"
            Else
                .Visible = False
            End If

        End With
    End Sub

    Private Sub cmdLoadFaktury_Click(sender As Object, e As EventArgs) Handles cmdLoadFaktury.Click
        RefreshFaktury()
    End Sub
End Class