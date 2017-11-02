Public Class ModalForm
    Inherits System.Web.UI.MasterPage

    Public Property ErrorMessage As String = ""
    Public Property HelpTopicID As String = ""
    Private Property _Factory As BL.Factory = Nothing
    Public Property neededPermission As BO.x53PermValEnum = Nothing
    Public Property neededPermissionIfSecond As BO.x53PermValEnum = Nothing

    Public Event Master_OnToolbarClick(ByVal strButtonValue As String)

    Public ReadOnly Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
    End Property
    Public ReadOnly Property RadToolbar As Telerik.Web.UI.RadToolBar
        Get
            Return Me.toolbar1
        End Get
    End Property

    Public Property DataPID() As Integer
        Get
            Return BO.BAS.IsNullInt(hidDataPID.Value)
        End Get
        Set(ByVal value As Integer)
            hidDataPID.Value = value.ToString
        End Set
    End Property

    Public Property HeaderText() As String
        Get
            Return hidPageTitle.Value
        End Get
        Set(ByVal value As String)
            hidPageTitle.Value = value
            pageTitle.Text = value
        End Set
    End Property
    Public Property HeaderIcon() As String
        Get
            Return icon1.ImageUrl
        End Get
        Set(ByVal value As String)
            If value <> "" Then
                icon1.Visible = True : icon1.ImageUrl = value
            Else
                icon1.Visible = False
            End If
        End Set
    End Property

    

    Public Sub StopPage(ByVal strMessage As String, Optional ByVal bolErrorInfo As Boolean = True, Optional ByVal strNeededPerms As String = "")
        Server.Transfer("~/stoppage.aspx?modal=1&err=" & BO.BAS.GB(bolErrorInfo) & "&message=" & Server.UrlEncode(strMessage) & "&neededperms=" & strNeededPerms, False)
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.User.Identity.IsAuthenticated And _Factory Is Nothing Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            _Factory = New BL.Factory(, strLogin)
            If _Factory.SysUser Is Nothing Then DoLogOut()
            basUI.PingAccessLog(_Factory, Request)

            'PersonalizeMenu()
        End If

        
    End Sub
    Private Sub DoLogOut()
        Response.Redirect("~/Account/Login.aspx?autologout=1") 'automatické odhlášení
    End Sub


    Public Sub Notify(ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        basUI.NotifyMessage(Me.notify1, strText, msgLevel)
    End Sub
    Public Sub CloseAndRefreshParent(Optional ByVal strFlag As String = "refresh", Optional strPar1 As String = "", Optional strJsFunction As String = "hardrefresh")
        hidForceOperation.Value = "closeandrefresh"
        hidCloseAndRefreshParent_Flag.Value = strFlag
        hidCloseAndRefreshParent_Par1.Value = strPar1
        hidCloseAndRefreshParent_JsFunction.Value = strJsFunction
        If Me.hidHRJS.Value <> "" Then
            hidCloseAndRefreshParent_JsFunction.Value = Me.hidHRJS.Value
        End If
    End Sub
   

    Private Sub toolbar1_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles toolbar1.ButtonClick
        RaiseEvent Master_OnToolbarClick(e.Item.Value)
    End Sub
    Public Overloads Sub AddToolbarButton(c As clsToolBarButton)
        basUI.AddToolbarButton(Me.toolbar1, c)
    End Sub
    Public Overloads Sub AddToolbarButton(ByVal strText As String, ByVal strValue As String, Optional ByVal Index As Integer = 0, Optional ByVal strImageURL As String = "", Optional ByVal bolPostBack As Boolean = True, Optional ByVal strNavigateURL As String = "", Optional strTarget As String = "", Optional bolShowLoading As Boolean = False)
        basUI.AddToolbarButton(Me.toolbar1, strText, strValue, Index, strImageURL, bolPostBack, strNavigateURL, strTarget, bolShowLoading)
    End Sub

    Public Sub RenameToolbarButton(ByVal strButtonValue As String, ByVal strNewText As String)
        basUI.RenameToolbarButton(Me.toolbar1, strButtonValue, strNewText)
    End Sub
    Public Sub ChangeToolbarButtonAttribute(strButtonValue As String, strAttribute As String, strValute As String)
        basUI.ChangeToolbarButtonAttribute(Me.toolbar1, strButtonValue, strAttribute, strButtonValue)
    End Sub
    Public Sub HideShowToolbarButton(ByVal strButtonValue As String, ByVal bolVisible As Boolean)
        basUI.HideShowToolbarButton(Me.toolbar1, strButtonValue, bolVisible)
    End Sub

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Me.neededPermission > 0 And Me.neededPermissionIfSecond > 0 Then
            TestNeededPermission(Me.neededPermission, Me.neededPermissionIfSecond)
        Else
            If Me.neededPermission > 0 Then
                TestNeededPermission(Me.neededPermission)
            End If
        End If
        
        If Not Page.IsPostBack Then
            hidHRJS.Value = Request.Item("hrjs")    'js funkce, která se má volat po submit záznamu
            With CType(toolbar1.FindItemByValue("help"), Telerik.Web.UI.RadToolBarButton)
                If Me.HelpTopicID = "" Then
                    .NavigateUrl = "http://www.marktime.net/doc/html/index.html"
                Else
                    .NavigateUrl = "http://www.marktime.net/doc/html/index.html?" & Me.HelpTopicID & ".htm"
                End If
            End With
            'CType(toolbar1.FindItemByValue("help"), Telerik.Web.UI.RadToolBarButton).NavigateUrl = "javascript:help('" & Request.FilePath & "')"
        Else
            pageTitle.Text = Me.HeaderText
        End If

        Me.lblRecordMessage.Text = Me.hidrecordMessage.Value
    End Sub

    Public Overloads Sub TestNeededPermission(neededPerm As BO.x53PermValEnum)
        If _Factory Is Nothing Then Return

        If Not _Factory.TestPermission(neededPerm) Then
            StopPage("Nedisponujete dostatečným oprávněním pro zobrazení této stránky", True, DirectCast(neededPerm, Int32).ToString)
            ''Server.Transfer("stoppage.aspx?modal=0&err=1&message=" & Server.UrlEncode("Nedisponujete dostatečným oprávněním pro zobrazení této stránky."), False)
        End If
    End Sub
    Public Overloads Sub TestNeededPermission(neededPerm As BO.x53PermValEnum, neededPermIfSecond As BO.x53PermValEnum)
        If _Factory Is Nothing Then Return

        If Not _Factory.TestPermission(neededPerm, neededPermIfSecond) Then
            StopPage("Nedisponujete dostatečným oprávněním pro zobrazení této stránky", True, DirectCast(neededPerm, Int32).ToString & " OR " & DirectCast(neededPermIfSecond, Int32).ToString)
            ''Server.Transfer("stoppage.aspx?modal=0&err=1&message=" & Server.UrlEncode("Nedisponujete dostatečným oprávněním pro zobrazení této stránky."), False)
        End If
    End Sub

    
    
    Public Sub master_show_message(strMessage As String)
        Me.hidrecordMessage.Value = strMessage
        lblRecordMessage.Text = strMessage
    End Sub
    
    
End Class