Public Class clue_p51_record
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)
            If Request.Item("dr") = "1" Then
                panContainer.Style.Clear()
            End If

            RefreshRecord()
            cmdEdit.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P51_Admin)
        End If
    End Sub

    Private Sub RefreshRecord()
        
        Dim cRec As BO.p51PriceList = Master.Factory.p51PriceListBL.Load(Master.DataPID)
        With cRec
            Me.ph1.Text = .NameWithCurr
            Me.j27Code.Text = .j27Code
            Me.p51DefaultRateT.Text = BO.BAS.FN(.p51DefaultRateT)

        End With

        Dim lisP52 As IEnumerable(Of BO.p52PriceList_Item) = Master.Factory.p51PriceListBL.GetList_p52(Master.DataPID)
        rpP52.DataSource = lisP52
        rpP52.DataBind()

        If cRec.p51ID_Master > 0 Then
            Me.panMaster.Visible = True
            cRec = Master.Factory.p51PriceListBL.Load(cRec.p51ID_Master)
            Me.MasterPricelist.Text = cRec.NameWithCurr
            Me.MasterPricelist_DefaultRate.Text = BO.BAS.FN(cRec.p51DefaultRateT)
            lisP52 = Master.Factory.p51PriceListBL.GetList_p52(cRec.PID)
            rpP52_Master.DataSource = lisP52
            rpP52_Master.DataBind()
        End If

    End Sub

    Private Sub rpP52_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP52.ItemDataBound
        Handle_itemdataBound(sender, e)
    End Sub
    Private Sub rpP52_Master_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP52_Master.ItemDataBound
        Handle_itemdataBound(sender, e)
    End Sub

    Private Sub Handle_itemdataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim cRec As BO.p52PriceList_Item = CType(e.Item.DataItem, BO.p52PriceList_Item)
        CType(e.Item.FindControl("p52Name"), Label).Text = cRec.p52Name
        CType(e.Item.FindControl("p52Rate"), Label).Text = BO.BAS.FN(cRec.p52Rate)
        CType(e.Item.FindControl("j27Code"), Label).Text = Me.j27Code.Text
        With CType(e.Item.FindControl("p34Name"), Label)
            .Text = cRec.p34Name
            If cRec.p52IsPlusAllTimeSheets Then
                .Text += "<br><i>+všechny ostatní časové sešity</i>"
            End If
        End With
        With CType(e.Item.FindControl("p32Name"), Label)
            .Text = cRec.p32Name
        End With
        With CType(e.Item.FindControl("Subject"), Label)
            If cRec.j02ID > 0 Then
                .Text = cRec.Person
            Else
                .Text = cRec.j07Name
            End If
        End With
    End Sub

    
End Class