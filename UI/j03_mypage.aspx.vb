Imports Telerik.Web.UI
Imports System.Web.Script.Serialization

Public Class j03_mypage
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub j03_mypage_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

        
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.SiteMenuValue = "dashboard"
            lblHeader.Text = BO.BAS.OM2(Me.lblHeader.Text, Master.Factory.SysUser.Person)

            cmdReadUpgradeInfo.Visible = Master.Factory.SysUser.j03IsShallReadUpgradeInfo
        End If

        RefreshRecord()
    End Sub

    Private Sub j03_mypage_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        basUIMT.MakeDockZonesUserFriendly(Me.docklayout1, False)
    End Sub

    Private Sub RefreshRecord()
        SetAllDocksInvisible()

        Dim lisProperties As IEnumerable(Of BO.x56SnippetProperty) = Master.Factory.x55HtmlSnippetBL.GetList_Properties(0)

        Dim cRec As BO.x58UserPage = InhaleRec()
        Dim lisDocks As IEnumerable(Of BO.x57UserPageBinding) = Master.Factory.x58UserPage.GetList_x57(cRec.PID)

        For Each cX57 In lisDocks
            Dim place As PlaceHolder = GetPH(cX57.x57DockID)
            Dim dock As RadDock = GetDC(cX57.x57DockID)
            dock.Visible = True
            Dim strDockHeight As String = cX57.x57Height
            If strDockHeight = "" And cX57.x55Height <> "" Then
                strDockHeight = cX57.x55Height
            End If
            If strDockHeight <> "" Then
                dock.Height = Unit.Parse(strDockHeight)
            End If
            dock.Title = cX57.x55Name

            Select Case cX57.x55TypeFlag
                Case BO.x55TypeENUM.DynamicHtml
                    Dim s As String = cX57.x55Content
                    If cX57.x55RecordSQL <> "" Then
                        s = Master.Factory.pluginBL.MergeRecordSQL(cX57.x55RecordSQL, s)

                    End If
                    For i As Integer = 1 To 9
                        Dim strTabID As String = "tab" & i.ToString
                        If s.IndexOf("#" & strTabID & "#") >= 0 Then
                            Dim p1 As New BO.PluginDataTable
                            p1.AddDbParameter("pid", Master.Factory.SysUser.j02ID)
                            p1.SQL = FindSnippetProperty(lisProperties, cX57.x55ID, strTabID, "SQL")
                            p1.ColHeaders = FindSnippetProperty(lisProperties, cX57.x55ID, strTabID, "ColHeaders")
                            p1.ColTypes = FindSnippetProperty(lisProperties, cX57.x55ID, strTabID, "ColTypes")
                            p1.NoDataMessage = FindSnippetProperty(lisProperties, cX57.x55ID, strTabID, "NoDataMessage")

                            s = Replace(s, "#" & strTabID & "#", Master.Factory.pluginBL.CreateDataTableIntoString(p1))

                        End If
                    Next

                    place.Controls.Add(New LiteralControl(s))
                Case BO.x55TypeENUM.StaticHtml
                    place.Controls.Add(New LiteralControl("<div>" & cX57.x55Content & "</div>"))
                Case BO.x55TypeENUM.ExternalPage

                    place.Controls.Add(New LiteralControl("<iframe width='100%' height='" & strDockHeight & "' src='" & cX57.x55Content & "' frameborder='0'></iframe>"))
            End Select


        Next
    End Sub

    Private Sub SetAllDocksInvisible()

        For Each z In Me.docklayout1.RegisteredZones
            For Each d In z.Docks
                d.Visible = False
            Next
        Next
    End Sub
    Private Function GetDC(strDockID As String) As RadDock
        For Each z In Me.docklayout1.RegisteredZones
            For Each d In z.Docks
                If d.ID = strDockID Then Return d
            Next
        Next
        Return Nothing
    End Function

    Private Function FindSnippetProperty(lis As IEnumerable(Of BO.x56SnippetProperty), intX55ID As Integer, strControl As String, strProperty As String) As String
        If lis.Where(Function(p) p.x55ID = intX55ID And p.x56Control = strControl And p.x56ControlPropertyName.ToLower = strProperty.ToLower).Count > 0 Then
            Return lis.Where(Function(p) p.x55ID = intX55ID And p.x56Control = strControl And p.x56ControlPropertyName.ToLower = strProperty.ToLower)(0).x56ControlPropertyValue
        Else
            Return ""
        End If
        
    End Function

    Private Function GetPH(strDockID As String) As PlaceHolder
        Select Case strDockID.ToLower
            Case "dock1" : Return Me.place_dock1
            Case "dock2" : Return Me.place_dock2
            Case "dock3" : Return Me.place_dock3
            Case "dock4" : Return Me.place_dock4
            Case "dock5" : Return Me.place_dock5
            Case "dock6" : Return Me.place_dock6
            Case "dock7" : Return Me.place_dock7
            Case "dock8" : Return Me.place_dock8
            Case "dock9" : Return Me.place_dock9
            Case Else
                Return Me.place99
        End Select
    End Function

    
    Protected Sub Handle_DockPositionChanged(sender As Object, e As DockPositionChangedEventArgs)
        Master.Notify(e.DockZoneID)
    End Sub

    

    Private Sub docklayout1_LoadDockLayout(sender As Object, e As DockLayoutEventArgs) Handles docklayout1.LoadDockLayout
        If Not Page.IsPostBack Then
            Dim c As BO.x58UserPage = InhaleRec()

            If c.x58DockState <> "" Then
                LoadState(c.x58DockState, e.Positions, e.Indices)
            End If
            If c.x58Skin <> "" Then
                Me.docklayout1.Skin = c.x58Skin
                basUI.SelectDropdownlistValue(Me.cbxSkin, c.x58Skin)
            End If

           
        End If
    End Sub

    Private Sub LoadState(myDockState As String, dockParents As Dictionary(Of String, String), dockIndices As Dictionary(Of String, Integer))
        Dim dock As New RadDock()
        Dim serializer As New JavaScriptSerializer()
        Dim converters As New List(Of JavaScriptConverter)()
        converters.Add(New UnitConverter())
        serializer.RegisterConverters(converters)
        For Each str As String In myDockState.Split("|"c)
            If str <> [String].Empty Then
                Dim state As DockState = serializer.Deserialize(Of DockState)(str)
                dock = TryCast(docklayout1.FindControl(state.UniqueName), RadDock)
                dock.ApplyState(state)
                dockParents(state.UniqueName) = state.DockZoneID
                dockIndices(state.UniqueName) = state.Index
            End If
        Next
    End Sub
    Private Function SaveState() As String
        Dim dockStates As List(Of DockState) = docklayout1.GetRegisteredDocksState()
        Dim serializer As New JavaScriptSerializer()
        Dim converters As New List(Of JavaScriptConverter)()
        converters.Add(New UnitConverter())
        serializer.RegisterConverters(converters)

        Dim stateString As String = [String].Empty
        For Each state As DockState In dockStates
            Dim ser As String = serializer.Serialize(state)
            stateString = stateString + "|" + ser
        Next
        Return stateString
    End Function

    Private Sub cmdSaveState_Click(sender As Object, e As EventArgs) Handles cmdSaveState.Click
        Dim c As BO.x58UserPage = InhaleRec()
        c.x58DockState = SaveState()
        Master.Factory.x58UserPage.Save(c, Nothing)
        Master.Notify("Rozložení boxů bylo uloženo", NotifyLevel.InfoMessage)
    End Sub

    Private Sub cmdResetState_Click(sender As Object, e As EventArgs) Handles cmdResetState.Click
        Dim c As BO.x58UserPage = InhaleRec()
        c.x58DockState = ""
        Dim lis As List(Of BO.x57UserPageBinding) = Master.Factory.x58UserPage.GetList_x57(c.PID).OrderBy(Function(p) p.x57ID).ToList, x As Integer = 1
        For Each box In lis
            box.x57DockID = "dock" & x.ToString
            x += 1
        Next
        Master.Factory.x58UserPage.Save(c, lis)

        ReloadPage()
    End Sub

    Private Function InhaleRec() As BO.x58UserPage
        Dim cRec As BO.x58UserPage = Master.Factory.x58UserPage.LoadByJ03(Master.Factory.SysUser.PID)
        If cRec Is Nothing Then
            cRec = New BO.x58UserPage
            cRec.j03ID = Master.Factory.SysUser.PID
            Dim boxes As IEnumerable(Of BO.x55HtmlSnippet) = Master.Factory.x55HtmlSnippetBL.GetList().Where(Function(p) p.x55IsSystem = True And (p.x55Code = "vykazovane_projekty" Or p.x55Code = "clue_j02_month" Or p.x55Code = "ukoly" Or p.x55Code = "udalosti"))
            Dim lis As New List(Of BO.x57UserPageBinding), x As Integer = 1
            For Each box In boxes
                Dim c As New BO.x57UserPageBinding
                c.x55ID = box.PID
                c.x57DockID = "dock" & x.ToString
                lis.Add(c)
                x += 1
            Next
            Master.Factory.x58UserPage.Save(cRec, lis)
            cRec = Master.Factory.x58UserPage.LoadByJ03(Master.Factory.SysUser.PID)
        End If
        Return cRec
    End Function

    Private Sub cbxSkin_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSkin.SelectedIndexChanged
        Dim c As BO.x58UserPage = InhaleRec()
        c.x58Skin = Me.cbxSkin.SelectedValue
        Master.Factory.x58UserPage.Save(c, Nothing)
        ReloadPage()

    End Sub

    Private Sub ReloadPage()
        Response.Redirect("j03_mypage.aspx")
    End Sub
End Class