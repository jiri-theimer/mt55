Public Class clue_p40_record
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            
            RefreshRecord()

        End If
    End Sub

    Private Sub RefreshRecord()

        Dim cRec As BO.p40WorkSheet_Recurrence = Master.Factory.p40WorkSheet_RecurrenceBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .p40Name
            'Me.j27Code.Text = .j27Code
            Me.p40Value.Text = BO.BAS.FN(.p40Value)
            Me.p40Text.Text = .p40Text
            Me.p34Name.Text = .p34Name
            Me.p32Name.Text = .p32Name
        End With

        Dim lisP39 As IEnumerable(Of BO.p39WorkSheet_Recurrence_Plan) = Master.Factory.p40WorkSheet_RecurrenceBL.GetList_p39(Master.DataPID)
        rpP39.DataSource = lisP39
        rpP39.DataBind()

        

    End Sub

    Private Sub rpP39_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP39.ItemDataBound
        Dim cRec As BO.p39WorkSheet_Recurrence_Plan = CType(e.Item.DataItem, BO.p39WorkSheet_Recurrence_Plan)
        CType(e.Item.FindControl("p39DateCreate"), Label).Text = BO.BAS.FD(cRec.p39DateCreate)
        CType(e.Item.FindControl("p39Date"), Label).Text = BO.BAS.FD(cRec.p39Date)
        CType(e.Item.FindControl("p39Text"), Label).Text = cRec.p39Text
        CType(e.Item.FindControl("p39ErrorMessage_NewInstance"), Label).Text = cRec.p39ErrorMessage_NewInstance
        With CType(e.Item.FindControl("cmdP31"), HyperLink)
            If cRec.p31ID_NewInstance = 0 Then
                .Visible = False
            Else
                .NavigateUrl = "javascript:p31_record(" & cRec.p31ID_NewInstance.ToString & ")"
                .Text = "Vygenerováno"
            End If
        End With

    End Sub
End Class