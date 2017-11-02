Public Class m62_import_setting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub m62_import_setting_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.AddToolbarButton("Uložit změny", "save", , "Images/save.png")

            Me.j27ids.DataSource = Master.Factory.ftBL.GetList_J27().Where(Function(p) p.PID <> 2).OrderBy(Function(p) p.j27Code)
            Me.j27ids.DataBind()

            Dim s As String = Master.Factory.x35GlobalParam.GetValueString("j27Codes_Import_CNB")
            If s = "" Then
                Me.chkAllowImport.Checked = False
            Else
                Me.chkAllowImport.Checked = True
                Dim a() As String = Split(s, ",")
                For i As Integer = 0 To UBound(a)
                    If Not Me.j27ids.Items.FindByValue(a(i)) Is Nothing Then
                        Me.j27ids.Items.FindByValue(a(i)).Selected = True
                    End If
                Next
            End If
            dat1.SelectedDate = Today
        End If
    End Sub
    Private Function GetJ27Codes() As String
        Dim lis As New List(Of String)
        For Each li As ListItem In Me.j27ids.Items
            If li.Selected Then lis.Add(li.Value)
        Next
        If lis.Count = 0 Then
            Master.Notify("Musíte zaškrtnout minimálně jednu měnu.", NotifyLevel.ErrorMessage) : Return ""
        Else
            Return String.Join(",", lis)
        End If
    End Function
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Dim s As String = ""
        If Me.chkAllowImport.Checked Then
            s = GetJ27Codes()
            If s = "" Then Return
        End If

        If Master.Factory.x35GlobalParam.UpdateValue("j27Codes_Import_CNB", s) Then
            Master.CloseAndRefreshParent()
        End If

    End Sub

    Private Sub m62_import_setting_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panJ27.Visible = Me.chkAllowImport.Checked
    End Sub

    Private Sub cmdImport_Click(sender As Object, e As EventArgs) Handles cmdImport.Click
        If dat1.IsEmpty Then Master.Notify("Chybí datum.") : Return
        Dim s As String = GetJ27Codes()
        If s = "" Then Return
        Master.Factory.m62ExchangeRateBL.ImportRateList_CNB(Me.dat1.SelectedDate, s)

        Master.CloseAndRefreshParent()
    End Sub
End Class