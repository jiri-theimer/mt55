Public Class j02_framework_detail
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As SubForm
    Public Property CurrentJ03ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidJ03ID.Value)
        End Get
        Set(value As Integer)
            hidJ03ID.Value = value.ToString
        End Set
    End Property

    Private Sub j02_framework_detail_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        menu1.Factory = Master.Factory
        menu1.DataPrefix = "j02"
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory
    End Sub
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cal1.factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .SiteMenuValue = "j02"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If Me.menu1.PageSource = "grid" Then
                    .IsHideAllRecZooms = True
                    ''If Request.Item("tab") <> "" Then
                    ''    .Factory.j03UserBL.SetUserParam("p41_framework_detail-tab", Request.Item("tab"))
                    ''End If
                End If
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("j02_framework_detail-pid")
                    .Add("j02_framework_detail-tab")
                    .Add("j02_menu-remember-tab")
                    .Add("j02_menu-tabskin")
                    ''.Add("j02_menu-menuskin")
                    .Add("j02_framework_detail-chkFFShowFilledOnly")
                    .Add("j02_menu-show-level1")
                    .Add("myscheduler-firstday")
                End With
                Dim intPID As Integer = Master.DataPID
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    If intPID = 0 Then
                        intPID = BO.BAS.IsNullInt(.GetUserParam("j02_framework_detail-pid", "O"))
                        If intPID = 0 Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=j02")
                    Else
                        If intPID <> BO.BAS.IsNullInt(.GetUserParam("j02_framework_detail-pid", "O")) Then
                            .SetUserParam("j02_framework_detail-pid", intPID.ToString)
                        End If
                    End If
                    Dim strTab As String = Request.Item("tab")
                    If strTab = "" And .GetUserParam("j02_menu-remember-tab", "0") = "1" Then strTab = .GetUserParam("j02_framework_detail-tab", "board")
                    Select Case strTab
                        Case "p31", "time", "expense", "fee", "kusovnik"
                            Server.Transfer("entity_framework_rec_p31.aspx?masterprefix=j02&masterpid=" & intPID.ToString & "&p31tabautoquery=" & strTab & "&source=" & menu1.PageSource, False)
                        Case "o23", "p91", "p56", "p41"
                            Server.Transfer("entity_framework_rec_" & strTab & ".aspx?masterprefix=j02&masterpid=" & intPID.ToString & "&source=" & menu1.PageSource, False)
                        Case Else
                            'zůstat zde na BOARD stránce
                    End Select
                    cal1.FirstDayMinus = BO.BAS.IsNullInt(.GetUserParam("myscheduler-firstday", "-1"))

                    menu1.TabSkin = .GetUserParam("j02_menu-tabskin")
                    ''menu1.MenuSkin = .GetUserParam("j02_menu-menuskin")
                    If .GetUserParam("j02_menu-remember-tab", "0") = "1" Then
                        menu1.LockedTab = .GetUserParam("j02_framework_detail-tab")
                    End If
                    ''menu1.ShowLevel1 = BO.BAS.BG(.GetUserParam("j02_menu-show-level1", "0"))
                    Me.chkFFShowFilledOnly.Checked = BO.BAS.BG(.GetUserParam("j02_framework_detail-chkFFShowFilledOnly", "0"))
                End With
                Master.DataPID = intPID
            End With




        End If
        RefreshRecord()
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(Master.DataPID)
        If cRec Is Nothing Then Response.Redirect("entity_framework_detail_missing.aspx?prefix=j02")

        Dim cRecSum As BO.j02PersonSum = Master.Factory.j02PersonBL.LoadSumRow(cRec.PID)

        menu1.j02_RefreshRecord(cRec, cRecSum, "board")

        With cRec
            Me.panIntraPerson.Visible = .j02IsIntraPerson
            Me.boxJ05.Visible = .j02IsIntraPerson
        End With
       

        With Me.panIntraPerson
            Me.c21Name.Visible = .Visible
            Me.lblFond.Visible = .Visible
            lblSMTP.Visible = .Visible
            smtpAccount.Visible = .Visible
            lblJ07Name.Visible = .Visible
            j07Name.Visible = .Visible
        End With

        If cRec.j02IsIntraPerson Then
            With cRecSum
                Me.Last_Access.Text = BO.BAS.FD(.Last_Access, True, True)
                Me.Last_Worksheet.Text = .Last_Worksheet
                If .p56_Actual_Count > 0 Then
                    Me.link_p56_actual_count.Text = .p56_Actual_Count.ToString
                    Me.link_p56_actual_count.NavigateUrl = "entity_framework_rec_p56.aspx?masterprefix=j02&masterpid=" & cRec.PID.ToString
                Else
                    link_p56_actual_count.Visible = False
                End If
            End With

            Dim cUser As BO.j03User = Nothing
            Dim mq As New BO.myQueryJ03
            mq.Closed = BO.BooleanQueryMode.NoQuery
            mq.j02ID = cRec.PID
            Dim lisJ03 As IEnumerable(Of BO.j03User) = Master.Factory.j03UserBL.GetList(mq)
            If lisJ03.Count > 0 Then
                panAccount.Visible = True
                cUser = lisJ03(0)
                With cUser
                    Me.j03Login.Text = .j03Login
                    Me.j04Name.Text = .j04Name
                    If .IsClosed Then
                        lblUserHeader.Text = "Uživatelský účet je uzavřený pro přihlašování!"
                        lblUserHeader.ForeColor = System.Drawing.Color.Red
                    End If
                End With
                AccountMessage.Text = ""
                Me.CurrentJ03ID = cUser.PID
                cmdLog.Visible = True
            Else
                Me.panAccount.Visible = False
                AccountMessage.Text = "Tento osobní profil není svázán s uživatelským účtem."
                cmdLog.Visible = False
            End If

        End If
        Handle_Permissions(cRec)


        With cRec
            Me.FullNameAsc.Text = .FullNameAsc
            Me.j02Email.Text = .j02Email
            Me.j02Code.Text = .j02Code
            Me.j02Email.NavigateUrl = "mailto:" & .j02Email
            If .j02IsInvoiceEmail Then Me.j02Email.ForeColor = Drawing.Color.Green
            Me.linkTimestamp.Text = .UserInsert & "/" & .DateInsert
            Me.linkTimestamp.ToolTip = "CHANGE-LOG"
            Me.linkTimestamp.NavigateUrl = "javascript:changelog()"
            If .j02Phone <> "" Then
                Me.Mediums.Text += " | " & .j02Phone
            End If
            If .j02Mobile <> "" Then
                Me.Mediums.Text += " | " & .j02Mobile
            End If
            If .j02Office <> "" Then
                Me.Mediums.Text += " | " & .j02Office
            End If
            If Me.Mediums.Text <> "" Then
                Me.Mediums.Text = BO.BAS.OM1(Trim(Me.Mediums.Text))
            End If
            If .j02Salutation <> "" Then
                Me.Correspondence.Text = String.Format("Oslovení pro korespondenci: {0}", "<b>" & .j02Salutation & "</b>")
            End If


            Me.j07Name.Text = .j07Name
            If Not cRec.j02IsIntraPerson Then
                Me.j07Name.Text = .j02JobTitle
            End If
            Me.c21Name.Text = .c21Name
            If .j17ID > 0 Then
                Me.j17Name.Text = Master.Factory.j17CountryBL.Load(.j17ID).j17Name : lblJ17Name.Visible = True
            Else
                Me.lblJ17Name.Visible = False
            End If
            If .j18ID = 0 Then
                lblJ18Name.Visible = False
            Else
                lblJ18Name.Visible = True : Me.j18Name.Text = .j18Name
            End If
            Me.TeamsInLine.Text = Master.Factory.j02PersonBL.GetTeamsInLine(.PID)
            If Me.TeamsInLine.Text = "" Then lblTeams.Visible = False
            If .j02AvatarImage <> "" Then
                imgAvatar.Visible = True
                imgAvatar.ImageUrl = "Plugins/Avatar/" & .j02AvatarImage
            End If
            If .o40ID > 0 Then
                Me.smtpAccount.Text = Master.Factory.o40SmtpAccountBL.Load(.o40ID).o40Name
            Else
                Me.smtpAccount.Text = "Výchozí aplikační poštovní účet"
            End If
        End With


        panMasters.Visible = False : panSlaves.Visible = False
        cmdAddJ05.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
        Dim lisJ05 As IEnumerable(Of BO.j05MasterSlave) = Master.Factory.j05MasterSlaveBL.GetList(cRec.PID, 0, 0)

        If lisJ05.Count > 0 Then
            panSlaves.Visible = True
            rpSlaves.DataSource = lisJ05
            rpSlaves.DataBind()
        End If
        lisJ05 = Master.Factory.j05MasterSlaveBL.GetList(0, cRec.PID, 0)
        If lisJ05.Count > 0 Then
            panMasters.Visible = True
            rpMasters.DataSource = lisJ05
            rpMasters.DataBind()
        End If




        Dim lisFF As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.j02Person, Master.DataPID, cRec.j07ID)
        If lisFF.Count > 0 Then
            ff1.FillData(lisFF, Not Me.chkFFShowFilledOnly.Checked)
        Else
            boxFF.Visible = False
        End If
        labels1.RefreshData(Master.Factory, BO.x29IdEnum.j02Person, cRec.PID)
        boxX18.Visible = labels1.ContainsAnyData

        

        Me.rpP30.DataSource = Master.Factory.p30Contact_PersonBL.GetList(0, 0, Master.DataPID)
        Me.rpP30.DataBind()
        If rpP30.Items.Count > 0 Then
            boxP30.Visible = True
        Else
            boxP30.Visible = False
        End If

        If Not (Master.Factory.SysUser.j04IsMenu_Project Or Master.Factory.SysUser.j04IsMenu_Contact) Then
            boxP30.Enabled = False
        End If

        If cal1.o25ID > 0 Or cRecSum.p56_Actual_Count > 0 Or cRecSum.o22_Actual_Count > 0 Then
            cal1.Visible = True
            cal1.o25ID = cRec.o25ID_Calendar
            cal1.RecordPID = Master.DataPID
            cal1.RefreshData(Today)
            cal1.RefreshTasksWithoutDate(True)
        Else
            cal1.Visible = False
        End If
        If cRecSum.b07_Count > 0 Then
            comments1.Visible = True
            comments1.RefreshData(Master.Factory, BO.x29IdEnum.j02Person, cRec.PID)
        Else
            comments1.Visible = False
        End If
        tags1.RefreshData(cRec.PID)
    End Sub

    Private Sub Handle_Permissions(cRec As BO.j02Person)

        Dim b As Boolean = Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin)
        With cmdAccount
            .Visible = b
            If Me.CurrentJ03ID = 0 And b Then
                .NavigateUrl = "javascript:j03_create()"
                .Text = "Založit uživatelský účet"
            Else
                .NavigateUrl = "javascript:j03_edit()"
                .Text = "Nastavení uživatelského účtu"
            End If
        End With


    End Sub

    Private Sub rpP30_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP30.ItemDataBound
        Dim cRec As BO.p30Contact_Person = CType(e.Item.DataItem, BO.p30Contact_Person)
        With CType(e.Item.FindControl("ContactLink"), HyperLink)
            If cRec.p28ID <> 0 Then
                .Text = cRec.p28Name
                .NavigateUrl = "p28_framework.aspx?pid=" & cRec.p28ID.ToString
            End If
            If cRec.p41ID <> 0 Then
                .Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, cRec.p41ID)
                .NavigateUrl = "p41_framework.aspx?pid=" & cRec.p41ID.ToString
            End If
        End With
        With CType(e.Item.FindControl("pm1"), HyperLink)
            If cRec.p28ID <> 0 Then
                .Attributes.Item("onclick") = "RCM('p28'," & cRec.p28ID.ToString & ",this)"
            End If
            If cRec.p41ID <> 0 Then
                .Attributes.Item("onclick") = "RCM('p41'," & cRec.p41ID.ToString & ",this)"
            End If
        End With
    End Sub
End Class