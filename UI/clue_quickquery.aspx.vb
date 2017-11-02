Public Class clue_quickquery
    Inherits System.Web.UI.Page
    Private Property _lastField As String
    
   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("prefix") = Request.Item("prefix")
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("j70id"))

            RefreshRecord()
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.j70QueryTemplate = Master.Factory.j70QueryTemplateBL.Load(Master.DataPID)
        If cRec.j70IsNegation Then lblHeader.Text = "Negace podmínky filtru" : lblHeader.ForeColor = Drawing.Color.Red
        Select Case cRec.j70BinFlag
            Case 0
                lblBinFlag.Text = "Otevřené i uzavřené záznamy"
            Case 1
                lblBinFlag.Text = "Pouze otevřené záznamy"
            Case 2
                lblBinFlag.Text = "<img src='Images/bin.png'/>Pouze záznamy v archivu"
        End Select
        Select Case cRec.x29ID
            Case BO.x29IdEnum.p41Project
                imgEntity.ImageUrl = "Images/project.png"
                Me.ph1.Text = "Detail filtru projektů"
            Case BO.x29IdEnum.p28Contact
                Me.ph1.Text = "Detail filtrů klientů"
                imgEntity.ImageUrl = "Images/contact.png"
            Case BO.x29IdEnum.j02Person
                imgEntity.ImageUrl = "Images/person.png"
                Me.ph1.Text = "Detail filtru lidí"
            Case BO.x29IdEnum.p91Invoice
                imgEntity.ImageUrl = "Images/invoice.png"
                Me.ph1.Text = "Detail filtru faktur"
        End Select

        ph1.Text += " (" & cRec.j70Name & ")"
        Me.lblTimeStamp.Text = cRec.Timestamp

        Dim lis As List(Of BO.j71QueryTemplate_Item) = Master.Factory.j70QueryTemplateBL.GetList_j71(Master.DataPID)

        rpJ71.DataSource = lis
        rpJ71.DataBind()
    End Sub



   
    Private Sub rpJ71_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpJ71.ItemDataBound
        Dim cRec As BO.j71QueryTemplate_Item = CType(e.Item.DataItem, BO.j71QueryTemplate_Item)
        
        If _lastField <> cRec.j71Field Then
            With CType(e.Item.FindControl("j71FieldLabel"), Label)
                Select Case cRec.j71Field
                    Case "_other"
                        .Text = "Různé:"
                    Case "x67id"
                        .Text = "Obsazení projektové role:"
                    Case "j02id_owner"
                        .Text = "Vlastník záznamu:"
                    Case Else
                        .Text = cRec.j71FieldLabel & ":"
                End Select
            End With

            
            e.Item.FindControl("nebo").Visible = False
        Else
            e.Item.FindControl("nebo").Visible = True
            
        End If
        

        CType(e.Item.FindControl("j71RecordName"), Label).Text = cRec.j71RecordName
        
        CType(e.Item.FindControl("j71RecordName_Extension"), Label).Text = cRec.j71RecordName_Extension
        
        _lastField = cRec.j71Field
    End Sub
End Class