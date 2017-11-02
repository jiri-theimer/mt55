Public Class p45_project_p49
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p45_project_p49_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public Property CurrentP45ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP45ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP45ID.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = Request.Item("guid")
            If ViewState("guid") = "" Then Master.StopPage("guid missing")
            Me.CurrentP45ID = BO.BAS.IsNullInt(Request.Item("p45id"))
            With Master
                .HeaderIcon = "Images/finplan_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Položka peněžního výdaje/příjmu rozpočtu"
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

        If Me.CurrentP45ID <> 0 Then
            Dim cP45 As BO.p45Budget = Master.Factory.p45BudgetBL.Load(Me.CurrentP45ID)
            Me.month1.SelectedYear = Year(cP45.p45PlanFrom)
            Me.month1.SelectedMonth = Month(cP45.p45PlanFrom)
        End If
       
        If Master.DataPID = 0 Then
            Dim cRecLast As BO.p49FinancialPlan = Master.Factory.p49FinancialPlanBL.LoadMyLastCreated(Me.CurrentP45ID)
            If Not cRecLast Is Nothing Then
                With cRecLast
                    Me.j27ID.SelectedValue = .j27ID.ToString
                    Me.month1.SelectedYear = Year(.p49DateUntil)
                    Me.month1.SelectedMonth = Month(.p49DateFrom)
                    Me.p34ID.SelectedValue = .p34ID.ToString
                    Handle_ChangeP34ID(.p34ID)
                    If .p32ID <> 0 Then
                        Me.p32ID.SelectedValue = .p32ID.ToString
                    End If
                End With
            End If
            Return
        End If
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(Master.DataPID)
        With cRec
            Me.p28ID_Supplier.Value = .p85OtherKey5.ToString
            Me.p28ID_Supplier.Text = .p85FreeText05
            Me.j02ID.Value = .p85OtherKey1.ToString
            Me.j02ID.Text = .p85FreeText01
            Me.j27ID.SelectedValue = .p85OtherKey4.ToString
            Me.p34ID.SelectedValue = .p85OtherKey2.ToString
            Handle_ChangeP34ID(.p85OtherKey2)
            Me.p32ID.SelectedValue = .p85OtherKey3.ToString
            Me.p49Amount.Value = .p85FreeFloat01
            Me.p49Text.Text = .p85Message
            Me.month1.SelectedYear = Year(.p85FreeDate01)
            Me.month1.SelectedMonth = Month(.p85FreeDate01)

            
        End With
    End Sub
    Private Sub Handle_ChangeP34ID(intP34ID As Integer)
        Dim mq As New BO.myQueryP32
        mq.p34ID = intP34ID
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()

        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(intP34ID)
        If cRec.p34IncomeStatementFlag = BO.p34IncomeStatementFlagENUM.Prijem Then
            p28ID_Supplier.Visible = False : lblSupplier.Visible = False
        Else
            p28ID_Supplier.Visible = True : lblSupplier.Visible = True
        End If
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(Master.DataPID)
        If cRec.p85DataPID <> 0 Then
            Dim mq As New BO.myQueryP49
            mq.AddItemToPIDs(cRec.p85DataPID)
            Dim lis As IEnumerable(Of BO.p49FinancialPlanExtended) = Master.Factory.p49FinancialPlanBL.GetList_Extended(mq)
            If lis(0).p31ID <> 0 Then
                Master.Notify("Záznam rozpočtu má již vazbu na reálně vykázaný worksheet úkon.", NotifyLevel.WarningMessage)
                Return
            End If
        End If
        With Master.Factory.p85TempBoxBL
            If .Delete(cRec) Then
                Master.CloseAndRefreshParent("p49-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Handle_ChangeP34ID(BO.BAS.IsNullInt(Me.p34ID.SelectedValue))
    End Sub
    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p85TempBoxBL
            Dim cRec As BO.p85TempBox = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p85TempBox)
            Dim cP34 As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(BO.BAS.IsNullInt(Me.p34ID.SelectedValue))
            With cRec
                .p85GUID = ViewState("guid")
                .p85Prefix = "p49"
                .p85OtherKey1 = BO.BAS.IsNullInt(Me.j02ID.Value)
                .p85OtherKey2 = cP34.PID
                .p85OtherKey3 = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                .p85OtherKey4 = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
                .p85OtherKey5 = BO.BAS.IsNullInt(Me.p28ID_Supplier.Value)
                .p85OtherKey6 = cP34.p34IncomeStatementFlag
                .p85FreeFloat01 = BO.BAS.IsNullNum(Me.p49Amount.Value)
                .p85Message = Me.p49Text.Text
                .p85FreeDate01 = Me.month1.SelectedDateFrom
                .p85FreeDate02 = Me.month1.SelectedDateUntil
                .p85FreeText01 = Me.j02ID.Text
                .p85FreeText02 = Me.p34ID.Text
                .p85FreeText03 = Me.p32ID.Text
                .p85FreeText04 = Me.j27ID.Text
                .p85FreeText05 = Me.p28ID_Supplier.Text
                .p85FreeText06 = Year(.p85FreeDate01).ToString & "/" & Month(.p85FreeDate01).ToString

            End With


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p49-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p45_project_p49_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

    End Sub
End Class