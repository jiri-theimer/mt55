Public Class p45_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p45_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP41ID.Value)
        End Get
        Set(value As Integer)
            hidP41ID.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))    'p45id
            End With
            Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))


            Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
            If Not cRec.p41PlanFrom Is Nothing Then Me.p45PlanFrom.SelectedDate = cRec.p41PlanFrom Else Me.p45PlanFrom.SelectedDate = DateSerial(Year(Now), Month(Now), 1).AddMonths(1)
            If Not cRec.p41PlanUntil Is Nothing Then Me.p45PlanUntil.SelectedDate = cRec.p41PlanUntil

            Dim cDisp As BO.p41RecordDisposition = Master.Factory.p41ProjectBL.InhaleRecordDisposition(cRec)
            If Not cDisp.p45_Owner Then
                Master.StopPage("V tomto projektu nedisponujete oprávněním k definici rozpočtu.")
            End If

            RefreshRecord()
            Master.HeaderText = "Hlavička rozpočtu projektu | " & Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)

            If Request.Item("clone") = "1" Then
                Me.hidClonePID.Value = Master.DataPID.ToString
                Master.DataPID = 0
                panCreateClone.Visible = True
                Master.HeaderText = "Zkopírovat rozpočet do nového"

            End If

        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            lblVersionIndex.Visible = False
            Me.chkMakeCurrentAsFirstVersion.Checked = True
            Return
        End If
        Dim cRec As BO.p45Budget = Master.Factory.p45BudgetBL.Load(Master.DataPID)
        With cRec
            Me.CurrentP41ID = .p41ID
            Me.p45PlanFrom.SelectedDate = .p45PlanFrom
            Me.p45PlanUntil.SelectedDate = .p45PlanUntil
            Me.p45Name.Text = .p45Name
            Me.p45VersionIndex.Text = .p45VersionIndex.ToString
            Me.chkMakeCurrentAsFirstVersion.Checked = Not .IsClosed
            If .IsClosed Then
                Master.ChangeToolbarSkin("BlackMetroTouch")
            End If
        End With

        
        ''Dim lis As IEnumerable(Of BO.p45Budget) = Master.Factory.p45BudgetBL.GetList(Master.DataPID)


    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p45BudgetBL
            If .Delete(Master.DataPID) Then
                Master.CloseAndRefreshParent("p45-delete")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With

    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
      
        With Master.Factory.p45BudgetBL
            Dim cRec As BO.p45Budget = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p45Budget)
            If Not Me.p45PlanFrom.IsEmpty Then cRec.p45PlanFrom = Me.p45PlanFrom.SelectedDate
            If Not Me.p45PlanUntil.IsEmpty Then cRec.p45PlanUntil = Me.p45PlanUntil.SelectedDate
            cRec.p45Name = Me.p45Name.Text
            cRec.p41ID = Me.CurrentP41ID
            If Not Me.chkMakeCurrentAsFirstVersion.Checked Then
                cRec.ValidUntil = Now.AddMinutes(-1)
                If cRec.ValidFrom > cRec.ValidUntil Then cRec.ValidFrom = cRec.ValidUntil.AddMinutes(-1)
            Else
                cRec.ValidUntil = DateSerial(3000, 1, 1)
            End If
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                If BO.BAS.IsNullInt(Me.hidClonePID.Value) <> 0 Then
                    'kopírovat rozpočet
                    .CloneBudget(BO.BAS.IsNullInt(Me.hidClonePID.Value), Master.DataPID, chkCloneP49.Checked, chkCloneP46.Checked, chkCloneP47.Checked)
                End If
                If Me.chkMakeCurrentAsFirstVersion.Checked Then
                    If Not .MakeActualVersion(Master.DataPID) Then
                        Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                    End If
                End If
                Master.CloseAndRefreshParent("p45-save")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    

    
End Class