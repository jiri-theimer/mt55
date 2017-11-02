Public Class record_validity
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private ReadOnly Property D1 As Date
        Get
            Return Me.datFrom.SelectedDate
        End Get
    End Property
    Private ReadOnly Property D2 As Date
        Get
            Return Me.datUntil.SelectedDate
        End Get
    End Property
    Private ReadOnly Property IsClosed As Boolean
        Get
            If Me.D1 <= Now And Me.D2 >= Now Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Private Sub record_validity_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                If Request.Item("current_date_from") = "" Then
                    .StopPage("current_date_from is missing.")
                Else
                    Me.datFrom.SelectedDate = DateTime.ParseExact(Request.Item("current_date_from"), "dd.MM.yyyy HH:mm:ss", Nothing)
                End If
                If Request.Item("current_date_until") = "" Then
                    .StopPage("current_date_until is missing.")
                Else
                    Me.datUntil.SelectedDate = DateTime.ParseExact(Request.Item("current_date_until"), "dd.MM.yyyy HH:mm:ss", Nothing)
                End If
                Me.curDatFrom.Text = BO.BAS.FD(Me.datFrom.SelectedDate, True)
                Me.curDatUntil.Text = BO.BAS.FD(Me.datUntil.SelectedDate, True)
                .neededPermission = BO.x53PermValEnum.GR_Admin
                If Me.IsClosed Then
                    .HeaderText = "Záznam je v archivu | Časová platnost záznamu je ukončena"
                    .HeaderIcon = "Images/bin_32.png"
                    .AddToolbarButton("Obnovit z archivu", "ok", , "Images/ok.png")
                    ViewState("isclosed") = "1"
                Else
                    .HeaderText = "Záznam je otevřený"
                    .HeaderIcon = "Images/recycle_32.png"
                    .AddToolbarButton("Přesunout do archivu", "ok", , "Images/ok.png")
                    ViewState("isclosed") = "0"
                End If
                If Year(datUntil.SelectedDate) = 3000 Then
                    Me.curDatUntil.Visible = False
                    imgInfinity.Visible = True
                Else
                    imgInfinity.Visible = False
                End If
                .AddToolbarButton("OK", "manual", , "Images/ok.png")

            End With

        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "manual" Then
            
        End If
        If strButtonValue = "ok" Then
            If Me.datUntil.IsEmpty Then
                Me.datUntil.SelectedDate = DateSerial(3000, 1, 1)
            End If
            If Me.datFrom.IsEmpty Then
                Me.datFrom.SelectedDate = Now
            End If
            If ViewState("isclosed") = "1" Then
                'obnovit z archivu
                Me.datUntil.SelectedDate = DateSerial(3000, 1, 1)
                If Me.datFrom.SelectedDate > Now Then
                    Me.datFrom.SelectedDate = Now
                End If
            End If
            If ViewState("isclosed") = "0" Then
                'přesunout do archivu
                Me.datUntil.SelectedDate = Now
            End If

        End If

        If Me.datFrom.SelectedDate >= Me.datUntil.SelectedDate Then
            Master.Notify("[Datum od] musí být menší než [Datum do].", NotifyLevel.WarningMessage)
            Return
        End If
        Master.CloseAndRefreshParent(Format(Me.datFrom.SelectedDate, "dd.MM.yyyy HH:mm:ss") & "|" & Format(Me.datUntil.SelectedDate, "dd.MM.yyyy HH:mm:ss"), , "hardrefresh_validity")

    End Sub

    Private Sub record_validity_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.panExplicitDates.Visible = chkExplicitDates.Checked
        Master.HideShowToolbarButton("ok", Not chkExplicitDates.Checked)
        Master.HideShowToolbarButton("manual", chkExplicitDates.Checked)

    End Sub
End Class