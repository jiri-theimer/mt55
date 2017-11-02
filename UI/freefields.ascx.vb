Imports Telerik.Web.UI


Public Class freefields
    Inherits System.Web.UI.UserControl
    Public Event RenderField(strName As String, ByVal strField As String, ByVal strDataType As String, ByVal dbValue As Object)
    Private _Error As String
    Private _dataprefix As String
    Private _lastX27ID As Integer
    Private _lisO23 As IEnumerable(Of BO.o23Doc)
    Public Factory As BL.Factory

   
    Public Function IsEmpty() As Boolean
        If rpFF.Items.Count > 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public ReadOnly Property FieldsCount As Integer
        Get
            Return rpFF.Items.Count
        End Get
    End Property
    Public ReadOnly Property TagsCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property

    Public Property DataTable() As String
        Get
            Return hidDataTable.Value
        End Get
        Set(ByVal value As String)
            hidDataTable.Value = value
            _dataprefix = Left(value, 3)
        End Set
    End Property
    Public Property DataPID As Integer
        Get
            Return BO.BAS.IsNullInt(hidDataPID.Value)
        End Get
        Set(value As Integer)
            hidDataPID.Value = value.ToString
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
       
    End Sub
    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        If e.CommandName = "clear" Then
            CType(e.Item.FindControl("hidO23IDs_Doc"), HiddenField).Value = ""
            RefreshDocListComplete()
        End If
    End Sub
    Public Sub RefreshDocListByOneDocument(intO23ID_Append As Integer)
        'obnoví pouze seznam dokumentů podle hodnot v hidO23IDs_Doc
        Dim c As BO.o23Doc = Factory.o23DocBL.Load(intO23ID_Append)
        For Each ri As RepeaterItem In rp1.Items
            If c.x18ID.ToString = CType(ri.FindControl("x18ID"), HiddenField).Value Then
                Dim pids As String = CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value
                If pids = "" Then pids = intO23ID_Append.ToString Else pids += "," & intO23ID_Append.ToString
                Dim lis As List(Of Integer) = BO.BAS.ConvertPIDs2List(pids, ",")

                CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value = String.Join(",", lis)

                With CType(ri.FindControl("rpO23IDs_Doc"), Repeater)
                    Dim mq As New BO.myQueryO23(0)
                    mq.PIDs = lis
                    .DataSource = Factory.o23DocBL.GetList(mq)
                    .DataBind()
                End With
                ri.FindControl("cmdO23_Clear").Visible = True
            End If
        Next
    End Sub
    Public Sub RefreshDocListComplete()
        For Each ri As RepeaterItem In rp1.Items
            If Not ri.FindControl("o23IDs_Tag").Visible Then
                Dim pids As String = CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value
                Dim lis As List(Of Integer) = BO.BAS.ConvertPIDs2List(pids, ",")
                ri.FindControl("cmdO23_Clear").Visible = False
                With CType(ri.FindControl("rpO23IDs_Doc"), Repeater)
                    If lis.Count > 0 Then
                        Dim mq As New BO.myQueryO23(0)
                        mq.PIDs = lis
                        Dim lisO23 As IEnumerable(Of BO.o23Doc) = Factory.o23DocBL.GetList(mq)
                        .DataSource = lisO23
                        .DataBind()
                        CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value = String.Join(",", lisO23.Select(Function(p) p.PID))
                        If lisO23.Count > 0 Then
                            ri.FindControl("cmdO23_Clear").Visible = True
                        End If
                    Else
                        .DataSource = Nothing
                        .DataBind()
                        CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value = ""
                    End If
                   
                End With

            End If           
        Next
    End Sub
    Public Sub FillData(ByVal listFF As IEnumerable(Of BO.FreeField), lisX20X18 As IEnumerable(Of BO.x20_join_x18), strDataTable As String, intDataPID As Integer, Optional strTempGUID As String = "")
        lisX20X18 = lisX20X18.Where(Function(p) p.x20IsClosed = False And p.x20EntryModeFlag = BO.x20EntryModeENUM.Combo)   'omezit pouze na otevřené vazby + vazby vyplňované přes combo
        'strTempGUID je tu pouze pro agendu kategorií
        Me.DataTable = strDataTable
        Me.DataPID = intDataPID

        rpFF.DataSource = listFF
        rpFF.DataBind()

        
        Dim curTags As List(Of BO.x19EntityCategory_Binding) = GetTags()    'naposledy ve formuláři vyplněné kategorie - pokud existují, mají přednost
        rp1.DataSource = lisX20X18
        rp1.DataBind()
        If rp1.Items.Count = 0 Then
            rp1.Visible = False 'žádné kategorie/dokumenty k vyplnění
        Else
            rp1.Visible = True
            If Me.DataPID <> 0 Then
                Dim lisX19 As List(Of BO.x19EntityCategory_Binding) = Nothing
                If curTags.Count = 0 Then
                    Dim x20IDs As List(Of Integer) = lisX20X18.Select(Function(p) p.x20ID).ToList
                    lisX19 = Me.Factory.x18EntityCategoryBL.GetList_X19(BO.BAS.GetX29FromPrefix(_dataprefix), intDataPID, strTempGUID, x20IDs).ToList
                Else
                    lisX19 = curTags
                End If
                For Each ri As RepeaterItem In rp1.Items
                    Dim intX20ID As Integer = CInt(CType(ri.FindControl("x20ID"), HiddenField).Value)
                    Dim lis As List(Of String) = lisX19.Where(Function(p) p.x20ID = intX20ID).Select(Function(p) p.o23ID.ToString).ToList
                    ri.FindControl("cmdO23_Clear").Visible = False
                    If lis.Count > 0 Then
                        If ri.FindControl("o23IDs_Tag").Visible Then
                            'kategorie
                            With CType(ri.FindControl("o23IDs_Tag"), UI.datacombo)
                                If .AllowCheckboxes Then
                                    .SelectCheckboxItems(lis)
                                Else
                                    If lis.Count > 0 Then .SelectedValue = lis(0)
                                End If

                            End With
                        Else
                            'dokument
                            With CType(ri.FindControl("hidO23IDs_Doc"), HiddenField)
                                .Value = String.Join(",", lis)
                            End With
                            With CType(ri.FindControl("rpO23IDs_Doc"), Repeater)
                                Dim mq As New BO.myQueryO23(0)
                                mq.PIDs = BO.BAS.ConvertPIDs2List(String.Join(",", lis), ",")
                                .DataSource = Factory.o23DocBL.GetList(mq)
                                .DataBind()
                                If .Items.Count > 0 Then
                                    ri.FindControl("cmdO23_Clear").Visible = True
                                End If
                            End With
                        End If

                    End If
                Next
            End If

        End If

        If Me.rpFF.Items.Count > 0 Or rp1.Items.Count > 0 Then
            Me.panContainer.Visible = True
        Else
            Me.panContainer.Visible = False
        End If

    End Sub

    

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.x20_join_x18 = CType(e.Item.DataItem, BO.x20_join_x18)
        With CType(e.Item.FindControl("x18Name"), Label)
            If cRec.x18Icon = "" Then
                .Text = "<img src='Images/label.png'/>"
                If cRec.x18IsManyItems Then .Text = "<img src='Images/notepad.png'/>"
            Else
                .Text = "<img src='" & cRec.x18Icon & "' alt='Kategorie'/>"
            End If
            If cRec.x20Name = "" Then
                .Text += " " & cRec.x18Name & ":"
            Else
                .Text += " " & cRec.x20Name & ":"
            End If
        End With
        e.Item.FindControl("o23IDs_Tag").Visible = False
        e.Item.FindControl("cmdX18_Items").Visible = False
        e.Item.FindControl("cmdO23_Create").Visible = False
        e.Item.FindControl("cmdO23_Find").Visible = False
        e.Item.FindControl("cmdO23_Clear").Visible = False
        If cRec.x18IsManyItems Then
            'dokument
            With CType(e.Item.FindControl("cmdO23_Create"), HtmlButton)
                .Visible = True
                .Attributes.Item("onclick") = "o23_create_x18(" & cRec.x18ID.ToString & ")"
            End With
            With CType(e.Item.FindControl("cmdO23_Find"), HtmlButton)
                .Visible = True
                .Attributes.Item("onclick") = "o23_find_x18(" & cRec.x18ID.ToString & "," & cRec.x20ID.ToString & ")"
            End With
        Else
            'kategorie
            With CType(e.Item.FindControl("cmdX18_Items"), HtmlButton)
                .Visible = True
                .Attributes.Item("onclick") = "x18_items(" & cRec.x18ID.ToString & ")"
            End With

            Dim lisO23 As IEnumerable(Of BO.o23Doc) = Me.Factory.o23DocBL.GetList(New BO.myQueryO23(cRec.x23ID)).Where(Function(p) p.IsClosed = False)
            With CType(e.Item.FindControl("o23IDs_Tag"), UI.datacombo)
                .Visible = True
                If Not cRec.x20IsMultiSelect Then
                    .AllowCheckboxes = False
                    .IsFirstEmptyRow = True
                    If lisO23.Count > 30 Then .Filter = datacombo.FilterMode.Contains
                End If
                .DataSource = lisO23
                .DataBind()
            End With
        End If
        

        
        CType(e.Item.FindControl("x18ID"), HiddenField).Value = cRec.x18ID.ToString
        CType(e.Item.FindControl("x20ID"), HiddenField).Value = cRec.x20ID.ToString
        'CType(e.Item.FindControl("x18IsMultiSelect"), HiddenField).Value = BO.BAS.GB(cRec.x18IsMultiSelect)
    End Sub

    Private Sub rpFF_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpFF.ItemDataBound
        Dim cRec As BO.FreeField = CType(e.Item.DataItem, BO.FreeField)

        e.Item.FindControl("txtFF_Number").Visible = False
        e.Item.FindControl("txtFF_Text").Visible = False
        e.Item.FindControl("chkFF").Visible = False
        e.Item.FindControl("txtFF_Date").Visible = False
        e.Item.FindControl("cbxFF").Visible = False
        CType(e.Item.FindControl("x28IsRequired"), HiddenField).Value = BO.BAS.GB(cRec.x28IsRequired)

        If BO.BAS.IsNullInt(cRec.x27ID) <> _lastX27ID Then
            e.Item.FindControl("trHeader").Visible = True
            CType(e.Item.FindControl("headerFF"), Label).Text = cRec.X27Name
        End If
        _lastX27ID = BO.BAS.IsNullInt(cRec.X27ID)
        With CType(e.Item.FindControl("lblFF"), Label)
            .Text = "<img src='Images/form.png'/> " & cRec.x28Name & ":"
            If cRec.x28IsRequired Then
                .ForeColor = Drawing.Color.Red
                .Text = .Text & "*"
            End If
        End With

        CType(e.Item.FindControl("hidField"), HiddenField).Value = cRec.x28Field
        CType(e.Item.FindControl("hidType"), HiddenField).Value = cRec.TypeName
        CType(e.Item.FindControl("hidX28ID"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("hidX23ID"), HiddenField).Value = cRec.x23ID.ToString

        If cRec.x28HelpText = "" Then
            e.Item.FindControl("clue_help").Visible = False
        Else
            e.Item.FindControl("clue_help").Visible = True
            CType(e.Item.FindControl("clue_help"), HyperLink).Attributes.Item("rel") = "clue_help.aspx?prefix=x28&pid=" & cRec.PID.ToString
        End If

        Select Case cRec.TypeName
            Case "string"
                If cRec.x28DataSource <> "" Then
                    e.Item.FindControl("cbxFF").Visible = True
                    Dim a() As String = Split(cRec.x28DataSource, ";")
                    With CType(e.Item.FindControl("cbxFF"), RadComboBox)
                        .AllowCustomText = Not cRec.x28IsFixedDataSource
                        If .AllowCustomText Then .Items.Add(New RadComboBoxItem(" "))
                        For i As Integer = 0 To UBound(a)
                            .Items.Add(New RadComboBoxItem(a(i)))
                        Next
                        If Not cRec.DBValue Is System.DBNull.Value Then
                            If .AllowCustomText Then
                                .Text = cRec.DBValue
                            Else
                                Try
                                    .Items.FindItemByText(cRec.DBValue).Selected = True
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                        If cRec.x28TextboxWidth > 0 Then
                            .Style.Item("width") = cRec.x28TextboxWidth.ToString & "px"
                        End If
                    End With
                Else
                    With CType(e.Item.FindControl("txtFF_Text"), TextBox)
                        .Visible = True
                        If cRec.x28TextboxWidth > 40 Then
                            .Style.Item("width") = cRec.x28TextboxWidth.ToString & "px"
                        End If
                        If cRec.x28TextboxHeight > 20 Then
                            .Style.Item("height") = cRec.x28TextboxHeight.ToString & "px"
                            .TextMode = TextBoxMode.MultiLine
                        End If
                        If Not cRec.DBValue Is System.DBNull.Value Then
                            .Text = cRec.DBValue
                        End If
                    End With
                End If

            Case "decimal"
                With CType(e.Item.FindControl("txtFF_Number"), RadNumericTextBox)
                    .Visible = True
                    .NumberFormat.DecimalDigits = 2
                    If Not cRec.DBValue Is System.DBNull.Value Then .Value = CDbl(cRec.DBValue)
                End With
            Case "integer"
                If cRec.x23ID <> 0 Then
                    If cRec.x23DataSource = "" Then
                        'combo seznam bez externího datového zdroje
                        If _lisO23 Is Nothing Then _lisO23 = Me.Factory.o23DocBL.GetList(New BO.myQueryO23(cRec.x23ID))
                        With CType(e.Item.FindControl("cbxFF"), RadComboBox)
                            .Visible = True
                            .AllowCustomText = False
                            .ShowToggleImage = True
                            .Items.Add(New RadComboBoxItem(""))
                            For Each c In _lisO23.Where(Function(p) p.IsClosed = False And p.x23ID = cRec.x23ID)
                                .Items.Add(New RadComboBoxItem(c.NameWithCode, c.PID.ToString))
                            Next
                            If Not (cRec.DBValue Is Nothing Or cRec.DBValue Is System.DBNull.Value) Then
                                .SelectedValue = cRec.DBValue.ToString
                                If .SelectedValue = "" Then
                                    'najít uzavřenou položku
                                    Dim lis As IEnumerable(Of BO.o23Doc) = _lisO23.Where(Function(p) p.PID = CInt(cRec.DBValue))
                                    If lis.Count > 0 Then
                                        Dim cItem As BO.o23Doc = lis(0)
                                        If Not cItem Is Nothing Then
                                            Dim cbxItem As RadComboBoxItem = New RadComboBoxItem(cItem.NameWithCode, cItem.PID.ToString)
                                            cbxItem.Font.Strikeout = cItem.IsClosed
                                            .Items.Add(cbxItem)
                                        End If
                                        .SelectedValue = cRec.DBValue.ToString
                                    End If
                                End If
                            End If
                        End With
                    End If
                    If cRec.x23DataSource <> "" Then
                        'combobox s externím datovým zdrojem
                        With CType(e.Item.FindControl("cbxFF"), RadComboBox)
                            .Visible = True : .AllowCustomText = False : .ShowToggleImage = True
                            .EnableLoadOnDemand = True
                            .OnClientItemsRequesting = Me.ClientID & "_OnClientItemsRequesting"
                            .MarkFirstMatch = True
                            .EnableTextSelection = True

                            .WebServiceSettings.Method = "LoadComboData"
                            .WebServiceSettings.UseHttpGet = False
                            .WebServiceSettings.Path = "~/Services/combo_external_datasource.asmx"
                            .Attributes.Item("x23id") = cRec.x23ID.ToString

                            If Not (cRec.DBValue Is Nothing Or cRec.DBValue Is System.DBNull.Value) Then
                                .SelectedValue = cRec.DBValue.ToString
                                .Text = cRec.ComboText
                            End If
                        End With

                    End If
                Else
                    With CType(e.Item.FindControl("txtFF_Number"), RadNumericTextBox)
                        .Visible = True
                        .NumberFormat.DecimalDigits = 0
                        If Not cRec.DBValue Is System.DBNull.Value Then .Value = CDbl(cRec.DBValue)
                    End With
                End If

            Case "boolean"
                e.Item.FindControl("lblFF").Visible = False
                With CType(e.Item.FindControl("chkFF"), CheckBox)
                    .Visible = True
                    .Text = cRec.x28Name
                    If Not cRec.DBValue Is System.DBNull.Value Then
                        .Checked = cRec.DBValue
                    End If
                End With
            Case "date", "datetime", "time"
                With CType(e.Item.FindControl("txtFF_Date"), RadDateInput)
                    .Visible = True
                    .MinDate = DateSerial(1900, 1, 1)
                    .MaxDate = DateSerial(2100, 1, 1)
                    Select Case cRec.TypeName
                        Case "datetime"
                            .DateFormat = "dd.MM.yyyy HH:mm"
                        Case "time"
                            .DateFormat = "HH:mm"
                        Case Else
                            .DateFormat = "dd.MM.yyyy"
                    End Select

                    If Not cRec.DBValue Is System.DBNull.Value Then
                        If Year(cRec.DBValue) > 1900 Then .SelectedDate = cRec.DBValue
                    End If
                End With
        End Select
        With cRec
            RaiseEvent RenderField(.x28Name, .x28Field, .TypeName, .DBValue)
        End With

    End Sub
    Public Function GetX20IDs() As List(Of Integer)
        Dim lis As New List(Of Integer)
        For Each ri As RepeaterItem In rp1.Items
            lis.Add(CInt(CType(ri.FindControl("x20ID"), HiddenField).Value))
        Next
        Return lis
    End Function
    Public Function GetTags() As List(Of BO.x19EntityCategory_Binding)
        Dim lis As New List(Of BO.x19EntityCategory_Binding), intDataPID As Integer = BO.BAS.IsNullInt(Me.hidDataPID.Value)
        For Each ri As RepeaterItem In rp1.Items
            Dim intX20ID As Integer = CInt(CType(ri.FindControl("x20ID"), HiddenField).Value)
            Dim o23IDs As New List(Of Integer)
            With CType(ri.FindControl("o23IDs_Tag"), UI.datacombo)
                If ri.FindControl("o23IDs_Tag").Visible Then
                    If .AllowCheckboxes Then
                        o23IDs = .GetAllCheckedIntegerValues
                    Else
                        If .SelectedValue <> "" Then o23IDs.Add(CInt(.SelectedValue))
                    End If
                Else
                    If CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value <> "" Then
                        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(CType(ri.FindControl("hidO23IDs_Doc"), HiddenField).Value, ",")
                        o23IDs.AddRange(pids)
                    End If
                End If
                
            End With
            For Each into23ID As Integer In o23IDs
                Dim c As New BO.x19EntityCategory_Binding
                c.x20ID = intX20ID
                c.o23ID = into23ID
                c.x19RecordPID = intDataPID
                lis.Add(c)
            Next
        Next
        Return lis
    End Function
    Public Function GetValues() As List(Of BO.FreeField)
        Dim lis As New List(Of BO.FreeField)

        For Each ri As RepeaterItem In rpFF.Items
            Dim cRec As New BO.FreeField()
            With cRec
                .SetPID(BO.BAS.IsNullInt(CType(ri.FindControl("hidX28ID"), HiddenField).Value))
                .x28Field = CType(ri.FindControl("hidField"), HiddenField).Value
                .SetTypeFromName(CType(ri.FindControl("hidType"), HiddenField).Value)
                .x28Name = CType(ri.FindControl("lblFF"), Label).Text.Replace(":", "").Replace("*", "")
                .x28IsRequired = BO.BAS.BG(CType(ri.FindControl("x28IsRequired"), HiddenField).Value)
                .x23ID = BO.BAS.IsNullInt(CType(ri.FindControl("hidX23ID"), HiddenField).Value)


                Select Case .x24ID
                    Case BO.x24IdENUM.tString
                        If ri.FindControl("cbxFF").Visible Then
                            .DBValue = CType(ri.FindControl("cbxFF"), RadComboBox).Text
                        Else
                            .DBValue = CType(ri.FindControl("txtFF_Text"), TextBox).Text
                        End If
                    Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime, BO.x24IdENUM.tTime
                        With CType(ri.FindControl("txtFF_Date"), RadDateInput)
                            If .IsEmpty Then
                                cRec.DBValue = Nothing
                            Else
                                cRec.DBValue = .DbSelectedDate
                            End If
                        End With
                    Case BO.x24IdENUM.tDecimal
                        .DBValue = CType(ri.FindControl("txtFF_Number"), RadNumericTextBox).DbValue
                    Case BO.x24IdENUM.tInteger
                        If ri.FindControl("cbxFF").Visible Then
                            .DBValue = BO.BAS.IsNullInt(CType(ri.FindControl("cbxFF"), RadComboBox).SelectedValue)
                            .ComboText = CType(ri.FindControl("cbxFF"), RadComboBox).Text
                            If CType(ri.FindControl("cbxFF"), RadComboBox).EnableLoadOnDemand Then
                                .IsExternalDataSource = True    'externí datový zdroj
                            End If
                        Else
                            .DBValue = CType(ri.FindControl("txtFF_Number"), RadNumericTextBox).DbValue
                        End If
                    Case BO.x24IdENUM.tBoolean
                        .DBValue = CType(ri.FindControl("chkFF"), CheckBox).Checked
                End Select
            End With

            lis.Add(cRec)
        Next

        Return lis
    End Function



    

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not Me.Factory Is Nothing Then
            Me.hidJ03ID.Value = Factory.SysUser.PID.ToString
        End If

    End Sub
End Class