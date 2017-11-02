Public Class p50_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p50_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Server.ScriptTimeout = 300

        Master.EnableRecordValidity = False
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .HeaderText = "Nákladové ceníky"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period2.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
            End With
            Dim mq As New BO.myQuery
            mq.Closed = BO.BooleanQueryMode.FalseQuery
            Me.p51ID.DataSource = Master.Factory.p51PriceListBL.GetList(mq).Where(Function(p) p.p51IsMasterPriceList = False And p.p51IsInternalPriceList = True)
            Me.p51ID.DataBind()


            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Me.p50ValidFrom.SelectedDate = Today
            Me.p50ValidUntil.SelectedDate = DateSerial(3000, 1, 1)
            Return
        End If
        Dim cRec As BO.p50OfficePriceList = Master.Factory.p50OfficePriceListBL.Load(Master.DataPID)
        With cRec
            Me.p51ID.SelectedValue = .p51ID.ToString
            Me.p50RatesFlag.SelectedValue = CInt(.p50RatesFlag).ToString
            Me.p50ValidFrom.SelectedDate = .ValidFrom
            Me.p50ValidUntil.SelectedDate = .ValidUntil
            ''Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp
        End With

       
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p50OfficePriceListBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p50-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p50OfficePriceListBL
            Dim cRec As New BO.p50OfficePriceList
            If Master.DataPID <> 0 Then cRec = .Load(Master.DataPID)
            With cRec
                .p51ID = BO.BAS.IsNullInt(Me.p51ID.SelectedValue)
                .p50RatesFlag = BO.BAS.IsNullInt(Me.p50RatesFlag.SelectedValue)
                .ValidFrom = Me.p50ValidFrom.SelectedDate
                .ValidUntil = Me.p50ValidUntil.SelectedDate

            End With
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p50-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p50_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.p51ID.SelectedValue <> "" Then
            Me.clue1.Attributes("rel") = "clue_p51_record.aspx?pid=" & Me.p51ID.SelectedValue
            Me.clue1.Visible = True
        Else
            Me.clue1.Visible = False
        End If
        Me.panRecalcFPR.Visible = False : panRecalcCostRates.Visible = False
        Select Case Me.p50RatesFlag.SelectedValue
            Case "1"
                panRecalcCostRates.Visible = True
            Case "2"
                Me.panRecalcFPR.Visible = True
        End Select
    End Sub

    Private Sub cmdRecalcFPR_Click(sender As Object, e As EventArgs) Handles cmdRecalcFPR.Click
        If Not ValidateBeforeRun(period1) Then Return
        Dim intP51ID As Integer = BO.BAS.IsNullInt(Me.p51ID.SelectedValue)
        If intP51ID = 0 Then
            Master.Notify("Musíte vybrat ceník.", NotifyLevel.WarningMessage)
            Return
        End If
        With Master.Factory.p91InvoiceBL
            If .RecalcFPR(period1.DateFrom, period1.DateUntil, intP51ID) Then
                Master.Notify("Operace dokončena.")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub p51ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then strAddMissingItemText = cRec.NameWithCurr
    End Sub

    

    Private Sub cmdRecalcCostRates_Click(sender As Object, e As EventArgs) Handles cmdRecalcCostRates.Click
        If Not ValidateBeforeRun(period2) Then Return

        Dim intP51ID As Integer = BO.BAS.IsNullInt(Me.p51ID.SelectedValue)

        With Master.Factory.p31WorksheetBL
            If .Recalc_Internal_Rates(period2.DateFrom, period2.DateUntil, intP51ID) Then
                Master.Notify("Operace dokončena.")
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Function ValidateBeforeRun(per As UI.periodcombo) As Boolean
        If DateDiff(DateInterval.Day, per.DateFrom, per.DateUntil) >= 366 Then
            Master.Notify("Zadané období pro zpětný přepočet je příliš široké (maximum je 1 rok).", NotifyLevel.WarningMessage)
            Return False
        End If
        If BO.BAS.IsNullInt(Me.p51ID.SelectedValue) = 0 Then
            Master.Notify("Musíte vybrat ceník.", NotifyLevel.WarningMessage)
            Return False
        End If
        Return True
    End Function
End Class