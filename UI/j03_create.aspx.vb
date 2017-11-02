Public Class j03_create
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub j03_create_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "j03_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/user_32.png"
                .HeaderText = "Založení uživatelského účtu"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With

            Me.j04id.DataSource = Master.Factory.j04UserRoleBL.GetList(New BO.myQuery)
            Me.j04id.DataBind()
            j07ID.DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
            j07ID.DataBind()
            c21ID.DataSource = Master.Factory.c21FondCalendarBL.GetList(New BO.myQuery)
            c21ID.DataBind()
            Me.j03PasswordExpiration.SelectedDate = DateAdd(DateInterval.Month, 6, Today)

            If Request.Item("clone") = "1" And Request.Item("pid") <> "" Then
                Dim cRec As BO.j03User = Master.Factory.j03UserBL.Load(BO.BAS.IsNullInt(Request.Item("pid")))
                With cRec
                    Me.j03login.Text = .j03Login
                    Me.j04id.SelectedValue = .j04ID.ToString
                End With
                If cRec.j02ID <> 0 Then
                    Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(cRec.j02ID)
                    Me.j07ID.SelectedValue = cJ02.j07ID.ToString
                    Me.c21ID.SelectedValue = cJ02.c21ID.ToString
                    Me.j02Email.Text = cJ02.j02Email
                End If

            End If
            Dim intDefJ02ID As Integer = BO.BAS.IsNullInt(Request.Item("j02id"))
            If intDefJ02ID > 0 Then
                'přednastavit osobu
                Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(intDefJ02ID)
                Me.opgJ02Bind.SelectedValue = "2"
                Me.j02id_search.Value = cJ02.PID.ToString
                Me.j02id_search.Text = cJ02.FullNameDesc
            End If
        End If
    End Sub

    Private Sub j03_create_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panNewJ02.Visible = False : Me.panSearchJ02.Visible = False
        Select Case Me.opgJ02Bind.SelectedValue
            Case "1"
                Me.panNewJ02.Visible = True
            Case "2"
                Me.panSearchJ02.Visible = True
        End Select


    End Sub

    Private Function TestBeforeSave() As Boolean
        If Not basMemberShip.ValidatBeforeCreate(j03login.Text, txtPassword.Text, txtVerify.Text) Then
            Master.Notify(basMemberShip.ErrorMessage, 2)
            Return False
        End If
        Select Case Me.opgJ02Bind.SelectedValue
            Case "1"
                If Trim(Me.j02FirstName.Text) = "" Or Trim(Me.j02LastName.Text) = "" Then
                    Master.Notify("Jméno a příjmení jsou povinná pole osobního profilu!", 2)
                    Return False
                End If

            Case "2"
                If BO.BAS.IsNullInt(j02id_search.Value) = 0 Then
                    Master.Notify("Musíte vybrat osobní profil!", 2)
                    Return False
                End If
        End Select

        Return True
    End Function

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Not TestBeforeSave() Then Return

            Dim cJ03 As New BO.j03User
            With cJ03
                .j04ID = BO.BAS.IsNullInt(Me.j04id.SelectedValue)
                .j03Login = Me.j03login.Text
                .j03IsLiveChatSupport = Me.j03IsLiveChatSupport.Checked
                .j03IsSiteMenuOnClick = False
                .j03IsMustChangePassword = Me.j03IsMustChangePassword.Checked
                If Not Me.j03PasswordExpiration.IsEmpty Then
                    .j03PasswordExpiration = Me.j03PasswordExpiration.SelectedDate
                End If
            End With

            Dim cJ02 As New BO.j02Person
            Select Case Me.opgJ02Bind.SelectedValue
                Case "1"
                    With cJ02
                        .j02IsIntraPerson = True
                        .j02FirstName = j02FirstName.Text
                        .j02LastName = j02LastName.Text
                        .j02TitleBeforeName = j02TitleBeforeName.Text
                        .j02TitleAfterName = j02TitleAfterName.Text
                        .j02Mobile = j02Mobile.Text
                        .j02Phone = j02Phone.Text
                        .j02Email = j02Email.Text
                        .j07ID = BO.BAS.IsNullInt(Me.j07ID.SelectedValue)
                        .c21ID = BO.BAS.IsNullInt(Me.c21ID.SelectedValue)
                    End With
                    If Not Master.Factory.j02PersonBL.ValidateBeforeSave(cJ02) Then
                        Master.Notify(Master.Factory.j02PersonBL.ErrorMessage, 2)
                        Return
                    End If
                Case "2"
                    cJ02 = Master.Factory.j02PersonBL.Load(BO.BAS.IsNullInt(j02id_search.Value))
                    cJ03.j02ID = cJ02.PID
            End Select

            If Master.Factory.j03UserBL.Save(cJ03) Then
                Dim intJ03ID As Integer = Master.Factory.j03UserBL.LastSavedPID
                cJ03 = Master.Factory.j03UserBL.Load(intJ03ID)
                Dim strMembershipID As String = "", strMessage As String = ""

                If Not basMemberShip.CreateUser(Me.j03login.Text, cJ02.j02Email, txtPassword.Text) Then
                    strMessage = "Uživatelský účet byl založen, ale při ukládání do membership databáze došlo k chybě:<br>" & basMemberShip.ErrorMessage
                Else
                    strMembershipID = basMemberShip.GetUserID(Me.j03login.Text)
                    cJ03.j03MembershipUserId = strMembershipID
                End If

                If Me.opgJ02Bind.SelectedValue = "1" Then
                    If Master.Factory.j02PersonBL.Save(cJ02, Nothing) Then
                        cJ03.j02ID = Master.Factory.j02PersonBL.LastSavedPID
                    Else
                        strMessage += "Uživatelský účet byl založen, ale u zakládání osobního profilu došlo k chybě: " & Master.Factory.j02PersonBL.ErrorMessage
                    End If
                End If
                Master.Factory.j03UserBL.Save(cJ03)
                
                If strMessage = "" Then
                    If Request.UrlReferrer.LocalPath.IndexOf("j02_record.aspx") >= 0 Then
                        Master.DataPID = cJ03.j02ID
                        Master.CloseAndRefreshParent("j02-save")
                    Else
                        Master.CloseAndRefreshParent("j03-create")
                    End If

                Else
                    Master.Notify(strMessage, 1)
                End If

            Else
                Master.Notify(Master.Factory.j03UserBL.ErrorMessage, 2)
            End If
        End If
    End Sub
End Class