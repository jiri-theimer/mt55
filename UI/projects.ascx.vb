Public Class projects
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Public Event OnChange()


    Public ReadOnly Property CurrentHeader As String
        Get
            Return hidHeader.Value
        End Get
    End Property
    Public Property CurrentScope As Integer
        Get
            Return CInt(Me.opgScope.SelectedValue)
        End Get
        Set(value As Integer)
            Select Case value
                Case 2
                    
                Case 3
                    If Me.j70ID.Items.Count = 0 Then SetupJ70Combo(value)
            End Select
            basUI.SelectRadiolistValue(Me.opgScope, value.ToString)
        End Set
    End Property
    Public Function CurrentP41IDs() As List(Of Integer)
        If hidP41IDs_All.Value = "" Then
            Return New List(Of Integer)
        Else
            Return BO.BAS.ConvertPIDs2List(Me.hidP41IDs_All.Value).Distinct.ToList
        End If
    End Function
    Public Property CurrentValue As String
        Get
            Select Case opgScope.SelectedValue
                Case "1"
                    Return ""
                Case "2"
                    Return Me.hidP41IDs.Value & "|" & hidP28IDs.Value
                Case "3"
                    Return Me.j70ID.SelectedValue
                Case Else
                    Return ""
            End Select
        End Get
        Set(value As String)
            Select Case opgScope.SelectedValue
                Case "2"
                    Dim a() As String = Split(value, "|")
                    hidP41IDs.Value = a(0)
                    If UBound(a) > 0 Then hidP28IDs.Value = a(1)

                Case "3"
                    If Me.j70ID.Items.Count = 0 Then SetupJ70Combo(BO.BAS.IsNullInt(value))
                    basUI.SelectDropdownlistValue(Me.j70ID, value)
            End Select
            RenderListAndCalculAllP41IDs()
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Private Sub SetupData()
     
        SetupJ70Combo(0)

    End Sub
    Private Sub SetupJ70Combo(intDef As Integer)
        Dim mq As New BO.myQuery
        j70ID.DataSource = Me.Factory.j70QueryTemplateBL.GetList(mq, BO.x29IdEnum.p41Project)
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
        If Me.hidP41IDs_All.Value <> "" Then
            linkClearAll.Visible = True
        Else
            linkClearAll.Visible = False
        End If
    End Sub


    Private Sub RenderListAndCalculAllP41IDs()
        Dim p41ids_all As New List(Of Integer), ali As New List(Of String)
        If opgScope.SelectedValue = "2" Then
            If Me.hidP28IDs.Value <> "" Then
                Dim mq As New BO.myQueryP28
                mq.PIDs = BO.BAS.ConvertPIDs2List(Me.hidP28IDs.Value)
                rpP28.DataSource = Me.Factory.p28ContactBL.GetList(mq)

                Dim mqP41 As New BO.myQueryP41
                mqP41.p28IDs = mq.PIDs
                For Each x In Me.Factory.p41ProjectBL.GetList(mqP41).Select(Function(p) p.PID).ToList
                    p41ids_all.Add(x)
                Next
            Else
                rpP28.DataSource = Nothing
            End If
            rpP28.DataBind()

            If Me.hidP41IDs.Value <> "" Then
                Dim mq As New BO.myQueryP41
                mq.PIDs = BO.BAS.ConvertPIDs2List(Me.hidP41IDs.Value)
                rpP41.DataSource = Me.Factory.p41ProjectBL.GetList(mq)

                For Each x In BO.BAS.ConvertPIDs2List(Me.hidP41IDs.Value)
                    p41ids_all.Add(x)
                Next
            Else
                rpP41.DataSource = Nothing
            End If
            rpP41.DataBind()

            If p41ids_all.Count = 0 And (hidP28IDs.Value <> "" Or hidP41IDs.Value <> "") Then
                se("Vstupní podmínce neodpovídá ani jeden projekt.")
            End If

            For Each ri As RepeaterItem In rpP28.Items
                ali.Add(CType(ri.FindControl("linkClient"), HyperLink).Text)
            Next
            For Each ri As RepeaterItem In rpP41.Items
                ali.Add(CType(ri.FindControl("linkProject"), HyperLink).Text)
            Next
        End If
        If opgScope.SelectedValue = "3" And Me.j70ID.SelectedIndex > 0 Then
            Dim mq As New BO.myQueryP41
            mq.Closed = BO.BooleanQueryMode.NoQuery
            mq.j70ID = BO.BAS.IsNullInt(Me.j70ID.SelectedValue)
            'mq.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead
            For Each x In Me.Factory.p41ProjectBL.GetList(mq).Select(Function(p) p.PID).ToList
                p41ids_all.Add(x)
            Next
            If p41ids_all.Count = 0 Then p41ids_all.Add(-1) 'umělá položka, by tam něco bylo

            ali.Add("Filtr projektů: " & Me.j70ID.SelectedItem.Text)
        End If
        If ali.Count = 0 Then
            ali.Add("Všechny projekty")
        End If

        Me.hidP41IDs_All.Value = String.Join(",", p41ids_all)

        hidHeader.Value = String.Join(", ", ali)
    End Sub
    Private Sub se(strMessage As String)
        lblMessage.Text = strMessage
    End Sub
    Private Sub Handle_Change(bolAppend As Boolean, strPrefix As String)
        se("")
        Dim intP28ID As Integer = 0, intP41ID As Integer = 0
        If strPrefix = "p28" Then intP28ID = BO.BAS.IsNullInt(Me.p28ID_Add.Value)
        If strPrefix = "p41" Then intP41ID = BO.BAS.IsNullInt(Me.p41ID_Add.Value)
        If intP28ID = 0 And intP41ID = 0 Then
            Return
        End If
        If Not bolAppend Then
            Me.hidP41IDs.Value = "" : Me.hidP28IDs.Value = ""
        End If

        If intP41ID > 0 Then
            Me.hidP41IDs.Value += "," & intP41ID.ToString
        End If
        If intP28ID > 0 Then
            Me.hidP28IDs.Value += "," & intP28ID.ToString
        End If
       
        Me.hidP41IDs.Value = BO.BAS.OM1(Me.hidP41IDs.Value, True)
        Me.hidP28IDs.Value = BO.BAS.OM1(Me.hidP28IDs.Value, True)
        


        RenderListAndCalculAllP41IDs()

    End Sub

    Private Sub rpP28_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP28.ItemCommand
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidP28IDs.Value).Distinct.ToList
        pids.Remove(e.CommandArgument)
        If pids.Count > 0 Then
            hidP28IDs.Value = String.Join(",", pids)
        Else
            hidP28IDs.Value = ""
        End If
        RenderListAndCalculAllP41IDs()
        RaiseEvent OnChange()
    End Sub




    Private Sub rpP28_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP28.ItemDataBound
        Dim cRec As BO.p28Contact = CType(e.Item.DataItem, BO.p28Contact)
        With CType(e.Item.FindControl("linkClient"), HyperLink)
            .Text = cRec.p28Name
            .NavigateUrl = "p28_framework.aspx?pid=" & cRec.PID.ToString
        End With
        CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID
    End Sub

    

    Private Sub rpP41_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP41.ItemCommand
        Dim pids As List(Of Integer) = BO.BAS.ConvertPIDs2List(Me.hidP41IDs.Value).Distinct.ToList
        pids.Remove(e.CommandArgument)
        If pids.Count > 0 Then
            hidP41IDs.Value = String.Join(",", pids)
        Else
            hidP41IDs.Value = ""
        End If
        RenderListAndCalculAllP41IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub rpP41_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP41.ItemDataBound
        Dim cRec As BO.p41Project = CType(e.Item.DataItem, BO.p41Project)
        With CType(e.Item.FindControl("linkProject"), HyperLink)
            .Text = cRec.FullName
            .NavigateUrl = "p41_framework.aspx?pid=" & cRec.PID.ToString
        End With
        CType(e.Item.FindControl("cmdDelete"), ImageButton).CommandArgument = cRec.PID
    End Sub

    Private Sub p41ID_Add_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41ID_Add.AutoPostBack_SelectedIndexChanged
        If Me.p41ID_Add.Value <> "" Then
            Handle_Change(True, "p41")
            Me.p41ID_Add.Text = ""
            Me.p41ID_Add.Value = ""
        End If
        RaiseEvent OnChange()
    End Sub

    



    Private Sub opgScope_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgScope.SelectedIndexChanged
        Me.CurrentScope = CInt(opgScope.SelectedValue)
        RenderListAndCalculAllP41IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub j70ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j70ID.SelectedIndexChanged
        RenderListAndCalculAllP41IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub linkClearAll_Click(sender As Object, e As EventArgs) Handles linkClearAll.Click
        Me.hidP41IDs.Value = "" : Me.hidP28IDs.Value = ""
        Me.hidP41IDs_All.Value = ""
        RenderListAndCalculAllP41IDs()
        RaiseEvent OnChange()
    End Sub

    Private Sub p28ID_Add_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p28ID_Add.AutoPostBack_SelectedIndexChanged
        If Me.p28ID_Add.Value <> "" Then
            Handle_Change(True, "p28")
            Me.p28ID_Add.Text = ""
            Me.p28ID_Add.Value = ""
        End If
        RaiseEvent OnChange()
    End Sub
End Class