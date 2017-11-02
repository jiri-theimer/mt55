Imports Telerik.Web.UI

Public Class admin_menu
    Inherits System.Web.UI.Page

    Public Property CurrentJ60ID As Integer
        Get
            Return BO.BAS.IsNullInt(cbxJ60ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.cbxJ60ID, value.ToString)
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "MENU návrhář"
                .SiteMenuValue = "admin_framework"
                .TestNeededPermission(BO.x53PermValEnum.GR_Admin)

               
            End With

            Me.cbxJ60ID.DataSource = Master.Factory.j62MenuHomeBL.GetList_J60().OrderByDescending(Function(p) p.PID)
            Me.cbxJ60ID.DataBind()

            Me.CurrentJ60ID = BO.BAS.IsNullInt(Request.Item("j60id"))
           

            RefreshRecord()


        End If
    End Sub

    Private Sub RefreshRecord()

        refreshtree()
    End Sub

    Private Sub RefreshTree()
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Dim lis As IEnumerable(Of BO.j62MenuHome) = Master.Factory.j62MenuHomeBL.GetList(Me.CurrentJ60ID, mq)

        With tree1
            .Clear()
            For Each c In lis
                Dim strParentID As String = ""
                If c.j62ParentID <> 0 Then strParentID = c.j62ParentID.ToString
                Dim n As New RadTreeNode(c.j62Name)
                n.ImageUrl = c.j62ImageUrl
                n.NavigateUrl = "javascript:rec(" & c.PID.ToString & ")"
                n.Value = c.PID.ToString
                If c.IsClosed Then n.Font.Strikeout = True
                If c.j62Ordinary <> 0 Then
                    n.Text += " #" & c.j62Ordinary.ToString
                End If
                n.ToolTip = c.j62Tag & " | " & c.j62Url
                .AddItem(n, strParentID)
                ''.AddItem(c.j62Name, c.PID.ToString, "javascript:rec(" & c.PID.ToString & ")", strParentID, c.j62ImageUrl, c.j62Name_ENG)

              

            Next
            .ExpandAll()
        End With

       
    End Sub

    Private Sub cmdRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdRefreshOnBehind.Click
        Dim x As Integer = Me.CurrentJ60ID
        Me.cbxJ60ID.DataSource = Master.Factory.j62MenuHomeBL.GetList_J60().OrderByDescending(Function(p) p.PID)
        Me.cbxJ60ID.DataBind()
        Me.CurrentJ60ID = x

        RefreshRecord()

    End Sub

    Private Sub cbxJ60ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxJ60ID.SelectedIndexChanged
        RefreshRecord()
    End Sub
End Class