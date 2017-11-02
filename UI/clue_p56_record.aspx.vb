Public Class clue_p56_record
    Inherits System.Web.UI.Page
 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

            RefreshRecord()



        End If
    End Sub

    Private Sub RefreshRecord()
        ''Dim mq As New BO.myQueryP56
        ''mq.AddItemToPIDs(Master.DataPID)
        ''mq.Closed = BO.BooleanQueryMode.NoQuery
        ''Dim lis As IEnumerable(Of BO.p56TaskWithWorksheetSum) = Master.Factory.p56TaskBL.GetList_WithWorksheetSum(mq)
        Dim cRec As BO.p56Task = Master.Factory.p56TaskBL.Load(Master.DataPID)
        Dim cP57 As BO.p57TaskType = Master.Factory.p57TaskTypeBL.Load(cRec.p57ID)
        Dim cRecSum As BO.p56TaskSum = Master.Factory.p56TaskBL.LoadSumRow(Master.DataPID)
        Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, cRec.p41ID)
        With cRec
            Master.HeaderText = .p57Name & ": " & .p56Code
            ph1.Text = cRec.p57Name & ": " & .p56Code
            Me.p56Name.Text = .p56Name
            Me.p56Name.Font.Strikeout = .IsClosed
            If .p56Description <> "" Then
                Me.p56Description.Text = BO.BAS.CrLfText2Html(.p56Description)
            Else
                panBody.Visible = False
            End If
            If .p59ID_Submitter > 0 Then
                Me.p59name_submitter.Text = .p59NameSubmitter
            Else
                Me.p59name_submitter.Visible = False : Me.lblPriority.Visible = False
            End If
            
            Me.Hours_Orig.Text = BO.BAS.FN(cRecSum.Hours_Orig)
            Me.b02Name.Text = .b02Name
            Me.Timestamp.Text = .Timestamp
            Me.Owner.Text = .Owner
        End With
        Me.RolesInLine.Text = Master.Factory.p56TaskBL.GetRolesInline(Master.DataPID)
        If Not BO.BAS.IsNullDBDate(cRec.p56PlanUntil) Is Nothing Then
            Me.p56PlanUntil.Text = BO.BAS.FD(cRec.p56PlanUntil, True, True)
            If cRec.p57PlanDatesEntryFlag <> 4 Then
                If cRec.p56PlanUntil < Now Then
                    Me.p56PlanUntil.Text += "...je po termínu!" : p56PlanUntil.ForeColor = Drawing.Color.Red
                Else
                    p56PlanUntil.ForeColor = Drawing.Color.Green
                End If
            End If
        End If
        If Not BO.BAS.IsNullDBDate(cRec.p56PlanFrom) Is Nothing Then
            Me.p56PlanFrom.Text = BO.BAS.FD(cRec.p56PlanFrom, True, True)
        Else
            lblp56PlanFrom.Visible = False
        End If
        With cP57
            If .p57Caption_PlanFrom <> "" Then
                lblp56PlanFrom.Text = .p57Caption_PlanFrom & ":"
            End If
            If .p57Caption_PlanUntil <> "" Then
                lblp56PlanUntil.Text = .p57Caption_PlanUntil & ":"
            End If
        End With
        
        If cRecSum.Expenses_Orig > 0 Then
            trExpenses.Visible = True
            Me.Expenses_Orig.Text = BO.BAS.FN(cRecSum.Expenses_Orig)

        End If
        If cRec.p56Plan_Hours > 0 Then
            trPlanHours.Visible = True
            p56Plan_Hours.Text = BO.BAS.FN(cRec.p56Plan_Hours)
            Select Case cRec.p56Plan_Hours - cRecSum.Hours_Orig
                Case Is > 0
                    Me.PlanHoursSummary.Text += "zbývá vykázat <span style='color:blue;'>" & BO.BAS.FN(cRec.p56Plan_Hours - cRecSum.Hours_Orig) & "h.</span>"
                Case Is < 0
                    Me.PlanHoursSummary.Text += " <img src='Images/warning.png'/> vykázáno přes plán <span style='color:red;'>" & BO.BAS.FN(cRecSum.Hours_Orig - cRec.p56Plan_Hours) & "h.</span>"
                Case 0
                    Me.PlanHoursSummary.Text += "vykázáno přesně podle plánu."
            End Select
            
            
        End If
        If cRec.p56Plan_Expenses > 0 Then
            trPlanExpenses.Visible = True
            p56Plan_Expenses.Text = BO.BAS.FN(cRec.p56Plan_Expenses)
            Select Case cRec.p56Plan_Expenses - cRecSum.Expenses_Orig
                Case Is > 0
                    Me.PlanExpensesSummary.Text += "zbývá vykázat <span style='color:blue;'>" & BO.BAS.FN(cRec.p56Plan_Expenses - cRecSum.Expenses_Orig) & ",-</span>"
                Case Is < 0
                    PlanExpensesSummary.Text += " <img src='Images/warning.png'/> vykázáno přes plán <span style='color:red;'>" & BO.BAS.FN(cRecSum.Expenses_Orig - cRec.p56Plan_Expenses) & ",-.</span>"
                Case 0
                    PlanExpensesSummary.Text = "vykázáno přesně podle plánu."
            End Select
            
        End If

        

        If cRec.IsClosed Then
            img1.ImageUrl = "Images/bin_32.png"
            p56Name.Font.Strikeout = True
        End If
       
        labels1.RefreshData(Master.Factory, BO.x29IdEnum.p56Task, Master.DataPID, True)
        Me.comments1.RefreshData(Master.Factory, BO.x29IdEnum.p56Task, Master.DataPID)
        tags1.RefreshData(Master.DataPID)
    End Sub

    

    Private Sub clue_p56_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
      
    End Sub
End Class