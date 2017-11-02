Public Class x27_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x27_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení skupiny uživatelských polí"
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

        Dim cRec As BO.x27EntityFieldGroup = Master.Factory.x27EntityFieldGroupBL.Load(Master.DataPID)
        With cRec
           
            Me.x27Name.Text = .x27Name
            Me.x27Ordinary.Value = .x27Ordinary

            Master.Timestamp = .Timestamp

        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x27EntityFieldGroupBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x27-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x27EntityFieldGroupBL
            Dim cRec As BO.x27EntityFieldGroup = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x27EntityFieldGroup)

            cRec.x27Name = Me.x27Name.Text
            cRec.x27Ordinary = BO.BAS.IsNullInt(Me.x27Ordinary.Value)
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x27-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class