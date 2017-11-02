Public Class mobile_search
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_search_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.MenuPrefix = "search"
        

        With Me.p41id_search.radComboBoxOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
        End With
        With Me.p28id_search.radComboBoxOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
            .OnClientSelectedIndexChanged = "p28id_search"
        End With
        With Me.p91id_search.RadComboOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
            .OnClientSelectedIndexChanged = "p91id_search"
        End With
        With Me.j02id_search.RadCombo
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
            .OnClientSelectedIndexChanged = "j02id_search"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim lisPars As New List(Of String)
            With lisPars
                ''.Add("handler_search_project-toprecs")
                ''.Add("handler_search_project-bin")
                ''.Add("handler_search_contact-toprecs")
                ''.Add("handler_search_contact-bin")
                ''.Add("handler_search_person-bin")
                ''.Add("handler_search_invoice-toprecs")
                .Add("p41_framework_detail-pid")
                .Add("p28_framework_detail-pid")
                .Add("p91_framework_detail-pid")
                .Add("p56_framework_detail-pid")
                .Add("j02_framework_detail-pid")

            End With
            With Master.Factory.SysUser
                panJ02.Visible = .j04IsMenu_People
                panP91.Visible = .j04IsMenu_Invoice
                panP28.Visible = .j04IsMenu_Contact
            End With

            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)

                Dim intPID As Integer = BO.BAS.IsNullInt(.GetUserParam("p41_framework_detail-pid", "O"))
                If intPID <> 0 Then
                    linkLastProject.Style.Item("display") = "block"
                    linkLastProject.Text = "<img src='Images/project.png' /> " & Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, intPID)
                End If
                If panP28.Visible Then
                    intPID = BO.BAS.IsNullInt(.GetUserParam("p28_framework_detail-pid", "O"))
                    If intPID <> 0 Then
                        linkLastClient.Style.Item("display") = "block"
                        linkLastClient.Text = "<img src='Images/contact.png' /> " & Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, intPID)
                    End If
                End If
                If panP91.Visible Then
                    intPID = BO.BAS.IsNullInt(.GetUserParam("p91_framework_detail-pid", "O"))
                    If intPID <> 0 Then
                        linkLastInvoice.Style.Item("display") = "block"
                        linkLastInvoice.Text = "<img src='Images/invoice.png' /> " & Master.Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, intPID)
                    End If
                End If

            End With
        End If
    End Sub


End Class