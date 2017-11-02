Public Class period
    Inherits System.Web.UI.UserControl
    Public Event x21ID_OnChange(x21ID As BO.x21IdEnum)


    
    Public Property Caption As String
        Get
            Return lblCaption.Text
        End Get
        Set(value As String)
            lblCaption.Text = value
        End Set
    End Property
    Public Property DateFrom As Date
        Get
            If datFrom.IsEmpty Or cbxX21ID.SelectedValue = "0" Then Return DateSerial(1900, 1, 1)
            Return datFrom.SelectedDate
        End Get
        Set(value As Date)
            If Year(value) > 1900 Then
                datFrom.SelectedDate = value
            End If

        End Set
    End Property
    Public Property DateUntil As Date
        Get
            If datUntil.IsEmpty Or cbxX21ID.SelectedValue = "0" Then Return DateSerial(3000, 1, 1)
            Return datUntil.SelectedDate
        End Get
        Set(value As Date)
            If Year(value) < 3000 Then
                datUntil.SelectedDate = value
            End If

        End Set
    End Property
    Public Property x21ID As BO.x21IdEnum
        Get
            Dim x As Integer = BO.BAS.IsNullInt(cbxX21ID.SelectedValue)
            Return CType(x, BO.x21IdEnum)
        End Get
        Set(value As BO.x21IdEnum)
            Dim x As Integer = value
            Try
                cbxX21ID.SelectedValue = x.ToString
                Handle_ChangePeriod()
            Catch ex As Exception

            End Try
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RefreshState()
    End Sub

    Public Sub FillData(lisX21 As List(Of BO.x21DatePeriod), strNoFilterCaption As String, strCustomPeriodCaption As String)

        With cbxX21ID
            .DataSource = lisX21
            .DataBind()
            If Page.Culture.IndexOf("Czech") < 0 And Page.Culture.IndexOf("Če") < 0 Then
                If strNoFilterCaption <> "" Then .Items.Insert(0, New ListItem("--No period filter--", "0"))
                If strCustomPeriodCaption <> "" Then .Items.Insert(1, New ListItem("--Custom period--", "-1"))
            Else
                If strNoFilterCaption <> "" Then .Items.Insert(0, New ListItem("--Nefiltrovat období--", "0"))
                If strCustomPeriodCaption <> "" Then .Items.Insert(1, New ListItem("--Vlastní období--", "-1"))
            End If
            
        End With
    End Sub

    Private Sub RefreshState()
        Dim bolEnabled As Boolean = True, bolVisible As Boolean = True, x As Integer = 0
        Select Case BO.BAS.IsNullInt(cbxX21ID.SelectedValue)
            Case 0  'nefiltrovat
                bolVisible = False : x += 1
            Case -1 'vlastní období
            Case Else
                bolEnabled = False
        End Select

        datFrom.Enabled = bolEnabled : datUntil.Enabled = bolEnabled
        datFrom.Visible = bolVisible : datUntil.Visible = bolVisible
        lblFrom.Visible = bolVisible : lblUntil.Visible = bolVisible

    End Sub

    Private Sub Handle_ChangePeriod()
        If BO.BAS.IsNullInt(cbxX21ID.SelectedValue) > 0 Then
            Dim cX21 As New BO.x21DatePeriod(CInt(cbxX21ID.SelectedValue), False)

            datFrom.SelectedDate = IIf(Year(cX21.DateFrom) < 2000, DateSerial(2000, 1, 1), cX21.DateFrom)
            datUntil.SelectedDate = IIf(Year(cX21.DateUntil) > 2050, DateSerial(2050, 1, 1), cX21.DateUntil)
        End If
        RefreshState()
    End Sub

    Private Sub cbxX21ID_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbxX21ID.SelectedIndexChanged
        Handle_ChangePeriod()
        Dim c As BO.x21IdEnum = BO.BAS.IsNullInt(cbxX21ID.SelectedValue)
        RaiseEvent x21ID_OnChange(c)

    End Sub
End Class