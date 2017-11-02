Imports Telerik.Web.UI

Public Class main_menu
    Inherits System.Web.UI.UserControl

    
    Public Property SelectedValue As String
        Get
            Dim n As NavigationNode = menu1.GetAllNodes.First(Function(p) p.Selected = True)
            If Not n Is Nothing Then
                Return n.ID
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If value = "" Then Return
            Try
                Dim n As NavigationNode = menu1.GetAllNodes.First(Function(p) p.ID = value)
                If Not n Is Nothing Then

                    n.Selected = True
                    If TypeOf n.Parent Is NavigationNode Then
                        CType(n.Parent, NavigationNode).Selected = True
                    End If
                End If
            Catch ex As Exception

            End Try
            
            
        End Set
    End Property
    Public Property MasterPageName As String
        Get
            Return Me.hidMasterPageName.Value
        End Get
        Set(value As String)
            hidMasterPageName.Value = value
        End Set
    End Property
    Public ReadOnly Property ItemsCount As Integer
        Get
            If Not panContainer.Visible Then Return 0
            Return menu1.Nodes.Count
        End Get
    End Property

   
    

    Public Sub ClearAll()
        panContainer.Controls.Remove(menu1)
        panContainer.Visible = False
    End Sub
    Public Sub RefreshData(factory As BL.Factory, strCurrentHelpID As String, strCurrentSiteMenuValue As String)
        Dim strLang As String = basUI.GetCookieValue(Request, "MT50-CultureInfo")

        Me.panContainer.Visible = True
        Dim bolAdmin As Boolean = factory.TestPermission(BO.x53PermValEnum.GR_Admin), n As NavigationNode
        With factory.SysUser

            If .j03SiteMenuSkin > "" Then menu1.Skin = .j03SiteMenuSkin
            'If menu1.Nodes.Count > 0 Then menu1.Nodes.Clear()

            If .j04IsMenu_Project Or .j04IsMenu_Contact Or .j04IsMenu_Invoice Or .j04IsMenu_People Then

                ai("", "searchbox", "javascript:mysearch()", "Images/search_silver.png", , "Najít projekt, klienta, fakturu, osobu nebo fulltext")
            End If

            ai("", "newrec", "javascript:RCM('newrec')", "Images/new4menu.png", , Resources.common.Novy)
            
            ''If Not b Then menu1.Nodes.Remove(n)
        End With

        RenderDbMenu(factory, strLang)


        With factory.SysUser
            n = ai(.Person, "me", "", "")
            If .Person = "" Then n.Text = .j03Login
            If .j04IsMenu_MyProfile Then ai(Resources.Site.MujProfil, "cmdMyProfile", "j03_myprofile.aspx", "", n)
            ai(Resources.Site.UlozitJakoVychoziStranku, "cmdDefaultPage", "javascript:defpage('" & Request.Url.PathAndQuery & "')", "", n)
            ai(Resources.Site.ZmenitHeslo, "cmdChangePassword", "changepassword.aspx", "", n)
            ai(Resources.Site.OdhlasitSe, "cmdLogout", "Account/Login.aspx?logout=1", "", n)

            If .MessagesCount > 0 Then
                n = ai("<img src='Images/globe.png'/>" + .MessagesCount.ToString, "messages", "javascript:messages()", "")
                n.ToolTip = "Zprávy a upozornění ze systému"
            End If
           

            Select Case strLang
                Case "en-US"
                    n = ai("<img src='Images/Flags/menu_uk.gif'/>", "lang", "", "")
                Case "-"
                    n = ai("<img src='Images/Flags/menu_czech.gif'/>", "lang", "", "")
                Case Else
                    n = ai("", "lang", "", "")
            End Select
            ai("Česky", "", "javascript:setlang('-')", "Images/Flags/menu_czech.gif", n)
            ai("English", "", "javascript:setlang('en-US')", "Images/Flags/menu_uk.gif", n)

            n = ai("?", "help", "http://www.marktime.net/doc/html", "")
            n.Target = "_blank"
            n.ToolTip = Resources.Site.Napoveda
            If strCurrentHelpID <> "" Then
                n.NavigateUrl = "http://www.marktime.net/doc/html/index.html?" & strCurrentHelpID & ".htm"
            End If
        End With
        Me.SelectedValue = strCurrentSiteMenuValue

        
    End Sub
    

    Private Function ai(strText As String, strValue As String, strURL As String, strImg As String, Optional nParent As NavigationNode = Nothing, Optional strTooltip As String = "") As NavigationNode
        Dim n As New NavigationNode(strText)
        n.NavigateUrl = strURL
        n.ImageUrl = strImg
        n.ID = strValue
        n.ToolTip = strTooltip
        If Not nParent Is Nothing Then
            nParent.Nodes.Add(n)
        Else
            menu1.Nodes.Add(n)
        End If
        Return n

    End Function
    

    ''Private Sub menu1_ItemCreated(sender As Object, e As RadMenuEventArgs) Handles menu1.ItemCreated
    ''    If Not TypeOf (e.Item) Is RadMenuItem Then Return
    ''    Select Case e.Item.Value
    ''        Case "begin"
    ''            e.Item.Controls.Add(New LiteralControl("<a href='default.aspx' title='ÚVOD'><img src='Images/logo_transparent.png' style='border:0px;' /></a>"))
    ''        Case "searchbox"
    ''            ''e.Item.Controls.Add(New LiteralControl("<a class='button-reczoom' rel='clue_search.aspx' dialogheight='600' title='Hledat' style='cursor:pointer;cursor:hand;padding-bottom:2px;padding-top:2px;border:0px;background-color:transparent;'><img src='Images/search_silver.png' style='border:0px;' title='Najít projekt/klienta/fakturu/osobu/fulltext'/></a>"))
    ''            e.Item.Controls.Add(New LiteralControl("<a href='javascript:mysearch()' style='cursor:pointer;cursor:hand;padding-bottom:2px;padding-top:2px;border:0px;background-color:transparent;'><img src='Images/search_silver.png' style='border:0px;' title='Najít projekt, klienta, fakturu, osobu nebo fulltext'/></a>"))
    ''    End Select


    ''End Sub

    


    Private Sub RenderDbMenu(factory As BL.Factory, strLang As String)
        Dim mq As New BO.myQuery
        mq.MG_GridSqlColumns = "a.x29ID,j62Name,a.j62Name_ENG,a.j62ParentID,a.j70ID,a.j62Url,a.j62Target,a.j62ImageUrl,a.j62Tag,a.j62TreeLevel as _j62TreeLevel"    'kvůli co nejvyšší rychlosti
        Dim lisJ62 As IEnumerable(Of BO.j62MenuHome) = factory.j62MenuHomeBL.GetList(factory.SysUser.j60ID, mq)
        Dim ns As New Dictionary(Of Integer, NavigationNode), bolGO As Boolean = False
        Dim nSeps As New List(Of NavigationNode)

        For Each c In lisJ62
            bolGO = True
            With factory.SysUser
                Select Case c.x29ID
                    Case BO.x29IdEnum.p31Worksheet
                        bolGO = .j04IsMenu_Worksheet
                        If c.j62Tag = "p31_pivot" Or c.j62Tag = "p31_sumgrid" Then bolGO = factory.TestPermission(BO.x53PermValEnum.GR_P31_Pivot)
                        If c.j62Tag = "p31_approving" Then bolGO = .IsApprovingPerson
                    Case BO.x29IdEnum.p41Project
                        bolGO = .j04IsMenu_Project
                    Case BO.x29IdEnum.p28Contact
                        bolGO = factory.SysUser.j04IsMenu_Contact
                    Case BO.x29IdEnum.p91Invoice : bolGO = .j04IsMenu_Invoice
                    Case BO.x29IdEnum.p90Proforma : bolGO = .j04IsMenu_Proforma
                    Case BO.x29IdEnum.j02Person : bolGO = .j04IsMenu_People
                    Case BO.x29IdEnum.o23Doc
                        bolGO = .j04IsMenu_Notepad
                    Case BO.x29IdEnum.System
                        bolGO = .IsAdmin
                        If c.j62Tag = "navigator" Then bolGO = factory.TestPermission(BO.x53PermValEnum.GR_Navigator)
                    Case BO.x29IdEnum.p51PriceList : bolGO = factory.TestPermission(BO.x53PermValEnum.GR_P51_Admin)
                    Case BO.x29IdEnum.x31Report : bolGO = .j04IsMenu_Report
                    Case BO.x29IdEnum.p56Task : bolGO = .j04IsMenu_Task
                    Case BO.x29IdEnum.p48OperativePlan : bolGO = factory.TestPermission(BO.x53PermValEnum.GR_P48_Creator, BO.x53PermValEnum.GR_P48_Reader)



                End Select
            End With

            If Not bolGO Then Continue For 'skočit na další c v cyklu

            Dim n As New NavigationNode(c.j62Name)
            If strLang = "en-US" And c.j62Name_ENG <> "" Then n.Text = c.j62Name_ENG 'menu v angličtině
            n.ImageUrl = c.j62ImageUrl
            n.ID = c.j62Tag

            If c.j62IsSeparator Then
                n.NavigateUrl = ""
                n.Enabled = False
            Else
                n.NavigateUrl = c.j62Url
                If c.j70ID > 0 Then
                    If n.NavigateUrl.IndexOf("?") > 0 Then
                        n.NavigateUrl += "&j62id=" & c.PID.ToString
                    Else
                        n.NavigateUrl += "?j62id=" & c.PID.ToString
                    End If
                    n.ID = "hm" & c.PID.ToString
                End If
                n.Target = c.j62Target
            End If

            If n.NavigateUrl = "" Then nSeps.Add(n)

            Dim nParent As NavigationNode = Nothing
            If c.j62ParentID > 0 Then
                Try
                    nParent = ns.First(Function(p) p.Key = c.j62ParentID).Value()
                Catch ex As Exception
                End Try
            End If
            If nParent Is Nothing Then
                menu1.Nodes.Add(n)
            Else
                With nParent
                    .Nodes.Add(n)
                End With

            End If            
            ns.Add(c.PID, n)
          
        Next

        For Each n In nSeps
            If n.Nodes.Count = 0 Then
                menu1.Nodes.Remove(n)
                'menu1.FindNodeByText(n.Text).Visible = False
                'menu1.FindNodeByText(n.Text).Text = ""
            End If
        Next
    End Sub

    ''Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
    ''    If panContainer.Visible Then SetupSearchbox()
    ''End Sub
End Class