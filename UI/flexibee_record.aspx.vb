Imports Newtonsoft.Json
Imports System.Net.ServicePointManager

Public Class flexibee_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub flexibee_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim strID As String = Request.Item("id")
            ExportRecord(strID)
        End If
    End Sub

    Private Sub ExportRecord(strID As String)
        System.Net.ServicePointManager.ServerCertificateValidationCallback = Function() True

        Dim strURL As String = BO.ASS.GetConfigVal("flexibee_server") & "/faktura-prijata/" & strID & ".json"

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

        Dim reader As New IO.StreamReader(rs.GetResponseStream())

        Dim s As String = reader.ReadToEnd()
        s = Replace(s, "faktura-prijata", "fakturaprijata")
        s = Replace(s, "firma@", "firma")
        s = Replace(s, "mena@", "mena")
        s = Replace(s, "stavUhrK@", "stavUhrK")
        s = Replace(s, "@version", "version")

        Dim result As BO.FlexiBee_Rootobject = JsonConvert.DeserializeObject(Of BO.FlexiBee_Rootobject)(s)
        If result.winstrom.fakturaprijata.Count = 0 Then
            Master.Notify("FlexiBee záznam nebyl nalezen.", NotifyLevel.ErrorMessage)
            Return
        End If
        Dim faktura As BO.FakturaPrijata = result.winstrom.fakturaprijata(0)
        Dim c As New BO.p85TempBox
        c.p85GUID = BO.BAS.GetGUID
        With faktura
            c.p85OtherKey1 = 172                 'p41ID
            c.p85FreeText01 = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, 172, True)
            c.p85OtherKey2 = 83                  'p32ID

            c.p85FreeDate01 = .duzpPuv          'duzp
            c.p85FreeText02 = .kod              'kod dokladu
            c.p85Message = .popis               'text
            c.p85FreeFloat01 = .sumZklCelkem    'bez dph
            c.p85FreeFloat02 = .sumDphCelkem    'částka dph
            c.p85FreeFloat03 = .sumCelkem       'vč. DPH
        End With

        reader.Close()

        If Master.Factory.p85TempBoxBL.Save(c) Then
            Response.Redirect("p31_record.aspx?pid=0&tempsource=" & c.p85GUID, True)
        End If




        ''txt1.Text = "DPH: " & faktura.sumDphCelkem & ", bez DPH: " & faktura.sumZklCelkem & ", celkem: " & faktura.sumCelkem


        'For Each faktura In result.winstrom.fakturaprijata
        '    Me.TextBox1.Text += vbCrLf & "ID: " & faktura.id & ", cena: " & faktura.sumCelkem & ", datum: " & faktura.datVyst & ", popis: " & faktura.popis & ", fima ID: " & faktura.firmainternalId & ", last-changed: " & faktura.lastUpdate
        'Next


    End Sub
End Class