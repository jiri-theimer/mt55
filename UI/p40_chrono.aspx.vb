Public Class p40_chrono
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p40_chrono_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderText = "Plán generování opakované odměny"
            End With

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.p40WorkSheet_Recurrence = Master.Factory.p40WorkSheet_RecurrenceBL.Load(Master.DataPID)
        With cRec
            Me.p40Name.Text = cRec.p40Name
            Me.p40Value.Text = BO.BAS.FN(.p40Value)
            Me.Project.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .p41ID)
            Me.p40Text.Text = .p40Text
            Me.p34Name.Text = .p34Name
            Me.p32Name.Text = .p32Name
        End With

        Dim lisP39 As IEnumerable(Of BO.p39WorkSheet_Recurrence_Plan) = Master.Factory.p40WorkSheet_RecurrenceBL.GetList_p39(Master.DataPID)
        rpP39.DataSource = lisP39
        rpP39.DataBind()
        
    End Sub

   

    Private Sub rpP39_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP39.ItemCommand
        Dim intP39ID As Integer = e.CommandArgument
        Dim cRec As BO.p39WorkSheet_Recurrence_Plan = Master.Factory.p40WorkSheet_RecurrenceBL.GetList_p39(Master.DataPID).Where(Function(p) p.p39ID = intP39ID)(0)
        Dim cP40 As BO.p40WorkSheet_Recurrence = Master.Factory.p40WorkSheet_RecurrenceBL.Load(Master.DataPID)

        Select Case e.CommandName
            Case "generate"
                If Master.Factory.p40WorkSheet_RecurrenceBL.GenerateWorksheetRecord(cP40, cRec) = 0 Then
                    Master.Notify("Systém nedokázal vygenerovat worksheet úkon.", NotifyLevel.ErrorMessage)
                End If
            Case "clear"
                Master.Factory.p40WorkSheet_RecurrenceBL.Update_p31Instance(cRec.p39ID, 0, "")
        End Select
        
        RefreshRecord()
    End Sub

    Private Sub rpP39_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP39.ItemDataBound
        Dim cRec As BO.p39WorkSheet_Recurrence_Plan = CType(e.Item.DataItem, BO.p39WorkSheet_Recurrence_Plan)
        Dim strErr As String = cRec.p39ErrorMessage_NewInstance
        CType(e.Item.FindControl("p39DateCreate"), Label).Text = BO.BAS.FD(cRec.p39DateCreate)
        CType(e.Item.FindControl("p39Date"), Label).Text = BO.BAS.FD(cRec.p39Date)
        CType(e.Item.FindControl("p39Text"), Label).Text = cRec.p39Text
        CType(e.Item.FindControl("cmdGenerate"), Button).CommandArgument = cRec.p39ID
        CType(e.Item.FindControl("cmdClear"), Button).CommandArgument = cRec.p39ID

        With CType(e.Item.FindControl("cmdP31"), HyperLink)
            If cRec.p31ID_NewInstance = 0 Then
                .Visible = False

                If DateDiff(DateInterval.Day, cRec.p39DateCreate, Now) > 2 Then                    
                    strErr += " Systém již nebude zkoušet automaticky generovat tak starý úkon."
                Else
                    CType(e.Item.FindControl("lblMessage"), Label).Text = "Čeká na automatické vygenerování."
                End If
                e.Item.FindControl("cmdGenerate").Visible = True

            Else
                .NavigateUrl = "p31_record.aspx?pid=" & cRec.p31ID_NewInstance.ToString
                Dim c As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(cRec.p31ID_NewInstance)
                .Text = "Vygenerováno"
                If Not c Is Nothing Then
                    .Text = String.Format("Již vygenerováno: {0}", BO.BAS.FD(c.DateInsert, True, False))
                Else
                    strErr += " Vygenerovaný úkon již neexistuje."
                    e.Item.FindControl("cmdClear").Visible = True
                    .Enabled = False
                    e.Item.FindControl("cmdGenerate").Visible = True
                End If
            End If
        End With

        CType(e.Item.FindControl("lblError"), Label).Text = strErr
    End Sub
End Class