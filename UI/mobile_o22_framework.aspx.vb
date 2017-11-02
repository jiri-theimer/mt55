Public Class mobile_o22_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile

    Private Sub mobile_o22_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("mobile_o22_framework-pid")

                End With

                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                End With
                If .DataPID = 0 Then
                    .DataPID = BO.BAS.IsNullInt(.Factory.j03UserBL.GetUserParam("mobile_o22_framework-pid", "O"))
                    If .DataPID = 0 Then Response.Redirect("mobile_scheduler.aspx")
                End If

            End With

            RefreshRecord()

        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.o22Milestone = Master.Factory.o22MilestoneBL.Load(Master.DataPID)

        Dim cClient As BO.p28Contact = Nothing
        With cRec
            
            Me.RecordHeader.Text = BO.BAS.OM3(.o22Name, 30)
            Me.RecordHeader.NavigateUrl = "mobile_o22_framework.aspx?pid=" & .PID.ToString
            If .IsClosed Then RecordHeader.Font.Strikeout = True
            Me.RecordName.Text = .o22Name
            If .p41ID <> 0 Then
                Me.Project.NavigateUrl = "mobile_p41_framework.aspx?pid=" & .p41ID.ToString
                Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID)
            Else
                Me.Project.Visible = False
            End If
            

            Me.o21Name.Text = .o21Name

            If .o21Flag = BO.o21FlagEnum.EventFromUntil Then
                Me.o22DateUntil.Text = BO.BAS.FD(.o22DateUntil, True, True)
            Else
                Me.o22DateFrom.Text = BO.BAS.FD(.o22DateFrom, True, True)
                Me.o22DateUntil.Text = BO.BAS.FD(.o22DateUntil, True, True)
            End If
           
       

            Me.Timestamp.Text = .Timestamp
            Me.o22Description.Text = BO.BAS.CrLfText2Html(.o22Description)

        End With
        rpO20.DataSource = Master.Factory.o22MilestoneBL.GetList_o20(cRec.PID)
        rpO20.DataBind()
        If rpO20.Items.Count = 0 Then rpO20.Visible = False

        
        

        
        labels1.RefreshData(Master.Factory, BO.x29IdEnum.o22Milestone, cRec.PID, True)
        boxX18.Visible = labels1.ContainsAnyData
       
    End Sub
End Class