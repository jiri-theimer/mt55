Public Class clue_search
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                If Request.Item("blank") = "1" Then panCommands.Visible = True

                Dim lis As New List(Of String)
                lis.Add("handler_search_project-toprecs")
                lis.Add("handler_search_project-bin")
                lis.Add("handler_search_contact-toprecs")
                lis.Add("handler_search_contact-bin")
                lis.Add("handler_search_person-bin")
                lis.Add("handler_search_invoice-toprecs")
                lis.Add("handler_search_fulltext-main")
                lis.Add("handler_search_fulltext-invoice")
                lis.Add("handler_search_fulltext-task")
                lis.Add("handler_search_fulltext-worksheet")
                lis.Add("handler_search_fulltext-doc")

                With .Factory.j03UserBL
                    .InhaleUserParams(lis)
                    Me.chkP41Bin.Checked = BO.BAS.BG(.GetUserParam("handler_search_project-bin", "0"))
                    Me.chkP28Bin.Checked = BO.BAS.BG(.GetUserParam("handler_search_contact-bin", "0"))
                    Me.chkJ02Bin.Checked = BO.BAS.BG(.GetUserParam("handler_search_person-bin", "0"))
                    basUI.SelectDropdownlistValue(Me.cbxP41Top, .GetUserParam("handler_search_project-toprecs", "20"))
                    basUI.SelectDropdownlistValue(Me.cbxP28Top, .GetUserParam("handler_search_contact-toprecs", "20"))
                    basUI.SelectDropdownlistValue(Me.cbxP91Top, .GetUserParam("handler_search_invoice-toprecs", "20"))

                    Me.chkMain.Checked = BO.BAS.BG(.GetUserParam("handler_search_fulltext-main", "1"))
                    Me.chkInvoice.Checked = BO.BAS.BG(.GetUserParam("handler_search_fulltext-invoice", "1"))
                    Me.chkDocument.Checked = BO.BAS.BG(.GetUserParam("handler_search_fulltext-doc", "0"))
                    Me.chkWorksheet.Checked = BO.BAS.BG(.GetUserParam("handler_search_fulltext-worksheet", "1"))
                    Me.chkTask.Checked = BO.BAS.BG(.GetUserParam("handler_search_fulltext-task", "1"))
                End With
                With .Factory.SysUser
                    trP41.Visible = .j04IsMenu_Project
                    trP28.Visible = .j04IsMenu_Contact
                    trP91.Visible = .j04IsMenu_Invoice
                    trJ02.Visible = .j04IsMenu_People
                End With
                With .Factory
                    Dim bolFulltext As Boolean = .TestPermission(BO.x53PermValEnum.GR_Admin)
                    If Not bolFulltext Then bolFulltext = .TestPermission(BO.x53PermValEnum.GR_P31_Reader)
                    If Not bolFulltext Then bolFulltext = .TestPermission(BO.x53PermValEnum.GR_P41_Reader)
                    panFulltext.Visible = bolFulltext
                   
                End With
            End With

            Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.Factory.SysUser.PID)
            With cRec
                basUI.SelectDropdownlistValue(Me.j03ProjectMaskIndex, .j03ProjectMaskIndex.ToString)
            End With
        End If
        Me.p41id_search.radComboBoxOrig.DropDownWidth = Unit.Parse("570px")
        Me.p28id_search.radComboBoxOrig.DropDownWidth = Unit.Parse("570px")
        Me.j02id_search.RadCombo.DropDownWidth = Unit.Parse("570px")
        Me.p91id_search.RadComboOrig.DropDownWidth = Unit.Parse("570px")
        If trP28.Visible Then Me.p28id_search.radComboBoxOrig.OnClientSelectedIndexChanged = "p28id_search"
        If trJ02.Visible Then j02id_search.RadCombo.OnClientSelectedIndexChanged = "j02id_search"
        If trP91.Visible Then p91id_search.RadComboOrig.OnClientSelectedIndexChanged = "p91id_search"
        fsP41.Visible = trP41.Visible
        fsP28.Visible = trP28.Visible
        fsP91.Visible = trP91.Visible
        fsJ02.Visible = trJ02.Visible
    End Sub

    Private Sub chkP41Bin_CheckedChanged(sender As Object, e As EventArgs) Handles chkP41Bin.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_project-bin", BO.BAS.GB(Me.chkP41Bin.Checked))
        Me.p41id_search.Text = ""
    End Sub

    Private Sub chkP28Bin_CheckedChanged(sender As Object, e As EventArgs) Handles chkP28Bin.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_contact-bin", BO.BAS.GB(Me.chkP28Bin.Checked))
        Me.p28id_search.Text = ""
    End Sub

    Private Sub chkJ02Bin_CheckedChanged(sender As Object, e As EventArgs) Handles chkJ02Bin.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_person-bin", BO.BAS.GB(Me.chkJ02Bin.Checked))
        Me.j02id_search.Text = ""
    End Sub

    Private Sub cbxP28Top_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxP28Top.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_contact-toprecs", Me.cbxP28Top.SelectedValue)
    End Sub

    Private Sub cbxP41Top_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxP41Top.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_project-toprecs", Me.cbxP41Top.SelectedValue)
    End Sub

    Private Sub cbxP91Top_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxP91Top.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_invoice-toprecs", Me.cbxP91Top.SelectedValue)
    End Sub

    Private Sub chkMain_CheckedChanged(sender As Object, e As EventArgs) Handles chkMain.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_fulltext-main", BO.BAS.GB(Me.chkMain.Checked))
    End Sub

    Private Sub chkTask_CheckedChanged(sender As Object, e As EventArgs) Handles chkTask.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_fulltext-task", BO.BAS.GB(Me.chkTask.Checked))
    End Sub

    Private Sub chkInvoice_CheckedChanged(sender As Object, e As EventArgs) Handles chkInvoice.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_fulltext-invoice", BO.BAS.GB(Me.chkInvoice.Checked))
    End Sub

    Private Sub chkWorksheet_CheckedChanged(sender As Object, e As EventArgs) Handles chkWorksheet.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_fulltext-worksheet", BO.BAS.GB(Me.chkWorksheet.Checked))
    End Sub

    Private Sub chkDocument_CheckedChanged(sender As Object, e As EventArgs) Handles chkDocument.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("handler_search_fulltext-document", BO.BAS.GB(Me.chkDocument.Checked))
    End Sub

    Private Sub j03ProjectMaskIndex_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j03ProjectMaskIndex.SelectedIndexChanged
        Dim cJ03 As BO.j03User = Master.Factory.j03UserBL.Load(Master.Factory.SysUser.PID)
        With cJ03
            .j03ProjectMaskIndex = CInt(Me.j03ProjectMaskIndex.SelectedValue)
        End With
        Master.Factory.j03UserBL.Save(cJ03)
    End Sub
End Class