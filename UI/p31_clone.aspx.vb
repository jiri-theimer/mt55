Public Class p31_clone
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_clone_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
           
            With Master
                .HeaderText = "Hromadně zkopírovat vybrané worksheet záznamy"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
            End With

            Dim pids As New List(Of Integer)
            If Request.Item("pids") <> "" Then
                pids = BO.BAS.ConvertPIDs2List(Request.Item("pids")).Distinct.ToList
            End If
            If pids.Count = 0 Then
                Master.StopPage("Na vstupu chybí vybrané záznamy.")
            End If
            SetupData(pids)
            
        End If
    End Sub

    Private Sub SetupData(pids As List(Of Integer))
        Dim mq As New BO.myQueryP31
        mq.PIDs = pids
        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(mq)
        With Master.Factory.SysUser
            If Not (.IsMasterPerson Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Creator)) Then 'není nadřízenou osobu ani nemá právo zapisovat do všech projektů -> tak v kopírovaném záznamu předvyplnit
                For Each c In lis
                    If c.j02ID <> Master.Factory.SysUser.j02ID Then
                        c.j02ID = Master.Factory.SysUser.j02ID
                        c.Person = Master.Factory.SysUser.PersonDesc
                    End If
                Next
            End If
        End With
        If lis.Where(Function(p) p.p33ID <> 1).Count > 0 Then
            Master.Notify("Na vstupu jsou i jiné než časové úkony.<hr>Do hromadného kopírování lze zařadit pouze časové úkony.", NotifyLevel.InfoMessage)
        End If
        rp1.DataSource = lis.Where(Function(p) p.p33ID = 1)
        rp1.DataBind()
    End Sub

    Private Sub rp1_Init(sender As Object, e As EventArgs) Handles rp1.Init

    End Sub

    Private Sub rp1_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemCreated
        With CType(e.Item.FindControl("p31Date"), Telerik.Web.UI.RadDatePicker)
            .SharedCalendar = Me.SharedCalendar1
        End With
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)
        CType(e.Item.FindControl("p31Text"), TextBox).Text = cRec.p31Text
        CType(e.Item.FindControl("Project"), Label).Text = cRec.ClientName & " - " & cRec.p41Name
        CType(e.Item.FindControl("p32Name"), Label).Text = cRec.p32Name
        CType(e.Item.FindControl("Person"), Label).Text = cRec.Person
        CType(e.Item.FindControl("pid"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("p41id"), HiddenField).Value = cRec.p41ID.ToString
        With CType(e.Item.FindControl("p31Date"), Telerik.Web.UI.RadDatePicker)
            .SelectedDate = cRec.p31Date
        End With

        If cRec.IsRecommendedHHMM() Then
            CType(e.Item.FindControl("p31Value_Orig"), TextBox).Text = cRec.p31HHMM_Orig
        Else
            CType(e.Item.FindControl("p31Value_Orig"), TextBox).Text = cRec.p31Hours_Orig.ToString
        End If

    End Sub

    Private Sub cmdSetProject_Click(sender As Object, e As EventArgs) Handles cmdSetProject.Click
        If Me.p41id.Value = "" Then
            Master.Notify("Musíte vybrat projekt.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(CInt(p41id.Value))
        For Each ri As RepeaterItem In rp1.Items
            With CType(ri.FindControl("p41id"), HiddenField)
                .Value = p41id.Value
            End With
            With CType(ri.FindControl("Project"), Label)
                .Text = cP41.FullName
            End With
        Next
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim bolOK As Boolean = False, errs As New List(Of String), err_pids As New List(Of Integer)
            For Each ri As RepeaterItem In rp1.Items
                Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(CInt(CType(ri.FindControl("pid"), HiddenField).Value))

                Dim strText As String = CType(ri.FindControl("p31Text"), TextBox).Text
                Dim d As Date = CType(ri.FindControl("p31Date"), Telerik.Web.UI.RadDatePicker).SelectedDate
                Dim strHours As String = CType(ri.FindControl("p31Value_Orig"), TextBox).Text

                Dim lisFF As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p31Worksheet, cRec.PID, cRec.p34ID)
                Dim c As New BO.p31WorksheetEntryInput
                c.p41ID = CInt(CType(ri.FindControl("p41id"), HiddenField).Value)
                c.p31Text = CType(ri.FindControl("p31Text"), TextBox).Text
                c.p31Date = CType(ri.FindControl("p31Date"), Telerik.Web.UI.RadDatePicker).SelectedDate
                c.p31HoursEntryflag = BO.p31HoursEntryFlagENUM.Hodiny
                c.Value_Orig = CType(ri.FindControl("p31Value_Orig"), TextBox).Text
                c.Value_Orig_Entried = CType(ri.FindControl("p31Value_Orig"), TextBox).Text
                c.p32ID = cRec.p32ID
                c.p34ID = cRec.p34ID

                If Not (Master.Factory.SysUser.IsMasterPerson Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_Creator)) Then 'není nadřízenou osobu ani nemá právo zapisovat do všech projektů -> tak v kopírovaném záznamu předvyplnit
                    c.j02ID = Master.Factory.SysUser.j02ID
                Else
                    c.j02ID = cRec.j02ID
                End If
                c.j02ID_ContactPerson = cRec.j02ID_ContactPerson

                If Master.Factory.p31WorksheetBL.SaveOrigRecord(c, lisFF) Then
                    bolOK = True
                Else
                    errs.Add(Master.Factory.p31WorksheetBL.ErrorMessage)
                    err_pids.Add(cRec.PID)
                End If
            Next
            If bolOK Then
                If errs.Count = 0 Then
                    Master.CloseAndRefreshParent()
                Else
                    Master.Notify(String.Join("<hr>", errs), NotifyLevel.ErrorMessage)
                    SetupData(err_pids)
                End If

            Else
                Master.Notify("Ani jeden nově vygenerovaný úkon!" & "<hr>" & String.Join("<hr>", errs), NotifyLevel.ErrorMessage)
            End If

        End If
    End Sub
End Class