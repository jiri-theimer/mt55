Public Class x67_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub x67_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return DirectCast(BO.BAS.IsNullInt(Me.x29ID.SelectedValue), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            basUI.SelectDropdownlistValue(Me.x29ID, CInt(value).ToString)
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení role"
            End With
            If Master.DataPID = 0 Then
                ViewState("prefix") = Left(Request.Item("prefix"), 3)
                If BO.BAS.GetX29FromPrefix(ViewState("prefix")) = BO.x29IdEnum._NotSpecified Then
                    Master.StopPage("Na vstupu chybí prefix (" & ViewState("prefix") & ").", True)
                Else
                    Me.CurrentX29ID = BO.BAS.IsNullInt(BO.BAS.GetX29FromPrefix(ViewState("prefix")))
                End If
            End If
            

            RefreshRecord()
            RefreshO28()

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If
        End If
    End Sub

    Private Sub RefreshO28()
        If Me.CurrentX29ID <> BO.x29IdEnum.p41Project Then
            panO28.Visible = False
            Return
        Else
            panO28.Visible = True
        End If
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        rp1.DataSource = Master.Factory.p34ActivityGroupBL.GetList(mq)
        rp1.DataBind()

        If Master.DataPID = 0 Then
            Return
        End If

        Dim lisO28 As IEnumerable(Of BO.o28ProjectRole_Workload) = Master.Factory.x67EntityRoleBL.GetList_o28(BO.BAS.ConvertInt2List(Master.DataPID))
        For Each c In lisO28
            For Each ri As RepeaterItem In rp1.Items
                If CType(ri.FindControl("_p34id"), HiddenField).Value = c.p34ID.ToString Then
                    Dim cbx As DropDownList = CType(ri.FindControl("_o28entryflag"), DropDownList)
                    basUI.SelectDropdownlistValue(cbx, CInt(c.o28EntryFlag).ToString)
                    cbx = CType(ri.FindControl("_o28permflag"), DropDownList)
                    basUI.SelectDropdownlistValue(cbx, CInt(c.o28PermFlag).ToString)
                    Exit For
                End If
            Next
        Next
    End Sub
    Private Sub SetupX53List()
        Me.x53ids.DataSource = Master.Factory.ftBL.GetList_X53(New BO.myQuery).Where(Function(p) p.x29ID = Me.CurrentX29ID)
        Me.x53ids.DataBind()
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            SetupX53List()
            Return
        End If

        Dim cRec As BO.x67EntityRole = Master.Factory.x67EntityRoleBL.Load(Master.DataPID)
        With cRec
            Me.CurrentX29ID = .x29ID
            SetupX53List()

            Me.x67Name.Text = .x67Name
            Me.x67Ordinary.Value = .x67Ordinary

            Master.Timestamp = .Timestamp

            Dim lis As IEnumerable(Of BO.x53Permission) = Master.Factory.x67EntityRoleBL.GetList_BoundX53(.PID)
            basUI.CheckItems(Me.x53ids, lis.Select(Function(p) p.PID).ToList)
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.x67EntityRoleBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("x67-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.x67EntityRoleBL
            Dim cRec As BO.x67EntityRole = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.x67EntityRole)
            cRec.x29ID = Me.CurrentX29ID
            cRec.x67Name = Me.x67Name.Text
            cRec.x67Ordinary = BO.BAS.IsNullInt(Me.x67Ordinary.Value)
            cRec.ValidFrom = Master.RecordValidFrom
            cRec.ValidUntil = Master.RecordValidUntil

            Dim mq As New BO.myQuery
            mq.AddItemToPIDs(-1)
            For Each intX53ID As Integer In basUI.GetCheckedItems(Me.x53ids)
                mq.AddItemToPIDs(intX53ID)
            Next
            Dim lisX53 As List(Of BO.x53Permission) = Master.Factory.ftBL.GetList_X53(mq).ToList
            If .Save(cRec, lisX53) Then
                Master.DataPID = .LastSavedPID

                If Me.CurrentX29ID = BO.x29IdEnum.p41Project Then    'projektová role má ještě další nastavení
                    Dim lisO28 As New List(Of BO.o28ProjectRole_Workload)
                    For Each ri As RepeaterItem In rp1.Items
                        Dim c As New BO.o28ProjectRole_Workload
                        c.o28EntryFlag = CInt(CType(ri.FindControl("_o28entryflag"), DropDownList).SelectedValue)
                        c.o28PermFlag = CInt(CType(ri.FindControl("_o28permflag"), DropDownList).SelectedValue)
                        c.p34ID = CInt(CType(ri.FindControl("_p34id"), HiddenField).Value)
                        lisO28.Add(c)
                    Next
                    .SaveO28(Master.DataPID, lisO28)

                    cRec = .LoadChild(Master.DataPID)
                    If Not cRec Is Nothing Then
                        .SaveO28(cRec.PID, lisO28)
                    End If
                End If

                Master.CloseAndRefreshParent("x67-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p34ActivityGroup = CType(e.Item.DataItem, BO.p34ActivityGroup)
       
       
        With cRec
            CType(e.Item.FindControl("_p34name"), Label).Text = cRec.p34Name
            If cRec.IsClosed Then
                CType(e.Item.FindControl("_p34name"), Label).Font.Strikeout = True
            End If
            CType(e.Item.FindControl("_p34id"), HiddenField).Value = cRec.PID.ToString
            
        End With
    End Sub
End Class