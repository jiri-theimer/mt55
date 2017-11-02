Public Class c26_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub c26_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

  


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Den svátku | Ne-pracovní den"
                Me.j17ID.DataSource = .Factory.j17CountryBL.GetList(New BO.myQuery)
                Me.j17ID.DataBind()
            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.c26Holiday = Master.Factory.c26HolidayBL.Load(Master.DataPID)
        With cRec
            Me.c26Name.Text = .c26Name
            Me.j17ID.SelectedValue = .j17ID.ToString
            Me.c26Date.SelectedDate = .c26Date

            Master.Timestamp = .Timestamp

            'Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.c26HolidayBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("c26-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.c26HolidayBL
            Dim cRec As BO.c26Holiday = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.c26Holiday)
            cRec.c26Name = Me.c26Name.Text
            cRec.j17ID = BO.BAS.IsNullInt(Me.j17ID.SelectedValue)
            cRec.c26Date = Me.c26Date.SelectedDate
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("c26-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

End Class