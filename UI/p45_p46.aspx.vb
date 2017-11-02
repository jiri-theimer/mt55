Imports Telerik.Web.UI

Public Class p45_p46
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Public Property CurrentP41ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidP41ID.Value)
        End Get
        Set(value As Integer)
            hidP41ID.Value = value.ToString
        End Set
    End Property

    Private Sub p45_project_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID

            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))    'p45ID
                If .DataPID = 0 Then .StopPage("pid is missing.")
                .HeaderText = "Rozpočet projektu | " & .Factory.GetRecordCaption(BO.x29IdEnum.p45Budget, .DataPID)
            End With
            RefreshRecord()

            Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(Me.CurrentP41ID)
            Dim cDisp As BO.p41RecordDisposition = Master.Factory.p41ProjectBL.InhaleRecordDisposition(cP41)
            If cDisp.p45_Owner Then
                Master.AddToolbarButton("Uložit změny", "save", 0, "Images/save.png")
            Else
                If Not cDisp.p45_Read Then Master.StopPage("Nemáte oprávnění pro čtení projektového rozpočtu.")
                cmdAddPerson.Visible = False                
                Master.Notify("Rozpočet projektu můžete pouze číst, nikoliv upravovat.")
            End If

            SetupPersonsOffer()
        End If
    End Sub
   
    Private Sub p45_project_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

      
       
        grid1.Rebind()

    End Sub

    


    Private Sub RefreshRecord()
        Dim cRec As BO.p45Budget = Master.Factory.p45BudgetBL.Load(Master.DataPID)
        Me.CurrentP41ID = cRec.p41ID
        ViewState("d1") = cRec.p45PlanFrom
        ViewState("d2") = cRec.p45PlanUntil
        SetupTempData()

      

    End Sub

    Private Sub SetupPersonsOffer()
        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, Me.CurrentP41ID)
        Dim j02ids As List(Of Integer) = lisX69.Select(Function(p) p.j02ID).Distinct.ToList
        Dim j11ids As List(Of Integer) = lisX69.Select(Function(p) p.j11ID).Distinct.ToList
        Dim persons As List(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList_j02_join_j11(j02ids, j11ids).Where(Function(p) p.IsClosed = False).ToList
        If persons.Count = 0 Then
            Master.Notify("Projekt nemá obsazené projektové role osobami!", NotifyLevel.WarningMessage)
        End If
        Dim j02ids_used As List(Of Integer) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"), False).Where(Function(p) p.p85Prefix = "p46").Select(Function(p) p.p85OtherKey1).ToList
        For Each intJ02ID As Integer In j02ids_used
            If persons.Where(Function(p) p.PID = intJ02ID).Count > 0 Then
                persons.Remove(persons.First(Function(p) p.PID = intJ02ID))
            End If
        Next
        rpJ02.DataSource = persons
        rpJ02.DataBind()
        If rpJ02.Items.Count = 0 Then
            cmdInsertPersons.Visible = False
            lblInsertPersonsHeader.Text = "Pro tento rozpočet nejsou další osoby k dispozici."
        End If
    End Sub

    Private Sub SetupTempData()
        Dim lisP46 As IEnumerable(Of BO.p46BudgetPerson) = Master.Factory.p45BudgetBL.GetList_p46(Master.DataPID)
       
        For Each c In lisP46
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid")
                .p85Prefix = "p46"
                .p85DataPID = c.PID
                .p85OtherKey1 = c.j02ID
                .p85OtherKey2 = c.p46ExceedFlag
                .p85FreeText01 = c.Person
                .p85FreeFloat01 = c.p46HoursBillable
                .p85FreeFloat02 = c.p46HoursNonBillable
                .p85FreeFloat03 = c.p46HoursTotal
                .p85FreeText02 = c.p46Description
                .p85FreeNumber01 = c.p46BillingRate
                .p85FreeNumber02 = c.p46CostRate
                .p85FreeNumber03 = c.BillingAmount
                .p85FreeNumber04 = c.CostAmount
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        
    End Sub







    

    Private Sub rpJ02_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpJ02.ItemDataBound
        Dim cRec As BO.j02Person = CType(e.Item.DataItem, BO.j02Person)
        CType(e.Item.FindControl("hidJ02ID"), HiddenField).Value = cRec.PID.ToString
        With CType(e.Item.FindControl("Person"), CheckBox)
            .Text = cRec.FullNameDesc
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        CType(e.Item.FindControl("clue_person"), HyperLink).Attributes("rel") = "clue_j02_capacity.aspx?pid=" & cRec.PID.ToString & "&p41id=" & Me.CurrentP41ID.ToString
    End Sub




    Private Sub ReloadPage()
        Server.Transfer("p45_p46.aspx?pid=" & Master.DataPID.ToString)
    End Sub


   

    Private Sub grid1_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles grid1.ItemCommand
        If e.CommandName = "delete" Then
            Dim intP85ID As Integer = CInt(e.Item.Attributes("p85id"))
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                SetupPersonsOffer()
                grid1.Rebind()
                ShowNeedSaveMessage()
            End If
        End If
    End Sub

    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
            With CType(CType(e.Item, GridDataItem)("p85OtherKey2").FindControl("combo1"), DropDownList)
                If cRec.p85OtherKey2 > 0 Then
                    .SelectedValue = cRec.p85OtherKey2.ToString
                End If
                .Attributes.Item("onchange") = "save_cellvalue(" & cRec.PID.ToString & ",'p85OtherKey2',this.value)"
            End With

            e.Item.Attributes.Item("p85id") = cRec.PID.ToString
        End If
    End Sub



    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource

        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
       
        grid1.DataSource = lis.Where(Function(p) p.p85Prefix = "p46")

        stat1.RefreshData(Master.Factory, Master.DataPID, ViewState("guid"))
    End Sub

    Private Sub cmdInsertPersons_Click(sender As Object, e As EventArgs) Handles cmdInsertPersons.Click
        Dim b As Boolean = False, mq As New BO.myQueryP32
        mq.p33ID = BO.p33IdENUM.Cas
        mq.Billable = BO.BooleanQueryMode.TrueQuery
        Dim lisP32 As IEnumerable(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq)
        Dim datRate As Date = Now, intP32ID As Integer = lisP32(0).PID
        datRate = ViewState("d1")
        Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Where(Function(p) p.p85Prefix = "p46")
        For Each ri As RepeaterItem In rpJ02.Items
            With CType(ri.FindControl("Person"), CheckBox)
                If .Checked Then
                    Dim intJ02ID As Integer = CInt(CType(ri.FindControl("hidJ02ID"), HiddenField).Value)
                    Dim cJ02 As BO.j02Person = Master.Factory.j02PersonBL.Load(intJ02ID)
                    If lisTemp.Where(Function(p) p.p85OtherKey1 = cJ02.PID).Count = 0 Then
                        b = True
                        Dim cTemp As New BO.p85TempBox()
                        With cTemp
                            .p85GUID = ViewState("guid")
                            .p85Prefix = "p46"
                            .p85OtherKey1 = intJ02ID
                            .p85FreeText01 = cJ02.FullNameDesc
                            .p85FreeNumber01 = Master.Factory.p31WorksheetBL.LoadRate(False, datRate, intJ02ID, Me.CurrentP41ID, intP32ID, 0)    'fakturační sazba
                            .p85FreeNumber02 = Master.Factory.p31WorksheetBL.LoadRate(True, datRate, intJ02ID, Me.CurrentP41ID, intP32ID, 0)    'nákladová sazba
                        End With
                        Master.Factory.p85TempBoxBL.Save(cTemp)
                    End If
                    
                End If
            End With
        Next
        If b Then
            grid1.Rebind()
            SetupPersonsOffer()
        Else
            Master.Notify("Musíte zaškrtnout minimálně jednu osobu.", NotifyLevel.WarningMessage)
        End If

    End Sub

    
   

    Private Sub ShowNeedSaveMessage()
        Master.master_show_message("Provedené změny je třeba uložit tlačítkem [Uložit změny].")
    End Sub

    

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        ShowNeedSaveMessage()


    End Sub

    Private Sub cmdRefreshStatement_Click(sender As Object, e As EventArgs) Handles cmdRefreshStatement.Click
        stat1.RefreshData(Master.Factory, Master.DataPID, ViewState("guid"))
    End Sub

    

  
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "save"
                Dim lisP46 As New List(Of BO.p46BudgetPerson)
                Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"), True)
                For Each cTemp In lisTemp.Where(Function(p) p.p85Prefix = "p46")
                    Dim item As New BO.p46BudgetPerson
                    With cTemp
                        item.SetPID(.p85DataPID)
                        item.j02ID = .p85OtherKey1
                        item.p46HoursBillable = .p85FreeFloat01
                        item.p46HoursNonBillable = .p85FreeFloat02
                        item.p46HoursTotal = item.p46HoursBillable + item.p46HoursNonBillable
                        If .p85OtherKey2 = 0 Then
                            item.p46ExceedFlag = BO.p46ExceedFlagENUM.StrictFaStrictNefa
                        Else
                            item.p46ExceedFlag = .p85OtherKey2
                        End If
                        item.p46Description = .p85FreeText02
                        item.IsSetAsDeleted = .p85IsDeleted
                    End With
                    lisP46.Add(item)
                Next

                With Master.Factory.p45BudgetBL
                    If .SaveListP46(Master.DataPID, lisP46) Then
                        Master.CloseAndRefreshParent("p45-save")
                    Else
                        Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                    End If

                End With
        End Select
    End Sub
End Class