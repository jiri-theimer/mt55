Public Class timer
    Inherits System.Web.UI.UserControl
    Public Property Factory As BL.Factory
    Private Property _curRowIndex As Integer


    Public Class TimerRecord
        Public Property PID As Integer
        Public Property DateInit As Date
        Public Property IsRunning As Boolean
        Public Property IsInit As Boolean
        Public Property CurrentDuration As Integer
        Public Property CurrentDurationSeconds As Integer
        Public Property DateLastDuration As Date
        Public Property DateLastStart As Date
        Public Property p41ID As Integer
        Public Property Project As String
        Public Property p31Text As String


        Public Sub New(cRec As BO.p85TempBox)
            With cRec
                PID = .PID
                DateInit = .p85FreeDate01
                IsInit = .p85FreeBoolean01
                If IsInit Then
                    IsRunning = Not .p85FreeBoolean02
                Else
                    IsRunning = False
                End If

                CurrentDuration = CInt(.p85FreeNumber01)
                CurrentDurationSeconds = CurrentDuration / 100
                If Not .p85FreeDate02 Is Nothing Then DateLastStart = .p85FreeDate02
                If Not .p85FreeDate03 Is Nothing Then DateLastDuration = .p85FreeDate03
                p31Text = .p85Message
                p41ID = .p85OtherKey1
                Project = .p85FreeText01
            End With
            
        End Sub
    End Class

    Public ReadOnly Property RowsCount As Integer
        Get
            Return rp1.Items.Count
        End Get
    End Property
    Public Property IsPanelView As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsPanelView.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsPanelView.Value = BO.BAS.GB(value)
            cbxTimerMode.Visible = Not value
        End Set
    End Property
    Public Property IsIFrame As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsIframe.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsIframe.Value = BO.BAS.GB(value)

        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            SaveWhileAway()
            Dim lisPars As New List(Of String)
            With lisPars
                .Add("timer-mode")
            End With
            With Factory.j03UserBL
                .InhaleUserParams(lisPars)
                basUI.SelectDropdownlistValue(Me.cbxTimerMode, .GetUserParam("timer-mode", "2"))

            End With
            RefreshList()
        End If


    End Sub

    Private Sub RefreshState()
        If rp1.Items.Count = 0 Then
            cmdClear.Visible = False
            panContainer.Visible = False
        Else
            cmdClear.Visible = True
            panContainer.Visible = True
        End If
    End Sub

    Private Sub cmdAddRow_Click(sender As Object, e As EventArgs) Handles cmdAddRow.Click
        SaveCurrentState()
        Dim cTemp As New BO.p85TempBox
        With cTemp
            .p85GUID = GetGUID()
            .p85DataPID = Factory.SysUser.j02ID
            .p85FreeDate05 = Today
        End With
        Factory.p85TempBoxBL.Save(cTemp)
        RefreshList()
    End Sub

    Private Function GetGUID() As String
        Return "timer-" & Factory.SysUser.j02ID.ToString
    End Function

    Public Sub RefreshList()
        _curRowIndex = 0
        Dim lis As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(GetGUID())
        rp1.DataSource = lis
        rp1.DataBind()
    End Sub
    Private Function GetSeconds(d1 As Date, d2 As Date) As Integer
        If d2 <= d1 Then Return 0
        Dim result As TimeSpan = d2 - d1
        Return result.TotalSeconds
    End Function
    Private Function FormatSeconds(intSeconds As Integer) As String
        Dim xx As New TimeSpan(0, 0, intSeconds)
        Return xx.Hours & ":" & xx.Minutes & ":" & xx.Seconds
    End Function
    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        SaveCurrentState()
        Dim cRec As BO.p85TempBox = Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(CType(e.Item.FindControl("p85id"), HiddenField).Value))

        Select Case e.CommandName
            Case "start"
                cRec.p85FreeDate03 = Now
                If cRec.p85FreeNumber01 = 0 Then
                    cRec.p85FreeDate01 = Now        'čas inicializace stopek
                    cRec.p85FreeBoolean01 = True    'inicializace stopek
                End If

                cRec.p85FreeBoolean02 = False
                cRec.p85FreeDate02 = Now            'čas posledního startu

                Factory.p85TempBoxBL.Save(cRec)
                If Me.cbxTimerMode.SelectedValue = "2" Then
                    'zastavit ostatní časovače
                    Dim lis As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(GetGUID()).Where(Function(p) p.p85FreeBoolean01 = True And p.p85FreeBoolean02 = False And p.PID <> cRec.PID)
                    For Each c In lis
                        c.p85FreeBoolean02 = True   'příznak, že jsou stopky zastaveny
                        Factory.p85TempBoxBL.Save(c)
                    Next
                End If
            Case "pause"
                cRec.p85FreeBoolean02 = True        'příznak, že jsou stopky zastaveny

                Factory.p85TempBoxBL.Save(cRec)
            Case "reset"
                cRec.p85FreeBoolean01 = False
                cRec.p85FreeBoolean02 = False
                cRec.p85FreeNumber01 = 0
                cRec.p85FreeDate01 = Nothing
                cRec.p85FreeDate02 = Nothing

                Factory.p85TempBoxBL.Save(cRec)
            Case "delete"
                Factory.p85TempBoxBL.Delete(cRec)
        End Select
       
        RefreshList()
    End Sub

    Private Sub rp1_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemCreated
        With CType(e.Item.FindControl("p41ID"), UI.project)
            If Me.IsPanelView Then
                .Width = "290px"
                .radComboBoxOrig.DropDownWidth = Unit.Parse("285px")
            End If
        End With
        With CType(e.Item.FindControl("panSave"), Panel)
            If Me.IsPanelView Then
                .Style.Item("width") = "342px"
                .Style.Item("background-color") = "#F1F1F1"
                .Style.Item("padding") = "2px"
            End If
        End With
        With CType(e.Item.FindControl("p31Text"), TextBox)
            If Me.IsPanelView Then
                .Style.Item("width") = "335px"
            End If
        End With
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        _curRowIndex += 1
        Dim cTempRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        Dim cRec As New TimerRecord(cTempRec)

        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString

        CType(e.Item.FindControl("RowIndex"), Label).Text = "#" & _curRowIndex.ToString

        If cRec.p41ID <> 0 Then
            With CType(e.Item.FindControl("p41ID"), UI.project)
                .Value = cRec.p41ID.ToString
                .Text = cRec.Project
            End With
        End If
        With CType(e.Item.FindControl("p41ID"), UI.project)
            .AddComboAttribute("pid", cRec.PID.ToString)
            .OnClientSelectedIndexChanged = "ProjectChanged"
        End With


        Dim cT As New BO.clsTime

        CType(e.Item.FindControl("CurrentDuration"), TextBox).Text = cRec.CurrentDuration.ToString

        CType(e.Item.FindControl("isrunning"), TextBox).Text = IIf(cRec.IsRunning, "true", "false")
        If cRec.IsRunning Then
            CType(e.Item.FindControl("DateLastDuration"), TextBox).Text = Format(cRec.DateLastDuration, "dd-MM-yyyy-HH-mm-ss")

        End If
        With CType(e.Item.FindControl("p31Text"), TextBox)
            .Text = cRec.p31Text
            .Attributes("onchange") = "SaveP31Text(" & cRec.PID.ToString & ",this)"
        End With
        With CType(e.Item.FindControl("cmdFinalSave"), ImageButton)
            If cRec.IsRunning Then
                .Visible = False
            Else
                .OnClientClick = "return p31_save(" & cRec.PID.ToString & ")"
            End If

        End With




        With CType(e.Item.FindControl("timer"), Label)
            .Text = cT.GetTimeFromSeconds(cRec.CurrentDurationSeconds, True)
            If cRec.IsRunning Then
                .ForeColor = Drawing.Color.Blue
            Else
                .ForeColor = Nothing
            End If
        End With



        If cRec.IsInit Then
            e.Item.FindControl("cmdStart").Visible = Not cRec.IsRunning
            e.Item.FindControl("cmdPause").Visible = cRec.IsRunning

            e.Item.FindControl("cmdReset").Visible = Not cRec.IsRunning
        Else
            e.Item.FindControl("cmdStart").Visible = True
            e.Item.FindControl("cmdPause").Visible = False
            e.Item.FindControl("cmdReset").Visible = False
        End If




        Dim runID As String = "run_" & e.Item.FindControl("p85id").ClientID


    End Sub

    Private Sub cmdClear_Click(sender As Object, e As EventArgs) Handles cmdClear.Click
        Factory.p85TempBoxBL.Truncate(GetGUID())
        RefreshList()
    End Sub

    
    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        RefreshState()
    End Sub

    Private Sub SaveCurrentState()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(GetGUID())
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cTempRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            Dim cRec As New TimerRecord(cTempRec)

            If cRec.IsRunning Then
                cTempRec.p85FreeNumber01 = BO.BAS.IsNullInt(CType(ri.FindControl("CurrentDuration"), TextBox).Text)
                cTempRec.p85FreeDate03 = Now
            End If
            With CType(ri.FindControl("p41ID"), UI.project)
                cTempRec.p85OtherKey1 = BO.BAS.IsNullInt(.Value)
                cTempRec.p85FreeText01 = .Text
            End With
            cTempRec.p85Message = CType(ri.FindControl("p31Text"), TextBox).Text

            Factory.p85TempBoxBL.Save(cTempRec)
        Next
    End Sub

    Private Sub SaveWhileAway()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Factory.p85TempBoxBL.GetList(GetGUID())
        For Each c In lisTEMP
            Dim cRec As New TimerRecord(c)
            If cRec.IsRunning Then
                Dim intSecordsAway As Integer = GetSeconds(cRec.DateLastDuration, Now)
                
                c.p85FreeNumber01 = c.p85FreeNumber01 + (intSecordsAway * 100)
                c.p85FreeDate03 = Now

                Factory.p85TempBoxBL.Save(c)
            End If
        Next
    End Sub

    
End Class