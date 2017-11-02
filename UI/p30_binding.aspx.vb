Public Class p30_binding
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Public Property CurrentPrefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property

    Private Sub p30_binding_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("masterpid"))
                Me.CurrentPrefix = Request.Item("masterprefix")

                Select Case Me.CurrentPrefix
                    Case "p28"
                        .HeaderText = "Kontaktní osoby | " & .Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, .DataPID)

                        lblBoundHeader.Text = .HeaderText
                    Case "p41"
                        .HeaderText = "Kontaktní osoby | " & .Factory.GetRecordCaption(BO.x29IdEnum.p41Project, .DataPID)

                        lblBoundHeader.Text = .HeaderText
                    Case Else
                        panMasterRecord.Visible = True
                        panPersons.Visible = False
                End Select
            End With
            With Me.p27ID
                .DataSource = Master.Factory.p30Contact_PersonBL.GetList_p27()
                .DataBind()
                If .Items.Count > 0 Then
                    .Items.Insert(0, "")
                Else
                    .Visible = False
                End If
            End With

            RefreshList()
        End If
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Handle_Save()
    End Sub

    Private Sub Handle_Save()
        Dim cRec As New BO.p30Contact_Person
        cRec.p27ID = BO.BAS.IsNullInt(Me.p27ID.SelectedValue)
        cRec.j02ID = BO.BAS.IsNullInt(Me.j02ID.Value)
        cRec.p27ID = BO.BAS.IsNullInt(Me.p27ID.SelectedValue)
        If cRec.j02ID = 0 Then
            Master.Notify("Musíte vybrat osobu.", NotifyLevel.ErrorMessage)
            Return
        End If
        Select Case Me.CurrentPrefix
            Case "p28"
                cRec.p28ID = Master.DataPID
            Case "p41"
                cRec.p41ID = Master.DataPID
        End Select
        With Master.Factory.p30Contact_PersonBL
            If .Save(cRec) Then
                Master.CloseAndRefreshParent("p30-save")
            Else
                Master.Notify(Master.Factory.p30Contact_PersonBL.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub RefreshList()
        Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = Nothing
        Select Case Me.CurrentPrefix
            Case "p28"
                lisP30 = Master.Factory.p30Contact_PersonBL.GetList(Master.DataPID, 0, 0)
            Case "p41"
                Dim cP41 As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                If cP41.p28ID_Client = 0 Then
                    lisP30 = Master.Factory.p30Contact_PersonBL.GetList(0, Master.DataPID, 0)
                Else
                    lisP30 = Master.Factory.p30Contact_PersonBL.GetList(cP41.p28ID_Client, 0, 0)
                End If
            Case Else
                Return  'zatím není znám prefix
        End Select
        rpP30.DataSource = lisP30
        rpP30.DataBind()

        Dim qry = From p In lisP30 Select p.j02ID, p.FullNameDesc Distinct
        With Me.j02ID_ContactPerson_DefaultInInvoice
            .DataSource = qry
            .DataBind()
            .Items.Insert(0, "--Dědí se z klienta projektu--")
        End With
        With Me.j02ID_ContactPerson_DefaultInWorksheet
            .DataSource = qry
            .DataBind()
            .Items.Insert(0, "--Dědí se z klienta projektu--")
        End With

        Select Case Me.CurrentPrefix
            Case "p41"
                Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInWorksheet, cRec.j02ID_ContactPerson_DefaultInWorksheet.ToString)
                basUI.SelectDropdownlistValue(Me.j02ID_ContactPerson_DefaultInInvoice, cRec.j02ID_ContactPerson_DefaultInInvoice.ToString)
        End Select
        Dim intJ02ID_Default As Integer = BO.BAS.IsNullInt(Request.Item("default_j02id"))
        If intJ02ID_Default <> 0 Then
            If lisP30.Where(Function(p) p.j02ID = intJ02ID_Default).Count = 0 Then

                Dim cRec As BO.j02Person = Master.Factory.j02PersonBL.Load(intJ02ID_Default)
                Me.j02ID.Value = cRec.PID.ToString
                Me.j02ID.Text = cRec.FullNameDesc
                Handle_Save()   'automaticky novou osobu přiřadit a zavřít okno

                '''Master.Notify(String.Format("Osobu [{0}] nyní můžete přiřadit.", cRec.FullNameAsc))
            End If
        End If
        
    End Sub

    Private Sub rpP30_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP30.ItemCommand
        Dim intP30ID As Integer = BO.BAS.IsNullInt(CType(e.Item.FindControl("p30id"), HiddenField).Value)
        Select Case e.CommandName
            Case "delete"
                If Master.Factory.p30Contact_PersonBL.Delete(intP30ID) Then
                    Master.CloseAndRefreshParent("p30-delete")
                End If
           
        End Select
    End Sub

    Private Sub rpP30_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP30.ItemDataBound
        Dim cRec As BO.p30Contact_Person = CType(e.Item.DataItem, BO.p30Contact_Person)

        CType(e.Item.FindControl("p30id"), HiddenField).Value = cRec.PID.ToString

        With CType(e.Item.FindControl("cmdDelete"), LinkButton)
            Select Case Me.CurrentPrefix
                Case "p28"
                    .Text = "Odstranit vazbu Osoba->Klient"
                    If cRec.p41ID <> 0 Then
                        .Visible = False
                        CType(e.Item.FindControl("imgDel"), Image).Visible = False
                        CType(e.Item.FindControl("Message"), Label).Text = String.Format("Vazba k projektu: {0}", "<b>" & cRec.Project & "</b>")
                    End If
                   
                Case "p41"
                    .Text = "Odstranit vazbu Osoba->Projekt"
                    If cRec.p28ID <> 0 And cRec.p41ID = 0 Then
                        .Visible = False
                        CType(e.Item.FindControl("imgDel"), Image).Visible = False
                        CType(e.Item.FindControl("Message"), Label).Text = "Vazba ke klientovi projektu"
                    End If
                    If cRec.p41ID <> 0 And cRec.p41ID <> Master.DataPID Then
                        .Visible = False
                        CType(e.Item.FindControl("imgDel"), Image).Visible = False
                        CType(e.Item.FindControl("Message"), Label).Text = String.Format("Vazba k projektu: {0}", "<b>" & cRec.Project & "</b>")
                    End If
                    
            End Select
        End With
        With CType(e.Item.FindControl("Person"), Label)
            .Text = cRec.FullNameDesc
            If cRec.IsClosed Then .Font.Strikeout = True
        End With
        CType(e.Item.FindControl("p27Name"), Label).Text = cRec.p27Name
        CType(e.Item.FindControl("cmdJ02"), HyperLink).NavigateUrl = "javascript:j02_edit(" & cRec.j02ID.ToString & ")"
        CType(e.Item.FindControl("clue_j02"), HyperLink).Attributes("rel") = "clue_j02_record.aspx?pid=" & cRec.j02ID.ToString


    End Sub

    Private Sub cmdContinue_Click(sender As Object, e As EventArgs) Handles cmdContinue.Click
        Dim s As String = ""
        If Me.p41id.Value <> "" Then
            s += "masterprefix=p41&masterpid=" & Me.p41id.Value
        Else
            If Me.p28id.Value <> "" Then
                s += "masterprefix=p28&masterpid=" & Me.p28id.Value
            End If
        End If
        If s = "" Then
            Master.Notify("Musíte vybrat projekt nebo klienta.", NotifyLevel.WarningMessage)
            Return
        Else
            Server.Transfer("p30_binding.aspx?" & s)
        End If
    End Sub

   
    Private Sub j02ID_ContactPerson_DefaultInWorksheet_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j02ID_ContactPerson_DefaultInWorksheet.SelectedIndexChanged
        SaveDefaultPersons()
    End Sub
    Private Sub j02ID_ContactPerson_DefaultInInvoice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles j02ID_ContactPerson_DefaultInInvoice.SelectedIndexChanged
        SaveDefaultPersons()
    End Sub
    Private Sub SaveDefaultPersons()
        Select Case Me.CurrentPrefix
            Case "p41"
                Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
                cRec.j02ID_ContactPerson_DefaultInWorksheet = BO.BAS.IsNullInt(Me.j02ID_ContactPerson_DefaultInWorksheet.SelectedValue)
                cRec.j02ID_ContactPerson_DefaultInInvoice = BO.BAS.IsNullInt(Me.j02ID_ContactPerson_DefaultInInvoice.SelectedValue)
                If Master.Factory.p41ProjectBL.Save(cRec, Nothing, Nothing, Nothing, Nothing) Then
                    Master.CloseAndRefreshParent("p30-save")
                End If
        End Select
    End Sub
End Class