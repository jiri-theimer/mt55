Public Class dbupdate
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub dbupdate_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.NoMenu = True

        If Not Page.IsPostBack Then
            Dim cF As New BO.clsFile
            lblLastSpLog.Text = "Naposledy provedená SP aktualizace: " & cF.GetFileContents(Master.Factory.x35GlobalParam.UploadFolder & "\sql_step2_sp.log")
            lblDbVersion.Text = "Tato distribuce obsahuje db verzi: " & cF.GetFileContents(BO.ASS.GetApplicationRootFolder() & "\sys\dbupdate\version.txt")
            lblLastRunDifferenceLog.Text = "Naposledy provedená změna db struktury: " & cF.GetFileContents(Master.Factory.x35GlobalParam.UploadFolder & "\sql_db_difference.log")

            If BO.ASS.GetConfigVal("dbupdate-dbs") <> "" Then
                Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(BO.ASS.GetConfigVal("dbupdate-dbs"), ";")
                If lis.Count > 0 Then
                    panMultiDbs.Visible = True
                    For Each s In lis
                        Me.dbs.Items.Add(s)
                    Next
                End If


            End If
        End If
    End Sub

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click
        Me.lblError.Text = "" : cmdRunResult.Visible = False
        Dim cBL As New BL.SysDbUpdateBL()
        Dim s As String = cBL.FindDifference()
        If cBL.ErrorMessage <> "" Then
            Me.lblError.Text = cBL.ErrorMessage
            Master.Notify(Me.lblError.Text, NotifyLevel.ErrorMessage)
            Return
        End If
        If s = "" Then
            Master.Notify("Není potřeba aktualizovat strukturu databáze.", NotifyLevel.InfoMessage)
        Else
            cmdRunResult.Visible = True
        End If
        Me.txtScript.Text = s
    End Sub

  

    Private Sub cmdSP_Click(sender As Object, e As EventArgs) Handles cmdSP.Click
        Dim cBL As New BL.SysDbUpdateBL()
        If Not cBL.RunSql_step2_sp() Then
            cBL.RunFixingSQLs_BeforeDbUpdate()  'spustit fixing sql dotazy
            cBL.RunFixingSQLs_AfterDbUpdate()  'spustit fixing sql dotazy
            Master.Notify(cBL.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            cBL.RunFixingSQLs_BeforeDbUpdate()  'spustit fixing sql dotazy
            cBL.RunFixingSQLs_AfterDbUpdate()  'spustit fixing sql dotazy
            Master.Notify("Operace dokončena.", NotifyLevel.InfoMessage)

        End If
    End Sub

    Private Sub cmdRunResult_Click(sender As Object, e As EventArgs) Handles cmdRunResult.Click
        Dim s As String = Trim(txtScript.Text)
        If s = "" Then
            SetUpgradeInfoLog()
            Master.Notify("Není co spouštět!", NotifyLevel.WarningMessage)
            Return
        End If
        Dim cBL As New BL.SysDbUpdateBL()
        If Not cBL.RunDbDifferenceResult(s) Then
            Master.Notify(cBL.ErrorMessage, NotifyLevel.ErrorMessage)
        Else
            txtScript.Text = ""
            cmdRunResult.Visible = False


            SetUpgradeInfoLog()


            Master.Notify("Operace dokončena.", NotifyLevel.InfoMessage)

        End If
    End Sub

    Private Sub SetUpgradeInfoLog()
        Dim cF As New BO.clsFile
        Dim strLogFile As String = BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\log_app_update.txt"
        If cF.FileExist(strLogFile) Then
            Dim s As String = cF.GetFileContents(strLogFile, , False, True)
            Dim c As BO.x35GlobalParam = Master.Factory.x35GlobalParam.Load("log_app_update", True)
            If c.x35Value <> s Then
                Master.Factory.j03UserBL.SetAsWaitingOnVisitUpgradeInfo()
            End If
            c.x35Value = s
            Master.Factory.x35GlobalParam.Save(c)
        End If
    End Sub

    Private Sub dbupdate_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If dbs.Items.Count > 0 Then
            cmdCheckDbs.Text = String.Format("Otestovat [{0}]", Me.dbs.SelectedItem.Text)


        End If
    End Sub

    Private Sub cmdCheckDbs_Click(sender As Object, e As EventArgs) Handles cmdCheckDbs.Click
        Dim cBL As New BL.SysDbUpdateBL()
        cBL.ChangeConnectString(String.Format("server=Sql.mycorecloud.net\MARKTIME;database={0};uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;", Me.dbs.SelectedItem.Text))

        Me.lblError.Text = "" : cmdRunResult.Visible = False
        Dim s As String = cBL.FindDifference()
        If cBL.ErrorMessage <> "" Then
            Me.lblError.Text = cBL.ErrorMessage
            Master.Notify(Me.lblError.Text, NotifyLevel.ErrorMessage)
            Return
        End If
        If s = "" Then
            Master.Notify("Není potřeba aktualizovat strukturu databáze.", NotifyLevel.InfoMessage)
        Else
            cmdRunResult.Visible = True
        End If
        Me.txtScript.Text = s
        cmdRunDbs.Visible = True
    End Sub

    Private Sub cmdRunDbs_Click(sender As Object, e As EventArgs) Handles cmdRunDbs.Click
        Dim s As String = Trim(txtScript.Text)
        If s = "" Then
            Me.lblDbsMessage.Text = "Není co spouštět!"
            Return
        End If
        Dim cBL As New BL.SysDbUpdateBL()
        cBL.ChangeConnectString(String.Format("server=Sql.mycorecloud.net\MARKTIME;database={0};uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;", Me.dbs.SelectedItem.Text))

        If Not cBL.RunDbDifferenceResult(s) Then
            Me.lblDbsMessage.Text = "<img src='Images/exclaim.png'/>" & cBL.ErrorMessage
        Else
            txtScript.Text = ""
            

            Me.lblDbsMessage.Text = "Operace dokončena."


        End If
    End Sub

    Private Sub dbs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dbs.SelectedIndexChanged
        cmdRunDbs.Visible = False
        txtScript.Text = ""
    End Sub

    Private Sub cmdRunSpDbs_Click(sender As Object, e As EventArgs) Handles cmdRunSpDbs.Click
        Dim cBL As New BL.SysDbUpdateBL()
        cBL.ChangeConnectString(String.Format("server=Sql.mycorecloud.net\MARKTIME;database={0};uid=MARKTIME;pwd=58PMapN2jhBvdblxqnIB;", Me.dbs.SelectedItem.Text))

        If Not cBL.RunSql_step2_sp() Then
            cBL.RunFixingSQLs_BeforeDbUpdate()  'spustit fixing sql dotazy
            cBL.RunFixingSQLs_AfterDbUpdate()  'spustit fixing sql dotazy
            Me.lblDbsMessage.Text = cBL.ErrorMessage
        Else
            cBL.RunFixingSQLs_BeforeDbUpdate()  'spustit fixing sql dotazy
            cBL.RunFixingSQLs_AfterDbUpdate()  'spustit fixing sql dotazy
            Me.lblDbsMessage.Text = "Operace dokončena."

        End If
    End Sub
End Class