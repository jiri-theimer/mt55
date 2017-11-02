Public Class mobile_p31_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
   
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p41id.Value)
        End Get
        Set(value As Integer)
            Me.p41id.Value = value.ToString
            If value = 0 Then
                linkCurProject.Visible = False
            Else
                linkCurProject.Visible = True
            End If
        End Set
    End Property
    Public Property CurrentP56ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP56ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP56ID.Value = value.ToString
        End Set
    End Property
    
    Public Property CurrentP31ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP31ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP31ID.Value = value.ToString
        End Set
    End Property

    Public Property CurrentP34ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.p34ID, value.ToString)
        End Set
    End Property
    Public Property CurrentP33ID As BO.p33IdENUM
        Get
            If BO.BAS.IsNullInt(Me.hidP33ID.Value) = 0 Then
                Return BO.p33IdENUM.Cas
            Else
                Return CType(Me.hidP33ID.Value, BO.p33IdENUM)
            End If
        End Get
        Set(value As BO.p33IdENUM)
            hidP33ID.Value = CInt(value).ToString
        End Set
    End Property
    Public Property IsDirectCallP41ID As Boolean
        Get
            Return BO.BAS.BG(Me.hidDirectCallP41ID.Value)
        End Get
        Set(value As Boolean)
            hidDirectCallP41ID.Value = BO.BAS.GB(value)
        End Set
    End Property

    Private Sub mobile_p31_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.MenuPrefix = "p31"
        With Me.p41id.radComboBoxOrig
            .RenderMode = Telerik.Web.UI.RenderMode.Mobile
            .DropDownWidth = Unit.Parse("350px")
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then            
            If Request.Item("source") <> "" Then
                If Not Request.UrlReferrer Is Nothing Then Me.hidRef.Value = Request.UrlReferrer.PathAndQuery
            End If
            Me.p31Date.Text = Format(Now, "dd.MM.yyyy")
            If Request.Item("defdate") <> "" Then
                Me.p31Date.Text = Request.Item("defdate")
            End If
            Dim pars As New List(Of String)
            With pars
                .Add("mobile_p31_framework-tab")
                .Add("mobile_p31_framework-p41id")
                .Add("mobile_p31_framework-p56id")
                .Add("mobile-daysquerybefore")
            End With

            With Master.Factory.j03UserBL
                .InhaleUserParams(pars)
               

                Me.hidDaysQueryBefore.Value = .GetUserParam("mobile-daysquerybefore", "10")
            End With
            If BO.BAS.IsNullInt(Request.Item("p56id")) <> 0 Then
                Me.CurrentP56ID = BO.BAS.IsNullInt(Request.Item("p56id"))
                Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.Load(Me.CurrentP56ID)
                If cTask Is Nothing Then Master.StopPage("Nelze načíst předávaný úkol.")
                Me.CurrentP41ID = cTask.p41ID
                Me.IsDirectCallP41ID = True
            End If

            If Me.CurrentP41ID = 0 And BO.BAS.IsNullInt(Request.Item("p41id")) <> 0 Then
                Me.IsDirectCallP41ID = True
                Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
            End If
            If BO.BAS.IsNullInt(Request.Item("pid")) <> 0 Then
                RefreshRecord(BO.BAS.IsNullInt(Request.Item("pid")))
            Else
                Handle_ChangeP41(, True)
            End If


            RefreshP31List()
        End If
    End Sub


   
    
  


    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click
        SW("")
        Dim strVal As String = Me.HardRefreshValue.Value
        Select Case Me.HardRefreshFlag.Value
            Case "daysquerybefore", "save", "saveandcopy", "delete"
            Case Else
                Me.IsDirectCallP41ID = False
        End Select


        Select Case Me.HardRefreshFlag.Value
           
            Case "daysquerybefore"
                Me.hidDaysQueryBefore.Value = strVal
                Master.Factory.j03UserBL.SetUserParam("mobile-daysquerybefore", strVal)
                RefreshP31List()
                ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_list';", True)
            Case "save"
                If SaveChanges() Then
                    TestIfRedirect()
                    Me.IsDirectCallP41ID = False
                    RefreshRecord(Me.CurrentP31ID)
                    RefreshRecord(0)
                    RefreshP31List()
                End If
            Case "saveandcopy"
                If SaveChanges() Then
                    Me.IsDirectCallP41ID = False
                    RefreshRecord(Me.CurrentP31ID)
                    Me.CurrentP31ID = 0
                    RefreshP31List()
                End If
            Case "clear"
                If Me.CurrentP31ID <> 0 Then RefreshRecord(Me.CurrentP31ID)
                RefreshRecord(0)
            Case "edit"
                RefreshRecord(BO.BAS.IsNullInt(strVal))
                ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_start';", True)
            Case "delete"
                If Master.Factory.p31WorksheetBL.Delete(Me.CurrentP31ID) Then
                    TestIfRedirect()
                    Me.IsDirectCallP41ID = False
                    RefreshRecord(0)
                    RefreshP31List()
                    ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_start';", True)
                Else
                    SW(Master.Factory.p31WorksheetBL.ErrorMessage)
                End If

        End Select

        Me.HardRefreshFlag.Value = ""
    End Sub

    Private Sub SW(strMessage As String, Optional x As Integer = 0)
        If strMessage <> "" Then
            Me.panMessage.Visible = True
            Me.WarningMessage.Text = strMessage
            ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_message';", True)
        Else
            Me.panMessage.Visible = False
            Me.WarningMessage.Text = ""
        End If

    End Sub

    Private Sub Handle_ChangeP34()
        panT.Visible = False : panU.Visible = False : panM.Visible = False
        If Me.CurrentP34ID = 0 Then
            SW("V projektu nemáte přístup k worksheet sešitu.") : Return
        End If

        Dim mq As New BO.myQueryP32
        mq.p34ID = Me.CurrentP34ID
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()
        Me.p32ID.Items.Insert(0, "--Aktivita úkonu--")

        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(Me.CurrentP34ID)
        Me.lblRecordHeader.Text = BO.BAS.OM3(cRec.p34Name, 15)
        Me.CurrentP33ID = cRec.p33ID
        Select Case cRec.p33ID
            Case BO.p33IdENUM.Cas
                panT.Visible = True
            Case BO.p33IdENUM.Kusovnik
                panU.Visible = True
            Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                panM.Visible = True
                Dim b As Boolean = True
                If cRec.p33ID = BO.p33IdENUM.PenizeBezDPH Then
                    b = False
                End If
                Me.p31Amount_WithVat_Orig.Visible = b : Me.lblp31Amount_WithVat_Orig.Visible = b
                Me.p31Amount_Vat_Orig.Visible = b ': Me.lblp31Amount_Vat_Orig.Visible = b
                Me.p31VatRate_Orig.Visible = b : Me.lblp31VatRate_Orig.Visible = b

                If Me.j27ID_Orig.Items.Count = 0 Then
                    Me.j27ID_Orig.DataSource = Master.Factory.ftBL.GetList_J27(New BO.myQuery)
                    Me.j27ID_Orig.DataBind()

                End If
                SetupVatRateCombo()
        End Select
        If cRec.p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava Then
            'aktivita se ručně nezadává
            Me.p32ID.Visible = False
        Else
            Me.p32ID.Visible = True
        End If
    End Sub
    Private Sub SetupVatRateCombo()

        Dim intJ27ID As Integer = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
        Dim lis As IEnumerable(Of BO.p53VatRate) = Master.Factory.p53VatRateBL.GetList(New BO.myQuery).Where(Function(p) p.j27ID = intJ27ID)
        Me.p31VatRate_Orig.DataSource = lis
        Me.p31VatRate_Orig.DataBind()
    End Sub

    Private Sub Handle_ChangeP41(Optional cRecP31 As BO.p31Worksheet = Nothing, Optional bolSetProjectComboText As Boolean = False)
        If Me.CurrentP41ID = 0 Then
            Return
        End If
        Dim cRecP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
        If bolSetProjectComboText Then
            Me.p41id.Text = cRecP41.ProjectWithMask(Master.Factory.SysUser.j03ProjectMaskIndex)
            
        End If

        Me.p34ID.DataSource = Master.Factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(Me.CurrentP41ID, cRecP41.p42ID, cRecP41.j18ID, Master.Factory.SysUser.j02ID)
        Me.p34ID.DataBind()

        If Not cRecP31 Is Nothing Then
            basUI.SelectDropdownlistValue(Me.p34ID, cRecP31.p34ID.ToString)
        End If
        Handle_ChangeP34()
        If Not cRecP31 Is Nothing Then
            basUI.SelectDropdownlistValue(Me.p32ID, cRecP31.p32ID.ToString)
        End If
        
        Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Master.Factory.p30Contact_PersonBL.GetList(cRecP41.p28ID_Client, cRecP41.PID, 0)
        If lisP30.Count > 0 Then
            Dim intDefj02ID As Integer = cRecP41.j02ID_ContactPerson_DefaultInWorksheet
            If intDefj02ID = 0 And cRecP41.p28ID_Client <> 0 Then
                Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(cRecP41.p28ID_Client)
                intDefj02ID = cP28.j02ID_ContactPerson_DefaultInWorksheet
            End If
            RefreshContactPersonCombo(cRecP41, intDefj02ID)
        Else
            Me.j02ID_ContactPerson.Visible = False
        End If
        SetupP56Combo()
    End Sub

   
    Private Sub mobile_p31_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.CurrentP31ID = 0 Then
            imgHeader.ImageUrl = "Images/new.png"
            Me.cmdDelete.Visible = False
        Else
            imgHeader.ImageUrl = "Images/fe.png"
            Me.cmdDelete.Visible = True
        End If
        If Me.CurrentP41ID = 0 Then
            panRecord.Visible = False
            Me.lblRecordHeader.Text = "Vyberte projekt"
            imgHeader.ImageUrl = "Images/project.png"
        Else
            panRecord.Visible = True
            Me.linkCurProject.Text = "<img src='Images/project.png' /> " & Me.p41id.Text
            Me.linkCurProject.NavigateUrl = "mobile_p41_framework.aspx?pid=" & Me.CurrentP41ID.ToString
        End If
    End Sub

    Private Sub RefreshRecord(intP31ID As Integer)
        Me.CurrentP31ID = intP31ID

        Dim cRec As BO.p31Worksheet = Nothing
        If intP31ID <> 0 Then
            cRec = Master.Factory.p31WorksheetBL.Load(intP31ID)
            If cRec Is Nothing Then
                SW("record not found!") : Return
            End If
            Me.CurrentP41ID = cRec.p41ID


            Me.TimeStamp.Text = cRec.Timestamp

            Select Case cRec.p33ID
                Case BO.p33IdENUM.Cas
                    If cRec.IsRecommendedHHMM Then
                        Me.p31Hours_Orig.Text = cRec.p31HHMM_Orig.ToString
                    Else
                        Me.p31Hours_Orig.Text = cRec.p31Value_Orig.ToString
                    End If
                Case BO.p33IdENUM.Kusovnik
                    Me.p31Value_Orig.Text = cRec.p31Value_Orig.ToString
                Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                    Me.p31Amount_WithoutVat_Orig.Text = cRec.p31Amount_WithoutVat_Orig.ToString
                    If cRec.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                        Me.p31Amount_WithVat_Orig.Text = cRec.p31Amount_WithVat_Orig.ToString
                        Me.p31Amount_Vat_Orig.Text = cRec.p31Amount_Vat_Orig.ToString
                    End If
                    basUI.SelectDropdownlistValue(Me.j27ID_Orig, cRec.j27ID_Billing_Orig.ToString)
                    basUI.SelectDropdownlistValue(Me.p31VatRate_Orig, CInt(cRec.p31VatRate_Orig).ToString)
            End Select

            Me.p31Text.Text = cRec.p31Text
            Me.p31Date.Text = Format(cRec.p31Date, "dd.MM.yyyy")
            If cRec.p56ID <> 0 Then
                Me.CurrentP56ID = cRec.p56ID

            End If

            If cRec.p71ID > BO.p71IdENUM.Nic Then
                SW("Úkon již prošel schvalováním.")

            End If
            If cRec.p91ID > 0 Then
                SW("Tento úkon již prošel fakturací.")
            End If
            
        Else
            Me.p31Text.Text = ""
            Me.p31Value_Orig.Text = ""
            Me.p31Hours_Orig.Text = ""
            ''Me.p31Date.Text = Format(Now, "dd.MM.yyyy")

            p31Amount_WithVat_Orig.Text = "" : p31Amount_Vat_Orig.Text = "" : p31Amount_WithoutVat_Orig.Text = ""
            Me.TimeStamp.Text = ""
        End If

        If intP31ID <> 0 Then
            Handle_ChangeP41(cRec, True)
            
            If cRec.j02ID_ContactPerson <> 0 Then
                basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson, cRec.j02ID_ContactPerson.ToString)
            End If
            If cRec.p56ID <> 0 Then
                basUI.SelectDropdownlistValue(Me.p56ID, cRec.p56ID.ToString)
            End If
        End If
        


    End Sub

    Private Sub p34ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p34ID.SelectedIndexChanged
        Handle_ChangeP34()
        ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_p32id';", True)
    End Sub
    Private Sub p32ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p32ID.SelectedIndexChanged
        ClientScript.RegisterStartupScript(Me.GetType, "hash", "location.hash = '#record_p32id';", True)
    End Sub

    

    Private Sub ReloadPage()
        Response.Redirect("mobile_p31_framework.aspx?" & basUI.GetCompleteQuerystring(Request), True)
    End Sub

    Private Sub RefreshContactPersonCombo(cP41 As BO.p41Project, intDefJ02ID As Integer)
        Me.j02ID_ContactPerson.Visible = False

        If Me.CurrentP41ID = 0 Then Return

        Dim mq As New BO.myQueryJ02
        mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
        mq.p41ID = cP41.PID

        Dim lisJ02 As List(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq).ToList
        If lisJ02.Count = 0 And cP41.p28ID_Client <> 0 Then
            mq.p41ID = 0 : mq.p28ID = cP41.p28ID_Client
        End If
        If intDefJ02ID > 0 And lisJ02.Where(Function(p) p.PID = intDefJ02ID).Count = 0 Then
            Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(intDefJ02ID)
            If Not c Is Nothing Then lisJ02.Add(c)
        End If
        If lisJ02.Count > 0 Then
            Me.j02ID_ContactPerson.DataSource = lisJ02
            Me.j02ID_ContactPerson.DataBind()
            Me.j02ID_ContactPerson.Items.Insert(0, "--Kontaktní osoba--")
            If intDefJ02ID > 0 Then basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson, intDefJ02ID.ToString)
            Me.j02ID_ContactPerson.Visible = True
        End If
    End Sub

    Private Sub SetupP56Combo()
        Me.p56ID.Visible = False
        Dim mq As New BO.myQueryP56
        mq.p41ID = Me.CurrentP41ID
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForWorksheetEntry
        mq.j02ID_ExplicitQueryFor = Master.Factory.SysUser.j02ID

        Me.p56ID.DataSource = Master.Factory.p56TaskBL.GetList(mq)
        Me.p56ID.DataBind()
        If Me.p56ID.Items.Count > 0 Then
            Me.p56ID.Visible = True
            Me.p56ID.Items.Insert(0, "--Úkol v rámci projektu--")
            If Me.CurrentP56ID <> 0 Then
                basUI.SelectDropdownlistValue(Me.p56ID, Me.CurrentP56ID.ToString)
            End If
        End If
       
        
    End Sub

    Private Function SaveChanges() As Boolean
        If Me.CurrentP31ID <> 0 Then
            Dim c As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Me.CurrentP31ID)
            If c.p71ID > BO.p71IdENUM.Nic Or c.p91ID <> 0 Then
                SW("Záznam nelze uložit, protože již prošel schvalováním.") : Return False
            End If
        End If
        With Master.Factory.p31WorksheetBL
            Dim cRec As New BO.p31WorksheetEntryInput()
            With cRec
                .SetPID(Me.CurrentP31ID)
                .j02ID = Master.Factory.SysUser.j02ID
                .p41ID = Me.CurrentP41ID
                .p56ID = BO.BAS.IsNullInt(Me.p56ID.SelectedValue)
                .p34ID = Me.CurrentP34ID
                .p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                .j02ID_ContactPerson = BO.BAS.IsNullInt(Me.j02ID_ContactPerson.SelectedValue)
                .p31Date = BO.BAS.ConvertString2Date(Me.p31Date.Text)
                .p31Text = Me.p31Text.Text

                Select Case Me.CurrentP33ID
                    Case BO.p33IdENUM.Cas
                        .p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Hodiny
                        .Value_Orig = Me.p31Hours_Orig.Text
                        .Value_Orig_Entried = .Value_Orig

                        If Not .ValidateEntryTime(5) Then

                            SW(.ErrorMessage, 2)
                            Return False
                        End If
                    Case BO.p33IdENUM.Kusovnik
                        .Value_Orig = BO.BAS.IsNullNum(Me.p31Value_Orig.Text)
                        .Value_Orig_Entried = .Value_Orig
                        If Not .ValidateEntryKusovnik() Then
                            SW(.ErrorMessage, 2)
                            Return False
                        End If
                    Case BO.p33IdENUM.PenizeBezDPH
                        .Amount_WithoutVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithoutVat_Orig.Text)
                        .Value_Orig_Entried = .Amount_WithoutVat_Orig.ToString
                        .j27ID_Billing_Orig = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
                    Case BO.p33IdENUM.PenizeVcDPHRozpisu
                        .Amount_WithoutVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithoutVat_Orig.Text)
                        .Value_Orig_Entried = .Amount_WithoutVat_Orig.ToString
                        .VatRate_Orig = BO.BAS.IsNullNum(Me.p31VatRate_Orig.SelectedItem.Text)
                        .j27ID_Billing_Orig = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
                        .Amount_WithVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithVat_Orig.Text)
                        .Amount_Vat_Orig = BO.BAS.IsNullNum(Me.p31Amount_Vat_Orig.Text)
                End Select

            End With
            If .SaveOrigRecord(cRec, Nothing) Then
                Me.CurrentP31ID = .LastSavedPID
                Return True
            Else
                SW(.ErrorMessage, 2)
                Return False
            End If

        End With

    End Function


    Private Sub RefreshP31List()
        Dim mq As New BO.myQueryP31
        mq.j02ID = Master.Factory.SysUser.j02ID
        mq.DateFrom = Today.AddDays(-1 * BO.BAS.IsNullInt(Me.hidDaysQueryBefore.Value))
        mq.MG_SortString = "p31Date DESC"

        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        Dim strHeader As String = "Úkony od " & BO.BAS.FD(mq.DateFrom, False, True) & " | " & lis.Count.ToString & "x"
        list1.RefreshData(lis, strHeader)
    End Sub

    

    

    Private Sub TestIfRedirect()
        If Me.hidRef.Value <> "" Then
            
            Response.Redirect(Me.hidRef.Value)
        End If
    End Sub

    Private Sub p41id_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41id.AutoPostBack_SelectedIndexChanged
        If Me.CurrentP41ID = 0 Then Return

        Master.Factory.j03UserBL.SetUserParam("mobile_p31_framework-p41id", Me.CurrentP41ID.ToString)
        Me.CurrentP56ID = 0
        Master.Factory.j03UserBL.SetUserParam("mobile-p41id", Me.CurrentP41ID.ToString)
        Dim cRecP31 As BO.p31Worksheet = Nothing
        If Me.CurrentP31ID <> 0 Then
            cRecP31 = Master.Factory.p31WorksheetBL.Load(Me.CurrentP31ID)
        End If
        Handle_ChangeP41(cRecP31, False)
    End Sub
End Class