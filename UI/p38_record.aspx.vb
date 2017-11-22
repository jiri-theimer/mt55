Public Class p38_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p38_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Kategorie aktivit"
            End With

            RefreshRecord()

           
            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p38ActivityTag = Master.Factory.p38ActivityTagBL.Load(Master.DataPID)
        With cRec
            Me.p38name.Text = .p38Name
            Me.p38Code.Text = .p38Code

            Master.Timestamp = .Timestamp
            Me.p38Ordinary.Value = .p38Ordinary
           
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p38ActivityTagBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p38-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p38ActivityTagBL
            Dim cRec As BO.p38ActivityTag = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p38ActivityTag)
            cRec.p38Name = Me.p38name.Text
            cRec.p38Code = Me.p38Code.Text
            cRec.p38Ordinary = BO.BAS.IsNullInt(Me.p38Ordinary.Value)

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p38-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class