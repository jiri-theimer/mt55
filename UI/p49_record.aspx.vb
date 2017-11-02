Public Class p49_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p49_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public Property CurrentP45ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p45ID.Value)
        End Get
        Set(value As Integer)
            Me.p45ID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p41ID.Value)
        End Get
        Set(value As Integer)
            Me.p41ID.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .HeaderIcon = "Images/finplan_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Záznam položky peněžního rozpočtu"
                Me.CurrentP45ID = BO.BAS.IsNullInt(Request.Item("p45id"))
                If Me.CurrentP45ID = 0 Then
                    If .DataPID <> 0 Then
                        Me.CurrentP45ID = .Factory.p49FinancialPlanBL.Load(.DataPID).p45ID
                    Else
                        If Request.Item("p41id") <> "" Then
                            Dim cP45 As BO.p45Budget = .Factory.p45BudgetBL.LoadByProject(CInt(Request.Item("p41id")))
                            If cP45 Is Nothing Then .StopPage("Tento projekt nemá založený rozpočet!")
                            Me.CurrentP45ID = cP45.PID
                        Else
                            Server.Transfer("select_project.aspx?oper=createp49")
                        End If
                    End If
                End If
                If Me.CurrentP45ID = 0 Then .StopPage("p45id is missing")
            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.j27ID.DataSource = Master.Factory.ftBL.GetList_J27(New BO.myQuery)
        Me.j27ID.DataBind()
        Me.j27ID.SelectedValue = Master.Factory.x35GlobalParam.j27ID_Invoice.ToString

        Me.p34ID.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery).Where(Function(p) p.p33ID = BO.p33IdENUM.PenizeBezDPH Or p.p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu)
        Me.p34ID.DataBind()

        Dim cP45 As BO.p45Budget = Master.Factory.p45BudgetBL.Load(Me.CurrentP45ID)
        Me.CurrentP41ID = cP45.p41ID
        Me.p45Name.Text = cP45.VersionWithName
       

        Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
        Me.p45Name.Text += " | " & cP41.FullName

        Dim cDisp As BO.p41RecordDisposition = Master.Factory.p41ProjectBL.InhaleRecordDisposition(cP41)
        If Not cDisp.p45_Owner Then
            Master.StopPage("Nedisponujete oprávněním pro úpravu projektového rozpočtu.")
        End If

        If Master.DataPID = 0 Then
            Me.month1.SelectedYear = Year(cP45.p45PlanFrom)
            Me.month1.SelectedMonth = Month(cP45.p45PlanUntil)

            Dim cRecLast As BO.p49FinancialPlan = Master.Factory.p49FinancialPlanBL.LoadMyLastCreated(cP45.PID)
            If Not cRecLast Is Nothing Then
                With cRecLast
                    Me.j27ID.SelectedValue = .j27ID.ToString
                    Me.p34ID.SelectedValue = .p34ID.ToString
                    Handle_ChangeP34ID(.p34ID)
                    If .p32ID <> 0 Then
                        Me.p32ID.SelectedValue = .p32ID.ToString
                    End If
                End With
            End If
            Return
        End If

        Dim cRec As BO.p49FinancialPlan = Master.Factory.p49FinancialPlanBL.Load(Master.DataPID)
        With cRec
            Me.j02ID.Value = .j02ID.ToString
            Me.j02ID.Text = .Person
            Me.j27ID.SelectedValue = .j27ID.ToString
            Me.p34ID.SelectedValue = .p34ID.ToString
            Handle_ChangeP34ID(.p34ID)
            Me.p32ID.SelectedValue = .p32ID.ToString
            Me.p28ID_Supplier.Value = .p28ID_Supplier.ToString
            Me.p28ID_Supplier.Text = .SupplierName
            Me.p49Amount.Value = .p49Amount
            Me.p49Text.Text = .p49Text
            Me.month1.SelectedYear = Year(.p49DateFrom)
            Me.month1.SelectedMonth = Month(.p49DateFrom)
            Master.Timestamp = .Timestamp
        End With
    End Sub
    Private Sub Handle_ChangeP34ID(intP34ID As Integer)
        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(intP34ID)
        If cRec.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
            Me.p28ID_Supplier.Visible = False
            Me.p28ID_Supplier.Value = ""
        Else
            Me.p28ID_Supplier.Visible = True
        End If
        lblSupplier.Visible = Me.p28ID_Supplier.Visible
        Dim mq As New BO.myQueryP32
        mq.p34ID = intP34ID
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()

    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p49FinancialPlanBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p49-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p49FinancialPlanBL
            Dim cRec As BO.p49FinancialPlan = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p49FinancialPlan)
            cRec.p45ID = Me.CurrentP45ID
            cRec.j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
            cRec.p34ID = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
            cRec.p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
            cRec.p28ID_Supplier = BO.BAS.IsNullInt(Me.p28ID_Supplier.Value)
            cRec.j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
            cRec.p49DateFrom = Me.month1.SelectedDateFrom
            cRec.p49DateUntil = Me.month1.SelectedDateUntil
            cRec.p49Amount = BO.BAS.IsNullNum(Me.p49Amount.Value)
            cRec.p49Text = Me.p49Text.Text
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p49-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Handle_ChangeP34ID(BO.BAS.IsNullInt(Me.p34ID.SelectedValue))
    End Sub
End Class