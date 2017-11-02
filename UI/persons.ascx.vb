Public Class persons
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Public Event OnChange()


    Public ReadOnly Property CurrentHeader As String
        Get
            Return hidHeader.Value
        End Get
    End Property
    
    Public Property CurrentPersonsRole As String
        Get
            Return Me.cbxPersonsRole.SelectedValue
        End Get
        Set(value As String)
            basUI.SelectDropdownlistValue(Me.cbxPersonsRole, value)
        End Set
    End Property
    Public Property CurrentScope As Integer
        Get
            Return CInt(Me.opgScope.SelectedValue)
        End Get
        Set(value As Integer)
            Select Case value
                Case 2
                    If Me.j07ID_Add.Rows = 0 Then
                        SetupData()
                    End If
                Case 3
                    If Me.j70ID.Items.Count = 0 Then SetupJ70Combo(value)
            End Select
            basUI.SelectRadiolistValue(Me.opgScope, value.ToString)
        End Set
    End Property
    Public Function CurrentJ02IDs() As List(Of Integer)
        If hidJ02IDs_All.Value = "" Then
            Return New List(Of Integer)
        Else
            Return BO.BAS.ConvertPIDs2List(Me.hidJ02IDs_All.Value).Distinct.ToList
        End If
    End Function
    Public Property CurrentValue As String
        Get
            Select Case opgScope.SelectedValue
                Case "1"
                    Return ""
                Case "2"
                    Return Me.hidJ02IDs.Value & "|" & hidJ11IDs.Value & "|" & hidJ07IDs.Value
                Case "3"
                    Return Me.j70ID.SelectedValue
                Case "4"
                    Return Me.Factory.SysUser.j02ID.ToString & "||"
                Case Else
                    Return ""
            End Select
        End Get
        Set(value As String)
            Select Case opgScope.SelectedValue
                Case "2"
                    Dim a() As String = Split(value, "|")
                    hidJ02IDs.Value = a(0)
                    If UBound(a) > 0 Then hidJ11IDs.Value = a(1)
                    If UBound(a) > 1 Then hidJ07IDs.Value = a(2)

                Case "3"
                    If Me.j70ID.Items.Count = 0 Then SetupJ70Combo(BO.BAS.IsNullInt(value))
                    basUI.SelectDropdownlistValue(Me.j70ID, value)
            End Select
            RenderListAndCalculAllJ02IDs()
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub SetupQueryPersonsRoles(lis As List(Of BO.ComboSource))
        With Me.cbxPersonsRole
            .DataSource = lis
            .DataBind()
            If lis.Count > 0 Then
                .Visible = True
            Else
                .Visible = False
            End If
        End With
    End Sub

    Private Sub SetupData()
        Me.j11ID_Add.DataSource = Me.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
        Me.j11ID_Add.DataBind()
        Me.j11ID_Add.ChangeItemText("", "--Tým osob--")
        Me.j07ID_Add.DataSource = Me.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
        Me.j07ID_Add.DataBind()
        Me.j07ID_Add.ChangeItemText("", "--Pozice--")
        Me.j02ID_Add.Flag = "all"
        SetupJ70Combo(0)

    End Sub
    Private Sub SetupJ70Combo(intDef As Integer)
        Dim mq As New BO.myQuery
        j70ID.DataSource = Me.Factory.j70QueryTemplateBL.GetList(mq, BO.x29IdEnum.j02Person)
        j70ID.DataBind()
        j70ID.Items.Insert(0, "--Pojmenovaný filtr--")

        basUI.SelectDropdownlistValue(Me.j70ID, intDef.ToString)
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        panManual.Visible = False : panQuery.Visible = False
        Select Case Me.opgScope.SelectedValue
            Case "1"
            Case "2"
                panManual.Visible = True
            Case "3"
                panQuery.Visible = True
                With Me.j70ID
                    If .SelectedIndex > 0 Then
                        Me.clue_query.Visible = True
                        .ToolTip = .SelectedItem.Text
                        Me.clue_query.Attributes("rel") = "clue_quickquery.aspx?j70id=" & .SelectedValue
                    Else
                        Me.clue_query.Visible = False
                    End If
                End With
        End Select
        If Me.hidJ02IDs_All.Value <> "" Then
            linkClearAll.Visible = True
        Else
            linkClearAll.Visible = False
        End If
        With Me.cbxPersonsRole
            If .Items.Count = 0 Then .Visible = False Else .Visible = True
        End With
    End Sub
    

    Private Sub RenderListAndCalculAllJ02IDs()
        Dim j02ids_all As New List(Of Integer), ali As New List(Of String)
        If opgScope.SelectedValue = "2" Then
            If Me.hidJ07IDs.Value <> "" Then
                Dim mq As New BO.myQuery
                mq.PIDs = BO.BAS.ConvertPIDs2List(Me.hidJ07IDs.Value)
                rpJ07.DataSource = Me.Factory.j07PersonPositionBL.GetList(mq)

                Dim mqJ02 As New BO.myQueryJ02
                'mqJ02.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead
                mqJ02.MG_AdditionalSqlWHERE = "a.j07ID IN (" & hidJ07IDs.Value & ")"
                For Each x In Me.Factory.j02PersonBL.GetList(mqJ02).Select(Function(p) p.PID).ToList
                    j02ids_all.Add(x)
                Next
            Else
                rpJ07.DataSource = Nothing
            End If
            rpJ07.DataBind()
            If Me.hidJ11IDs.Value <> "" Then
                Dim mq As New BO.myQuery
                mq.PIDs = BO.BAS.ConvertPIDs2List(Me.hidJ11IDs.Value)
                rpJ11.DataSource = Me.Factory.j11TeamBL.GetList(mq)

                Dim mqJ02 As New BO.myQueryJ02
                'mqJ02.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead
                mqJ02.MG_AdditionalSqlWHERE = "a.j02ID IN (SELECT j02ID FROM j12Team_Person WHERE j11ID IN (" & hidJ11IDs.Value & "))"
                For Each x In Me.Factory.j02PersonBL.GetList(mqJ02).Select(Function(p) p.PID).ToList
                    j02ids_all.Add(x)
                Next
            Else
                rpJ11.DataSource = Nothing
            End If
            rpJ11.DataBind()
            If Me.hidJ02IDs.Value <> "" Then
                Dim mq As New BO.myQueryJ02
                mq.PIDs = BO.BAS.ConvertPIDs2List(Me.hidJ02IDs.Value)
                rpJ02.DataSource = Me.Factory.j02PersonBL.GetList(mq)

                For Each x In BO.BAS.ConvertPIDs2List(Me.hidJ02IDs.Value)
                    j02ids_all.Add(x)
                Next
            Else
                rpJ02.DataSource = Nothing
            End If
            rpJ02.DataBind()

            If j02ids_all.Count = 0 And (hidJ02IDs.Value <> "" Or hidJ07IDs.Value <> "" Or hidJ11IDs.Value <> "") Then
                se("Vstupní podmínce neodpovídá ani jeden osobní profil.")
            End If

            For Each ri As RepeaterItem In rpJ07.Items
                ali.Add(CType(ri.FindControl("j07Name"), Label).Text)
            Next
            For Each ri As RepeaterItem In rpJ11.Items
                ali.Add(CType(ri.FindControl("j11Name"), Label).Text)
            Next
            For Each ri As RepeaterItem In rpJ02.Items
                ali.Add(CType(ri.FindControl("linkPerson"), HyperLink).Text)
            Next
        End If
        If opgScope.SelectedValue = "3" And Me.j70ID.SelectedIndex > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.Closed = BO.BooleanQueryMode.NoQuery
            mq.j70ID = BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
            'mq.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead
            For Each x In Me.Factory.j02PersonBL.GetList(mq).Select(Function(p) p.PID).ToList
                j02ids_all.Add(x)
            Next
            If j02ids_all.Count = 0 Then j02ids_all.Add(-1) 'umělá položka, by tam něco bylo

            ali.Add("Filtr osob: " & Me.j70ID.SelectedItem.Text)
        End If
        If opgScope.SelectedValue = "4" Then
            ali.Add(Me.Factory.SysUser.Person)
            j02ids_all.Add(Me.Factory.SysUser.j02ID)
        End If
        If ali.Count = 0 Then
            ali.Add("Všechny osoby")
        End If

        Me.hidJ02IDs_All.Value = String.Join(",", j02ids_all)

        hidHeader.Value = String.Join(", ", ali)
    End Sub
    Private Sub se(strMessage As String)
        lblMessage.Text = strMessage
    End Sub
    Private Sub Handle_Change(bolAppend As Boolean, strPrefix As String)
        se("")
        Dim intJ11ID As Integer = 0, intJ07ID As Integer = 0, intJ02ID As Integer = 0
        If strPrefix = "j11" Then intJ11ID = BO.BAS.IsNullInt(Me.j11ID_Add.SelectedValue)
        If strPrefix = "j07" Then intJ07ID = BO.BAS.IsNullInt(Me.j07ID_Add.SelectedValue)
        If strPrefix = "j02" Then intJ02ID = BO.BAS.IsNullInt(Me.j02ID_Add.Value)
        If intJ02ID = 0 And intJ07ID = 0 And intJ11ID = 0 Then
            Return
        End If
        If Not bolAppend Then
            Me.hidJ02IDs.Value = "" : Me.hidJ07IDs.Value = "" : Me.hidJ11IDs.Value = ""
        End If

        If intJ02ID > 0 Then
            Me.hidJ02IDs.Value += "," & intJ02ID.ToString
        End If
        If intJ07ID > 0 Then
            Me.hidJ07IDs.Value += "," & intJ07ID.ToString
        End If
        If intJ11ID <> 0 Then
            Me.hidJ11IDs.Value += "," & intJ11ID.ToString
        End If
        
        Me.hidJ02IDs.Value = BO.BAS.OM1(Me.hidJ02IDs.Value, True)
        Me.hidJ07IDs.Value = BO.BAS.OM1(Me.hidJ07IDs.Value, True)
        Me.hidJ11IDs.Value = BO.BAS.OM1(Me.hidJ11IDs.Value, True)

        

        RenderListAndCalculAllJ02IDs()

    End Sub

    Private Sub rpJ07_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpJ07.ItemCommand
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidJ07IDs.Value).Distinct.ToList
        pids.Remove(e.CommandArgument)
        If pids.Count > 0 Then
            hidJ07IDs.Value = String.Join(",", pids)
        Else
            hidJ07IDs.Value = ""
        End If
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    
    

    Private Sub rpJ07_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpJ07.ItemDataBound
        Dim cRec As BO.j07PersonPosition = CType(e.Item.DataItem, BO.j07PersonPosition)
        CType(e.Item.FindControl("j07Name"), Label).Text = cRec.j07Name
        CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID
    End Sub

    Private Sub rpJ11_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpJ11.ItemCommand
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidJ11IDs.Value).Distinct.ToList
        pids.Remove(e.CommandArgument)
        If pids.Count > 0 Then
            hidJ11IDs.Value = String.Join(",", pids)
        Else
            hidJ11IDs.Value = ""
        End If
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub rpJ11_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpJ11.ItemDataBound
        Dim cRec As BO.j11Team = CType(e.Item.DataItem, BO.j11Team)
        CType(e.Item.FindControl("j11Name"), Label).Text = cRec.j11Name
        CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID
    End Sub

    Private Sub rpJ02_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpJ02.ItemCommand
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidJ02IDs.Value).Distinct.ToList
        pids.Remove(e.CommandArgument)
        If pids.Count > 0 Then
            hidJ02IDs.Value = String.Join(",", pids)
        Else
            hidJ02IDs.Value = ""
        End If
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub rpJ02_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpJ02.ItemDataBound
        Dim cRec As BO.j02Person = CType(e.Item.DataItem, BO.j02Person)
        With CType(e.Item.FindControl("linkPerson"), HyperLink)
            .Text = cRec.FullNameDesc
            .NavigateUrl = "j02_framework.aspx?pid=" & cRec.PID.ToString
        End With
        CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID
    End Sub

    Private Sub j02ID_Add_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles j02ID_Add.AutoPostBack_SelectedIndexChanged
        If Me.j02ID_Add.Value <> "" Then
            Handle_Change(True, "j02")
            Me.j02ID_Add.Text = ""
            Me.j02ID_Add.Value = ""
        End If
        RaiseEvent OnChange()
    End Sub

    Private Sub j07ID_Add_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles j07ID_Add.SelectedIndexChanged
        Handle_Change(True, "j07")
        Me.j07ID_Add.SelectedIndex = 0
        RaiseEvent OnChange()
    End Sub

    Private Sub j11ID_Add_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles j11ID_Add.SelectedIndexChanged
        Handle_Change(True, "j11")
        Me.j11ID_Add.SelectedIndex = 0
        RaiseEvent OnChange()
    End Sub

    

    Private Sub opgScope_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgScope.SelectedIndexChanged
        Me.CurrentScope = CInt(opgScope.SelectedValue)
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub linkClearAll_Click(sender As Object, e As EventArgs) Handles linkClearAll.Click
        Me.hidJ02IDs.Value = "" : Me.hidJ07IDs.Value = "" : Me.hidJ11IDs.Value = ""
        Me.hidJ02IDs_All.Value = ""
        RenderListAndCalculAllJ02IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub cbxPersonsRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPersonsRole.SelectedIndexChanged
        RaiseEvent OnChange()
    End Sub
End Class