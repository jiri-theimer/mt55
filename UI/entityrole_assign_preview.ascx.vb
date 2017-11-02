Public Class entityrole_assign_preview
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
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
    Public Property NoDataText As String
        Get
            Return noData.Text
        End Get
        Set(value As String)
            noData.Text = value
        End Set
    End Property
    Public ReadOnly Property LastInhaledDataRecordPID As Integer
        Get
            Return BO.BAS.IsNullInt(hidInhaledDataPID.Value)
        End Get
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return rpX69.Items.Count
        End Get
    End Property

    Public Sub RefreshData(lisX69 As IEnumerable(Of BO.x69EntityRole_Assign), intDataRecordPID As Integer)
        rpX69.DataSource = lisX69
        rpX69.DataBind()
        Me.hidInhaledDataPID.Value = intDataRecordPID.ToString
        If lisX69.Count = 0 Then
            noData.Visible = True
        Else
            noData.Visible = False
        End If
    End Sub

    Private Sub rpX69_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX69.ItemDataBound
        Dim cRec As BO.x69EntityRole_Assign = CType(e.Item.DataItem, BO.x69EntityRole_Assign)
        CType(e.Item.FindControl("_x67name"), Label).Text = cRec.x67Name & ":"
        If cRec.IsAllPersons Then
            CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/infinity.png"
        Else
            If cRec.j02ID > 0 Then
                CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole.png"
                CType(e.Item.FindControl("_subject"), Label).Text = cRec.Person
            Else
                CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole_team.png"
                CType(e.Item.FindControl("_subject"), Label).Text = cRec.j11Name
            End If
        End If
    End Sub
End Class