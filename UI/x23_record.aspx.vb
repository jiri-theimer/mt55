Public Class x23_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x23_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Combo seznam"
            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.x23EntityField_Combo = Master.Factory.x23EntityField_ComboBL.Load(Master.DataPID)
        With cRec
            Me.x23Name.Text = .x23Name
            Me.x23Ordinary.Value = .x23Ordinary
            Me.x23DataSource.Text = .x23DataSource
            If .x23DataSource <> "" Then
                Me.chkIsDatasource.Checked = True
            Else
                Me.chkIsDatasource.Checked = False
            End If
            Me.x23DataSourceTable.Text = .x23DataSourceTable
            Me.x23DataSourceFieldPID.Text = .x23DataSourceFieldPID
            Me.x23DataSourceFieldTEXT.Text = .x23DataSourceFieldTEXT
            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

        If Not Me.chkIsDatasource.Checked Then
            Me.rp1.DataSource = Master.Factory.o23DocBL.GetList(New BO.myQueryO23(cRec.PID))
            Me.rp1.DataBind()
        End If
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x23EntityField_ComboBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x23-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x23EntityField_ComboBL
            Dim cRec As BO.x23EntityField_Combo = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x23EntityField_Combo)
            With cRec
                .x23Name = Me.x23Name.Text
                .x23Ordinary = BO.BAS.IsNullInt(Me.x23Ordinary.Value)
                .x23DataSource = Me.x23DataSource.Text
                .x23DataSourceTable = Me.x23DataSourceTable.Text
                .x23DataSourceFieldPID = Me.x23DataSourceFieldPID.Text
                .x23DataSourceFieldTEXT = Me.x23DataSourceFieldTEXT.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            

            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x23-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub x23_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panDataSource.Visible = Me.chkIsDatasource.Checked
        If Master.DataPID <> 0 Then
            Me.panItems.Visible = Not Me.panDataSource.Visible
        End If

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.o23Doc = CType(e.Item.DataItem, BO.o23Doc)
        With CType(e.Item.FindControl("o23Name"), HyperLink)
            .Text = cRec.o23Name
            .NavigateUrl = "o23_record.aspx?pid=" & cRec.PID.ToString
        End With
    End Sub

    Private Sub cmdAddItem_Click(sender As Object, e As EventArgs) Handles cmdAddItem.Click
        Server.Transfer("o23_record.aspx?pid=0&x23id=" & Master.DataPID.ToString)
    End Sub
End Class