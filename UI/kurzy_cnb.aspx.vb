Public Class kurzy_cnb
    Inherits System.Web.UI.Page
   
    Public Class Listek
        Public Property Zeme As String
        Public Property Mena As String
        Public Property Mnozstvi As String
        Public Property Kod As String
        Public Property Kurz As String
    End Class
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.Datum.SelectedDate = Today
            Me.txtKody.Text = Master.Factory.j03UserBL.GetUserParam("kurzy_cnb-sels", "EUR,USD,GBP,CAD")
        End If

    End Sub

    Private Sub RefreshRecord()


        Dim client As New System.Net.WebClient
        client.Encoding = UTF8Encoding.UTF8
        Dim strURL As String = "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt"
        If Not Me.Datum.IsEmpty Then
            strURL += "?date=" & Format(Me.Datum.SelectedDate, "dd.MM.yyyy")
        End If
        Dim s As String = client.DownloadString(strURL)

        Dim a() As String = Split(s, Chr(10)), lis As New List(Of Listek)
        Dim sels As List(Of String) = Split(Me.txtKody.Text, ",").ToList
        For i = 2 To UBound(a)
            Dim b() As String = Split(a(i), "|")
            If UBound(b) >= 4 Then
                Dim c As New Listek
                c.Zeme = b(0)
                c.Mena = b(1)
                c.Mnozstvi = b(2)
                c.Kod = b(3)
                c.Kurz = b(4)


                If sels.Where(Function(p) p = c.Kod).Count > 0 Or Trim(Me.txtKody.Text) = "" Then
                    lis.Add(c)
                End If


            End If
        Next

        rp1.DataSource = lis
        rp1.DataBind()

        Me.Hlavicka.Text = a(0)
    End Sub

    Private Sub kurzy_cnb_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        RefreshRecord()
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As ImageClickEventArgs) Handles cmdSave.Click

        Master.Factory.j03UserBL.SetUserParam("kurzy_cnb-sels", Me.txtKody.Text)
    End Sub
End Class