Public Class periodmonth
    Inherits System.Web.UI.UserControl
    Public Event OnSelectedChanged()

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        With query_year
            If .Items.Count = 0 Then
                For i As Integer = -2 To 1
                    Dim intY As Integer = Year(Now) + i
                    .Items.Add(New ListItem(intY.ToString, intY.ToString))
                Next
            End If
        End With
    End Sub

    Public Property AutoPostback As Boolean
        Get
            Return Me.query_year.AutoPostBack
        End Get
        Set(value As Boolean)
            Me.query_year.AutoPostBack = value
            Me.query_month.AutoPostBack = value
        End Set
    End Property

    Public Property SelectedYear As Integer
        Get
            Return CInt(Me.query_year.SelectedValue)
        End Get
        Set(value As Integer)
            If Me.query_year.Items.FindByValue(value.ToString) Is Nothing Then
                Me.query_year.Items.Add(value.ToString)
            End If
            basUI.SelectDropdownlistValue(Me.query_year, value.ToString)
        End Set
    End Property
    Public Property SelectedMonth As Integer
        Get
            Return CInt(Me.query_month.SelectedValue)
        End Get
        Set(value As Integer)
            basUI.SelectDropdownlistValue(Me.query_month, value.ToString)
        End Set
    End Property
    Public ReadOnly Property SelectedDate As Date
        Get
            Return DateSerial(Me.SelectedYear, Me.SelectedMonth, 1)
        End Get
    End Property
    Public ReadOnly Property SelectedDateFrom As Date
        Get
            Return Me.SelectedDate
        End Get
    End Property
    Public ReadOnly Property SelectedDateUntil As Date
        Get
            Return Me.SelectedDate.AddMonths(1).AddDays(-1)
        End Get
    End Property

    Private Sub query_month_SelectedIndexChanged(sender As Object, e As EventArgs) Handles query_month.SelectedIndexChanged
        RaiseEvent OnSelectedChanged()
    End Sub

    Private Sub query_year_SelectedIndexChanged(sender As Object, e As EventArgs) Handles query_year.SelectedIndexChanged
        RaiseEvent OnSelectedChanged()
    End Sub
End Class