
Public Class report_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Enum treeStructure
        PerCategory = 1
        PerFormat = 2

    End Enum
    Private Property CurrentStructure As treeStructure
        Get
            Return BO.BAS.IsNullInt(Me.opgStructure.SelectedValue)
        End Get
        Set(value As treeStructure)
            Me.opgStructure.SelectedValue = CInt(value).ToString
        End Set
    End Property
    Private Class ReportCategory
        Public Property j25ID As Integer
        Public Property j25Name As String

    End Class


    Private Sub report_framework_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                If Not .Factory.SysUser.j04IsMenu_Report Then .StopPage("Nedisponujete oprávněním k zobrazení stránky [Sestavy].")

                If Handle_DefaultPersonalPage() Then
                    Return  'obsah byl změněn na výchozí (startovací) stránku, dál už nemá cenu pokračovat
                End If
                .PageTitle = "Tiskové sestavy a pluginy"
                .SiteMenuValue = "report_framework"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("report_framework-structure")
                    .Add("report_framework-navigationPane_width")
                    .Add("report_framework_detail-x31id")

                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)

                    Me.CurrentStructure = BO.BAS.IsNullInt(.GetUserParam("report_framework-structure", "1"))
                    Me.navigationPane.Width = Unit.Parse(.GetUserParam("report_framework-navigationPane_width", "350") & "px")
                    If Request.Item("x31id") = "" Then
                        SetupTree(.GetUserParam("report_framework_detail-x31id"))
                    Else
                        SetupTree(Request.Item("x31id"))
                    End If


                End With

            End With
            If Request.Item("x31id") <> "" And tree1.SelectedValue <> "" Then
                contentPane.ContentUrl = "report_framework_detail1.aspx?x31id=" & tree1.SelectedValue

            End If
        End If
    End Sub

    Private Sub SetupTree(strSelectedX31ID As String)
        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        Dim lisX31 As IEnumerable(Of BO.x31Report) = Master.Factory.x31ReportBL.GetList(mq).Where(Function(p) p.x29ID = BO.x29IdEnum._NotSpecified)

        Dim j25ids As IEnumerable(Of Integer) = lisX31.Select(Function(p) p.j25ID).Distinct

        With tree1
            .Clear()
            For Each intJ25ID As Integer In j25ids.Where(Function(p) p <> 0)
                Dim n As New Telerik.Web.UI.RadPanelItem(lisX31.Where(Function(p) p.j25ID = intJ25ID)(0).j25Name)
                n.Value = "j25-" & intJ25ID.ToString
                n.Font.Bold = True
                ''n.CssClass = "framework_header_span"
                ''n.ImageUrl = "Images/folder.png"
                .AddItem(n)
            Next
            If j25ids.Where(Function(p) p = 0).Count > 0 Then
                Dim n As New Telerik.Web.UI.RadPanelItem("Bez kategorie")
                n.Value = "j25-0"
                n.Font.Bold = True
                ''n.CssClass = "framework_header_span"
                ''n.ImageUrl = "Images/folder.png"
                .AddItem(n)
            End If
            

            For Each c In lisX31
                'Dim n As New Telerik.Web.UI.RadTreeNode(c.x31Name, c.PID.ToString, "javascript:sr(" & c.PID.ToString & ",'" & CInt(c.x31FormatFlag).ToString & "')")
                Dim strURL As String = "javascript:sr(" & c.PID.ToString & ",'" & CInt(c.x31FormatFlag).ToString & "')", strIMG As String = "", strTarget As String = ""
                Select Case c.x31FormatFlag
                    Case BO.x31FormatFlagENUM.ASPX
                        strIMG = "Images/plugin.png"
                        strURL = "Plugins/" & c.ReportFileName & "?caller=report_framework"
                        strTarget = contentPane.ClientID
                    Case BO.x31FormatFlagENUM.Telerik
                        strIMG = "Images/report.png"
                    Case BO.x31FormatFlagENUM.XLSX
                        strIMG = "Images/xls.png"
                End Select

                If c.x31QueryFlag > BO.x31QueryFlagENUM._None Then strIMG = "Images/query.png"
                If DateDiff(DateInterval.Day, c.ValidFrom, Now, Microsoft.VisualBasic.FirstDayOfWeek.Monday, FirstWeekOfYear.System) < 6 Then strIMG = "Images/upgraded_32.png"
                .AddItem(c.x31Name, c.PID.ToString, strURL, "j25-" & c.j25ID.ToString, strIMG, , strTarget)
            Next

            .ExpandAll()
            If strSelectedX31ID <> "" Then .SelectedValue = strSelectedX31ID

        End With
    End Sub

    Private Sub opgStructure_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgStructure.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("report_framework-structure", Me.opgStructure.SelectedValue)

        SetupTree(Me.tree1.SelectedValue)
    End Sub



   
   
    Private Sub cmdExpandAll_Click(sender As Object, e As ImageClickEventArgs) Handles cmdExpandAll.Click
        tree1.ExpandAll()
    End Sub

    Private Function Handle_DefaultPersonalPage() As Boolean
        If Request.Item("defpage") = "1" Then
            'změnit prostředí na výchozí (startovací) stránku
            'navigationPane.Visible = False
            RadSplitter1.Items.Remove(navigationPane)
            RadSplitter1.Items.Remove(RadSplitbar1)
            RadSplitter1.BorderStyle = BorderStyle.None
            'RadSplitbar1.Visible = False
            contentPane.ContentUrl = GetAspxPluginURL()
            Master.SiteMenuValue = "dashboard"
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetAspxPluginURL() As String
        Dim intX31ID As Integer = BO.BAS.IsNullInt(Master.Factory.SysUser.PersonalPage)
        If intX31ID = 0 Then Return ""
        Dim cRec As BO.x31Report = Master.Factory.x31ReportBL.Load(intX31ID)
        If Not cRec Is Nothing Then
            Return "Plugins/" & cRec.ReportFileName
        Else
            Return "entity_framework_detail_missing.aspx?prefix=x31&pid=" & intX31ID.ToString
        End If

    End Function

    Private Sub cmdCollapseAll_Click(sender As Object, e As ImageClickEventArgs) Handles cmdCollapseAll.Click
        tree1.CollapseAll()
    End Sub
End Class