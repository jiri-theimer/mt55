Public Class sys_dbobjects
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.neededPermission = BO.x53PermValEnum.GR_Admin
        If BO.ASS.GetConfigVal("Guru") <> "1" Then
            Master.StopPage("Access denied.")
            Return
        End If
    End Sub

    
    Private Sub cmdGenerate1_Click(sender As Object, e As System.EventArgs) Handles cmdGenerate1.Click
        Dim cBL As New BL.SysObjectBL()
        cBL.GenerateCreateScripts(True)
        Master.Notify("Výstup byl vytvořen.")
    End Sub

    Private Sub cmdGenerate2_Click(sender As Object, e As System.EventArgs) Handles cmdGenerate2.Click
        Dim cBL As New BL.SysObjectBL()
        cBL.GenerateCreateScripts(False)
        Master.Notify("Výstup byl vytvořen do složky: c:\asp2013\marktime50\ui\sys\dbupdate + do INNO SETUP jako sql_step2_sp.sql.")
    End Sub

    Private Sub cmdGenerate3_Click(sender As Object, e As System.EventArgs) Handles cmdGenerate3.Click
        Dim cBL As New BL.SysObjectBL()
        cBL.CompareTables(txtCompareCon.Text)
        Master.Notify("Výstup byl vytvořen.")
    End Sub

    Private Sub cmdDump_Click(sender As Object, e As System.EventArgs) Handles cmdDump.Click
        Dim cBL As New BL.SysObjectBL()

        Me.txtDUMPResult.Text = cBL.GetDump(Me.txtDUMPDef.Text, , , Me.chkIncludeGO.Checked)
    End Sub

   
    Private Sub cmdNoIdentity_Click(sender As Object, e As System.EventArgs) Handles cmdNoIdentity.Click
        Dim cBL As New BL.SysObjectBL()
        txtDUMPResult.Text = String.Join(vbCrLf, cBL.GetList_NoIdentityTables(IIf(chkUseCompareCon.Checked, txtCompareCon.Text, "")))
    End Sub

   
    Private Sub cmdBackup_Click(sender As Object, e As System.EventArgs) Handles cmdBackup.Click
        Dim cBL As New BL.SysObjectBL()
        Dim strRet As String = cBL.CreateDbBackup(txtCompareCon.Text)
        Master.Notify("Operace dokončena, záloha vygenerována do TEMP složky:<br>" & strRet)
    End Sub

   
    Private Sub cmdGenerateXmlDistributionFiles_Click(sender As Object, e As EventArgs) Handles cmdGenerateXmlDistributionFiles.Click
        Dim cBL As New BL.SysObjectBL()
        cBL.GenerateDistributionXmlFiles(Me.txtDatabaseSourceForXmlDistribution.Text)
        Master.Notify("Operace dokončena, XML soubory vygenerovány do složky: c:\asp2013\marktime50\ui\sys\dbupdate")
    End Sub
End Class