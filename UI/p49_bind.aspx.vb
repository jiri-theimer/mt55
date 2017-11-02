Imports Telerik.Web.UI

Public Class p49_bind
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP41ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP41ID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentP34ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP34ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP34ID.Value = value.ToString
        End Set
    End Property
    Public Property CurrentP45ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP45ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP45ID.Value = value.ToString
        End Set
    End Property
    Private Sub p49_bind_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
            Me.CurrentP34ID = BO.BAS.IsNullInt(Request.Item("p34id"))
            With Master
                If Me.CurrentP41ID = 0 Or Me.CurrentP34ID = 0 Then .StopPage("p41id or p34id missing...")
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))    'p49ID
                .HeaderText = "Spárovat peněžní výdaj/odměnu s položkou v rozpočtu | " & .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)

                .Factory.j03UserBL.InhaleUserParams("p49_bind-IncludeExtended")
                Dim cRec As BO.p45Budget = .Factory.p45BudgetBL.LoadByProject(Me.CurrentP41ID)
                If cRec Is Nothing Then .StopPage("Vybraný projekt nemá otevřený rozpočet.")
                Me.CurrentP45ID = cRec.PID
                Me.Budget.Text = cRec.VersionWithName

                Dim cP34 As BO.p34ActivityGroup = .Factory.p34ActivityGroupBL.Load(Me.CurrentP34ID)
                Me.Sheet.Text = cP34.p34Name
            End With
            With Master.Factory.j03UserBL
                Me.chkIncludeExtended.Checked = BO.BAS.BG(.GetUserParam("p49_bind-IncludeExtended", "1"))

            End With

            SetupGrid()


        End If
    End Sub


    Private Sub SetupGrid()
        With grid1
            .ClearColumns()
            .radGridOrig.ShowFooter = False
            Dim strGroupNamePlan As String = ""
            If Me.chkIncludeExtended.Checked Then
                .radGridOrig.MasterTableView.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                strGroupNamePlan = "plan"
                Dim group As New Telerik.Web.UI.GridColumnGroup
                .radGridOrig.MasterTableView.ColumnGroups.Add(group)
                group.Name = "plan" : group.HeaderText = "Rozpočet"
                group = New Telerik.Web.UI.GridColumnGroup
                .radGridOrig.MasterTableView.ColumnGroups.Add(group)
                group.Name = "real" : group.HeaderText = "Vykázaná realita"
            End If
            

            .AddColumn("Period", "Měsíc", , , , , , , , strGroupNamePlan)
            '.AddColumn("p34Name", "Sešit")
            .AddColumn("p32Name", "Aktivita", , , , , , , , strGroupNamePlan)
            '.AddColumn("p85FreeText01", "Osoba")
            .AddColumn("SupplierName", "Dodavatel", , , , , , , , strGroupNamePlan)
            .AddColumn("p49Text", "Text", , , , , , , , strGroupNamePlan)
            .AddColumn("p49Amount", "Částka", BO.cfENUM.Numeric, , , , , , , strGroupNamePlan)
            .AddColumn("j27Code", "Měna", , , , , , , , strGroupNamePlan)
            If Me.chkIncludeExtended.Checked Then
                .AddColumn("p31Date", "Datum", BO.cfENUM.DateOnly, , , , , , , "real")
                .AddColumn("p31Amount_WithoutVat_Orig", "Částka", BO.cfENUM.Numeric, , , , , , , "real")
                .AddColumn("p31Code", "Kód dokladu", , , , , , , , "real")
                .AddColumn("p31Count", "Počet", BO.cfENUM.Numeric0, , , , , , , "real")
            End If
        End With
        ''With gridP49.radGridOrig.MasterTableView
        ''    .GroupByExpressions.Clear()
        ''    .ShowGroupFooter = False
        ''    Dim GGE As New GridGroupByExpression
        ''    Dim fld As New GridGroupByField
        ''    fld.FieldName = "p85FreeText02"
        ''    fld.HeaderText = "Sešit"

        ''    GGE.SelectFields.Add(fld)
        ''    GGE.GroupByFields.Add(fld)

        ''    .GroupByExpressions.Add(GGE)
        ''End With
    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim mq As New BO.myQueryP49
        mq.p34ID = Me.CurrentP34ID
        mq.p45ID = Me.CurrentP45ID
        Dim lis As IEnumerable(Of BO.p49FinancialPlanExtended) = Master.Factory.p49FinancialPlanBL.GetList_Extended(mq, Me.CurrentP41ID)
        If Not Me.chkIncludeExtended.Checked Then lis = lis.Where(Function(p) p.p31ID = 0)
        grid1.DataSource = lis



    End Sub

    Private Sub chkIncludeExtended_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeExtended.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("p49_bind-IncludeExtended", BO.BAS.GB(Me.chkIncludeExtended.Checked))
        Server.Transfer("p49_bind.aspx?p41id=" & Me.CurrentP41ID.ToString & "&p34id=" & Me.CurrentP34ID.ToString)
    End Sub
End Class