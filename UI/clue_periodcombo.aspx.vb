Public Class clue_periodcombo
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master.Factory.j03UserBL
                .InhaleUserParams("periodcombo-custom_query")

                Dim s As String = .GetUserParam("periodcombo-custom_query"), bolEnglish As Boolean = False
                If basUI.GetCookieValue(Request, "MT50-CultureInfo") = "en-US" Then
                    bolEnglish = True
                End If
                
                If Page.Culture.IndexOf("Czech") < 0 Then bolEnglish = True
                Me.hidCustomQueries.Value = s
                If s <> "" Then
                    Dim lis As New List(Of BO.x21DatePeriod)
                    Dim a() As String = s.Split("|"), x As Integer = 0
                    For Each strPair As String In a
                        x += 1
                        Dim b() As String = strPair.Split(";")
                        Dim cX21 As New BO.x21DatePeriod(x, BO.BAS.ConvertString2Date(b(0)), BO.BAS.ConvertString2Date(b(1)), b(2), bolEnglish)
                        If Left(cX21.x21Name, 2) = "**" Then
                            Me.d1.SelectedDate = cX21.DateFrom
                            Me.d2.SelectedDate = cX21.DateUntil
                        Else
                            lis.Add(cX21)
                        End If

                    Next
                    Me.period1.DataSource = lis
                    Me.period1.DataBind()
                End If
            End With

           
            
        End If
    End Sub

    Private Sub cmdSubmit_Click(sender As Object, e As EventArgs) Handles cmdSubmit.Click
        SavePeriod("")


    End Sub

    Public Property x21ID As BO.x21IdEnum
        Get
            If Me.period1.SelectedValue = "" Then Return BO.x21IdEnum._NoQuery

            Dim a() As String = Me.period1.SelectedValue.Split("-")

            Return CType(CInt(a(0)), BO.x21IdEnum)
        End Get
        Set(value As BO.x21IdEnum)
            basUI.SelectRadiolistValue(Me.period1, CInt(value).ToString)
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
    Public ReadOnly Property CurrentX21 As BO.x21DatePeriod
        Get
            If Me.x21ID < BO.x21IdEnum._CutomQuery Then
                Return New BO.x21DatePeriod(Me.x21ID, False)
            End If

            Dim a() As String = Me.hidCustomQueries.Value.Split("|"), x As Integer = 0, bolEnglish As Boolean = False
            If Page.Culture.IndexOf("Czech") < 0 Then bolEnglish = True
            Dim strPair As String = a(Me.CustomQueryIndex - 1)
            a = strPair.Split(";")
            Return New BO.x21DatePeriod(x, BO.BAS.ConvertString2Date(a(0)), BO.BAS.ConvertString2Date(a(1)), a(2), bolEnglish)

        End Get
    End Property
    Public ReadOnly Property CustomQueryIndex As Integer
        Get
            Dim a() As String = Me.period1.SelectedValue.Split("-")

            Return CInt(a(1))
        End Get
    End Property

    Private Sub period1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles period1.SelectedIndexChanged
        Me.d1.SelectedDate = Me.DateFrom
        Me.d2.SelectedDate = Me.DateUntil
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        If Trim(Me.txtPeriodName.Text) = "" Then
            Master.Notify("Chybí název období.")
            Return
        End If
        SavePeriod(Me.txtPeriodName.Text)
    End Sub

    Private Sub SavePeriod(strName As String)
        If Me.d1.IsEmpty Or Me.d2.IsEmpty Then
            Master.Notify("Datumy musí být vyplněny.", NotifyLevel.WarningMessage) : Return
        End If
        If Me.d1.SelectedDate > Me.d2.SelectedDate Then
            Master.Notify("[Datum od] musí být menší nebo rovno než [Datum do].", NotifyLevel.WarningMessage) : Return
        End If
        If Me.d1.SelectedDate = Me.DateFrom Or Me.d2.SelectedDate = Me.DateUntil Then
            Master.Notify("Období s tímto rozsahem datumů již máte uložené.", NotifyLevel.InfoMessage)
            Return
        End If


        'přidat katalogu vlastních období
        Dim t As String = Format(Me.d1.SelectedDate, "dd.MM.yyyy") & ";" & Format(Me.d2.SelectedDate, "dd.MM.yyyy")

        Master.Factory.j03UserBL.InhaleUserParams("periodcombo-custom_query")
        ''Dim s As String = Master.Factory.j03UserBL.GetUserParam("periodcombo-custom_query")
        Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(Master.Factory.j03UserBL.GetUserParam("periodcombo-custom_query"), "|")
        If strName = "" Then
            Dim x As Integer = lis.FindIndex(Function(p) Right(p, 2) = "**")
            If x >= 0 Then
                lis.RemoveAt(x)
            End If
            lis.Insert(0, t & ";**Období**")
        Else
            lis.Add(t & ";" & Replace(strName, ";", ","))
        End If
        Dim s As String = String.Join("|", lis)
        If Len(s) > 500 Then
            Master.Notify("Z kapacitních důvodů již nelze přidat další pojmenovaná období.")
            Return
        End If

        Master.Factory.j03UserBL.SetUserParam("periodcombo-custom_query", s)
        If strName = "" Then
            ClientScript.RegisterStartupScript(Me.GetType, "refresh", "window.parent.hardrefresh_periodcombo('-2');", True)
        Else
            ClientScript.RegisterStartupScript(Me.GetType, "refresh", "window.parent.hardrefresh_periodcombo('-1');", True)
        End If
    End Sub
End Class