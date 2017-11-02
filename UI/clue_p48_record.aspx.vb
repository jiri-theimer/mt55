Public Class clue_p48_record
    Inherits System.Web.UI.Page
    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

            If Request.Item("dr") = "1" Then
                panContainer.Style.Clear()
            End If

            If Request.Item("js_edit") <> "" Then
                imgEdit.Visible = True : cmdEdit.Visible = True : cmdEdit.NavigateUrl = "javascript: parent.window." & Request.Item("js_edit")
            End If
            If Request.Item("js_convert") <> "" Then
                cmdConvert.Visible = True : cmdConvert.NavigateUrl = "javascript: parent.window." & Request.Item("js_convert")
            End If
            If Request.Item("js_p31record") <> "" Then
                cmdWorksheet.Visible = True : cmdWorksheet.NavigateUrl = "javascript: parent.window." & Request.Item("js_p31record")
            End If

            RefreshRecord()
            If cmdConvert.Visible Or cmdWorksheet.Visible Then imgWorksheet.Visible = True
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Master.StopPage("pid is missing", , , False)

        Dim cRec As BO.p48OperativePlan = Master.Factory.p48OperativePlanBL.Load(Master.DataPID)
        If cRec Is Nothing Then Master.StopPage("record not found")
        With cRec
            If .p31ID > 0 Then
                ph1.Font.Strikeout = True
                cmdConvert.Visible = False
            Else
                cmdWorksheet.Visible = False
            End If
            Me.Project.Text = .ClientAndProject
            If .p34Name <> "" Then
                Me.p32Name.Text = .p34Name
            End If
            If .p32Name <> "" Then
                Me.p32Name.Text += "/" & .p32Name
            End If

            Me.Person.Text = .Person
            Me.p48Date.Text = BO.BAS.FD(.p48Date)
            Me.p48Hours.Text = BO.BAS.FN(.p48Hours)
            If Not .p48TimeFrom Is Nothing Then
                Me.TimePeriod.Text = .p48TimeFrom & "-" & .p48TimeUntil
            End If
            Me.p48Text.Text = BO.BAS.CrLfText2Html(.p48Text)

            Me.Timestamp.Text = .Timestamp
        End With
      
    End Sub
End Class