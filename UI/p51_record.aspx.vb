Public Class p51_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p51_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p51_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            With Master
                .EnableRecordValidity = True
                .neededPermission = BO.x53PermValEnum.GR_P51_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Ceník sazeb"


            End With
            Me.j27ID.DataSource = Master.Factory.ftBL.GetList_J27(New BO.myQuery)
            Me.j27ID.DataBind()
            Me.j27ID.SelectedValue = Master.Factory.x35GlobalParam.j27ID_Invoice.ToString


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If

            If Master.DataPID = 0 Then
                Handle_ChangeJ27()
                If Request.Item("customtailor") = "1" Then
                    Me.p51IsCustomTailor.Checked = True
                    Me.p51Name.Visible = False
                    Me.lblName.Visible = False
                End If
               
            End If
        End If
    End Sub

    Private Sub SetupP51MasterCombo()
        Me.p51ID_Master.DataSource = Master.Factory.p51PriceListBL.GetList(New BO.myQuery).Where(Function(p) p.p51IsMasterPriceList = True And p.j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue))
        Me.p51ID_Master.DataBind()
    End Sub

    Private Sub Handle_ChangeJ27()
        Dim s As String = Me.p51ID_Master.SelectedValue

        SetupP51MasterCombo

        If s <> "" Then
            Me.p51ID_Master.SelectedValue = s
        End If
    End Sub
    Private Sub RefreshRecord()
        
        If Master.DataPID = 0 Then
            Me.panWizard.Visible = True
            Me.RadTabStrip1.Visible = False
            Me.RadMultiPage1.Visible = False
            Master.RenameToolbarButton("save", "Pokračovat->")
            HandleWizard()
            
            Return
        End If

        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(Master.DataPID)
        With cRec
            Me.p51Name.Text = .p51Name
            Me.p51Code.Text = .p51Code
            Me.j27ID.SelectedValue = .j27ID.ToString

            Handle_ChangeJ27()
            Me.p51ID_Master.SelectedValue = .p51ID_Master.ToString

            Me.p51Code.Text = .p51Code
            Me.p51IsInternalPriceList.Checked = .p51IsInternalPriceList
            Me.p51IsMasterPriceList.Checked = .p51IsMasterPriceList
            Me.p51IsCustomTailor.Checked = .p51IsCustomTailor
            If .p51IsCustomTailor Then
                If .p51Name = "" Then
                    Me.p51Name.Visible = False
                    Me.lblName.Visible = False
                Else
                    Me.p51Name.Enabled = False
                    Me.lblName.Visible = True
                End If
                
            End If
            Me.p51Ordinary.Value = .p51Ordinary
            Me.p51DefaultRateT.Value = .p51DefaultRateT
            Master.Timestamp = .Timestamp
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)

            'val1.InitVals(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        Dim lisP52 As IEnumerable(Of BO.p52PriceList_Item) = Master.Factory.p51PriceListBL.GetList_p52(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid"))
        For Each c In lisP52
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid")
                .p85DataPID = c.PID
                .p85OtherKey1 = c.j02ID
                .p85OtherKey2 = c.j07ID
                .p85FreeText01 = c.Person
                .p85FreeText02 = c.j07Name
                .p85OtherKey3 = c.p34ID
                .p85OtherKey4 = c.p32ID
                .p85FreeText03 = c.p34Name
                .p85FreeText04 = c.p32Name
                .p85FreeNumber01 = c.p52Rate
                .p85FreeText05 = c.p52Name
                .p85FreeBoolean01 = c.p52IsPlusAllTimeSheets
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempP52()
    End Sub
    Private Sub RefreshTempP52()
        rp1.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rp1.DataBind()
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p51PriceListBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p51-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        If Master.DataPID = 0 And Not RadTabStrip1.Visible Then
            'uložit obsah wizardu a odejít
            SaveWizard()
            Return
        End If
        If Me.p51IsMasterPriceList.Checked Then
            Me.p51IsCustomTailor.Checked = False
            Me.p51IsInternalPriceList.Checked = False
        End If
        With Master.Factory.p51PriceListBL
            Dim cRec As BO.p51PriceList = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p51PriceList)
            With cRec
                .p51ID_Master = BO.BAS.IsNullInt(Me.p51ID_Master.SelectedValue)
                .p51Name = Me.p51Name.Text
                .p51DefaultRateT = BO.BAS.IsNullNum(Me.p51DefaultRateT.Value)
                .p51Code = Me.p51Code.Text
                .j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
                .p51IsInternalPriceList = Me.p51IsInternalPriceList.Checked
                .p51IsMasterPriceList = Me.p51IsMasterPriceList.Checked
                .p51Ordinary = BO.BAS.IsNullInt(Me.p51Ordinary.Value)
                .p51IsCustomTailor = Me.p51IsCustomTailor.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            

            Dim mq As New BO.myQuery

            Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            Dim lisP52 As New List(Of BO.p52PriceList_Item)
            For Each cTMP In lisTEMP
                Dim c As New BO.p52PriceList_Item
                With cTMP
                    c.p34ID = .p85OtherKey3
                    c.p32ID = .p85OtherKey4
                    c.j02ID = .p85OtherKey1
                    c.j07ID = .p85OtherKey2
                    c.p52Rate = .p85FreeNumber01
                    c.p52Name = .p85FreeText05
                    c.p52IsPlusAllTimeSheets = .p85FreeBoolean01
                End With
                lisP52.Add(c)
            Next
            If .Save(cRec, lisP52) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p51-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub j27ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles j27ID.SelectedIndexChanged
        Handle_ChangeJ27()
    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempP52()
            End If
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With CType(e.Item.FindControl("edit"), ImageButton)
            .OnClientClick = "javascript: return edit_p52(" & cRec.PID.ToString & ")"
        End With
        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        With CType(e.Item.FindControl("clone"), ImageButton)
            .OnClientClick = "javascript: return clone_p52(" & cRec.PID.ToString & ")"
        End With

        With cRec
            If .p85OtherKey1 = 0 And .p85OtherKey2 = 0 Then
                CType(e.Item.FindControl("Subject"), Label).Text = "--Všichni--"
            Else
                CType(e.Item.FindControl("Subject"), Label).Text = Trim(.p85FreeText01 & " " & .p85FreeText02)
            End If
            With CType(e.Item.FindControl("p34Name"), Label)
                .Text = cRec.p85FreeText03
                If cRec.p85FreeBoolean01 Then
                    .Text += "<br><i>+všechny ostatní časové sešity</i>"
                End If
            End With
            
            CType(e.Item.FindControl("p32Name"), Label).Text = .p85FreeText04
            CType(e.Item.FindControl("p52Rate"), Label).Text = BO.BAS.FN(.p85FreeNumber01)
            CType(e.Item.FindControl("p52Name"), Label).Text = .p85FreeText05
        End With
        
    End Sub

    Private Sub cmdHardRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdHardRefreshOnBehind.Click
        RefreshTempP52()
    End Sub

    Private Sub p51_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        refreshstate()
    End Sub
    Private Sub RefreshState()
        Dim b As Boolean = Not Me.p51IsMasterPriceList.Checked

        Me.lblp51ID_Master.Visible = b
        Me.p51ID_Master.Visible = b
        Me.p51IsCustomTailor.Visible = b
        Me.p51IsInternalPriceList.Visible = b

        
    End Sub

    Private Sub p51ID_Master_NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String) Handles p51ID_Master.NeedMissingItem
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(CInt(strFoundedMissingItemValue))
        If Not cRec Is Nothing Then
            strAddMissingItemText = cRec.NameWithCurr
        End If
    End Sub

    Private Sub opg1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opg1.SelectedIndexChanged
        HandleWizard()
    End Sub

    Private Sub HandleWizard()
        If Me.opg1.SelectedValue = "j07" Then
            rpWizard.DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
        Else
            rpWizard.DataSource = Master.Factory.j02PersonBL.GetList(New BO.myQueryJ02).Where(Function(p) p.j02IsIntraPerson = True)
        End If

        rpWizard.DataBind()
    End Sub

    
    Private Sub rpWizard_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpWizard.ItemDataBound
        If Me.opg1.SelectedValue = "j07" Then
            Dim c As BO.j07PersonPosition = CType(e.Item.DataItem, BO.j07PersonPosition)
            CType(e.Item.FindControl("item1"), Label).Text = c.j07Name
            CType(e.Item.FindControl("pid"), HiddenField).Value = c.PID.ToString
        Else
            Dim c As BO.j02Person = CType(e.Item.DataItem, BO.j02Person)
            CType(e.Item.FindControl("item1"), Label).Text = c.FullNameDesc & IIf(c.j07ID > 0, " [" & c.j07Name & "]", "")
            CType(e.Item.FindControl("pid"), HiddenField).Value = c.PID.ToString
        End If
    End Sub

    Private Sub SaveWizard()
        Dim mq As New BO.myQueryP32
        mq.p33ID = BO.p33IdENUM.Cas
        ''Dim lisx As IEnumerable(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq).Where(Function(p) p.p32IsBillable = True)
        Dim lis As IEnumerable(Of BO.p34ActivityGroup) = Master.Factory.p34ActivityGroupBL.GetList(mq).Where(Function(p) p.p33ID = BO.p33IdENUM.Cas Or p.p33ID = BO.p33IdENUM.Kusovnik)
        Dim intP34ID As Integer = lis(0).PID, strP34Name As String = lis(0).p34Name

        For Each ri As RepeaterItem In rpWizard.Items
            Dim c As New BO.p85TempBox
            c.p85GUID = ViewState("guid")
            c.p85FreeNumber01 = BO.BAS.IsNullNum(CType(ri.FindControl("rate1"), Telerik.Web.UI.RadNumericTextBox).Value)
            If c.p85FreeNumber01 <> 0 Then
                If opg1.SelectedValue = "j07" Then
                    c.p85OtherKey2 = CInt(CType(ri.FindControl("pid"), HiddenField).Value)
                    c.p85FreeText02 = CType(ri.FindControl("item1"), Label).Text
                Else
                    c.p85OtherKey1 = CInt(CType(ri.FindControl("pid"), HiddenField).Value)
                    c.p85FreeText01 = CType(ri.FindControl("item1"), Label).Text
                End If
                c.p85OtherKey3 = intP34ID
                c.p85FreeText03 = strP34Name
                Master.Factory.p85TempBoxBL.Save(c)
            End If
        Next

        Me.RadTabStrip1.Visible = True
        Me.RadMultiPage1.Visible = True
        Me.panWizard.Visible = False
        Master.RenameToolbarButton("save", "Uložit změny")
        RefreshTempP52()
        RefreshState()
    End Sub
End Class