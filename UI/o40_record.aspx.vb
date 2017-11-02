Public Class o40_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o40_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                If Request.Item("myaccount") = "1" Then
                    Dim cJ02 As BO.j02Person = .Factory.j02PersonBL.Load(.Factory.SysUser.j02ID)
                    .DataPID = cJ02.o40ID
                    hidMyAccount.Value = "1"
                Else
                    .EnableRecordValidity = True
                    .neededPermission = BO.x53PermValEnum.GR_Admin
                    .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                End If
                
                .HeaderIcon = "Images/settings_32.png"

                .HeaderText = "SMTP účet"


            End With


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.o40Password.Visible = True

            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            o40Password.Visible = True
            cmdTest.Visible = False
            If hidMyAccount.Value = "1" Then
                Me.o40EmailAddress.Text = Master.Factory.SysUser.PersonEmail
                Me.o40Name.Text = Master.Factory.SysUser.Person & " via MARKTIME"
            End If
            Return
        Else
            o40Password.Visible = False
        End If

        Dim cRec As BO.o40SmtpAccount = Master.Factory.o40SmtpAccountBL.Load(Master.DataPID)
        With cRec
            Me.o40Name.Text = .o40Name
            basUI.SelectDropdownlistValue(Me.o40SslModeFlag, CInt(.o40SslModeFlag).ToString)
            basUI.SelectDropdownlistValue(Me.o40SmtpAuthentication, CInt(.o40SmtpAuthentication).ToString)
            Me.o40IsVerify.Checked = .o40IsVerify
            Me.o40login.Text = .o40Login
            Me.o40Password.Text = .o40Password
            Me.o40EmailAddress.Text = .o40EmailAddress
            Me.o40Server.Text = .o40Server
            Me.o40Port.Text = .o40Port

            Master.Timestamp = .Timestamp
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)


        End With


    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o40SmtpAccountBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o40-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o40SmtpAccountBL
            Dim cRec As BO.o40SmtpAccount = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o40SmtpAccount)
            With cRec
                .o40Login = Me.o40login.Text
                .o40Name = Me.o40Name.Text
                .o40EmailAddress = Me.o40EmailAddress.Text
                .o40Server = Me.o40Server.Text
                .o40SslModeFlag = CInt(Me.o40SslModeFlag.SelectedValue)
                .o40IsVerify = Me.o40IsVerify.Checked
                .o40Port = Me.o40Port.Text
                .o40SmtpAuthentication = CInt(Me.o40SmtpAuthentication.SelectedValue)

                If Me.o40Password.Visible Then .o40Password = BO.Crypto.Encrypt(Me.o40Password.Text, "o40SmtpAccount")

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                If hidMyAccount.Value = "1" Then
                    With Master.Factory.j02PersonBL
                        Dim cJ02 As BO.j02Person = .Load(Master.Factory.SysUser.j02ID)
                        cJ02.o40ID = Master.DataPID
                        .Save(cJ02, Nothing)
                    End With


                End If
                Master.CloseAndRefreshParent("o40-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdChangePWD_Click(sender As Object, e As EventArgs) Handles cmdChangePWD.Click
        Me.o40Password.Visible = True
        Me.cmdChangePWD.Visible = False
    End Sub


    Private Sub o40_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblo40Password.Visible = Me.o40Password.Visible
        If Master.DataPID = 0 Then
            cmdChangePWD.Visible = False
        Else
            cmdChangePWD.Visible = True
        End If
        If hidMyAccount.Value = "1" Then Master.HideShowToolbarButton("bin", False)
    End Sub

    Private Sub cmdTest_Click(sender As Object, e As EventArgs) Handles cmdTest.Click
        Dim c As BO.o40SmtpAccount = Master.Factory.o40SmtpAccountBL.Load(Master.DataPID)


        If Master.Factory.x40MailQueueBL.TestConnect(c.o40Server, c.o40Login, c.DecryptedPassword, BO.BAS.IsNullInt(c.o40Port), CType(Me.o40SslModeFlag.SelectedValue, BO.SslModeENUM), c.o40SmtpAuthentication) Then
            Master.Notify("Připojení se podařilo.", NotifyLevel.InfoMessage)
        Else
            Master.Notify(Master.Factory.x40MailQueueBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub
End Class