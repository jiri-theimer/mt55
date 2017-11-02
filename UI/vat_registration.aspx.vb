Public Class vat_registration
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim strVAT As String = Replace(Request.Item("vat"), " ", "").Replace(".", "")
            With Master
                .HeaderText = "Ověření subjektu v DPH registrech"
                If strVAT = "" Then
                    .Notify("Na vstupu chybí DIČ.", NotifyLevel.WarningMessage)
                Else
                    Me.txtDIC.Text = strVAT
                    RunVerify()
                End If
            End With


        End If
    End Sub

    Private Sub RunVerify()
        Dim strVAT As String = UCase(Replace(txtDIC.Text, " ", ""))
        If strVAT = "" Then Return
        VIES(strVAT)
        MFREG(strVAT)
    End Sub

    Private Sub VIES(strVAT As String)
        vies_country.Text = Left(strVAT, 2)
        vies_vatnumber.Text = vies_country.Text + " " + Right(strVAT, Len(strVAT) - 2)
        Dim bolValid As Boolean = False, strName As String = "", strAddress = ""
        Try
            Dim c As New VatService.checkVatPortTypeClient
            c.checkVat(Left(strVAT, 2), Right(strVAT, Len(strVAT) - 2), bolValid, strName, strAddress)
            If bolValid Then
                vies_isvalid.Text = "ANO - platné DIČ"
            Else
                vies_isvalid.Text = "NE"
            End If
            vies_name.Text = strName
            vies_address.Text = strAddress.Replace(Chr(10), "<br/>")
            

        Catch ex As Exception
            Me.vies_error.Text = "VIES web service, Error: " & ex.Message
        End Try
    End Sub
    Private Sub MFREG(strVAT As String)
        
        Dim dic(0) As String, res(0) As SpolehlivostDPH.InformaceOPlatciType
        dic(0) = strVAT
        Try
            Dim c As New SpolehlivostDPH.rozhraniCRPDPHClient
            c.getStatusNespolehlivyPlatce(dic, res)
            If res.Count = 0 Then
                mf_error.Text = "Registr nedokázal ověřit DIČ." : Return
            End If
        Catch ex As Exception
            Me.mf_error.Text = "Registr plátců DPH, Chyba: " & ex.Message
            Return
        End Try

        dic_mf.Text = res(0).dic
        fu_mf.Text = res(0).cisloFu
        Select Case res(0).nespolehlivyPlatce
            Case SpolehlivostDPH.NespolehlivyPlatceType.ANO
                nespolehlivyPlatce.Text = "<h3 style='color:red;'>NESPOLEHLIVÝ PLÁTCE</h3>"
            Case SpolehlivostDPH.NespolehlivyPlatceType.NENALEZEN
                nespolehlivyPlatce.Text = "NENALEZEN"
                Return
            Case SpolehlivostDPH.NespolehlivyPlatceType.NE
                nespolehlivyPlatce.Text = "SPOLEHLIVÝ"
        End Select
        If res(0).zverejneneUcty Is Nothing Then Return

        For i As Integer = 0 To res(0).zverejneneUcty.Count - 1
            If TypeOf res(0).zverejneneUcty(i).Item Is SpolehlivostDPH.StandardniUcetType Then
                Dim ucet As SpolehlivostDPH.StandardniUcetType = CType(res(0).zverejneneUcty(i).Item, SpolehlivostDPH.StandardniUcetType)
                bankovni_ucet.Text += ucet.predcisli & " " & ucet.cislo & "/" & ucet.kodBanky & "<br>"
                bankovni_ucet_datum.Text += BO.BAS.FD(res(0).zverejneneUcty(i).datumZverejneni) & "<br>"
            End If

            If TypeOf res(0).zverejneneUcty(i).Item Is SpolehlivostDPH.NestandardniUcetType Then
                Dim ucet As SpolehlivostDPH.NestandardniUcetType = CType(res(0).zverejneneUcty(i).Item, SpolehlivostDPH.NestandardniUcetType)
                bankovni_ucet.Text += ucet.cislo & "<br>"
                bankovni_ucet_datum.Text += BO.BAS.FD(res(0).zverejneneUcty(i).datumZverejneni) & "<br>"
            End If
        Next



    End Sub

    Private Sub cmdVerify_Click(sender As Object, e As EventArgs) Handles cmdVerify.Click
        RunVerify()
    End Sub
End Class