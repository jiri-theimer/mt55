Public Class plugin_modal
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Public Property CurrentX31ID As Integer
        Get
            If Me.x31ID.Items.Count = 0 Then Return 0
            Return BO.BAS.IsNullInt(Me.x31ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.x31ID, value.ToString)
        End Set
    End Property
    Public Property CurrentX29ID As BO.x29IdEnum
        Get
            Return DirectCast(CInt(Me.hidX29ID.Value), BO.x29IdEnum)
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
            Me.hidPrefix.Value = BO.BAS.GetDataPrefix(value)
        End Set
    End Property
    Public ReadOnly Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Me.CurrentX29ID = BO.BAS.GetX29FromPrefix(Request.Item("prefix"))
            If Me.CurrentX29ID = BO.x29IdEnum._NotSpecified Or Master.DataPID = 0 Then
                Master.StopPage("prefix or pid is missing")
            End If

            Dim mq As New BO.myQuery
            mq.Closed = BO.BooleanQueryMode.FalseQuery
            Dim lisX31 As List(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(mq).Where(Function(p) p.x29ID = Me.CurrentX29ID And (p.x31FormatFlag = BO.x31FormatFlagENUM.ASPX)).ToList

            Me.x31ID.DataSource = lisX31
            Me.x31ID.DataBind()
            If lisX31.Count = 0 Then
                Master.Notify("Ani jeden dostupný plugin pro tento druh entity.", NotifyLevel.InfoMessage)
                Return
            Else
                With Master.Factory.j03UserBL
                    .InhaleUserParams("plugin_modal-x31id-" & Me.CurrentPrefix)
                    basUI.SelectDropdownlistValue(Me.x31ID, .GetUserParam("plugin_modal-x31id-" & Me.CurrentPrefix))
                End With

            End If
            RenderReport()
        End If
    End Sub

    Private Sub x31ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x31ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("plugin_modal-x31id-" & Me.CurrentPrefix, Me.x31ID.SelectedValue)
        RenderReport()
    End Sub

    Private Sub RenderReport()
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(Me.CurrentX31ID)
        If cRec Is Nothing Then
            Master.StopPage("Nelze načíst plugin.<hr>" & Master.Factory.x31ReportBL.ErrorMessage)
            Return
        End If
        Me.contentPane.ContentUrl = "Plugins/" & cRec.ReportFileName & "?pid=" & Master.DataPID.ToString
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        RenderReport()
    End Sub
End Class