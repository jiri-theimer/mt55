Imports System.Configuration
Imports System.Web.Configuration
Imports System.Net.Configuration

Public Class admin_smtp
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub admin_smtp_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderText = "Výchozí aplikační poštovní účet"
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")

                Me.cbxO40ID.DataSource = .Factory.o40SmtpAccountBL.GetList(New BO.myQuery)
                Me.cbxO40ID.DataBind()
                Me.cbxO40ID.Items.Insert(0, "")
            End With

            RefreshRecord()
        End If
    End Sub
    Private Sub RefreshRecord()
        Dim config As Configuration = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath)
        Dim settings As MailSettingsSectionGroup = DirectCast(config.GetSectionGroup("system.net/mailSettings"), MailSettingsSectionGroup)
        If Not settings Is Nothing Then
            With settings.Smtp.Network
                default_server.Text = .Host
            End With
            With settings.Smtp
                default_sender.Text = .From
            End With
        End If


        With Master.Factory.x35GlobalParam
            Me.AppHost.Text = .GetValueString("AppHost")
            If Me.AppHost.Text = "" Then
                Me.AppHost.Text = Context.Request.Url.GetLeftPart(UriPartial.Authority)
            End If
            Dim bolUseWC As Boolean = BO.BAS.BG(.GetValueString("IsUseWebConfigSetting", "1"))
            Me.SMTP_SenderAddress.Text = .GetValueString("SMTP_SenderAddress")
            If bolUseWC Then
                opgSMTP_UseWebConfigSetting.SelectedValue = "1"
            Else
                opgSMTP_UseWebConfigSetting.SelectedValue = "0"
                basUI.SelectDropdownlistValue(Me.cbxO40ID, .GetValueString("SMTP_Server"))
            End If
         
            

        End With

    End Sub

    Private Sub admin_smtp_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panRec.Visible = Not BO.BAS.BG(Me.opgSMTP_UseWebConfigSetting.SelectedValue)
        Me.panWebConfig.Visible = BO.BAS.BG(Me.opgSMTP_UseWebConfigSetting.SelectedValue)
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            With Master.Factory.x35GlobalParam
                If opgSMTP_UseWebConfigSetting.SelectedValue = "0" Then
                    'zkontrolovat vlastní nastavení
                    If Me.cbxO40ID.SelectedValue = "" Then
                        Master.Notify("Musíte vybrat jeden ze zavedených SMTP účtů.", NotifyLevel.ErrorMessage)
                        Return
                    End If
                Else
                    cbxO40ID.SelectedValue = ""
                End If
                If Trim(Me.SMTP_SenderAddress.Text) = "" Then
                    Master.Notify("Chybí adresa odesílatele.", NotifyLevel.WarningMessage)
                    Return
                End If

                Dim cRec As BO.x35GlobalParam = .Load("IsUseWebConfigSetting")
                cRec.x35Value = Me.opgSMTP_UseWebConfigSetting.SelectedValue
                .Save(cRec)



                cRec = .Load("SMTP_SenderAddress")
                cRec.x35Value = Me.SMTP_SenderAddress.Text
                .Save(cRec)

                cRec = .Load("AppHost")
                cRec.x35Value = Me.AppHost.Text
                .Save(cRec)

                cRec = .Load("SMTP_Server")
                If opgSMTP_UseWebConfigSetting.SelectedValue = "0" Then
                    cRec.x35Value = Me.cbxO40ID.SelectedValue
                Else
                    cRec.x35Value = ""
                End If
                .Save(cRec)

                Master.Factory.o40SmtpAccountBL.SetGlobalDefaultSmtpAccount(BO.BAS.IsNullInt(Me.cbxO40ID.SelectedValue))

              


            End With
            Master.CloseAndRefreshParent("smtp")
        End If
    End Sub
End Class