Public Class j03_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j03_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "j03_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/user_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Uživatelský účet"
            End With


            RefreshRecord()

            If Master.IsRecordClone Then
                Response.Redirect("j03_create.aspx?clone=1&pid=" & Master.DataPID)
            Else
                If Master.DataPID = 0 Then Response.Redirect("j03_create.aspx")
            End If
            
        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        j04id.DataSource = Master.Factory.j04UserRoleBL.GetList(New BO.myQuery)
        j04id.DataBind()

        Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.DataPID)
        With cRec
            Me.j03login.Text = .j03Login
            Me.j04id.SelectedValue = .j04ID.ToString
            Me.j02ID.Value = .j02ID.ToString
            Me.j02ID.Text = .PersonDesc
            Me.j03IsMustChangePassword.Checked = .j03IsMustChangePassword
            If Not .j03PasswordExpiration Is Nothing Then
                Me.j03PasswordExpiration.SelectedDate = .j03PasswordExpiration
            End If

            Master.Timestamp = .Timestamp


            If .j03IsSystemAccount Then
                Master.StopPage("Tento účet si vyhrazuje systém pro jeho vnitřní potřeby.<hr>Přes systémový účet se nelze uživatelsky přihlašovat do systému.", False)

            Else
                Me.j03IsLiveChatSupport.Checked = .j03IsLiveChatSupport
                Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
                If Not .IsClosed And .PID <> Master.Factory.SysUser.PID Then
                    panSwitchUser.Visible = True
                End If
            End If

        End With

        cmdUnlockMembership.Visible = False
        Dim user As MembershipUser = Membership.GetUser(cRec.j03Login)
        If Not user Is Nothing Then
            If user.IsLockedOut Then
                cmdUnlockMembership.Visible = True
                Master.Notify("Uživatelský účet byl automaticky zablokován po několika-násobném neúspěšném pokusu o přihlášení do systému!", 1)
            End If
        End If
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j03UserBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j03-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.j03UserBL
            Dim cRec As BO.j03User = .Load(Master.DataPID)
            With cRec
                .j04ID = BO.BAS.IsNullInt(j04id.SelectedValue)
                .j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
                .j03IsLiveChatSupport = Me.j03IsLiveChatSupport.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .j03IsMustChangePassword = Me.j03IsMustChangePassword.Checked
                If Me.j03PasswordExpiration.IsEmpty Then
                    .j03PasswordExpiration = Nothing
                Else
                    .j03PasswordExpiration = Me.j03PasswordExpiration.SelectedDate
                End If

            End With

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j03-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j04id_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j04id.NeedMissingItem
        Dim cRec As BO.j04UserRole = Master.Factory.j04UserRoleBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        strAddMissingItemText = cRec.j04Name
    End Sub

    Private Sub cmdRecoveryPassword_Click(sender As Object, e As EventArgs) Handles cmdRecoveryPassword.Click
        Dim strNewPWD As String = basMemberShip.RecoveryPassword(j03login.Text)
        If strNewPWD = "" Then
            Master.Notify(basMemberShip.ErrorMessage, 2)
        Else
            panPasswordRecovery.Visible = True
            txtNewPassword.Text = strNewPWD
        End If
    End Sub

    Private Sub cmdChangeLogin_Click(sender As Object, e As EventArgs) Handles cmdChangeLogin.Click
        If Trim(hidNewLogin.Value).ToLower = j03login.Text.ToLower Then
            Master.Notify("Nové přihlašovací jméno se musí lišit od stávajícího.", 2)
            Return
        End If
        With Master.Factory.j03UserBL
            Dim cRec As BO.j03User = .Load(Master.DataPID)
            If .RenameLogin(cRec, Trim(hidNewLogin.Value).ToLower) Then
                Master.Notify("Přihlašovací jméno bylo změněno.")
                RefreshRecord()
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdUnlockMembership_Click(sender As Object, e As EventArgs) Handles cmdUnlockMembership.Click
        Dim user As MembershipUser = Membership.GetUser(j03login.Text)
        Try
            If user.UnlockUser() Then
                Master.Notify("Účet byl odblokován.")
                cmdUnlockMembership.Visible = False
            End If
        Catch ex As Exception
            Master.Notify(ex.Message, 2)
        End Try
    End Sub

    Private Sub cmdRecoveryMembership_Click(sender As Object, e As EventArgs) Handles cmdRecoveryMembership.Click
        Dim strNewPWD As String = hidNewLogin.Value & "!" & Format(Now, "ddMMyyyyHHmmss")
        Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.DataPID)

        Dim intRet As Integer = basMemberShip.RecoveryAccount(j03login.Text, cRec.PersonEmail, strNewPWD)
        Select Case intRet
            Case 1
                Master.Notify("Účet nebylo nutné opravovat.", 0)
            Case 2
                Master.Notify("Opravený účet musí mít nové přístupové heslo.", 0)
                panPasswordRecovery.Visible = True
                txtNewPassword.Text = strNewPWD
            Case 0
                Master.Notify("Neznámá Membership chyba.", 2)
        End Select
        If intRet > 0 Then
            With Master.Factory.j03UserBL
                cRec = .Load(Master.DataPID)
                cRec.j03MembershipUserId = basMemberShip.GetUserID(cRec.j03Login)
                .Save(cRec)
            End With
        End If
    End Sub

    Private Sub cmdDeleteUserParams_Click(sender As Object, e As EventArgs) Handles cmdDeleteUserParams.Click
        If Master.Factory.j03UserBL.DeleteAllUserParams(Master.DataPID) Then
            Master.Notify("Paměť uživatelského profilu byla vyčištěna.")
        End If
    End Sub

    Private Sub cmdGeneratePasswordAgain_Click(sender As Object, e As EventArgs) Handles cmdGeneratePasswordAgain.Click
        Dim strNewPWD As String = basMemberShip.RecoveryPassword(j03login.Text, Trim(Me.txtNewPassword.Text))
        If strNewPWD = "" Then
            Master.Notify(basMemberShip.ErrorMessage, 2)
        Else
            txtNewPassword.Text = strNewPWD
            Master.Notify("Nové heslo bylo pře-generováno.")
        End If
    End Sub

    Private Sub cmdSwitch2User_Click(sender As Object, e As EventArgs) Handles cmdSwitch2User.Click
        Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(Master.DataPID)
        If cRec.IsClosed Then
            Master.Notify("Uživatelský účet je uzavřený.")
            Return
        End If

        If BO.ASS.GetConfigVal("super-user-pin", "werwerwer") = Me.txtPIN.Text Then
            FormsAuthentication.SetAuthCookie(cRec.j03Login, True)
            ClientScript.RegisterStartupScript(Me.GetType, "hash", "parent.window.open('default.aspx','_top');", True)
        Else
            Master.Notify("Zadaný PIN není správný, kontaktujte MARKTIME podporu.", NotifyLevel.ErrorMessage)
        End If
        
    End Sub
End Class