Public Class j18_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j18_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Středisko"
            End With
            Me.j17ID.DataSource = Master.Factory.j17CountryBL.GetList()
            Me.j17ID.DataBind()

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.j18Region = Master.Factory.j18RegionBL.Load(Master.DataPID)
        With cRec
            Me.j18Name.Text = .j18Name
            Me.j18Ordinary.Value = .j18Ordinary
            Me.j18Code.Text = .j18Code
            Me.j17ID.SelectedValue = .j17ID.ToString

            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            roles1.InhaleInitialData(.PID)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j18RegionBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j18-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        roles1.SaveCurrentTempData()

        With Master.Factory.j18RegionBL
            Dim cRec As BO.j18Region = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j18Region)
            cRec.j18Name = Me.j18Name.Text
            cRec.j18Code = Me.j18Code.Text
            cRec.j18Ordinary = BO.BAS.IsNullInt(Me.j18Ordinary.Value)
            cRec.j17ID = BO.BAS.IsNullInt(Me.j17ID.SelectedValue)
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If

            If .Save(cRec, lisX69) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j18-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub
End Class