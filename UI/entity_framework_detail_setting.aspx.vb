Public Class entity_framework_detail_setting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Public Property CurrentPrefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property

    Private Sub entity_framework_detail_setting_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentPrefix = Request.Item("prefix")
            With Master
                .HeaderText = "Nastavení vzhledu stránky"
                .AddToolbarButton("Uložit změny", "ok")
            End With

            Dim lisPars As New List(Of String)
            With lisPars
                .Add(Me.CurrentPrefix + "_menu-tabskin")
                .Add(Me.CurrentPrefix + "_menu-menuskin")
                .Add(Me.CurrentPrefix + "_menu-x31id-plugin")
                .Add(Me.CurrentPrefix + "_menu-remember-tab")
                .Add(Me.CurrentPrefix + "_menu-show-level1")
                .Add(Me.CurrentPrefix + "_menu-show-cal1")
                ''.Add(Me.CurrentPrefix + "_menu-searchbox")
            End With

            Me.x31ID_Plugin.DataSource = Master.Factory.x31ReportBL.GetList().Where(Function(p) p.x31FormatFlag = BO.x31FormatFlagENUM.ASPX And p.x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentPrefix) And p.x31PluginFlag = BO.x31PluginFlagENUM._AfterEntityMenu)
            Me.x31ID_Plugin.DataBind()
            Me.x31ID_Plugin.Items.Insert(0, "")

            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)

                If .GetUserParam(Me.CurrentPrefix + "_menu-remember-tab", "0") = "1" Then
                    cmdClearLockedTab.Visible = True
                End If
                ''Me.chkShowLevel1.Checked = BO.BAS.BG(.GetUserParam(Me.CurrentPrefix + "_menu-show-level1", "0"))
                Me.chkScheduler.Checked = BO.BAS.BG(.GetUserParam(Me.CurrentPrefix + "_menu-show-cal1", "1"))
                basUI.SelectDropdownlistValue(Me.skin1, .GetUserParam(Me.CurrentPrefix + "_menu-tabskin", "Default"))
                basUI.SelectDropdownlistValue(Me.skin0, .GetUserParam(Me.CurrentPrefix + "_menu-menuskin", "Default"))
                basUI.SelectDropdownlistValue(Me.x31ID_Plugin, .GetUserParam(Me.CurrentPrefix + "_menu-x31id-plugin"))

            End With
            With Master.Factory
                


                colsSource.DataSource = .ftBL.GetList_X61(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix))
                colsSource.DataBind()

                Dim lis As IEnumerable(Of BO.x61PageTab) = .j03UserBL.GetList_PageTabs(.SysUser.PID, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix))
                For Each c In lis
                    Dim it As Telerik.Web.UI.RadListBoxItem = colsSource.FindItem(Function(p) p.Value = c.x61ID.ToString)
                    colsSource.Transfer(it, colsSource, colsDest)
                    colsSource.ClearSelection()
                    colsDest.ClearSelection()
                Next
            End With
            colsSource.ClearSelection()
            If Me.CurrentPrefix = "p91" Then
                Me.cmdClearLockedTab.Visible = False
            End If
            Select Case Me.CurrentPrefix
                Case "p41", "p56", "p91"
                    panTabs.Visible = False 
            End Select
            Select Case Me.CurrentPrefix
                Case "p41", "p28", "j02"
                    panPlugin.Visible = True
                    chkScheduler.Visible = True
                Case Else
                    panPlugin.Visible = False
                    chkScheduler.Visible = False
            End Select

            If Master.Factory.SysUser.j03PageMenuFlag = 0 Then panMenuSkin.Visible = False
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If panTabs.Visible Then
                Dim x61ids As New List(Of Integer)
                For Each it As Telerik.Web.UI.RadListBoxItem In colsDest.Items
                    x61ids.Add(CInt(it.Value))
                Next
                With Master.Factory.j03UserBL
                    If Not .SavePageTabs(Master.Factory.SysUser.PID, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), x61ids) Then
                        Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                        Return
                    End If
                End With
            End If
            With Master.Factory.j03UserBL
                .SetUserParam(Me.CurrentPrefix + "_menu-tabskin", Me.skin1.SelectedValue)
                .SetUserParam(Me.CurrentPrefix + "_menu-menuskin", Me.skin0.SelectedValue)
                .SetUserParam(Me.CurrentPrefix + "_menu-x31id-plugin", Me.x31ID_Plugin.SelectedValue)
                .SetUserParam(Me.CurrentPrefix + "_menu-show-cal1", BO.BAS.GB(Me.chkScheduler.Checked))
            End With
            Master.CloseAndRefreshParent("setting")
        End If

    End Sub

    Private Sub cmdClearLockedTab_Click(sender As Object, e As EventArgs) Handles cmdClearLockedTab.Click
        Master.Factory.j03UserBL.SetUserParam(Me.CurrentPrefix + "_menu-remember-tab", "0")
        Master.Notify("Vyčištěno")
    End Sub

    Private Sub entity_framework_detail_setting_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

    End Sub
End Class