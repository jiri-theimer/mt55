Public Class p98_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p97_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID

            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Zaokrouhlování vystavených faktur"


                

            End With

            RefreshRecord()


            If Master.IsRecordClone Then
                Master.DataPID = 0
                Me.p98IsDefault.Checked = False
            End If


        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p98Invoice_Round_Setting_Template = Master.Factory.p98Invoice_Round_Setting_TemplateBL.Load(Master.DataPID)
        With cRec
            Me.p98Name.Text = .p98Name
            Me.p98IsDefault.Checked = .p98IsDefault


            'Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp
        End With

        Dim lis As IEnumerable(Of BO.p97Invoice_Round_Setting) = Master.Factory.p98Invoice_Round_Setting_TemplateBL.GetList_P97(cRec.PID)
        For Each c In lis
            Dim cTemp As New BO.p85TempBox
            cTemp.p85GUID = ViewState("guid")
            cTemp.p85DataPID = cRec.PID
            cTemp.p85OtherKey1 = c.j27ID
            cTemp.p85OtherKey2 = c.p97AmountFlag
            cTemp.p85OtherKey3 = c.p97Scale
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next

        RefreshTempList()
    End Sub

    Private Sub RefreshTempList()
        rp1.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rp1.DataBind()
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p98Invoice_Round_Setting_TemplateBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p98-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveTemp()
        With Master.Factory.p98Invoice_Round_Setting_TemplateBL
            Dim cRec As BO.p98Invoice_Round_Setting_Template = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p98Invoice_Round_Setting_Template)
            With cRec
                .p98Name = Me.p98Name.Text
                .p98IsDefault = Me.p98IsDefault.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

            Dim lis As New List(Of BO.p97Invoice_Round_Setting)
            Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            For Each cTemp In lisTEMP
                Dim c As New BO.p97Invoice_Round_Setting
                c.j27ID = cTemp.p85OtherKey1
                c.p97AmountFlag = cTemp.p85OtherKey2
                c.p97Scale = cTemp.p85OtherKey3

                lis.Add(c)
            Next

            If .Save(cRec, lis) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p98-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        SaveTemp()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(e.CommandArgument))
        Master.Factory.p85TempBoxBL.Delete(cRec)
        RefreshTempList()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("j27ID"), DropDownList)
            .DataSource = Master.Factory.ftBL.GetList_J27()
            .DataBind()
        End With
        CType(e.Item.FindControl("del"), ImageButton).CommandArgument = cRec.PID

        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j27ID"), DropDownList), cRec.p85OtherKey1.ToString)
        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p97AmountFlag"), DropDownList), cRec.p85OtherKey2.ToString)
        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p97Scale"), DropDownList), cRec.p85OtherKey3.ToString)
    End Sub

    Private Sub SaveTemp()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("del"), ImageButton).CommandArgument)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("j27id"), DropDownList).SelectedValue)
                .p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("p97AmountFlag"), DropDownList).SelectedValue)
                .p85OtherKey3 = BO.BAS.IsNullInt(CType(ri.FindControl("p97Scale"), DropDownList).SelectedValue)
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub cmdAddRow_Click(sender As Object, e As EventArgs) Handles cmdAddRow.Click
        Dim c As New BO.p85TempBox
        c.p85GUID = ViewState("guid")
        Master.Factory.p85TempBoxBL.Save(c)
        RefreshTempList()
    End Sub
End Class