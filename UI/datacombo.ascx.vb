Imports Telerik.Web.UI
Public Class datacombo
    Inherits System.Web.UI.UserControl

    Public Event SelectedIndexChanged(ByVal OldValue As String, ByVal OldText As String, ByVal CurValue As String, ByVal CurText As String)
    Public Event ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemEventArgs)
    Public Event NeedMissingItem(strFoundedMissingItemValue As String, ByRef strAddMissingItemText As String)

    Public ReadOnly Property RadComboClientID As String
        Get
            Return cbx1.ClientID
        End Get
    End Property
    Public Property OnClientTextChange() As String
        Get
            Return Me.cbx1.OnClientTextChange
        End Get
        Set(ByVal value As String)
            Me.cbx1.OnClientTextChange = value

        End Set
    End Property
    Public Property OnClientSelectedIndexChanged() As String
        Get
            Return Me.cbx1.OnClientSelectedIndexChanged
        End Get
        Set(ByVal value As String)
            Me.cbx1.OnClientSelectedIndexChanged = value
        End Set
    End Property
    Public Property OnClientBlur As String
        Get
            Return cbx1.OnClientBlur
        End Get
        Set(value As String)
            cbx1.OnClientBlur = value
        End Set
    End Property
    Public Property AllowCustomText() As Boolean
        Get
            Return cbx1.AllowCustomText
        End Get
        Set(ByVal value As Boolean)
            cbx1.AllowCustomText = value

        End Set
    End Property
    Public Property IsFirstEmptyRow() As Boolean
        Get
            If hidIsEmptyFirstRow.Value = "1" Then Return True Else Return False
        End Get
        Set(ByVal value As Boolean)
            If value Then hidIsEmptyFirstRow.Value = "1" Else hidIsEmptyFirstRow.Value = "0"

        End Set
    End Property
    Public Property BackgroundColor As String
        Get
            Return cbx1.BackColor.ToString
        End Get
        Set(ByVal value As String)
            cbx1.BackColor = System.Drawing.Color.FromName(value)
        End Set
    End Property

    Public Overridable Property Enabled() As Boolean
        Get
            Return cbx1.Enabled
        End Get
        Set(ByVal value As Boolean)
            cbx1.Enabled = value
        End Set
    End Property
    Public Overridable Property Width() As String
        Get
            Return Me.cbx1.Width.ToString
        End Get
        Set(ByVal value As String)

            Me.cbx1.Width = Unit.Parse(value)

        End Set
    End Property

    Public Property SelectedValue() As String
        Get
            Return Me.cbx1.SelectedValue
        End Get
        Set(ByVal value As String)
            Dim strMissingItemText As String = ""
            Try
                Me.cbx1.SelectedValue = value
                If Me.cbx1.FindItemByValue(value) Is Nothing And value <> "" And value <> "0" Then
                    RaiseEvent NeedMissingItem(value, strMissingItemText)
                End If
            Catch ex As Exception
                If value <> "" And value <> "0" Then RaiseEvent NeedMissingItem(value, strMissingItemText)
            End Try
            If strMissingItemText <> "" And value <> "" And value <> "0" Then
                AddOneComboItem(value, strMissingItemText)

                Me.cbx1.FindItemByValue(value).Selected = True
                Me.cbx1.FindItemByValue(value).Font.Strikeout = True
            End If
        End Set
    End Property
    Public Function GetAllCheckedValues() As List(Of String)
        Dim lis As New list(Of String)
        For Each item As RadComboBoxItem In cbx1.CheckedItems
            lis.Add(item.Value)
        Next
        Return lis
    End Function
    Public Function GetAllCheckedIntegerValues() As List(Of Integer)
        Dim lis As New List(Of Integer)
        For Each item As RadComboBoxItem In cbx1.CheckedItems
            lis.Add(CInt(item.Value))
        Next
        Return lis
    End Function
    Public Sub SelectCheckboxItems(values As List(Of String))
        If values Is Nothing Then Return
        For Each strVal As String In values
            If Not cbx1.FindItemByValue(strVal) Is Nothing Then
                cbx1.FindItemByValue(strVal).Checked = True
            End If
        Next
    End Sub
    Public Property SelectedIndex() As Integer
        Get
            Return Me.cbx1.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            Me.cbx1.SelectedIndex = value
        End Set
    End Property
    Public Sub ChangeItemText(strFindValue As String, strNewText As String)
        If Not cbx1.FindItemByValue(strFindValue) Is Nothing Then
            cbx1.FindItemByValue(strFindValue).Text = strNewText
        End If
    End Sub


    Public ReadOnly Property SelectedValue_Attribute(ByVal strAttributeKey As String) As String
        Get
            If cbx1.SelectedItem Is Nothing Then Return ""
            Return Me.cbx1.SelectedItem.Attributes(strAttributeKey)
        End Get
    End Property
    Public Sub SetText(ByVal strText As String)
        Try
            cbx1.Text = strText
        Catch ex As Exception

        End Try
    End Sub
    Public ReadOnly Property Text As String
        Get
            Try
                If Not cbx1.AllowCustomText Then
                    Return cbx1.SelectedItem.Text
                Else
                    Return cbx1.Text
                End If

            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property
    Public ReadOnly Property Rows As Integer
        Get
            Return cbx1.Items.Count
        End Get
    End Property




    Public Property AutoPostBack() As Boolean
        Get
            Return Me.cbx1.AutoPostBack
        End Get
        Set(ByVal value As Boolean)
            cbx1.AutoPostBack = value
        End Set
    End Property

    Public Enum FilterMode As Integer
        None = 0
        Contains = 1
        StartsWith = 2
    End Enum

    Public Property Filter() As FilterMode
        Get
            Select Case cbx1.Filter
                Case RadComboBoxFilter.Contains
                    Return 1
                Case RadComboBoxFilter.None
                    Return 0
                Case RadComboBoxFilter.StartsWith
                    Return 2
                Case Else
                    Return 0
            End Select
        End Get
        Set(ByVal value As FilterMode)
            Select Case value
                Case FilterMode.Contains
                    cbx1.Filter = RadComboBoxFilter.Contains
                    cbx1.MarkFirstMatch = True
                Case FilterMode.StartsWith
                    cbx1.Filter = RadComboBoxFilter.StartsWith
                    cbx1.MarkFirstMatch = True
                Case Else
                    cbx1.Filter = RadComboBoxFilter.None
                    cbx1.MarkFirstMatch = False
            End Select

        End Set
    End Property

    Public Property DefaultValues() As String
        Get
            Dim s As String = ""
            For Each item As RadComboBoxItem In cbx1.Items
                If s = "" Then
                    s = item.Text
                Else
                    s += ";" & item.Text
                End If
            Next
            Return s
        End Get
        Set(value As String)
            Dim a() As String = Split(value, ";")
            For Each s As String In a
                cbx1.Items.Add(New RadComboBoxItem(s))
            Next
        End Set
    End Property
    Public Property ShowToggleImage As Boolean
        Get
            Return cbx1.ShowToggleImage
        End Get
        Set(value As Boolean)
            cbx1.ShowToggleImage = value

        End Set
    End Property




    Public Sub FillData(ByVal dataSource As Object, Optional ByVal bolFirstEmptyRow As Boolean = False, Optional ByVal DefaultValue As Object = Nothing, Optional ByVal strFirstEmptyRowText As String = "")
        cbx1.Items.Clear()

        cbx1.DataSource = dataSource
        cbx1.DataBind()

        If bolFirstEmptyRow Then
            If strFirstEmptyRowText = "" Then strFirstEmptyRowText = "--------"
            cbx1.Items.Insert(0, New RadComboBoxItem(strFirstEmptyRowText, ""))
        End If
        Me.IsFirstEmptyRow = bolFirstEmptyRow
        Try
            If Not DefaultValue Is Nothing Then cbx1.SelectedValue = DefaultValue.ToString
        Catch ex As Exception

        End Try

    End Sub
    Public Overloads Sub AddOneComboItem(ByVal strValue As String, ByVal strText As String, Optional intIndex As Integer = -1)
        If intIndex < 0 Then
            cbx1.Items.Add(New Telerik.Web.UI.RadComboBoxItem(strText, strValue))
        Else
            cbx1.Items.Insert(intIndex, New Telerik.Web.UI.RadComboBoxItem(strText, strValue))
        End If


    End Sub
    Public Overloads Sub AddOneComboItem(ByVal strValue As String, ByVal strText As String, ByVal strAttributeKey As String, ByVal strAttributeValue As String)
        Dim item As New RadComboBoxItem(strText, strValue)
        item.Attributes.Add(strAttributeKey, strAttributeValue)
        cbx1.Items.Add(item)

    End Sub
    Private Sub AddFirstEmptyRow()
        Dim item As New RadComboBoxItem("", "")
        cbx1.Items.Insert(0, item)
    End Sub
    Public Sub Clear()
        cbx1.Items.Clear()
        cbx1.Text = ""
        cbx1.SelectedValue = ""
    End Sub

    Private Sub cbx1_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles cbx1.ItemDataBound
        RaiseEvent ItemDataBound(sender, e)
    End Sub


    Private Sub cbx1_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles cbx1.SelectedIndexChanged
        RaiseEvent SelectedIndexChanged(e.OldValue, e.OldText, e.Value, e.Text)
    End Sub

    Public Property DataTextField() As String
        Get
            Return Me.cbx1.DataTextField
        End Get
        Set(ByVal value As String)
            Me.cbx1.DataTextField = value
        End Set
    End Property
    Public Property DataValueField() As String
        Get
            Return Me.cbx1.DataValueField
        End Get
        Set(ByVal value As String)
            Me.cbx1.DataValueField = value
        End Set
    End Property
    Public Property DataSource() As Object
        Get
            Return Me.cbx1.DataSource
        End Get
        Set(ByVal value As Object)
            Me.cbx1.DataSource = value
        End Set
    End Property
    Public Overrides Sub DataBind()
        Try
            Me.cbx1.DataBind()
        Catch ex As Exception

        End Try



        If IsFirstEmptyRow Then
            AddFirstEmptyRow()
        End If
    End Sub

    Public Property RadCombo As Telerik.Web.UI.RadComboBox
        Get
            Return Me.cbx1
        End Get
        Set(value As Telerik.Web.UI.RadComboBox)
            Me.cbx1 = value
        End Set
    End Property

    Public Property AllowCheckboxes As Boolean
        Get
            Return cbx1.CheckBoxes
        End Get
        Set(value As Boolean)
            cbx1.CheckBoxes = value
        End Set
    End Property
    Public Property CheckedValues() As List(Of String)
        Get
            Dim lis As New List(Of String)
            For Each c As RadComboBoxItem In cbx1.CheckedItems
                lis.Add(c.Value)
            Next
            Return lis
        End Get
        Set(value As List(Of String))
            For Each s As String In value
                If Not cbx1.Items.FindItemByValue(s) Is Nothing Then
                    cbx1.Items.FindItemByValue(s).Checked = True
                End If
            Next
        End Set
    End Property
End Class