Public Class j05_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j05_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení vztahu [Podřízený x Nadřízený]"
            End With
            Me.j11ID_Slave.DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            Me.j11ID_Slave.DataBind()
            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.j05MasterSlave = Master.Factory.j05MasterSlaveBL.Load(Master.DataPID)
        With cRec
            Me.j02ID_Master.Value = .j02ID_Master.ToString
            Me.j02ID_Master.Text = .PersonMaster
            Me.j02ID_Slave.Value = .j02ID_Slave.ToString
            Me.j02ID_Slave.Text = .PersonSlave
            Me.j11ID_Slave.SelectedValue = .j11ID_Slave.ToString

            basUI.SelectRadiolistValue(Me.j05Disposition_p31, CInt(.j05Disposition_p31).ToString)
            basUI.SelectRadiolistValue(Me.j05Disposition_p48, CInt(.j05Disposition_p48).ToString)
            Me.j05IsCreate_p31.Checked = .j05IsCreate_p31
            Me.j05IsCreate_p48.Checked = .j05IsCreate_p48

            Master.Timestamp = .Timestamp

        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j05MasterSlaveBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j05-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.j05MasterSlaveBL
            Dim cRec As BO.j05MasterSlave = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j05MasterSlave)
            With cRec
                .j02ID_Master = BO.BAS.IsNullInt(Me.j02ID_Master.Value)
                .j02ID_Slave = BO.BAS.IsNullInt(Me.j02ID_Slave.Value)
                .j11ID_Slave = BO.BAS.IsNullInt(Me.j11ID_Slave.SelectedValue)
                .j05IsCreate_p31 = Me.j05IsCreate_p31.Checked
                .j05IsCreate_p48 = Me.j05IsCreate_p48.Checked
                .j05Disposition_p31 = Me.j05Disposition_p31.SelectedValue
                .j05Disposition_p48 = Me.j05Disposition_p48.SelectedValue
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j05-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j11ID_Slave_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles j11ID_Slave.NeedMissingItem
        Dim cRec As BO.j11Team = Master.Factory.j11TeamBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.j11Name
        End If
    End Sub
End Class