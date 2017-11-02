Public Class p31_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord
    Private Property _Project As BO.p41Project = Nothing
    Private Property _Sheet As BO.p34ActivityGroup = Nothing

    Private Sub p31_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p31_record"
    End Sub
    Private ReadOnly Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p41ID.Value)
        End Get
    End Property
    Private Property CurrentJ02ID As Integer
        Get
            If Me.hidCurPerson_J02ID.Value = "" Then Me.hidCurPerson_J02ID.Value = Master.Factory.SysUser.j02ID.ToString
            Return BO.BAS.IsNullInt(Me.hidCurPerson_J02ID.Value)
        End Get
        Set(value As Integer)
            Me.hidCurPerson_J02ID.Value = value.ToString
            Me.j02ID.Value = value.ToString
            Me.p41ID.J02ID_Explicit = Me.j02ID.Value
            If value > 0 Then
                If value = Master.Factory.SysUser.j02ID Then
                    Me.hidCurPerson_Name.Value = Master.Factory.SysUser.PersonDesc
                Else
                    Me.hidCurPerson_Name.Value = Master.Factory.j02PersonBL.Load(value).FullNameDesc
                End If
            Else
                Me.hidCurPerson_Name.Value = ""
            End If
        End Set
    End Property
    Private Property CurrentP34ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        End Get
        Set(value As Integer)
            Me.p34ID.SelectedValue = value.ToString
        End Set
    End Property
    Private ReadOnly Property CurrentP33ID As BO.p33IdENUM
        Get
            If Me.hidP33ID.Value = "" Or Me.hidP33ID.Value = "0" Then Return BO.p33IdENUM.Cas
            Return CType(Me.hidP33ID.Value, BO.p33IdENUM)
        End Get
    End Property
    Private Property CurrentHoursEntryFlag As BO.p31HoursEntryFlagENUM
        Get
            Return CType(BO.BAS.IsNullInt(Me.hidHoursEntryFlag.Value), BO.p31HoursEntryFlagENUM)
        End Get
        Set(value As BO.p31HoursEntryFlagENUM)
            Me.hidHoursEntryFlag.Value = CInt(value).ToString
        End Set
    End Property
    Private Property CurrentIsScheduler As Boolean
        Get
            Return BO.BAS.BG(Me.hidCurIsScheduler.Value)
        End Get
        Set(value As Boolean)
            Me.hidCurIsScheduler.Value = BO.BAS.GB(value)
        End Set
    End Property
    Private Property CurrentP32ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
        End Get
        Set(value As Integer)
            Me.p32ID.SelectedValue = value.ToString
        End Set
    End Property
    Private Property CurrentP91ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP91ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP91ID.Value = value.ToString
        End Set
    End Property
    Private Property CurrentP48ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP48ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP48ID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentP85ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP85ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP85ID.Value = value.ToString
        End Set
    End Property
    
    Public Property MyDefault_p34ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidDefaultP34ID.Value)
        End Get
        Set(value As Integer)
            Me.hidDefaultP34ID.Value = value.ToString
        End Set
    End Property
    Private Property MyDefault_p31Date As Date
        Get
            Return BO.BAS.ConvertString2Date(Me.hidDefaultP31Date.Value)
        End Get
        Set(value As Date)
            Me.hidDefaultP31Date.Value = BO.BAS.ConvertString2Date(value)
        End Set
    End Property
    Private Property MyDefault_p32ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidDefaultP32ID.Value)
        End Get
        Set(value As Integer)
            Me.hidDefaultP32ID.Value = value.ToString
        End Set
    End Property
    Private Property GuidApprove As String
        Get
            Return Me.hidGuidApprove.Value
        End Get
        Set(value As String)
            Me.hidGuidApprove.Value = value
        End Set
    End Property
    
    Private ReadOnly Property CurrentPerson As String
        Get
            Return Me.hidCurPerson_Name.Value
        End Get

    End Property
    Public Property MyDefault_j27ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidDefaultJ27ID.Value)
        End Get
        Set(value As Integer)
            Me.hidDefaultJ27ID.Value = value.ToString
        End Set
    End Property
    Public Property MyDefault_VatRate As Double
        Get
            Return BO.BAS.IsNullNum(Me.hidDefaultVatRate.Value)
        End Get
        Set(value As Double)
            Me.hidDefaultVatRate.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            If Request.Item("clone") = "1" Then
                If Request.Item("pid").IndexOf(",") > 0 Then Server.Transfer("p31_clone.aspx?pids=" & Request.Item("pid")) 'hromadné kopírování záznamů
            End If
            With Master
                .HeaderIcon = "Images/worksheet_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            End With
            InhaleWorksheetSetting()


            InhaleMyDefault()


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
                If Not (Me.CurrentIsScheduler Or Request.Item("clonedate") = "1") Then p31Date.SelectedDate = MyDefault_p31Date
                If Me.p49ID.Value <> "" Then Me.p49ID.Value = "" : Me.p49_record.Text = ""
                With Master.Factory.SysUser
                    If Not (.IsMasterPerson Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Creator)) Then 'není nadřízenou osobu ani nemá právo zapisovat do všech projektů -> tak v kopírovaném záznamu předvyplnit
                        Me.j02ID.Value = .j02ID.ToString
                        Me.j02ID.Text = .PersonDesc
                    End If
                End With
            End If

            If Master.DataPID = 0 Then
                Master.HeaderText = Resources.p31_record.Nadpis
                If Me.CurrentP91ID > 0 Then
                    Master.HeaderText = "Zapsat úkon do faktury | " & Master.Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, Me.CurrentP91ID)
                End If
            End If
            

            If Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo And (Me.TimeFrom.Text <> "" Or Me.TimeUntil.Text <> "") Then
                panT.Visible = True
            End If

            If Not Me.MultiDateInput.Visible Then
                Master.AddToolbarButton("Uložit a kopírovat", "saveandcopy", 1, "Images/saveandcopy.png", True)
            End If
        End If
    End Sub

    Private Sub InhaleMyDefault()
        With Master.Factory.SysUser
            Me.CurrentJ02ID = .j02ID
        End With
        Me.MyDefault_p31Date = Today
        If Request.Item("scheduler") = "1" Then
            Me.CurrentIsScheduler = True
        End If
        If Master.DataPID <> 0 Or Master.IsRecordClone Then
            If Request.Item("edit_approve") = "1" Then Me.GuidApprove = Request.Item("guid_approve") 'editace záznamu vyvolána ze schvalovacího dialogu
            Return
        End If

        Dim intDefP41ID As Integer = BO.BAS.IsNullInt(Request.Item("p41id"))
        Dim intDefP56ID As Integer = BO.BAS.IsNullInt(Request.Item("p56id"))
        If intDefP56ID = 0 And Request.Item("p56code") > "" Then
            Dim cTask As BO.p56Task = Master.Factory.p56TaskBL.LoadByCode(Request.Item("p56code"))
            If Not cTask Is Nothing Then intDefP56ID = cTask.PID Else Master.Notify("Pro předávaný kód úkolu nebyl nalezen záznam.")
        End If
        If Request.Item("p48id") <> "" Then Me.CurrentP48ID = BO.BAS.IsNullInt(Request.Item("p48id"))

        'dál se pokračuje pouze pro nové záznamy
        If Master.DataPID = 0 Then
            If Me.CurrentP48ID > 0 Then
                'překlopení operativního plánu do reality
                Dim cP48 As BO.p48OperativePlan = Master.Factory.p48OperativePlanBL.Load(Me.CurrentP48ID)
                Me.CurrentJ02ID = cP48.j02ID
                Me.j02ID.Value = cP48.j02ID
                Me.j02ID.Text = cP48.Person
                Me.p31Date.SelectedDate = cP48.p48Date
                Me.p31Text.Text = cP48.p48Text
                Me.p41ID.Value = cP48.p41ID.ToString
                Me.p41ID.Text = cP48.ClientAndProject
                Handle_ChangeP41(False, 0)
                If cP48.p34ID <> 0 Then
                    Me.p34ID.SelectedValue = cP48.p34ID.ToString
                    Handle_ChangeP34()
                    If cP48.p32ID <> 0 Then Me.p32ID.SelectedValue = cP48.p32ID.ToString
                End If
                Me.p31Value_Orig.Text = cP48.p48Hours.ToString
                If Not cP48.p48DateTimeFrom Is Nothing Then
                    Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo
                    Me.TimeFrom.Text = cP48.p48TimeFrom : Me.TimeUntil.Text = cP48.p48TimeUntil
                End If
                Return
            End If

            If Request.Item("j02id") <> "" Then
                Dim intJ02ID As Integer = BO.BAS.IsNullInt(Request.Item("j02id"))
                'zápis za jinou osobu
                If intJ02ID > 0 Then
                    Me.CurrentJ02ID = intJ02ID
                End If
            End If
            If Request.Item("guid_approve") <> "" Then
                'zápis vyvolaný z rozhraní schvalování
                Me.GuidApprove = Request.Item("guid_approve")
            End If
            'načíst mnou naposledy zapsaný worksheet záznam
            Dim cRecLast As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadMyLastCreated(True, intDefP41ID)
            If Not cRecLast Is Nothing Then
                With cRecLast
                    Me.MyDefault_p34ID = .p34ID
                    If Master.Factory.j03UserBL.GetUserParam("p31_PreFillP32ID", "1") Then
                        Me.MyDefault_p32ID = .p32ID 'načítat aktivitu z posledního úkonu                    
                        If .p32ManualFeeFlag = 1 Then
                            tdManulFee.Style.Item("display") = "block"  'pevný honorář
                            Dim cP32 As BO.p32Activity = Master.Factory.p32ActivityBL.Load(.p32ID)
                            Me.ManualFee.Value = cP32.p32ManualFeeDefAmount
                        End If
                    End If

                    
                    If DateDiff(DateInterval.Hour, .DateInsert.Value, Now) < 1 Then
                        'do hodiny starý záznam bere jako výchozí datum posledního úkonu + uživatele posledního úkonu
                        Me.MyDefault_p31Date = .p31Date
                    End If
                    If .p33ID = BO.p33IdENUM.Cas Then
                        If .p31HoursEntryFlag = BO.p31HoursEntryFlagENUM.NeniCas Then
                            Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.Hodiny
                        End If
                    Else
                        Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.NeniCas
                    End If

                    Me.MyDefault_VatRate = .p31VatRate_Orig
                End With
            Else
                'uživatel zatím nezapsal žádný worksheet úkon
                Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.Hodiny

            End If
        End If
        If Request.Item("p91id") <> "" Then Me.CurrentP91ID = BO.BAS.IsNullInt(Request.Item("p91id"))

        If Request.Item("p34id") <> "" Then
            'uživatel  požaduje zapisovat napřímo do konkrétního sešitu
            Me.MyDefault_p34ID = BO.BAS.IsNullInt(Request.Item("p34id"))
        End If
        If Request.Item("p32id") <> "" Then
            Me.MyDefault_p32ID = BO.BAS.IsNullInt(Request.Item("p32id"))
        End If
        
        If Request.Item("p31date") <> "" Then
            Me.MyDefault_p31Date = BO.BAS.ConvertString2Date(Request.Item("p31date"))
        End If

        Me.j02ID.Text = Me.CurrentPerson
        Me.p31Date.SelectedDate = Me.MyDefault_p31Date


        If Master.DataPID = 0 And Request.Item("timelineinput") <> "" Then
            InhaleTimelineInput(intDefP41ID)    'zápis času pro hromadně označené dny v timeline
        End If

        If Request.Item("p85id") <> "" Then
            'zakládání záznamu z časovače - uloženo v tempu
            Me.CurrentP85ID = BO.BAS.IsNullInt(Request.Item("p85id"))
            Dim cTempRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(Me.CurrentP85ID)
            With cTempRec
                intDefP41ID = .p85OtherKey1
                Me.p31Text.Text = .p85Message
                Dim cT As New BO.clsTime
                Me.p31Value_Orig.Text = cT.GetTimeFromSeconds(.p85FreeNumber01 / 100)
                Me.MyDefault_p31Date = .p85FreeDate05
                Me.p31Date.SelectedDate = .p85FreeDate05

            End With
        End If
        If Request.Item("p49id") <> "" Then
            'překlopit peněžní rozpočet do úkonu
            Dim cP49 As BO.p49FinancialPlan = Master.Factory.p49FinancialPlanBL.Load(BO.BAS.IsNullInt(Request.Item("p49id")))
            If cP49 Is Nothing Then Master.StopPage("Nelze načíst položku rozpočtu.")
            With cP49
                Me.p41ID.Value = .p41ID
                Me.p41ID.Text = .Client & " - " & .Project
                Handle_ChangeP41(False, 0)
                Me.p34ID.SelectedValue = .p34ID.ToString
                Handle_ChangeP34()
                Me.p32ID.SelectedValue = .p32ID.ToString
                If .p28ID_Supplier <> 0 Then
                    Me.p28ID_Supplier.Value = .p28ID_Supplier.ToString
                    Me.p28ID_Supplier.Text = .SupplierName
                End If
                Me.p31Text.Text = .p49Text
                Me.p31Amount_WithoutVat_Orig.Value = .p49Amount
                Dim b As Boolean = True
                If Not _Sheet Is Nothing Then
                    If _Sheet.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then b = True Else b = False
                End If
                If b Then
                    Me.p31Amount_Vat_Orig.Value = .p49Amount * BO.BAS.IsNullNum(Me.p31VatRate_Orig.Text) / 100
                    Me.p31Amount_WithVat_Orig.Value = BO.BAS.IsNullNum(Me.p31Amount_WithoutVat_Orig.Value) + BO.BAS.IsNullNum(Me.p31Amount_Vat_Orig.Value)
                End If
                Me.p49ID.Value = .PID.ToString
                Me.p49_record.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p49FinancialPlan, .PID) & "</br>"
            End With
        Else
            If intDefP56ID > 0 Then
                Dim cP56 As BO.p56Task = Master.Factory.p56TaskBL.Load(intDefP56ID)
                If Not cP56 Is Nothing Then intDefP41ID = cP56.p41ID
            End If
            If intDefP41ID = 0 And Request.Item("p28id") <> "" Then
                intDefP41ID = FindDefaultP41ID_FromClient(BO.BAS.IsNullInt(Request.Item("p28id")))
            End If

            If intDefP41ID > 0 Then
                Me.p41ID.Value = intDefP41ID.ToString
                Handle_ChangeP41(True, intDefP56ID)
                If Not _Project Is Nothing Then
                    ''p41ID.Text = _Project.FullName
                    p41ID.Text = _Project.ProjectWithMask(Master.Factory.SysUser.j03ProjectMaskIndex)
                End If
            End If
           
            If Request.Item("t1") <> "" And Request.Item("t2") <> "" Then Me.CurrentIsScheduler = True
            If Master.DataPID = 0 And Me.CurrentIsScheduler Then
                Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo   'požadavek na zápis hodin z denního kalendáře, přepnout na čas od/do
                Dim c As New BO.DateTimeByQuerystring(Request.Item("t1"))
                Me.p31Date.SelectedDate = c.DateOnly
                Me.TimeFrom.Text = c.TimeOnly
                c = New BO.DateTimeByQuerystring(Request.Item("t2"))
                Me.TimeUntil.Text = c.TimeOnly

                Dim cT As New BO.clsTime
                Me.p31Value_Orig.Text = cT.ShowAsDec(Me.TimeUntil.Text) - cT.ShowAsDec(Me.TimeFrom.Text)
                Handle_ChangeHoursEntryFlag()

            End If
        End If
        

    End Sub

    Private Function FindDefaultP41ID_FromClient(intP28ID As Integer) As Integer
        If intP28ID = 0 Then Return 0
        Dim mq As New BO.myQueryP41 'najít první možný projekt v rámci klienta
        mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForWorksheetEntry
        mq.p28ID = intP28ID
        Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
        If lisP41.Count > 0 Then
            Return lisP41(0).PID
        Else
            Return 0
        End If
    End Function

    Private Sub InhaleTimelineInput(ByRef intDefP41ID As Integer)
        Dim intYear As Integer = BO.BAS.IsNullInt(Request.Item("year"))
        Dim intMonth As Integer = BO.BAS.IsNullInt(Request.Item("month"))
        Dim a() As String = Split(Request.Item("timelineinput"), ","), dats As New List(Of String), intLastP41ID As Integer, intLastJ02ID As Integer
        For i As Integer = 0 To UBound(a)
            Dim b() As String = Split(a(i), "-")
            Dim intJ02ID As Integer = BO.BAS.IsNullInt(b(1))
            If intJ02ID = 0 Then intJ02ID = Master.Factory.SysUser.j02ID
            Dim intP41ID As Integer = BO.BAS.IsNullInt(b(2))
            Dim d As Date = DateSerial(intYear, intMonth, CInt(b(0)))
            dats.Add(Format(d, "dd.MM.yyyy"))
            If i = 0 Then
                Me.p31Date.SelectedDate = d
                Me.MyDefault_p31Date = d
            End If

            If intLastP41ID <> intP41ID And intLastP41ID > 0 Then
                Master.StopPage("Zapsat časový úkon do více dnů můžete pouze v rámci jednoho projektu.", False)
            End If
            If intLastJ02ID <> intJ02ID And intLastJ02ID > 0 Then
                Master.StopPage("Zapsat časový úkon do více dnů můžete pouze v rámci jedné osoby.", False)
            End If
            intLastP41ID = intP41ID
            intLastJ02ID = intJ02ID
        Next
        If intLastP41ID > 0 Then
            If Request.Item("rozklad") = "3" Then
                'v timeline je rozklad osoba->klient a nikoliv osoba->projekt, v intLastP41ID je p28id
                intDefP41ID = FindDefaultP41ID_FromClient(intLastP41ID)
            Else
                intDefP41ID = intLastP41ID
            End If
        End If
        If intLastJ02ID > 0 Then
            Me.CurrentJ02ID = intLastJ02ID
            Me.j02ID.Text = Me.CurrentPerson
        End If
        If dats.Count = 1 Then
            'pouze jeden datum na vstupu - normální režim bez multi-day vstupu
            Return
        End If
        Me.MultiDateInput.Text = String.Join(", ", dats.Distinct) : Me.MultiDateInput.Visible = True
        lblDate.Text = BO.BAS.OM2(Me.lblDate.Text, dats.Distinct.Count.ToString & "x")
        Me.p31Date.Visible = False
        Me.SharedCalendar.Visible = False
    End Sub
   
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Handle_FF()
            Return
        End If

        Dim cD As BO.p31WorksheetDisposition = Master.Factory.p31WorksheetBL.InhaleRecordDisposition(Master.DataPID)
        If cD.RecordDisposition <> BO.p31RecordDisposition.CanEdit Then
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Owner) Then
                cD.RecordDisposition = BO.p31RecordDisposition.CanEdit  'uživatel má právo vlastníka všech worksheet záznamů v db
            End If
        End If
        If cD.RecordDisposition = BO.p31RecordDisposition._NoAccess Then
            If Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Reader) Then
                cD.RecordDisposition = BO.p31RecordDisposition.CanRead  'právo číst všechny worksheet záznamy v db
            End If
        End If
        Select Case cD.RecordDisposition
            Case BO.p31RecordDisposition._NoAccess
                Master.StopPage("K záznamu nedisponujete oprávněním.")
            Case BO.p31RecordDisposition.CanRead, BO.p31RecordDisposition.CanApprove
                If Not Master.IsRecordClone Then Server.Transfer("p31_record_AA.aspx?pid=" & Master.DataPID.ToString)
            Case BO.p31RecordDisposition.CanEdit, BO.p31RecordDisposition.CanApproveAndEdit
                'zbývá právo k editaci - lze pokračovat dál

            Case Else
                Master.StopPage("Nelze zjistit oprávnění k záznamu.")
        End Select


        Select Case cD.RecordState
            Case BO.p31RecordState._NotExists
                Master.StopPage("Záznam nebyl nalezen v databázi.", True)
            Case BO.p31RecordState.Editing
                'záznam lze editovat
                'lze pokračovat v editaci
            Case Else
                If Request.Item("edit_approve") = "1" Then
                Else
                    'záznam pouze pro čtení
                    If Not Master.IsRecordClone Then Server.Transfer("p31_record_AA.aspx?pid=" & Master.DataPID.ToString)
                End If
                
        End Select

        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        If cRec Is Nothing Then
            Master.StopPage("Záznam nebyl nalezen.", True)
        End If
        With cRec

            If Me.CurrentJ02ID <> .j02ID Then
                Me.CurrentJ02ID = .j02ID

                Handle_ChangeJ02()
            End If

            Me.j02ID.Text = .Person

            If .p41ID <> Me.CurrentP41ID Then
                Me.p41ID.Value = .p41ID.ToString
                Handle_ChangeP41(False, .p56ID)
                If Not _Project Is Nothing Then
                    Me.p41ID.Text = _Project.ProjectWithMask(Master.Factory.SysUser.j03ProjectMaskIndex)
                End If
            Else
                Me.p56ID.SelectedValue = .p56ID.ToString
                
            End If
            If .p34ID <> Me.CurrentP34ID Then
                Me.CurrentP34ID = .p34ID
                Handle_ChangeP34()
            End If
            Me.CurrentP32ID = .p32ID
            If .p32ManualFeeFlag = 1 Then
                tdManulFee.Style.Item("display") = "block" : Me.ManualFee.Value = .p31Amount_WithoutVat_Orig    'pevný honorář
            End If

            Me.p31Text.Text = .p31Text
            Me.p31Date.SelectedDate = .p31Date
            Me.CurrentHoursEntryFlag = .p31HoursEntryFlag
            Handle_ChangeHoursEntryFlag()

            Me.p28ID_Supplier.Value = .p28ID_Supplier.ToString
            Me.p28ID_Supplier.Text = .SupplierName            
            Me.p31Code.Text = .p31Code
            If .j02ID_ContactPerson > 0 Then
                Me.chkBindToContactPerson.Checked = True
                RefreshContactPersonCombo(False, .j02ID_ContactPerson)
            Else
                Me.chkBindToContactPerson.Checked = False
            End If
            If .p72ID_AfterTrimming > BO.p72IdENUM._NotSpecified Then
                Me.chkTrimming.Checked = True
            Else
                Me.chkTrimming.Checked = False
            End If
            RefreshTrimming(cRec)
            Select Case CurrentP33ID
                Case BO.p33IdENUM.Cas
                    Select Case .p31HoursEntryFlag
                        Case BO.p31HoursEntryFlagENUM.Minuty
                            Me.p31Value_Orig.Text = .p31Minutes_Orig.ToString
                        Case BO.p31HoursEntryFlagENUM.PresnyCasOdDo
                            If .IsRecommendedHHMM() Then
                                Me.p31Value_Orig.Text = .p31HHMM_Orig
                            Else
                                Me.p31Value_Orig.Text = .p31Value_Orig.ToString
                            End If
                            ''Me.p31Value_Orig.Text = .p31Value_Orig.ToString
                            Me.TimeFrom.Text = .TimeFrom
                            Me.TimeUntil.Text = .TimeUntil()
                        Case Else
                            If .IsRecommendedHHMM() Then
                                Me.p31Value_Orig.Text = .p31HHMM_Orig
                            Else
                                Me.p31Value_Orig.Text = .p31Value_Orig.ToString
                            End If

                    End Select
                Case BO.p33IdENUM.Kusovnik
                    Me.Units_Orig.Value = .p31Value_Orig
                Case BO.p33IdENUM.PenizeBezDPH
                    Me.p31Amount_WithoutVat_Orig.Value = .p31Amount_WithoutVat_Orig
                    Me.j27ID_Orig.SelectedValue = .j27ID_Billing_Orig.ToString
                Case BO.p33IdENUM.PenizeVcDPHRozpisu
                    Me.p31Amount_WithoutVat_Orig.Value = .p31Amount_WithoutVat_Orig
                    Me.p31Amount_WithVat_Orig.Value = .p31Amount_WithVat_Orig

                    Me.p31VatRate_Orig.SetText(.p31VatRate_Orig.ToString)
                    Me.p31VatRate_Orig.SelectedValue = .p31VatRate_Orig.ToString

                    Me.p31Amount_Vat_Orig.Value = .p31Amount_Vat_Orig
                    Me.j27ID_Orig.SelectedValue = .j27ID_Billing_Orig.ToString
            End Select
            If CurrentP33ID = BO.p33IdENUM.PenizeBezDPH Or CurrentP33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                Me.p31Calc_PieceAmount.Value = cRec.p31Calc_PieceAmount
                Me.p31Calc_Pieces.Value = cRec.p31Calc_Pieces
                Me.p35ID.SelectedValue = cRec.p35ID.ToString
            End If
            If .j19ID > 0 Then
                If Me.j19ID.Items.Count = 0 Then SetupJ19Combo()
                basUI.SelectDropdownlistValue(Me.j19ID, .j19ID.ToString)
            End If
            
            If .p49ID > 0 Then
                Me.p49ID.Value = .p49ID.ToString
                Me.p49_record.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p49FinancialPlan, .p49ID) & "</br>"
            End If


            Master.Timestamp = .Timestamp & " | Vlastník záznamu: <span class='val'>" & .Owner & "</span> | <a href='javascript:changelog()' class='wake_link'>CHANGE-LOG</a>"
            Master.HeaderText = .p34Name & " | " & BO.BAS.FD(.p31Date) & " | " & .Person & " | " & .p41Name
            If .IsClosed Then
                Master.ChangeToolbarSkin("BlackMetroTouch")
            End If
        End With
        tags1.RefreshData(cRec.PID)

    End Sub

    Private Sub Handle_FF()

        Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p31Worksheet, Master.DataPID, BO.BAS.IsNullInt(Me.p34ID.SelectedValue))
        Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p31Worksheet, BO.BAS.IsNullInt(Me.p34ID.SelectedValue))
        ff1.FillData(fields, lisX20X18, "p31Worksheet_FreeField", Master.DataPID)

    End Sub

    Private Sub Handle_ChangeP41(bolTryRun_Handle_P34 As Boolean, intDefP56ID As Integer)
        imgFlag.Visible = False
        If Me.CurrentP41ID = 0 Then
            _Project = Nothing
        Else
            _Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
        End If

        Dim intDefP34ID As Integer = Me.CurrentP34ID
        Dim intDefP32ID As Integer = Me.CurrentP32ID
        Dim intCurP34ID As Integer = 0, intCurP32ID As Integer = intDefP32ID

        If Master.DataPID = 0 Then
            intDefP34ID = Me.MyDefault_p34ID
        End If
        If intDefP32ID = 0 And Master.DataPID = 0 Then
            intDefP32ID = Me.MyDefault_p32ID
        End If
        Me.hidP61ID.Value = ""

        If _Project Is Nothing Then
            Me.p34ID.Clear()
            Me.p32ID.Clear()
            Me.clue_project.Visible = False
        Else
            If _Project.p61ID > 0 Then Me.hidP61ID.Value = _Project.p61ID.ToString
            Me.clue_project.Visible = True
            Me.clue_project.Attributes.Item("rel") = "clue_p41_myworksheet.aspx?pid=" & _Project.PID.ToString & "&j02id=" & Me.CurrentJ02ID.ToString
            Dim intLangIndex As Integer = 0
            If _Project.p87ID > 0 Or _Project.p87ID_Client > 0 Then
                Dim cP87 As BO.p87BillingLanguage = Master.Factory.ftBL.LoadP87(IIf(_Project.p87ID > 0, _Project.p87ID, _Project.p87ID_Client))
                intLangIndex = cP87.p87LangIndex
                If cP87.p87Icon <> "" Then
                    imgFlag.Visible = True
                    imgFlag.ImageUrl = "Images/flags/" & cP87.p87Icon
                    imgFlag.ToolTip = cP87.p87Name
                End If
            Else
                If BO.ASS.GetConfigVal("Implementation") = "hh" Then
                    imgFlag.Visible = True : imgFlag.ImageUrl = "Images/flags/czechrepublic.gif"
                End If
            End If
            Dim intJ02ID As Integer = Me.CurrentJ02ID
            ''If _Project.p41IsEntryP31ByStranger Then intJ02ID = Master.Factory.SysUser.j02ID 'v projektu povoleno zapisovat worksheet za osoby, které nemají roli v projektu
            Dim lisP34 As IEnumerable(Of BO.p34ActivityGroup) = Master.Factory.p34ActivityGroupBL.GetList_WorksheetEntryInProject(_Project.PID, _Project.p42ID, _Project.j18ID, intJ02ID)
            Me.p34ID.DataSource = lisP34
            Me.p34ID.DataBind()
            If Master.DataPID = 0 And bolTryRun_Handle_P34 Then
                If Not Page.IsPostBack And Request.Item("tabqueryflag") <> "" Then
                    Select Case Request.Item("tabqueryflag")
                        Case "time" : lisP34 = lisP34.Where(Function(p) p.p33ID = BO.p33IdENUM.Cas)
                        Case "expense" : lisP34 = lisP34.Where(Function(p) p.p33ID = BO.p33IdENUM.PenizeBezDPH Or p.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu And p.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj)
                        Case "fee" : lisP34 = lisP34.Where(Function(p) p.p33ID = BO.p33IdENUM.PenizeBezDPH Or p.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu And p.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem)
                    End Select
                    For Each c In lisP34
                        If Not Me.p34ID.RadCombo.Items.FindItemByValue(c.PID.ToString) Is Nothing Then intDefP34ID = c.PID : Exit For
                    Next
                End If
                If intDefP34ID = 0 And lisP34.Count > 0 Then
                    intDefP34ID = lisP34(0).PID
                End If
            End If

            SetupP56Combo(_Project.PID, True, intDefP56ID)
            If _Project.p41WorksheetOperFlag = BO.p41WorksheetOperFlagEnum.WithTaskOnly Then
                Me.lblP56ID.CssClass = "lblReq"  'v projektu je povinnost zapisovat přes úkol
            Else
                Me.lblP56ID.CssClass = "lbl"
            End If

            If Not bolTryRun_Handle_P34 Then Return 'dál už se nemá pokračovat


            If Me.p34ID.Rows > 1 And intDefP34ID > 0 Then
                Me.CurrentP34ID = intDefP34ID
            End If
            If Me.CurrentP34ID <> intCurP34ID Then
                Handle_ChangeP34()  'došlo ke změně sešitu -> vyvolat změnu sešitu, aby se naplnila nabídka aktivit
            End If
            If Me.p32ID.Rows > 1 And intDefP32ID > 0 Then
                Me.CurrentP32ID = intDefP32ID
                If Me.CurrentP32ID > 0 And Master.DataPID = 0 Then
                    'výchozí hodnoty aktivity pro nový záznam
                    Dim strDefText As String = ""
                    Dim cP32 As BO.p32Activity = Master.Factory.p32ActivityBL.Load(Me.CurrentP32ID)
                    Select Case intLangIndex
                        Case 0 : strDefText = cP32.p32DefaultWorksheetText
                        Case 1 : strDefText = cP32.p32DefaultWorksheetText_Lang1
                        Case 2 : strDefText = cP32.p32DefaultWorksheetText_Lang2
                        Case 3 : strDefText = cP32.p32DefaultWorksheetText_Lang3
                        Case 4 : strDefText = cP32.p32DefaultWorksheetText_Lang4
                    End Select
                    If strDefText <> "" Then
                        If Me.p31Text.Text = "" Then
                            Me.p31Text.Text = strDefText
                        Else
                            Me.p31Text.Text += vbCrLf & vbCrLf & strDefText
                        End If
                    End If
                    Select Case Me.CurrentP33ID
                        Case BO.p33IdENUM.Cas
                            If cP32.p32Value_Default <> 0 And Me.p31Value_Orig.Text = "" Then
                                Me.p31Value_Orig.Text = cP32.p32Value_Default.ToString
                            End If
                        Case BO.p33IdENUM.Kusovnik
                            If cP32.p32Value_Default <> 0 And BO.BAS.IsNullNum(Me.Units_Orig.Value) = 0 Then
                                Me.Units_Orig.Value = cP32.p32Value_Default
                            End If
                    End Select

                    If cP32.p32IsTextRequired Then Me.lblP31Text.CssClass = "lblReq" Else lblP31Text.CssClass = "lbl"
                End If
            End If


            Me.chkBindToContactPerson.Checked = False : Me.j02ID_ContactPerson.Visible = False
            Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Master.Factory.p30Contact_PersonBL.GetList(_Project.p28ID_Client, _Project.PID, 0)
            If lisP30.Count > 0 Then
                Me.j02ID_ContactPerson.Visible = True
                chkBindToContactPerson.Text = BO.BAS.OM2(chkBindToContactPerson.Text, lisP30.Select(Function(p) p.j02ID).Distinct.Count.ToString)

                Dim intDefj02ID As Integer = _Project.j02ID_ContactPerson_DefaultInWorksheet
                If intDefj02ID = 0 And _Project.p28ID_Client <> 0 Then
                    Dim cP28 As BO.p28Contact = Master.Factory.p28ContactBL.Load(_Project.p28ID_Client)
                    intDefj02ID = cP28.j02ID_ContactPerson_DefaultInWorksheet
                End If
                If intDefj02ID <> 0 Then Me.chkBindToContactPerson.Checked = True
                RefreshContactPersonCombo(True, intDefj02ID)

            End If

            

            
        End If
    End Sub
    Private Sub Handle_ChangeP34()
        If Me.CurrentP34ID = 0 Then
            _Sheet = Nothing
            Me.p32ID.Clear()
          
            Return
        End If

        _Sheet = Master.Factory.p34ActivityGroupBL.Load(Me.CurrentP34ID)
        Dim mq As New BO.myQueryP32
        mq.p34ID = Me.CurrentP34ID
        If Me.hidP61ID.Value <> "" Then mq.p61ID = CInt(Me.hidP61ID.Value)
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()

        panT.Visible = False : panU.Visible = False : panM.Visible = False
        Me.hidP33ID.Value = CInt(_Sheet.p33ID).ToString
        Select Case _Sheet.p33ID
            Case BO.p33IdENUM.Cas
                panT.Visible = True
                If Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.NeniCas Then
                    Me.CurrentHoursEntryFlag = FindLastHoursEntryFlag()
                End If
                Handle_ChangeHoursEntryFlag()
                'If Me.p31Value_Orig.Visible Then Me.p31Value_Orig.Focus()
            Case BO.p33IdENUM.Kusovnik
                panU.Visible = True
                'Me.Units_Orig.Focus()
            Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                panM.Visible = True

                Dim b As Boolean = True
                If _Sheet.p33ID = BO.p33IdENUM.PenizeBezDPH Then
                    b = False
                End If
                Me.p31Amount_WithVat_Orig.Visible = b : Me.lblp31Amount_WithVat_Orig.Visible = b
                Me.p31Amount_Vat_Orig.Visible = b : Me.lblp31Amount_Vat_Orig.Visible = b
                Me.p31VatRate_Orig.Visible = b : Me.lblp31VatRate_Orig.Visible = b
                Me.cmdRecalcVat1.Visible = b

                If Me.j27ID_Orig.Rows = 0 Then
                    Me.j27ID_Orig.DataSource = Master.Factory.ftBL.GetList_J27(New BO.myQuery)
                    Me.j27ID_Orig.DataBind()
                    If Me.MyDefault_j27ID > 0 Then
                        Me.j27ID_Orig.SelectedValue = Me.MyDefault_j27ID.ToString
                    End If
                    Me.p31VatRate_Orig.SetText(Me.MyDefault_VatRate.ToString)
                End If
                SetupVatRateCombo()
                If Me.p35ID.Rows = 0 Then
                    Me.p35ID.DataSource = Master.Factory.ftBL.GetList_P35()
                    Me.p35ID.DataBind()
                End If
        End Select
        If _Sheet.p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava Then
            'aktivita se ručně nezadává
            Me.lblP32ID.Visible = False
            Me.p32ID.Visible = False
        Else
            Me.lblP32ID.Visible = True
            Me.p32ID.Visible = True
        End If
        Handle_FF()
    End Sub

    Private Function FindLastHoursEntryFlag() As BO.p31HoursEntryFlagENUM
        Dim cRecLast As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadMyLastCreated_TimeRecord()
        If Not cRecLast Is Nothing Then
            If cRecLast.p31HoursEntryFlag = BO.p31HoursEntryFlagENUM.NeniCas Then Return BO.p31HoursEntryFlagENUM.Hodiny
            Return cRecLast.p31HoursEntryFlag
        Else
            Return BO.p31HoursEntryFlagENUM.Hodiny
        End If
    End Function

    Private Sub Handle_ChangeHoursEntryFlag()
        Dim bolET As Boolean = False
        Select Case Me.CurrentHoursEntryFlag
            Case BO.p31HoursEntryFlagENUM.Hodiny, BO.p31HoursEntryFlagENUM.PresnyCasOdDo
                lblHours.Text = Resources.p31_record.Hodiny + ":"
                lblHours.ToolTip = Resources.p31_record.Hodiny_Tooltip
                Me.p31Value_Orig.Attributes.Item("onchange") = "handle_hours()"
            Case BO.p31HoursEntryFlagENUM.Minuty
                lblHours.Text = Resources.p31_record.Minuty + ":"
                lblHours.ToolTip = Resources.p31_record.Minuty_Tooltip
                Me.p31Value_Orig.Attributes.Item("onchange") = "handle_minutes()"
        End Select
        If Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo Or Me.p31_default_HoursEntryFlag.Value = "3" Or Me.CurrentIsScheduler Then
            bolET = True
        End If
        lblTimeFrom.Visible = bolET : lblTimeUntil.Visible = bolET
        Me.TimeFrom.Visible = bolET : Me.TimeUntil.Visible = bolET
     

        
    End Sub

   

    Private Sub p41ID_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41ID.AutoPostBack_SelectedIndexChanged
        Handle_ChangeP41(True, 0)
    End Sub

    Private Sub p34ID_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles p34ID.ItemDataBound
        Dim cRec As BO.p34ActivityGroup = CType(e.Item.DataItem, BO.p34ActivityGroup)
        Select Case cRec.p33ID
            Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                If cRec.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
                    e.Item.ForeColor = Drawing.Color.Blue
                Else
                    e.Item.ForeColor = Drawing.Color.Brown
                End If

            Case BO.p33IdENUM.Kusovnik
                e.Item.ForeColor = Drawing.Color.Green
        End Select
    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Handle_ChangeP34()
        Me.p32ID.Focus()
    End Sub

    Private Sub p32ID_ItemDataBound(sender As Object, e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles p32ID.ItemDataBound
        Dim cRec As BO.p32Activity = CType(e.Item.DataItem, BO.p32Activity)
        If Not cRec.p32IsBillable Then
            e.Item.ForeColor = Drawing.Color.Red
        End If
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p31WorksheetBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p31-delete")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.WarningMessage)
            End If
        End With
    End Sub
    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveChanges(False)
    End Sub

    Private Sub SaveChanges(bolSaveAndCopy As Boolean)
        Dim bolEnglish As Boolean = False
        If Page.Culture.IndexOf("Czech") < 0 And Page.Culture.IndexOf("Če") < 0 Then bolEnglish = True
        With Master.Factory.p31WorksheetBL
            Dim cRec As New BO.p31WorksheetEntryInput()
            With cRec
                .SetPID(Master.DataPID)
                .j02ID = Me.CurrentJ02ID
                .p41ID = Me.CurrentP41ID
                .p34ID = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
                .p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                If trTask.Visible Then .p56ID = BO.BAS.IsNullInt(Me.p56ID.SelectedValue)
                .p48ID = Me.CurrentP48ID
                .p28ID_Supplier = BO.BAS.IsNullInt(Me.p28ID_Supplier.Value)
                If Me.chkBindToContactPerson.Checked Then
                    .j02ID_ContactPerson = BO.BAS.IsNullInt(Me.j02ID_ContactPerson.SelectedValue)
                Else
                    .j02ID_ContactPerson = 0
                End If

                If Me.p31Date.IsEmpty Then
                    .p31Date = Today
                Else
                    .p31Date = Me.p31Date.SelectedDate
                End If
                .p31Text = Me.p31Text.Text
                .p31Code = Me.p31Code.Text
                .p49ID = BO.BAS.IsNullInt(Me.p49ID.Value)

                Select Case Me.CurrentP33ID
                    Case BO.p33IdENUM.Cas
                        .p31HoursEntryflag = Me.CurrentHoursEntryFlag
                        .Value_Orig = Me.p31Value_Orig.Text
                        .Value_Orig_Entried = .Value_Orig
                        If chkTrimming.Checked Then
                            .p72ID_AfterTrimming = DirectCast(CInt(Me.p72ID_AfterTrimming.SelectedValue), BO.p72IdENUM)
                            .Value_Trimmed = Me.p31Value_Trimmed.Text
                            If Not .ValidateTrimming(.p72ID_AfterTrimming, .Value_Trimmed) Then
                                Master.Notify(.ErrorMessage, 2)
                                Return
                            End If
                        End If
                        If Me.CurrentHoursEntryFlag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo Or (TimeFrom.Visible And TimeUntil.Visible) Then
                            .TimeFrom = Me.TimeFrom.Text
                            .TimeUntil = Me.TimeUntil.Text
                            .p31HoursEntryflag = BO.p31HoursEntryFlagENUM.PresnyCasOdDo
                        End If
                        If Not .ValidateEntryTime(5, bolEnglish) Then
                            Master.Notify(.ErrorMessage, 2)
                            Return
                        End If
                        .ManualFee = BO.BAS.IsNullNum(Me.ManualFee.Value)

                    Case BO.p33IdENUM.Kusovnik
                        .Value_Orig = BO.BAS.IsNullNum(Me.Units_Orig.Value)
                        .Value_Orig_Entried = .Value_Orig
                        If Not .ValidateEntryKusovnik() Then
                            Master.Notify(.ErrorMessage, 2)
                            Return
                        End If
                    Case BO.p33IdENUM.PenizeBezDPH
                        .Amount_WithoutVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithoutVat_Orig.Value)
                        .Value_Orig_Entried = .Amount_WithoutVat_Orig.ToString
                        .j27ID_Billing_Orig = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
                        .SetAmounts()
                    Case BO.p33IdENUM.PenizeVcDPHRozpisu
                        .Amount_WithoutVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithoutVat_Orig.Value)
                        .Value_Orig_Entried = .Amount_WithoutVat_Orig.ToString
                        .VatRate_Orig = BO.BAS.IsNullNum(Me.p31VatRate_Orig.Text)
                        .j27ID_Billing_Orig = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
                        .Amount_WithVat_Orig = BO.BAS.IsNullNum(Me.p31Amount_WithVat_Orig.Value)
                        .Amount_Vat_Orig = BO.BAS.IsNullNum(Me.p31Amount_Vat_Orig.Value)
                        .SetAmounts()

                End Select
                Select Case Me.CurrentP33ID
                    Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                        .p31Calc_PieceAmount = BO.BAS.IsNullNum(Me.p31Calc_PieceAmount.Value)
                        .p31Calc_Pieces = BO.BAS.IsNullNum(Me.p31Calc_Pieces.Value)
                        .p35ID = BO.BAS.IsNullInt(Me.p35ID.SelectedValue)
                        If Me.j19ID.Visible Then .j19ID = BO.BAS.IsNullInt(Me.j19ID.SelectedValue)
                End Select

            End With

            If Me.MultiDateInput.Visible And Me.MultiDateInput.Text <> "" Then
                Dim dats As New List(Of Date)   'hromadné uložení úkonu do více dnů najednou
                Dim a() As String = Split(Me.MultiDateInput.Text, ",")
                For i = 0 To UBound(a)
                    dats.Add(BO.BAS.ConvertString2Date(Trim(a(i))))
                Next
                If .SaveOrigRecordBatch(dats, cRec, Nothing) Then
                    Master.CloseAndRefreshParent("p31-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.WarningMessage)
                End If
                Return
            End If

            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()
            Dim bolNewRec As Boolean = False
            If cRec.PID = 0 Then bolNewRec = True

            If .SaveOrigRecord(cRec, lisFF) Then
                Master.DataPID = .LastSavedPID
                If Me.CurrentP91ID > 0 Then
                    'přesměrovat úkon rovnou na schvalování
                    Server.Transfer("p31_record_approve.aspx?pid=" & Master.DataPID.ToString & "&p91id=" & Me.CurrentP91ID.ToString, False)
                    Return
                End If
                If Me.GuidApprove <> "" And bolNewRec Then
                    'zařadit úkon do rozpracovaného schvalování
                    Dim c As New BO.p85TempBox()
                    c.p85GUID = Me.GuidApprove
                    c.p85DataPID = Master.DataPID
                    Master.Factory.p85TempBoxBL.Save(c)
                    AddRecord2Approving()
                End If
                If Me.GuidApprove <> "" And Not bolNewRec Then
                    Master.Factory.p31WorksheetBL.UpdateTemp_After_EditOrig(Master.DataPID, Me.GuidApprove) 'záznam otevřený k editaci ze schvalovacího dialogu
                End If
                If tags1.IsNeed2Save Or Master.IsRecordClone Then
                    Master.Factory.o51TagBL.SaveBinding("p31", Master.DataPID, tags1.Geto51IDs())
                End If

                If Not bolNewRec Or ff1.TagsCount > 0 Then
                    If ff1.TagsCount > 0 Then Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p31Worksheet, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs())
                End If
                If Me.CurrentP85ID > 0 Then
                    'odstranit temp záznam předlohy
                    Master.Factory.p85TempBoxBL.Delete(Master.Factory.p85TempBoxBL.Load(Me.CurrentP85ID))
                End If
                If bolSaveAndCopy Then
                    MakeReadyToCopy()
                Else
                    Master.CloseAndRefreshParent("p31-save")
                End If

            Else
                Master.Notify(.ErrorMessage, NotifyLevel.WarningMessage)
            End If
        End With
    End Sub

    Private Sub AddRecord2Approving()
        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        Dim cApprove As New BO.p31WorksheetApproveInput(Master.DataPID, cRec.p33ID)
        With cApprove
            .GUID_TempData = Me.GuidApprove
            .p31Date = cRec.p31Date
            .p71id = BO.p71IdENUM.Schvaleno
            If cRec.p32IsBillable Then
                .p72id = BO.p72IdENUM.Fakturovat
                .Value_Approved_Billing = cRec.p31Value_Orig
                .Value_Approved_Internal = cRec.p31Value_Orig
                Select Case .p33ID
                    Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                        .Rate_Billing_Approved = cRec.p31Rate_Billing_Orig
                        If cRec.p31Rate_Billing_Orig = 0 Then
                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                        End If
                    Case BO.p33IdENUM.PenizeBezDPH
                        If cRec.p31Value_Orig = 0 Then
                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                        End If
                    Case BO.p33IdENUM.PenizeVcDPHRozpisu
                        .VatRate_Approved = cRec.p31VatRate_Orig
                        If cRec.p31Value_Orig = 0 Then
                            .p72id = BO.p72IdENUM.ZahrnoutDoPausalu
                        End If
                End Select
            Else
                .p72id = BO.p72IdENUM.SkrytyOdpis
            End If
        End With
        Master.Factory.p31WorksheetBL.Save_Approving(cApprove, True)
    End Sub

    Private Sub cmdHardRefresh_Click(sender As Object, e As EventArgs) Handles cmdHardRefresh.Click
        Select Case HardRefreshFlag.Value
            Case "p31-setting"
                If Me.CurrentP33ID = BO.p33IdENUM.Cas Then
                    InhaleWorksheetSetting()
                    Handle_ChangeHoursEntryFlag()

                End If
            Case "o23-save", "o23-queue"
                ff1.RefreshDocListByOneDocument(CInt(Me.HardRefreshPID.Value))
                Master.Notify("Vazba na dokument se uloží až po stisknutí tlačítka [Uložit změny].")
            Case "o23-delete"
                ff1.RefreshDocListComplete()
            
            Case "p49-bind"
                Me.p49ID.Value = Me.HardRefreshPID.Value
                Me.p49_record.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p49FinancialPlan, CInt(Me.p49ID.Value)) & "</br>"
                Master.Notify("Vazbu úkonu na rozpočet potvrdíte tlačítkem [Uložit změny].")
        End Select


        Me.HardRefreshFlag.Value = ""
        Me.HardRefreshPID.Value = ""
    End Sub

    
    Private Sub p32ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p32ID.NeedMissingItem
        If Master.DataPID = 0 Then Return
        Dim cRec As BO.p32Activity = Master.Factory.p32ActivityBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.p32Name
        End If
    End Sub

    Private Sub j27ID_Orig_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j27ID_Orig.NeedMissingItem
        Dim cRec As BO.j27Currency = Master.Factory.ftBL.LoadJ27(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.j27Code
        End If
    End Sub

    Private Sub j02ID_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles j02ID.AutoPostBack_SelectedIndexChanged
        Me.CurrentJ02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
        Handle_ChangeJ02()
    End Sub

    Private Sub Handle_ChangeJ02()
        Handle_ChangeP41(True, BO.BAS.IsNullInt(Me.p56ID.SelectedValue))

    End Sub

    Private Sub j27ID_Orig_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles j27ID_Orig.SelectedIndexChanged
        SetupVatRateCombo()
    End Sub

    Private Sub SetupVatRateCombo()

        Dim intJ27ID As Integer = BO.BAS.IsNullInt(Me.j27ID_Orig.SelectedValue)
        Dim lis As IEnumerable(Of BO.p53VatRate) = Master.Factory.p53VatRateBL.GetList(New BO.myQuery).Where(Function(p) p.j27ID = intJ27ID)
        Me.p31VatRate_Orig.DataSource = lis
        Me.p31VatRate_Orig.DataBind()
    End Sub

    Private Sub p31_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

        Me.panTrimming.Visible = panT.Visible
        If Not _Sheet Is Nothing And panM.Visible Then
            Dim bolVydaj As Boolean = False
            If _Sheet.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Vydaj Then bolVydaj = True

            lblSupplier.Visible = bolVydaj : Me.p28ID_Supplier.Visible = bolVydaj : p31Code.Visible = bolVydaj : lblCode.Visible = bolVydaj : Me.j19ID.Visible = bolVydaj
            If bolVydaj And Me.j19ID.Items.Count = 0 Then SetupJ19Combo()
        End If
        If _Project Is Nothing And p41ID.Value <> "" Then _Project = Master.Factory.p41ProjectBL.Load(BO.BAS.IsNullInt(Me.p41ID.Value))
        If Not _Project Is Nothing Then
            Dim cP42 As BO.p42ProjectType = Master.Factory.p42ProjectTypeBL.Load(_Project.p42ID)
           
            If panM.Visible Then
                panP49.Visible = cP42.p42IsModule_p45
            Else
                panP49.Visible = False
            End If
        End If
        If Me.panP49.Visible Then
            If Me.p49ID.Value <> "" Then cmdClearP49ID.Visible = True Else Me.cmdClearP49ID.Visible = False
        End If
    End Sub

    

    Private Sub SetupP56Combo(intP41ID As Integer, bolSilent As Boolean, intDefP56ID As Integer)
        Me.trTask.Visible = False
        If intP41ID = 0 And Not bolSilent Then
            Master.Notify("Seznam úkolů je možné zobrazit až po výběru projektu.")
            Return
        End If
        Dim mq As New BO.myQueryP56
        mq.p41ID = intP41ID
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForWorksheetEntry
        mq.j02ID_ExplicitQueryFor = Me.CurrentJ02ID

        Me.p56ID.DataSource = Master.Factory.p56TaskBL.GetList(mq)
        Me.p56ID.DataBind()

        If intDefP56ID > 0 Then Me.p56ID.SelectedValue = intDefP56ID.ToString

      
        If Me.p56ID.Rows > 1 Then
            trTask.Visible = True
        End If

        If Me.p56ID.Rows <= 1 And Not bolSilent Then
            Master.Notify(String.Format("Pro [{0}] a projekt [{1}] není dostupný ani jeden otevřený úkol k vykazování.", Master.Factory.SysUser.Person, _Project.FullName))
        End If

    End Sub

    

    Private Sub InhaleWorksheetSetting()
        Dim lisPars As New List(Of String)
        lisPars.Add("p31_default_HoursEntryFlag")
        lisPars.Add("p31_HoursInputInterval")
        lisPars.Add("p31_HoursInputFormat")
        lisPars.Add("p31_TimeInputInterval")
        lisPars.Add("p31_TimeInput_Start")
        lisPars.Add("p31_TimeInput_End")
        lisPars.Add("p31_PreFillP32ID")

        With Master.Factory.j03UserBL
            .InhaleUserParams(lisPars)
            Me.p31_default_HoursEntryFlag.Value = .GetUserParam("p31_default_HoursEntryFlag", "1")
            Me.hidHoursEntryFlag.Value = Me.p31_default_HoursEntryFlag.Value
            Dim intStart As Integer = CInt(.GetUserParam("p31_HoursInputInterval", "30"))
            Dim s As String = "", cT As New BO.clsTime, strFormat As String = .GetUserParam("p31_HoursInputFormat", "dec")
            Dim intKratHodin As Integer = 4
            If intStart = 60 Then intKratHodin = 10
            If intStart = 30 Then intKratHodin = 8
            If intStart = 5 Or intStart = 10 Then intKratHodin = 3
            For i As Integer = intStart To intKratHodin * 60 Step intStart
                If i > intStart Then
                    s += ","
                End If
                If strFormat = "hhmm" Then
                    s += "'" & cT.ShowAsHHMM((CDbl(i) / 60).ToString) & "'"
                Else
                    If Me.p31_default_HoursEntryFlag.Value = "2" Then
                        s += "'" & (CDbl(i)).ToString & "'"    'čas se zadává v minutách
                    Else
                        s += "'" & (CDbl(i) / 60).ToString & "'"    'čas se zadává v hodinách
                    End If

                End If
            Next
            ViewState("hours_offer") = s
            'nabídka času od-do
            intStart = CInt(.GetUserParam("p31_TimeInput_Start", "8")) : s = ""
            Dim intEnd As Integer = CInt(.GetUserParam("p31_TimeInput_End", "19"))
            Dim intInterval As Integer = CInt(.GetUserParam("p31_TimeInputInterval", "30"))
            For i As Integer = intStart To intEnd
                For j As Integer = 0 To 59 Step intInterval
                    If s <> "" Then s += ","
                    s += "'" & Right("0" & i.ToString, 2) & ":" & Right("0" & j.ToString, 2) & "'"
                    If i = intEnd Then Exit For
                Next
            Next
            ViewState("times_offer") = s
        End With
    End Sub

    Private Sub cmdClearP49ID_Click(sender As Object, e As EventArgs) Handles cmdClearP49ID.Click
        Me.p49ID.Value = "" : Me.p49_record.Text = ""
        Master.Notify("Vyčištění vazby na rozpočet je třeba potvrdit tlačítkem [Uložit změny].")
    End Sub

    Private Sub chkBindToContactPerson_CheckedChanged(sender As Object, e As EventArgs) Handles chkBindToContactPerson.CheckedChanged
        RefreshContactPersonCombo(False, 0)

    End Sub
    
    Private Sub RefreshContactPersonCombo(bolSilent As Boolean, intDefJ02ID As Integer)
        
        Me.j02ID_ContactPerson.Visible = False
        If Not Me.chkBindToContactPerson.Checked Then
            Me.j02ID_ContactPerson.DataSource = Nothing
            Me.j02ID_ContactPerson.DataBind()
            Return
        End If
        If Me.CurrentP41ID = 0 Then
            If Not bolSilent Then Master.Notify("Chybí projekt.")
            Return
        End If
        If _Project Is Nothing Then _Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)

        Dim mq As New BO.myQueryJ02
        mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
        mq.p41ID = _Project.PID
        ''If _Project.p28ID_Client > 0 Then mq.p28ID = _Project.p28ID_Client Else mq.p41ID = _Project.PID

        Dim lisJ02 As List(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mq).ToList
        If lisJ02.Count = 0 And _Project.p28ID_Client <> 0 Then
            mq.p41ID = 0 : mq.p28ID = _Project.p28ID_Client
        End If
        If intDefJ02ID > 0 And lisJ02.Where(Function(p) p.PID = intDefJ02ID).Count = 0 Then
            Dim c As BO.j02Person = Master.Factory.j02PersonBL.Load(intDefJ02ID)
            If Not c Is Nothing Then lisJ02.Add(c)
        End If
        If lisJ02.Count = 0 Then
            If Not bolSilent Then Master.Notify(String.Format(Resources.p31_record.NejsouZavedenyKontaktniOsoby, _Project.p41Name, _Project.Client))
        Else

            Me.j02ID_ContactPerson.DataSource = lisJ02
            Me.j02ID_ContactPerson.DataBind()
            If intDefJ02ID > 0 Then
                basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson, intDefJ02ID.ToString)
            End If
            Me.j02ID_ContactPerson.Visible = True
        End If
    End Sub

    Private Sub chkTrimming_CheckedChanged(sender As Object, e As EventArgs) Handles chkTrimming.CheckedChanged
        RefreshTrimming(Nothing)
    End Sub
    Private Sub RefreshTrimming(cRec As BO.p31Worksheet)
        Me.p72ID_AfterTrimming.Visible = Me.chkTrimming.Checked
        Me.lblValueTrimmed.Visible = Me.chkTrimming.Checked
        Me.p31Value_Trimmed.Visible = Me.chkTrimming.Checked
        If Not Me.chkTrimming.Checked Then Return

        If Not cRec Is Nothing Then
            If cRec.p72ID_AfterTrimming > BO.p72IdENUM._NotSpecified Then basUI.SelectRadiolistValue(Me.p72ID_AfterTrimming, CInt(cRec.p72ID_AfterTrimming).ToString)
            If cRec.p72ID_AfterTrimming = BO.p72IdENUM.Fakturovat Then
                If cRec.IsRecommendedHHMM() Then
                    Me.p31Value_Trimmed.Text = cRec.p31HHMM_Trimmed
                Else
                    Me.p31Value_Trimmed.Text = CStr(cRec.p31Value_Trimmed)
                End If
            End If
        End If
        Select Case Me.p72ID_AfterTrimming.SelectedValue
            Case "4"
                Me.p31Value_Trimmed.Enabled = True
                Dim cT As New BO.clsTime
                If cT.ShowAsDec(Me.p31Value_Trimmed.Text) = 0 Then
                    Me.p31Value_Trimmed.Text = Me.p31Value_Orig.Text
                End If
            Case Else
                Me.p31Value_Trimmed.Text = "0"
                Me.p31Value_Trimmed.Enabled = False
        End Select
    End Sub

    Private Sub p72ID_AfterTrimming_SelectedIndexChanged(sender As Object, e As EventArgs) Handles p72ID_AfterTrimming.SelectedIndexChanged
        RefreshTrimming(Nothing)
    End Sub

    
    Private Sub cmdRecalcVat1_Click(sender As Object, e As ImageClickEventArgs) Handles cmdRecalcVat1.Click
        Dim n1 As Double = BO.BAS.IsNullNum(Me.p31Amount_WithVat_Orig.Value)
        Dim n2 As Double = 1 + BO.BAS.IsNullNum(Me.p31VatRate_Orig.Text) / 100
        If n2 <> 0 And BO.BAS.IsNullNum(Me.p31VatRate_Orig.Text) <> 0 Then
            Dim n3 As Double = n1 / n2
            Me.p31Amount_WithoutVat_Orig.Value = n3
            Me.p31Amount_Vat_Orig.Value = n1 - n3
        Else
            Me.p31Amount_WithoutVat_Orig.Value = n1
            Me.p31Amount_Vat_Orig.Value = 0
        End If
        'Master.Notify(Me.p31Amount_WithoutVat_Orig.Value)
    End Sub

    Private Sub MakeReadyToCopy()
        Server.Transfer("p31_record.aspx?pid=" & Master.DataPID.ToString & "&clone=1&clonedate=1", False)
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "saveandcopy" Then
            SaveChanges(True)
        End If
    End Sub
    Private Sub SetupJ19Combo()
        Me.j19ID.DataSource = Master.Factory.ftBL.GetList_j19(New BO.myQuery)
        Me.j19ID.DataBind()
        Me.j19ID.Items.Insert(0, "")
    End Sub

    Private Sub p56ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p56ID.NeedMissingItem

        Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.NameWithTypeAndCode
        End If

       
    End Sub
End Class