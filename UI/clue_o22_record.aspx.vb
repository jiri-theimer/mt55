Public Class clue_o22_record
    Inherits System.Web.UI.Page
    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
           
            RefreshRecord()

            If Request.Item("mode") = "readonly" Then
                panContainer.Style.Clear()
                cmDetail.Visible = False

                ph1.Visible = False
            End If
            comments1.RefreshData(Master.Factory, BO.x29IdEnum.o22Milestone, Master.DataPID)
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cO22 As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)
        With cO22
            Master.DataPID = .PID
            If .p41ID <> 0 Then
                ViewState("masterprefix") = "p41"
                ViewState("masterpid") = .p41ID.ToString
                ph1.Text = .Project
            End If
            If .p28ID <> 0 Then
                ViewState("masterprefix") = "p28"
                ViewState("masterpid") = .p28ID.ToString
                ph1.Text = .Contact
            End If
            If .j02ID <> 0 Then
                ViewState("masterprefix") = "j02"
                ViewState("masterpid") = .j02ID.ToString
                ph1.Text = .Person
            End If
            If .p56ID <> 0 Then
                ViewState("masterprefix") = "p56"
                ViewState("masterpid") = .p56ID.ToString
                ph1.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p56Task, .p56ID)
            End If
            If .p91ID <> 0 Then
                ViewState("masterprefix") = "p91"
                ViewState("masterpid") = .p91ID.ToString
                ph1.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p91Invoice, .p91ID)
            End If
            If .o21Flag = BO.o21FlagEnum.DeadlineOrMilestone Then
                img1.ImageUrl = "Images/calendar_32.png"
            Else
                img1.ImageUrl = "Images/event_32.png"
            End If
            Me.o21Name.Text = .o21Name

            Me.Period.Text = .Period
            Me.o22Name.Text = .o22Name
            Me.o22Location.Text = .o22Location
            If .o22Location = "" Then Me.lblLocation.Visible = False
            Me.o22Description.Text = .o22Description
            Me.Timestamp.Text = .Timestamp
            If .o22DateUntil.Value < Now Then
                Me.Period.Font.Strikeout = True
                Me.lblPeriodMessage.Visible = True
            End If
            If Not BO.BAS.IsNullDBDate(.o22ReminderDate) Is Nothing Then
                Me.o22ReminderDate.Text = BO.BAS.FD(.o22ReminderDate.Value, True, True)
                If .o22ReminderDate.Value < Now Then
                    Me.o22ReminderDate.Font.Strikeout = True
                End If
            Else
                Me.lblReminder.Visible = False
            End If
            If .j02ID_Owner = Master.Factory.SysUser.j02ID Or Master.Factory.TestPermission(BO.x53PermValEnum.GR_Admin) Then
                cmDetail.Visible = True
            Else
                cmDetail.Visible = False
            End If
        End With

        rpO20.DataSource = Master.Factory.o22MilestoneBL.GetList_o20(Master.DataPID)
        rpO20.DataBind()
        If rpO20.Items.Count = 0 Then rpO20.Visible = False

        labels1.RefreshData(Master.Factory, BO.x29IdEnum.o22Milestone, Master.DataPID, True)

    End Sub

    
End Class