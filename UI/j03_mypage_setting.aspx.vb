Public Class j03_mypage_setting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _lisX55 As IEnumerable(Of BO.x55HtmlSnippet)
    Private _curIndex As Integer

    Private Sub j03_mypage_setting_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                ViewState("guid") = BO.BAS.GetGUID()
                .HeaderText = "Nastavení osobní stránky"
                .HeaderIcon = "Images/setting_32.png"
                .AddToolbarButton("Uložit změny", "save", , "Images/save.png")
            End With
            RefreshRecord()

        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.x58UserPage = Master.Factory.x58UserPage.LoadByJ03(Master.Factory.SysUser.PID)
        Dim lisX57 As IEnumerable(Of BO.x57UserPageBinding) = Master.Factory.x58UserPage.GetList_x57(cRec.PID)


        For Each c In lisX57
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid")
                .p85DataPID = c.x57ID
                .p85OtherKey1 = c.x55ID
                .p85FreeText01 = c.x57Height
                .p85FreeText02 = c.x57Width
                .p85FreeText03 = c.x57DockID
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTemp()
    End Sub
    Private Sub RefreshTemp()
        _curIndex = 0
        rp1.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rp1.DataBind()
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        SaveTemp()
        If Master.Factory.p85TempBoxBL.GetList(ViewState("guid")).Count = 9 Then
            Master.Notify("Na osobní stránce může být maximálně 9 gadgetů.", NotifyLevel.InfoMessage)
            Return
        End If
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
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("x55ID"), DropDownList).SelectedValue)
              

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
        _curIndex += 1
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("Index"), Label).Text = "gadget #" & _curIndex.ToString

        If _lisX55 Is Nothing Then
            _lisX55 = Master.Factory.x55HtmlSnippetBL.GetList(New BO.myQuery)

        End If
        With CType(e.Item.FindControl("x55ID"), DropDownList)
            .DataSource = _lisX55
            .DataBind()
            .Items.Insert(0, "")
            Try
                .SelectedValue = cRec.p85OtherKey1.ToString
            Catch ex As Exception
            End Try
        End With
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            SaveTemp()

            Dim cRec As BO.x58UserPage = Master.Factory.x58UserPage.LoadByJ03(Master.Factory.SysUser.PID)
            Dim lisX57 As New List(Of BO.x57UserPageBinding)
            Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            For Each cTemp In lisTemp.Where(Function(p) p.p85OtherKey1 <> 0)
                Dim c As New BO.x57UserPageBinding
                c.x55ID = cTemp.p85OtherKey1
                If cTemp.p85DataPID <> 0 Then
                    c.x57DockID = cTemp.p85FreeText03
                Else
                    c.x57DockID = FindFreeDockID(lisTemp)
                    cTemp.p85FreeText03 = c.x57DockID
                End If

                lisX57.Add(c)
            Next
            If Master.Factory.x58UserPage.Save(cRec, lisX57) Then
                Master.CloseAndRefreshParent()
            Else
                Master.Notify(Master.Factory.x58UserPage.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End If
    End Sub

    Private Function FindFreeDockID(lisTemp As IEnumerable(Of BO.p85TempBox)) As String
        For i As Integer = 1 To 9
            Dim s As String = "dock" & i.ToString
            If lisTemp.Where(Function(p) p.p85FreeText03 = s).Count = 0 Then
                Return s
            End If
        Next
        Return ""
    End Function
End Class