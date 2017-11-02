Public Class x55_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x55_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Gadget nastavení"
            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.x55Code.Enabled = True
                Me.x55Code.Text = ""
            End If
        End If
    End Sub
    Private Sub RefreshRecord()

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.x55HtmlSnippet = Master.Factory.x55HtmlSnippetBL.Load(Master.DataPID)
        With cRec
            Me.x55Name.Text = .x55Name
            Me.x55Code.Text = .x55Code
            
            Me.x55Ordinary.Value = .x55Ordinary
            basUI.SelectDropdownlistValue(Me.x55TypeFlag, CInt(.x55TypeFlag).ToString)
            Me.x55Content.Content = .x55Content
            Me.x55RecordSQL.Text = .x55RecordSQL
            Me.x55Height.Text = .x55Height
            Master.Timestamp = .Timestamp

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        Me.x55Code.Enabled = Not cRec.x55IsSystem


        Dim lisX56 As IEnumerable(Of BO.x56SnippetProperty) = Master.Factory.x55HtmlSnippetBL.GetList_Properties(cRec.PID)
        For Each c In lisX56
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid")
                .p85DataPID = c.PID
                .p85FreeText01 = c.x56Control
                .p85FreeText02 = c.x56ControlPropertyName
                .p85Message = c.x56ControlPropertyValue
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTemp()
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x55HtmlSnippetBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x55-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        Server.Transfer("x55_record.aspx?pid=" & Master.DataPID.ToString)
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveTemp()
        With Master.Factory.x55HtmlSnippetBL
            Dim cRec As BO.x55HtmlSnippet = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x55HtmlSnippet)
            With cRec
                .x55Name = Me.x55Name.Text
                .x55Code = Me.x55Code.Text
                .x55Ordinary = BO.BAS.IsNullInt(Me.x55Ordinary.Value)
                .x55TypeFlag = CType(CInt(Me.x55TypeFlag.SelectedValue), BO.x55TypeENUM)
                .x55RecordSQL = Me.x55RecordSQL.Text
                .x55Content = Me.x55Content.Content
                .x55Height = Me.x55Height.Text
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            Dim lisX56 As New List(Of BO.x56SnippetProperty)
            Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            For Each cTemp In lisTemp
                Dim c As New BO.x56SnippetProperty
                c.x56Control = cTemp.p85FreeText01
                c.x56ControlPropertyName = cTemp.p85FreeText02
                c.x56ControlPropertyValue = cTemp.p85Message
                lisX56.Add(c)
            Next


            If .Save(cRec, lisX56) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("x55-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        SaveTemp()
        
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid")
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTemp()
    End Sub
    Private Sub SaveTemp()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85FreeText01 = CType(ri.FindControl("x56Control"), TextBox).Text
                .p85FreeText02 = CType(ri.FindControl("x56ControlPropertyName"), TextBox).Text
                .p85Message = CType(ri.FindControl("x56ControlPropertyValue"), TextBox).Text


            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub
    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        SaveTemp()
        Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(e.Item.FindControl("p85id"), HiddenField).Value)

        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTemp()
            End If
        End If
    End Sub
    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("x56Control"), TextBox).Text = cRec.p85FreeText01
        CType(e.Item.FindControl("x56ControlPropertyName"), TextBox).Text = cRec.p85FreeText02
        CType(e.Item.FindControl("x56ControlPropertyValue"), TextBox).Text = cRec.p85Message

       
    End Sub
    Private Sub RefreshTemp()
        rp1.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rp1.DataBind()
    End Sub

    Private Sub x55_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.x55RecordSQL.Visible = False
        panParams.Visible = False

        Select Case Me.x55TypeFlag.SelectedValue
            Case "1"
                panParams.Visible = True
                lblContentLabel.Text = "HTML obsah:"
                Me.x55RecordSQL.Visible = True
            Case "2"
                lblContentLabel.Text = "HTML obsah:"
            Case "3"
                lblContentLabel.Text = "URL www stránky:"
        End Select
        lblx55RecordSQL.Visible = Me.x55RecordSQL.Visible

    End Sub

   
End Class