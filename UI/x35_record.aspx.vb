Public Class x35_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub x35_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderText = "Nastavení globálního parametru"
                .HeaderIcon = "Images/settings_32.png"
                ViewState("key") = Request.Item("key")
                If ViewState("key") = "" Then .StopPage("Na vstupu chybí klíč.")
                .AddToolbarButton("OK", "ok", , "Images/ok.png")
            End With

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.x35Value.Visible = False : Me.x35Value_Combo.Visible = False
        Dim cRec As BO.x35GlobalParam = Master.Factory.x35GlobalParam.Load(ViewState("key"))
        If cRec Is Nothing Then
            'položka nenalezena - je třeba založit
            Master.StopPage("Záznam nebyl nalezen.")
        End If
        
        Select Case cRec.x35Key
            Case "j27ID_Domestic"
                With Me.x35Value_Combo
                    .Visible = True
                    .DataTextField = "j27Code"
                    .DataValueField = "pid"
                    .DataSource = Master.Factory.ftBL.GetList_J27(New BO.myQuery)
                    .DataBind()
                    .SelectedValue = cRec.x35Value
                End With
            Case "p32ID_CreditNote"
                With Me.x35Value_Combo
                    .Visible = True
                    .DataTextField = "p32Name"
                    .DataValueField = "pid"
                    Dim mq As New BO.myQueryP32
                    mq.IsMoneyInput = BO.BooleanQueryMode.TrueQuery
                    .DataSource = Master.Factory.p32ActivityBL.GetList(mq)
                    .DataBind()
                    .SelectedValue = cRec.x35Value
                End With
            
            Case "UserAuthenticationMode"
                With Me.x35Value_Combo
                    .RadCombo.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Přihlašování povoleno pouze přes aplikační databázi účtů", "3"))
                    .RadCombo.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Přihlašování povoleno aplikačně i přes Windows doménu", "1"))
                    .RadCombo.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Přihlašování povoleno pouze přes Windows doménu", "2"))
                    .Visible = True
                    .SelectedValue = cRec.x35Value
                End With
            Case Else
                Me.x35Value.Text = cRec.x35Value
                Me.x35Value.Visible = True
        End Select

        Me.x35Description.Text = cRec.x35Description

    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim cRec As BO.x35GlobalParam = Master.Factory.x35GlobalParam.Load(ViewState("key"))

            If Me.x35Value_Combo.Visible Then
                cRec.x35Value = Me.x35Value_Combo.SelectedValue
                ''If cRec.x35Value = "" Then
                ''    Master.Notify("Musíte vybrat hodnotu.", NotifyLevel.ErrorMessage)
                ''    Return
                ''End If
            Else
                cRec.x35Value = Me.x35Value.Text
            End If


            With Master.Factory.x35GlobalParam
                If .Save(cRec) Then
                    Master.DataPID = .LastSavedPID

                    If cRec.x35Key = "j27ID_Domestic" Then
                        Dim cRecClone As BO.x35GlobalParam = Master.Factory.x35GlobalParam.Load("j27ID_Invoice")
                        cRecClone.x35Value = cRec.x35Value
                        Master.Factory.x35GlobalParam.Save(cRecClone)
                    End If
                    Master.CloseAndRefreshParent()
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With
        End If
        
    End Sub
End Class