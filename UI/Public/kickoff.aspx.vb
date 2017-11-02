Public Class kickoff
    Inherits System.Web.UI.Page
    Private _Factory As BL.Factory

    Private Sub kickoff_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not basMemberShip.RecoverySystemAccount() Then
            Me.lblError.Text = basMemberShip.ErrorMessage
        End If
        _Factory = New BL.Factory(, "mtservice")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        

        If Not Page.IsPostBack Then
            Dim lisJ03 As IEnumerable(Of BO.j03User) = _Factory.j03UserBL.GetList(New BO.myQueryJ03).Where(Function(p) p.j03IsSystemAccount = False)
            If lisJ03.Count > 0 Then
                'v systému už jsou založeni uživatelé
                panKickOffReady.Visible = False
                lblHeader.Text = "Databáze není prázdná, nelze pokračovat!"
                lblMessage.Text = String.Format("V systému již jsou založeny uživatelské účty ({0}), viz: {1}", lisJ03.Count, String.Join(",", lisJ03.Select(Function(p) p.j03Login)))
            Else
                panKickOffReady.Visible = True
            End If

            Me.j04ID.DataSource = _Factory.j04UserRoleBL.GetList()
            Me.j04ID.DataBind()
        End If
    End Sub

    

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click
        Dim cJ03 As New BO.j03User
        With cJ03
            .j04ID = BO.BAS.IsNullInt(Me.j04ID.SelectedValue)
            .j03Login = Me.txtLogin.Text
            .j03IsLiveChatSupport = False
            .j03IsSiteMenuOnClick = False
        End With
        Dim cJ02 As New BO.j02Person
        With cJ02
            .j02IsIntraPerson = True
            .j02FirstName = j02FirstName.Text
            .j02LastName = j02Lastname.Text
            .j02Email = j02Email.Text
        End With
        If Trim(Me.txtLogin.Text) = "" Or Trim(Me.j02Email.Text) = "" Or Trim(Me.j02FirstName.Text) = "" Or Trim(Me.j02Lastname.Text) = "" Then
            lblError.Text = "Uživatelské jméno, e-mail adresa, jméno a příjmení jsou povinná pole k vyplnění!"
            Return
        End If
        If Len(Trim(Me.txtPassword.Text)) < 6 Then
            Me.lblError.Text = "Heslo musí obsahovat minimálně 6 znaků."
            Return
        End If
        If Trim(Me.txtPassword.Text) <> Trim(Me.txtVerify.Text) Then
            Me.lblError.Text = "Heslo nesouhlasí s ověřením."
            Return
        End If
        If Not _Factory.j02PersonBL.ValidateBeforeSave(cJ02) Then
            Me.lblError.Text = _Factory.j02PersonBL.ErrorMessage
            Return
        Else
            If _Factory.j03UserBL.Save(cJ03) Then
                Dim intJ03ID As Integer = _Factory.j03UserBL.LastSavedPID
                cJ03 = _Factory.j03UserBL.Load(intJ03ID)
                Dim strMembershipID As String = ""

                If Not basMemberShip.CreateUser(Me.txtLogin.Text, cJ02.j02Email, txtPassword.Text) Then
                    Me.lblError.Text = "Uživatelský účet byl založen, ale při ukládání do membership databáze došlo k chybě:<br>" & basMemberShip.ErrorMessage
                Else
                    strMembershipID = basMemberShip.GetUserID(Me.txtLogin.Text)
                    cJ03.j03MembershipUserId = strMembershipID
                End If

                If _Factory.j02PersonBL.Save(cJ02, Nothing) Then
                    cJ03.j02ID = _Factory.j02PersonBL.LastSavedPID
                Else
                    Me.lblError.Text += "Uživatelský účet byl založen, ale u zakládání osobního profilu došlo k chybě: " & _Factory.j02PersonBL.ErrorMessage
                End If
                _Factory.j03UserBL.Save(cJ03)
                Response.Redirect("kickoff_after1.aspx")
            Else
                Me.lblError.Text = _Factory.j03UserBL.ErrorMessage
            End If
        End If
    End Sub
End Class