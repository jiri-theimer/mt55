Public Class j60_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub j60_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/workflow_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "MENU šablona"
               
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Me.j60Name.Text += " KOPIE"
                chkClone.Checked = True
                chkClone.Visible = True
                ViewState("pid_clone") = Master.DataPID
                Master.DataPID = 0
            End If



        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.j60MenuTemplate = Master.Factory.j62MenuHomeBL.Load_j60(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record is missing.")
        If cRec.j60IsSystem And Not Master.IsRecordClone Then
            If BO.ASS.GetConfigVal("Guru") <> "1" Then
                Master.StopPage("Systémové menu nelze upravovat.")
            End If
        End If
        


        With cRec

            Me.j60Name.Text = .j60Name

            Master.Timestamp = .Timestamp

        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j62MenuHomeBL
            If .Delete_J60(Master.DataPID) Then
                Master.CloseAndRefreshParent("j60-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.j62MenuHomeBL
            Dim cRec As BO.j60MenuTemplate = IIf(Master.DataPID <> 0, .Load_j60(Master.DataPID), New BO.j60MenuTemplate)
           
            With cRec
                .j60Name = Me.j60Name.Text
                .j60IsSystem = cRec.j60IsSystem
            End With
            Dim intClonePID As Integer = 0
            If chkClone.Checked Then intClonePID = ViewState("pid_clone")

            If .Save_j60(cRec, intClonePID) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("j60-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class