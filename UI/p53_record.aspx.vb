Public Class p53_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p53_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "DPH sazba"

                Me.j27ID.DataSource = .Factory.ftBL.GetList_J27(New BO.myQuery)
                Me.j27ID.DataBind()
                Me.x15ID.DataSource = .Factory.ftBL.GetList_X15(New BO.myQuery)
                Me.x15ID.DataBind()
              
                Me.j17ID.DataSource = .Factory.j17CountryBL.GetList()
                Me.j17ID.DataBind()

            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If


        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Me.p53ValidFrom.SelectedDate = Today
            Me.p53ValidUntil.SelectedDate = DateSerial(3000, 1, 1)
            Return
        End If

        Dim cRec As BO.p53VatRate = Master.Factory.p53VatRateBL.Load(Master.DataPID)
        With cRec
            Me.p53Value.Value = .p53Value

            Me.x15ID.SelectedValue = CInt(.x15ID).ToString
            Me.j27ID.SelectedValue = .j27ID.ToString
            Me.j17ID.SelectedValue = .j17ID.ToString
            Me.p53ValidFrom.SelectedDate = .ValidFrom
            Me.p53ValidUntil.SelectedDate = .ValidUntil

            Master.Timestamp = .Timestamp


            ''Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)

            ''Me.Validity.Text = BO.BAS.FD(.ValidFrom) & " - " & BO.BAS.FD(.ValidUntil)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p53VatRateBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p53-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p53VatRateBL
            Dim cRec As BO.p53VatRate = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p53VatRate)
            cRec.p53Value = BO.BAS.IsNullNum(Me.p53Value.Value)
            cRec.x15ID = BO.BAS.IsNullInt(Me.x15ID.SelectedValue)
            cRec.j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
            cRec.j17ID = BO.BAS.IsNullInt(Me.j17ID.SelectedValue)
            cRec.ValidFrom = Me.p53ValidFrom.SelectedDate
            cRec.ValidUntil = Me.p53ValidUntil.SelectedDate

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p53-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class