Public Class m62_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub m62_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení měnového kurzu"


                Me.j27ID_Master.DataSource = .Factory.ftBL.GetList_J27(New BO.myQuery)
                Me.j27ID_Master.DataBind()
                Me.j27ID_Slave.DataSource = .Factory.ftBL.GetList_J27(New BO.myQuery)
                Me.j27ID_Slave.DataBind()
            End With

            RefreshRecord()

           

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.m62ExchangeRate = Master.Factory.m62ExchangeRateBL.Load(Master.DataPID)
        With cRec
            Me.m62Rate.Value = .m62Rate
            Me.m62Date.SelectedDate = .m62Date
            Me.m62RateType.SelectedValue = CInt(.m62RateType).ToString
            Me.j27ID_Master.SelectedValue = .j27ID_Master.ToString
            Me.j27ID_Slave.SelectedValue = .j27ID_Slave.ToString
            Me.m62Units.Value = .m62Units
            Master.Timestamp = .Timestamp
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.m62ExchangeRateBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("m62-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Me.m62Date.IsEmpty Then
            Master.Notify("Musíte vyplnit datum.", NotifyLevel.WarningMessage)
            Return
        End If
        With Master.Factory.m62ExchangeRateBL
            Dim cRec As BO.m62ExchangeRate = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.m62ExchangeRate)
            cRec.m62RateType = CType(CInt(Me.m62RateType.SelectedValue), BO.m62RateTypeENUM)
            cRec.m62Rate = BO.BAS.IsNullNum(Me.m62Rate.Value)
            cRec.j27ID_Master = BO.BAS.IsNullInt(Me.j27ID_Master.SelectedValue)
            cRec.j27ID_Slave = BO.BAS.IsNullInt(Me.j27ID_Slave.SelectedValue)
            cRec.m62Date = Me.m62Date.SelectedDate
            cRec.m62Units = BO.BAS.IsNullInt(Me.m62Units.Value)


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("m62-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class