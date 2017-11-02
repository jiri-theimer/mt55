Public Class entityrole_assign_inline
    Inherits System.Web.UI.UserControl
    Private Property _cRecLast As New BO.x69EntityRole_Assign
    

    Public Property EntityX29ID As BO.x29IdEnum
        Get
            If Me.hidX29ID.Value <> "" Then
                Return CInt(Me.hidX29ID.Value)
            Else
                Return BO.x29IdEnum._NotSpecified
            End If
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
        End Set
    End Property
    Public Property IsShowClueTip As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsShowClueTip.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsShowClueTip.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property IsRenderAsTable As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsRenderAsTable.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsRenderAsTable.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return rpX69.Items.Count
        End Get
    End Property
    Public Property NoDataText As String
        Get
            Return noData.Text
        End Get
        Set(value As String)
            noData.Text = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(lisX69 As IEnumerable(Of BO.x69EntityRole_Assign), intDataRecordPID As Integer)
        rpX69.DataSource = lisX69.OrderBy(Function(p) p.x67ID)
        rpX69.DataBind()
        Me.hidInhaledDataPID.Value = intDataRecordPID.ToString
        If lisX69.Count = 0 Then
            noData.Visible = True
        Else
            noData.Visible = False
            CType(rpX69.Items(rpX69.Items.Count - 1).FindControl("_subject"), Label).Text += ")"
            If Me.hidIsRenderAsTable.Value = "1" Then rpX69.Controls.Add(New LiteralControl("</td></tr>"))
        End If
    End Sub

    Private Sub rpX69_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX69.ItemDataBound
        Dim cRec As BO.x69EntityRole_Assign = CType(e.Item.DataItem, BO.x69EntityRole_Assign)
        
        If _cRecLast.x67ID <> cRec.x67ID Then
            With CType(e.Item.FindControl("_x67name"), Label)
                If _cRecLast.x69ID > 0 Then
                    '.Text += ")<span style='padding-right:10px;'></span>"
                    If Me.hidIsRenderAsTable.Value = "1" Then
                        CType(e.Item.FindControl("place1"), PlaceHolder).Controls.Add(New LiteralControl("<span>)</span></td></tr><tr><td>"))
                    Else
                        CType(e.Item.FindControl("place1"), PlaceHolder).Controls.Add(New LiteralControl("<span>)</span></br>"))
                    End If
                Else
                    If Me.hidIsRenderAsTable.Value = "1" Then CType(e.Item.FindControl("place1"), PlaceHolder).Controls.Add(New LiteralControl("<tr><td>"))
                End If

                .Text += cRec.x67Name
            End With
            If Me.IsShowClueTip Then
                CType(e.Item.FindControl("role_clue"), HyperLink).Attributes("rel") = "clue_x67_record.aspx?pid=" & cRec.x67ID.ToString & "&j02id=" & cRec.j02ID.ToString & "&j11id=" & cRec.j11ID.ToString
            Else
                e.Item.FindControl("role_clue").Visible = False
            End If


        Else
            e.Item.FindControl("_x67name").Visible = False
            e.Item.FindControl("role_clue").Visible = False

        End If
        
        With CType(e.Item.FindControl("_subject"), Label)
            If _cRecLast.x67ID <> cRec.x67ID Then
                .Text += " ("
            Else
                .Text += " + "
            End If
            If cRec.IsAllPersons Then
                .Text += "<b style='color:red;' title='Všichni'>*</b>"
            Else
                If cRec.j02ID > 0 Then
                    .Text += cRec.Person
                Else
                    .Text += cRec.j11Name
                End If
            End If
        End With
        
        

        _cRecLast = cRec
    End Sub
    
End Class