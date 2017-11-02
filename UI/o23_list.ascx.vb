Public Class o23_list
    Inherits System.Web.UI.UserControl
    Private _rowsCount As Integer = 0
   
    Public Property IsShowClueTip As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsShowClueTip.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsShowClueTip.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return rpO23.Items.Count
        End Get
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(lisO23 As IEnumerable(Of BO.o23Doc), intDataRecordPID As Integer)
        _rowsCount = lisO23.Count

        rpO23.DataSource = lisO23
        rpO23.DataBind()
        Me.hidInhaledDataPID.Value = intDataRecordPID.ToString

    End Sub

    Private Sub rpO23_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpO23.ItemCommand
        If e.CommandName = "go2module" Then
            Response.Redirect("o23_framework.aspx?pid=" & e.CommandArgument)
        End If
    End Sub

    Private Sub rpO23_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpO23.ItemDataBound
        Dim cRec As BO.o23Doc = CType(e.Item.DataItem, BO.o23Doc)

        If Me.hidIsShowClueTip.Value = "1" Then
            CType(e.Item.FindControl("clue_o23"), HyperLink).Attributes.Item("rel") = "clue_o23_record.aspx?pid=" & cRec.PID.ToString
        Else
            e.Item.FindControl("clue_o23").Visible = False
        End If

        With CType(e.Item.FindControl("img1"), Image)
            .ImageUrl = "Images/notepad.png"
            If cRec.o23IsEncrypted Then
                .ImageUrl = "Images/lock.png"
            
            End If

        End With

        With CType(e.Item.FindControl("x18Name"), Label)
            .Text = cRec.x23Name
        End With
        With CType(e.Item.FindControl("o23Name"), HyperLink)
            If cRec.o23Name = "" Then
                .Text = cRec.o23Code
            Else
                .Text = cRec.o23Name
            End If
            .NavigateUrl = "javascript:o23_record(" & cRec.PID.ToString & ")"
            .ToolTip = cRec.UserUpdate & "/" & BO.BAS.FD(cRec.DateUpdate, True)
        End With

    End Sub

End Class