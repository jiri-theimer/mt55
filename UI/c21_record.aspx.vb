Public Class c21_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub c21_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení pracovního fondu"
            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.c21FondCalendar = Master.Factory.c21FondCalendarBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.c21ScopeFlag, CInt(.c21ScopeFlag).ToString)
            Me.c21Name.Text = .c21Name
            Me.c21Day1_Hours.Value = .c21Day1_Hours
            Me.c21Day2_Hours.Value = .c21Day2_Hours
            Me.c21Day3_Hours.Value = .c21Day3_Hours
            Me.c21Day4_Hours.Value = .c21Day4_Hours
            Me.c21Day5_Hours.Value = .c21Day5_Hours
            Me.c21Day6_Hours.Value = .c21Day6_Hours
            Me.c21Day7_Hours.Value = .c21Day7_Hours


            Me.c21Ordinary.Value = .c21Ordinary

            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.c21FondCalendarBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("c21-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.c21FondCalendarBL
            Dim cRec As BO.c21FondCalendar = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.c21FondCalendar)
            With cRec
                .c21Name = Me.c21Name.Text
                .c21ScopeFlag = CType(CInt(Me.c21ScopeFlag.SelectedValue), BO.c21ScopeFlagENUM)
                .c21Day1_Hours = BO.BAS.IsNullNum(Me.c21Day1_Hours.Value)
                .c21Day2_Hours = BO.BAS.IsNullNum(Me.c21Day2_Hours.Value)
                .c21Day3_Hours = BO.BAS.IsNullNum(Me.c21Day3_Hours.Value)
                .c21Day4_Hours = BO.BAS.IsNullNum(Me.c21Day4_Hours.Value)
                .c21Day5_Hours = BO.BAS.IsNullNum(Me.c21Day5_Hours.Value)
                .c21Day6_Hours = BO.BAS.IsNullNum(Me.c21Day6_Hours.Value)
                .c21Day7_Hours = BO.BAS.IsNullNum(Me.c21Day7_Hours.Value)

                .c21Ordinary = BO.BAS.IsNullInt(Me.c21Ordinary.Value)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("c21-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub c21_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        panScope1.Visible = False
        Select Case Me.c21ScopeFlag.SelectedValue
            Case "1"
                panScope1.Visible = True

        End Select
    End Sub
End Class