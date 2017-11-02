Imports Telerik.Web.UI
Public Class navigator
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Public Class DTS
        Public Property PID As Integer
        Public Property ParentPID As Integer
        Public Property ItemText As String
        Public Property Prefix As String
        Public Property Level As Integer
        Public Property IsFinalLevel As Boolean
        Public Property IsClosed As Boolean
        Public Property URL As String

        Public Sub New(pid As Integer, strText As String, strPrefix As String, bolIsClosed As Boolean)
            Me.PID = pid
            Me.ItemText = strText
            Me.Prefix = strPrefix
            Me.IsClosed = bolIsClosed
        End Sub
    End Class

    Private Sub navigator_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.SiteMenuValue = "navigator"

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
           
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Navigator
                Dim pars As New List(Of String)
                pars.Add("navigator-navigationPane_width")
                pars.Add("navigator-path")
                pars.Add("navigator-bin")

                With .Factory.j03UserBL
                    .InhaleUserParams(pars)
                    Dim strDefWidth As String = "420"

                    Dim strW As String = .GetUserParam("navigator-navigationPane_width", strDefWidth)
                    If strW = "-1" Then
                        Me.navigationPane.Collapsed = True
                    Else
                        Me.navigationPane.Width = Unit.Parse(strW & "px")
                    End If
                    basUI.SelectDropdownlistValue(Me.cbxPath, .GetUserParam("navigator-path"))
                    basUI.SelectRadiolistValue(Me.opgBIN, .GetUserParam("navigator-bin", "0"))
                End With
            End With

            RestartData(False)

        End If
    End Sub
    Private Function GetData(nParent As RadTreeNode) As List(Of DTS)
        Dim dtss As New List(Of DTS), intParentLevel As Integer = 0, prefixes As New List(Of String), intParentPID As Integer = 0, strParentPrefix As String = ""

        If nParent Is Nothing Then
            prefixes.Add(hidLevel0.Value)
        Else
            intParentPID = CInt(nParent.Value)
            intParentLevel = CInt(nParent.Attributes.Item("level"))
            strParentPrefix = nParent.Attributes.Item("prefix")
            Select Case strParentPrefix
                Case "p41"
                    prefixes.Add("p41") 'podřízené projekty
                Case "p28"
                    prefixes.Add("p28") 'podřízení klienti
            End Select
            Select Case intParentLevel
                Case 0
                    If hidLevel1.Value <> "" Then prefixes.Add(Me.hidLevel1.Value)
                Case 1
                    If Me.hidLevel2.Value <> "" Then prefixes.Add(Me.hidLevel2.Value)
            End Select
        End If
        For Each strPrefix In prefixes
            Select Case strPrefix
                Case "p28"
                    Dim mq As New BO.myQueryP28
                    mq.Closed = CType(Me.opgBIN.SelectedValue, BO.BooleanQueryMode)
                    mq.MG_GridSqlColumns = "p28Name,p28TreePrev,p28TreeNext"
                    mq.SpecificQuery = BO.myQueryP28_SpecificQuery.AllowedForRead
                    If intParentPID <> 0 And strParentPrefix = "p28" Then mq.p28ParentID = intParentPID
                    If strParentPrefix <> "p28" Then mq.p28TreeLevel = 0
                    If cbxPath.SelectedValue.IndexOf("p41") > 0 Then
                        If mq.Closed = BO.BooleanQueryMode.FalseQuery Then
                            mq.QuickQuery = BO.myQueryP28_QuickQuery.WithOpenProjects
                        Else
                            mq.QuickQuery = BO.myQueryP28_QuickQuery.WithProjects
                        End If
                    End If
                    If cbxPath.SelectedValue.IndexOf("p91") > 0 Then
                        mq.QuickQuery = BO.myQueryP28_QuickQuery.WithAnyInvoice
                    End If

                    Dim dt As DataTable = Master.Factory.p28ContactBL.GetGridDataSource(mq)
                    For Each dbRow In dt.Rows
                        Dim c As New DTS(dbRow.item("pid"), dbRow.item("p28Name"), "p28", dbRow.item("IsClosed"))
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            If strPrefix <> strParentPrefix Then c.Level = intParentLevel + 1
                        End If
                        If Not dbRow.item("p28TreeNext") Is System.DBNull.Value Then
                            If dbRow.item("p28TreePrev") = dbRow.item("p28TreeNext") Then c.IsFinalLevel = IsLastLevel(c.Level)
                        Else
                            c.IsFinalLevel = IsLastLevel(c.Level)
                        End If
                        If Master.Factory.SysUser.j04IsMenu_Contact Then c.URL = "javascript:rw(" & c.PID.ToString & ",'p28')"

                        dtss.Add(c)
                    Next
                Case "p41"
                    Dim mq As New BO.myQueryP41
                    mq.Closed = CType(Me.opgBIN.SelectedValue, BO.BooleanQueryMode)
                    mq.MG_GridSqlColumns = "p41Name,p41TreePrev,p41TreeNext"
                    mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
                    If cbxPath.SelectedValue.IndexOf("p56") > 0 Then
                        If mq.Closed = BO.BooleanQueryMode.FalseQuery Then
                            mq.QuickQuery = BO.myQueryP41_QuickQuery.WithOpenTasks
                        Else
                            mq.QuickQuery = BO.myQueryP41_QuickQuery.WithAnyTasks
                        End If
                    End If
                    If cbxPath.SelectedValue.IndexOf("p91") > 0 Then
                        mq.QuickQuery = BO.myQueryP41_QuickQuery.Invoiced
                    End If

                    If cbxPath.SelectedValue.IndexOf("p91") > 0 Then mq.QuickQuery = BO.myQueryP41_QuickQuery.Invoiced
                    If intParentPID <> 0 Then
                        If strParentPrefix = "p41" Then mq.p41ParentID = intParentPID
                        If strParentPrefix = "p28" Then mq.p28ID = intParentPID
                        If strParentPrefix = "j18" Then mq.j18ID = intParentPID
                    End If
                    If strParentPrefix <> "p41" Then mq.p41TreeLevel = 0
                    Dim dt As DataTable = Master.Factory.p41ProjectBL.GetGridDataSource(mq)
                    For Each dbRow In dt.Rows
                        Dim c As New DTS(dbRow.item("pid"), dbRow.item("p41Name"), "p41", dbRow.item("IsClosed"))
                        c.Level = intParentLevel
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            If strPrefix <> strParentPrefix Then c.Level += 1

                        End If
                        If Not dbRow.item("p41TreeNext") Is System.DBNull.Value Then
                            If dbRow.item("p41TreePrev") = dbRow.item("p41TreeNext") Then c.IsFinalLevel = IsLastLevel(c.Level)
                        Else
                            c.IsFinalLevel = IsLastLevel(c.Level)
                        End If
                        If Master.Factory.SysUser.j04IsMenu_Project Then c.URL = "javascript:rw(" & c.PID.ToString & ",'p41')"
                        dtss.Add(c)
                    Next
                Case "j02"
                    Dim mq As New BO.myQueryJ02
                    mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
                    mq.Closed = CType(Me.opgBIN.SelectedValue, BO.BooleanQueryMode)
                    mq.MG_GridSqlColumns = "j02LastName+' '+j02FirstName as Person"
                    If cbxPath.SelectedValue.IndexOf("p56") > 0 Then
                        If mq.Closed = BO.BooleanQueryMode.FalseQuery Then
                            mq.QuickQuery = BO.myQueryJ02_QuickQuery.WithOpenTask
                        Else
                            mq.QuickQuery = BO.myQueryJ02_QuickQuery.WithAnyTask
                        End If
                    End If

                    If intParentPID <> 0 Then
                        If strParentPrefix = "p41" Then mq.p41ID = intParentPID
                        If strParentPrefix = "p28" Then mq.p28ID = intParentPID
                    End If
                    Dim dt As DataTable = Master.Factory.j02PersonBL.GetGridDataSource(mq)
                    For Each dbRow In dt.Rows
                        Dim c As New DTS(dbRow.item("pid"), dbRow.item("Person"), "j02", dbRow.item("IsClosed"))
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            c.Level = intParentLevel + 1
                        End If
                        c.IsFinalLevel = IsLastLevel(c.Level)
                        If Master.Factory.SysUser.IsAdmin Then c.URL = "javascript:rw(" & c.PID.ToString & ",'j02')"
                        dtss.Add(c)
                    Next
                Case "p56"
                    Dim mq As New BO.myQueryP56
                    mq.Closed = CType(Me.opgBIN.SelectedValue, BO.BooleanQueryMode)
                    mq.SpecificQuery = BO.myQueryP56_SpecificQuery.AllowedForRead
                    mq.MG_GridSqlColumns = "p56Name,p56Code,dbo.p56_getroles_inline(a.p56ID) as Resitel"
                    If intParentPID <> 0 Then
                        If strParentPrefix = "p41" Then mq.p41ID = intParentPID
                        If strParentPrefix = "j02" Then mq.j02ID = intParentPID
                        If strParentPrefix = "p28" Then mq.p28ID = intParentPID
                    End If
                    Dim dt As DataTable = Master.Factory.p56TaskBL.GetGridDataSource(mq)
                    For Each dbRow In dt.Rows
                        Dim c As New DTS(dbRow.item("pid"), dbRow.item("p56Name") & " (" & dbRow.item("p56Code") & ")", "p56", dbRow.item("IsClosed"))
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            c.Level = intParentLevel + 1
                        End If
                        c.IsFinalLevel = IsLastLevel(c.Level)
                        c.URL = "javascript:rw(" & c.PID.ToString & ",'p56')"
                        dtss.Add(c)
                    Next
                Case "p91"
                    Dim mq As New BO.myQueryP91
                    mq.Closed = CType(Me.opgBIN.SelectedValue, BO.BooleanQueryMode)
                    mq.SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
                    mq.MG_GridSqlColumns = "p91Code,p91DateSupply,p91Amount_WithoutVat,j27Code"
                    If intParentPID <> 0 Then
                        If strParentPrefix = "p41" Then mq.p41ID = intParentPID
                        If strParentPrefix = "j02" Then mq.j02ID = intParentPID
                        If strParentPrefix = "p28" Then mq.p28ID = intParentPID
                    End If
                    Dim dt As DataTable = Master.Factory.p91InvoiceBL.GetGridDataSource(mq)
                    For Each dbRow In dt.Rows
                        Dim c As New DTS(dbRow.item("pid"), dbRow.item("p91Code") & "/" & Format(dbRow.item("p91DateSupply"), "dd.MM.yyyy") & ":   " & BO.BAS.FN(dbRow.item("p91Amount_WithoutVat")) & " " & dbRow.item("j27Code"), "p91", dbRow.item("IsClosed"))
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            c.Level = intParentLevel + 1
                        End If
                        c.IsFinalLevel = IsLastLevel(c.Level)
                        If Master.Factory.SysUser.j04IsMenu_Invoice Then c.URL = "javascript:rw(" & c.PID.ToString & ",'p91')"
                        dtss.Add(c)
                    Next
                Case "j18"
                    Dim lis As IEnumerable(Of BO.j18Region) = Master.Factory.j18RegionBL.GetList(New BO.myQuery)
                    For Each dbRow In lis
                        Dim c As New DTS(dbRow.PID, dbRow.j18Name, "j18", dbRow.IsClosed)
                        If Not nParent Is Nothing Then
                            c.ParentPID = intParentPID
                            If strPrefix <> strParentPrefix Then c.Level = intParentLevel + 1
                        End If
                        c.IsFinalLevel = IsLastLevel(c.Level)
                        dtss.Add(c)
                    Next
            End Select
        Next


        Return dtss
    End Function
    Private Sub RefreshTreeData(nParent As RadTreeNode)
        Dim dtss As List(Of DTS) = GetData(nParent)
        If nParent Is Nothing Then
            tr1.Nodes.Clear()
        End If

        For Each c In dtss
            RenderTreeItem(c, nParent)
        Next
        If nParent Is Nothing And tr1.Nodes.Count = 0 Then
            Master.Notify("Pro zadanou úroveň žádná data.")
        End If


    End Sub
    Public Function RenderTreeItem(cRec As DTS, nParent As Telerik.Web.UI.RadTreeNode) As RadTreeNode
        Dim n As New Telerik.Web.UI.RadTreeNode(cRec.ItemText, cRec.PID.ToString)
        If Len(cRec.ItemText) > 40 Then
            n.Text = Left(n.Text, 40) & "..."
            n.ToolTip = cRec.ItemText
        End If

        n.Attributes.Add("prefix", cRec.Prefix)
        n.Attributes.Add("level", cRec.Level.ToString)
        n.NavigateUrl = cRec.URL
        Select Case cRec.Prefix
            Case "p28" : n.ImageUrl = "Images/contact.png"
            Case "p41" : n.ImageUrl = "Images/project.png" : n.NavigateUrl = "javascript:rw(" & n.Value & ",'p41')"
            Case "j02" : n.ImageUrl = "Images/person.png" : n.NavigateUrl = "javascript:rw(" & n.Value & ",'j02')"
            Case "p56" : n.ImageUrl = "Images/task.png" : n.NavigateUrl = "javascript:rw(" & n.Value & ",'p56')"
            Case "p91" : n.ImageUrl = "Images/invoice.png" : n.NavigateUrl = "javascript:rw(" & n.Value & ",'p91')"
        End Select
        If Not cRec.IsFinalLevel Then
            n.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
        End If
        If cRec.IsClosed Then n.Font.Strikeout = True

        If nParent Is Nothing Then
            tr1.Nodes.Add(n)
        Else
            nParent.Nodes.Add(n)
        End If

        Return n

    End Function

  

    Private Sub tr1_NodeExpand(sender As Object, e As RadTreeNodeEventArgs) Handles tr1.NodeExpand
        RefreshTreeData(e.Node)
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        ''ValidateLevels()
        RefreshTreeData(Nothing)
    End Sub

   

    Private Sub RestartData(bolUpdateUserSetting As Boolean)
        If bolUpdateUserSetting Then            
            With Master.Factory.j03UserBL
              
                .SetUserParam("navigator-path", Me.cbxPath.SelectedValue)
                .SetUserParam("navigator-bin", Me.opgBIN.SelectedValue)
            End With
        End If
        Dim a() As String = Split(Me.cbxPath.SelectedValue & "----", "-")
        Me.hidLevel0.Value = a(0)
        Me.hidLevel1.Value = a(1)
        Me.hidLevel2.Value = a(2)
        Me.hidLevel3.Value = a(3)

        RefreshTreeData(Nothing)
    End Sub


    Private Function IsLastLevel(intLevel As Integer) As Boolean
        If intLevel = 0 And hidLevel1.Value = "" Then Return True
        If intLevel = 1 And hidLevel2.Value = "" Then Return True
        If intLevel = 2 And hidLevel3.Value = "" Then Return True
        Return False
    End Function

    Private Sub opgBIN_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgBIN.SelectedIndexChanged
        RestartData(True)
        hidSettingIsActive.Value = "1"
    End Sub

    Private Sub cbxPath_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPath.SelectedIndexChanged
        RestartData(True)
    End Sub
End Class