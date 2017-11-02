Public Class mytags
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Public ReadOnly Property IsNeed2Save As Boolean
        Get
            Return BO.BAS.BG(hidNeed2save.Value)
        End Get
    End Property



    Public Property Prefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property
    Public Property RecordPID As Integer
        Get
            Return BO.BAS.IsNullInt(hidRecordPID.Value)
        End Get
        Set(value As Integer)
            hidRecordPID.Value = value.ToString
        End Set
    End Property
    Public Property ModeUi As Integer   '0 - readonly + odkaz, 1 - v editačním formuláři, 2 - pouze readonly
        Get
            Return BO.BAS.IsNullInt(hidMode.Value)
        End Get
        Set(value As Integer)
            hidMode.Value = value.ToString
        End Set
    End Property

    Public Sub RefreshData(intRecordPID As Integer)
        If hidMode.Value = "2" Then
            cmdTags.Visible = False   'readonly režim
        End If
        hidRecordPID.Value = intRecordPID.ToString
        If intRecordPID > 0 Then
            Dim lis As IEnumerable(Of BO.o52TagBinding) = Me.Factory.o51TagBL.GetList_o52(hidPrefix.Value, intRecordPID)
            If hidMode.Value = "1" Then
                hidO51IDs.Value = String.Join(",", lis.Select(Function(p) p.o51ID))
            End If

            rp1.DataSource = lis
        Else
            rp1.DataSource = Nothing
        End If

        rp1.DataBind()
        If rp1.Items.Count > 0 Then
            cmdTags.Text = "[Štítky]"
        Else
            cmdTags.Text = "[Zatím žádné štítky]"
        End If

    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim c As BO.o52TagBinding = CType(e.Item.DataItem, BO.o52TagBinding)
        With CType(e.Item.FindControl("panItem"), Panel)
            If c.o51BackColor <> "" Then
                .Style.Item("background-color") = c.o51BackColor
            End If
            If c.o51ForeColor <> "" Then
                .Style.Item("color") = c.o51ForeColor
            End If
        End With
        With CType(e.Item.FindControl("o51Name"), Label)
            .Text = c.o51Name
            .ToolTip = c.o52UserUpdate & "/" & BO.BAS.FD(c.o52DateUpdate, True)
        End With

    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        hidNeed2save.Value = "1"
        Dim lis As New List(Of BO.o52TagBinding)
        For Each intO51ID As Integer In BO.BAS.ConvertPIDs2List(hidO51IDs.Value, ",")
            Dim c As New BO.o52TagBinding
            Dim cRec As BO.o51Tag = Factory.o51TagBL.Load(intO51ID)
            c.o51ID = intO51ID
            c.o51Name = cRec.o51Name
            c.o51BackColor = cRec.o51BackColor
            c.o51ForeColor = cRec.o51ForeColor
            c.o52DateUpdate = Now
            c.o52UserUpdate = Factory.SysUser.j03Login

            lis.Add(c)
        Next
        rp1.DataSource = lis
        rp1.DataBind()
        cmdTags.Text = "Štítky"
    End Sub

    Public Function Geto51IDs() As List(Of Integer)
        Return BO.BAS.ConvertPIDs2List(Me.hidO51IDs.Value)
    End Function
End Class