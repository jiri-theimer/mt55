Imports Telerik.Web.UI

Public Class periodcombo_setting
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub periodcombo_setting_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID
            With Master
                .HeaderText = "Pojmenovaná časová období"
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("Uložit změny", "ok", , "Images/save.png")
                .Factory.j03UserBL.InhaleUserParams("periodcombo-custom_query")

            End With

            Me.SetupData(Master.Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
            RefreshTempList()



        End If
    End Sub

    Private Sub SetupData(s As String)

        If s = "" Then
            Return
        End If
        Dim a() As String = s.Split("|"), x As Integer = 0, bolEnglish As Boolean = False
        If Page.Culture.IndexOf("Czech") < 0 Then bolEnglish = True
        For Each strPair As String In a
            x += 1
            Dim b() As String = strPair.Split(";")
            Dim c As New BO.x21DatePeriod(x, BO.BAS.ConvertString2Date(b(0)), BO.BAS.ConvertString2Date(b(1)), b(2), bolEnglish)
            Dim cRec As New BO.p85TempBox

            cRec.p85GUID = ViewState("guid")
            cRec.p85FreeText01 = c.x21Name
            cRec.p85FreeDate01 = c.DateFrom
            cRec.p85FreeDate02 = c.DateUntil
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
        
    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        SaveTempdata()

        If e.CommandName = "delete" Then
            Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(CInt(e.CommandArgument))
            With Master.Factory.p85TempBoxBL
                .Delete(cRec)
            End With


            RefreshTempList()
        End If
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        With CType(e.Item.FindControl("datFrom"), RadDatePicker)
            .SharedCalendar = Me.SharedCalendar
            .SelectedDate = cRec.p85FreeDate01
        End With
        With CType(e.Item.FindControl("datUntil"), RadDatePicker)
            .SharedCalendar = Me.SharedCalendar
            .SelectedDate = cRec.p85FreeDate02
        End With
        CType(e.Item.FindControl("txtName"), TextBox).Text = cRec.p85FreeText01
        CType(e.Item.FindControl("del"), ImageButton).CommandArgument = cRec.PID.ToString

        If Left(cRec.p85FreeText01, 2) = "**" Then
            e.Item.FindControl("trRow").Visible = False
        End If
    End Sub

    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        SaveTempdata()

        Dim c As New BO.p85TempBox
        c.p85FreeDate01 = DateSerial(Year(Now), Month(Now), 1)
        c.p85FreeDate02 = DateSerial(Year(Now), Month(Now), 1).AddMonths(1).AddDays(-1)
        c.p85FreeText01 = "Pojmenované období"
        c.p85GUID = ViewState("guid")
        Master.Factory.p85TempBoxBL.Save(c)
        RefreshTempList()
    End Sub

    Private Sub RefreshTempList()
        rp1.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rp1.DataBind()
        If rp1.Items.Count > 0 Then
            Me.SharedCalendar.Visible = True
        Else
            Me.SharedCalendar.Visible = False
        End If
    End Sub

    Private Sub SaveTempdata()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("del"), ImageButton).CommandArgument)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                With CType(ri.FindControl("datFrom"), RadDatePicker)
                    If Not .IsEmpty Then cRec.p85FreeDate01 = .SelectedDate
                End With
                With CType(ri.FindControl("datUntil"), RadDatePicker)
                    If Not .IsEmpty Then cRec.p85FreeDate02 = .SelectedDate
                End With


                .p85FreeText01 = CType(ri.FindControl("txtName"), TextBox).Text


            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            SaveTempdata()
            Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            Dim s As String = "", x As Integer = 0
            For Each c In lisTEMP
                x += 1
                If BO.BAS.IsNullDBDate(c.p85FreeDate01) Is Nothing Or BO.BAS.IsNullDBDate(c.p85FreeDate02) Is Nothing Then
                    Master.Notify("V řádku #" & x.ToString & " je nevyplněné datum.", NotifyLevel.ErrorMessage)
                    Return
                End If
                If c.p85FreeDate01 > c.p85FreeDate02 Then
                    Master.Notify("V řádku #" & x.ToString & " je datum začátku větší než datum konce.")
                    Return
                End If
                s += "|" & Format(c.p85FreeDate01, "dd.MM.yyyy") & ";" & Format(c.p85FreeDate02, "dd.MM.yyyy") & ";" & c.p85FreeText01
            Next
            s = BO.BAS.OM1(s)
            If Len(s) > 500 Then
                Master.Notify("Počet období přesáhl maximální kapacitu, musíte snížit jejich počet.", NotifyLevel.WarningMessage)
                Return
            End If
            If Master.Factory.j03UserBL.SetUserParam("periodcombo-custom_query", s) Then
                Master.CloseAndRefreshParent("periodcombo")
            End If
        End If
    End Sub
End Class