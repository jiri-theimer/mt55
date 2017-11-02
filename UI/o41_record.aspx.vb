Public Class o41_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o41_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .EnableRecordValidity = True
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "IMAP účet"


            End With


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.o41Password.Visible = True

            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            o41Password.Visible = True
            cmdTest.Visible = False
            Return
        Else
            o41Password.Visible = False
        End If

        Dim cRec As BO.o41InboxAccount = Master.Factory.o41InboxAccountBL.Load(Master.DataPID)
        With cRec
            Me.o41Name.Text = .o41Name
            basUI.SelectDropdownlistValue(Me.o41SslModeFlag, CInt(.o41SslModeFlag).ToString)
            Me.o41IsDeleteMesageAfterImport.Checked = .o41IsDeleteMesageAfterImport
            Me.o41login.Text = .o41Login
            Me.o41Password.Text = .o41Password
            Me.o41Folder.Text = .o41Folder
            Me.o41Server.Text = .o41Server
            Me.o41Port.Text = .o41Port

            basUI.SelectDropdownlistValue(Me.o41ForwardFlag_New, CInt(.o41ForwardFlag_New).ToString)
            Me.o41ForwardEmail_New.Text = .o41ForwardEmail_New
            basUI.SelectDropdownlistValue(Me.o41ForwardFlag_Answer, CInt(.o41ForwardFlag_Answer).ToString)
            Me.o41ForwardEmail_Answer.Text = .o41ForwardEmail_Answer
            Me.o41ForwardEmail_UnBound.Text = .o41ForwardEmail_UnBound


            Master.Timestamp = .Timestamp
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)


        End With


    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o41InboxAccountBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o41-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o41InboxAccountBL
            Dim cRec As BO.o41InboxAccount = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o41InboxAccount)
            With cRec
                .o41Login = Me.o41login.Text
                .o41Name = Me.o41Name.Text
                .o41Folder = Me.o41Folder.Text
                .o41Server = Me.o41Server.Text
                .o41SslModeFlag = CInt(Me.o41SslModeFlag.SelectedValue)
                .o41IsDeleteMesageAfterImport = Me.o41IsDeleteMesageAfterImport.Checked
                .o41Port = Me.o41Port.Text
                .o41ForwardEmail_UnBound = Me.o41ForwardEmail_UnBound.Text
                .o41ForwardFlag_New = CType(CInt(Me.o41ForwardFlag_New.SelectedValue), BO.o41ForwardENUM)
                .o41ForwardFlag_Answer = CType(CInt(Me.o41ForwardFlag_Answer.SelectedValue), BO.o41ForwardENUM)
                .o41ForwardEmail_New = Me.o41ForwardEmail_New.Text
                .o41ForwardEmail_Answer = Me.o41ForwardEmail_Answer.Text

                If Me.o41Password.Visible Then .o41Password = BO.Crypto.Encrypt(Me.o41Password.Text, "o41InboxAccount")

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("o41-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdChangePWD_Click(sender As Object, e As EventArgs) Handles cmdChangePWD.Click
        Me.o41Password.Visible = True
    End Sub


    Private Sub o41_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        lblo41Password.Visible = Me.o41Password.Visible
        If Master.DataPID = 0 Then
            cmdChangePWD.Visible = False
        Else
            cmdChangePWD.Visible = True
        End If
        If Me.o41ForwardFlag_New.SelectedValue = "2" Then
            o41ForwardEmail_New.Visible = True
        Else
            o41ForwardEmail_New.Visible = False
            o41ForwardEmail_New.Text = ""
        End If
        If Me.o41ForwardFlag_Answer.SelectedValue = "2" Then
            Me.o41ForwardEmail_Answer.Visible = True
        Else
            Me.o41ForwardEmail_Answer.Visible = False
            o41ForwardEmail_Answer.Text = ""
        End If
    End Sub

    Private Sub cmdTest_Click(sender As Object, e As EventArgs) Handles cmdTest.Click
        Dim c As BO.o41InboxAccount = Master.Factory.o41InboxAccountBL.Load(Master.DataPID)

        If Master.Factory.o42ImapRuleBL.Connect(c) Then
            Master.Notify("Připojení se podařilo.", NotifyLevel.InfoMessage)
        Else
            Master.Notify(Master.Factory.o42ImapRuleBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub


End Class