Public Class j62_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Public ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            Return CType(Me.x29ID.SelectedValue, BO.x29IdEnum)
        End Get
    End Property
    Public Property CurrentJ60ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidJ60ID.Value)
        End Get
        Set(value As Integer)
            Me.hidJ60ID.Value = value.ToString
        End Set
    End Property

    Private Sub j62_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roles1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Menu položka"
                Me.CurrentJ60ID = BO.BAS.IsNullInt(Request.Item("j60id"))
                If Me.CurrentJ60ID = 0 Then
                    .StopPage("j60id is missing.")
                Else
                    Dim cJ60 As BO.j60MenuTemplate = .Factory.j62MenuHomeBL.Load_j60(Me.CurrentJ60ID)
                    Me.j60Name.Text = cJ60.j60Name
                    If cJ60.j60IsSystem And BO.ASS.GetConfigVal("Guru") <> "1" Then
                        .StopPage("Systémové menu nelze upravovat.")
                    End If
                End If

                Me.j62ParentID.DataSource = .Factory.j62MenuHomeBL.GetList(Me.CurrentJ60ID, New BO.myQuery).Where(Function(p) p.PID <> .DataPID)
                Me.j62ParentID.DataBind()
            End With
            With Me.cbxUrls
                .DataSource = Master.Factory.x31ReportBL.GetList_PersonalPageSource()
                .DataBind()
                Dim item As New ListItem("--Vybrat menu odkaz--", "")
                .Items.Insert(0, item)
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

        Dim cRec As BO.j62MenuHome = Master.Factory.j62MenuHomeBL.Load(Master.DataPID)
        With cRec
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(.x29ID).ToString)
            SetupQueryAndColumns()

            Me.j62ParentID.SelectedValue = .j62ParentID.ToString
            Me.j62Name.Text = .j62Name
            Me.j62Name_ENG.Text = .j62Name_ENG
            Me.j62Ordinary.Value = .j62Ordinary
            Master.Timestamp = .Timestamp
            Me.j62Url.Text = .j62Url
            Me.j62Target.SelectedValue = .j62Target
            Me.j62ImageUrl.Text = .j62ImageUrl
            Me.j70ID.SelectedValue = .j70ID.ToString

            basUI.SelectDropdownlistValue(Me.j62GridGroupBy, .j62GridGroupBy)
            Me.j62IsSeparator.Checked = .j62IsSeparator
            Me.j62Tag.Text = .j62Tag

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With

        roles1.InhaleInitialData(cRec.PID)
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.j62MenuHomeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("j62-delete")
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
        With Master.Factory.j62MenuHomeBL
            Dim cRec As BO.j62MenuHome = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.j62MenuHome)
            With cRec
                .j60ID = Me.CurrentJ60ID
                .x29ID = BO.BAS.IsNullInt(Me.x29ID.SelectedValue)
                .j62ParentID = BO.BAS.IsNullInt(Me.j62ParentID.SelectedValue)
                .j70ID = BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
                .j62GridGroupBy = Me.j62GridGroupBy.SelectedValue
                .j62Name = Me.j62Name.Text
                .j62Name_ENG = Me.j62Name_ENG.Text
                .j62Url = Trim(Me.j62Url.Text)
                .j62Target = Me.j62Target.SelectedValue
                .j62ImageUrl = Me.j62ImageUrl.Text
                .j62IsSeparator = Me.j62IsSeparator.Checked
                .j62Ordinary = BO.BAS.IsNullInt(Me.j62Ordinary.Value)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .j62Tag = Me.j62Tag.Text
            End With

            Dim lisX69 As List(Of BO.x69EntityRole_Assign) = roles1.GetData4Save()
            If roles1.ErrorMessage <> "" Then
                Master.Notify(roles1.ErrorMessage, 2)
                Return
            End If

            If .Save(cRec, lisX69) Then
                Master.DataPID = .LastSavedPID


                Master.CloseAndRefreshParent("j62-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAddX69_Click(sender As Object, e As EventArgs) Handles cmdAddX69.Click
        roles1.AddNewRow()
    End Sub

    Private Sub cbxUrls_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxUrls.SelectedIndexChanged
        If Me.cbxUrls.SelectedIndex > 0 Then
            Me.j62Url.Text = Me.cbxUrls.SelectedValue
            Dim lis As List(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList_PersonalPageSource()
            For Each c In lis
                If c.ReportFileName = Me.cbxUrls.SelectedValue Then
                    basUI.SelectDropdownlistValue(Me.x29ID, CInt(c.x29ID).ToString)
                    Me.j62Tag.Text = c.x31Code
                    Exit For
                End If
            Next
            Me.cbxUrls.SelectedIndex = 0
        End If
        SetupQueryAndColumns()
    End Sub

    

    Private Sub x29ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x29ID.SelectedIndexChanged
        SetupQueryAndColumns()
    End Sub

    Private Sub SetupQueryAndColumns()
        ''Me.j70ID.DataSource = Master.Factory.j70QueryTemplateBL.GetList(New BO.myQuery, Me.CurrentX29ID).Where(Function(p) (p.j02ID_Owner = Master.Factory.SysUser.j02ID And p.j70IsSystem = False))
        Me.j70ID.DataSource = Master.Factory.j70QueryTemplateBL.GetList(New BO.myQuery, Me.CurrentX29ID).Where(Function(p) (p.j70IsSystem = False))
        Me.j70ID.DataBind()
      
        
        Me.j62GridGroupBy.DataSource = Master.Factory.j70QueryTemplateBL.GroupByPallet(Me.CurrentX29ID)
        Me.j62GridGroupBy.DataBind()

    End Sub

    Private Sub j62_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        tabLink.Visible = Not Me.j62IsSeparator.Checked
    End Sub
End Class