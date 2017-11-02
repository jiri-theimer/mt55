Public Class datepicker
    Inherits System.Web.UI.UserControl
    Public Property IsUseTimepicker As Boolean
        Get
            Return BO.BAS.BG(hidIsUseTime.Value)
        End Get
        Set(value As Boolean)
            hidIsUseTime.Value = BO.BAS.GB(value)
            Me.cmdTime.Visible = value
            Me.ul1.Visible = value
        End Set
    End Property
    Public Property DateFormat As String
        Get
            Return Me.hidFormat.Value
        End Get
        Set(value As String)
            Me.hidFormat.Value = value
        End Set
    End Property
    Public Property NumberOfMonths As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidNumberOfMonths.Value)
        End Get
        Set(value As Integer)
            Me.hidNumberOfMonths.Value = value.ToString
        End Set
    End Property
    Public Property StartHour As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidStartHour.Value)
        End Get
        Set(value As Integer)
            Me.hidStartHour.Value = value.ToString
        End Set
    End Property
    Public Property EndHour As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidEndHour.Value)
        End Get
        Set(value As Integer)
            Me.hidEndHour.Value = value.ToString
        End Set
    End Property
    Public Property IsTimePer30Minutes As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsTimePer30Minutes.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsTimePer30Minutes.Value = BO.BAS.GB(value)
        End Set
    End Property
    Public Property SelectedDate As Date?
        Get
            Me.txtTime.Text = Trim(Me.txtTime.Text) : Me.txtDate.Text = Trim(Me.txtDate.Text)
            If Me.txtDate.Text = "" Then Return Nothing
            Dim s As String = "", strVAL As String = Me.txtDate.Text
            Select Case LCase(Me.DateFormat)
                Case "dd.mm.yy", "dd.mm.yyyy"
                    s = "dd.MM.yyyy"
                Case Else
                    s = Me.DateFormat
            End Select
            If Me.IsUseTimepicker Then
                If Me.txtTime.Text = "" Then Me.txtTime.Text = "00:00"
                If Me.txtTime.Text.IndexOf(":") > 0 Then
                Else
                    Me.txtTime.Text += ":00"
                End If
                strVAL += " " & Left(Me.txtTime.Text, 5)
                s += " HH:mm"
            End If
            Return BO.BAS.ConvertString2Date(strVAL, s)
        End Get
        Set(value As Date?)
            If value Is Nothing Then
                Me.txtDate.Text = ""
                If Me.IsUseTimepicker Then
                    Me.txtTime.Text = "00:00"
                Else
                    Me.txtTime.Text = ""
                End If

            Else
                Select Case LCase(Me.DateFormat)
                    Case "dd.mm.yy", "dd.mm.yyyy"
                        Me.txtDate.Text = Format(value, "dd.MM.yyyy")
                    Case Else
                        Me.txtDate.Text = Format(value, Me.DateFormat)
                End Select
                If Me.IsUseTimepicker And Me.dlTime.Items.Count = 0 Then SetupTimeCombo()
                Me.txtTime.Text = Format(value, "HH:mm")
            End If
            
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsUseTimepicker And dlTime.Items.Count = 0 Then
            SetupTimeCombo()
        End If
        
    End Sub

    Private Sub SetupTimeCombo()
        Dim lis As New List(Of String)
        lis.Add("00:00")
        For i As Integer = Me.StartHour To Me.EndHour
            lis.Add(Right("0" & i.ToString, 2) & ":00")
            If Me.IsTimePer30Minutes Then
                lis.Add(Right("0" & i.ToString, 2) & ":30")
            End If
        Next
        dlTime.DataSource = lis
        dlTime.DataBind()
    End Sub

    
    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Me.IsUseTimepicker Then
            If Me.txtTime.Text = "" Then Me.txtTime.Text = "00:00"
        End If
        Me.txtTime.Visible = Me.IsUseTimepicker
        Me.cmdTime.Visible = Me.IsUseTimepicker
    End Sub

    Private Sub dlTime_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dlTime.ItemDataBound
        Dim s As String = CType(e.Item.DataItem, String)
        With CType(e.Item.FindControl("ti"), HyperLink)
            .Text = s
            If Right(s, 2) = "30" Then .ForeColor = Drawing.Color.DarkOrange
            .NavigateUrl = "javascript: si_" & Me.ClientID & "('" & s & "')"
        End With
    End Sub
End Class