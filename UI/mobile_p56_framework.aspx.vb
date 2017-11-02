Public Class mobile_p56_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_p56_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .MenuPrefix = "p56"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p56_framework_detail-pid")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                End With
                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p56_framework_detail-pid", "O"))
                    If .DataPID = 0 Then Response.Redirect("mobile_entity_framework_missing.aspx?prefix=p56")
                Else
                    If .DataPID <> BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("p56_framework_detail-pid", "O")) Then
                        .Factory.j03UserBL.SetUserParam("p56_framework_detail-pid", .DataPID.ToString)
                    End If
                End If

            End With

            RefreshRecord()
            history1.RefreshData(Master.Factory, BO.x29IdEnum.p56Task, Master.DataPID)
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        ''Dim mq As New BO.myQueryP56
        ''mq.AddItemToPIDs(Master.DataPID)
        ''mq.Closed = BO.BooleanQueryMode.NoQuery
        ''Dim lis As IEnumerable(Of BO.p56TaskWithWorksheetSum) = Master.Factory.p56TaskBL.GetList_WithWorksheetSum(mq)
        Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(Master.DataPID)
        Dim cRecSum As BO.p56TaskSum = Master.Factory.p56TaskBL.LoadSumRow(Master.DataPID)
        Dim cClient As BO.p28Contact = Nothing
        With cRec
            Me.linkWorkflow.NavigateUrl = "mobile_workflow_dialog.aspx?prefix=p56&pid=" & .PID.ToString
            Me.p56Code.Text = .p56Code
            Me.RecordHeader.Text = BO.BAS.OM3(.p57Name & ": " & .p56Code, 30)
            Me.RecordHeader.NavigateUrl = "mobile_p56_framework.aspx?pid=" & .PID.ToString
            If .IsClosed Then RecordHeader.Font.Strikeout = True
            Me.RecordName.Text = .p56Name
            Me.cmdP31Grid.NavigateUrl = "mobile_grid.aspx?source=task&prefix=p31&masterprefix=p56&masterpid=" & .PID.ToString
            Me.Project.NavigateUrl = "mobile_p41_framework.aspx?pid=" & .p41ID.ToString
            Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID)

            Me.p57Name.Text = .p57Name

            If .b02ID > 0 Then
                Me.b02Name.Text = .b02Name
                If .b02Color <> "" Then
                    Me.b02Name.Style.Item("background-color") = .b02Color
                End If
            Else
                trB02.Visible = False
            End If
            If Not .p56PlanFrom Is Nothing Then
                Me.p56PlanFrom.Text = BO.BAS.FD(.p56PlanFrom, True, True)
            Else
                trp56PlanFrom.Visible = False
            End If

            Me.p56PlanUntil.Text = BO.BAS.FD(.p56PlanUntil, True, True)
            Me.Owner.Text = .Owner
            Me.Timestamp.Text = .Timestamp
            Me.p56Description.Text = BO.BAS.CrLfText2Html(.p56Description)

        End With

        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p56Task, cRec.PID)

        Me.roles_project.RefreshData(lisX69, cRec.PID)

        Dim mqO23 As New BO.myQueryO23(0)
        mqO23.p56IDs = BO.BAS.ConvertInt2List(Master.DataPID)
        mqO23.SpecificQuery = BO.myQueryO23_SpecificQuery.AllowedForRead
        Dim lisO23 As IEnumerable(Of BO.o23Doc) = Master.Factory.o23DocBL.GetList(mqO23)
        If lisO23.Count > 0 Then
            Me.boxO23.Visible = True
            notepad1.RefreshData(lisO23, cRec.PID)
            CountO23.Text = lisO23.Count.ToString
        Else
            Me.boxO23.Visible = False
        End If

        labels1.RefreshData(Master.Factory, BO.x29IdEnum.p56Task, cRec.PID, True)
        boxX18.Visible = labels1.ContainsAnyData


        With cRec
            Me.Hours_Orig.Text = BO.BAS.FN(cRecSum.Hours_Orig)
            If cRecSum.Expenses_Orig <> 0 Then
                trExpenses.Visible = True
                Me.Expenses_Orig.Text = BO.BAS.FN(cRecSum.Expenses_Orig)
            End If
            If cRec.p56Plan_Hours > 0 Then
                trPlanHours.Visible = True
                p56Plan_Hours.Text = BO.BAS.FN(.p56Plan_Hours)
                Select Case .p56Plan_Hours - cRecSum.Hours_Orig
                    Case Is > 0
                        Me.PlanHoursSummary.Text += "zbývá <span style='color:blue;'>" & BO.BAS.FN(.p56Plan_Hours - cRecSum.Hours_Orig) & "h.</span>"
                    Case Is < 0
                        Me.PlanHoursSummary.Text += " <img src='Images/warning.png'/> vykázáno přes plán <span style='color:red;'>" & BO.BAS.FN(cRecSum.Hours_Orig - .p56Plan_Hours) & "h.</span>"
                    Case 0
                        Me.PlanHoursSummary.Text += "vykázáno přesně podle plánu."
                End Select
            End If
            If .p56Plan_Expenses > 0 Then
                trPlanExpenses.Visible = True
                p56Plan_Expenses.Text = BO.BAS.FN(.p56Plan_Expenses)
                Select Case .p56Plan_Expenses - cRecSum.Expenses_Orig
                    Case Is > 0
                        Me.PlanExpensesSummary.Text += "zbývá <span style='color:blue;'>" & BO.BAS.FN(.p56Plan_Expenses - cRecSum.Expenses_Orig) & ",-</span>"
                    Case Is < 0
                        PlanExpensesSummary.Text += " <img src='Images/warning.png'/> vykázáno přes plán <span style='color:red;'>" & BO.BAS.FN(cRecSum.Expenses_Orig - .p56Plan_Expenses) & ",-.</span>"
                    Case 0
                        PlanExpensesSummary.Text = "vykázáno přesně podle plánu."
                End Select
            End If
        End With
    End Sub

    

  
End Class