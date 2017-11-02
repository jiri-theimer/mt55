Public Class periodcombo
    Inherits System.Web.UI.UserControl
    Public Event OnChanged(DateFrom As Date, DateUntil As Date)
    Public Property Width As String
        Get
            Return Me.per1.Style.Item("width")
        End Get
        Set(value As String)
            Me.per1.Style.Item("width") = value
        End Set
    End Property
    Public Property x21ID As BO.x21IdEnum
        Get
            If Me.per1.SelectedValue = "" Then Return BO.x21IdEnum._NoQuery

            Dim a() As String = Me.per1.SelectedValue.Split("-")

            Return CType(CInt(a(0)), BO.x21IdEnum)
        End Get
        Set(value As BO.x21IdEnum)
            basUI.SelectDropdownlistValue(Me.per1, CInt(value).ToString)
        End Set
    End Property
    Public ReadOnly Property CurrentX21 As BO.x21DatePeriod
        Get
            If Me.x21ID < BO.x21IdEnum._CutomQuery Then
                Return New BO.x21DatePeriod(Me.x21ID, False)
            End If

            Dim a() As String = Me.hidCustomQueries.Value.Split("|"), x As Integer = 0
            Dim strPair As String = a(Me.CustomQueryIndex - 1)
            a = strPair.Split(";")
            
            Return New BO.x21DatePeriod(x, BO.BAS.ConvertString2Date(a(0)), BO.BAS.ConvertString2Date(a(1)), a(2), False)

        End Get
    End Property
    Public ReadOnly Property CustomQueryIndex As Integer
        Get
            Dim a() As String = Me.per1.SelectedValue.Split("-")

            Return CInt(a(1))
        End Get
    End Property
    Public Property SelectedValue As String
        Get
            Return per1.SelectedValue
        End Get
        Set(value As String)
            basUI.SelectDropdownlistValue(Me.per1, value)
            Me.clue_period.Attributes.Item("rel") = "clue_periodcombo.aspx?value=" & value & "&d1=" & Format(Me.DateFrom, "dd.MM.yyyy") & "&d2=" & Format(Me.DateUntil, "dd.MM.yyyy")
        End Set
    End Property
    Public ReadOnly Property DateFrom As Date
        Get
            Select Case Me.x21ID
                Case BO.x21IdEnum._CutomQuery
                    Return Me.CurrentX21.DateFrom
                Case BO.x21IdEnum._NoQuery
                    Return DateSerial(1900, 1, 1)
                Case Else
                    Dim c As New BO.x21DatePeriod(Me.x21ID, False)
                    Return c.DateFrom
            End Select
           
        End Get
    End Property
    Public ReadOnly Property DateUntil As Date
        Get
            Select Case Me.x21ID
                Case BO.x21IdEnum._CutomQuery
                    Return Me.CurrentX21.DateUntil
                Case BO.x21IdEnum._NoQuery
                    Return DateSerial(3000, 1, 1)
                Case Else
                    Dim c As New BO.x21DatePeriod(Me.x21ID, False)
                    Return c.DateUntil
            End Select
        End Get
    End Property
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
       
    End Sub

    Private Sub per1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles per1.SelectedIndexChanged
        RaiseEvent OnChanged(Me.DateFrom, Me.DateUntil)

    End Sub

    Public ReadOnly Property RowsCount As Integer
        Get
            Return Me.per1.Items.Count
        End Get
    End Property
    Public Sub SetupData(factory As BL.Factory, strCustomQueries As String, Optional bolIncludeFuture As Boolean = False, Optional bolMobile As Boolean = False)
        Me.hidLogin.Value = factory.SysUser.j03Login
        Dim bolEnglish As Boolean = False
        If Page.Culture.IndexOf("Czech") < 0 And Page.Culture.IndexOf("Če") < 0 Then bolEnglish = True

        Dim lis As List(Of BO.x21DatePeriod) = factory.ftBL.GetList_X21_NonDB(bolIncludeFuture, bolEnglish)
        Me.hidCustomQueries.Value = strCustomQueries

        If strCustomQueries <> "" Then

            Dim a() As String = strCustomQueries.Split("|"), x As Integer = 0
            For Each strPair As String In a
                Dim b() As String = strPair.Split(";")
                x += 1
                Dim cX21 As New BO.x21DatePeriod(x, BO.BAS.ConvertString2Date(b(0)), BO.BAS.ConvertString2Date(b(1)), b(2), bolEnglish)
                If Left(cX21.x21Name, 2) = "**" Then
                    lis.Insert(1, cX21)
                Else
                    lis.Add(cX21)
                End If

                
            Next
        End If

        Me.per1.DataSource = lis
        Me.per1.DataBind()

        If bolMobile Then
            Me.clue_period.Visible = False
        End If
    End Sub

    Public Property BackColor As System.Drawing.Color
        Get
            Return Me.per1.BackColor
        End Get
        Set(value As System.Drawing.Color)
            Me.per1.BackColor = value
        End Set
    End Property

    Private Sub cmdPeriodComboRefresh_Click(sender As Object, e As EventArgs) Handles cmdPeriodComboRefresh.Click
        If Me.hidExplicitValue.Value = "" Then Return
        'Me.period1.SelectedValue = Me.hidExplicitValue.Value
        If Me.hidExplicitValue.Value = "-1" Or Me.hidExplicitValue.Value = "-2" Then
            'nutnost kompletně naplnit combo
            Dim factory As New BL.Factory(, Me.hidLogin.Value)
            SetupData(factory, factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
            If Me.hidExplicitValue.Value = "-2" Then
                Me.per1.SelectedIndex = 1
            Else
                Me.per1.SelectedIndex = Me.per1.Items.Count - 1
            End If

            RaiseEvent OnChanged(Me.DateFrom, Me.DateUntil)
            Me.hidExplicitValue.Value = ""
            Return
        End If
        Me.SelectedValue = Me.hidExplicitValue.Value

        RaiseEvent OnChanged(Me.DateFrom, Me.DateUntil)
        Me.hidExplicitValue.Value = ""
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        With per1
            If .SelectedIndex > 0 Then
                .ToolTip = .SelectedItem.Text
            Else
                .ToolTip = Resources.common.FiltrObdobi
            End If
        End With
    End Sub
End Class