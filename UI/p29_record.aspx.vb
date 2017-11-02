Public Class p29_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p29_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Typ klienta"

                
                Dim lisX38 As IEnumerable(Of BO.x38CodeLogic) = .Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p28Contact)
                Me.x38ID.DataSource = lisX38.Where(Function(p) p.x38IsDraft = False)
                Me.x38ID.DataBind()
                Me.x38ID_Draft.DataSource = lisX38.Where(Function(p) p.x38IsDraft = True)
                Me.x38ID_Draft.DataBind()
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If


        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p29ContactType = Master.Factory.p29ContactTypeBL.Load(Master.DataPID)
        With cRec
            Me.p29Name.Text = .p29Name
            Me.p29Ordinary.Value = .p29Ordinary
            Me.b01ID.SelectedValue = .b01ID.ToString
            Me.x38ID.SelectedValue = .x38ID.ToString
            Me.x38ID_Draft.SelectedValue = .x38ID_Draft.ToString
            Me.p29IsDefault.Checked = .p29IsDefault
            Master.Timestamp = .Timestamp

            
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p29ContactTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p29-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p29ContactTypeBL
            Dim cRec As BO.p29ContactType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p29ContactType)
            With cRec
                .p29Name = Me.p29Name.Text
                .p29Ordinary = BO.BAS.IsNullInt(Me.p29Ordinary.Value)
                .b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)
                .x38ID = BO.BAS.IsNullInt(Me.x38ID.SelectedValue)
                .x38ID_Draft = BO.BAS.IsNullInt(Me.x38ID_Draft.SelectedValue)
                .p29IsDefault = Me.p29IsDefault.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p29-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub


   
End Class