Public Class kickoff_after2
    Inherits System.Web.UI.Page
    Private _Factory As BL.Factory

    Private Sub kickoff_after2_Init(sender As Object, e As EventArgs) Handles Me.Init
        _Factory = New BL.Factory(, "mtservice")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.AppHost.Text = Context.Request.Url.GetLeftPart(UriPartial.Authority)
            Me.robot_host.Text = Me.AppHost.Text

        End If
    End Sub

    Private Sub kickoff_after2_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

    End Sub

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click
        If Trim(Me.txtUploadFolder.Text) = "" Then
            Me.lblError.Text = "Chybí vyplnit Upload složku." : Return
        End If
        If Not System.IO.Directory.Exists(Me.txtUploadFolder.Text) Then
            lblMessage.Text = String.Format("Složka {0} na serveru neexistuje!", Me.txtUploadFolder.Text)
            Return
        Else
            Dim cF As New BO.clsFile
            If Not cF.SaveText2File(Me.txtUploadFolder.Text & "\test.txt", "Proveden úspěšný test zápisu do UPLOAD složky.", , , False) Then
                lblMessage.Text = cF.ErrorMessage
                Return
            End If
        End If

        With _Factory.x35GlobalParam
            .UpdateValue("Upload_Folder", Me.txtUploadFolder.Text)
            .UpdateValue("AppHost", Me.AppHost.Text)
            .UpdateValue("SMTP_SenderAddress", Me.SMTP_SenderAddress.Text)
            .UpdateValue("robot_host", Me.robot_host.Text)

            'spustit fixační sql skripty
            Dim cBL As New BL.SysDbUpdateBL()
            cBL.RunFixingSQLs_BeforeDbUpdate()  'spustit fixing sql dotazy
            cBL.RunFixingSQLs_AfterDbUpdate()  'spustit fixing sql dotazy

            'přejít do aktualizace tiskových sestav
            Response.Redirect("../sys/dbupdate_reports.aspx")

            ''Response.Redirect("../default.aspx")
        End With
    End Sub
End Class