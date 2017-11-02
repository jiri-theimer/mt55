Public Class mygrid
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Public Property ModeFlag As Integer
        Get
            Return BO.BAS.IsNullInt(hidModeFlag.Value)
        End Get
        Set(value As Integer)
            hidModeFlag.Value = value.ToString
        End Set
    End Property
    Public Property Prefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
            hidX29ID.Value = CInt(BO.BAS.GetX29FromPrefix(value)).ToString
        End Set
    End Property
    Public Property Width As String
        Get
            Return Me.j70ID.Style.Item("width")
        End Get
        Set(value As String)
            Me.j70ID.Style.Item("width") = value
        End Set
    End Property
    Public Property MasterPrefix As String
        Get
            Return hidMasterPrefix.Value
        End Get
        Set(value As String)
            hidMasterPrefix.Value = value
        End Set
    End Property
    Public Property MasterPrefixFlag As Integer '1: pokud je masterprefix, zobrazují se přehledy pro masterprefix i s nevyplněným masterprefix, 2: zobrazují se přehledy pouze pro masterprefix
        Get
            Return CInt(hidMasterPrefixFlag.Value)
        End Get
        Set(value As Integer)
            hidMasterPrefixFlag.Value = value.ToString
        End Set
    End Property

    Public Property CurrentJ62ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidJ62ID.Value)
        End Get
        Set(value As Integer)
            hidJ62ID.Value = value.ToString
        End Set
    End Property
    Private ReadOnly Property CurrentX29ID As BO.x29IdEnum
        Get
            If hidX29ID.Value = "" Then Return BO.x29IdEnum._NotSpecified
            Return CType(CInt(hidX29ID.Value), BO.x29IdEnum)
        End Get
       
    End Property
    Public Property CurrentJ70ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.j70ID, value.ToString)
        End Set
    End Property
    Public ReadOnly Property CurrentName As String
        Get
            If Me.j70ID.SelectedItem Is Nothing Then
                Return ""
            Else
                Return Me.j70ID.SelectedItem.Text
            End If
        End Get
    End Property
    Public Property ReloadUrl As String
        Get
            Return hidReloadURL.Value
        End Get
        Set(value As String)
            hidReloadURL.Value = value
        End Set
    End Property
    Public Property x36Key As String
        Get
            Return hidX36Key.Value
        End Get
        Set(value As String)
            hidX36Key.Value = value
        End Set
    End Property
    Public Property AllowSettingButton As Boolean
        Get
            Return cmdSetting.Visible
        End Get
        Set(value As Boolean)
            cmdSetting.Visible = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub RefreshData(intDefJ70ID As Integer)
        SetupJ70Combo(intDefJ70ID)
    End Sub
    Private Sub SetupJ70Combo(intDef As Integer)
        Dim mq As New BO.myQuery
        Dim onlyQuery As BO.BooleanQueryMode = BO.BooleanQueryMode.NoQuery
        If hidModeFlag.Value = "2" Then
            onlyQuery = BO.BooleanQueryMode.TrueQuery
            cmdSetting.InnerHtml = "<img src='Images/query.png'/>"
        End If
        Dim s As String = Me.MasterPrefix
        If Me.MasterPrefix <> "" And Me.MasterPrefixFlag = 1 Then s = "-1"
        Dim lisJ70 As IEnumerable(Of BO.j70QueryTemplate) = Me.Factory.j70QueryTemplateBL.GetList(mq, Me.CurrentX29ID, s, onlyQuery)
        If Me.MasterPrefix <> "" Then
            lisJ70 = lisJ70.Where(Function(p) p.j70MasterPrefix = Me.MasterPrefix Or (p.j70MasterPrefix = "" And p.j70IsSystem = False))
        End If
        'If hidModeFlag.Value <> "2" Then
        If lisJ70.Where(Function(p) p.j70IsSystem = True).Count = 0 Then
            'uživatel zatím nemá výchozí systémovou šablonu přehledu - založit první j70IsSystem=1
            If Me.Factory.j70QueryTemplateBL.CheckDefaultTemplate(Me.CurrentX29ID, Factory.SysUser.PID, Me.MasterPrefix) Then
                If Me.MasterPrefix <> "" Then Threading.Thread.Sleep(1000 * 1) 'počkat  1 sekundu, jinak to zlobí -> pouze pro ne-prázdný masterprefix
                lisJ70 = Me.Factory.j70QueryTemplateBL.GetList(mq, Me.CurrentX29ID, s, onlyQuery)
                If Me.MasterPrefix <> "" Then
                    lisJ70 = lisJ70.Where(Function(p) p.j70MasterPrefix = Me.MasterPrefix Or (p.j70MasterPrefix = "" And p.j70IsSystem = False))
                End If
            End If
        End If
        'End If


        j70ID.DataSource = lisJ70
        j70ID.DataBind()
        If hidModeFlag.Value = "2" Then
            j70ID.Items.Insert(0, New ListItem("--Pojmenovaný filtr--", ""))
        End If



        If hidJ62ID.Value <> "" Then
            Dim cJ62 As BO.j62MenuHome = Me.Factory.j62MenuHomeBL.Load(Me.CurrentJ62ID)
            If Not cJ62 Is Nothing Then
                If cJ62.j70ID <> 0 Then
                    intDef = cJ62.j70ID
                    If Me.j70ID.Items.FindByValue(intDef.ToString) Is Nothing Then
                        Dim c As BO.j70QueryTemplate = Me.Factory.j70QueryTemplateBL.Load(intDef)
                        Me.j70ID.Items.Add(New ListItem(c.j70Name, intDef.ToString))
                    End If
                End If
            End If
        End If

        basUI.SelectDropdownlistValue(Me.j70ID, intDef.ToString)
        Me.clue_query.Visible = False
        With Me.j70ID
            If .SelectedIndex > 0 Then
                .ToolTip = .SelectedItem.Text
                Me.clue_query.Attributes("rel") = "clue_quickquery.aspx?j70id=" & .SelectedValue
                Me.clue_query.Visible = True
            End If
        End With
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        basUIMT.RenderQueryCombo(Me.j70ID)
    End Sub
End Class