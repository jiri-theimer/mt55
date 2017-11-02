Public Class b01_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub b01_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/workflow_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Workflow šablona"

                Me.o40ID.DataSource = .Factory.o40SmtpAccountBL.GetList(New BO.myQuery)
                Me.o40ID.DataBind()
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

        Dim cRec As BO.b01WorkflowTemplate = Master.Factory.b01WorkflowTemplateBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record is missing.")

        With cRec

            Me.b01Name.Text = .b01Name
            Me.b01Code.Text = .b01Code
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            Me.o40ID.SelectedValue = .o40ID.ToString
           
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.b01WorkflowTemplateBL
            If .Delete(Master.DataPID) Then
                Master.CloseAndRefreshParent("b01-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.b01WorkflowTemplateBL
            Dim cRec As BO.b01WorkflowTemplate = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.b01WorkflowTemplate)

            With cRec
                .x29ID = DirectCast(BO.BAS.IsNullInt(Me.x29ID.SelectedValue), BO.x29IdEnum)
                .o40ID = BO.BAS.IsNullInt(Me.o40ID.SelectedValue)
                .b01Name = Me.b01Name.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .b01Code = Me.b01Code.Text
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("b01-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class