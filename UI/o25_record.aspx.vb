Public Class o25_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub o25_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                 .neededPermission = BO.x53PermValEnum.GR_Admin
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

                .HeaderIcon = "Images/settings_32.png"

                .HeaderText = "Externí IS"


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

        Dim cRec As BO.o25App = Master.Factory.o25AppBL.Load(Master.DataPID)
        With cRec
            Me.o25Name.Text = .o25Name
            basUI.SelectDropdownlistValue(Me.o25AppFlag, CInt(.o25AppFlag).ToString)
            Me.o25IsMainMenu.Checked = .o25IsMainMenu
            Me.o25Code.Text = .o25Code
            Me.o25Url.Text = .o25Url
         

            Master.Timestamp = .Timestamp
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)


        End With


    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.o25AppBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("o25-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.o25AppBL
            Dim cRec As BO.o25App = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.o25App)
            cRec.o25AppFlag = DirectCast(CInt(Me.o25AppFlag.SelectedValue), BO.o25AppFlagENUM)
            cRec.o25Name = Me.o25Name.Text
            cRec.o25Code = Me.o25Code.Text
            cRec.o25Url = Me.o25Url.Text
            cRec.o25IsMainMenu = Me.o25IsMainMenu.Checked
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID


                Master.CloseAndRefreshParent("o25-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class