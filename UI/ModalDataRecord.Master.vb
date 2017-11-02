Public Class ModalDataRecord
    Inherits System.Web.UI.MasterPage
    Public Property ErrorMessage As String = ""
    Public Property HelpTopicID As String = ""
    Private Property _Factory As BL.Factory = Nothing
    Public Property neededPermission As BO.x53PermValEnum   'oprávnění, které je nutné pro spuštění stránky
    Public Property neededPermissionIfSecond As BO.x53PermValEnum = Nothing

    Public Event Master_OnSave()
    Public Event Master_OnRefresh()
    Public Event Master_OnDelete()
    Public Event Master_OnToolbarClick(ByVal strButtonValue As String)

    Public ReadOnly Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
    End Property
    Public Property DataPID() As Integer
        Get
            If IsNumeric(hidDataPID.Value) Then Return CInt(hidDataPID.Value) Else Return 0
        End Get
        Set(ByVal value As Integer)
            hidDataPID.Value = value.ToString
            If value = 0 And Me.EnableRecordValidity Then
                Me.RecordValidFrom = Now
                Me.RecordValidUntil = DateSerial(3000, 1, 1)
            End If
        End Set
    End Property

    Public Property EnableRecordValidity As Boolean
        Get
            Return BO.BAS.BG(Me.hidShowValidity.Value)
        End Get
        Set(value As Boolean)
            Me.hidShowValidity.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property RecordValidFrom As Date
        Get
            If Me.record_valid_from.Value = "" Then Return Now()
            Return DateTime.ParseExact(Me.record_valid_from.Value, "dd.MM.yyyy HH:mm:ss", Nothing)
        End Get
        Set(value As Date)
            Me.record_valid_from.Value = Format(value, "dd.MM.yyyy HH:mm:ss")
            RefreshState_RecordValidity()
        End Set
    End Property
    Public Property RecordValidUntil As Date
        Get
            If Me.record_valid_until.Value = "" Then Return DateSerial(3000, 1, 1)
            Return DateTime.ParseExact(Me.record_valid_until.Value, "dd.MM.yyyy HH:mm:ss", Nothing)
        End Get
        Set(value As Date)
            Me.record_valid_until.Value = Format(value, "dd.MM.yyyy HH:mm:ss")
            RefreshState_RecordValidity()
        End Set
    End Property
    Public Sub InhaleRecordValidity(datValidFrom As Date?, datValidUntil As Date?, datRecordInserted As Date?)
        Me.EnableRecordValidity = True
        toolbar1.FindItemByValue("bin").Visible = True
        If datValidFrom Is Nothing Then datValidFrom = datRecordInserted
        If datValidFrom Is Nothing Then datValidFrom = Now()
        If datValidUntil Is Nothing Then datValidUntil = DateSerial(3000, 1, 1)
        Me.RecordValidFrom = datValidFrom
        Me.RecordValidUntil = datValidUntil
       

        
    End Sub

    Public ReadOnly Property RecordValidityIsClosed As Boolean
        Get
            If Me.RecordValidFrom <= Now And Me.RecordValidUntil >= Now Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Public Sub ChangeToolbarSkin(strNewSkin As String)
        toolbar1.Skin = strNewSkin
    End Sub

    Private Sub RefreshState_RecordValidity()
        Dim d1 As Date = Me.RecordValidFrom, d2 As Date = Me.RecordValidUntil, cmd As Telerik.Web.UI.RadToolBarButton = toolbar1.FindButtonByCommandName("bin")
        Dim strColor As String = "black"
        If RecordValidityIsClosed Then strColor = "red"
        Dim s1 As String = "<span style='color:" & strColor & ";'>" & BO.BAS.FD(d1, True) & "</span>", s2 As String = "<span style='color:" & strColor & ";'>" & BO.BAS.FD(d2, True) & "</span>"
        If RecordValidityIsClosed Then
            panRecValidity.Style.Item("display") = "block"
            toolbar1.Skin = "BlackMetroTouch"
            cmd.Text = Resources.common.move_from_archive
            cmd.ImageUrl = "Images/recycle.png"

            lblValidity.Text = "Záznam je v archivu (byl otevřený od " & s1 & " do " & s2 & ")"

        Else
            panRecValidity.Style.Item("display") = "none"
            toolbar1.Skin = "Bootstrap"

            cmd.Text = Resources.common.move_to_archive
            cmd.ImageUrl = "Images/bin.png"
            If Year(d2) = 3000 Then
                lblValidity.Text = "Záznam je otevřený od " & s1
            Else
                lblValidity.Text = "Záznam je otevřený od " & s1 & " do " & s2
                panRecValidity.Style.Item("display") = "block"
            End If
        End If
        cmd.NavigateUrl = "javascript: rec_validity('" & Me.record_valid_from.Value & "','" & Me.record_valid_until.Value & "')"
    End Sub

    Public ReadOnly Property IsRecordNew() As Boolean
        Get
            If DataPID = 0 Then Return True Else Return False
        End Get
    End Property
    Public ReadOnly Property IsRecordClone As Boolean
        Get
            If hidIsRecordClone.Value = "1" Then Return True Else Return False
        End Get
    End Property


    Public Property IsRecordEditable() As Boolean
        Get
            Return BO.BAS.BG(hidIsRecordEditable.Value)
        End Get
        Set(ByVal value As Boolean)
            hidIsRecordEditable.Value = BO.BAS.GB(value)
            RefreshState_Toolbar()
        End Set
    End Property
    

    Public Property IsRecordDeletable() As Boolean
        Get
            Return BO.BAS.BG(hidIsRecordDeletable.Value)
        End Get
        Set(ByVal value As Boolean)
            hidIsRecordDeletable.Value = BO.BAS.GB(value)
            RefreshState_Toolbar()
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

    Public Property Timestamp() As String
        Get
            Return lblTimestamp.Text
        End Get
        Set(ByVal value As String)
            lblTimestamp.Text = value
        End Set
    End Property


    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.User.Identity.IsAuthenticated And _Factory Is Nothing Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            _Factory = New BL.Factory(, strLogin)
            If _Factory.SysUser Is Nothing Then DoLogOut()
            basUI.PingAccessLog(_Factory, Request)
            
            'PersonalizeMenu()
        End If
        If Request.Item("clone") = "1" Then
            hidIsRecordClone.Value = "1"
        End If
        If Not Page.IsPostBack Then
            hidHRJS.Value = Request.Item("hrjs")    'js funkce, která se má volat po submit záznamu
        End If
    End Sub
    
    Private Sub DoLogOut()
        Response.Redirect("~/Account/Login.aspx?autologout=1") 'automatické odhlášení
    End Sub

   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        If Me.neededPermission > 0 And Me.neededPermissionIfSecond > 0 Then
            TestNeededPermission(Me.neededPermission, Me.neededPermissionIfSecond)
        Else
            If Me.neededPermission > 0 Then
                TestNeededPermission(Me.neededPermission)
            End If
        End If

        If Not Page.IsPostBack Then
            If Request.Item("ReturnUrl") <> "" Then
                HideShowToolbarButton("goback", True)
                HideShowToolbarButton("sep_goback", True)
                CType(toolbar1.FindItemByValue("goback"), Telerik.Web.UI.RadToolBarButton).PostBackUrl = Server.UrlDecode(Request.Item("ReturnUrl"))
            Else
                HideShowToolbarButton("goback", False)
                HideShowToolbarButton("sep_goback", False)
            End If
            If Request.Item("clone") = "1" Then

                Me.HeaderText += " | Nový záznam kopírováním"
                Me.Timestamp = "Nový záznam kopírováním"
            End If
            'CType(toolbar1.FindItemByValue("help"), Telerik.Web.UI.RadToolBarButton).NavigateUrl = "javascript:help('" & Request.FilePath & "')"
            With CType(toolbar1.FindItemByValue("help"), Telerik.Web.UI.RadToolBarButton)
                If Me.HelpTopicID = "" Then
                    .NavigateUrl = "http://www.marktime.net/doc/html/index.html"
                Else
                    .NavigateUrl = "http://www.marktime.net/doc/html/index.html?" & Me.HelpTopicID & ".htm"
                End If
            End With


        Else
            pageTitle.Text = Me.HeaderText
        End If

        RefreshState_Toolbar()
    End Sub



    Private Sub RefreshState_Toolbar()
        HideShowToolbarButton("save", Me.IsRecordEditable)
        HideShowToolbarButton("delete", Me.IsRecordEditable)
        If Me.EnableRecordValidity Then HideShowToolbarButton("bin", Me.IsRecordEditable)

        With toolbar1.Items

            HideShowToolbarButton("delete", Me.IsRecordDeletable)
            .FindItemByValue("delete").Attributes.Item("onclick") = "trydelete();"
            If Me.IsRecordNew Then
                HideShowToolbarButton("delete", False)
                If Me.EnableRecordValidity Then HideShowToolbarButton("bin", False)

            End If
            If Not IsRecordEditable Then
                HideShowToolbarButton("save", False)
                HideShowToolbarButton("delete", False)
               
            End If

        End With
        If hidForceOperation.Value = "delete" Then
            hidForceOperation.Value = ""
            RaiseEvent Master_OnDelete()
        End If
        need2SaveMessage.Text = hidNeed2SaveMessage.Value
    End Sub

    

    Public Sub Notify(ByVal strText As String, Optional ByVal msgLevel As NotifyLevel = NotifyLevel.InfoMessage, Optional ByVal strTitle As String = "")
        basUI.NotifyMessage(Me.notify1, strText, msgLevel)
    End Sub
    Public Sub StopPage(ByVal strMessage As String, Optional ByVal bolErrorInfo As Boolean = True, Optional ByVal strNeededPerms As String = "")
        Server.Transfer("~/stoppage.aspx?modal=1&err=" & BO.BAS.GB(bolErrorInfo) & "&message=" & Server.UrlEncode(strMessage) & "&neededperms=" & strNeededPerms, False)
    End Sub

    Public Sub CloseAndRefreshParent(Optional ByVal strFlag As String = "refresh")
        hidForceOperation.Value = "closeandrefresh"
        hidCloseAndRefreshParent_Flag.Value = strFlag
    End Sub

    Private Sub toolbar1_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles toolbar1.ButtonClick
        hidNeed2SaveMessage.Value = "" : need2SaveMessage.Text = ""
        Select Case e.Item.Value
            Case "save"
                RaiseEvent Master_OnSave()
            Case "refresh"
                RaiseEvent Master_OnRefresh()
            Case "delete"
                RaiseEvent Master_OnDelete()
            Case Else
        End Select
        RaiseEvent Master_OnToolbarClick(e.Item.Value)
    End Sub

    Public Sub RenameToolbarButton(ByVal strButtonValue As String, ByVal strNewText As String)
        basUI.RenameToolbarButton(Me.toolbar1, strButtonValue, strNewText)
    End Sub
    Public Sub HideShowToolbarButton(ByVal strButtonValue As String, ByVal bolVisible As Boolean)
        basUI.HideShowToolbarButton(Me.toolbar1, strButtonValue, bolVisible)
    End Sub

    Public Sub AddToolbarButton(ByVal strText As String, ByVal strValue As String, Optional ByVal Index As Integer = 0, Optional ByVal strImageURL As String = "", Optional ByVal bolPostBack As Boolean = True, Optional ByVal strNavigateURL As String = "")
        basUI.AddToolbarButton(Me.toolbar1, strText, strValue, Index, strImageURL, bolPostBack, strNavigateURL)

    End Sub
    Public Sub HideShowToolbar(bolToolbarIsVisible As Boolean)
        Me.toolbar1.Visible = bolToolbarIsVisible
    End Sub

    Public Overloads Sub TestNeededPermission(neededPerm As BO.x53PermValEnum)
        If _Factory Is Nothing Then Return

        If Not _Factory.TestPermission(neededPerm) Then
            StopPage("Nedisponujete dostatečným oprávněním pro zobrazení této stránky.", True, DirectCast(neededPerm, Int32).ToString)
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
End Class