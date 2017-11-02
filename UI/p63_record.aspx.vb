Public Class p63_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p63_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Režijní přirážka k fakturaci"


                Dim mq As New BO.myQueryP32
                mq.IsMoneyInput = BO.BooleanQueryMode.TrueQuery
                Me.p32ID.DataSource = .Factory.p32ActivityBL.GetList(mq)
                Me.p32ID.DataBind()
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p63Overhead = Master.Factory.p63OverheadBL.Load(Master.DataPID)
        With cRec
            Me.p63Name.Text = .p63Name
            Me.p63PercentRate.Value = .p63PercentRate
            Me.p32ID.SelectedValue = .p32ID.ToString
            
            Me.p63IsIncludeExpense.Checked = .p63IsIncludeExpense
            Me.p63IsIncludeFees.Checked = .p63IsIncludeFees
            Me.p63IsIncludeTime.Checked = .p63IsIncludeTime
            Master.Timestamp = .Timestamp


            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p63OverheadBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p63-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p63OverheadBL
            Dim cRec As BO.p63Overhead = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p63Overhead)
            With cRec
                .p63Name = Me.p63Name.Text
                .p32ID = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                .p63PercentRate = BO.BAS.IsNullNum(Me.p63PercentRate.Value)
                .p63IsIncludeExpense = Me.p63IsIncludeExpense.Checked
                .p63IsIncludeFees = Me.p63IsIncludeFees.Checked
                .p63IsIncludeTime = Me.p63IsIncludeTime.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p63-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class