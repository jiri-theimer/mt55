Public Class ErrorPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim strHttpError As String = Request.Item("error")
            Me.lblRefPage.Text = basUI.ParseRefUri(Request.UrlReferrer)
            Try
                Me.lblPage.Text = Request.Url.ToString
            Catch ex1 As Exception
                Me.lblPage.Text = "???"
            End Try



            lblDate.Text = BO.BAS.FD(Now, True, True)
            Dim ex As Exception = Server.GetLastError().GetBaseException()
            If Not ex Is Nothing Then
                lblError.Text = ex.Message
            End If
            Dim usr As MembershipUser = Membership.GetUser()
            If Not usr Is Nothing Then
                lblUser.Text = usr.UserName
            End If

            
            Select Case strHttpError
                Case "400"
                    lblHTTPError.Text = "Bad Request - chybný požadavek – server nerozumí požadavku."
                Case "401"
                    lblHTTPError.Text = "Unauthorized - neautorizovaný přístup – neoprávněný přístup k webové stránce, klient nesplnil identifikační požadavky."
                Case "403"
                    lblHTTPError.Text = "Forbidden - This means the IP address or the username/password entered were not correct and the request was denied as there was no permission to access the data."
                Case "404"
                    lblHTTPError.Text = "Not found – objekt nenalezen – objekt s požadovaným URL neexistuje."
                Case "405"
                    lblHTTPError.Text = "Method Not Allowed – nepovolená metoda – metoda specifikovaná v požadavku není povolena."
                Case "408"
                    lblHTTPError.Text = "Request Timeout – vypršení doby požadavku – potřeba požadavku je delší, než kolik si server připravil na čekání."
                Case "414"
                    lblHTTPError.Text = "Request-url Too Long – URL požadavku je příliš dlouhé – požadavek nebyl akceptován serverem."
                Case "500"
                    lblHTTPError.Text = "Internal server error – vnitřní chyba serveru – při zpracování dotazu došlo v programu serveru k blíže neurčené chybě."
                Case "503"
                    lblHTTPError.Text = "Service unavailable – služba nedostupná – může být způsobeno přetížením serveru. Server momentálně nedokáže vykonat přijatý požadavek (server je přetížen, může probíhat údržba serveru)."
                Case "505"
                    lblHTTPError.Text = "HTTP Version Not Supported – nepodporovaná verze HTTP – server nepodporuje verzi HTTP protokolu."

                Case Else

            End Select

            log4net.LogManager.GetLogger("httperrorlog").Error(lblError.Text & vbCrLf & vbCrLf & strHttpError & ": " & lblHTTPError.Text & vbCrLf & "User: " & lblUser.Text & vbCrLf & "Page: " & lblPage.Text & vbCrLf & "Ref page:" & lblRefPage.Text)

            If strHttpError <> "" Then lblHTTPError.Text = "Http error type " & strHttpError & ": " & lblHTTPError.Text
            'Me.lblError.Text = strErrorMessage
        End If
    End Sub

End Class