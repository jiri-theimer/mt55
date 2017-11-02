Public Class p40_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p40_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p40_record"
    End Sub
    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP41ID.Value)
        End Get
        Set(value As Integer)
            Me.hidP41ID.Value = value.ToString
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.CurrentP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
            With Master
                If Me.CurrentP41ID = 0 Then
                    .StopPage("Na vstupu chybí ID projektu (p41id).")
                End If
                .HeaderIcon = "Images/worksheet_recurrence_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Opakovaná odměna/paušál/úkon | " & .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.CurrentP41ID)


            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.p34ID.FillData(Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery), True)
        Me.x15ID.FillData(Master.Factory.ftBL.GetList_X15(New BO.myQuery), False)
        Me.j27ID.FillData(Master.Factory.ftBL.GetList_J27(New BO.myQuery), True)

        Dim mq As New BO.myQueryP56
        mq.p41ID = Me.CurrentP41ID
        mq.Closed = BO.BooleanQueryMode.NoQuery
        mq.IsRecurrenceMother = BO.BooleanQueryMode.TrueQuery
        Me.p56ID.DataSource = Master.Factory.p56TaskBL.GetList(mq)
        Me.p56ID.DataBind()

        If Master.DataPID = 0 Then
            Me.j02ID.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID.Text = Master.Factory.SysUser.Person
            Me.j27ID.SelectedValue = Master.Factory.x35GlobalParam.j27ID_Invoice.ToString
            Me.p40FirstSupplyDate.SelectedDate = DateSerial(Year(Now), Month(Now) + 1, 1).AddDays(-1)
            Me.p40LastSupplyDate.SelectedDate = DateSerial(Year(Now), Month(Now) + 1, 1).AddMonths(12).AddDays(-1)
            Me.p40GenerateDayAfterSupply.Value = 1
            Me.x15ID.SelectedValue = "3"
            Return
        End If

        Dim cRec As BO.p40WorkSheet_Recurrence = Master.Factory.p40WorkSheet_RecurrenceBL.Load(Master.DataPID)
        With cRec
            basUI.SelectRadiolistValue(Me.p40RecurrenceType, CInt(.p40RecurrenceType).ToString)
            Me.p40name.Text = .p40Name
            Me.p40Text.Text = .p40Text

            Me.p34ID.SelectedValue = .p34ID.ToString
            Handle_ChangeP34ID(.p34ID)
            Me.p32ID.SelectedValue = .p32ID.ToString
            Me.x15ID.SelectedValue = CInt(.x15ID).ToString
            Me.j27ID.SelectedValue = .j27ID.ToString
            Me.j02ID.Value = .j02ID.ToString
            Me.p56ID.SelectedValue = .p56ID.ToString
            If .p56ID > 0 Then
                opgGenType.SelectedValue = "2"
            Else
                opgGenType.SelectedValue = "1"
            End If
            Me.j02ID.Text = .Person
            Me.p40FirstSupplyDate.SelectedDate = .p40FirstSupplyDate
            Me.p40LastSupplyDate.SelectedDate = .p40LastSupplyDate
            Me.p40Value.Value = .p40Value
            Me.p40GenerateDayAfterSupply.Value = .p40GenerateDayAfterSupply


            Master.Timestamp = .Timestamp


            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p40WorkSheet_RecurrenceBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p40-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Me.opgGenType.SelectedValue = "2" And Me.p56ID.SelectedValue = "" Then
            Master.Notify("Chybí vybrat úkol.", NotifyLevel.ErrorMessage) : Return
        End If
        With Master.Factory.p40WorkSheet_RecurrenceBL
            Dim cRec As BO.p40WorkSheet_Recurrence = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p40WorkSheet_Recurrence)
            With cRec
                .p41ID = Me.CurrentP41ID
                .p40Name = Me.p40name.Text
                .p40Text = Me.p40Text.Text
                .p40Value = BO.BAS.IsNullNum(Me.p40Value.Value)
                .p40GenerateDayAfterSupply = BO.BAS.IsNullInt(Me.p40GenerateDayAfterSupply.Value)
                .p34ID = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
                .p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                .x15ID = BO.BAS.IsNullInt(Me.x15ID.SelectedValue)
                .j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
                .j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
                .p56ID = BO.BAS.IsNullInt(Me.p56ID.SelectedValue)
                .p40RecurrenceType = CType(CInt(Me.p40RecurrenceType.SelectedValue), BO.RecurrenceType)
                .p40FirstSupplyDate = BO.BAS.IsNullDBDate(Me.p40FirstSupplyDate.SelectedDate)
                .p40LastSupplyDate = BO.BAS.IsNullDBDate(Me.p40LastSupplyDate.SelectedDate)

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p40-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Dim intP34ID As Integer = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        If intP34ID = 0 Then
            Me.p32ID.Clear()
        Else

            Handle_ChangeP34ID(intP34ID)
        End If
    End Sub
    Private Sub Handle_ChangeP34ID(intP34ID As Integer)
        Dim mq As New BO.myQueryP32
        mq.p34ID = intP34ID
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()
    End Sub

    Private Sub p56ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p56ID.NeedMissingItem
        Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.NameWithTypeAndCode
        End If
    End Sub

    Private Sub p40_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If opgGenType.SelectedValue = "1" Then
            panDateSettings.Visible = True
        Else
            panDateSettings.Visible = False
        End If
        panTask.Visible = Not panDateSettings.Visible
    End Sub
End Class