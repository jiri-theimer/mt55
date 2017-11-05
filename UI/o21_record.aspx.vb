Public Class o21_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o21_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Typ události"
            End With
            If Master.DataPID = 0 Then
                ViewState("prefix") = Left(Request.Item("prefix"), 3)
                If BO.BAS.GetX29FromPrefix(ViewState("prefix")) = BO.x29IdEnum._NotSpecified Then
                    Master.StopPage("Na vstupu chybí prefix (" & ViewState("prefix") & ").", True)
                Else
                    basUI.SelectDropdownlistValue(Me.x29ID, BO.BAS.IsNullInt(BO.BAS.GetX29FromPrefix(ViewState("prefix"))))
                End If
            End If
            Me.o25ID.DataSource = Master.Factory.o25AppBL.GetList(New BO.myQuery).Where(Function(p) p.o25AppFlag = BO.o25AppFlagENUM.GoogleCalendar)
            Me.o25ID.DataBind()

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub

   

    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.o21MilestoneType = Master.Factory.o21MilestoneTypeBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            Me.o21Name.Text = .o21name
            Me.o21Ordinary.Value = .o21Ordinary
            basUI.SelectRadiolistValue(Me.o21Flag, CInt(.o21Flag).ToString)
            Me.o21ColorID.SelectedValue = .o21ColorID
            Me.o25ID.SelectedValue = .o25ID.ToString
            Master.Timestamp = .Timestamp

          
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o21MilestoneTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o21-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o21MilestoneTypeBL
            Dim cRec As BO.o21MilestoneType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o21MilestoneType)
            cRec.x29ID = BO.BAS.IsNullInt(Me.x29ID.SelectedValue)
            cRec.o21Name = Me.o21Name.Text
            cRec.o21Flag = DirectCast(CInt(Me.o21Flag.SelectedValue), BO.o21FlagEnum)
            cRec.o21Ordinary = BO.BAS.IsNullInt(Me.o21Ordinary.Value)
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil
            cRec.o21ColorID = Me.o21ColorID.SelectedValue
            cRec.o25ID = BO.BAS.IsNullInt(Me.o25ID.SelectedValue)
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID

                
                Master.CloseAndRefreshParent("o21-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    

    Private Sub o21_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.o21ColorID.BackColor = Me.o21ColorID.SelectedItem.BackColor
    End Sub

    Private Sub o25ID_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles o25ID.NeedMissingItem
        Dim cRec As BO.o25App = Master.Factory.o25AppBL.Load(BO.BAS.IsNullInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.o25Name
        End If
    End Sub
End Class