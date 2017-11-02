Public Class p48_multiple_edit_delete
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _lastP34ID As Integer
    Private _lastListP32 As IEnumerable(Of BO.p32Activity)

    Private Sub p48_multiple_edit_delete_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.TestNeededPermission(BO.x53PermValEnum.GR_P48_Creator)
            ViewState("guid") = BO.BAS.GetGUID()
            ViewState("p48ids") = Request.Item("p48ids")
            With Master
                .HeaderText = "Upravit operativní plán"
                .AddToolbarButton("Odstranit všechny záznamy", "delete", , "Images/delete.png")
                .AddToolbarButton("Uložit změny", "save", , "Images/save.png")
                If ViewState("p48ids") = "" Then
                    .StopPage("p48ids missing")
                End If
            End With
            Dim a() As String = Split(ViewState("p48ids"), ",")
            Dim mq As New BO.myQueryP48
            For i As Integer = 0 To UBound(a)
                mq.AddItemToPIDs(CInt(a(i)))
            Next
            Dim lis As IEnumerable(Of BO.p48OperativePlan) = Master.Factory.p48OperativePlanBL.GetList(mq)
            For Each cRec In lis
                Dim c As New BO.p85TempBox
                With c
                    .p85GUID = ViewState("guid")
                    .p85DataPID = cRec.PID
                    .p85FreeDate01 = cRec.p48Date
                    .p85OtherKey1 = cRec.j02ID
                    .p85OtherKey2 = cRec.p41ID
                    .p85FreeText01 = cRec.Person
                    .p85FreeText02 = cRec.Project
                    .p85FreeFloat01 = cRec.p48Hours
                    .p85Message = cRec.p48Text
                    .p85FreeText03 = cRec.p34Name
                    .p85OtherKey3 = cRec.p34ID
                    .p85FreeText04 = cRec.p32Name
                    .p85OtherKey4 = cRec.p32ID
                    .p85FreeText05 = cRec.p48TimeFrom
                    .p85FreeText06 = cRec.p48TimeUntil
                End With
                Master.Factory.p85TempBoxBL.Save(c)
            Next






            RefreshRecord()
            RefreshTempList()
        End If
    End Sub
    Private Sub RefreshRecord()

    End Sub
    Private Sub RefreshTempList()
        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        
        rp1.DataSource = lis.OrderBy(Function(p) p.p85FreeText01).ThenBy(Function(p) p.p85FreeDate01)
        rp1.DataBind()

    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(e.Item.FindControl("p85id"), HiddenField).Value)
        Dim c As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)
        Master.Factory.p85TempBoxBL.Delete(c)

        RefreshTempList()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("p41id"), HiddenField).Value = cRec.p85OtherKey2.ToString
        CType(e.Item.FindControl("p48Date"), Label).Text = BO.BAS.FD(cRec.p85FreeDate01)
        CType(e.Item.FindControl("Person"), Label).Text = cRec.p85FreeText01
        CType(e.Item.FindControl("Project"), Label).Text = cRec.p85FreeText02
        CType(e.Item.FindControl("p48Hours"), TextBox).Text = cRec.p85FreeFloat01.ToString
        CType(e.Item.FindControl("p48Text"), TextBox).Text = cRec.p85Message
        CType(e.Item.FindControl("p34Name"), Label).Text = cRec.p85FreeText03
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Text = cRec.p85FreeText05
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Text = cRec.p85FreeText06
        If _lastP34ID <> cRec.p85OtherKey3 Then
            _lastP34ID = cRec.p85OtherKey3
            Dim mq As New BO.myQueryP32
            mq.p34ID = _lastP34ID
            _lastListP32 = Master.Factory.p32ActivityBL.GetList(mq)
        End If
        With CType(e.Item.FindControl("p32ID"), DropDownList)
            .DataSource = _lastListP32
            .DataBind()
            .Items.Insert(0, "--Vybrat aktivitu--")
            Try
                If cRec.p85OtherKey4 > 0 Then .SelectedValue = cRec.p85OtherKey4
            Catch ex As Exception
            End Try

        End With
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("cas_od") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("hodiny") = e.Item.FindControl("p48Hours").ClientID

        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("cas_od") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("hodiny") = e.Item.FindControl("p48Hours").ClientID

        CType(e.Item.FindControl("p48Hours"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48Hours"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID

        _lastP34ID = cRec.p85OtherKey3
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Save2Temp()

        Select Case strButtonValue
            'Case "delete"
            '    Dim a() As String = Split(ViewState("p48ids"), ",")

            '    For i As Integer = 0 To UBound(a)
            '        Master.Factory.p48OperativePlanBL.Delete(CInt(a(i)))
            '    Next
            '    Master.CloseAndRefreshParent()
            Case "save", "delete"
                Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"), True)
                Dim lisP48 As New List(Of BO.p48OperativePlan)
                For Each c In lis
                    Dim intPID As Integer = c.p85DataPID
                    Dim cRec As BO.p48OperativePlan = Master.Factory.p48OperativePlanBL.Load(intPID)
                    If c.p85IsDeleted Or strButtonValue = "delete" Then cRec.SetAsDeleted()

                    cRec.p48Text = c.p85Message
                    cRec.p32ID = c.p85OtherKey4
                    cRec.p48Hours = c.p85FreeFloat01
                    cRec.p48TimeFrom = c.p85FreeText05
                    cRec.p48TimeUntil = c.p85FreeText06
                    lisP48.Add(cRec)
                Next
                If Master.Factory.p48OperativePlanBL.SaveBatch(lisP48) Then
                    Master.CloseAndRefreshParent()
                Else
                    Master.Notify(Master.Factory.p48OperativePlanBL.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
                
        End Select
    End Sub

    Private Sub Save2Temp()
        Dim ct As New BO.clsTime
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = CInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim c As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)

            c.p85Message = CType(ri.FindControl("p48Text"), TextBox).Text
            c.p85OtherKey4 = BO.BAS.IsNullInt(CType(ri.FindControl("p32ID"), DropDownList).SelectedValue)

            Dim strHours As String = CType(ri.FindControl("p48Hours"), TextBox).Text
            c.p85FreeFloat01 = ct.ShowAsDec(strHours)
            c.p85FreeText05 = CType(ri.FindControl("p48TimeFrom"), TextBox).Text
            c.p85FreeText06 = CType(ri.FindControl("p48TimeUntil"), TextBox).Text

            Master.Factory.p85TempBoxBL.Save(c)
        Next
    End Sub

End Class