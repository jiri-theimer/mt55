Public Class p91_add_worksheet_gateway
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_add_worksheet_gateway_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_add_worksheet"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    ViewState("p41id") = Request.Item("p41id")
                    ViewState("p28id") = Request.Item("p28id")
                    ViewState("period") = Request.Item("period")
                    panFindInvoice.Visible = True
                    
                Else
                    panPage.Visible = True
                End If
                .HeaderIcon = "Images/worksheet_32.png"

                .AddToolbarButton("Pokračovat", "continue", , "Images/continue.png")


                .HeaderText = "Přidat do faktury další položky | " & .Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, .DataPID)

                Dim mq As New BO.myQueryP31
                mq.p91ID = .DataPID

                Dim lis As IEnumerable(Of BO.p31Worksheet) = .Factory.p31WorksheetBL.GetList(mq)
                If lis.Count > 0 Then
                    Me.p41id.Value = lis(0).p41ID.ToString
                    If lis(0).p28ID_Client = 0 Then
                        Me.p41id.Text = lis(0).p41Name
                    Else
                        Me.p41id.Text = lis(0).ClientName & " - " & lis(0).p41Name
                    End If

                End If
            End With
            RecalcStat()
            SetupP34List()
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If Me.panFindInvoice.Visible Then
            If BO.BAS.IsNullInt(Me.p91ID.SelectedValue) = 0 Then
                Master.Notify("Musíte vybrat fakturu.", NotifyLevel.WarningMessage)
                Return
            Else
                Server.Transfer("p91_add_worksheet.aspx?pid=" & Me.p91ID.SelectedValue & "&p41id=" & ViewState("p41id") & "&p28id=" & ViewState("p28id") & "&period=" & ViewState("period"), False)
            End If
        End If
        Dim intP41ID As Integer = BO.BAS.IsNullInt(Me.p41id.Value)
        If intP41ID = 0 Then
            Master.Notify("Musíte vybrat projekt úkonu.", NotifyLevel.WarningMessage)
            Return
        End If
        If Me.opgCreateOrAppend.SelectedValue = "create" Then
            If Me.p34ID.SelectedItem Is Nothing Then
                Master.Notify("Musíte zvolit sešit úkonu.", NotifyLevel.WarningMessage)
                Return
            End If
            Server.Transfer("p31_record.aspx?p41id=" & intP41ID.ToString & "&p34id=" & Me.p34ID.SelectedValue & "&p91id=" & Master.DataPID.ToString)
        End If
        If Me.opgCreateOrAppend.SelectedValue = "append" Then
            Server.Transfer("p91_add_worksheet.aspx?pid=" & Master.DataPID.ToString & "&p41id=" & intP41ID.ToString & "&period=" & ViewState("period"), False)
        End If
    End Sub

    Private Sub p91_add_worksheet_gateway_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.opgCreateOrAppend.SelectedValue = "create" Then
            panP34.Visible = True
        Else
            panP34.Visible = False
        End If
    End Sub

    Private Sub SetupP34List()

        Dim intP41ID As Integer = BO.BAS.IsNullInt(Me.p41id.Value)
        If intP41ID = 0 Then Return
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(intP41ID)
        
        Me.p34ID.DataSource = Master.Factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(intP41ID, cRec.p42ID, cRec.j18ID, Master.Factory.SysUser.j02ID)
        Me.p34ID.DataBind()
        If Me.p34ID.Items.Count = 0 Then
            Master.Notify("Do projektu [" & Me.p41id.Text & "] pravděpodobně nemáte oprávnění zapisovat worksheet.", NotifyLevel.WarningMessage)
        End If
    End Sub

    Private Sub p41id_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41id.AutoPostBack_SelectedIndexChanged
        SetupP34List()
        RecalcStat()
    End Sub

    Public Sub RecalcStat()
        If BO.BAS.IsNullInt(Me.p41id.Value) = 0 Then
            panStats.Visible = False
            Return
        Else
            panStats.Visible = True
        End If

        Dim mq As New BO.myQueryP31
        mq.p41ID = BO.BAS.IsNullInt(Me.p41id.Value)

        Dim cSum As BO.p31WorksheetSum = Master.Factory.p31WorksheetBL.LoadSumRow(mq, True, True)
        If Not cSum Is Nothing Then
            lblApproved_Hours.Text = BO.BAS.FN(cSum.WaitingOnInvoice_Hours_Sum) & "hod. (" & cSum.WaitingOnInvoice_Hours_Count.ToString & "x)"
            lblApproved_OtherCount.Text = BO.BAS.FNI(cSum.WaitingOnInvoice_Other_Count)
            lblEditing_Hours.Text = BO.BAS.FN(cSum.WaitingOnApproval_Hours_Sum) & "hod. (" & cSum.WaitingOnApproval_Hours_Count.ToString & "x)"
            lblEditing_OtherCount.Text = BO.BAS.FNI(cSum.WaitingOnApproval_Other_Count)

        End If

    End Sub

    Private Sub p28ID_Find_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p28ID_Find.AutoPostBack_SelectedIndexChanged

        Dim mq As New BO.myQueryP91
        mq.p28ID = BO.BAS.IsNullInt(Me.p28ID_Find.Value)
        If mq.p28ID = 0 Then
            Master.Notify("Musíte vybrat klienta.", NotifyLevel.WarningMessage) : Return
        End If
        Me.p91ID.DataSource = Master.Factory.p91InvoiceBL.GetList(mq)
        Me.p91ID.DataBind()
    End Sub
End Class