Public Class o23_record_readonly
    Inherits System.Web.UI.UserControl
    Public Factory As BL.Factory
    Private Property _curRec As BO.o23Doc
    Private Property _isEncrypted As Boolean

    
    Public Function IsEmpty() As Boolean
        If rpFF.Items.Count > 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Property X18ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidX18ID.Value)
        End Get
        Set(value As Integer)
            hidX18ID.Value = value.ToString
        End Set
    End Property
    Public ReadOnly Property PID As Integer
        Get
            Return BO.BAS.IsNullInt(hidPID.Value)
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub FillData(cRec As BO.o23Doc, cX18 As BO.x18EntityCategory, bolShowBoundEntities As Boolean)
        hidPID.Value = cRec.PID.ToString
        If cRec.o23IsEncrypted Then
            _isEncrypted = True
            Factory.o23DocBL.DecryptRecord(cRec)
        End If

        _curRec = cRec
        If Not cX18 Is Nothing Then
            Me.X18ID = cX18.PID
        End If
        If Me.X18ID <> 0 And cX18 Is Nothing Then
            cX18 = Me.Factory.x18EntityCategoryBL.Load(Me.X18ID)
        End If
        With cRec
            Me.x18Name.Text = .DocType
            If .o23Name <> "" Then
                Me.o23Name.Text = .o23Name
                If .o23BackColor <> "" Then Me.o23Name.Style.Item("background-color") = .o23BackColor
                If .o23ForeColor <> "" Then Me.o23Name.Style.Item("color") = .o23ForeColor
            Else
                trName.Visible = False
            End If
            If .o23Code <> "" Then
                Me.o23Code.Text = .o23Code
                Me.o23Code.ToolTip = .o23ArabicCode
            Else
                trCode.Visible = False
            End If
            If cRec.b02ID <> 0 Then
                b02Name.Text = .b02Name
                If .b02Color <> "" Then
                    b02Name.Style.Item("background-color") = .b02Color
                End If
            Else
                trB02Name.Visible = False
            End If
            Me.Timestamp.Text = .Timestamp
        End With

        ''Dim s As String = Me.Factory.o23DocBL.LoadFolders(cRec.PID)
        

        If bolShowBoundEntities Then
            'Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Me.Factory.x18EntityCategoryBL.GetList_x20_join_x18(Me.X18ID).Where(Function(p) p.x20EntryModeFlag = BO.x20EntryModeENUM.InsertUpdateWithoutCombo Or p.x20EntryModeFlag = BO.x20EntryModeENUM.ExternalByWorkflow)
            Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Me.Factory.x18EntityCategoryBL.GetList_x20_join_x18(Me.X18ID)
            'Dim x20IDs As List(Of Integer) = lisX20X18.Where(Function(p) p.x29ID <> 331).Select(Function(p) p.x20ID).ToList
            Dim x20IDs As List(Of Integer) = lisX20X18.Select(Function(p) p.x20ID).ToList
            Dim lisX19 As IEnumerable(Of BO.x19EntityCategory_Binding) = Me.Factory.x18EntityCategoryBL.GetList_X19(cRec.PID, x20IDs, True).OrderBy(Function(p) p.x20IsMultiselect).ThenBy(Function(p) p.x20ID).ThenBy(Function(p) p.RecordAlias)

            rpX19.DataSource = lisX19
            rpX19.DataBind()
        Else
            rpX19.Visible = False
        End If
        

        RefreshUserFields()
    End Sub

    

    Private Sub RefreshUserFields()
        Dim lisX16 As IEnumerable(Of BO.x16EntityCategory_FieldSetting) = Me.Factory.x18EntityCategoryBL.GetList_x16(Me.X18ID)
        Dim bolHTML As Boolean = False
        If lisX16.Where(Function(p) p.x16Field = "o23HtmlContent").Count > 0 Then
            bolHTML = True
            lisX16 = lisX16.Where(Function(p) p.x16Field <> "o23HtmlContent")
        End If
        If lisX16.Count > 0 Then

            rpFF.DataSource = lisX16
            rpFF.DataBind()

        End If

        panHtml.Visible = bolHTML
    End Sub

    Private Sub rpFF_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpFF.ItemDataBound
        Dim cRec As BO.x16EntityCategory_FieldSetting = CType(e.Item.DataItem, BO.x16EntityCategory_FieldSetting)

        With CType(e.Item.FindControl("x16Name"), Label)
            Select Case cRec.FieldType
                Case BO.x24IdENUM.tDecimal : .Text = "<img src='Images/type_number.png'/> "
                Case BO.x24IdENUM.tBoolean : .Text = "<img src='Images/type_checkbox.png'/> "
                Case BO.x24IdENUM.tDate, BO.x24IdENUM.tDateTime : .Text = "<img src='Images/type_date.png'/> "
                Case Else : .Text = "<img src='Images/type_text.png'/> "
            End Select
            .Text += cRec.x16Name & ":"

        End With


        Dim curValue As Object = BO.BAS.GetPropertyValue(_curRec, cRec.x16Field)
        If curValue Is System.DBNull.Value Then curValue = Nothing
        If Not curValue Is Nothing Then
            Select Case cRec.FieldType
                Case BO.x24IdENUM.tString
                    With CType(e.Item.FindControl("valFF"), Label)
                        If curValue.ToString.IndexOf(vbCrLf) > 0 Or cRec.x16Field = "o23BigText" Then
                            .Text = BO.BAS.CrLfText2Html(curValue.ToString)
                            .CssClass = "val"
                            .Font.Italic = True
                        Else
                            .Text = curValue
                        End If
                    End With



                Case BO.x24IdENUM.tDecimal
                    CType(e.Item.FindControl("valFF"), Label).Text = BO.BAS.FN2(curValue)

                Case BO.x24IdENUM.tBoolean
                    If curValue = True Then
                        CType(e.Item.FindControl("valFF"), Label).Text = "ANO"
                    Else
                        CType(e.Item.FindControl("valFF"), Label).Text = "NE"
                    End If
                Case BO.x24IdENUM.tDateTime
                    With CType(e.Item.FindControl("valFF"), Label)
                        .Text = BO.BAS.FD(curValue, True, True)
                        If CDate(curValue) > Now Then
                            .ForeColor = Drawing.Color.Green
                        Else
                            .ForeColor = Drawing.Color.Red
                        End If
                    End With

            End Select
        End If

    End Sub

    Private Sub rpX19_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX19.ItemDataBound
        Dim cRec As BO.x19EntityCategory_Binding = CType(e.Item.DataItem, BO.x19EntityCategory_Binding)
        Dim strClue As String = ""
        With CType(e.Item.FindControl("BindName"), Label)
            Select Case cRec.x29ID
                Case 102
                    .Text = "<img src='Images/person.png'/> "
                    strClue = "clue_j02_record.aspx?pid=" & cRec.x19RecordPID.ToString
                Case 141
                    .Text = "<img src='Images/project.png'/> "
                    strClue = "clue_p41_record.aspx?pid=" & cRec.x19RecordPID.ToString
                Case 328
                    .Text = "<img src='Images/contact.png'/> "
                    strClue = "clue_p28_record.aspx?pid=" & cRec.x19RecordPID.ToString
                Case 391
                    .Text = "<img src='Images/invoice.png'/> "
                    strClue = "clue_p91_record.aspx?pid=" & cRec.x19RecordPID.ToString
                Case 331
                    .Text = "<img src='Images/worksheet.png'/> "
                    panWorksheetGrid.Visible = True
                Case 223
                    .Text = "<img src='Images/notepad.png'/> "
                    strClue = "clue_o23_record.aspx?pid=" & cRec.x19RecordPID.ToString
                Case Else : .Text = "<img src='Images/record.png'/> "
            End Select
            If cRec.x20Name <> "" Then
                .Text += cRec.x20Name & ":"
            Else
                .Text += BO.BAS.GetX29EntityAlias(CType(cRec.x29ID, BO.x29IdEnum), False) & ":"
            End If


        End With

        With CType(e.Item.FindControl("BindValue"), Label)
            Select Case cRec.x29ID
                Case 102 And Factory.SysUser.j04IsMenu_People = True
                    .Text = "<a href='j02_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                Case 141 And Factory.SysUser.j04IsMenu_Project = True
                    .Text = "<a href='p41_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                Case 328 And Factory.SysUser.j04IsMenu_Contact = True
                    .Text = "<a href='p28_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                Case 356 And Factory.SysUser.j04IsMenu_Task = True
                    .Text = "<a href='p56_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                Case 391 And Factory.SysUser.j04IsMenu_Invoice = True
                    .Text = "<a href='p91_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                Case 223 And Factory.SysUser.j04IsMenu_Notepad
                    .Text = "<a href='o23_framework.aspx?pid=" & cRec.x19RecordPID.ToString & "' target='_top'>" & cRec.RecordAlias & "</a>"
                
            End Select
            If .Text = "" Then .Text = cRec.RecordAlias
        End With
        With CType(e.Item.FindControl("pm1"), HyperLink)
            .Attributes("onclick") = "RCM('" & BO.BAS.GetDataPrefix(cRec.x29ID) & "'," & cRec.x19RecordPID.ToString & ",this)"

        End With
        If strClue <> "" Then
            With CType(e.Item.FindControl("clue1"), HyperLink)
                .Attributes("rel") = strClue

            End With
        Else
            e.Item.FindControl("clue1").Visible = False
        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If panHtml.Visible And BO.BAS.IsNullInt(hidPID.Value, 0) > 0 Then
            Dim s As String = Me.Factory.o23DocBL.LoadHtmlContent(CInt(hidPID.Value))
            If _isEncrypted Then
                s = Factory.o23DocBL.DecryptString(s)
            End If
            place1.Controls.Add(New LiteralControl(s))
        End If
    End Sub
End Class